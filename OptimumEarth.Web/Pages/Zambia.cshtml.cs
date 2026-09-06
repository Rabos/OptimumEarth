using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class ZambiaModel : PageModel
{
    [BindProperty]
    public QuickInquiry Quick { get; set; } = new() { Destination = "Zambia" };

    public bool Submitted { get; set; }

    private static readonly List<string> ServiceOptions = new()
    {
        "Water supply solutions",
        "Energy & infrastructure",
        "Groundwater monitoring",
        "GIS & mapping",
    };

    public DestinationPageViewModel BuildContent() => new()
    {
        Theme = "zambia",
        HeroImageLabel = "Zambia hero image",
        HeroEyebrow = "OPTIMUM EARTH ZAMBIA",
        HeroHeadline = "Bringing regional engineering and environmental capability to the Zambian market.",
        HeroSub = "Energy, water, infrastructure and environmental services delivered by the same technical team that operates across East and Southern Africa.",
        OverviewEyebrow = "COUNTRY OVERVIEW",
        OverviewQuote = "Zambia is our emerging Southern Africa operation. We bring the hydrogeology, water management, environmental and geo-intelligence capability built over years of regional delivery to Zambian clients in mining, energy, utilities and development.",
        OverviewBody = "Local partnerships and regulatory alignment are being established now, so early clients work directly with senior technical staff and a mobilised regional field team.",
        Facts = new()
        {
            ("Status", "Establishing"),
            ("Office", "To be confirmed"),
            ("Service lines", "4"),
            ("Sectors", "Mining · Energy · Water"),
        },
        SectionOneTitle = "Services in Zambia",
        SectionOneLinkText = "All services",
        SectionOneLinkHref = "/services",
        SectionOneCards = new()
        {
            new("Water supply solutions", "Groundwater assessment, borehole siting and supervision, supply planning for industry and institutions."),
            new("Energy & infrastructure", "Support to energy projects and the infrastructure that surrounds them."),
            new("Groundwater monitoring", "Monitoring networks and reporting regimes for mining and industrial operations."),
            new("GIS & mapping", "Spatial analysis, remote sensing and field data systems."),
        },
        SectionTwoTitle = "Projects & pipeline",
        SectionTwoLinkText = "All projects",
        SectionTwoLinkHref = "/projects",
        SectionTwoCards = new()
        {
            new("Project photo", "ZAMBIA", "Project to be confirmed", "Case study to be published as evidence becomes available"),
            new("Project photo", "ZAMBIA", "Project to be confirmed", "Case study to be published as evidence becomes available"),
            new("Project photo", "REGIONAL", "Regional experience applies here", "Delivery record from Uganda and the wider region"),
        },
        SectionTwoNote = "Zambia case studies will be published as projects are delivered and client permissions are confirmed.",
        CtaTitle = "Work with our Zambia team",
        CtaBody = "Office address to be confirmed · zambia@optimum-earth.com",
        CtaServiceLabel = "Service of interest",
        CtaServiceOptions = ServiceOptions,
        Quick = Quick,
        Submitted = Submitted,
    };

    public void OnGet()
    {
    }

    public IActionResult OnPostQuick()
    {
        Quick.Destination = "Zambia";
        if (!ModelState.IsValid)
        {
            return Page();
        }

        ModelState.Clear();
        Quick = new QuickInquiry { Destination = "Zambia" };
        Submitted = true;
        return Page();
    }
}
