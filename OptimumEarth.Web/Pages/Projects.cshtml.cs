using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages;

public class ProjectsModel : PageModel
{
    private readonly ContentReader _content;

    public ProjectsModel(ContentReader content)
    {
        _content = content;
    }

    public static readonly List<ProjectFilter> Filters = new()
    {
        new("all", "All"),
        new("uganda", "Uganda"),
        new("zambia", "Zambia"),
        new("water", "Water supply"),
        new("groundwater", "Groundwater monitoring"),
        new("environment", "Environment"),
        new("gis", "GIS & mapping"),
    };

    public string ActiveFilter { get; private set; } = "all";

    public IReadOnlyList<ProjectCard> VisibleProjects { get; private set; } = Array.Empty<ProjectCard>();

    public async Task OnGetAsync(string? filter)
    {
        var all = await _content.ProjectCardsAsync();
        ActiveFilter = Filters.Any(f => f.Key == filter) ? filter! : "all";
        VisibleProjects = ActiveFilter == "all"
            ? all
            : all.Where(p => p.Country == ActiveFilter || p.Service == ActiveFilter).ToList();
    }
}
