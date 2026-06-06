# Phase 1 Data Model: Typed-Controls-Migration Follow-Ups (071)

This feature introduces **no new runtime types**. It extends the population of an
existing build-side record and the lockstep maps/fixtures that cross-check it.
The entities below are the data the feature reads, extends, and generates.

## E1 — `TypedCatalogFact` (existing, `build/Governance/CatalogGen.fs`)

The single-source record for one catalog row. Shape is unchanged; only the
*population* grows from 6 to 47 instances.

| Field | Type | Notes |
| --- | --- | --- |
| `Id` | `string` | Stable catalog id (e.g. `line-chart`). Key for markers, fixtures, currency. |
| `DisplayName` | `string` | Human label. |
| `Category` | `string` | Mechanic group: `display`, `input`, `selection`, `layout`, `data`, `navigation`, `overlay`, `feedback`, `chart`, `graph`, `custom`. |
| `Module` | `string` | Typed module name under `FS.Skia.UI.Controls.Typed`. |
| `Purpose` | `string` | One-line description rendered into the row. |
| `RequiredAttributes` | `string list` | Required attribute names; **`[]` for `custom-control`** (bridge-typed, R3/FR-006). |
| `Events` | `string list` | Event hook names (e.g. `onClick`). |
| `AccessibilityRole` | `string` | Role string carried into the catalog row. |

**Validation / invariants**
- The 47 `Id`s MUST equal the set of typed control ids (FR-001); enforced by the
  `066` cross-check assertion "`catalogFacts` ids == typed ids".
- Each non-`custom-control` `RequiredAttributes` entry, PascalCased, MUST be a
  field of that control's `Props` record (cross-check assertion).
- `custom-control.RequiredAttributes = []` (no fabricated attribute, FR-006/R3).

**State transition**: 6 facts → 47 facts (additive; no field added/removed).

## E2 — Evidence-carrying id set (the `renderFSharpRow` special-case)

Not a type — a generation rule. Generalized from `fact.Id = "data-grid"` to
membership in:

```
{ "data-grid"; "line-chart"; "bar-chart"; "pie-chart"; "scatter-plot"; "graph-view" }
```

Rows whose `Id` is in this set append `|> withChartDataGridEvidence` (F#) and the
chart/data-grid evidence path (YAML). Invariant (FR-004): exactly these six rows
carry the extra evidence pointer after regeneration — verified against the
pre-regeneration `grep withChartDataGridEvidence src/Controls/Catalog.fs` set.

## E3 — Generated catalog artifacts (existing, regenerated)

`src/Controls/catalog.yml` and `src/Controls/Catalog.fs`. After regeneration each
of the 47 rows lives inside its own `BEGIN/END GENERATED: typed-catalog/<id>`
region; **zero** rows are hand-maintained outside the markers (FR-002/SC-001).

**Validation**: `ControlsCatalogGenerationCheck` (`CatalogGen.currency` /
`isCurrent` / `currencyDrift`) over all 47 — a hand-edit to any region flips that
region to `Stale`/`Missing`, the gate fails, and the diagnostic names the stale
`typed-catalog/<id>` region + the regeneration command (FR-003/SC-002).

## E4 — `typedPropsById` lockstep map (existing, `tests/Controls.Tests/CatalogTests.fs`)

`Map<string, fact>` / `Map<string, System.Type>` mapping each catalog id to its
fact and `Props` record type. Extended from 6 to the full typed set so the
cross-check assertions iterate the complete catalog. `custom-control` is excluded
from the Props-field assertion (bridge-typed). Must stay in lockstep with E1
(FR-005/SC-003).

## E5 — Per-fact parity fixtures (new instances of an existing scheme)

Golden byte snapshots read by the `066` fixture-iteration test from
`specs/066-typed-catalog-generation/readiness/parity-fixtures/`:

- `Catalog.fs.<id>.txt` — expected `renderFSharpRow fact` output.
- `catalog.yml.<id>.txt` — expected `renderYamlRow fact` output.

One pair per id. Currently 6 ids (12 files); this feature adds the 41 remaining
ids (82 files). **Captured from real generator output** (golden), trailing
newline trimmed as the test does (`.TrimEnd('\n')`). Invariant (FR-005): a fixture
exists for every fact; a missing fixture fails the iteration.

## E6 — Typed gallery panel (extended sample value, `samples/ControlsGallery/Program.fs`)

`typedAuthoringPanel : Control<Msg>` — a `Control` value composed purely from
`FS.Skia.UI.Controls.Typed.*` `view` calls. Extended from {TextBlock, Button,
CheckBox} to ≥1 control per mechanic group (R5). Constraint (SC-004): no `Attr` /
`*.create` call appears in the panel code. Stateful controls reuse the existing
`070` MVU models (no new `Model`/`Msg`).

## E7 — Typed gallery render evidence (new readiness artifact)

`specs/071-typed-controls-followups/readiness/controls-rendering.md` — render-only
viewport evidence for E6, at ≥2 viewports, deterministic (byte-identical
re-capture), **no** `[S]`/`[S*]` disclosure (FR-009/SC-006).

## Entity relationships

```
E1 catalogFacts (47) ──renders──> E3 catalog.yml / Catalog.fs (47 marked regions)
        │                               │
        │ E2 evidence-id set            └─ currency-gated by ControlsCatalogGenerationCheck
        │ (6 rows + evidence pointer)
        │
        ├──cross-checked-by──> E4 typedPropsById (lockstep) + E5 parity fixtures (066 test)
        │
E6 typed gallery panel ──rendered/asserted──> RenderingTests/AccessibilityTests (≥2 viewports)
        │
        └──captured──> E7 controls-rendering.md (render-only, deterministic)
```
