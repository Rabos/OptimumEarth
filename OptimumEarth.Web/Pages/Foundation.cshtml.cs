using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages;

public class FoundationModel : PageModel
{
    private readonly ContentReader _content;
    private readonly InquiryService _inquiries;

    public FoundationModel(ContentReader content, InquiryService inquiries)
    {
        _content = content;
        _inquiries = inquiries;
    }

    [BindProperty]
    public QuickInquiry Quick { get; set; } = new() { Destination = "Foundation" };

    public bool Submitted { get; set; }

    public static readonly List<string> EngagementOptions = new()
    {
        "Partner with a programme",
        "Fund a programme",
        "Collaborate on research or delivery",
        "Learn more",
    };

    public IReadOnlyList<(string Number, string Label)> SdgGoals { get; private set; } = Array.Empty<(string, string)>();

    public IReadOnlyList<PillarDetail> Pillars { get; private set; } = Array.Empty<PillarDetail>();

    public IReadOnlyList<string> FocusAreas { get; private set; } = Array.Empty<string>();

    public IReadOnlyList<WorkPrinciple> WorkPrinciples { get; private set; } = Array.Empty<WorkPrinciple>();

    public IReadOnlyList<StoryTile> StoryTiles { get; private set; } = Array.Empty<StoryTile>();

    // The lightbox gallery behind each tile. A gallery's Key must match a
    // StoryTile.Key or the tile opens nothing, and the first image is that
    // tile's cover so the gallery opens on the image the visitor just clicked.
    // Both are managed under Foundation in the dashboard.
    public IReadOnlyList<StoryGallery> Galleries { get; private set; } = Array.Empty<StoryGallery>();

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostQuickAsync()
    {
        await LoadAsync();
        Quick.Destination = "Foundation";
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _inquiries.AddQuickAsync(Quick);
        ModelState.Clear();
        Quick = new QuickInquiry { Destination = "Foundation" };
        Submitted = true;
        return Page();
    }

    private async Task LoadAsync()
    {
        var content = await _content.FoundationAsync();
        SdgGoals = content.SdgGoals;
        Pillars = content.Pillars;
        FocusAreas = content.FocusAreas;
        WorkPrinciples = content.WorkPrinciples;
        StoryTiles = content.StoryTiles;
        Galleries = content.Galleries;
    }
}
