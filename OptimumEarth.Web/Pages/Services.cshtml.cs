using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class ServicesModel : PageModel
{
    public List<ServiceLine> Rows { get; } = new()
    {
        new("01", "Technical Studies", "Uganda · Zambia",
            "Know the ground before you commit capital. Good projects start with good data. We plan, collect and interpret the field and desk studies that reduce risk at concept, feasibility and bankability stage, whether the project is a single borehole or a 330 kV transmission corridor. Our geoscientists, engineers, surveyors, hydrologists and GIS specialists work in-house, so their findings go straight into design without gaps.",
            "Technical Studies",
            new List<string>
            {
                "Hydrogeological surveys, borehole drilling, test pumping and groundwater modelling",
                "Surface water hydrology, hydraulic modelling and flood risk assessment",
                "Geophysical surveys: electrical resistivity (VES, ERT), seismic refraction and MASW, GPR, and downhole logging",
                "Geotechnical investigations: drilling, sampling, laboratory testing and slope stability",
                "Land, topographic and cadastral surveys, and drone mapping",
                "GIS, remote sensing, spatial analytics and mobile field data systems",
                "Groundwater and water quality monitoring networks, including real-time satellite telemetry",
                "Feasibility studies for water, energy and mining projects"
            }),
        new("02", "Engineering Design", "Uganda · Zambia",
            "Designs built on real site data. We turn study findings into designs that can be priced, permitted and built. Because investigation and design sit in the same team, every design assumption is tested against measured site conditions rather than taken from a desk study. Our designs carry through to tender documents and construction supervision, so one team is accountable from first calculation to handover.",
            "Engineering Design",
            new List<string>()
            {
                "Water supply systems: production boreholes, pumping mains, reservoirs, elevated tanks and distribution networks",
                "Solar-powered pumping and irrigation systems",
                "Water treatment and sanitation systems",
                "Drainage, stormwater and flood protection works",
                "Utility-scale and commercial solar PV design, and grid integration studies",
                "Transmission line routing and substation design support",
                "Roads, haul roads, foundations, retaining structures and site civil works",
                "Mine water management and dewatering systems",
                "Bills of quantities, specifications, tender documents and owner's engineer services"
            }),
        new("03", "Construction Services", "Uganda · Zambia",
            "Built by the team that designed it. We build, install and commission with our own crews and equipment, and we supervise works for clients who build with other contractors. Every contract includes HSE management, quality control and as-built records as standard. We are accountable for the result, not just the activity.",
            "Construction Services",
            new List<string>()
            {
                "Borehole drilling, construction and rehabilitation, and monitoring well installation",
                "Pipelines, pump stations, storage tanks and complete water supply schemes",
                "Solar pumping installation, testing and commissioning",
                "Earthworks, trenching, concrete and structural works, drainage and culverts",
                "Water treatment plant installation",
                "Downhole camera inspection and borehole diagnostics",
                "Construction supervision, quality assurance and contract administration",
                "Plant and equipment hire, with or without operators"
            }),
        new("04", "Environmental & Social Impact Assessments", "Uganda · Zambia",
            "Approvals that satisfy both the regulator and the lender. We take projects through the full environmental and social compliance cycle, from screening to closure. Our work meets national requirements (NEMA, ZEMA) and the international lender safeguards that financiers apply. Hydrogeology, hydrology and GIS are in-house, so the specialist baseline studies that most ESIAs subcontract are done by the same team that writes the assessment.",
            "Environmental & Social Impact Assessments",
            new List<string>()
            {
                "Screening, scoping and project briefs",
                "Full ESIAs and environmental impact statements",
                "Specialist baselines: groundwater, surface water, water quality, soils, air, noise, biodiversity and social",
                "Environmental and social management plans, and environmental and social due diligence",
                "Compliance audits and environmental monitoring programmes",
                "Stakeholder engagement, public consultation and grievance mechanisms",
                "Resettlement planning support, mine closure and rehabilitation planning"
            }),
        new("05", "Climate Resilience & Natural Resources", "Uganda · Zambia",
            "Planning water and land for the climate ahead. Climate change is changing how much water is available, when rain falls and how land can be used. We help governments, development partners, conservation organisations and agribusiness plan at the scale these changes happen: the catchment, the district, the landscape. We combine satellite data, hydrological modelling and field verification to show what resources exist, how they are changing and how to manage them. Our work turns that evidence into plans, maps and investments that communities and institutions can act on.",
            "Climate Resilience & Natural Resources",
            new List<string>()
            {
                "Water resources assessments and integrated water resources management (IWRM) master plans at catchment, district and basin scale",
                "Water balance modelling with climate and demand scenarios",
                "Climate risk and vulnerability assessments for water supply, agriculture and infrastructure",
                "Climate-smart agriculture: irrigation suitability mapping, solar-powered irrigation, rainwater harvesting, and soil and water conservation",
                "Land use and land cover change analysis from multi-date satellite imagery",
                "Resource management mapping for forests, wetlands, rangelands and catchments, including encroachment and degradation mapping",
                "Catchment management plans, and ecosystem-based adaptation and nature-based solutions such as wetland restoration, riparian buffers and reforestation",
                "Drought and flood monitoring systems and decision-support dashboards",
                "Technical inputs to climate finance proposals"
            }),
    };

    public void OnGet()
    {
    }
}
