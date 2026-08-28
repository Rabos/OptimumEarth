namespace OptimumEarth.Web.Models;

public class ContactPanelViewModel
{
    public required ContactInquiry Contact { get; set; }
    public string Heading { get; set; } = "Let's work together";
    public string Intro { get; set; } = "Tell us a little about what you need and the right team will come back to you.";
    public List<(string Label, string Value)> InfoItems { get; set; } = new();
    public List<string> Reasons { get; set; } = new();
    public bool Submitted { get; set; }
}
