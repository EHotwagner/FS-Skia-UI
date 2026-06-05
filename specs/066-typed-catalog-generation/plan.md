# Implementation Plan: Typed Catalog Generation

**Branch**: `066-typed-catalog-generation` | **Date**: 2026-06-05 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/066-typed-catalog-generation/spec.md`

## Summary

Make the typed registry of the six `065` controls (`TextBlock`, `Button`,
`CheckBox`, `Stack`, `TextBox`, `DataGrid`) the **single source** for those six
controls' catalog rows, and generate those rows deterministically into both
`src/Controls/catalog.yml` and `src/Controls/Catalog.fs`. Add a
generation-currency (drift) gate so a hand-edit that diverges from regeneration
fails the build and names the divergent control. The migration is **non-behavioral**:
each generated row is byte-identical to the row it replaces (parity, proven by test).

Technical approach — mirror the repository's existing single-source machinery
rather than inventing a new mechanism:

- A new pure generator module `build/Governance/CatalogGen.fs(/.fsi)` owns a typed
  **catalog-fact table** for exactly the six controls (the single source) and renders
  each fact to (a) an F# `Catalog.definition …` row and (b) a `catalog.yml` row block.
  This mirrors `ContractView` (Routing.fs → validation.contract.yml) and reuses the
  `GovernedBlocks` marker/splice/currency primitives.
- Because the six rows are **non-contiguous** in both files, generation uses
  **per-control inline marked regions** (`BEGIN/END GENERATED: typed-catalog/<id>`,
  YAML `#` markers in `catalog.yml`, `//` markers in `Catalog.fs`). The 41 hand-authored
  rows lie outside every marker and are never touched (FR-003).
- A new named gate `ControlsCatalogGenerationCheck` runs the currency check, is added to
  the existing `controls-public-surface` routing rule so `./fake.sh build -t Route`
  lists it, and `Route --enforce` blocks on a stale catalog (FR-005/FR-006, US3).
- Regeneration folds into the existing `RefreshSurfaceBaselines` target via a new
  `RegenerateCatalog` effect, writing both files' regions in one invocation (FR-002).

No runtime dependency is added to `FS.Skia.UI.Controls`; all generation logic lives in
`FS.Skia.UI.Build` (FR-009). The public `ControlDefinition` shape and
`Catalog.supportedControls` values are unchanged.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (build front `FS.Skia.UI.Build`; shipped `FS.Skia.UI.Controls`)
**Primary Dependencies**: None new. Generator is pure F# over in-memory text; reuses
`FS.Skia.UI.Build.GovernedBlocks` splice/currency primitives. File I/O stays at the
`Engine/Interpret.fs` edge (Principle IV).
**Testing**: Expecto (`tests/Controls.Tests/CatalogTests.fs` extended with a
generated-vs-source parity test + a typed-registry correspondence test); FAKE targets
(`ControlsCatalogGenerationCheck`, `TargetMetadataDrift`, `GeneratedGuidanceCheck`); FSI
where a transcript is the honest audience.
**Target Platform**: Windows and Linux (build-time transform; platform-independent).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification**: **Tier 2 (internal change)** for the shipped package — no
public API surface, behavior, or dependency change (`Catalog.supportedControls` /
`ControlDefinition` are unchanged in shape and values; the six rows are byte-identical).
Tier-1-style artifact rigor (routing, evidence, currency gate) still applies because the
change touches `src/Controls/**` (escalated by `controls-public-surface`) and adds a
governance gate. No `.fsi` change is expected; if a surface baseline moves, that is a
regression to investigate, not an expected delta.

### Repository Governance Decisions

- **Template ownership**: N/A — no change to `.template.config/template.json`, generated
  samples, docs, or command surface that the template ships. The catalog files are
  framework-internal package sources, not template-emitted artifacts; the typed-catalog
  generator and gate live only in `FS.Skia.UI.Build`, which the template does not include.
- **Dependency impact**: N/A — no new dependency. No edit to
  `Directory.Packages.props`, `docs/dependencies.md`, or `DependencyReport` coverage; the
  generator is pure F# reusing in-repo `GovernedBlocks` primitives, adding nothing to
  `FS.Skia.UI.Controls` (FR-009).
- **Command-surface impact**: `build.fsx`/`FS.Skia.UI.Build` **changes**: add a new
  `Target` case `ControlsCatalogGenerationCheck` (enum + `allTargets` + `name` +
  `directPrerequisites = []` + `timeoutClass "focused"` + `cost "low"` +
  `failureOwner "product"`), register it in `AgentValidation.ValidationContract.knownGates`,
  add it to the `controls-public-surface` rule's `RequiredGates` in `Routing.fs`, add a
  `RegenerateCatalog` effect interpreted under `RefreshSurfaceBaselines`, and wire the new
  gate arm in `Engine/Update.fs`. `validation.contract.yml` is regenerated from `Routing.fs`
  (its currency is enforced by `TargetMetadataDrift`). FAKE-backed targets are run
  **sequentially** in the deterministic order in `CLAUDE.md`/`AGENTS.md` (never
  concurrently; shared `.fake` state). Route-printed gates run first.
- **Generated project impact**: N/A — no change to default/minimal generated contents,
  selected Controls guidance, local skills, validation logs, placeholder/excluded-history
  scans, or generated `Dev` behavior. The catalog rows' observable content is byte-identical,
  so `GeneratedProductCheck` consumers see no change.
- **Evidence paths**: `specs/066-typed-catalog-generation/readiness/` —
  `typed-catalog-generation.md` (single-source design, the per-control marker model, the
  drift gate, and an explicit statement that generation is **real**, no `[S]`), and
  `typed-catalog-parity.md` (the six-row generated-vs-source parity matrix + any
  hand-authored row corrected to the registry under FR-008). The `controls-public-surface`
  rule's existing expected artifacts (`readiness/typed-controls-front-door.md`,
  `readiness/package-surface-expectations.md`) continue to apply.
- **`.fsi` / contract impact**: No change to `src/Controls/Catalog.fsi`,
  `ControlDefinition`, or surface baselines (the six rows keep identical values). A **new**
  build-front `.fsi` is added: `build/Governance/CatalogGen.fsi` (build-tooling scope, not a
  tracked runtime surface baseline, per the `SkillTreeGen.fsi` precedent). `catalog.yml` is a
  consumer contract whose six-row **content** is unchanged; only the authoring mechanism
  (now generated regions) changes.
- **MVU/effect boundary**: N/A — no stateful or I/O-bearing workflow. Generation is a pure,
  deterministic transform: `CatalogGen.render`/`currency` are pure over in-memory text;
  the file reads/writes happen only at the `Engine/Interpret.fs` interpreter edge
  (consistent with `RegenerateGovernedBlocks`/`ContractView`).
- **Synthetic evidence**: None. Generation is real (the gate compares regenerated bytes to
  on-disk bytes), parity is asserted against the real pre-migration rows, and the
  correspondence test reads the real `FS.Skia.UI.Controls.Typed` surface. No `[S]`/`[SEH]`
  task is expected; the evidence files state this explicitly (SC-006).
- **Test evidence**: Failing-first — (1) a parity test in `CatalogTests.fs` asserting each
  of the six generated rows equals the captured pre-migration row; (2) a drift test asserting
  `CatalogGen.currency` flags a hand-mutated region and names the control; (3) a
  correspondence test asserting the fact table covers exactly the six
  `FS.Skia.UI.Controls.Typed` modules and that each fact's `requiredAttributes` agree with the
  typed `Props`. Governance evidence: `ControlsCatalogGenerationCheck` PASS report,
  unchanged `ControlsCatalogCheck` + `CatalogTests`, `TargetMetadataDrift` green on a
  regenerated tree.
- **Observability**: The new gate writes a structured readiness report and, on drift,
  `FailWith` a diagnostic that **names the divergent control(s)** and the regeneration
  command (`./fake.sh build -t RefreshSurfaceBaselines`) — mirroring `GovernedBlocks`/
  `ContractView` currency diagnostics. Missing generated regions fail explicitly (no silent
  passthrough).
- **Deferred scope**: Out of scope and deferred to later roadmap features — typing/migrating
  the other 41 controls (070), keyed reconciliation (067), `Controls.Elmish` command model
  (068), design tokens/Penpot (069), and any `supportedCount`/schema/category change. No
  visual/screenshot evidence is required (the catalog describes controls, does not render
  them).

## Project Structure

Real paths for this feature (✎ = edited, ✚ = new, ◆ = generated regions inside an existing file):

```
build/Governance/
  CatalogGen.fsi                         ✚ public surface of the catalog generator (build-tooling scope)
  CatalogGen.fs                          ✚ typed six-control fact table (single source) + render/currency
  Targets.fs / Targets.fsi               ✎ add ControlsCatalogGenerationCheck (enum, allTargets, name, prereqs, meta)
  Routing.fs                             ✎ add the gate to the controls-public-surface RequiredGates
  AgentValidation.fs                     ✎ add "ControlsCatalogGenerationCheck" to knownGates
  GovernedBlocks.fs                      · reused (splice/currency primitives; no edit expected)
  Engine/Interpret.fs / .fsi             ✎ add RegenerateCatalog effect + handler
  Engine/Update.fs                       ✎ new gate arm; fold RegenerateCatalog into RefreshSurfaceBaselines
  FS.Skia.UI.Build.fsproj                ✎ add CatalogGen.fsi/.fs to the compile order (after GovernedBlocks)

src/Controls/
  catalog.yml                            ◆ six rows wrapped in `# BEGIN/END GENERATED: typed-catalog/<id>` markers
  Catalog.fs                             ◆ six `definition …` rows wrapped in `// BEGIN/END GENERATED: typed-catalog/<id>` markers
  Catalog.fsi                            · unchanged (public ControlDefinition shape stable)
  Widgets/Primitives.fsi, TextBoxWidget.fsi, DataGridWidget.fsi   · read-only correspondence source

validation.contract.yml                 ◆ regenerated from Routing.fs (now lists the new gate; currency via TargetMetadataDrift)

tests/Controls.Tests/
  CatalogTests.fs                        ✎ + parity, drift, and typed-registry correspondence tests

specs/066-typed-catalog-generation/
  spec.md plan.md research.md data-model.md quickstart.md
  contracts/                             ✚ catalog-generation.md, public-catalog-contract.md
  readiness/                             ✚ typed-catalog-generation.md, typed-catalog-parity.md
```

### Phase 0 — Research

See [research.md](./research.md). All NEEDS CLARIFICATION resolved; six design
decisions recorded (single-source location, per-control marker splice, new named gate
vs. `TargetMetadataDrift` fold, regeneration target, registry correspondence, gate
prerequisites).

### Phase 1 — Design & Contracts

- [data-model.md](./data-model.md) — `TypedCatalogFact` (the single-source row), its
  mapping to `ControlDefinition` / YAML, and the currency result.
- [contracts/catalog-generation.md](./contracts/catalog-generation.md) — the build-front
  generation/gate/regeneration contract.
- [contracts/public-catalog-contract.md](./contracts/public-catalog-contract.md) — the
  **unchanged** public catalog surface, asserted to be stable.
- [quickstart.md](./quickstart.md) — edit-a-fact → regenerate → gate walkthrough.

### Post-design constitution re-check

Re-evaluated after Phase 1: no new violation. Tier-2 internal change; no `.fsi`/baseline
delta for the shipped package; all generation pure with I/O at the edge; no synthetic
evidence; every governed-decision area filled (GeneratedGuidanceCheck-ready).
