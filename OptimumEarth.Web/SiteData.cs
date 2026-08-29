using OptimumEarth.Models;

namespace OptimumEarth;

/// <summary>
/// Static content for the site, translated from the original site's
/// COUNTRIES / ALL_PROJECTS / SERVICE_ROWS data objects.
/// In a production app this would come from a database or CMS instead.
/// </summary>
public static class SiteData
{
    public static readonly List<ProjectItem> AllProjects = new()
    {
        new ProjectItem { Tag = "UGANDA · 2020", Title = "Uganda Production Wells Survey", Meta = "NWSC · Water supply solutions", Country = "uganda", Service = "water" },
        new ProjectItem { Tag = "UGANDA · 2022", Title = "Kingfisher Monitoring Wells", Meta = "CNOOC · Groundwater monitoring", Country = "uganda", Service = "groundwater" },
        new ProjectItem { Tag = "UGANDA · 2023", Title = "Pivot Irrigation ESIA Initiative", Meta = "NASECO · Environmental assessment", Country = "uganda", Service = "environment" },
        new ProjectItem { Tag = "UGANDA", Title = "Hima Quarry Water Management", Meta = "Hima Cement · Groundwater monitoring", Country = "uganda", Service = "groundwater" },
        new ProjectItem { Tag = "UGANDA", Title = "Makuutu Hydrogeology Programme", Meta = "Geothermal consulting · GIS & mapping", Country = "uganda", Service = "gis" },
        new ProjectItem { Tag = "UGANDA", Title = "Quarry Site ERT Survey", Meta = "2D ERT survey · 3 quarry site assessments", Country = "uganda", Service = "water" }
    };

    public static readonly Dictionary<string, CountryInfo> Countries = new()
    {
        ["uganda"] = new CountryInfo
        {
            Key = "uganda",
            Eyebrow = "OPTIMUM EARTH UGANDA",
            Headline = "Water, environment and engineering expertise, delivered in Uganda since 2017.",
            Sub = "Kampala-based teams supporting national utilities, energy operators, industry and development partners across the country.",
            Overview = "Uganda is our home market. From Kampala we deliver hydrogeology, borehole development, groundwater monitoring, environmental assessment and geo-intelligence for national utilities, oil and gas operators, cement and quarry industry and humanitarian organisations.",
            Overview2 = "Our Ugandan team combines field capability with in-house analysis, so surveys, testing and reporting run under one accountable contract – with the compliance documentation regulators and lenders expect.",
            Facts = new List<Fact>
            {
                new() { K = "Established", V = "2017" },
                new() { K = "Office", V = "Kampala" },
                new() { K = "Service lines", V = "5" },
                new() { K = "Sectors", V = "Water · Energy · Mining" }
            },
            ServicesTitle = "Services in Uganda",
            Services = new List<ServiceItem>
            {
                new() { T = "Water supply solutions", D = "Surface and groundwater diagnostics, borehole siting, drilling supervision and test pumping." },
                new() { T = "Groundwater monitoring", D = "Monitoring well networks, water-level and quality regimes, long-term reporting." },
                new() { T = "Environmental services", D = "ESIA, environmental audits and compliance support for regulated projects." },
                new() { T = "GIS & mapping", D = "Geo-intelligence products, remote sensing and spatial analysis for decision-making." },
                new() { T = "Mines, oil & gas", D = "Integrated water management for quarry, mining and petroleum operations." }
            },
            ProjectsTitle = "Featured projects",
            Projects = new List<CountryProject>
            {
                new() { Tag = "WATER SUPPLY · 2020", Title = "Uganda Production Wells Survey", Meta = "National Water and Sewerage Corporation · 13 production wells across multiple districts" },
                new() { Tag = "GROUNDWATER · 2022", Title = "Kingfisher Monitoring Wells", Meta = "CNOOC · Monitoring well design and installation, Albertine region" },
                new() { Tag = "ENVIRONMENT · 2023", Title = "Pivot Irrigation ESIA Initiative", Meta = "NASECO · Environmental and social impact assessment" }
            },
            ProjectsNote = "",
            CtaTitle = "Work with our Uganda team",
            Contact = "Roston House, Plot 56/57, P.O Box 200032, Kampala · +256 784 080551 · uganda@optimum-earth.com"
        },
        ["zambia"] = new CountryInfo
        {
            Key = "zambia",
            Eyebrow = "OPTIMUM EARTH ZAMBIA",
            Headline = "Bringing regional engineering and environmental capability to the Zambian market.",
            Sub = "Energy, water, infrastructure and environmental services delivered by the same technical team that operates across East and Southern Africa.",
            Overview = "Zambia is our emerging Southern Africa operation. We bring the hydrogeology, water management, environmental and geo-intelligence capability built over years of regional delivery to Zambian clients in mining, energy, utilities and development.",
            Overview2 = "Local partnerships and regulatory alignment are being established now, so early clients work directly with senior technical staff and a mobilised regional field team.",
            Facts = new List<Fact>
            {
                new() { K = "Status", V = "Establishing" },
                new() { K = "Office", V = "To be confirmed" },
                new() { K = "Service lines", V = "4" },
                new() { K = "Sectors", V = "Mining · Energy · Water" }
            },
            ServicesTitle = "Services in Zambia",
            Services = new List<ServiceItem>
            {
                new() { T = "Water supply solutions", D = "Groundwater assessment, borehole siting and supervision, supply planning for industry and institutions." },
                new() { T = "Energy & infrastructure", D = "Support to energy projects and the infrastructure that surrounds them." },
                new() { T = "Groundwater monitoring", D = "Monitoring networks and reporting regimes for mining and industrial operations." },
                new() { T = "GIS & mapping", D = "Spatial analysis, remote sensing and field data systems." }
            },
            ProjectsTitle = "Projects & pipeline",
            Projects = new List<CountryProject>
            {
                new() { Tag = "ZAMBIA", Title = "Project to be confirmed", Meta = "Case study to be published as evidence becomes available" },
                new() { Tag = "ZAMBIA", Title = "Project to be confirmed", Meta = "Case study to be published as evidence becomes available" },
                new() { Tag = "REGIONAL", Title = "Regional experience applies here", Meta = "Delivery record from Uganda and the wider region" }
            },
            ProjectsNote = "Zambia case studies will be published as projects are delivered and client permissions are confirmed.",
            CtaTitle = "Work with our Zambia team",
            Contact = "Office address to be confirmed · zambia@optimum-earth.com"
        }
    };

    public static readonly List<ServiceRow> ServiceRows = new()
    {
        new ServiceRow { N = "01", T = "Water supply solutions", Where = "Uganda · Zambia", D = "Surface water and groundwater diagnostics, borehole siting and supervision, test pumping, well rehabilitation and long-term supply planning for utilities, industry and institutions." },
        new ServiceRow { N = "02", T = "Groundwater monitoring", Where = "Uganda · Zambia", D = "Monitoring well design and installation, water level and quality regimes, data management and reporting that stands up to regulatory scrutiny." },
        new ServiceRow { N = "03", T = "Environmental services", Where = "Uganda", D = "Environmental and social impact assessment, audits, management plans and ongoing compliance support for regulated and lender-financed projects." },
        new ServiceRow { N = "04", T = "GIS, mapping & remote sensing", Where = "Uganda · Zambia", D = "Geo-intelligence products for water, energy, humanitarian relief and infrastructure – spatial analysis, basemaps, dashboards and field data systems." },
        new ServiceRow { N = "05", T = "Solutions for mines, oil & gas", Where = "Uganda · Zambia", D = "Integrated water management for mineral, metal and aggregate operations, plus geothermal consulting and hydrogeology for the extractives sector." }
    };

    public static readonly List<ProjectFilter> Filters = new()
    {
        new ProjectFilter { Key = "all", Label = "All" },
        new ProjectFilter { Key = "uganda", Label = "Uganda" },
        new ProjectFilter { Key = "zambia", Label = "Zambia" },
        new ProjectFilter { Key = "water", Label = "Water supply" },
        new ProjectFilter { Key = "groundwater", Label = "Groundwater monitoring" },
        new ProjectFilter { Key = "environment", Label = "Environment" },
        new ProjectFilter { Key = "gis", Label = "GIS & mapping" }
    };
}
