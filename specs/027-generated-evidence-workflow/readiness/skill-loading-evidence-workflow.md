# Skill Loading Evidence Workflow

Skill-loading evidence is now generated and validated as one required row per
task/skill pairing. The graph script writes a row template and rejects malformed
rows without allowing duplicate or prose rows to mask missing required pairings.

## T007 Failing-First Tests

- captured_at: `2026-05-28T15:42:01+02:00`
- command: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj -- --filter "skill-loading evidence workflow"`
- log_path: `specs/027-generated-evidence-workflow/readiness/logs/t007-skill-loading-tests.txt`
- expected_result: failing-first test failure before implementation.
- observed_failures:
  - missing `expected_skill_loading_rows` helper/output in `compute-task-graph.py`
  - missing duplicate/collapsed/equal-timestamp diagnostic contract strings in `compute-task-graph.py`
- note: The Expecto filter did not narrow to only these tests; the same run also reports the already-recorded T005 red test.

## T008 Synthetic Malformed Fixtures

- captured_at: `2026-05-28T15:43:54+02:00`
- fixture_root: `specs/027-generated-evidence-workflow/readiness/skill-loading-fixtures/`
- approval: `[SEH] synthetic-error-handling-approved`
- code_level_disclosure: fixture files include `SYNTHETIC FIXTURE` banners.
- malformed_classes: collapsed task range, multi-skill prose row, duplicate rows, late timestamp, equal timestamp.
- verification_logs:
  - `specs/027-generated-evidence-workflow/readiness/logs/t008/collapsed-range-graph.txt`
  - `specs/027-generated-evidence-workflow/readiness/logs/t008/multi-skill-prose-graph.txt`
  - `specs/027-generated-evidence-workflow/readiness/logs/t008/duplicate-graph.txt`
  - `specs/027-generated-evidence-workflow/readiness/logs/t008/late-graph.txt`
  - `specs/027-generated-evidence-workflow/readiness/logs/t008/equal-graph.txt`
- current_validator_behavior: superseded by T023-T026; exact collapsed,
  duplicate, multi-skill prose, late timestamp, and equal timestamp diagnostics
  are now emitted by the graph validator.

## T021-T026 Row Generation And Validation Evidence

- captured_at: `2026-05-28T16:03:58+02:00`
- implementation:
  - `.specify/extensions/evidence/scripts/python/compute-task-graph.py` exposes `expected_skill_loading_rows`, `missing_skill_loading_rows`, and `generate_skill_loading_evidence_template`.
  - Graph runs write `readiness/skill-loading-evidence.template.md` with one row per task and skill pairing.
  - Validation rejects collapsed task ranges, multi-task/prose skill rows, duplicate rows, missing rows, unreadable paths, late rows, and equal timestamps.
- real_generation:
  - command: `.specify/extensions/evidence/scripts/python/compute-task-graph.py specs/027-generated-evidence-workflow`
  - log: `specs/027-generated-evidence-workflow/readiness/logs/t023-graph-template.txt`
  - generated_template: `specs/027-generated-evidence-workflow/readiness/skill-loading-evidence.template.md`
- governance_tests:
  - command: `dotnet run --project tests/Governance.Tests/Governance.Tests.fsproj -- --filter-test-list "V3 local skill validation" --sequenced --summary`
  - log: `specs/027-generated-evidence-workflow/readiness/logs/t021-t022-skill-validation-tests.txt`
  - result: 7 passed, including real row derivation and `Synthetic` malformed-row diagnostics tests.
- malformed_rejection_logs:
  - `specs/027-generated-evidence-workflow/readiness/logs/t022/collapsed-range-graph.txt`
  - `specs/027-generated-evidence-workflow/readiness/logs/t022/multi-skill-prose-graph.txt`
  - `specs/027-generated-evidence-workflow/readiness/logs/t022/duplicate-graph.txt`
  - `specs/027-generated-evidence-workflow/readiness/logs/t022/late-graph.txt`
  - `specs/027-generated-evidence-workflow/readiness/logs/t022/equal-graph.txt`
- diagnostics_observed:
  - `collapsed task range row is invalid`
  - `multi-skill prose row is invalid`
  - `duplicate skill-loading evidence row`
  - `loaded_at must be earlier than work_started_at`
  - `equal timestamps are invalid`
