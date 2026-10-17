namespace OptimumEarth.Web.Models;

/// <summary>The project shown as "In practice" under a service.</summary>
public record PracticeCard(string Title, string Description, string ClientLine, string ImagePath, string Href);

/// <summary>One service line's section on the Services page.</summary>
public record ServiceSection(
    string Index,
    string Slug,
    string Title,
    string ShortTitle,
    string Tagline,
    string Lead,
    string Layout,
    string ImagePath,
    string ImageAlt,
    string TileImagePath,
    IReadOnlyList<string> TileItems,
    IReadOnlyList<string> Highlights,
    IReadOnlyList<string> Capabilities,
    string StatValue,
    string StatLabel,
    PracticeCard? Practice);

public record SectorTile(string Key, string Name, string Blurb, string ImagePath);

public record ClientLogo(string Name, string LogoPath);

public record AerialShot(string Caption, string ImagePath, string Alt);

/// <summary>A service's name and anchor, for the footer and other links into the Services page.</summary>
public record ServiceLink(string ShortTitle, string Slug)
{
    public string Href => "/services#" + Slug;
}

/// <summary>The free text on the Services page.</summary>
public record ServicesPageText(
    string Headline,
    string Lede,
    string PosterPath,
    string AerialHeadline,
    string AerialBody,
    string FleetHeadline,
    string FleetBody,
    IReadOnlyList<string> FleetItems,
    string FleetImagePath,
    string FleetImageAlt,
    string FleetInsetImagePath,
    string FleetInsetImageAlt,
    string CaseStudyStat,
    string CaseStudyStatText,
    string CaseStudyCaption,
    string CtaImagePath);
