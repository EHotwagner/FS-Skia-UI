# Feature Specification: Add Animations — Declarative Motion for FS.Skia.UI

**Feature Branch**: `073-add-animations`
**Created**: 2026-06-06
**Status**: Draft
**Input**: User description: "add animations"

## Overview

FS.Skia.UI today renders every scene as a **static snapshot**: a model produces a
view, the view renders once, and the rendered output only changes when the model
changes through a discrete message. There is no notion of *motion over time* — no way
for a value to travel smoothly from one state to another, no way to fade, slide,
scale, or rotate a widget across a span of frames. Authors who want even a simple fade
or slide must hand-roll a per-frame clock, recompute interpolated values themselves,
and force redraws manually. Nothing in the public surface helps them.

This feature adds a **declarative animation capability**: an author describes the
*intent* of a motion — "this property should travel from value A to value B over this
duration, with this easing" — and the framework drives that motion over time and
produces the in-between frames automatically. Animation is expressed as data the author
declares against their existing view, not as imperative frame-by-frame code they write.

Consistent with how this framework has shipped every prior capability (a bounded
*representative slice* rather than the exhaustive family — as `065` and `072` did for
typed controls), this feature delivers a **focused, reference slice of animation** that
proves the whole pattern end-to-end:

- A small, well-defined set of **animatable visual properties** (opacity; transform —
  translate, scale, rotate; and tweened color), animated on any existing widget without
  changing what that widget renders when static.
- **Property tweens** between a start and end value over a declared duration, shaped by
  a choice from a **named easing set** (e.g. linear and a few standard ease curves).
- **State-driven transitions**: when the model changes such that a property's target
  value changes, the property animates toward the new target rather than snapping.
- A **deterministic time model** so that animated output is reproducible and can be
  captured as render evidence at explicit time samples — preserving this repository's
  render-only, deterministic evidence discipline.

The breadth beyond this slice stays **deferred**: physics/spring simulation as a
general system, gesture- or input-driven interactive scrubbing, sequenced/chained
timelines and keyframe tracks, particle systems, video, and GPU shader effects are all
out of scope for this feature and named explicitly under Unsupported scope. This is the
*representative-slice* expansion that establishes the animation front door, not the
complete motion system.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A product author fades and slides a widget into view (Priority: P1)

A product developer building a generated FS.Skia.UI app wants a panel to fade in and
slide up when it first appears, instead of popping in instantly. Today they have no
mechanism for this. With this feature they declare, against the widget they already
author, that its opacity should animate from 0 to 1 and its vertical translation from a
small offset to 0 over a short duration with an ease-out curve. They write no clock and
no per-frame interpolation. When the panel appears, the framework produces the
intermediate frames and the panel smoothly fades and slides into place; when the
animation completes, the widget rests at its final static appearance with no further
redraws.

**Why this priority**: A fade/slide entrance is the single most common, most
recognizable motion and the clearest user-visible proof that "animation" exists at all.
Proving one declarative tween end-to-end (declared intent → driven over time →
intermediate frames → rest state) validates the entire capability for every later
property and curve.

**Independent Test**: Author a panel with a declared opacity + translate entrance
animation; advance the deterministic time model across the animation's duration; confirm
that captured frames at start, midpoint, and end show monotonic progression of opacity
and position; confirm the final frame is identical to the static (un-animated) rendering
of the same widget; and confirm no redraw is requested after the animation settles.

### User Story 2 - A property animates toward a new target when the model changes (Priority: P1)

A product author has a value indicator (for example, a bar or a highlight position)
bound to model state. When a message updates the model so the target value changes, the
author wants the indicator to glide to the new value rather than jump. With this feature
the author declares the property as animated; each time the target changes, the
framework animates from the property's current displayed value to the new target over
the declared duration. Rapid successive changes retarget smoothly from wherever the
property currently is, without visual jumps or restarts from the original start value.

**Why this priority**: State-driven transition is the core reason animation belongs in
an MVU framework — motion is a consequence of state change, expressed declaratively, not
an imperative side effect. It is distinct from the one-shot entrance of Story 1 and must
be proven independently.

**Independent Test**: Author a property bound to model state with a declared transition;
dispatch a message that changes the target; advance time and confirm the displayed value
interpolates from the prior displayed value to the new target; mid-transition, dispatch a
second target change and confirm the property retargets from its current displayed value
(no snap-back to the original start), arriving at the latest target.

### User Story 3 - Animated output is captured as deterministic evidence (Priority: P2)

A framework maintainer (and the repository's evidence gates) must be able to prove an
animation renders correctly without depending on wall-clock timing or frame-rate jitter.
With this feature, animations advance against an explicit, supplied time model, so a test
or evidence run can sample the animation at chosen time points and get byte-stable output
for the same inputs. The maintainer captures render evidence at several time samples
across an animation and the captured frames are reproducible across runs and machines.

**Why this priority**: This repository's render-only, deterministic evidence discipline
is non-negotiable; an animation capability that produced non-reproducible output would be
unmergeable. Determinism is what makes the rest shippable, so it is a first-class scenario
rather than an implementation footnote — but it is P2 because it gates *acceptance* rather
than delivering the author-visible value of Stories 1 and 2.

**Independent Test**: Render the same animation at the same set of explicit time samples
twice (and on a fresh process); confirm the captured frames are identical between runs.

### User Story 4 - Animation is opt-in and never degrades static authoring (Priority: P3)

An author who does not use animation, and every existing generated app and control,
must be completely unaffected: the same view authored without any animation declaration
renders exactly as it does today, with no continuous redraw, no new required parameters,
and no behavioral change. Animation is purely additive — present only where an author
declares it.

**Why this priority**: The framework's existing static contract and all current
consumers must be preserved; this guards against regression. It is P3 because it is a
constraint on the other stories rather than new user-facing value.

**Independent Test**: Render a representative existing view/control with no animation
declarations and confirm its output and redraw behavior are unchanged from current
behavior (golden parity); confirm no animation-related parameter is required to author it.

### Edge Cases

- **Zero or negative duration**: A declared duration of zero (or non-positive) resolves
  immediately to the end value on the next frame — it does not divide by zero, hang, or
  animate forever.
- **Retargeting mid-flight**: Changing the target while an animation is running animates
  from the current displayed value, not from the original start (covered by Story 2).
- **Interrupted / removed widget**: If an animating widget is removed from the view before
  its animation completes, the animation stops cleanly and is not leaked or left driving
  redraws.
- **Simultaneous animations**: Multiple independent animations active at once (on the same
  or different widgets) each advance correctly against the shared time model without
  interfering.
- **Out-of-range easing inputs**: Time progress is clamped to the animation's domain so a
  sample before the start or after the end yields the start or end value, respectively.
- **Unsupported / headless environment**: Where rendering is unsupported, animation
  produces the same benign, classified diagnostic behavior as existing static rendering —
  it does not introduce a new failure mode.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The framework MUST let an author declare, against an existing widget, that a
  supported visual property animates from a start value to an end value over a stated
  duration, without the author writing per-frame interpolation or managing a clock.
- **FR-002**: The supported animatable properties MUST include, at minimum, opacity,
  transform translation, transform scale, transform rotation, and color — applied so that
  a property's value at a given time is the interpolation between its endpoints shaped by
  the chosen easing.
- **FR-003**: The framework MUST provide a named, bounded easing set (at minimum linear and
  a small number of standard ease curves) selectable per animation; an unspecified easing
  MUST default to a documented standard curve.
- **FR-004**: The framework MUST advance animations against an explicit, supplied time model
  such that the rendered output for a given set of inputs and a given time sample is
  deterministic and reproducible across runs and machines.
- **FR-005**: When the target value of an animated property changes (because the model
  changed), the framework MUST animate from the property's current displayed value toward
  the new target over the declared duration, retargeting smoothly without snapping back to
  the original start value.
- **FR-006**: A completed animation MUST settle to its end (or current-target) value and
  MUST NOT request further redraws while at rest; the settled output MUST be identical to
  the static rendering of the same widget at that value.
- **FR-007**: Animation MUST be opt-in and additive: any view or control authored without
  an animation declaration MUST render and behave exactly as it does today, with no new
  required parameter and no continuous redraw.
- **FR-008**: The framework MUST handle the defined edge cases deterministically — non-positive
  duration resolves immediately to the end value, time progress is clamped to the
  animation domain, removing an animating widget stops its animation cleanly, and multiple
  concurrent animations advance independently.
- **FR-009**: Animated output MUST be capturable as render evidence at explicit time samples
  using the existing evidence mechanism, preserving the repository's render-only,
  deterministic evidence discipline.
- **FR-010**: In unsupported or headless rendering environments, animation MUST exhibit the
  same benign/blocking host-warning classification as existing static rendering and MUST
  NOT introduce a new uncaught failure mode.

> Interacting / conflicting requirements: FR-006 (settle and stop redrawing at rest) and
> the continuous-motion need of FR-001/FR-005 pull in opposite directions. Resolution:
> the framework drives redraws **only while at least one animation is active**, and stops
> driving redraws once all animations have settled — motion is bounded to active
> animations, not a perpetual frame loop.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Adds a new public animation authoring capability. Whether it ships as
  a new module/surface within an existing package (e.g. the scene/host/typed-controls
  surface) or as a new package is a plan-phase decision; either way it is **additive** —
  no existing package identity, contents, or version semantics are removed or repurposed,
  and no new third-party package dependency is required. No Charts/Graph/DataGrid package
  migration is implied.
- **Public contract impact**: Yes — new public API for declaring animations (new `.fsi`
  signatures and surface-baseline additions). All additions are purely additive; no
  existing public signature changes shape.
- **State workflow impact**: Yes — animations advance over time, so this introduces a
  time/clock-driven advancement integrated with the existing host update loop
  (subscription/command/effect/interpreter behavior for time-based advancement). The
  user-authored MVU `init`/`update`/`view` contract is unchanged for authors who do not
  use animation.
- **Layout/rendering impact**: Yes — animation produces time-varying visual output and new
  screenshots/render evidence sampled across frames. The bounded property set is chosen so
  that the static rendering of a settled animation is identical to today's output; any
  layout-reflowing motion beyond the bounded transform/opacity/color set is out of scope.
- **Evidence obligations**: Real render evidence captured at multiple explicit time samples
  (start/midpoint/end) for at least the Story 1 and Story 2 reference animations, plus a
  golden-parity artifact proving a settled animation equals static output (FR-006) and that
  an un-animated view is unchanged (FR-007).
- **Unsupported scope**: General physics/spring simulation as a system, gesture- or
  input-driven interactive scrubbing, sequenced/chained timelines and keyframe tracks,
  particle systems, video playback, GPU/shader visual effects, and layout-reflowing
  animation beyond the bounded property set. Platform, release, and distribution boundaries
  are unchanged by this feature.
- **Build-target impact**: Likely touches the rendering/evidence path — `Verify` and the
  evidence targets (`EvidenceGraph`/`EvidenceAudit`) gain animation evidence; `Dev` covers
  the new unit/parity tests. New public `.fsi` and any template/consumer-facing surface
  escalate routing per `Route`. No change to `PackLocal`/`TemplateCheck` semantics beyond
  the additive surface, to be confirmed by `Route` against the actual diff at plan time.

## Success Criteria *(mandatory)*

- **SC-001**: An author can make an existing widget fade and slide into view by adding a
  declarative animation to that widget alone, writing no clock and no per-frame
  interpolation.
- **SC-002**: For a declared tween, the displayed property value progresses monotonically
  (per its easing) from start to end across the duration, and the value sampled at the end
  of the duration equals the declared end value.
- **SC-003**: Rendering the same animation at the same explicit time samples produces
  identical captured output across repeated runs and across separate processes (100%
  reproducible).
- **SC-004**: A settled animation's rendered output is identical to the static rendering of
  the same widget at its final value, and no redraw is requested once all animations have
  settled.
- **SC-005**: A view or control authored with no animation declaration renders identically
  to current behavior (golden parity) and requires no new parameter.
- **SC-006**: When an animation's target changes mid-flight, the displayed value continues
  from its current point to the new target without a visible jump back to the original
  start value.
- **SC-007**: All defined edge cases (non-positive duration, out-of-range time, removed
  animating widget, concurrent animations) resolve to their specified deterministic
  outcome with no hang, exception, or perpetual redraw.

## Assumptions

- **Representative slice, not the full motion system**: Following the established `065`/`072`
  pattern, this feature delivers a bounded reference slice (the property set, easing set,
  and transition behavior above). The broader motion family is intentionally deferred and
  enumerated under Unsupported scope.
- **Bounded property set**: The animatable properties are limited to opacity, transform
  (translate/scale/rotate), and tweened color — chosen because a settled value of each
  reproduces today's static output, keeping the change additive and parity-provable.
- **Explicit time model over wall-clock**: Animations advance against a supplied/virtual
  time value rather than ambient wall-clock time, so evidence and tests are deterministic.
  A real run feeds real elapsed time into the same model; evidence feeds chosen samples.
- **Redraws bounded to active animations**: The framework requests redraws only while an
  animation is active and stops when all settle (resolves the FR-006 conflict above),
  preserving today's no-idle-redraw behavior for static views.
- **Author-facing MVU contract preserved**: Authors who do not use animation see no change
  to `init`/`update`/`view` shape and no new required parameters.
- **Standard motion defaults**: Where the author does not specify duration or easing, the
  framework uses documented standard defaults (a short duration and a standard ease curve)
  rather than requiring every value to be stated.

## Key Entities

- **Animation declaration**: An author-declared description of a motion — the target
  property, its start and end (or target) values, a duration, and an easing choice.
  Declared as data against a widget, not imperative code.
- **Animatable property**: A supported visual dimension of a widget that can be
  interpolated over time (opacity, transform translate/scale/rotate, color).
- **Easing**: A named curve from the bounded set that maps normalized time progress to
  eased progress, shaping how a value moves between its endpoints.
- **Time model / clock**: The explicit time value against which animations advance,
  supplied so that output is deterministic and reproducible for a given sample.
- **Animation state**: The currently-displayed value and progress of an in-flight
  animation, which is what enables smooth retargeting when a target changes mid-flight.
