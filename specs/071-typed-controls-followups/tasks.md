# Tasks: Close Out the Deferred Typed-Controls-Migration Follow-Ups

**Feature branch**: `071-typed-controls-followups`
**Spec**: `specs/071-typed-controls-followups/spec.md`
**Plan**: `specs/071-typed-controls-followups/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/evidence-graph.md` for the propagated view.

**No `[S]` / `[S*]` / `[SEH]` is planned for this feature.** Catalog generation
runs against the real fact table and real on-disk artifacts; the parity fixtures
are captured **from** the generator's real output (golden bytes, not fabricated
literals); the render evidence is real render-only output through the existing IR
path (FR-009, SC-006/SC-008). The Synthetic-Evidence Inventory below stays empty.

## Vertical-slice rule (US phases)

A `[US*]` task may only be marked `[X]` when the change is reachable from a
user-facing entry point and that path was actually exercised. For US1 the
maintainer-facing path is the regenerated catalog + the currency gate biting on a
hand-edit (T011). For US2 the consumer-facing path is the persistent
`ControlsGallery` launch showing the typed panel (T015) — a passing render/unit
test alone does **not** satisfy `[X]` for the US2 launch task.

Principle IV (Elmish/MVU) evidence is **not applicable** to a new contract here:
this feature adds no `Model`/`Msg`/`Effect`. The typed gallery panel reuses the
already-shipped `070` typed façades and their existing pure `update` models; no
I/O is added to any `update` (recorded in T002).

## Success-criterion → assertion mapping

Each mechanically-testable success criterion is pinned to a concrete enforcing
assertion so a headline SC cannot be silently violated while gates stay green:

- **SC-001/SC-002** (all 47 generated, currency enforced) → `T011` proves
  `ControlsCatalogGenerationCheck` fails on a deliberate hand-edit naming the
  stale `typed-catalog/<id>` region + regen command, then passes after revert.
- **SC-003** (cross-check over 47) → `T006`'s `CatalogTests.fs` assertions
  (`catalogFacts` ids == typed ids; each `requiredAttribute` PascalCased ∈ `Props`
  fields; one fixture per fact) fail RED on the 6-fact table, pass after T010.
- **SC-005** (≥2 viewports) → `T013`'s `RenderingTests.fs` / `AccessibilityTests.fs`
  cases assert the typed panel at two viewport sizes, RED before the panel exists.
- **SC-006** (deterministic render-only) → `T017` re-captures byte-identically.
- **SC-007** (additive-only surface) → `T018` `PackageSurfaceCheck` /
  `PerPackageSurfaceDiff` delta is additive-only or empty.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]** — user-story scope
- Feature is **Tier 2 (internal change)** throughout; per-task `[T2]` omitted
  because every phase matches the spec tier.

Every task line mirrors its structured `tasks.deps.yml` `skillist` as
`[skillist: ...]` (`[skillist: []]` when empty), in exact structured order.

## Governance risk levels

- **Small**: T006–T008 (test edit + fact-table edit) — focused `./fake.sh build
  -t Dev` is sufficient.
- **Medium**: T009–T010 (generated governance artifacts + fixtures) — add
  `ControlsCatalogGenerationCheck` currency proof (T011) and the per-package
  surface review (T018).
- **Broad**: T019–T021 — `Route` re-run on the full implementation diff, then the
  serialized FAKE-backed gate order it prints, finishing on `EvidenceGraph` +
  `EvidenceAudit`. Broad validation is required only at close-out; aggregate
  FAKE-backed results are non-authoritative and recorded under `readiness/logs/`
  and `readiness/evidence-audit.md`. FAKE-backed targets share `.fake` state — run
  them **sequentially**, never concurrently.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory is scaffolded and `spec.md` / `plan.md` / `research.md` / `data-model.md` / `quickstart.md` / `contracts/` are linked and current
- [X] T002 [P] [skillist: []] Record feature Tier (T2, internal), affected layer (build-side fact table + generated governance artifacts + tests/sample/evidence — no shipped public `.fsi`), public-API impact (none; additive-only per-package surface), Elmish/MVU applicability (**Principle IV not applicable** — reuses `070` façades, no new `Model`/`Msg`/`Effect`, no I/O added to any `update`), and the evidence obligations (`readiness/catalog-single-source.md`, `readiness/controls-rendering.md`, parity fixtures under `specs/066-typed-catalog-generation/readiness/parity-fixtures/`, gate evidence) — recorded in `readiness/feature-scope.md`
- [X] T003 [P] [skillist: []] Add readiness placeholders discoverable before implementation: `readiness/catalog-single-source.md`, `readiness/controls-rendering.md`, `readiness/governance-risk-levels.md`, `readiness/runtime-limitations.md`, `readiness/skill-loading-evidence-workflow.md`, `readiness/evidence-graph.md`, `readiness/evidence-audit.md` — each naming its authoritative command, artifact path, failure class, and next action

---

## Phase 2: Foundation

- [X] T004 [skillist: []] Record the surface invariant: no shipped public `FS.Skia.UI.Controls` `.fsi` signature changes (the 41 typed modules shipped in `070`); the catalog single source (`catalog.yml`/`Catalog.fs`) and the fact table are generated/internal cross-check inputs, so the per-package surface baseline delta MUST be additive-only or empty (FR-010, SC-007, contract C11)
- [X] T005 [P] [skillist: []] Record unsupported-scope handling and failure diagnostics in `readiness/runtime-limitations.md`: the currency gate names the stale `typed-catalog/<id>` region + the `./fake.sh build -t RefreshSurfaceBaselines` command on drift; the `066` fixture-iteration names the missing fixture id on a gap; render evidence is render-only (no GPU window) through the `Widget.toControl` IR path — and the deferred-scope boundary (no catalog expansion / overlays / virtualization / motion, FR-011)

**Checkpoint**: Foundation ready — US1 and US2 share no code; do US1 (P1) first.

---

## Phase 3: User Story 1 (US1) — Every catalog row generated from the single fact source

### Tests First (Principle I, Principle VI)

- [X] T006 [P] [US1] [skillist: fs-skia-typed-controls] Extend the `066` catalog cross-check in `tests/Controls.Tests/CatalogTests.fs`: grow `typedPropsById` toward all 47 typed ids and assert `catalogFacts` ids == typed ids, each non-`custom-control` `requiredAttribute` PascalCased ∈ that control's `Props` fields, `custom-control` excluded from the Props-field assertion, and one fixture exists per fact — confirm RED on the 6-fact table / missing fixtures (contracts C8/C9/C10, SC-003)

### Implementation

- [X] T007 [US1] [skillist: fsharp-code-generation] Extend `CatalogGen.catalogFacts` from 6 → all 47 ids in `build/Governance/CatalogGen.fs`, copying each control's facts (id, display name, category, module, purpose, required attributes, events, accessibility role) from the matching hand-maintained row; set `RequiredAttributes = []` for `custom-control` (FR-001/FR-006, contract C6)
- [X] T008 [US1] [skillist: fsharp-code-generation] Generalize the `renderFSharpRow` (and YAML) chart/data-grid evidence special-case from `fact.Id = "data-grid"` to membership in `{ data-grid; line-chart; bar-chart; pie-chart; scatter-plot; graph-view }` so exactly those six rows append `|> withChartDataGridEvidence` / the YAML evidence path and no other row does (FR-004, contracts C4/C5)
- [X] T009 [US1] [skillist: fsharp-code-generation] Regenerate `src/Controls/catalog.yml` + `src/Controls/Catalog.fs` via `./fake.sh build -t RefreshSurfaceBaselines`; confirm 47 generator-emitted `BEGIN/END GENERATED: typed-catalog/<id>` regions, **zero** rows hand-maintained outside markers, and that the 41 new regions' inner bytes match the previously hand-maintained rows (markers-only diff) (FR-001/FR-002, contracts C1/C7, SC-001)
- [X] T010 [US1] [skillist: fsharp-code-generation] Capture the 41 new parity-fixture pairs (`Catalog.fs.<id>.txt` + `catalog.yml.<id>.txt`, 82 files) into `specs/066-typed-catalog-generation/readiness/parity-fixtures/` from real `renderFSharpRow` / `renderYamlRow` output, trailing newline trimmed as the test does (FR-005, data-model E5)
- [X] T011 [US1] [skillist: []] Run `./fake.sh build -t Dev` (Controls.Tests incl. `CatalogTests.fs` green over 47) then prove the currency gate bites: hand-edit one generated region, run `ControlsCatalogGenerationCheck`, confirm it fails naming the stale `typed-catalog/<id>` region + the regen command; revert and confirm green (FR-003, contracts C2/C3, SC-002)
- [X] T012 [US1] [skillist: []] Write `readiness/catalog-single-source.md` — the 6→47 fact-table extension, the regeneration rationale, the six evidence-carrying ids the special-case covers, and the statement that all 47 rows are generated (zero hand-maintained)

**Checkpoint**: US1 functional — catalog single source enforced over all 47.

---

## Phase 4: User Story 2 (US2) — Typed-authored gallery panel rendered, tested, evidenced

### Tests First (Principle I, Principle VI)

- [X] T013 [P] [US2] [skillist: fs-skia-ui-widgets] Add `tests/Controls.Tests/RenderingTests.fs` + `AccessibilityTests.fs` cases that render/assert the typed gallery panel at ≥2 viewports through the existing render path (mirroring the existing viewport-coverage + typed-vs-legacy parity tests); confirm RED before the panel exists (contracts G5/G6/G7, FR-008, SC-005)

### Implementation

- [X] T014 [US2] [skillist: fs-skia-typed-controls, fs-skia-ui-widgets] Extend `typedAuthoringPanel` in `samples/ControlsGallery/Program.fs` from {TextBlock, Button, CheckBox} to ≥1 control per mechanic group (display, input, stateful input, layout container, navigation/composite, overlay, selection collection, charts/graph) authored **only** through `FS.Skia.UI.Controls.Typed.*` `view` functions — no `Attr`, no `*.create` call; stateful controls reuse the shipped `070` MVU models. Resolve "≥1 per group" against the **mechanic-group → catalog `Category` crosswalk** in `contracts/typed-gallery-panel.contract.md` (the 8 gallery groups map onto the 11-value catalog taxonomy; `data`/`feedback`/`custom` are not required groups) (FR-007/SC-004, contracts G1/G2/G4)
- [X] T015 [US2] [skillist: fs-skia-ui-widgets] Launch the persistent `ControlsGallery` default executable and confirm the typed-authored panel renders alongside the existing panels as a real render/interaction smoke over the migrated surface (FR-007 AS2, contract G3) — this is the user-reachable entry point required for `[X]` on US2
- [X] T016 [US2] [skillist: []] Run `./fake.sh build -t Dev` and confirm the rendering + accessibility suites pass over the typed panel at ≥2 viewports with expected accessibility roles (FR-008, contracts G5/G6, SC-005)
- [X] T017 [US2] [skillist: fs-skia-evidence-mode] Capture deterministic typed-gallery viewport render evidence to `readiness/controls-rendering.md` — render-only, ≥2 viewports, **no** `[S]`/`[S*]` disclosure; re-run the capture and confirm byte-identical output. Record the per-group satisfying control ids (per the `Category` crosswalk) so SC-004 coverage is auditable from the evidence (FR-009/SC-006, contracts G8/G9/G10)

**Checkpoint**: US2 functional — typed panel rendered, covered at ≥2 viewports, evidenced.

---

## Phase 5: Integration & Polish

- [X] T018 [skillist: []] Run `./fake.sh build -t PackageSurfaceCheck` / `PerPackageSurfaceDiff` and confirm the `FS.Skia.UI.Controls` per-package surface baseline delta is additive-only or empty — no shipped public signature changed (FR-010/SC-007, contract C11)
- [X] T019 [skillist: []] Run `./fake.sh build -t Route` on the full implementation diff and run **exactly** the gates it prints, FAKE-backed targets sequentially in deterministic order (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit` when escalated); record the aggregate non-authoritative result under `readiness/logs/` (SC-008)
- [X] T020 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the DAG is acyclic with no dangling refs and no `[S*]` surprises, the echoed `feature-directory=` / `tasks=` match this feature, and write `readiness/evidence-graph.md`
- [X] T021 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict **PASS** with no `[S]`/`[S*]` disclosures, and write `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. **None planned** for
this feature — all evidence is real (real fact table, golden generator-output
fixtures, render-only IR output).

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
