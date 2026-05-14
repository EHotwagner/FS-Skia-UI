# Merge Summary

Implemented v1 Template Framework Governance:

- Added `.config/dotnet-tools.json`, `fake.sh`, `fake.cmd`, and `build.fsx`
  with targets `Dev`, `Verify`, `Ci`, `PackLocal`, `RefreshSurfaceBaselines`,
  `PackageSurfaceCheck`, `FsiTranscripts`, `SampleContractSmoke`,
  `EvidenceGraph`, and `EvidenceAudit`.
- Added a local workflow effect boundary in `build.fsx`: `BuildModel`,
  `BuildMsg`, `BuildEffect`, `init`, pure `update`, and an interpreter at the
  process/filesystem edge.
- Moved current package surface baselines to
  `readiness/surface-baselines/*.txt` and updated package tests plus the
  refresh script to use that stable path.
- Kept package consumer smoke as the explicit deferred `PackageSmoke` path.
- Added governance tests for target contracts, artifact paths, docs, workflow,
  and generated task guidance.
- Added `docs/build.md`, `docs/testing.md`, and `docs/evidence.md`.
- Updated README, Spec Kit workflow automation, and generated task guidance to
  call canonical targets.

Verification:

| Command | Result | Evidence |
|---------|--------|----------|
| `dotnet test tests/Governance.Tests/Governance.Tests.fsproj` | PASS | console run |
| `dotnet test tests/Package.Tests/Package.Tests.fsproj` | PASS | console run |
| `./fake.sh build -t Dev` | PASS | `readiness/logs/dev-verdict.txt` |
| `./fake.sh build -t RefreshSurfaceBaselines` | PASS | `readiness/logs/surface-refresh.txt` |
| `./fake.sh build -t PackageSurfaceCheck` | PASS | `readiness/logs/package-surface-check.txt` |
| `./fake.sh build -t FsiTranscripts` | PASS | `readiness/fsi/*.txt` |
| `./fake.sh build -t SampleContractSmoke` | PASS | `readiness/sample-smoke/*.txt` |
| `./fake.sh build -t PackLocal` | PASS | `~/.local/share/nuget-local/*.nupkg` |
| `./fake.sh build -t Verify` | PASS | `readiness/logs/verify-verdict.txt` |
| `./fake.sh build -t Ci` | PASS | `readiness/logs/ci-verdict.txt` |
| clean-copy `./fake.sh build -t Verify` | PASS | `readiness/clean-copy-verify.md` |
| graph-only audit | PASS | `readiness/logs/final-graph-only-audit.txt` |
| full evidence audit | PASS | `readiness/logs/final-evidence-audit.txt` |

Synthetic-evidence inventory: none.

Deferred roadmap boundaries: template packaging, dependency governance,
generated spec/plan hardening, layout evidence, visual evidence, package
consumer smoke, and release validation.
