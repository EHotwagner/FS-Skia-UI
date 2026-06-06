# Catalog Single-Source — Maintainer Add Recipe (072, US2)

Adding a new catalog control is a **one-edit-plus-regenerate** operation. The
single source is `build/Governance/CatalogGen.fs` `catalogFacts`; `catalog.yml`
and `Catalog.fs` are generated from it and currency-enforced by
`ControlsCatalogGenerationCheck`.

## Recipe (as performed for the five 072 controls)

1. **Add the fact** to `CatalogGen.catalogFacts` (one `fact id displayName
   category module purpose required events role` line per control). 47 → 52.
2. **Place the marker pairs** `BEGIN/END GENERATED: typed-catalog/<id>` in both
   `src/Controls/catalog.yml` and `src/Controls/Catalog.fs` (the splice only
   *replaces* existing markers, so the regions must exist first). A compilable
   placeholder row inside each region is fine — regeneration overwrites it.
3. **Bump** the `catalog.yml` `supportedCount` header (47 → 52).
4. **Regenerate**: `./fake.sh build -t RefreshSurfaceBaselines` splices the
   rendered rows from the fact table into both files (and refreshes the surface
   baseline). No row is hand-maintained.
5. **Verify**: `./fake.sh build -t ControlsCatalogGenerationCheck` = Ok; the
   `CatalogTests` cross-check maps each id → its typed `Props` type and each
   `RequiredAttributes` entry (PascalCased) to a `Props` field.

## US2 independent validation path

Add the ids to `catalogFacts` → regenerate → confirm the new rows appear in both
artifacts → hand-edit one generated row and confirm the gate fails naming the
region → revert and confirm it passes (see
[`control-catalog-generation.md`](./control-catalog-generation.md)).

## Cross-check additions

`tests/Controls.Tests/CatalogTests.fs` `typedPropsById` gained the 5 ids
(`toggle-button`→`ToggleButtonProps`, `split-button`→`SplitButtonProps`,
`date-picker`→`DatePickerProps`, `time-picker`→`TimePickerProps`,
`color-picker`→`ColorPickerProps`); the `supportedCount` assertion is 52; the
"catalogFacts corresponds to exactly the 52 catalog typed ids" test passes.
