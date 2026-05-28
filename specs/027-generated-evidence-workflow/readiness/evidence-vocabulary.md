# Evidence Vocabulary

Generated evidence vocabulary now distinguishes deterministic semantic facts,
pixel-readback fallback, and live screenshot proof.

## T010 Failing-First Vocabulary Tests

- captured_at: `2026-05-28T15:46:17+02:00`
- command: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj -- --filter "generated game guidance"`
- log_path: `specs/027-generated-evidence-workflow/readiness/logs/t010-generated-guidance-tests.txt`
- expected_result: failing-first test failure before implementation.
- observed_failure: generated guidance does not yet include the full pixel-readback fallback, `fallback-reason`, and `proves-screenshot=false` vocabulary across required generated guidance paths.

## T031-T035 Vocabulary Proof

- captured_at: `2026-05-28T16:12:40+02:00`
- command: `dotnet run --project tests/Governance.Tests/Governance.Tests.fsproj -- --filter-test-list "Generated guidance hardening" --sequenced --summary`
- log_path: `specs/027-generated-evidence-workflow/readiness/logs/t031-t034-generated-guidance-tests.txt`
- result: 22 passed, 0 failed.
- enforced_terms:
  - `semantic scene facts`
  - `deterministic-scene-evidence`
  - `does not prove semantic object presence`
  - `lander`
  - `terrain`
  - `landing pad`
  - `HUD metrics`
  - `pixel-readback fallback`
  - `fallback-reason`
  - `proves-screenshot=false`
