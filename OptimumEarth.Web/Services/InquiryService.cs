using OptimumEarth.Web.Data;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Services;

/// <summary>Stores what the public enquiry forms collect, so it can be answered from the dashboard.</summary>
public sealed class InquiryService
{
    private readonly AppDbContext _db;
    private readonly ILogger<InquiryService> _logger;

    public InquiryService(AppDbContext db, ILogger<InquiryService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task AddContactAsync(ContactInquiry form)
    {
        _db.Inquiries.Add(new Inquiry
        {
            Source = "Contact",
            EmailDelivery = EmailDeliveryStatus.Pending,
            Destination = form.Destination,
            FullName = form.FullName.Trim(),
            Organisation = form.Organisation?.Trim() ?? string.Empty,
            Email = form.Email.Trim(),
            Phone = form.Phone?.Trim() ?? string.Empty,
            Audience = form.Audience,
            Country = form.Country,
            Service = form.Service,
            Message = form.Message.Trim(),
        });
        await _db.SaveChangesAsync();
        _logger.LogInformation("Enquiry received for {Destination} from the Contact page.", form.Destination);
    }

    public async Task AddQuickAsync(QuickInquiry form)
    {
        _db.Inquiries.Add(new Inquiry
        {
            Source = "Quick",
            EmailDelivery = EmailDeliveryStatus.Pending,
            Destination = form.Destination,
            FullName = form.FullName.Trim(),
            Email = form.Email.Trim(),
            Service = form.Service,
            Country = form.Destination == "Foundation" ? string.Empty : form.Destination,
            Audience = form.Destination == "Foundation" ? "Foundation supporter" : "Client",
        });
        await _db.SaveChangesAsync();
        _logger.LogInformation("Enquiry received for {Destination} from a quick form.", form.Destination);
    }
}
