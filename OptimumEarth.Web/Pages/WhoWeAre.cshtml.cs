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

    public IReadOnlyList<CapabilityItem> Capabilities { get; private set; } = Array.Empty<CapabilityItem>();

    public string Mission { get; private set; } = string.Empty;

    public string Vision { get; private set; } = string.Empty;

    public IReadOnlyList<ValueItem> Values { get; private set; } = Array.Empty<ValueItem>();

    public async Task OnGetAsync()
    {
        var content = await _content.WhoWeAreAsync();
        Capabilities = content.Capabilities;
        Mission = content.Mission;
        Vision = content.Vision;
        Values = content.Values;
    }
}
