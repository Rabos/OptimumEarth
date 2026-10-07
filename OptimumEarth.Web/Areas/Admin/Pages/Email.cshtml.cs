using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Areas.Admin.Pages;

public sealed record MailboxView(string Destination, string Address, string Recipient, bool HasPassword);

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class EmailModel(AppDbContext db, InquiryEmailService sender, AuditService audit, ContentCache cache) : AdminPageModel
{
    protected override bool SuperAdminOnly => true;
    public List<MailboxView> Mailboxes { get; private set; } = [];
    public string DefaultDestination { get; private set; } = "Uganda";
    [BindProperty] public MailboxInput Input { get; set; } = new();

    public sealed class MailboxInput
    {
        [Required] public string Destination { get; set; } = string.Empty;
        [Required, EmailAddress, StringLength(180)] public string Address { get; set; } = string.Empty;
        [Required, EmailAddress, StringLength(180)] public string Recipient { get; set; } = string.Empty;
        [StringLength(1000)] public string? Password { get; set; }
    }

    public async Task OnGetAsync() => await LoadAsync();

    private async Task LoadAsync()
    {
        var settings = await db.Settings.AsNoTracking().FirstAsync();
        DefaultDestination = settings.DefaultEmailDestination;
        var mailboxes = await db.EmailMailboxes.AsNoTracking().ToListAsync();
        Mailboxes = InquiryEmailService.Destinations.Select(destination =>
        {
            var mailbox = mailboxes.SingleOrDefault(m => m.Destination == destination);
            var recipient = InquiryEmailService.Recipient(settings, destination);
            return new MailboxView(destination, mailbox?.Address ?? recipient, recipient, !string.IsNullOrEmpty(mailbox?.ProtectedPassword));
        }).ToList();
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!InquiryEmailService.Destinations.Contains(Input.Destination)) return BadRequest();
        var mailbox = await db.EmailMailboxes.SingleOrDefaultAsync(m => m.Destination == Input.Destination);
        // Changing the login must not silently reuse another mailbox's credential.
        if ((mailbox is null || string.IsNullOrEmpty(mailbox.ProtectedPassword) || !string.Equals(mailbox.Address, Input.Address?.Trim(), StringComparison.OrdinalIgnoreCase))
            && string.IsNullOrEmpty(Input.Password))
            ModelState.AddModelError("Input.Password", "Enter a password when setting up or changing the sending mailbox.");
        if (!ModelState.IsValid)
        {
            Input.Password = null;
            ModelState.Remove("Input.Password");
            TempData["FlashError"] = "Check the email addresses and enter a password when setting up or changing a mailbox (maximum 1000 characters).";
            await LoadAsync();
            return Page();
        }
        if (mailbox is null)
        {
            mailbox = new EmailMailbox { Destination = Input.Destination };
            db.EmailMailboxes.Add(mailbox);
        }
        mailbox.Address = Input.Address!.Trim();
        if (!string.IsNullOrEmpty(Input.Password)) mailbox.ProtectedPassword = sender.ProtectPassword(Input.Password);
        Input.Password = null;
        ModelState.Remove("Input.Password");
        var settings = await db.Settings.FirstAsync();
        switch (Input.Destination)
        {
            case "Uganda": settings.UgandaTo = Input.Recipient.Trim(); break;
            case "Zambia": settings.ZambiaTo = Input.Recipient.Trim(); break;
            case "Foundation": settings.FoundationTo = Input.Recipient.Trim(); break;
        }
        await db.SaveChangesAsync();
        cache.Invalidate();
        await audit.LogAsync(AdminAreas.Settings, "Email configuration", "Saved mailbox configuration", Input.Destination);
        TempData["Flash"] = "Mailbox saved. Send a test email to verify delivery. Enquiries that previously failed can be retried in Inquiries.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDefaultAsync(string destination)
    {
        if (!InquiryEmailService.Destinations.Contains(destination)) return BadRequest();
        var settings = await db.Settings.FirstAsync();
        settings.DefaultEmailDestination = destination;
        await db.SaveChangesAsync();
        cache.Invalidate();
        await audit.LogAsync(AdminAreas.Settings, "Email configuration", "Changed default destination", destination);
        TempData["Flash"] = "Default destination saved.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTestAsync(string destination)
    {
        if (!InquiryEmailService.Destinations.Contains(destination)) return BadRequest();
        try
        {
            await sender.SendAsync(destination, "Optimum Earth — SMTP test", "This test confirms that the website can send through your Hostinger mailbox.", null, HttpContext.RequestAborted);
            TempData["Flash"] = "Test email accepted by Hostinger. Check the configured recipient's inbox and spam folder.";
            await audit.LogAsync(AdminAreas.Settings, "Email configuration", "Sent test email", destination);
        }
        catch (Exception ex) when (!HttpContext.RequestAborted.IsCancellationRequested)
        {
            TempData["FlashError"] = InquiryEmailService.Error(ex);
        }
        return RedirectToPage();
    }
}
