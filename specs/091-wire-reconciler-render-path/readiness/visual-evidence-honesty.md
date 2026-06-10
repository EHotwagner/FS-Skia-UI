# Visual Evidence Honesty — Feature 091

Feature 091 wires the parked 067 keyed reconciler onto the live render path. Its correctness
proofs are **pure and structural**, and any attempted image capture is classified honestly:

- **No visual proof is claimed.** The golden-diff parity (SC-004) is a **structural equality** of
  the produced `ControlRenderResult.Scene` values (wired `RetainedRender.step` == full
  `Control.renderTree next`), a pure value comparison — not an image diff. The
  focus/animation-survives proof (SC-002) is the pure carry of the stable `RetainedId` through
  `RetainedRender.StateByIdentity`. The work-reduction metric (SC-003) is a node count.
- **Deterministic-render-only, not readable layout.** An optional `SceneEvidence.renderPng`
  capture was attempted, but `SceneEvidence.renderPng` / `Scene.renderReadbackEvidence` are
  deterministic **capability-hash** functions (a hash of size + the sorted scene-capability
  descriptors), **not pixel encoders** — so they emit a hash, never a decodable image. This is a
  property of the API, **not** of the hardware (the environment HAS a GPU and a live Vulkan/Skia
  window can open). A capability hash is `DeterministicRenderOnly` and is **never** presented as a
  decodable image or as readable-layout proof (`artifact-decodable=false`,
  `proves-scene-rendering=false`; see `real-image-evidence.md`). A real pixel PNG would require the
  windowed render-target path (`SkiaViewer.captureScreenshotEvidence`), out of scope here.
- **No fallback substitution.** No 1x1 fallback image, metadata-only report, or layout-only bounds
  claim is offered as visual proof.
- **No desktop visibility claimed.** Pixel-readback alone cannot prove desktop visibility, and no
  live window was launched (render-only posture; see `window-visibility.md`).

The authoritative evidence is the pure structural parity/survival/work-reduction proofs asserted by
`Feature091RetainedRenderTests` (181/181 tests pass), which need no rasterizer and no live window
([[fs-skia-evidence-mode]]).
