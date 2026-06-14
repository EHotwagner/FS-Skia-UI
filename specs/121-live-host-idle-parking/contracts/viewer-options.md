# Contract: `ViewerOptions` frame-cap

**Package**: `FS.Skia.UI.SkiaViewer` · **File**: `src/SkiaViewer/SkiaViewer.fsi`

## Before

```fsharp
type ViewerOptions =
    { Title: string
      InitialSize: Size
      PresentMode: ViewerPresentMode }
```

## After (additive, defaulted)

```fsharp
type ViewerOptions =
    { Title: string
      InitialSize: Size
      PresentMode: ViewerPresentMode
      /// Optional consumer frame-rate cap for the live persistent loop. `None`
      /// keeps the default (60 FPS) — the exact pre-feature-121 behaviour. `Some n`
      /// (n > 0) bounds BOTH the update and the render cadence of the native loop;
      /// `Some n` with n <= 0 is rejected at validation. Ignored by the offscreen /
      /// evidence (`runBounded`) path, which does not use the persistent event loop.
      FrameRateCap: int option }
```

### Construction / back-compat

- A defaulting path keeps existing call sites compiling:
  ```fsharp
  /// Options with FrameRateCap = None (default 60).
  val create: title: string -> initialSize: Size -> presentMode: ViewerPresentMode -> ViewerOptions
  /// Copy with an explicit cap.
  val withFrameRateCap: cap: int -> options: ViewerOptions -> ViewerOptions
  ```
  (If `create`/`withFrameRateCap` are judged surplus during implementation, the record
  may instead be constructed directly — but every construction site, including samples and
  `scripts/*-prelude.fsx`, must then add `FrameRateCap = None`; `RefreshSurfaceBaselines`
  Build + `FsiTranscripts` catch any miss.)

### Behaviour

| `FrameRateCap` | Native `TargetFrameRate` | Update cadence | Render cadence |
|----------------|--------------------------|----------------|----------------|
| `None` | `Some 60` | gated by 60 (today) | **gated by 60 (new — was every poll)** |
| `Some n`, n > 0 | `Some n` | gated by n | gated by n |
| `Some n`, n ≤ 0 | — | startup `Result.Error` | startup `Result.Error` |

### Validation

`validateOptions` (`SkiaViewer.fs` ~826) adds: `FrameRateCap = Some n && n <= 0` ⇒
`Result.Error (makeFailure Window ProductDefect Startup "Viewer frame-rate cap must be positive." None)`.

### Compatibility note

Public record shape changes (one added field). It is additive and defaulted, so at-rest
output is byte-identical and no existing consumer behaviour changes unless they opt in.
Surface baselines regenerate via `./fake.sh build -t RefreshSurfaceBaselines`.
</content>
