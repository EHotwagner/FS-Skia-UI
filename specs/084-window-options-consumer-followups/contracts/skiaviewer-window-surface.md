# Contract: `FS.Skia.UI.SkiaViewer` public surface delta

Tier 1 public-surface change to `src/SkiaViewer/SkiaViewer.fsi`. Additive case +
default value change; no signature changes. Surface baselines (cross-package +
per-package `FS.Skia.UI.SkiaViewer`) move and MUST be recaptured.

## Before

```fsharp
[<RequireQualifiedAccess>]
type ViewerWindowStartupState =
    | Normal
    | Maximized
    | Minimized
    | Fullscreen
```

`defaultWindowBehavior.StartupState = ViewerWindowStartupState.Normal`
`validateWindowBehavior`: `Fullscreen` → `UnsupportedOption`
("Fullscreen startup is not yet supported by the viewer host.")

## After

```fsharp
[<RequireQualifiedAccess>]
type ViewerWindowStartupState =
    | Normal
    | Maximized
    | Minimized
    | Fullscreen
    | WindowedFullscreen        // NEW: borderless coverage of the monitor work area
```

- `defaultWindowBehavior.StartupState = ViewerWindowStartupState.WindowedFullscreen`
- `validateWindowBehavior` / `validateWindowLaunchBehavior`:
  - `Fullscreen` → `Honored`
  - `WindowedFullscreen` → `Honored`
  - `Minimized` → `UnsupportedOption` (unchanged)
- `applyWindowBehaviorToOptions`:
  - `Fullscreen` → `WindowState.Fullscreen` (unchanged)
  - `WindowedFullscreen` → `WindowBorder.Hidden` + monitor work-area `Position`/`Size`
    + `WindowState.Normal`

## Signatures unchanged

`runAppWithWindowBehavior`, `runApp`, `validateWindowBehavior`,
`validateWindowLaunchBehavior`, `ViewerWindowBehaviorRequest`,
`ViewerWindowOptionResult`, `ViewerWindowOptionStatus`, `ApplyWindowOptions`
effect — all signatures stay identical. Only the union grows and one default value
changes.

## Compatibility

Additive union case. Existing consumers that pattern-match
`ViewerWindowStartupState` exhaustively get a (desirable) incomplete-match warning
prompting them to handle `WindowedFullscreen`. The default-value change alters the
no-flag launch behavior (windowed fullscreen instead of normal) — documented in the
spec and template docs; explicit selection (FR-006) overrides it.
