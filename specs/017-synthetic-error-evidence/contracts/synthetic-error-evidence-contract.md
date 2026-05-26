# Contract: Synthetic Error Evidence Classification

## Task Line Requirements

An accepted synthetic error-handling task must be visible in `tasks.md` with:

```text
- [S] T### [SEH] ... synthetic-error-handling-approved ... [skillist: ...]
```

Rules:

- `[S]` is the only valid completed status for synthetic-only `[SEH]` work.
- `[SEH]` must appear before implementation begins.
- `synthetic-error-handling-approved` must be mirrored in structured task metadata or the Synthetic-Evidence Inventory.
- The task description must name the error-handling behavior under test.

## Inventory Row Requirements

The Synthetic-Evidence Inventory must include these fields for each `[SEH]` task:

| Field | Requirement |
|-------|-------------|
| Task | Stable task identifier |
| Reason | Why synthetic input is required |
| Real-evidence path | Real input replacement path, or explicit infeasible rationale |
| Tracking issue | Required only when future real evidence is planned |
| Label | `synthetic-error-handling-approved` |
| Design source | Spec, plan, or task location assigning `[SEH]` |
| Synthetic input class | Malformed/error-path input category |
| Expected error behavior | Diagnostic, rejection, fallback, or recovery behavior |
| Acceptance status | `accepted-seh` or blocking diagnostic |

## Eligibility Rules

Eligible `[SEH]` examples:

- malformed parser input
- corrupt file content
- invalid command arguments
- protocol violations
- missing required data
- hostile payloads
- forced error-result fixtures

Non-eligible examples:

- convenience mocks
- incomplete integrations
- unavailable product capability
- missing host support
- placeholder outputs
- tests avoiding real behavior only for speed
- ordinary in-memory substitutes

## Provenance Rules

`[SEH]` classification is valid only when the design source proves that the classification existed before implementation work for the task began.

Late reclassification is invalid when:

- `[SEH]` first appears after implementation starts
- the label first appears during readiness cleanup
- the label appears only after an audit failure
- task split/rename loses the original rationale
- provenance cannot be established
