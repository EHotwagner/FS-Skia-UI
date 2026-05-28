# Compatibility Impact

Status: reviewed.

This is an additive public-contract change for screenshot evidence. Existing
launch, bounded smoke, deterministic scene, layout, image evidence, and
pixel-readback workflows remain available and are not reclassified as screenshot
proof.

Generated product pins were updated to consume:

- `FS.Skia.UI.SkiaViewer` 0.1.27-preview.1
- `FS.Skia.UI.Testing` 0.1.27-preview.1
- `FS.Skia.UI.Template` 0.1.27-preview.1

No package identity was renamed or removed.
