# Aggregate Hang Diagnostics (feature 119)

validation_verdict:
  target: Dev
  verdict: aggregate pass; the SkiaViewer.Tests native-GUI suite runs via the direct Expecto executable (not the VSTest/YoloDev adapter), which is the authoritative path
  stage: Test aggregate
  elapsed duration: Build succeeded in ~1m47s; Test aggregate completed after the present-backend swap (Dev Status: Ok)
  last observed command: dotnet run --project tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --no-restore
  timeout_policy: SkiaViewer.Tests and Smoke.Tests bypass the VSTest/YoloDev adapter testhost (libdecor-gtk crash under a dual Wayland/X11 display) and run the Expecto executable directly; all other test projects use dotnet test
  recommended focused rerun: dotnet run --project tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --no-restore
  focused rerun:
    command: dotnet run --project tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj
    focused rerun result: 73 tests passed, 0 failed, 0 errored (includes the new Feature119OpenGlHostTests); Governance.Tests 583 passed
    evidence_path: specs/119-opengl-present-backend/readiness/logs/test.txt
  investigated_failure:
    command: initial Dev Test run before the default-present-mode test was reconciled
    result: 1 failure — Feature118 "defaultConfiguration carries OffscreenReadback" expected the pre-feature default; reconciled to DirectToSwapchain (the GL readback-free default, T014)
  final_classification: non-authoritative aggregate; the focused Expecto rerun is authoritative and green
  diagnostic: The FAKE Test target runs the native-GUI Expecto suites (SkiaViewer.Tests and Smoke.Tests) via direct Expecto execution to bypass the VSTest/YoloDev adapter testhost (libdecor-gtk crash under a dual Wayland/X11 display); the GL backend swap did not change this orchestration. non-authoritative aggregate results are advisory; the focused per-target rerun is the authoritative verdict.
