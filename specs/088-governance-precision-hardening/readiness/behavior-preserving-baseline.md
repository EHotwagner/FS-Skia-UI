# Behavior-Preserving Baseline — Tier 1 & Tier 3 (Feature 088)

## Tier 1 (byte-identical)
- `AgentValidation.knownGates` is now DERIVED from `Targets.routableGates |> List.map name`;
  set-equal to the prior 30-entry literal (Feature 088 test `routableGates derive the prior
  knownGates set`).
- `Verify.ProductChecksRun` is DERIVED from `Targets.productCheckGates |> List.map name`;
  byte-for-byte and in order equal to the prior 12-entry literal (Feature 088 test).
- `focusedGateContract` re-keyed by `Targets.Target`, exhaustive, wildcard-free. Non-routable/
  internal targets resolve through `internalTargetContract`, reproducing the exact former
  wildcard value (`VerificationDegraded`, no readiness) ⇒ their `target-metadata.json` rows are
  byte-identical. Routable gates that previously fell through the wildcard now resolve to a
  non-degraded contract (the SC-003 fix); their Authority flips degraded→authoritative (the
  intended correction, not a regression).
- `TargetMetadataDrift` PASS; `validation.contract.yml` unaffected by Tier 1 (it derives from
  `Routing.fs`, not the focused-gate contract).

## Tier 3 (byte-identical findings)
- The paired NuGet.config templates are rendered from one source (`renderNuGetConfig`); the
  public-feed-only consumer config is byte-identical to the prior literal (Feature 088 test).
- `missingRequiredFiles` is the shared required-file validator both generated-row scans use;
  it is the same `List.filter (not << isPresent)` the call sites inlined, so scan findings are
  byte-identical by construction. No `.fsi` / `validation.contract.yml` change for Tier 3.
