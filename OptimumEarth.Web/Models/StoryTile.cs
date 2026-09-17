namespace OptimumEarth.Web.Models;

/// <summary>One of the three Stories &amp; Impact tiles (Communities, Partnerships, Learning); clicking it opens a modal with ModalBody.</summary>
public record StoryTile(string Key, string Title, string Summary, string ModalBody, string ImageLabel, string Cover);
