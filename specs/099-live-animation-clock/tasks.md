# Tasks: Animation Clock on Retained Identity (R4)

**Feature branch**: `099-live-animation-clock`
**Spec**: `specs/099-live-animation-clock/spec.md`
**Plan**: `specs/099-live-animation-clock/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

`[SEH]` is an annotation for design-approved synthetic error-handling work; it
remains `[S]` when completed. **None planned for this feature.** R4's clock
advance/sample/retarget are **pure, total** functions of the accumulated
injected `TimeSpan` delta: a non-positive delta is a designed **no-op** (never a
throw, never a rewind) and a very-large delta **clamps** to the settled end (no
overshoot) — these are normal control flow, not error paths to fixture. US1's
headline proof is a **real** frame-sequence through the live `runInteractiveApp`
host seam (injected deltas); US2's survival is driven through that same real seam
(replacing the hand-seeded `Feature092LiveSurvivalTests` PRECONDITION); US3's
determinism uses FsCheck-**generated** delta sequences. Any `[S]` that appears
triggers the full Principle V disclosure regime.

## Tier & MVU posture

This is a **Tier 1 (contracted) change** — uniform across all tasks, so per-task
`[T1]` marks are omitted. The surface moves **narrowly and internally**: the
carried slot `RetainedUiState.Animation` in `src/Controls/RetainedRender.fsi` is
generalized from `AnimationState<Transform> option` to the feature-073
multi-channel paint carrier (a per-identity clock: `Anim` / `Elapsed` / `Target`),
mirrored in `RetainedRender.fs`, with internal `advance` / `sampleOnPaint` /
retarget helpers declared `internal` for the test assembly. The **public**
`src/Controls.Elmish/ControlsElmish.fsi` `runInteractiveApp` / `InteractiveAppHost`
surface is **unchanged** — the seam is internal host wiring driven by the
already-present `Tick : TimeSpan -> 'msg option` delta. Feature-073
`src/Scene/Animation.fsi` is **consumed, not changed**. Because a
`src/Controls/**/*.fsi` line changes (the carried-slot type), `Route` **escalates**
to the serialized six-target maintainer-verify path; surface-area baselines
(api-surface + per-package `.fsi.txt`) for `FS.Skia.UI.Controls` are **recaptured**
for the internal slot-type change (SC-007).

**No new public MVU surface.** R4 adds **no** consumer `Model`/`Msg`/`Effect`/`Cmd`/
`init`/`update`; the consumer's `update`/`view : 'model -> Control<'msg>` contract
is untouched. The clock advance and sample are **pure functions of the accumulated
injected delta** — no `Date.now`, no randomness, resume-safe. The animation state
lives in the host's existing `retained` ref (`StateByIdentity`), advanced by the
wrapped `Tick` and read on paint; the host loop is the **interpreter edge** that
injects the per-frame delta. The wrapped `Tick` still delegates to `host.Tick` so
the consumer's own tick message is unaffected (no double-dispatch, no swallowed
consumer tick). `AnimationState.advance`/`retarget`/`Animation.applyAt` (feature
073) are the reused pure transition + sample functions — R4 does **not**
re-implement an animation engine.

**Persistent-launch / viewer-launch rule does not newly apply.** R4 wires
animation into the **existing** `runInteractiveApp` host loop; it adds and changes
**no** default-executable / persistent-launch entry point. The
**animates-vs-snaps** proof is a deterministic **sampled-frame-sequence** captured
through the **real** `runInteractiveApp` seam (the responds-vs-renders /
evidence-mode primitive — a no-seam build snaps and fails it), not a new
persistent-launch/screenshot obligation. At-rest frames are **byte-identical** to
the pre-R4 golden (no rendered-output change at rest). Recorded as a **visible
decision** in T003: no persistent-launch / window-visibility obligation is
introduced; `real-image-evidence.md` records the frame-sequence as the
rendered-output evidence captured through the deterministic seam.

## Vertical-slice rule (US phases)

A `[US*]` task is `[X]` only when the user-reachable surface — an authored `view`
rendered and ticked through the **real** `runInteractiveApp` host seam (the same
loop the live window uses), with injected per-frame deltas — was actually
exercised and produced the observable behavior (a gradual transition / a surviving
clock / a GC'd identity). Passing unit tests on the pure `advance`/`sample`/
`retarget` helpers alone do **not** satisfy `[X]` for a `[US*]` task. Because the
consumer runtime model is untouched, the MVU evidence for these stories is the
read/advance of the existing `RetainedId`-keyed `StateByIdentity` carry driving a
real animated frame on the live seam; no new consumer transition is introduced to
assert.

## Success-criterion → assertion mapping

- **SC-001** (a live hover/press/focus transition **animates** — ≥1 intermediate
  sampled appearance across consecutive frames before the target, zero consumer
  animation code; a no-seam build snaps and fails) → T008 failing-first
  animates-vs-snaps test through `runInteractiveApp` + T009 seam wiring
  (`us1-animates-vs-snaps.md`).
- **SC-002** (an in-flight tween **survives a sibling-shifting unrelated re-render**
  and **completes deterministically through the real seam**, same final result as
  an un-shifted run; the hand-seeded `PRECONDITION` survival test is **replaced**)
  → T011 seam-driven survival rewrite + T012 (`us2-survival.md`).
- **SC-003** (a frame with **no active animation** is **byte-identical** to the
  pre-R4 golden and reports a **zero** at-rest recompute/animation-output count) →
  T014 failing-first identity-at-rest test + T015 fast-path impl
  (`us3-identity-at-rest.md`).
- **SC-004** (two runs over an **identical injected-delta sequence** produce
  **identical** sampled output; no wall-clock consulted) → T013 FsCheck determinism
  suite (≥1000 cases) + T015 pure-core impl (`us3-determinism.md`).
- **SC-005** (an animation clock for a **removed identity** is **absent** the
  following frame, GC'd via the existing live-identity filter) → T017 failing-first
  GC test + T018 (`us4-gc.md`).
- **SC-006** (advancing an animating clock keeps the per-frame repaint **scoped to
  its own subtree** — the work-reduction metric shows no whole-tree repaint or
  re-measure; R2 incremental measure preserved) → T019 (`scoped-repaint.md`).
- **SC-007** (the escalated serialized order is green with `EvidenceAudit` passing,
  no synthetic/stub work; surface baselines recaptured for the internal slot-type
  change) → T021 surface recapture + T022 first-four sequential + T023 graph + T024
  audit (`surface-baseline.md` / `validation-log.md` / `evidence-graph.md` /
  `evidence-audit.md`).

## Non-SC requirement traceability

- **FR-001** (per-identity clock advanced each frame by the **injected** delta,
  never a wall-clock) → T006 (advance core) + T009 (wrap `host.Tick` → advance
  before render) + T015 (no `Date.now` purity).
- **FR-002** (sample **every** live clock each frame into that frame's paint) →
  T006 (`sampleOnPaint` via `Animation.applyAt`) + T009 (wired into the paint pass).
- **FR-003** (a live `VisualState` flip **starts or retargets** a tween on that
  identity, from the current sampled value — no snap-to-start) → T006 (retarget
  helper) + T009 (reads the stamped `VisualState` from `applyRuntimeVisualState`).
- **FR-004** (survive an unrelated sibling-shifting re-render, continue from prior
  `Elapsed`, **complete deterministically** through the real seam) → T011 + T012.
- **FR-005** (no active tween ⇒ **no** animation output, byte-identical to the
  non-animated build; identity-at-rest / zero-recompute preserved) → T014 + T015.
- **FR-006** (identical injected-delta sequence ⇒ identical output; the clock is a
  pure function of accumulated injected deltas) → T013 + T015.
- **FR-007** (clocks for removed identities dropped via the existing `liveIds` GC
  filter; no leak) → T017 + T018.
- **FR-008** (reuse the E2 `RetainedId`-keyed `StateByIdentity` carry; **no parallel
  identity scheme**) → T005 (generalize the carried slot on the existing map) + T012.
- **FR-009** (reuse the feature-073 Scene animation primitives — `Tween`/
  `Animation`/`AnimationState`/`applyAt`/`advance`/`retarget`/`isSettled`; **no**
  new engine) → T006 + T015.
- **FR-010** (the scoped-repaint behavior from E2/R2 is preserved — advancing a
  clock does **not** force a whole-tree repaint/re-measure; paint-level only) →
  T019.

## Governance risk levels

- **Small** — the pure `advance`/`sampleOnPaint`/retarget core (totality,
  non-positive no-op, settled-end clamp, settled-`Normal` drop, determinism):
  focused validation is `Dev` + the targeted `Controls.Tests` determinism /
  identity-at-rest / edge suites.
- **Medium** — the host seam wiring in `runInteractiveApp` (wrap `Tick` → advance
  before render, sample on paint, retarget from stamped `VisualState`) and the
  survival / GC / scoped-repaint behaviors through the live adapter: `Dev` + the
  `Elmish.Tests` animates-vs-snaps / seam-driven survival / removed-identity GC /
  scoped-repaint suites.
- **Broad** — escalation **applies**: the internal `src/Controls/**/*.fsi`
  slot-type change forces the serialized `Dev → GeneratedGuidanceCheck →
  TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit`
  maintainer-verify path. **`Route` is authoritative** — run `./fake.sh build -t
  Route` against the actual diff and run exactly the gates it prints. FAKE-backed
  targets run **sequentially** (shared `.fake` state); aggregate results are
  recorded as **non-authoritative** unless re-confirmed sequentially.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]**, **[US4]** — user-story scope
- Every task has a matching `tasks.deps.yml` entry; every line mirrors the
  structured `skillist` via `[skillist: ...]`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory artifacts are present and linked (`spec.md`, `plan.md`, `research.md`, `data-model.md`, `quickstart.md`, `contracts/host-animation-seam.md`, `contracts/sample-on-paint.md`, `checklists/`) and that `.specify/feature.json` resolves `specs/099-live-animation-clock`
- [X] T002 [P] [skillist: fs-skia-evidence-mode] Scaffold audit-discoverable readiness placeholders under `readiness/`: `us1-animates-vs-snaps.md`, `us2-survival.md`, `us3-identity-at-rest.md`, `us3-determinism.md`, `us4-gc.md`, `scoped-repaint.md`, `surface-baseline.md`, `fsi-transcript.md`, `validation-log.md`, plus `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, `real-image-evidence.md`, `evidence-graph.md`, `evidence-audit.md` — each naming its authoritative command, artifact path, failure class, and next action (use `key=value` lines, not bare image-filename claims; `real-image-evidence.md` records the **animates-vs-snaps frame-sequence** captured through the deterministic `runInteractiveApp` seam as the rendered-output evidence, cross-referencing `us1-animates-vs-snaps.md` — there is no persistent-launch / window obligation)
- [X] T003 [P] [skillist: []] Record feature Tier 1 (contracted: the internal `RetainedUiState.Animation` carried-slot type generalized to the feature-073 multi-channel clock in `RetainedRender.fsi`/`.fs`), affected layers (`FS.Skia.UI.Controls` — `RetainedRender.fsi`/`RetainedRender.fs` slot type + `advance`/`sampleOnPaint`/retarget core + carry/GC reuse; `FS.Skia.UI.Controls.Elmish` — `ControlsElmish.fs` host-seam wiring only; `FS.Skia.UI.Scene` feature-073 animation **consumed, not modified**; `ControlRuntime` R1 bridge **consumed, not modified**), public-API impact (the **public** `runInteractiveApp`/`InteractiveAppHost` surface is **unchanged**; only the internal `RetainedRender.fsi` slot type moves), MVU applicability (no new consumer `Model`/`Msg`/`Effect`/`update`; clock advance/sample are pure functions of the injected delta; the host loop is the interpreter edge), and the evidence obligations from the plan; record as a **visible decision** that the persistent-launch / viewer-launch task-generation rule does **not** newly apply (no default-exe / persistent-launch entry point added; animation is observed through the existing `runInteractiveApp` seam; at-rest frames byte-identical; no window-visibility / screenshot obligation)
- [X] T004 [skillist: []] Run `./fake.sh build -t Route`; confirm the internal `src/Controls/**/*.fsi` slot-type change **escalates** to the serialized six-target maintainer-verify path (`Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit`) and record the authoritative gate list plus the small/medium/broad governance risk levels into `readiness/governance-risk-levels.md` (note: `Route` escalates only **after** the `.fsi` edit exists; T004 records the **expected** escalation, T022/T023/T024 verify it on the real diff)

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-reconciliation] Generalize the **internal** carried-slot type: in `src/Controls/RetainedRender.fsi` change `RetainedUiState.Animation` from `AnimationState<Transform> option` to a feature-073 multi-channel clock `option` (per-identity record carrying `Anim : FS.Skia.UI.Scene.Animation`, `Elapsed : System.TimeSpan`, `Target : VisualState`), mirror it in `src/Controls/RetainedRender.fs` (`:26–28`), and declare the internal `advance` / `sampleOnPaint` / retarget helper signatures `internal` in the `.fsi` for the test assembly; record the current `FS.Skia.UI.Controls` api-surface + per-package `.fsi.txt` baselines as the **pre-change reference** for the Phase-7 recapture (this internal slot type moves the baseline; SC-007/FR-008). The existing `Feature092LiveSurvivalTests` hand-seed no longer compiles against the new type and is **rewritten** in US2 (T011)
- [X] T006 [skillist: fs-skia-reconciliation, fs-skia-scene] Implement the pure core in `src/Controls/RetainedRender.fs`, **reusing** feature-073 `Animation.applyAt`/`AnimationState.advance`/`retarget`/`isSettled` (no new engine; FR-009): `advance (delta : TimeSpan) clock` accumulates `Elapsed` (basic path); `sampleOnPaint` derives the painted node through `Animation.applyAt clock.Elapsed clock.Anim` (opacity/transform/color, paint-level only — never layout); the retarget helper **starts** a tween (no clock + desired ≠ `Normal`) or **retargets** from the **current sampled value** (clock + desired ≠ `Target`) when the stamped `VisualState` differs from `clock.Target`, using the single pinned framework default `defaultTransitionDuration = 150 ms` + `EaseOut`, opacity/tint channel (research §R4 / data-model constant — a fixed value, not a per-control knob, so the determinism goldens reach the settled end after the same fixed frame count); carry + GC are **unchanged** (the existing `liveIds` filter already drops the slot with its identity). The host seam is **not** yet wired (`ControlsElmish.fs` still passes `Tick` through), so the **live host still snaps** and the US1 failing-first test (T008) goes **RED**
- [X] T007 [P] [skillist: fs-skia-evidence-mode] Record unsupported-scope, permanent non-goals, and failure diagnostics into `readiness/runtime-limitations.md` (Out of Scope / Assumptions): no consumer-facing animation authoring API (keyframes, timelines, per-control DSL); no spring/physics or easing models beyond the feature-073 set; **no layout geometry animation** (size/position reflow) — paint-level only, so R2 incremental measure + scoped-repaint are preserved; no general animation scheduler beyond the per-frame tick advance; no full-52-control animation coverage (tracked with E3/R1); the clock is driven by **injected deltas only** (no `Date.now`/wall-clock); a **non-positive** delta is a designed **no-op** (never rewinds), a **very-large** delta **clamps** to the settled end (no overshoot); a reduced-motion / opt-out policy is **not required** but the design must **not preclude** one (snap = the pre-R4 path); CSS selectors, attached/dependency properties, lookless templates, and data binding remain permanent roadmap non-goals

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — a visual-state transition animates on the live host

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: fs-skia-elmish, fs-skia-testing] Add the failing-first **animates-vs-snaps** suite (`tests/Elmish.Tests`, fails against the un-wired seam from T006; SC-001/FR-002/FR-003): drive a hover/focus interaction on a **`Button`** (the representative R1-migrated interactive kind — feature 096's migrated set is `Button`, `CheckBox`, `Slider`, `TextBox`, `RadioGroup`, `Switch`; `Button` exercises hover/press/focus-ring fade on the opacity/tint channel) through the **real** `runInteractiveApp` host seam with a fixed sequence of injected per-frame deltas, capture the sampled appearance across consecutive frames, and assert at least one **intermediate** sampled value is present **before** the target is reached (a gradual transition), then that the target is reached and converges to exactly the snapped appearance; a build without the seam snaps in one step and fails the assertion

### Implementation

- [X] T009 [US1] [skillist: fs-skia-elmish, fs-skia-reconciliation] Wire the host animation seam inside `runInteractiveApp` (`src/Controls.Elmish/ControlsElmish.fs`, `renderRetained` `:583`, `viewerHost` `:695–702`; contract C1/C2): **wrap** `host.Tick` so the injected per-frame `delta` **advances** every live per-identity clock in `retained.Value.StateByIdentity` (via T006 `advance`) **before** the next `renderRetained`, then **delegates** to `host.Tick delta` for the consumer message (total delegation — no swallowed consumer tick, no double-dispatch); the **retarget** reads the per-identity `VisualState` already stamped by `applyRuntimeVisualState` (`:587`,`:594`, R1) and starts/retargets the tween (T006 retarget helper); **sample on paint** feeds each active clock's `sampleOnPaint` value into that identity's painted node, scoped to its subtree — making the live transition **animate**; this makes T008 **GREEN**. The public `ControlsElmish.fsi` surface stays unchanged (internal wiring); if a host-internal value must reach the test assembly declare it `internal` and recapture only that package baseline
- [X] T010 [US1] [skillist: fs-skia-evidence-mode] Capture US1 to `readiness/us1-animates-vs-snaps.md` (the real `runInteractiveApp` seam, injected deltas, the captured frame sequence for the **`Button`** representative kind showing ≥1 intermediate sampled appearance before the target, converging to the snapped target; name the kind and the `defaultTransitionDuration = 150 ms` so the frame count is reproducible; an un-wired/no-seam build snaps and cannot produce this artifact) (SC-001)

**Checkpoint**: User Story 1 is functional and testable independently.

---

## Phase 4: User Story 2 (US2) — an in-flight animation survives an unrelated re-render and completes

### Tests First (Principle I)

- [X] T011 [P] [US2] [skillist: fs-skia-elmish, fs-skia-testing] **Rewrite** `tests/Elmish.Tests/Feature092LiveSurvivalTests.fs` to drive survival entirely through the **real** seam (delete the hand-seeded `startedClock ()` PRECONDITION `:54–58,98–105`; SC-002/FR-004): start a tween via a real interaction through `runInteractiveApp`, advance a few frames with injected deltas, apply an **unrelated, sibling-shifting** re-render (a model change that reorders siblings), then continue ticking — assert the **same** `RetainedId`'s clock continues from its **prior** `Elapsed` (not reset, not dropped) and **completes** with the **same final result** as an un-shifted run. This is failing-first against the un-wired build and against any parallel-identity regression
- [X] T012 [US2] [skillist: fs-skia-reconciliation, fs-skia-evidence-mode] Confirm survival holds via the **existing** `RetainedId`-keyed `StateByIdentity` carry (the clock rides the same identity map E2 established — **no parallel identity scheme**, FR-008); demonstrate the sibling-shift keeps the identity stable and the clock continues from its prior `Elapsed` to completion; capture `readiness/us2-survival.md` (seam-driven, replacing the prior hand-seeded precondition) (SC-002)

**Checkpoint**: User Story 2 is functional and testable independently.

---

## Phase 5: User Story 3 (US3) — animation is deterministic and identity-at-rest is preserved

### Tests First (Principle I, Principle VI)

- [X] T013 [P] [US3] [skillist: fs-skia-testing] Add the FsCheck **determinism + edge** suite (`tests/Controls.Tests`, **≥1000** generated cases; SC-004/FR-006): two runs over an **identical** generated injected-delta sequence produce **identical** sampled output, and the clock consults **no** wall-clock source (pure function of accumulated deltas); plus edge cases (failing-first for the hardening in T015) — **zero / non-positive** delta is a **no-op** (clock unchanged, never rewinds), a **very-large** delta **clamps** to the settled end (no overshoot past target), a transition **retargeted mid-flight** re-aims from the **current sampled value** (no snap to start), a **return-to-`Normal`** clock that has settled **drops** to no output, and **multiple controls animating simultaneously** advance **independent** clocks — given two identities with active clocks at different `Elapsed`, advancing one frame moves each by its own injected delta and **one clock completing/dropping does not perturb the other's `Elapsed` or sampled output** (spec edge case "Multiple controls animating simultaneously"; SC-006/FR-010 independence)
- [X] T014 [P] [US3] [skillist: fs-skia-reconciliation, fs-skia-testing] Add the failing-first **identity-at-rest** test (`tests/Controls.Tests`; SC-003/FR-005): a frame for an identity with **no active clock** (`Animation = None`, including a settled-and-dropped clock) emits **no** animation attribute and is **byte-identical** to the pre-R4 static render, and the at-rest recompute / animation-output count for that frame is **zero** (E2 `RecomputedNodeCount = 0` preserved)
- [X] T015 [US3] [skillist: fs-skia-reconciliation, fs-skia-scene] Harden the `advance`/`sampleOnPaint` core (T006) for totality, determinism, and the at-rest fast path (makes T013/T014 **GREEN**; FR-005/FR-006/FR-009): non-positive delta ⇒ no-op (`Elapsed` unchanged); `Elapsed` past the tween `Duration` ⇒ **clamp** to the settled end value via `Animation.isSettled` (no overshoot); a settled clock whose `Target = Normal` is **dropped** to `None` so the identity returns to byte-identical at-rest output (resolving the FR-003 vs FR-005 interaction — the converged frame reaches **exactly** the snapped target); advance/sample remain **pure** (no `Date.now`, no randomness, resume-safe); the no-active-clock paint path emits **no** attribute (byte-identical fast path retained)
- [X] T016 [US3] [skillist: fs-skia-evidence-mode] Capture `readiness/us3-determinism.md` (two runs over an identical injected-delta sequence → identical sampled output, ≥1000 cases, no wall-clock consulted) and `readiness/us3-identity-at-rest.md` (a no-active-clock frame is byte-identical to the pre-R4 golden with a zero at-rest recompute/output count) — read from the real suites, not assumed (SC-003/SC-004)

**Checkpoint**: User Story 3 is functional and testable independently.

---

## Phase 6: User Story 4 (US4) — clocks are garbage-collected for removed identities

### Tests First (Principle I)

- [X] T017 [P] [US4] [skillist: fs-skia-elmish, fs-skia-testing] Add the failing-first **removed-identity GC** test (`tests/Elmish.Tests`; SC-005/FR-007): animate a control through the real seam (its `RetainedId` has an active clock), then apply a re-render in which that control is **removed** from the tree (its identity no longer appears); assert the retained state for that identity — **including its animation clock** — is **absent** on the next frame, matching the existing focus/text GC behavior, with no leaked or dangling animation state

### Implementation / Evidence

- [X] T018 [US4] [skillist: fs-skia-reconciliation, fs-skia-evidence-mode] Confirm the clock GC reuses the **existing** `liveIds` filter (`RetainedRender.fs:363–371`) that already drops focus/text state for removed identities — **no new GC code** (the generalized slot is dropped with the rest of its `RetainedUiState`); capture `readiness/us4-gc.md` (a removed identity's animation clock is gone the following frame, via the live-identity filter) (SC-005)

**Checkpoint**: User Story 4 is functional and testable independently.

---

## Phase 7: Integration & Polish

- [X] T019 [P] [skillist: fs-skia-reconciliation, fs-skia-evidence-mode] Write `readiness/scoped-repaint.md` (SC-006/FR-010): advancing and sampling an animating identity's clock keeps the per-frame repaint **scoped to that identity's own subtree** — the work-reduction metric shows animation does **not** force a whole-tree repaint or re-measure, and the presence of one active animation does **not** invalidate the at-rest fast path for other identities (R2 incremental measure preserved); demonstrate via the `Elmish.Tests` work-reduction assertion
- [X] T020 [P] [skillist: fs-skia-ui-widgets] Exercise the animation seam from FSI against the packed library per `quickstart.md` — host a migrated control, drive a hover/focus transition with injected deltas, observe a **gradual** (non-snapping) transition with **zero** consumer animation code, and confirm an at-rest frame emits no animation attribute — capture the session transcript to `readiness/fsi-transcript.md`
- [X] T021 [P] [skillist: fs-skia-reconciliation] Recapture the `FS.Skia.UI.Controls` api-surface + per-package `.fsi.txt` baselines vs the T005 pre-change reference and confirm the diff shows **exactly** the internal `RetainedUiState.Animation` slot-type generalization (and any `internal` helper declarations) with no other surface drift; confirm the public `ControlsElmish.fsi` `runInteractiveApp`/`InteractiveAppHost` surface is **unchanged**; record to `readiness/surface-baseline.md` (SC-007/FR-008)
- [X] T022 [skillist: fs-skia-testing] Run exactly the gates `Route` printed (T004) — the serialized `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck` prefix **sequentially** (shared `.fake` state, never concurrently) — and record the aggregate results as **non-authoritative** into `readiness/generated-guidance-validation.md` and the run transcript into `readiness/validation-log.md`; rerun any race-like FAKE failure sequentially before any product-regression claim; if an aggregate hangs, record the diagnosis in `readiness/aggregate-hang-diagnostics.md` (SC-007)
- [X] T023 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory` + `tasks=<n>` match this feature, no cycles, no dangling refs, no `[S*]` surprises; record to `readiness/evidence-graph.md`
- [X] T024 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (synthetic-propagation + diff-scan; no synthetic/stub work) or document every `--accept-synthetic` override; record to `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. **None planned** — see
the Status Legend rationale (the clock advance/sample/retarget are pure, total,
deterministic functions of the injected delta; the non-positive no-op and
large-delta clamp are designed control flow, not error paths; the US1
animates-vs-snaps and US2 survival proofs run through the **real**
`runInteractiveApp` seam — survival **replaces** the hand-seeded
`Feature092LiveSurvivalTests` PRECONDITION; US3 determinism uses FsCheck-generated
delta sequences; GC reuses the existing `liveIds` filter). For any `[SEH]` rows,
include the approval label, design-phase source, synthetic input class, expected
error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
