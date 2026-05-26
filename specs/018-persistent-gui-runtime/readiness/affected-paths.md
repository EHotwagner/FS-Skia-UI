# Affected Path Inventory

Task: T004

## Source

- `src/SkiaViewer/SkiaViewer.fsi`
- `src/SkiaViewer/SkiaViewer.fs`
- `src/Elmish/Elmish.fsi`
- `src/Elmish/Elmish.fs`
- `src/KeyboardInput/KeyboardInput.fsi`
- `src/KeyboardInput/KeyboardInput.fs`
- `src/Scene/Scene.fsi`
- `src/Scene/Scene.fs`
- `src/Testing/Testing.fsi`
- `src/Testing/Testing.fs`

## Template And Generated Product

- `template/base/src/Product/Program.fs`
- `template/base/tests/Product.Tests/Tests.fs`
- `template/base/tests/Product.Tests/Program.fs`
- `template/base/src/Product/Product.fsproj`
- `template/base/tests/Product.Tests/Product.Tests.fsproj`
- `template/base/Directory.Packages.props`
- `template/base/README.md`
- `template/base/docs/product.md`
- `template/base/build.fsx`

## Tests

- `tests/SkiaViewer.Tests/Tests.fs`
- `tests/SkiaViewer.Tests/Program.fs`
- `tests/Governance.Tests/GeneratedProjectValidationTests.fs`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs`
- `tests/Governance.Tests/GovernanceEvidenceTests.fs`
- `tests/Governance.Tests/PersistentViewerEvidenceTests.fs`
- `tests/Governance.Tests/SyntheticErrorEvidenceTests.fs`
- `tests/Testing.Tests/Tests.fs`
- `tests/Scene.Tests/Tests.fs`
- `tests/KeyboardInput.Tests/Tests.fs`
- `tests/Elmish.Tests/Tests.fs`

## Build, Package, And Governance

- `build.fsx`
- `Directory.Packages.props`
- `.specify/extensions/evidence/scripts/python/compute-task-graph.py`
- `.specify/extensions/evidence/scripts/bash/run-audit.sh`
- `.specify/extensions/evidence/audit-patterns.yml`
- `scripts/refresh-surface-baselines.fsx`
- `scripts/dependency-report.fsx`
- `scripts/template-drift.fsx`

## Docs And Readiness

- `docs/build.md`
- `docs/evidence.md`
- `docs/generated-apps.md`
- `docs/runtime-design.md`
- `specs/018-persistent-gui-runtime/contracts/launch-runtime-contract.md`
- `specs/018-persistent-gui-runtime/contracts/generated-verification-contract.md`
- `specs/018-persistent-gui-runtime/contracts/readiness-evidence-contract.md`
- `specs/018-persistent-gui-runtime/readiness/`
