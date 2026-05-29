# Generated Validation Authority

Status: complete for focused guidance implementation.

Generated validation must inspect generated output, not only source templates.

Implemented checks:

- `tests/Governance.Tests/SequentialFakeGuidanceTests.fs` validates template
  source guidance for generated README, product docs, and local skills.
- `build.fsx` `GeneratedGuidanceCheck` scans repository, agent, template, and
  generated-product guidance for FAKE-backed command serialization semantics.
- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --no-restore`
  passed with 225 tests.
