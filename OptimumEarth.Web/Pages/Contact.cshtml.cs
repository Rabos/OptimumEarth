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

    // The Services and Who We Are pages link here with ?project= to say what the visitor is after;
    // the message box starts with that line so they only have to add the details.
    private static readonly Dictionary<string, string> ProjectOpeners = new(StringComparer.OrdinalIgnoreCase)
    {
        ["borehole"] = "We're planning a borehole or water supply. ",
        ["power"] = "We're planning a substation or power line. ",
        ["mining"] = "We're planning work for a mine or an oil and gas development. ",
        ["climate"] = "We're planning a catchment or climate project. ",
        ["equipment"] = "We'd like to hire plant or equipment in Zambia. ",
        ["careers"] = "I'd like to work with Optimum Earth. ",
    };

    // ?project=similar&about=<project title> comes from a project's "Discuss a similar project" button.
    public void OnGet(string? project, string? about)
    {
        if (project == "similar" && !string.IsNullOrWhiteSpace(about))
        {
            Contact.Message = $"We're planning something similar to your project “{about.Trim()[..Math.Min(about.Trim().Length, 120)]}”. ";
        }
        else if (project is not null && ProjectOpeners.TryGetValue(project, out var opener))
        {
            Contact.Message = opener;
        }
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
