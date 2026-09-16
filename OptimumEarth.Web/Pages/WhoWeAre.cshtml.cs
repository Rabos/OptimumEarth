using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Models;

namespace OptimumEarth.Web.Pages;

public class WhoWeAreModel : PageModel
{
    public List<CapabilityItem> Capabilities { get; } = new()
    {
        new("01", "Water resources", "Supply, diagnostics, monitoring"),
        new("02", "Energy", "Geothermal and infrastructure"),
        new("03", "Environment", "Assessment, audit, compliance"),
        new("04", "GIS & mapping", "Spatial analysis and mapping"),
        new("05", "Engineering", "Full project lifecycle"),
    };

    public string Mission { get; } = "We are committed to value-driven partnerships with our clients by providing services of the highest quality, safety, and integrity while focusing on customer satisfaction and innovative solutions. We strive to be the company that clients want to work with and employees want to work for, by providing excellent results and rewarding careers.";

    public string Vision { get; } = "To be the leading global engineering and environmental services provider, focused on creating value and success for our clients.";

    public List<ValueItem> Values { get; } = new()
    {
        new("Culture", "We will cultivate a culture that values the contributions of our diverse and talented team members. We will seek out and encourage colleagues who are passionate, curious and client-focused. We will maintain an environment of mutual respect and commitment to professional development."),
        new("Integrity", "We will act with the highest ethics, honesty, and respect in all business dealings. Integrity is at the heart of who we are and what we do. We will treat customers and company resources with the respect they deserve. We will do the right thing."),
        new("Innovation", "We will apply technology and evolve our solutions to fit the needs of our clients. We will attack complacency and continually improve to foster the development of new and creative solutions."),
        new("Safety and Health", "Safety and Health are our top priority. We expect everyone to actively participate in and take responsibility for their own safety, the safety of the public, and the safety of those around them. Clients entrust us to keep safety first in every service we provide."),
        new("Quality", "We will be passionate about excellence and doing our work right the first time. Quality is a shared responsibility. We will hold each other accountable and maintain a reputation for delivering."),
    };

    public void OnGet()
    {
    }
}
