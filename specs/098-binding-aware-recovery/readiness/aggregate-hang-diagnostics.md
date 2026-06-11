# Aggregate Hang Diagnostics (feature 098, R3)

validation_verdict:
  target: Dev
  verdict: aggregate pass; no hang observed. Any aggregate FAKE result is treated as a
    non-authoritative aggregate (a "non-authoritative aggregate" result) unless re-confirmed by a
    sequential single-target rerun (shared `.fake` state).
  stage: Test aggregate
  elapsed duration: Dev passed in ~2 minutes 55 seconds (Build + all test projects)
  last observed command: dotnet test tests/Controls.Tests/Controls.Tests.fsproj
  timeout_policy: the FAKE Test target runs the native-GUI Expecto suites (Smoke.Tests, SkiaViewer.Tests)
    via direct Expecto execution to bypass the VSTest/YoloDev adapter testhost (libdecor-gtk crash under a
    dual Wayland/X11 display); all other test projects use dotnet test.
  recommended focused rerun: dotnet test tests/Controls.Tests/Controls.Tests.fsproj
  focused rerun:
    command: dotnet test tests/Controls.Tests/Controls.Tests.fsproj
    focused rerun result: passed 282/282 in ~36 seconds
    evidence_path: specs/098-binding-aware-recovery/readiness/logs/test.txt
  control_check:
    command: dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj
    result: passed 55/55 (routing-seam dispatch suite Feature098DispatchTests green)
  final_classification: no hang; product-and-metric change only. The libdecor-gtk plugin load warning is
    the known benign dual-display host warning, not a product defect.
  diagnostic: R3 is a pure id/recovery correction; there is no new I/O or native path that could hang. FAKE
    targets are run sequentially when more than one is needed; any aggregate result is a
    non-authoritative aggregate until re-confirmed sequentially.
