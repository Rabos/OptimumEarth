using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Areas.Admin.Pages;

/// <summary>Any signed-in person can change their own name and password.</summary>
public class AccountModel : AdminPageModel
{
    private readonly UserManager<AppUser> _users;
    private readonly SignInManager<AppUser> _signIn;
    private readonly AuditService _audit;

    public AccountModel(UserManager<AppUser> users, SignInManager<AppUser> signIn, AuditService audit)
    {
        _users = users;
        _signIn = signIn;
        _audit = audit;
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Enter your current password.")]
        [DataType(DataType.Password)]
        public string Current { get; set; } = string.Empty;

        [Required(ErrorMessage = "Choose a new password.")]
        [DataType(DataType.Password)]
        public string New { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repeat the new password.")]
        [DataType(DataType.Password)]
        [Compare(nameof(New), ErrorMessage = "The two new passwords do not match.")]
        public string Confirm { get; set; } = string.Empty;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    [Required(ErrorMessage = "Enter your name.")]
    [StringLength(80)]
    public string DisplayName { get; set; } = string.Empty;

    public string? Error { get; set; }

    public void OnGet()
    {
        DisplayName = Access.User!.DisplayName;
    }

    public async Task<IActionResult> OnPostNameAsync()
    {
        ModelState.Remove("Input.Current");
        ModelState.Remove("Input.New");
        ModelState.Remove("Input.Confirm");
        if (!ModelState.IsValid || Access.User!.IsHidden)
        {
            return Page();
        }

        var user = await _users.FindByIdAsync(Access.User.Id.ToString());
        user!.DisplayName = DisplayName.Trim();
        await _users.UpdateAsync(user);
        TempData["Flash"] = "Name saved.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostPasswordAsync()
    {
        ModelState.Remove(nameof(DisplayName));
        DisplayName = Access.User!.DisplayName;
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _users.FindByIdAsync(Access.User.Id.ToString());
        var result = await _users.ChangePasswordAsync(user!, Input.Current, Input.New);
        if (!result.Succeeded)
        {
            Error = string.Join(" ", result.Errors.Select(e => e.Code == "PasswordMismatch" ? "Your current password is not right." : e.Description));
            return Page();
        }

        await _signIn.RefreshSignInAsync(user!);
        await _audit.LogAsync(string.Empty, "Account", "Changed own password", Access.Actor);
        TempData["Flash"] = "Password changed.";
        return RedirectToPage();
    }
}
