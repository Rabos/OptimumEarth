using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using OptimumEarth.Web.Services;
using OptimumEarth.Web.TagHelpers;
using ImageOptimizer = OptimumEarth.Web.Services.ImageOptimizer;

static void Check(bool passed, string message)
{
    if (!passed) throw new Exception(message);
    Console.WriteLine("PASS: " + message);
}

using var original = new MagickImage(MagickColors.ForestGreen, 4000, 3000);
var jpeg = original.ToByteArray(MagickFormat.Jpeg);
using var large = new MagickImage(ImageOptimizer.Optimize(jpeg));
Check(large.Width == 1920 && large.Height == 1440 && large.Format == MagickFormat.WebP, "Large upload becomes a 1920px WebP with preserved aspect ratio");
using var small = new MagickImage(ImageOptimizer.Optimize(jpeg, 480));
Check(small.Width == 480 && small.Height == 360, "Mobile variant is 480px");
using var tiny = new MagickImage(MagickColors.Transparent, 120, 80);
using var transparent = new MagickImage(ImageOptimizer.Optimize(tiny.ToByteArray(MagickFormat.Png)));
Check(transparent.Width == 120 && transparent.Height == 80 && transparent.HasAlpha, "Small transparent images retain alpha and are never upscaled");
using var animated = new MagickImageCollection();
animated.Add(new MagickImage(MagickColors.Red, 20, 20));
animated.Add(new MagickImage(MagickColors.Blue, 20, 20));
try { ImageOptimizer.Optimize(animated.ToByteArray(MagickFormat.Gif)); throw new Exception("Animated image was accepted"); }
catch (InvalidDataException) { Console.WriteLine("PASS: Animated images are rejected before full decoding"); }
try { ImageOptimizer.Optimize([1, 2, 3]); throw new Exception("Invalid image was accepted"); }
catch (MagickException) { Console.WriteLine("PASS: Invalid image bytes are rejected"); }

var root = Path.Combine(Path.GetTempPath(), "optimum-image-checks-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(Path.Combine(root, "img"));
Directory.CreateDirectory(Path.Combine(root, "uploads"));
try
{
    var source = Path.Combine(root, "img", "photo.jpg");
    await File.WriteAllBytesAsync(source, jpeg);
    using var provider = new PhysicalFileProvider(root);
    var env = new TestEnvironment { WebRootPath = root, ContentRootPath = root, WebRootFileProvider = provider };
    var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Storage:UploadsPath"] = Path.Combine(root, "uploads") }).Build();
    var attributes = new TagHelperAttributeList { { "src", "/img/photo.jpg" } };
    var tagContext = new TagHelperContext(attributes, new Dictionary<object, object>(), "image-check");
    var output = new TagHelperOutput("img", attributes, (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    new ResponsiveImageTagHelper(env, config) { Priority = "high" }.Process(tagContext, output);
    Check(output.Attributes["srcset"].Value.ToString()!.Contains("480w") && output.Attributes["loading"].Value.ToString() == "eager" && output.Attributes["fetchpriority"].Value.ToString() == "high", "Hero markup includes responsive sources and eager high priority");
    var backgroundAttributes = new TagHelperAttributeList { { "style", "background-image: url('/img/photo.jpg')" } };
    var background = new TagHelperOutput("div", backgroundAttributes, (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    new ResponsiveImageTagHelper(env, config).Process(new TagHelperContext(backgroundAttributes, new Dictionary<object, object>(), "background-check"), background);
    Check(background.Attributes.ContainsName("data-image-style") && background.Attributes["style"].Value.ToString() == "background-image: none" && background.PostElement.GetContent().Contains("noscript"), "Background images defer loading and include a no-script fallback");
    using var services = new ServiceCollection().AddLogging().AddRouting().BuildServiceProvider();
    var fallback = false;
    var middleware = new ImageVariantMiddleware(_ => { fallback = true; return Task.CompletedTask; }, env, config, services.GetRequiredService<ILogger<ImageVariantMiddleware>>());
    DefaultHttpContext Request(string query)
    {
        var request = new DefaultHttpContext { RequestServices = services };
        request.Request.Method = "GET";
        request.Request.Path = "/img/photo.jpg";
        request.Request.QueryString = new QueryString(query);
        request.Response.Body = new MemoryStream();
        return request;
    }
    var first = Request("?w=480");
    await middleware.InvokeAsync(first);
    using var served = new MagickImage(((MemoryStream)first.Response.Body).ToArray());
    Check(!fallback && served.Width == 480 && first.Response.ContentType == "image/webp", "HTTP variant response contains a real mobile WebP");
    Check(first.Response.Headers.CacheControl.ToString().Contains("max-age=86400"), "Image response has browser caching");
    var second = Request("?w=480");
    second.Request.Headers.IfNoneMatch = first.Response.Headers.ETag;
    await middleware.InvokeAsync(second);
    Check(second.Response.StatusCode == 304, "Cached image supports conditional 304 responses");
    File.SetLastWriteTimeUtc(source, DateTime.UtcNow.AddMinutes(1));
    var updated = Request("?w=480");
    await middleware.InvokeAsync(updated);
    Check(updated.Response.Headers.ETag != first.Response.Headers.ETag, "Replacing the source invalidates the variant cache");
    await middleware.InvokeAsync(Request("?w=777"));
    Check(fallback, "Unapproved dimensions bypass image processing");
}
finally { Directory.Delete(root, recursive: true); }

Console.WriteLine("All image checks passed.");

sealed class TestEnvironment : IWebHostEnvironment
{
    public string WebRootPath { get; set; } = "";
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    public string ApplicationName { get; set; } = "ImageChecks";
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    public string ContentRootPath { get; set; } = "";
    public string EnvironmentName { get; set; } = "Development";
}
