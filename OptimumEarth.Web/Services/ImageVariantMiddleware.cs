using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;

namespace OptimumEarth.Web.Services;

/// <summary>Only three permitted sizes, cached on disk outside the deployed app.</summary>
public sealed class ImageVariantMiddleware(RequestDelegate next, IWebHostEnvironment env, IConfiguration config, ILogger<ImageVariantMiddleware> logger)
{
    private static readonly SemaphoreSlim Processing = new(1, 1);

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";
        var extension = Path.GetExtension(path).ToLowerInvariant();
        if (!HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method)
            || extension is not (".jpg" or ".jpeg" or ".png" or ".webp")
            || !int.TryParse(context.Request.Query["w"], out var width)
            || !ImageOptimizer.Widths.Contains(width))
        {
            await next(context);
            return;
        }
        var uploads = MediaService.ResolveUploadsPath(config, env);
        using var uploadProvider = new PhysicalFileProvider(uploads);
        var source = path.StartsWith("/uploads/", StringComparison.Ordinal)
            ? uploadProvider.GetFileInfo(path[9..])
            : path.StartsWith("/img/", StringComparison.Ordinal) ? env.WebRootFileProvider.GetFileInfo(path) : null;
        if (source is not { Exists: true, PhysicalPath: not null } || source.Length > 20 * 1024 * 1024)
        {
            await next(context);
            return;
        }
        var key = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"v1|{source.PhysicalPath}|{source.LastModified.UtcTicks}|{source.Length}|{width}")));
        var cacheRoot = Path.Combine(uploads, ".image-cache");
        var cached = Path.Combine(cacheRoot, key + ".webp");
        try
        {
            await Processing.WaitAsync(context.RequestAborted);
            try
            {
                if (!File.Exists(cached))
                {
                    var bytes = await File.ReadAllBytesAsync(source.PhysicalPath, context.RequestAborted);
                    var optimized = ImageOptimizer.Optimize(bytes, width);
                    Directory.CreateDirectory(cacheRoot);
                    var temporary = cached + "." + Guid.NewGuid().ToString("N") + ".tmp";
                    try
                    {
                        await File.WriteAllBytesAsync(temporary, optimized, context.RequestAborted);
                        File.Move(temporary, cached, overwrite: true);
                    }
                    finally { if (File.Exists(temporary)) File.Delete(temporary); }
                }
            }
            finally { Processing.Release(); }
        }
        catch (Exception ex) when (ex is ImageMagick.MagickException or InvalidDataException or IOException or UnauthorizedAccessException)
        {
            logger.LogWarning(ex, "Could not create image variant for {Path}", path);
            await next(context);
            return;
        }
        context.Response.Headers.CacheControl = "public,max-age=86400";
        await Results.File(cached, "image/webp", lastModified: source.LastModified,
            entityTag: new EntityTagHeaderValue('"' + key + '"')).ExecuteAsync(context);
    }
}
