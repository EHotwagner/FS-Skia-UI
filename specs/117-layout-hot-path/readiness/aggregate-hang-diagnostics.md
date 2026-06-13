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
    command: dotnet test tests/Controls.Tests/Controls.Tests.fsproj --filter "FullyQualifiedName~Feature 117"; dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj --filter "FullyQualifiedName~Feature 117"
    focused rerun result: 15 Feature 117 Controls tests + 5 Feature 117 Elmish metrics tests passed; full suites 445 Controls + 155 Elmish green; the routed controls-public-surface gate set PASS
    evidence_path: specs/117-layout-hot-path/readiness/logs/test.txt
  investigated_failure:
    command: VSTest/YoloDev adapter execution filtered to KeyboardInputGallery
    result: hung before launching the KeyboardInputGallery child process
  control_check:
    command: dotnet run --project samples/KeyboardInputGallery/KeyboardInputGallery.fsproj --no-build --no-restore -- --contract-smoke
    result: passed and printed contract smoke output
  final_classification: VSTest/YoloDev adapter orchestration concern for the smoke executable, not a sample or product failure
  diagnostic: The FAKE Test target runs the native-GUI Expecto suites (Smoke.Tests and SkiaViewer.Tests) via direct Expecto execution to bypass the VSTest/YoloDev adapter testhost (libdecor-gtk crash under a dual Wayland/X11 display); all other test projects continue to use dotnet test.
