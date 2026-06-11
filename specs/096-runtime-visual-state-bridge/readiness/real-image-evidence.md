# Real-image evidence — applicability (feature 096)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=true
proves-desktop-visibility=false

This feature is **not** a persistent graphical viewer feature (recorded as a visible decision in T003):
it adds a pure projection (`deriveVisualState`), a pure internal bridge (`applyRuntimeVisualState`), a
host call site, and four widened geometry functions. There is no new window, host-launch, or
user-driven interactive surface — the behavior is automatic on the existing built-in retained host.

Therefore the window-visibility / desktop-screenshot obligations do not apply and no persistent-launch
artifact is produced:

- artifact-decodable=not-applicable — no image/screenshot is produced by this feature; there is nothing
  to decode.
- proves-scene-rendering=true — rendering correctness is proven by **structural `Scene` equality** and
  **resolved-style** equality (the `SceneEvidence` render functions are deterministic capability-hash
  functions, not pixel encoders), captured in `byte-identity-at-rest.md`, `live-restyle.md`,
  `responds-proof.md`, and `widened-kinds.md`.
- proves-desktop-visibility=false — pixel-readback / structural evidence alone cannot prove desktop
  visibility, and this feature makes no desktop-visibility claim. The responds-proof is the **live
  retained render-step path** (input → `Update` patch → restyle) that an inert/un-bridged build fails,
  not a screenshot of a desktop window.
