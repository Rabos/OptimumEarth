param(
    [string]$ImageRoot = "$PSScriptRoot/../OptimumEarth.Web/wwwroot/img",
    [int]$MaxDimension = 1920,
    [int]$Quality = 85,
    [long]$MinimumBytes = 512000,
    [switch]$Apply
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
if ($MaxDimension -lt 1 -or $Quality -lt 1 -or $Quality -gt 100) {
    throw 'MaxDimension must be positive and Quality must be between 1 and 100.'
}
$root = (Resolve-Path -LiteralPath $ImageRoot).Path
$backupRoot = Join-Path $PSScriptRoot "../artifacts/image-backups/$(Get-Date -Format 'yyyyMMdd-HHmmss-fff')"
$encoder = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | Where-Object MimeType -eq 'image/jpeg'
$results = @()
foreach ($file in (Get-ChildItem -LiteralPath $root -Recurse -File | Where-Object { $_.Extension -in '.jpg', '.jpeg' -and $_.Length -ge $MinimumBytes })) {
    $source = $bitmap = $graphics = $parameters = $stream = $null
    try {
        $source = [System.Drawing.Image]::FromFile($file.FullName)
        # Normalize camera orientation before stripping metadata.
        if ($source.PropertyIdList -contains 274) {
            $orientation = [BitConverter]::ToUInt16($source.GetPropertyItem(274).Value, 0)
            $rotation = @{ 2 = 'RotateNoneFlipX'; 3 = 'Rotate180FlipNone'; 4 = 'Rotate180FlipX'; 5 = 'Rotate90FlipX'; 6 = 'Rotate90FlipNone'; 7 = 'Rotate270FlipX'; 8 = 'Rotate270FlipNone' }
            if ($rotation.ContainsKey([int]$orientation)) {
                $source.RotateFlip([System.Drawing.RotateFlipType]::$($rotation[[int]$orientation]))
            }
        }
        $scale = [Math]::Min([double]1, [double]$MaxDimension / [double][Math]::Max([int]$source.Width, [int]$source.Height))
        $width = [Math]::Max(1, [int][Math]::Round($source.Width * $scale))
        $height = [Math]::Max(1, [int][Math]::Round($source.Height * $scale))
        if ($width -gt $MaxDimension -or $height -gt $MaxDimension -or ($source.Width -gt 100 -and $width -lt 100)) {
            throw "Unexpected resize dimensions for $($file.Name): ${width}x${height}"
        }
        $bitmap = [System.Drawing.Bitmap]::new($width, $height)
        $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
        $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $graphics.DrawImage($source, 0, 0, $width, $height)
        $parameters = [System.Drawing.Imaging.EncoderParameters]::new(1)
        $parameters.Param[0] = [System.Drawing.Imaging.EncoderParameter]::new([System.Drawing.Imaging.Encoder]::Quality, [long]$Quality)
        $stream = [System.IO.MemoryStream]::new()
        $bitmap.Save($stream, $encoder, $parameters)
        $bytes = $stream.ToArray()
        $source.Dispose()
        $source = $null
        if ($bytes.Length -ge $file.Length) { continue }
        $relative = [System.IO.Path]::GetRelativePath($root, $file.FullName)
        $results += [pscustomobject]@{ Path = $relative; BeforeBytes = $file.Length; AfterBytes = $bytes.Length; Width = $width; Height = $height }
        if ($Apply) {
            $backup = Join-Path $backupRoot $relative
            New-Item -ItemType Directory -Path (Split-Path $backup) -Force | Out-Null
            Copy-Item -LiteralPath $file.FullName -Destination $backup
            [System.IO.File]::WriteAllBytes($file.FullName, $bytes)
            $check = [System.Drawing.Image]::FromFile($file.FullName)
            try {
                if ($check.Width -ne $width -or $check.Height -ne $height) { throw "Image verification failed: $relative" }
            } finally { $check.Dispose() }
        }
    } finally {
        foreach ($resource in @($source, $graphics, $bitmap, $parameters, $stream)) {
            if ($null -ne $resource) { $resource.Dispose() }
        }
    }
}
$results | Format-Table -AutoSize
if ($results.Count) {
    $before = ($results | Measure-Object BeforeBytes -Sum).Sum
    $after = ($results | Measure-Object AfterBytes -Sum).Sum
    Write-Output ("{0} images: {1:N2} MB -> {2:N2} MB ({3:N1}% reduction)" -f $results.Count, ($before/1MB), ($after/1MB), (100*(1-$after/$before)))
    if ($Apply) {
        $results | ConvertTo-Json | Set-Content (Join-Path $backupRoot 'report.json')
        Write-Output "Originals and report: $backupRoot"
    } else { Write-Output 'Dry run only. Pass -Apply to save optimized images.' }
}
