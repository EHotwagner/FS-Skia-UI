# Data Model: Add Animations

All types live in the new `FS.Skia.UI.Scene.Animation` module unless noted. Everything is an
immutable record/DU; all transitions are pure functions. Reused existing Scene types are cited
inline. The Elmish tick helper lives in `FS.Skia.UI.Elmish` (see the last section).

## Reused existing Scene types (no change)

| Type | Source | Role here |
|------|--------|-----------|
| `Color` (`{ Red; Green; Blue; Alpha : byte }`) | `Scene.fsi` | tweened color; `Color.lerp` interpolant |
| `Paint` (`Opacity : float`) | `Scene.fsi` | opacity lowering folds into `Paint.Opacity` / `Color.Alpha` |
| `PerspectiveTransform` (3×3) | `Scene.fsi` | affine `Transform` lowers into this matrix |
| `PerspectiveNode of PerspectiveTransform * Scene` | `Scene.fsi` (`SceneNode`) | wrapper a non-identity transform lowers to |
| `Scene` (`{ Nodes : SceneNode list }`) / `SceneNode` | `Scene.fsi` | the animation target subtree |
| `SceneEvidence.render` / `RendererMode = "deterministic-scene"` | `Scene.fsi` | evidence sink for sampled frames |

## `Easing` (DU)

| Case | Curve on `t ∈ [0,1]` | Notes |
|------|----------------------|-------|
| `Linear` | `t` | identity progress |
| `EaseIn` | `t³` (cubic) | slow start |
| `EaseOut` | `1 - (1-t)³` | slow finish |
| `EaseInOut` | piecewise cubic | **documented default** when easing unspecified (FR-003) |

- `Easing.apply : Easing -> float -> float` — maps normalized progress to eased progress.
  Endpoints are pinned: `apply e 0.0 = 0.0`, `apply e 1.0 = 1.0` for every case (SC-002).
- Validation: input `t` is clamped to `[0,1]` before the curve, so out-of-domain samples yield
  the endpoint (FR-008 / edge "out-of-range easing inputs").

## `Transform` (record)

| Field | Type | Default (identity) | Notes |
|-------|------|--------------------|-------|
| `TranslateX` | `float` | `0.0` | px offset; motion-specific label (not Scene `X`) |
| `TranslateY` | `float` | `0.0` | px offset |
| `ScaleX` | `float` | `1.0` | uniform or per-axis scale |
| `ScaleY` | `float` | `1.0` | |
| `RotationDegrees` | `float` | `0.0` | rotation about the local origin |

- `Transform.identity : Transform` — all-identity value.
- `Transform.lerp : Transform -> Transform -> float -> Transform` — per-field linear interpolation.
- `Transform.toPerspectiveTransform : Transform -> PerspectiveTransform` — composes
  translate∘rotate∘scale into the existing 3×3 (`M31 = M32 = 0`, `M33 = 1`).
- `Transform.isIdentity : Transform -> bool` — drives the identity-at-rest pass-through (R5).

## `Tween<'a>` (record) — one declared property motion

| Field | Type | Notes |
|-------|------|-------|
| `Start` | `'a` | value at `elapsed = 0` |
| `End` | `'a` | value at `elapsed ≥ Duration` |
| `Duration` | `TimeSpan` | non-positive ⇒ resolves immediately to `End` (FR-008) |
| `Easing` | `Easing` | shaping curve |

- Construction uses an interpolant the caller supplies for `'a` (`float`, `Color`, `Transform`)
  — see `lerpFloat` / `Color.lerp` / `Transform.lerp`.
- `Tween.progress : TimeSpan -> Tween<'a> -> float` — normalized, eased, **clamped** progress
  in `[0,1]`; `Duration ≤ 0` ⇒ `1.0`.
- `Tween.sample : interp:('a -> 'a -> float -> 'a) -> TimeSpan -> Tween<'a> -> 'a` — the value
  at a time sample. Monotone in elapsed per its easing (SC-002).

## `Animation` (record) — the author-declared, sample-as-data motion

A bounded set of animated properties applied to a target `Scene`. Each property is optional;
an absent property is treated as its identity.

| Field | Type | Notes |
|-------|------|-------|
| `Opacity` | `Tween<float> option` | folds into `Paint.Opacity` / `Color.Alpha` over the subtree |
| `Transform` | `Tween<Transform> option` | lowers to `PerspectiveNode` unless identity |
| `Color` | `Tween<Color> option` | exposes a sampled `Color` for the author to bind |

- `Animation.applyAt : elapsed:TimeSpan -> Animation -> Scene -> SceneNode` — **pure**;
  produces the target scene transformed for that time sample. **Identity-at-rest rule (R5):**
  when the sampled opacity is `1.0` and the sampled transform is identity, returns the target
  scene's node(s) unwrapped (byte-identical to static — FR-006/SC-004/SC-005).
- `Animation.sampleFrames : times:TimeSpan list -> Animation -> Scene -> Scene list` — samples
  the animation at explicit points for deterministic evidence (R8/FR-009).
- `Animation.isSettled : elapsed:TimeSpan -> Animation -> bool` — every present tween has
  `elapsed ≥ Duration`; drives redraw gating.

### Edge-case resolution (FR-008 / SC-007)

| Edge | Resolution |
|------|------------|
| `Duration ≤ 0` | progress `= 1.0` on the next sample ⇒ immediate end value; no divide-by-zero |
| Sample before start / after end | clamped to start / end value respectively |
| Animating widget removed from view | the author drops the `AnimationState` from their model ⇒ no further ticks; nothing leaked |
| Multiple concurrent animations | each is an independent value sampled against the shared time ⇒ no interference |

## `AnimationState<'a>` (record) — stateful retargeting (Story 2)

Held by the author in their own model; all transitions pure (Principle IV).

| Field | Type | Notes |
|-------|------|-------|
| `Current` | `'a` | currently displayed value (what `view` reads) |
| `Start` | `'a` | value the in-flight transition began from |
| `Target` | `'a` | value being animated toward |
| `Elapsed` | `TimeSpan` | time into the current transition |
| `Duration` | `TimeSpan` | transition length |
| `Easing` | `Easing` | shaping curve |

| Transition | Signature | Behavior |
|------------|-----------|----------|
| init | `AnimationState.create : interp -> 'a -> TimeSpan -> Easing -> AnimationState<'a>` | `Current = Start = Target = initial`, `Elapsed = 0` |
| advance | `AnimationState.advance : TimeSpan -> AnimationState<'a> -> AnimationState<'a>` | adds delta to `Elapsed` (capped at `Duration`), recomputes `Current` via easing `Start`→`Target` |
| retarget | `AnimationState.retarget : 'a -> AnimationState<'a> -> AnimationState<'a>` | `Start = Current`, `Target = new`, `Elapsed = 0` ⇒ continues from displayed value, **no snap-back** (FR-005/SC-006) |
| read | `AnimationState.value : AnimationState<'a> -> 'a` | returns `Current` |
| active | `AnimationState.isActive : AnimationState<'a> -> bool` | `Elapsed < Duration && Current <> Target` |

The `interp` argument is the per-`'a` interpolant (`lerpFloat` / `Color.lerp` /
`Transform.lerp`), so one state machine serves opacity, color, and transform.

## Elmish tick helper (`FS.Skia.UI.Elmish`, additive)

| Symbol | Signature | Role |
|--------|-----------|------|
| `AnimationTick` | `AnimationTick of TimeSpan` (msg case or wrapper) | per-frame elapsed delta routed into the author's `update` |
| `Animation.tickSubscription` | `isAnimating:('model -> bool) -> Sub<'msg>` (routed to `AnimationTick`) | emits deltas **only while active**; self-suspends on settle (FR-006/R7) |

The subscription is the only interpreter-edge component: the host feeds real elapsed time;
evidence feeds chosen samples through `Animation.sampleFrames` directly (no subscription). The
author's `init`/`update`/`view` shape is unchanged for anyone not using animation (FR-007).
