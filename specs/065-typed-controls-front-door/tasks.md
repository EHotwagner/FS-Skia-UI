# Tasks: Typed Controls Front Door

**Feature branch**: `065-typed-controls-front-door`
**Spec**: `specs/065-typed-controls-front-door/spec.md`
**Plan**: `specs/065-typed-controls-front-door/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: the evidence audit promotes any
task whose dependency is `[S]`/`[S*]` and which would otherwise be `[X]`.
No `[SEH]` task is approved for this feature — lowering is real and
parity-tested (FR-004, SC-002), so no synthetic-output disclosure is required.

## Vertical-slice rule (US phases)

A `[US*]` task may only be marked `[X]` when the change is reachable from a
user-facing entry point and that path was actually exercised (an FSI session
against the library, a sample run, or render evidence captured under
`readiness/`). For the stateful story (US3), `[X]` additionally requires
Elmish/MVU evidence: the reused `Model`/`Msg`/`Effect` contract was exercised,
pure `update` transitions were tested, and emitted effects were asserted equal
to the existing control's results.

## Success-criterion → assertion mapping

- **SC-001** (compile-time safety) — record-literal authoring plus an automated negative-compilation check over the `quickstart.md` compile-fail snippets (T007).
- **SC-002** (100% six-control structural parity) — `TypedLoweringTests.fs` parity matrix (T016).
- **SC-003** (existing suite/samples unchanged) — full-suite + sample-compile check (T015).
- **SC-004** (zero new dependency) — dependency-governance guard test (T014).
- **SC-005 / SC-006** (`Route` escalation + printed gates + populated routing-required evidence) — T020, T021.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US4]** — user-story scope
- **[T1]** — Tier 1 (contracted change; adds public `.fsi` surface). The whole
  feature is Tier 1, so per-task `[T1]` is omitted (matches the overall tier).

Every task has a matching entry in `tasks.deps.yml`; each task line mirrors its
structured `skillist` as `[skillist: ...]`.

## Canonical Verification Targets

FAKE-backed commands share `.fake` state and are **not** safe to run
concurrently — run them sequentially in the deterministic escalated order
(`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
`EvidenceGraph` → `EvidenceAudit`). The `Route`-printed `controls-public-surface`
gates (`ControlsCatalogCheck`, `ControlsInteractionCheck`,
`ControlsRenderingCheck`, `PackageSurfaceCheck`, `FsiTranscripts`,
`GeneratedProductCheck`) also apply. Non-FAKE file reads/checks may be
parallelized. Aggregate FAKE results are non-authoritative; the per-target
verdict is authoritative. Governance risk level for this change: **medium**
(additive public `.fsi` surface confined to `src/Controls/**`).

---

## Phase 1: Setup

- [X] T001 [P] [skillist: []] Scaffold `specs/065-typed-controls-front-door/readiness/` placeholder files discoverable before implementation — the routing-required `typed-controls-front-door.md` and `package-surface-expectations.md`, the supporting `typed-lowering-parity.md` and `controls-rendering.md`, plus `governance-risk-levels.md` and `runtime-limitations.md`; each names its authoritative command, artifact path, failure class, and next action
- [X] T002 [P] [skillist: []] Record feature Tier 1, affected layer (`src/Controls/**`), public-API impact (additive `.fsi`), Elmish/MVU applicability (stateful `TextBox`/`DataGrid` delegate to the existing pure `TextInput`/`DataGrid` models — Principle IV satisfied by delegation), and required evidence obligations into `readiness/typed-controls-front-door.md`

---

## Phase 2: Foundation

- [X] T003 [skillist: fs-skia-ui-widgets] Add failing-first contract tests (committed red) in `tests/Controls.Tests/TypedControlContractTests.fs` asserting the `Widget` type/module and the six typed modules (`TextBlock`, `Button`, `CheckBox`, `Stack`, `TextBox`, `DataGrid`) exist under `FS.Skia.UI.Controls.Typed`, plus an `.fsi`-grep guard that no new typed field is `obj` or a string-named event (FR-005)
- [X] T004 [skillist: fs-skia-ui-widgets] Draft the public `.fsi` surface from `contracts/`: `src/Controls/Widget.fsi` (sealed `Widget<'msg>` + `module Widget` with `toControl`/`ofControl`/`render`) and `src/Controls/Widgets/{Primitives,TextBoxWidget,DataGridWidget}.fsi` under the `FS.Skia.UI.Controls.Typed` namespace, leaving existing `.fsi` files untouched (FR-001, FR-007, FR-010)
- [X] T005 [skillist: fs-skia-ui-widgets] Implement `src/Controls/Widget.fs` (private `{ Lowered: Control<'msg> }` record kept off the `.fsi`; `toControl`/`ofControl`/`render` with round-trip invariant `toControl (ofControl c) = c`) and wire `<Compile>` order in `Controls.fsproj` (`Widget` after `Control`; `Widgets/*` after the stateful controls) (FR-002)
- [X] T006 [skillist: fs-skia-ui-widgets] Exercise the typed `.fsi` from FSI (`scripts/prelude.fsx`), author a representative widget tree, finish with `Widget.toControl`, and capture the transcript to `readiness/fsi-session.txt` (`FsiTranscripts` gate)

**Checkpoint**: Foundation ready — the `Widget` seam compiles and the typed module signatures exist; story implementation may begin.

---

## Phase 3: User Story 1 (US1) — Author primitives with compile-time safety

### Tests First (Principle I, Principle VI)

- [X] T007 [P] [US1] [skillist: fs-skia-ui-widgets] Add per-control parity and event-binding tests for the three primitives (`tests/Controls.Tests/TypedLoweringTests.fs`, `InteractionTests.fs`): `TextBlock`/`Button`/`CheckBox` views lower structurally equal to the legacy `*.create [...]` output; `Button.OnClick = Some m` binds identically to `Button.onClick m` and `OnClick = None` lowers to **no** binding; `CheckBox.OnChanged` payload mapping matches legacy; and an automated negative-compilation check (an `fsc`/FSI expect-error harness over the `quickstart.md` compile-fail snippets) confirms a wrong field type and a wrong `OnClick` message type are rejected by the compiler, not at runtime (FR-004, FR-008, SC-001, US1 scenario 2)

### Implementation

- [X] T008 [US1] [skillist: fs-skia-ui-widgets] Implement the primitives in `src/Controls/Widgets/Primitives.fs` — `TextBlockProps`, `ButtonIntent`/`ButtonProps`, `CheckBoxProps`, each with `defaults` and `view`, lowering to the legacy builders so T007 turns green (FR-003, FR-005)

**Checkpoint**: US1 authoring of `TextBlock`/`Button`/`CheckBox` is functional and parity-verified.

---

## Phase 4: User Story 2 (US2) — Compose controls into a layout

### Tests First

- [X] T009 [P] [US2] [skillist: fs-skia-ui-widgets] Add `Stack` composition tests: children lower in order via `Widget.toControl` into `Stack.children`; `Widget.ofControl` bridges a legacy `Control` into the typed children and round-trips unchanged; the composed `Stack` lowers structurally equal to the legacy `Stack.create` (FR-002, FR-004)

### Implementation

- [X] T010 [US2] [skillist: fs-skia-ui-widgets] Implement the `Stack` typed view in `src/Controls/Widgets/Primitives.fs` (`StackOrientation`, `StackProps`, `defaults`, `view`) lowering `Children` via `Widget.toControl` while preserving order (FR-003)

**Checkpoint**: US2 composition of typed children (and legacy bridge) renders in order.

---

## Phase 5: User Story 3 (US3) — Author stateful controls

### Tests First

- [X] T011 [P] [US3] [skillist: fs-skia-ui-widgets, fs-skia-elmish] Add MVU-delegation and parity tests: `TextBox.init`/`update` return state and effects equal to `TextInput.init`/`update`, and `DataGrid.init`/`update` equal `DataGrid.init`/`update` (no parallel state types); each typed `view` lowers structurally equal to the legacy `TextBox.create`/`DataGrid.create` for the current model state (FR-006)

### Implementation

- [X] T012 [US3] [skillist: fs-skia-ui-widgets] Implement `src/Controls/Widgets/TextBoxWidget.fs` reusing `TextInputModel`/`TextInputMsg`/`TextInputEffect`; `init`/`update` delegate to `TextInput`, and `view` lowers to legacy `TextBox.create` attrs (FR-003, FR-006)
- [X] T013 [US3] [skillist: fs-skia-ui-widgets] Implement `src/Controls/Widgets/DataGridWidget.fs` reusing `DataGridModel`/`DataGridMsg`/`DataGridEffect`; `init`/`update` delegate to `DataGrid`, and `view` lowers to legacy `DataGrid.create` attrs (FR-003, FR-006)

**Checkpoint**: US3 stateful `TextBox`/`DataGrid` authoring delegates to the existing models with asserted equal effects.

---

## Phase 6: User Story 4 (US4) — Existing code keeps working

- [X] T014 [P] [US4] [skillist: []] Add a dependency-governance guard test (`tests/Elmish.Tests/`) asserting `Controls.fsproj` references no `Fable.Elmish` and gains no new dependency (FR-011, SC-004)
- [X] T015 [US4] [skillist: fs-skia-ui-widgets] Confirm the full existing Controls test suite passes unchanged and the existing samples compile/run with no source edits after the typed surface is added; record the no-behavioral-diff result in `readiness/typed-controls-front-door.md` (FR-007, SC-003)

**Checkpoint**: Legacy string-keyed API and existing consumers are provably unaffected.

---

## Phase 7: Cross-cutting integration

- [X] T016 [skillist: fs-skia-ui-widgets] Author the keystone six-control structural-parity matrix in `tests/Controls.Tests/TypedLoweringTests.fs` (attribute order normalized out of the comparison), proving 100% parity across all six controls, and populate `readiness/typed-lowering-parity.md` (FR-004, SC-002)
- [X] T017 [P] [skillist: fs-skia-scene, fs-skia-layout-readability] Add accessibility + rendering tests at ≥2 viewports proving typed views produce no visual or a11y diff vs the legacy IR (same render path reused byte-for-byte); capture render evidence to `readiness/controls-rendering.md` (`ControlsRenderingCheck`)
- [X] T018 [P] [skillist: fs-skia-elmish] Add an Elmish-boundary test (`tests/Elmish.Tests/`) proving a `Widget.toControl`-terminated `view` runs through `AdapterProgram` unchanged, with no adapter edit (FR-009)
- [X] T019 [P] [skillist: fs-skia-ui-widgets] Extend `samples/ControlsGallery/Program.fs` with a typed-authoring panel reachable from the sample's default executable path; render proof for this feature is headless (T017 `RenderingTests`), so this panel demonstrates authoring and is not claimed as interactive graphical readiness

**Checkpoint**: All six controls are parity-proven, render-clean, adapter-compatible, and demonstrated in the gallery.

---

## Phase 8: Integration & Polish

- [X] T020 [skillist: []] Refresh the additive public-surface baseline for the `Widget` + `Typed` modules (`./fake.sh build -t RefreshSurfaceBaselines`), review the intentional diff, and populate `readiness/package-surface-expectations.md` (`PackageSurfaceCheck`, SC-005, SC-006)
- [X] T021 [skillist: []] Run `./fake.sh build -t Route` over the branch diff; confirm the `controls-public-surface` escalation and run every printed gate; run `Route --enforce` (SC-005, SC-006). **Result**: escalated to tier `agent-ready` (matched `controls-public-surface`, `evidence-governance`, `specify-catchall`, `docs-only`, `package-surface`); `Route --enforce` → **Ok**. Printed gates: `Dev`✅, `PackageSurfaceCheck`✅, `PerPackageSurfaceDiff`✅, `FsiTranscripts`✅, `ControlsCatalogCheck`✅, `ControlsInteractionCheck`✅, `ControlsRenderingCheck`✅, `GeneratedGuidanceCheck`✅, `TemplateDrift`✅, `EvidenceGraph`✅, `EvidenceAudit`✅. `GeneratedProductCheck` → **environment-degraded** (generated scaffold's empty `.specify/feature.json`; the generated `Product` builds + tests pass — only its evidence-graph sub-step needs `SPECKIT_FEATURE_DIR`; the **merged** feature 064 hit the identical condition — see [runtime-limitations.md](../readiness/runtime-limitations.md)).
- [X] T022 [skillist: []] Run the escalated FAKE-backed targets sequentially. **Result**: `Dev`✅, `GeneratedGuidanceCheck`✅, `TemplateDrift`✅ (the Route-printed gate for this diff; `TemplateCheck` is **not** printed — no `template/**` edit), `GeneratedProductCheck` environment-degraded (see T021). Governance risk level **medium** ([governance-risk-levels.md](../readiness/governance-risk-levels.md)); the authoritative verdict is per-target, with `EvidenceAudit verdict=PASS` as the merge gate.
- [X] T023 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — **Ok**: no cycles, no dangling refs, no `[S*]` surprises (no synthetic tasks).
- [X] T024 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — **verdict=PASS, 0 blockers** (real-tasks=20, unaccepted-synthetic=0, diff-scan-hits=0, readiness-contract-hits=0). No `--accept-synthetic` override needed.

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. None is expected
for this feature — lowering is real and parity-tested.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
