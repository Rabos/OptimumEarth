using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages;

public class ContactModel : PageModel
{
    private readonly InquiryService _inquiries;

    public ContactModel(InquiryService inquiries)
    {
        _inquiries = inquiries;
    }

    [BindProperty]
    public ContactInquiry Contact { get; set; } = new() { Audience = "Client" };

    public bool Submitted { get; set; }

    public static readonly List<string> Audiences = new() { "Client", "Partner", "Foundation supporter" };

    public static readonly List<string> Countries = new() { "Uganda", "Zambia", "Not sure yet" };

    public static readonly List<string> ServiceOptions = new()
    {
        "Water supply solutions",
        "Groundwater monitoring",
        "Environmental services",
        "GIS, mapping & remote sensing",
        "Solutions for mines, oil & gas",
        "Foundation partnership",
        "General enquiry",
    };

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Stored for the dashboard inbox, where it is routed - by
        // Contact.Destination - to the Uganda team, the Zambia team or the
        // Foundation, and answered within two working days.
        await _inquiries.AddContactAsync(Contact);
        ModelState.Clear();
        Contact = new ContactInquiry { Audience = "Client" };
        Submitted = true;
        return Page();
    }
}
