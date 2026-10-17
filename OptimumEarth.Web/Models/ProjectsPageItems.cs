namespace OptimumEarth.Web.Models;

/// <summary>A service line as projects are tagged with it: its colour on the cards and its section on the Services page.</summary>
public record ProjectService(string Key, string Label, string Color, string Anchor);

public static class ProjectServices
{
    public static readonly IReadOnlyList<ProjectService> All = new ProjectService[]
    {
        new("technical", "Technical Studies", "#214292", "technical-studies"),
        new("engineering", "Engineering Design", "#14808F", "engineering-design"),
        new("construction", "Construction Services", "#C46D14", "construction"),
        new("environmental", "Environmental & Social", "#3B8A4A", "esia"),
        new("climate", "Climate Resilience", "#7D8F1F", "climate-resilience"),
    };

    public static ProjectService? Find(string key) => All.FirstOrDefault(s => s.Key == key);
}

/// <summary>One project as the Projects page shows it: a card, a map entry and a detail panel.</summary>
public record ProjectItem(
    string Slug,
    string Title,
    string Client,
    string Country,
    string Location,
    string Sector,
    bool Ongoing,
    IReadOnlyList<string> Services,
    string ImagePath,
    bool RepresentativePhoto,
    string Description,
    string FlagshipFigure,
    string FlagshipLabel,
    int FlagshipOrder);

public record SectorOption(string Key, string Name);

public record HeroPhoto(string Kicker, string Caption, string ImagePath);

/// <summary>The free text on the Projects page.</summary>
public record ProjectsPageText(string Headline, string Lede, string StatFigure, string StatLabel, string CtaImagePath);

public static class ProjectSlugs
{
    /// <summary>A project's address on the page (/projects#slug), made from its title.</summary>
    public static string From(string title)
    {
        var slug = System.Text.RegularExpressions.Regex.Replace(title.ToLowerInvariant().Replace("&", "and"), "[^a-z0-9]+", "-").Trim('-');
        return slug.Length > 80 ? slug[..80].TrimEnd('-') : slug;
    }
}
