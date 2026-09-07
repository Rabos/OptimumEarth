using System.ComponentModel.DataAnnotations;

namespace OptimumEarth.Web.Models;

/// <summary>Backs the full enquiry form on the dedicated Contact page.</summary>
public class ContactInquiry
{
    [Required(ErrorMessage = "Please tell us your name.")]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(160)]
    public string? Organisation { get; set; }

    [Required(ErrorMessage = "Please add an email address so we can reply.")]
    [EmailAddress(ErrorMessage = "That doesn't look like a valid email address.")]
    [StringLength(180)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "That doesn't look like a valid phone number.")]
    [StringLength(40)]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Please tell us which audience you're getting in touch as.")]
    public string Audience { get; set; } = "Client";

    [Required(ErrorMessage = "Please choose a country.")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please choose a service.")]
    public string Service { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please tell us how we can help.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Please add a little more detail (at least 10 characters).")]
    public string Message { get; set; } = string.Empty;

    /// <summary>Computed routing target: Uganda, Zambia or Foundation.</summary>
    public string Destination => Audience == "Foundation supporter" ? "Foundation" : Country;
}
