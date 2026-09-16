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
        new("Project photo", "UGANDA · 2020", "Uganda Production Wells Survey", "Client : National Water and Sewerage Cooperation", "uganda", "water", "/img/optimum-earth-images/recentwork1.jpg"),
        new("Project photo", "UGANDA · 2022", "Kingfisher Monitoring Wells", "Client : CNOOC", "uganda", "groundwater", "/img/optimum-earth-images/recentwork2.jpg"),
        new("Project photo", "UGANDA · 2023", "Pivot Irrigation ESIA Initiative", "Client : NASECO", "uganda", "environment", "/img/optimum-earth-images/recentwork3.jpg"),
        new("Project photo", "UGANDA", "Hima Quarry Water Management", "Client : Hima Cement", "uganda", "groundwater", "/img/optimum-earth-images/hima-quarry.jpg"),
        new("Project photo", "UGANDA", "Makuutu Hydrogeology Programme", "Client : Geothermal consulting · GIS & mapping", "uganda", "gis", "/img/optimum-earth-images/makuttu.jpg"),
        new("Project photo", "UGANDA", "Quarry Site ERT Survey", "Client : Agaba South West Services", "uganda", "gis", "/img/optimum-earth-images/servicepanel4.jpg"),
        new("Project photo", "UGANDA", "Uganda Borehole Testing Initiative", "Client: EPM Engineering Consults(U) Ltd", "uganda", "water", "/img/optimum-earth-images/borehole.jpg"),
        new("Project photo", "UGANDA · 2021/2022", "Enhancing Climate Resilient WASH Initiatives", "Client: UNICEF / SGI – Studio Galli Ingegneria", "uganda", "groundwater", "/img/optimum-earth-images/enhancing-climate.jpg"),
        new("Project photo", "UGANDA · 2021/2022", "Pivot Irrigation Development Assessment", "Client: NASECO", "uganda", "gis", "/img/optimum-earth-images/pivot-irrigation.jpg"),
        new("Project photo", "UGANDA", "Mbale Industrial Infrastructure Consultancy", "Client: Tangshan Mbale Industrial Park", "uganda", "water", "/img/optimum-earth-images/waterweb.jpg"),
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
