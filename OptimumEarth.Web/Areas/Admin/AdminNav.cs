using OptimumEarth.Web.Data;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Areas.Admin;

public sealed record NavItem(string Label, string Href, string? Area = null, bool SuperAdminOnly = false);

public sealed record NavGroup(string Title, IReadOnlyList<NavItem> Items);

/// <summary>The dashboard's sidebar. Each person sees only the items they have access to.</summary>
public static class AdminNav
{
    public static readonly IReadOnlyList<NavGroup> Groups = new List<NavGroup>
    {
        new("Home", new NavItem[] { new("Hero slides", "/admin/content/slides", AdminAreas.Home) }),
        new("Work", new NavItem[]
        {
            new("Projects", "/admin/content/projects", AdminAreas.Projects),
            new("Services", "/admin/content/services", AdminAreas.Services),
        }),
        new("Who we are", new NavItem[]
        {
            new("Mission & vision", "/admin/content/mission", AdminAreas.About),
            new("Capabilities", "/admin/content/capabilities", AdminAreas.About),
            new("Values", "/admin/content/values", AdminAreas.About),
        }),
        new("Foundation", new NavItem[]
        {
            new("Pillars", "/admin/content/pillars", AdminAreas.Foundation),
            new("How we work", "/admin/content/principles", AdminAreas.Foundation),
            new("Focus areas", "/admin/content/focus", AdminAreas.Foundation),
            new("SDG goals", "/admin/content/sdg", AdminAreas.Foundation),
            new("Story tiles", "/admin/content/stories", AdminAreas.Foundation),
            new("Story gallery", "/admin/content/gallery", AdminAreas.Foundation),
        }),
        new("Blog", new NavItem[]
        {
            new("Posts", "/admin/content/posts", AdminAreas.Blog),
            new("Categories", "/admin/content/categories", AdminAreas.Blog),
        }),
        new("Pages", new NavItem[] { new("Country pages", "/admin/country-pages", AdminAreas.Pages) }),
        new("Inbox", new NavItem[] { new("Inquiries", "/admin/inquiries", AdminAreas.Inquiries) }),
        new("Library", new NavItem[] { new("Media", "/admin/media", AdminAreas.Media) }),
        new("Site", new NavItem[]
        {
            new("Site settings", "/admin/settings", AdminAreas.Settings),
            new("Audit log", "/admin/audit"),
        }),
        new("Access", new NavItem[] { new("Users", "/admin/users", SuperAdminOnly: true) }),
    };

    public static IEnumerable<NavGroup> VisibleTo(AccessSnapshot access)
    {
        foreach (var group in Groups)
        {
            var items = group.Items.Where(i => Allowed(i, access)).ToList();
            if (items.Count > 0)
            {
                yield return new NavGroup(group.Title, items);
            }
        }
    }

    private static bool Allowed(NavItem item, AccessSnapshot access)
    {
        if (item.SuperAdminOnly)
        {
            return access.IsSuperAdmin;
        }

        // The audit log has no area of its own: anyone with any page can open it (it filters itself).
        return item.Area is null ? access.HasAnyArea : access.CanView(item.Area);
    }
}
