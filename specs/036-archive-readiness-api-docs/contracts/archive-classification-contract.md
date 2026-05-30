# Contract: Archive Classification

Archive classification output is a Markdown inventory at:

`specs/036-archive-readiness-api-docs/readiness/archive-inventory.md`

## Required Sections

- `# Archive Inventory`
- `## Classification Policy`
- `## Current Evidence`
- `## Archived In Place`
- `## Replaced Or Retained`
- `## Removable Candidates`
- `## Informational Historical Findings`

## Required Row Fields

Each archived or retained row must include:

- `feature-id`
- `path`
- `classification`
- `archival-marker`
- `rationale`
- `preservation-status`
- `owner`
- `replacement-path` or `none`

## Rules

- Historical readiness files remain in place by default.
- Archived rows must not be presented as satisfying current gates.
- Synthetic-evidence disclosures, unsupported-host classifications, command
  logs, and prior audit reports must stay traceable when retained.
- Deletion is allowed only for clearly obsolete generated output with an
  inventory rationale.

## Failure Conditions

- Missing feature id for a historical artifact.
- Archived row without marker or rationale.
- Active evidence map cites an archived path as current evidence.
- Removable row lacks audit-safety rationale.
