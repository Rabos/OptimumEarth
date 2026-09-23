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
        new("Project photo", "UGANDA · 2026", "Kilyango & Kasinyi Borehole Programme", "Client: TotalEnergies EP Uganda", "uganda", "construction", "/img/optimum-earth-images/project1.jpg"),
        new("Project photo", "UGANDA · 2025", "RAP 2-5 Compensation Boreholes", "Client: TotalEnergies EP Uganda", "uganda", "construction", "/img/optimum-earth-images/project2.jpg"),
        new("Project photo", "UGANDA · 2024-2025", "Replacement Boreholes for Project-Affected Persons", "Client: TotalEnergies EP Uganda", "uganda", "technical", "/img/optimum-earth-images/project3.jpg"),
        new("Project photo", "UGANDA · 2024", "Tilenga - Production Well Integrity Survey", "Client: ASTRO Energy & Mining", "uganda", "technical", "/img/optimum-earth-images/project4.jpg"),
        new("Project photo", "UGANDA · 2022-2023", "Groundwater Monitoring Network - Kingfisher Oilfield", "Client: CNOOC Uganda Limited", "uganda", "construction", "/img/optimum-earth-images/recentwork2.jpg"),
        new("Project photo", "UGANDA · COMPLETED", "Hohwa Production Well", "Client: Daqing / Kikuube Oil Field", "uganda", "construction", "/img/optimum-earth-images/project6.jpg"),
        new("Project photo", "UGANDA · 2026", "Groundwater Supply Development - Tilenga Industrial Area", "Client: Gauff Consultants Uganda Ltd", "uganda", "construction", "/img/optimum-earth-images/project7.jpg"),
        new("Project photo", "UGANDA · 2024", "Buliisa Borehole Integrity Assessment", "Client: ATRO Engineering", "uganda", "technical", "/img/optimum-earth-images/project8.jpg"),
        new("Project photo", "UGANDA · 2024", "Orom Cross Graphite Mine - Community Water Programme", "Client: Consolidated African Resources Ltd / Blencowe Plc", "uganda", "technical", "/img/optimum-earth-images/project9.jpg"),
        new("Project photo", "UGANDA · 2021-2022", "Makuutu Bankable Feasibility Study - Hydrogeology Programme", "Client: Rwenzori Rare Metals / Ionic Rare Earth", "uganda", "technical", "/img/optimum-earth-images/makuttu.jpg"),
        new("Project photo", "UGANDA · 2023", "Orom-Cross Graphite DFS Borefield Assessment", "Client: Consolidated African Resources Ltd / Blencowe Plc", "uganda", "technical", "/img/optimum-earth-images/project11.jpg"),
        new("Project photo", "UGANDA · 2021-2022", "Hima Limestone Quarry Dewatering", "Client: Lafarge Holcim / Hima Cement Ltd", "uganda", "construction", "/img/optimum-earth-images/project12.jpg"),
        new("Project photo", "UGANDA · 2022", "Dura Quarry Topographic Survey", "Client: Hima Cement Ltd", "uganda", "technical", "/img/optimum-earth-images/project13.jpg"),
        new("Project photo", "UGANDA · 2021", "Hima Quarry Water Supply System", "Client: Hima Cement Ltd", "uganda", "construction", "/img/optimum-earth-images/hima-quarry.jpg"),
        new("Project photo", "UGANDA · 2022", "Quarry Site ERT Survey", "Client: Agaba South West Services", "uganda", "technical", "/img/optimum-earth-images/servicepanel4.jpg"),
        new("Project photo", "UGANDA · 2021", "Quarry Bedrock Characterisation Surveys", "Client: Private quarry operators", "uganda", "technical", "/img/optimum-earth-images/project15.jpg"),
        new("Project photo", "ZAMBIA · ONGOING", "50 MW Kasempa Solar PV Plant", "Client: Optimum Earth Engineering Ltd", "zambia", "technical", "/img/optimum-earth-images/project16.jpg"),
        new("Project photo", "ZAMBIA · 2026 (ONGOING)", "Pensulo-Mansa 330 kV Transmission Line Corridor", "Client: Optimum Earth Engineering Ltd / ZESCO Limited", "zambia", "technical", "/img/optimum-earth-images/project17.jpg"),
        new("Project photo", "UGANDA · 2024", "Waste-to-Energy Plant Site Investigation", "Client: NLS Waste Services Ltd", "uganda", "technical", "/img/optimum-earth-images/project18.jpg"),
        new("Project photo", "BURUNDI · 2025", "Pilot Aquifer Recharge Pre-Feasibility Studies", "Client: Rivan Engineering Ltd / Nile Basin Initiative", "burundi", "technical", "/img/optimum-earth-images/project19.jpg"),
        new("Project photo", "UGANDA · 2021-2022", "Solar Water Supply & Irrigation, Several Sites", "Client: EPM / Nexus Green Ltd / Ministry of Water", "uganda", "engineering", "/img/optimum-earth-images/project20.jpg"),
        new("Project photo", "UGANDA · 2023", "Corporate Operations Environmental Review", "Client: NASECO", "uganda", "environmental", "/img/optimum-earth-images/project21.jpg"),
        new("Project photo", "UGANDA · ONGOING", "Pivot Irrigation ESIA", "Client: NASECO", "uganda", "environmental", "/img/optimum-earth-images/enhancing-climate.jpg"),
        new("Project photo", "NILE BASIN · COMPLETED", "Nile Basin Transboundary Groundwater Database", "Client: Centric Solutions Ltd", "uganda", "climate", "/img/optimum-earth-images/project23.jpg"),
        new("Project photo", "UGANDA · 2022", "Namanve Wastewater Quality Testing & Monitoring", "Client: Lagan DOTT", "uganda", "environmental", "/img/optimum-earth-images/project24.jpg"),
        new("Project photo", "DRC · COMPLETED", "Fire Protection System Installation", "Client: CAAMENIHU", "drc", "construction", "/img/optimum-earth-images/project25.jpg"),
        new("Project photo", "UGANDA · 2024-2025", "Kiryandongo Solar-Powered Water Supply - Design & Build", "Client: DBS Establishments - Embassy of the UAE", "uganda", "construction", "/img/optimum-earth-images/project26.jpg"),
        new("Project photo", "UGANDA · 2024", "Kasese Solar Borehole Development", "Client: Tulima Solar", "uganda", "technical", "/img/optimum-earth-images/project27.jpg"),
        new("Project photo", "UGANDA · 2026", "National Surface Water Source Mapping", "Client: Tulima Solar", "uganda", "climate", "/img/optimum-earth-images/project28.jpg"),
        new("Project photo", "UGANDA · 2025-2026", "Solar-Powered Piped Water System - Feasibility & Design", "Client: Ibanda District Local Government", "uganda", "engineering", "/img/optimum-earth-images/project29.jpg"),
        new("Project photo", "UGANDA · 2023-2024", "National Utility Hydrogeological Survey Programme", "Client: National Water & Sewerage Corporation (NWSC)", "uganda", "technical", "/img/optimum-earth-images/project30.jpg"),
        new("Project photo", "UGANDA · 2025", "VIP Latrine Construction Programme", "Client: DBS Establishments - Embassy of the UAE", "uganda", "construction", "/img/optimum-earth-images/project31.jpg"),
        new("Project photo", "UGANDA · 2020-2021", "Bwindi Eco School Water & Treatment System", "Client: Bwindi Eco School", "uganda", "construction", "/img/optimum-earth-images/project32.jpg"),
        new("Project photo", "UGANDA · ONGOING", "Muhazi Legacy Estate Water Infrastructure", "Client: Eco-Lakes International", "uganda", "engineering", "/img/optimum-earth-images/project33.jpg"),
        new("Project photo", "UGANDA · 2023", "Kyaliwajjala Water Supply Network Trenching", "Client: National Water & Sewerage Corporation (NWSC)", "uganda", "construction", "/img/optimum-earth-images/project34.jpg"),
        new("Project photo", "UGANDA · 2019-2020", "Kako Hospital Water Programme", "Client: Engineers Without Borders", "uganda", "construction", "/img/optimum-earth-images/project35.jpg"),
        new("Project photo", "UGANDA · 2020", "Ten-Village Borehole Programme", "Client: A Chance for Children", "uganda", "construction", "/img/optimum-earth-images/project36.jpg"),
        new("Project photo", "UGANDA · 2021", "Borehole Rehabilitation Assessment", "Client: Windle Trust International", "uganda", "technical", "/img/optimum-earth-images/project37.jpg"),
        new("Project photo", "UGANDA · 2021", "Data Centre Water Supply Borehole", "Client: Raxio Data Centre", "uganda", "construction", "/img/optimum-earth-images/project38.jpg"),
        new("Project photo", "UGANDA · 2024-2025", "Hospitality Sector Borehole Development", "Client: The Great Wall Hotel & Investments", "uganda", "construction", "/img/optimum-earth-images/project39.jpg"),
        new("Project photo", "UGANDA · 2020", "Multi-District Borehole Casing Integrity Verification", "Client: NSI Water Ltd", "uganda", "technical", "/img/optimum-earth-images/project40.jpg"),
        new("Project photo", "UGANDA · 2021-2022", "Regional Borehole Performance Assessment", "Client: EPM Engineering Consults (U) Ltd", "uganda", "technical", "/img/optimum-earth-images/project41.jpg"),
        new("Project photo", "UGANDA · 2021", "Nakivale Refugee Settlement Borehole Testing", "Client: Karf Aqua Engineering", "uganda", "technical", "/img/optimum-earth-images/project42.jpg"),
        new("Project photo", "UGANDA · COMPLETED", "Borehole Construction & Water Permitting", "Client: The Xsabo Group", "uganda", "construction", "/img/optimum-earth-images/project43.jpg"),
        new("Project photo", "UGANDA · 2020, 2022", "Mbale Industrial Park Water & Sewage Masterplan", "Client: Tangshan Mbale Industrial Park", "uganda", "engineering", "/img/optimum-earth-images/waterweb.jpg"),
        new("Project photo", "UGANDA · 2023 & 2025", "St Joseph of Nazareth School Water Programme", "Client: St Joseph of Nazareth High School", "uganda", "construction", "/img/optimum-earth-images/project45.jpg"),
        new("Project photo", "UGANDA · 2026 (ONGOING)", "Kitukiro Rural Water Supply - Pipeline Trenching & Fabrication", "Client: Water Mission Uganda", "uganda", "construction", "/img/optimum-earth-images/project46.jpg"),
        new("Project photo", "UGANDA · 2018-2026", "Buliisa & Hoima Regional Water Programme", "Client: Multiple institutional and private clients", "uganda", "construction", "/img/optimum-earth-images/borehole.jpg"),
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
