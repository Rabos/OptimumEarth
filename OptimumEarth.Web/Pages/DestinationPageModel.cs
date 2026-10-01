using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages;

/// <summary>
/// Shared behaviour of the Uganda and Zambia pages: both render the same
/// destination template from a country page managed in the dashboard, and both
/// carry the short enquiry form in the call-to-action band.
/// </summary>
public abstract class DestinationPageModel : PageModel
{
    private readonly ContentReader _content;
    private readonly InquiryService _inquiries;
    private readonly string _slug;
    private readonly string _destination;

    protected DestinationPageModel(ContentReader content, InquiryService inquiries, string slug, string destination)
    {
        _content = content;
        _inquiries = inquiries;
        _slug = slug;
        _destination = destination;
        Quick = new QuickInquiry { Destination = destination };
    }

    [BindProperty]
    public QuickInquiry Quick { get; set; }

    public bool Submitted { get; set; }

    public DestinationPageViewModel Content { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync() => await RenderAsync();

    public async Task<IActionResult> OnPostQuickAsync()
    {
        Quick.Destination = _destination;
        if (!ModelState.IsValid)
        {
            return await RenderAsync();
        }

        await _inquiries.AddQuickAsync(Quick);
        ModelState.Clear();
        Quick = new QuickInquiry { Destination = _destination };
        Submitted = true;
        return await RenderAsync();
    }

    private async Task<IActionResult> RenderAsync()
    {
        var page = await _content.CountryPageAsync(_slug);
        if (page is null)
        {
            return NotFound();
        }

        Content = new DestinationPageViewModel
        {
            Theme = page.Theme,
            HeroImageLabel = page.HeroImageLabel,
            HeroEyebrow = page.HeroEyebrow,
            HeroHeadline = page.HeroHeadline,
            HeroSub = page.HeroSub,
            OverviewEyebrow = page.OverviewEyebrow,
            OverviewQuote = page.OverviewQuote,
            OverviewBody = page.OverviewBody,
            Facts = page.Facts.ToList(),
            SectionOneTitle = page.SectionOneTitle,
            SectionOneLinkText = page.SectionOneLinkText,
            SectionOneLinkHref = page.SectionOneLinkHref,
            SectionOneCards = page.ServiceCards.ToList(),
            SectionTwoTitle = page.SectionTwoTitle,
            SectionTwoLinkText = page.SectionTwoLinkText,
            SectionTwoLinkHref = page.SectionTwoLinkHref,
            SectionTwoCards = page.ProjectCards.ToList(),
            SectionTwoNote = page.SectionTwoNote,
            CtaTitle = page.CtaTitle,
            CtaBody = page.CtaBody,
            CtaServiceLabel = page.CtaServiceLabel,
            CtaServiceOptions = page.CtaServices.ToList(),
            Quick = Quick,
            Submitted = Submitted,
        };
        return Page();
    }
}
