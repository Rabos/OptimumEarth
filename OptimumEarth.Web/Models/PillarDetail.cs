namespace OptimumEarth.Web.Models;

/// <summary>One of the Foundation's three strategic pillars (Access, Resilience, Impact).</summary>
public record PillarDetail(string Key, string SdgLabel, string Title, string Description, List<string> Items);
