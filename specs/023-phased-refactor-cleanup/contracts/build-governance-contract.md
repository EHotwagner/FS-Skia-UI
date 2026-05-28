# Contract: Build Governance Decomposition

## Public Build Surface

`build.fsx` remains the public FAKE entrypoint for target registration,
dependency wiring, and final command orchestration.

The following target names and user-facing semantics remain stable:

- `Dev`
- `Verify`
- `Ci`
- `PackLocal`
- `DependencyReport`
- `TemplateCheck`
- `GeneratedGuidanceCheck`
- `TemplateDrift`
- `EvidenceGraph`
- `EvidenceAudit`

## Internal Extraction

Helpers may move to loaded scripts under `scripts/build/` when they do not
require direct FAKE target declarations. Candidate responsibilities include
paths, process execution, reports, template validation, generated scanning,
package resolution, and process-health policy.

## Validation

For every moved helper family, run the focused FAKE target that owns that
behavior. If target wiring or readiness path discovery changes, run `Dev`,
`Verify`, and evidence graph/audit checks before accepting the phase.

## Failure Conditions

- A target name changes.
- A target dependency changes user-visible behavior.
- A readiness file moves without an approved contract change.
- Failure wording loses the actionable path, command, or missing-artifact class.
