namespace OptimumEarth.Web.Data;

/// <summary>
/// The site's content as it was hard-coded in the page models before the
/// dashboard existed, brought up to date with the content refresh from the
/// fork's pull request (new service lines with deliverables, the larger
/// project list with descriptions, the revised Who We Are copy and values,
/// the Uganda and Zambia pages, and the SDG approach documents). Inserted
/// once, into empty tables, so the site looks the same on day one and
/// everything after is edited through the dashboard.
/// </summary>
public static class SeedData
{
    private const string Img = "/img/optimum-earth-images/";

    /// <summary>The content version a freshly seeded database starts at. See <see cref="ContentUpgrades"/>.</summary>
    public const int CurrentContentVersion = 2;

    public static List<Slide> Slides() => new()
    {
        new() { Eyebrow = "Optimum Earth Uganda", Headline = "Sustainable infrastructure for East and Southern Africa.", Body = "Water, energy, environment, GIS and engineering, delivered since 2017 from our Kampala base.", ImagePath = Img + "heroslide1.jpg" },
        new() { Eyebrow = "Optimum Earth Zambia", Headline = "Regional engineering capability, brought to the Zambian market.", Body = "Energy, water, infrastructure and environmental services for mining, utilities and industry.", ImagePath = Img + "heroslide2.jpg" },
        new() { Eyebrow = "Optimum Earth Foundation", Headline = "Communities designing the infrastructure they need.", Body = "Engineering capability channelled into community-led development, measured by results.", ImagePath = Img + "heroslide3.jpg" },
    };

    public static List<Service> Services() => new()
    {
        new() { Title = "Technical Studies", Coverage = "Uganda · Zambia", ImagePath = Img + "services/technical-studies.jpg",
            Description = "Know the ground before you commit capital. Good projects start with good data. We plan, collect and interpret the field and desk studies that reduce risk at concept, feasibility and bankability stage, whether the project is a single borehole or a 330 kV transmission corridor. Our geoscientists, engineers, surveyors, hydrologists and GIS specialists work in-house, so their findings go straight into design without gaps.",
            Deliverables = new()
            {
                "Hydrogeological surveys, borehole drilling, test pumping and groundwater modelling",
                "Surface water hydrology, hydraulic modelling and flood risk assessment",
                "Geophysical surveys: electrical resistivity (VES, ERT), seismic refraction and MASW, GPR, and downhole logging",
                "Geotechnical investigations: drilling, sampling, laboratory testing and slope stability",
                "Land, topographic and cadastral surveys, and drone mapping",
                "GIS, remote sensing, spatial analytics and mobile field data systems",
                "Groundwater and water quality monitoring networks, including real-time satellite telemetry",
                "Feasibility studies for water, energy and mining projects",
            } },
        new() { Title = "Engineering Design", Coverage = "Uganda · Zambia", ImagePath = Img + "services/engineering-design.jpeg",
            Description = "Designs built on real site data. We turn study findings into designs that can be priced, permitted and built. Because investigation and design sit in the same team, every design assumption is tested against measured site conditions rather than taken from a desk study. Our designs carry through to tender documents and construction supervision, so one team is accountable from first calculation to handover.",
            Deliverables = new()
            {
                "Water supply systems: production boreholes, pumping mains, reservoirs, elevated tanks and distribution networks",
                "Solar-powered pumping and irrigation systems",
                "Water treatment and sanitation systems",
                "Drainage, stormwater and flood protection works",
                "Utility-scale and commercial solar PV design, and grid integration studies",
                "Transmission line routing and substation design support",
                "Roads, haul roads, foundations, retaining structures and site civil works",
                "Mine water management and dewatering systems",
                "Bills of quantities, specifications, tender documents and owner's engineer services",
            } },
        new() { Title = "Construction Services", Coverage = "Uganda · Zambia", ImagePath = Img + "services/construction-services.jpg",
            Description = "Built by the team that designed it. We build, install and commission with our own crews and equipment, and we supervise works for clients who build with other contractors. Every contract includes HSE management, quality control and as-built records as standard. We are accountable for the result, not just the activity.",
            Deliverables = new()
            {
                "Borehole drilling, construction and rehabilitation, and monitoring well installation",
                "Pipelines, pump stations, storage tanks and complete water supply schemes",
                "Solar pumping installation, testing and commissioning",
                "Earthworks, trenching, concrete and structural works, drainage and culverts",
                "Water treatment plant installation",
                "Downhole camera inspection and borehole diagnostics",
                "Construction supervision, quality assurance and contract administration",
                "Plant and equipment hire, with or without operators",
            } },
        new() { Title = "Environmental & Social Impact Assessments", Coverage = "Uganda · Zambia", ImagePath = Img + "services/environmental-and-social-impact-assessments.jpg",
            Description = "Approvals that satisfy both the regulator and the lender. We take projects through the full environmental and social compliance cycle, from screening to closure. Our work meets national requirements (NEMA, ZEMA) and the international lender safeguards that financiers apply. Hydrogeology, hydrology and GIS are in-house, so the specialist baseline studies that most ESIAs subcontract are done by the same team that writes the assessment.",
            Deliverables = new()
            {
                "Screening, scoping and project briefs",
                "Full ESIAs and environmental impact statements",
                "Specialist baselines: groundwater, surface water, water quality, soils, air, noise, biodiversity and social",
                "Environmental and social management plans, and environmental and social due diligence",
                "Compliance audits and environmental monitoring programmes",
                "Stakeholder engagement, public consultation and grievance mechanisms",
                "Resettlement planning support, mine closure and rehabilitation planning",
            } },
        new() { Title = "Climate Resilience & Natural Resources", Coverage = "Uganda · Zambia", ImagePath = Img + "services/climate-resilience-&-natural-resources.jpg",
            Description = "Planning water and land for the climate ahead. Climate change is changing how much water is available, when rain falls and how land can be used. We help governments, development partners, conservation organisations and agribusiness plan at the scale these changes happen: the catchment, the district, the landscape. We combine satellite data, hydrological modelling and field verification to show what resources exist, how they are changing and how to manage them. Our work turns that evidence into plans, maps and investments that communities and institutions can act on.",
            Deliverables = new()
            {
                "Water resources assessments and integrated water resources management (IWRM) master plans at catchment, district and basin scale",
                "Water balance modelling with climate and demand scenarios",
                "Climate risk and vulnerability assessments for water supply, agriculture and infrastructure",
                "Climate-smart agriculture: irrigation suitability mapping, solar-powered irrigation, rainwater harvesting, and soil and water conservation",
                "Land use and land cover change analysis from multi-date satellite imagery",
                "Resource management mapping for forests, wetlands, rangelands and catchments, including encroachment and degradation mapping",
                "Catchment management plans, and ecosystem-based adaptation and nature-based solutions such as wetland restoration, riparian buffers and reforestation",
                "Drought and flood monitoring systems and decision-support dashboards",
                "Technical inputs to climate finance proposals",
            } },
    };

    /// <summary>The projects shown on the Projects page, in the order they appear.</summary>
    public static List<Project> ListProjects() => new()
    {
        P("Kilyango & Kasinyi Borehole Programme", "UGANDA · 2026", "Client: TotalEnergies EP Uganda", "uganda", "construction", Img + "project1.jpg", "Geo-seismic and borehole process/reprocess works: hydrogeological survey, drilling, testing and installation of a new borehole, plus rehabilitation of an existing one."),
        P("RAP 2-5 Compensation Boreholes", "UGANDA · 2025", "Client: TotalEnergies EP Uganda", "uganda", "construction", Img + "project2.jpg", "Drilling and equipping of six boreholes delivered under an in-kind resettlement compensation programme."),
        P("Replacement Boreholes for Project-Affected Persons", "UGANDA · 2024-2025", "Client: TotalEnergies EP Uganda", "uganda", "technical", Img + "project3.jpg", "Hydrogeological studies for six replacement boreholes serving communities affected by oil-field development."),
        P("Tilenga - Production Well Integrity Survey", "UGANDA · 2024", "Client: ASTRO Energy & Mining", "uganda", "technical", Img + "project4.jpg", "Survey, test pumping and borehole integrity assessment of production wells on the Tilenga oil development."),
        P("Groundwater Monitoring Network - Kingfisher Oilfield", "UGANDA · 2022-2023", "Client: CNOOC Uganda Limited", "uganda", "construction", Img + "recentwork2.jpg", "Drilling of six groundwater monitoring boreholes across the Kingfisher Oil Field Development Area."),
        P("Hohwa Production Well", "UGANDA · COMPLETED", "Client: Daqing / Kikuube Oil Field", "uganda", "construction", Img + "project6.jpg", "Construction of a production water well serving oil-field operations in Hohwa, Kikuube district."),
        P("Groundwater Supply Development - Tilenga Industrial Area", "UGANDA · 2026", "Client: Gauff Consultants Uganda Ltd", "uganda", "construction", Img + "project7.jpg", "Drilling and later flushing of production wells supplying the Buliisa oil-industry support zone."),
        P("Buliisa Borehole Integrity Assessment", "UGANDA · 2024", "Client: ATRO Engineering", "uganda", "technical", Img + "project8.jpg", "Test pumping and borehole integrity assessment of production boreholes supporting oil-field infrastructure."),
        P("Orom Cross Graphite Mine - Community Water Programme", "UGANDA · 2024", "Client: Consolidated African Resources Ltd / Blencowe Plc", "uganda", "technical", Img + "project9.jpg", "Rehabilitation and upgrade of the locomo solar powered community water-system"),
        P("Makuutu Bankable Feasibility Study - Hydrogeology Programme", "UGANDA · 2021-2022", "Client: Rwenzori Rare Metals / Ionic Rare Earth", "uganda", "technical", Img + "makuttu.jpg", "Hydrogeology programme delivered as part of the mine's Bankable Feasibility Study."),
        P("Orom-Cross Graphite DFS Borefield Assessment", "UGANDA · 2023", "Client: Consolidated African Resources Ltd / Blencowe Plc", "uganda", "technical", Img + "project11.jpg", "Definitive Feasibility Study hydrological work and project borefield assessment for a graphite development. Including: drilling, aquifer testing, water quality assessments, hydrogeological design studies"),
        P("Hima Limestone Quarry Dewatering", "UGANDA · 2021-2022", "Client: Lafarge Holcim / Hima Cement Ltd", "uganda", "construction", Img + "project12.jpg", "Mine dewatering and groundwater control, plus a new water-supply system for a limestone quarry."),
        P("Dura Quarry Topographic Survey", "UGANDA · 2022", "Client: Hima Cement Ltd", "uganda", "technical", Img + "project13.jpg", "Topographic surveying in support of quarry operations and expansion planning."),
        P("Hima Quarry Water Supply System", "UGANDA · 2021", "Client: Hima Cement Ltd", "uganda", "construction", Img + "hima-quarry.jpg", "Pipeline and powered water-supply infrastructure serving quarry operations."),
        P("Quarry Site ERT Survey", "UGANDA · 2022", "Client: Agaba South West Services", "uganda", "technical", Img + "servicepanel4.jpg", "2D electrical resistivity tomography survey for bedrock and site assessment ahead of quarry development."),
        P("Quarry Bedrock Characterisation Surveys", "UGANDA · 2021", "Client: Private quarry operators", "uganda", "technical", Img + "project15.jpg", "Geophysical surveys for bedrock characterisation at two quarry sites in Mukono district."),
        P("50 MW Kasempa Solar PV Plant", "ZAMBIA · ONGOING", "Client: Optimum Earth Engineering Ltd", "zambia", "technical", Img + "project16.jpg", "Prefeasibility study complete; detailed feasibility study under way for a 50 MW utility-scale solar PV plant."),
        P("Pensulo-Mansa 330 kV Transmission Line Corridor", "ZAMBIA · 2026 (ONGOING)", "Client: Optimum Earth Engineering Ltd / ZESCO Limited", "zambia", "technical", Img + "project17.jpg", "Authorised by Zambia's Ministry of Energy to undertake feasibility studies for a 330 kV transmission corridor from Pensulo to Mansa, in collaboration with ZESCO Limited."),
        P("Waste-to-Energy Plant Site Investigation", "UGANDA · 2024", "Client: NLS Waste Services Ltd", "uganda", "technical", Img + "project18.jpg", "Geotechnical and hydrological investigations for a proposed waste-to-energy generation plant."),
        P("Pilot Aquifer Recharge Pre-Feasibility Studies", "BURUNDI · 2025", "Client: Rivan Engineering Ltd / Nile Basin Initiative", "burundi", "technical", Img + "project19.jpg", "Geophysical, geotechnical and topographic surveys for pre-feasibility studies of pilot managed aquifer recharge systems."),
        P("Solar Water Supply & Irrigation, Several Sites", "UGANDA · 2021-2022", "Client: EPM / Nexus Green Ltd / Ministry of Water", "uganda", "engineering", Img + "project20.jpg", "Engineering design and construction supervision for solar-powered water supply and irrigation systems across up to 687 locations nationwide."),
        P("Corporate Operations Environmental Review", "UGANDA · 2023", "Client: NASECO", "uganda", "environmental", Img + "project21.jpg", "Independent review of company operations and assessment of their environmental impact."),
        P("Pivot Irrigation ESIA", "UGANDA · ONGOING", "Client: NASECO", "uganda", "environmental", Img + "enhancing-climate.jpg", "Environmental and Social Impact Assessment closure works for an irrigation development project."),
        P("Nile Basin Transboundary Groundwater Database", "NILE BASIN · COMPLETED", "Client: Centric Solutions Ltd", "uganda", "climate", Img + "project23.jpg", "Development of an online groundwater database management system for transboundary aquifers across the Nile Basin."),
        P("Namanve Wastewater Quality Testing & Monitoring", "UGANDA · 2022", "Client: Lagan DOTT", "uganda", "environmental", Img + "project24.jpg", "Wastewater quality monitoring programme supporting an industrial park."),
        P("Fire Protection System Installation", "DRC · COMPLETED", "Client: CAAMENIHU", "drc", "construction", Img + "project25.jpg", "Design, supply and installation of a fire-protection system for an institutional facility."),
        P("Kiryandongo Solar-Powered Water Supply - Design & Build", "UGANDA · 2024-2025", "Client: DBS Establishments - Embassy of the UAE", "uganda", "construction", Img + "project26.jpg", "Full design-and-build delivery of a solar-powered water-supply system serving a refugee settlement."),
        P("Kasese Solar Borehole Development", "UGANDA · 2024", "Client: Tulima Solar", "uganda", "technical", Img + "project27.jpg", "Hydrogeological investigation ahead of drilling a solar-powered production well."),
        P("National Surface Water Source Mapping", "UGANDA · 2026", "Client: Tulima Solar", "uganda", "climate", Img + "project28.jpg", "Surface water source mapping across five districts to guide solar-powered water infrastructure planning."),
        P("Solar-Powered Piped Water System - Feasibility & Design", "UGANDA · 2025-2026", "Client: Ibanda District Local Government", "uganda", "engineering", Img + "project29.jpg", "Feasibility study and detailed engineering design for a solar-powered piped water supply scheme."),
        P("National Utility Hydrogeological Survey Programme", "UGANDA · 2023-2024", "Client: National Water & Sewerage Corporation (NWSC)", "uganda", "technical", Img + "project30.jpg", "Hydrogeological survey for twelve production wells delivered across twelve NWSC regions nationwide."),
        P("VIP Latrine Construction Programme", "UGANDA · 2025", "Client: DBS Establishments - Embassy of the UAE", "uganda", "construction", Img + "project31.jpg", "Design and construction of eight VIP latrine blocks serving a refugee settlement community."),
        P("Bwindi Eco School Water & Treatment System", "UGANDA · 2020-2021", "Client: Bwindi Eco School", "uganda", "construction", Img + "project32.jpg", "Design and construction of a complete water-supply and treatment system for a school campus."),
        P("Muhazi Legacy Estate Water Infrastructure", "UGANDA · ONGOING", "Client: Eco-Lakes International", "uganda", "engineering", Img + "project33.jpg", "Feasibility study, design and construction of water infrastructure for a lakeside estate development."),
        P("Kyaliwajjala Water Supply Network Trenching", "UGANDA · 2023", "Client: National Water & Sewerage Corporation (NWSC)", "uganda", "construction", Img + "project34.jpg", "Trenching works for gate crossings on a piped water-distribution network."),
        P("Kako Hospital Water Programme", "UGANDA · 2019-2020", "Client: Engineers Without Borders", "uganda", "construction", Img + "project35.jpg", "Borehole situation analysis, siting, drilling and construction delivering a reliable water source for a rural hospital."),
        P("Ten-Village Borehole Programme", "UGANDA · 2020", "Client: A Chance for Children", "uganda", "construction", Img + "project36.jpg", "Construction of ten boreholes across rural villages under a framework arrangement."),
        P("Borehole Rehabilitation Assessment", "UGANDA · 2021", "Client: Windle Trust International", "uganda", "technical", Img + "project37.jpg", "Rehabilitation and test pumping of boreholes serving a refugee-education programme."),
        P("Data Centre Water Supply Borehole", "UGANDA · 2021", "Client: Raxio Data Centre", "uganda", "construction", Img + "project38.jpg", "Survey and construction of a dedicated production borehole for a regional data-centre facility."),
        P("Hospitality Sector Borehole Development", "UGANDA · 2024-2025", "Client: The Great Wall Hotel & Investments", "uganda", "construction", Img + "project39.jpg", "Survey and drilling of production boreholes securing water supply for hospitality developments."),
        P("Multi-District Borehole Casing Integrity Verification", "UGANDA · 2020", "Client: NSI Water Ltd", "uganda", "technical", Img + "project40.jpg", "Casing integrity verification and rehabilitation of four production boreholes across four districts."),
        P("Regional Borehole Performance Assessment", "UGANDA · 2021-2022", "Client: EPM Engineering Consults (U) Ltd", "uganda", "technical", Img + "project41.jpg", "Test pumping of 47 boreholes and water-quality analysis across 75 sites region-wide."),
        P("Nakivale Refugee Settlement Borehole Testing", "UGANDA · 2021", "Client: Karf Aqua Engineering", "uganda", "technical", Img + "project42.jpg", "Test pumping of boreholes supporting water supply to a major refugee settlement."),
        P("Borehole Construction & Water Permitting", "UGANDA · COMPLETED", "Client: The Xsabo Group", "uganda", "construction", Img + "project43.jpg", "Borehole siting, drilling and water-use permitting services delivered under a repeat engagement."),
        P("Mbale Industrial Park Water & Sewage Masterplan", "UGANDA · 2020, 2022", "Client: Tangshan Mbale Industrial Park", "uganda", "engineering", Img + "waterweb.jpg", "Preliminary feasibility studies and designs for a 5,000 m3/day water supply and 5,000 m3/day sewage system, followed by geotechnical, topographic and hydrological assessment."),
        P("St Joseph of Nazareth School Water Programme", "UGANDA · 2023 & 2025", "Client: St Joseph of Nazareth High School", "uganda", "construction", Img + "project45.jpg", "Survey, drilling, test pumping, casing and hand-pump installation delivering safe water to a school community, in two phases."),
        P("Kitukiro Rural Water Supply - Pipeline Trenching & Fabrication", "UGANDA · 2026 (ONGOING)", "Client: Water Mission Uganda", "uganda", "construction", Img + "project46.jpg", "Trenching and backfilling of a 27 km pipeline route, plus fabrication works, for a rural community water-supply scheme."),
        P("Buliisa & Hoima Regional Water Programme", "UGANDA · 2018-2026", "Client: Multiple institutional and private clients", "uganda", "construction", Img + "borehole.jpg", "150+ borehole siting, drilling, test pumping and rehabilitation assignments completed since 2018 across households, schools, hospitals, hotels, refugee settlements and utilities nationwide."),
    };

    /// <summary>
    /// The six cards the Uganda and Zambia pages feature. They are projects in
    /// their own right but hidden from the Projects page, which has its own list.
    /// Their description is the text shown on the country page card.
    /// </summary>
    public static List<Project> CountryCardProjects() => new()
    {
        C("Uganda Production Wells Survey", "WATER SUPPLY · 2020", "Client : National Water and Sewerage Corporation", "uganda", "technical", Img + "recentwork1.jpg", "We conducted a survey of production wells for the National Water and Sewerage Corporation, assessing performance and recommending improvements to enhance water supply reliability."),
        C("Kingfisher Monitoring Wells", "GROUNDWATER · 2022", "Client : CNOOC", "uganda", "technical", Img + "recentwork2.jpg", "We designed and implemented a groundwater monitoring program for CNOOC's Kingfisher project, providing critical data for sustainable water resource management."),
        C("Pivot Irrigation ESIA Initiative", "ENVIRONMENT · 2023", "Client : NASECO", "uganda", "environmental", Img + "recentwork3.jpg", "We led the Environmental and Social Impact Assessment (ESIA) for NASECO's Pivot Irrigation Initiative, ensuring compliance with environmental regulations and promoting sustainable agricultural practices."),
        C("50MW Kasempa Solar Plant", "ZAMBIA", "Prefeasibility complete, detailed feasibility study under way. Visit www.kasempasolar.com", "zambia", "technical", Img + "project4.jpg", "Prefeasibility complete, detailed feasibility study under way. Visit www.kasempasolar.com"),
        C("Pensulo - Mansa 330kV Powerline Corridor", "ZAMBIA", "Authorisation to do feasibility studies granted by MoE Zambia", "zambia", "technical", Img + "project5.jpg", "Authorisation to do feasibility studies granted by MoE Zambia"),
        C("Partnerships with Ntux Investments Ltd - Zambia", "ZAMBIA", "Earthmoving equipment and transport.", "zambia", "construction", Img + "project6.jpg", "Earthmoving equipment and transport."),
    };

    private static Project P(string title, string tag, string meta, string country, string category, string image, string description) =>
        new() { Title = title, Tag = tag, Meta = meta, Country = country, Service = category, ImagePath = image, Description = description };

    private static Project C(string title, string tag, string meta, string country, string category, string image, string description) =>
        new() { Title = title, Tag = tag, Meta = meta, Country = country, Service = category, ImagePath = image, Description = description, OnListPage = false };

    public static List<Capability> Capabilities() => new()
    {
        new() { Title = "Technical Studies", Description = "Know the ground before you commit capital." },
        new() { Title = "Engineering Design", Description = "Designs built on real site data." },
        new() { Title = "Construction Services", Description = "Built by the team that designed it." },
        new() { Title = "Environmental & Social Impact Assessments", Description = "Approvals that satisfy both the regulator and the lender" },
        new() { Title = "Climate Resilience & Natural Resources", Description = "Planning water and land for the climate ahead." },
    };

    public static List<CoreValue> Values() => new()
    {
        new() { Title = "Safety", Description = "Safety comes first, always. Everyone is responsible for their own health and safety and that of their colleagues, the public and the communities where we work. No deadline, task or budget justifies compromising it, and anyone may stop work they believe is unsafe. We protect the dignity and wellbeing of everyone our work touches, especially children and vulnerable people." },
        new() { Title = "Collaboration", Description = "Better solutions are built together. We work with colleagues, clients, donors, communities, partners and government with respect, inclusion and openness. We value the contributions of a diverse and talented team and seek out people who are passionate, curious and committed to those we serve. We invest in one another's professional development." },
        new() { Title = "Accountability & Integrity", Description = "We act with honesty and the highest ethics in every dealing, commercial or charitable, and we do the right thing even when no one is watching. We do not tolerate bribery, fraud or undeclared conflicts of interest. We own our commitments, decisions and results, raise problems early, and answer to our clients, donors, regulators and the communities we serve." },
        new() { Title = "Learning & Innovation", Description = "We stay curious and challenge complacency. We learn from experience and from those we serve, apply appropriate technology, and continually adapt our solutions to meet changing needs." },
        new() { Title = "Excellence", Description = "We aim to do our work right the first time. From technical services and engineering delivery to community programmes and partnerships, we hold high standards of quality. We treat quality as a shared responsibility and deliver work that is reliable, measurable and fit for purpose." },
        new() { Title = "Stewardship", Description = "We take responsibility for everything entrusted to us: client assets, company resources, donor and grant funds, and the communities and environments where we work. We use resources responsibly, account for them transparently, create lasting value, and leave people, communities and the planet better than we found them." },
    };

    public const string Mission = "We are committed to value-driven partnerships with our clients by providing services of the highest quality, safety, and integrity while focusing on customer satisfaction and innovative solutions. We strive to be the company that clients want to work with and employees want to work for, by providing excellent results and rewarding careers.";

    public const string Vision = "To be the leading global engineering and environmental services provider, focused on creating value and success for our clients.";

    public static List<Pillar> Pillars() => new()
    {
        new() { Key = "access", SdgLabel = "SDG 6 & 7", Title = "Access",
            Description = "Expanding access to essential services and practical climate solutions, particularly clean water, clean energy and appropriate climate technologies.",
            Items = new() { "Water", "Clean Energy", "Climate Solutions" } },
        new() { Key = "resilience", SdgLabel = "SDG 13", Title = "Resilience",
            Description = "Strengthening the ability of communities and local actors to anticipate, respond to and recover from climate and development pressures.",
            Items = new() { "Community resilience & capacity strengthening", "Local leadership and capabilities", "Climate preparedness and adaptation" } },
        new() { Key = "impact", SdgLabel = "SDG 17", Title = "Impact",
            Description = "Connecting local solutions to partnerships, finance and knowledge so effective approaches can be sustained, adapted and scaled responsibly.",
            Items = new() { "Partnerships", "Finance & resource mobilisation", "Knowledge sharing & learning" } },
    };

    public static List<Principle> Principles() => new()
    {
        new() { Title = "Community-led", Slug = "community-led", Description = "We start with the lived realities, priorities and capabilities of the communities and local actors we work with, not with pre-set solutions." },
        new() { Title = "Evidence-informed", Slug = "evidence-informed", Description = "We draw on evidence, local knowledge and continuous learning to shape our decisions and improve our approach." },
        new() { Title = "Partnership-driven", Slug = "partnership-driven", Description = "We bring together communities, philanthropy, public institutions, private capital and technical actors around a shared goal." },
        new() { Title = "Practical and scalable", Slug = "practical-and-scalable", Description = "We focus on solutions that work in real settings, and that we can adapt or expand responsibly as they prove out." },
        new() { Title = "Locally grounded", Slug = "locally-grounded", Description = "We prioritise African leadership, local ownership and context-specific solutions in everything we do." },
        new() { Title = "Learning-oriented", Slug = "learning-oriented", Description = "We document what works and what doesn't, so our learning contributes to wider practice across the region." },
    };

    public static List<FocusArea> FocusAreas() => new[]
    {
        "Water & WASH", "Clean Energy", "Climate Action", "Community Resilience", "Partnerships & Finance", "Knowledge & Learning",
    }.Select(l => new FocusArea { Label = l }).ToList();

    public static List<SdgGoal> SdgGoals() => new()
    {
        new() { Number = "06", Label = "Clean Water & Sanitation", DocumentPath = "/docs/sdg/OEF_SDG6_APPROACH.docx.pdf" },
        new() { Number = "07", Label = "Affordable & Clean Energy", DocumentPath = "/docs/sdg/OEF_SDG7_APPROACH.docx.pdf" },
        new() { Number = "13", Label = "Climate Action", DocumentPath = "/docs/sdg/OEF_SDG13_APPROACH.docx.pdf" },
        new() { Number = "17", Label = "Partnerships for the Goals", DocumentPath = "/docs/sdg/OEF_SDG17_APPROACH.docx.pdf" },
    };

    public static List<Story> Stories() => new()
    {
        new() { Key = "communities", Title = "Communities", Summary = "Stories from the field, in the words of the people we work with.",
            Body = "Stories from the field, in the words of the people we work with. This space holds community voices, before/after and challenge/response stories, and photography from the field — the human face of OEF's work.",
            CoverPath = Img + "foundation2.jpg" },
        new() { Key = "partnerships", Title = "Partnerships", Summary = "A look at what we're building together.",
            Body = "A look at what we're building together. This space features project and partnership case studies, impact indicators and simple dashboards as data becomes available, and news or updates on active collaborations.",
            CoverPath = Img + "foundation3.jpg" },
        new() { Key = "learning", Title = "Learning", Summary = "Where OEF shares what it's discovering and who it's growing with.",
            Body = "Where OEF shares what it's discovering and who it's growing with. This space holds practical learning notes and insights, alongside our Academic & Learning Partnerships — joint research, student placements and fellowships, mentorship, and career-pathway stories connecting young African talent to climate, water and energy work.",
            CoverPath = Img + "foundation4.jpg" },
    };

    // The first image of each story is its cover, so the lightbox opens on the image the visitor clicked.
    public static List<GalleryImage> Gallery() => new()
    {
        G("communities", "Community water committee, Karamoja", "foundation2.jpg"),
        G("communities", "Planning a scheme with the households it will serve", "community-led.jpg"),
        G("communities", "First draw from the new borehole", "borehole.jpg"),
        G("communities", "Makuttu: the site before work began", "makuttu.jpg"),
        G("communities", "Households on the extended network", "foundation1.jpg"),
        G("partnerships", "Partners walking a proposed site together", "foundation3.jpg"),
        G("partnerships", "Joint delivery with local contractors", "partnership-driven.jpg"),
        G("partnerships", "Hima quarry: shared infrastructure works", "hima-quarry.jpg"),
        G("partnerships", "Pivot irrigation commissioned with growers", "pivot-irrigation.jpg"),
        G("partnerships", "Solar array sized against real demand", "enhancing-climate.jpg"),
        G("learning", "A learning session with partner students", "foundation4.jpg"),
        G("learning", "Documenting what worked and what did not", "learning-oriented.jpg"),
        G("learning", "Field measurement feeding the evidence base", "evidence-informed.jpg"),
        G("learning", "Local teams leading the technical work", "locally-grounded.jpg"),
        G("learning", "Testing an approach before scaling it", "practical-and-scalable.jpg"),
    };

    private static GalleryImage G(string story, string caption, string image) =>
        new() { StoryKey = story, Caption = caption, ImagePath = Img + image };

    public static List<BlogCategory> BlogCategories() => new()
    {
        new() { Name = "News", Slug = "news", Description = "Company and office announcements", SortOrder = 1 },
        new() { Name = "Field notes", Slug = "field-notes", Description = "Practical notes from site teams", SortOrder = 2 },
        new() { Name = "Insights", Slug = "insights", Description = "Longer pieces on methods and standards", SortOrder = 3 },
        new() { Name = "Foundation", Slug = "foundation", Description = "Updates from the Foundation", SortOrder = 4 },
    };

    public static CountryPage Uganda() => new()
    {
        Slug = "uganda", Name = "Uganda", Theme = "uganda",
        HeroImageLabel = "Uganda hero image",
        HeroEyebrow = "OPTIMUM EARTH UGANDA",
        HeroHeadline = "Water, environment and engineering expertise, delivered in Uganda since 2017.",
        HeroSub = "Kampala-based teams supporting national utilities, energy operators, industry and development partners across the country.",
        OverviewEyebrow = "COUNTRY OVERVIEW",
        OverviewQuote = "Uganda is our home market. From Kampala we deliver hydrogeology, borehole development, groundwater monitoring, environmental assessment and geo-intelligence for national utilities, oil and gas operators, cement and quarry industry and humanitarian organisations.",
        OverviewBody = "Our Ugandan team combines field capability with in-house analysis, so surveys, testing and reporting run under one accountable contract - with the compliance documentation regulators and lenders expect.",
        Facts = new() { "Established | 2017", "Office | Kampala", "Service lines | 5", "Sectors | Water · Energy · Mining · Infrastructure" },
        SectionOneTitle = "Services in Uganda", SectionOneLinkText = "All services", SectionOneLinkHref = "/services",
        SectionTwoTitle = "Featured projects", SectionTwoLinkText = "All projects", SectionTwoLinkHref = "/projects",
        SectionTwoNote = string.Empty,
        CtaTitle = "Work with our Uganda team",
        CtaBody = "Roston House, Plot 56/57, P.O Box 200032, Kampala · +256 784 080551 · uganda@optimum-earth.com",
        CtaServices = new() { "Water supply solutions", "Groundwater monitoring", "Environmental services", "GIS & mapping", "Mines, oil & gas" },
    };

    public static CountryPage Zambia() => new()
    {
        Slug = "zambia", Name = "Zambia", Theme = "zambia",
        HeroImageLabel = "Zambia hero image",
        HeroEyebrow = "OPTIMUM EARTH ZAMBIA",
        HeroHeadline = "Bringing regional engineering and environmental capability to the Zambian market.",
        HeroSub = "Energy, water, infrastructure and environmental services delivered by the same technical team that operates across East and Southern Africa.",
        OverviewEyebrow = "COUNTRY OVERVIEW",
        OverviewQuote = "Zambia is our emerging Southern Africa operation. We bring the hydrogeology, water management, environmental and geo-intelligence capability built over years of regional delivery to Zambian clients in mining, energy, utilities and development.",
        OverviewBody = "Local partnerships and regulatory alignment are being established now, so early clients work directly with senior technical staff and a mobilised regional field team.",
        Facts = new() { "Status | Established in 2026", "Office | Lusaka", "Service lines | 6", "Sectors | Mining · Energy · Water · Equipment Hire & Transportation" },
        SectionOneTitle = "Services in Zambia", SectionOneLinkText = "All services", SectionOneLinkHref = "/services",
        SectionTwoTitle = "Projects", SectionTwoLinkText = "All projects", SectionTwoLinkHref = "/projects",
        SectionTwoNote = "Zambia case studies will be published as projects are delivered and client permissions are confirmed.",
        CtaTitle = "Work with our Zambia team",
        CtaBody = "Office address to be confirmed · zambia@optimum-earth.com",
        CtaServices = new() { "Water supply solutions", "Energy & infrastructure", "Groundwater monitoring", "GIS & mapping" },
    };

    /// <summary>
    /// One service card on a country page: the service it links to (an index into
    /// <see cref="Services"/>, or null for a custom card), the wording used on that
    /// page, and an optional link that turns the card into a call-to-action.
    /// </summary>
    public sealed record ServiceCardSpec(int? ServiceIndex, string Title, string Text, string Url = "");

    public static List<ServiceCardSpec> UgandaServiceCards() => new()
    {
        new(0, "Technical studies", "Hydrogeological, geophysical, geotechnical and topographic surveys, groundwater modelling and feasibility studies that de-risk projects early."),
        new(1, "Engineering design", "Water supply, solar pumping, drainage, civil and energy infrastructure designs, from concept to tender documents."),
        new(2, "Construction services", "Borehole drilling, pipelines, pump stations and civil works, built by our own crews with full supervision and HSE management."),
        new(3, "ESIA and compliance", "Screening, impact assessments, management plans, audits and stakeholder engagement to NEMA and lender standards."),
        new(4, "Climate and natural resources", "Water resources master plans, climate-smart agriculture, and forest, wetland and catchment, mapping for resilient landscapes."),
        new(null, "Not sure which service you need?", "Describe your site and we'll scope it with you.", "/contact"),
    };

    public static List<ServiceCardSpec> ZambiaServiceCards() => new()
    {
        new(null, "Equipment hire and transportation", "Haul trucks, tippers, low-beds, excavators, graders and site support equipment, with or without operators, for mining and construction."),
        new(0, "Technical studies", "Hydrogeological, geophysical, geotechnical and topographic surveys, and feasibility studies for energy, water and mining projects."),
        new(1, "Engineering design", "Solar PV, transmission and substation support, water supply and civil infrastructure designs, from concept to tender documents."),
        new(2, "Construction services", "Borehole drilling, pipelines, pump stations, earthworks and civil works, with full supervision and HSE management."),
        new(3, "ESIA and compliance", "Screening, impact assessments, management plans, audits and stakeholder engagement to ZEMA and lender standards."),
        new(4, "Climate and natural resources", "Water resources master plans, climate-smart agriculture, and forest, wetland and catchment mapping for resilient landscapes."),
        new(null, "Not sure which service you need?", "Describe your site and we'll scope it with you.", "/contact"),
    };
}
