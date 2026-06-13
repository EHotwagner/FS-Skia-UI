# Aggregate Hang Diagnostics

validation_verdict:
  target: Dev
  verdict: aggregate pass after smoke orchestration isolation; previous adapter hang was a non-authoritative aggregate result
  stage: Test aggregate
  elapsed duration: Verify passed in 3 minutes 58 seconds after the smoke runner change
  last observed command: dotnet run --project tests/Smoke.Tests/Smoke.Tests.fsproj --no-restore
  timeout_policy: Smoke.Tests bypasses the VSTest/YoloDev adapter path and runs the Expecto executable directly
  recommended focused rerun: dotnet run --project tests/Smoke.Tests/Smoke.Tests.fsproj --no-restore
  focused rerun:
    command: dotnet run --project tests/Smoke.Tests/Smoke.Tests.fsproj
    focused rerun result: passed 3 smoke tests in 2.6 seconds during investigation
    evidence_path: specs/020-asteroids-integration-feedback/readiness/logs/test.txt
  investigated_failure:
    command: VSTest/YoloDev adapter execution filtered to KeyboardInputGallery
    result: hung before launching the KeyboardInputGallery child process
  control_check:
    command: dotnet run --project samples/KeyboardInputGallery/KeyboardInputGallery.fsproj --no-build --no-restore -- --contract-smoke
    result: passed and printed contract smoke output
  final_classification: VSTest/YoloDev adapter orchestration concern for the smoke executable, not a sample or product failure
  diagnostic: The FAKE Test target runs the native-GUI Expecto suites (Smoke.Tests and SkiaViewer.Tests) via direct Expecto execution to bypass the VSTest/YoloDev adapter testhost (libdecor-gtk crash under a dual Wayland/X11 display); all other test projects continue to use dotnet test.
