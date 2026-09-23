using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class ProjectsModel : PageModel
{
    public static readonly List<ProjectFilter> Filters = new()
    {
        new("all", "All"),
        new("uganda", "Uganda"),
        new("zambia", "Zambia"),
        new("burundi", "Burundi"),
        new("drc", "DRC"),
        new("construction", "Construction Services"),
        new("technical", "Technical Studies"),
        new("engineering", "Engineering Design"),
        new("environmental", "Environmental & Social Impact"),
        new("climate", "Climate Resilience"),
    };

    private static readonly List<ProjectCard> AllProjects = new()
    {
        new("Project photo", "UGANDA · 2026", "Kilyango & Kasinyi Borehole Programme", "Client: TotalEnergies EP Uganda", "uganda", "construction", "/img/optimum-earth-images/project1.jpg", "Geo-seismic and borehole process/reprocess works: hydrogeological survey, drilling, testing and installation of a new borehole, plus rehabilitation of an existing one."),
        new("Project photo", "UGANDA · 2025", "RAP 2-5 Compensation Boreholes", "Client: TotalEnergies EP Uganda", "uganda", "construction", "/img/optimum-earth-images/project2.jpg", "Drilling and equipping of six boreholes delivered under an in-kind resettlement compensation programme."),
        new("Project photo", "UGANDA · 2024-2025", "Replacement Boreholes for Project-Affected Persons", "Client: TotalEnergies EP Uganda", "uganda", "technical", "/img/optimum-earth-images/project3.jpg", "Hydrogeological studies for six replacement boreholes serving communities affected by oil-field development."),
        new("Project photo", "UGANDA · 2024", "Tilenga - Production Well Integrity Survey", "Client: ASTRO Energy & Mining", "uganda", "technical", "/img/optimum-earth-images/project4.jpg", "Survey, test pumping and borehole integrity assessment of production wells on the Tilenga oil development."),
        new("Project photo", "UGANDA · 2022-2023", "Groundwater Monitoring Network - Kingfisher Oilfield", "Client: CNOOC Uganda Limited", "uganda", "construction", "/img/optimum-earth-images/recentwork2.jpg", "Drilling of six groundwater monitoring boreholes across the Kingfisher Oil Field Development Area."),
        new("Project photo", "UGANDA · COMPLETED", "Hohwa Production Well", "Client: Daqing / Kikuube Oil Field", "uganda", "construction", "/img/optimum-earth-images/project6.jpg", "Construction of a production water well serving oil-field operations in Hohwa, Kikuube district."),
        new("Project photo", "UGANDA · 2026", "Groundwater Supply Development - Tilenga Industrial Area", "Client: Gauff Consultants Uganda Ltd", "uganda", "construction", "/img/optimum-earth-images/project7.jpg", "Drilling and later flushing of production wells supplying the Buliisa oil-industry support zone."),
        new("Project photo", "UGANDA · 2024", "Buliisa Borehole Integrity Assessment", "Client: ATRO Engineering", "uganda", "technical", "/img/optimum-earth-images/project8.jpg", "Test pumping and borehole integrity assessment of production boreholes supporting oil-field infrastructure."),
        new("Project photo", "UGANDA · 2024", "Orom Cross Graphite Mine - Community Water Programme", "Client: Consolidated African Resources Ltd / Blencowe Plc", "uganda", "technical", "/img/optimum-earth-images/project9.jpg", "Rehabilitation and upgrade of the locomo solar powered community water-system"),
        new("Project photo", "UGANDA · 2021-2022", "Makuutu Bankable Feasibility Study - Hydrogeology Programme", "Client: Rwenzori Rare Metals / Ionic Rare Earth", "uganda", "technical", "/img/optimum-earth-images/makuttu.jpg", "Hydrogeology programme delivered as part of the mine's Bankable Feasibility Study."),
        new("Project photo", "UGANDA · 2023", "Orom-Cross Graphite DFS Borefield Assessment", "Client: Consolidated African Resources Ltd / Blencowe Plc", "uganda", "technical", "/img/optimum-earth-images/project11.jpg", "Definitive Feasibility Study hydrological work and project borefield assessment for a graphite development. Including: drilling, aquifer testing, water quality assessments, hydrogeological design studies"),
        new("Project photo", "UGANDA · 2021-2022", "Hima Limestone Quarry Dewatering", "Client: Lafarge Holcim / Hima Cement Ltd", "uganda", "construction", "/img/optimum-earth-images/project12.jpg", "Mine dewatering and groundwater control, plus a new water-supply system for a limestone quarry."),
        new("Project photo", "UGANDA · 2022", "Dura Quarry Topographic Survey", "Client: Hima Cement Ltd", "uganda", "technical", "/img/optimum-earth-images/project13.jpg", "Topographic surveying in support of quarry operations and expansion planning."),
        new("Project photo", "UGANDA · 2021", "Hima Quarry Water Supply System", "Client: Hima Cement Ltd", "uganda", "construction", "/img/optimum-earth-images/hima-quarry.jpg", "Pipeline and powered water-supply infrastructure serving quarry operations."),
        new("Project photo", "UGANDA · 2022", "Quarry Site ERT Survey", "Client: Agaba South West Services", "uganda", "technical", "/img/optimum-earth-images/servicepanel4.jpg", "2D electrical resistivity tomography survey for bedrock and site assessment ahead of quarry development."),
        new("Project photo", "UGANDA · 2021", "Quarry Bedrock Characterisation Surveys", "Client: Private quarry operators", "uganda", "technical", "/img/optimum-earth-images/project15.jpg", "Geophysical surveys for bedrock characterisation at two quarry sites in Mukono district."),
        new("Project photo", "ZAMBIA · ONGOING", "50 MW Kasempa Solar PV Plant", "Client: Optimum Earth Engineering Ltd", "zambia", "technical", "/img/optimum-earth-images/project16.jpg", "Prefeasibility study complete; detailed feasibility study under way for a 50 MW utility-scale solar PV plant."),
        new("Project photo", "ZAMBIA · 2026 (ONGOING)", "Pensulo-Mansa 330 kV Transmission Line Corridor", "Client: Optimum Earth Engineering Ltd / ZESCO Limited", "zambia", "technical", "/img/optimum-earth-images/project17.jpg", "Authorised by Zambia's Ministry of Energy to undertake feasibility studies for a 330 kV transmission corridor from Pensulo to Mansa, in collaboration with ZESCO Limited."),
        new("Project photo", "UGANDA · 2024", "Waste-to-Energy Plant Site Investigation", "Client: NLS Waste Services Ltd", "uganda", "technical", "/img/optimum-earth-images/project18.jpg", "Geotechnical and hydrological investigations for a proposed waste-to-energy generation plant."),
        new("Project photo", "BURUNDI · 2025", "Pilot Aquifer Recharge Pre-Feasibility Studies", "Client: Rivan Engineering Ltd / Nile Basin Initiative", "burundi", "technical", "/img/optimum-earth-images/project19.jpg", "Geophysical, geotechnical and topographic surveys for pre-feasibility studies of pilot managed aquifer recharge systems."),
        new("Project photo", "UGANDA · 2021-2022", "Solar Water Supply & Irrigation, Several Sites", "Client: EPM / Nexus Green Ltd / Ministry of Water", "uganda", "engineering", "/img/optimum-earth-images/project20.jpg", "Engineering design and construction supervision for solar-powered water supply and irrigation systems across up to 687 locations nationwide."),
        new("Project photo", "UGANDA · 2023", "Corporate Operations Environmental Review", "Client: NASECO", "uganda", "environmental", "/img/optimum-earth-images/project21.jpg", "Independent review of company operations and assessment of their environmental impact."),
        new("Project photo", "UGANDA · ONGOING", "Pivot Irrigation ESIA", "Client: NASECO", "uganda", "environmental", "/img/optimum-earth-images/enhancing-climate.jpg", "Environmental and Social Impact Assessment closure works for an irrigation development project."),
        new("Project photo", "NILE BASIN · COMPLETED", "Nile Basin Transboundary Groundwater Database", "Client: Centric Solutions Ltd", "uganda", "climate", "/img/optimum-earth-images/project23.jpg", "Development of an online groundwater database management system for transboundary aquifers across the Nile Basin."),
        new("Project photo", "UGANDA · 2022", "Namanve Wastewater Quality Testing & Monitoring", "Client: Lagan DOTT", "uganda", "environmental", "/img/optimum-earth-images/project24.jpg", "Wastewater quality monitoring programme supporting an industrial park."),
        new("Project photo", "DRC · COMPLETED", "Fire Protection System Installation", "Client: CAAMENIHU", "drc", "construction", "/img/optimum-earth-images/project25.jpg", "Design, supply and installation of a fire-protection system for an institutional facility."),
        new("Project photo", "UGANDA · 2024-2025", "Kiryandongo Solar-Powered Water Supply - Design & Build", "Client: DBS Establishments - Embassy of the UAE", "uganda", "construction", "/img/optimum-earth-images/project26.jpg", "Full design-and-build delivery of a solar-powered water-supply system serving a refugee settlement."),
        new("Project photo", "UGANDA · 2024", "Kasese Solar Borehole Development", "Client: Tulima Solar", "uganda", "technical", "/img/optimum-earth-images/project27.jpg", "Hydrogeological investigation ahead of drilling a solar-powered production well."),
        new("Project photo", "UGANDA · 2026", "National Surface Water Source Mapping", "Client: Tulima Solar", "uganda", "climate", "/img/optimum-earth-images/project28.jpg", "Surface water source mapping across five districts to guide solar-powered water infrastructure planning."),
        new("Project photo", "UGANDA · 2025-2026", "Solar-Powered Piped Water System - Feasibility & Design", "Client: Ibanda District Local Government", "uganda", "engineering", "/img/optimum-earth-images/project29.jpg", "Feasibility study and detailed engineering design for a solar-powered piped water supply scheme."),
        new("Project photo", "UGANDA · 2023-2024", "National Utility Hydrogeological Survey Programme", "Client: National Water & Sewerage Corporation (NWSC)", "uganda", "technical", "/img/optimum-earth-images/project30.jpg", "Hydrogeological survey for twelve production wells delivered across twelve NWSC regions nationwide."),
        new("Project photo", "UGANDA · 2025", "VIP Latrine Construction Programme", "Client: DBS Establishments - Embassy of the UAE", "uganda", "construction", "/img/optimum-earth-images/project31.jpg", "Design and construction of eight VIP latrine blocks serving a refugee settlement community."),
        new("Project photo", "UGANDA · 2020-2021", "Bwindi Eco School Water & Treatment System", "Client: Bwindi Eco School", "uganda", "construction", "/img/optimum-earth-images/project32.jpg", "Design and construction of a complete water-supply and treatment system for a school campus."),
        new("Project photo", "UGANDA · ONGOING", "Muhazi Legacy Estate Water Infrastructure", "Client: Eco-Lakes International", "uganda", "engineering", "/img/optimum-earth-images/project33.jpg", "Feasibility study, design and construction of water infrastructure for a lakeside estate development."),
        new("Project photo", "UGANDA · 2023", "Kyaliwajjala Water Supply Network Trenching", "Client: National Water & Sewerage Corporation (NWSC)", "uganda", "construction", "/img/optimum-earth-images/project34.jpg", "Trenching works for gate crossings on a piped water-distribution network."),
        new("Project photo", "UGANDA · 2019-2020", "Kako Hospital Water Programme", "Client: Engineers Without Borders", "uganda", "construction", "/img/optimum-earth-images/project35.jpg", "Borehole situation analysis, siting, drilling and construction delivering a reliable water source for a rural hospital."),
        new("Project photo", "UGANDA · 2020", "Ten-Village Borehole Programme", "Client: A Chance for Children", "uganda", "construction", "/img/optimum-earth-images/project36.jpg", "Construction of ten boreholes across rural villages under a framework arrangement."),
        new("Project photo", "UGANDA · 2021", "Borehole Rehabilitation Assessment", "Client: Windle Trust International", "uganda", "technical", "/img/optimum-earth-images/project37.jpg", "Rehabilitation and test pumping of boreholes serving a refugee-education programme."),
        new("Project photo", "UGANDA · 2021", "Data Centre Water Supply Borehole", "Client: Raxio Data Centre", "uganda", "construction", "/img/optimum-earth-images/project38.jpg", "Survey and construction of a dedicated production borehole for a regional data-centre facility."),
        new("Project photo", "UGANDA · 2024-2025", "Hospitality Sector Borehole Development", "Client: The Great Wall Hotel & Investments", "uganda", "construction", "/img/optimum-earth-images/project39.jpg", "Survey and drilling of production boreholes securing water supply for hospitality developments."),
        new("Project photo", "UGANDA · 2020", "Multi-District Borehole Casing Integrity Verification", "Client: NSI Water Ltd", "uganda", "technical", "/img/optimum-earth-images/project40.jpg", "Casing integrity verification and rehabilitation of four production boreholes across four districts."),
        new("Project photo", "UGANDA · 2021-2022", "Regional Borehole Performance Assessment", "Client: EPM Engineering Consults (U) Ltd", "uganda", "technical", "/img/optimum-earth-images/project41.jpg", "Test pumping of 47 boreholes and water-quality analysis across 75 sites region-wide."),
        new("Project photo", "UGANDA · 2021", "Nakivale Refugee Settlement Borehole Testing", "Client: Karf Aqua Engineering", "uganda", "technical", "/img/optimum-earth-images/project42.jpg", "Test pumping of boreholes supporting water supply to a major refugee settlement."),
        new("Project photo", "UGANDA · COMPLETED", "Borehole Construction & Water Permitting", "Client: The Xsabo Group", "uganda", "construction", "/img/optimum-earth-images/project43.jpg", "Borehole siting, drilling and water-use permitting services delivered under a repeat engagement."),
        new("Project photo", "UGANDA · 2020, 2022", "Mbale Industrial Park Water & Sewage Masterplan", "Client: Tangshan Mbale Industrial Park", "uganda", "engineering", "/img/optimum-earth-images/waterweb.jpg", "Preliminary feasibility studies and designs for a 5,000 m3/day water supply and 5,000 m3/day sewage system, followed by geotechnical, topographic and hydrological assessment."),
        new("Project photo", "UGANDA · 2023 & 2025", "St Joseph of Nazareth School Water Programme", "Client: St Joseph of Nazareth High School", "uganda", "construction", "/img/optimum-earth-images/project45.jpg", "Survey, drilling, test pumping, casing and hand-pump installation delivering safe water to a school community, in two phases."),
        new("Project photo", "UGANDA · 2026 (ONGOING)", "Kitukiro Rural Water Supply - Pipeline Trenching & Fabrication", "Client: Water Mission Uganda", "uganda", "construction", "/img/optimum-earth-images/project46.jpg", "Trenching and backfilling of a 27 km pipeline route, plus fabrication works, for a rural community water-supply scheme."),
        new("Project photo", "UGANDA · 2018-2026", "Buliisa & Hoima Regional Water Programme", "Client: Multiple institutional and private clients", "uganda", "construction", "/img/optimum-earth-images/borehole.jpg", "150+ borehole siting, drilling, test pumping and rehabilitation assignments completed since 2018 across households, schools, hospitals, hotels, refugee settlements and utilities nationwide."),
    };

    public string ActiveFilter { get; private set; } = "all";

    public List<ProjectCard> VisibleProjects { get; private set; } = AllProjects;

    public void OnGet(string? filter)
    {
        ActiveFilter = Filters.Any(f => f.Key == filter) ? filter! : "all";
        VisibleProjects = ActiveFilter == "all"
            ? AllProjects
            : AllProjects.Where(p => p.Country == ActiveFilter || p.Service == ActiveFilter).ToList();
    }
}
