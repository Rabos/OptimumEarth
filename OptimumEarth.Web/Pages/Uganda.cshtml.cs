using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class UgandaModel : PageModel
{
    [BindProperty]
    public QuickInquiry Quick { get; set; } = new() { Destination = "Uganda" };

    public bool Submitted { get; set; }

    private static readonly List<string> ServiceOptions = new()
    {
        "Water supply solutions",
        "Groundwater monitoring",
        "Environmental services",
        "GIS & mapping",
        "Mines, oil & gas",
    };

    public DestinationPageViewModel BuildContent() => new()
    {
        Theme = "uganda",
        HeroImageLabel = "Uganda hero image",
        HeroEyebrow = "OPTIMUM EARTH UGANDA",
        HeroHeadline = "Water, environment and engineering expertise, delivered in Uganda since 2017.",
        HeroSub = "Kampala-based teams supporting national utilities, energy operators, industry and development partners across the country.",
        OverviewEyebrow = "COUNTRY OVERVIEW",
        OverviewQuote = "Uganda is our home market. From Kampala we deliver hydrogeology, borehole development, groundwater monitoring, environmental assessment and geo-intelligence for national utilities, oil and gas operators, cement and quarry industry and humanitarian organisations.",
        OverviewBody = "Our Ugandan team combines field capability with in-house analysis, so surveys, testing and reporting run under one accountable contract - with the compliance documentation regulators and lenders expect.",
        Facts = new()
        {
            ("Established", "2017"),
            ("Office", "Kampala"),
            ("Service lines", "5"),
            ("Sectors", "Water · Energy · Mining · Infrastructure"),
        },
        SectionOneTitle = "Services in Uganda",
        SectionOneLinkText = "All services",
        SectionOneLinkHref = "/services",
        SectionOneCards = new()
        {
            new("Water supply solutions", "Surface and groundwater diagnostics, borehole siting, drilling supervision and test pumping."),
            new("Groundwater monitoring", "Monitoring well networks, water-level and quality regimes, long-term reporting."),
            new("Environmental services", "ESIA, environmental audits and compliance support for regulated projects."),
            new("GIS & mapping", "Geo-intelligence products, remote sensing and spatial analysis for decision-making."),
            new("Mines, oil & gas", "Integrated water management for quarry, mining and petroleum operations."),
        },
        SectionTwoTitle = "Featured projects",
        SectionTwoLinkText = "All projects",
        SectionTwoLinkHref = "/projects",
        SectionTwoCards = new()
        {
            new("Project photo", "WATER SUPPLY · 2020", "Uganda Production Wells Survey", "Client : National Water and Sewerage Corporation", "", "", "/img/optimum-earth-images/recentwork1.jpg"),
            new("Project photo", "GROUNDWATER · 2022", "Kingfisher Monitoring Wells", "Client : CNOOC", "", "", "/img/optimum-earth-images/recentwork2.jpg"),
            new("Project photo", "ENVIRONMENT · 2023", "Pivot Irrigation ESIA Initiative", "Client : NASECO", "", "", "/img/optimum-earth-images/recentwork3.jpg"),
        },
        SectionTwoCardDescriptions = new()
        {
            new("uganda-production-wells-survey", "We conducted a survey of production wells for the National Water and Sewerage Corporation, assessing performance and recommending improvements to enhance water supply reliability.", "/img/optimum-earth-images/recentwork1.jpg", "Client : National Water and Sewerage Corporation", "Uganda Production Wells Survey", "Project photo"),
            new("kingfisher-monitoring-wells", "We designed and implemented a groundwater monitoring program for CNOOC's Kingfisher project, providing critical data for sustainable water resource management.", "/img/optimum-earth-images/recentwork2.jpg", "Client : CNOOC", "Kingfisher Monitoring Wells", "Project photo"),
            new("pivot-irrigation-esia-initiative", "We led the Environmental and Social Impact Assessment (ESIA) for NASECO's Pivot Irrigation Initiative, ensuring compliance with environmental regulations and promoting sustainable agricultural practices.", "/img/optimum-earth-images/recentwork3.jpg", "Client : NASECO", "Pivot Irrigation ESIA Initiative", "Project photo"),
        },
        CtaTitle = "Work with our Uganda team",
        CtaBody = "Roston House, Plot 56/57, P.O Box 200032, Kampala · +256 784 080551 · uganda@optimum-earth.com",
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
        Quick.Destination = "Uganda";
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // In production this would persist the enquiry and route it to the
        // Uganda team's inbox.
        ModelState.Clear();
        Quick = new QuickInquiry { Destination = "Uganda" };
        Submitted = true;
        return Page();
    }
}
