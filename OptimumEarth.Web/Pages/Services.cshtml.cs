using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class ServicesModel : PageModel
{
    public List<ServiceLine> Rows { get; } = new()
    {
        new("01", "Water supply solutions", "Uganda · Zambia",
            "Surface water and groundwater diagnostics, borehole siting and supervision, test pumping, well rehabilitation and long-term supply planning for utilities, industry and institutions.",
            "Water supply solutions"),
        new("02", "Groundwater monitoring", "Uganda · Zambia",
            "Monitoring well design and installation, water level and quality regimes, data management and reporting that stands up to regulatory scrutiny.",
            "Groundwater monitoring"),
        new("03", "Environmental services", "Uganda",
            "Environmental and social impact assessment, audits, management plans and ongoing compliance support for regulated and lender-financed projects.",
            "Environmental services"),
        new("04", "GIS, mapping & remote sensing", "Uganda · Zambia",
            "Geo-intelligence products for water, energy, humanitarian relief and infrastructure — spatial analysis, basemaps, dashboards and field data systems.",
            "GIS, mapping and remote sensing"),
        new("05", "Solutions for mines, oil & gas", "Uganda · Zambia",
            "Integrated water management for mineral, metal and aggregate operations, plus geothermal consulting and hydrogeology for the extractives sector.",
            "Mines, oil and gas"),
    };

    public void OnGet()
    {
    }
}
