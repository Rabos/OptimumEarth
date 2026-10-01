using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Data;

namespace OptimumEarth.Web.Areas.Admin.Pages;

[AllowAnonymous]
public class LogoutModel : PageModel
{
    private readonly SignInManager<AppUser> _signIn;

    public LogoutModel(SignInManager<AppUser> signIn)
    {
        _signIn = signIn;
    }

    public IActionResult OnGet() => LocalRedirect("/admin");

    public async Task<IActionResult> OnPostAsync()
    {
        await _signIn.SignOutAsync();
        return LocalRedirect("/admin/login");
    }
}
