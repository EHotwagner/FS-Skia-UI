# Implementation Plan: Close Out the Deferred Typed-Controls-Migration Follow-Ups

**Branch**: `071-typed-controls-followups` | **Date**: 2026-06-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/071-typed-controls-followups/spec.md`

## Summary

Close the four deferred `070` tasks (T007, T037–T039), which split into two
independent themes that share no code:

- **Catalog single-source completion (US1, P1).** Extend
  `CatalogGen.catalogFacts` in `build/Governance/CatalogGen.fs` from the original
  **6** reference facts to all **47** catalog ids, generalize the `renderFSharpRow`
  chart/data-grid evidence special-case from the single `data-grid` id to the set
  of six evidence-carrying ids (`data-grid`, `line-chart`, `bar-chart`,
  `pie-chart`, `scatter-plot`, `graph-view`), and regenerate
  `src/Controls/catalog.yml` + `src/Controls/Catalog.fs` via
  `./fake.sh build -t RefreshSurfaceBaselines` so the generator emits all 47
  `BEGIN/END GENERATED: typed-catalog/<id>` regions itself. After regeneration
  **zero** rows are hand-maintained, `ControlsCatalogGenerationCheck` enforces
  currency over the full 47, and the `066` fixture-iteration cross-check
  (`tests/Controls.Tests/CatalogTests.fs`) stays green — which requires a
  per-fact parity fixture (`Catalog.fs.<id>.txt` + `catalog.yml.<id>.txt`) for
  each of the 41 newly-generated ids under
  `specs/066-typed-catalog-generation/readiness/parity-fixtures/` (where that
  test reads), and the hand-maintained `typedPropsById` map extended to lockstep.

- **Typed gallery panel coverage and render evidence (US2, P2).** Extend the
  existing `samples/ControlsGallery/Program.fs` `typedAuthoringPanel` (today:
  TextBlock, Button, CheckBox) to ≥1 control per mechanic group authored only
  through `FS.Skia.UI.Controls.Typed.*` `view` functions, extend
  `RenderingTests.fs` / `AccessibilityTests.fs` to cover that panel at ≥2
  viewports, and capture deterministic render-only viewport evidence to
  `specs/071-typed-controls-followups/readiness/controls-rendering.md`.

Both themes are pure completion of in-flight `070` scope. No shipped public
`FS.Skia.UI.Controls` `.fsi` signature changes — the 41 typed modules and their
parity tests already shipped in `070`; this feature touches only the build-side
fact table, generated governance artifacts, test code, sample code, parity
fixtures, and readiness evidence. **Tier 2 (internal change)** at the package
surface (additive-only/empty per-package surface delta), with a generated
governance-artifact regeneration that is currency-gated.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: None added. Existing only — SkiaSharp (render path),
Expecto + FsCheck (tests), FAKE front-end `FS.Skia.UI.Build` (catalog generation,
routing, currency gate). No dependency-manifest change (`Directory.Packages.props`
untouched).
**Testing**: Expecto over `tests/Controls.Tests/` (`CatalogTests.fs`,
`TypedMigrationTests.fs`, `RenderingTests.fs`, `AccessibilityTests.fs`); FAKE
targets `RefreshSurfaceBaselines` (regenerate) and `ControlsCatalogGenerationCheck`
(currency); deterministic render-only evidence capture for the typed gallery
panel.
**Target Platform**: Windows and Linux. The render/accessibility coverage runs
headless through the existing `Control.render`/`Widget.toControl` IR path (no GPU
window required); evidence is render-only and re-capture byte-identical.

**Routing note**: `./fake.sh build -t Route` on the *spec-only* working tree
prints `tier=focused-authority`, `gates=Dev, GeneratedGuidanceCheck,
TemplateDrift, EvidenceGraph` (matched `specify-catchall, docs-only`). Once the
implementation diff lands (`build/Governance/CatalogGen.fs`,
`src/Controls/catalog.yml`, `src/Controls/Catalog.fs`, `tests/Controls.Tests/**`,
`samples/ControlsGallery/Program.fs`, parity fixtures, readiness), **re-run
`Route`** and run exactly the gates it then prints — generated governance + catalog
changes are expected to add `ControlsCatalogGenerationCheck` and per-package
surface verification (`PackageSurfaceCheck` / `PerPackageSurfaceDiff`), and the
`after_implement` hook runs `EvidenceAudit`. All Technical Context items are
**fully resolved** — no open clarifications remain.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: N/A — no `.template.config/template.json` change. This
  feature edits the build-side fact table (`build/Governance/CatalogGen.fs`),
  regenerates governance artifacts (`src/Controls/catalog.yml`,
  `src/Controls/Catalog.fs`), extends tests (`tests/Controls.Tests/**`) and the
  repo sample (`samples/ControlsGallery/Program.fs`), and writes parity fixtures +
  readiness evidence. None is a generated-project template fragment,
  package-policy, or command-surface change the template must mirror. Version
  bump/pack and template-pin refresh are explicitly **post-merge**, owned by
  `speckit-merge` / `fs-skia-template-update` (out of scope, per spec).
- **Dependency impact**: N/A — no dependency added (spec Package impact / FR-010).
  `Directory.Packages.props`, `docs/dependencies.md`, generated template
  inclusion, and `DependencyReport` are unchanged.
- **Command-surface impact**: No `build.fsx`/`Routing.fs`/wrapper change. The
  catalog regeneration reuses the existing `RefreshSurfaceBaselines` /
  `ControlsCatalogGenerationCheck` path; the gallery/test changes route through
  the standard controls gates. `validation.contract.yml` stays generated from
  `Routing.fs`. **Run `./fake.sh build -t Route` first on the implementation diff
  and run only the gates it prints.** FAKE-backed targets share `.fake` state and
  are **not** safe to run concurrently — run them sequentially. If `Route`
  escalates this consumer-adjacent change, use the deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  Safe non-FAKE reads/checks may still run in parallel.
- **Generated project impact**: N/A — no change to default/minimal generated
  contents, selected-Controls guidance, local skills, validation logs,
  placeholder/excluded-history scans, or generated `Dev` behavior. `catalog.yml` /
  `Catalog.fs` are framework governance artifacts (catalog cross-check inputs),
  not generated-project output; the typed modules they mirror already shipped in
  `070`.
- **Evidence paths**: All under `specs/071-typed-controls-followups/readiness/`:
  - `readiness/catalog-single-source.md` — the 6→47 fact-table extension, the
    regeneration rationale, and the statement that all 47 rows are generated (zero
    hand-maintained); names the six evidence-carrying ids the special-case covers.
  - `readiness/controls-rendering.md` — deterministic typed gallery viewport
    render evidence (render-only, **no** `[S]`).
  - Per-fact parity fixtures the `066` cross-check reads land in the pre-existing
    `specs/066-typed-catalog-generation/readiness/parity-fixtures/` (41 new ids ×
    `Catalog.fs.<id>.txt` + `catalog.yml.<id>.txt`).
  - Gate evidence (`evidence-graph.md`, `evidence-audit.md`, per-package surface
    diff, skill-loading) lands under `readiness/` per the printed gates.
- **`.fsi` / contract impact**: **No shipped public `.fsi` signature changes
  (FR-010).** The 41 typed modules already shipped in `070`. The catalog single
  source (`catalog.yml`/`Catalog.fs`) is regenerated; the `FS.Skia.UI.Controls`
  per-package surface baseline delta MUST be **additive-only or empty** —
  verified by `PackageSurfaceCheck` / `PerPackageSurfaceDiff` (SC-007). This is a
  **Tier 2** internal change at the package surface. `custom-control` keeps its
  bridge-typed treatment (`Widget.ofControl`, `070` FR-006) — no fabricated
  required attribute (FR-006).
- **MVU/effect boundary**: No **new** MVU model and no I/O added to any `update`.
  Catalog generation is pure text render/splice/currency over in-memory strings
  (file read/write stays at the `Engine/Interpret.fs` edge, as today). The typed
  gallery panel reuses already-shipped typed façades and their existing pure
  `update` models (stateful façades delegate to `TextInput` / `Collections` etc.,
  unchanged from `070`).
- **Synthetic evidence**: **None planned — no `[S]`/`[S*]`/`[SEH]`.** Catalog
  generation is exercised against the real fact table and real on-disk artifacts;
  the parity fixtures are captured **from** the generator's real output (golden
  bytes, not fabricated literals); the render evidence is real render-only output
  through the existing IR path. SC-006 / SC-008 require the `EvidenceAudit`
  verdict to be PASS with no `[S]`/`[S*]` disclosures.
- **Test evidence**: Failing-first then green. US1: the `066` fixture-iteration
  and currency tests in `CatalogTests.fs` fail on a 47-fact table without the new
  fixtures / `typedPropsById` lockstep, then pass once fixtures and the map are
  extended; a deliberate hand-edit to a generated region makes
  `ControlsCatalogGenerationCheck` fail (SC-002). US2: `RenderingTests.fs` /
  `AccessibilityTests.fs` gain typed-panel coverage at ≥2 viewports that fails
  before the panel/coverage exists and passes after. Governance: `Route` →
  printed gates → `EvidenceAudit` PASS.
- **Observability**: The currency gate already emits an actionable diagnostic
  naming the stale `typed-catalog/<id>` region and the regeneration command
  (`./fake.sh build -t RefreshSurfaceBaselines`); extending to 47 facts preserves
  that per-region diagnostic for the full set (FR-003 / SC-002). The fixture
  cross-check names the missing fixture id on a gap. Render evidence records the
  captured viewports and the determinism (byte-identical re-capture) claim.
- **Deferred scope**: Catalog **expansion** (controls beyond the 47 rows),
  overlays/virtualization, motion/animation, live Penpot/MCP, design-token value
  changes, legacy-API deprecation, `067` keyed-reconciliation and `068`
  `Controls.Elmish` changes all remain deferred to a later `071+` feature
  (spec Out of Scope / FR-011). The typed gallery panel is deliberately
  representative (≥1 per mechanic group), not exhaustive — per-control parity is
  already proven by `070`'s suites.

## Project Structure

```
build/Governance/CatalogGen.fs           # MODIFIED — catalogFacts 6→47; renderFSharpRow
                                         #   special-case generalized to 6 evidence ids
src/Controls/catalog.yml                 # REGENERATED — 47 typed-catalog/<id> regions
src/Controls/Catalog.fs                  # REGENERATED — 47 typed-catalog/<id> regions
tests/Controls.Tests/CatalogTests.fs     # MODIFIED — typedPropsById extended to 47 lockstep
tests/Controls.Tests/TypedMigrationTests.fs  # (reference) T036 cross-check stays green
tests/Controls.Tests/RenderingTests.fs   # MODIFIED — typed gallery panel @ ≥2 viewports
tests/Controls.Tests/AccessibilityTests.fs   # MODIFIED — typed gallery panel @ ≥2 viewports
samples/ControlsGallery/Program.fs       # MODIFIED — typedAuthoringPanel → ≥1 per group

specs/066-typed-catalog-generation/readiness/parity-fixtures/
    Catalog.fs.<id>.txt                  # NEW — one per newly-generated id (41)
    catalog.yml.<id>.txt                 # NEW — one per newly-generated id (41)

specs/071-typed-controls-followups/
    spec.md  plan.md  research.md  data-model.md  quickstart.md
    contracts/
        catalog-single-source.contract.md
        typed-gallery-panel.contract.md
    readiness/
        catalog-single-source.md         # NEW — fact-table extension + regen rationale
        controls-rendering.md            # NEW — typed gallery viewport evidence (render-only)
```
