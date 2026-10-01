using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;

namespace OptimumEarth.Web.Areas.Admin.Pages;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signIn;
    private readonly UserManager<AppUser> _users;

    public LoginModel(SignInManager<AppUser> signIn, UserManager<AppUser> users)
    {
        _signIn = signIn;
        _users = users;
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Enter your email address.")]
        [EmailAddress(ErrorMessage = "That doesn't look like an email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? Error { get; set; }

    public IActionResult OnGet()
    {
        return User.Identity?.IsAuthenticated == true ? LocalRedirect("/admin") : Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // One message for every failure, so the form does not reveal which emails have accounts.
        const string generic = "Email or password is incorrect.";
        var user = await _users.FindByEmailAsync(Input.Email.Trim());
        if (user is null || !user.IsActive)
        {
            Error = generic;
            return Page();
        }

        var result = await _signIn.PasswordSignInAsync(user, Input.Password, isPersistent: false, lockoutOnFailure: true);
        if (result.IsLockedOut)
        {
            Error = "Too many failed attempts. Try again in 15 minutes.";
            return Page();
        }

        if (!result.Succeeded)
        {
            Error = generic;
            return Page();
        }

        await _users.Users.Where(u => u.Id == user.Id).ExecuteUpdateAsync(s => s.SetProperty(u => u.LastSignInUtc, DateTime.UtcNow));
        return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : LocalRedirect("/admin");
    }
}
