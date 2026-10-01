using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Areas.Admin.Pages;

public class UserEditModel : AdminPageModel
{
    private readonly UserManager<AppUser> _users;
    private readonly AppDbContext _db;
    private readonly AuditService _audit;

    public UserEditModel(UserManager<AppUser> users, AppDbContext db, AuditService audit)
    {
        _users = users;
        _db = db;
        _audit = audit;
    }

    protected override bool SuperAdminOnly => true;

    public class InputModel
    {
        [Required(ErrorMessage = "Enter the person's name.")]
        [StringLength(80)]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter an email address.")]
        [EmailAddress(ErrorMessage = "That doesn't look like an email address.")]
        [StringLength(180)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression("Editor|Viewer", ErrorMessage = "Choose Editor or Viewer.")]
        public string Role { get; set; } = Roles.Viewer;

        /// <summary>Area key to "none", "view" or "edit".</summary>
        public Dictionary<string, string> Access { get; set; } = new();
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public int? UserId { get; private set; }

    public string Status { get; private set; } = "New";

    public bool IsActive { get; private set; } = true;

    /// <summary>A one-use link to hand to the person. Shown once after inviting or resetting; no email is sent.</summary>
    public string? SetPasswordLink { get; private set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            Input = new InputModel { Access = AdminAreas.All.ToDictionary(a => a.Key, _ => "none") };
            return Page();
        }

        return await LoadAsync(id.Value) ? Page() : NotFound();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        Normalise();
        if (!ModelState.IsValid)
        {
            await FillStateAsync(id);
            return Page();
        }

        var email = Input.Email.Trim();
        var clash = await _db.Users.AnyAsync(u => u.NormalizedEmail == email.ToUpper() && u.Id != (id ?? 0));
        if (clash)
        {
            ModelState.AddModelError("Input.Email", "Someone already has this email address.");
            await FillStateAsync(id);
            return Page();
        }

        AppUser user;
        var isNew = id is null;
        if (isNew)
        {
            user = new AppUser { UserName = email, Email = email, DisplayName = Input.DisplayName.Trim(), EmailConfirmed = false };
            var created = await _users.CreateAsync(user);
            if (!created.Succeeded)
            {
                ModelState.AddModelError(string.Empty, string.Join(" ", created.Errors.Select(e => e.Description)));
                await FillStateAsync(id);
                return Page();
            }

            await _users.AddToRoleAsync(user, Input.Role);
        }
        else
        {
            var found = await _users.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsHidden);
            if (found is null)
            {
                return NotFound();
            }

            user = found;
            user.DisplayName = Input.DisplayName.Trim();
            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                await _users.SetEmailAsync(user, email);
                await _users.SetUserNameAsync(user, email);
            }

            var current = (await _users.GetRolesAsync(user)).FirstOrDefault();
            if (current != Input.Role)
            {
                if (current is not null)
                {
                    await _users.RemoveFromRoleAsync(user, current);
                }

                await _users.AddToRoleAsync(user, Input.Role);
            }

            await _users.UpdateAsync(user);
            // Roles or access changed: end their current sessions so nothing stale lingers.
            await _users.UpdateSecurityStampAsync(user);
        }

        await SaveAccessAsync(user.Id);
        await _audit.LogAsync(string.Empty, "Users", isNew ? "Invited user" : "Changed access", email);

        if (isNew)
        {
            // Show the invite link right here: there is no mail server, so the super admin passes it on.
            UserId = user.Id;
            Status = "Invited";
            IsActive = true;
            SetPasswordLink = await BuildLinkAsync(user);
            ModelState.Clear();
            return Page();
        }

        TempData["Flash"] = "Saved. The change applies on their next click.";
        return RedirectToPage(new { id = user.Id });
    }

    public async Task<IActionResult> OnPostResetAsync(int id)
    {
        var user = await _users.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsHidden);
        if (user is null)
        {
            return NotFound();
        }

        await LoadAsync(id);
        SetPasswordLink = await BuildLinkAsync(user);
        await _audit.LogAsync(string.Empty, "Users", "Created password link", user.Email ?? string.Empty);
        return Page();
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(int id)
    {
        var user = await _users.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsHidden);
        if (user is null)
        {
            return NotFound();
        }

        user.IsActive = !user.IsActive;
        await _users.UpdateAsync(user);
        await _users.UpdateSecurityStampAsync(user);
        await _audit.LogAsync(string.Empty, "Users", user.IsActive ? "Reactivated user" : "Deactivated user", user.Email ?? string.Empty);
        TempData["Flash"] = user.IsActive ? "Reactivated." : "Deactivated. They are signed out and can no longer sign in.";
        return RedirectToPage(new { id });
    }

    private async Task<bool> LoadAsync(int id)
    {
        var user = await _users.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id && !u.IsHidden);
        if (user is null)
        {
            return false;
        }

        var role = (await _users.GetRolesAsync(user)).FirstOrDefault() ?? Roles.Viewer;
        var stored = await _db.AreaAccess.AsNoTracking().Where(a => a.UserId == id).ToDictionaryAsync(a => a.Area, a => a.Level);
        Input = new InputModel
        {
            DisplayName = user.DisplayName,
            Email = user.Email ?? string.Empty,
            Role = role,
            Access = AdminAreas.All.ToDictionary(a => a.Key, a => ToText(Cap(role, stored.GetValueOrDefault(a.Key)))),
        };
        UserId = id;
        IsActive = user.IsActive;
        Status = !user.IsActive ? "Deactivated" : user.PasswordHash is null ? "Invited" : "Active";
        return true;
    }

    private async Task FillStateAsync(int? id)
    {
        UserId = id;
        if (id is not null)
        {
            var user = await _users.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            IsActive = user?.IsActive ?? true;
            Status = user is null ? "New" : !user.IsActive ? "Deactivated" : user.PasswordHash is null ? "Invited" : "Active";
        }
    }

    /// <summary>Make every area present, drop unknown keys, and cap a Viewer at View.</summary>
    private void Normalise()
    {
        var clean = new Dictionary<string, string>();
        foreach (var (key, _) in AdminAreas.All)
        {
            var value = Input.Access.GetValueOrDefault(key)?.ToLowerInvariant();
            value = value is "view" or "edit" ? value : "none";
            if (Input.Role == Roles.Viewer && value == "edit")
            {
                value = "view";
            }

            clean[key] = value;
        }

        Input.Access = clean;
    }

    private async Task SaveAccessAsync(int userId)
    {
        var existing = await _db.AreaAccess.Where(a => a.UserId == userId).ToListAsync();
        _db.AreaAccess.RemoveRange(existing);
        foreach (var (area, value) in Input.Access)
        {
            if (value != "none")
            {
                _db.AreaAccess.Add(new UserAreaAccess { UserId = userId, Area = area, Level = value == "edit" ? AccessLevel.Edit : AccessLevel.View });
            }
        }

        await _db.SaveChangesAsync();
    }

    private async Task<string> BuildLinkAsync(AppUser user)
    {
        var token = await _users.GeneratePasswordResetTokenAsync(user);
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        return Url.Page("/SetPassword", null, new { area = "Admin", uid = user.Id, token = encoded }, Request.Scheme, Request.Host.ToString())!;
    }

    private static AccessLevel Cap(string role, AccessLevel level) => role == Roles.Viewer && level == AccessLevel.Edit ? AccessLevel.View : level;

    private static string ToText(AccessLevel level) => level switch { AccessLevel.Edit => "edit", AccessLevel.View => "view", _ => "none" };
}
