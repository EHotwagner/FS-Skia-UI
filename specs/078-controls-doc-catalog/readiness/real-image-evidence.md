# Real Image Evidence (078)

status=not-applicable
evidence-kind=none
requested-image-evidence=false

## Why not-applicable

This window-visibility record concerns proving a **desktop window** is visible.
Feature 078 opens no window, so no desktop-window screenshot is requested or
produced, and none is needed to complete the feature.

The feature **does** commit per-control preview PNGs under
`docs/img/controls/<id>.png`, but those are **render-only control previews**
(off-window raster output), not desktop-window visibility screenshots. Their
honesty — decodable, non-1×1 dimensions, non-trivial content via
`Testing.readPngArtifact`, with explicit unsupported notes for any control that
cannot be honestly rendered — is recorded in `controls-preview-evidence.md`. No
metadata-only or fabricated image is claimed as a desktop-visibility proof here.
