using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages.Blog;

/// <summary>What the shared blog list needs: the page of posts, the category chips, the active one, and the path paging links start from.</summary>
public sealed record BlogListVm(BlogListPage Posts, IReadOnlyList<BlogCategoryInfo> Categories, string? ActiveCategory, string Path);
