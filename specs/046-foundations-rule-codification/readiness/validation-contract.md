# Validation contract (T003, T007)

## Gates added / changed by this feature (no new top-level FAKE target — A5)

| Rule | Host gate | Authoritative command | Artifact | Failure class | Next action |
|------|-----------|-----------------------|----------|---------------|-------------|
| Constitution-Check completeness (FR-001/002/003) | `GeneratedGuidanceCheck` (`Guidance.runGeneratedGuidanceScan` → `validateConstitutionCheck`) | `./fake.sh build -t GeneratedGuidanceCheck` | `readiness/logs/generated-guidance-check.log`, `readiness/seeded-violations/constitution-check.md` | `constitution-check:<areaId>` finding naming the area + plan path; or `unrecognized-template-revision` | fill the named area (or N/A-with-rationale); re-run the gate |
| Versioned generated-product contract (FR-004/005/006, SC-011) | `GeneratedProductCheck` (`GeneratedProduct.runScanV3GeneratedProducts` consults `GeneratedProductContract`) | `./fake.sh build -t GeneratedProductCheck` | `readiness/logs/generated-product-check.log` (schema_version header), `readiness/unit-tests.md` | structural-rule violation routed through `classifyViolation` (Required→fail, Deprecated→warn, window-closed→fail); contract-self-inconsistency failure | fix the structural violation; bump `schema_version` + add a changelog entry for a breaking rule change |

## Build-tooling `.fsi` surface handling (T007)

The new/changed `.fsi` are **build-tooling scope** (`FS.Skia.UI.Build`, `net10.0`) — **not**
tracked product surface baselines:

- `build/Governance/Guidance.fsi` — adds `RequiredDecisionArea` / `AreaStatus` /
  `ConstitutionCheckResult`, `requiredDecisionAreas`, `classifyConstitutionCheck`,
  `constitutionCheckFindings`.
- `build/Governance/GeneratedProductContract.fsi` — NEW: `ContractSchemaVersion`,
  `RuleLifecycle`, `StructuralRule`, `ContractChangeKind`, `ContractChangelogEntry`,
  `GeneratedProductContract`, `RuleOutcome`, `current`, `classifyViolation`,
  `renderContractHeader`, `changelogConsistencyFindings`.
- `build/Governance/GeneratedProduct.fsi` — unchanged surface; only the body consults the
  contract.

`PackageSurfaceCheck` / `FsiTranscripts` show **no product baseline diff** (intentional,
Principle II). The contract header renders an explicit `schema_version` (FSI transcript:
`readiness/fsi-session.txt`). The unrecognized-template-revision path emits a distinct
actionable diagnostic rather than a false pass (FR-003).
