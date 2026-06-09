# Contract: `Targets.fsi` delta (governance-internal)

> Governance-internal `.fsi` change in `FS.Skia.UI.Build`. **Not** product public API; no product
> surface baseline. `TargetMetadataDrift` enforces currency of the regenerated `validation.contract.yml`.

## Additive DU cases (FR-006)

```fsharp
type Target =
    | …
    | GeneratedProductCheck            // unchanged: now an umbrella
    // Feature 088 (US2, FR-006): the GeneratedProductCheck split.
    | GeneratedProductStructure        // cheap: generate + structural scan + file-list evidence
    | GeneratedConsumerValidation      // expensive: consumer restore/build/Verify
    | …
```

- Both cases MUST also be added to `allTargets` (registry order; documented position adjacent to
  `GeneratedProductCheck`) and are picked up by `dispatchTargets` automatically.
- `name`, `directPrerequisites`, `timeoutClass`, `cost`, `failureOwner` matches gain arms (exhaustive ⇒
  a missing arm is a compile error). Values per [data-model.md](../data-model.md) §`Targets.Target`.

## New routable-gate projection vals (FR-003, FR-004, SC-002)

```fsharp
/// The gates a routing rule can require, plus the composites Verify/Ci. Single source for
/// AgentValidation.knownGates. (Feature 088, FR-003.)
val routableGates: Target list

/// Verify's product-facing evidence gates (prerequisites filtered by isProductCheck), the single
/// source for Verify's ProductChecksRun. (Feature 088, FR-004.)
val productCheckGates: Target list
```

## Obligations

- `routableGates |> List.map name` set-equals the prior `knownGates` literal; renders in pinned registry
  order (test-pinned).
- `productCheckGates |> List.map name` equals the prior `ProductChecksRun` literal byte-for-byte, in
  order.
- `GeneratedProductCheck` stays resolvable; routing rules and `Verify` continue to reference it (no
  downstream reference breaks).
