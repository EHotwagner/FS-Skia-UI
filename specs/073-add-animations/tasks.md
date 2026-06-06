# Tasks: Add Animations — Declarative Motion for FS.Skia.UI

**Feature branch**: `073-add-animations`
**Spec**: `specs/073-add-animations/spec.md`
**Plan**: `specs/073-add-animations/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/evidence-graph.md` for the propagated view.

**No `[S]` / `[S*]` / `[SEH]` is planned for this feature** (plan Constitution
Check → Synthetic evidence). Easing and tween sampling are real pure
computation; deterministic-scene evidence is real render-only output through the
existing `SceneEvidence.render` path; parity fixtures are golden bytes captured
from the real sampler (`FS_SKIA_CAPTURE_GOLDEN=1`), not fabricated literals; the
tick subscription is exercised through the real Elmish subscription plumbing.
`EvidenceAudit` must be PASS with no disclosures.

## Vertical-slice rule (US phases)

A `[US*]` task may only be marked `[X]` when the declared motion is reachable
from a user-facing authoring entry point and that path was actually exercised —
here, an FSI session against the packed `FS.Skia.UI.Scene` / `FS.Skia.UI.Elmish`
modules and/or captured deterministic render-only evidence under `readiness/`
showing the animation driven over time. Pure-math unit tests that compile green
do **not** by themselves satisfy `[X]` for a `[US*]` task; the author-facing
`Animation` / `AnimationState` / tick-subscription surface must have been driven.

This feature **is** stateful/time-driven (Principle IV applies). The MVU
evidence obligation is satisfied by: the pure `AnimationState.advance` /
`AnimationState.retarget` transitions tested (T019, T021), the `AnimationTick`
message routed through a pure `update` (T021, T023), and the tick **subscription**
(the only interpreter-edge component) asserted to emit while active and go silent
once settled (T020, T022). No hidden mutable animation registry exists.

## Success-criterion → assertion mapping

- **SC-001** (author fades/slides a widget with no clock and no per-frame
  interpolation) → the end-to-end entrance exercise driving `Animation.applyAt`
  at start/mid/settle: T017.
- **SC-002** (displayed value progresses monotonically per easing; end sample =
  declared end value) → `Easing.apply` endpoint-pinning + per-curve monotonicity
  (FsCheck) and `Tween.sample` monotonicity: T012, T013, enforced in `applyAt`
  at T016.
- **SC-003** (same animation + same time samples → identical output across runs
  and a fresh process) → `Animation.sampleFrames` through `SceneEvidence.render`
  `deterministic-scene` with byte-identical re-capture and fresh-process goldens:
  T025, T026.
- **SC-004** (settled animation ≡ static render of the same widget at its final
  value; no redraw once settled) → the identity-at-rest pass-through rule
  (`applyAt` returns the target unwrapped at identity): T014, T016, and the
  redraw-gating self-suspend: T020, T022.
- **SC-005** (un-animated view/control byte-identical to current behavior; no new
  required parameter) → `Animation.empty` / no-animation parity against today's
  static evidence: T029, T030.
- **SC-006** (mid-flight target change continues from the displayed value, no jump
  back to the original start) → `AnimationState.retarget` sets `Start = Current`:
  T019, exercised at T023.
- **SC-007** (every edge case resolves to its deterministic outcome with no hang,
  exception, or perpetual redraw) → non-positive duration ⇒ immediate end value
  and out-of-range clamp: T013; `advance` capped at `Duration` + removed-widget /
  settled-silent gating: T020, T022; concurrent independent samples: T025.

## Functional-requirement → task coverage

Every FR is implemented by at least one task (some are surfaced via the SC mapping
above; this block makes the FR↔task linkage explicit):

- **FR-001** (declare property animation, no per-frame code) → T005, T015, T016, T017
- **FR-002** (opacity / translate / scale / rotate / color) → T013, T015, T016
- **FR-003** (named easing set + documented default) → T012, T015 (`Easing.Default = EaseInOut`)
- **FR-004** (deterministic, supplied time model) → T025, T026
- **FR-005** (retarget from the current displayed value, no snap-back) → T019, T021, T023
- **FR-006** (settle, no idle redraw, settled ≡ static) → T014, T016, T020, T022
- **FR-007** (opt-in additive, un-animated unchanged) → T029, T030
- **FR-008** (edge cases deterministic) → T013, T020, T022, T025
- **FR-009** (capture evidence at explicit time samples) → T025, T026, T027
- **FR-010** (headless benign host-warning classification, no new failure mode) → T011, T022

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]**, **[US4]** — user-story scope
- **[T1]** — Tier 1 (contracted change); the whole feature is Tier 1, so the
  per-task tier annotation is omitted (matches the spec-level tier).

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors the
structured `skillist` as `[skillist: ...]` (exact order); `[skillist: []]` when
no capability skill applies.

## Canonical Verification Targets

FAKE-backed commands (`./fake.sh`, `fake.cmd`, `dotnet fake`) share `.fake`
state and are **not** safe to run concurrently — serialize them. `Route` is
authoritative: run `./fake.sh build -t Route` on the implementation diff and run
only the gates it prints. A new public `src/**/*.fsi` selects the
**`package-surface`** rule (`FocusedAuthority`: `PackageSurfaceCheck`,
`FsiTranscripts`, `PerPackageSurfaceDiff`); the `after_implement` hook runs
`EvidenceAudit`. The escalated serialized order, when more than one FAKE-backed
target is needed:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Intentional surface baseline refresh uses `./fake.sh build -t RefreshSurfaceBaselines`
(plus `PerPackageSurface.captureCurrent` for the per-package `.fsi.txt` snapshots,
which `RefreshSurfaceBaselines` does not regenerate).

**Governance risk levels**: an additive new public module + Elmish subscription
helper, with no renderer/IR change and no new dependency, is a **small/medium**
governance risk — focused validation is the `package-surface` gate set
(`PackageSurfaceCheck`, `FsiTranscripts`, `PerPackageSurfaceDiff`) plus `Dev` for
the new tests; **broad** validation (the full serialized six-target order above)
is required only because the new public `.fsi` escalates the route. Non-authoritative
aggregate results (e.g. `GeneratedProductCheck`'s known local environment failure)
are recorded as such, not as product regressions.

Template source: `.specify/presets/fsharp-opinionated/templates/tasks-template.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm branch `073-add-animations` and link spec, plan, research, data-model, quickstart, and the two contracts in `specs/073-add-animations/`
- [X] T002 [P] [skillist: []] Scaffold `specs/073-add-animations/readiness/` with the audit-enforced placeholder files discoverable before implementation: `animation-front-door.md`, `deterministic-sampling.md`, `settled-static-parity.md`, `redraw-gating.md`, `package-surface-expectations.md`, `per-package-surface-diff.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `skill-loading-evidence.md`, `evidence-graph.md`, `evidence-audit.md` — each naming the authoritative command, artifact path, failure class, and next action
- [X] T003 [P] [skillist: []] Scaffold the parity golden-fixture slot `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/` for the `animation-*.txt` frame hashes (the captured goldens land in US3)
- [X] T004 [skillist: fs-skia-scene] Record feature Tier 1, affected layers (`FS.Skia.UI.Scene` core + additive `FS.Skia.UI.Elmish` tick helper), additive public-API impact, Principle IV applicability (author-owned `AnimationState`, `AnimationTick` message, tick subscription at the interpreter edge — no hidden registry), and the **no-`[S]`** evidence obligations in `readiness/animation-front-door.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-scene] Draft the additive public `.fsi` surface `src/Scene/Animation.fsi` per `contracts/animation-surface.contract.md` — value types (`Easing`, `Transform`, `Tween<'a>`, `Animation`, `AnimationState<'a>`) and the `Easing` / `Transform` / `Color` / `Tween` / `Animation` / `AnimationState` module signatures plus `lerpFloat`, including `Easing.Default = EaseInOut` (the **FR-003** documented default authors reference for an unspecified curve). Note: `Tween.Easing` / `Tween.Duration` are mandatory record fields — the spec Assumptions' "short default duration" is quickstart guidance (e.g. 300 ms), not an omitted-field API default. No existing `FS.Skia.UI.Scene` signature changes shape
- [X] T006 [P] [skillist: fs-skia-elmish] Draft the additive public `.fsi` surface `src/Elmish/AnimationTick.fsi` — the `AnimationTick of TimeSpan` message (or wrapper) and `tickSubscription : isAnimating:('model -> bool) -> Sub<'msg>` routed to `AnimationTick`, per `data-model.md`, and land a **compiling stub** `src/Elmish/AnimationTick.fs` (a no-op subscription) so the project builds from foundation onward; the real emit-while-active / settled-silent gating is implemented at T022 (Principle II keeps the `.fsi` complete). No existing `FS.Skia.UI.Elmish` signature changes shape
- [X] T007 [skillist: fs-skia-scene] Add the `Animation.fsi` / `Animation.fs` compile entries to `src/Scene/Scene.fsproj` immediately after `Scene.fs` (the module depends on `Scene`); land `Animation.fs` with **compiling stubs** for the vals filled in by later stories (`AnimationState` → T021, `Animation.sampleFrames` → T026) so each phase ends in a buildable, test-runnable state per the tests-first cycle
- [X] T008 [skillist: fs-skia-elmish] Add the `AnimationTick.fsi` / `AnimationTick.fs` compile entries to `src/Elmish/Elmish.fsproj` after the existing subscription block (the `.fs` is the compiling stub from T006; its real gating behavior lands at T022)
- [X] T009 [skillist: []] Exercise the draft `.fsi` from FSI (`scripts/prelude.fsx` or ad-hoc): `Easing.apply`, `Tween.sample`, `Animation.applyAt`, `AnimationState.create`/`retarget`, and `AnimationTick`/`tickSubscription` shape; capture the session transcript to `readiness/fsi/animation-session.txt`
- [X] T010 [skillist: fs-skia-scene] Record the additive surface-area delta (new `Animation` module in Scene, new `AnimationTick` surface in Elmish, the generic `Tween<'a>` / `AnimationState<'a>` records) and the regenerated-baseline rationale (additions only) in `readiness/package-surface-expectations.md`
- [X] T011 [P] [skillist: fs-skia-evidence-mode] Record unsupported/headless host-warning handling (FR-010 — animation falls through the existing benign/blocking/deferred classification; the deterministic-scene path needs no GPU), the small/medium/broad governance risk levels, and aggregate-hang diagnostics in `readiness/runtime-limitations.md`, `readiness/governance-risk-levels.md`, and `readiness/aggregate-hang-diagnostics.md`

**Checkpoint**: Foundation ready — the animation `.fsi` surfaces, project wiring, FSI transcript, and surface/runtime expectations exist; story implementation may begin.

---

## Phase 3: User Story 1 (US1) — A product author fades and slides a widget into view (P1 keystone)

### Tests First (Principle I, Principle VI)

- [X] T012 [P] [US1] [skillist: fs-skia-scene] Add the red-first easing tests in `tests/Scene.Tests/AnimationTests.fs` — `Easing.apply e 0.0 = 0.0` and `Easing.apply e 1.0 = 1.0` for every case, per-curve monotonicity over `t ∈ [0,1]` (FsCheck), `t` clamped outside the domain, and `Easing.Default = EaseInOut` (the FR-003 documented default for an unspecified curve) (SC-002, FR-003)
- [X] T013 [P] [US1] [skillist: fs-skia-scene] Add the red-first tween/transform tests in `tests/Scene.Tests/AnimationTests.fs` — `Tween.progress`/`Tween.sample` clamp `elapsed` to `[0, Duration]`, non-positive `Duration` ⇒ progress `1.0` ⇒ immediate `End` value (no divide-by-zero), `lerpFloat` / `Color.lerp` / `Transform.lerp` per-field bounds, and `Transform.identity`/`isIdentity` (SC-002, SC-007)
- [X] T014 [P] [US1] [skillist: fs-skia-scene] Add the red-first identity-at-rest assertion in `tests/Parity.Tests/AnimationOutputTests.fs` — `Animation.applyAt` returns the target scene **unwrapped** (structurally equal to the static node) when sampled opacity `= 1.0` and transform is identity, and wraps in a `PerspectiveNode` only for a non-identity transform (SC-004)
- [X] T015 [US1] [skillist: fs-skia-scene] Implement the pure interpolation primitives in `src/Scene/Animation.fs` — `Easing.apply` (cubic ease curves, clamped input, `Easing.Default = EaseInOut`), `lerpFloat`, `Color.lerp` (per-RGBA-byte rounded), `Transform` (`identity`/`isIdentity`/`lerp`/`toPerspectiveTransform` composing translate∘rotate∘scale into the existing 3×3), and `Tween.progress`/`Tween.sample`; green T012–T013
- [X] T016 [US1] [skillist: fs-skia-scene] Implement `Animation.empty`, `Animation.applyAt`, and `Animation.isSettled` in `src/Scene/Animation.fs` with the identity-at-rest lowering rule (R5) — fold sampled opacity into `Paint.Opacity`/`Color.Alpha`, lower a non-identity sampled transform to `PerspectiveNode`, and pass the target through unwrapped at identity; green T014 (SC-002, SC-004)
- [X] T017 [US1] [skillist: fs-skia-evidence-mode] Exercise the entrance end-to-end (opacity 0→1 + translateY 24→0, ease-out, 300ms): drive `Animation.applyAt` at start/midpoint/settle through the deterministic-scene render path, confirm monotonic opacity/position progression and that the settled frame equals the static render with no redraw requested; capture the render-only evidence under `readiness/` and the real-sampling (no `[S]`) statement in `readiness/animation-front-door.md` (SC-001, SC-004)
- [X] T018 [US1] [skillist: []] Document the US1 independent validation path (declare entrance → advance across the duration → start/mid/end monotone frames → settled ≡ static → no idle redraw) in `readiness/animation-front-door.md`, and record the FR-003 default resolution: the documented default curve is `Easing.Default = EaseInOut`, and `Tween.Easing` / `Tween.Duration` are explicit fields (no omitted-field defaulting — the spec Assumptions' "short default duration" is quickstart guidance only)

**Checkpoint**: User Story 1 is fully functional and testable independently.

---

## Phase 4: User Story 2 (US2) — A property animates toward a new target when the model changes (P1)

### Tests First

- [X] T019 [P] [US2] [skillist: fs-skia-scene] Add the red-first `AnimationState` tests in `tests/Scene.Tests/AnimationTests.fs` — `create` sets `Current = Start = Target`, `advance` adds the delta capped at `Duration` and recomputes `Current` via easing `Start`→`Target`, `retarget` sets `Start = Current` / `Target = new` / `Elapsed = 0` (no snap-back, including a mid-flight second retarget), and `isActive` is false once settled (SC-006, SC-007)
- [X] T020 [P] [US2] [skillist: fs-skia-elmish] Add the red-first tick-subscription gating tests in `tests/Elmish.Tests/AnimationTickTests.fs` — `tickSubscription` emits `AnimationTick` deltas while `isAnimating` holds and goes silent once all animations settle (FR-006), and dropping the animating state from the model (removed widget) stops further ticks cleanly (SC-004, SC-007)
- [X] T021 [US2] [skillist: fs-skia-scene] Implement `AnimationState.create`/`advance`/`retarget`/`value`/`isActive` in `src/Scene/Animation.fs` (replacing the T007 stubs) as pure transitions over the author-supplied `interp`; green T019 (SC-006)
- [X] T022 [US2] [skillist: fs-skia-elmish] Implement the real `tickSubscription` in `src/Elmish/AnimationTick.fs` (replacing the T006 no-op stub) — a subscription that yields `AnimationTick` frame deltas only while `isAnimating` holds and self-suspends on settle (redraw gating at the framework-request level, host present loop unchanged); green T020 (FR-006)
- [X] T023 [US2] [skillist: fs-skia-elmish] Exercise the value-glide end-to-end through a pure `update` (FSI/render): `advance` over ticks toward a target, then a mid-flight `retarget` continuing from the displayed value with no jump back to the original start; capture the evidence and the active-emits/settled-silent observation in `readiness/redraw-gating.md` (SC-006, FR-006)
- [X] T024 [US2] [skillist: []] Document the US2 independent validation path (bind a value to `AnimationState` → dispatch target change → glide → mid-flight retarget without snap-back) in `readiness/animation-front-door.md`

**Checkpoint**: User Story 2 is fully functional and testable independently.

---

## Phase 5: User Story 3 (US3) — Animated output is captured as deterministic evidence (P2)

### Tests First

- [X] T025 [P] [US3] [skillist: fs-skia-evidence-mode] Add the red-first deterministic-evidence test in `tests/Parity.Tests/AnimationOutputTests.fs` — `Animation.sampleFrames` at start/midpoint/end rendered through `SceneEvidence.render` with `RendererMode = "deterministic-scene"` produces **distinct** hashes whose underlying property values move monotonically, and re-rendering the same samples is byte-identical; assert concurrent independent animations each sample without interference (SC-003, SC-007)
- [X] T026 [US3] [skillist: fs-skia-evidence-mode] Implement `Animation.sampleFrames : times -> Animation -> Scene -> Scene list` in `src/Scene/Animation.fs` (replacing the T007 stub), then capture the golden frame hashes `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-*.txt` via `FS_SKIA_CAPTURE_GOLDEN=1 dotnet test tests/Parity.Tests`; re-run without the env var (and in a fresh process) to prove byte-identical re-capture; green T025 (SC-003)
- [X] T027 [US3] [skillist: fs-skia-evidence-mode] Record the start/midpoint/end distinct-hash progression and the same-process + fresh-process byte-identical re-capture proof in `readiness/deterministic-sampling.md` (SC-003)
- [X] T028 [US3] [skillist: []] Document the US3 independent validation path (sample at explicit `TimeSpan` points → render through the existing deterministic-scene path → re-render twice and on a fresh process → identical bytes) in `readiness/deterministic-sampling.md`

**Checkpoint**: User Story 3 is fully functional and testable independently.

---

## Phase 6: User Story 4 (US4) — Animation is opt-in and never degrades static authoring (P3)

### Tests First

- [X] T029 [P] [US4] [skillist: fs-skia-evidence-mode] Add the red-first opt-in parity test in `tests/Parity.Tests/AnimationOutputTests.fs` — a representative existing scene rendered with `Animation.empty` (and with no animation node at all) is byte-identical to today's static deterministic-scene evidence for that scene, and authoring it requires no new parameter (FR-007, SC-005)
- [X] T030 [US4] [skillist: fs-skia-evidence-mode] Record the settled ≡ static proof (identity-at-rest, FR-006/SC-004) and the un-animated-unchanged proof (FR-007/SC-005) in `readiness/settled-static-parity.md`; green T029 (no new code beyond the T016 identity-at-rest rule)
- [X] T031 [US4] [skillist: []] Document the US4 independent validation path (render a representative existing view with no animation declaration → golden parity vs. current behavior → no animation-related parameter required) in `readiness/settled-static-parity.md`

**Checkpoint**: User Story 4 is fully functional and testable independently.

---

## Phase 7: Integration & Polish

- [X] T032 [P] [skillist: fs-skia-scene] Regenerate the Scene + Elmish public-surface baselines (`./fake.sh build -t RefreshSurfaceBaselines`) and the per-package `.fsi.txt` snapshots (`PerPackageSurface.captureCurrent`), and confirm the only delta is additions via `PackageSurfaceCheck` / `PerPackageSurfaceDiff`; record `readiness/per-package-surface-diff.md`
- [X] T033 [skillist: fsharp-build-orchestration] Run the focused `package-surface` gates sequentially — `Dev`, `PackageSurfaceCheck`, `FsiTranscripts`, `PerPackageSurfaceDiff` — and record the focused-gate list plus non-authoritative aggregate notes (e.g. `GeneratedProductCheck`'s known local environment failure) in `readiness/package-surface-expectations.md`
- [X] T034 [skillist: fsharp-build-orchestration] Run `./fake.sh build -t Route --enforce` over the branch diff; confirm escalation to the `package-surface` rule and that every required evidence artifact is present and populated
- [X] T035 [P] [skillist: []] Record skill-loading evidence — one row per `[X]` task with a non-empty `skillist`, each `ResolvedSkillPath` the registry-scanned path (`.agents/skills/<id>/SKILL.md`, or `src/Scene/skill/SKILL.md` / `src/Elmish/skill/SKILL.md` for `fs-skia-scene` / `fs-skia-elmish`) and `LoadedAt` strictly before `WorkStartedAt` — in `readiness/skill-loading-evidence.md`
- [X] T036 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; record `readiness/evidence-graph.md`
- [X] T037 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with no synthetic disclosures; record `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

No `[S]` / `[SEH]` tasks are planned for this feature (plan Constitution Check →
Synthetic evidence: easing/tween sampling is real pure computation, deterministic
render evidence is real render-only output through `SceneEvidence.render`, parity
fixtures are golden bytes captured from the real sampler, and the tick
subscription runs through the real Elmish plumbing). This table stays empty
unless `/speckit.implement` discovers an unavoidable synthetic path, which must
return to design/task review before being marked.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
