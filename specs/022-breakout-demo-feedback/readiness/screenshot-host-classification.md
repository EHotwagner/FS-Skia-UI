# Screenshot Host Classification

Task: T011

Skills:

- `fs-skia-layout-evidence` loaded from `.agents/skills/fs-skia-layout-evidence/SKILL.md`
- `fs-skia-skiaviewer` loaded from `src/SkiaViewer/skill/SKILL.md`

Classification rules:

- `status=ok` is valid only when a bounded screenshot artifact exists, dimensions are reported, and the artifact path is included.
- `status=unsupported` is a real negative host fact when screenshot capture is unavailable, the desktop session cannot expose capture, or the viewer host lacks a screenshot capture path.
- Unsupported screenshot evidence must include `evidence-kind=screenshot`, `unsupported-host-reason`, and `fallback=deterministic-scene-evidence`.
- Unsupported screenshot evidence must not include `screenshot-path`, `width`, `height`, or wording that claims screenshot proof.
- Deterministic scene evidence may be a fallback, but it remains distinct from screenshot proof.

Benign warning rules:

- Known environment warnings are benign only when launch/render/layout/package facts still report success or explicit unsupported status.
- `LaunchFailure`, `RenderingFailure`, `LayoutFailure`, and `PackageFailure` diagnostics are never downgraded because a known benign marker is also present.
- Any warning attached to `status=unsupported` must preserve the unsupported reason and owner-actionable diagnostic.

Current implementation note:

- `Viewer.captureScreenshotEvidence` returns `ScreenshotUnsupported` with `fallback=deterministic-scene-evidence` when the current viewer host does not expose screenshot capture. This is recorded as unsupported host capability, not synthetic screenshot success.

