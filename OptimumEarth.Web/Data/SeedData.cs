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
    public const int CurrentContentVersion = 7;

    public static List<Slide> Slides() => new()
    {
        new() { Eyebrow = "Optimum Earth Uganda", Headline = "Sustainable infrastructure for East and Southern Africa.", Body = "Water, energy, environment, GIS and engineering, delivered since 2017 from our Kampala base.", ImagePath = Img + "heroslide1.jpg" },
        new() { Eyebrow = "Optimum Earth Zambia", Headline = "Regional engineering capability, brought to the Zambian market.", Body = "Energy, water, infrastructure and environmental services for mining, utilities and industry.", ImagePath = Img + "heroslide2.jpg" },
        new() { Eyebrow = "Optimum Earth Foundation", Headline = "Communities designing the infrastructure they need.", Body = "Engineering capability channelled into community-led development, measured by results.", ImagePath = Img + "heroslide3.jpg" },
    };

    private const string Field = Img + "field/";

    public static List<Service> Services() => new()
    {
        new() { Title = "Technical Studies", ShortTitle = "Technical Studies", Slug = "technical-studies", Coverage = "Uganda · Zambia", Layout = "split",
            Tagline = "Know the ground before you commit capital.",
            Description = "We plan, collect and interpret the field and desk studies that reduce risk at concept, feasibility and bankability stage, from a single borehole to a 330 kV transmission corridor. Our specialists are in-house, so findings go straight into design.",
            ImagePath = Field + "mining-geology.jpg", TileImagePath = Field + "ves-survey.jpg",
            TileItems = new() { "Hydrogeology & groundwater modelling", "Geophysics: VES, ERT, MASW, GPR", "Geotechnical & survey" },
            Highlights = new()
            {
                "Hydrogeological surveys, drilling, test pumping & modelling",
                "Geophysics: VES, ERT, seismic refraction, MASW, GPR, logging",
                "Geotechnical investigations & slope stability",
                "GIS, remote sensing & drone mapping",
            },
            StatValue = "12 regions", StatLabel = "NWSC production well surveys, nationwide",
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
        new() { Title = "Engineering Design", ShortTitle = "Engineering Design", Slug = "engineering-design", Coverage = "Uganda · Zambia", Layout = "split-alt",
            Tagline = "Designs built on real site data.",
            Description = "We turn study findings into designs that can be priced, permitted and built. Every assumption is tested against measured site conditions, and one team stays accountable from first calculation to handover.",
            ImagePath = Field + "tank-stand-media.jpg", TileImagePath = Field + "tank-stand-p.jpg",
            TileItems = new() { "Water supply & treatment", "Solar PV & solar pumping", "Transmission & substation support" },
            Highlights = new()
            {
                "Water supply: boreholes, mains, reservoirs & networks",
                "Solar PV, solar pumping & grid integration",
                "Transmission routing & substation design support",
                "BoQs, specifications & owner's engineer services",
            },
            StatValue = "79 sites", StatLabel = "Solar water supply and irrigation systems designed, Uganda",
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
        new() { Title = "Construction Services", ShortTitle = "Construction Services", Slug = "construction", Coverage = "Uganda · Zambia", Layout = "band",
            Tagline = "Built by the team that designed it.",
            Description = "Our own crews and equipment build, install and commission, and we supervise works for clients who build with other contractors. HSE management, quality control and as-built records are standard on every contract.",
            ImagePath = Field + "drainage-wide.jpg", TileImagePath = Field + "drilling-rig-p.jpg",
            TileItems = new() { "Borehole drilling & rehabilitation", "Pipelines & pump stations", "Supervision & QA" },
            Highlights = new()
            {
                "Borehole drilling, construction & rehabilitation",
                "Pipelines, pump stations & storage tanks",
                "Earthworks, trenching, concrete & structures",
                "Supervision, QA & contract administration",
            },
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
        new() { Title = "Environmental & Social Impact Assessments", ShortTitle = "Environmental & Social", Slug = "esia", Coverage = "Uganda · Zambia", Layout = "split-alt",
            Tagline = "Approvals that satisfy both the regulator and the lender.",
            Description = "We take projects through the full compliance cycle, from screening to closure. Hydrogeology, hydrology and GIS are in-house, so the specialist baselines most ESIAs subcontract are done by the team that writes the assessment.",
            ImagePath = Field + "stakeholder-meeting.jpg", TileImagePath = Field + "river-survey.jpg",
            TileItems = new() { "Full ESIAs (NEMA, ZEMA)", "In-house specialist baselines", "Audits & monitoring" },
            Highlights = new()
            {
                "Full ESIAs & environmental impact statements",
                "Specialist baselines: water, soils, air, biodiversity",
                "Stakeholder engagement & grievance mechanisms",
                "Compliance audits & monitoring programmes",
            },
            StatValue = "NEMA · ZEMA", StatLabel = "Plus international lender safeguards",
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
        new() { Title = "Climate Resilience & Natural Resources", ShortTitle = "Climate Resilience", Slug = "climate-resilience", Coverage = "Uganda · Zambia", Layout = "case-study",
            Tagline = "Planning water and land for the climate ahead.",
            Description = "We help governments, development partners, conservation organisations and agribusiness plan at the scale climate change happens: the catchment, the district, the landscape. Then we turn the evidence into plans and investments people can act on.",
            ImagePath = Field + "owen-falls-p.jpg", TileImagePath = Field + "owen-falls-p.jpg",
            TileItems = new() { "Catchment & IWRM plans", "Land cover change analysis", "Nature-based solutions" },
            Highlights = new()
            {
                "Water resources assessments & IWRM plans",
                "Land use & land cover change analysis",
                "Nature-based solutions & catchment management",
                "Drought & flood monitoring dashboards",
            },
            Deliverables = new()
            {
                "Water resources assessments and IWRM master plans at catchment, district and basin scale",
                "Water balance modelling with climate and demand scenarios",
                "Climate risk and vulnerability assessments for water supply, agriculture and infrastructure",
                "Climate-smart agriculture: irrigation suitability mapping, solar irrigation, rainwater harvesting, soil and water conservation",
                "Land use and land cover change analysis from multi-date satellite imagery",
                "Resource management mapping for forests, wetlands, rangelands and catchments, including encroachment and degradation",
                "Catchment management plans, ecosystem-based adaptation and nature-based solutions",
                "Drought and flood monitoring systems and decision-support dashboards",
                "Technical inputs to climate finance proposals",
            } },
    };

    /// <summary>The project each service shows as "In practice", by service slug and project title.</summary>
    public static readonly IReadOnlyDictionary<string, string> PracticeProjects = new Dictionary<string, string>
    {
        ["technical-studies"] = "Makuutu Bankable Feasibility Study: Hydrogeology Programme",
        ["engineering-design"] = "Mbale Industrial Park Water & Sewage Masterplan",
        ["construction"] = "Groundwater Monitoring Network, Kingfisher Oilfield",
        ["esia"] = "Pivot Irrigation ESIA",
        ["climate-resilience"] = ImatongTitle,
    };

    public const string ImatongTitle = "Imatong Forest Reserve: Land Cover Change & Management Zoning";

    /// <summary>The Services page's climate case study project; databases from before it was seeded get it from the content upgrade.</summary>
    public static Project Imatong() => ListProjects().Single(p => p.Title == ImatongTitle);

    /// <summary>The Services page text, applied to a fresh database and to existing ones where it is still empty.</summary>
    public static void ApplyServicesText(SiteSettings s)
    {
        s.ServicesHeadline = "Water, energy, mining, infrastructure and the ground beneath them all.";
        s.ServicesLede = "Since 2017, we have delivered studies, designs and construction for utilities, energy developers, mines, oil companies and development partners, from Kampala and Lusaka across East, Central and Southern Africa.";
        s.ServicesPosterPath = Field + "drone-poster.jpg";
        s.AerialHeadline = "Every survey starts with the landscape.";
        s.AerialBody = "Drone imagery from fieldwork near Orom, northern Uganda. Reading terrain, drainage and land use from the air shapes where the ground surveys go.";
        s.FleetHeadline = "Our own fleet, on site when you need it.";
        s.FleetBody = "From Lusaka, Optimum Earth Engineering Ltd supplies plant, haulage and site equipment to mining, energy and infrastructure projects, with or without operators. The same machines support our own construction work, so they are maintained to our HSE standards.";
        s.FleetItems = new() { "Tipper trucks & haulage", "Backhoe loaders & excavators", "Drilling & test pumping equipment", "Operated or dry hire" };
        s.FleetImagePath = Field + "zambia-truck-p.jpg";
        s.FleetInsetImagePath = Field + "zambia-backhoe-p.jpg";
        s.CaseStudyStat = "198,350 ha";
        s.CaseStudyStatText = "of natural vegetation converted in the Imatong landscape between 2015 and 2025, close to half of what stood in 2015.";
        s.CaseStudyCaption = "Imatong project landscape, Eastern Equatoria, South Sudan: 584,660 ha classified from 10 m Sentinel-2 imagery. Prepared for IUCN and the Ministry of Environment & Forestry.";
        s.ServicesCtaImagePath = Field + "orom-inselberg.jpg";
    }

    public static List<Sector> Sectors() => new()
    {
        new() { Key = "oil", Name = "Oil & gas", Blurb = "Water supply, monitoring and ESIA for upstream developments", ImagePath = Field + "excavation-supervision.jpg" },
        new() { Key = "mining", Name = "Mining & quarrying", Blurb = "Feasibility hydrogeology, dewatering and borefields", ImagePath = Field + "mining-geology-p.jpg" },
        new() { Key = "energy", Name = "Power & energy", Blurb = "Solar PV, transmission corridors and site investigation", ImagePath = Field + "solar-handover-p.jpg" },
        new() { Key = "public", Name = "Utilities & public sector", Blurb = "Production wells, piped schemes and tank stands", ImagePath = Field + "tank-stand-p.jpg" },
        new() { Key = "community", Name = "Communities", Blurb = "Boreholes and water for schools, hospitals and settlements", ImagePath = Field + "pump-test-p.jpg" },
        new() { Key = "private", Name = "Industry & private", Blurb = "Water, treatment and permitting for industrial sites", ImagePath = Field + "water-treatment-p.jpg" },

        // A sector for projects only: as a draft it stays off the six Services page tiles.
        new() { Key = "conservation", Name = "Conservation & climate", Blurb = "Land cover, catchment and climate studies for conservation landscapes", ImagePath = Img + "aerial/drone-orom-03.jpg", Status = ContentStatus.Draft },
    };

    /// <summary>The Projects page text, applied to a fresh database and to existing ones where it is still empty.</summary>
    public static void ApplyProjectsText(SiteSettings s)
    {
        s.ProjectsHeadline = "Work delivered, with the client and service attached.";
        s.ProjectsLede = "Since 2017 we have surveyed, designed and built for oil companies, mines, utilities, power developers and development partners, from Kampala and Lusaka across East, Central and Southern Africa.";
        s.ProjectsStatFigure = "150+";
        s.ProjectsStatLabel = "Groundwater development assignments";
        s.ProjectsCtaImagePath = Img + "aerial/drone-orom-02.jpg";
    }

    public static List<FieldPhoto> FieldPhotos() => new[]
    {
        ("tank-stand-p", "Water supply", "Elevated steel tank stand, commissioned"),
        ("zambia-truck-p", "Zambia", "Plant and haulage fleet, Optimum Earth Engineering"),
        ("mining-geology-p", "Mining", "Hydrogeology fieldwork with the client's geologists"),
        ("pump-test-p", "Groundwater", "Pumping test on a new production borehole"),
        ("drilling-rig-p", "Drilling", "Borehole drilling and construction"),
        ("drainage-works-p", "Construction", "Drainage channel excavation under our supervision"),
        ("solar-handover-p", "Energy", "Solar-powered water system handover"),
    }.Select(f => new FieldPhoto { ImagePath = Field + f.Item1 + ".jpg", Kicker = f.Item2, Caption = f.Item3 }).ToList();

    public static List<Client> Clients() => new[]
    {
        ("TotalEnergies", "totalenergies"), ("CNOOC", "cnooc"), ("EACOP", "eacop"), ("Daqing Oilfield", "daqing"), ("ATRO Engineering", "atro"),
        ("ZESCO", "zesco"), ("Ministry of Energy, Zambia", "zambia-energy"), ("UEDCL and REA", "uedcl-rea"), ("Hima Cement", "hima-cement"), ("Holcim", "holcim"),
        ("Rwenzori Rare Metals", "rwenzori"), ("Ionic Rare Earths", "ionic"), ("Consolidated Africa", "consolidated-africa"), ("Blencowe Resources", "blencowe"),
        ("NASECO Seeds", "naseco"), ("Sino-Uganda Mbale Industrial Park", "mbale-park"), ("Lagan DOTT Namanve", "lagan-dott"), ("Raxio Data Centre", "raxio"),
        ("Xsabo Group", "xsabo"), ("CAAMENIHU", "caamenihu"), ("WSS Services", "wss"), ("Studio Galli Ingegneria", "sgi"), ("ROKO Construction", "roko"),
    }.Select(c => new Client { Name = c.Item1, LogoPath = Img + "clients/" + c.Item2 + ".png" }).ToList();

    public static List<AerialPhoto> AerialPhotos() => new[]
    {
        ("orom-01", "Woodland and open savanna"), ("orom-02", "Drainage line through cultivated slopes"), ("orom-03", "Riparian corridor below the ridge"),
        ("orom-04", "Seasonal stream and cultivation"), ("camp-01", "Dry-season woodland canopy"), ("camp-02", "Access track through bushland"),
        ("camp-03", "Bushland toward the escarpment"),
    }.Select(a => new AerialPhoto { Caption = a.Item2, ImagePath = Img + "aerial/drone-" + a.Item1 + ".jpg" }).ToList();

    private const string Projects = Img + "projects/";

    /// <summary>The projects shown on the Projects page, in the order they appear.</summary>
    public static List<Project> ListProjects() => new()
    {
        L("Kilyango & Kasinyi Borehole Programme", "UGANDA · 2026", "TotalEnergies EP Uganda", "uganda", "Buliisa District", "oil", false, new() { "technical", "construction" }, Projects + "kilyango-kasinyi.jpg", false, "Geo-seismic and borehole process and reprocess works: hydrogeological survey, drilling, testing and installation of a new borehole, plus rehabilitation of an existing one."),
        L("RAP 2-5 Compensation Boreholes", "UGANDA · 2025", "TotalEnergies EP Uganda", "uganda", "Buliisa District", "oil", false, new() { "construction" }, Projects + "rap-2-5.jpg", false, "Drilling and equipping of six boreholes delivered under an in-kind resettlement compensation programme."),
        L("Replacement Boreholes for Project-Affected Persons", "UGANDA · 2024-2025", "TotalEnergies EP Uganda", "uganda", "Buliisa District", "oil", false, new() { "technical" }, Projects + "pap-replacement.jpg", true, "Hydrogeological studies for six replacement boreholes serving communities affected by oil-field development."),
        L("Tilenga Production Well Integrity Survey", "UGANDA · 2024", "ATRO Engineering", "uganda", "Buliisa District", "oil", false, new() { "technical" }, Projects + "tilenga-well-integrity.jpg", true, "Survey, test pumping and borehole integrity assessment of production wells on the Tilenga oil development."),
        L("Groundwater Monitoring Network, Kingfisher Oilfield", "UGANDA · 2022-2023", "CNOOC Uganda Limited", "uganda", "Kikuube District", "oil", false, new() { "construction", "technical" }, Projects + "kingfisher-monitoring.jpg", false, "Drilling of six groundwater monitoring boreholes across the Kingfisher Oil Field Development Area.", flag: ("6", "monitoring boreholes across the Kingfisher development area", 3)),
        L("Hohwa Production Well", "UGANDA · COMPLETED", "Daqing Oilfield Services Ltd", "uganda", "Hohwa, Kikuube District", "oil", false, new() { "construction" }, Projects + "hohwa-production-well.jpg", true, "Construction of a production water well serving oil-field operations in Hohwa, Kikuube District."),
        L("Groundwater Supply Development, Tilenga Industrial Area", "UGANDA · 2026", "Gauff Consultants Uganda Ltd", "uganda", "Buliisa District", "oil", false, new() { "construction" }, Projects + "tilenga-industrial-area.jpg", false, "Drilling and later flushing of production wells supplying the Buliisa oil-industry support zone.", flag: ("Tilenga", "production wells for the industrial area", 4)),
        L("Buliisa Borehole Integrity Assessment", "UGANDA · 2024", "ATRO Engineering", "uganda", "Buliisa District", "oil", false, new() { "technical" }, Projects + "buliisa-integrity.jpg", true, "Test pumping and borehole integrity assessment of production boreholes supporting oil-field infrastructure."),
        L("Orom Cross Graphite Mine: Community Water Programme", "UGANDA · 2024", "Consolidated African Resources Ltd / Blencowe Plc", "uganda", "Orom, northern Uganda", "mining", false, new() { "construction" }, Img + "aerial/drone-orom-03.jpg", false, "Rehabilitation and upgrade of the Locomo Solar Powered Water Supply System serving the community around the mine."),
        L("Makuutu Bankable Feasibility Study: Hydrogeology Programme", "UGANDA · 2021-2022", "Rwenzori Rare Metals / Ionic Rare Earth", "uganda", "Bugweri, eastern Uganda", "mining", false, new() { "technical" }, Projects + "makuutu-bfs.jpg", false, "Hydrogeology programme delivered as part of the rare earth mine's Bankable Feasibility Study."),
        L("Orom-Cross Graphite DFS Borefield Assessment", "UGANDA · 2023", "Consolidated African Resources Ltd / Blencowe Plc", "uganda", "Orom, northern Uganda", "mining", false, new() { "technical", "construction" }, Img + "aerial/drone-orom-01.jpg", false, "Definitive Feasibility Study hydrological work and borefield assessment for a graphite development, including drilling, aquifer testing, water quality assessment and hydrogeological design studies.", flag: ("DFS", "borefield for a graphite mine", 7)),
        L("Hima Limestone Quarry Dewatering", "UGANDA · 2021-2022", "Lafarge Holcim / Hima Cement Ltd", "uganda", "Hima, Kasese District", "mining", false, new() { "engineering", "construction" }, Projects + "hima-dewatering.jpg", false, "Mine dewatering and groundwater control, plus a new water-supply system for a limestone quarry."),
        L("Dura Quarry Topographic Survey & Drainage Works", "UGANDA · 2022", "Hima Cement Ltd", "uganda", "Dura, Kamwenge District", "mining", false, new() { "technical", "construction" }, Projects + "dura-quarry.jpg", false, "Topographic surveying in support of quarry operations and expansion planning, followed by excavation and reinstatement of the quarry drainage channels."),
        L("Hima Quarry Water Supply System", "UGANDA · 2021", "Hima Cement Ltd", "uganda", "Hima, Kasese District", "mining", false, new() { "engineering", "construction" }, Projects + "hima-water-supply.jpg", false, "Pipeline and powered water-supply infrastructure serving quarry operations."),
        L("Quarry Site ERT Survey", "UGANDA · 2022", "Agaba South West Services", "uganda", "South-western Uganda", "mining", false, new() { "technical" }, Projects + "quarry-ert.jpg", true, "2D electrical resistivity tomography survey for bedrock and site assessment ahead of quarry development."),
        L("Quarry Bedrock Characterisation Surveys", "UGANDA · 2021", "Private quarry operators", "uganda", "Mukono District", "mining", false, new() { "technical" }, Projects + "quarry-bedrock.jpg", true, "Geophysical surveys for bedrock characterisation at two quarry sites in Mukono District."),
        L("50 MW Kasempa Solar PV Plant", "ZAMBIA · ONGOING", "Optimum Earth Engineering Ltd", "zambia", "Kasempa, North-Western Province", "energy", true, new() { "technical", "engineering" }, Projects + "kasempa-solar.jpg", true, "Prefeasibility study complete; detailed feasibility study under way for a 50 MW utility-scale solar PV plant.", flag: ("50 MW", "utility-scale solar PV, Zambia", 2)),
        L("Pensulo-Mansa 330 kV Transmission Line Corridor", "ZAMBIA · 2026 (ONGOING)", "Optimum Earth Engineering Ltd / ZESCO Limited", "zambia", "Central to Luapula Province", "energy", true, new() { "technical" }, Projects + "pensulo-mansa.jpg", true, "Authorised by Zambia's Ministry of Energy to undertake feasibility studies for a 330 kV transmission corridor from Pensulo to Mansa, in collaboration with ZESCO Limited.", flag: ("330 kV", "Pensulo to Mansa transmission corridor", 1)),
        L("Waste-to-Energy Plant Site Investigation", "UGANDA · 2024", "NLS Waste Services Ltd", "uganda", "Uganda", "energy", false, new() { "technical" }, Projects + "waste-to-energy.jpg", true, "Geotechnical and hydrological investigations for a proposed waste-to-energy generation plant."),
        L("Pilot Aquifer Recharge Pre-Feasibility Studies", "BURUNDI · 2025", "Rivan Engineering Ltd / Nile Basin Initiative", "burundi", "Burundi", "public", false, new() { "technical", "climate" }, Projects + "burundi-mar.jpg", false, "Geophysical, geotechnical and topographic surveys for pre-feasibility studies of pilot managed aquifer recharge systems."),
        L("Solar Water Supply & Irrigation, Several Sites", "UGANDA · 2021-2022", "EPM / Nexus Green Ltd / Ministry of Water", "uganda", "Nationwide", "public", false, new() { "engineering", "construction" }, Projects + "solar-water-irrigation.jpg", true, "Engineering design and construction supervision for solar-powered water supply and irrigation systems across 79 sites nationwide.", flag: ("79", "solar water and irrigation sites designed", 9)),
        L("Corporate Operations Environmental Review", "UGANDA · 2023", "NASECO", "uganda", "Uganda", "private", false, new() { "environmental" }, Projects + "naseco-review.jpg", false, "Independent review of company operations and assessment of their environmental impact."),
        L("Pivot Irrigation ESIA", "UGANDA · ONGOING", "NASECO", "uganda", "Uganda", "private", true, new() { "environmental" }, Projects + "naseco-pivot-esia.jpg", false, "Environmental and Social Impact Assessment and closure works for an irrigation development project."),
        L("Nile Basin Transboundary Groundwater Database", "NILE BASIN · COMPLETED", "Centric Solutions Ltd", "nilebasin", "Nile Basin, regional", "public", false, new() { "technical", "climate" }, Projects + "nile-gw-database.jpg", true, "Development of an online groundwater database management system for transboundary aquifers across the Nile Basin."),
        L("Namanve Wastewater Quality Testing & Monitoring", "UGANDA · 2022", "Lagan DOTT", "uganda", "Namanve, Mukono District", "private", false, new() { "environmental" }, Projects + "namanve-wastewater.jpg", true, "Wastewater quality monitoring programme supporting an industrial park."),
        L("Fire Protection System Installation", "DRC · COMPLETED", "CAAMENIHU", "drc", "DRC", "community", false, new() { "engineering", "construction" }, Projects + "drc-fire-protection.jpg", true, "Design, supply and installation of a fire-protection system for an institutional facility."),
        L("Kiryandongo Solar-Powered Water Supply: Design & Build", "UGANDA · 2024-2025", "DBS Establishments - Embassy of the UAE", "uganda", "Kiryandongo District", "community", false, new() { "engineering", "construction" }, Projects + "kiryandongo-dnb.jpg", true, "Full design-and-build delivery of a solar-powered water-supply system serving a refugee settlement."),
        L("Kasese Solar Borehole Development", "UGANDA · 2024", "Tulima Solar", "uganda", "Kasese District", "private", false, new() { "technical" }, Projects + "kasese-solar-borehole.jpg", true, "Hydrogeological investigation ahead of drilling a solar-powered production well."),
        L("Multi Regional Surface Water Sources Mapping", "UGANDA · 2026", "Tulima Solar", "uganda", "Multiple regions", "private", false, new() { "technical", "climate" }, Projects + "surface-water-mapping.jpg", true, "Surface water source mapping across five districts to guide solar-powered water infrastructure planning."),
        L("Solar-Powered Piped Water System: Feasibility & Design", "UGANDA · 2025-2026", "Ibanda District Local Government", "uganda", "Ibanda District", "public", false, new() { "technical", "engineering" }, Projects + "ibanda-piped-water.jpg", true, "Feasibility study and detailed engineering design for a solar-powered piped water supply scheme."),
        L("National Utility Hydrogeological Survey Programme", "UGANDA · 2023-2024", "National Water & Sewerage Corporation (NWSC)", "uganda", "Twelve NWSC regions", "public", false, new() { "technical" }, Projects + "nwsc-national-survey.jpg", true, "Hydrogeological survey for twelve production wells delivered across twelve NWSC regions nationwide.", flag: ("12", "NWSC regions, production well surveys", 8)),
        L("VIP Latrine Construction Programme", "UGANDA · 2025", "DBS Establishments - Embassy of the UAE", "uganda", "Refugee settlement, Uganda", "community", false, new() { "engineering", "construction" }, Projects + "vip-latrines.jpg", true, "Design and construction of eight VIP latrine blocks serving a refugee settlement community."),
        L("Bwindi Eco School Water & Treatment System", "UGANDA · 2020-2021", "Bwindi Eco School", "uganda", "Bwindi, south-western Uganda", "community", false, new() { "engineering", "construction" }, Projects + "bwindi-eco-school.jpg", true, "Design and construction of a complete water-supply and treatment system for a school campus."),
        L("Muhazi Legacy Estate Water Infrastructure", "UGANDA · ONGOING", "Eco-Lakes International", "uganda", "Masindi District", "private", true, new() { "technical", "engineering", "construction" }, Projects + "muhazi-estate.jpg", false, "Feasibility study, design and construction of water infrastructure for a lakeside estate development."),
        L("Kyaliwajjala Water Supply Network Trenching", "UGANDA · 2023", "National Water & Sewerage Corporation (NWSC)", "uganda", "Kyaliwajjala, Wakiso District", "public", false, new() { "construction" }, Projects + "kyaliwajjala-trenching.jpg", true, "Trenching works for gate crossings on a piped water-distribution network."),
        L("Kako Hospital Water Programme", "UGANDA · 2019-2020", "Engineers Without Borders", "uganda", "Kako, Masaka District", "community", false, new() { "technical", "construction" }, Projects + "kako-hospital.jpg", true, "Borehole situation analysis, siting, drilling and construction delivering a reliable water source for a rural hospital."),
        L("Ten-Village Borehole Programme", "UGANDA · 2020", "A Chance for Children", "uganda", "Rural Uganda", "community", false, new() { "construction" }, Projects + "ten-village.jpg", false, "Construction of ten boreholes across rural villages under a framework arrangement."),
        L("Borehole Rehabilitation Assessment", "UGANDA · 2021", "Windle Trust International", "uganda", "Uganda", "community", false, new() { "construction", "technical" }, Field + "pump-test.jpg", true, "Rehabilitation and test pumping of boreholes serving a refugee-education programme."),
        L("Data Centre Water Supply Borehole", "UGANDA · 2021", "Raxio Data Centre", "uganda", "Kampala", "private", false, new() { "technical", "construction" }, Projects + "raxio-data-centre.jpg", true, "Survey and construction of a dedicated production borehole for a regional data-centre facility."),
        L("Hospitality Sector Borehole Development", "UGANDA · 2024-2025", "The Great Wall Hotel & Investments", "uganda", "Uganda", "private", false, new() { "technical", "construction" }, Field + "drilling-rig.jpg", true, "Survey and drilling of production boreholes securing water supply for hospitality developments."),
        L("Multi-District Borehole Casing Integrity Verification", "UGANDA · 2020", "NSI Water Ltd", "uganda", "Four districts", "private", false, new() { "technical", "construction" }, Projects + "nsi-casing.jpg", true, "Casing integrity verification and rehabilitation of four production boreholes across four districts."),
        L("Regional Borehole Performance Assessment", "UGANDA · 2021-2022", "EPM Engineering Consults (U) Ltd", "uganda", "75 sites region-wide", "public", false, new() { "technical" }, Projects + "epm-performance.jpg", true, "Test pumping of 47 boreholes and water-quality analysis across 75 sites region-wide."),
        L("Nakivale Refugee Settlement Borehole Testing", "UGANDA · 2021", "Karf Aqua Engineering", "uganda", "Nakivale, Isingiro District", "community", false, new() { "technical" }, Projects + "nakivale-testing.jpg", true, "Test pumping of boreholes supporting water supply to a major refugee settlement."),
        L("Borehole Construction & Water Permitting", "UGANDA · COMPLETED", "The Xsabo Group", "uganda", "Uganda", "private", false, new() { "technical", "construction" }, Projects + "xsabo-permitting.jpg", false, "Borehole siting, drilling and water-use permitting services delivered under a repeat engagement."),
        L("Mbale Industrial Park Water & Sewage Masterplan", "UGANDA · 2020, 2022", "Tangshan Mbale Industrial Park", "uganda", "Mbale", "private", false, new() { "technical", "engineering" }, Projects + "mbale-industrial-park.jpg", false, "Preliminary feasibility studies and designs for a 5,000 m³/day water supply and 5,000 m³/day sewage system, followed by geotechnical, topographic and hydrological assessment."),
        L("St Joseph of Nazareth School Water Programme", "UGANDA · 2023 & 2025", "St Joseph of Nazareth High School", "uganda", "Uganda", "community", false, new() { "technical", "construction" }, Projects + "st-joseph-nazareth.jpg", true, "Survey, drilling, test pumping, casing and hand-pump installation delivering safe water to a school community, in two phases."),
        L("Kitukiro Rural Water Supply: Pipeline Trenching & Fabrication", "UGANDA · 2026 (ONGOING)", "Water Mission Uganda", "uganda", "Kitukiro, Buyende District", "community", true, new() { "construction" }, Projects + "kitukiro-trenching.jpg", false, "Trenching and backfilling of a 27 km pipeline route, plus fabrication works, for a rural community water-supply scheme.", flag: ("27 km", "pipeline trench for a rural water scheme", 5)),
        L("Buliisa & Hoima Regional Water Programme", "UGANDA · 2018-2026", "Multiple institutional and private clients", "uganda", "Buliisa, Hoima and nationwide", "community", false, new() { "technical", "construction" }, Projects + "buliisa-hoima-programme.jpg", true, "150+ groundwater development assignments, from borehole siting and drilling to test pumping and rehabilitation, completed since 2018 across households, schools, hospitals, hotels, refugee settlements and utilities nationwide."),
        L("Imatong Forest Reserve: Land Cover Change & Management Zoning", "SOUTH SUDAN · 2025", "IUCN", "southsudan", "Eastern Equatoria, South Sudan", "conservation", false, new() { "climate", "environmental" }, Projects + "imatong-landcover.jpg", false, "Ten-year land cover change analysis from 10 m Sentinel-2 imagery over 584,660 ha, with hot spot statistics, erosion and landslide risk, and management zoning for the reserve and its surrounding landscape. Prepared for IUCN and the Ministry of Environment & Forestry.", flag: ("584,660 ha", "classified, Imatong landscape", 6)),
    };

    private static Project L(string title, string tag, string client, string country, string location, string sector, bool ongoing,
        List<string> services, string image, bool representative, string description, (string Figure, string Label, int Order)? flag = null) =>
        new()
        {
            Title = title, Tag = tag, Meta = "Client: " + client, Client = client, Country = country, Location = location, Sector = sector,
            Ongoing = ongoing, Services = services, Service = services[0], ImagePath = image, RepresentativePhoto = representative,
            Description = description, FlagshipFigure = flag?.Figure ?? string.Empty, FlagshipLabel = flag?.Label ?? string.Empty, FlagshipOrder = flag?.Order ?? 0,
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
        new() { Title = "Technical Studies", Description = "Hydrogeology, geophysics, geotechnics, survey and GIS.", LinkUrl = "/services#technical-studies" },
        new() { Title = "Engineering Design", Description = "Water supply, solar PV and pumping, transmission support.", LinkUrl = "/services#engineering-design" },
        new() { Title = "Construction Services", Description = "Drilling, pipelines, pump stations, supervision.", LinkUrl = "/services#construction" },
        new() { Title = "Environmental & Social", Description = "ESIAs, specialist baselines, audits and monitoring.", LinkUrl = "/services#esia" },
        new() { Title = "Climate Resilience", Description = "Catchment plans, land cover change, nature-based solutions.", LinkUrl = "/services#climate-resilience" },
    };

    // Each description's first sentence is shown as the value's lead line on the Who We Are page.
    public static List<CoreValue> Values() => new()
    {
        new() { Title = "Safety", Description = "Safety comes first, always. Everyone is responsible for their own health and safety and that of their colleagues, the public and the communities where we work. No deadline, task or budget justifies compromising it, and anyone may stop work they believe is unsafe. We protect the dignity and wellbeing of everyone our work touches, especially children and vulnerable people." },
        new() { Title = "Collaboration", Description = "Better solutions are built together. We work with colleagues, clients, donors, communities, partners and government with respect, inclusion and openness. We value the contributions of a diverse and talented team and seek out people who are passionate, curious and committed to those we serve. We invest in one another's professional development." },
        new() { Title = "Accountability & Integrity", Description = "We do the right thing, even when no one is watching. We act with honesty and the highest ethics in every dealing, commercial or charitable. We do not tolerate bribery, fraud or undeclared conflicts of interest. We own our commitments, decisions and results, raise problems early, and answer to our clients, donors, regulators and the communities we serve." },
        new() { Title = "Learning & Innovation", Description = "We stay curious and challenge complacency. We learn from experience and from those we serve, apply appropriate technology, and continually adapt our solutions to meet changing needs." },
        new() { Title = "Excellence", Description = "We aim to do our work right the first time. From technical services and engineering delivery to community programmes and partnerships, we hold high standards of quality. We treat quality as a shared responsibility and deliver work that is reliable, measurable and fit for purpose." },
        new() { Title = "Stewardship", Description = "We leave people and places better than we found them. We take responsibility for everything entrusted to us: client assets, company resources, donor and grant funds, and the communities and environments where we work. We use resources responsibly, account for them transparently and create lasting value." },
    };

    public const string Mission = "We are committed to value-driven partnerships with our clients, providing services of the highest quality, safety and integrity while focusing on customer satisfaction and innovative solutions. We strive to be the company that clients want to work with and employees want to work for, by providing excellent results and rewarding careers.";

    public const string Vision = "To be the leading global engineering and environmental services provider, focused on creating value and success for our clients.";

    private const string Who = Img + "who/";

    /// <summary>The Who We Are page text, applied to a fresh database and to existing ones where it is still empty.</summary>
    public static void ApplyWhoWeAreText(SiteSettings s)
    {
        s.WhoHeadline = "Engineering solutions for industry.";
        s.WhoHighlight = "Resilience for communities.";
        s.WhoLede = "Two engineering companies and a foundation, working across East, Central and Southern Africa since 2017. We study the ground, design what goes on it, build it, and stay accountable for how it performs.";
        s.WhoHeroImagePath = Who + "hero.jpg";
        s.StoryHeadline = "It began with a friendship and a shared ambition.";
        s.StoryBody = "Optimum Earth was founded in Kampala in August 2017 by four students from Makerere University, who believed young Africans could build solutions that matter.\n\n"
            + "That belief became a firm of hydrogeologists, geophysicists, engineers, surveyors and environmental specialists. Today we support oil and gas developers, mines, utilities, power projects, development partners and the communities around them.\n\n"
            + "We have kept the habit we started with: practical problem-solving, done by people who go to site themselves.";
        s.StoryImagePath = Who + "field.jpg";
        s.StoryInsetImagePath = Who + "team.jpg";
        s.FoundedOn = new DateOnly(2017, 8, 7);
        s.WorkCountries = new() { "Uganda", "Zambia", "South Sudan", "DRC", "Burundi", "Rwanda" };
        s.WhoCtaHeadline = "Work with us, or work for us.";
        s.WhoCtaBody = "Tell us what you're planning and we'll scope it with you. If you're an engineer, scientist or field technician who wants to build things that last, we'd like to hear from you too.";
        s.WhoCtaImagePath = Who + "field.jpg";
    }

    public static List<Milestone> Milestones() => new()
    {
        new() { Year = "2017", Title = "Founded in Kampala", Description = "Four Makerere University students start Optimum Earth (U) Ltd." },
        new() { Year = "2018", Title = "Groundwater programme", Description = "Siting, drilling and testing work begins across Buliisa and Hoima." },
        new() { Year = "2021", Title = "Mining clients", Description = "Quarry dewatering and water supply for Hima Cement." },
        new() { Year = "2022", Title = "Oil and gas", Description = "Groundwater monitoring network for CNOOC at Kingfisher." },
        new() { Year = "2025", Title = "Zambia", Description = "Optimum Earth Engineering Ltd opens in Lusaka." },
        new() { Year = "2026", Title = "Power", Description = "Pensulo–Mansa 330 kV corridor and 50 MW Kasempa solar studies." },
    };

    public static List<GroupCompany> GroupCompanies() => new()
    {
        new() { Kind = "Uganda · Since 2017", Name = "Optimum Earth (U) Ltd", ImagePath = Who + "hero.jpg", LinkText = "Optimum Earth Uganda", LinkUrl = "/uganda",
            Description = "Our founding company, headquartered in Luzira, Kampala. It delivers studies, design and construction for water, oil and gas, mining and infrastructure across the region.",
            Tags = new() { "Hydrogeology", "Geophysics", "Water supply", "ESIA" } },
        new() { Kind = "Zambia · Since 2025", Name = "Optimum Earth Engineering Ltd", ImagePath = Who + "energy.jpg", LinkText = "Optimum Earth Zambia", LinkUrl = "/zambia",
            Description = "Our Lusaka company, focused on energy and infrastructure in Southern Africa, from utility-scale solar to high-voltage transmission corridors, plus equipment hire.",
            Tags = new() { "Solar PV", "Transmission", "Equipment hire" } },
        new() { Kind = "Not-for-profit", Name = "Optimum Earth Foundation", ImagePath = Who + "community.jpg", LinkText = "Our Foundation", LinkUrl = "/foundation", IsFoundation = true,
            Description = "Our not-for-profit arm brings partners, knowledge and finance together to create lasting change across Africa.",
            Tags = new() { "Access", "Resilience", "Impact" } },
    };

    public static List<Stat> Stats() => new()
    {
        new() { Kind = "years", Label = "Years in operation" },
        new() { Value = 100, Suffix = "+", Label = "Organisations served" },
        new() { Value = 150, Suffix = "+", Label = "Groundwater development assignments" },
        new() { Value = 6, Label = "Countries delivered in" },
        new() { Value = 2, Label = "Offices: Kampala & Lusaka" },
    };

    public static List<Office> Offices() => new()
    {
        new() { Name = "Optimum Earth Uganda", Country = "Uganda", AddressLines = new() { "Roston House, Plot 56/57", "Chorley Crescent Rd", "Luzira, Uganda" },
            Phone = "+256 784 080551", Email = "uganda@optimum-earth.com" },
        new() { Name = "Optimum Earth Zambia", Country = "Zambia", AddressLines = new() { "Oilfield Garden, Plot 12 - Flat 4", "Kabulonga, Kudu Rd", "Lusaka, Zambia" },
            Email = "zambia@optimum-earth.com" },
    };

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
        HeroImagePath = Img + "heroslide1.jpg",
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
        HeroImagePath = Img + "heroslide2.jpg",
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
        CtaBody = "Oilfield Garden, Plot 12 - Flat 4 · Kabulonga, Kudu Rd · Lusaka, Zambia · zambia@optimum-earth.com",
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
