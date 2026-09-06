using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class ProjectsModel : PageModel
{
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

    private static readonly List<ProjectCard> AllProjects = new()
    {
        new("Project photo", "UGANDA · 2020", "Uganda Production Wells Survey", "NWSC · Water supply solutions", "uganda", "water"),
        new("Project photo", "UGANDA · 2022", "Kingfisher Monitoring Wells", "CNOOC · Groundwater monitoring", "uganda", "groundwater"),
        new("Project photo", "UGANDA · 2023", "Pivot Irrigation ESIA Initiative", "NASECO · Environmental assessment", "uganda", "environment"),
        new("Project photo", "UGANDA", "Hima Quarry Water Management", "Hima Cement · Groundwater monitoring", "uganda", "groundwater"),
        new("Project photo", "UGANDA", "Makuutu Hydrogeology Programme", "Geothermal consulting · GIS & mapping", "uganda", "gis"),
        new("Project photo", "UGANDA", "Quarry Site ERT Survey", "2D ERT survey · 3 quarry site assessments", "uganda", "water"),
    };

    public string ActiveFilter { get; private set; } = "all";

    public List<ProjectCard> VisibleProjects { get; private set; } = AllProjects;

    public void OnGet(string? filter)
    {
        ActiveFilter = Filters.Any(f => f.Key == filter) ? filter! : "all";
        VisibleProjects = ActiveFilter == "all"
            ? AllProjects
            : AllProjects.Where(p => p.Country == ActiveFilter || p.Service == ActiveFilter).ToList();
    }
}
