using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Areas.Admin;

/// <summary>
/// Base of every signed-in dashboard page. Before any handler runs it resolves
/// the person's access from the database and refuses the request unless they
/// have it: View on the page's area to read, Edit to change anything.
/// This is the real protection; hiding a menu link is only a convenience.
/// </summary>
public abstract class AdminPageModel : PageModel
{
    /// <summary>The page area this page belongs to, or null for pages every signed-in person may open.</summary>
    protected virtual string? Area => null;

    /// <summary>Pages only the super admin may open (user management).</summary>
    protected virtual bool SuperAdminOnly => false;

    public AccessSnapshot Access { get; private set; } = null!;

    public bool CanEdit => Area is null ? Access.IsSuperAdmin : Access.CanEdit(Area);

    public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        Access = await HttpContext.RequestServices.GetRequiredService<AccessContext>().GetAsync();

        if (!Access.IsSignedIn)
        {
            // A deactivated account, or a cookie for a user that no longer exists.
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            context.Result = new RedirectToPageResult("/Login", new { area = "Admin" });
            return;
        }

        if (SuperAdminOnly && !Access.IsSuperAdmin)
        {
            context.Result = new ForbidResult();
            return;
        }

        if (Area is not null)
        {
            var reading = HttpMethods.IsGet(Request.Method) || HttpMethods.IsHead(Request.Method);
            var needed = reading ? AccessLevel.View : AccessLevel.Edit;
            if (Access.Level(Area) < needed)
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        await next();
    }
}
