# Settled ≡ static parity — Add Animations (073)

Story 4 (FR-006/SC-004, FR-007/SC-005): animation is opt-in and never degrades static
authoring.

## Settled ≡ static (identity-at-rest, FR-006/SC-004)

`Animation.applyAt` applies the **identity-at-rest rule (R5)**: when the sampled
opacity is `1.0` and the sampled transform is identity, it returns the target scene's
node **unwrapped** — structurally (and therefore byte-) identical to the static render
of the same widget. Proven two ways in `tests/Parity.Tests/AnimationOutputTests.fs`:

- **Structural**: `Animation.applyAt 300ms entrance panel` is `=` the panel's single
  static node ("settled entrance returns the target node UNWRAPPED").
- **Render**: the settled frame's `SceneEvidence.render` `deterministic-scene` hash
  equals the static panel's hash ("settled frame's deterministic-scene hash equals the
  static render's hash").

`Tween.sample` pins the exact `End` value at `progress = 1.0`, so the settled transform
is exactly `Transform.identity` and the settled opacity is exactly `1.0` — the rest
detection is exact, not floating-point approximate.

## Un-animated unchanged (FR-007/SC-005)

`Animation.applyAt _ Animation.empty panel` returns the static node at **every** time
sample (no animated property present), and its `deterministic-scene` hash equals the
static panel's hash ("Animation.empty renders byte-identically to the static scene").
No new required parameter is introduced: a widget that declares no animation is
authored and rendered exactly as before — `Animation.empty` is a convenience, not a
mandatory wrapper.

## US4 independent validation path

1. Render a representative existing view with **no** animation declaration (or with
   `Animation.empty`).
2. Compare against the static deterministic-scene evidence for that scene → byte
   parity.
3. Confirm no animation-related parameter is required to author the static view.
