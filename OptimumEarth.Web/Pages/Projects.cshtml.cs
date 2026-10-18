using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages;

/// <summary>
/// The Projects page. Every project is rendered, and the filters (country, service,
/// sector, ongoing, search) are applied here too, so a filtered link or a page without
/// JavaScript shows the right cards; with JavaScript, site's projects.js filters in place.
/// </summary>
public class ProjectsModel : PageModel
{
    private readonly ContentReader _content;

    public ProjectsModel(ContentReader content)
    {
        _content = content;
    }

    public ProjectsPageContent Data { get; private set; } = null!;

    public HashSet<string> Countries { get; } = new();
    public HashSet<string> Services { get; } = new();
    public HashSet<string> Sectors { get; } = new();
    public bool OngoingOnly { get; private set; }
    public string Query { get; private set; } = string.Empty;
    public bool MapView { get; private set; }

    public IReadOnlyList<ProjectItem> Visible { get; private set; } = Array.Empty<ProjectItem>();

    public bool AnyFilter => Countries.Count + Services.Count + Sectors.Count > 0 || OngoingOnly || Query.Length > 0;

    // Older links (and the old Services page) used ?filter=<country or service>.
    public async Task OnGetAsync(string[]? country, string[]? service, string[]? sector, string? status, string? q, string? view, string? filter)
    {
        Data = await _content.ProjectsPageAsync();

        Add(Countries, country, ProjectCountries.All.Select(c => c.Key));
        Add(Services, service, ProjectServices.All.Select(s => s.Key));
        Add(Sectors, sector, Data.Sectors.Select(s => s.Key));
        if (filter is not null)
        {
            Add(Countries, new[] { filter }, ProjectCountries.All.Select(c => c.Key));
            Add(Services, new[] { filter }, ProjectServices.All.Select(s => s.Key));
        }

        OngoingOnly = status == "ongoing";
        Query = (q ?? string.Empty).Trim();
        MapView = view == "map";
        Visible = Data.Projects.Where(Matches).ToList();
    }

    private static void Add(HashSet<string> into, string[]? values, IEnumerable<string> allowed)
    {
        var known = allowed.ToHashSet();
        foreach (var value in (values ?? Array.Empty<string>()).SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)))
        {
            if (known.Contains(value)) into.Add(value);
        }
    }

    public bool Matches(ProjectItem p) => Matches(p, null);

    /// <summary>Whether a project passes every filter, optionally ignoring one group (for that group's counts).</summary>
    public bool Matches(ProjectItem p, string? ignore)
    {
        if (ignore != "country" && Countries.Count > 0 && !Countries.Contains(p.Country)) return false;
        if (ignore != "service" && Services.Count > 0 && !p.Services.Any(Services.Contains)) return false;
        if (ignore != "sector" && Sectors.Count > 0 && !Sectors.Contains(p.Sector)) return false;
        if (OngoingOnly && !p.Ongoing) return false;
        if (Query.Length > 0)
        {
            var hay = SearchText(p);
            if (!Query.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries).All(hay.Contains)) return false;
        }

        return true;
    }

    /// <summary>What the search box looks through: the same text on the server and in the browser.</summary>
    public string SearchText(ProjectItem p) =>
        string.Join(' ', p.Title, p.Client, p.Description, p.Location, ProjectCountries.NameOf(p.Country), SectorName(p.Sector)).ToLowerInvariant();

    public string SectorName(string key) => Data.Sectors.FirstOrDefault(s => s.Key == key)?.Name ?? string.Empty;

    public int CountFor(string group, string key) => Data.Projects.Count(p => Matches(p, group) && group switch
    {
        "country" => p.Country == key,
        "service" => p.Services.Contains(key),
        _ => p.Sector == key,
    });

    public int CountriesDelivered => Data.Projects.Select(p => p.Country).Where(c => c != "nilebasin").Distinct().Count();

    /// <summary>The data the browser needs for the detail panel and the map.</summary>
    public string ClientJson() => JsonSerializer.Serialize(new
    {
        countries = ProjectCountries.All.Select(c => new { key = c.Key, name = c.Name, lat = c.Lat, lng = c.Lng }),
        services = ProjectServices.All.Select(s => new { key = s.Key, label = s.Label, color = s.Color, anchor = s.Anchor }),
        sectors = Data.Sectors.Select(s => new { key = s.Key, name = s.Name }),
        projects = Data.Projects.Select(p => new
        {
            slug = p.Slug, title = p.Title, client = p.Client, country = p.Country, location = p.Location, sector = p.Sector,
            ongoing = p.Ongoing, services = p.Services, image = p.ImagePath, representative = p.RepresentativePhoto, description = p.Description,
            search = SearchText(p),
        }),
    }).Replace("</", "<\\/");
}
