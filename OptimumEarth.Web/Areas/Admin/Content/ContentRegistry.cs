using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Areas.Admin.Content;

/// <summary>Every content type the generic list and edit pages manage, declared in one place.</summary>
public static class ContentRegistry
{
    // The project categories and countries are the filters on the public Projects page.
    private static readonly IReadOnlyList<(string, string)> Services = new[]
    {
        ("construction", "Construction Services"),
        ("technical", "Technical Studies"),
        ("engineering", "Engineering Design"),
        ("environmental", "Environmental & Social Impact"),
        ("climate", "Climate Resilience"),
    };

    private static readonly IReadOnlyList<(string, string)> Countries = new[]
    {
        ("uganda", "Uganda"), ("zambia", "Zambia"), ("burundi", "Burundi"), ("drc", "DRC"), ("southsudan", "South Sudan"),
    };

    private static readonly IReadOnlyList<(string, string)> ServiceLayouts = new[]
    {
        ("split", "Photo left, text right"), ("split-alt", "Tinted, photo right"), ("band", "Full-width photo band"), ("case-study", "Imatong map case study"),
    };

    private static readonly IReadOnlyList<(string, string)> StatKinds = new[]
    {
        ("number", "A fixed number"), ("years", "Years since founding"),
    };

    private const string KeyPattern = "^[a-z0-9-]+$";
    private const string KeyMessage = "Use lowercase letters, numbers and hyphens only.";

    private static string Like(string s) => "%" + s.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";

    private static string Label(IEnumerable<(string Value, string Label)> options, string value) =>
        options.FirstOrDefault(o => o.Value == value).Label ?? value;

    public static readonly IReadOnlyList<IContentDescriptor> All = Build();

    public static IContentDescriptor? Find(string? slug) => All.FirstOrDefault(d => string.Equals(d.Slug, slug, StringComparison.OrdinalIgnoreCase));

    private static IReadOnlyList<IContentDescriptor> Build() => new IContentDescriptor[]
    {
        new ContentDescriptor<Slide>
        {
            Slug = "slides", Label = "Hero slides", Singular = "Slide", Area = AdminAreas.Home,
            Description = "The slides at the top of the Home page, in the order they rotate.",
            Set = db => db.Slides, TitleOf = x => x.Headline,
            Fields = new FieldDef[]
            {
                new() { Key = nameof(Slide.Eyebrow), Label = "Eyebrow", Required = true, Max = 40, Hint = "Names the destination, e.g. Optimum Earth Uganda." },
                new() { Key = nameof(Slide.Headline), Label = "Headline", Kind = FieldKind.Area, Required = true, Max = 100, Rows = 2 },
                new() { Key = nameof(Slide.Body), Label = "Body", Kind = FieldKind.Area, Max = 180, Rows = 3 },
                new() { Key = nameof(Slide.ImagePath), Label = "Image", Kind = FieldKind.Image, Required = true },
            },
            Columns = new ColumnDef<Slide>[]
            {
                new() { Header = "Slide", Html = (x, _) => Cell.Thumb(x.ImagePath, x.Headline) },
                new() { Header = "Eyebrow", Html = (x, _) => Cell.E(x.Eyebrow) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Headline, Like(s)) || EF.Functions.ILike(x.Eyebrow, Like(s))),
        },

        new ContentDescriptor<Project>
        {
            Slug = "projects", Label = "Projects", Singular = "Project", Area = AdminAreas.Projects,
            Description = "Project cards. Choose where each one appears: the Projects page, and the Uganda or Zambia page.",
            Set = db => db.Projects, TitleOf = x => x.Title,
            Fields = new FieldDef[]
            {
                new() { Key = nameof(Project.Title), Label = "Title", Required = true, Max = 90 },
                new() { Key = nameof(Project.Tag), Label = "Tag line", Half = true, Max = 30, Hint = "e.g. UGANDA · 2022" },
                new() { Key = nameof(Project.Country), Label = "Country", Kind = FieldKind.Select, Options = Countries, Half = true },
                new() { Key = nameof(Project.Meta), Label = "Client / meta line", Max = 120 },
                new() { Key = nameof(Project.Service), Label = "Category", Kind = FieldKind.Select, Options = Services, Half = true },
                new() { Key = nameof(Project.ImagePath), Label = "Image", Kind = FieldKind.Image, Required = true, Half = true },
                new() { Key = nameof(Project.Description), Label = "Description", Kind = FieldKind.Area, Max = 600, Rows = 4, Hint = "The fuller write-up: shown in the project's detail dialog and under its card on the country pages." },
                new() { Key = "shownOn", Label = "Appears on", Kind = FieldKind.Placements },
            },
            Columns = new ColumnDef<Project>[]
            {
                new() { Header = "Project", Html = (x, _) => Cell.Thumb(x.ImagePath, x.Title, x.Tag) },
                new() { Header = "Category", Html = (x, _) => Cell.E(Label(Services, x.Service)) },
                new() { Header = "Country", Html = (x, _) => Cell.E(Label(Countries, x.Country)) },
                new() { Header = "Shown on", Html = (x, ctx) => ShownOn(x.Id, x.OnListPage, "Projects", ctx) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Title, Like(s)) || EF.Functions.ILike(x.Meta, Like(s)) || EF.Functions.ILike(x.Tag, Like(s))),
            FilterDefs = new FilterDef<Project>[]
            {
                new() { Key = "country", Label = "Country", Options = Countries, Apply = (q, v) => q.Where(x => x.Country == v) },
                new() { Key = "service", Label = "Service", Options = Services, Apply = (q, v) => q.Where(x => x.Service == v) },
            },
            PrepareColumns = async (db, rows) =>
            {
                var ids = rows.Select(r => r.Id).ToList();
                var placed = await db.CountryPageProjects.AsNoTracking().Where(p => ids.Contains(p.ProjectId))
                    .Select(p => new { p.ProjectId, p.CountryPage!.Name }).ToListAsync();
                return placed.GroupBy(p => p.ProjectId).ToDictionary(g => g.Key, g => g.Select(p => p.Name).ToList());
            },
            Hooks = new ProjectHooks(),
        },

        new ContentDescriptor<Service>
        {
            Slug = "services", Label = "Services", Singular = "Service", Area = AdminAreas.Services,
            Description = "The service lines. Choose where each one appears: the Services page, and the Uganda or Zambia page.",
            Set = db => db.Services, TitleOf = x => x.Title,
            Fields = new FieldDef[]
            {
                new() { Key = nameof(Service.Title), Label = "Title", Required = true, Max = 70, Half = true },
                new() { Key = nameof(Service.ShortTitle), Label = "Short title", Required = true, Max = 30, Half = true, Hint = "For the tiles, jump menu and footer." },
                new() { Key = nameof(Service.Slug), Label = "Anchor", Required = true, Max = 40, Half = true, Pattern = KeyPattern, PatternMessage = KeyMessage, Hint = "Links to the section as /services#anchor." },
                new() { Key = nameof(Service.Coverage), Label = "Coverage", Half = true, Max = 60, Hint = "e.g. Uganda · Zambia" },
                new() { Key = nameof(Service.Tagline), Label = "Headline", Required = true, Max = 80 },
                new() { Key = nameof(Service.Description), Label = "Intro", Kind = FieldKind.Area, Required = true, Max = 1000, Rows = 4 },
                new() { Key = nameof(Service.Highlights), Label = "Headline capabilities", Kind = FieldKind.Lines, Rows = 4, Hint = "Four short lines, shown with icons." },
                new() { Key = nameof(Service.Deliverables), Label = "All capabilities", Kind = FieldKind.Lines, Rows = 8, Hint = "One per line, shown under \"See all\"." },
                new() { Key = nameof(Service.Layout), Label = "Section layout", Kind = FieldKind.Select, Options = ServiceLayouts, Half = true, Group = "Services page section" },
                new() { Key = nameof(Service.ImagePath), Label = "Section photo", Kind = FieldKind.Image, Required = true, Half = true },
                new() { Key = nameof(Service.StatValue), Label = "Photo badge figure", Max = 20, Half = true, Hint = "e.g. 12 regions. Leave empty for no badge." },
                new() { Key = nameof(Service.StatLabel), Label = "Photo badge text", Max = 80, Half = true },
                new() { Key = nameof(Service.PracticeProjectId), Label = "\"In practice\" project", Kind = FieldKind.Select, OptionsSource = "projects", Hint = "Its photo, description and client show under the section." },
                new() { Key = nameof(Service.TileImagePath), Label = "Tile photo", Kind = FieldKind.Image, Required = true, Half = true, Group = "Overview tile" },
                new() { Key = nameof(Service.TileItems), Label = "Tile points", Kind = FieldKind.Lines, Rows = 3, Half = true, Hint = "Three short lines." },
                new() { Key = "shownOn", Label = "Appears on", Kind = FieldKind.Placements },
            },
            Columns = new ColumnDef<Service>[]
            {
                new() { Header = "Service", Html = (x, _) => Cell.Thumb(x.ImagePath, x.Title) },
                new() { Header = "Coverage", Html = (x, _) => Cell.E(x.Coverage) },
                new() { Header = "Shown on", Html = (x, ctx) => ShownOn(x.Id, x.OnListPage, "Services", ctx) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Title, Like(s)) || EF.Functions.ILike(x.Description, Like(s))),
            Unique = new[] { nameof(Service.Slug) },
            PrepareColumns = async (db, rows) =>
            {
                var ids = rows.Select(r => r.Id).ToList();
                var placed = await db.CountryPageServices.AsNoTracking().Where(p => p.ServiceId != null && ids.Contains(p.ServiceId.Value))
                    .Select(p => new { Id = p.ServiceId!.Value, p.CountryPage!.Name }).ToListAsync();
                return placed.GroupBy(p => p.Id).ToDictionary(g => g.Key, g => g.Select(p => p.Name).ToList());
            },
            Hooks = new ServiceHooks(),
        },

        new ContentDescriptor<SiteSettings>
        {
            Slug = "mission", Label = "Mission & vision", Singular = "Statement", Area = AdminAreas.About, Single = true, CanAdd = false, CanDelete = false, Orderable = false,
            Description = "The two statements on the Who We Are page.",
            Set = db => db.Settings, TitleOf = _ => "Mission & vision",
            Fields = new FieldDef[]
            {
                new() { Key = nameof(SiteSettings.Mission), Label = "Mission", Kind = FieldKind.Area, Required = true, Max = 600, Rows = 6 },
                new() { Key = nameof(SiteSettings.Vision), Label = "Vision", Kind = FieldKind.Area, Required = true, Max = 300, Rows = 4 },
            },
            Columns = Array.Empty<ColumnDef<SiteSettings>>(),
        },

        new ContentDescriptor<SiteSettings>
        {
            Slug = "services-page", Label = "Page text", Singular = "Page text", Area = AdminAreas.Services, Single = true, CanAdd = false, CanDelete = false, Orderable = false,
            Description = "The hero, drone strip, Zambia equipment hire, Imatong case study and closing photo on the Services page.",
            Set = db => db.Settings, TitleOf = _ => "Services page text",
            Fields = new FieldDef[]
            {
                new() { Key = nameof(SiteSettings.ServicesHeadline), Label = "Headline", Required = true, Max = 100, Group = "Hero" },
                new() { Key = nameof(SiteSettings.ServicesLede), Label = "Intro", Kind = FieldKind.Area, Required = true, Max = 320, Rows = 3 },
                new() { Key = nameof(SiteSettings.ServicesPosterPath), Label = "Still image", Kind = FieldKind.Image, Required = true, Hint = "Shown behind the hero until the drone video plays, and instead of it on phones." },
                new() { Key = nameof(SiteSettings.AerialHeadline), Label = "Headline", Required = true, Max = 80, Group = "From the air" },
                new() { Key = nameof(SiteSettings.AerialBody), Label = "Text", Kind = FieldKind.Area, Required = true, Max = 300, Rows = 3, Hint = "The photos are under Drone photos." },
                new() { Key = nameof(SiteSettings.FleetHeadline), Label = "Headline", Required = true, Max = 80, Group = "Plant & equipment hire (Zambia)" },
                new() { Key = nameof(SiteSettings.FleetBody), Label = "Text", Kind = FieldKind.Area, Required = true, Max = 400, Rows = 4 },
                new() { Key = nameof(SiteSettings.FleetItems), Label = "Points", Kind = FieldKind.Lines, Rows = 4, Hint = "One per line." },
                new() { Key = nameof(SiteSettings.FleetImagePath), Label = "Main photo", Kind = FieldKind.Image, Required = true, Half = true },
                new() { Key = nameof(SiteSettings.FleetInsetImagePath), Label = "Second photo", Kind = FieldKind.Image, Required = true, Half = true },
                new() { Key = nameof(SiteSettings.CaseStudyStat), Label = "Headline figure", Required = true, Max = 20, Half = true, Group = "Imatong case study" },
                new() { Key = nameof(SiteSettings.CaseStudyStatText), Label = "Figure text", Kind = FieldKind.Area, Required = true, Max = 200, Rows = 2 },
                new() { Key = nameof(SiteSettings.CaseStudyCaption), Label = "Map caption", Kind = FieldKind.Area, Required = true, Max = 300, Rows = 2 },
                new() { Key = nameof(SiteSettings.ServicesCtaImagePath), Label = "Background photo", Kind = FieldKind.Image, Required = true, Group = "Closing \"Not sure which service\"" },
                new() { Key = nameof(SiteSettings.ShowClientLogos), Label = "Show the client logo strip", Kind = FieldKind.Bool, Group = "Clients", Hint = "Turn on once the clients have agreed to their logos being shown. The logos are under Clients." },
            },
            Columns = Array.Empty<ColumnDef<SiteSettings>>(),
        },

        new ContentDescriptor<Sector>
        {
            Slug = "sectors", Label = "Sectors", Singular = "Sector", Area = AdminAreas.Services,
            Description = "The \"Who we work for\" tiles on the Services page. Each opens the Projects page filtered to its key.",
            Set = db => db.Sectors, TitleOf = x => x.Name, AutoSlug = (nameof(Sector.Name), nameof(Sector.Key)),
            Unique = new[] { nameof(Sector.Key) },
            Fields = new FieldDef[]
            {
                new() { Key = nameof(Sector.Name), Label = "Name", Required = true, Max = 40, Half = true },
                new() { Key = nameof(Sector.Key), Label = "Key", Max = 30, Half = true, Pattern = KeyPattern, PatternMessage = KeyMessage, Hint = "Filled in from the name. Used in the Projects page link." },
                new() { Key = nameof(Sector.Blurb), Label = "Line under the name", Max = 90 },
                new() { Key = nameof(Sector.ImagePath), Label = "Photo", Kind = FieldKind.Image, Required = true },
            },
            Columns = new ColumnDef<Sector>[]
            {
                new() { Header = "Sector", Html = (x, _) => Cell.Thumb(x.ImagePath, x.Name, x.Blurb) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Name, Like(s)) || EF.Functions.ILike(x.Blurb, Like(s))),
        },

        new ContentDescriptor<Client>
        {
            Slug = "clients", Label = "Clients", Singular = "Client", Area = AdminAreas.Services,
            Description = "The logo strip on the Services page. It only shows once it is switched on in the Services page text.",
            Set = db => db.Clients, TitleOf = x => x.Name,
            Fields = new FieldDef[]
            {
                new() { Key = nameof(Client.Name), Label = "Name", Required = true, Max = 60, Hint = "Read out by screen readers in place of the logo." },
                new() { Key = nameof(Client.LogoPath), Label = "Logo", Kind = FieldKind.Image, Required = true, Hint = "A PNG with a transparent background works best." },
            },
            Columns = new ColumnDef<Client>[]
            {
                new() { Header = "Client", Html = (x, _) => Cell.Thumb(x.LogoPath, x.Name) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Name, Like(s))),
        },

        new ContentDescriptor<AerialPhoto>
        {
            Slug = "aerial", Label = "Drone photos", Singular = "Drone photo", Area = AdminAreas.Services,
            Description = "The \"From the air\" photo strip on the Services page.",
            Set = db => db.AerialPhotos, TitleOf = x => x.Caption,
            Fields = new FieldDef[]
            {
                new() { Key = nameof(AerialPhoto.Caption), Label = "Caption", Required = true, Max = 80 },
                new() { Key = nameof(AerialPhoto.ImagePath), Label = "Photo", Kind = FieldKind.Image, Required = true },
            },
            Columns = new ColumnDef<AerialPhoto>[]
            {
                new() { Header = "Photo", Html = (x, _) => Cell.Thumb(x.ImagePath, x.Caption) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Caption, Like(s))),
        },

        new ContentDescriptor<SiteSettings>
        {
            Slug = "who-page", Label = "Page text", Singular = "Page text", Area = AdminAreas.About, Single = true, CanAdd = false, CanDelete = false, Orderable = false,
            Description = "The hero, story, \"Where we work\" and closing call to action on the Who We Are page.",
            Set = db => db.Settings, TitleOf = _ => "Who We Are page text",
            Fields = new FieldDef[]
            {
                new() { Key = nameof(SiteSettings.WhoHeadline), Label = "Headline", Required = true, Max = 80, Half = true, Group = "Hero" },
                new() { Key = nameof(SiteSettings.WhoHighlight), Label = "Highlighted line", Max = 80, Half = true, Hint = "Follows the headline in the accent colour." },
                new() { Key = nameof(SiteSettings.WhoLede), Label = "Intro", Kind = FieldKind.Area, Required = true, Max = 320, Rows = 3 },
                new() { Key = nameof(SiteSettings.WhoHeroImagePath), Label = "Hero image", Kind = FieldKind.Image, Required = true },
                new() { Key = nameof(SiteSettings.StoryHeadline), Label = "Headline", Required = true, Max = 90, Group = "Our story" },
                new() { Key = nameof(SiteSettings.StoryBody), Label = "Story", Kind = FieldKind.Area, Required = true, Max = 1500, Rows = 8, Hint = "Leave a blank line between paragraphs." },
                new() { Key = nameof(SiteSettings.StoryImagePath), Label = "Main photo", Kind = FieldKind.Image, Required = true, Half = true },
                new() { Key = nameof(SiteSettings.StoryInsetImagePath), Label = "Inset photo", Kind = FieldKind.Image, Required = true, Half = true },
                new() { Key = nameof(SiteSettings.FoundedOn), Label = "Founded on", Kind = FieldKind.Date, Required = true, Half = true, Hint = "Shown on the story photo, and counts the \"years\" stat." },
                new() { Key = nameof(SiteSettings.WorkCountries), Label = "Countries we work in", Kind = FieldKind.Lines, Required = true, Rows = 6, Group = "Where we work", Hint = "One per line. Countries with an office (see Offices) are highlighted." },
                new() { Key = nameof(SiteSettings.WhoCtaHeadline), Label = "Headline", Required = true, Max = 80, Group = "Closing call to action" },
                new() { Key = nameof(SiteSettings.WhoCtaBody), Label = "Body", Kind = FieldKind.Area, Required = true, Max = 320, Rows = 3 },
                new() { Key = nameof(SiteSettings.WhoCtaImagePath), Label = "Background image", Kind = FieldKind.Image, Required = true },
            },
            Columns = Array.Empty<ColumnDef<SiteSettings>>(),
        },

        new ContentDescriptor<Stat>
        {
            Slug = "stats", Label = "Stats", Singular = "Stat", Area = AdminAreas.About,
            Description = "The counters under the Who We Are hero. Five fit on one row.",
            Set = db => db.Stats, TitleOf = x => x.Label,
            Fields = new FieldDef[]
            {
                new() { Key = nameof(Stat.Label), Label = "Label", Required = true, Max = 50 },
                new() { Key = nameof(Stat.Kind), Label = "Number", Kind = FieldKind.Select, Options = StatKinds, Half = true },
                new() { Key = nameof(Stat.Value), Label = "Value", Kind = FieldKind.Number, Min = 0, MaxValue = 100000, Half = true, Hint = "Ignored for years since founding." },
                new() { Key = nameof(Stat.Suffix), Label = "Suffix", Max = 3, Half = true, Hint = "e.g. +" },
            },
            Columns = new ColumnDef<Stat>[]
            {
                new() { Header = "Stat", Html = (x, _) => Cell.Title(x.Label, x.Kind == "years" ? "Years since founding" : x.Value + x.Suffix) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Label, Like(s))),
        },

        new ContentDescriptor<Milestone>
        {
            Slug = "milestones", Label = "Milestones", Singular = "Milestone", Area = AdminAreas.About,
            Description = "The timeline on the Who We Are page, oldest first. Six fit on one row.",
            Set = db => db.Milestones, TitleOf = x => x.Year + " " + x.Title,
            Fields = new FieldDef[]
            {
                new() { Key = nameof(Milestone.Year), Label = "Year", Required = true, Max = 4, Half = true, Pattern = @"^\d{4}$", PatternMessage = "Use a four-digit year." },
                new() { Key = nameof(Milestone.Title), Label = "Title", Required = true, Max = 40, Half = true },
                new() { Key = nameof(Milestone.Description), Label = "Description", Kind = FieldKind.Area, Required = true, Max = 140, Rows = 2 },
            },
            Columns = new ColumnDef<Milestone>[]
            {
                new() { Header = "Milestone", Html = (x, _) => $"<span class=\"mono\">{Cell.E(x.Year)}</span> &nbsp;<b>{Cell.E(x.Title)}</b>" },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Title, Like(s)) || x.Year == s),
        },

        new ContentDescriptor<GroupCompany>
        {
            Slug = "group", Label = "Group companies", Singular = "Company", Area = AdminAreas.About,
            Description = "The company and Foundation cards in \"The group\" on the Who We Are page.",
            Set = db => db.GroupCompanies, TitleOf = x => x.Name,
            Fields = new FieldDef[]
            {
                new() { Key = nameof(GroupCompany.Name), Label = "Name", Required = true, Max = 60, Half = true },
                new() { Key = nameof(GroupCompany.Kind), Label = "Small line above", Max = 40, Half = true, Hint = "e.g. Uganda · Since 2017" },
                new() { Key = nameof(GroupCompany.Description), Label = "Description", Kind = FieldKind.Area, Required = true, Max = 260, Rows = 3 },
                new() { Key = nameof(GroupCompany.Tags), Label = "Tags", Kind = FieldKind.Lines, Rows = 4, Hint = "One per line." },
                new() { Key = nameof(GroupCompany.LinkText), Label = "Link text", Max = 40, Half = true },
                new() { Key = nameof(GroupCompany.LinkUrl), Label = "Link", Max = 200, Half = true, Pattern = @"^(/\S*|https://\S+)$", PatternMessage = "Use a site path such as /uganda, or an https:// link.", Hint = "e.g. /uganda" },
                new() { Key = nameof(GroupCompany.ImagePath), Label = "Image", Kind = FieldKind.Image, Required = true, Half = true },
                new() { Key = nameof(GroupCompany.IsFoundation), Label = "Foundation style (green card)", Kind = FieldKind.Bool, Half = true },
            },
            Columns = new ColumnDef<GroupCompany>[]
            {
                new() { Header = "Company", Html = (x, _) => Cell.Thumb(x.ImagePath, x.Name, x.Kind) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Name, Like(s)) || EF.Functions.ILike(x.Description, Like(s))),
        },

        new ContentDescriptor<Office>
        {
            Slug = "offices", Label = "Offices", Singular = "Office", Area = AdminAreas.About,
            Description = "The offices in the site footer and under \"Where we work\" on the Who We Are page.",
            Set = db => db.Offices, TitleOf = x => x.Name,
            Fields = new FieldDef[]
            {
                new() { Key = nameof(Office.Name), Label = "Name", Required = true, Max = 60, Half = true },
                new() { Key = nameof(Office.Country), Label = "Country", Required = true, Max = 40, Half = true, Hint = "The footer shows OPTIMUM EARTH and this country." },
                new() { Key = nameof(Office.AddressLines), Label = "Address", Kind = FieldKind.Lines, Required = true, Rows = 4, Hint = "One line per row." },
                new() { Key = nameof(Office.Phone), Label = "Phone", Max = 30, Half = true, Pattern = @"^\+?[0-9 ]+$", PatternMessage = "Use digits and spaces, optionally starting with +." },
                new() { Key = nameof(Office.Email), Label = "Email", Required = true, Max = 180, Half = true, Pattern = @"^\S+@\S+\.\S+$", PatternMessage = "Enter an email address." },
            },
            Columns = new ColumnDef<Office>[]
            {
                new() { Header = "Office", Html = (x, _) => Cell.Title(x.Name, x.Email) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Name, Like(s)) || EF.Functions.ILike(x.Country, Like(s))),
        },

        new ContentDescriptor<Capability>
        {
            Slug = "capabilities", Label = "Service lines", Singular = "Service line", Area = AdminAreas.About,
            Description = "The numbered \"What we do\" cards on the Who We Are page. Numbers follow the order.",
            Set = db => db.Capabilities, TitleOf = x => x.Title,
            Fields = new FieldDef[]
            {
                new() { Key = nameof(Capability.Title), Label = "Title", Required = true, Max = 60 },
                new() { Key = nameof(Capability.Description), Label = "Description", Max = 120 },
                new() { Key = nameof(Capability.LinkUrl), Label = "Explore link", Max = 200, Pattern = @"^(/\S*|https://\S+)$", PatternMessage = "Use a site path such as /services#esia, or an https:// link.", Hint = "e.g. /services#technical-studies" },
            },
            Columns = new ColumnDef<Capability>[]
            {
                new() { Header = "Capability", Html = (x, _) => Cell.Title(x.Title, x.Description) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Title, Like(s)) || EF.Functions.ILike(x.Description, Like(s))),
        },

        new ContentDescriptor<CoreValue>
        {
            Slug = "values", Label = "Values", Singular = "Value", Area = AdminAreas.About,
            Description = "The company values on the Who We Are page.",
            Set = db => db.CoreValues, TitleOf = x => x.Title,
            Fields = new FieldDef[]
            {
                new() { Key = nameof(CoreValue.Title), Label = "Title", Required = true, Max = 40 },
                new() { Key = nameof(CoreValue.Description), Label = "Description", Kind = FieldKind.Area, Required = true, Max = 700, Rows = 6 },
            },
            Columns = new ColumnDef<CoreValue>[]
            {
                new() { Header = "Value", Html = (x, _) => Cell.Title(x.Title, x.Description.Length > 70 ? x.Description[..70] + "…" : x.Description) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Title, Like(s)) || EF.Functions.ILike(x.Description, Like(s))),
        },

        new ContentDescriptor<Pillar>
        {
            Slug = "pillars", Label = "Pillars", Singular = "Pillar", Area = AdminAreas.Foundation,
            Description = "The Foundation's strategic pillars: Access, Resilience, Impact.",
            Set = db => db.Pillars, TitleOf = x => x.Title, AutoSlug = (nameof(Pillar.Title), nameof(Pillar.Key)),
            Fields = new FieldDef[]
            {
                new() { Key = nameof(Pillar.Title), Label = "Title", Required = true, Max = 30, Half = true },
                new() { Key = nameof(Pillar.SdgLabel), Label = "SDG label", Max = 20, Half = true, Hint = "e.g. SDG 6 & 7" },
                new() { Key = nameof(Pillar.Key), Label = "Key", Max = 30, Half = true, Pattern = KeyPattern, PatternMessage = KeyMessage, Hint = "Filled in from the title." },
                new() { Key = nameof(Pillar.Description), Label = "Description", Kind = FieldKind.Area, Required = true, Max = 360, Rows = 4 },
                new() { Key = nameof(Pillar.Items), Label = "Bullet items", Kind = FieldKind.Lines, Required = true, Hint = "One per line." },
            },
            Columns = new ColumnDef<Pillar>[]
            {
                new() { Header = "Pillar", Html = (x, _) => Cell.Title(x.Title, x.SdgLabel) },
                new() { Header = "Items", Html = (x, _) => x.Items.Count.ToString() },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Title, Like(s)) || EF.Functions.ILike(x.Description, Like(s))),
        },

        new ContentDescriptor<Principle>
        {
            Slug = "principles", Label = "How we work", Singular = "Principle", Area = AdminAreas.Foundation,
            Description = "The operating principles. The slug is the link anchor, and also names the principle's image file.",
            Set = db => db.Principles, TitleOf = x => x.Title, AutoSlug = (nameof(Principle.Title), nameof(Principle.Slug)),
            Unique = new[] { nameof(Principle.Slug) },
            Fields = new FieldDef[]
            {
                new() { Key = nameof(Principle.Title), Label = "Title", Required = true, Max = 40, Half = true },
                new() { Key = nameof(Principle.Slug), Label = "Slug", Max = 60, Half = true, Pattern = KeyPattern, PatternMessage = KeyMessage, Hint = "Filled in from the title." },
                new() { Key = nameof(Principle.Description), Label = "Description", Kind = FieldKind.Area, Required = true, Max = 320, Rows = 4 },
            },
            Columns = new ColumnDef<Principle>[]
            {
                new() { Header = "Principle", Html = (x, _) => Cell.Title(x.Title, x.Slug) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Title, Like(s)) || EF.Functions.ILike(x.Description, Like(s))),
        },

        new ContentDescriptor<FocusArea>
        {
            Slug = "focus", Label = "Focus areas", Singular = "Focus area", Area = AdminAreas.Foundation,
            Description = "The focus-area chips on the Foundation page.",
            Set = db => db.FocusAreas, TitleOf = x => x.Label,
            Fields = new FieldDef[] { new() { Key = nameof(FocusArea.Label), Label = "Label", Required = true, Max = 40 } },
            Columns = new ColumnDef<FocusArea>[] { new() { Header = "Focus area", Html = (x, _) => Cell.Title(x.Label) } },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Label, Like(s))),
        },

        new ContentDescriptor<SdgGoal>
        {
            Slug = "sdg", Label = "SDG goals", Singular = "Goal", Area = AdminAreas.Foundation,
            Description = "The Sustainable Development Goals shown in the Foundation's SDG alignment.",
            Set = db => db.SdgGoals, TitleOf = x => x.Label,
            Fields = new FieldDef[]
            {
                new() { Key = nameof(SdgGoal.Number), Label = "Goal number", Required = true, Max = 2, Half = true, Pattern = @"^\d{1,2}$", PatternMessage = "Use one or two digits, e.g. 06." },
                new() { Key = nameof(SdgGoal.Label), Label = "Label", Required = true, Max = 50, Half = true },
                new() { Key = nameof(SdgGoal.DocumentPath), Label = "Approach document (PDF)", Max = 200, Pattern = @"^(/docs/[A-Za-z0-9._/-]+\.pdf|https://\S+\.pdf)$", PatternMessage = "Use a PDF path such as /docs/sdg/OEF_SDG6_APPROACH.docx.pdf, or an https:// link to a PDF.", Hint = "Opens in a lightbox when the goal is clicked. Leave empty for no lightbox. PDFs are added to the site's wwwroot/docs folder." },
            },
            Columns = new ColumnDef<SdgGoal>[]
            {
                new() { Header = "Goal", Html = (x, _) => $"<span class=\"mono\">{Cell.E(x.Number)}</span> &nbsp;<b>{Cell.E(x.Label)}</b>" },
                new() { Header = "Document", Html = (x, _) => string.IsNullOrEmpty(x.DocumentPath) ? Cell.Tag("None") : Cell.Tag("PDF", "t-green") },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Label, Like(s)) || x.Number == s),
        },

        new ContentDescriptor<Story>
        {
            Slug = "stories", Label = "Story tiles", Singular = "Story tile", Area = AdminAreas.Foundation,
            Description = "The Stories & Impact tiles. Each opens a gallery of images (see Story gallery).",
            Set = db => db.Stories, TitleOf = x => x.Title, AutoSlug = (nameof(Story.Title), nameof(Story.Key)),
            Unique = new[] { nameof(Story.Key) },
            Fields = new FieldDef[]
            {
                new() { Key = nameof(Story.Title), Label = "Title", Required = true, Max = 30, Half = true },
                new() { Key = nameof(Story.Key), Label = "Key", Max = 40, Half = true, Pattern = KeyPattern, PatternMessage = KeyMessage, Hint = "Links the tile to its gallery images. Changing it detaches them." },
                new() { Key = nameof(Story.Summary), Label = "Summary", Kind = FieldKind.Area, Required = true, Max = 160, Rows = 2 },
                new() { Key = nameof(Story.Body), Label = "Hover text", Kind = FieldKind.Area, Max = 500, Rows = 5 },
                new() { Key = nameof(Story.CoverPath), Label = "Cover image", Kind = FieldKind.Image, Required = true },
            },
            Columns = new ColumnDef<Story>[]
            {
                new() { Header = "Tile", Html = (x, _) => Cell.Thumb(x.CoverPath, x.Title, x.Key) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Title, Like(s)) || EF.Functions.ILike(x.Summary, Like(s))),
        },

        new ContentDescriptor<GalleryImage>
        {
            Slug = "gallery", Label = "Story gallery", Singular = "Gallery image", Area = AdminAreas.Foundation,
            Description = "The images in each story's lightbox. The first image of a story is the one the visitor sees first.",
            Set = db => db.GalleryImages, TitleOf = x => x.Caption,
            Fields = new FieldDef[]
            {
                new() { Key = nameof(GalleryImage.StoryKey), Label = "Story", Kind = FieldKind.Select, OptionsSource = "stories", Required = true, Half = true },
                new() { Key = nameof(GalleryImage.ImagePath), Label = "Image", Kind = FieldKind.Image, Required = true, Half = true },
                new() { Key = nameof(GalleryImage.Caption), Label = "Caption", Required = true, Max = 90 },
            },
            Columns = new ColumnDef<GalleryImage>[]
            {
                new() { Header = "Image", Html = (x, _) => Cell.Thumb(x.ImagePath, x.Caption) },
                new() { Header = "Story", Html = (x, _) => Cell.E(x.StoryKey) },
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Caption, Like(s))),
            FilterDefs = new FilterDef<GalleryImage>[]
            {
                new() { Key = "story", Label = "Story", OptionsSource = "stories", Apply = (q, v) => q.Where(x => x.StoryKey == v) },
            },
        },

        new ContentDescriptor<BlogCategory>
        {
            Slug = "categories", Label = "Categories", Singular = "Category", Area = AdminAreas.Blog,
            Description = "Each post belongs to one category. A category that still has posts cannot be deleted.",
            Set = db => db.BlogCategories, TitleOf = x => x.Name, AutoSlug = (nameof(BlogCategory.Name), nameof(BlogCategory.Slug)),
            Unique = new[] { nameof(BlogCategory.Slug) },
            Fields = new FieldDef[]
            {
                new() { Key = nameof(BlogCategory.Name), Label = "Name", Required = true, Max = 40, Half = true },
                new() { Key = nameof(BlogCategory.Slug), Label = "Slug", Max = 40, Half = true, Pattern = KeyPattern, PatternMessage = KeyMessage, Hint = "The address: /blog/category/your-slug" },
                new() { Key = nameof(BlogCategory.Description), Label = "Description", Kind = FieldKind.Area, Max = 160, Rows = 2 },
            },
            Columns = new ColumnDef<BlogCategory>[]
            {
                new() { Header = "Category", Html = (x, _) => Cell.Title(x.Name) },
                new() { Header = "Address", Html = (x, _) => $"<span class=\"mono\">/blog/category/{Cell.E(x.Slug)}</span>" },
                new() { Header = "Posts", Html = (x, ctx) => (ctx is Dictionary<int, int> d && d.TryGetValue(x.Id, out var n) ? n : 0).ToString() },
            },
            PrepareColumns = async (db, rows) =>
            {
                var ids = rows.Select(r => r.Id).ToList();
                return await db.BlogPosts.AsNoTracking().Where(p => ids.Contains(p.CategoryId)).GroupBy(p => p.CategoryId).Select(g => new { g.Key, N = g.Count() }).ToDictionaryAsync(g => g.Key, g => g.N);
            },
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Name, Like(s))),
            Hooks = new CategoryHooks(),
        },

        new ContentDescriptor<BlogPost>
        {
            Slug = "posts", Label = "Posts", Singular = "Post", Area = AdminAreas.Blog, Orderable = false,
            Description = "Blog articles. A post dated in the future is scheduled and goes live on its date.",
            Set = db => db.BlogPosts, TitleOf = x => x.Title, AutoSlug = (nameof(BlogPost.Title), nameof(BlogPost.Slug)),
            Unique = new[] { nameof(BlogPost.Slug) },
            Fields = new FieldDef[]
            {
                new() { Key = nameof(BlogPost.Title), Label = "Title", Required = true, Max = 120 },
                new() { Key = nameof(BlogPost.Slug), Label = "Slug", Max = 80, Half = true, Pattern = KeyPattern, PatternMessage = KeyMessage, Hint = "The address: /blog/your-slug. Filled in from the title." },
                new() { Key = nameof(BlogPost.PublishDate), Label = "Publish date", Kind = FieldKind.Date, Required = true, Half = true, Hint = "A future date schedules the post." },
                new() { Key = nameof(BlogPost.CategoryId), Label = "Category", Kind = FieldKind.Select, OptionsSource = "categories", Required = true, Half = true },
                new() { Key = nameof(BlogPost.AuthorName), Label = "Author", Required = true, Max = 80, Half = true },
                new() { Key = nameof(BlogPost.CoverPath), Label = "Cover image", Kind = FieldKind.Image },
                new() { Key = nameof(BlogPost.Excerpt), Label = "Excerpt", Kind = FieldKind.Area, Required = true, Max = 200, Rows = 3, Hint = "Shown on the blog list and in search results." },
                new() { Key = nameof(BlogPost.Body), Label = "Article", Kind = FieldKind.Markdown, Required = true },
                new() { Key = nameof(BlogPost.Tags), Label = "Tags", Kind = FieldKind.Lines, Rows = 3, Hint = "One per line." },
                new() { Key = nameof(BlogPost.Pinned), Label = "Pin to the top of the blog", Kind = FieldKind.Bool },
                new() { Key = nameof(BlogPost.SeoTitle), Label = "Search title", Max = 60, Group = "Search and sharing", Hint = "Leave empty to use the post title." },
                new() { Key = nameof(BlogPost.SeoDescription), Label = "Search description", Kind = FieldKind.Area, Max = 160, Rows = 2, Hint = "Leave empty to use the excerpt." },
            },
            Columns = new ColumnDef<BlogPost>[]
            {
                new() { Header = "Post", Html = (x, _) => Cell.Thumb(x.CoverPath, x.Title, x.PublishDate.ToString("d MMM yyyy") + (x.Pinned ? " · pinned" : "")) },
                new() { Header = "Category", Html = (x, ctx) => Cell.E(ctx is Dictionary<int, string> d && d.TryGetValue(x.CategoryId, out var n) ? n : "") },
                new() { Header = "Author", Html = (x, _) => Cell.E(x.AuthorName) },
            },
            PrepareColumns = async (db, _) => await db.BlogCategories.AsNoTracking().ToDictionaryAsync(c => c.Id, c => c.Name),
            Search = (q, s) => q.Where(x => EF.Functions.ILike(x.Title, Like(s)) || EF.Functions.ILike(x.Excerpt, Like(s))),
            FilterDefs = new FilterDef<BlogPost>[]
            {
                new() { Key = "category", Label = "Category", OptionsSource = "categories", Apply = (q, v) => q.Where(x => x.CategoryId == int.Parse(v)) },
            },
            CustomStatuses = new[] { new StatusFilter("published", "Published"), new StatusFilter("scheduled", "Scheduled"), new StatusFilter("draft", "Drafts") },
            StatusKeyOf = x => x.Status == ContentStatus.Draft ? "draft" : x.PublishDate > Today() ? "scheduled" : "published",
            StatusQuery = (q, key) =>
            {
                var today = Today();
                return key switch
                {
                    "draft" => q.Where(x => x.Status == ContentStatus.Draft),
                    "scheduled" => q.Where(x => x.Status == ContentStatus.Published && x.PublishDate > today),
                    _ => q.Where(x => x.Status == ContentStatus.Published && x.PublishDate <= today),
                };
            },
            DefaultOrder = q => q.OrderByDescending(x => x.PublishDate).ThenByDescending(x => x.Id),
            Hooks = new PostHooks(),
        },

        new ContentDescriptor<CountryPage>
        {
            Slug = "country-pages", Label = "Country pages", Singular = "Country page", Area = AdminAreas.Pages, CanAdd = false, CanDelete = false, Orderable = false,
            Description = "The Uganda and Zambia pages. Choose which services and projects each shows, and in what order.",
            Set = db => db.CountryPages, TitleOf = x => x.Name,
            DefaultOrder = q => q.OrderBy(x => x.Name),
            Fields = new FieldDef[]
            {
                new() { Key = nameof(CountryPage.HeroImagePath), Label = "Image", Kind = FieldKind.Image, Required = true, Group = "Hero" },
                new() { Key = nameof(CountryPage.HeroEyebrow), Label = "Eyebrow", Required = true, Max = 40 },
                new() { Key = nameof(CountryPage.HeroHeadline), Label = "Headline", Kind = FieldKind.Area, Required = true, Max = 160, Rows = 2 },
                new() { Key = nameof(CountryPage.HeroSub), Label = "Sub-heading", Kind = FieldKind.Area, Max = 240, Rows = 3 },
                new() { Key = nameof(CountryPage.OverviewQuote), Label = "Lead quote", Kind = FieldKind.Area, Required = true, Max = 500, Rows = 5, Group = "Overview" },
                new() { Key = nameof(CountryPage.OverviewBody), Label = "Body", Kind = FieldKind.Area, Max = 400, Rows = 3 },
                new() { Key = nameof(CountryPage.Facts), Label = "At a glance", Kind = FieldKind.Lines, Rows = 5, Hint = "One per line: Key | Value, e.g. Established | 2017." },
                new() { Key = nameof(CountryPage.SectionOneTitle), Label = "Services heading", Required = true, Max = 50, Group = "Services shown" },
                new() { Key = "svc", Label = "Service cards", Kind = FieldKind.Picker },
                new() { Key = nameof(CountryPage.SectionTwoTitle), Label = "Projects heading", Required = true, Max = 50, Half = true, Group = "Projects shown" },
                new() { Key = nameof(CountryPage.SectionTwoNote), Label = "Note under the projects", Max = 160, Half = true, Hint = "Optional." },
                new() { Key = "prj", Label = "Project cards", Kind = FieldKind.Picker },
                new() { Key = nameof(CountryPage.CtaTitle), Label = "Title", Required = true, Max = 60, Half = true, Group = "Call to action" },
                new() { Key = nameof(CountryPage.CtaBody), Label = "Contact line", Max = 220, Half = true },
                new() { Key = nameof(CountryPage.CtaServices), Label = "Services in the quick enquiry form", Kind = FieldKind.Lines, Rows = 5, Hint = "One per line. These are the choices on the short form." },
            },
            Columns = new ColumnDef<CountryPage>[]
            {
                new() { Header = "Page", Html = (x, _) => Cell.Title(x.Name, "/" + x.Slug) },
                new() { Header = "Service cards", Html = (x, ctx) => ((ctx as (Dictionary<int, int> s, Dictionary<int, int> p)?)?.s.GetValueOrDefault(x.Id) ?? 0).ToString() },
                new() { Header = "Projects", Html = (x, ctx) => ((ctx as (Dictionary<int, int> s, Dictionary<int, int> p)?)?.p.GetValueOrDefault(x.Id) ?? 0).ToString() },
            },
            PrepareColumns = async (db, _) =>
            {
                var s = await db.CountryPageServices.AsNoTracking().GroupBy(x => x.CountryPageId).Select(g => new { g.Key, N = g.Count() }).ToDictionaryAsync(g => g.Key, g => g.N);
                var p = await db.CountryPageProjects.AsNoTracking().GroupBy(x => x.CountryPageId).Select(g => new { g.Key, N = g.Count() }).ToDictionaryAsync(g => g.Key, g => g.N);
                return (s, p);
            },
            Hooks = new CountryPageHooks(),
        },

        new ContentDescriptor<SiteSettings>
        {
            Slug = "settings", Label = "Site settings", Singular = "Settings", Area = AdminAreas.Settings, Single = true, CanAdd = false, CanDelete = false, Orderable = false,
            Description = "Values used across the site and by the enquiry forms.",
            Set = db => db.Settings, TitleOf = _ => "Site settings",
            Fields = new FieldDef[]
            {
                new() { Key = nameof(SiteSettings.ProjectSlots), Label = "Projects shown per country page", Kind = FieldKind.Number, Required = true, Min = 1, MaxValue = 6, Half = true, Group = "Country pages", Hint = "The layout fits three across. Services have no limit." },
                new() { Key = nameof(SiteSettings.BlogPageSize), Label = "Posts per page on /blog", Kind = FieldKind.Number, Required = true, Min = 3, MaxValue = 50, Half = true, Group = "Blog" },
                new() { Key = nameof(SiteSettings.BlogIntro), Label = "Blog intro line", Kind = FieldKind.Area, Max = 200, Rows = 2 },
                new() { Key = nameof(SiteSettings.UgandaTo), Label = "Uganda enquiries go to", Required = true, Max = 180, Half = true, Group = "Enquiry routing", Pattern = @"^\S+@\S+\.\S+$", PatternMessage = "Enter an email address." },
                new() { Key = nameof(SiteSettings.ZambiaTo), Label = "Zambia enquiries go to", Required = true, Max = 180, Half = true, Pattern = @"^\S+@\S+\.\S+$", PatternMessage = "Enter an email address." },
                new() { Key = nameof(SiteSettings.FoundationTo), Label = "Foundation enquiries go to", Required = true, Max = 180, Half = true, Pattern = @"^\S+@\S+\.\S+$", PatternMessage = "Enter an email address." },
                new() { Key = nameof(SiteSettings.RetentionMonths), Label = "Keep enquiries for (months)", Kind = FieldKind.Number, Required = true, Min = 0, MaxValue = 120, Half = true, Hint = "Enquiries hold personal data and are deleted after this long. 0 keeps them forever." },
            },
            Columns = Array.Empty<ColumnDef<SiteSettings>>(),
        },
    };

    private static DateOnly Today() => DateOnly.FromDateTime(DateTime.UtcNow);

    private static string ShownOn(int id, bool onList, string listName, object? context)
    {
        var tags = new List<string>();
        if (onList)
        {
            tags.Add(Cell.Tag(listName));
        }

        if (context is Dictionary<int, List<string>> placed && placed.TryGetValue(id, out var pages))
        {
            tags.AddRange(pages.Select(p => Cell.Tag(p)));
        }

        return tags.Count == 0 ? Cell.Tag("Hidden", "t-red") : string.Join(' ', tags);
    }

    // ---------- Hooks ----------

    private sealed class ProjectHooks : ContentHooks<Project>
    {
        public override async Task<List<PlacementOption>> PlacementOptionsAsync(AppDbContext db, AccessSnapshot access, Project? entity)
        {
            var options = new List<PlacementOption> { new("list", "Projects page", entity?.OnListPage ?? true, false) };
            var placed = entity is null ? new HashSet<int>() : (await db.CountryPageProjects.Where(p => p.ProjectId == entity.Id).Select(p => p.CountryPageId).ToListAsync()).ToHashSet();
            var canPlace = access.CanEdit(AdminAreas.Pages);
            foreach (var page in await db.CountryPages.AsNoTracking().OrderBy(p => p.Name).ToListAsync())
            {
                options.Add(new PlacementOption($"page:{page.Id}", page.Name + " page", placed.Contains(page.Id), !canPlace, canPlace ? null : "Needs Edit on Country pages"));
            }

            return options;
        }

        public override async Task ValidateAsync(AppDbContext db, AccessSnapshot access, Project? existing, FormView form, Dictionary<string, string> errors)
        {
            if (!access.CanEdit(AdminAreas.Pages))
            {
                return;
            }

            var cap = (await db.Settings.AsNoTracking().Select(s => (int?)s.ProjectSlots).FirstOrDefaultAsync()) ?? 3;
            foreach (var value in form.Many("shownOn").Where(v => v.StartsWith("page:")))
            {
                var pageId = int.Parse(value["page:".Length..]);
                var already = existing is not null && await db.CountryPageProjects.AnyAsync(p => p.CountryPageId == pageId && p.ProjectId == existing.Id);
                if (already)
                {
                    continue;
                }

                var count = await db.CountryPageProjects.CountAsync(p => p.CountryPageId == pageId);
                if (count >= cap)
                {
                    var name = await db.CountryPages.Where(p => p.Id == pageId).Select(p => p.Name).FirstAsync();
                    errors["shownOn"] = $"The {name} page already shows its maximum of {cap} projects. Remove one under Country pages first.";
                }
            }
        }

        public override async Task AfterSaveAsync(AppDbContext db, AccessSnapshot access, Project entity, FormView form)
        {
            var ticked = form.Many("shownOn").ToHashSet();
            entity.OnListPage = ticked.Contains("list");
            if (!access.CanEdit(AdminAreas.Pages))
            {
                return;
            }

            var existing = await db.CountryPageProjects.Where(p => p.ProjectId == entity.Id).ToListAsync();
            foreach (var page in await db.CountryPages.AsNoTracking().ToListAsync())
            {
                var row = existing.FirstOrDefault(p => p.CountryPageId == page.Id);
                var want = ticked.Contains($"page:{page.Id}");
                if (want && row is null)
                {
                    var next = (await db.CountryPageProjects.Where(p => p.CountryPageId == page.Id).MaxAsync(p => (int?)p.SortOrder) ?? 0) + 1;
                    db.CountryPageProjects.Add(new CountryPageProject { CountryPageId = page.Id, ProjectId = entity.Id, SortOrder = next });
                }
                else if (!want && row is not null)
                {
                    db.CountryPageProjects.Remove(row);
                }
            }
        }
    }

    private sealed class ServiceHooks : ContentHooks<Service>
    {
        public override async Task<List<PlacementOption>> PlacementOptionsAsync(AppDbContext db, AccessSnapshot access, Service? entity)
        {
            var options = new List<PlacementOption> { new("list", "Services page", entity?.OnListPage ?? true, false) };
            var placed = entity is null ? new HashSet<int>() : (await db.CountryPageServices.Where(p => p.ServiceId == entity.Id).Select(p => p.CountryPageId).ToListAsync()).ToHashSet();
            var canPlace = access.CanEdit(AdminAreas.Pages);
            foreach (var page in await db.CountryPages.AsNoTracking().OrderBy(p => p.Name).ToListAsync())
            {
                options.Add(new PlacementOption($"page:{page.Id}", page.Name + " page", placed.Contains(page.Id), !canPlace, canPlace ? null : "Needs Edit on Country pages"));
            }

            return options;
        }

        public override async Task AfterSaveAsync(AppDbContext db, AccessSnapshot access, Service entity, FormView form)
        {
            var ticked = form.Many("shownOn").ToHashSet();
            entity.OnListPage = ticked.Contains("list");
            if (!access.CanEdit(AdminAreas.Pages))
            {
                return;
            }

            var existing = await db.CountryPageServices.Where(p => p.ServiceId == entity.Id).ToListAsync();
            foreach (var page in await db.CountryPages.AsNoTracking().ToListAsync())
            {
                var row = existing.FirstOrDefault(p => p.CountryPageId == page.Id);
                var want = ticked.Contains($"page:{page.Id}");
                if (want && row is null)
                {
                    var next = (await db.CountryPageServices.Where(p => p.CountryPageId == page.Id).MaxAsync(p => (int?)p.SortOrder) ?? 0) + 1;
                    db.CountryPageServices.Add(new CountryPageService { CountryPageId = page.Id, ServiceId = entity.Id, SortOrder = next });
                }
                else if (!want && row is not null)
                {
                    db.CountryPageServices.Remove(row);
                }
            }
        }
    }

    private sealed class CategoryHooks : ContentHooks<BlogCategory>
    {
        public override async Task<string?> BeforeDeleteAsync(AppDbContext db, BlogCategory entity)
        {
            var posts = await db.BlogPosts.CountAsync(p => p.CategoryId == entity.Id);
            return posts == 0 ? null : $"{posts} post{(posts == 1 ? "" : "s")} use this category. Move them to another category first.";
        }
    }

    private sealed class PostHooks : ContentHooks<BlogPost>
    {
        public override Dictionary<string, string> Defaults(AccessSnapshot access) => new()
        {
            [nameof(BlogPost.AuthorName)] = access.User is { IsHidden: false } ? access.Actor : "Optimum Earth",
            [nameof(BlogPost.PublishDate)] = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd"),
        };

        public override async Task AfterSaveAsync(AppDbContext db, AccessSnapshot access, BlogPost entity, FormView form)
        {
            // Keep the last ten saved versions of each post, so a bad edit can be recovered.
            db.BlogRevisions.Add(new BlogRevision { BlogPostId = entity.Id, SavedBy = access.Actor, Title = entity.Title, Body = entity.Body });
            await db.SaveChangesAsync();
            var old = await db.BlogRevisions.Where(r => r.BlogPostId == entity.Id).OrderByDescending(r => r.SavedUtc).ThenByDescending(r => r.Id).Skip(10).ToListAsync();
            db.BlogRevisions.RemoveRange(old);
        }
    }

    private sealed class CountryPageHooks : ContentHooks<CountryPage>
    {
        private static async Task<int> CapAsync(AppDbContext db) =>
            (await db.Settings.AsNoTracking().Select(s => (int?)s.ProjectSlots).FirstOrDefaultAsync()) ?? 3;

        private static async Task<(List<PickerChoice> Services, List<PickerChoice> Projects)> ChoicesAsync(AppDbContext db)
        {
            var services = await db.Services.AsNoTracking().OrderBy(s => s.SortOrder).ThenBy(s => s.Id).ToListAsync();
            var projects = await db.Projects.AsNoTracking().OrderBy(p => p.SortOrder).ThenBy(p => p.Id).ToListAsync();
            return (
                services.Select(s => new PickerChoice(s.Id, s.Title, s.Status == ContentStatus.Draft ? "draft" : "published", s.Title, s.Description)).ToList(),
                projects.Select(p => new PickerChoice(p.Id, p.Title, p.Status == ContentStatus.Draft ? "draft" : "published", p.Tag, p.Meta)).ToList());
        }

        private static PickerEntry Entry(PickerChoice? choice, int? id, string title, string text, bool custom, string url = "") =>
            custom
                ? new PickerEntry(null, string.IsNullOrWhiteSpace(title) ? "Custom card" : title, "published", true, title, text, string.Empty, string.Empty, url)
                : new PickerEntry(id, choice?.Label ?? "Removed item", choice?.Status ?? "draft", false, title, text, choice?.Title ?? string.Empty, choice?.Text ?? string.Empty);

        public override async Task<Dictionary<string, PickerData>> PickersAsync(AppDbContext db, AccessSnapshot access, CountryPage? entity)
        {
            var (serviceChoices, projectChoices) = await ChoicesAsync(db);
            var svc = new PickerData { Prefix = "svc", AllowCustom = true, Choices = serviceChoices };
            var prj = new PickerData { Prefix = "prj", Max = await CapAsync(db), Choices = projectChoices };

            if (entity is not null)
            {
                var services = await db.CountryPageServices.AsNoTracking().Where(x => x.CountryPageId == entity.Id).OrderBy(x => x.SortOrder).ToListAsync();
                svc.Entries = services.Select(x => Entry(serviceChoices.FirstOrDefault(c => c.Id == x.ServiceId), x.ServiceId, x.Title, x.Text, x.ServiceId is null, x.Url)).ToList();
                var projects = await db.CountryPageProjects.AsNoTracking().Where(x => x.CountryPageId == entity.Id).OrderBy(x => x.SortOrder).ToListAsync();
                prj.Entries = projects.Select(x => Entry(projectChoices.FirstOrDefault(c => c.Id == x.ProjectId), x.ProjectId, x.Tag, x.Meta, false)).ToList();
            }

            return new() { ["svc"] = svc, ["prj"] = prj };
        }

        public override async Task<Dictionary<string, PickerData>> PickersFromFormAsync(AppDbContext db, AccessSnapshot access, CountryPage? entity, FormView form)
        {
            var pickers = await PickersAsync(db, access, entity);
            foreach (var (key, picker) in pickers)
            {
                var ids = form.Raw(key + "_id");
                var titles = form.Raw(key + "_title");
                var texts = form.Raw(key + "_text");
                var urls = form.Raw(key + "_url");
                picker.Entries = new();
                for (var i = 0; i < ids.Count; i++)
                {
                    var title = i < titles.Count ? titles[i] : string.Empty;
                    var text = i < texts.Count ? texts[i] : string.Empty;
                    var linked = int.TryParse(ids[i], out var id);
                    picker.Entries.Add(Entry(linked ? picker.Choices.FirstOrDefault(c => c.Id == id) : null, linked ? id : null, title, text, !linked, i < urls.Count ? urls[i] : string.Empty));
                }
            }

            return pickers;
        }

        private static List<(int? Id, string Title, string Text, string Url)> Rows(FormView form, string prefix)
        {
            var ids = form.Raw(prefix + "_id");
            var titles = form.Raw(prefix + "_title");
            var texts = form.Raw(prefix + "_text");
            var urls = form.Raw(prefix + "_url");
            var rows = new List<(int?, string, string, string)>();
            for (var i = 0; i < ids.Count; i++)
            {
                rows.Add((int.TryParse(ids[i], out var id) ? id : null, (i < titles.Count ? titles[i] : string.Empty).Trim(), (i < texts.Count ? texts[i] : string.Empty).Trim(), (i < urls.Count ? urls[i] : string.Empty).Trim()));
            }

            return rows;
        }

        public override async Task ValidateAsync(AppDbContext db, AccessSnapshot access, CountryPage? existing, FormView form, Dictionary<string, string> errors)
        {
            var services = Rows(form, "svc");
            var serviceIds = (await db.Services.AsNoTracking().Select(s => s.Id).ToListAsync()).ToHashSet();
            var seen = new HashSet<int>();
            foreach (var (id, title, text, url) in services)
            {
                if (id is null && title.Length == 0)
                {
                    errors["svc"] = "Every custom card needs a title.";
                }
                else if (id is not null && !serviceIds.Contains(id.Value))
                {
                    errors["svc"] = "One of the chosen services no longer exists. Remove it.";
                }
                else if (id is not null && !seen.Add(id.Value))
                {
                    errors["svc"] = "The same service is chosen twice.";
                }
                else if (title.Length > 70 || text.Length > 400)
                {
                    errors["svc"] = "Keep card titles under 70 characters and card text under 400.";
                }
                else if (url.Length > 0 && !(url.StartsWith('/') && !url.StartsWith("//") || url.StartsWith("https://")))
                {
                    errors["svc"] = "A card link must start with / (a page on this site) or https://.";
                }
            }

            var projects = Rows(form, "prj");
            var cap = await CapAsync(db);
            var projectIds = (await db.Projects.AsNoTracking().Select(p => p.Id).ToListAsync()).ToHashSet();
            if (projects.Count > cap)
            {
                errors["prj"] = $"This page can show at most {cap} projects (set in Site settings). Remove {projects.Count - cap}.";
            }

            var seenProjects = new HashSet<int>();
            foreach (var (id, tag, meta, _) in projects)
            {
                if (id is null || !projectIds.Contains(id.Value))
                {
                    errors["prj"] = "One of the chosen projects no longer exists. Remove it.";
                }
                else if (!seenProjects.Add(id.Value))
                {
                    errors["prj"] = "The same project is chosen twice.";
                }
                else if (tag.Length > 40 || meta.Length > 160)
                {
                    errors["prj"] = "Keep tag lines under 40 characters and the client line under 160.";
                }
            }
        }

        public override async Task AfterSaveAsync(AppDbContext db, AccessSnapshot access, CountryPage entity, FormView form)
        {
            db.CountryPageServices.RemoveRange(await db.CountryPageServices.Where(x => x.CountryPageId == entity.Id).ToListAsync());
            db.CountryPageProjects.RemoveRange(await db.CountryPageProjects.Where(x => x.CountryPageId == entity.Id).ToListAsync());

            var order = 0;
            foreach (var (id, title, text, url) in Rows(form, "svc"))
            {
                db.CountryPageServices.Add(new CountryPageService { CountryPageId = entity.Id, ServiceId = id, Title = title, Text = text, Url = url, SortOrder = ++order });
            }

            order = 0;
            foreach (var (id, tag, meta, _) in Rows(form, "prj"))
            {
                db.CountryPageProjects.Add(new CountryPageProject { CountryPageId = entity.Id, ProjectId = id!.Value, Tag = tag, Meta = meta, SortOrder = ++order });
            }
        }
    }
}
