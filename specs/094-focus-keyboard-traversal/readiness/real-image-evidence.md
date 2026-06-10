# Real image evidence (094) — render-only logic proof, desktop visibility not claimed

This feature (E4 focus model + key routing) ships **no new visual surface** that requires a live
desktop window: the focus reducers are pure, the `routeFocusedKey` route-probe is offscreen, and the
focus-indicator + responds-proof are exercised through the production `Control.renderTree` path with
deterministic `Scene` equality. No live Vulkan window is opened, and **none is required**.

- **evidence-kind**: render-only-logic-proof (deterministic reducer + route-probe + `Control.renderTree`
  Scene equality) — NOT a desktop screenshot.
- **status**: deferred (no live-window evidence is in scope for this feature; the FR/SC proofs are
  window-free and deterministic).
- **artifact-decodable**: n/a — there is no committed PNG/screenshot artifact for this feature; the
  authoritative proof is structural `Scene` equality, not a decoded image (`[[fs-skia-evidence-mode]]`).
- **proves-scene-rendering**: true — the responds-proof renders BEFORE/AFTER frames through the real
  `Control.renderTree` production path and compares the resulting `Scene` (a key-driven focus change
  yields `Responsive`; identical frames yield `Inert`).
- **proves-desktop-visibility**: false — pixel-readback / Scene equality alone cannot prove desktop
  visibility, and this feature deliberately makes no desktop-visibility claim (no live window opened).

The interactive host that surfaces this behavior is `ControlsElmish.runInteractiveApp`, whose live
desktop window-visibility was established by the earlier interactive-host features (085/092) and is
unchanged here — E4 only adds the focus-first key routing path inside that already-proven host.
