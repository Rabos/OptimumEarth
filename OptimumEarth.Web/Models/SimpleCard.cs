namespace OptimumEarth.Web.Models;

/// <summary>A plain card used for service lists and Foundation pillars. Index is set only for pillar cards.</summary>
public record SimpleCard(string Title, string Description, string? Index = null);
