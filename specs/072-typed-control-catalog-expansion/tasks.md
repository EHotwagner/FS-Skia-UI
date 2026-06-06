# Tasks: Catalog Expansion — New Typed Controls (Buttons / Pickers / Date-Time)

**Feature branch**: `072-typed-control-catalog-expansion`
**Spec**: `specs/072-typed-control-catalog-expansion/spec.md`
**Plan**: `specs/072-typed-control-catalog-expansion/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

**No `[S]` / `[S*]` / `[SEH]` is planned for this feature.** Lowering parity is
proven against real composed IR, catalog rows are generated from the real fact
table, render evidence is real render-only output through the existing IR path,
and parity fixtures are golden bytes captured from the generator (plan
Constitution Check → Synthetic evidence). `EvidenceAudit` must be PASS with no
disclosures (SC-006).

## Vertical-slice rule (US phases)

A `[US*]` task may only be marked `[X]` when the new control is reachable from a
user-facing entry point and that path was actually exercised — here, the
`samples/ControlsGallery` typed-authoring panel rendered through the real IR
path and/or a captured render-evidence image under `readiness/`. A typed module
that compiles but is not dogfooded in the gallery is `[ ]` or `[S]`, never `[X]`.

These controls are **stateless from the framework's view** (Principle IV: no new
`Model`/`Msg`/`Effect`; values are product-owned in `Props`). The MVU evidence
obligation is therefore satisfied by the interaction tests asserting each typed
callback dispatches its message (`OnToggle`/`OnChange`/`OnSelected`/`OnClick`),
not by a new pure-`update` suite.

## Success-criterion → assertion mapping

- **SC-002** (lowering parity) → the per-control parity tests in
  `TypedExpansionTests.fs` (`view props |> Widget.toControl` ≡ explicit
  composition, order-normalized, events canonicalized): T012, T024.
- **SC-003** (catalog single-source currency) → `CatalogTests` `typedPropsById`
  cross-check + `supportedCount` 47→52 (T019) and the deliberate hand-edit →
  `ControlsCatalogGenerationCheck` fails → revert → passes proof (T022).
- **SC-001** (typed authoring compiles, no string keys / `obj`) → DatePicker /
  remaining-controls implementation compiling against `Props` + `defaults`:
  T015, T027, T028.
- **SC-004** (existing signatures byte-unchanged, additive-only delta) → the
  surface-baseline refresh + `PackageSurfaceCheck`/`PerPackageSurfaceDiff`: T032.
- **SC-007** (no new dependency / `StandardControlKind` variant / MVU primitive) →
  implementation tasks composing existing builders only: T015, T027, T028, verified at T034.
- **SC-005** (≥2-viewport render + accessibility metadata, stable node counts) → the
  rendering/accessibility coverage and captured render evidence: T014, T016, T026, T029.
- **SC-006** (`Route --enforce` prints the escalated `controls-public-surface` path; required
  evidence present) → T034 (with the gate runs at T033, T036, T037).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- **[T1]** — Tier 1 (contracted change); the whole feature is Tier 1, so the
  per-task tier annotation is omitted (matches the spec-level tier).

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors the
structured `skillist` as `[skillist: ...]` (exact order); `[skillist: []]` when
no capability skill applies.

## Canonical Verification Targets

FAKE-backed commands (`./fake.sh`, `fake.cmd`, `dotnet fake`) share `.fake`
state and are **not** safe to run concurrently — serialize them. `Route` is
authoritative: run `./fake.sh build -t Route` on the implementation diff and run
only the gates it prints. Public `.fsi` + catalog facts escalate to
`controls-public-surface`. The escalated serialized order:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

plus the printed controls gates (`ControlsCatalogCheck`,
`ControlsInteractionCheck`, `ControlsRenderingCheck`, `PackageSurfaceCheck`,
`FsiTranscripts`, `GeneratedProductCheck`), `ControlsCatalogGenerationCheck`
(catalog currency), and `DesignTokenDrift` (must stay green). Intentional
surface/catalog baseline refresh uses `./fake.sh build -t RefreshSurfaceBaselines`.

**Governance risk levels**: catalog/typed-surface additions are a **small/medium**
governance risk (additive-only, no renderer/IR change) — focused validation is
the `controls-public-surface` gate set above; **broad** validation (the full
serialized six-target order) is required only because public `.fsi` escalates the
route. Non-authoritative aggregate results (e.g. `GeneratedProductCheck`'s known
local environment failure) are recorded as such, not as product regressions.

Template source: `.specify/presets/fsharp-opinionated/templates/tasks-template.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm branch `072-typed-control-catalog-expansion` and link spec, plan, research, data-model, and quickstart in `specs/072-typed-control-catalog-expansion/`
- [X] T002 [P] [skillist: []] Scaffold `specs/072-typed-control-catalog-expansion/readiness/` with the audit-enforced placeholder files discoverable before implementation: `typed-controls-front-door.md`, `package-surface-expectations.md`, `controls-rendering.md`, `typed-lowering-parity.md`, `control-catalog-generation.md`, `catalog-single-source.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `skill-loading-evidence.md`, `evidence-graph.md`, `evidence-audit.md` — each naming the authoritative command, artifact path, failure class, and next action
- [X] T003 [P] [skillist: []] Scaffold the per-id golden-fixture target directory `specs/066-typed-catalog-generation/readiness/parity-fixtures/` and the `072` parity matrix slot for the 5 new ids (`toggle-button`, `split-button`, `date-picker`, `time-picker`, `color-picker`)
- [X] T004 [skillist: fs-skia-typed-controls] Record feature Tier 1, affected layer (`FS.Skia.UI.Controls`), additive public-API impact, Principle IV applicability (no new `Model`/`Msg`/`Effect` — values product-owned in `Props`, mirroring `CheckBox`), and the **no-`[S]`** evidence obligations in `readiness/typed-controls-front-door.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-typed-controls] Draft the additive public `.fsi` surface — `src/Controls/Widgets/Buttons.fsi` (`ToggleButton`, `SplitButton`, `SplitButtonItem`) and `src/Controls/Widgets/Pickers.fsi` (`DatePicker`, `TimePicker`, `ColorPicker`, `ColorSwatch`): each `Props<'msg>` record, `defaults`, and `view : Props<'msg> -> Widget<'msg>` per `data-model.md`; no existing signature changes
- [X] T006 [P] [skillist: fsharp-code-generation] Add the 5 catalog facts (`toggle-button`, `split-button`, `date-picker`, `time-picker`, `color-picker`) to `build/Governance/CatalogGen.fs` `catalogFacts` (47→52) with their `Module` / category / `RequiredAttributes` / `Events` / `AccessibilityRole` per `data-model.md` — the single source; no generator-mechanism change
- [X] T007 [P] [skillist: fsharp-code-generation] Place the 5 `BEGIN/END GENERATED: typed-catalog/<id>` marker pairs in `src/Controls/catalog.yml` and `src/Controls/Catalog.fs` so the splice has regions to replace (generation itself is proven in US2)
- [X] T008 [skillist: fs-skia-ui-widgets] Add the 4 new compile entries (`Widgets/Buttons.fsi`, `Widgets/Buttons.fs`, `Widgets/Pickers.fsi`, `Widgets/Pickers.fs`) to `src/Controls/Controls.fsproj` after the existing `Widgets/*` block
- [X] T009 [skillist: []] Exercise the draft `.fsi` from FSI (`Props` + `defaults` for each of the 5 modules) and capture the session transcript to `readiness/fsi-session.txt`
- [X] T010 [skillist: fs-skia-typed-controls] Record the additive surface-area delta (new modules / `Props` records / `SplitButtonItem` / `ColorSwatch`) and the regenerated-baseline rationale in `readiness/package-surface-expectations.md`
- [X] T011 [skillist: fs-skia-evidence-mode] Record unsupported-scope handling, runtime limitations, the small/medium/broad governance risk levels, and aggregate-hang diagnostics in `readiness/runtime-limitations.md`, `readiness/governance-risk-levels.md`, and `readiness/aggregate-hang-diagnostics.md`

**Checkpoint**: Foundation ready — the typed surface, catalog fact table, splice markers, and project wiring exist; story implementation may begin.

---

## Phase 3: User Story 1 (US1) — A product author uses the new DatePicker through the typed front door (P1 keystone)

### Tests First (Principle I, Principle VI)

- [X] T012 [P] [US1] [skillist: fs-skia-typed-controls] Add the red-first lowering-parity test for `DatePicker` in `tests/Controls.Tests/TypedExpansionTests.fs` — `DatePicker.view props |> Widget.toControl` ≡ the explicit hand-written composition of existing legacy builders (field + trigger `Button` + `Overlay` calendar `Stack`/`Grid` of day `Button`s), order-normalized, events canonicalized (SC-002)
- [X] T013 [P] [US1] [skillist: fs-skia-typed-controls] Add the red-first interaction test in `tests/Controls.Tests/InteractionTests.fs` — selecting a day dispatches `OnChange` carrying the chosen `DateOnly`; `Value = None` renders an empty field and dispatches nothing; `OnChange = None` lowers to no binding
- [X] T014 [P] [US1] [skillist: fs-skia-evidence-mode] Add red-first rendering + accessibility coverage for `DatePicker` at ≥2 viewports (role `TextBox`, keyboard affordance — focusable trigger + activation/arrow keys, stable node counts) in `tests/Controls.Tests/RenderingTests.fs` and `tests/Controls.Tests/AccessibilityTests.fs`

### Implementation

- [X] T015 [US1] [skillist: fs-skia-typed-controls] Implement `DatePicker.view` in `src/Controls/Widgets/Pickers.fs` composing existing legacy builders only (no new `StandardControlKind` variant): `Border`/`Stack` of [ field showing the formatted `Value` or placeholder; trigger `Button` ] plus an `Overlay` calendar popup when `IsOpen`; green T012–T014 (SC-001, SC-007)
- [X] T016 [US1] [skillist: fs-skia-evidence-mode] Capture deterministic render-only evidence for `DatePicker` and record it in `readiness/controls-rendering.md`, plus the real-lowering parity statement (explicitly **no `[S]`**) in `readiness/typed-controls-front-door.md`
- [X] T017 [US1] [skillist: fs-skia-ui-widgets] Add `DatePicker` to the `samples/ControlsGallery/Program.fs` `typedAuthoringPanel` so the date-time front door is dogfooded end-to-end (FR-010)
- [X] T018 [US1] [skillist: []] Document the `DatePicker` independent validation path (author panel → render ≥2 viewports → parity → `OnChange` dispatch) in `readiness/typed-controls-front-door.md`

**Checkpoint**: User Story 1 is fully functional and testable independently.

---

## Phase 4: User Story 2 (US2) — A maintainer adds the new catalog rows from the single fact source (P1)

### Tests First

- [X] T019 [P] [US2] [skillist: fsharp-code-generation] Extend the catalog cross-check (red-first): add the 5 new ids to `typedPropsById` (each id → its `*Props` type, with every `RequiredAttributes` entry PascalCased present as a `Props` field) and bump the `supportedCount` assertion 47→52 in `tests/Controls.Tests/CatalogTests.fs` (SC-003)

### Implementation

- [X] T020 [US2] [skillist: fsharp-code-generation] Regenerate `src/Controls/catalog.yml` and `src/Controls/Catalog.fs` from the fact table via `./fake.sh build -t RefreshSurfaceBaselines`, bump the `catalog.yml` `supportedCount` header 47→52, and confirm the 5 new rows appear in both artifacts (no row hand-edited)
- [X] T021 [US2] [skillist: fsharp-code-generation] Capture the per-id golden parity fixtures (`Catalog.fs.<id>.txt` + `catalog.yml.<id>.txt`) for the 5 new ids under `specs/066-typed-catalog-generation/readiness/parity-fixtures/`; record a pointer to this cross-feature fixture location in `readiness/typed-lowering-parity.md` so the coupling is discoverable (066 archival must keep these)
- [X] T022 [US2] [skillist: fsharp-build-orchestration] Prove currency: hand-edit one generated new row, confirm `./fake.sh build -t ControlsCatalogGenerationCheck` fails naming the stale `typed-catalog/<id>` region, then revert and confirm it passes; record the proof in `readiness/control-catalog-generation.md` (SC-003)
- [X] T023 [US2] [skillist: []] Document the maintainer single-source add recipe and US2 independent validation path in `readiness/catalog-single-source.md`

**Checkpoint**: User Story 2 is fully functional and testable independently.

---

## Phase 5: User Story 3 (US3) — New button-family controls carry state without a new model; picker/date-time breadth (P2)

### Tests First

- [X] T024 [P] [US3] [skillist: fs-skia-typed-controls] Add the red-first lowering-parity tests for `ToggleButton`, `SplitButton`, `TimePicker`, and `ColorPicker` in `tests/Controls.Tests/TypedExpansionTests.fs` — each `view |> Widget.toControl` ≡ its explicit existing-builder composition, order-normalized, events canonicalized (SC-002)
- [X] T025 [P] [US3] [skillist: fs-skia-typed-controls] Add the red-first interaction tests in `tests/Controls.Tests/InteractionTests.fs` — `ToggleButton` `OnToggle (not IsOn)`; `SplitButton` `OnClick` + `OnSelected key`; `ColorPicker` `OnSelected swatch`; `TimePicker` `OnChange time`; empty `Items`/`Swatches` lower to an empty/disabled popup (must not fail to lower); `None` callbacks lower to no binding
- [X] T026 [P] [US3] [skillist: fs-skia-evidence-mode] Add red-first rendering + accessibility coverage for the 4 controls at ≥2 viewports with roles `Button` / `Menu` / `TextBox` / `List`, each control's keyboard affordance (focusable trigger + activation/arrow keys) asserted, and stable node counts in `tests/Controls.Tests/RenderingTests.fs` and `tests/Controls.Tests/AccessibilityTests.fs`

### Implementation

- [X] T027 [US3] [skillist: fs-skia-typed-controls] Implement `ToggleButton.view` and `SplitButton.view` in `src/Controls/Widgets/Buttons.fs` — product-owned `IsOn` boolean and `Items`/`IsOpen` command-list + `Overlay`/`Menu`; composed from existing builders only, no new `Model`/`Msg`/`Effect`; green T024–T026 (SC-001, SC-007)
- [X] T028 [US3] [skillist: fs-skia-typed-controls] Implement `TimePicker.view` and `ColorPicker.view` in `src/Controls/Widgets/Pickers.fs` — `TimeOnly` segment composition and a `Wrap`/`Grid` of `FS.Skia.UI.Scene.Color` swatch cells (`Selected` highlighted); composed from existing builders only; green T024–T026 (SC-001, SC-007)
- [X] T029 [US3] [skillist: fs-skia-evidence-mode] Capture deterministic render-only evidence for the 4 controls; extend `readiness/controls-rendering.md` and complete the 5-control parity matrix in `readiness/typed-lowering-parity.md` (explicitly **no `[S]`**)
- [X] T030 [US3] [skillist: fs-skia-ui-widgets] Add `ToggleButton`, `SplitButton`, `TimePicker`, and `ColorPicker` to the `samples/ControlsGallery/Program.fs` `typedAuthoringPanel` (FR-010)
- [X] T031 [US3] [skillist: []] Document the US3 independent validation paths (toggle pressed-state, split-button popup menu, swatch grid selection, time segments) in `readiness/typed-controls-front-door.md`

**Checkpoint**: User Story 3 is fully functional and testable independently.

---

## Phase 6: Integration & Polish

- [X] T032 [skillist: fs-skia-typed-controls] Regenerate the controls public-surface and per-package surface baselines (`./fake.sh build -t RefreshSurfaceBaselines` + `PerPackageSurface.captureCurrent`) and confirm the only delta is additions via `PackageSurfaceCheck` / `PerPackageSurfaceDiff`; record `readiness/per-package-surface-diff.md` (SC-004)
- [X] T033 [skillist: fsharp-build-orchestration] Run the focused `controls-public-surface` gates sequentially — `Dev`, `ControlsCatalogGenerationCheck`, `ControlsCatalogCheck`, `ControlsInteractionCheck`, `ControlsRenderingCheck`, `PackageSurfaceCheck`, `FsiTranscripts`, `DesignTokenDrift` — and record the focused-gate list plus non-authoritative aggregate notes in `readiness/focused-gates.md`
- [X] T034 [skillist: fs-skia-typed-controls] Run `./fake.sh build -t Route --enforce` over the branch diff; confirm escalation to `controls-public-surface` and that every required evidence artifact is present and populated (SC-006, SC-007)
- [X] T035 [skillist: []] Record skill-loading evidence and the selected-skills set in `readiness/skill-loading-evidence.md`
- [X] T036 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; record `readiness/evidence-graph.md`
- [X] T037 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with no synthetic disclosures; record `readiness/evidence-audit.md` (SC-006)

---

## Synthetic-Evidence Inventory

No `[S]` / `[SEH]` tasks are planned for this feature (plan Constitution Check →
Synthetic evidence: lowering is real, catalog rows and fixtures are generated
from real sources, render evidence is real render-only output). This table stays
empty unless `/speckit.implement` discovers an unavoidable synthetic path, which
must return to design/task review before being marked.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
