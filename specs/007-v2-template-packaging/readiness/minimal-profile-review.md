# Minimal Profile Review

Evidence:

- Package minimal root: `artifacts/template-check/007-v2-template-packaging/package-minimal`
- Package minimal Dev log: `specs/007-v2-template-packaging/readiness/template/package-minimal/dev.log`
- Package minimal scan: `specs/007-v2-template-packaging/readiness/template/package-minimal/scan.md`

Verdict: PASS.

Required minimal contents are present:

- Core library: `src/Lib/Lib.fsproj`
- Core tests: `tests/Lib.Tests/Lib.Tests.fsproj`
- Package checks: `tests/Package.Tests/Package.Tests.fsproj`
- Governance checks: `tests/Governance.Tests/Governance.Tests.fsproj`
- Basic sample: `samples/BasicViewer/BasicViewer.fsproj`
- Central dependency policy: `Directory.Packages.props`
- Template/dependency/evidence/testing guidance under `docs/`
- Spec Kit governance assets under `.specify/`
- Root drift deferral policy: `readiness/template-deferrals.yml`

Optional scope is absent as intended:

- `src/Charts/`
- `src/Layout/`
- `tests/Charts.Tests/`
- `tests/Layout.Tests/`
- `tests/Parity.Tests/`
- `tests/Smoke.Tests/`
- Optional visual/sample projects beyond `samples/BasicViewer/`
- Historical `specs/00*` feature readiness directories

The minimal profile still completes generated `Dev`, including restore, build,
core non-visual tests, package checks, and governance checks.
