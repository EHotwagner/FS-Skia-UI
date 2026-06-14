# Aggregate Hang Diagnostics (feature 122)

validation_verdict:
  target: Dev
  verdict: aggregate pass; the native-GUI Expecto suites (Smoke.Tests, SkiaViewer.Tests) run via direct Expecto execution, bypassing the VSTest/YoloDev adapter testhost
  stage: Test aggregate
  elapsed duration: Dev completed after the standard test aggregate
  last observed command: dotnet run --project tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --no-restore
  timeout_policy: SkiaViewer.Tests / Smoke.Tests bypass the VSTest/YoloDev adapter path and run the Expecto executable directly
  recommended focused rerun: dotnet run --project tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj -- --filter-test-list "122"
  focused rerun:
    command: dotnet run --project tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj -- --filter-test-list "122"
    focused rerun result: 7 Feature 122 present-path tests passed in 0.06s
    evidence_path: specs/122-spread3-consumer-feedback/readiness/logs/
  control_check:
    command: dotnet run --project tests/Controls.Tests/Controls.Tests.fsproj -- --filter-test-list "122"
    result: 3 Feature 122 CustomControl guard tests passed
  final_classification: no aggregate hang observed for this feature; any VSTest/YoloDev adapter concern on the native-GUI suites is an adapter orchestration matter, not a product failure
  diagnostic: The FAKE Test target runs the native-GUI Expecto suites via direct Expecto execution to bypass the VSTest/YoloDev adapter testhost (libdecor-gtk crash under a dual Wayland/X11 display); all other test projects continue to use dotnet test. Non-authoritative aggregate results are advisory; the authoritative verdict is the focused per-target rerun above.
