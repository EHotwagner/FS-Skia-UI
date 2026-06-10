# Real image evidence — feature 091

evidence-kind=deterministic-render-only
status=deferred
artifact-decodable=false
proves-scene-rendering=false
proves-desktop-visibility=false

## Honest classification (render-only; no decodable image in this environment)

Feature 091's correctness proofs are **pure and structural**, not image-based, so they do not
depend on a rasterizer at all:

- **Golden-diff parity (SC-004):** the wired `RetainedRender.step(...).Render.Scene` is compared
  for **structural equality** against `Control.renderTree theme size next` — a pure value
  comparison that is byte-identical (zero diff), asserted by `Feature091RetainedRenderTests`
  (`readiness/retained-parity/retained-parity.txt`). `parity-proof=structural-scene-equality`.
- **Focus/animation survives (SC-002):** the stable `RetainedId` is carried through
  `RetainedRender.StateByIdentity` across an unrelated re-render (pure), with a rebuild-every-frame
  baseline failing the same proof (`readiness/survives-proof/survives-proof.txt`).
- **Work reduction (SC-003):** a measured node count (`readiness/partial-update/work-reduction.txt`).

An optional decodable PNG capture was **attempted** via `SceneEvidence.renderPng`, but that API
(and `Scene.renderReadbackEvidence`) is a deterministic **capability-hash** function — it hashes
the output size plus the sorted scene-capability descriptors and, for the `Png` format, returns
that hash **text as bytes**; it does **not** rasterize to pixels. So it yields a hash, never a
decodable image — a property of the **API**, not of the hardware. The environment **HAS** a GPU
(a live Vulkan/Skia window can open via the windowed path), so this is **not** a "no GPU"
limitation. Therefore `artifact-decodable=false` and `proves-scene-rendering=false` — a capability
hash is **not** a decodable image and is **not** presented as one ([[fs-skia-evidence-mode]]:
metadata/hash-only reports do not satisfy visual proof; 1x1 fallback images do not satisfy visual
proof). Pixel-readback alone cannot prove desktop visibility (`proves-desktop-visibility=false`). A
real pixel PNG would require the windowed render-target path (`SkiaViewer.captureScreenshotEvidence`
`ViewerRenderTargetPng`), which is out of scope for this internal render-path feature. The
authoritative evidence is the pure structural parity/survival/work-reduction proofs above, which
need no rasterizer and no live window.
