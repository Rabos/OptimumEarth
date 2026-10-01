using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;

namespace OptimumEarth.Web.Areas.Admin.Pages;

public sealed record UserRow(int Id, string Name, string Email, string Role, string Status, DateTime? LastSignInUtc, int EditAreas, int ViewAreas);

public class UsersModel : AdminPageModel
{
    private readonly AppDbContext _db;

    public UsersModel(AppDbContext db)
    {
        _db = db;
    }

    protected override bool SuperAdminOnly => true;

    public List<UserRow> Rows { get; private set; } = new();

    public async Task OnGetAsync()
    {
        // The hidden super admin is never listed, counted or searchable.
        var users = await _db.Users.AsNoTracking().Where(u => !u.IsHidden).OrderBy(u => u.DisplayName).ThenBy(u => u.Email).ToListAsync();
        var roles = await (from ur in _db.UserRoles join r in _db.Roles on ur.RoleId equals r.Id select new { ur.UserId, r.Name }).ToListAsync();
        var access = await _db.AreaAccess.AsNoTracking().ToListAsync();

        Rows = users.Select(u =>
        {
            var role = roles.FirstOrDefault(r => r.UserId == u.Id)?.Name ?? Roles.Viewer;
            var mine = access.Where(a => a.UserId == u.Id).ToList();
            var edit = role == Roles.Viewer ? 0 : mine.Count(a => a.Level == AccessLevel.Edit);
            var view = mine.Count(a => a.Level == AccessLevel.View) + (role == Roles.Viewer ? mine.Count(a => a.Level == AccessLevel.Edit) : 0);
            var status = !u.IsActive ? "Deactivated" : u.PasswordHash is null ? "Invited" : "Active";
            return new UserRow(u.Id, string.IsNullOrWhiteSpace(u.DisplayName) ? u.Email ?? "" : u.DisplayName, u.Email ?? "", role, status, u.LastSignInUtc, edit, view);
        }).ToList();
    }
}
