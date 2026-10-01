using System.Globalization;
using System.Xml.Linq;

namespace OptimumEarth.Web.Services;

/// <summary>The sitemap and the blog's RSS feed, built from the database so new posts appear without editing a file.</summary>
public static class SeoEndpoints
{
    private static readonly XNamespace SitemapNs = "http://www.sitemaps.org/schemas/sitemap/0.9";

    private static readonly (string Path, string Frequency, string Priority)[] StaticPages =
    {
        ("/", "weekly", "1.0"),
        ("/uganda", "monthly", "0.9"),
        ("/zambia", "monthly", "0.9"),
        ("/foundation", "monthly", "0.8"),
        ("/services", "monthly", "0.8"),
        ("/projects", "weekly", "0.7"),
        ("/contact", "yearly", "0.6"),
    };

    public static void Map(WebApplication app)
    {
        app.MapGet("/sitemap.xml", async (ContentReader content, IConfiguration config) =>
        {
            var site = (config["SiteUrl"] ?? "https://optimum-earth.com").TrimEnd('/');
            var urlset = new XElement(SitemapNs + "urlset");
            foreach (var (path, frequency, priority) in StaticPages)
            {
                urlset.Add(Url(site + path, null, frequency, priority));
            }

            if (await content.HasBlogAsync())
            {
                urlset.Add(Url(site + "/blog", null, "weekly", "0.6"));
                foreach (var category in await content.BlogCategoriesAsync())
                {
                    urlset.Add(Url($"{site}/blog/category/{category.Slug}", null, "weekly", "0.4"));
                }

                foreach (var (slug, updated, _) in await content.BlogSitemapAsync())
                {
                    urlset.Add(Url($"{site}/blog/{slug}", updated, "monthly", "0.5"));
                }
            }

            return Results.Text(new XDocument(new XDeclaration("1.0", "UTF-8", null), urlset).ToString(), "application/xml");
        });

        app.MapGet("/blog/rss.xml", async (ContentReader content, IConfiguration config) =>
        {
            var site = (config["SiteUrl"] ?? "https://optimum-earth.com").TrimEnd('/');
            var settings = await content.SettingsAsync();
            var channel = new XElement("channel",
                new XElement("title", "Optimum Earth Blog"),
                new XElement("link", site + "/blog"),
                new XElement("description", string.IsNullOrWhiteSpace(settings.BlogIntro) ? "Field notes, insights and news from Optimum Earth." : settings.BlogIntro),
                new XElement("language", "en"));

            foreach (var post in await content.BlogLatestAsync(20))
            {
                var link = $"{site}/blog/{post.Slug}";
                channel.Add(new XElement("item",
                    new XElement("title", post.Title),
                    new XElement("link", link),
                    new XElement("guid", new XAttribute("isPermaLink", "true"), link),
                    new XElement("pubDate", post.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).ToString("R", CultureInfo.InvariantCulture)),
                    new XElement("category", post.CategoryName),
                    new XElement("description", post.Excerpt)));
            }

            var rss = new XElement("rss", new XAttribute("version", "2.0"), channel);
            return Results.Text(new XDocument(new XDeclaration("1.0", "UTF-8", null), rss).ToString(), "application/rss+xml");
        });
    }

    private static XElement Url(string loc, DateTime? lastmod, string frequency, string priority)
    {
        var url = new XElement(SitemapNs + "url", new XElement(SitemapNs + "loc", loc));
        if (lastmod is not null)
        {
            url.Add(new XElement(SitemapNs + "lastmod", lastmod.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        url.Add(new XElement(SitemapNs + "changefreq", frequency), new XElement(SitemapNs + "priority", priority));
        return url;
    }
}
