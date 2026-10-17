namespace OptimumEarth.Web.Models;

/// <summary>A country projects can be filed under. The map pins every project at its country's point.</summary>
public record ProjectCountry(string Key, string Name, double Lat, double Lng);

/// <summary>
/// The countries the Projects page filters and maps by, in display order. A project's
/// Country is one of these keys. The points are roughly central and only indicative;
/// "Nile Basin" is the regional programme, pinned at Lake Tana so it stands apart from Uganda.
/// </summary>
public static class ProjectCountries
{
    public static readonly IReadOnlyList<ProjectCountry> All = new ProjectCountry[]
    {
        new("uganda", "Uganda", 1.37, 32.29),
        new("zambia", "Zambia", -13.13, 27.85),
        new("burundi", "Burundi", -3.37, 29.92),
        new("drc", "DRC", -4.04, 21.76),
        new("southsudan", "South Sudan", 6.88, 31.31),
        new("nilebasin", "Nile Basin", 12.0, 37.3),
    };

    public static string NameOf(string key) => All.FirstOrDefault(c => c.Key == key)?.Name ?? key;
}
