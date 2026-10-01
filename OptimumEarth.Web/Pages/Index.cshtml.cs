using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ContentReader _content;

    public IndexModel(ContentReader content)
    {
        _content = content;
    }

    public IReadOnlyList<HeroSlide> HeroSlides { get; private set; } = Array.Empty<HeroSlide>();

    public async Task OnGetAsync()
    {
        HeroSlides = await _content.HeroSlidesAsync();
    }
}
