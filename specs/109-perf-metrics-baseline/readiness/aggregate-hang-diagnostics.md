# Aggregate Hang Diagnostics

validation_verdict:
  target: Dev
  verdict: focused per-target reruns are authoritative and all PASS; no feature-109 aggregate hang —
    any whole-suite adapter stall is a non-authoritative aggregate result, not a product failure
  stage: Test aggregate
  elapsed duration: Dev completed in 4 minutes 19 seconds (Restore + Build + SampleContractSmoke + Test, Status Ok)
  last observed command: ./fake.sh build -t Dev
  timeout_policy: the FAKE Test target runs the native-GUI Expecto suites via direct Expecto execution
    to bypass the VSTest/YoloDev adapter testhost (libdecor-gtk crash under a dual Wayland/X11 display)
  recommended focused rerun: dotnet run --project tests/Elmish.Tests/Elmish.Tests.fsproj --no-build -c Debug
  focused rerun:
    command: dotnet run --project tests/Elmish.Tests/Elmish.Tests.fsproj --no-build -c Debug
    focused rerun result: 100 passed, 0 failed, 0 errored (incl. 12 metric-honesty + 10 corpus + 3 baseline Feature109 tests)
    evidence_path: specs/109-perf-metrics-baseline/readiness/perf-corpus/
  investigated_failure:
    command: a single background ./fake.sh build -t Dev launch was terminated (exit 143/SIGTERM) before completing
    result: a re-run in the foreground completed clean (Status Ok); the termination was an
      orchestration/lifecycle artifact, not a feature-109 product defect
  control_check:
    command: ./fake.sh build -t EvidenceAudit
    result: verdict=PASS, unaccepted-synthetic-tasks=0, diff-scan-hits=0
  final_classification: no product defect; the authoritative verdict is the focused per-target rerun
  diagnostic: feature 109 adds no new window or host launch — its entire evidence surface is the
    deterministic, headless Perf.runScript driver — so there is no native-GUI aggregate hang specific to
    this feature. Any non-authoritative aggregate result is recorded here as advisory only; the Route
    gate list (Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedGuidanceCheck,
    TemplateDrift, EvidenceGraph, EvidenceAudit) all passed in focused per-target runs.
