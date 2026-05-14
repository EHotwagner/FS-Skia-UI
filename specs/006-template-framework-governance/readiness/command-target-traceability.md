# Command Target Traceability

| Target | Implementation | Tests | Docs | Evidence |
|--------|----------------|-------|------|----------|
| `Dev` | `build.fsx` | `tests/Governance.Tests` | `docs/build.md`, `docs/testing.md` | `readiness/logs/dev-verdict.txt` |
| `Verify` | `build.fsx` | `tests/Governance.Tests` | `docs/build.md`, `docs/evidence.md` | `readiness/logs/verify-verdict.txt` |
| `Ci` | `build.fsx`, `.specify/workflows/speckit/workflow.yml` | `tests/Governance.Tests` | `docs/build.md` | `readiness/logs/ci-verdict.txt` |
| `PackLocal` | `build.fsx` | `tests/Package.Tests`, `tests/Governance.Tests` | `docs/build.md`, `docs/testing.md` | `readiness/logs/pack-local.txt` |
| `RefreshSurfaceBaselines` | `build.fsx`, `scripts/refresh-surface-baselines.fsx` | `tests/Governance.Tests` | `docs/build.md`, `docs/evidence.md` | `readiness/surface-baselines/*.txt` |
| `PackageSurfaceCheck` | `build.fsx`, `tests/Package.Tests/SurfaceAreaTests.fs` | `tests/Package.Tests` | `docs/testing.md`, `docs/evidence.md` | `readiness/logs/package-surface-check.txt` |
| `FsiTranscripts` | `build.fsx`, `scripts/*prelude*.fsx` | `tests/Governance.Tests` | `docs/testing.md`, `docs/evidence.md` | `readiness/fsi/*.txt` |
| `SampleContractSmoke` | `build.fsx`, `samples/* --contract-smoke` | `tests/Smoke.Tests`, `tests/Governance.Tests` | `docs/testing.md`, `docs/evidence.md` | `readiness/sample-smoke/*.txt` |
| `EvidenceGraph` | `build.fsx`, evidence extension | `tests/Governance.Tests` | `docs/evidence.md` | `readiness/task-graph.json`, `.md` |
| `EvidenceAudit` | `build.fsx`, evidence extension | `tests/Governance.Tests` | `docs/evidence.md` | `readiness/logs/evidence-audit.txt` |
