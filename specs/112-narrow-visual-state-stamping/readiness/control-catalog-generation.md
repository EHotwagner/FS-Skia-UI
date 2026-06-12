# Controls Catalog Generation

PASS: the six typed-catalog rows in src/Controls/catalog.yml and src/Controls/Catalog.fs are a current, byte-identical regeneration of CatalogGen.catalogFacts.

- generated-controls: 6 (text-block, button, text-box, check-box, data-grid, stack)
- generated-files: src/Controls/catalog.yml, src/Controls/Catalog.fs
- single-source: build/Governance/CatalogGen.fs (catalogFacts)
- regenerate: ./fake.sh build -t RefreshSurfaceBaselines
- failure-class: stale-generated-catalog