# Data Model: Synthetic Error Evidence

## Synthetic Error-Handling Task

Represents a task whose purpose is to validate malformed input, hostile payloads, corrupt data, impossible states, or explicit error-path behavior using synthetic evidence.

**Fields**
- `TaskId`: stable task identifier such as `T014`
- `Status`: task completion status; completed synthetic work remains `[S]`
- `Annotation`: `[SEH]`
- `Label`: `synthetic-error-handling-approved`
- `StoryScope`: user story or phase that owns the task
- `DesignSource`: plan, task, or clarification location where `[SEH]` was assigned
- `SyntheticInputClass`: malformed input category under test
- `ExpectedErrorBehavior`: diagnostic, rejection, fallback, or recovery behavior being validated
- `Rationale`: reason real input is infeasible, unsafe, impossible, or not representative

**Validation**
- `Annotation` and `Label` must both be present.
- `Status` must remain `[S]` when evidence is synthetic-only.
- `DesignSource` must predate implementation work for the task.
- `SyntheticInputClass`, `ExpectedErrorBehavior`, and `Rationale` must be non-empty.
- The task must be scoped to malformed input or explicit error-path validation.

**State transitions**
- `Planned` -> `DesignApprovedSEH` when `[SEH]`, label, and rationale are added before implementation.
- `DesignApprovedSEH` -> `CompletedAcceptedSynthetic` when the task completes with `[S]` evidence and required disclosures.
- `Planned` -> `LateSynthetic` when synthetic-only evidence appears without prior `[SEH]`.
- `LateSynthetic` -> `Rejected` if `[SEH]` or the label is added during implementation or readiness cleanup.

## Synthetic-Evidence Inventory Entry

Reviewer-visible disclosure row for synthetic evidence.

**Fields**
- `TaskId`
- `Status`
- `Annotation`
- `Label`
- `Reason`
- `RealEvidencePath`
- `DesignSource`
- `SyntheticInputClass`
- `ExpectedErrorBehavior`
- `AcceptanceStatus`

**Validation**
- `[SEH]` entries must set `AcceptanceStatus` to `accepted-seh` only when all required task fields are present.
- Ordinary `[S]` entries must not use `accepted-seh`.
- `RealEvidencePath` may state that real input is infeasible for the specific malformed/error condition.
- Missing or contradictory fields make the entry audit-blocking.

## Evidence Audit Verdict

Represents the final synthetic-evidence verdict for a task graph.

**Fields**
- `Verdict`: `PASS | FAIL`
- `RealTaskCount`
- `AcceptedSEHTaskCount`
- `UnacceptedSyntheticTaskCount`
- `AutoSyntheticTaskCount`
- `LateSEHTaskCount`
- `Diagnostics`
- `ReportPath`

**Validation**
- `PASS` is allowed when `UnacceptedSyntheticTaskCount = 0`, `LateSEHTaskCount = 0`, all `[S]` tasks are valid accepted `[SEH]`, and no other readiness blockers exist.
- `FAIL` is required when any ordinary `[S]`, `[S*]`, late `[SEH]`, missing rationale, missing label, non-eligible synthetic fixture, or unresolved diff-scan hit remains.
- The report must list accepted `[SEH]` counts separately from real task counts.

## Late Reclassification Diagnostic

Represents an audit failure caused by assigning `[SEH]` too late.

**Fields**
- `TaskId`
- `FirstSeenAsSynthetic`
- `FirstSeenWithSEH`
- `ImplementationStartedAt`
- `FailureReason`
- `RequiredAction`

**Validation**
- `FirstSeenWithSEH` must be before `ImplementationStartedAt`.
- If provenance cannot prove design-phase classification, the diagnostic is blocking.
- `RequiredAction` must direct the contributor back to planning/task update rather than implementation cleanup.

## Eligibility Rule

Represents a classification rule used by guidance and audit fixtures.

**Fields**
- `Category`
- `EligibleForSEH`
- `Example`
- `Reason`

**Validation**
- Eligible categories include malformed parser input, corrupt file content, invalid command arguments, protocol violations, missing required data, hostile payloads, and forced error-result fixtures.
- Non-eligible categories include convenience mocks, incomplete integrations, unavailable product capability, missing host support, placeholder outputs, speed-only fixtures, and unsupported-host substitutes unless the synthetic input itself is the malformed/error-path condition.
