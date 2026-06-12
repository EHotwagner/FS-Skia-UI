# Generated-product check — feature 106 (T012, FR-003)

`./fake.sh build -t GeneratedProductCheck` — **Status: Ok (PASS)**.

The regenerated starter — now authoring through the typed Props front door — compiles and
renders the same controls with no behaviour regression:

- The full prerequisite chain ran green: `Dev` (Restore/Build/SampleContractSmoke/Test — 579
  Governance.Tests passing), `TemplateCheck`, `GeneratedProductStructure`,
  `GeneratedConsumerValidation`.
- `readiness/generated-file-lists/summary.md`: **PASS** — "generated product file lists,
  selected skills, Controls-owned form/chart/graph/DataGrid authoring, Controls.Elmish adapter
  references, … passed."
- The instantiated generated projects author through the typed front door:
  `artifacts/generated-products/106-controls-api-discoverability/app-source/src/Product/View.fs`
  references the `Controls.Typed` modules and lowers via `Widget.toControl`.
- The bundled per-control catalog reference reaches the generated project:
  `artifacts/generated-products/106-controls-api-discoverability/{app-source,headless-scene-source,sample-pack-source}/docs/controls-catalog.md`.

FR-003 (no behaviour regression): the typed front door lowers structurally equal to the legacy
builders (`tests/Controls.Tests/TypedLoweringTests.fs`, 13 cases incl. the RichText/LineChart/
GraphView cases added in T008, all green), so the starter renders the same controls it did
before.

- failure-class: generated-product-defect (none observed)
