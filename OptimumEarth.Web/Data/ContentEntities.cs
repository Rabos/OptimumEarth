namespace OptimumEarth.Web.Data;

public enum ContentStatus
{
    Draft = 0,
    Published = 1,
}

/// <summary>
/// Shared shape of every editable list item: a manual order, a draft/published
/// switch and a last-changed stamp. Public pages only ever read Published rows.
/// </summary>
public abstract class ContentEntity
{
    public int Id { get; set; }
    public int SortOrder { get; set; }
    public ContentStatus Status { get; set; } = ContentStatus.Published;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}

/// <summary>One slide of the Home page hero carousel.</summary>
public class Slide : ContentEntity
{
    public string Eyebrow { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
}

public class Project : ContentEntity
{
    public string Title { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string Meta { get; set; } = string.Empty;

    /// <summary>"uganda" or "zambia" - matches the public Projects filter keys.</summary>
    public string Country { get; set; } = "uganda";

    /// <summary>"water", "groundwater", "environment" or "gis" - matches the public filter keys.</summary>
    public string Service { get; set; } = "water";

    public string ImagePath { get; set; } = string.Empty;

    /// <summary>The fuller write-up shown with the project: in the Projects page's detail dialog and on country page cards.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Whether the card appears on the public Projects page itself.</summary>
    public bool OnListPage { get; set; } = true;
}

public class Service : ContentEntity
{
    public string Title { get; set; } = string.Empty;
    public string Coverage { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>The bullet list under "What we deliver" on the Services page.</summary>
    public List<string> Deliverables { get; set; } = new();
    public string ImagePath { get; set; } = string.Empty;

    /// <summary>Whether the row appears on the public Services page itself.</summary>
    public bool OnListPage { get; set; } = true;
}

public class Capability : ContentEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CoreValue : ContentEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class Pillar : ContentEntity
{
    public string Key { get; set; } = string.Empty;
    public string SdgLabel { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Items { get; set; } = new();
}

public class Principle : ContentEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class FocusArea : ContentEntity
{
    public string Label { get; set; } = string.Empty;
}

public class SdgGoal : ContentEntity
{
    public string Number { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;

    /// <summary>The goal's approach document (a PDF), opened in a lightbox when the goal is clicked. Empty means no lightbox.</summary>
    public string DocumentPath { get; set; } = string.Empty;
}

/// <summary>A Stories &amp; Impact tile. Key links it to its gallery images.</summary>
public class Story : ContentEntity
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string CoverPath { get; set; } = string.Empty;
}

public class GalleryImage : ContentEntity
{
    /// <summary>Matches Story.Key.</summary>
    public string StoryKey { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
}
