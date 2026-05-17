# Control Catalog

PASS: Controls catalog tests verified supported row count, metadata, examples, tests, evidence, accessibility, and Controls-owned chart/graph rows.

- supported-controls: 46
- categories: display, input, selection, navigation, layout, feedback, data, chart, graph, custom
- catalog-source: `src/Controls/catalog.yml`
- example: `samples/ControlsGallery/Program.fs`
- checks: `tests/Controls.Tests/CatalogTests.fs`
- chart-graph-owner: controls

## T076 Catalog Gate

| Gate | Log | Verdict | Duration |
|------|-----|---------|----------|
| `./fake.sh build -t ControlsCatalogCheck` | `readiness/logs/t076-controls-catalog-check.txt` | PASS | 2s |

The final split target runs the catalog slice with
`dotnet test tests/Controls.Tests/Controls.Tests.fsproj -m:1 --no-restore --filter Catalog`
and writes this catalog readiness report.
