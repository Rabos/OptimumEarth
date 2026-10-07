using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Areas.Admin.Pages;

public class InquiriesModel : AdminPageModel
{
    public static readonly int[] PageSizes = { 10, 25, 50, 100 };
    public static readonly string[] Destinations = { "Uganda", "Zambia", "Foundation", "Not sure yet" };

    private readonly AppDbContext _db;
    private readonly AuditService _audit;

    public InquiriesModel(AppDbContext db, AuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    protected override string? Area => AdminAreas.Inquiries;

    [BindProperty(SupportsGet = true)] public string? Q { get; set; }
    [BindProperty(SupportsGet = true)] public string? Status { get; set; }
    [BindProperty(SupportsGet = true)] public string? Dest { get; set; }
    [BindProperty(SupportsGet = true)] public int P { get; set; } = 1;
    [BindProperty(SupportsGet = true)] public int Size { get; set; } = 25;
    [BindProperty(SupportsGet = true)] public int? Id { get; set; }

    public List<Inquiry> Items { get; private set; } = new();

    public int Total { get; private set; }

    public Inquiry? Selected { get; private set; }

    public Dictionary<string, string> Carry => new()
    {
        ["q"] = Q ?? string.Empty,
        ["status"] = Status ?? string.Empty,
        ["dest"] = Dest ?? string.Empty,
        ["size"] = Size.ToString(),
    };

    private IQueryable<Inquiry> Filtered()
    {
        IQueryable<Inquiry> query = _db.Inquiries.AsNoTracking();
        if (Enum.TryParse<InquiryStatus>(Status, true, out var status))
        {
            query = query.Where(i => i.Status == status);
        }

        if (!string.IsNullOrEmpty(Dest) && Destinations.Contains(Dest))
        {
            query = query.Where(i => i.Destination == Dest);
        }

        if (!string.IsNullOrWhiteSpace(Q))
        {
            var like = "%" + Q.Trim().Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";
            query = query.Where(i => EF.Functions.ILike(i.FullName, like) || EF.Functions.ILike(i.Email, like) || EF.Functions.ILike(i.Organisation, like) || EF.Functions.ILike(i.Message, like) || EF.Functions.ILike(i.Service, like));
        }

        return query;
    }

    public async Task OnGetAsync()
    {
        if (!PageSizes.Contains(Size))
        {
            Size = 25;
        }

        var query = Filtered();
        Total = await query.CountAsync();
        var pages = Math.Max(1, (int)Math.Ceiling(Total / (double)Size));
        P = Math.Min(Math.Max(1, P), pages);
        Items = await query.OrderByDescending(i => i.CreatedUtc).Skip((P - 1) * Size).Take(Size).ToListAsync();
        Selected = Id is null ? Items.FirstOrDefault() : await _db.Inquiries.AsNoTracking().FirstOrDefaultAsync(i => i.Id == Id);
    }

    public async Task<IActionResult> OnPostNoteAsync(int id, string? note, string? status)
    {
        var inquiry = await _db.Inquiries.FirstOrDefaultAsync(i => i.Id == id);
        if (inquiry is null)
        {
            return NotFound();
        }

        var changes = new List<string>();
        if (note is not null && (note = note.Trim()) != inquiry.Note)
        {
            if (note.Length > 2000)
            {
                TempData["FlashError"] = "Keep notes under 2000 characters.";
                return Back(id);
            }

            inquiry.Note = note;
            changes.Add("Added note");
        }

        if (Enum.TryParse<InquiryStatus>(status, true, out var next) && next != inquiry.Status)
        {
            inquiry.Status = next;
            changes.Add("Marked " + next.ToString().ToLowerInvariant());
        }

        if (changes.Count > 0)
        {
            await _db.SaveChangesAsync();
            await _audit.LogAsync(AdminAreas.Inquiries, "Inquiries", string.Join(", ", changes), inquiry.FullName);
            TempData["Flash"] = "Saved.";
        }

        return Back(id);
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var inquiry = await _db.Inquiries.FirstOrDefaultAsync(i => i.Id == id);
        if (inquiry is null)
        {
            return NotFound();
        }

        _db.Inquiries.Remove(inquiry);
        await _db.SaveChangesAsync();
        await _audit.LogAsync(AdminAreas.Inquiries, "Inquiries", "Deleted", inquiry.FullName);
        TempData["Flash"] = "Inquiry deleted.";
        return RedirectToPage(new { q = Q, status = Status, dest = Dest, size = Size });
    }

    public async Task<IActionResult> OnPostRetryEmailAsync(int id)
    {
        var updated = await _db.Inquiries.Where(i => i.Id == id &&
            (i.EmailDelivery == EmailDeliveryStatus.Failed || i.EmailDelivery == EmailDeliveryStatus.NotConfigured || i.EmailDelivery == EmailDeliveryStatus.NotQueued))
            .ExecuteUpdateAsync(s => s.SetProperty(i => i.EmailDelivery, EmailDeliveryStatus.Pending)
                .SetProperty(i => i.EmailDeliveryError, string.Empty));
        if (updated > 0)
        {
            await _audit.LogAsync(AdminAreas.Inquiries, "Inquiries", "Queued email retry", id.ToString());
            TempData["Flash"] = "Email queued for delivery.";
        }
        else TempData["FlashError"] = "Email is already queued, sending or sent.";
        return Back(id);
    }

    /// <summary>Downloads the filtered inquiries as CSV. Personal data, so it needs Edit.</summary>
    public async Task<IActionResult> OnGetExportAsync()
    {
        if (!CanEdit)
        {
            return Forbid();
        }

        var rows = await Filtered().OrderByDescending(i => i.CreatedUtc).ToListAsync();
        var csv = new StringBuilder("Received (UTC),Status,Routed to,Name,Organisation,Email,Phone,Audience,Country,Service,Message,Note\r\n");
        foreach (var i in rows)
        {
            csv.AppendLine(string.Join(',', new[]
            {
                Cell(i.CreatedUtc.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)), Cell(i.Status.ToString()), Cell(i.Destination), Cell(i.FullName),
                Cell(i.Organisation), Cell(i.Email), Cell(i.Phone), Cell(i.Audience), Cell(i.Country), Cell(i.Service), Cell(i.Message), Cell(i.Note),
            }));
        }

        await _audit.LogAsync(AdminAreas.Inquiries, "Inquiries", "Exported CSV", $"{rows.Count} rows");
        return File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray(), "text/csv; charset=utf-8", $"inquiries-{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    /// <summary>Quotes a cell, and defuses values a spreadsheet would run as a formula.</summary>
    private static string Cell(string value)
    {
        if (value.Length > 0 && value[0] is '=' or '+' or '-' or '@' or '\t' or '\r')
        {
            value = "'" + value;
        }

        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }

    private IActionResult Back(int id) => RedirectToPage(new { id, q = Q, status = Status, dest = Dest, size = Size, p = P });
}
