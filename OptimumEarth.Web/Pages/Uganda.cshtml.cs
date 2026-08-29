using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Models;

namespace OptimumEarth.Pages;

public class UgandaModel : PageModel
{
    public CountryInfo Country { get; private set; } = default!;

    public void OnGet()
    {
        Country = SiteData.Countries["uganda"];
    }
}
