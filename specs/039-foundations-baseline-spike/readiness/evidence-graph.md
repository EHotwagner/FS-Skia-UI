# Evidence Graph Readiness — 039

| Field | Value |
|---|---|
| Authoritative command | `./fake.sh build -t EvidenceGraph` (reads `.specify/feature.json` → 039) |
| Artifact path | `specs/039-foundations-baseline-spike/readiness/task-graph.json` + `task-graph.md` |
| Failure class | graph-validation (cycles / dangling refs / mirror mismatch / unresolved skill) |
| Next action on failure | Fix `tasks.md` / `tasks.deps.yml`; never hand-write `[S*]`; re-run the target |

## Notes

The task graph for this feature is computed by the **existing, unchanged**
evidence engine (`.specify/extensions/evidence/scripts/python/compute-task-graph.py`,
driven by the `EvidenceGraph` FAKE target). This feature does not modify the
evidence engine.

Expected: acyclic, consistent, no `[S*]` surprises (no synthetic evidence is
anticipated — all evidence is real). The resolved feature id and real task
count are echoed by the target.
