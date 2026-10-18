using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages;

public class ServicesModel : PageModel
{
    private readonly ContentReader _content;

    public ServicesModel(ContentReader content)
    {
        _content = content;
    }

    public ServicesPageContent Services { get; private set; } = null!;

    public async Task OnGetAsync()
    {
        Services = await _content.ServicesPageAsync();
    }

    // The outline icons beside each service's headline capabilities, in order, by service anchor.
    // A service added in the dashboard, or a fifth line, falls back to a tick.
    private static readonly Dictionary<string, string[]> Icons = new()
    {
        ["technical-studies"] = new[]
        {
            "<path d=\"M12 3s6 6.5 6 11a6 6 0 0 1-12 0c0-4.5 6-11 6-11z\"/>",
            "<path d=\"M3 12h3l3-7 4 14 3-7h5\"/>",
            "<path d=\"M3 20l6-12 4 7 3-4 5 9z\"/>",
            "<circle cx=\"12\" cy=\"12\" r=\"9\"/><path d=\"M3 12h18M12 3c3 3 3 15 0 18M12 3c-3 3-3 15 0 18\"/>",
        },
        ["engineering-design"] = new[]
        {
            "<path d=\"M4 20V10l8-6 8 6v10M9 20v-6h6v6\"/>",
            "<circle cx=\"12\" cy=\"12\" r=\"4\"/><path d=\"M12 2v2M12 20v2M2 12h2M20 12h2M5 5l1.5 1.5M17.5 17.5L19 19M5 19l1.5-1.5M17.5 6.5L19 5\"/>",
            "<path d=\"M13 2L4 14h7l-1 8 9-12h-7z\"/>",
            "<rect x=\"4\" y=\"3\" width=\"16\" height=\"18\" rx=\"2\"/><path d=\"M8 8h8M8 12h8M8 16h5\"/>",
        },
        ["construction"] = new[]
        {
            "<path d=\"M12 2v14M8 12l4 4 4-4M5 22h14\"/>",
            "<path d=\"M2 12h6l2-3h4l2 3h6M2 16h20\"/>",
            "<rect x=\"3\" y=\"10\" width=\"18\" height=\"10\" rx=\"1\"/><path d=\"M7 10V6h10v4\"/>",
            "<path d=\"M9 12l2 2 4-4\"/><circle cx=\"12\" cy=\"12\" r=\"9\"/>",
        },
        ["esia"] = new[]
        {
            "<path d=\"M7 3h7l5 5v13H7z\"/><path d=\"M14 3v5h5\"/>",
            "<path d=\"M12 22c-4-3-7-7-7-12a7 7 0 0 1 14 0c0 5-3 9-7 12z\"/>",
            "<circle cx=\"9\" cy=\"8\" r=\"3\"/><circle cx=\"17\" cy=\"9\" r=\"2.5\"/><path d=\"M3 20c0-3.5 2.7-6 6-6s6 2.5 6 6M15 20c0-2.5 1.5-4.5 4-4.5\"/>",
            "<path d=\"M9 12l2 2 4-4\"/><rect x=\"4\" y=\"3\" width=\"16\" height=\"18\" rx=\"2\"/>",
        },
        ["climate-resilience"] = new[]
        {
            "<path d=\"M3 7c3-2 6-2 9 0s6 2 9 0\"/><path d=\"M3 17c3-2 6-2 9 0s6 2 9 0\"/><path d=\"M3 12c3-2 6-2 9 0s6 2 9 0\"/>",
            "<path d=\"M3 6h18v12H3z\"/><path d=\"M3 13l5-4 4 3 4-3 5 4\"/>",
            "<path d=\"M12 22V11\"/><path d=\"M12 11c0-4 3-7 7-7 0 4-3 7-7 7z\"/><path d=\"M12 14c0-3-2.5-5.5-6-5.5 0 3 2.5 5.5 6 5.5z\"/>",
            "<path d=\"M4 19l4-6 4 2 4-5 4 4v5H4\"/><path d=\"M4 12l3-4 4 2 5-6 4 4\"/>",
        },
    };

    private const string Tick = "<path d=\"M9 12l2 2 4-4\"/><circle cx=\"12\" cy=\"12\" r=\"9\"/>";

    public static string IconFor(string slug, int index) =>
        Icons.TryGetValue(slug, out var set) && index < set.Length ? set[index] : Tick;
}
