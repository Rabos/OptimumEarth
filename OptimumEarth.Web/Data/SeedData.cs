namespace OptimumEarth.Web.Data;

/// <summary>
/// The site's content as it was hard-coded in the page models before the
/// dashboard existed. Inserted once, into empty tables, so the site looks the
/// same on day one and everything after is edited through the dashboard.
/// </summary>
public static class SeedData
{
    private const string Img = "/img/optimum-earth-images/";

    public static List<Slide> Slides() => new()
    {
        new() { Eyebrow = "Optimum Earth Uganda", Headline = "Sustainable infrastructure for East and Southern Africa.", Body = "Water, energy, environment, GIS and engineering, delivered since 2017 from our Kampala base.", ImagePath = Img + "heroslide1.jpg" },
        new() { Eyebrow = "Optimum Earth Zambia", Headline = "Regional engineering capability, brought to the Zambian market.", Body = "Energy, water, infrastructure and environmental services for mining, utilities and industry.", ImagePath = Img + "heroslide2.jpg" },
        new() { Eyebrow = "Optimum Earth Foundation", Headline = "Communities designing the infrastructure they need.", Body = "Engineering capability channelled into community-led development, measured by results.", ImagePath = Img + "heroslide3.jpg" },
    };

    public static List<Service> Services() => new()
    {
        new() { Title = "Water supply solutions", Coverage = "Uganda · Zambia", ImagePath = Img + "servicepanel1.jpg",
            Description = "Surface water and groundwater diagnostics, borehole siting and supervision, test pumping, well rehabilitation and long-term supply planning for utilities, industry and institutions." },
        new() { Title = "Groundwater monitoring", Coverage = "Uganda · Zambia", ImagePath = Img + "servicepanel2.jpg",
            Description = "Monitoring well design and installation, water level and quality regimes, data management and reporting that stands up to regulatory scrutiny." },
        new() { Title = "Environmental services", Coverage = "Uganda", ImagePath = Img + "servicepanel3.jpg",
            Description = "Environmental and social impact assessment, audits, management plans and ongoing compliance support for regulated and lender-financed projects." },
        new() { Title = "GIS, mapping & remote sensing", Coverage = "Uganda · Zambia", ImagePath = Img + "servicepanel4.jpg",
            Description = "Geo-intelligence products for water, energy, humanitarian relief and infrastructure - spatial analysis, basemaps, dashboards and field data systems." },
        new() { Title = "Solutions for mines, oil & gas", Coverage = "Uganda · Zambia", ImagePath = Img + "servicepanel5.jpg",
            Description = "Integrated water management for mineral, metal and aggregate operations, plus geothermal consulting and hydrogeology for the extractives sector." },
    };

    /// <summary>The ten cards on the Projects page. The first three are also the Uganda page's featured projects.</summary>
    public static List<Project> UgandaProjects() => new()
    {
        P("Uganda Production Wells Survey", "UGANDA · 2020", "Client : National Water and Sewerage Cooperation", "water", "recentwork1.jpg"),
        P("Kingfisher Monitoring Wells", "UGANDA · 2022", "Client : CNOOC", "groundwater", "recentwork2.jpg"),
        P("Pivot Irrigation ESIA Initiative", "UGANDA · 2023", "Client : NASECO", "environment", "recentwork3.jpg"),
        P("Hima Quarry Water Management", "UGANDA", "Client : Hima Cement", "groundwater", "hima-quarry.jpg"),
        P("Makuutu Hydrogeology Programme", "UGANDA", "Client : Geothermal consulting · GIS & mapping", "gis", "makuttu.jpg"),
        P("Quarry Site ERT Survey", "UGANDA", "Client : Agaba South West Services", "gis", "servicepanel4.jpg"),
        P("Uganda Borehole Testing Initiative", "UGANDA", "Client: EPM Engineering Consults(U) Ltd", "water", "borehole.jpg"),
        P("Enhancing Climate Resilient WASH Initiatives", "UGANDA · 2021/2022", "Client: UNICEF / SGI – Studio Galli Ingegneria", "groundwater", "enhancing-climate.jpg"),
        P("Pivot Irrigation Development Assessment", "UGANDA · 2021/2022", "Client: NASECO", "gis", "pivot-irrigation.jpg"),
        P("Mbale Industrial Infrastructure Consultancy", "UGANDA", "Client: Tangshan Mbale Industrial Park", "water", "waterweb.jpg"),
    };

    /// <summary>
    /// The Zambia page shows three stand-in cards until real case studies exist.
    /// They are kept as projects (hidden from the Projects page) so they can be
    /// edited into the real thing, or swapped for other projects, in the dashboard.
    /// </summary>
    public static List<Project> ZambiaPlaceholders() => new()
    {
        new() { Title = "Project to be confirmed", Tag = "ZAMBIA", Meta = "Case study to be published as evidence becomes available", Country = "zambia", Service = "water", ImagePath = Img + "project4.jpg", OnListPage = false },
        new() { Title = "Project to be confirmed", Tag = "ZAMBIA", Meta = "Case study to be published as evidence becomes available", Country = "zambia", Service = "water", ImagePath = Img + "project5.jpg", OnListPage = false },
        new() { Title = "Regional experience applies here", Tag = "REGIONAL", Meta = "Delivery record from Uganda and the wider region", Country = "zambia", Service = "water", ImagePath = Img + "project6.jpg", OnListPage = false },
    };

    private static Project P(string title, string tag, string meta, string service, string image) =>
        new() { Title = title, Tag = tag, Meta = meta, Country = "uganda", Service = service, ImagePath = Img + image };

    public static List<Capability> Capabilities() => new()
    {
        new() { Title = "Water resources", Description = "Supply, diagnostics, monitoring" },
        new() { Title = "Energy", Description = "Geothermal and infrastructure" },
        new() { Title = "Environment", Description = "Assessment, audit, compliance" },
        new() { Title = "GIS & mapping", Description = "Spatial analysis and mapping" },
        new() { Title = "Engineering", Description = "Full project lifecycle" },
    };

    public static List<CoreValue> Values() => new()
    {
        new() { Title = "Culture", Description = "We will cultivate a culture that values the contributions of our diverse and talented team members. We will seek out and encourage colleagues who are passionate, curious and client-focused. We will maintain an environment of mutual respect and commitment to professional development." },
        new() { Title = "Integrity", Description = "We will act with the highest ethics, honesty, and respect in all business dealings. Integrity is at the heart of who we are and what we do. We will treat customers and company resources with the respect they deserve. We will do the right thing." },
        new() { Title = "Innovation", Description = "We will apply technology and evolve our solutions to fit the needs of our clients. We will attack complacency and continually improve to foster the development of new and creative solutions." },
        new() { Title = "Safety and Health", Description = "Safety and Health are our top priority. We expect everyone to actively participate in and take responsibility for their own safety, the safety of the public, and the safety of those around them. Clients entrust us to keep safety first in every service we provide." },
        new() { Title = "Quality", Description = "We will be passionate about excellence and doing our work right the first time. Quality is a shared responsibility. We will hold each other accountable and maintain a reputation for delivering." },
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
        new() { Number = "06", Label = "Clean Water & Sanitation" },
        new() { Number = "07", Label = "Affordable & Clean Energy" },
        new() { Number = "13", Label = "Climate Action" },
        new() { Number = "17", Label = "Partnerships for the Goals" },
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
        G("communities", "Makuutu: the site before work began", "makuttu.jpg"),
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
        Facts = new() { "Established | 2017", "Office | Kampala", "Service lines | 5", "Sectors | Water · Energy · Mining" },
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
        Facts = new() { "Status | Establishing", "Office | To be confirmed", "Service lines | 4", "Sectors | Mining · Energy · Water" },
        SectionOneTitle = "Services in Zambia", SectionOneLinkText = "All services", SectionOneLinkHref = "/services",
        SectionTwoTitle = "Projects & pipeline", SectionTwoLinkText = "All projects", SectionTwoLinkHref = "/projects",
        SectionTwoNote = "Zambia case studies will be published as projects are delivered and client permissions are confirmed.",
        CtaTitle = "Work with our Zambia team",
        CtaBody = "Office address to be confirmed · zambia@optimum-earth.com",
        CtaServices = new() { "Water supply solutions", "Energy & infrastructure", "Groundwater monitoring", "GIS & mapping" },
    };
}
