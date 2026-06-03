# Validation-Contract Currency (FR-008, C6)

`validation.contract.yml` is generated from `build/Governance/Routing.fs` and
currency-checked by `TargetMetadataDrift`. Feature 058 **does** edit `Routing.fs`
— it adds the new `skill-quality` routing rule (FR-001/FR-005) — so the contract
was regenerated (via `RefreshSurfaceBaselines` → `ContractView.render`) and MUST
reflect that rule.

## Confirmation

- **authoritative command**: `./fake.sh build -t TargetMetadataDrift` → `Status: Ok` (2026-06-03)
- **`Routing.fs` byte-status**: committed/current (`git status --short build/Governance/Routing.fs` → no uncommitted diff)
- **`validation.contract.yml` byte-status**: committed/current (`git status --short validation.contract.yml` → no uncommitted diff)
- **new rule rendered**: the generated contract carries
  ```
  - id: skill-quality
    required_gates:
      - SkillQualityCheck
      - SkillSyncCheck
    required_evidence:
      - readiness/skill-quality-check.md
  ```
- **failure class**: validation-yml-drift (not triggered)

## Contract-validator allowlist fix (2026-06-03)

The empirical maintainer-verify run surfaced that the new `skill-quality` rule
referenced `SkillSyncCheck` in `routing_rules.required_gates` for the first time,
but the contract validator's `AgentValidation.knownGates` allowlist had only been
extended with `SkillQualityCheck`. `Governance.Tests` (US1 validation routing) was
therefore failing the regenerated contract with `unknown gate SkillSyncCheck`.
Fix: `SkillSyncCheck` was added to `knownGates` (`build/Governance/AgentValidation.fs`).
After the fix the contract parses with **zero** governance diagnostics,
`Governance.Tests` is 391/391 green, and `TargetMetadataDrift` is `Status: Ok` —
the contract is current and the validator accepts every gate it names.
