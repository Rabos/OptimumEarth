using Microsoft.AspNetCore.Mvc;
using OptimumEarth.Web.Areas.Admin.Content;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Areas.Admin.Pages.Content;

public class EditModel : AdminPageModel
{
    private readonly AppDbContext _db;
    private readonly ContentCache _cache;
    private readonly AuditService _audit;

    public EditModel(AppDbContext db, ContentCache cache, AuditService audit)
    {
        _db = db;
        _cache = cache;
        _audit = audit;
    }

    protected override string? Area => ContentRegistry.Find(RouteData.Values["slug"] as string)?.Area;

    public IContentDescriptor Descriptor { get; private set; } = null!;

    public EditData Data { get; private set; } = null!;

    public Dictionary<string, string> Errors { get; private set; } = new();

    public Dictionary<string, List<(string Value, string Label)>> Options { get; } = new();

    public async Task<IActionResult> OnGetAsync(string slug, int? id)
    {
        var descriptor = ContentRegistry.Find(slug);
        if (descriptor is null)
        {
            return NotFound();
        }

        Descriptor = descriptor;
        if (id is null && !descriptor.Single && !descriptor.CanAdd)
        {
            return NotFound();
        }

        if (id is null && !descriptor.Single && !CanEdit)
        {
            // Adding something is an edit: a view-only person has no use for the empty form.
            return Forbid();
        }

        var data = await descriptor.LoadAsync(_db, Access, id);
        if (data is null)
        {
            return NotFound();
        }

        Data = data;
        await LoadOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string slug, int? id, string? submit)
    {
        var descriptor = ContentRegistry.Find(slug);
        if (descriptor is null)
        {
            return NotFound();
        }

        Descriptor = descriptor;
        if (id is null && !descriptor.Single && !descriptor.CanAdd)
        {
            return NotFound();
        }

        var form = new FormView(Request.Form);
        var mode = !descriptor.HasStatus || descriptor.Single ? SaveMode.Keep : submit == "draft" ? SaveMode.Draft : SaveMode.Publish;
        var outcome = await descriptor.SaveAsync(_db, Access, id, form, mode);

        if (!outcome.Succeeded)
        {
            Errors = outcome.Errors;
            Data = await descriptor.FromFormAsync(_db, Access, id, form);
            await LoadOptionsAsync();
            TempData.Remove("Flash");
            ModelState.AddModelError(string.Empty, "Some fields need attention.");
            return Page();
        }

        _cache.Invalidate();
        var action = outcome.IsNew ? "Created" : mode == SaveMode.Draft ? "Saved draft" : mode == SaveMode.Publish ? "Published" : "Edited";
        await _audit.LogAsync(descriptor.Area, descriptor.Label, action, outcome.Title);

        TempData["Flash"] = descriptor.Single ? "Saved." : mode == SaveMode.Draft ? $"Saved “{outcome.Title}” as a draft. It is not visible on the site." : $"Saved “{outcome.Title}”.";
        return descriptor.Single ? RedirectToPage(new { slug }) : Redirect($"/admin/content/{slug}");
    }

    public async Task<IActionResult> OnPostDeleteAsync(string slug, int id)
    {
        var descriptor = ContentRegistry.Find(slug);
        if (descriptor is null)
        {
            return NotFound();
        }

        var result = await descriptor.DeleteAsync(_db, id);
        if (result.Error is not null)
        {
            TempData["FlashError"] = result.Error;
            return Redirect($"/admin/content/{slug}/edit/{id}");
        }

        _cache.Invalidate();
        await _audit.LogAsync(descriptor.Area, descriptor.Label, "Deleted", result.Title);
        TempData["Flash"] = $"Deleted “{result.Title}”.";
        return Redirect($"/admin/content/{slug}");
    }

    private async Task LoadOptionsAsync()
    {
        foreach (var field in Descriptor.Fields.Where(f => f.Kind is FieldKind.Select or FieldKind.Image))
        {
            Data.Values.TryGetValue(field.Key, out var current);
            Options[field.Key] = await Descriptor.OptionsAsync(_db, field, current);
        }
    }
}
