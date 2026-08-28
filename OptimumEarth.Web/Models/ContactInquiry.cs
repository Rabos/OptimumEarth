using System.ComponentModel.DataAnnotations;

namespace OptimumEarth.Web.Models;

/// <summary>
/// Backs the contact / inquiry form on the gateway and every destination
/// page (Uganda, Zambia, Foundation). Kept generic so the same partial
/// view and posting pattern can be reused everywhere, with "Destination"
/// recording which part of the site the enquiry came from.
/// </summary>
public class ContactInquiry
{
    [Required(ErrorMessage = "Please tell us your name.")]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please add an email address so we can reply.")]
    [EmailAddress(ErrorMessage = "That doesn't look like a valid email address.")]
    [StringLength(180)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "That doesn't look like a valid phone number.")]
    [StringLength(40)]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Please choose what you'd like to talk to us about.")]
    public string Reason { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please add a short message.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Please add a little more detail (at least 10 characters).")]
    public string Message { get; set; } = string.Empty;

    /// <summary>Which destination the enquiry was submitted from: Gateway, Uganda, Zambia or Foundation.</summary>
    public string Destination { get; set; } = "Gateway";
}
