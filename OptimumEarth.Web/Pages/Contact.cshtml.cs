using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class ContactModel : PageModel
{
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

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // In production this would persist the enquiry and route it - by
        // Contact.Destination - to the Uganda team, the Zambia team or the
        // Foundation, and reply within two working days.
        ModelState.Clear();
        Contact = new ContactInquiry { Audience = "Client" };
        Submitted = true;
        return Page();
    }
}
