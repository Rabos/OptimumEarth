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
            ("Status", "Established in 2026"),
            ("Office", "Lusaka"),
            ("Service lines", "6"),
            ("Sectors", "Mining · Energy · Water · Equipment Hire & Transportation"),
        },
        SectionOneTitle = "Services in Zambia",
        SectionOneLinkText = "All services",
        SectionOneLinkHref = "/services",
        SectionOneCards = new()
        {
            new("Equipment hire and transportation", "Haul trucks, tippers, low-beds, excavators, graders and site support equipment, with or without operators, for mining and construction."),
            new("Technical studies", "Hydrogeological, geophysical, geotechnical and topographic surveys, and feasibility studies for energy, water and mining projects."),
            new("Engineering design", "Solar PV, transmission and substation support, water supply and civil infrastructure designs, from concept to tender documents."),
            new("Construction services", "Borehole drilling, pipelines, pump stations, earthworks and civil works, with full supervision and HSE management."),
            new("ESIA and compliance", "Screening, impact assessments, management plans, audits and stakeholder engagement to ZEMA and lender standards."),
            new("Climate and natural resources", "Water resources master plans, climate-smart agriculture, and forest, wetland and catchment mapping for resilient landscapes."),
        },
        SectionTwoTitle = "Projects",
        SectionTwoLinkText = "All projects",
        SectionTwoLinkHref = "/projects",
        SectionTwoCards = new()
        {
            new("Project photo", "ZAMBIA", "50MW Kasempa Solar Plant", "Prefeasibility complete, detailed feasibility study under way. Visit www.kasempasolar.com", "", "", "/img/optimum-earth-images/project4.jpg"),
            new("Project photo", "ZAMBIA", "Pensulo - Mansa 330kV Powerline Corridor", "Authorisation to do feasibility studies granted by MoE Zambia", "", "", "/img/optimum-earth-images/project5.jpg"),
            new("Project photo", "ZAMBIA", "Partnerships with Ntux Investments Ltd - Zambia", "Earthmoving equipment and transport.", "", "", "/img/optimum-earth-images/project6.jpg"),
        },
        SectionTwoCardDescriptions = new()
        {
            new("50mw-kasempa-solar-plant", "Prefeasibility complete, detailed feasibility study under way. Visit www.kasempasolar.com", "/img/optimum-earth-images/project4.jpg", "Client : Kasempa Solar", "50MW Kasempa Solar Plant", "Project photo"),
            new("pensulo-mansa-330kv-powerline-corridor", "Authorisation to do feasibility studies granted by MoE Zambia", "/img/optimum-earth-images/project5.jpg", "Client : MoE Zambia", "Pensulo - Mansa 330kV Powerline Corridor", "Project photo"),
            new("partnerships-with-ntux-investments-ltd-zambia", "Earthmoving equipment and transport.", "/img/optimum-earth-images/project6.jpg", "Client : Ntux Investments Ltd", "Partnerships with Ntux Investments Ltd - Zambia", "Project photo"),
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
