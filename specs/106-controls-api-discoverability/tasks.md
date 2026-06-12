# Tasks: Controls Authoring API Discoverability

**Feature branch**: `106-controls-api-discoverability`
**Spec**: `specs/106-controls-api-discoverability/spec.md`
**Plan**: `specs/106-controls-api-discoverability/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/evidence-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. **None is anticipated** — the new
gate runs against the **real** `src/Controls/**/*.fsi` surface, the starter
migration is proven by the **real** existing `TypedLoweringTests` parity suite,
and `GeneratedProductCheck` renders the **real** generated product. The gate's
red-before fixture is ordinary red/green test data (a planted placeholder in a
temp `.fsi`), not `[S]` synthetic evidence (plan "Synthetic evidence").

## Tier & scope banner

**Tier 1 (contracted change).** `///` doc comments are added/replaced across the
13 public Controls `.fsi` files (186 placeholders), the sample/template contract
(`template/base/src/Product/View.fs`, `README.md`) changes, a consumer-visible
catalog reference is bundled, and a new `ControlsDocCoverageCheck` governance
gate is added and routed. **No `.fsi` signature *shape* change** (no
added/removed/retyped members — doc-only). MVU/effect boundary **N/A** (the gate
is a pure `.fsi` text → findings analysis; no product `Model`/`Msg`/`Effect`).
Not a graphical-viewer feature — no persistent-launch task; render proof is
`GeneratedProductCheck` (the starter renders the same controls it does today).
`Route` is authoritative and will **escalate** to the controls-public-surface
set (triggered by `src/Controls/**/*.fsi`, `template/**`, `build/**`).

## Success-criterion → assertion mapping

- **SC-002** (100% of the Controls public surface non-placeholder; 0 boilerplate)
  → the new `ControlsDocCoverageCheck` gate authored in **T005**/**T007** and run
  green over the real surface in **T014** (`analyze() = []` over `src/Controls/**`).
- **SC-003** (starter authors all controls via the typed front door; 0 legacy
  attr-list constructions) → the grep verification in **T009** over the rewritten
  `View.fs` from **T010**.
- **SC-001** (add an unshown control kind from `defaults with` + IntelliSense; it
  compiles and renders) → the walkthrough recorded in **T009** + the render proof
  in **T012** (`GeneratedProductCheck`).
- **SC-006** (typed front door present in `docs/api-surface/Controls/` + covered
  by baselines) → the bundle currency confirmed in **T011** and stability in **T015**.
- **SC-004** (from the README reach the discovery API + catalog reference and
  determine a control's supported-attribute set without reflection) → the
  `TemplateCheck` walkthrough in **T018**.
- **SC-005** (every "do not reflect" line resolves to a populated alternative) →
  the README pointer authored in **T017**, exercised in **T018**.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- Tier annotations omitted: every phase matches the spec's overall **Tier 1**.

FAKE-backed commands (`./fake.sh`, `fake.cmd`, `dotnet fake`) share repository
`.fake` state and are **not** safe to run concurrently. The FAKE-backed gate
tasks (**T012 → T014 → T015 → T018 → T021 → T022 → T023**) carry explicit graph
dependencies (direct or via phase-checkpoint edges) that serialize them in
deterministic order. Non-FAKE checks (the doc rewrite, the catalog bundle, the
README pointer, the `.fsi` diff) are parallel-safe.

## Governance risk levels

- **Small**: a single `.fsi` file's doc-comment rewrite (e.g. `Collections.fsi`,
  3 summaries).
- **Medium**: the 186-summary rewrite across 13 files + the starter migration +
  README/catalog bundling — **this feature's level**. Focused validation = the
  gate set `Route` prints (the escalated `controls-public-surface` set incl.
  `ControlsDocCoverageCheck` + `TargetMetadataDrift`, plus
  `GeneratedProductCheck`, `TemplateCheck`).
- **Broad**: required only when adding the new gate to the routed set or a FAKE
  failure looks race-like; rerun the affected FAKE-backed commands
  **sequentially** before any product-regression claim. Aggregate-suite results
  obtained outside the routed focused set are recorded as **non-authoritative**.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm `specs/106-controls-api-discoverability/` is the active feature (`.specify/feature.json`), link spec + plan, and validate the `106-controls-api-discoverability` branch
- [X] T002 [P] [skillist: []] Re-verify the placeholder inventory against the current tree: 186 `"Public contract function exposed by this FS.Skia.UI package."` summaries across the 13 Controls `.fsi` files (`Control.fsi` 88, `Attributes.fsi` 25, `Diagnostics.fsi` 16, `Catalog.fsi` 11, `Charts.fsi` 10, `DataGrid.fsi` 8, `Theme.fsi`/`RichText.fsi`/`Accessibility.fsi` 5 each, `ControlRuntime.fsi`/`TextInput.fsi` 4 each, `Collections.fsi` 3, `CustomControl.fsi` 2), and confirm the `Widgets/*.fsi` typed surface carries 0 placeholders (positive exemplar, research D1)
- [X] T003 [P] [skillist: []] Scaffold `specs/106-controls-api-discoverability/readiness/` with audit-enforced placeholder files discoverable before implementation: `doc-coverage.md`, `generated-product.md`, `template-check.md`, `surface-baselines.md`, `evidence-graph.md`, `evidence-audit.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md` — each naming its authoritative command, artifact path, failure class, and next action
- [X] T004 [P] [skillist: []] Record feature Tier (1, contracted), affected layer (Controls public `.fsi` doc-only, template starter/README/catalog bundle, governance home), public-API impact (no `.fsi` signature shape change; `///` docs added; new `ControlsDocCoverageCheck` gate routed), Elmish/MVU applicability (**N/A** — pure analysis gate, no stateful/I/O product workflow), and the evidence obligations (doc-coverage 0 findings, surface-baseline currency, `GeneratedProductCheck`, `TemplateCheck`, `EvidenceGraph` + `EvidenceAudit` verdict)

---

## Phase 2: Foundation (the durable guard)

- [X] T005 [P] [skillist: fsharp-parsing, fsharp-io-globbing] Implement `build/Governance/ControlsDocCoverage.fs` as a pure analysis: `DocFinding` record (`File`/`Line`/`Identifier`/`Reason` (`Placeholder`|`Empty`|`DuplicateOnly`)/`Detail`) + `analyze : unit -> DocFinding list` that enumerates `src/Controls/**/*.fsi` (reusing `FS.Skia.UI.SkillSupport.Globbing`), attaches each leading `///` block to the next `val`/`type`/`member` declaration (`Parsing`-style line grammar), and flags the placeholder / empty / duplicate-only predicate (research D2/D3); no file I/O inside `analyze` (MVU/effect boundary N/A)
- [X] T006 [skillist: fsharp-build-orchestration] Wire `ControlsDocCoverageCheck` into the single governance home following the `DesignTokenDrift` precedent (research D4): `Targets.fs` (`Target` DU + `allTargets` + `name` map + `directPrerequisites` = `[]` + `routableGates`), `Routing.fs` (add to the controls-public-surface rule's required gates), and `Engine/Update.fs` + `Engine/Interpret.fs` (the effect that runs `analyze` and renders `readiness/doc-coverage.md`); `validation.contract.yml` + `AgentValidation.knownGates` are derived/regenerated by `RefreshSurfaceBaselines`, never hand-edited (`TargetMetadataDrift` enforces currency)
- [X] T007 [skillist: fsharp-build-orchestration] Author the red-before/green-after Expecto governance test for `ControlsDocCoverage.analyze` over `.fsi` fixtures (authored alongside T005 so the RED fixture proves the analyzer's failure mode before the live surface is rewritten — the inverted T005→T007 numbering is a build-tooling analyzer, not a product-surface change): a planted-placeholder fixture returns a `Placeholder` finding (RED proves the gate detects the real failure mode), a **short but meaningful** substantive-summary fixture returns 0 findings (GREEN, anti-false-positive — proves the gate does NOT fire on legitimately brief summaries per the spec edge case), and a generic-sentence-shared-across-many-members fixture returns a `DuplicateOnly` finding (anti-evasion, research D2)

**Checkpoint**: the gate exists and fails on a planted placeholder (and on the 186 live placeholders) — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — Compiler-guided authoring is the demonstrated default (P1)

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: fs-skia-typed-controls] Confirm the starter's typed controls (`TextBlock` display, `TextBox` input with `OnChanged`, `Button` with `OnClick`) are each covered by `tests/Controls.Tests/TypedLoweringTests.fs` (typed `view` lowers structurally equal to the legacy builder); add a parity case only if the starter introduces a typed control not yet in the suite (research D8). **Note the `TextBox` signature divergence**: `TextBox.defaults: ControlId -> TextBoxProps<'msg>` and `TextBox.view: props -> model: TextInputModel -> Widget<'msg>` (unlike `TextBlock`/`Button` whose `defaults` is a bare value and `view` takes only `props`); ensure the parity case exercises the `TextBox.view props model` form the starter will use, not the bare `{ defaults with … } |> view` form
- [X] T009 [P] [US1] [skillist: fs-skia-typed-controls] Author the US1 verification: a grep transcript asserting 0 legacy `Module.create [ ... ]` attr-list constructions in `template/base/src/Product/View.fs` (SC-003), plus the "add a control kind not shown in the starter using only `defaults with` + IntelliSense → compiles and renders" walkthrough mapped to SC-001

### Implementation

- [X] T010 [US1] [skillist: fs-skia-typed-controls] Rewrite `template/base/src/Product/View.fs` to author every control through `FS.Skia.UI.Controls.Typed`, demonstrating the FR-002 variety (display `TextBlock`, interactive `TextBox` with `OnChanged`, `Button` with `OnClick = Some msg`) and showing the `OnClick = None` → "binds nothing" idiom in a comment (FR-001/FR-002, edge case). **Use each module's real signature, not one uniform idiom**: `TextBlock`/`Button` use `{ Module.defaults with Field = ... } |> Module.view`; the interactive `TextBox` uses `TextBox.view { TextBox.defaults "<id>" with Value = ...; OnChanged = ... } <textModel>`, where `<textModel>` is the retained per-identity `TextInputModel` the live host already tracks (the starter must show where that model comes from — do not invent a literal). The typed `view` returns `Widget<'msg>`, so confirm the rewritten `controlsExampleView` still type-checks as the view `ControlsElmish.program` consumes (compose/lower the `Widget` tree the way the legacy tree is consumed today); `GeneratedProductCheck` (T012) proves the wiring compiles + renders. Any starter control not yet in the typed front door stays on the legacy builder with a one-line pointer to the typed path
- [X] T011 [US1] [skillist: fs-skia-template-update] Regenerate the api-surface bundle via `./fake.sh build -t RefreshSurfaceBaselines` and confirm `template/base/docs/api-surface/Controls/` contains the typed `Widgets/*.fsi` signatures the starter relies on and passes `ApiSurfaceGen.currency` (verify-and-keep-current, research D6; FR-004/SC-006)
- [X] T012 [US1] [skillist: fs-skia-template-update] Run `./fake.sh build -t GeneratedProductCheck` — confirm the regenerated starter compiles and renders the same controls with no behavior regression (FR-003); write `readiness/generated-product.md`

**Checkpoint**: the generated starter demonstrates the typed front door and renders unchanged.

---

## Phase 4: User Story 2 (US2) — IntelliSense explains the authoring surface (P2)

### Implementation

- [X] T013 [US2] [skillist: fsdocs-api-doc] Replace all 186 placeholder summaries across the 13 Controls `.fsi` files with substantive per-member `///` docs per `contracts/doc-comment-standard.md`: attribute builders state what the attribute does + value meaning/units + accepting control kind(s) + omitted-optional lowering; per-control entries state what the control is + required attrs + key events + the typed `Props` cross-reference; `Catalog.fsi` functions state what they return + how to enumerate a control's contract; public types state what they represent (FR-005/FR-006/FR-008/FR-009); no `.fsi` signature shape change. **Each rewritten summary MUST carry a member-specific token** (a backticked identifier and/or a value/units description) so substantively-distinct members never collapse into the `DuplicateOnly ≥8-identical` predicate (data-model D1) — avoid boilerplate-shaped phrasing shared verbatim across many attribute builders

### Verification

- [X] T014 [US2] [skillist: fsharp-build-orchestration] Run `./fake.sh build -t ControlsDocCoverageCheck` (now green over the real surface) — confirm `analyze()` returns `[]` over `src/Controls/**`, and write `readiness/doc-coverage.md` recording the enumerated surface (`findings=0` over N members across M files), so the documented surface cannot regress to boilerplate (SC-002, FR-007)
- [X] T015 [US2] [skillist: fs-skia-template-update] Confirm the per-package surface baseline (`readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt`) stays byte-stable after the doc-only `.fsi` edits (`PerPackageSurface.normalize` strips `//`-prefixed lines) while the api-surface *bundle* is regenerated current; write `readiness/surface-baselines.md` (research D5)

**Checkpoint**: every public Controls member carries substantive IntelliSense documentation; the gate guards it.

---

## Phase 5: User Story 3 (US3) — Per-control facts are discoverable without reflection (P3)

### Implementation

- [X] T016 [P] [US3] [skillist: fs-skia-template-update] Bundle a consumer-visible per-control catalog reference into the generated project under `template/base/docs/` (derived from the `catalog.yml` the package already ships in `contentFiles/`, or the `CatalogDocsGen` per-control markdown the generated repo produces), and update `.template.config/template.json` if the manifest must list the new content file (FR-011, research D7)
- [X] T017 [US3] [skillist: fs-skia-controls-host] Populate `template/base/README.md` so every "do not use reflection / read the source-shaped API reference" line resolves to a concrete, populated target: the typed starter (`View.fs`), the `docs/api-surface/Controls/*.fsi` bundle, the documented `Catalog.*` discovery API (`requiredAttributes`/`supportedAttributes`/`supportedEvents`/`knownControlKinds`/`markdownSummary`), the bundled catalog reference, and the interactive host authoring seam (`runInteractiveApp`, the `fs-skia-controls-host` surface) so authoring a controls app is discoverable end to end (FR-010/FR-012/FR-013, SC-005)
- [X] T018 [US3] [skillist: fs-skia-template-update] Run `./fake.sh build -t TemplateCheck` — confirm the bundled catalog reference and the README discovery pointer are present in the generated project, and walk the SC-004 path (from the README reach the `Catalog.*` API and the catalog reference, determine a named control's complete supported-attribute set without reflection); **also walk the not-yet-typed control case** — pick a control that lacks a typed module and confirm the catalog reference + discovery API still report its full attribute contract and do **not** mark it unsupported merely because it has no typed `Props` (spec edge case "must not imply a control is unsupported"); write `readiness/template-check.md`

**Checkpoint**: per-control facts and the host seam are reachable from the README without reflection.

---

## Phase 6: Integration & Polish

- [X] T019 [P] [skillist: []] Prove **no `.fsi` signature shape change**: `git diff origin/main...HEAD -- 'src/Controls/**/*.fsi'` shows only `///` comment-line changes (no added/removed/retyped `val`/`type`/`member`), and confirm no added/retained doc comment introduces a literal evidence filename or bare gate token that a governance scan (window-visibility / diff-scan) could misparse as a status/behavior token
- [X] T020 [P] [skillist: []] Finalize `readiness/governance-risk-levels.md`, `readiness/aggregate-hang-diagnostics.md`, `readiness/runtime-limitations.md`, and `readiness/generated-guidance-validation.md`: record the selected medium risk level, the focused validation for it, when broad validation is required, and how non-authoritative aggregate results are recorded
- [X] T021 [P] [skillist: fsharp-build-orchestration] Run `./fake.sh build -t Route` then exactly the gates it prints, FAKE-backed targets **sequentially** in the documented order (`Dev` → the escalated `controls-public-surface` set incl. `ControlsDocCoverageCheck` + `TargetMetadataDrift` → `GeneratedGuidanceCheck` / `TemplateCheck` / `GeneratedProductCheck` if printed); capture the focused-gates log and confirm the Controls + Controls.Elmish suites are green
- [X] T022 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory=specs/106-controls-api-discoverability` and `tasks=<n>` match, no cycles, no dangling refs, no `[S*]` surprises; write `readiness/evidence-graph.md`
- [X] T023 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with **0 synthetic** tasks and no diff-scan blockers; write `readiness/evidence-audit.md` with the verdict token

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — the gate runs against the real `src/Controls/**/*.fsi` surface; the starter migration is proven by the real `TypedLoweringTests` parity suite; `GeneratedProductCheck` renders the real product)_ | | | | | | | | |
