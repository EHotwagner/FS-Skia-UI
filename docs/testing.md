# Testing Workflow

Use `./fake.sh build -t Dev` for the normal local check. It restores tools and
packages, builds the solution, and runs the default non-visual test projects:

| Test Scope | Project |
|------------|---------|
| Core library semantics | `tests/Lib.Tests/Lib.Tests.fsproj` |
| Charts semantics | `tests/Charts.Tests/Charts.Tests.fsproj` |
| Layout semantics | `tests/Layout.Tests/Layout.Tests.fsproj` |
| Parity semantics | `tests/Parity.Tests/Parity.Tests.fsproj` |
| Sample contract checks | `tests/Smoke.Tests/Smoke.Tests.fsproj` |
| Governance command and docs checks | `tests/Governance.Tests/Governance.Tests.fsproj` |

The full testing and evidence target set is `Dev`, `Verify`, `Ci`,
`PackLocal`, `RefreshSurfaceBaselines`, `PackageSurfaceCheck`,
`FsiTranscripts`, `SampleContractSmoke`, `EvidenceGraph`, and `EvidenceAudit`.

`PackageSurfaceCheck` runs `tests/Package.Tests/Package.Tests.fsproj` against
the stable package surface baselines in `readiness/surface-baselines/*.txt`.
The target verifies that the current exported public names still satisfy the
reviewed baseline.

`FsiTranscripts` runs:

| Script | Transcript |
|--------|------------|
| `scripts/prelude.fsx` | `specs/006-template-framework-governance/readiness/fsi/prelude.txt` |
| `scripts/charts-prelude.fsx` | `specs/006-template-framework-governance/readiness/fsi/charts-prelude.txt` |
| `scripts/input-prelude.fsx` | `specs/006-template-framework-governance/readiness/fsi/input-prelude.txt` |
| `scripts/layout-prelude.fsx` | `specs/006-template-framework-governance/readiness/fsi/layout-prelude.txt` |

`SampleContractSmoke` runs every sample that exposes `--contract-smoke` and
writes one log per sample under
`specs/006-template-framework-governance/readiness/sample-smoke/`.

## V1 Exclusions

Template packaging, dependency governance, generated spec/plan hardening,
layout evidence, visual evidence, package consumer smoke, and release
validation are deferred. They must not become pass/fail requirements for
`Dev`, `Verify`, or `Ci` until a later roadmap phase adds explicit targets.

Package consumer smoke is intentionally opt-in through `PackageSmoke`, which
sets `FS_SKIA_RUN_PACKAGE_CONSUMER_SMOKE=1` for the package test project.
