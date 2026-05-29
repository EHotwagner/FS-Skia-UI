# Governance Validation Notes

Recorded: 2026-05-28T17:17:57+02:00

## Risk Levels

- Small risk: docs-only or metadata-only routing edits. Focused validation is
  selected rule coverage plus `EvidenceGraph`.
- Medium risk: controls, generated template, validation contract, generated
  evidence workflow, or readiness policy edits. Focused validation is selected
  rule gates plus `EvidenceGraph` and `EvidenceAudit`.
- Broad risk: native target migration, command aggregation, package-surface
  changes, public `.fsi` changes, or multi-rule ambiguity. Run `Verify` and
  record aggregate output as non-authoritative unless the agent verdict
  identifies completed authoritative gates.

## Focused Validation

- `AgentReady` is the preferred focused handoff path once implemented.
- Before `AgentReady` exists, use the task-specific governed targets named in
  `tasks.md` and retain `EvidenceGraph` after every status change.
- `EvidenceAudit` is required for medium/broad evidence changes and final
  readiness.

## Aggregate Results

- `Verify` and `Ci` are broad validation signals.
- Broad aggregate success is not by itself an authoritative focused verdict.
- Broad aggregate failure must be classified by the selected gate verdict or
  the governed diagnostics that identify product, template, governance,
  environment, unsupported-host, stale-prerequisite, missing-evidence, or
  unknown ownership.
