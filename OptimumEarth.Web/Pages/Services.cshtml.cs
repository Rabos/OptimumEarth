using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages;

public class ServicesModel : PageModel
{
    private readonly ContentReader _content;

    public ServicesModel(ContentReader content)
    {
        _content = content;
    }

    public IReadOnlyList<ServiceLine> Rows { get; private set; } = Array.Empty<ServiceLine>();

    public async Task OnGetAsync()
    {
        Rows = await _content.ServiceLinesAsync();
    }
}
