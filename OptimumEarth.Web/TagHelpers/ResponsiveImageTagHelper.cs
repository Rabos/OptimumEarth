using System.Text.RegularExpressions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.FileProviders;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.TagHelpers;

[HtmlTargetElement("img", Attributes = "src")]
[HtmlTargetElement("*", Attributes = "style")]
public sealed partial class ResponsiveImageTagHelper(IWebHostEnvironment env, IConfiguration config) : TagHelper
{
    public override int Order => 100;

    [HtmlAttributeName("image-priority")]
    public string? Priority { get; set; }

    [GeneratedRegex("url\\(['\"]?(?<path>/[^'\"()<>\\\\{}]+)['\"]?\\)")]
    private static partial Regex BackgroundUrl();

    private string? Versioned(string path)
    {
        var clean = path.Split('?')[0];
        if (Path.GetExtension(clean).ToLowerInvariant() is not (".jpg" or ".jpeg" or ".png" or ".webp")) return null;
        Microsoft.Extensions.FileProviders.IFileInfo? info;
        if (clean.StartsWith("/uploads/", StringComparison.Ordinal))
        {
            using var provider = new PhysicalFileProvider(MediaService.ResolveUploadsPath(config, env));
            info = provider.GetFileInfo(clean[9..]);
        }
        else if (clean.StartsWith("/img/", StringComparison.Ordinal)) info = env.WebRootFileProvider.GetFileInfo(clean);
        else return null;
        return info.Exists ? QueryHelpers.AddQueryString(clean, "v", info.LastModified.UtcTicks.ToString()) : null;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.RemoveAll("image-priority");
        if (output.TagName == "img")
        {
            var src = output.Attributes["src"]?.Value?.ToString();
            var path = src is null ? null : Versioned(src);
            if (path is null) return;
            var variants = ImageOptimizer.Widths.Select(w => $"{QueryHelpers.AddQueryString(path, "w", w.ToString())} {w}w");
            output.Attributes.SetAttribute("src", QueryHelpers.AddQueryString(path, "w", "1920"));
            output.Attributes.SetAttribute("srcset", string.Join(", ", variants));
            if (!output.Attributes.ContainsName("loading")) output.Attributes.SetAttribute("loading", "lazy");
            output.Attributes.SetAttribute("decoding", "async");
            if (Priority == "high")
            {
                output.Attributes.SetAttribute("loading", "eager");
                output.Attributes.SetAttribute("fetchpriority", "high");
            }
            if (!output.Attributes.ContainsName("sizes"))
                output.Attributes.SetAttribute("sizes", output.Attributes["loading"]?.Value?.ToString() == "lazy" ? "auto, 100vw" : "100vw");
            return;
        }
        var style = output.Attributes["style"]?.Value?.ToString() ?? "";
        var match = BackgroundUrl().Match(style);
        if (!match.Success) return;
        var versioned = Versioned(match.Groups["path"].Value);
        if (versioned is null) return;
        var template = style.Replace(match.Value, $"url('{versioned}&w=IMAGE_WIDTH')");
        if (Priority == "high")
        {
            // Start the first visible background without waiting for JavaScript.
            var responsive = $"image-set(url('{versioned}&w=960') 1x, url('{versioned}&w=1920') 2x)";
            output.Attributes.SetAttribute("style", style.Replace(match.Value, responsive));
            var preload = new Microsoft.AspNetCore.Mvc.Rendering.TagBuilder("link");
            preload.Attributes["rel"] = "preload";
            preload.Attributes["as"] = "image";
            preload.Attributes["imagesrcset"] = $"{versioned}&w=960 1x, {versioned}&w=1920 2x";
            preload.Attributes["fetchpriority"] = "high";
            preload.TagRenderMode = Microsoft.AspNetCore.Mvc.Rendering.TagRenderMode.SelfClosing;
            output.PreElement.AppendHtml(preload);
            return;
        }
        output.Attributes.SetAttribute("data-image-style", template);
        if (Priority == "deferred") output.Attributes.SetAttribute("data-image-deferred", "true");
        output.Attributes.SetAttribute("style", style.Replace(match.Value, "none"));
        // Preserve images when scripting is disabled, including gradient overlays.
        var id = output.Attributes["id"]?.Value?.ToString();
        if (string.IsNullOrEmpty(id))
        {
            id = "image-" + Guid.NewGuid().ToString("N");
            output.Attributes.SetAttribute("id", id);
        }
        if (Regex.IsMatch(id, "^[a-zA-Z0-9_-]+$") && !template.Contains('<') && !template.Contains('>'))
            output.PostElement.AppendHtml($"<noscript><style>#{id}{{{template.Replace("IMAGE_WIDTH", "1920")}}}</style></noscript>");
    }
}
