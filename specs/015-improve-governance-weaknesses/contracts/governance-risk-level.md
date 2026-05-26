# Contract: Governance Risk Level

## Record Shape

```yaml
governance_risk:
  level: medium
  scope_signals:
    - evidence graph parser changed
    - generated guidance unchanged
  required_checks:
    - ./fake.sh build -t EvidenceGraph
    - ./fake.sh build -t EvidenceAudit
  broad_required: false
  rationale: "Task-readiness validation changed, but no runtime product or package surface changed."
  non_authoritative_results: []
```

## Levels

- `small`: documentation, checklist, or metadata-only work with no generated output, command orchestration, runtime behavior, public contract, or package impact.
- `medium`: focused governance, validation, guidance, or script changes with bounded command targets and no runtime product/package/API change.
- `broad`: product runtime behavior, generated product output, package identity/content, public contracts, build orchestration, or cross-target validation changes.

## Required Behavior

- Reports must name the minimum required checks for the selected level.
- Reports must explain why broad aggregate validation is or is not required.
- Final readiness fails when the selected evidence path is incomplete for the declared level.
- Broad aggregate failures or hangs may be classified as non-authoritative only with focused rerun evidence and an explicit verdict.
