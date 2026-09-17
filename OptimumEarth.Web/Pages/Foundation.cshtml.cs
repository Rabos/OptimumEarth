using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class FoundationModel : PageModel
{
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

    public List<(string Number, string Label)> SdgGoals { get; } = new()
    {
        ("06", "Clean Water & Sanitation"),
        ("07", "Affordable & Clean Energy"),
        ("13", "Climate Action"),
        ("17", "Partnerships for the Goals"),
    };

    public List<PillarDetail> Pillars { get; } = new()
    {
        new("access", "SDG 6 & 7", "Access",
            "Expanding access to essential services and practical climate solutions, particularly clean water, clean energy and appropriate climate technologies.",
            new() { "Water", "Clean Energy", "Climate Solutions" }),
        new("resilience", "SDG 13", "Resilience",
            "Strengthening the ability of communities and local actors to anticipate, respond to and recover from climate and development pressures.",
            new() { "Community resilience & capacity strengthening", "Local leadership and capabilities", "Climate preparedness and adaptation" }),
        new("impact", "SDG 17", "Impact",
            "Connecting local solutions to partnerships, finance and knowledge so effective approaches can be sustained, adapted and scaled responsibly.",
            new() { "Partnerships", "Finance & resource mobilisation", "Knowledge sharing & learning" }),
    };

    public List<string> FocusAreas { get; } = new()
    {
        "Water & WASH",
        "Clean Energy",
        "Climate Action",
        "Community Resilience",
        "Partnerships & Finance",
        "Knowledge & Learning",
    };

    public List<WorkPrinciple> WorkPrinciples { get; } = new()
    {
        new("Community-led", "We start with the lived realities, priorities and capabilities of the communities and local actors we work with, not with pre-set solutions.", "community-led"),
        new("Evidence-informed", "We draw on evidence, local knowledge and continuous learning to shape our decisions and improve our approach.", "evidence-informed"),
        new("Partnership-driven", "We bring together communities, philanthropy, public institutions, private capital and technical actors around a shared goal.", "partnership-driven"),
        new("Practical and scalable", "We focus on solutions that work in real settings, and that we can adapt or expand responsibly as they prove out.", "practical-and-scalable"),
        new("Locally grounded", "We prioritise African leadership, local ownership and context-specific solutions in everything we do.", "locally-grounded"),
        new("Learning-oriented", "We document what works and what doesn't, so our learning contributes to wider practice across the region.", "learning-oriented"),
    };

    public List<StoryTile> StoryTiles { get; } = new()
    {
        new("communities", "Communities", "Stories from the field, in the words of the people we work with.",
            "Stories from the field, in the words of the people we work with. This space holds community voices, before/after and challenge/response stories, and photography from the field — the human face of OEF's work.",
            "Community story photo", "/img/optimum-earth-images/foundation2.jpg"),
        new("partnerships", "Partnerships", "A look at what we're building together.",
            "A look at what we're building together. This space features project and partnership case studies, impact indicators and simple dashboards as data becomes available, and news or updates on active collaborations.",
            "Partnership story photo", "/img/optimum-earth-images/foundation3.jpg"),
        new("learning", "Learning", "Where OEF shares what it's discovering and who it's growing with.",
            "Where OEF shares what it's discovering and who it's growing with. This space holds practical learning notes and insights, alongside our Academic & Learning Partnerships — joint research, student placements and fellowships, mentorship, and career-pathway stories connecting young African talent to climate, water and energy work.",
            "Learning story photo", "/img/optimum-earth-images/foundation4.jpg"),
    };

    // The lightbox gallery behind each tile. Key must match a StoryTile.Key or
    // the tile opens nothing, and the first item is that tile's Cover so the
    // gallery opens on the image the visitor just clicked.
    //
    // PLACEHOLDER MAPPING: which photograph belongs to which story has not been
    // decided yet, so these are grouped by subject from what is already in
    // wwwroot/img/optimum-earth-images, and the captions are descriptive
    // stand-ins. Editing this one list is all that is needed to fix either.
    public List<StoryGallery> Galleries { get; } = new()
    {
        new("communities", new List<StoryGalleryItem>
        {
            new("Community water committee, Karamoja", "/img/optimum-earth-images/foundation2.jpg"),
            new("Planning a scheme with the households it will serve", "/img/optimum-earth-images/community-led.jpg"),
            new("First draw from the new borehole", "/img/optimum-earth-images/borehole.jpg"),
            new("Makuttu: the site before work began", "/img/optimum-earth-images/makuttu.jpg"),
            new("Households on the extended network", "/img/optimum-earth-images/foundation1.jpg"),
        }),
        new("partnerships", new List<StoryGalleryItem>
        {
            new("Partners walking a proposed site together", "/img/optimum-earth-images/foundation3.jpg"),
            new("Joint delivery with local contractors", "/img/optimum-earth-images/partnership-driven.jpg"),
            new("Hima quarry: shared infrastructure works", "/img/optimum-earth-images/hima-quarry.jpg"),
            new("Pivot irrigation commissioned with growers", "/img/optimum-earth-images/pivot-irrigation.jpg"),
            new("Solar array sized against real demand", "/img/optimum-earth-images/enhancing-climate.jpg"),
        }),
        new("learning", new List<StoryGalleryItem>
        {
            new("A learning session with partner students", "/img/optimum-earth-images/foundation4.jpg"),
            new("Documenting what worked and what did not", "/img/optimum-earth-images/learning-oriented.jpg"),
            new("Field measurement feeding the evidence base", "/img/optimum-earth-images/evidence-informed.jpg"),
            new("Local teams leading the technical work", "/img/optimum-earth-images/locally-grounded.jpg"),
            new("Testing an approach before scaling it", "/img/optimum-earth-images/practical-and-scalable.jpg"),
        }),
    };

    public void OnGet()
    {
    }

    public IActionResult OnPostQuick()
    {
        Quick.Destination = "Foundation";
        if (!ModelState.IsValid)
        {
            return Page();
        }

        ModelState.Clear();
        Quick = new QuickInquiry { Destination = "Foundation" };
        Submitted = true;
        return Page();
    }
}
