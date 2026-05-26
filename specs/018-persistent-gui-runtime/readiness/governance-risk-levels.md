# Governance Risk Levels

Task: T015

This feature is broad Tier 1 because it changes public SkiaViewer contracts,
generated game defaults, package verification, visual evidence, and evidence
audit behavior.

## Levels

- `small`: documentation-only or readiness wording with no command-surface or
  package impact. Required evidence is a focused file scan or focused
  governance test.
- `medium`: one package surface, template fragment, generated validation helper,
  or single governance workflow. Required evidence is the owning project tests
  and the relevant FAKE target.
- `broad`: public API, generated executable defaults, package resolution,
  visual evidence, evidence graph/audit behavior, or cross-template workflow.
  Required evidence is focused validation plus final broad validation.

## Broad Validation

Broad validation is required for this feature before final completion:

- `./fake.sh build -t Verify`
- `./fake.sh build -t EvidenceGraph`
- `./fake.sh build -t EvidenceAudit`
- `./fake.sh build -t GeneratedGuidanceCheck`

If an aggregate target times out or is interrupted, the result is a
non-authoritative aggregate until focused reruns isolate the failing stage.
Record target, verdict category, elapsed duration, last observed command,
recommended focused rerun, focused rerun result, and whether the focused rerun
is authoritative product evidence.
