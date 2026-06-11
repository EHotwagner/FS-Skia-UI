# Real-image evidence — applicability (feature 100, R5)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=true
proves-desktop-visibility=false

This feature is **not** a persistent graphical viewer feature (recorded as a visible decision in T003):
R5 wires general navigation into the **existing** `runInteractiveApp` host loop and adds **no**
default-executable / persistent-launch entry point. Navigation is automatic on the built-in retained
host; there is no new window, host-launch, or user-driven interactive surface introduced by this
feature. At-rest rendered output is **unchanged** (navigation produces a `'msg`; there is no layout/
render algorithm change).

Therefore the window-visibility / desktop-screenshot obligations do not apply and no persistent-launch
artifact is produced:

- artifact-decodable=not-applicable — no image/screenshot is produced by this feature; there is nothing
  to decode.
- proves-scene-rendering=true — the rendered-output evidence is the **responds-vs-renders** capture
  through the real `runInteractiveApp` seam (`RetainedRender.init`/`step` + `routeFocusedKey`),
  recorded in [responds-vs-renders.md](./responds-vs-renders.md) (cross-reference): a focused
  radio-group/tab arrow press moves selection and dispatches its binding with the moved item, while a
  pre-R5 build dispatches nothing. It is proven by **dispatch observation** through the deterministic
  retained seam, not pixel encoding.
- proves-desktop-visibility=false — dispatch / structural evidence alone cannot prove desktop
  visibility, and this feature makes no desktop-visibility claim. At-rest frames are byte-identical to
  the pre-R5 output (no rendered-output change at rest); a pre-R5 build dispatches nothing on a focused
  radio-group arrow and fails the responds-vs-renders capture.
