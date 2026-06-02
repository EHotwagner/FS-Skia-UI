# Aggregate Hang Diagnostics

validation_verdict:
  target: Dev
  verdict: aggregate pass — no hang observed; the escalated gate set completed deterministically
  stage: Test aggregate
  elapsed duration: Dev passed in 1 minute 44 seconds (SkillSyncCheck + Restore + Build + SampleContractSmoke + Test)
  last observed command: dotnet test tests/Governance.Tests/Governance.Tests.fsproj
  timeout_policy: FAKE-backed targets share `.fake` state and were run strictly sequentially (never concurrently)
  recommended focused rerun: dotnet test tests/Input.Tests/Input.Tests.fsproj
  focused rerun:
    command: dotnet test tests/Input.Tests/Input.Tests.fsproj
    focused rerun result: 12 tests, 12 passed, 0 failed (Input.Tests.dll, net10.0) — the migrated rich keyboard-input suite
    evidence_path: specs/052-v3-lib-decoupling/readiness/logs/dev.log
  investigated_failure:
    command: first Dev run before the packable-fsi path-enumeration fix
    result: one failure — AsteroidsFeedbackSkillGuidanceTests + PublicRecordInvariantTests enumerated the now-moved `src/Lib/KeyboardInput.fsi` by path; corrected by repointing the packable-fsi lists to `src/Input/KeyboardInput.fsi` and refreshing the xml-documentation-validation evidence doc (the Stage-2 file-path lesson)
  control_check:
    command: ./fake.sh build -t PackageSurfaceCheck
    result: clean — the monolith aggregate baseline matches the built reflection surface after shedding the rich KeyboardInput lines (seven `Parity`-helper names remain); the new `FS.Skia.UI.Input` baseline matches
  final_classification: no aggregate hang; a single stale path-enumeration assertion, fixed and re-run green
  diagnostic: Aggregate FAKE results are recorded as a non-authoritative aggregate; the focused migrated suite and the focused PackageSurfaceCheck / PerPackageSurfaceDiff runs are authoritative.
