using OptimumEarth.Web.Data;

namespace OptimumEarth.Web.Services;

/// <summary>Records who changed what. Called by the dashboard after every change.</summary>
public sealed class AuditService
{
    private readonly AppDbContext _db;
    private readonly AccessContext _access;

    public AuditService(AppDbContext db, AccessContext access)
    {
        _db = db;
        _access = access;
    }

    public async Task LogAsync(string area, string section, string action, string item)
    {
        var who = await _access.GetAsync();
        _db.AuditEntries.Add(new AuditEntry
        {
            Actor = who.Actor,
            Area = area,
            Section = section,
            Action = action,
            Item = Trim(item, 200),
        });
        await _db.SaveChangesAsync();
    }

    private static string Trim(string value, int max) => value.Length <= max ? value : value[..max];
}
