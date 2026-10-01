using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace OptimumEarth.Web.Data;

/// <summary>
/// Runs once at startup, before the site accepts requests: applies migrations,
/// creates the roles, fills empty content tables from <see cref="SeedData"/>,
/// registers the images shipped in the repository, and seeds the hidden super admin.
/// Every step is safe to repeat.
/// </summary>
public sealed class DatabaseInitializer : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IServiceProvider services, ILogger<DatabaseInitializer> logger)
    {
        _services = services;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync(cancellationToken);
        await EnsureRolesAsync(sp.GetRequiredService<RoleManager<IdentityRole<int>>>());
        await SeedContentAsync(db, cancellationToken);
        await ContentUpgrades.ApplyAsync(db, _logger, cancellationToken);
        await SeedMediaAsync(db, sp.GetRequiredService<IWebHostEnvironment>(), cancellationToken);
        await SeedSuperAdminAsync(sp.GetRequiredService<UserManager<AppUser>>(), sp.GetRequiredService<IConfiguration>());
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole<int>> roles)
    {
        foreach (var name in new[] { Roles.SuperAdmin, Roles.Editor, Roles.Viewer })
        {
            if (!await roles.RoleExistsAsync(name))
            {
                await roles.CreateAsync(new IdentityRole<int>(name));
            }
        }
    }

    private static async Task SeedContentAsync(AppDbContext db, CancellationToken ct)
    {
        if (!await db.Settings.AnyAsync(ct))
        {
            db.Settings.Add(new SiteSettings
            {
                Mission = SeedData.Mission,
                Vision = SeedData.Vision,
                BlogIntro = "Field notes, insights and updates from our teams and the Foundation.",
                UgandaTo = "uganda@optimum-earth.com",
                ZambiaTo = "zambia@optimum-earth.com",
                FoundationTo = "foundation@optimum-earth.com",
                ContentVersion = SeedData.CurrentContentVersion,
            });
        }

        await SeedListAsync(db, db.Slides, SeedData.Slides(), ct);
        await SeedListAsync(db, db.Capabilities, SeedData.Capabilities(), ct);
        await SeedListAsync(db, db.CoreValues, SeedData.Values(), ct);
        await SeedListAsync(db, db.Pillars, SeedData.Pillars(), ct);
        await SeedListAsync(db, db.Principles, SeedData.Principles(), ct);
        await SeedListAsync(db, db.FocusAreas, SeedData.FocusAreas(), ct);
        await SeedListAsync(db, db.SdgGoals, SeedData.SdgGoals(), ct);
        await SeedListAsync(db, db.Stories, SeedData.Stories(), ct);
        await SeedListAsync(db, db.GalleryImages, SeedData.Gallery(), ct);

        if (!await db.BlogCategories.AnyAsync(ct))
        {
            db.BlogCategories.AddRange(SeedData.BlogCategories());
        }

        if (!await db.Services.AnyAsync(ct) && !await db.Projects.AnyAsync(ct) && !await db.CountryPages.AnyAsync(ct))
        {
            var services = SeedData.Services();
            var listed = SeedData.ListProjects();
            var featured = SeedData.CountryCardProjects();
            Number(services);

            // One running order: the Projects page's list first, then the six country-page cards.
            Number(listed.Concat(featured).ToList());
            db.Services.AddRange(services);
            db.Projects.AddRange(listed);
            db.Projects.AddRange(featured);

            var uganda = SeedData.Uganda();
            uganda.Services = ServiceCards(SeedData.UgandaServiceCards(), services);
            uganda.Projects = featured.Take(3).Select((p, i) => new CountryPageProject { Project = p, SortOrder = i }).ToList();

            var zambia = SeedData.Zambia();
            zambia.Services = ServiceCards(SeedData.ZambiaServiceCards(), services);
            zambia.Projects = featured.Skip(3).Take(3).Select((p, i) => new CountryPageProject { Project = p, SortOrder = i }).ToList();

            db.CountryPages.AddRange(uganda, zambia);
        }

        await db.SaveChangesAsync(ct);
    }

    /// <summary>Builds a country page's service cards from their specs; a spec with no service index is a custom card.</summary>
    public static List<CountryPageService> ServiceCards(List<SeedData.ServiceCardSpec> specs, List<Service> services) =>
        specs.Select((c, i) => new CountryPageService
        {
            Service = c.ServiceIndex is { } index ? services[index] : null,
            Title = c.Title,
            Text = c.Text,
            Url = c.Url,
            SortOrder = i + 1,
        }).ToList();

    private static async Task SeedListAsync<T>(AppDbContext db, DbSet<T> set, List<T> rows, CancellationToken ct) where T : ContentEntity
    {
        if (await set.AnyAsync(ct))
        {
            return;
        }

        Number(rows);
        set.AddRange(rows);
    }

    private static void Number<T>(List<T> rows) where T : ContentEntity
    {
        for (var i = 0; i < rows.Count; i++)
        {
            rows[i].SortOrder = i + 1;
        }
    }

    private static readonly Dictionary<string, string> BuiltInAlt = new()
    {
        ["borehole.jpg"] = "Water flowing from a newly drilled borehole",
        ["makuttu.jpg"] = "Makuutu project site",
        ["hima-quarry.jpg"] = "Hima Cement quarry with water management works",
        ["recentwork1.jpg"] = "Production well survey in progress",
        ["recentwork2.jpg"] = "Monitoring well installation",
        ["recentwork3.jpg"] = "Pivot irrigation site assessment",
        ["servicepanel1.jpg"] = "Water supply solutions",
        ["servicepanel2.jpg"] = "Groundwater monitoring",
        ["servicepanel3.jpg"] = "Environmental services",
        ["servicepanel4.jpg"] = "GIS, mapping and remote sensing",
        ["servicepanel5.jpg"] = "Mines, oil and gas",
    };

    private async Task SeedMediaAsync(AppDbContext db, IWebHostEnvironment env, CancellationToken ct)
    {
        var folder = Path.Combine(env.WebRootPath, "img", "optimum-earth-images");
        if (!Directory.Exists(folder))
        {
            return;
        }

        var known = (await db.Media.Select(m => m.Path).ToListAsync(ct)).ToHashSet();
        foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories).OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
        {
            var name = Path.GetFileName(file);
            var url = "/img/optimum-earth-images/" + Path.GetRelativePath(folder, file).Replace('\\', '/');
            if (known.Contains(url))
            {
                continue;
            }

            db.Media.Add(new MediaAsset
            {
                FileName = name,
                Path = url,
                Alt = BuiltInAlt.GetValueOrDefault(name, string.Empty),
                SizeBytes = new FileInfo(file).Length,
                ContentType = Path.GetExtension(file).ToLowerInvariant() is ".png" ? "image/png" : "image/jpeg",
                BuiltIn = true,
            });
        }

        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Seeds the hidden super admin from configuration (SuperAdmin:Email and SuperAdmin:Password,
    /// normally supplied as environment variables). The password is only used when the account is
    /// first created; changing the setting later does not overwrite it.
    /// </summary>
    private async Task SeedSuperAdminAsync(UserManager<AppUser> users, IConfiguration config)
    {
        if (users.Users.Any(u => u.IsHidden))
        {
            return;
        }

        var email = config["SuperAdmin:Email"];
        var password = config["SuperAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("No super admin exists and SuperAdmin:Email / SuperAdmin:Password are not set. The dashboard cannot be signed into until they are.");
            return;
        }

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            DisplayName = "Platform admin",
            IsHidden = true,
        };

        var created = await users.CreateAsync(user, password);
        if (!created.Succeeded)
        {
            _logger.LogError("Could not create the super admin: {Errors}", string.Join("; ", created.Errors.Select(e => e.Description)));
            return;
        }

        await users.AddToRoleAsync(user, Roles.SuperAdmin);
        _logger.LogInformation("Seeded the super admin account.");
    }
}
