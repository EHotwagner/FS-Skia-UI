# Contract — Single-Source Catalog Expansion (47 → 52)

The governance contract for adding new controls to the catalog under the `066`/`071`
single-source generation, without regressing currency.

## Single source

`build/Governance/CatalogGen.fs` `catalogFacts` gains 5 facts (in catalog file order):

| id | DisplayName | Category | Module | RequiredAttributes | Events | Role |
| --- | --- | --- | --- | --- | --- | --- |
| `toggle-button` | Toggle Button | input | `ToggleButton` | `["text"]` | `["onToggle"]` | Button |
| `split-button` | Split Button | input | `SplitButton` | `["text"]` | `["onClick";"onSelected"]` | Menu |
| `date-picker` | Date Picker | input | `DatePicker` | `[]` | `["onChange"]` | TextBox |
| `time-picker` | Time Picker | input | `TimePicker` | `[]` | `["onChange"]` | TextBox |
| `color-picker` | Color Picker | selection | `ColorPicker` | `["swatches"]` | `["onSelected"]` | List |

None is evidence-carrying, so `evidenceCarryingIds` and the chart/data-grid special-case are
**unchanged**.

## Generated artifacts (never hand-edited)

- `src/Controls/catalog.yml`: 5 new `BEGIN/END GENERATED: typed-catalog/<id>` regions; header
  `supportedCount` 47 → 52.
- `src/Controls/Catalog.fs`: 5 new `BEGIN/END GENERATED: typed-catalog/<id>` regions.
- Regenerated via `./fake.sh build -t RefreshSurfaceBaselines` — the generator emits the
  region bodies; the marker pairs are placed first (per the catalog-splice-marker rule).

## Contract guarantees (asserted by tests / gates)

1. **Currency**: `ControlsCatalogGenerationCheck` passes after regeneration; a deliberate
   hand-edit to **any** generated row (old or new) makes it fail, naming the stale
   `typed-catalog/<id>` region and the regen command. *(SC-003)*
2. **Cross-check**: `CatalogTests.typedPropsById` gains the 5 ids → their `*Props` types;
   every `RequiredAttributes` entry (PascalCased) is a field of the mapped `Props` type;
   `TypedMigrationTests` correspondence stays green. *(SC-002)*
3. **Count**: `catalog.yml` header is `supportedCount: 52`; the `CatalogTests.fs` assertion
   is `52`; `Catalog.supportedCount ()` returns 52 (row count, automatic).
4. **Parity fixtures**: one `Catalog.fs.<id>.txt` + `catalog.yml.<id>.txt` per new id under
   `specs/066-typed-catalog-generation/readiness/parity-fixtures/`, captured from generator
   output (golden bytes, not fabricated). The `066` fixture-iteration test stays green.
5. **No mechanism change**: marker grammar, `renderFSharpRow`, and the YAML renderer are
   reused as-is; only `catalogFacts` and the count constant change on the source side.
