# Visual Evidence Honesty — Feature 092

Feature 092 wires the retained identity into live interactive state. Its correctness proofs are
**pure and structural / identity-based**, and any attempted image capture is classified honestly:

- **No visual proof is claimed.** The live-survival proof (SC-001) is the carry of the focused
  control's stable `RetainedId`-keyed state (`RetainedRender.StateByIdentity[id].Text`/`.Animation`)
  across a positional shift, driven through the REAL `ControlsElmish.resolveFocus` +
  `routeFocusedText` + `RetainedRender.step` seam — an identity/value comparison, not an image diff.
  Focus-resolution (SC-002) is a per-node box hit-test returning distinct `RetainedId`s. Theme reuse
  (SC-006) and multi-frame parity (SC-004) are **structural equality** of the produced
  `ControlRenderResult.Scene` values (wired `step` == full `Control.renderTree`). Work reduction
  (SC-003) is a node count.
- **Deterministic-render-only, not readable layout.** `SceneEvidence.renderPng` /
  `Scene.renderReadbackEvidence` are deterministic **capability-hash** functions, **not** pixel
  encoders — they emit a hash, never a decodable image. This is a property of the API, **not** of
  the hardware (the environment HAS a GPU and a live Vulkan/Skia window can open). A capability hash
  is `DeterministicRenderOnly` and is **never** presented as a decodable image or readable-layout
  proof (`artifact-decodable=false`, `proves-scene-rendering=false`; see `real-image-evidence.md`).
- **No fallback substitution.** No 1x1 fallback image, metadata-only report, or layout-only bounds
  claim is offered as visual proof.
- **No desktop visibility claimed.** No live window was launched (render-only posture; see
  `window-visibility.md`).

The authoritative evidence is the pure survival/focus-resolution/parity/work-reduction proofs
asserted by `Feature092RetainedRenderTests` (Controls) and `Feature092LiveSurvivalTests` (Elmish),
which need no rasterizer and no live window ([[fs-skia-evidence-mode]]).
