# Verification Notes

## Governance Risk Levels

- small: documentation-only readiness report refresh with no build target or package output change; focused validation is the affected report generator plus the matching Expecto tests.
- medium: generated guidance or template documentation change; focused validation is `dotnet test tests/Governance.Tests/Governance.Tests.fsproj`, `./fake.sh build -t GeneratedGuidanceCheck`, and `./fake.sh build -t TemplateDrift` when template files changed.
- broad: package contents, template manifest, public `.fsi`, generated product code, or build target behavior changes; broad validation requires the sequential FAKE order in the feature plan.

Broad validation is required only if the implementation changes package contents, generated template contents, public contracts, or command behavior. This feature keeps runtime rendering, Vulkan behavior, and package public surface out of scope.

## Aggregate Hang Diagnostics

Aggregate `Dev` or `Verify` output is non-authoritative when it hangs, times out, or multiplexes unrelated failures. Record target, verdict category, elapsed duration, last observed command, recommended focused rerun, focused rerun result, and next action. Prefer focused reruns before product debugging.

## Runtime Limitations

This feature does not expand .NET 10 desktop, Vulkan, SkiaSharp preview, Linux display/session, macOS, mobile, browser, screenshot, or software-renderer support. Runtime limitations remain supporting context only for this documentation/governance feature.

## Command Ordering

FAKE-backed commands share `.fake` state and are not safe to run concurrently. Run them sequentially:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateDrift`
4. `./fake.sh build -t EvidenceGraph`
5. `./fake.sh build -t EvidenceAudit`

Safe non-FAKE file reads, `rg`, and direct `dotnet test` commands that do not invoke FAKE may run independently.
