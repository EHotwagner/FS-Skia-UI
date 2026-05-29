# Contract: Agent Verdict

## Artifacts

Agent-ready validation writes:

- `readiness/agent-verdict.json`
- `readiness/agent-verdict.md`

Feature-specific planning evidence for this feature is recorded at `specs/028-agent-validation-framework/readiness/agent-ready-verdict.md`.

## JSON Shape

```json
{
  "status": "passed",
  "authority": "focused-authoritative",
  "changed_path_source": {
    "kind": "active-feature-metadata",
    "paths": ["src/Controls/Control.fsi"]
  },
  "selected_rule_ids": ["controls-public-surface"],
  "required_gates": ["ControlsCatalogCheck", "PackageSurfaceCheck", "EvidenceGraph", "EvidenceAudit"],
  "completed_gates": ["ControlsCatalogCheck", "PackageSurfaceCheck", "EvidenceGraph", "EvidenceAudit"],
  "missing_gates": [],
  "skipped_gates": [],
  "failure_owner": "product",
  "failure_class": null,
  "next_command": null,
  "artifacts": ["readiness/control-catalog.md"],
  "diagnostics": [],
  "timestamp_utc": "2026-05-28T00:00:00Z"
}
```

## Status Semantics

- `passed`: all required gates completed and required artifacts are present.
- `failed`: at least one completed gate reported a product, template, governance, prerequisite, or missing-evidence failure.
- `unsupported`: the selected proof cannot run on the current host and the reason is classified.
- `degraded`: focused authority could not be selected confidently, or a required prerequisite is stale and a broader/remediation command is needed.

## Required Failure Classes

- `environment`
- `unsupported-host`
- `stale-prerequisite`
- `product`
- `template`
- `governance`
- `missing-evidence`
- `unknown`

## Invariants

- `status=passed` requires `missing_gates=[]`.
- `status=degraded` requires `next_command`.
- `authority=focused-authoritative` requires at least one selected rule.
- `selected_rule_ids` must exist in `validation.contract.yml`.
- `required_gates` must reference registered targets with metadata.
- Markdown verdict wording must not claim stronger authority than JSON records.
