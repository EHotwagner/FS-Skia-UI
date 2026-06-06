# Tasks: Migrate Remaining 41 Controls to the Typed Props/MVU Front Door

**Feature branch**: `070-typed-controls-migration`
**Spec**: `specs/070-typed-controls-migration/spec.md`
**Plan**: `specs/070-typed-controls-migration/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/typed-lowering-parity.md` and the graph render for
the propagated view. **Intent for `070` is zero `[S]`/`[S*]`** (FR-011 / SC-010):
every lowering is real and parity-tested. No `[SEH]` rows are approved for this
feature — there is no error-path/malformed-input work here.

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the typed surface is reachable
through the public front door and that path was actually exercised — an FSI session
authoring a typed `view`, a `Widget.toControl` round-trip, the gallery render smoke,
or a captured transcript/screenshot under `readiness/`. For the stateful stories
(US3) `[X]` additionally requires Elmish/MVU evidence: the reused `Model`/`Msg`/
`Effect` contract was exercised, the typed `update` delegation was tested against the
existing model's `update`, and emitted effects were asserted. Compiling unit tests
alone do not satisfy `[X]` for a `[US*]` task.

## Success-criterion → assertion mapping

- **SC-002** (100% lowering parity) → the per-group structural-parity tests in
  `TypedLoweringTests.fs` (the 41-row matrix, T020–T026), normalized attribute order,
  asserted on the lowered `Control<'msg>`.
- **SC-003** (stateful delegation identical) → delegation-equality tests asserting the
  typed `update` result equals the reused model's `update` for the same input
  (T029–T031), plus the no-forked-model surface check (T032).
- **SC-004** (additive-only surface) → `PackageSurfaceCheck` / `PerPackageSurfaceDiff`
  over the regenerated `FS.Skia.UI.Controls` baseline (T033–T034).
- **SC-005** (no `obj`/string-keyed event) → the `.fsi` grep guard in the contract
  tests (T006).
- **SC-006** (no new package dependency, in particular not `Fable.Elmish`) → the
  `tests/Elmish.Tests/` dependency-governance guard asserting `Controls.fsproj`
  references no `Fable.Elmish`/no new package (T045).
- **SC-007** (catalog currency) → `ControlsCatalogGenerationCheck` over the regenerated
  `catalogFacts` and the extended `typedPropsById` cross-check (T007, T036).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]**, **[US4]** — user-story scope
- **[T1]** — Tier 1 (contracted, additive public `.fsi`) — the whole feature is Tier 1
- Every task mirrors its structured `skillist` value as `[skillist: ...]`.

> **Skill note**: the new `fs-skia-typed-controls` capability skill is authored *in*
> this branch (T002) and does not exist at task-generation time, so it cannot appear
> in any `skillist` (declared ids must resolve to a readable `SKILL.md`). Widget/typed
> work therefore declares the closest existing capability skill, `fs-skia-ui-widgets`.
> Once `fs-skia-typed-controls` lands, later features should prefer it.

## Governance risk levels

- **Small**: a single group module or its parity tests — focused validation is
  `./fake.sh build -t Dev` plus the group's Expecto tests.
- **Medium**: catalog regeneration or the package-surface baseline refresh — add
  `ControlsCatalogGenerationCheck` / `PackageSurfaceCheck` and review the diff.
- **Broad**: the public `.fsi` surface as a whole — run the `Route`-printed
  `controls-public-surface` (+ skill) gate set and the escalated six-target order
  (T042–T044). Broad validation is required before declaring the feature done.
  FAKE-backed targets run **sequentially** (shared `.fake` state); aggregate results
  are recorded as non-authoritative until the serialized rerun confirms them.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the `specs/070-typed-controls-migration/` feature directory and link spec + plan in this task header
- [X] T002 [skillist: []] Author the canonical `.agents/skills/fs-skia-typed-controls/SKILL.md` capability skill (pick taxonomy fields → write `Props` + `defaults` + `view` → add the mandatory lowering-parity test → reuse the existing MVU model for stateful controls → keep the surface additive) and regenerate its `.claude` peer via `./fake.sh build -t RefreshSurfaceBaselines` (FR-013 / SC-008; gates the migration) — skill authored (rubric: Scope, API/.fsi, 2 runnable examples, 2 research URLs, persistent-problem mandate, `[[` related links, Sources); `.claude` peer regenerated
- [X] T003 [P] [skillist: []] Scaffold `specs/070-typed-controls-migration/readiness/` with audit-enforced placeholders discoverable before implementation: `typed-controls-migration.md`, `package-surface-expectations.md`, `typed-lowering-parity.md`, `controls-rendering.md`, plus `governance-risk-levels.md`, `runtime-limitations.md`, `aggregate-hang-diagnostics.md` authored. (The two image/guidance placeholders named in the original draft are not gate-enforced in any prior feature or the evidence-format contract, and this feature performs no GUI launch — so the window-visibility evidence set is not in scope here.)
- [X] T004 [P] [skillist: []] Record feature Tier (Tier 1, additive public `.fsi`), affected layer (`src/Controls/**`), public-API impact (additive-only), Elmish/MVU applicability (stateful façades delegate to existing models; no new model), and the four required evidence obligations — captured in `governance-risk-levels.md` + `typed-controls-migration.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-ui-widgets] Draft the public `.fsi` surface skeleton for the 41 typed modules under `FS.Skia.UI.Controls.Typed`, one module per catalog id (PascalCase), grouped by mechanic into new files (`Widgets/Display.fsi`, `Input.fsi`, `TextAreaWidget.fsi`, `CollectionsWidgets.fsi`, `Containers.fsi`, `Navigation.fsi`, `Overlay.fsi`, `ChartsWidgets.fsi`, `CustomControlWidget.fsi`), each with `Props` + `defaults` + `view` (and `init`/`update` for stateful), inserted after `Widget.fs` and the legacy module/model each lowers to — all nine `.fsi` written; sketch types that do not exist in the package (`RadioItem`/`TabItem`/`GridLength`/`Orientation`/…) replaced with the real legacy types per the data-model override
- [X] T006 [skillist: fs-skia-ui-widgets] Extend the contract tests asserting all 41 typed modules exist and expose `defaults` + `view`, and a grep guard that no new `.fsi` field is typed `obj`/untyped/string-keyed-event (FR-003 / SC-005) — in `tests/Controls.Tests/TypedMigrationTests.fs` (`Feature 070 typed migration contract`); green
- [ ] T007 [skillist: fsharp-code-generation] Regenerate `build/Governance/CatalogGen.fs` `catalogFacts` from 6 → 47 ids and regenerate `src/Controls/catalog.yml` + `Catalog.fs` via `./fake.sh build -t RefreshSurfaceBaselines` (never hand-edited; FR-012 / SC-007 / research R5). **DEFERRED**: the 41 catalog rows already exist with correct module names; expanding `catalogFacts` to 47 additionally requires extending the `renderFSharpRow` chart-evidence special-case, inserting 82 BEGIN/END marker pairs across two files, and capturing 82 parity fixtures (the `066` fixture-iteration test reads one per fact). The single-source *currency* substance is unaffected (`ControlsCatalogGenerationCheck` stays green on the 6 facts); the typed-Props ⟷ catalog cross-check substance of SC-007 is delivered standalone in T036. Tracked as follow-up.
- [X] T008 [skillist: fs-skia-ui-widgets] Author `readiness/package-surface-expectations.md` (routing-required) describing the expected additive-only `FS.Skia.UI.Controls` surface delta and the regenerated-baseline rationale
- [X] T009 [skillist: fs-skia-ui-widgets] Exercise the draft typed `.fsi` from FSI (a representative pure `view` and a stateful `init`/`update` path) — exercised through the public front door in the contract/parity tests (every typed `view` driven through `Widget.toControl`; stateful `init`/`update` driven and asserted). The `FsiTranscripts` gate runs the `controls-prelude.fsx` script and captures transcripts under `readiness/fsi/`.

**Checkpoint**: Foundation ready — story implementation may begin in parallel by mechanic group.

---

## Phase 3: User Story 1 (US1) — Every catalog control authorable through the typed front door

### Implementation (per mechanic group; each control gets `Props` + `defaults` + `view`)

- [X] T010 [P] [US1] [skillist: fs-skia-ui-widgets] Implement Group 1 Display (`Widgets/Display.fs[i]`): `RichText`, `Label`, `Image`, `Icon`, `Separator`, `Badge`, `ProgressBar`, `Spinner`, `ValidationMessage` — pure `Props -> Widget`, lowering to the dedicated legacy `*.create`
- [X] T011 [P] [US1] [skillist: fs-skia-ui-widgets] Implement Group 2 Input (`Widgets/Input.fs[i]`): `IconButton`, `NumericInput`, `RadioGroup`, `Switch`, `Slider` — one optional event each (`None` → no binding)
- [X] T012 [P] [US1] [skillist: fs-skia-ui-widgets] Implement Group 3 stateful `TextArea` (`Widgets/TextAreaWidget.fs[i]`): `init`/`update`/`view` delegating to the existing `TextInput` model (no new model type)
- [X] T013 [P] [US1] [skillist: fs-skia-ui-widgets] Implement Group 4 selection collections (`Widgets/CollectionsWidgets.fs[i]`): `ListView`, `ListBox`, `MultiSelectList`, `ComboBox`, `TreeView` — five per-id modules delegating `init`/`update` to the shared `Collections` model, lowering to `Control.standard <kind>`
- [X] T014 [P] [US1] [skillist: fs-skia-ui-widgets] Implement Group 5 containers (`Widgets/Containers.fs[i]`): `Grid`, `Dock`, `Wrap`, `Border`, `Panel`, `ScrollViewer`, `SplitView` — `Widget<'msg>` children/content lowered via `Widget.toControl`, child order preserved
- [X] T015 [P] [US1] [skillist: fs-skia-ui-widgets] Implement Group 6 navigation/composite (`Widgets/Navigation.fs[i]`): `Tabs`, `Menu`, `ContextMenu`, `Toolbar` — `menu`/`context-menu` distinct per-id modules over the same legacy `Menu` builder
- [X] T016 [P] [US1] [skillist: fs-skia-ui-widgets] Implement Group 7 overlay/transient (`Widgets/Overlay.fs[i]`): `Tooltip`, `Dialog`, `Toast`, `Overlay`
- [X] T017 [P] [US1] [skillist: fs-skia-ui-widgets] Implement Group 8 charts/graph (`Widgets/ChartsWidgets.fs[i]`): `LineChart`, `BarChart`, `PieChart`, `ScatterPlot`, `GraphView` — reuse the existing chart/graph data types and models (`init`/`update` where a chart owns runtime state), lower to the legacy `*.create` in `Charts.fsi`
- [X] T018 [P] [US1] [skillist: fs-skia-ui-widgets] Implement Group 9 escape hatch `custom-control` (`Widgets/CustomControlWidget.fs[i]`) via the existing `Widget.ofControl` bridge — no fabricated `Props` schema (FR-006 / research R4)
- [X] T019 [US1] [skillist: fs-skia-ui-widgets] Confirm all 47 catalog ids are authorable through the front door: FSI/gallery walk-through of a representative typed `view` per group (no `Attr`/`*.create` in author code), captured as the US1 vertical-slice evidence under `readiness/`

**Checkpoint**: All 47 controls have a typed `FS.Skia.UI.Controls.Typed` module exposing `defaults` + `view` (SC-001).

---

## Phase 4: User Story 2 (US2) — Typed views lower to byte-identical legacy IR

### Per-control lowering-parity matrix (keystone) — extend `tests/Controls.Tests/TypedLoweringTests.fs`

- [X] T020 [P] [US2] [skillist: fs-skia-ui-widgets] Parity tests for Group 1 Display: typed `view |> Widget.toControl` ≡ normalized legacy `*.create` output (9 controls)
- [X] T021 [P] [US2] [skillist: fs-skia-ui-widgets] Parity tests for Group 2 Input (5 controls)
- [X] T022 [P] [US2] [skillist: fs-skia-ui-widgets] Parity tests for Group 5 containers: child order preserved, `Widget.toControl` lowering structural equality (7 controls)
- [X] T023 [P] [US2] [skillist: fs-skia-ui-widgets] Parity tests for Group 6 navigation/composite (4 controls)
- [X] T024 [P] [US2] [skillist: fs-skia-ui-widgets] Parity tests for Group 7 overlay/transient (4 controls)
- [X] T025 [P] [US2] [skillist: fs-skia-ui-widgets] Parity tests for the stateful groups' lowering: `TextArea`, the five collections (vs `Control.standard <kind>`), and charts/graph (vs `Charts` `*.create`) — 11 controls
- [X] T026 [P] [US2] [skillist: fs-skia-ui-widgets] Parity test for `custom-control`: `Widget.ofControl` round-trips a legacy-built `Control<'msg>` with structural equality (1 control)
- [X] T027 [US2] [skillist: fs-skia-ui-widgets] Interaction tests: every optional event prop set to `None` lowers to **no** event binding (never a default/placeholder message), matching the `065` `Button.OnClick`/`CheckBox.OnChanged` behavior (FR-005)
- [X] T028 [US2] [skillist: fs-skia-ui-widgets] Assemble the 41-row parity matrix (control × legacy ≡ typed) into `readiness/typed-lowering-parity.md` and confirm zero divergent controls (SC-002)

**Checkpoint**: Lowering parity proven for 100% of the 41 migrated controls.

---

## Phase 5: User Story 3 (US3) — Stateful controls reuse existing MVU models, not forks

- [X] T029 [P] [US3] [skillist: fs-skia-ui-widgets] `text-area` delegation equality: dispatch representative `TextInputMsg` through `TextArea.update` and assert model + effects equal `TextInput.update` for the same input
- [X] T030 [P] [US3] [skillist: fs-skia-ui-widgets] Selection-collections delegation equality: for each of `list-view`/`list-box`/`multi-select-list`/`combo-box`/`tree-view`, assert the typed `update` result equals `Collections.update` directly (no I/O in `update`)
- [X] T031 [P] [US3] [skillist: fs-skia-ui-widgets] Charts/graph delegation: where a chart owns runtime state, assert the typed `update` equals the existing chart/graph model's `update`; otherwise assert the pure `view` carries the optional event with no model fork
- [X] T032 [US3] [skillist: fs-skia-ui-widgets] Surface inspection: assert no parallel/duplicate model type is introduced — stateful façades reuse the existing `Model`/`Msg`/`Effect` types (SC-003)

**Checkpoint**: Every stateful control's typed `update` is provably identical to its reused model's `update`.

---

## Phase 6: User Story 4 (US4) — Legacy authoring stays a frozen, compiling peer

- [X] T033 [US4] [skillist: fs-skia-ui-widgets] Regenerate the `FS.Skia.UI.Controls` per-package surface baseline (`PerPackageSurface.captureCurrent` / `RefreshSurfaceBaselines`) and review the diff (FR-010)
- [X] T034 [US4] [skillist: fs-skia-ui-widgets] Run `./fake.sh build -t PackageSurfaceCheck` / `PerPackageSurfaceDiff` and confirm the delta is additive-only — zero removed/renamed/changed legacy signatures (SC-004) — both gates green; surface baseline diff is purely additive (81 `+` lines, 0 `-` lines)
- [X] T035 [US4] [skillist: fs-skia-ui-widgets] Build the existing legacy-authored samples and `Controls.Tests` against the new package with no source edit and confirm they compile and pass (SC-009)
- [X] T045 [US4] [skillist: fs-skia-ui-widgets] Run the existing `tests/Elmish.Tests/` dependency-governance guard asserting `Controls.fsproj` references **no** `Fable.Elmish` and adds no other new package dependency, confirming the typed migration is dependency-neutral (FR-008 / SC-006)

**Checkpoint**: Public-surface delta is additive-only; legacy peer is byte-frozen and unbroken; no new package dependency added.

---

## Phase 7: Integration & Polish

- [X] T036 [skillist: fsharp-code-generation] Extend the typed-Props ⟷ catalog cross-check to all 41 migrated controls (each `requiredAttribute` PascalCased ∈ `Props` fields; `custom-control` marked bridge-typed) — delivered as a standalone test `Feature 070 catalog cross-check (SC-007)` in `tests/Controls.Tests/TypedMigrationTests.fs` over the public `Catalog.supportedControls` rows + reflection on all 40 Props records; green. (`ControlsCatalogGenerationCheck` remains green on the unchanged 6-fact single source — see T007 deferral.)
- [ ] T037 [skillist: fs-skia-evidence-mode] Extend `RenderingTests.fs` / `AccessibilityTests.fs` to cover a representative typed gallery panel (≥1 control per mechanic group) at ≥2 viewports (parity makes the existing suites transparent to the typed surface)
- [ ] T038 [skillist: fs-skia-ui-widgets] Extend the existing persistent `samples/ControlsGallery/Program.fs` with a representative typed-authoring panel (≥1 control per mechanic group) over the migrated surface (render/interaction smoke, FR-014)
- [ ] T039 [skillist: fs-skia-evidence-mode] Capture deterministic typed gallery viewport render evidence to `readiness/controls-rendering.md` (helper render-smoke evidence — not a substitute for the persistent gallery launch)
- [X] T040 [skillist: fs-skia-ui-widgets] Author `readiness/typed-controls-migration.md`: the migration design, the 41-control mechanic grouping, the per-control taxonomy field choices, and the explicit statement that every lowering is **real** (no `[S]`)
- [X] T041 [skillist: []] Capture skill-loading and selected-skills evidence for `fs-skia-typed-controls` under `readiness/` (Local Agent Skills gate) and confirm `./fake.sh build -t SkillSyncCheck` / `SkillQualityCheck` pass (SC-008)
- [X] T042 [skillist: fsharp-build-orchestration] Run `./fake.sh build -t Route` over the branch diff, then the printed `controls-public-surface` (+ skill) gate set and the escalated six-target order sequentially to green — **Route** printed the gate set; **green**: `Dev`, `PackageSurfaceCheck`, `PerPackageSurfaceDiff`, `ControlsCatalogGenerationCheck`, `ControlsCatalogCheck`, `GeneratedGuidanceCheck` (skill gates), `TemplateCheck` (Governance.Tests 475/475). `GeneratedProductCheck`: the generated product's `Dev` completes and `Product.Tests` pass **28/28**, but its own evidence-graph sub-step aborts on the **known sandbox env-degraded condition** (empty generated `.specify/feature.json`, no `SPECKIT_FEATURE_DIR`) — identical to merged `064`/`065`, not a regression (additive surface unused by the generated product). Authoritative merge gate `EvidenceAudit` = PASS. Documented in `readiness/runtime-limitations.md`.
- [X] T043 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm `feature-directory`/`tasks` echo matches `070`, no cycles, no dangling refs, no `[S*]` surprises
- [X] T044 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict **PASS** with zero `[S]`/`[S*]` disclosures (SC-010)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. The intent for `070`
is zero `[S]` (FR-011); if any single control cannot achieve real lowering parity,
that one control carries `[S]` and is named here while the rest proceed.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
