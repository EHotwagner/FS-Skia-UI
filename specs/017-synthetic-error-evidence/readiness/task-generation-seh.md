# Task Generation SEH Evidence

Task generation guidance now covers eight representative classes:

| Class | Classification |
|-------|----------------|
| malformed parser input | eligible `[SEH]` |
| corrupt file content | eligible `[SEH]` |
| invalid command arguments | eligible `[SEH]` |
| protocol violations | eligible `[SEH]` |
| missing required data | eligible `[SEH]` |
| hostile payloads | eligible `[SEH]` |
| forced error-result fixtures | eligible `[SEH]` |
| convenience mocks | non-eligible ordinary `[S]` |

The generated guidance states that `[SEH]` and
`synthetic-error-handling-approved` are assigned only during design, planning,
clarification, or task generation. Split or renamed tasks preserve `[SEH]` only
when the original rationale still applies.

Evidence: `GeneratedGuidanceCheck` passed and `Synthetic error evidence
governance.guidance Synthetic documents eligible and non-eligible SEH examples`
passed on 2026-05-26.
