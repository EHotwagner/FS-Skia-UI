# Audit Diagnostics

Audit diagnostics now name the exact readiness file status and missing terms in
console output and persisted JSON artifacts.

## T009 Synthetic Diagnostic Fixtures

- captured_at: `2026-05-28T15:45:01+02:00`
- fixture_root: `specs/027-generated-evidence-workflow/readiness/audit-diagnostics-fixtures/`
- approval: `[SEH] synthetic-error-handling-approved`
- code_level_disclosure: fixture files include `SYNTHETIC FIXTURE` banners.
- missing_files_log: `specs/027-generated-evidence-workflow/readiness/logs/t009/missing-files-audit.txt`
- incomplete_terms_log: `specs/027-generated-evidence-workflow/readiness/logs/t009/incomplete-terms-audit.txt`
- observed_missing_file_diagnostics:
  - `missing governance-risk-levels.md`
  - `missing aggregate-hang-diagnostics.md`
  - `missing runtime-limitations.md`
- observed_incomplete_diagnostics:
  - `governance risk level evidence is incomplete`
  - `aggregate timeout verdict evidence is incomplete`
  - `runtime limitation evidence is incomplete`

## T027-T030 Diagnostic Evidence

- captured_at: `2026-05-28T16:07:00+02:00`
- implementation: `.specify/extensions/evidence/scripts/bash/run-audit.sh` readiness contract scan writes `status`, `reason`, `missing_terms`, `missing_sections`, `blocking`, and `validation_area` fields for each hit.
- missing_file_log: `specs/027-generated-evidence-workflow/readiness/logs/t027/missing-files-audit.txt`
  - observed: `status=missing`
  - observed: `missing-terms=small,medium,broad,required evidence,broad validation`
  - observed: `validation-area=readiness-contract`
- missing_term_log: `specs/027-generated-evidence-workflow/readiness/logs/t027/incomplete-terms-audit.txt`
  - observed: `status=incomplete`
  - observed: `missing-terms=stage,elapsed duration,last observed command,focused rerun,non-authoritative aggregate`
  - observed: `validation-area=readiness-contract`
- generated_audit_distinction: `template/base/build.fsx` maps generated audit failures to distinct validation areas: `task-graph`, `readiness-contract`, `diff-scan`, `synthetic-evidence`, and `unsupported-host-classification`.
- passing_log: `specs/027-generated-evidence-workflow/readiness/logs/t030-current-feature-audit.txt`
  - observed: `readiness-contract: 0 blocking`
  - observed: `verdict=PASS`
