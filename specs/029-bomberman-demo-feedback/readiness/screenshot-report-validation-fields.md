# Screenshot Report Validation Fields

Task: T008
Captured: 2026-05-29T11:48:32+02:00

## Required Fields

Every screenshot report must include:

- `status`
- `command`
- `output`
- `mode`
- `evidence-kind`
- `app-or-sample`
- `host-facts`
- `capture-mode`
- `viewer-open-status`
- `first-frame-status`
- `capture-availability`
- `capture-source`
- `proves-screenshot`
- `message`
- `timestamp`
- `diagnostics`

`ok` reports also require readiness-local `artifact-path` and `screenshot-path`, dimensions, and `pixel-content-validation=PixelContentNonBlank`.

`unsupported` reports also require `unsupported-host-reason`, `fallback`, `blocked-stage`, `classification`, and `category`.

## Failure Vocabulary

- `SuccessfulCapture`: real artifact exists, decodes, is readiness-local, and is nonblank.
- `UnsupportedHostCapability`: real capture path was probed or could not be attempted for documented host facts.
- `AppCommandImplementationError`: command failed due to generated app/build/reporting defects and must not be labeled unsupported.
- `InvalidReport`: malformed or incomplete report fields.
