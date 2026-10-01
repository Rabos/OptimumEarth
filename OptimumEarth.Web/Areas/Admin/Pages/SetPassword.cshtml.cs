using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using OptimumEarth.Web.Data;

namespace OptimumEarth.Web.Areas.Admin.Pages;

/// <summary>Where an invited or reset user chooses their password, using the one-use link the super admin sends them.</summary>
[AllowAnonymous]
public class SetPasswordModel : PageModel
{
    private readonly UserManager<AppUser> _users;

    public SetPasswordModel(UserManager<AppUser> users)
    {
        _users = users;
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Choose a password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repeat the password.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "The two passwords do not match.")]
        public string Confirm { get; set; } = string.Empty;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int Uid { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Token { get; set; } = string.Empty;

    public string? Error { get; set; }

    public string? PersonName { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await FindAsync();
        if (user is null)
        {
            Error = "This link is no longer valid. Ask for a new one.";
            return Page();
        }

        PersonName = user.DisplayName;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await FindAsync();
        if (user is null)
        {
            Error = "This link is no longer valid. Ask for a new one.";
            return Page();
        }

        PersonName = user.DisplayName;
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string token;
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Token));
        }
        catch (FormatException)
        {
            Error = "This link is no longer valid. Ask for a new one.";
            return Page();
        }

        var result = await _users.ResetPasswordAsync(user, token, Input.Password);
        if (!result.Succeeded)
        {
            Error = string.Join(" ", result.Errors.Select(e => e.Code is "InvalidToken" ? "This link has expired or was already used. Ask for a new one." : e.Description));
            return Page();
        }

        user.EmailConfirmed = true;
        await _users.UpdateAsync(user);
        TempData["Flash"] = "Your password is set. Sign in below.";
        return LocalRedirect("/admin/login");
    }

    private async Task<AppUser?> FindAsync()
    {
        if (Uid <= 0 || string.IsNullOrEmpty(Token))
        {
            return null;
        }

        var user = await _users.FindByIdAsync(Uid.ToString());
        return user is { IsActive: true, IsHidden: false } ? user : null;
    }
}
