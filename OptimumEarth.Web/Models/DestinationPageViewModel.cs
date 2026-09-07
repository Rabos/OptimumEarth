namespace OptimumEarth.Web.Models;

/// <summary>
/// Drives the shared destination template (hero, overview + at-a-glance panel,
/// a cards section, an evidence section, and a CTA band with a quick enquiry
/// form) used by the Uganda, Zambia and Foundation pages.
/// </summary>
public class DestinationPageViewModel
{
    public required string Theme { get; init; }

    public required string HeroImageLabel { get; init; }
    public required string HeroEyebrow { get; init; }
    public required string HeroHeadline { get; init; }
    public required string HeroSub { get; init; }

    public required string OverviewEyebrow { get; init; }
    public required string OverviewQuote { get; init; }
    public required string OverviewBody { get; init; }
    public required List<(string Key, string Value)> Facts { get; init; }

    public required string SectionOneTitle { get; init; }
    public string? SectionOneLinkText { get; init; }
    public string? SectionOneLinkHref { get; init; }
    public bool UsePillarCards { get; init; }
    public required List<SimpleCard> SectionOneCards { get; init; }

    public required string SectionTwoTitle { get; init; }
    public string? SectionTwoLinkText { get; init; }
    public string? SectionTwoLinkHref { get; init; }
    public required List<ProjectCard> SectionTwoCards { get; init; }
    public string? SectionTwoNote { get; init; }

    public required string CtaTitle { get; init; }
    public required string CtaBody { get; init; }
    public required string CtaServiceLabel { get; init; }
    public required List<string> CtaServiceOptions { get; init; }

    public required QuickInquiry Quick { get; init; }
    public bool Submitted { get; init; }
}
