using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class WhoWeAreModel : PageModel
{
    public List<CapabilityItem> Capabilities { get; } = new()
    {
        new("01", "Technical Studies", "Know the ground before you commit capital."),
        new("02", "Engineering Design", "Designs built on real site data."),
        new("03", "Construction Services", "Built by the team that designed it."),
        new("04", "Environmental & Social Impact Assessments", "Approvals that satisfy both the regulator and the lender"),
        new("05", "Climate Resilience & Natural Resources", "Planning water and land for the climate ahead."),
    };

    public string Mission { get; } = "We are committed to value-driven partnerships with our clients by providing services of the highest quality, safety, and integrity while focusing on customer satisfaction and innovative solutions. We strive to be the company that clients want to work with and employees want to work for, by providing excellent results and rewarding careers.";

    public string Vision { get; } = "To be the leading global engineering and environmental services provider, focused on creating value and success for our clients.";

    public List<ValueItem> Values { get; } = new()
    {
        new("Safety", "Safety comes first, always. Everyone is responsible for their own health and safety and that of their colleagues, the public and the communities where we work. No deadline, task or budget justifies compromising it, and anyone may stop work they believe is unsafe. We protect the dignity and wellbeing of everyone our work touches, especially children and vulnerable people."),
        new("Collaboration", "Better solutions are built together. We work with colleagues, clients, donors, communities, partners and government with respect, inclusion and openness. We value the contributions of a diverse and talented team and seek out people who are passionate, curious and committed to those we serve. We invest in one another's professional development."),
        new("Accountability & Integrity", "We act with honesty and the highest ethics in every dealing, commercial or charitable, and we do the right thing even when no one is watching. We do not tolerate bribery, fraud or undeclared conflicts of interest. We own our commitments, decisions and results, raise problems early, and answer to our clients, donors, regulators and the communities we serve."),
        new("Learning & Innovation", "We stay curious and challenge complacency. We learn from experience and from those we serve, apply appropriate technology, and continually adapt our solutions to meet changing needs."),
        new("Excellence", "We aim to do our work right the first time. From technical services and engineering delivery to community programmes and partnerships, we hold high standards of quality. We treat quality as a shared responsibility and deliver work that is reliable, measurable and fit for purpose."),
        new("Stewardship", "We take responsibility for everything entrusted to us: client assets, company resources, donor and grant funds, and the communities and environments where we work. We use resources responsibly, account for them transparently, create lasting value, and leave people, communities and the planet better than we found them."),
    };

    public void OnGet()
    {
    }
}
