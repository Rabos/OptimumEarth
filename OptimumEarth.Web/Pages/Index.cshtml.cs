using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public ContactInquiry Contact { get; set; } = new() { Destination = "Gateway" };

    public ContactPanelViewModel Panel { get; set; } = null!;
    public bool Submitted { get; set; }

    private static readonly List<string> Reasons = new()
    {
        "Client enquiry — Uganda",
        "Client enquiry — Zambia",
        "Partner with the Foundation",
        "Media / press",
        "General enquiry"
    };

    public void OnGet()
    {
        BuildPanel();
    }

    public IActionResult OnPostContact()
    {
        Contact.Destination = "Gateway";

        if (!ModelState.IsValid)
        {
            BuildPanel();
            return Page();
        }

        // In production this would persist the enquiry and route it by
        // Reason (Uganda / Zambia / Foundation / general) to the right
        // regional inbox, e.g. via an IContactInquiryService.
        Submitted = true;
        ModelState.Clear();
        Contact = new ContactInquiry { Destination = "Gateway" };
        BuildPanel(submitted: true);
        return Page();
    }

    private void BuildPanel(bool submitted = false)
    {
        Submitted = submitted;
        Panel = new ContactPanelViewModel
        {
            Contact = Contact,
            Heading = "Let's work together",
            Intro = "Whether you're a client in Uganda or Zambia, a prospective Foundation partner, or just want to learn more — tell us and the right team will follow up.",
            Reasons = Reasons,
            Submitted = submitted,
            InfoItems = new()
            {
                ("Email", "info@optimum-earth.com"),
                ("Uganda office", "Kampala, Uganda"),
                ("Zambia office", "Lusaka, Zambia"),
            }
        };
    }
}
