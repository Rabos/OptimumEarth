namespace OptimumEarth.Web.Data;

public class BlogCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public List<BlogPost> Posts { get; set; } = new();
}

public class BlogPost : ContentEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public BlogCategory? Category { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateOnly PublishDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string CoverPath { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;

    /// <summary>The article as Markdown. Converted and sanitised when rendered.</summary>
    public string Body { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public bool Pinned { get; set; }
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public List<BlogRevision> Revisions { get; set; } = new();

    /// <summary>Visible to visitors: published, and its publish date has arrived.</summary>
    public bool IsLive(DateOnly today) => Status == ContentStatus.Published && PublishDate <= today;
}

/// <summary>A saved version of a post. The last few are kept so a bad edit can be recovered.</summary>
public class BlogRevision
{
    public int Id { get; set; }
    public int BlogPostId { get; set; }
    public BlogPost? BlogPost { get; set; }
    public DateTime SavedUtc { get; set; } = DateTime.UtcNow;
    public string SavedBy { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
