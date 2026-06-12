# Aggregate Hang Diagnostics

validation_verdict:
  target: Dev
  verdict: focused per-target reruns are authoritative and all PASS; no feature-110 aggregate hang — any whole-suite adapter stall is a non-authoritative aggregate result, not a product failure
  stage: Test aggregate
  elapsed duration: Dev completed in approximately 4-5 minutes (Restore + Build + SampleContractSmoke + Test, Status Ok)
  last observed command: ./fake.sh build -t Dev
  timeout_policy: the FAKE Test target runs the native-GUI Expecto suites via direct Expecto execution to bypass the VSTest/YoloDev adapter testhost (libdecor-gtk crash under a dual Wayland/X11 display)
  recommended focused rerun: dotnet run --project tests/Elmish.Tests/Elmish.Tests.fsproj --no-build -c Debug
  focused rerun:
    command: dotnet run --project tests/Elmish.Tests/Elmish.Tests.fsproj --no-build -c Debug
    focused rerun result: 116 passed, 0 failed, 0 errored (incl. the 3 new Feature110 suites — retained routing, parity-vs-oracle, and the counted fallback — plus the updated Feature108/109 metrics tests)
    evidence_path: specs/110-retained-pointer-routing/readiness/routing-fullrender-delta.md
  investigated_failure:
    command: not triggered — no feature-110 background Dev termination was observed
    result: no product defect; the focused per-target rerun is the authoritative verdict
  control_check:
    command: ./fake.sh build -t EvidenceAudit
    result: verdict=PASS, unaccepted-synthetic-tasks=0, diff-scan-hits=0
  final_classification: no product defect; the authoritative verdict is the focused per-target rerun
  diagnostic: feature 110 adds no new window or host launch — its entire evidence surface is the deterministic, headless Perf.runScript driver plus the internal retained-route seams reached via InternalsVisibleTo — so there is no native-GUI aggregate hang specific to this feature; any non-authoritative aggregate result is recorded here as advisory only while every Route gate passed in focused per-target runs.
