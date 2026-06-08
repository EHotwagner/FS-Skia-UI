# Tasks: Authoritative Controls Catalog in Published Docs

**Feature branch**: `078-controls-doc-catalog`
**Spec**: `specs/078-controls-doc-catalog/spec.md`
**Plan**: `specs/078-controls-doc-catalog/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label, assigned only during design/planning/
task generation. None are required for this feature: the currency-check failure
tests (T010) exercise the **real** pure `catalogDocsCurrency` function against
constructed in-memory fixtures and assert real `Finding` output — real test
evidence, not synthetic product evidence — so they stay `[X]`-capable.

## Vertical-slice rule (US phases)

A `[US*]` task may only be `[X]` when the change is reachable from a user-facing
entry point and that path was actually exercised. Here the user-facing surface is
the **published docs site**: a `[US*]` task is `[X]` only when the relevant page/
asset is present in a real `dotnet fsdocs build --strict --eval` output (or the
governance gate that enforces it passes against the real tree), captured under
`readiness/`. Generator/test green alone does not satisfy `[X]` for a `[US*]`
task.

This feature is **not** an interactive graphical viewer feature: it adds no
default executable / persistent viewer. Per-control previews are produced by the
existing deterministic render-only evidence path as committed source assets
(T014/T015) — bounded helper evidence, **not** interactive graphical readiness.

**MVU/Elmish applicability**: N/A as a runtime concern. The generator and currency
check are pure functions with file I/O at the `Engine/Update.fs` interpreter edge
(Principle IV honored by shape, no framework `Model`/`Msg`/`Effect` change).

## Success-criterion → assertion mapping

- **SC-002** (count exact, zero missing/extra) → T009/T010 byte-identity + currency
  tests against `CatalogGen.catalogFacts`, enforced live by `ControlsCatalogDocsCheck`
  (T019).
- **SC-003** (every control has detail page + resolving preview/API link) →
  currency completeness + link-resolution clauses (T010/T019), preview honesty
  validation (T015), site build (T018).
- **SC-004** (add/rename/remove → index updates, stale caught) → currency
  `IndexStale`/`MissingDetailPage`/`OrphanDetailPage` findings (T010/T019).
- **SC-001/SC-005/SC-006** → reachability + narrative/Penpot verification (T017,
  T023, T025).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**/**[US2]**/**[US3]** — user-story scope
- **[T2]** — Tier 2 (internal) change; this feature changes no public product
  `.fsi`, but escalates on the governance / generated-guidance path.

Every task has a matching entry in `tasks.deps.yml`. Every line mirrors the
structured `skillist` as `[skillist: ...]` (`[skillist: []]` when empty).

## Governance risk levels

- **Small**: prose-only edit to an authored region of one detail page or the
  narrative → focused validation is the local `dotnet fsdocs build` link check.
- **Medium**: regenerating the index/detail-header regions or touching the
  generator → run `ControlsCatalogDocsCheck` + `GeneratedGuidanceCheck`.
- **Broad**: routing/target/`validation.contract.yml`/`knownGates` edits →
  required before merge; run the serialized governance suite (T028) then
  `EvidenceGraph`→`EvidenceAudit`. Aggregate results are recorded
  **non-authoritatively** in `readiness/` (see `generated-product-check-env-failure`
  caveat for any locally-failing generated check).

## Canonical Verification Targets

FAKE-backed commands share `.fake` state and run **sequentially** in deterministic
order. The serialized governance order for this feature:

1. `./fake.sh build -t Route`
2. `./fake.sh build -t Dev`
3. `./fake.sh build -t ControlsCatalogDocsCheck`
4. `./fake.sh build -t GeneratedGuidanceCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Plus `./fake.sh build -t RefreshSurfaceBaselines` for intentional generated-region
refreshes and `dotnet fsdocs build --strict --eval` for the site.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory and `.specify/feature.json` resolve to `specs/078-controls-doc-catalog`; verify the `AGENTS.md` Spec Kit marker points at this plan
- [X] T002 [P] [skillist: []] Create readiness scaffolding under `specs/078-controls-doc-catalog/readiness/` with audit-discoverable placeholders, each naming the authoritative command, artifact path, failure class, and next action: `controls-catalog-docs.md`, `controls-preview-evidence.md`, `docs-build.md`, `visual-evidence-honesty.md`, `real-image-evidence.md`, `window-visibility.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, `evidence-graph.md`, `evidence-audit.md`
- [X] T003 [P] [T2] [skillist: []] Record feature Tier (Tier 2 internal, governance/generated-guidance escalation), affected layer (`build/Governance/**` + `docs/**`), public-API/`.fsi` impact (none), MVU/Elmish applicability (N/A — pure generators at the interpreter edge), and the evidence obligations (generated index, per-control detail pages, per-control preview, currency-check PASS, site build)

---

## Phase 2: Foundation (generator + gate scaffolding)

- [X] T004 [skillist: fsharp-code-generation] Draft `build/Governance/CatalogDocsGen.fsi` build-tool signatures: `renderCatalogIndex`, `renderDetailHeader`, `spliceCatalogDocs`, `catalogDocsCurrency` and the `Finding` discriminated union (closed over the data-model finding classes)
- [X] T005 [P] [skillist: fsharp-build-orchestration] Register `Target.ControlsCatalogDocsCheck` in `build/Governance/Targets.fs`, add it to `AgentValidation.knownGates`, and route `docs/controls/**`, `docs/img/controls/**`, the catalog single source (`CatalogGen.fs` / `src/Controls/catalog.yml`), and the new generator in `Routing.fs`
- [X] T006 [P] [skillist: fsharp-code-generation] Pre-place `BEGIN/END GENERATED: catalog-docs/<key>` marker pairs first (filled, never invented): the index region in `docs/controls/catalog.md` and a header-region marker pair in each of the 52 `docs/controls/<id>.md` detail-page stubs
- [X] T007 [skillist: fsharp-build-orchestration] Regenerate `build/Governance/validation.contract.yml` from `Routing.fs` via `RefreshSurfaceBaselines` and confirm `TargetMetadataDrift` currency (contract is generated, never hand-edited)
- [X] T008 [skillist: []] Record the new gate's unsupported-scope handling and failure diagnostics: each finding class names an actionable remedy + the `RefreshSurfaceBaselines` regenerate command, and missing required artifacts fail loudly via `RequireFiles`

**Checkpoint**: Foundation ready — generator surface, gate routing, and markers in place; story implementation may begin.

---

## Phase 3: User Story 1 — Discover and understand every control (US1, P1)

### Tests First (Principle I, Principle VI)

- [X] T009 [P] [US1] [skillist: fsharp-code-generation] Failing-first test in the `FS.Skia.UI.Build` test project: `renderCatalogIndex`/`renderDetailHeader` round-trip + byte-identity against `CatalogGen.catalogFacts`, and `spliceCatalogDocs` idempotence (replaces only inside existing markers)
- [X] T010 [P] [US1] [skillist: fsharp-io-globbing] Failing-first test: `catalogDocsCurrency` returns the right `Finding` for each class — `IndexStale`, `MissingDetailPage`, `StaleDetailHeader`, `OrphanDetailPage`, `MissingPreview`, `UndecodablePreview`, `OrphanPreview`, `DeadLink` — and an empty list (PASS) on a clean tree (SC-002, SC-004)

### Implementation

- [X] T011 [US1] [skillist: fsharp-code-generation] Implement `renderCatalogIndex`/`renderDetailHeader`/`spliceCatalogDocs`: deterministic, invariant-culture projection grouped by `Category`, DisplayName→`controls/<id>.html`, one-line `Purpose`, total count, and API-reference link derived from `Module` (slug verified per research R2)
- [X] T012 [US1] [skillist: fsharp-io-globbing] Implement the pure `catalogDocsCurrency` core plus its `Engine/Update.fs` edge handler (file read/listing, `WriteStructuredReport`/`FailWith`, `RequireFiles`) and wire region regeneration into the `RefreshSurfaceBaselines` handler
- [X] T013 [US1] [skillist: fsharp-code-generation] Run `./fake.sh build -t RefreshSurfaceBaselines` to fill the index region and all 52 detail-page header regions from `catalogFacts`; confirm a clean re-run produces no diff
- [X] T014 [P] [US1] [skillist: fs-skia-evidence-mode, fs-skia-skiaviewer] Produce per-control render-only preview PNGs at `docs/img/controls/<id>.png` through the existing deterministic render-only evidence path (committed source assets, GPU-free docs CI consumes them); for any control that cannot be honestly rendered, emit **no** asset and an explicit unsupported note — never a 1×1/placeholder image
- [X] T015 [P] [US1] [skillist: fs-skia-testing, fs-skia-evidence-mode] Validate each produced preview with `Testing.readPngArtifact` (decodable, non-1×1 dimensions, non-trivial content); record per-control honesty (mode, dimensions, fallback classification, explicit unsupported reasons) in `readiness/controls-preview-evidence.md`
- [X] T016 [US1] [skillist: []] Author each `docs/controls/<id>.md` detail page's prose/usage **outside** the generated header region — purpose/explanation, the catalog usage example where one exists (honest omission otherwise), and the preview embed or honest unsupported note
- [X] T017 [US1] [skillist: []] Add the new top-level "Controls" section to `docs/index.md` entry-point nav and wire index→detail, detail→API-reference, and detail→back-to-index links so every page is reachable (FR-008, SC-001)
- [X] T018 [US1] [skillist: fsdocs-build] Build the site with `dotnet tool restore` then `dotnet fsdocs build --strict --eval`; confirm the catalog index, all 52 detail pages, and previews appear in `output/`; record `readiness/docs-build.md`
- [X] T019 [US1] [skillist: []] Run `./fake.sh build -t ControlsCatalogDocsCheck` → PASS; record `readiness/controls-catalog-docs.md` (index currency vs `catalogFacts`, detail-page completeness, preview present/current/decodable, link resolution) (SC-002, SC-003, SC-004)
- [X] T020 [US1] [skillist: []] Document the US1 independent validation path: open the site index → reach the catalog in one step → 52 controls grouped by category → drill into any detail page → resolving preview + API link

**Checkpoint**: US1 functional — the authoritative catalog index, detail pages, and previews ship and the currency gate is green.

---

## Phase 4: User Story 2 — Controls in the Spec Kit workflow (US2, P2)

### Implementation

- [X] T021 [P] [US2] [skillist: fsdocs-technical] Author `docs/controls/spec-kit-workflow.md` (lowest `index:` in the section): where controls are **chosen** (specify/plan), **authored** (implement, via the typed front door), and **validated** (the evidence/gates the workflow expects) across specify → plan → tasks → implement
- [X] T022 [US2] [skillist: fs-skia-typed-controls] Cross-link the narrative's "author during implement" step to `controls-design/typed-front-door.md` and the typed-controls authoring guidance; verify no dead links (FR-002 authoring path, SC-005)

### Verification

- [X] T023 [US2] [skillist: []] US2 verification: confirm a reader can, from the narrative alone, name the workflow phase(s) where controls are chosen/authored/validated and point to the relevant authoring guidance; record the reviewer checklist and pass/fail outcome under `readiness/` (SC-005)

**Checkpoint**: US2 functional — the usage narrative precedes the catalog and explains the workflow path.

---

## Phase 5: User Story 3 — Penpot drives control theming (US3, P3)

### Implementation

- [X] T024 [P] [US3] [skillist: fs-skia-design-tokens] Author the Penpot/design-tokens `##` subsection inside `docs/controls/spec-kit-workflow.md`: how control theming derives from design tokens, the token→typed-token-surface path, where the token single source lives, linking to `controls-design/design-tokens-penpot.md` as the deep dive (FR-007)

### Verification

- [X] T025 [US3] [skillist: []] US3 verification: confirm a reader can describe the design-token/Penpot → control-theming path and locate the design-token single source from the subsection; record the reviewer checklist and pass/fail outcome under `readiness/` (SC-006)

**Checkpoint**: US3 functional — the Penpot subsection completes the narrative.

---

## Phase 6: Integration & Polish

- [X] T026 [skillist: []] Cross-link existing pages **into** the new section without relocating or duplicating it: `docs/architecture/controls.md` → catalog index; `docs/controls-design/typed-front-door.md` and `design-tokens-penpot.md` referenced from the narrative (FR-006, contract S3)
- [X] T027 [skillist: fsdocs-build] Full-site link sweep on a fresh `dotnet fsdocs build --strict --eval`: confirm 100% of narrative, Penpot, index→detail, detail→API, and cross-link targets resolve in `output/` — no dead links (FR-009, SC-003)
- [X] T028 [skillist: fsharp-build-orchestration] Run the serialized governance suite sequentially — `Dev` → `ControlsCatalogDocsCheck` → `GeneratedGuidanceCheck` — and record the broad-risk-level aggregate results non-authoritatively in `readiness/` (note any environment-failure caveats)
- [X] T029 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` (speckit.evidence.graph) — confirm no cycles, no dangling refs, no `[S*]` surprises
- [X] T030 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` (speckit.evidence.audit) — confirm verdict PASS or document every `--accept-synthetic` override in the Synthetic-Evidence Inventory

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. For `[SEH]` rows,
include the approval label, design-phase source, synthetic input class, expected
error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
