# Contract: Evidence Audit for `[SEH]`

## Report Summary

`EvidenceAudit` output must include:

```text
verdict=PASS|FAIL
real-tasks=<count>
accepted-seh-tasks=<count>
unaccepted-synthetic-tasks=<count>
auto-synthetic-tasks=<count>
late-seh-tasks=<count>
diff-scan-hits=<count>
message=<reviewer-facing summary>
```

## PASS Conditions

The audit may return PASS when all conditions are true:

- every synthetic task is `[S]` plus valid `[SEH]`
- every `[SEH]` task has `synthetic-error-handling-approved`
- every `[SEH]` task has design-phase provenance
- every `[SEH]` task has required inventory fields
- no ordinary `[S]` or `[S*]` task remains
- no late reclassification diagnostic remains
- no other readiness or diff-scan blocker remains

The PASS report must still count accepted `[SEH]` tasks as synthetic, not real.

## FAIL Conditions

The audit must return FAIL when any condition is true:

- a synthetic task lacks `[SEH]`
- a `[SEH]` task lacks the approval label
- a `[SEH]` task lacks design-phase source
- a `[SEH]` task was first classified during implementation or after audit failure
- a task uses `[SEH]` for a convenience mock, unavailable host, incomplete product capability, placeholder, or speed-only fixture
- an `[S*]` dependency propagation remains outside accepted `[SEH]` scope
- required inventory fields are missing

## Diagnostic Requirements

Each failure diagnostic must name:

- task identifier
- failed rule
- observed tag/label/status
- missing or invalid field
- source location if available
- required action

Required actions must direct late or missing classification back to design/task update rather than implementation-time cleanup.
