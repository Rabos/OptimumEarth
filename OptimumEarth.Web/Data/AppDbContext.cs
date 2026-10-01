using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OptimumEarth.Web.Data;

public class AppDbContext : IdentityDbContext<AppUser, Microsoft.AspNetCore.Identity.IdentityRole<int>, int>, IDataProtectionKeyContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>Where ASP.NET stores its signing keys, so logins and form tokens survive a redeploy.</summary>
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    public DbSet<Slide> Slides => Set<Slide>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Capability> Capabilities => Set<Capability>();
    public DbSet<CoreValue> CoreValues => Set<CoreValue>();
    public DbSet<Pillar> Pillars => Set<Pillar>();
    public DbSet<Principle> Principles => Set<Principle>();
    public DbSet<FocusArea> FocusAreas => Set<FocusArea>();
    public DbSet<SdgGoal> SdgGoals => Set<SdgGoal>();
    public DbSet<Story> Stories => Set<Story>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<CountryPage> CountryPages => Set<CountryPage>();
    public DbSet<CountryPageService> CountryPageServices => Set<CountryPageService>();
    public DbSet<CountryPageProject> CountryPageProjects => Set<CountryPageProject>();
    public DbSet<BlogCategory> BlogCategories => Set<BlogCategory>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<BlogRevision> BlogRevisions => Set<BlogRevision>();
    public DbSet<SiteSettings> Settings => Set<SiteSettings>();
    public DbSet<Inquiry> Inquiries => Set<Inquiry>();
    public DbSet<MediaAsset> Media => Set<MediaAsset>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();
    public DbSet<UserAreaAccess> AreaAccess => Set<UserAreaAccess>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<AppUser>().HasIndex(u => u.IsHidden);

        b.Entity<UserAreaAccess>(e =>
        {
            e.HasKey(a => new { a.UserId, a.Area });
            e.Property(a => a.Area).HasMaxLength(40);
            e.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<CountryPage>(e =>
        {
            e.HasIndex(p => p.Slug).IsUnique();
            e.HasMany(p => p.Services).WithOne(s => s.CountryPage).HasForeignKey(s => s.CountryPageId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(p => p.Projects).WithOne(s => s.CountryPage).HasForeignKey(s => s.CountryPageId).OnDelete(DeleteBehavior.Cascade);
        });

        // Deleting a service or project removes it from every page it was placed on.
        b.Entity<CountryPageService>().HasOne(s => s.Service).WithMany().HasForeignKey(s => s.ServiceId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<CountryPageProject>().HasOne(p => p.Project).WithMany().HasForeignKey(p => p.ProjectId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<CountryPageProject>().HasIndex(p => new { p.CountryPageId, p.ProjectId }).IsUnique();

        b.Entity<BlogCategory>(e =>
        {
            e.HasIndex(c => c.Slug).IsUnique();
            // A category with posts cannot be deleted.
            e.HasMany(c => c.Posts).WithOne(p => p.Category).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<BlogPost>(e =>
        {
            e.HasIndex(p => p.Slug).IsUnique();
            e.HasIndex(p => new { p.Status, p.PublishDate });
            e.HasMany(p => p.Revisions).WithOne(r => r.BlogPost).HasForeignKey(r => r.BlogPostId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<SiteSettings>().Property(s => s.ContentVersion).HasDefaultValue(1);

        b.Entity<MediaAsset>().HasIndex(m => m.Path).IsUnique();
        b.Entity<Inquiry>().HasIndex(i => i.CreatedUtc);
        b.Entity<AuditEntry>().HasIndex(a => a.WhenUtc);
    }
}
