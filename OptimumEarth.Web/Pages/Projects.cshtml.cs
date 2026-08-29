using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Models;

namespace OptimumEarth.Pages;

public class ProjectsModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Filter { get; set; } = "all";

    public List<ProjectFilter> Filters { get; private set; } = new();
    public List<ProjectItem> VisibleProjects { get; private set; } = new();

    public void OnGet()
    {
        Filters = SiteData.Filters;

        VisibleProjects = string.IsNullOrEmpty(Filter) || Filter == "all"
            ? SiteData.AllProjects
            : SiteData.AllProjects
                .Where(p => p.Country == Filter || p.Service == Filter)
                .ToList();
    }
}
