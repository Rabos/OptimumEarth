namespace OptimumEarth.Web.Data;

/// <summary>One Hostinger profile per destination. Passwords are protected with Data Protection.</summary>
public class EmailMailbox
{
    public string Destination { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ProtectedPassword { get; set; } = string.Empty;
}

public enum EmailDeliveryStatus { NotQueued, Pending, Sending, Sent, Failed, NotConfigured }
