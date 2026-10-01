using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages' default routing already gives us the brief's SEO-aligned
// structure with no extra configuration: Pages/Index.cshtml -> "/",
// Pages/Uganda.cshtml -> "/Uganda" (matches "/uganda" case-insensitively),
// and likewise for Zambia and Foundation. The dashboard lives in the Admin
// area and takes explicit /admin/... routes.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaFolder("Admin", "/");
    options.Conventions.AllowAnonymousToAreaPage("Admin", "/Login");
    options.Conventions.AllowAnonymousToAreaPage("Admin", "/Denied");
    options.Conventions.AllowAnonymousToAreaPage("Admin", "/SetPassword");
});

builder.Services.AddResponseCompression();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:Default is not set. Point it at the PostgreSQL database.");
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services
    .AddIdentity<AppUser, IdentityRole<int>>(options =>
    {
        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "oe.admin";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
    options.LoginPath = "/admin/login";
    options.AccessDeniedPath = "/admin/denied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// Invite and reset links are one-use tokens; give them a few days.
builder.Services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromDays(3));

// Data Protection defaults to persisting its key ring under the OS temp
// directory, which crashes every antiforgery-token request (any page with
// a <form>) on hosts where /tmp is mounted read-only. The keys now live in
// the database, so they also survive a redeploy: nobody is signed out and
// no outstanding form token is invalidated when a new build is published.
builder.Services.AddDataProtection()
    .SetApplicationName("OptimumEarth.Web")
    .PersistKeysToDbContext<AppDbContext>();

builder.Services.AddSingleton<ContentCache>();
builder.Services.AddSingleton<MarkdownRenderer>();
builder.Services.AddScoped<ContentReader>();
builder.Services.AddScoped<AccessContext>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<InquiryService>();
builder.Services.AddScoped<MediaService>();
builder.Services.AddHostedService<DatabaseInitializer>();
builder.Services.AddHostedService<InquiryRetentionService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseStaticFiles();

// Uploaded images live outside wwwroot so a redeploy that replaces the app
// folder does not delete them. Point Storage:UploadsPath at a folder that
// survives deploys and that the service user can write to.
var uploadsPath = MediaService.ResolveUploadsPath(app.Configuration, app.Environment);
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads",
});

// The dashboard is for staff only: keep it out of search results.
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/admin"))
    {
        context.Response.Headers["X-Robots-Tag"] = "noindex, nofollow";
        context.Response.Headers.CacheControl = "no-store";
    }

    await next();
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

SeoEndpoints.Map(app);
app.MapRazorPages();

app.Run();
