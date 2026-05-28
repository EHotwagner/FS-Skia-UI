# Framework Guidance

Generated framework guidance now qualifies app-owned messages, makes domain
vector to scene point conversion explicit, and separates semantic scene facts
from screenshot proof.

## T010 Failing-First Guidance Tests

- captured_at: `2026-05-28T15:46:17+02:00`
- command: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj -- --filter "generated game guidance"`
- log_path: `specs/027-generated-evidence-workflow/readiness/logs/t010-generated-guidance-tests.txt`
- expected_result: failing-first test failure before implementation.
- observed_failures:
  - missing `Product.Program.Msg.CloseRequested` / app-owned message qualification guidance.
  - missing `toScenePoint`, domain vector, `Scene.Point`, and explicit conversion guidance.
  - missing semantic scene facts guidance for lander, terrain, landing pad, and HUD metrics.
- note: The Expecto filter did not narrow to only these tests; the run also includes previously recorded red tests.

## T031-T035 Guidance Proof

- captured_at: `2026-05-28T16:12:40+02:00`
- command: `dotnet run --project tests/Governance.Tests/Governance.Tests.fsproj -- --filter-test-list "Generated guidance hardening" --sequenced --summary`
- log_path: `specs/027-generated-evidence-workflow/readiness/logs/t031-t034-generated-guidance-tests.txt`
- result: 22 passed, 0 failed.
- updated_paths:
  - `template/base/docs/product.md`
  - `template/fragments/scene/README.md`
  - `template/fragments/testing/README.md`
  - `docs/generated-apps.md`
  - `docs/evidence.md`
- enforced_terms:
  - `Product.Program.Msg.CloseRequested`
  - `app-owned message`
  - `toScenePoint`
  - `domain vector`
  - `Scene.Point`
  - `explicit conversion`
