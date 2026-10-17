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

    /// <summary>Whole years since the founding date, for the "years" stat.</summary>
    public int YearsInOperation { get; private set; }

    public async Task OnGetAsync()
    {
        Who = await _content.WhoWeAreAsync();
        var founded = Who.Text.FoundedOn;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        YearsInOperation = Math.Max(0, today.Year - founded.Year - (today < founded.AddYears(today.Year - founded.Year) ? 1 : 0));
    }

    public int StatValue(StatItem stat) => stat.CountsYears ? YearsInOperation : stat.Value;

    /// <summary>Countries with an office are highlighted in the "Where we work" list.</summary>
    public bool IsBase(string country) => Who.Offices.Any(o => string.Equals(o.Country, country, StringComparison.OrdinalIgnoreCase));
}
