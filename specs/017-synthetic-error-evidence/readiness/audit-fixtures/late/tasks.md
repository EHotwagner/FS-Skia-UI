# Tasks: Late SEH Fixture

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only
- `[F]` — failed
- `[-]` — skipped

## Phase 1: Setup

- [S] T001 [US1] [SEH] synthetic-error-handling-approved [skillist: []] Validate placeholder output shortcut
- [X] T002 [skillist: []] Document rejection

## Synthetic-Evidence Inventory

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T001 | placeholder output shortcut added after audit failure | real product output required | n/a | synthetic-error-handling-approved | implementation readiness cleanup after audit failure | placeholder output | return canned placeholder | accepted-seh |
