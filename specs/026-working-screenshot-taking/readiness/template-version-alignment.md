# Template Version Alignment

Status: aligned.

The template package version was bumped to `0.1.27-preview.1` with the generated
product package pins for:

- `FS.Skia.UI.SkiaViewer` 0.1.27-preview.1
- `FS.Skia.UI.Testing` 0.1.27-preview.1

Reason: generated products must restore the additive screenshot request/result
and validator contracts introduced by this feature. Reusing
`0.1.26-preview.1` allowed stale local/global packages to satisfy restore and
caused generated products to compile against the old screenshot surface.
