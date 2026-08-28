using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class ZambiaModel : PageModel
{
    [BindProperty]
    public ContactInquiry Contact { get; set; } = new() { Destination = "Zambia" };

    public ContactPanelViewModel Panel { get; set; } = null!;

    public List<ProjectCaseStudy> Projects { get; } = new()
    {
        new("Energy Infrastructure Feasibility Study", "Regional Client", "Energy", "Lusaka, Zambia", "2024"),
        new("Water Resources Baseline Assessment", "Development Partner", "Water Supply", "Copperbelt, Zambia", "2023"),
        new("Environmental Screening & GIS Support", "Infrastructure Client", "Environmental Services", "Zambia", "2024"),
    };

    private static readonly List<string> Reasons = new()
    {
        "Energy / infrastructure project",
        "Water supply project",
        "GIS & mapping services",
        "Environmental assessment",
        "General enquiry — Zambia"
    };

    public void OnGet() => BuildPanel();

    public IActionResult OnPostContact()
    {
        Contact.Destination = "Zambia";
        if (!ModelState.IsValid)
        {
            BuildPanel();
            return Page();
        }

        ModelState.Clear();
        Contact = new ContactInquiry { Destination = "Zambia" };
        BuildPanel(submitted: true);
        return Page();
    }

    private void BuildPanel(bool submitted = false)
    {
        Panel = new ContactPanelViewModel
        {
            Contact = Contact,
            Heading = "Talk to the Zambia team",
            Intro = "Tell us about your project — our regional team supporting Zambia will follow up.",
            Reasons = Reasons,
            Submitted = submitted,
            InfoItems = new()
            {
                ("Office", "Lusaka, Zambia"),
                ("Email", "zambia@optimum-earth.com"),
                ("Coverage", "Nationwide, project-dependent"),
            }
        };
    }
}
