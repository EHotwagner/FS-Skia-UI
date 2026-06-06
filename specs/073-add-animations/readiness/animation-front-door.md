# Animation front door — Add Animations (073)

**Feature tier**: Tier 1 (contracted), additive-only.
**Affected layers**: `FS.Skia.UI.Scene` core (new `Animation` module) + additive
`FS.Skia.UI.Elmish` tick helper (`AnimationTick` + `Animation.tickSubscription`).
**Public-API impact**: additive new `.fsi` declarations only; no existing Scene/Elmish
signature changes shape.

## The bounded property / easing slice

An author declares, as data against an existing `Scene`, that a bounded set of visual
properties travels from a start to a target value over a duration shaped by a named
easing curve:

- **Properties**: opacity (`Tween<float>`), affine transform
  (`Tween<Transform>` — translate / scale / rotate), color (`Tween<Color>`).
- **Easing curves**: `Linear`, `EaseIn`, `EaseOut`, `EaseInOut`. The documented
  default for an unspecified curve (FR-003) is `Easing.Default = EaseInOut`.
- `Tween.Easing` / `Tween.Duration` are **mandatory record fields** — the spec
  Assumptions' "short default duration" (e.g. 300 ms) is quickstart guidance, not an
  omitted-field API default.

## Design

- `Animation.applyAt : elapsed -> Animation -> Scene -> SceneNode` is a **pure**
  sampling function of an explicit `TimeSpan`. Same inputs + same time sample ⇒
  byte-identical output (FR-004/FR-009/SC-003).
- **Identity-at-rest (R5):** when the sampled opacity is `1.0` and the sampled
  transform is identity, `applyAt` returns the target's node **unwrapped** —
  byte-identical to the static render of the same widget (FR-006/SC-004) — so an
  un-animated view is unchanged (FR-007/SC-005). A non-identity transform lowers to a
  `PerspectiveNode`; sub-unity opacity folds into `Paint.Opacity` / `Color.Alpha`.
- State-driven transitions (Story 2) use `AnimationState<'a>` held by the author in
  their own model. `create`/`advance`/`retarget`/`value`/`isActive` are pure
  transitions; `retarget` sets `Start = Current` so a mid-flight target change
  continues from the displayed value with no snap-back (FR-005/SC-006).
- Time advancement + redraw gating is the additive Elmish tick subscription
  (`Animation.tickSubscription`), the only interpreter-edge component. It emits
  `AnimationTick` deltas only while `isAnimating` holds and self-suspends on settle
  (FR-006).

## Principle IV applicability

This feature **is** stateful/time-driven, so Principle IV applies and the boundary is
explicit: **Model** = author-held `AnimationState`; **Msg** = `AnimationTick of TimeSpan`;
**Effect** = the tick subscription at the interpreter edge; **`update`** stays pure
(`advance`/`retarget`). There is **no hidden mutable animation registry**.

## No-`[S]` evidence obligation (real sampling)

Sampling is **real pure computation** — there is **no `[S]` / `[S*]` / `[SEH]`** in
this feature. Easing and tween sampling are arithmetic; deterministic-scene evidence
is real render-only output through `SceneEvidence.render`; parity goldens are real
bytes captured from the sampler (`FS_SKIA_CAPTURE_GOLDEN=1`); the tick subscription is
exercised through the real Elmish `Sub` plumbing. `EvidenceAudit` must be PASS with no
disclosures.

## US1 independent validation path (entrance: opacity 0→1 + translateY 24→0, ease-out, 300 ms)

1. Declare the entrance as data (`Animation.empty with Opacity = …; Transform = …`).
2. Drive `Animation.applyAt` at start / midpoint / settle.
3. Observe monotone progression: opacity `0 → 0.875 → 1`, translateY `24 → 3 → 0`
   (captured in `readiness/fsi/animation-session.txt` and the parity goldens
   `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-entrance-*.txt`).
4. Confirm the settled frame equals the static render of the same widget
   (identity-at-rest; `AnimationOutputTests` settled≡static hash).
5. No redraw is requested once settled (the tick subscription returns `Sub.none`).

FR-003 default resolution: the documented default curve is `Easing.Default = EaseInOut`;
`Tween.Easing` / `Tween.Duration` are explicit fields (no omitted-field defaulting).

## US2 independent validation path (value glide + retarget)

1. Hold an `AnimationState<float>` in the model; bind `view` to `AnimationState.value`.
2. Dispatch a target change → `AnimationState.retarget`.
3. Advance over ticks (`AnimationState.advance`) → the displayed value glides toward
   the target.
4. A mid-flight second retarget continues from the displayed value — no jump back to
   the original start (`AnimationTests` retarget-no-snapback;
   `readiness/fsi/animation-session.txt`).
