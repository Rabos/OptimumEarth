using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Areas.Admin.Content;

/// <summary>
/// Extension points for content types that need more than plain fields: the
/// "Appears on" placements of projects and services, uniqueness rules, and
/// protecting items that other things depend on.
/// </summary>
public abstract class ContentHooks<T> where T : class
{
    public virtual Task<List<PlacementOption>> PlacementOptionsAsync(AppDbContext db, AccessSnapshot access, T? entity) =>
        Task.FromResult(new List<PlacementOption>());

    /// <summary>Starting values for a new item, keyed by field key.</summary>
    public virtual Dictionary<string, string> Defaults(AccessSnapshot access) => new();

    /// <summary>Add to <paramref name="errors"/> (keyed by field key) to refuse the save.</summary>
    public virtual Task ValidateAsync(AppDbContext db, AccessSnapshot access, T? existing, FormView form, Dictionary<string, string> errors) =>
        Task.CompletedTask;

    /// <summary>Runs after the entity itself is saved, so it has an Id.</summary>
    public virtual Task AfterSaveAsync(AppDbContext db, AccessSnapshot access, T entity, FormView form) => Task.CompletedTask;

    /// <summary>Return a message to block deleting this item.</summary>
    public virtual Task<string?> BeforeDeleteAsync(AppDbContext db, T entity) => Task.FromResult<string?>(null);
}

/// <summary>Named option lists for Select, Image and filter controls.</summary>
public static class OptionSources
{
    public static async Task<List<(string Value, string Label)>> LoadAsync(AppDbContext db, string source, string? current)
    {
        var list = new List<(string Value, string Label)>();
        switch (source)
        {
            case "media":
                var media = await db.Media.AsNoTracking().OrderBy(m => m.FileName).Select(m => new { m.Path, m.FileName }).ToListAsync();
                list.AddRange(media.Select(m => (m.Path, m.FileName)));
                break;
            case "categories":
                var cats = await db.BlogCategories.AsNoTracking().OrderBy(c => c.SortOrder).ThenBy(c => c.Name).Select(c => new { c.Id, c.Name }).ToListAsync();
                list.AddRange(cats.Select(c => (c.Id.ToString(), c.Name)));
                break;
            case "stories":
                var stories = await db.Stories.AsNoTracking().OrderBy(s => s.SortOrder).Select(s => new { s.Key, s.Title }).ToListAsync();
                list.AddRange(stories.Select(s => (s.Key, s.Title)));
                break;
            case "authors":
                var editors = await (from ur in db.UserRoles
                                     join r in db.Roles on ur.RoleId equals r.Id
                                     join u in db.Users on ur.UserId equals u.Id
                                     where r.Name == Roles.Editor && u.IsActive && !u.IsHidden
                                     orderby u.DisplayName
                                     select u.DisplayName).ToListAsync();
                list.AddRange(editors.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => (n, n)));
                break;
        }

        // Keep an existing value selectable even if it is no longer in the list (a renamed author, an image added by hand).
        if (!string.IsNullOrEmpty(current) && list.All(o => o.Value != current))
        {
            list.Insert(0, (current, current));
        }

        return list;
    }
}
