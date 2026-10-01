using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages.Blog;

public class CategoryModel : PageModel
{
    private readonly ContentReader _content;

    public CategoryModel(ContentReader content)
    {
        _content = content;
    }

    [BindProperty(SupportsGet = true, Name = "p")]
    public int PageNumber { get; set; } = 1;

    public BlogListPage Posts { get; private set; } = null!;

    public IReadOnlyList<BlogCategoryInfo> Categories { get; private set; } = Array.Empty<BlogCategoryInfo>();

    public BlogCategoryInfo Category { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        var category = await _content.BlogCategoryAsync(slug);
        if (category is null)
        {
            return NotFound();
        }

        Category = category;
        Posts = await _content.BlogPageAsync(slug, PageNumber);
        Categories = await _content.BlogCategoriesAsync();
        return Page();
    }
}
