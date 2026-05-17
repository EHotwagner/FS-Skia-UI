# Generated Product Usage Evidence

Status: US4 generated product validation evidence captured through T070.

## Required Evidence

- Generated app profiles include Controls package references and product-owned
  examples for form controls, rich rendering, chart or graph controls,
  DataGrid, generic message flow, and Elmish adapter integration where selected.
- Generated products do not include `FS.Skia.UI.Charts`, framework
  implementation projects, copied framework samples, historical specs, or
  readiness evidence.

## Current Generated Product Wiring

`build.fsx` has `GeneratedProductCheck`, generated file-list collection,
copied-asset scans, app profile package-reference checks, Controls.Elmish
adapter package inclusion, stale Charts exclusions, and Controls-owned
form/chart/graph/DataGrid authoring diagnostics.

## Profile And Package Evidence

| Profile row | Required Controls-boundary package evidence |
|-------------|---------------------------------------------|
| `app-source` | `FS.Skia.UI.Controls`, `FS.Skia.UI.Controls.Elmish`, `FS.Skia.UI.KeyboardInput`, `FS.Skia.UI.Layout`, `FS.Skia.UI.Elmish`, `FS.Skia.UI.Scene`, `FS.Skia.UI.SkiaViewer` |
| `app-package` | Same package set as `app-source`; generated from the packaged template payload. |
| `headless-scene-source` | `FS.Skia.UI.Scene` only; no Controls, adapter, or chart/DataGrid generated authoring path. |
| `governed-source` | `FS.Skia.UI.Scene` and `FS.Skia.UI.Testing`; Controls remains optional governed capability metadata. |
| `sample-pack-source` | Scene, SkiaViewer, Elmish, and sample assets; no generated Controls skill unless Controls is selected. |

## US2 Red Test Evidence

- `readiness/logs/t041-us2-composition-red.txt`: targeted governance tests
  require the generated product template to compose form inputs, a chart, and a
  DataGrid through `FS.Skia.UI.Controls` only; the current template stops at
  basic form controls.

## US3 T051 Generated Product Red Evidence

- Added generated product composition assertions in
  `tests/Governance.Tests/ControlsBoundaryCompositionTests.fs`.
- Red log: `readiness/logs/t051-generated-product-red.txt`.
- Current failures are expected before US3 implementation: the generated
  product template uses basic form controls but does not yet compose
  `LineChart.create`, `DataGrid.create`, `DataGrid.columns`, or
  `DataGrid.rows`, and product-owned tests do not yet assert the form, chart,
  and DataGrid example surface.

## US3 T054 Package Reference Update

- `template/base/src/Product/Product.fsproj` and
  `template/base/Directory.Packages.props` now include
  `FS.Skia.UI.Controls.Elmish` for app profiles that select Controls and
  Elmish.
- Generated Controls and Elmish fragments describe generic `Control<'msg>`
  views and adapter commands/subscriptions.
- Evidence: `readiness/logs/t054-guidance-profile.txt`.

## US3 T056 Generated Example Evidence

- Updated `template/base/src/Product/Program.fs` with product-owned examples
  for form controls, rich text, `LineChart`, `GraphView`, `DataGrid`, generic
  product messages, and `FS.Skia.UI.Controls.Elmish` adapter program wiring.
- Updated `template/base/tests/Product.Tests/Tests.fs` so generated product
  tests cover constructible form, chart, DataGrid, and adapter views.
- Pass log: `readiness/logs/t056-generated-product-composition.txt`.

## US3 T057 Generated Check Diagnostics

- `build.fsx` now adds `FS.Skia.UI.Controls.Elmish` for generated products that
  select both Controls and Elmish.
- `GeneratedProductCheck` diagnostics now name Controls-owned
  form/chart/graph/DataGrid authoring, Controls.Elmish adapter references,
  stale Charts exclusions, removed Charts package references, copied framework
  samples/specs/readiness/docs, and framework implementation paths.
- `GeneratedGuidanceCheck` now validates generated Controls guidance, DataGrid,
  adapter wiring, and Charts replacement notes.
- Evidence:
  - `readiness/logs/t057-governance-tests.txt`
  - `readiness/logs/t057-active-guidance-stale-scan.txt`
  - `readiness/logs/t057-template-drift-script.txt`
  - `readiness/template-drift-target-output.md`
- Local `./fake.sh build -t GeneratedGuidanceCheck` remains blocked by the
  existing FAKE cache issue (`readiness/logs/t057-generated-guidance-check.txt`);
  the target logic is covered by governance tests and direct script evidence.

## US3 T059 Independent Validation

- Validation path: `readiness/us3-validation.md`.

## US4 T070 Generated Validation Roots

- Generated source-template app root:
  `artifacts/generated-products/011-controls-boundary-refactor/app-source`.
- Generated package-template app root:
  `artifacts/generated-products/011-controls-boundary-refactor/app-package`.
- Both validation roots include product-owned Controls examples for rich text,
  `LineChart`, `GraphView`, `DataGrid`, and `ControlsElmish.program`.
- Both validation roots include `FS.Skia.UI.Controls`,
  `FS.Skia.UI.Controls.Elmish`, `FS.Skia.UI.KeyboardInput`,
  `FS.Skia.UI.Layout`, `FS.Skia.UI.Elmish`, `FS.Skia.UI.Scene`, and
  `FS.Skia.UI.SkiaViewer` package references.
- Generated file-list scans reject removed Charts package references and copied
  framework implementation paths such as framework source projects, framework
  samples, historical specs, framework readiness evidence, and framework docs.
- Evidence:
  - `readiness/logs/t070-template-source-install.txt`
  - `readiness/logs/t070-app-source-generate.txt`
  - `readiness/logs/t070-template-pack.txt`
  - `readiness/logs/t070-template-package-install.txt`
  - `readiness/logs/t070-app-package-generate.txt`
  - `readiness/logs/t070-generated-product-scan.txt`
  - `readiness/logs/t070-generated-validation-governance.txt`
  - `readiness/generated-file-lists/app-source.txt`
  - `readiness/generated-file-lists/app-package.txt`
  - `readiness/generated-file-lists/summary.md`

## T078 Generated Product Gates

| Gate | Log Or Report | Verdict | Duration |
|------|---------------|---------|----------|
| `./fake.sh build -t TemplateCheck` | `readiness/logs/t078-template-check.txt` | PASS | 28s |
| `./fake.sh build -t GeneratedProductCheck` | `readiness/logs/t078-generated-product-check.txt` | PASS | 12s |
| Generated file-list summary | `readiness/generated-file-lists/summary.md` | PASS | generated by target |

The generated product scanner keeps `samples/` forbidden for ordinary generated
products while allowing the explicit `sample-pack` profile to include generated
sample-pack content (`samples/README.md` and `samples/skill/SKILL.md`). The
updated scan continues to reject copied framework sample projects, framework
source projects, historical specs, readiness evidence, and stale Charts package
references.

## T079 Representative Controls Product Commands

| Generated product | Command logs | Verdict |
|-------------------|--------------|---------|
| `app-source` | `readiness/generated-product-verify/app-source/dev.log`, `test.log`, `verify.log` | PASS |
| `app-package` | `readiness/generated-product-verify/app-package/dev.log`, `test.log`, `verify.log` | PASS |

The representative Controls selections ran generated `Dev`, `Test`, and
`Verify` through `GeneratedProductCheck`. The generated file/package-reference
inventories are `readiness/generated-file-lists/app-source.txt` and
`readiness/generated-file-lists/app-package.txt`; the focused inventory scan is
`readiness/logs/t079-generated-app-inventory.txt`.
