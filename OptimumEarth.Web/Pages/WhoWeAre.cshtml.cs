using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class WhoWeAreModel : PageModel
{
    public List<CapabilityItem> Capabilities { get; } = new()
    {
        new("01", "Water resources", "Supply, diagnostics, monitoring"),
        new("02", "Energy", "Geothermal and infrastructure"),
        new("03", "Environment", "Assessment, audit, compliance"),
        new("04", "GIS & mapping", "Spatial analysis and mapping"),
        new("05", "Engineering", "Full project lifecycle"),
    };

    public void OnGet()
    {
    }
}
