using Ganss.Xss;
using Markdig;

namespace OptimumEarth.Web.Services;

/// <summary>
/// Turns a post's Markdown into HTML that is safe to put on the page. Raw HTML
/// in the source is switched off, and the result is run through a sanitiser as
/// a second line of defence, so an editor cannot inject script or styles.
/// </summary>
public sealed class MarkdownRenderer
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .DisableHtml()
        .UseAutoLinks()
        .UsePipeTables()
        .UseEmphasisExtras()
        .Build();

    public string ToHtml(string? markdown)
    {
        var html = Markdown.ToHtml(markdown ?? string.Empty, Pipeline);

        // A new sanitiser per call: its option collections are not safe to share across threads.
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedSchemes.Clear();
        sanitizer.AllowedSchemes.Add("http");
        sanitizer.AllowedSchemes.Add("https");
        sanitizer.AllowedSchemes.Add("mailto");
        return sanitizer.Sanitize(html);
    }

    /// <summary>About 200 words a minute, at roughly six characters a word.</summary>
    public static int ReadingMinutes(int characters) => Math.Max(1, (int)Math.Round(characters / 6.0 / 200.0));
}
