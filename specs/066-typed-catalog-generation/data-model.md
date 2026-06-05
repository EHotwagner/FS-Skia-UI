# Phase 1 Data Model: Typed Catalog Generation

This feature introduces no new *runtime* entity. The public
`ControlDefinition` / `CatalogAccessibility` shapes (`src/Controls/Catalog.fsi`)
are unchanged. The new entities below live in the build front
(`FS.Skia.UI.Build`) and are the single source + generation/currency values.

## Entity: `TypedCatalogFact` (the single source) — `build/Governance/CatalogGen.fs`

The canonical, one-place declaration of a typed control's catalog-relevant facts.
Exactly six instances (one per `065` control). Field names mirror the catalog row
so the mapping is mechanical and reviewable.

| Field | Type | Notes |
| --- | --- | --- |
| `Id` | `string` | e.g. `"button"`; the marker region key `typed-catalog/<Id>`; also the join key to the `Typed` module |
| `DisplayName` | `string` | e.g. `"Button"` |
| `Category` | `string` | one of the existing 10 categories (display/input/selection/navigation/layout/feedback/data/chart/graph/custom) — generation does not add categories (FR-003) |
| `Module` | `string` | the owning module name as it appears in the catalog (`"Button"`, `"TextBlock"`, …) |
| `Purpose` | `string` | one-line description, byte-identical to today's row |
| `RequiredAttributes` | `string list` | e.g. `["text"]`; cross-checked against the typed `Props` required fields by test (R5) |
| `Events` | `string list` | catalog event names, e.g. `["onClick"]` |
| `AccessibilityRole` | `string` | e.g. `"Button"`; feeds `CatalogAccessibility.Role` |

**Derived/shared facts (not stored per fact)**: `CommonAttributes`,
`VisualStates`, `NameSource`, `StateMetadata`, `FocusBehavior`,
`KeyboardOperation`, `ContrastEvidence`, `Examples`, `Tests`, `Evidence`,
`SupportStatus`, `Owner` come from the existing shared defaults
(`Catalog.common`, `Catalog.states`, `Catalog.accessibility`, the `definition`
constructor's literal lists, and `catalog.yml`'s `defaults:` block /
per-row evidence). Generation reproduces these from the same shared constants the
hand-authored rows use, so they stay byte-identical and are not re-declared per fact.

**Validation rules** (asserted by tests / the correspondence test):
- The set of `Id`s is exactly `{text-block, button, check-box, stack, text-box, data-grid}`.
- Each `Module` matches a real `FS.Skia.UI.Controls.Typed` module.
- `Category` ∈ the existing 10 categories; never introduces a new one.
- `RequiredAttributes` agree with the typed `Props` required (non-optional) fields.
- For `data-grid`: `Category = "data"` and `Module = "DataGrid"` (preserves the
  invariants `Catalog.validate` already enforces, `Catalog.fs:307-311`).

## Entity: generated row mappings (pure renderers) — `CatalogGen`

Two pure functions render a `TypedCatalogFact` to the exact on-disk text of each
target, reusing the shared constants so output is byte-identical to today:

- `renderFSharpRow : TypedCatalogFact -> string` → the `definition <id> <displayName>
  <category> <module> <purpose> [<required>] common [<events>] states "<role>"` line
  (plus `|> withChartDataGridEvidence` for `data-grid`, matching `Catalog.fs:93-94`).
- `renderYamlRow : TypedCatalogFact -> string` → the `- id: …` YAML block matching
  the `catalog.yml` row shape (`catalog.yml:31-45` style).

Each rendered row is spliced into its own marked region (R2) keyed by
`typed-catalog/<Id>`.

## Entity: currency result — `CatalogGen` (mirrors `GovernedBlocks.BlockCurrency`)

Result of comparing each on-disk region against a fresh render. Clean iff every
region in both files is `Current`.

| Field | Type | Notes |
| --- | --- | --- |
| `ControlId` | `string` | the divergent control, named in the failure diagnostic (FR-005) |
| `FilePath` | `string` | `src/Controls/catalog.yml` or `src/Controls/Catalog.fs` |
| `Status` | `Current \| Stale \| Missing` | `Stale` = region bytes differ from render; `Missing` = marker region absent |

`currencyDrift : currency -> string option` → `None` when current; otherwise a
diagnostic naming the file, the control, and the regeneration command
(`./fake.sh build -t RefreshSurfaceBaselines`) — same shape as
`GovernedBlocks.currencyDrift`.

## Relationship to existing entities (unchanged)

- `ControlDefinition` / `CatalogAccessibility` (`Catalog.fsi`) — **unchanged**;
  the six generated F# rows construct the same records via the existing
  `definition` helper.
- `Catalog.supportedControls` — same 47 entries, same order, same values.
- `Catalog.standardSchema` — **out of scope**; it is the typed-validation schema,
  not a catalog row. The correspondence test may optionally assert agreement
  between a fact's events/required and the matching `standardSchema` entry as a guard.
- `catalog.yml` `summary.supportedCount: 47`, `categories`, `defaults` — unchanged.
