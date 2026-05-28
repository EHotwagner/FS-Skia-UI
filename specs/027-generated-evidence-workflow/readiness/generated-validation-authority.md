# Generated Validation Authority

Generated `EvidenceGraph` and `EvidenceAudit` now delegate to the copied Spec
Kit evidence scripts instead of writing completion-only placeholder logs.

## T005 Failing-First Governance Tests

- captured_at: `2026-05-28T15:38:36+02:00`
- command: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj -- --filter "generated evidence targets"`
- log_path: `specs/027-generated-evidence-workflow/readiness/logs/t005-generated-evidence-tests.txt`
- expected_result: failing-first test failure before implementation.
- observed_failure: `generated evidence targets must not share the completion-only placeholder logger`
- contract_gap: generated `EvidenceGraph` and `EvidenceAudit` still route through `writeLog target`, which writes completion-only output instead of delegated authoritative validation.
- normal_launch_guard: added governance coverage that inspects the generated default launch branch and rejects evidence graph/audit command execution from normal interactive launch.

## T006 Synthetic Error-Handling Fixtures

- captured_at: `2026-05-28T15:40:07+02:00`
- fixture_root: `specs/027-generated-evidence-workflow/readiness/generated-validation-fixtures/`
- approval: `[SEH] synthetic-error-handling-approved`
- code_level_disclosure: fixture files include `SYNTHETIC FIXTURE` banners.
- cycle_verification: `run-audit.sh .../cycle --graph-only` exited `3` and reported `cycle: T001 -> T002 -> T001`.
- dangling_verification: `run-audit.sh .../dangling --graph-only` exited `3` and reported `T001 depends on T999`.
- missing_readiness_verification: `run-audit.sh .../missing-readiness` exited `2` and reported missing readiness files.
- skipped_authority_fixture: `skipped-authority/readiness/generated-evidence-command-report.md` records `authority: skipped` with a placeholder pass claim for generated command rejection tests.
- logs:
  - `specs/027-generated-evidence-workflow/readiness/logs/t006/cycle-graph.txt`
  - `specs/027-generated-evidence-workflow/readiness/logs/t006/dangling-graph.txt`
  - `specs/027-generated-evidence-workflow/readiness/logs/t006/missing-readiness-audit.txt`

## T013-T020 Authoritative Generated Command Evidence

- captured_at: `2026-05-28T16:00:12+02:00`
- implementation:
  - `template/base/build.fsx` `EvidenceGraph` calls `.specify/extensions/evidence/scripts/bash/run-audit.sh <feature> --graph-only`.
  - `template/base/build.fsx` `EvidenceAudit` first runs graph validation and only runs the full audit when graph validation succeeds.
  - `template/base/src/Product/EvidenceCommands.fs` contains `GeneratedEvidenceCommandReport` with command, target, generated identity, authority, status, exit code, validation area, report path, and diagnostics fields.
- pass_graph: `dotnet fsi template/base/build.fsx --target EvidenceGraph`
  - log: `specs/027-generated-evidence-workflow/readiness/logs/t013-generated-evidence-graph-pass.txt`
  - result: exit `0`; graph-only validation wrote `task-graph.json` and `task-graph.md` for the generated package.
- reject_graph: `SPECKIT_FEATURE_DIR=.../generated-validation-fixtures/cycle dotnet fsi template/base/build.fsx --target EvidenceGraph`
  - log: `specs/027-generated-evidence-workflow/readiness/logs/t013-generated-evidence-graph-cycle.txt`
  - result: generated target exited non-zero after authoritative graph validation reported `cycle: T001 -> T002 -> T001`; no pass claim was written.
- pass_audit: `dotnet fsi template/base/build.fsx --target EvidenceAudit`
  - log: `specs/027-generated-evidence-workflow/readiness/logs/t014-generated-evidence-audit-pass.txt`
  - result: exit `0`; audit output includes `verdict=PASS`, `readiness-contract: 0 blocking`, and `diff-scan-hits=0`.
- reject_audit: `SPECKIT_FEATURE_DIR=.../generated-validation-fixtures/missing-readiness dotnet fsi template/base/build.fsx --target EvidenceAudit`
  - log: `specs/027-generated-evidence-workflow/readiness/logs/t014-generated-evidence-audit-missing-readiness.txt`
  - result: generated target exited non-zero after authoritative audit validation reported `readiness contract hits: 3` and named each missing readiness file.
- normal_launch_guard:
  - log: `specs/027-generated-evidence-workflow/readiness/logs/t015-normal-launch-separation-test.txt`
  - result: governance test passed, confirming the default generated launch branch remains `Viewer.runApp viewerOptions generatedHost` and does not dispatch `EvidenceGraph` or `EvidenceAudit`.
- report_contract_tests:
  - logs:
    - `specs/027-generated-evidence-workflow/readiness/logs/t013-generated-evidence-targets-test.txt`
    - `specs/027-generated-evidence-workflow/readiness/logs/t014-generated-evidence-targets-test.txt`
    - `specs/027-generated-evidence-workflow/readiness/logs/t018-generated-evidence-report-test.txt`
  - result: generated product/governance assertions passed for delegated authority, graph-first audit sequencing, failed validation areas, and required report fields.
