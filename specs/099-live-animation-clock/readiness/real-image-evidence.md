# Real-image evidence — applicability (feature 099, R4)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=true
proves-desktop-visibility=false

This feature is **not** a persistent graphical viewer feature (recorded as a visible decision in T003):
R4 wires animation into the **existing** `runInteractiveApp` host loop and adds **no** default-executable
/ persistent-launch entry point. The animation is automatic on the built-in retained host; there is no
new window, host-launch, or user-driven interactive surface introduced by this feature.

Therefore the window-visibility / desktop-screenshot obligations do not apply and no persistent-launch
artifact is produced:

- artifact-decodable=not-applicable — no image/screenshot is produced by this feature; there is nothing
  to decode.
- proves-scene-rendering=true — the rendered-output evidence is the **animates-vs-snaps frame
  sequence** captured through the deterministic `runInteractiveApp` seam (`RetainedRender.advance` +
  `RetainedRender.step` over `ControlRuntime.applyRuntimeVisualState`), recorded in
  `us1-animates-vs-snaps.md` (cross-reference). It is proven by **structural `Scene` equality** (the
  `SceneEvidence` render functions are deterministic capability-hash functions, not pixel encoders):
  ≥1 intermediate sampled appearance, structurally distinct from the target, precedes a frame
  byte-equal to the static snapped target.
- proves-desktop-visibility=false — pixel-readback / structural evidence alone cannot prove desktop
  visibility, and this feature makes no desktop-visibility claim. At-rest frames are **byte-identical**
  to the pre-R4 golden (no rendered-output change at rest); a no-seam build snaps and fails the
  intermediate-frame assertion.
