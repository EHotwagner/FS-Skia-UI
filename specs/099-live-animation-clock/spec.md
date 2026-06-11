# Feature Specification: Animation Clock on Retained Identity

**Feature Branch**: `099-live-animation-clock`  
**Created**: 2026-06-11  
**Status**: Draft  
**Input**: User description: "create the next part of @docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md"

## Context & Source

This feature is **R4 — Animation clock on retained identity** from the controls
architecture evolution roadmap
(`docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md`, §10.6).
It is the next remediation in the roadmap's recommended order
**R1 → {R3, R2} → R4 → R5**, after R1 (feature 096, runtime visual-state bridge),
R2 (feature 097, incremental partial re-layout), and R3 (feature 098, binding-aware
recovery) have all shipped and merged.

R4 **completes E2** (the retained-identity step). E2's exit criterion — "an in-flight
animation survives an unrelated state change" — is today met only as a *carried slot*:
the per-identity animation state field exists and is preserved across frames, but
**nothing in the running host ever writes or advances it**. The survival test that
exercises it hand-seeds the clock and explicitly labels it a `PRECONDITION` because
"no animation seam exists yet". This feature builds that missing seam so per-control
animation actually runs live, deterministically, on injected frame deltas.

R4 is **architecture-preserving and non-goal-preserving**: it finishes wiring a
capability the existing features already built. It introduces no data binding, no
dependency properties, no CSS selectors, and no template engine.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A visual-state transition animates on the live host (Priority: P1)

A consumer runs an interactive app built on the framework with **zero animation
code**. When the user hovers, presses, or focuses a control whose kind participates
in visual-state styling (the R1-migrated interactive set), the control's appearance
does not *snap* between states — it **animates** the transition (e.g. a focus ring
fades in, a press tint eases). The animation is driven entirely by the host advancing
a per-control clock each frame; the consumer's `view` is unchanged.

**Why this priority**: This is the headline live-window behavior the roadmap promises
("a visual-state transition animates on the live host with zero consumer code") and
the most visible proof the seam works end-to-end. It is the primary integration of the
R1 state-change trigger with the carried animation slot.

**Independent test**: On the live host, drive a hover/focus interaction on a migrated
control and capture a sequence of frames; the rendered appearance changes *gradually*
across consecutive frames (intermediate sampled values present), not in a single step,
and reaches the target state. A build without the seam produces an instant snap and
fails the proof.

### User Story 2 - An in-flight animation survives an unrelated re-render and completes (Priority: P1)

While a control's transition is mid-flight, an **unrelated** state change elsewhere
in the model triggers a re-render that shifts sibling positions. The animating
control keeps its identity (via E2 retained identity), its clock keeps advancing from
where it was, and the animation **completes deterministically** — it is neither reset
to the start nor dropped. This realizes E2's exit criterion as true behavior through
the real seam, replacing the hand-seeded precondition test.

**Why this priority**: This is the exact exit criterion R4 exists to make true, and
the one E2 left only carried-but-unproven. It is independently demonstrable and is the
correctness heart of "retained identity for animation".

**Independent test**: Start a tween on a control, advance a few frames, then apply a
sibling-shifting re-render and continue ticking; the same retained identity's clock
continues from its prior elapsed value and reaches completion, with the same final
result as if no shift had occurred (driven through the real host seam, not hand-seeded).

### User Story 3 - Animation is deterministic and identity-at-rest is preserved (Priority: P1)

A control with **no active tween** contributes **no animation output** and renders
**byte-identical** to a non-animated build — preserving E2's "recomputed node count is
zero at rest" invariant. When animations *are* active, replaying the same sequence of
**injected frame deltas** produces the **same** sampled values every time, with no
dependence on wall-clock time.

**Why this priority**: Determinism and identity-at-rest are constitutional invariants;
violating either would regress the golden-diff evidence spine that every prior feature
relies on. This story is the governance gate that keeps the seam admissible.

**Independent test**: (a) Render a frame with no active animations and assert
byte-identity against the pre-R4 golden and a zero recompute/at-rest count. (b) Drive
two runs with an identical fixed delta sequence and assert identical sampled output;
confirm no wall-clock source is consulted.

### User Story 4 - Clocks are garbage-collected for removed identities (Priority: P2)

When a control is removed from the tree (its identity no longer appears after a
re-render), any animation clock attached to that identity is **dropped**, not leaked
or carried indefinitely. This reuses the existing live-identity filter that already
GCs focus/text state for removed identities.

**Why this priority**: Prevents unbounded retained state growth in long-running apps;
lower priority because it reuses an existing, proven GC mechanism rather than building
new machinery.

**Independent test**: Animate a control, then re-render with that control removed; the
retained state for its identity (including its animation clock) is absent on the next
frame, matching the existing GC behavior for focus/text state.

### Edge Cases

- **Zero / negative / very large frame delta**: A zero delta advances nothing (clock
  is unchanged, still deterministic). A negative delta MUST NOT occur from the host;
  the projection treats non-positive deltas as no-ops rather than rewinding. A very
  large delta clamps the tween to its completed end state (no overshoot past the
  target).
- **A new transition triggered while one is in flight**: A state change that retargets
  an already-animating control re-aims the tween toward the new target from the current
  sampled value (no snap to start), preserving continuity.
- **Transition trigger when R1 derives `Normal`**: If a control returns to `Normal`,
  the transition animates back toward the at-rest appearance and, once complete, emits
  no animation output so identity-at-rest is restored.
- **Multiple controls animating simultaneously**: Each retained identity advances its
  own independent clock; one control completing does not affect another's elapsed time.
- **A control removed mid-animation**: Its clock is GC'd (User Story 4); no partial or
  dangling sample is painted on the frame it disappears.
- **Reduced-motion / consumer opt-out** *(see Assumptions)*: When animation is disabled
  by policy, transitions snap (the pre-R4 behavior) and identity-at-rest holds trivially.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The interactive host MUST maintain a per-retained-identity animation
  clock and advance it each frame by the host's **injected per-frame delta**, never by
  a wall-clock or system-time source.
- **FR-002**: Each frame, the host MUST sample every live per-identity animation state
  at its current elapsed value and feed the sampled values into that frame's paint, so
  the rendered appearance reflects the in-progress animation.
- **FR-003**: A live visual-state transition (the R1 bridge flipping a control's
  derived `VisualState`, e.g. `Normal → Hover` or gaining/losing `Focused`) MUST
  **start (or retarget) a tween** on that control's retained identity, so the style
  transition animates rather than snapping.
- **FR-004**: An in-flight animation MUST **survive an unrelated, sibling-shifting
  re-render** by retaining its identity and continuing from its prior elapsed value,
  and MUST **complete deterministically** through the real host seam (not via a
  hand-seeded test fixture).
- **FR-005**: A control with **no active tween** MUST contribute **no animation
  output** and render byte-identical to the non-animated build, preserving the
  identity-at-rest / zero-recompute invariant.
- **FR-006**: Replaying an **identical sequence of injected frame deltas** MUST produce
  **identical** sampled output across runs; the animation clock MUST be a pure function
  of accumulated injected deltas with no nondeterministic input.
- **FR-007**: Animation clocks for **removed identities** MUST be dropped using the
  existing live-identity GC filter, so retained animation state does not leak for
  controls no longer present in the tree.
- **FR-008**: The clock carry-across-frames MUST reuse the existing per-identity
  (retained-id-keyed) state carry established by E2; R4 MUST NOT introduce a parallel
  identity scheme.
- **FR-009**: Animation MUST be sampled and applied using the **existing feature-073
  Scene animation primitives** (the tween / animation-state / sample-at machinery);
  R4 MUST NOT re-implement an animation engine.
- **FR-010**: The scoped-repaint behavior from E2/R2 MUST be preserved — an animating
  control's per-frame repaint stays scoped to its own subtree; advancing the clock MUST
  NOT force a whole-tree repaint or re-measure.

> Interacting / conflicting requirements: FR-002 (sample-and-paint every active
> animation each frame) vs. FR-005/FR-010 (identity-at-rest, scoped repaint). Resolution:
> only identities with an **active** tween are sampled and contribute a per-frame
> repaint; identities at rest emit no animation output and are not repainted on account
> of animation. The presence of *any* active animation does not globally invalidate the
> at-rest fast path for the *other* controls.
>
> FR-003 (a transition starts a tween) vs. FR-005 (at rest, no output): a transition is
> "active" only from trigger until the tween reaches its end; on completion the control
> returns to emitting no animation output, restoring identity-at-rest. The completed
> frame must converge to exactly the snapped target appearance so the at-rest golden is
> reached, not merely approached.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This
> section is *expected* to name concrete packages, `.fsi` signatures, build targets,
> effects/interpreters, Vulkan/Skia, and evidence paths — that is its purpose.

- **Package impact**: Active packages `FS.Skia.UI.Controls` and
  `FS.Skia.UI.Controls.Elmish` change (the host animation seam and the per-identity
  clock advance/sample). The Scene animation primitives in `FS.Skia.UI.Scene`
  (feature 073) are **reused, not modified**. No legacy Charts migration. Generated
  package consumers (template, generated app host) gain live animation transparently;
  no consumer API rename is required.
- **Public contract impact**: Expect `.fsi` changes in
  `src/Controls.Elmish/ControlsElmish.fsi` for the host tick → clock-advance seam, and
  possibly `src/Controls/RetainedRender.fsi` if the sample-on-paint helper is promoted
  beyond module-internal. The R1 visual-state derivation surface is **consumed, not
  changed**. If only module-internal wiring is touched, the public surface stays stable;
  any `.fsi` edit escalates to the controls-public-surface (agent-ready / maintainer-
  verify) route and requires recaptured published api-surface + per-package baselines.
- **State workflow impact**: The interactive host loop changes — the per-frame `Tick`
  delta now advances per-identity `AnimationState` before paint. This is internal
  framework state (retained-id-keyed), not consumer-visible model state; the MVU
  `view : 'model -> Control<'msg>` contract is unchanged. The clock is driven by
  injected deltas only (no `Date.now`), honoring the determinism constitution and the
  environment's no-wall-clock constraint.
- **Layout/rendering impact**: Rendering changes — animating controls paint sampled
  intermediate values across frames. Layout is **not** affected (animation here targets
  paint-level transform/opacity/color per the Scene `Animation` shape, not flex
  geometry), so R2's incremental measure and the scoped-repaint reduction are preserved.
  Vulkan/Skia output for at-rest frames stays byte-identical.
- **Evidence obligations**: A **responds-vs-renders / animates-vs-snaps** runtime
  artifact proving input→gradual-visible-change across consecutive frames on the live
  host (an unbridged/no-seam build fails it); a **survival** proof that an in-flight
  tween continues through a sibling-shifting re-render and completes via the real seam
  (replacing the hand-seeded `Feature092LiveSurvivalTests` precondition); an
  **identity-at-rest** golden showing no animation output and byte-identity when no
  tween is active; a **determinism** property proof that a fixed injected-delta sequence
  yields identical sampled output. Real evidence paths under
  `specs/099-live-animation-clock/` (e.g. `evidence/`), plus the standard
  `EvidenceGraph` / `EvidenceAudit` artifacts.
- **Unsupported scope**: No new authored animation API for consumers (no per-control
  animation DSL, keyframe authoring, or timeline surface) — R4 wires the *automatic*
  visual-state-transition animation only. No spring/physics models beyond the existing
  feature-073 easing/tween set. No animation of layout geometry (size/position reflow).
  No general-purpose animation scheduler beyond the per-frame tick advance. Full-52
  control restyle/animation remains out (tracked with E3/R1). Release, platform, and
  distribution boundaries unchanged.
- **Build-target impact**: `Dev` runs the new unit/property/integration tests. The
  change is consumer-contract-touching if any `.fsi` moves, so the escalated path
  applies: `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
  `EvidenceGraph`, `EvidenceAudit` per the serialized six-target order. `Route` is the
  authority — run `./fake.sh build -t Route` against the actual diff and run only the
  gates it prints; `--enforce` for missing evidence. No new FAKE target is expected.

## Success Criteria *(mandatory)*

- **SC-001**: On the live host, a hover/press/focus visual-state transition on a
  migrated control **animates** — at least one intermediate sampled appearance is
  observable across consecutive frames before the target is reached — with **zero
  consumer animation code**; a no-seam build snaps instantly and fails this criterion.
- **SC-002**: An in-flight tween **survives a sibling-shifting unrelated re-render** and
  **completes deterministically through the real host seam**, reaching the same final
  result as an un-shifted run; the prior hand-seeded `PRECONDITION` survival test is
  replaced by one driven entirely through the real seam.
- **SC-003**: A frame with **no active animation** is **byte-identical** to the pre-R4
  golden and reports a **zero** at-rest recompute/animation-output count.
- **SC-004**: Two runs over an **identical injected-delta sequence** produce **identical**
  sampled output (property-tested over randomized fixed-delta sequences); no wall-clock
  source is consulted.
- **SC-005**: An animation clock for a **removed identity** is **absent** on the
  following frame (GC'd via the existing live-identity filter), with no leaked or
  dangling animation state.
- **SC-006**: Advancing the clock for an animating control keeps its per-frame repaint
  **scoped to its own subtree** — the work-reduction metric shows animation does not
  force whole-tree repaint or re-measure (R2 incremental measure preserved).
- **SC-007**: The serialized validation order (or the minimal gate set `Route` prints
  for the actual diff) is **green**, with `EvidenceAudit` passing and no synthetic or
  stubbed work.

## Key Entities *(data involved)*

- **Per-identity animation clock**: The accumulated-elapsed state attached to a retained
  control identity, advanced by injected frame deltas and sampled each frame. Carried
  across frames by the existing retained-id-keyed state map; GC'd when its identity is
  removed. (Realized via the existing carried animation slot on the retained UI state
  and the feature-073 animation-state primitive.)
- **Transition trigger**: The event of R1's runtime visual-state bridge flipping a
  control's derived `VisualState`, which starts or retargets that identity's tween.
- **Injected frame delta**: The per-frame time delta supplied by the host loop's tick;
  the **sole** time input to the clock (deterministic, no wall-clock).
- **Sampled animation output**: The per-frame interpolated paint value (transform /
  opacity / color per the Scene `Animation` shape) fed into the paint pass; absent when
  no tween is active (identity-at-rest).

## Assumptions

- **Animation targets are paint-level** (transform/opacity/color via the existing
  feature-073 `Animation` shape), **not layout geometry**. Animating reflowable size or
  position is out of scope, which is what keeps R2's incremental layout and the
  scoped-repaint reduction intact.
- **The transition is automatic and visual-state-driven**, not consumer-authored. The
  consumer opts in only by using the R1-migrated kinds; there is no new animation
  authoring API in this feature.
- **A reasonable default transition** (short duration + a standard easing from the
  feature-073 set) is applied to visual-state transitions; the exact duration/easing is
  a plan-level detail and can be a single framework default, not a per-control consumer
  knob.
- **The host already supplies a per-frame delta** via its tick (confirmed present and
  wired through to the host, but currently unused for animation); R4 consumes that
  existing delta rather than introducing a new time source.
- **Reduced-motion / opt-out**: if a motion-disable policy exists or is added, disabling
  animation reverts to the pre-R4 snap behavior; this feature does not *require* such a
  policy but must not preclude it. (If no policy exists, transitions animate by default.)
- **R1 is landed** (it is — feature 096), so the visual-state derivation that triggers
  transitions is available on the live path.

## Dependencies

- **R1 (feature 096, runtime visual-state bridge)** — provides the live visual-state
  change that is the transition trigger. **R4 sequences after R1.**
- **E2 (features 091 + 092, retained identity)** — provides the retained identity the
  clock attaches to and the carry/GC machinery the clock reuses.
- **Feature 073 (Scene animation)** — provides the tween / animation-state / sample-at
  primitives, reused unchanged.
- Independent of R2 (097) and R3 (098); benefits from neither nor blocks them.

## Out of Scope

- A consumer-facing animation authoring API (keyframes, timelines, per-control DSL).
- Spring/physics or easing models beyond the existing feature-073 set.
- Animation of **layout** geometry (size/position reflow).
- A general animation scheduler beyond the per-frame tick advance.
- Full-52-control animation/restyle coverage (tracked with E3/R1).
- R5 (general navigation-key delivery) — the next and final roadmap remediation.
