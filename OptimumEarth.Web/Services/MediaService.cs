using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;

namespace OptimumEarth.Web.Services;

public sealed record UploadResult(MediaAsset? Asset, string? Error);

/// <summary>Stores uploaded images, and works out where an image is used before it can be deleted.</summary>
public sealed class MediaService
{
    public const long MaxBytes = 5 * 1024 * 1024;

    private static readonly Dictionary<string, string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".webp"] = "image/webp",
        [".gif"] = "image/gif",
    };

    private readonly AppDbContext _db;
    private readonly string _uploadsPath;

    public MediaService(AppDbContext db, IConfiguration config, IWebHostEnvironment env)
    {
        _db = db;
        _uploadsPath = ResolveUploadsPath(config, env);
    }

    public static string ResolveUploadsPath(IConfiguration config, IWebHostEnvironment env)
    {
        var configured = config["Storage:UploadsPath"];
        return string.IsNullOrWhiteSpace(configured) ? Path.Combine(env.ContentRootPath, "uploads") : configured;
    }

    public async Task<UploadResult> SaveAsync(IFormFile file)
    {
        if (file.Length == 0)
        {
            return new(null, $"{file.FileName} is empty.");
        }

        if (file.Length > MaxBytes)
        {
            return new(null, $"{file.FileName} is larger than {MaxBytes / 1024 / 1024} MB.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedTypes.TryGetValue(extension, out var contentType))
        {
            return new(null, $"{file.FileName}: only JPG, PNG, WebP and GIF images can be uploaded.");
        }

        // The extension is only a claim; check the first bytes really are that image format.
        var header = new byte[12];
        await using (var probe = file.OpenReadStream())
        {
            var read = await probe.ReadAsync(header);
            if (read < 12 || !LooksLikeImage(header, extension))
            {
                return new(null, $"{file.FileName} is not a valid {extension.TrimStart('.').ToUpperInvariant()} image.");
            }
        }

        var folder = DateTime.UtcNow.ToString("yyyyMM");
        Directory.CreateDirectory(Path.Combine(_uploadsPath, folder));
        var stored = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var physical = Path.Combine(_uploadsPath, folder, stored);
        await using (var target = File.Create(physical))
        {
            await file.CopyToAsync(target);
        }

        var asset = new MediaAsset
        {
            FileName = Path.GetFileName(file.FileName),
            Path = $"/uploads/{folder}/{stored}",
            SizeBytes = file.Length,
            ContentType = contentType,
        };
        _db.Media.Add(asset);
        await _db.SaveChangesAsync();
        return new(asset, null);
    }

    private static bool LooksLikeImage(byte[] h, string extension) => extension.ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF,
        ".png" => h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47,
        ".gif" => h[0] == 'G' && h[1] == 'I' && h[2] == 'F' && h[3] == '8',
        ".webp" => h[0] == 'R' && h[1] == 'I' && h[2] == 'F' && h[3] == 'F' && h[8] == 'W' && h[9] == 'E' && h[10] == 'B' && h[11] == 'P',
        _ => false,
    };

    /// <summary>Everywhere the image at this path is referenced, as "Section: item" labels.</summary>
    public async Task<List<string>> UsedByAsync(string path)
    {
        var used = new List<string>();
        used.AddRange((await _db.Slides.Where(x => x.ImagePath == path).Select(x => x.Headline).ToListAsync()).Select(x => "Hero slide: " + x));
        used.AddRange((await _db.Projects.Where(x => x.ImagePath == path).Select(x => x.Title).ToListAsync()).Select(x => "Project: " + x));
        used.AddRange((await _db.Services.Where(x => x.ImagePath == path).Select(x => x.Title).ToListAsync()).Select(x => "Service: " + x));
        used.AddRange((await _db.Stories.Where(x => x.CoverPath == path).Select(x => x.Title).ToListAsync()).Select(x => "Story: " + x));
        used.AddRange((await _db.GalleryImages.Where(x => x.ImagePath == path).Select(x => x.Caption).ToListAsync()).Select(x => "Gallery: " + x));
        used.AddRange((await _db.BlogPosts.Where(x => x.CoverPath == path).Select(x => x.Title).ToListAsync()).Select(x => "Blog post: " + x));
        return used;
    }

    public async Task<string?> DeleteAsync(MediaAsset asset)
    {
        if (asset.BuiltIn)
        {
            return "This image ships with the site and cannot be deleted here.";
        }

        var used = await UsedByAsync(asset.Path);
        if (used.Count > 0)
        {
            return $"In use by {used.Count} item{(used.Count == 1 ? "" : "s")}. Replace it there first.";
        }

        var relative = asset.Path["/uploads/".Length..].Replace('/', Path.DirectorySeparatorChar);
        var physical = Path.GetFullPath(Path.Combine(_uploadsPath, relative));
        if (physical.StartsWith(Path.GetFullPath(_uploadsPath), StringComparison.Ordinal) && File.Exists(physical))
        {
            File.Delete(physical);
        }

        _db.Media.Remove(asset);
        await _db.SaveChangesAsync();
        return null;
    }
}
