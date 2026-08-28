using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class FoundationModel : PageModel
{
    [BindProperty]
    public ContactInquiry Contact { get; set; } = new() { Destination = "Foundation" };

    public ContactPanelViewModel Panel { get; set; } = null!;

    public List<ProjectCaseStudy> Programmes { get; } = new()
    {
        new("Community Water Access Programme", "Local communities", "Access", "Uganda", "Ongoing"),
        new("Climate Resilience Pilot", "Community partners", "Resilience", "East Africa", "Ongoing"),
        new("Clean Energy for Households", "Community partners", "Impact", "Uganda & Zambia", "Ongoing"),
    };

    private static readonly List<string> Reasons = new()
    {
        "Partner with the Foundation",
        "Fund a programme",
        "Collaborate on a project",
        "Media / press",
        "General enquiry — Foundation"
    };

    public void OnGet() => BuildPanel();

    public IActionResult OnPostContact()
    {
        Contact.Destination = "Foundation";
        if (!ModelState.IsValid)
        {
            BuildPanel();
            return Page();
        }

        ModelState.Clear();
        Contact = new ContactInquiry { Destination = "Foundation" };
        BuildPanel(submitted: true);
        return Page();
    }

    private void BuildPanel(bool submitted = false)
    {
        Panel = new ContactPanelViewModel
        {
            Contact = Contact,
            Heading = "Partner with the Foundation",
            Intro = "Whether you want to fund, collaborate or simply learn more, tell us how you'd like to get involved.",
            Reasons = Reasons,
            Submitted = submitted,
            InfoItems = new()
            {
                ("Email", "foundation@optimum-earth.com"),
                ("Based in", "Kampala, Uganda"),
                ("Focus", "Access · Resilience · Impact"),
            }
        };
    }
}
