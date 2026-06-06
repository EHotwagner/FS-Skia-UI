# Research: Add Animations

Phase 0 decisions. Each resolves a Technical Context unknown; no `NEEDS CLARIFICATION`
remains. Format: Decision / Rationale / Alternatives considered.

## R1 — Package placement: Scene module vs. new package

- **Decision**: Ship the deterministic core as a new `Animation` module inside the existing
  `FS.Skia.UI.Scene` package (`src/Scene/Animation.fsi` + `Animation.fs`), with a thin additive
  subscription helper in `FS.Skia.UI.Elmish`.
- **Rationale**: The animatable properties are already Scene concepts — `Paint.Opacity`
  exists, transforms already lower to the existing `PerspectiveTransform` + `PerspectiveNode`,
  and `Color` is a Scene record. The deterministic render-only evidence path
  (`SceneEvidence.render`, `RendererMode = "deterministic-scene"`) also lives in Scene, so the
  sampling-as-data design composes directly with it. Keeping it in Scene keeps the surface
  additive and avoids a new packable identity (spec Package impact: additive, no new package
  required).
- **Alternatives considered**: (a) A brand-new `FS.Skia.UI.Animation` package — rejected as
  unnecessary packable/versioning overhead for a bounded slice; nothing here is independent of
  Scene. (b) Putting it in `FS.Skia.UI.Controls.Typed` — rejected because animation applies to
  *any* `SceneNode`, not only typed controls, and Scene is the lower, dependency-light layer.

## R2 — Time model: explicit `TimeSpan` vs. wall-clock vs. frame index

- **Decision**: Animations advance against an explicit, supplied `System.TimeSpan` elapsed
  value. Sampling is a pure function `applyAt : elapsed:TimeSpan -> Animation -> Scene ->
  SceneNode`; state advancement is `AnimationState.advance : delta:TimeSpan -> state -> state`.
- **Rationale**: FR-004/FR-009/SC-003 demand reproducibility across runs, machines, and fresh
  processes. A supplied virtual time decouples output from frame-rate jitter: evidence feeds
  chosen samples, a real run feeds real elapsed time into the *same* model. `TimeSpan` is BCL
  (no dependency), is the natural unit for durations, and is exact for fixed sample points.
- **Alternatives considered**: (a) Ambient wall-clock (`DateTime.Now`) inside the sampler —
  rejected, non-deterministic and untestable. (b) Integer frame index — rejected, couples the
  model to a fixed frame rate and makes variable real-time advancement awkward. (c) `float`
  seconds — workable but `TimeSpan` is more self-documenting and avoids unit ambiguity.

## R3 — Easing set: hand-rolled curves, no dependency

- **Decision**: A bounded `Easing` DU — `Linear | EaseIn | EaseOut | EaseInOut` — implemented
  as pure float→float functions over normalized progress `t ∈ [0,1]` (ease curves are cubic).
  `Easing.apply : Easing -> float -> float`. An unspecified easing defaults to `EaseInOut`
  (the documented standard, FR-003).
- **Rationale**: Each curve is ~1 line of arithmetic (Principle III simplicity); a third-party
  tween/easing library would add a dependency for trivially hand-rollable math and is
  explicitly disallowed by the spec governance ("no new third-party package dependency"). A
  bounded named set matches the representative-slice scope and is the smallest set that proves
  "selectable easing" end-to-end.
- **Alternatives considered**: (a) Arbitrary cubic-bézier control points — deferred as broader
  scope; the named set is the proven slice. (b) Adding spring/physics easing — explicitly
  Unsupported scope. (c) A NuGet easing library — rejected per the no-dependency constraint.

## R4 — Transform representation: affine record lowering to `PerspectiveTransform`

- **Decision**: A `Transform` record `{ TranslateX; TranslateY; ScaleX; ScaleY;
  RotationDegrees }` with an `identity` value and `Transform.toPerspectiveTransform : Transform
  -> PerspectiveTransform`. An animated transform lowers to wrapping the target `Scene` in the
  existing `PerspectiveNode`. Labels are deliberately motion-specific (not `X`/`Y`/`Width`/
  `Height`) per the `docs/scaffold-map.md` record-label-collision warning.
- **Rationale**: translate/scale/rotate compose into a 2-D affine matrix, which fits exactly in
  the existing 3×3 `PerspectiveTransform` (`M31 = M32 = 0`, `M33 = 1`). Reusing
  `PerspectiveNode` means **zero new render-path code and no `paintNode` exhaustiveness change**
  — the existing `canvas.Concat(&matrix)` handling already draws it. This keeps the
  layout/rendering change minimal and parity-provable.
- **Alternatives considered**: (a) New `TranslateNode`/`ScaleNode`/`RotateNode` SceneNode cases
  — rejected: expands the DU and forces `paintNode` changes for no expressive gain. (b)
  Interpolating each leaf node's raw geometry — rejected: not composable, leaks into every node
  kind, and breaks the identity-at-rest parity rule.

## R5 — Identity-at-rest lowering (the parity keystone)

- **Decision**: When a sampled property is its identity (opacity `= 1.0`, transform `=
  identity`, i.e. translate 0 / scale 1 / rotation 0), the lowering emits **no** wrapper node —
  the target `Scene` passes through unchanged. A settled animation whose final value is the
  identity is therefore byte-identical to the static render; an un-animated view has no
  animation node at all.
- **Rationale**: This is what makes FR-006/SC-004 (settled ≡ static) and FR-007/SC-005
  (un-animated unchanged) *provable* rather than approximate. Wrapping in an identity
  `PerspectiveNode` would change the scene structure (and therefore the deterministic-scene
  hash) even when visually identical — the pass-through rule guarantees structural identity.
- **Alternatives considered**: (a) Always wrap and rely on Skia producing identical pixels —
  rejected: the deterministic-scene evidence hashes *structure*, so an extra node breaks byte
  parity even with identical pixels. (b) Snapping the final value but leaving the wrapper —
  same rejection.

## R6 — State-driven retargeting (Story 2) without snap-back

- **Decision**: A pure `AnimationState<'a>` record `{ Current; Start; Target; Elapsed;
  Duration; Easing }` with pure transitions: `advance : TimeSpan -> state -> state` (moves
  `Elapsed`, recomputes `Current` via easing from `Start`→`Target`), `retarget : 'a -> state ->
  state` (sets `Start = Current`, `Target = new`, `Elapsed = 0`), `value : state -> 'a`, and
  `isActive : state -> bool`. The author stores these in their own model and calls them from
  their own pure `update`.
- **Rationale**: FR-005/SC-006 require that a mid-flight target change continues from the
  *currently displayed* value, not the original start. Capturing `Start = Current` at retarget
  time is exactly that, and rapid successive retargets compose correctly. Keeping it a pure
  value the author owns honors Principle IV (pure `update`, no hidden framework state) and keeps
  the author MVU contract unchanged for non-animation authors (FR-007).
- **Alternatives considered**: (a) A framework-owned mutable animation registry keyed by widget
  identity — rejected: hidden state, violates the pure-`update` boundary, and complicates the
  "removed widget stops cleanly" edge (it would need explicit deregistration). (b) Re-deriving
  current value from absolute wall time — rejected: can't retarget without snap-back.

## R7 — Redraw gating and the FR-006/FR-001 tension

- **Decision**: Time advancement is delivered as an Elmish **subscription** (`AnimationTick of
  TimeSpan`) that emits frame deltas **only while at least one animation is active**, gated by
  an author-supplied `isAnimating : 'model -> bool`. When all animations settle, the
  subscription stops yielding ticks, so no further model change and no redraw is *requested*.
  The host's internal present loop is **not** rewritten — gating is at the framework-request
  (subscription) level.
- **Rationale**: The spec's own resolution note says motion is bounded to active animations,
  not a perpetual frame loop. Modeling advancement as a subscription that self-suspends on
  settle is the idiomatic Elmish expression of that and keeps the change additive. Rewriting the
  Vulkan present loop for per-widget dirty regions is broader scope and explicitly deferred.
- **Alternatives considered**: (a) A perpetual per-frame tick that always advances — rejected:
  violates FR-006 (idle redraws) for static views. (b) Deep host-loop surgery to suppress
  `DoRender()` when idle — deferred as out-of-scope host work; the subscription-level gate
  satisfies the requirement at the framework boundary.

## R8 — Deterministic evidence: sample-then-render, reuse existing path

- **Decision**: Capture animation evidence by sampling the animation at explicit `TimeSpan`
  points (start / midpoint / end) into distinct `Scene` values
  (`Animation.sampleFrames : times -> Animation -> Scene -> Scene list`) and running each
  through the existing `SceneEvidence.render` with `RendererMode = "deterministic-scene"`. No
  new evidence mechanism, no GPU. Distinct hashes across samples prove progression; the final
  sample's hash equals the static render's hash (parity).
- **Rationale**: FR-009 requires capture "using the existing evidence mechanism." Sampling into
  Scenes and reusing the structural-hash path keeps the render-only, deterministic discipline
  (`docs/scaffold-map.md` must-survive vocabulary) and reproduces byte-identically across runs
  and a fresh process (SC-003).
- **Alternatives considered**: (a) A new time-aware evidence renderer that internally iterates
  frames — rejected: duplicates the existing path and risks introducing nondeterminism. (b)
  GPU-captured PNG sequences — rejected: not deterministic across machines and unnecessary for
  structural proof.

## R9 — Color interpolation

- **Decision**: `Color.lerp : Color -> Color -> float -> Color` interpolates each RGBA byte
  channel linearly (rounded), reusing the existing `FS.Skia.UI.Scene.Color` record. Animated
  color is the author binding the sampled `Color` into the value their view already consumes.
- **Rationale**: Byte-channel lerp is the simplest deterministic color tween and the settled
  end color is exactly the static color (parity). No premultiplied/linear-space conversion is
  needed for the representative slice and would add complexity without proving anything new.
- **Alternatives considered**: (a) Interpolating in linear/HSL space for perceptual smoothness
  — deferred as polish; straight RGBA lerp is the proven slice and is deterministic. (b) A new
  color type — rejected, reuse the Scene `Color`.
