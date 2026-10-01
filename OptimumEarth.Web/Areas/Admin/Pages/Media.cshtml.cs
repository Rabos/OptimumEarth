using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Areas.Admin.Pages;

public class MediaModel : AdminPageModel
{
    public static readonly int[] PageSizes = { 12, 24, 48, 96 };

    private readonly AppDbContext _db;
    private readonly MediaService _media;
    private readonly AuditService _audit;

    public MediaModel(AppDbContext db, MediaService media, AuditService audit)
    {
        _db = db;
        _media = media;
        _audit = audit;
    }

    protected override string? Area => AdminAreas.Media;

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    [BindProperty(SupportsGet = true)]
    public int P { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int Size { get; set; } = 24;

    [BindProperty(SupportsGet = true)]
    public bool MissingAlt { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? Id { get; set; }

    public List<MediaAsset> Items { get; private set; } = new();

    public int Total { get; private set; }

    public MediaAsset? Selected { get; private set; }

    public List<string> UsedBy { get; private set; } = new();

    public Dictionary<string, string> Carry => new()
    {
        ["q"] = Q ?? string.Empty,
        ["size"] = Size.ToString(),
        ["missingAlt"] = MissingAlt ? "true" : string.Empty,
        ["id"] = Id?.ToString() ?? string.Empty,
    };

    public async Task OnGetAsync()
    {
        if (!PageSizes.Contains(Size))
        {
            Size = 24;
        }

        IQueryable<MediaAsset> query = _db.Media.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(Q))
        {
            var like = "%" + Q.Trim().Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";
            query = query.Where(m => EF.Functions.ILike(m.FileName, like) || EF.Functions.ILike(m.Alt, like));
        }

        if (MissingAlt)
        {
            query = query.Where(m => m.Alt == string.Empty);
        }

        Total = await query.CountAsync();
        var pages = Math.Max(1, (int)Math.Ceiling(Total / (double)Size));
        P = Math.Min(Math.Max(1, P), pages);
        Items = await query.OrderByDescending(m => m.UploadedUtc).ThenBy(m => m.FileName).Skip((P - 1) * Size).Take(Size).ToListAsync();

        if (Id is not null)
        {
            Selected = await _db.Media.AsNoTracking().FirstOrDefaultAsync(m => m.Id == Id);
            if (Selected is not null)
            {
                UsedBy = await _media.UsedByAsync(Selected.Path);
            }
        }
    }

    public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
    {
        if (files.Count == 0)
        {
            TempData["FlashError"] = "Choose at least one image to upload.";
            return RedirectToPage();
        }

        var saved = new List<MediaAsset>();
        var problems = new List<string>();
        foreach (var file in files)
        {
            var result = await _media.SaveAsync(file);
            if (result.Asset is not null)
            {
                saved.Add(result.Asset);
                await _audit.LogAsync(AdminAreas.Media, "Media", "Uploaded", result.Asset.FileName);
            }
            else
            {
                problems.Add(result.Error!);
            }
        }

        if (problems.Count > 0)
        {
            TempData["FlashError"] = string.Join(" ", problems);
        }

        if (saved.Count > 0)
        {
            TempData["Flash"] = $"Uploaded {saved.Count} image{(saved.Count == 1 ? "" : "s")}. Add alt text before using {(saved.Count == 1 ? "it" : "them")}.";
        }

        return saved.Count > 0 ? RedirectToPage(new { id = saved[0].Id }) : RedirectToPage();
    }

    public async Task<IActionResult> OnPostAltAsync(int id, string? alt)
    {
        var asset = await _db.Media.FirstOrDefaultAsync(m => m.Id == id);
        if (asset is null)
        {
            return NotFound();
        }

        asset.Alt = (alt ?? string.Empty).Trim();
        if (asset.Alt.Length > 200)
        {
            TempData["FlashError"] = "Keep alt text under 200 characters.";
            return RedirectToPage(new { id });
        }

        await _db.SaveChangesAsync();
        await _audit.LogAsync(AdminAreas.Media, "Media", "Edited alt text", asset.FileName);
        TempData["Flash"] = "Alt text saved.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var asset = await _db.Media.FirstOrDefaultAsync(m => m.Id == id);
        if (asset is null)
        {
            return NotFound();
        }

        var error = await _media.DeleteAsync(asset);
        if (error is not null)
        {
            TempData["FlashError"] = error;
            return RedirectToPage(new { id });
        }

        await _audit.LogAsync(AdminAreas.Media, "Media", "Deleted", asset.FileName);
        TempData["Flash"] = $"Deleted {asset.FileName}.";
        return RedirectToPage();
    }
}
