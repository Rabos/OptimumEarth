namespace OptimumEarth.Web.Data;

/// <summary>
/// The copy for one country page (Uganda, Zambia), rendered through the shared
/// destination template. Which services and projects appear is chosen through
/// <see cref="Services"/> and <see cref="Projects"/>, not typed in.
/// </summary>
public class CountryPage
{
    public int Id { get; set; }

    /// <summary>"uganda" or "zambia"; also the public route.</summary>
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public string HeroImageLabel { get; set; } = string.Empty;
    public string HeroEyebrow { get; set; } = string.Empty;
    public string HeroHeadline { get; set; } = string.Empty;
    public string HeroSub { get; set; } = string.Empty;
    public string OverviewEyebrow { get; set; } = string.Empty;
    public string OverviewQuote { get; set; } = string.Empty;
    public string OverviewBody { get; set; } = string.Empty;

    /// <summary>"Key | Value" lines for the at-a-glance panel.</summary>
    public List<string> Facts { get; set; } = new();

    public string SectionOneTitle { get; set; } = string.Empty;
    public string SectionOneLinkText { get; set; } = string.Empty;
    public string SectionOneLinkHref { get; set; } = string.Empty;
    public string SectionTwoTitle { get; set; } = string.Empty;
    public string SectionTwoLinkText { get; set; } = string.Empty;
    public string SectionTwoLinkHref { get; set; } = string.Empty;
    public string SectionTwoNote { get; set; } = string.Empty;
    public string CtaTitle { get; set; } = string.Empty;
    public string CtaBody { get; set; } = string.Empty;
    public string CtaServiceLabel { get; set; } = "Service of interest";
    public List<string> CtaServices { get; set; } = new();

    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

    public List<CountryPageService> Services { get; set; } = new();
    public List<CountryPageProject> Projects { get; set; } = new();
}

/// <summary>
/// A service card on a country page. Linked cards read the service's title and
/// description unless overridden; custom cards (ServiceId null) carry their own.
/// </summary>
public class CountryPageService
{
    public int Id { get; set; }
    public int CountryPageId { get; set; }
    public CountryPage? CountryPage { get; set; }
    public int? ServiceId { get; set; }
    public Service? Service { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    /// <summary>When set the card is a call-to-action link (e.g. "Not sure which service you need?") rather than a plain service card.</summary>
    public string Url { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class CountryPageProject
{
    public int Id { get; set; }
    public int CountryPageId { get; set; }
    public CountryPage? CountryPage { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    /// <summary>Optional wording for this page only; empty means use the project's own.</summary>
    public string Tag { get; set; } = string.Empty;
    public string Meta { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
