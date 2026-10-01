using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace OptimumEarth.Web.Services;

/// <summary>
/// Holds the public pages' content in memory so a page view does not query the
/// database. Any change made in the dashboard calls <see cref="Invalidate"/>,
/// which drops everything at once; the next view reads fresh values.
/// </summary>
public sealed class ContentCache
{
    private readonly IMemoryCache _cache;
    private CancellationTokenSource _version = new();

    public ContentCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory) where T : class
    {
        if (_cache.TryGetValue(key, out T? cached) && cached is not null)
        {
            return cached;
        }

        var created = await factory();
        var options = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) };
        options.AddExpirationToken(new CancellationChangeToken(_version.Token));
        _cache.Set(key, created, options);
        return created;
    }

    public void Invalidate()
    {
        var previous = Interlocked.Exchange(ref _version, new CancellationTokenSource());
        previous.Cancel();
        previous.Dispose();
    }
}
