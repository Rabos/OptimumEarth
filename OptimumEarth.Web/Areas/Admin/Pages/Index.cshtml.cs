using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;

namespace OptimumEarth.Web.Areas.Admin.Pages;

public sealed record Tile(int Count, string Label, string Href, bool Hot);

public sealed record Coverage(string Label, string Href, int Published, int Total);

public sealed record ActivityRow(DateTime WhenUtc, string Actor, string Action, string Item, string Section);

public class IndexModel : AdminPageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<Tile> Tiles { get; } = new();

    public List<Coverage> Sections { get; } = new();

    public List<ActivityRow> Activity { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var a = Access;

        if (a.CanView(AdminAreas.Inquiries))
        {
            var fresh = await _db.Inquiries.CountAsync(i => i.Status == InquiryStatus.New);
            Tiles.Add(new Tile(fresh, fresh == 1 ? "new inquiry to answer" : "new inquiries to answer", "/admin/inquiries?status=new", fresh > 0));
        }

        var drafts = 0;
        async Task Section<T>(string area, string label, string href, IQueryable<T> set) where T : ContentEntity
        {
            if (!a.CanView(area))
            {
                return;
            }

            var published = await set.CountAsync(x => x.Status == ContentStatus.Published);
            var total = await set.CountAsync();
            drafts += total - published;
            Sections.Add(new Coverage(label, href, published, total));
        }

        await Section(AdminAreas.Home, "Hero slides", "/admin/content/slides", _db.Slides);
        await Section(AdminAreas.Projects, "Projects", "/admin/content/projects", _db.Projects);
        await Section(AdminAreas.Services, "Services", "/admin/content/services", _db.Services);
        await Section(AdminAreas.About, "Capabilities", "/admin/content/capabilities", _db.Capabilities);
        await Section(AdminAreas.About, "Values", "/admin/content/values", _db.CoreValues);
        await Section(AdminAreas.Foundation, "Pillars", "/admin/content/pillars", _db.Pillars);
        await Section(AdminAreas.Foundation, "How we work", "/admin/content/principles", _db.Principles);
        await Section(AdminAreas.Foundation, "Focus areas", "/admin/content/focus", _db.FocusAreas);
        await Section(AdminAreas.Foundation, "SDG goals", "/admin/content/sdg", _db.SdgGoals);
        await Section(AdminAreas.Foundation, "Story tiles", "/admin/content/stories", _db.Stories);
        await Section(AdminAreas.Foundation, "Story gallery", "/admin/content/gallery", _db.GalleryImages);
        await Section(AdminAreas.Blog, "Blog posts", "/admin/content/posts", _db.BlogPosts);

        if (Sections.Count > 0)
        {
            Tiles.Add(new Tile(drafts, drafts == 1 ? "draft item not yet published" : "draft items not yet published", "#coverage", drafts > 0));
        }

        if (a.CanView(AdminAreas.Media))
        {
            var noAlt = await _db.Media.CountAsync(m => m.Alt == string.Empty && (
                _db.Projects.Any(p => p.ImagePath == m.Path) || _db.Services.Any(s => s.ImagePath == m.Path) || _db.Slides.Any(s => s.ImagePath == m.Path) || _db.CountryPages.Any(p => p.HeroImagePath == m.Path)));
            Tiles.Add(new Tile(noAlt, noAlt == 1 ? "image in use without alt text" : "images in use without alt text", "/admin/media?missingAlt=true", noAlt > 0));
        }

        if (a.CanView(AdminAreas.Projects))
        {
            var placed = await _db.CountryPageProjects.Select(p => p.ProjectId).Distinct().CountAsync();
            Tiles.Add(new Tile(placed, "projects placed on country pages", "/admin/content/country-pages", false));
        }

        // Everyone sees only the activity on pages they can reach. Account-level and user events (no area) are for the super admin.
        var visible = AdminAreas.All.Where(x => a.CanView(x.Key)).Select(x => x.Key).ToList();
        Activity = await _db.AuditEntries.AsNoTracking()
            .Where(e => a.IsSuperAdmin || (e.Area != string.Empty && visible.Contains(e.Area)))
            .OrderByDescending(e => e.WhenUtc)
            .Take(8)
            .Select(e => new ActivityRow(e.WhenUtc, e.Actor, e.Action, e.Item, e.Section))
            .ToListAsync();
    }
}
