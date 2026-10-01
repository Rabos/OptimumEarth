namespace OptimumEarth.Web.Areas.Admin.Pages;

/// <summary>Everything the shared pager partial needs: where we are, how big a page is, and the other query values to keep.</summary>
public sealed record PagerModel(int Page, int Size, int Total, string Path, IReadOnlyDictionary<string, string> Query, int[] Sizes)
{
    public int Pages => Math.Max(1, (int)Math.Ceiling(Total / (double)Size));

    public string Url(int page)
    {
        var parts = Query.Where(kv => !string.IsNullOrEmpty(kv.Value) && kv.Key is not "p").Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}").ToList();
        parts.Add($"p={page}");
        return Path + "?" + string.Join('&', parts);
    }
}
