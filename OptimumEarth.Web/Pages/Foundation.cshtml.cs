using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class FoundationModel : PageModel
{
    [BindProperty]
    public QuickInquiry Quick { get; set; } = new() { Destination = "Foundation" };

    public bool Submitted { get; set; }

    private static readonly List<string> EngagementOptions = new()
    {
        "Partner with a programme",
        "Fund a programme",
        "Collaborate on research or delivery",
        "Learn more",
    };

    public DestinationPageViewModel BuildContent() => new()
    {
        Theme = "foundation",
        HeroImageLabel = "Foundation hero image",
        HeroEyebrow = "OPTIMUM EARTH FOUNDATION",
        HeroHeadline = "Communities designing the infrastructure they need.",
        HeroSub = "Engineering capability channelled into community-led development: clean water, affordable energy and climate resilience, delivered with partners and measured by results.",
        OverviewEyebrow = "FOUNDATION OVERVIEW",
        OverviewQuote = "The Foundation exists to put engineering capability where the market does not reach, working with communities that will own and maintain what gets built.",
        OverviewBody = "Programmes are designed with their communities, baselined before work starts and reported honestly afterwards. Government, funders, NGOs and private operators work to one plan, and solutions are engineered to be repeated rather than demonstrated once.",
        Facts = new()
        {
            ("Mandate", "Non-profit arm"),
            ("Pillars", "Access · Resilience · Impact"),
            ("Model", "Community-led"),
            ("Reach", "Uganda · Zambia"),
        },
        SectionOneTitle = "Three pillars, one mandate",
        UsePillarCards = true,
        SectionOneCards = new()
        {
            new("Access", "Clean water, sanitation and affordable energy reaching households and institutions the market has not served.", "01"),
            new("Resilience", "Climate adaptation, water security and systems that keep working through drought, flood and disruption.", "02"),
            new("Impact", "Evidence-informed delivery, measured outcomes and partnerships that mobilise resources where they count.", "03"),
        },
        SectionTwoTitle = "Programmes & stories",
        SectionTwoCards = new()
        {
            new("Programme photo", "ACCESS", "Community water points", "Programme description to be confirmed."),
            new("Programme photo", "RESILIENCE", "Climate-smart water security", "Programme description to be confirmed."),
            new("Programme photo", "IMPACT", "Clean energy for institutions", "Programme description to be confirmed."),
        },
        SectionTwoNote = "Programme case studies will be published as delivery progresses and partner permissions are confirmed.",
        CtaTitle = "Work with the Foundation",
        CtaBody = "Partner, fund or collaborate. Tell us which and we will route your enquiry to the right programme lead.",
        CtaServiceLabel = "How would you like to engage?",
        CtaServiceOptions = EngagementOptions,
        Quick = Quick,
        Submitted = Submitted,
    };

    public void OnGet()
    {
    }

    public IActionResult OnPostQuick()
    {
        Quick.Destination = "Foundation";
        if (!ModelState.IsValid)
        {
            return Page();
        }

        ModelState.Clear();
        Quick = new QuickInquiry { Destination = "Foundation" };
        Submitted = true;
        return Page();
    }
}
