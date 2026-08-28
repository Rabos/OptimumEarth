using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class UgandaModel : PageModel
{
    [BindProperty]
    public ContactInquiry Contact { get; set; } = new() { Destination = "Uganda" };

    public ContactPanelViewModel Panel { get; set; } = null!;

    public List<ProjectCaseStudy> Projects { get; } = new()
    {
        new("Rural Water Supply Expansion", "District Local Government", "Water Supply", "Central Region, Uganda", "2023"),
        new("Catchment & GIS Mapping Study", "Development Partner", "GIS & Remote Sensing", "Western Uganda", "2022"),
        new("Environmental & Social Impact Assessment", "Infrastructure Client", "Environmental Services", "Kampala, Uganda", "2024"),
    };

    private static readonly List<string> Reasons = new()
    {
        "Water supply project",
        "Energy / infrastructure project",
        "GIS & mapping services",
        "Environmental assessment",
        "General enquiry — Uganda"
    };

    public void OnGet() => BuildPanel();

    public IActionResult OnPostContact()
    {
        Contact.Destination = "Uganda";
        if (!ModelState.IsValid)
        {
            BuildPanel();
            return Page();
        }

        ModelState.Clear();
        Contact = new ContactInquiry { Destination = "Uganda" };
        BuildPanel(submitted: true);
        return Page();
    }

    private void BuildPanel(bool submitted = false)
    {
        Panel = new ContactPanelViewModel
        {
            Contact = Contact,
            Heading = "Talk to the Uganda team",
            Intro = "Tell us about your project and our Uganda-based engineers and environmental specialists will follow up.",
            Reasons = Reasons,
            Submitted = submitted,
            InfoItems = new()
            {
                ("Office", "Kampala, Uganda"),
                ("Email", "uganda@optimum-earth.com"),
                ("Coverage", "Nationwide, project-dependent"),
            }
        };
    }
}
