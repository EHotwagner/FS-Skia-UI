# Evidence Graph Evidence

Status: setup placeholder.

## Current Generated Files

- `specs/011-controls-boundary-refactor/readiness/task-graph.json`
- `specs/011-controls-boundary-refactor/readiness/task-graph.md`

## Regeneration Command

`./fake.sh build -t EvidenceGraph`

The graph is regenerated after each task status update during implementation.

## T081 Graph-Only Run

| Command | Log | Verdict | Duration |
|---------|-----|---------|----------|
| `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/011-controls-boundary-refactor --graph-only` | `readiness/logs/t081-graph-only.txt` | PASS | 0s |

The T081 graph-only run parsed 85 tasks, wrote `readiness/task-graph.json` and
`readiness/task-graph.md`, and reported status `80[X], 5[ ]` before T081 was
marked complete.
