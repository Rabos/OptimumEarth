using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;

namespace OptimumEarth.Web.Services;

/// <summary>What the signed-in person may do, resolved once per request from the database.</summary>
public sealed class AccessSnapshot
{
    private readonly Dictionary<string, AccessLevel> _levels;

    public AccessSnapshot(AppUser? user, string role, Dictionary<string, AccessLevel> levels)
    {
        User = user;
        Role = role;
        _levels = levels;
    }

    /// <summary>Null when nobody valid is signed in (including a deactivated account).</summary>
    public AppUser? User { get; }

    public string Role { get; }
    public bool IsSuperAdmin => Role == Roles.SuperAdmin;
    public bool IsSignedIn => User is not null;

    /// <summary>The name shown in the audit log. The hidden super admin never appears under its own name.</summary>
    public string Actor => User is null ? "Unknown" : User.IsHidden ? "Platform admin" : (string.IsNullOrWhiteSpace(User.DisplayName) ? User.Email ?? "User" : User.DisplayName);

    public AccessLevel Level(string area)
    {
        if (!IsSignedIn)
        {
            return AccessLevel.None;
        }

        if (IsSuperAdmin)
        {
            return AccessLevel.Edit;
        }

        var level = _levels.GetValueOrDefault(area, AccessLevel.None);

        // The role is the ceiling: a Viewer can never edit, whatever was ticked.
        return Role == Roles.Viewer && level == AccessLevel.Edit ? AccessLevel.View : level;
    }

    public bool CanView(string area) => Level(area) >= AccessLevel.View;
    public bool CanEdit(string area) => Level(area) >= AccessLevel.Edit;
    public bool HasAnyArea => IsSuperAdmin || AdminAreas.All.Any(a => CanView(a.Key));
}

/// <summary>
/// Looks up the current user's role and page allocations. Reads the database on
/// every request, so a change made by the super admin, or a deactivation, takes
/// effect on the person's very next click rather than at their next sign-in.
/// </summary>
public sealed class AccessContext
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _http;
    private readonly UserManager<AppUser> _users;
    private AccessSnapshot? _snapshot;

    public AccessContext(AppDbContext db, IHttpContextAccessor http, UserManager<AppUser> users)
    {
        _db = db;
        _http = http;
        _users = users;
    }

    public async Task<AccessSnapshot> GetAsync()
    {
        if (_snapshot is not null)
        {
            return _snapshot;
        }

        var principal = _http.HttpContext?.User;
        var idText = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (principal?.Identity?.IsAuthenticated != true || !int.TryParse(idText, out var id))
        {
            return _snapshot = new AccessSnapshot(null, string.Empty, new());
        }

        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (user is null || !user.IsActive)
        {
            return _snapshot = new AccessSnapshot(null, string.Empty, new());
        }

        var roles = await _users.GetRolesAsync(user);
        var role = roles.Contains(Roles.SuperAdmin) ? Roles.SuperAdmin : roles.Contains(Roles.Editor) ? Roles.Editor : Roles.Viewer;
        var levels = await _db.AreaAccess.AsNoTracking().Where(a => a.UserId == id).ToDictionaryAsync(a => a.Area, a => a.Level);
        return _snapshot = new AccessSnapshot(user, role, levels);
    }
}
