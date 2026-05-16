# Evidence Graph

`./fake.sh build -t EvidenceGraph` refreshes:

- `readiness/task-graph.json`
- `readiness/task-graph.md`

Final result:

- graph status: PASS, acyclic and consistent
- parsed tasks: 109
- done: 108
- synthetic: 0
- propagated synthetic: 0
- skipped: 1

T108 is skipped because external first-time evaluator evidence cannot be
produced inside this workspace.
