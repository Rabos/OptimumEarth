var builder = WebApplication.CreateBuilder(args);

// Razor Pages' default routing already gives us the brief's SEO-aligned
// structure with no extra configuration: Pages/Index.cshtml -> "/",
// Pages/Uganda.cshtml -> "/Uganda" (matches "/uganda" case-insensitively),
// and likewise for Zambia and Foundation.
builder.Services.AddRazorPages();

builder.Services.AddResponseCompression();

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
