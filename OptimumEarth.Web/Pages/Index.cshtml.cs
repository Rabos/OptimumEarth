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

    public List<CapabilityItem> Capabilities { get; } = new()
    {
        new("01", "Water resources", "Supply, diagnostics, monitoring"),
        new("02", "Energy", "Geothermal and infrastructure"),
        new("03", "Environment", "Assessment, audit, compliance"),
        new("04", "GIS & mapping", "Spatial analysis and mapping"),
        new("05", "Engineering", "Full project lifecycle"),
    };

    public List<ServicePanelContent> ServicePanels { get; } = new()
    {
        new("01 / 06", "Water supply solutions", "Surface and groundwater diagnostics, well troubleshooting and long-term supply planning.", "Water supply solutions", "left", "navy"),
        new("02 / 06", "Energy and geothermal", "Geothermal consulting and energy infrastructure support across the region.", "Energy and geothermal", "right", "white"),
        new("03 / 06", "Environment and sustainability", "Impact assessment, monitoring and compliance for major projects.", "Environment and sustainability", "left", "navy"),
        new("04 / 06", "GIS, mapping and remote sensing", "Geo-intelligence products for water, energy and humanitarian decision-making.", "GIS, mapping and remote sensing", "right", "white"),
        new("05 / 06", "Mines, oil and gas", "Integrated water management for mineral, metal and aggregate operations.", "Mines, oil and gas", "left", "navy"),
        new("06 / 06", "Engineering and construction support", "Full project lifecycle support, from planning to implementation.", "Engineering and construction support", "right", "white"),
    };

    public List<ProjectCard> RecentWork { get; } = new()
    {
        new("Project photo", "UGANDA · 2020", "Uganda Production Wells Survey", "National Water and Sewerage Corporation · Water supply solutions"),
        new("Project photo", "UGANDA · 2022", "Kingfisher Monitoring Wells", "CNOOC · Groundwater monitoring"),
        new("Project photo", "UGANDA · 2023", "Pivot Irrigation ESIA Initiative", "NASECO · Environmental and social impact assessment"),
    };

    public void OnGet()
    {
    }
}
