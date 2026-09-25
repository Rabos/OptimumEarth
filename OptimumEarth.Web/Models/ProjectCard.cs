namespace OptimumEarth.Web.Models;

/// <summary>A project/programme evidence card: an image slot, a tag, a title, a meta line, and a description.</summary>
public record ProjectCard(string ImageLabel, string Tag, string Title, string Meta, string Country = "", string Service = "", string ImagePath = "", string Description = "")
{
    public string Slug
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Title)) return "";
            return Title.ToLower().Replace(" ", "-").Replace("&", "and").Replace("'", "").Replace(",", "").Replace(".", "").Replace(":", "").Replace(";", "");
        }
    }
}

public record ProjectCardDescription(string ProjectSlug, string Description, string ImagePath, string Meta, string Title, string ImageLabel);