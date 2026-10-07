namespace OptimumEarth.Web.Data;

/// <summary>Site-wide values. A single row (Id 1).</summary>
public class SiteSettings
{
    public int Id { get; set; } = 1;
    public string Mission { get; set; } = string.Empty;
    public string Vision { get; set; } = string.Empty;

    /// <summary>How many projects a country page may show (the layout fits three across).</summary>
    public int ProjectSlots { get; set; } = 3;
    public int BlogPageSize { get; set; } = 9;
    public string BlogIntro { get; set; } = string.Empty;
    public string UgandaTo { get; set; } = string.Empty;
    public string ZambiaTo { get; set; } = string.Empty;
    public string FoundationTo { get; set; } = string.Empty;
    public string DefaultEmailDestination { get; set; } = "Uganda";

    /// <summary>Enquiries older than this are deleted, since they hold personal data.</summary>
    public int RetentionMonths { get; set; } = 24;

    /// <summary>
    /// Which revision of the built-in content this database has been brought up to.
    /// Existing databases start at 1; see <see cref="ContentUpgrades"/>.
    /// </summary>
    public int ContentVersion { get; set; } = 1;
}

public enum InquiryStatus
{
    New = 0,
    Replied = 1,
    Closed = 2,
}

/// <summary>An enquiry submitted through the Contact page or a country page's quick form.</summary>
public class Inquiry
{
    public int Id { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>"Contact" for the full form, "Quick" for the short one.</summary>
    public string Source { get; set; } = "Contact";

    /// <summary>Routing target: Uganda, Zambia, Foundation, or "Not sure yet".</summary>
    public string Destination { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Organisation { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public InquiryStatus Status { get; set; } = InquiryStatus.New;
    public string Note { get; set; } = string.Empty;
    public EmailDeliveryStatus EmailDelivery { get; set; } = EmailDeliveryStatus.NotQueued;
    public DateTime? EmailAttemptUtc { get; set; }
    public DateTime? EmailSentUtc { get; set; }
    public string EmailDeliveryError { get; set; } = string.Empty;
}

public class MediaAsset
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;

    /// <summary>Public URL, e.g. /img/optimum-earth-images/borehole.jpg or /uploads/abc.jpg.</summary>
    public string Path { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>True for the images shipped in the repository, which cannot be deleted here.</summary>
    public bool BuiltIn { get; set; }
}

public class AuditEntry
{
    public int Id { get; set; }
    public DateTime WhenUtc { get; set; } = DateTime.UtcNow;
    public string Actor { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;

    /// <summary>The page area key (see <see cref="AdminAreas"/>) so entries can be filtered by access. Empty for account-level events.</summary>
    public string Area { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string Item { get; set; } = string.Empty;
}
