# Controls Catalog Generation

PASS: the six typed-catalog rows in src/Controls/catalog.yml and src/Controls/Catalog.fs are a current, byte-identical regeneration of CatalogGen.catalogFacts.

- generated-controls: 6 (text-block, button, text-box, check-box, data-grid, stack)
- generated-files: src/Controls/catalog.yml, src/Controls/Catalog.fs
- single-source: build/Governance/CatalogGen.fs (catalogFacts)
- regenerate: ./fake.sh build -t RefreshSurfaceBaselines
- failure-class: stale-generated-catalog

> Note: the PASS summary above is the gate's fixed template (it names the original
> six-row proof set). For feature 072 the single source `CatalogGen.catalogFacts`
> grew 47 → 52 and the five new rows (`toggle-button`, `split-button`,
> `date-picker`, `time-picker`, `color-picker`) are generated into both files; the
> committed tree is current (gate Status: Ok).

## T022 drift proof (SC-003)

Hand-editing one generated new row and re-running the gate WITHOUT regenerating:

1. Mutated `catalog.yml` `toggle-button` purpose →
   `./fake.sh build -t ControlsCatalogGenerationCheck` → **Failure** with:
   `src/Controls/catalog.yml is stale — its generated typed-catalog/toggle-button
   region no longer matches CatalogGen.catalogFacts. Regenerate via ./fake.sh build
   -t RefreshSurfaceBaselines.` (names the stale file, the divergent region, and
   the regeneration command.)
2. Reverted → `./fake.sh build -t ControlsCatalogGenerationCheck` → **Ok**.

Hand-editing any generated row (old or new) fails the currency gate.