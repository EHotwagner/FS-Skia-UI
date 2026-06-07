# Feature Specification: Mouse Input & Pointer Events

**Feature Branch**: `075-mouse-input-events`
**Created**: 2026-06-07
**Status**: Draft
**Input**: User description: "add mouse support/ mouse events...."

## Overview

The framework already receives raw pointer coordinates from the host window
(`PointerMoved`, `PointerPressed`, `PointerReleased`) and already models hover,
press, and drag state in the control runtime, and it already computes per-control
bounds and can hit-test a point against them. What is missing is the **public,
consumer-facing pointer-interaction contract** and the **coordination layer** that
turns a stream of raw pointer coordinates into meaningful, control-addressed
interactions (hover enter/leave, click, press/release, drag lifecycle) that an
application's MVU update loop can subscribe to — at parity with the existing
keyboard input story.

This feature defines that consumer experience: an application author can make a
control respond to the mouse (hover feedback, click activation, drag) with the
same declarative, message-driven approach they already use for keyboard input,
without writing their own hit-testing or coordinate plumbing.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Hover feedback follows the pointer (Priority: P1)

An application author builds a view containing interactive controls. As the user
moves the mouse across the window, the control currently under the pointer is
reported as "hovered", and the previously hovered control is reported as "no
longer hovered". The author can render hover affordances (e.g. a highlighted
button) purely as a function of this state.

**Why this priority**: Hover is the foundational pointer interaction and the
minimal proof that raw coordinates are correctly mapped to the right control. It
is independently demonstrable with no dependency on press/click/drag, and it
exercises the full coordinate → hit-test → control-identity → message path that
every other pointer interaction reuses.

**Independent Test**: Drive a known view with a scripted sequence of pointer-move
coordinates and assert that the reported hovered control changes to exactly the
control whose bounds contain each point, and that enter/leave transitions fire in
the correct order with no duplicate or skipped transitions.

**Acceptance Scenarios**:

1. **Given** a view with two side-by-side buttons, **When** the pointer moves from
   over button A to over button B, **Then** the runtime reports hover-leave for A
   and hover-enter for B, in that order, and the hovered target is B.
2. **Given** the pointer is over a control, **When** the pointer moves to empty
   space within the window (over no control), **Then** the runtime reports
   hover-leave for that control and no control is hovered.
3. **Given** the pointer is over a control, **When** the pointer leaves the window
   entirely, **Then** the runtime reports hover-leave and no control is hovered.

### User Story 2 - Click activates a control (Priority: P2)

An application author marks a control as clickable and supplies a message to emit
on click. When the user presses and releases the mouse over the same control, the
author's message is dispatched into the MVU update loop — the same outcome as
activating that control from the keyboard.

**Why this priority**: Click activation is the single most common mouse
interaction and delivers the headline value ("buttons work with the mouse"). It
builds directly on the hit-testing established by P1 and reaches parity with the
existing keyboard activation path.

**Independent Test**: Script a press at a point inside a clickable control
followed by a release at a point inside the same control, and assert the control's
click message is dispatched exactly once; script a press inside the control
followed by a release outside it and assert no click message is dispatched.

**Acceptance Scenarios**:

1. **Given** a clickable button, **When** the pointer is pressed and released
   while over that button, **Then** the button's click message is dispatched
   exactly once.
2. **Given** a clickable button, **When** the pointer is pressed over the button
   but released after moving off it, **Then** no click message is dispatched and
   the button returns to its un-pressed state.
3. **Given** a focusable control, **When** the pointer presses it, **Then** that
   control becomes the focused control (so subsequent keyboard input is routed to
   it), consistent with the existing focus model.

### User Story 3 - Drag interactions (Priority: P3)

An application author builds a control whose value changes as the user drags
(e.g. a slider thumb, a scrollbar, a draggable list item). When the user presses
on the control and moves the pointer beyond a small movement threshold while held,
a drag begins; subsequent moves report the current drag position; releasing ends
the drag. The author updates model state from this drag lifecycle.

**Why this priority**: Drag enables richer controls but is needed by fewer
consumers than hover and click, and it depends on the press/release tracking
established by P2. It is valuable but can ship after the foundation.

**Independent Test**: Script press-then-move-past-threshold-then-move-then-release
and assert a single drag-start, ordered drag-move updates carrying the running
position, and a single drag-end; script press-then-tiny-move-then-release (below
threshold) and assert it is treated as a click, not a drag.

**Acceptance Scenarios**:

1. **Given** a draggable control, **When** the pointer presses it and moves beyond
   the drag threshold while held, **Then** a drag begins reporting the start
   position and is updated with each subsequent move position until release.
2. **Given** an in-progress drag, **When** the pointer is released, **Then** the
   drag ends exactly once and the final position is reported.
3. **Given** an in-progress drag, **When** the pointer leaves the window or focus
   is lost while still held, **Then** the drag is cancelled deterministically and
   the control returns to a consistent (non-dragging) state.

### User Story 4 - Secondary-button (context) interaction (Priority: P3)

An application author wants a control to respond to a right-click (e.g. to open a
context action) differently from a left-click. When the user presses and releases
the secondary button over a control, the author receives a secondary-button click
distinguishable from a primary-button click, addressed to the same control.

**Why this priority**: Per-button discrimination unlocks context interactions but
is needed by fewer consumers than primary-button click; it reuses the same
press/release/hit-test machinery as P2 with button identity added.

**Independent Test**: Script a secondary-button press and release over a control
and assert a secondary-button click is reported for that control and that no
primary-button click is reported; script a primary-button press/release and assert
the converse.

**Acceptance Scenarios**:

1. **Given** a control with distinct primary and secondary handlers, **When** the
   secondary button is pressed and released over it, **Then** the secondary-button
   click is dispatched and the primary-button click is not.
2. **Given** the primary and secondary buttons are pressed in overlapping order,
   **When** each is released, **Then** each button's press/release is tracked
   independently and resolved to the correct per-button outcome.

### User Story 5 - Wheel / scroll (Priority: P3)

An application author builds a scrollable view. When the user rolls the mouse
wheel while the pointer is over the view, the author receives a scroll event
carrying the scroll delta, addressed to the control under the pointer, and updates
the scroll position from it.

**Why this priority**: Scrolling is a common expectation for content views but is
orthogonal to hover/click/drag and depends on a new host wheel event; it can land
alongside or just after the press/click foundation.

**Independent Test**: Script wheel deltas at a point over a scrollable control and
assert scroll events carrying the expected delta are reported for that control;
script wheel deltas over empty space and assert no control receives them (or a
miss diagnostic is emitted per FR-010).

**Acceptance Scenarios**:

1. **Given** a scrollable control under the pointer, **When** the wheel is rolled,
   **Then** a scroll event carrying the signed delta is dispatched to that control.
2. **Given** the pointer is over no control, **When** the wheel is rolled, **Then**
   no control receives a scroll interaction.

### Edge Cases

- **Overlapping controls**: when controls overlap at a point, the topmost
  (front-most in paint order) control receives the interaction; ties resolve
  deterministically.
- **Hidden / collapsed controls**: controls that are not visible are never hover,
  press, click, or drag targets.
- **Press then release on different controls**: press on A, release on B → no
  click on either; A's pressed state is cleared.
- **Pointer leaves window mid-interaction**: a held press whose pointer exits the
  window must resolve to a deterministic cancel rather than a dangling pressed
  state.
- **Rapid event bursts**: a flood of move events between a press and release must
  preserve ordering (hover/press/move/release) and must not drop or reorder the
  press/release pair.
- **Out-of-date layout**: if a pointer event arrives referencing a control whose
  bounds are stale (removed/relayouted), the interaction targeting that control is
  reported as a stale-target diagnostic rather than dispatching to a wrong control.
- **Coordinate space / scaling**: pointer coordinates from the host are mapped
  into the same coordinate space used for control bounds, including any device
  pixel scaling and pixel-snap policy already applied to layout.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The framework MUST expose a public, consumer-facing way to subscribe
  a control (or the application) to pointer interactions — hover, press, release,
  and click — using the same declarative, message-returning style consumers
  already use for keyboard input.
- **FR-002**: The framework MUST map raw host pointer coordinates to the specific
  control under the pointer using the existing post-layout bounds, honoring paint
  order for overlaps and excluding non-visible controls.
- **FR-003**: On pointer movement, the framework MUST maintain a single current
  hover target and MUST emit ordered hover-leave (for the prior target) and
  hover-enter (for the new target) transitions; moving within the same control
  MUST NOT emit redundant enter/leave transitions.
- **FR-004**: On pointer press, the framework MUST record the pressed control and
  MUST move input focus to that control when it is focusable, consistent with the
  existing focus model shared with keyboard input.
- **FR-005**: On pointer release, the framework MUST dispatch a click for the
  target control if and only if the release occurs over the same control that
  received the press; otherwise it MUST clear the pressed state without
  dispatching a click.
- **FR-006**: The framework MUST support a drag lifecycle (begin, position
  updates, end) initiated by a press followed by movement beyond a defined
  movement threshold while held, and ending on release. A press/release with
  movement at or below the threshold MUST be treated as a click, not a drag.
- **FR-007**: The framework MUST resolve in-progress interactions (press, drag) to
  a deterministic cancelled state when the pointer leaves the window or input focus
  is lost, leaving no dangling pressed/dragging state.
- **FR-008**: The framework MUST preserve event ordering within a single
  interaction sequence (hover → press → move(s) → release/click) and MUST NOT drop
  or reorder the press/release pair under rapid event bursts.
- **FR-009**: Pointer interactions MUST be deterministic and replayable from a
  recorded sequence of pointer events, consistent with how the existing input
  systems support recorded-event replay, so that tests and evidence can assert
  exact interaction outcomes.
- **FR-010**: When a pointer event cannot be resolved to a current control (e.g.
  stale or removed target, hit-test miss inside the window), the framework MUST
  emit a diagnostic rather than dispatching the interaction to an unrelated
  control.
- **FR-011**: The framework MUST distinguish pointer-originated interactions from
  keyboard/text/focus-originated ones in the events it surfaces to consumers, so a
  consumer can tell a click apart from a keyboard activation.
- **FR-012**: The framework MUST allow a consumer to ignore/opt out of pointer
  events (i.e. pointer support MUST NOT force pointer handling onto applications
  that do not want it); existing keyboard-only applications MUST continue to behave
  unchanged.
- **FR-013**: The framework MUST report which mouse button (primary, secondary,
  and middle) originated a press, release, or click, so consumers can respond
  differently per button. Hover, click, and drag interactions MUST be expressible
  per button; in particular, a secondary-button (right-button) press/release over
  a control MUST be surfaced to consumers so they can drive context-style actions.
- **FR-014**: The framework MUST surface mouse wheel / scroll interactions to
  consumers as a control-addressed event carrying the scroll delta (and axis where
  applicable), hit-tested to the control under the pointer, so consumers can drive
  scrollable views.

> Interacting / conflicting requirements: FR-005 (click = press+release on same
> control) and FR-006 (drag begins on movement past a threshold) can both apply to
> one press → move → release sequence. Resolve consistently: movement strictly
> beyond the threshold while held commits the sequence to a drag (no click on
> release); movement at or below the threshold keeps it a click candidate. A
> sequence cannot produce both a click and a drag.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: New public surface is expected primarily in
  `FS.Skia.UI.Controls` (the pointer-interaction front door over the existing
  `ControlRuntime` hover/press/drag model) and its MVU adapter
  `FS.Skia.UI.Controls.Elmish` (bridging pointer runtime effects to `Cmd<'msg>`).
  `FS.Skia.UI.Layout` already provides `hitTestComputed` and is expected to be
  consumed, not changed. `FS.Skia.UI.SkiaViewer` (host) already publishes
  `PointerMoved/Pressed/Released`; because button discrimination (FR-013) and
  wheel/scroll (FR-014) are **in scope**, the host `ViewerEvent` contract **will**
  be extended to carry mouse-button identity on press/release and to emit a new
  wheel/scroll event. No package identities are added or removed; no Charts/legacy
  package migration is involved. (Final package split is a planning decision.)
- **Public contract impact**: Expected to add/extend public `.fsi` signatures for
  the pointer-interaction surface (consumer-facing pointer event/attachment types
  and the runtime/effect bridge), and to add sample contracts demonstrating mouse
  interaction. Surface baselines and per-package `.fsi` snapshots will move. This
  is a **consumer-contract change** and routes to the escalated maintainer-verify
  path.
- **State workflow impact**: Pointer interactions flow through the MVU update loop
  as messages/effects (hover/press/release/click/drag lifecycle), extending the
  existing control-runtime reducer and its Elmish command bridge. No new external
  I/O; determinism and recorded-event replay are required (FR-009).
- **Layout/rendering impact**: Consumes existing layout bounds + `hitTestComputed`
  for hit-testing; coordinate mapping must honor existing device-pixel scaling and
  pixel-snap policy. Hover/press/drag state may drive visual affordances in
  samples, but no new rendering primitives are required. Screenshots/visual proof
  for the mouse sample fall under evidence mode's render-only honesty rules.
- **Evidence obligations**: Deterministic interaction tests (scripted pointer
  sequences → asserted hover/click/drag outcomes and ordering), surface-baseline
  updates for any new `.fsi`, and a runnable sample demonstrating mouse-driven
  hover/click/drag with captured readiness evidence. Real evidence paths under
  `specs/075-mouse-input-events/readiness/`.
- **Unsupported scope**: No gesture recognition beyond click/drag (e.g.
  pinch/rotate/multi-touch), no touch/stylus pressure modeling, no OS-level
  cursor-shape management or custom cursor art, no drag-and-drop across windows or
  to other applications, and no platform-specific pointer acceleration tuning.
  Wheel/scroll (FR-014) and multi-button/secondary-button interactions (FR-013)
  ARE in scope; however, full context-menu UI (rendering a menu) is the
  consumer's responsibility — the framework surfaces the secondary-button event,
  not a menu widget. Horizontal/precision (high-resolution) wheel handling beyond
  a signed delta per axis is out of scope.
- **Build-target impact**: Run `Route` first; because this is a consumer-contract
  change it is expected to escalate to the serialized maintainer-verify path
  (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
  `EvidenceGraph`, `EvidenceAudit`). New surface baselines may require
  `RefreshSurfaceBaselines` / per-package surface regeneration. No routing-rule
  change is anticipated unless a new package or gate is introduced during planning.

## Success Criteria *(mandatory)*

- **SC-001**: Given a known view and a scripted sequence of pointer coordinates, a
  consumer can determine the hovered control purely from framework-reported state,
  and that control matches the control whose bounds contain the point in 100% of
  scripted cases.
- **SC-002**: A press-and-release over the same clickable control dispatches its
  click outcome exactly once; a press-and-release over different controls
  dispatches no click — verified across the scripted acceptance scenarios with
  zero false clicks and zero missed clicks.
- **SC-003**: A drag interaction produces exactly one begin and one end with
  ordered position updates in between, and a sub-threshold press/release never
  produces a drag — verified deterministically.
- **SC-004**: Interrupted interactions (pointer leaves window / focus lost mid
  press or drag) always resolve to a consistent non-pressed, non-dragging state
  with no dangling interaction, in 100% of interruption scenarios.
- **SC-005**: The same recorded pointer-event sequence replayed twice yields
  identical reported interaction outcomes (determinism), enabling reproducible
  evidence capture.
- **SC-006**: An existing keyboard-only application, rebuilt against this feature,
  exhibits unchanged behavior (no pointer handling is forced on it).
- **SC-007**: A consumer can attach mouse hover/click/drag handling to a control
  without writing any coordinate math or hit-testing themselves — demonstrated by a
  sample whose application code references only control-level interaction messages.
- **SC-008**: A secondary-button press/release over a control yields a
  secondary-button outcome distinguishable from a primary-button outcome, and each
  button's interaction is tracked independently — verified with zero cross-button
  misattribution across the scripted scenarios.
- **SC-009**: A wheel roll while the pointer is over a scrollable control reports a
  scroll event carrying the correct signed delta addressed to that control, and a
  wheel roll over no control reports no scroll interaction — verified
  deterministically.

## Assumptions

- **Parity with keyboard input is the design target**: the pointer story mirrors
  the existing declarative, message/effect-based keyboard model rather than
  introducing a different interaction paradigm.
- **Reuse over rebuild**: the feature builds on the already-present host pointer
  events, the `ControlRuntime` hover/press/drag model, and `Layout.hitTestComputed`;
  it does not re-implement hit-testing or a new event pump.
- **Multi-button + wheel are in scope** (per clarification): the framework
  discriminates primary/secondary/middle buttons and surfaces wheel/scroll. The
  primary-button hover/press/click/drag path (already modeled by the runtime) is
  the foundation the other buttons and the wheel event build on. Only one pointer
  device is assumed (no multi-touch / simultaneous pointers).
- **Click = press+release on the same control** is the activation definition, with
  a small movement threshold separating click from drag (FR-006).
- **Determinism and replay** are required because the project's evidence/readiness
  model depends on reproducible, asserted outcomes.
- **Coordinate mapping** reuses the existing device-pixel scaling and pixel-snap
  policy already applied to layout; no new DPI model is introduced.
- **Double-click / multi-click counting** is assumed out of v1 unless raised;
  consumers can derive it from click timing if needed (revisit during planning).

## Key Entities

- **Pointer interaction event** (consumer-facing): a control-addressed interaction
  surfaced to the application — hover-enter/leave, press, release, click, and drag
  lifecycle, plus wheel/scroll — carrying the target control identity, the pointer
  position, the pointer origin (to distinguish from keyboard), the originating
  mouse button (primary/secondary/middle, FR-013), and, for scroll events, the
  wheel delta (FR-014).
- **Hover state**: the single control currently under the pointer (or none).
- **Press state**: the control that received the active press (the click/drag
  candidate) until release or cancel.
- **Drag**: an in-progress drag with its originating control, start position, and
  current position, from begin to end/cancel.
- **Hit-test result**: the mapping from a window pointer coordinate to the
  front-most visible control whose post-layout bounds contain that point (or none).
