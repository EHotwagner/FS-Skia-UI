# US4 Independent Validation

Status: US4 maintainer validation path captured through T072.

## Maintainer Goal

Validate that the refactored Controls boundary is auditable from public
contracts, package surfaces, dependency reports, generated guidance, generated
product roots, compatibility guidance, and command logs.

## Independent Validation Path

Run these focused checks when reviewing the US4 boundary:

| Step | Command or evidence | Expected verdict |
|------|---------------------|------------------|
| Public surface | `dotnet test tests/Package.Tests/Package.Tests.fsproj -m:1 --no-restore` | PASS: Controls, KeyboardInput, and Controls.Elmish baselines match exported public surfaces; Charts baseline is absent. |
| Dependency placement | `dotnet fsi scripts/dependency-report.fsx specs/011-controls-boundary-refactor/readiness/dependencies.md` | PASS: Controls has no direct external packages, KeyboardInput owns `YamlDotNet`, Controls.Elmish owns `Fable.Elmish`, and active project files do not reference the removed Charts package/project. |
| Governance evidence | `dotnet test tests/Governance.Tests/Governance.Tests.fsproj -m:1 --no-restore --filter "Controls boundary readiness evidence"` | PASS: readiness reports cover public surface, package boundary, generated guidance, dependency impact, compatibility impact, and lower-level path preservation. |
| Sample evidence | `dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj -m:1 --no-restore --filter "Controls boundary gallery"` plus direct gallery `--contract-smoke` logs | PASS: ControlsGallery, ChartsGallery, and DataGridGallery expose Controls-owned contract-smoke paths without the removed Charts package. |
| Generated products | `specs/011-controls-boundary-refactor/readiness/generated-file-lists/summary.md` | PASS: source and package app validation roots use Controls and Controls.Elmish package references and exclude copied framework implementation source. |
| Compatibility | `specs/011-controls-boundary-refactor/readiness/compatibility-impact.md` | PASS: replacement path is Controls-owned chart/graph/DataGrid authoring, lower-level paths remain available, and no compatibility shim, automated migration, or release publishing promise is made. |

The full `./fake.sh build -t Verify` and `./fake.sh build -t Ci` targets remain
the intended aggregate gates. The T080 aggregate runs reached `Test` and then
failed locally while VSTest started the `Lib.Tests` testhost under memory/process
pressure. The final logs are `readiness/logs/t080-verify.txt`,
`readiness/logs/t080-ci.txt`, and `readiness/logs/test.txt`; focused commands
above exercise the governed surfaces directly.

## Maintainer Review Checklist

- [ ] Public `.fsi` contracts exist for Controls, KeyboardInput, and
  Controls.Elmish implementation files.
- [ ] Surface baselines include Controls, KeyboardInput, and Controls.Elmish
  intentional exports and do not include `FS.Skia.UI.Charts.txt`.
- [ ] `src/Controls/Controls.fsproj` references only Scene, Layout, and
  KeyboardInput.
- [ ] `src/KeyboardInput/KeyboardInput.fsproj` owns the `YamlDotNet`
  dependency.
- [ ] `src/Controls.Elmish/Controls.Elmish.fsproj` owns the `Fable.Elmish`
  dependency and references only Controls and KeyboardInput.
- [ ] Generated app source and package roots reference Controls,
  Controls.Elmish, KeyboardInput, Layout, Elmish, Scene, and SkiaViewer.
- [ ] Generated app roots contain product-owned `RichText`, `LineChart`,
  `GraphView`, `DataGrid`, and `ControlsElmish.program` examples.
- [ ] Generated app roots do not copy framework source projects, framework
  samples, historical specs, framework readiness evidence, framework docs, or
  the removed Charts package/project.
- [ ] Compatibility guidance states the Controls replacement path and does not
  promise a compatibility shim, automated external-app migration, or release
  publishing automation.
- [ ] Lower-level Scene, Layout, KeyboardInput, SkiaViewer, and Elmish paths
  remain documented for products that do not select Controls.

## Evidence Index

- Public surface: `readiness/public-surface.md`
- Package boundary: `readiness/package-boundary.md`
- Dependency impact: `readiness/dependency-report.md` and
  `readiness/dependencies.md`
- Template drift and generated guidance: `readiness/template-drift.md` and
  `readiness/generated-guidance.md`
- Generated product roots: `readiness/generated-product-usage.md` and
  `readiness/generated-file-lists/summary.md`
- Compatibility: `readiness/compatibility-impact.md`
- Evidence coverage: `readiness/evidence-audit.md`
