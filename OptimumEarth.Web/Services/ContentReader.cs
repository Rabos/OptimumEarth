using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Services;

public sealed record WhoWeAreContent(
    WhoPageText Text,
    string Mission,
    string Vision,
    IReadOnlyList<CapabilityItem> Capabilities,
    IReadOnlyList<ValueItem> Values,
    IReadOnlyList<StatItem> Stats,
    IReadOnlyList<MilestoneItem> Milestones,
    IReadOnlyList<GroupCard> Group,
    IReadOnlyList<OfficeInfo> Offices);

public sealed record ServicesPageContent(
    ServicesPageText Text,
    IReadOnlyList<ServiceSection> Sections,
    IReadOnlyList<AerialShot> Aerial,
    IReadOnlyList<StatItem> Stats,
    DateOnly FoundedOn,
    IReadOnlyList<SectorTile> Sectors,
    IReadOnlyList<ClientLogo> Clients);

public sealed record ProjectsPageContent(
    ProjectsPageText Text,
    IReadOnlyList<ProjectItem> Projects,
    IReadOnlyList<SectorOption> Sectors,
    IReadOnlyList<HeroPhoto> HeroPhotos,
    IReadOnlyList<ClientLogo> Clients);

public sealed record FoundationContent(
    IReadOnlyList<PillarDetail> Pillars,
    IReadOnlyList<string> FocusAreas,
    IReadOnlyList<WorkPrinciple> WorkPrinciples,
    IReadOnlyList<StoryTile> StoryTiles,
    IReadOnlyList<StoryGallery> Galleries,
    IReadOnlyList<(string Number, string Label, string PdfUrl)> SdgGoals);

public sealed record CountryPageContent(
    string Theme,
    string HeroImagePath,
    string HeroImageLabel,
    string HeroEyebrow,
    string HeroHeadline,
    string HeroSub,
    string OverviewEyebrow,
    string OverviewQuote,
    string OverviewBody,
    IReadOnlyList<(string Key, string Value)> Facts,
    string SectionOneTitle,
    string? SectionOneLinkText,
    string? SectionOneLinkHref,
    IReadOnlyList<SimpleCard> ServiceCards,
    string SectionTwoTitle,
    string? SectionTwoLinkText,
    string? SectionTwoLinkHref,
    IReadOnlyList<ProjectCard> ProjectCards,
    IReadOnlyList<ProjectCardDescription> ProjectDescriptions,
    string? SectionTwoNote,
    string CtaTitle,
    string CtaBody,
    string CtaServiceLabel,
    IReadOnlyList<string> CtaServices);

public sealed record BlogCard(string Slug, string Title, string Excerpt, string CoverPath, string CategoryName, string CategorySlug, string Author, DateOnly Date, int ReadingMinutes, bool Pinned);

public sealed record BlogListPage(IReadOnlyList<BlogCard> Posts, int Total, int Page, int Pages, int Size);

public sealed record BlogCategoryInfo(string Name, string Slug, string Description, int Count);

public sealed record BlogPostView(
    string Slug, string Title, string Excerpt, string CoverPath, string CategoryName, string CategorySlug, string Author,
    DateOnly Date, int ReadingMinutes, string Html, IReadOnlyList<string> Tags, string SeoTitle, string SeoDescription, DateTime UpdatedUtc);

/// <summary>
/// Reads the content the public pages show, as the same small records the
/// views already use. Only Published rows are returned. Results are cached
/// until something is saved in the dashboard.
/// </summary>
public sealed class ContentReader
{
    private readonly AppDbContext _db;
    private readonly ContentCache _cache;

    private readonly MarkdownRenderer _markdown;

    public ContentReader(AppDbContext db, ContentCache cache, MarkdownRenderer markdown)
    {
        _db = db;
        _cache = cache;
        _markdown = markdown;
    }

    private static string Two(int i) => (i + 1).ToString("00");

    public Task<IReadOnlyList<HeroSlide>> HeroSlidesAsync() => _cache.GetOrCreateAsync("slides", async () =>
    {
        var rows = await _db.Slides.AsNoTracking().Where(s => s.Status == ContentStatus.Published).OrderBy(s => s.SortOrder).ThenBy(s => s.Id).ToListAsync();
        return (IReadOnlyList<HeroSlide>)rows.Select(s => new HeroSlide(s.Eyebrow.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() + " hero", s.Eyebrow, s.Headline, s.Body, s.ImagePath)).ToList();
    });

    public Task<ServicesPageContent> ServicesPageAsync() => _cache.GetOrCreateAsync("services.page", async () =>
    {
        var settings = await SettingsAsync();
        var rows = await _db.Services.AsNoTracking().Include(s => s.PracticeProject)
            .Where(s => s.Status == ContentStatus.Published && s.OnListPage).OrderBy(s => s.SortOrder).ThenBy(s => s.Id).ToListAsync();
        var aerial = await _db.AerialPhotos.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        var sectors = await _db.Sectors.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        var clients = settings.ShowClientLogos
            ? await _db.Clients.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync()
            : new List<Client>();
        var stats = await _db.Stats.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();

        // Image alt text is edited under Media; fall back to a description of the slot.
        var paths = rows.Select(r => r.ImagePath).Concat(aerial.Select(a => a.ImagePath)).Append(settings.FleetImagePath).Append(settings.FleetInsetImagePath).ToList();
        var alts = await _db.Media.AsNoTracking().Where(m => paths.Contains(m.Path)).ToDictionaryAsync(m => m.Path, m => m.Alt);
        string Alt(string path, string fallback) => alts.GetValueOrDefault(path) is { Length: > 0 } alt ? alt : fallback;

        // A published project only: a draft one stays out of the "In practice" slot.
        PracticeCard? Practice(Project? p) => p is null || p.Status != ContentStatus.Published ? null : new PracticeCard(
            p.Title,
            p.Description,
            string.Join(" · ", new[] { p.Client.Length > 0 ? p.Client : ClientName(p.Meta), ProjectCountries.NameOf(p.Country) }.Where(x => x.Length > 0)),
            p.ImagePath,
            "/projects?service=" + p.Service + "#" + ProjectSlugs.From(p.Title));

        var text = new ServicesPageText(
            settings.ServicesHeadline,
            settings.ServicesLede,
            settings.ServicesPosterPath,
            settings.AerialHeadline,
            settings.AerialBody,
            settings.FleetHeadline,
            settings.FleetBody,
            settings.FleetItems.ToList(),
            settings.FleetImagePath,
            Alt(settings.FleetImagePath, "Equipment from the Optimum Earth Engineering fleet"),
            settings.FleetInsetImagePath,
            Alt(settings.FleetInsetImagePath, "Equipment from the Optimum Earth Engineering fleet"),
            settings.CaseStudyStat,
            settings.CaseStudyStatText,
            settings.CaseStudyCaption,
            settings.ServicesCtaImagePath);

        return new ServicesPageContent(
            text,
            rows.Select((s, i) => new ServiceSection(Two(i), s.Slug, s.Title, s.ShortTitle.Length > 0 ? s.ShortTitle : s.Title, s.Tagline, s.Description, s.Layout,
                s.ImagePath, Alt(s.ImagePath, s.Title), s.TileImagePath.Length > 0 ? s.TileImagePath : s.ImagePath, s.TileItems.ToList(), s.Highlights.ToList(),
                s.Deliverables.ToList(), s.StatValue, s.StatLabel, Practice(s.PracticeProject))).ToList(),
            aerial.Select(a => new AerialShot(a.Caption, a.ImagePath, Alt(a.ImagePath, a.Caption))).ToList(),
            stats.Select(x => new StatItem(x.Kind == "years", x.Value, x.Suffix, x.Label)).ToList(),
            settings.FoundedOn,
            sectors.Select(x => new SectorTile(x.Key, x.Name, x.Blurb, x.ImagePath)).ToList(),
            clients.Select(x => new ClientLogo(x.Name, x.LogoPath)).ToList());
    });

    /// <summary>The project meta line reads "Client: Name"; the cards only want the name.</summary>
    private static string ClientName(string meta) =>
        Regex.Replace(meta, @"^\s*Client\s*:\s*", string.Empty, RegexOptions.IgnoreCase).Trim();

    public Task<IReadOnlyList<ServiceLink>> ServiceLinksAsync() => _cache.GetOrCreateAsync("services.links", async () =>
    {
        var rows = await _db.Services.AsNoTracking().Where(s => s.Status == ContentStatus.Published && s.OnListPage && s.Slug != string.Empty)
            .OrderBy(s => s.SortOrder).ThenBy(s => s.Id).Select(s => new { s.ShortTitle, s.Title, s.Slug }).ToListAsync();
        return (IReadOnlyList<ServiceLink>)rows.Select(s => new ServiceLink(s.ShortTitle.Length > 0 ? s.ShortTitle : s.Title, s.Slug)).ToList();
    });

    public Task<ProjectsPageContent> ProjectsPageAsync() => _cache.GetOrCreateAsync("projects.page", async () =>
    {
        var settings = await SettingsAsync();

        // Ongoing work first, then the order set in the dashboard.
        var rows = await _db.Projects.AsNoTracking().Where(p => p.Status == ContentStatus.Published && p.OnListPage)
            .OrderByDescending(p => p.Ongoing).ThenBy(p => p.SortOrder).ThenBy(p => p.Id).ToListAsync();
        var sectors = await _db.Sectors.AsNoTracking().OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        var photos = await _db.FieldPhotos.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        var clients = settings.ShowClientLogos
            ? await _db.Clients.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync()
            : new List<Client>();

        var slugs = new HashSet<string>();
        string UniqueSlug(string title)
        {
            var slug = ProjectSlugs.From(title);
            var unique = slug;
            for (var n = 2; !slugs.Add(unique); n++) unique = slug + "-" + n;
            return unique;
        }

        var projects = rows.Select(p => new ProjectItem(
            UniqueSlug(p.Title),
            p.Title,
            p.Client.Length > 0 ? p.Client : ClientName(p.Meta),
            p.Country,
            p.Location,
            p.Sector,
            p.Ongoing,
            // The main service always counts, ticked or not.
            p.Services.Prepend(p.Service).Where(s => ProjectServices.Find(s) is not null).Distinct().ToList(),
            p.ImagePath,
            p.RepresentativePhoto,
            p.Description,
            p.FlagshipFigure,
            p.FlagshipLabel,
            p.FlagshipOrder)).ToList();

        // Only sectors some project uses are offered as filters; a draft sector still counts.
        var used = projects.Select(p => p.Sector).ToHashSet();

        return new ProjectsPageContent(
            new ProjectsPageText(settings.ProjectsHeadline, settings.ProjectsLede, settings.ProjectsStatFigure, settings.ProjectsStatLabel, settings.ProjectsCtaImagePath),
            projects,
            sectors.Where(s => used.Contains(s.Key)).Select(s => new SectorOption(s.Key, s.Name)).ToList(),
            photos.Select(f => new HeroPhoto(f.Kicker, f.Caption, f.ImagePath)).ToList(),
            clients.Select(c => new ClientLogo(c.Name, c.LogoPath)).ToList());
    });

    public Task<WhoWeAreContent> WhoWeAreAsync() => _cache.GetOrCreateAsync("who", async () =>
    {
        var settings = await SettingsAsync();
        var caps = await _db.Capabilities.AsNoTracking().Where(c => c.Status == ContentStatus.Published).OrderBy(c => c.SortOrder).ThenBy(c => c.Id).ToListAsync();
        var values = await _db.CoreValues.AsNoTracking().Where(c => c.Status == ContentStatus.Published).OrderBy(c => c.SortOrder).ThenBy(c => c.Id).ToListAsync();
        var stats = await _db.Stats.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        var milestones = await _db.Milestones.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        var group = await _db.GroupCompanies.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();

        // Image alt text is edited under Media; fall back to a description of the slot.
        var paths = group.Select(g => g.ImagePath).Append(settings.WhoHeroImagePath).ToList();
        var alts = await _db.Media.AsNoTracking().Where(m => paths.Contains(m.Path)).ToDictionaryAsync(m => m.Path, m => m.Alt);
        string Alt(string path, string fallback) => alts.GetValueOrDefault(path) is { Length: > 0 } alt ? alt : fallback;

        var text = new WhoPageText(
            settings.WhoHeadline,
            settings.WhoHighlight,
            settings.WhoLede,
            settings.WhoHeroImagePath,
            Alt(settings.WhoHeroImagePath, "Optimum Earth team on site"),
            settings.StoryHeadline,
            Regex.Split(settings.StoryBody, @"\n\s*\n").Select(p => p.Trim()).Where(p => p.Length > 0).ToList(),
            settings.StoryImagePath,
            settings.StoryInsetImagePath,
            settings.FoundedOn,
            settings.WorkCountries.ToList(),
            settings.WhoCtaHeadline,
            settings.WhoCtaBody,
            settings.WhoCtaImagePath);

        return new WhoWeAreContent(
            text,
            settings.Mission,
            settings.Vision,
            caps.Select((c, i) => new CapabilityItem(Two(i), c.Title, c.Description, c.LinkUrl.Length > 0 ? c.LinkUrl : "/services")).ToList(),
            values.Select(v => new ValueItem(v.Title, v.Description)).ToList(),
            stats.Select(x => new StatItem(x.Kind == "years", x.Value, x.Suffix, x.Label)).ToList(),
            milestones.Select(x => new MilestoneItem(x.Year, x.Title, x.Description)).ToList(),
            group.Select(x => new GroupCard(x.Kind, x.Name, x.Description, x.Tags.ToList(), x.LinkText, x.LinkUrl, x.ImagePath, Alt(x.ImagePath, x.Name), x.IsFoundation)).ToList(),
            await OfficesAsync());
    });

    public Task<IReadOnlyList<OfficeInfo>> OfficesAsync() => _cache.GetOrCreateAsync("offices", async () =>
    {
        var rows = await _db.Offices.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        return (IReadOnlyList<OfficeInfo>)rows.Select(x => new OfficeInfo(x.Name, x.Country, x.AddressLines.ToList(), x.Phone, x.Email)).ToList();
    });

    private static readonly Dictionary<string, string> StoryPhotoLabels = new()
    {
        ["communities"] = "Community story photo",
        ["partnerships"] = "Partnership story photo",
        ["learning"] = "Learning story photo",
    };

    public Task<FoundationContent> FoundationAsync() => _cache.GetOrCreateAsync("foundation", async () =>
    {
        var pillars = await _db.Pillars.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        var focus = await _db.FocusAreas.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        var principles = await _db.Principles.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        var stories = await _db.Stories.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        var gallery = await _db.GalleryImages.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        var goals = await _db.SdgGoals.AsNoTracking().Where(x => x.Status == ContentStatus.Published).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();

        return new FoundationContent(
            pillars.Select(p => new PillarDetail(p.Key, p.SdgLabel, p.Title, p.Description, p.Items.ToList())).ToList(),
            focus.Select(f => f.Label).ToList(),
            principles.Select(p => new WorkPrinciple(p.Title, p.Description, p.Slug)).ToList(),
            stories.Select(s => new StoryTile(s.Key, s.Title, s.Summary, s.Body, StoryPhotoLabels.GetValueOrDefault(s.Key, s.Title + " story photo"), s.CoverPath)).ToList(),
            gallery.GroupBy(g => g.StoryKey)
                .Select(g => new StoryGallery(g.Key, g.Select(i => new StoryGalleryItem(i.Caption, i.ImagePath)).ToList()))
                .ToList(),
            goals.Select(g => (g.Number, g.Label, g.DocumentPath)).ToList());
    });

    public Task<CountryPageContent?> CountryPageAsync(string slug) => _cache.GetOrCreateAsync("country." + slug, async () =>
    {
        var page = await _db.CountryPages.AsNoTracking()
            .Include(p => p.Services.OrderBy(s => s.SortOrder)).ThenInclude(s => s.Service)
            .Include(p => p.Projects.OrderBy(s => s.SortOrder)).ThenInclude(s => s.Project)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Slug == slug);
        if (page is null)
        {
            return new Optional<CountryPageContent>(null);
        }

        var serviceCards = page.Services
            .Where(s => s.Service is null || s.Service.Status == ContentStatus.Published)
            .Select(s => new SimpleCard(
                string.IsNullOrWhiteSpace(s.Title) ? s.Service?.Title ?? string.Empty : s.Title,
                string.IsNullOrWhiteSpace(s.Text) ? s.Service?.Description ?? string.Empty : s.Text,
                null,
                !string.IsNullOrWhiteSpace(s.Url),
                string.IsNullOrWhiteSpace(s.Url) ? null : s.Url))
            .ToList();

        var projectCards = page.Projects
            .Where(p => p.Project is not null && p.Project.Status == ContentStatus.Published)
            .Select(p => new ProjectCard(
                "Project photo",
                string.IsNullOrWhiteSpace(p.Tag) ? p.Project!.Tag : p.Tag,
                p.Project!.Title,
                string.IsNullOrWhiteSpace(p.Meta) ? p.Project.Meta : p.Meta,
                string.Empty,
                string.Empty,
                p.Project.ImagePath))
            .ToList();

        // The card text shown beneath each project tile comes from the project's own description.
        var projectDescriptions = page.Projects
            .Where(p => p.Project is not null && p.Project.Status == ContentStatus.Published)
            .Select(p => new ProjectCardDescription(
                new ProjectCard(string.Empty, string.Empty, p.Project!.Title, string.Empty).Slug,
                p.Project.Description,
                p.Project.ImagePath,
                string.IsNullOrWhiteSpace(p.Meta) ? p.Project.Meta : p.Meta,
                p.Project.Title,
                "Project photo"))
            .ToList();

        var facts = page.Facts.Select(f =>
        {
            var parts = f.Split('|', 2);
            return (parts[0].Trim(), parts.Length > 1 ? parts[1].Trim() : string.Empty);
        }).ToList();

        return new Optional<CountryPageContent>(new CountryPageContent(
            page.Theme, page.HeroImagePath, page.HeroImageLabel, page.HeroEyebrow, page.HeroHeadline, page.HeroSub,
            page.OverviewEyebrow, page.OverviewQuote, page.OverviewBody, facts,
            page.SectionOneTitle, NullIfEmpty(page.SectionOneLinkText), NullIfEmpty(page.SectionOneLinkHref), serviceCards,
            page.SectionTwoTitle, NullIfEmpty(page.SectionTwoLinkText), NullIfEmpty(page.SectionTwoLinkHref), projectCards, projectDescriptions,
            NullIfEmpty(page.SectionTwoNote),
            page.CtaTitle, page.CtaBody, page.CtaServiceLabel, page.CtaServices.ToList()));
    }).ContinueWith(t => t.Result.Value);

    public Task<SiteSettings> SettingsAsync() => _cache.GetOrCreateAsync("settings", async () =>
        await _db.Settings.AsNoTracking().FirstOrDefaultAsync() ?? new SiteSettings());

    // ---------- Blog ----------
    // A post is live when it is Published and its publish date has arrived. The date is part of every
    // cache key, so a scheduled post appears on its day without anyone saving anything.

    private static DateOnly Today() => DateOnly.FromDateTime(DateTime.UtcNow);

    public async Task<bool> HasBlogAsync()
    {
        var today = Today();
        var flag = await _cache.GetOrCreateAsync("blog.has." + today, async () => new Flag(await _db.BlogPosts.AnyAsync(p => p.Status == ContentStatus.Published && p.PublishDate <= today)));
        return flag.Value;
    }

    public Task<BlogListPage> BlogPageAsync(string? categorySlug, int page)
    {
        var today = Today();
        return _cache.GetOrCreateAsync($"blog.list.{today}.{categorySlug}.{page}", async () =>
        {
            var size = Math.Clamp((await SettingsAsync()).BlogPageSize, 3, 50);
            var query = _db.BlogPosts.AsNoTracking().Where(p => p.Status == ContentStatus.Published && p.PublishDate <= today);
            if (!string.IsNullOrEmpty(categorySlug))
            {
                query = query.Where(p => p.Category!.Slug == categorySlug);
            }

            var total = await query.CountAsync();
            var pages = Math.Max(1, (int)Math.Ceiling(total / (double)size));
            var current = Math.Min(Math.Max(1, page), pages);
            var rows = await query
                .OrderByDescending(p => p.Pinned).ThenByDescending(p => p.PublishDate).ThenByDescending(p => p.Id)
                .Skip((current - 1) * size).Take(size)
                .Select(p => new { p.Slug, p.Title, p.Excerpt, p.CoverPath, CategoryName = p.Category!.Name, CategorySlug = p.Category.Slug, p.AuthorName, p.PublishDate, Length = p.Body.Length, p.Pinned })
                .ToListAsync();

            return new BlogListPage(
                rows.Select(r => new BlogCard(r.Slug, r.Title, r.Excerpt, r.CoverPath, r.CategoryName, r.CategorySlug, r.AuthorName, r.PublishDate, MarkdownRenderer.ReadingMinutes(r.Length), r.Pinned)).ToList(),
                total, current, pages, size);
        });
    }

    public Task<IReadOnlyList<BlogCategoryInfo>> BlogCategoriesAsync()
    {
        var today = Today();
        return _cache.GetOrCreateAsync("blog.categories." + today, async () =>
        {
            var rows = await _db.BlogCategories.AsNoTracking()
                .Select(c => new BlogCategoryInfo(c.Name, c.Slug, c.Description, c.Posts.Count(p => p.Status == ContentStatus.Published && p.PublishDate <= today)))
                .ToListAsync();
            return (IReadOnlyList<BlogCategoryInfo>)rows.Where(c => c.Count > 0).OrderBy(c => c.Name).ToList();
        });
    }

    public Task<BlogCategoryInfo?> BlogCategoryAsync(string slug) => BlogCategoriesAsync().ContinueWith(t => t.Result.FirstOrDefault(c => c.Slug == slug));

    public async Task<BlogPostView?> BlogPostAsync(string slug)
    {
        var today = Today();
        var box = await _cache.GetOrCreateAsync($"blog.post.{today}.{slug}", async () =>
        {
            var post = await _db.BlogPosts.AsNoTracking().Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == ContentStatus.Published && p.PublishDate <= today);
            return new Optional<BlogPostView>(post is null ? null : new BlogPostView(
                post.Slug, post.Title, post.Excerpt, post.CoverPath, post.Category!.Name, post.Category.Slug, post.AuthorName, post.PublishDate,
                MarkdownRenderer.ReadingMinutes(post.Body.Length), _markdown.ToHtml(post.Body), post.Tags.ToList(),
                string.IsNullOrWhiteSpace(post.SeoTitle) ? post.Title : post.SeoTitle,
                string.IsNullOrWhiteSpace(post.SeoDescription) ? post.Excerpt : post.SeoDescription,
                post.UpdatedUtc));
        });
        return box.Value;
    }

    public Task<IReadOnlyList<(string Slug, DateTime UpdatedUtc, DateOnly Date)>> BlogSitemapAsync()
    {
        var today = Today();
        return _cache.GetOrCreateAsync("blog.sitemap." + today, async () =>
        {
            var rows = await _db.BlogPosts.AsNoTracking().Where(p => p.Status == ContentStatus.Published && p.PublishDate <= today)
                .OrderByDescending(p => p.PublishDate).Select(p => new { p.Slug, p.UpdatedUtc, p.PublishDate }).ToListAsync();
            return (IReadOnlyList<(string, DateTime, DateOnly)>)rows.Select(r => (r.Slug, r.UpdatedUtc, r.PublishDate)).ToList();
        });
    }

    public Task<IReadOnlyList<BlogCard>> BlogLatestAsync(int count) => BlogPageAsync(null, 1).ContinueWith(t => (IReadOnlyList<BlogCard>)t.Result.Posts.Take(count).ToList());

    private sealed record Flag(bool Value);

    private static string? NullIfEmpty(string value) => string.IsNullOrWhiteSpace(value) ? null : value;

    /// <summary>Lets a "not found" result be cached (the cache only stores reference types).</summary>
    private sealed record Optional<T>(T? Value) where T : class;
}
