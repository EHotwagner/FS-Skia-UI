# Tasks: Accepted SEH Fixture

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only
- `[F]` — failed
- `[-]` — skipped

## Phase 1: Setup

- [S] T001 [US1] [SEH] synthetic-error-handling-approved [skillist: []] Validate corrupt file rejection
- [X] T002 [skillist: []] Document accepted audit report

## Synthetic-Evidence Inventory

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T001 | Corrupt file content is the error condition, not a real successful input | infeasible, see spec FR-004 | n/a | synthetic-error-handling-approved | specs/017-synthetic-error-evidence/tasks.md:T019 | corrupt file content | fail with actionable diagnostic | accepted-seh |
