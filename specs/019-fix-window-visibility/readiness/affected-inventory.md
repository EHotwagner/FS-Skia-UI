# Affected Inventory

## Source

- `src/SkiaViewer/SkiaViewer.fsi`: public launch/window/outcome/evidence contract.
- `src/SkiaViewer/SkiaViewer.fs`: viewer lifecycle, native window diagnostics, option application, close reasons, visual evidence handling.
- `src/SkiaViewer/SkiaViewer.fsproj`: package surface and dependency impact if new implementation files or dependencies are required.
- `src/Elmish/Elmish.fsi` and `src/Elmish/Elmish.fs`: review only if shared MVU primitives need public changes.
- `src/KeyboardInput/KeyboardInput.fsi` and `src/KeyboardInput/KeyboardInput.fs`: input-device observation and dispatch evidence.
- `src/Scene/Scene.fsi` and `src/Scene/Scene.fs`: scene rendering, pixel-readback, and image evidence support.
- `src/Testing/Testing.fsi` and `src/Testing/Testing.fs`: generated validation contracts and helpers.

## Template And Generated Product

- `template/base/src/Product/Program.fs`: default interactive launch path and explicit evidence commands.
- `template/base/tests/Product.Tests/Tests.fs`: generated product tests for launch, evidence, diagnostics, options, and close reasons.
- `template/base/tests/Product.Tests/Program.fs` and `template/base/tests/Product.Tests/Product.Tests.fsproj`: generated test wiring.
- `template/base/README.md` and `template/base/docs/product.md`: generated command and evidence contract docs.
- `template/base/build.fsx`, `template/base/fake.sh`, and `template/base/fake.cmd`: generated `Test`/`Verify` behavior where required.
- `template/base/Directory.Packages.props`: package resolution expectations if package versions or sources change.
- `.template.config/template.json`: review if new generated files, defaults, or artifacts are added.

## Tests

- `tests/Governance.Tests/GeneratedProjectValidationTests.fs`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs`
- `tests/Governance.Tests/GovernanceEvidenceTests.fs`
- `tests/Governance.Tests/CommandContractTests.fs`
- `tests/Governance.Tests/ProcessReliabilityContractTests.fs`
- `tests/Governance.Tests/SyntheticErrorEvidenceTests.fs`
- `tests/Governance.Tests/TemplateWorkflowTests.fs`
- `tests/Governance.Tests/TemplateDriftTests.fs`
- `tests/Governance.Tests/PublicRecordInvariantTests.fs`
- `tests/Testing.Tests/Tests.fs`
- New or existing `tests/SkiaViewer.Tests/` coverage for semantic viewer contracts.

## Build And Governance Targets

- `build.fsx`: `Verify`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`, `DependencyReport`, `EvidenceGraph`, `EvidenceAudit`, `RefreshSurfaceBaselines`, `PackageSurfaceCheck`, `PackLocal`, generated validation output, and readiness paths.
- `fake.sh` and `fake.cmd`: target entry points.
- `scripts/refresh-surface-baselines.fsx`: surface baseline refresh.
- `scripts/dependency-report.fsx`: package/dependency impact report.
- `.specify/extensions/evidence/scripts/python/compute-task-graph.py`: graph metadata and propagation.
- `.specify/extensions/evidence/scripts/bash/run-audit.sh`: audit execution.
- `.specify/extensions/evidence/audit-patterns.yml`: readiness/diff-scan blocking patterns.

## Documentation

- `docs/build.md`
- `docs/evidence.md`
- `docs/generated-apps.md`
- `docs/runtime-design.md`
- `docs/testing.md`
- `docs/dependencies.md`
- `docs/2026-05-26-2227-silk-window-platform-analysis.md`

## Package Guidance And Baselines

- `Directory.Packages.props`
- `template/base/Directory.Packages.props`
- `docs/dependencies.md`
- `specs/019-fix-window-visibility/readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`
- Local package output under `~/.local/share/nuget-local/` when `PackLocal` is run.

## Fixtures And Readiness Paths

- `specs/019-fix-window-visibility/readiness/interactive-visible-window.md`
- `specs/019-fix-window-visibility/readiness/close-reason-separation.md`
- `specs/019-fix-window-visibility/readiness/window-state-diagnostics.md`
- `specs/019-fix-window-visibility/readiness/window-options.md`
- `specs/019-fix-window-visibility/readiness/real-image-evidence.md`
- `specs/019-fix-window-visibility/readiness/generated-validation.md`
- `specs/019-fix-window-visibility/readiness/evidence-audit.md`
- `specs/019-fix-window-visibility/readiness/task-graph.md`
- `specs/019-fix-window-visibility/readiness/task-graph.json`
- `specs/019-fix-window-visibility/readiness/logs/`
