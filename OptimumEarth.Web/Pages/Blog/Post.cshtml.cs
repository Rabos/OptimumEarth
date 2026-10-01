using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Pages.Blog;

public class PostModel : PageModel
{
    private readonly ContentReader _content;

    public PostModel(ContentReader content)
    {
        _content = content;
    }

    public BlogPostView Post { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        var post = await _content.BlogPostAsync(slug);
        if (post is null)
        {
            return NotFound();
        }

        Post = post;
        return Page();
    }
}
