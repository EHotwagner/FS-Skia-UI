# Visual Evidence Honesty — 079-doc-preview-examples

Authoritative command: `./fake.sh build -t ControlsCatalogDocsCheck` (committed-byte
structural validation) + the render-harness on a render-capable host.
Artifact path: this file + `controls-preview-evidence.md` + `real-image-evidence.md`.
Failure class: `visual-proof-dishonesty` / `trivial-preview`.
Next action: keep every preview a real render-only capture or an honest unsupported
declaration — never a metadata-only, 1x1, or placeholder image.

## Proof-level classification (per fs-skia-evidence-mode)

Every committed preview is **`DeterministicRenderOnly`** evidence: a real
render-only raster of the control's sample state through
`Widget.toControl` → `Control.render Theme.light` → `SceneNode.Group` →
`SkiaViewer.captureScreenshotEvidence` with `CaptureMode = ViewerRenderTargetPng`,
`status = ScreenshotOk`. This is **not** `ReadableLayout` proof and is not reported
as readability — it proves the control rendered recognizable, control-specific
content, not HUD/gameplay readability.

Accepted visual proof names: a **decodable image** (PNG signature + IHDR),
**image dimensions** (320×160, non-1×1), **non-trivial content** (committed bytes
≥ the pinned trivial-content floor `T`), **renderer mode**
(`render-only / ViewerRenderTargetPng`), and the **content classification**
(`demonstrative` | `unsupported`).

Rejection phrases honored: *metadata-only reports do not satisfy visual proof;
1x1 fallback images do not satisfy visual proof; layout-only bounds claims do not
satisfy visual proof.*

## Honest `Unsupported` vs real `RenderingFailure` (benign/blocking)

The harness and gate distinguish two non-rendered outcomes:

1. **Honest `Unsupported` declaration** (benign, PASS): a control with
   `Kind = Unsupported` in the single sample source commits **no** PNG and its
   detail page carries the literal marker `preview-status: unsupported`. The gate's
   `MissingPreview` finding is suppressed only by that marker, so an honest omission
   is distinguishable from an accidental one. This is a real, actionable honesty
   statement — never a fabricated/1×1/placeholder image.
   - In this feature exactly **one** control is `Unsupported`: `custom-control`
     ("Product-owned wrapper for custom Skia content" — there is no canonical
     sample content to depict render-only; what it shows is entirely
     product-defined). It is an honest declaration, not a `[S]`.

2. **Real `RenderingFailure`** (blocking, FAIL): if the harness's
   `captureScreenshotEvidence` returns a non-`ScreenshotOk` status for a
   `Demonstrative` entry, that is a genuine rendering failure and is preserved as a
   real diagnostic — never silently downgraded to `Unsupported`. The harness fails
   loudly with the control id and the returned status; benign known environment
   warnings do not mask it.

A genuinely non-renderable `Demonstrative` control that cannot be honestly produced
this iteration would be a disclosed `[S]` follow-up with its evidence row — **none
is present**: all 51 `Demonstrative` controls render `ScreenshotOk` (see
`real-image-evidence.md` and `controls-preview-evidence.md`).
