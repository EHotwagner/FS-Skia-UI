# Contract: Catalog Single Source over all 47 controls (US1)

**Surface kind**: build-side generator + generated governance artifacts +
currency gate (no shipped public `.fsi`). This is a Tier 2 internal/governance
contract — the consumer is the catalog cross-check and the
`ControlsCatalogGenerationCheck` gate, not an end user.

## Provider

`build/Governance/CatalogGen.fs` — `catalogFacts : TypedCatalogFact list`,
`renderFSharpRow`, `renderYamlRow`, `render`, `currency`, `isCurrent`,
`currencyDrift`. Shape unchanged; `catalogFacts` population grows 6 → 47.

## Generation contract

| # | Given | When | Then |
| --- | --- | --- | --- |
| C1 | `catalogFacts` extended to all 47 ids | regenerate via `./fake.sh build -t RefreshSurfaceBaselines` | `catalog.yml` and `Catalog.fs` each contain 47 `BEGIN/END GENERATED: typed-catalog/<id>` regions; **zero** rows hand-maintained outside markers (FR-002/SC-001) |
| C2 | the regenerated artifacts | `ControlsCatalogGenerationCheck` runs | passes; currency enforced over all 47 facts (FR-003/SC-002) |
| C3 | any one generated region hand-altered | currency gate runs | fails; diagnostic names the stale `typed-catalog/<id>` region **and** the command `./fake.sh build -t RefreshSurfaceBaselines` (FR-003/SC-002) |
| C4 | a fact whose `Id` ∈ {`data-grid`,`line-chart`,`bar-chart`,`pie-chart`,`scatter-plot`,`graph-view`} | rendered | F# row ends `\|> withChartDataGridEvidence`; YAML row carries the chart/data-grid evidence path (FR-004) |
| C5 | a fact whose `Id` ∉ that set | rendered | no chart/data-grid evidence pointer (generic row) |
| C6 | the `custom-control` fact (`RequiredAttributes = []`) | rendered | a valid row with **no** fabricated required attribute; matches its T036 bridge-typed classification (FR-006) |
| C7 | the 41 new regions on first regeneration | generated | markers are emitted by the generator, not hand-typed (edge "marker insertion is generated") |

## Cross-check contract (`066` fixture-iteration, `tests/Controls.Tests/CatalogTests.fs`)

| # | Given | When | Then |
| --- | --- | --- | --- |
| C8 | one `Catalog.fs.<id>.txt` + `catalog.yml.<id>.txt` fixture per fact | the test iterates `catalogFacts` | a fixture exists for every one of the 47 ids; `renderFSharpRow`/`renderYamlRow` output equals the fixture (FR-005/SC-003) |
| C9 | `typedPropsById` extended to lockstep | assertion runs | `catalogFacts` ids == typed ids; each non-`custom-control` `requiredAttribute` PascalCased ∈ that control's `Props` fields (FR-005/SC-003) |
| C10 | `custom-control` | the Props-field assertion | excluded (bridge-typed, no `Props` schema); still present in the catalog rows (FR-006) |

## Surface invariant

| # | Given | When | Then |
| --- | --- | --- | --- |
| C11 | the implementation diff | `PackageSurfaceCheck` / `PerPackageSurfaceDiff` | `FS.Skia.UI.Controls` per-package surface delta is **additive-only or empty** — no shipped public signature changed (FR-010/SC-007) |
