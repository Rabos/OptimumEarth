using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages' default routing already gives us the brief's SEO-aligned
// structure with no extra configuration: Pages/Index.cshtml -> "/",
// Pages/Uganda.cshtml -> "/Uganda" (matches "/uganda" case-insensitively),
// and likewise for Zambia and Foundation.
builder.Services.AddRazorPages();

builder.Services.AddResponseCompression();

// Data Protection defaults to persisting its key ring under the OS temp
// directory, which crashes every antiforgery-token request (any page with
// a <form>) on hosts where /tmp is mounted read-only - e.g. a systemd unit
// with ProtectSystem=strict/full and no ReadWritePaths covering /tmp. Pin
// key storage to the app's own content root instead, which the deploy
// already writes to, so it doesn't depend on /tmp being writable.
// Note: this directory gets wiped on every deploy (the publish step
// extracts a fresh tarball over it), which invalidates outstanding
// antiforgery tokens/cookies right after a deploy - a much smaller issue
// than the current hard crash, but if that's undesirable, point this at a
// directory outside /var/www/optimumearth that survives redeploys, and
// make sure the service user can write to it.
var keysDirectory = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "keys"));
builder.Services.AddDataProtection()
    .SetApplicationName("OptimumEarth.Web")
    .PersistKeysToFileSystem(keysDirectory);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
