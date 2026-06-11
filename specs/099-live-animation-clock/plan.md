# Implementation Plan: Animation Clock on Retained Identity (R4)

**Branch**: `099-live-animation-clock` | **Date**: 2026-06-11 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/099-live-animation-clock/spec.md`

## Summary

E2 (features 091 + 092) built a per-retained-identity animation **slot** —
`RetainedUiState.Animation` (`src/Controls/RetainedRender.fs:27`) — and carries it
across frames via the `liveIds` GC filter, but **nothing in the running host ever
writes or advances it**. The only test that exercises it hand-seeds the clock and
labels itself `PRECONDITION (no animation seam exists yet)`
(`tests/Elmish.Tests/Feature092LiveSurvivalTests.fs:54,98–105`). So E2's exit
criterion — "an in-flight animation survives an unrelated state change" — is true
only as a carried value, never as live behavior.

R4 builds the missing **host animation seam** that makes per-control animation run
live, deterministically, on injected frame deltas. Three architecture-preserving
moves:

1. **Advance the clock from the host tick.** Wrap the host loop's existing per-frame
   delta (`InteractiveAppHost.Tick: TimeSpan -> 'msg option`, today unused for
   animation) so that, each frame, every live per-identity animation in
   `retained.Value.StateByIdentity` advances by the **injected delta** (never
   `Date.now`).
2. **Couple the transition trigger to R1.** When R1's bridge
   (`ControlRuntime.deriveVisualState`/`applyRuntimeVisualState`, feature 096) flips a
   control's derived `VisualState` (e.g. `Normal → Hover`, gaining `Focused`), **start
   or retarget** a tween on that retained identity so the style transition *animates*
   instead of snapping.
3. **Sample on paint, byte-identical at rest.** Each frame, sample every active
   per-identity animation at its current elapsed value via feature-073's `Animation`
   primitives and feed the sampled paint-level value (opacity / transform / color) into
   that identity's paint, scoped to its own subtree. An identity with **no active
   tween** emits **no** animation output and renders byte-identical to the pre-R4
   golden, preserving E2's `RecomputedNodeCount = 0` at-rest invariant.

Carry-across-frames and GC reuse the existing `RetainedId`-keyed `StateByIdentity`
map and its `liveIds` filter — **no parallel identity scheme**. The feature-073 Scene
animation primitives (`Tween`/`Animation`/`AnimationState`/`applyAt`) are **reused,
not modified**. No data binding, dependency properties, CSS selectors, template
engine, or consumer-facing animation authoring API.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: existing only — `FS.Skia.UI.Controls`,
`FS.Skia.UI.Controls.Elmish`, and (reused, unchanged) `FS.Skia.UI.Scene` feature-073
animation. No new package, dependency, or DTCG token.
**Testing**: Expecto + FsCheck (`tests/Controls.Tests`, `tests/Elmish.Tests`);
deterministic delta-driven integration through the real `runInteractiveApp` host seam;
FAKE escalated six-target order.
**Target Platform**: Windows and Linux. Paint-level animation (opacity/transform/color)
only — **no layout geometry animation**, so R2 incremental measure and the
scoped-repaint reduction are preserved; at-rest Vulkan/Skia output stays byte-identical.
**Change Tier**: **Tier 1 (contracted change)** — the carried `RetainedUiState.Animation`
slot type is generalized to carry the feature-073 multi-channel paint shape (an
**internal** `.fsi` change in `src/Controls/RetainedRender.fsi`), which escalates the
diff to the controls-public-surface route. The **public** `runInteractiveApp` /
`InteractiveAppHost` surface is **unchanged** (the seam is internal host wiring driven
by the already-present `Tick` delta). `Route` against the actual diff is the authority.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: N/A — no `.template.config/template.json` or command-surface
  change. R4 is framework-internal host wiring in two existing `src/**` packages plus a
  reused Scene primitive; generated consumers gain live animation transparently with no
  API rename. Template pins are refreshed only on merge by the standard all-packable-libs
  version-bump flow, not by this plan.
- **Dependency impact**: N/A — no dependency change. No edit to
  `Directory.Packages.props`, `docs/dependencies.md`, generated template inclusion, or
  `DependencyReport` coverage; the change uses only already-referenced packages
  (feature-073 Scene animation is already on the `FS.Skia.UI.Scene` surface).
- **Command-surface impact**: No new gate and no `build.fsx`/`scripts/build/**` change.
  Because a `src/Controls/**/*.fsi` line changes (the carried-slot type), `Route`
  escalates this to the serialized maintainer-verify path; run only the gates `Route`
  prints, in deterministic order, never concurrently (shared `.fake` state):
  1) `./fake.sh build -t Dev` 2) `GeneratedGuidanceCheck` 3) `TemplateCheck`
  4) `GeneratedProductCheck` 5) `EvidenceGraph` 6) `EvidenceAudit`. Surface baselines
  (api-surface + per-package `.fsi.txt`) for `FS.Skia.UI.Controls` are recaptured for the
  internal-slot type change.
- **Generated project impact**: N/A — no change to default/minimal generated contents,
  selected Controls guidance text, local skills, validation logs, placeholder/excluded-
  history scans, or generated `Dev` behavior. Live animation is consumed by the running
  host automatically; scaffold defaults are unchanged.
- **Evidence paths**: All readiness artifacts under
  `specs/099-live-animation-clock/readiness/` (real evidence; an unbridged/no-seam build
  fails the animation proofs):
  - `us1-animates-vs-snaps.md` — drive a hover/focus interaction through the **real**
    `runInteractiveApp` seam (injected deltas), capture a frame sequence, assert ≥1
    intermediate sampled appearance before the target (a no-seam build snaps and fails).
  - `us2-survival.md` — start a tween, advance frames, apply a sibling-shifting unrelated
    re-render, continue ticking; the same `RetainedId`'s clock continues from its prior
    elapsed value and **completes** with the same final result — driven entirely through
    the seam, replacing the hand-seeded `Feature092LiveSurvivalTests` PRECONDITION.
  - `us3-identity-at-rest.md` — a frame with no active tween is byte-identical to the
    pre-R4 golden and reports zero at-rest animation-output / recompute count.
  - `us3-determinism.md` — FsCheck: two runs over an identical injected-delta sequence
    yield identical sampled output; no wall-clock source consulted (≥1000 cases).
  - `us4-gc.md` — animate, then re-render with the control removed; its identity's
    animation state is absent on the next frame via the existing `liveIds` filter.
  - `scoped-repaint.md` — advancing an animating clock keeps the per-frame repaint scoped
    to its subtree; the work-reduction metric shows no whole-tree repaint/re-measure (R2
    preserved).
  - `surface-baseline.md` — recaptured api-surface + per-package `.fsi.txt` diff showing
    the carried-slot type generalization.
  - `validation-log.md` — the six-target run transcript + `EvidenceAudit` verdict.
- **`.fsi` / contract impact**: **Yes — Tier 1, but narrowly.** (1)
  `src/Controls/RetainedRender.fsi` — the **internal** `RetainedUiState.Animation` field
  type is generalized from `AnimationState<Transform> option` to the feature-073
  multi-channel paint carrier (per research §R2), with its mirror in
  `RetainedRender.fs`; an internal sample-on-paint helper may be declared. (2) The public
  `src/Controls.Elmish/ControlsElmish.fsi` `InteractiveAppHost`/`runInteractiveApp`
  surface is **unchanged** — the seam consumes the already-present `Tick` delta via
  internal wiring. (3) Feature-073 `src/Scene/Animation.fsi` is **consumed, not changed**.
  Compatibility note: the carried slot is `internal`, never a consumer surface; the only
  external effect is *more faithful* live rendering for the same `view`.
- **MVU/effect boundary**: No new public MVU surface. The clock advance and sample are
  **pure functions of the accumulated injected delta** — no `Date.now`, no randomness,
  resume-safe. The animation state lives in the host's existing `retained` ref
  (`StateByIdentity`), advanced by the wrapped `Tick` and read on paint; the consumer's
  `update`/`view : 'model -> Control<'msg>` contract and `Model`/`Msg` are untouched.
  `AnimationState.advance`/`retarget`/`Animation.applyAt` (feature 073) are the pure
  transition + sample functions; the host loop is the interpreter edge that injects the
  per-frame delta. The wrapped `Tick` still delegates to `host.Tick` so the consumer's
  own tick message is unaffected (no double-dispatch, no swallowed consumer tick).
- **Synthetic evidence**: None planned. US1/US2 headline proofs run through the **real**
  `runInteractiveApp` host seam with injected deltas (the same loop the live window uses),
  explicitly **replacing** the hand-seeded `Feature092LiveSurvivalTests` PRECONDITION with
  seam-driven survival. US3 determinism uses FsCheck-**generated** delta sequences. No
  `[S]`/`[SEH]` task is anticipated; if any arises it carries full Principle V disclosure
  at task/code/test/spec/PR surfaces.
- **Test evidence**: Failing-first semantic tests in existing projects:
  - `tests/Elmish.Tests` — animates-vs-snaps over consecutive frames through
    `runInteractiveApp`; **seam-driven survival** (rewrite of `Feature092LiveSurvivalTests`
    to drive the clock through the real seam, not `startedClock ()`); GC of a removed
    identity's clock; scoped-repaint work-reduction assertion.
  - `tests/Controls.Tests` — clock advance/sample purity and retarget-on-state-flip;
    identity-at-rest byte-identity + zero-output count; FsCheck determinism over fixed
    delta sequences (≥1000 cases); edge cases (zero/non-positive delta no-op, very-large
    delta clamps to end, retarget mid-flight from current value, return-to-`Normal`
    settles to no output).
  - Surface-baseline test recaptured for the carried-slot type change.
- **Observability**: No new diagnostics needed — the existing responds-vs-renders /
  animates-vs-snaps evidence primitive is the actionable signal (a no-seam build fails
  `us1-animates-vs-snaps.md`). The clock is internal, deterministic state; no new log
  path, report field, missing-artifact-class failure, or unsupported-environment message
  is introduced. Non-positive deltas are treated as no-ops (safe failure: no rewind), and
  a very-large delta clamps to the settled end (no overshoot).
- **Deferred scope**: Out of scope and explicitly deferred — a consumer-facing animation
  authoring API (keyframes, timelines, per-control DSL); spring/physics or easing models
  beyond the feature-073 set; **layout** geometry animation (size/position reflow); a
  general animation scheduler beyond the per-frame tick advance; full-52-control
  animation/restyle coverage (tracked with E3/R1); R5 (general navigation-key delivery).
  A reduced-motion / opt-out policy is **not required** by this feature but the design
  must not preclude one (snap = the pre-R4 path). CSS selectors, attached/dependency
  properties, lookless templates, and data binding remain permanent roadmap non-goals.

**Initial Constitution Check: PASS** — Tier 1 declared with the internal `.fsi`
generalization + baseline recapture planned (Principle I/II); pure delta-driven
advance/sample keeps the change idiomatic and avoids a hidden mutable animation registry
beyond the existing retained ref (III); the clock advance/sample is modeled as pure
transitions with the host loop as the injecting interpreter edge, consumer MVU untouched
(IV); no synthetic evidence — real-seam survival replaces the hand-seeded precondition
(V); failing-first animates-vs-snaps + determinism property tests defined (VI); non-
positive-delta no-op and large-delta clamp give explicit safe-failure behavior (VII).

## Project Structure

```
specs/099-live-animation-clock/
├── spec.md
├── plan.md                       # this file
├── research.md                   # Phase 0 — seam, carried-slot type, trigger, channels, determinism
├── data-model.md                 # Phase 1 — entities: per-identity clock, trigger, injected delta, sample
├── contracts/
│   ├── host-animation-seam.md    # Tick→advance + state-flip→retarget + carry/GC contract
│   └── sample-on-paint.md        # sample-on-paint + identity-at-rest + scoped-repaint contract
├── quickstart.md                 # Phase 1 — run the host, hover a control, observe a gradual transition
└── readiness/                    # populated during /speckit-implement (evidence paths above)
```

### Source files touched (existing only — no new files)

- `src/Controls/RetainedRender.fsi` — generalize the **internal**
  `RetainedUiState.Animation` field from `AnimationState<Transform> option` to the
  feature-073 multi-channel paint carrier (research §R2); declare any internal
  sample-on-paint helper surfaced for the test assembly.
- `src/Controls/RetainedRender.fs` —
  - mirror the carried-slot type change (`:26–28`);
  - **advance**: a total function that advances every live identity's carried clock by
    an injected `TimeSpan` delta (non-positive → no-op; clamp settled at end);
  - **retarget on state flip**: when the stamped `VisualState` for an identity differs
    from the clock's current target, start/retarget the tween from the **current sampled
    value** (no snap-to-start), using a single framework default duration + easing;
  - **sample on paint**: in the paint pass (`step`/`init`, around the carried-state
    lookup), apply each active clock's sampled value via feature-073 `Animation.applyAt`
    so the painted node reflects the in-progress value; an identity with no active tween
    emits nothing (byte-identical fast path retained);
  - carry + GC unchanged — the existing `liveIds` filter (`:363–371`) already drops the
    slot with its identity.
- `src/Controls.Elmish/ControlsElmish.fs` — wire the seam inside `runInteractiveApp`
  (`renderRetained` `:583`, `viewerHost` `:695–702`): wrap `host.Tick` so the injected
  per-frame delta advances `retained.Value`'s clocks **before** the next `renderRetained`,
  then delegate to `host.Tick` for the consumer message (no swallowed consumer tick); the
  visual-state flip is already produced by `applyRuntimeVisualState` (`:587`,`:594`) — the
  retarget reads the stamped state per identity.
- `src/Controls.Elmish/ControlsElmish.fsi` — **unchanged public surface** expected (the
  seam is internal wiring driven by the existing `Tick` field). If a host-internal value
  must cross to the test assembly, declare it `internal` here (package-surface route) and
  recapture that baseline; the public `runInteractiveApp`/`InteractiveAppHost` shape stays.
- `src/Scene/Animation.fs[i]` — **not modified** (reused: `Tween`/`Animation`/
  `AnimationState`/`applyAt`/`advance`/`retarget`/`isSettled`).
- `src/Controls/ControlRuntime.fs[i]` — **consumed, not modified** (`deriveVisualState`
  `:203`, `applyRuntimeVisualState` `:229` are the existing R1 trigger source).

### Tests touched (existing projects)

- `tests/Elmish.Tests/**` — animates-vs-snaps through `runInteractiveApp`; **rewrite**
  `Feature092LiveSurvivalTests.fs` to drive survival through the real seam (delete
  `startedClock ()` hand-seed `:55–58,98–105`); removed-identity GC; scoped-repaint.
- `tests/Controls.Tests/**` — advance/sample purity, retarget-on-flip, identity-at-rest
  byte-identity + zero count, FsCheck determinism (≥1000 cases), edge cases.

## Phase 0 — Research

See [research.md](./research.md). The spec's Assumptions/Key Entities settle the headline
choices (paint-level via the feature-073 `Animation` shape; automatic visual-state-driven;
a single framework default duration + easing; injected-delta-only clock). Phase 0
additionally settles the plan-level mechanism decisions: **(R1)** where the injected delta
enters the advance (wrap `Tick` → advance `retained.Value` before render vs a pending-delta
ref); **(R2)** the carried-slot type generalization (multi-channel feature-073 carrier vs
`AnimationState<Transform>`); **(R3)** how a `VisualState` flip becomes a tween
start/retarget (track prior stamped target per identity; retarget from current sampled
value); **(R4)** the default transition channel(s)/duration/easing; **(R5)** sample-on-paint
placement and the identity-at-rest fast path; **(R6)** determinism + non-positive/large-delta
edge handling; **(R7)** carry + GC reuse. All NEEDS CLARIFICATION resolved.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md) — the per-identity animation clock, the transition
  trigger, the injected frame delta, and the sampled animation output as data, with the
  determinism / identity-at-rest / scoped-repaint invariants.
- [contracts/host-animation-seam.md](./contracts/host-animation-seam.md) — the
  `Tick → clock-advance` and `state-flip → retarget` contract and the carry/GC reuse.
- [contracts/sample-on-paint.md](./contracts/sample-on-paint.md) — the sample-on-paint
  contract, identity-at-rest byte-identity, and the scoped-repaint guarantee.
- [quickstart.md](./quickstart.md) — run a host, hover/focus a migrated control, observe a
  gradual (non-snapping) transition with zero consumer animation code.
- Agent context: `AGENTS.md` SPECKIT marker repointed to this plan.

## Phase 2 — (planning ends here)

`/speckit-tasks` will break this into story-grouped tasks with `skillist` metadata
(`fs-skia-reconciliation` for the retained-path clock advance/sample/carry, `fs-skia-scene`
for the feature-073 animation reuse, `fs-skia-elmish` for the host `Tick` seam,
`fs-skia-testing` for the failing-first / determinism suites, `fs-skia-evidence-mode` for
the animates-vs-snaps / survival / at-rest artifacts) and emit `tasks.deps.yml`.
