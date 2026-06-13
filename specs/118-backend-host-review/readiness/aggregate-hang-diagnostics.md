# Aggregate Hang Diagnostics (feature 118)

validation_verdict:
  target: Dev
  verdict: focused per-target gates are authoritative and PASS; any full-solution aggregate build/test is a non-authoritative aggregate result recorded as advisory only
  stage: Test aggregate
  elapsed duration: Dev (Restore+Build+SampleContractSmoke+Test) passed in 3 minutes 49 seconds; Test stage 1 minute 38 seconds
  last observed command: ./fake.sh build -t Dev
  timeout_policy: native-GUI Expecto suites (SkiaViewer.Tests, Smoke.Tests) run via direct Expecto execution to bypass the VSTest/YoloDev adapter testhost (libdecor-gtk init failure under a dual Wayland/X11 display); all other test projects use dotnet test
  recommended focused rerun: dotnet run --project tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj -c Debug --no-build -- --filter-test-list "Feature 118"
  focused rerun:
    command: dotnet run --project tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj -c Debug --no-build -- --filter-test-list "Feature 118"
    focused rerun result: 5 Feature 118 present-mode tests passed; full SkiaViewer suite 67 passed / 0 failed / 0 errored
    evidence_path: specs/118-backend-host-review/readiness/logs
  investigated_failure:
    command: (none — no hang observed during this feature's Dev run)
    result: not applicable; the libdecor-gtk plugin load warning is benign (dual Wayland/X11 display) and does not fail tests
  control_check:
    command: FEATURE118_MODE=direct dotnet specs/118-backend-host-review/readiness/live-host/bin/Debug/net10.0/LiveHost.dll
    result: passed — persistent window opened, presented 40 frames, captured screenshot, self-closed (RESULT: ok)
  final_classification: no aggregate hang; focused gates authoritative and green
  diagnostic: A non-authoritative aggregate (full-solution build/test) is advisory only; the routed focused gate set Route prints is authoritative. A race-like or unknown concurrent FAKE failure is rerun sequentially (shared .fake state) before any product-regression classification. GeneratedProductCheck's pre-merge pin-lag failure is non-authoritative (resolved by the speckit-merge version bump; see generated-validation.md).
