# Tasks: Controls.Elmish Command Model (Widget View + Cmd Alignment)

**Feature branch**: `068-controls-elmish-command-model`
**Spec**: `specs/068-controls-elmish-command-model/spec.md`
**Plan**: `specs/068-controls-elmish-command-model/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

This feature anticipates **no synthetic evidence** (Principle V): the Widget
lowering and the `Cmd<'msg>` bridge are real and the property test runs real
generated commands. No `[S]`/`[S*]`/`[SEH]` is planned. If any task ships
placeholder logic it must carry the Principle V disclosure in the inventory
below.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, … — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal); the whole
  feature is **Tier 1 (contracted)**, so per-task tier annotations are omitted.

Every task has a matching entry in `tasks.deps.yml`. Each task line mirrors the
structured `skillist` value using `[skillist: ...]` (`[skillist: []]` when no
capability skill applies).

## Governance risk levels

- **Small** (this feature's level): additive public surface confined to one
  package (`FS.Skia.UI.Controls.Elmish`); focused validation = the
  `package-surface` gate set (`PackageSurfaceCheck`, `FsiTranscripts`,
  `PerPackageSurfaceDiff`) printed by `Route`, plus `Dev`.
- **Medium**: would apply if a second package's surface or a dependency edge
  changed — none does here.
- **Broad**: the serialized six-target `maintainer-verify` order; required only
  because this is a consumer-contract (`src/**/*.fsi`) change, run sequentially
  (FAKE state is not concurrency-safe). Aggregate broad-run results are
  non-authoritative and recorded in `readiness/controls-elmish-command-model.md`;
  `Route` over the branch diff is the authoritative selector.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Record feature classification: Tier 1 (contracted), affected layer `src/Controls.Elmish/**`, additive-only public-API impact on `FS.Skia.UI.Controls.Elmish`, Elmish/MVU applicability (this package **is** the MVU boundary; `init`/`update` stay pure, interpreters unchanged), and the evidence obligations (`readiness/package-surface-expectations.md`, `readiness/controls-elmish-command-model.md`)
- [X] T002 [P] [skillist: []] Scaffold the two audit-relevant readiness placeholders discoverable before implementation: `readiness/package-surface-expectations.md` (required by the `package-surface` routing rule) and `readiness/controls-elmish-command-model.md` (feature-specific), each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [skillist: fsharp-build-orchestration] Wire up the test project: add the test-only `FsCheck` `<PackageReference>` (pinned 3.3.3) and a placeholder `AdapterCmdTests.fs` (before `Program.fs`) to `tests/Elmish.Tests/Elmish.Tests.fsproj`; confirm `./fake.sh build -t Dev` still builds green

---

## Phase 2: Foundation

- [X] T004 [skillist: fs-skia-elmish, fs-skia-ui-widgets] Draft the additive public surface in `src/Controls.Elmish/ControlsElmish.fsi` per `contracts/controls-elmish.fsi`: `ControlsElmish.widgetView`, `ControlsElmish.programOfWidget`, and `module AdapterCmd` (`none`/`ofMessage`/`productMessages`/`toCmd`), with `open Elmish` to name `Cmd<'msg>`; every existing signature (`AdapterProgram.View`, `program`, `AdapterCommand`/`AdapterEffect`/`AdapterSubscription`, interpreters) left byte-for-byte unchanged (FR-002)
- [X] T005 [skillist: fs-skia-elmish] Add minimal compiling stubs in `src/Controls.Elmish/ControlsElmish.fs` for the new symbols (e.g. `failwith`/placeholder bodies) so the package builds against the new `.fsi` and the failing-first tests can compile red; confirm `./fake.sh build -t Dev` builds with the expanded surface

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) — Typed program with no boundary shim (P1)

### Tests First (Principle I, Principle VI)

- [X] T006 [P] [US1] [skillist: fs-skia-elmish, fsharp-build-orchestration] Failing-first test in `tests/Elmish.Tests/TypedControlsAdapterTests.fs`: build a `view: 'model -> Widget<'msg>` from typed modules, construct the program via `programOfWidget`, render it, and assert (a) no `Widget.toControl` appears in product code (SC-001) and (b) the resulting `Control<'msg>` tree is structurally equal to `program init update (view >> Widget.toControl) subscriptions` (lowering parity, SC-002 / FR-004)

### Implementation

- [X] T007 [US1] [skillist: fs-skia-elmish] Implement `ControlsElmish.widgetView` (= `view >> Widget.toControl`) and `ControlsElmish.programOfWidget` (= `program init update (widgetView view) subscriptions`) as pure composition in `ControlsElmish.fs`; green the US1 parity test (FR-001/FR-004)

**Checkpoint**: User Story 1 is fully functional and testable independently.

---

## Phase 4: User Story 2 (US2) — Adapter effects in a standard Elmish command model (P1)

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US2] [skillist: fs-skia-elmish, fsharp-build-orchestration] Failing-first unit tests in `tests/Elmish.Tests/AdapterCmdTests.fs`: empty-command edge (`toCmd route [] = AdapterCmd.none`), effect-order preservation, and a recording dispatcher delivering exactly the carried `DispatchProductMessage` payloads in order with none dropped or duplicated (FR-003/FR-008 acceptance scenarios)
- [X] T009 [P] [US2] [skillist: fsharp-build-orchestration] Failing-first FsCheck round-trip property in `AdapterCmdTests.fs`: for generated commands, `dispatchedMessages (toCmd projectProduct command) = productMessages command` across ≥1,000 cases with no counterexample (FR-008/SC-003)
- [X] T010 [US2] [skillist: fs-skia-elmish] Implement `module AdapterCmd` in `ControlsElmish.fs` — `none` (= `Cmd.none`), `ofMessage`, `productMessages` (ordered `List.choose`), and a **total** `toCmd route` mapping every `AdapterEffect` case (product and non-product) to a `'msg` preserving order with `[] -> Cmd.none`; green the US2 unit and FsCheck property tests (FR-003/FR-008)

**Checkpoint**: User Story 2 is fully functional and testable independently.

---

## Phase 5: User Story 3 (US3) — Legacy Control-returning program unchanged (P1)

- [X] T011 [P] [US3] [skillist: fs-skia-elmish] Extend `tests/Elmish.Tests/ControlsElmishAdapterContractTests.fs`: assert the existing `.fsi` surface (`AdapterProgram.View: 'model -> Control<'msg>`, `program`, `AdapterCommand`/`AdapterEffect`/`AdapterSubscription`, `interpretKeyboardEffect`/`interpretControlEffect`/`subscriptions`) is unchanged and that a `Control<'msg>`-view program compiles with no source edit and behaves identically (FR-002/FR-009/SC-004)
- [X] T012 [P] [US3] [skillist: fs-skia-elmish] Retain/extend the dependency guard asserting the base `FS.Skia.UI.Controls` package declares **no** `Fable.Elmish` reference, preserving the dependency split (FR-006/SC-005)

**Checkpoint**: Additive guarantee proven — existing consumers unaffected.

---

## Phase 6: User Story 4 (US4) — Mixed migration (P3)

- [X] T013 [P] [US4] [skillist: fs-skia-elmish, fs-skia-ui-widgets] Verification test in `TypedControlsAdapterTests.fs`: a `Widget<'msg>` built via `Widget.ofControl` lowers identically to rendering the legacy control directly (`toControl (ofControl c) = c`), and a program on the Widget-view path coexists with another on the Control-view path with no interference (FR-010 / edge cases)

**Checkpoint**: Widget and Control authoring paths coexist as peers.

---

## Phase 7: Integration & Polish

- [X] T014 [skillist: fsharp-build-orchestration] Surface-area baseline refresh (Tier 1): run `./fake.sh build -t RefreshSurfaceBaselines` to regenerate `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt` and `readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt`; review the diff and confirm it is **additive-only** and confined to this package (SC-006)
- [X] T015 [skillist: fs-skia-elmish] Author `readiness/package-surface-expectations.md`: the additive-only `FS.Skia.UI.Controls.Elmish` delta and the regenerated-baseline rationale, satisfying the `package-surface` routing rule and `Route --enforce` (SC-006)
- [X] T016 [skillist: fs-skia-elmish] Author `readiness/controls-elmish-command-model.md`: the Widget-view path, the `AdapterCommand`↔`Cmd<'msg>` total-mapping rule, the lowering-parity result, the command round-trip property results, and the additive/peer compatibility note (Widget preferred, Control frozen peer)
- [X] T017 [skillist: fsharp-build-orchestration] Run `./fake.sh build -t Route` over the branch diff; confirm it prints the `package-surface` escalation and run **only** the printed gates (`PackageSurfaceCheck`, `FsiTranscripts`, `PerPackageSurfaceDiff`) sequentially to green (SC-007); then run the serialized broad maintainer-verify order (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`) sequentially as this is a consumer-contract change
- [X] T018 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the feature DAG resolves with no cycles, no dangling refs, and no `[S*]` surprises
- [X] T019 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (no synthetic evidence to accept)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. None is
anticipated for this feature (the Widget lowering and `Cmd` bridge are real).

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
