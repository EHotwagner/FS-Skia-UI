# Tasks: Non-Eligible SEH Fixture

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only
- `[F]` — failed
- `[-]` — skipped

## Phase 1: Setup

- [S] T001 [US1] [skillist: []] Validate with convenience mock
- [X] T002 [skillist: []] Document rejection

## Synthetic-Evidence Inventory

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T001 | Convenience mock avoids real integration | real integration smoke | n/a |  | specs/017-synthetic-error-evidence/tasks.md:T030 | convenience mock | return canned success | blocking |
