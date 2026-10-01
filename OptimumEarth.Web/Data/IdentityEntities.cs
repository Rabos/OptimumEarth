using Microsoft.AspNetCore.Identity;

namespace OptimumEarth.Web.Data;

public static class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Editor = "Editor";
    public const string Viewer = "Viewer";
}

public class AppUser : IdentityUser<int>
{
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>The seeded super admin. Excluded from every user list, count and search.</summary>
    public bool IsHidden { get; set; }

    /// <summary>False once deactivated: the account can no longer sign in or use an open session.</summary>
    public bool IsActive { get; set; } = true;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastSignInUtc { get; set; }
}

public enum AccessLevel
{
    None = 0,
    View = 1,
    Edit = 2,
}

/// <summary>What one user may do on one page area. No row means no access.</summary>
public class UserAreaAccess
{
    public int UserId { get; set; }
    public AppUser? User { get; set; }
    public string Area { get; set; } = string.Empty;
    public AccessLevel Level { get; set; }
}

/// <summary>The page areas access can be allocated on, and the label each shows in the dashboard.</summary>
public static class AdminAreas
{
    public const string Home = "home";
    public const string Projects = "projects";
    public const string Services = "services";
    public const string About = "about";
    public const string Foundation = "foundation";
    public const string Blog = "blog";
    public const string Pages = "pages";
    public const string Inquiries = "inquiries";
    public const string Media = "media";
    public const string Settings = "settings";

    public static readonly IReadOnlyList<(string Key, string Label)> All = new List<(string, string)>
    {
        (Home, "Home"),
        (Projects, "Projects"),
        (Services, "Services"),
        (About, "Who we are"),
        (Foundation, "Foundation"),
        (Blog, "Blog"),
        (Pages, "Country pages"),
        (Inquiries, "Inquiries"),
        (Media, "Media library"),
        (Settings, "Site settings"),
    };

    public static string Label(string key) => All.FirstOrDefault(a => a.Key == key).Label ?? key;
}
