Images uploaded through the media dashboard are validated, oriented correctly,
resized without upscaling to a maximum of 1,920 pixels on either edge, and saved
as WebP at quality 85. Transparency is retained and metadata is removed. The
existing 5 MB upload limit remains; still JPG, PNG, WebP and GIF images are
accepted. Animated images and images above 40 million pixels are rejected.

Public Razor pages automatically get responsive image sources. Background
images load near the viewport; later carousel slides load when selected.
Use `image-priority="high"` for a visible hero, and `sizes` on image elements
when the rendered width is smaller than the viewport. Use
`image-priority="deferred"` only for carousel backgrounds loaded by its script.
External images and SVGs are left alone.

Requests for local images with `w=480`, `w=960`, or `w=1920` generate WebP
variants once, then reuse the disk cache at `Storage:UploadsPath/.image-cache`.
This requires write access to the existing upload directory. Only these three
sizes are processed. Source timestamp and size form part of the cache key;
rendered URLs include the timestamp so replacing a file refreshes it.
Browser caching lasts one day and supports conditional requests. If processing
fails, the original image is served and a warning is logged.

Old variant files can be removed from `.image-cache` during routine maintenance;
they regenerate when requested. Deployment must include the Magick.NET native
runtime assets from normal `dotnet publish` output. No separate image tool is
needed on the host.

Run checks with `dotnet run --project tests/ImageChecks`.
The Windows-only `scripts/Optimize-StoredImages.ps1` is a separate manual tool
for existing JPEGs and defaults to a dry run.
