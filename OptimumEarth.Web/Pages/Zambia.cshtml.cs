using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Models;

namespace OptimumEarth.Pages;

public class ZambiaModel : PageModel
{
    public CountryInfo Country { get; private set; } = default!;

    public void OnGet()
    {
        Country = SiteData.Countries["zambia"];
    }
}
