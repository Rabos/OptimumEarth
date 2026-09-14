using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class IndexModel : PageModel
{
    public List<HeroSlide> HeroSlides { get; } = new()
    {
        new("Uganda hero", "Optimum Earth Uganda", "Sustainable infrastructure for East and Southern Africa.",
            "Water, energy, environment, GIS and engineering, delivered since 2017 from our Kampala base."),
        new("Zambia hero", "Optimum Earth Zambia", "Regional engineering capability, brought to the Zambian market.",
            "Energy, water, infrastructure and environmental services for mining, utilities and industry."),
        new("Foundation hero", "Optimum Earth Foundation", "Communities designing the infrastructure they need.",
            "Engineering capability channelled into community-led development, measured by results."),
    };

    public void OnGet()
    {
    }
}
