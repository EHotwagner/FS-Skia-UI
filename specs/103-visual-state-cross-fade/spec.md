# Feature Specification: True Visual-State Cross-Fade

**Feature Branch**: `103-visual-state-cross-fade`
**Created**: 2026-06-11
**Status**: Draft
**Input**: User description: "create the next part" — selected the next rung of
`docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md`, namely
**R6 (True visual-state cross-fade; completes R4)** from §11.3.

## Context & Source

This feature implements **R6** from the roadmap's second-pass audit
(`docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md`, §11.3).
R6 is one of three follow-ups (R6/R7/R8) the second-pass audit added after R1–R5
(features 096–100) landed. R7 shipped as feature 101 and R8 as feature 102. §11.6
sequenced the two low-risk rungs ({R7, R8}) first and R6 — **the one behavior-changing
rung** — last, so with R7 and R8 merged, **R6 is the final remaining rung** of the
controls-architecture-evolution roadmap (maintainer decision, 2026-06-11). There is no
successor after R6.

Unlike R7 and R8 (which hardened and documented without moving a parity row), R6 is the
**only** follow-up that introduces **new visible behavior**: it moves the per-control
animation parity row from "live clock + opacity fade-in only" to "live clock + multi-
channel style cross-fade". It therefore carries a golden-evidence budget for the stable
endpoints (see Evidence obligations) while treating mid-flight frames as animation, not
golden.

### The problem in one paragraph

R4 (feature 099) shipped a real, deterministic, host-advanced animation clock and a
sample-on-paint seam, and its `AnimationClock` type advertises a **multi-channel** paint
carrier (opacity / transform / **color** tweens, via feature-073's `Animation` shape).
But the only channel actually driven on a visual-state transition is **opacity**:
`updateClockForState` builds a single fixed `fadeAnimation` — a uniform 150 ms `EaseOut`
ramp of opacity from `0.0` to `1.0` — and `sampleOnPaint` overlays only that opacity
channel. So a `Normal → Hover`/`Focused` transition fades the **new** (already
Hover/Focused-styled) appearance in **from transparent**; it never interpolates the
resolved paint channels (foreground / background / accent) **between** the prior and the
next state. The color channel the `AnimationClock` doc advertises is never populated.
R4's exit criterion ("animates rather than snapping") is thus met only in the literal
opacity sense, and the doc overstates what the type drives. R6 closes this single genuine
behavior gap by cross-fading the **prior** state's painted snapshot (fading out) under the
**next** state's painted snapshot (fading in) — both driven by the existing opacity tween
through `Animation.applyAt` — so the displayed paint interpolates mid-flight, and by
reconciling the `AnimationClock` doc to the channels actually driven. (Planning established
that `Animation.applyAt` never samples the `Color` tween, so the cross-fade is realized by
the two-snapshot composite rather than a standalone color tween — see plan.md / research.md.)

### Root cause (grounding for implementers)

- `fadeAnimation` (`src/Controls/RetainedRender.fs:~94`) constructs a fixed opacity-only
  `Animation` (`Start = startOpacity; End = 1.0; EaseOut`). No `Color` (or other) tween is
  ever set. `FS.Skia.UI.Scene.Animation` carries a `Color: Tween<Color> option` channel, but
  `Animation.applyAt` samples **opacity/transform only and never reads that `Color` channel**
  (it is counted by `isSettled` but never recolors the scene) — so the advertised color
  capability has no live path even when set (established in research.md).
- `updateClockForState` (`src/Controls/RetainedRender.fs:~123`) detects a state change and
  builds that fixed opacity ramp from the current sampled opacity (or `0.0`), with **no
  knowledge of the resolved style of either endpoint**.
- `sampleOnPaint` (`src/Controls/RetainedRender.fs:~153`) wraps the identity's static own
  paint through `applyAt` at the clock's `Elapsed` — so it overlays whatever channels the
  `Animation` carries, but today that is opacity only.
- The `AnimationClock` type doc (`src/Controls/RetainedRender.fsi:~40–51`) advertises a
  press-tint / focus-ring **color** capability that is never driven.

### Permanent non-goals (unchanged by R6)

R6 holds every permanent non-goal of the architecture-evolution program
(`[[architecture-evolution-no-redesign]]`): no observable property graph, no dependency
properties, no selector engine, no template engine, no XAML/data-binding. Crucially, R6
animates a **closed, token-derived channel set** — exactly the channels `Style.resolve`
already produces (token-derived paint + opacity) — and adds **no** general per-property
animation surface. It does not introduce a consumer-facing animation API, transition
authoring, easing/duration knobs, or per-property tween configuration. The transition
duration and easing remain the framework defaults R4 established.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A state transition visibly cross-fades its colors (Priority: P1)

A user hovers (or focuses) an interactive control whose Hover/Focused style differs from
its Normal style in a token-derived paint channel (e.g. background or accent). Instead of
the target appearance fading in from transparent over a uniform opacity ramp, the control's
**resolved paint channels interpolate** from the prior state's colors to the new state's
colors over the transition — a genuine cross-fade. At the end of the transition the control
is byte-identical to the statically snapped render of the new state.

**Why this priority**: This is the one observable behavior R6 exists to deliver and the
single parity row it moves. It directly realizes the multi-channel transition the
`AnimationClock` type already advertises. Without it the feature has no point.

**Independent Test**: Drive a control through `Normal → Hover` (or `Normal → Focused`) with
a fixed sequence of injected frame deltas on the retained render path; sample an
intermediate frame and assert at least one **color** channel value lies strictly between
the prior and next resolved-style endpoints (not merely a transparent→opaque opacity ramp
of the new appearance).

### User Story 2 - At-rest and settled output is unchanged (Priority: P1)

A maintainer relies on the invariant that a control not mid-transition renders exactly as
it did before any animation feature existed, and that a finished transition is
indistinguishable from a static render of the new state. After R6, identity-at-rest still
emits **no** animation attribute and is byte-identical to the pre-R4 static render, and the
**final** transition frame is byte-identical to the statically snapped render of the
settled state.

**Why this priority**: R6 is the only behavior-changing rung; the program's whole
credibility rests on the "byte-identical at the stable points" contract that R1–R5
established. Breaking at-rest or final-frame identity would be a regression, not a feature.

**Independent Test**: With no transition in flight, assert the rendered fragment is
byte-identical to the static render and carries no animation attribute. Advance a
transition clock past its duration with a large injected delta and assert the resulting
frame is byte-identical to the statically snapped render of the new state.

### User Story 3 - The advertised channels and the driven channels agree (Priority: P2)

A contributor reads the `AnimationClock` type doc to learn what a live clock can animate.
After R6, the doc names exactly the channels the implementation actually drives on a
transition: any channel it advertises (opacity + the token-derived paint/color channels) is
genuinely driven, and any channel that is intentionally out of scope is removed from the
doc rather than left as an unfulfilled promise.

**Why this priority**: Doc↔behavior agreement is a first-class deliverable of this program
(it is the entire substance of R8). R6 must not leave the same doc-overstates-behavior gap
it set out to close. It is P2 because it follows mechanically once US1 lands.

**Independent Test**: Read `src/Controls/RetainedRender.fsi`; confirm every channel the
`AnimationClock` doc claims is exercised by a test in this feature, and no claimed channel
is undriven.

### Edge Cases

- **Mid-flight retarget.** A second state change while a transition is in flight (e.g.
  `Normal → Hover → Focused`, or `Hover → Normal` before settling) must retarget without
  snapping back to the **original stale** endpoint, mirroring R4's existing mid-flight opacity
  retarget. The fade-out layer is re-seeded from the **previous target's** own-scene snapshot
  (the layer that was fading in becomes the one now fading out) and `Elapsed` resets — the
  vector-scene analogue of "continue from what is displayed" (INV-5).
- **No channel differs.** When the prior and next resolved styles are identical in every
  animated channel (the transition changes no token-derived paint), the tween is a no-op in
  those channels; behavior collapses to the existing settled/at-rest result with no spurious
  repaint.
- **Held state.** A state that is entered and then held (no further change) remains a `Keep`
  after the clock settles — the `VisualStateValue` equality case that makes a held state a
  scoped repaint (feature 099) stays intact; R6 does not cause a held state to re-fire or
  repaint every frame.
- **Return to Normal.** A settled return-to-`Normal` clock is still **dropped** so the
  identity returns to byte-identical at-rest output (the FR-003 vs FR-005 interaction R4
  resolved), now also dropping any color channel it carried.
- **Non-positive / very large injected delta.** Determinism under injected deltas is
  preserved: a non-positive delta is a no-op (never rewinds), and a delta past the duration
  settles canonically at the new endpoint (no overshoot in any channel).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: On a detected visual-state change, the system MUST resolve **both** the prior
  and the next visual state's style via the existing `Style.resolve` resolver and build the
  transition tween over the channels that **differ** between them — not a fixed opacity ramp
  from `0.0`.
- **FR-002**: The transition MUST make the token-derived **paint** (foreground / background /
  accent as produced by `Style.resolve`) **interpolate** between the prior and next state
  mid-flight by compositing the two states' painted own-scene snapshots — the prior fading out
  under the next fading in — each driven by the existing opacity tween through
  `Animation.applyAt`, in addition to the existing opacity behavior. (`Animation.applyAt` does
  not itself sample a `Color` tween; the paint cross-fade comes from the two-snapshot composite
  — see FR-009.)
- **FR-003**: The animated channel set MUST be **closed** to exactly the channels
  `Style.resolve` already produces (token-derived paint + opacity). The system MUST NOT
  introduce any general per-property animation surface, consumer-facing transition authoring,
  or configurable easing/duration knobs.
- **FR-004**: Identity-at-rest MUST remain byte-identical to the pre-R4 static render and
  MUST emit no animation attribute (FR-005 of feature 099 preserved).
- **FR-005**: The **final** transition frame (clock settled at the duration) MUST be
  byte-identical to the statically snapped render of the new (now-stamped) state, for every
  animated channel including color.
- **FR-006**: The clock MUST remain deterministic under injected frame deltas — identical
  delta sequences produce identical sampled frames; there is no wall-clock dependency, no
  overshoot past an endpoint, and a non-positive delta never rewinds.
- **FR-007**: A mid-flight state change MUST retarget without snapping back to the **original
  stale** endpoint: it re-seeds the fade-out layer from the **previous target's** own-scene
  snapshot (the layer that was fading in becomes the one now fading out) and resets `Elapsed`,
  generalizing R4's opacity retarget. (Because scenes are vector lists, not bitmaps, the prior
  layer is the previous target snapshot rather than a rasterized mid-blend; this preserves "no
  snap to a stale endpoint" — see INV-5 in data-model.md.)
- **FR-008**: A held (unchanged) state MUST remain a `Keep` once settled, and a settled
  return-to-`Normal` clock MUST still be dropped so the identity returns to byte-identical
  at-rest output — preserving the scoped-repaint and at-rest invariants of feature 099.
- **FR-009**: The `AnimationClock` type doc in `src/Controls/RetainedRender.fsi` MUST name
  exactly the channels the implementation drives: any advertised channel MUST be genuinely
  driven by a transition, and any channel that is intentionally out of scope MUST be removed
  from the doc. The doc and the driven channels MUST agree (closes the doc↔behavior gap R6
  itself must not reopen).

> Interacting / conflicting requirements: FR-002 (interpolate paint mid-flight) vs FR-004
> (byte-identical at rest) and FR-005 (byte-identical final frame) pull in opposite
> directions during the run but agree at the endpoints. Resolution: the cross-fade is an
> **assembly-time paint overlay over the static fragment**, gated so that **only mid-flight
> frames differ** from the static render; at rest no animation attribute is emitted and at
> settle the overlay collapses to identity — exactly as R4's opacity overlay does today. The
> stable points are byte-identical; mid-flight frames are animation, not golden.
>
> FR-002 (drive the color channel) vs FR-003 (closed, no open property animation): resolved
> by driving **only** the channels `Style.resolve` already emits. If a channel the
> `AnimationClock` doc advertises is decided to be out of scope rather than driven, FR-009
> requires trimming the doc to match rather than adding a new animation surface to fulfill it.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This section is
> *expected* to name concrete `.fsi` signatures, modules, build targets, and evidence paths.

- **Package impact**: No package identity or contents change beyond the `FS.Skia.UI.Controls`
  library source. No new package; no legacy Charts migration. The standard post-merge version
  bump + pack of the 12 packable libraries applies (`[[merge-bump-scope-libs-not-template]]`,
  `[[build-package-version-drift-gotcha]]`).
- **Public contract impact**: The **public** `runInteractiveApp` / consumer surface is
  **unchanged**. The change is confined to the `RetainedRender` internals; the only `.fsi`
  edit is the **doc-comment** on the `internal AnimationClock` type
  (`src/Controls/RetainedRender.fsi`) to reconcile advertised vs driven channels (FR-009).
  No public signature changes. (Note: any `src/Controls/**` edit escalates `Route` to the
  `controls-public-surface` gate set regardless of `.fsi` delta —
  `[[feature-101-layout-dirty-set-guard]]`, `[[feature-102-doc-narrowing-reconciliation]]`.)
- **State workflow impact**: No change to commands, effects, subscriptions, or interpreter
  behavior. The host tick → `advance` → `sampleOnPaint` seam from feature 099 is reused; only
  the **content** of the tween built on a transition changes (paint channels added).
- **Layout/rendering impact**: Rendering output **changes mid-transition only**: a
  `Normal → Hover`/`Focused` transition now interpolates token-derived paint channels.
  At-rest and final-frame output are byte-identical (FR-004/FR-005). No layout change. No
  unsupported-environment diagnostic change.
- **Evidence obligations**: Real evidence under `specs/103-visual-state-cross-fade/readiness/`:
  (1) an **at-rest byte-identity** proof (rendered fragment == static render, no animation
  attribute); (2) a **final-frame == snapped-static** byte-identity proof for the animated
  channels; (3) a **mid-flight color-interpolation** proof (an intermediate sampled frame whose
  color channel lies strictly between the two resolved-style endpoints); (4) a **determinism**
  proof under a fixed injected-delta sequence (property-tested with a fixed delta sequence, as
  R4 does — repo has no `testProperty`, use `Check.One`, `[[feature-099-live-animation-clock]]`);
  (5) `evidence-graph.md` + `evidence-audit.md` with a verdict token (0 synthetic). Mid-flight
  frames are treated as animation, not golden (no golden churn budget for intermediate frames).
- **Unsupported scope**: Out of scope — any consumer-facing animation/transition authoring
  API; configurable easing or duration; transform-channel animation on state change (opacity +
  token paint only); animating channels `Style.resolve` does not produce; enabling default
  arrow-key routing for `Chart`/`Graph`/`Progress` (an R8-noted, separate behavior decision);
  the deferred R6 color channel becoming an open property system. No release, platform, or
  distribution change.
- **Build-target impact**: No build-target definitions change. Validation runs the gates
  `Route` prints for a `controls-public-surface`-escalated change (`Dev` +
  controls-public-surface set), then `EvidenceGraph` and `EvidenceAudit`. No `TemplateCheck`
  / `DependencyReport` / `GeneratedGuidanceCheck` rule change is required.

## Success Criteria *(mandatory)*

- **SC-001**: A `Normal → Hover` (and `Normal → Focused`) transition that changes a
  token-derived paint channel shows that channel's displayed value at an intermediate frame
  lying **strictly between** the prior and next resolved-style endpoint values — an
  interpolated color change, not only a transparent→opaque opacity fade-in of the new
  appearance.
- **SC-002**: With no transition in flight, 100% of rendered fragments are byte-identical to
  the corresponding static render and carry no animation attribute.
- **SC-003**: The final transition frame (clock advanced past its duration) is byte-identical
  to the statically snapped render of the new state in **every** animated channel.
- **SC-004**: Replaying any fixed sequence of injected frame deltas produces an identical
  sequence of sampled frames across runs (deterministic; no wall-clock influence).
- **SC-005**: Every channel named in the `AnimationClock` type doc is exercised by a test in
  this feature, and no doc-advertised channel is left undriven (doc↔behavior agreement).
- **SC-006**: The existing controls and Elmish test suites remain green, and a held state
  still produces a single scoped repaint (no per-frame repaint of a settled state).

## Assumptions

- The recommended sequence ({R7, R8} before R6) is followed and both R7 (feature 101) and R8
  (feature 102) are merged; R6 is built last and depends on R1 (the state-change trigger,
  feature 096) and R4 (the live clock/seam, feature 099), both shipped.
- The transition duration and easing remain the framework defaults established by R4; R6 adds
  channels to the tween but introduces no duration/easing surface.
- `Style.resolve` is the single source of the closed channel set to animate; "token-derived
  paint" means foreground / background / accent (and opacity) as that resolver produces them.
- Feature-073's `Animation` carrier exposes a `Color` tween channel, but `Animation.applyAt`
  does **not** sample it (opacity/transform only); R6 therefore realizes the paint cross-fade
  by compositing the prior/next painted own-scene snapshots under the **opacity** tween rather
  than populating a standalone color channel, and reconciles the `AnimationClock` doc to match
  (FR-009).
- Mid-flight frames are not gated as golden evidence; only the two stable points (at-rest and
  final-frame) carry byte-identity obligations.

## Key Entities

- **AnimationClock** (internal): the per-identity clock carrying the feature-073 `Animation`
  (its **opacity** tween is the live channel), accumulated injected `Elapsed`, the `Target`
  `VisualState`, and a **new `From : Scene list`** field — the prior state's painted own-scene
  snapshot captured at transition start. R6 cross-fades by compositing `From` (fading out)
  under the next own-scene (fading in) via the opacity tween; it does **not** populate a
  standalone `Color` tween (`Animation.applyAt` never samples one).
- **Two-layer snapshot composite**: the cross-fade is realized by compositing the prior
  state's painted own-scene snapshot (faded out) under the next state's snapshot (faded in),
  each via the opacity tween — replacing today's fixed opacity-only fade-in. (The
  planning-time "style-delta `Color` tween" was rejected: `applyAt` never samples `Color` and
  one tween cannot carry multi-channel paint — research.md Option C.)
- **VisualState / VisualStateValue**: the stamped per-control state driving transitions and the
  equality case (feature 099) that keeps a held state a scoped repaint.
