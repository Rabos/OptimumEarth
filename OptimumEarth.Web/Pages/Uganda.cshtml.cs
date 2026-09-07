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
            ("Sectors", "Water · Energy · Mining"),
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
            new("Project photo", "WATER SUPPLY · 2020", "Uganda Production Wells Survey", "National Water and Sewerage Corporation · 13 production wells across multiple districts"),
            new("Project photo", "GROUNDWATER · 2022", "Kingfisher Monitoring Wells", "CNOOC · Monitoring well design and installation, Albertine region"),
            new("Project photo", "ENVIRONMENT · 2023", "Pivot Irrigation ESIA Initiative", "NASECO · Environmental and social impact assessment"),
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
