# Aggregate Hang Diagnostics

validation_verdict:
  target: Dev
  verdict: aggregate pass — no hang observed; the serialized escalated gate set completed deterministically
  stage: Test aggregate
  elapsed duration: Dev passed in 1 minute 46 seconds (Restore + Build + SampleContractSmoke + Test)
  last observed command: dotnet test tests/Governance.Tests/Governance.Tests.fsproj
  timeout_policy: FAKE-backed targets share `.fake` state and were run strictly sequentially (never concurrently)
  recommended focused rerun: dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter AgentValidationFrameworkTests
  focused rerun:
    command: ./fake.sh build -t Dev
    focused rerun result: 347 tests, 347 passed, 0 failed (Governance.Tests.dll, net10.0)
    evidence_path: specs/051-relocate-agentvalidation/readiness/logs/dev.log
  investigated_failure:
    command: first Dev run before the packable-fsi enumeration fix
    result: 1 failure — AsteroidsFeedbackSkillGuidanceTests enumerated the now-moved `src/Lib/AgentValidation.fsi`; corrected by dropping that path from the `FS.Skia.UI` packable list (the monolith no longer ships the module)
  control_check:
    command: ./fake.sh build -t PackageSurfaceCheck
    result: clean — the monolith surface baseline matches the built reflection surface after shedding the 47 `FS.Skia.UI.AgentValidation.*` lines
  final_classification: no aggregate hang; a single stale path-enumeration assertion, fixed and re-run green
  diagnostic: Aggregate FAKE results are recorded as a non-authoritative aggregate; the focused repointed suite and the focused PackageSurfaceCheck run are authoritative.
