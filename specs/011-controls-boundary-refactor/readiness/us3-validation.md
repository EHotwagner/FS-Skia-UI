# US3 Independent Validation

Status: US3 independently functional and testable through generated guidance,
generated product template governance, and direct template drift validation.

## Goal

Generated product consumers get one Controls path for ordinary controls, rich
text, chart controls, graph views, and DataGrid. Generated Elmish products use
`FS.Skia.UI.Controls.Elmish` at the program edge, and generated output avoids
removed Charts package guidance, stale chart skills, renderer-neutral wording,
and copied framework assets.

## Validation Path

| Step | Command | Evidence |
|------|---------|----------|
| Generated guidance and profile tests | `dotnet test tests/Governance.Tests/Governance.Tests.fsproj -m:1 --no-restore --filter "Generated project validation contract|Generated guidance hardening|Template drift governance|Controls boundary generated guidance|Template profile metadata|Controls boundary composition"` | `readiness/logs/t057-governance-tests.txt` |
| Generated product composition | `dotnet test tests/Governance.Tests/Governance.Tests.fsproj -m:1 --no-restore --filter "Controls boundary composition"` | `readiness/logs/t056-generated-product-composition.txt` |
| Template drift report | `dotnet fsi scripts/template-drift.fsx specs/011-controls-boundary-refactor/readiness/template-drift-target-output.md` | `readiness/logs/t057-template-drift-script.txt`, `readiness/template-drift-target-output.md` |
| Active generated guidance stale scan | `rg` over `build.fsx`, template fragments, spec/plan templates, template drift script, and local Controls skill | `readiness/logs/t057-active-guidance-stale-scan.txt` |
| Readiness capture scan | `rg` over generated guidance, generated product usage, and template drift readiness files | `readiness/logs/t058-readiness-capture-scan.txt` |

## Generated Product Consumer Contract

- `app-source` and `app-package` include `FS.Skia.UI.Controls`,
  `FS.Skia.UI.Controls.Elmish`, `FS.Skia.UI.KeyboardInput`,
  `FS.Skia.UI.Layout`, `FS.Skia.UI.Elmish`, `FS.Skia.UI.Scene`, and
  `FS.Skia.UI.SkiaViewer`.
- `headless-scene-source` remains Scene-only.
- `governed-source` remains Scene plus Testing with Controls available as
  optional governed metadata.
- `sample-pack-source` remains sample-focused and does not receive the Controls
  local skill unless Controls is selected.

## Generated Files Exercised

- `template/base/src/Product/Program.fs`
- `template/base/tests/Product.Tests/Tests.fs`
- `template/base/src/Product/Product.fsproj`
- `template/base/Directory.Packages.props`
- `template/fragments/controls/skill/SKILL.md`
- `template/fragments/elmish/README.md`
- `template/capabilities.yml`
- `src/Controls/skill/SKILL.md`

## Local Environment Note

`./fake.sh build -t GeneratedGuidanceCheck` is blocked locally by the existing
FAKE cache issue, captured in
`readiness/logs/t057-generated-guidance-check.txt`. The target logic is covered
by governance tests, direct `scripts/template-drift.fsx` execution, and stale
scan evidence above.
