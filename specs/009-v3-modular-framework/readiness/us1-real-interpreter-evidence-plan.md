# US1 Real-Interpreter Evidence Plan

## Scope

US1 validates the default V3 generated product as a framework consumer with
Scene, SkiaViewer, Elmish, KeyboardInput, Layout, and Charts selected.

## Source And Package Rows

| Row | Generated root | Evidence |
|-----|----------------|----------|
| `app-source` | `artifacts/generated-products/009-v3-modular-framework/app-source` | `generated-file-lists/app-source.txt`, `generated-product-verify/app-source/*.log` |
| `app-package` | `artifacts/generated-products/009-v3-modular-framework/app-package` | `generated-file-lists/app-package.txt`, `generated-product-verify/app-package/*.log` |

## Commands

The framework `GeneratedProductCheck` target creates the generated product
rows, copies selected local skills, scans file lists, and runs each generated
product's `Dev`, `Test`, and `Verify` commands through the generated product
`fake.sh` wrapper.

## Required Verdict

- exactly one product app
- exactly one product test suite
- selected skills for project, Scene, SkiaViewer, Elmish, KeyboardInput, Layout, and Charts
- consumer-mode package references for the default capabilities
- no framework samples, galleries, parity suite, historical specs, framework readiness, framework docs, framework README content, framework implementation projects, template package project, or generated validation roots in the default product file list
