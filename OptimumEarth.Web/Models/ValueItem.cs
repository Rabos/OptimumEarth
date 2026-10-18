namespace OptimumEarth.Web.Models;

public record ValueItem(string Title, string Description)
{
    /// <summary>The description's first sentence, shown as the value's lead line.</summary>
    public string Lead => Split().Lead;

    /// <summary>The rest of the description after the lead.</summary>
    public string Body => Split().Body;

    private (string Lead, string Body) Split()
    {
        var end = Description.IndexOf(". ", StringComparison.Ordinal);
        return end < 0 ? (Description, string.Empty) : (Description[..(end + 1)], Description[(end + 2)..].Trim());
    }
}
