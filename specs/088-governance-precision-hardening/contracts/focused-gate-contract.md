# Contract: typed `focusedGateContract` (FR-001, FR-002, FR-005, SC-001, SC-003)

## Signature change

```fsharp
// before:  focusedGateContract : BuildModel -> string        -> FocusedGateContract  (had a `_ -> degraded` wildcard)
// after:   focusedGateContract : BuildModel -> Targets.Target -> FocusedGateContract  (exhaustive, no wildcard)
```

`focusedGateSummary` and `focusedGateAssumptionCheck` change parameter type to `Targets.Target`
accordingly. `targetMetadata` passes `spec.Target` (not `spec.Name`).

## Exhaustiveness obligation (SC-001)

The `match target with …` over `Targets.Target` MUST have **no `_` wildcard**. Every case is classified:

- **Routable gates** → explicit arm with `VerdictCategory = VerificationSuccess` and a real
  `ReadinessPath` where one exists. Includes new explicit arms for gates that previously fell through the
  wildcard: `ContrastCheck`, `ControlFidelityCheck`, `PerPackageSurfaceDiff`, `SkillContractPathCheck`,
  `DesignTokenDrift`, `ControlsCatalogGenerationCheck`, `ControlsCatalogDocsCheck`, `SkillQualityCheck`,
  `PhaseHookParityCheck`, `TemplateUpdateSkillPackageCheck`, `SymbolCrossCheck`, `TargetMetadataDrift`,
  `PrePublishCheck`, `Publish`, and the new `GeneratedProductStructure` / `GeneratedConsumerValidation`.
- **Non-routable / internal targets** → `internalTargetContract target`, a named helper reproducing the
  former wildcard value exactly (`VerificationDegraded`, `ReadinessPath = None`).

⇒ Adding a future `Target` case without classifying it **fails to compile** (SC-001), not a silent
degrade.

## Verdict obligation (SC-003)

No routable gate resolves to `VerificationDegraded`. A failing-first test enumerates `routableGates` and
asserts `(focusedGateContract model g).VerdictCategory <> VerificationDegraded` for every `g`.

## Byte-identity obligation (Tier 1)

For every non-routable/internal target, `targetMetadata model target` is byte-identical to the
pre-change `target-metadata.json` (the named helper reproduces the wildcard value).
