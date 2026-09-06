namespace OptimumEarth.Web.Models;

/// <summary>A project/programme evidence card: an image slot, a tag, a title and a meta line.</summary>
public record ProjectCard(string ImageLabel, string Tag, string Title, string Meta, string Country = "", string Service = "");
