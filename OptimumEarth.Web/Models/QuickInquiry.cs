using System.ComponentModel.DataAnnotations;

namespace OptimumEarth.Web.Models;

/// <summary>
/// Backs the short enquiry form in the CTA band of each destination page
/// (Uganda, Zambia, Foundation): name, email and a service of interest.
/// </summary>
public class QuickInquiry
{
    [Required(ErrorMessage = "Please tell us your name.")]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please add an email address so we can reply.")]
    [EmailAddress(ErrorMessage = "That doesn't look like a valid email address.")]
    [StringLength(180)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please choose a service of interest.")]
    public string Service { get; set; } = string.Empty;

    /// <summary>Which destination the enquiry was submitted from: Uganda, Zambia or Foundation.</summary>
    public string Destination { get; set; } = string.Empty;
}
