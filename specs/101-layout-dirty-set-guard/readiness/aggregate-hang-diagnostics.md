# Aggregate Hang Diagnostics — feature 101 (R7, T016)

validation_verdict:
  target: Dev
  verdict: aggregate pass; no hang observed. The Dev aggregate is recorded as a non-authoritative aggregate result and the per-suite Expecto outcomes are the authoritative evidence.
  stage: Test aggregate
  elapsed duration: Dev passed in 3 minutes 31 seconds (Restore 44s, Build 1m32s, SampleContractSmoke 7s, Test 1m06s), exit code 0
  last observed command: ./fake.sh build -t Dev
  timeout_policy: FAKE-backed targets share .fake state and are run sequentially, never concurrently; a race-like or hung aggregate is re-run sequentially in isolation before any product-regression claim
  recommended focused rerun: dotnet run --project tests/Controls.Tests -c Debug -- --filter-test-list "Feature101"
  focused rerun:
    command: dotnet run --project tests/Controls.Tests -c Debug -- --filter-test-list "Feature101"
    focused rerun result: Feature101 layout dirty-set anti-drift guard (R7) — 12 passed, 0 failed, 0 errored in 0.29s
    evidence_path: specs/101-layout-dirty-set-guard/readiness/validation-log.md
  final_classification: no hang; the new guard + the unchanged R2 suites pass under the Dev Test aggregate, and the full Route-printed controls-public-surface gate set passed sequentially. The non-authoritative aggregate verdict is confirmed by the focused per-suite reruns.
  diagnostic: R7 adds only in-process, deterministic Expecto tests (no window, GPU, wall-clock, or external process), so no GUI/adapter hang class applies; the native-GUI Smoke/SkiaViewer suites are unaffected by this framework-internal change.
