# Controls Catalog Docs

PASS: the published Controls docs section is a current, complete, honest projection of CatalogGen.catalogFacts (52 controls).

- supported-controls: 52
- generated-index: docs/controls/catalog.md (catalog-docs/index region)
- detail-pages: one per control; catalog-docs/<id> header region current
- previews-present: 51 (validated decodable, non-1x1, non-trivial)
- rendered-previews: 51 (render-only, non-1x1, >= 420-byte trivial floor)
- unsupported-declared: 1 (honest no-image: 'preview-status: unsupported')
- reconciled: 51 rendered + 1 unsupported == 52 supported controls
- api-links: resolved against output/reference/
- single-source: build/Governance/CatalogGen.fs (catalogFacts)
- regenerate: ./fake.sh build -t RefreshSurfaceBaselines
- failure-class: none