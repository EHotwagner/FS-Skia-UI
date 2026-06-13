# Aggregate Hang Diagnostics (feature 115)

validation_verdict:
  target: Dev
  verdict: aggregate pass; no hang observed across the routed gate runs. Any whole-suite run that hangs or is inconclusive is recorded here as a non-authoritative aggregate result, and the focused per-target sequential rerun is authoritative.
  stage: Test aggregate
  elapsed duration: Dev (after safe + adopted bumps) passed in ~4 minutes; Test target 1 minute 17 seconds
  last observed command: ./fake.sh build -t Dev
  timeout_policy: Smoke.Tests / SkiaViewer.Tests run the Expecto executable directly (bypassing the VSTest/YoloDev adapter testhost) to avoid the libdecor-gtk crash under a dual Wayland/X11 display
  recommended focused rerun: ./fake.sh build -t Dev
  focused rerun:
    command: ./fake.sh build -t Dev
    focused rerun result: passed (Restore + Build + SampleContractSmoke + Test green) on the adopted tree
    evidence_path: specs/115-dependency-updates/readiness/logs/dev-after-safe.txt
  investigated_failure:
    command: ./fake.sh build -t Dev with the Expecto/Test.Sdk/YoloDev cluster bumped (T014)
    result: Restore failed NU1608 — YoloDev.Expecto.TestSdk 1.0.0 requires Expecto >=9.0.0 && <10.0.0, conflicts with Expecto 11.0.0; this is a deterministic package-incompatibility, not a hang
  control_check:
    command: ./fake.sh build -t Dev on the adopted tree (cluster reverted)
    result: passed; the cluster revert returns to the Fable.Elmish-validated state
  final_classification: a deterministic version-bump validation pass; the one US2 cluster failure was a published-metadata package incompatibility (reverted), not a race-like or non-authoritative aggregate result
  diagnostic: FAKE-backed commands share .fake state and are never run concurrently; every gate in this feature was run sequentially. If a failure looks race-like or the concurrent FAKE context is unknown, the affected FAKE-backed commands are rerun sequentially before any product debugging. A non-authoritative aggregate result is never the merge verdict.
