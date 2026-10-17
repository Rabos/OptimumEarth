using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages;

public class WhoWeAreModel : PageModel
{
    private readonly ContentReader _content;

    public WhoWeAreModel(ContentReader content)
    {
        _content = content;
    }

    public WhoWeAreContent Who { get; private set; } = null!;

    public async Task OnGetAsync()
    {
        Who = await _content.WhoWeAreAsync();
    }

    public int StatValue(StatItem stat) => stat.ValueOn(Who.Text.FoundedOn, DateOnly.FromDateTime(DateTime.UtcNow));

    /// <summary>Countries with an office are highlighted in the "Where we work" list.</summary>
    public bool IsBase(string country) => Who.Offices.Any(o => string.Equals(o.Country, country, StringComparison.OrdinalIgnoreCase));
}
