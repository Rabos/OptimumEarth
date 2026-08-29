using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Models;

namespace OptimumEarth.Pages;

public class ServicesModel : PageModel
{
    public List<ServiceRow> ServiceRows { get; private set; } = new();

    public void OnGet()
    {
        ServiceRows = SiteData.ServiceRows;
    }
}
