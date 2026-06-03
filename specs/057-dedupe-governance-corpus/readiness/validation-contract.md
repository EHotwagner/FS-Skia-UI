# Validation-Contract Currency (FR-008, C6)

`validation.contract.yml` is generated from `build/Governance/Routing.fs` and
currency-checked by `TargetMetadataDrift`. Feature 057 does **not** edit
`Routing.fs` (the rule *set* / routing is unchanged; only governed-prose carriage
changed), so `validation.contract.yml` MUST stay byte-identical.

## Confirmation (T013)

- **authoritative command**: `./fake.sh build -t TargetMetadataDrift` → `Status: Ok`
- **`Routing.fs` byte-status**: unmodified (`git status --short build/Governance/Routing.fs` → no diff)
- **`validation.contract.yml` byte-status**: unmodified (`git status --short validation.contract.yml` → no diff)
- **failure class**: validation-yml-drift (not triggered)

Verified 2026-06-03: `git status --short` reports no diff for either
`build/Governance/Routing.fs` or `validation.contract.yml`; `TargetMetadataDrift`
is green. The contract is current and byte-unchanged, satisfying FR-008 / C6 for
the "Routing.fs unedited" branch.
