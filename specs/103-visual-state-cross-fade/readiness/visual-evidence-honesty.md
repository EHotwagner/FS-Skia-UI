# Visual-evidence honesty — feature 103 (R6, T002)

evidence-kind=visual-evidence-honesty
renderer-mode=DeterministicRenderOnly
status=pass

## What the R6 evidence proves — and what it does not

R6's proofs are **structural Scene assertions**, not pixel screenshots. The framework's
`SceneEvidence`/render-readback hashes are deterministic CAPABILITY hashes, not pixel encoders, so the
authoritative cross-fade evidence is the **scene description** the production assemble path
(`RetainedRender.step` → `sampleOnPaint`) emits — exercised through the real `runInteractiveApp` seam
(`ControlRuntime.applyRuntimeVisualState` + `RetainedRender.advance` + `RetainedRender.step`).

- **Honest claim** — mid-flight the composited own-scene contains BOTH endpoint colours (the prior
  state's colour fading out under the next fading in), each at partial alpha, and the source-over
  displayed colour is strictly between the endpoints (SC-001). This is a structural fact about the
  scene the production path paints, read directly from the `SceneNode` colours.
- **NOT claimed** — no desktop-visibility / windowed-screenshot proof is asserted (R6 needs none; it
  is GPU-free deterministic assembly). Pixel-readback alone cannot prove desktop visibility, and R6
  makes no such claim. See [window-visibility.md](./window-visibility.md) / [real-image-evidence.md](./real-image-evidence.md).
- **Production render path** — the evidence drives the surface the user actually reaches: the retained
  assemble walk's per-identity overlay over the cached static own-scene, NOT a hand-built parallel
  scene. The representative control is an off `Switch`, whose track FILL genuinely restyles
  Muted→Accent on Hover via `Style.resolve` — a region painted in both states.

## Benign vs blocking

No host warnings are produced (no window, no GPU). Any local `GeneratedProductCheck` environment
failure is recorded as **non-authoritative** environment-class, not a product defect (see
[aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md) / [generated-validation.md](./generated-validation.md)).
