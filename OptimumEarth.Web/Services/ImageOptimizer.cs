using ImageMagick;

namespace OptimumEarth.Web.Services;

public static class ImageOptimizer
{
    public const int MaxDimension = 1920;
    public static readonly int[] Widths = [480, 960, 1920];

    public static byte[] Optimize(byte[] bytes, int width = MaxDimension)
    {
        using var probe = new MagickImageCollection();
        probe.Ping(bytes);
        if (probe.Count != 1)
            throw new InvalidDataException("Please upload a still image; animated images are not supported.");
        if ((long)probe[0].Width * probe[0].Height > 40_000_000)
            throw new InvalidDataException("Images must contain fewer than 40 million pixels.");
        using var frames = new MagickImageCollection(bytes);
        if (frames.Count != 1)
            throw new InvalidDataException("Please upload a still image; animated images are not supported.");
        var image = frames[0];
        image.AutoOrient();
        image.ColorSpace = ColorSpace.sRGB;
        image.Resize(new MagickGeometry((uint)width, MaxDimension) { Greater = true });
        image.Strip();
        image.Quality = 85;
        return image.ToByteArray(MagickFormat.WebP);
    }
}
