# Contract: Viewer Internal Boundary

## Public Facade

`src/SkiaViewer/SkiaViewer.fsi` and the public viewer facade remain unchanged.
Internal files may be added before `SkiaViewer.fs` in
`src/SkiaViewer/SkiaViewer.fsproj`.

## Internal Responsibilities

The following implementation responsibilities may move behind the facade:

- legacy scene conversion,
- diagnostics filtering and dispatch,
- desktop session and host capability detection,
- window behavior validation,
- visual evidence artifact generation,
- screenshot evidence result handling,
- generated app host interpretation.

## Stable Behavior

The cleanup must preserve observable viewer behavior, including diagnostics,
window option validation, desktop/unsupported-host classification, visual
evidence artifacts, screenshot evidence reports, and existing failure messages.

## Validation

Run targeted `SkiaViewer.Tests` and governance checks for supported and
unsupported host classifications. Public surface baselines must remain
unchanged.
