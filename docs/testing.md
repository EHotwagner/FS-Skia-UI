# Testing Workflow

Use `./fake.sh build -t Dev` for the normal local check. It restores tools and
packages, builds the solution, and runs the default non-visual test projects:

| Test Scope | Project |
|------------|---------|
| Core library semantics | `tests/Lib.Tests/Lib.Tests.fsproj` |
| Scene package semantics | `tests/Scene.Tests/Scene.Tests.fsproj` |
| SkiaViewer host semantics | `tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` |
| Elmish adapter semantics | `tests/Elmish.Tests/Elmish.Tests.fsproj` |
| KeyboardInput runtime semantics | `tests/KeyboardInput.Tests/KeyboardInput.Tests.fsproj` |
| Layout semantics | `tests/Layout.Tests/Layout.Tests.fsproj` |
| Controls, charts, graph, DataGrid, and rich rendering semantics | `tests/Controls.Tests/Controls.Tests.fsproj` |
| Parity semantics | `tests/Parity.Tests/Parity.Tests.fsproj` |
| Sample contract checks | `tests/Smoke.Tests/Smoke.Tests.fsproj` |
| Governance command and docs checks | `tests/Governance.Tests/Governance.Tests.fsproj` |

The full testing and evidence target set is `Dev`, `Verify`, `Ci`,
`VerifyPreflight`, `CiPreflight`, `PackLocal`, `RefreshSurfaceBaselines`, `PackageSurfaceCheck`,
`FsiTranscripts`, `SampleContractSmoke`, `TemplateCheck`,
`DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`,
`StaleBoundaryScan`, `EvidenceGraph`, `EvidenceAudit`, and `FinalReadiness`.

Governance tests include section-aware generated guidance checks, semantic
template drift fixtures, public record invariant inventory coverage, and build
workflow command-contract checks. The process reliability contract tests cover
process-health snapshots, bootstrap validation, broad verification verdicts,
focused-gate direct invocation, stale build/restore diagnostics, scanner
fixture helpers, and pure `BuildModel`/`BuildMsg`/`BuildEffect` transition
assertions. Layout tests include recoverable Yoga execution fallback
diagnostics through the existing `LayoutDiagnostic` surface.

`PackageSurfaceCheck` runs `tests/Package.Tests/Package.Tests.fsproj` against
the stable package surface baselines in `readiness/surface-baselines/*.txt`.
The target verifies that the current exported public names still satisfy the
reviewed baseline.

`FsiTranscripts` runs:

| Script | Transcript |
|--------|------------|
| `scripts/prelude.fsx` | active feature `readiness/fsi/prelude.txt` |
| `scripts/input-prelude.fsx` | active feature `readiness/fsi/input-prelude.txt` |
| `scripts/keyboardinput-package-prelude.fsx` | active feature `readiness/fsi/keyboardinput-package-prelude.txt` |
| `scripts/layout-prelude.fsx` | active feature `readiness/fsi/layout-prelude.txt` |
| `scripts/controls-prelude.fsx` | active feature `readiness/fsi/controls-prelude.txt` |
| `scripts/controls-elmish-prelude.fsx` | active feature `readiness/fsi/controls-elmish-prelude.txt` |

`SampleContractSmoke` runs every sample that exposes `--contract-smoke` and
writes one log per sample under
`specs/006-template-framework-governance/readiness/sample-smoke/`.

## V3 Template Matrix

`TemplateCheck` validates eight rows:

| Artifact | Profile | Required checks |
|----------|---------|-----------------|
| source directory | app | install, instantiate, placeholder scan, excluded-history scan, V3 package-reference scan, generated `Dev` |
| source directory | headless-scene | install, instantiate, placeholder scan, excluded-history scan, V3 package-reference scan, generated `Dev` |
| source directory | governed | install, instantiate, placeholder scan, excluded-history scan, V3 package-reference scan, generated `Dev` |
| source directory | sample-pack | install, instantiate, placeholder scan, excluded-history scan, V3 package-reference scan, generated `Dev` |
| local package | app | package install, instantiate, placeholder scan, excluded-history scan, V3 package-reference scan, generated `Dev` |
| local package | headless-scene | package install, instantiate, placeholder scan, excluded-history scan, V3 package-reference scan, generated `Dev` |
| local package | governed | package install, instantiate, placeholder scan, excluded-history scan, V3 package-reference scan, generated `Dev` |
| local package | sample-pack | package install, instantiate, placeholder scan, excluded-history scan, V3 package-reference scan, generated `Dev` |

`DependencyReport`, `GeneratedGuidanceCheck`, and `TemplateDrift` are focused
governance targets and can be run independently while developing those
surfaces.

## Process Health And Focused Gate Checks

Run broad preflight checks directly when diagnosing runner health:

```bash
./fake.sh build -t VerifyPreflight
./fake.sh build -t CiPreflight
```

These targets write `process-health.md`, `bootstrap-runner.md`, and
`verification-verdicts.md`. Malformed threshold overrides, missing runner
dependencies, CoreCLR startup failures, process pressure, and unsupported
signals are classified as environment evidence.

Focused gates should be run directly when isolating a failure:

```bash
./fake.sh build -t PackageSurfaceCheck
./fake.sh build -t ControlsCatalogCheck
./fake.sh build -t ControlsInteractionCheck
./fake.sh build -t ControlsRenderingCheck
./fake.sh build -t DependencyReport
```

If a focused gate relies on restored or built artifacts, stale assumptions must
produce a diagnostic naming the affected gate and the remediation command.
The command-contract tests fail if a focused gate is recoupled to `Verify`,
`Ci`, or undocumented broad work.

## Template Exclusions

Full visual evidence, release validation, an external template repository split,
and distribution automation are deferred. They must not become pass/fail
requirements for `Dev`, `Verify`, or `Ci` until a later roadmap phase adds
explicit targets.

Package consumer smoke is intentionally opt-in through `PackageSmoke`, which
sets `FS_SKIA_RUN_PACKAGE_CONSUMER_SMOKE=1` for the package test project.
