using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace OptimumEarth.Web.Data;

/// <summary>
/// Brings an existing database's built-in content up to the latest revision (see
/// <see cref="SiteSettings.ContentVersion"/>) without overwriting anyone's work.
///
/// Version 2 is the content refresh from the fork's pull request: new service lines with
/// deliverables, a larger project list with descriptions, revised Who We Are copy, the
/// SDG approach documents and the rewritten Uganda and Zambia pages.
///
/// Version 3 normalizes the Zambia country-page contact line to match the footer.
/// Version 4 fills the confirmed Zambia office address.
/// Version 5 is the Who We Are redesign: shorter service-line blurbs, values that open with a
/// lead sentence, the revised mission, and the new page text (the new milestone, group, stat
/// and office lists are filled by the seeder, as for any empty table).
/// Version 6 is the Services redesign: each service gains its anchor, tagline, tiles, highlights,
/// stat badge and "In practice" project, and untouched services take the shorter section text and
/// new photos. It also adds the Imatong project, the "Explore" links on the Who We Are service
/// cards, and the Services page text (sectors, clients and drone photos are seeded as empty tables).
/// Version 7 is the Projects redesign: each listed project gains its client, location, sector,
/// status, services and flagship figure, and untouched ones take the new title, text and photo.
/// It also adds the draft "Conservation & climate" sector and the Projects page text.
///
/// Each section is replaced only if it still matches the original seeded content exactly;
/// a section someone has edited in the dashboard is left alone and reported in the log, so
/// the dashboard stays the source of truth. A fresh database is seeded at the latest version
/// and never comes through here.
/// </summary>
public static class ContentUpgrades
{
    private static readonly string[] V1Services =
    {
        "Water supply solutions", "Groundwater monitoring", "Environmental services", "GIS, mapping & remote sensing", "Solutions for mines, oil & gas",
    };

    private static readonly string[] V1Capabilities = { "Water resources", "Energy", "Environment", "GIS & mapping", "Engineering" };

    private static readonly string[] V1Values = { "Culture", "Integrity", "Innovation", "Safety and Health", "Quality" };

    private static readonly string[] V1ProjectTitles =
    {
        "Uganda Production Wells Survey", "Kingfisher Monitoring Wells", "Pivot Irrigation ESIA Initiative", "Hima Quarry Water Management",
        "Makuutu Hydrogeology Programme", "Quarry Site ERT Survey", "Uganda Borehole Testing Initiative", "Enhancing Climate Resilient WASH Initiatives",
        "Pivot Irrigation Development Assessment", "Mbale Industrial Infrastructure Consultancy", "Project to be confirmed", "Project to be confirmed",
        "Regional experience applies here",
    };

    private const string V1UgandaFacts = "Sectors | Water · Energy · Mining";
    private const string V1ZambiaFacts = "Status | Establishing";
    private const string V2ZambiaCtaBody = "Office address to be confirmed · zambia@optimum-earth.com";
    private const string V3ZambiaCtaBody = "Lusaka, Zambia · Office address to be confirmed · zambia@optimum-earth.com";

    private static readonly (string Title, string Description)[] V4Capabilities =
    {
        ("Technical Studies", "Know the ground before you commit capital."),
        ("Engineering Design", "Designs built on real site data."),
        ("Construction Services", "Built by the team that designed it."),
        ("Environmental & Social Impact Assessments", "Approvals that satisfy both the regulator and the lender"),
        ("Climate Resilience & Natural Resources", "Planning water and land for the climate ahead."),
    };

    // Only these two values were reworded in version 5; the others are compared with the current seed.
    private static readonly Dictionary<string, string> V4ValueText = new()
    {
        ["Accountability & Integrity"] = "We act with honesty and the highest ethics in every dealing, commercial or charitable, and we do the right thing even when no one is watching. We do not tolerate bribery, fraud or undeclared conflicts of interest. We own our commitments, decisions and results, raise problems early, and answer to our clients, donors, regulators and the communities we serve.",
        ["Stewardship"] = "We take responsibility for everything entrusted to us: client assets, company resources, donor and grant funds, and the communities and environments where we work. We use resources responsibly, account for them transparently, create lasting value, and leave people, communities and the planet better than we found them.",
    };

    private const string V4Mission = "We are committed to value-driven partnerships with our clients by providing services of the highest quality, safety, and integrity while focusing on customer satisfaction and innovative solutions. We strive to be the company that clients want to work with and employees want to work for, by providing excellent results and rewarding careers.";

    private const string V5Img = "/img/optimum-earth-images/";

    // The services as seeded up to version 5, to tell untouched ones from edited ones.
    private static List<Service> V5Services() => new()
    {
        new() { Title = "Technical Studies", Coverage = "Uganda · Zambia", ImagePath = V5Img + "services/technical-studies.jpg",
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
        new() { Title = "Engineering Design", Coverage = "Uganda · Zambia", ImagePath = V5Img + "services/engineering-design.jpeg",
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
        new() { Title = "Construction Services", Coverage = "Uganda · Zambia", ImagePath = V5Img + "services/construction-services.jpg",
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
        new() { Title = "Environmental & Social Impact Assessments", Coverage = "Uganda · Zambia", ImagePath = V5Img + "services/environmental-and-social-impact-assessments.jpg",
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
        new() { Title = "Climate Resilience & Natural Resources", Coverage = "Uganda · Zambia", ImagePath = V5Img + "services/climate-resilience-&-natural-resources.jpg",
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

    // The Projects page list as seeded up to version 6: old title, new title, and the text and photo that mark it untouched.
    private static readonly (string OldTitle, string NewTitle, string ImagePath, string Description)[] V6Projects =
    {
        ("Kilyango & Kasinyi Borehole Programme", "Kilyango & Kasinyi Borehole Programme", V5Img + "project1.jpg", "Geo-seismic and borehole process/reprocess works: hydrogeological survey, drilling, testing and installation of a new borehole, plus rehabilitation of an existing one."),
        ("RAP 2-5 Compensation Boreholes", "RAP 2-5 Compensation Boreholes", V5Img + "project2.jpg", "Drilling and equipping of six boreholes delivered under an in-kind resettlement compensation programme."),
        ("Replacement Boreholes for Project-Affected Persons", "Replacement Boreholes for Project-Affected Persons", V5Img + "project3.jpg", "Hydrogeological studies for six replacement boreholes serving communities affected by oil-field development."),
        ("Tilenga - Production Well Integrity Survey", "Tilenga Production Well Integrity Survey", V5Img + "project4.jpg", "Survey, test pumping and borehole integrity assessment of production wells on the Tilenga oil development."),
        ("Groundwater Monitoring Network - Kingfisher Oilfield", "Groundwater Monitoring Network, Kingfisher Oilfield", V5Img + "recentwork2.jpg", "Drilling of six groundwater monitoring boreholes across the Kingfisher Oil Field Development Area."),
        ("Hohwa Production Well", "Hohwa Production Well", V5Img + "project6.jpg", "Construction of a production water well serving oil-field operations in Hohwa, Kikuube district."),
        ("Groundwater Supply Development - Tilenga Industrial Area", "Groundwater Supply Development, Tilenga Industrial Area", V5Img + "project7.jpg", "Drilling and later flushing of production wells supplying the Buliisa oil-industry support zone."),
        ("Buliisa Borehole Integrity Assessment", "Buliisa Borehole Integrity Assessment", V5Img + "project8.jpg", "Test pumping and borehole integrity assessment of production boreholes supporting oil-field infrastructure."),
        ("Orom Cross Graphite Mine - Community Water Programme", "Orom Cross Graphite Mine: Community Water Programme", V5Img + "project9.jpg", "Rehabilitation and upgrade of the locomo solar powered community water-system"),
        ("Makuutu Bankable Feasibility Study - Hydrogeology Programme", "Makuutu Bankable Feasibility Study: Hydrogeology Programme", V5Img + "makuttu.jpg", "Hydrogeology programme delivered as part of the mine's Bankable Feasibility Study."),
        ("Orom-Cross Graphite DFS Borefield Assessment", "Orom-Cross Graphite DFS Borefield Assessment", V5Img + "project11.jpg", "Definitive Feasibility Study hydrological work and project borefield assessment for a graphite development. Including: drilling, aquifer testing, water quality assessments, hydrogeological design studies"),
        ("Hima Limestone Quarry Dewatering", "Hima Limestone Quarry Dewatering", V5Img + "project12.jpg", "Mine dewatering and groundwater control, plus a new water-supply system for a limestone quarry."),
        ("Dura Quarry Topographic Survey", "Dura Quarry Topographic Survey & Drainage Works", V5Img + "project13.jpg", "Topographic surveying in support of quarry operations and expansion planning."),
        ("Hima Quarry Water Supply System", "Hima Quarry Water Supply System", V5Img + "hima-quarry.jpg", "Pipeline and powered water-supply infrastructure serving quarry operations."),
        ("Quarry Site ERT Survey", "Quarry Site ERT Survey", V5Img + "servicepanel4.jpg", "2D electrical resistivity tomography survey for bedrock and site assessment ahead of quarry development."),
        ("Quarry Bedrock Characterisation Surveys", "Quarry Bedrock Characterisation Surveys", V5Img + "project15.jpg", "Geophysical surveys for bedrock characterisation at two quarry sites in Mukono district."),
        ("50 MW Kasempa Solar PV Plant", "50 MW Kasempa Solar PV Plant", V5Img + "project16.jpg", "Prefeasibility study complete; detailed feasibility study under way for a 50 MW utility-scale solar PV plant."),
        ("Pensulo-Mansa 330 kV Transmission Line Corridor", "Pensulo-Mansa 330 kV Transmission Line Corridor", V5Img + "project17.jpg", "Authorised by Zambia's Ministry of Energy to undertake feasibility studies for a 330 kV transmission corridor from Pensulo to Mansa, in collaboration with ZESCO Limited."),
        ("Waste-to-Energy Plant Site Investigation", "Waste-to-Energy Plant Site Investigation", V5Img + "project18.jpg", "Geotechnical and hydrological investigations for a proposed waste-to-energy generation plant."),
        ("Pilot Aquifer Recharge Pre-Feasibility Studies", "Pilot Aquifer Recharge Pre-Feasibility Studies", V5Img + "project19.jpg", "Geophysical, geotechnical and topographic surveys for pre-feasibility studies of pilot managed aquifer recharge systems."),
        ("Solar Water Supply & Irrigation, Several Sites", "Solar Water Supply & Irrigation, Several Sites", V5Img + "project20.jpg", "Engineering design and construction supervision for solar-powered water supply and irrigation systems across up to 687 locations nationwide."),
        ("Corporate Operations Environmental Review", "Corporate Operations Environmental Review", V5Img + "project21.jpg", "Independent review of company operations and assessment of their environmental impact."),
        ("Pivot Irrigation ESIA", "Pivot Irrigation ESIA", V5Img + "enhancing-climate.jpg", "Environmental and Social Impact Assessment closure works for an irrigation development project."),
        ("Nile Basin Transboundary Groundwater Database", "Nile Basin Transboundary Groundwater Database", V5Img + "project23.jpg", "Development of an online groundwater database management system for transboundary aquifers across the Nile Basin."),
        ("Namanve Wastewater Quality Testing & Monitoring", "Namanve Wastewater Quality Testing & Monitoring", V5Img + "project24.jpg", "Wastewater quality monitoring programme supporting an industrial park."),
        ("Fire Protection System Installation", "Fire Protection System Installation", V5Img + "project25.jpg", "Design, supply and installation of a fire-protection system for an institutional facility."),
        ("Kiryandongo Solar-Powered Water Supply - Design & Build", "Kiryandongo Solar-Powered Water Supply: Design & Build", V5Img + "project26.jpg", "Full design-and-build delivery of a solar-powered water-supply system serving a refugee settlement."),
        ("Kasese Solar Borehole Development", "Kasese Solar Borehole Development", V5Img + "project27.jpg", "Hydrogeological investigation ahead of drilling a solar-powered production well."),
        ("National Surface Water Source Mapping", "Multi Regional Surface Water Sources Mapping", V5Img + "project28.jpg", "Surface water source mapping across five districts to guide solar-powered water infrastructure planning."),
        ("Solar-Powered Piped Water System - Feasibility & Design", "Solar-Powered Piped Water System: Feasibility & Design", V5Img + "project29.jpg", "Feasibility study and detailed engineering design for a solar-powered piped water supply scheme."),
        ("National Utility Hydrogeological Survey Programme", "National Utility Hydrogeological Survey Programme", V5Img + "project30.jpg", "Hydrogeological survey for twelve production wells delivered across twelve NWSC regions nationwide."),
        ("VIP Latrine Construction Programme", "VIP Latrine Construction Programme", V5Img + "project31.jpg", "Design and construction of eight VIP latrine blocks serving a refugee settlement community."),
        ("Bwindi Eco School Water & Treatment System", "Bwindi Eco School Water & Treatment System", V5Img + "project32.jpg", "Design and construction of a complete water-supply and treatment system for a school campus."),
        ("Muhazi Legacy Estate Water Infrastructure", "Muhazi Legacy Estate Water Infrastructure", V5Img + "project33.jpg", "Feasibility study, design and construction of water infrastructure for a lakeside estate development."),
        ("Kyaliwajjala Water Supply Network Trenching", "Kyaliwajjala Water Supply Network Trenching", V5Img + "project34.jpg", "Trenching works for gate crossings on a piped water-distribution network."),
        ("Kako Hospital Water Programme", "Kako Hospital Water Programme", V5Img + "project35.jpg", "Borehole situation analysis, siting, drilling and construction delivering a reliable water source for a rural hospital."),
        ("Ten-Village Borehole Programme", "Ten-Village Borehole Programme", V5Img + "project36.jpg", "Construction of ten boreholes across rural villages under a framework arrangement."),
        ("Borehole Rehabilitation Assessment", "Borehole Rehabilitation Assessment", V5Img + "project37.jpg", "Rehabilitation and test pumping of boreholes serving a refugee-education programme."),
        ("Data Centre Water Supply Borehole", "Data Centre Water Supply Borehole", V5Img + "project38.jpg", "Survey and construction of a dedicated production borehole for a regional data-centre facility."),
        ("Hospitality Sector Borehole Development", "Hospitality Sector Borehole Development", V5Img + "project39.jpg", "Survey and drilling of production boreholes securing water supply for hospitality developments."),
        ("Multi-District Borehole Casing Integrity Verification", "Multi-District Borehole Casing Integrity Verification", V5Img + "project40.jpg", "Casing integrity verification and rehabilitation of four production boreholes across four districts."),
        ("Regional Borehole Performance Assessment", "Regional Borehole Performance Assessment", V5Img + "project41.jpg", "Test pumping of 47 boreholes and water-quality analysis across 75 sites region-wide."),
        ("Nakivale Refugee Settlement Borehole Testing", "Nakivale Refugee Settlement Borehole Testing", V5Img + "project42.jpg", "Test pumping of boreholes supporting water supply to a major refugee settlement."),
        ("Borehole Construction & Water Permitting", "Borehole Construction & Water Permitting", V5Img + "project43.jpg", "Borehole siting, drilling and water-use permitting services delivered under a repeat engagement."),
        ("Mbale Industrial Park Water & Sewage Masterplan", "Mbale Industrial Park Water & Sewage Masterplan", V5Img + "waterweb.jpg", "Preliminary feasibility studies and designs for a 5,000 m3/day water supply and 5,000 m3/day sewage system, followed by geotechnical, topographic and hydrological assessment."),
        ("St Joseph of Nazareth School Water Programme", "St Joseph of Nazareth School Water Programme", V5Img + "project45.jpg", "Survey, drilling, test pumping, casing and hand-pump installation delivering safe water to a school community, in two phases."),
        ("Kitukiro Rural Water Supply - Pipeline Trenching & Fabrication", "Kitukiro Rural Water Supply: Pipeline Trenching & Fabrication", V5Img + "project46.jpg", "Trenching and backfilling of a 27 km pipeline route, plus fabrication works, for a rural community water-supply scheme."),
        ("Buliisa & Hoima Regional Water Programme", "Buliisa & Hoima Regional Water Programme", V5Img + "borehole.jpg", "150+ borehole siting, drilling, test pumping and rehabilitation assignments completed since 2018 across households, schools, hospitals, hotels, refugee settlements and utilities nationwide."),
        ("Imatong Forest Reserve: land cover change and management zoning", SeedData.ImatongTitle, V5Img + "aerial/drone-orom-03.jpg", "Ten-year change analysis, hot spot statistics, erosion and landslide risk, and management zoning for the reserve and its surrounding landscape."),
    };

    public static async Task ApplyAsync(AppDbContext db, ILogger logger, CancellationToken ct)
    {
        var settings = await db.Settings.FirstOrDefaultAsync(ct);
        if (settings is null || settings.ContentVersion >= SeedData.CurrentContentVersion)
        {
            return;
        }

        var upgraded = new List<string>();
        var skipped = new List<string>();
        void Report(string section, bool done) => (done ? upgraded : skipped).Add(section);

        if (settings.ContentVersion < 2)
        {
            await ApplyV2Async(db, upgraded, skipped, Report, ct);
        }

        var zambia = await db.CountryPages.FirstOrDefaultAsync(p => p.Slug == "zambia", ct);
        if (zambia is not null && (zambia.CtaBody == V2ZambiaCtaBody || zambia.CtaBody == V3ZambiaCtaBody))
        {
            zambia.CtaBody = SeedData.Zambia().CtaBody;
            upgraded.Add("Zambia CTA contact line");
        }

        if (settings.ContentVersion < 5)
        {
            await ApplyV5Async(db, settings, Report, ct);
        }

        if (settings.ContentVersion < 6)
        {
            await ApplyV6Async(db, settings, Report, ct);
        }

        if (settings.ContentVersion < 7)
        {
            await ApplyV7Async(db, settings, Report, ct);
        }

        settings.ContentVersion = SeedData.CurrentContentVersion;
        await db.SaveChangesAsync(ct);

        if (upgraded.Count > 0)
        {
            logger.LogInformation("Content refresh applied to: {Sections}.", string.Join(", ", upgraded));
        }

        if (skipped.Count > 0)
        {
            logger.LogWarning("Content refresh skipped for {Sections} because they have been edited since they were seeded. Update them in the dashboard if you want the new wording.", string.Join(", ", skipped));
        }
    }

    private static async Task ApplyV2Async(AppDbContext db, List<string> upgraded, List<string> skipped, Action<string, bool> Report, CancellationToken ct)
    {
        var services = await db.Services.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync(ct);
        var servicesPristine = services.Select(x => x.Title).SequenceEqual(V1Services);
        if (servicesPristine)
        {
            var next = SeedData.Services();
            for (var i = 0; i < services.Count; i++)
            {
                services[i].Title = next[i].Title;
                services[i].Coverage = next[i].Coverage;
                services[i].Description = next[i].Description;
                services[i].Deliverables = next[i].Deliverables;
                services[i].ImagePath = next[i].ImagePath;
                services[i].UpdatedUtc = DateTime.UtcNow;
            }
        }

        Report("Services", servicesPristine);

        var capabilities = await db.Capabilities.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync(ct);
        var capabilitiesPristine = capabilities.Select(x => x.Title).SequenceEqual(V1Capabilities);
        if (capabilitiesPristine)
        {
            var next = SeedData.Capabilities();
            for (var i = 0; i < capabilities.Count; i++)
            {
                capabilities[i].Title = next[i].Title;
                capabilities[i].Description = next[i].Description;
            }
        }

        Report("Capabilities", capabilitiesPristine);

        var values = await db.CoreValues.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync(ct);
        var valuesPristine = values.Select(x => x.Title).SequenceEqual(V1Values);
        if (valuesPristine)
        {
            var next = SeedData.Values();
            for (var i = 0; i < next.Count; i++)
            {
                if (i < values.Count)
                {
                    values[i].Title = next[i].Title;
                    values[i].Description = next[i].Description;
                }
                else
                {
                    next[i].SortOrder = i + 1;
                    db.CoreValues.Add(next[i]);
                }
            }
        }

        Report("Values", valuesPristine);

        // The SDG documents only fill gaps: an empty path becomes the goal's approach document.
        var documents = SeedData.SdgGoals().ToDictionary(g => g.Number, g => g.DocumentPath);
        foreach (var goal in await db.SdgGoals.Where(g => g.DocumentPath == string.Empty).ToListAsync(ct))
        {
            if (documents.TryGetValue(goal.Number, out var path))
            {
                goal.DocumentPath = path;
            }
        }

        var projects = await db.Projects.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync(ct);
        var projectsPristine = projects.Select(x => x.Title).OrderBy(x => x).SequenceEqual(V1ProjectTitles.OrderBy(x => x));
        if (projectsPristine)
        {
            // The old projects stay (hidden from the Projects page, which now has its own list), so nothing is lost
            // and the country pages can keep featuring them.
            var listed = SeedData.ListProjects();
            var featured = SeedData.CountryCardProjects();
            var oldCards = projects.Where(p => featured.Any(f => f.Title == p.Title)).ToList();
            foreach (var project in projects)
            {
                project.OnListPage = false;
            }

            foreach (var card in featured.Where(f => f.Country == "uganda"))
            {
                Apply(projects.First(p => p.Title == card.Title), card);
            }

            var zambiaOld = projects.Where(p => p.Country == "zambia").ToList();
            var zambiaNew = featured.Where(f => f.Country == "zambia").ToList();
            for (var i = 0; i < zambiaOld.Count && i < zambiaNew.Count; i++)
            {
                Apply(zambiaOld[i], zambiaNew[i]);
            }

            var offset = listed.Count;
            foreach (var project in projects)
            {
                project.SortOrder += offset;
            }

            for (var i = 0; i < listed.Count; i++)
            {
                listed[i].SortOrder = i + 1;
            }

            db.Projects.AddRange(listed);
            _ = oldCards;
        }

        Report("Projects", projectsPristine);

        var pages = await db.CountryPages.Include(p => p.Services).Include(p => p.Projects).ToListAsync(ct);
        var uganda = pages.FirstOrDefault(p => p.Slug == "uganda");
        var zambia = pages.FirstOrDefault(p => p.Slug == "zambia");
        var pagesPristine = uganda is not null && zambia is not null
            && uganda.Facts.Contains(V1UgandaFacts) && zambia.Facts.Contains(V1ZambiaFacts)
            && servicesPristine && projectsPristine;
        if (pagesPristine)
        {
            var freshUganda = SeedData.Uganda();
            var freshZambia = SeedData.Zambia();
            uganda!.Facts = freshUganda.Facts;
            zambia!.Facts = freshZambia.Facts;
            zambia.SectionTwoTitle = freshZambia.SectionTwoTitle;
            zambia.CtaBody = freshZambia.CtaBody;

            db.CountryPageServices.RemoveRange(uganda.Services);
            db.CountryPageServices.RemoveRange(zambia.Services);
            uganda.Services = DatabaseInitializer.ServiceCards(SeedData.UgandaServiceCards(), services);
            zambia.Services = DatabaseInitializer.ServiceCards(SeedData.ZambiaServiceCards(), services);

            // The cards keep pointing at the same projects; only their wording moves onto the projects themselves.
            foreach (var placement in uganda.Projects.Concat(zambia.Projects))
            {
                placement.Tag = string.Empty;
                placement.Meta = string.Empty;
            }
        }
        else if (uganda is not null && zambia is not null)
        {
            skipped.Add("Country pages");
        }

        if (pagesPristine)
        {
            upgraded.Add("Country pages");
        }
    }

    private static async Task ApplyV5Async(AppDbContext db, SiteSettings settings, Action<string, bool> report, CancellationToken ct)
    {
        var capabilities = await db.Capabilities.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync(ct);
        var capabilitiesPristine = capabilities.Select(x => (x.Title, x.Description)).SequenceEqual(V4Capabilities);
        if (capabilitiesPristine)
        {
            var next = SeedData.Capabilities();
            for (var i = 0; i < capabilities.Count; i++)
            {
                capabilities[i].Title = next[i].Title;
                capabilities[i].Description = next[i].Description;
                capabilities[i].UpdatedUtc = DateTime.UtcNow;
            }
        }

        report("Service lines (Who We Are)", capabilitiesPristine);

        var values = await db.CoreValues.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync(ct);
        var nextValues = SeedData.Values();
        var valuesPristine = values.Count == nextValues.Count && values.Zip(nextValues).All(p =>
            p.First.Title == p.Second.Title && p.First.Description == V4ValueText.GetValueOrDefault(p.Second.Title, p.Second.Description));
        if (valuesPristine)
        {
            for (var i = 0; i < values.Count; i++)
            {
                values[i].Description = nextValues[i].Description;
                values[i].UpdatedUtc = DateTime.UtcNow;
            }
        }

        report("Values", valuesPristine);

        var missionPristine = settings.Mission == V4Mission;
        if (missionPristine)
        {
            settings.Mission = SeedData.Mission;
        }

        report("Mission", missionPristine);

        // The page text is new, so there is nothing to overwrite.
        SeedData.ApplyWhoWeAreText(settings);
    }

    private static async Task ApplyV6Async(AppDbContext db, SiteSettings settings, Action<string, bool> report, CancellationToken ct)
    {
        // The Imatong project backs the climate section's "In practice" card.
        var projects = await db.Projects.ToListAsync(ct);
        if (projects.All(p => p.Title != SeedData.ImatongTitle))
        {
            var imatong = SeedData.Imatong();
            imatong.SortOrder = projects.Count == 0 ? 1 : projects.Max(p => p.SortOrder) + 1;
            db.Projects.Add(imatong);
            projects.Add(imatong);
        }

        var seeded = SeedData.Services().ToDictionary(s => s.Title);
        var previous = V5Services().ToDictionary(s => s.Title);
        var services = await db.Services.ToListAsync(ct);
        var allPristine = true;
        foreach (var service in services)
        {
            if (!seeded.TryGetValue(service.Title, out var next))
            {
                // A service added in the dashboard still needs an anchor for the jump menu and footer.
                if (service.Slug.Length == 0)
                {
                    service.Slug = Slugify(service.Title);
                    service.ShortTitle = service.Title;
                    service.Layout = "split";
                }

                continue;
            }

            // The new fields are empty on every existing service, so filling them overwrites nothing.
            if (service.Slug.Length == 0)
            {
                service.Slug = next.Slug;
                service.ShortTitle = next.ShortTitle;
                service.Tagline = next.Tagline;
                service.Layout = next.Layout;
                service.TileImagePath = next.TileImagePath;
                service.TileItems = next.TileItems;
                service.Highlights = next.Highlights;
                service.StatValue = next.StatValue;
                service.StatLabel = next.StatLabel;
            }

            if (service.PracticeProjectId is null && SeedData.PracticeProjects.TryGetValue(next.Slug, out var practiceTitle))
            {
                service.PracticeProject = projects.FirstOrDefault(p => p.Title == practiceTitle);
            }

            var old = previous[service.Title];
            var pristine = service.Description == old.Description && service.ImagePath == old.ImagePath && service.Deliverables.SequenceEqual(old.Deliverables);
            if (pristine)
            {
                service.Description = next.Description;
                service.ImagePath = next.ImagePath;
                service.Deliverables = next.Deliverables;
            }

            allPristine &= pristine;
            service.UpdatedUtc = DateTime.UtcNow;
        }

        report("Services text", allPristine);

        var links = SeedData.Capabilities().ToDictionary(c => c.Title, c => c.LinkUrl);
        foreach (var capability in await db.Capabilities.Where(c => c.LinkUrl == string.Empty).ToListAsync(ct))
        {
            capability.LinkUrl = links.GetValueOrDefault(capability.Title, "/services");
        }

        // The page text is new, so there is nothing to overwrite.
        SeedData.ApplyServicesText(settings);
    }

    private static async Task ApplyV7Async(AppDbContext db, SiteSettings settings, Action<string, bool> report, CancellationToken ct)
    {
        var seeded = SeedData.ListProjects().ToDictionary(p => p.Title);
        var previous = V6Projects.ToDictionary(p => p.OldTitle);
        var projects = await db.Projects.ToListAsync(ct);
        var allPristine = true;
        foreach (var project in projects)
        {
            // A project already under its new title (Imatong, added by version 6) only needs its new fields.
            Project? next;
            if (previous.TryGetValue(project.Title, out var old))
            {
                next = seeded[old.NewTitle];
                var pristine = project.Description == old.Description && project.ImagePath == old.ImagePath;
                if (pristine)
                {
                    project.Title = next.Title;
                    project.Description = next.Description;
                    project.ImagePath = next.ImagePath;
                    project.Meta = next.Meta;
                    project.Country = next.Country;
                    project.Service = next.Service;
                }

                allPristine &= pristine;
            }
            else if (!seeded.TryGetValue(project.Title, out next))
            {
                // Country-page cards and projects added in the dashboard keep what they have; their one service carries over.
                if (project.Services.Count == 0)
                {
                    project.Services = new() { project.Service };
                }

                continue;
            }

            // The new fields are empty on every existing project, so filling them overwrites nothing.
            if (project.Services.Count == 0)
            {
                project.Client = next.Client;
                project.Location = next.Location;
                project.Sector = next.Sector;
                project.Ongoing = next.Ongoing;
                project.Services = next.Services.Contains(project.Service) ? next.Services : next.Services.Prepend(project.Service).ToList();
                project.RepresentativePhoto = next.RepresentativePhoto;
                project.FlagshipFigure = next.FlagshipFigure;
                project.FlagshipLabel = next.FlagshipLabel;
                project.FlagshipOrder = next.FlagshipOrder;
            }

            project.UpdatedUtc = DateTime.UtcNow;
        }

        report("Projects text and photos", allPristine);

        // Version 6 linked "In practice" projects by their old titles; relink any it missed under the new ones.
        // A link version 6 made in this same run (to the Imatong project it added) is not saved yet, so check the entity too.
        foreach (var service in await db.Services.Where(s => s.PracticeProjectId == null).ToListAsync(ct))
        {
            if (service.PracticeProject is null && SeedData.PracticeProjects.TryGetValue(service.Slug, out var title)
                && projects.FirstOrDefault(p => p.Title == title) is { } match)
            {
                service.PracticeProject = match;
            }
        }

        if (!await db.Sectors.AnyAsync(s => s.Key == "conservation", ct))
        {
            var conservation = SeedData.Sectors().Single(s => s.Key == "conservation");
            conservation.SortOrder = (await db.Sectors.MaxAsync(s => (int?)s.SortOrder, ct) ?? 0) + 1;
            db.Sectors.Add(conservation);
        }

        // The page text is new, so there is nothing to overwrite.
        SeedData.ApplyProjectsText(settings);
    }

    private static string Slugify(string title) =>
        Regex.Replace(title.ToLowerInvariant().Replace("&", "and"), "[^a-z0-9]+", "-").Trim('-');

    private static void Apply(Project target, Project source)
    {
        target.Title = source.Title;
        target.Tag = source.Tag;
        target.Meta = source.Meta;
        target.Country = source.Country;
        target.Service = source.Service;
        target.ImagePath = source.ImagePath;
        target.Description = source.Description;
        target.OnListPage = false;
        target.UpdatedUtc = DateTime.UtcNow;
    }
}
