# Governance.Tests results (typed-finding coverage + parity, SC-004 / FR-008)

Authoritative command:
`dotnet run --project tests/Governance.Tests/Governance.Tests.fsproj --no-build --no-restore -- --sequenced`

Result: **295 tests run, 295 passed, 0 failed, 0 errored.**

## New typed-finding suites (≥6 cases over crafted typed values, SC-004)

- `TargetMetadataTests.fs` — 6 cases over the real `TargetMetadata.validateMetadataDrift`:
  `MissingMetadata`, `MissingRunnableTarget`, `MissingExpectedOutput`, `MissingFailureOwner`,
  `DependencyDivergence`, plus a structural "derived registry has no drift against itself"
  case (SC-003).
- `CapabilityCatalogTests.fs` — 5 cases over the real `Capabilities.validateRows` (injected
  surface-baseline probe, no disk): rule ids `displayName`, `dependency`, `project`,
  `surfaceBaseline`, `default-app`.
- `ReportParityTests.fs` — 4 cases: byte-equality of `capability-catalog.md`,
  `target-metadata-drift.md`, and the `target-metadata.json` renderer round-trip vs the
  golden fixtures, plus the R2 timestamp well-formedness assertion.

Total typed-finding classes: **3 catalog error classes + 5 target-metadata drift classes =
8 ≥ 6** (SC-004 satisfied; FR-008 — the moved rules now have fast precise typed coverage the
script-trapped logic could not have).

## Re-pointed existing contract tests (T018)

The build's command/dependency-graph contract is now embodied by the typed
`FS.Skia.UI.Build.Targets` model (FR-001), so the source-text contract tests that scanned
build.fsx for the retired string-tuple registries were re-pointed at the real library values
(no weakening — strictly stronger, typed assertions):

- `GovernanceTestSupport.expectFakeTarget` → asserts membership in
  `Targets.dispatchTargets |> List.map Targets.name`.
- New `GovernanceTestSupport.dependencyRow` / `expectDependency` → assert against
  `Targets.targetDependencyRows`.
- `readBuildGovernanceSources()` now also scans `build/Governance/*.fsi` and `*.fs` (the
  moved validators + typed model), so the `AgentValidationFrameworkTests` metadata-contract
  scan finds `type TargetMetadata`, the field names, the drift messages, and
  `validateMetadataDrift` (the retired `TargetMetadataReport`/`ValidateTargetMetadataDrift`
  names were updated to their replacements).
- `CommandContractTests`, `V2CommandContractTests`, `ProcessReliabilityContractTests` —
  dependency-row needles re-pointed to `expectDependency`.

Failure class: `governance / unit-test`. Next action: run the focused Governance.Tests
executable directly (above) — it is authoritative; aggregate FAKE Test results are not.
