using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages.Blog;

public class IndexModel : PageModel
{
    private readonly ContentReader _content;

    public IndexModel(ContentReader content)
    {
        _content = content;
    }

    [BindProperty(SupportsGet = true, Name = "p")]
    public int PageNumber { get; set; } = 1;

    public BlogListPage Posts { get; private set; } = null!;

    public IReadOnlyList<BlogCategoryInfo> Categories { get; private set; } = Array.Empty<BlogCategoryInfo>();

    public string Intro { get; private set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        Posts = await _content.BlogPageAsync(null, PageNumber);
        if (Posts.Total == 0)
        {
            return NotFound();
        }

        Categories = await _content.BlogCategoriesAsync();
        Intro = (await _content.SettingsAsync()).BlogIntro;
        return Page();
    }
}
