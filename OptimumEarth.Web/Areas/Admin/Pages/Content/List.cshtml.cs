using Microsoft.AspNetCore.Mvc;
using OptimumEarth.Web.Areas.Admin.Content;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Areas.Admin.Pages.Content;

public class ListModel : AdminPageModel
{
    private readonly AppDbContext _db;
    private readonly ContentCache _cache;
    private readonly AuditService _audit;

    public ListModel(AppDbContext db, ContentCache cache, AuditService audit)
    {
        _db = db;
        _cache = cache;
        _audit = audit;
    }

    protected override string? Area => ContentRegistry.Find(RouteData.Values["slug"] as string)?.Area;

    public IContentDescriptor Descriptor { get; private set; } = null!;

    public ListQuery Query { get; private set; } = new();

    public ListResult Result { get; private set; } = new(Array.Empty<RowView>(), 0, 1, 25);

    public Dictionary<string, List<(string Value, string Label)>> FilterOptions { get; } = new();

    /// <summary>The query values to carry through paging, filtering and reordering.</summary>
    public Dictionary<string, string> Carry { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        var descriptor = ContentRegistry.Find(slug);
        if (descriptor is null)
        {
            return NotFound();
        }

        Descriptor = descriptor;
        if (descriptor.Single)
        {
            return RedirectToPage("Edit", new { slug });
        }

        await LoadAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostMoveAsync(string slug, int id, int delta)
    {
        var descriptor = ContentRegistry.Find(slug);
        if (descriptor is null || delta is not (-1 or 1))
        {
            return NotFound();
        }

        Descriptor = descriptor;
        BuildQuery(Request.Form.ToDictionary(kv => kv.Key, kv => kv.Value.ToString()));
        await descriptor.MoveAsync(_db, id, delta, Query);
        _cache.Invalidate();
        await _audit.LogAsync(descriptor.Area, descriptor.Label, "Reordered", string.Empty);
        return Redirect(PathFor(slug) + "?" + string.Join('&', Carry.Where(kv => !string.IsNullOrEmpty(kv.Value)).Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}")));
    }

    private async Task LoadAsync()
    {
        BuildQuery(Request.Query.ToDictionary(kv => kv.Key, kv => kv.Value.ToString()));
        Result = await Descriptor.ListAsync(_db, Query);
        Query.Page = Result.Page;
        foreach (var filter in Descriptor.Filters)
        {
            FilterOptions[filter.Key] = filter.Options?.ToList() ?? (filter.OptionsSource is null ? new() : await OptionSources.LoadAsync(_db, filter.OptionsSource, null));
        }
    }

    private void BuildQuery(Dictionary<string, string> source)
    {
        var q = new ListQuery();
        if (source.TryGetValue("p", out var p) && int.TryParse(p, out var page))
        {
            q.Page = page;
        }

        if (source.TryGetValue("size", out var s) && int.TryParse(s, out var size))
        {
            q.Size = size;
        }

        q.Search = source.GetValueOrDefault("q") ?? string.Empty;
        q.Status = source.GetValueOrDefault("status") is { Length: > 0 } st ? st : "all";
        foreach (var filter in Descriptor.Filters)
        {
            q.Filters[filter.Key] = source.GetValueOrDefault("f_" + filter.Key) ?? string.Empty;
        }

        Query = q;
        Carry = new Dictionary<string, string> { ["q"] = q.Search, ["status"] = q.Status == "all" ? string.Empty : q.Status, ["size"] = q.Size.ToString(), ["p"] = q.Page.ToString() };
        foreach (var (key, value) in q.Filters)
        {
            Carry["f_" + key] = value;
        }
    }

    public static string PathFor(string slug) => $"/admin/content/{slug}";
}
