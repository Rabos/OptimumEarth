using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;

namespace OptimumEarth.Web.Areas.Admin.Pages;

public class AuditModel : AdminPageModel
{
    public static readonly int[] PageSizes = { 10, 25, 50, 100 };

    private readonly AppDbContext _db;

    public AuditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)] public string? Who { get; set; }
    [BindProperty(SupportsGet = true)] public int P { get; set; } = 1;
    [BindProperty(SupportsGet = true)] public int Size { get; set; } = 25;

    public List<AuditEntry> Items { get; private set; } = new();

    public int Total { get; private set; }

    public List<string> Actors { get; private set; } = new();

    public Dictionary<string, string> Carry => new() { ["who"] = Who ?? string.Empty, ["size"] = Size.ToString() };

    public async Task<IActionResult> OnGetAsync()
    {
        // No area of its own: anyone with any page can open it, and sees only the changes on their own pages.
        if (!Access.HasAnyArea)
        {
            return Forbid();
        }

        if (!PageSizes.Contains(Size))
        {
            Size = 25;
        }

        IQueryable<AuditEntry> query = _db.AuditEntries.AsNoTracking();
        if (!Access.IsSuperAdmin)
        {
            var visible = AdminAreas.All.Where(a => Access.CanView(a.Key)).Select(a => a.Key).ToList();
            query = query.Where(e => e.Area != string.Empty && visible.Contains(e.Area));
        }

        Actors = await query.Select(e => e.Actor).Distinct().OrderBy(a => a).ToListAsync();
        if (!string.IsNullOrEmpty(Who))
        {
            query = query.Where(e => e.Actor == Who);
        }

        Total = await query.CountAsync();
        var pages = Math.Max(1, (int)Math.Ceiling(Total / (double)Size));
        P = Math.Min(Math.Max(1, P), pages);
        Items = await query.OrderByDescending(e => e.WhenUtc).ThenByDescending(e => e.Id).Skip((P - 1) * Size).Take(Size).ToListAsync();
        return Page();
    }
}
