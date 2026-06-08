# Real Image Evidence (US1) — 079-doc-preview-examples

Authoritative command: `dotnet run --project tests/ControlsPreview.Harness -- --render`
(render-only path on a render-capable host) then visual inspection of the committed PNGs +
`docs/controls/catalog.html`.
Artifact path: this file + `docs/img/controls/<id>.png` + `controls-preview-evidence.md`.
Failure class: `not-real-render` / `near-empty-preview`.

evidence-kind=none

(No desktop-window image is claimed here. The previews are deterministic **render-only**
raster assets recorded in `controls-preview-evidence.md`, not window-visibility proofs.)

## What was rendered (FR-001/FR-003, SC-001)

All **51** demonstrative previews were regenerated on a render-capable host (Linux x64,
SkiaSharp `linux-x64` native asset) through the **real render-only evidence path**
(`Widget.toControl` → `Control.render Theme.light` → `SceneNode.Group` →
`SkiaViewer.captureScreenshotEvidence`, `CaptureMode = ViewerRenderTargetPng`,
`status = ScreenshotOk`) from the single sample source `PreviewSamples.fs`. The previous
078 previews were rendered with **bare defaults** — 12 of them were ~363-byte near-blank
canvases. Every regenerated preview now shows recognizable, control-specific content.

## Recognizable, control-specific content (verified)

Visual spot-checks of the committed renders confirm real content, not empty boxes:

- **button** → renders the label text **"SAVE"** (not an empty button).
- **text-block** → renders **"Status: all systems nominal"**.
- **slider** → renders its mid-track value **"0.5"**.
- **check-box** → checked, with the label "Enable notifications".
- **data-grid** → columns (Name / Qty) and rows (Widget 12 / Gadget 7) with a selected row.
- **radio-group / tabs / list-box** → a chosen option / active page / highlighted row.
- **charts (line/bar/pie/scatter)** → a plotted sample series; **graph-view** → nodes.
- **date-picker / time-picker** → the fixed sample date / time segments.

## 0 controls near-empty (SC-001)

The smallest committed demonstrative preview is **486 bytes** (icon-button); every preview
exceeds the pinned **420-byte** trivial-content floor (the near-empty 320×160 baseline is
~363 bytes). Per-control bytes/dimensions are in `controls-preview-evidence.md`:
**rendered = 51, unsupported = 1 (custom-control), 51 + 1 == 52** — no silent omission.

## Honesty

`custom-control` is the one honest **Unsupported** declaration (no image; detail page
carries `preview-status: unsupported`) — see `visual-evidence-honesty.md`. No `[S]`:
every demonstrative control rendered `ScreenshotOk`. Determinism/idempotence (byte-identical
re-renders, committed == fresh render) is proven by the `ControlsPreview.Harness` tests
(7 passed).
