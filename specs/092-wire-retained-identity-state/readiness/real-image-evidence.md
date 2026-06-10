# Real image evidence — feature 092

evidence-kind=deterministic-render-only
status=deferred
artifact-decodable=false
proves-scene-rendering=false
proves-desktop-visibility=false

## Honest classification (render-only; no decodable image in this environment)

Feature 092's correctness proofs are **pure and structural / identity-based**, not image-based, so
they do not depend on a rasterizer at all:

- **Live survival (SC-001):** the focused control's stable `RetainedId`-keyed state is carried
  through `RetainedRender.StateByIdentity` across a positional shift, driven through the REAL seam
  (`resolveFocus` + `routeFocusedText` + `RetainedRender.step`) with no hand-seeded focus/text — the
  draft text continues (`hi`→`hix`→shift→`hixy`) and the clock advances rather than resets; a
  rebuild-every-frame baseline fails the same proof (`live-survival/survival.txt`,
  `live-survival/baseline-fails.txt`).
- **Focus resolution (SC-002):** keyed / unkeyed / keyed-container-wrapped fields each resolve to a
  distinct `RetainedId`; a pre-filled multi-line first keystroke appends
  (`focus-resolution/focus-resolution.txt`, `focus-resolution/prefilled-append.txt`).
- **Theme reuse (SC-006) + multi-frame parity (SC-004):** the wired `step(...).Render.Scene` is
  **structurally equal** to `Control.renderTree theme size next` (zero diff), including across a
  theme change and a chained 3+-frame sequence (`theme-reuse/theme-reuse.txt`,
  `multi-frame/round-trip.txt`).
- **Work reduction (SC-003):** a measured node count satisfying
  `RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount < BaselineNodeCount`
  (`work-reduction/work-reduction.txt`).

An optional decodable PNG capture is **not** produced: `SceneEvidence.renderPng` (and
`Scene.renderReadbackEvidence`) is a deterministic **capability-hash** function — it hashes the
output size plus the sorted scene-capability descriptors and returns that hash **text as bytes**; it
does **not** rasterize to pixels. So it yields a hash, never a decodable image — a property of the
**API**, not of the hardware. The environment **HAS** a GPU (a live Vulkan/Skia window can open via
the windowed path), so this is **not** a "no GPU" limitation. Therefore `artifact-decodable=false`
and `proves-scene-rendering=false` — a capability hash is not a decodable image and is not presented
as one ([[fs-skia-evidence-mode]]). Pixel-readback alone cannot prove desktop visibility
(`proves-desktop-visibility=false`). A real pixel PNG would require the windowed render-target path
(`SkiaViewer.captureScreenshotEvidence` `ViewerRenderTargetPng`), out of scope for this
internal-state-wiring feature. The authoritative evidence is the pure proofs above.
