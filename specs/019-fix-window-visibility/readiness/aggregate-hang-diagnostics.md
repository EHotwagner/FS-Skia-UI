# Aggregate Hang Diagnostics

verdict=non-authoritative aggregate
stage=Test/Smoke.Tests
elapsed duration=greater than 5 minutes
last observed command=./fake.sh build -t Verify
focused rerun=PackLocal, TemplateCheck, GeneratedProductCheck, DependencyReport, GeneratedGuidanceCheck, EvidenceGraph, EvidenceAudit
failure-class=aggregate-hang

## T056 Verify Attempt

`./fake.sh build -t Verify` was run for broad Tier 1 validation.

First attempt:

- Log: `specs/019-fix-window-visibility/readiness/logs/t056-verify.txt`
- Stage: `VerifyPreflight`
- Result: failed before aggregate work because readiness impact files were not present yet.

Second attempt after adding readiness impact files:

- Log: `specs/019-fix-window-visibility/readiness/logs/t056-verify-rerun.txt`
- Stage: `Test/Elmish.Tests`
- Result: failed because `tests/Elmish.Tests` had not been updated for the `ApplyWindowOptions` viewer effect emitted by `SkiaViewer.Viewer.init`.

Third attempt after updating the Elmish adapter test:

- Log: `specs/019-fix-window-visibility/readiness/logs/t056-verify-after-elmish-fix.txt`
- Stage: `Test/Smoke.Tests`
- Result: non-authoritative aggregate hang. The log showed prior suites passing through Lib, Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Controls, Testing, and Parity, then no progress after starting `dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj -m:1`.
- Disposition: stopped after more than five minutes so implementation could continue with explicit aggregate diagnostics instead of leaving a live hung process.

The broad `Verify` result is therefore not authoritative for final readiness.

Missing files from the first failed preflight:

- `readiness/public-surface.md`
- `readiness/package-boundary.md`
- `readiness/generated-product-usage.md`
- `readiness/compatibility-impact.md`

## Focused Rerun Evidence

Focused reruns already captured the changed runtime and generated surfaces:

- `PackLocal`: passed after the presenter bridge; see `readiness/logs/t047-retry-pack-local-presenter-tests.txt`.
- `TemplateCheck`: passed; see `readiness/logs/t047-retry-template-check-presenter-tests.txt`.
- `GeneratedProductCheck`: passed with supported-host visible/interactable window evidence; see `readiness/logs/t047-retry-generated-product-check-presenter-tests.txt`.
- `DependencyReport`: passed; see `readiness/logs/t048-dependency-report-after-docs.txt`.
- `GeneratedGuidanceCheck`: passed; see `readiness/logs/t055-generated-guidance-check.txt`.
- `EvidenceGraph`: passed; see `readiness/logs/t054-evidence-graph-target.txt`.
- `EvidenceAudit`: ran and failed with blocking diagnostics; see `readiness/logs/t054-evidence-audit-target.txt`.

This record is non-authoritative aggregate evidence. A focused pass does not replace a green broad `Verify` result.
