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

        if (zambia is not null && (zambia.CtaBody == V2ZambiaCtaBody || zambia.CtaBody == V3ZambiaCtaBody))
        {
            zambia.CtaBody = SeedData.Zambia().CtaBody;
            upgraded.Add("Zambia CTA contact line");
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
