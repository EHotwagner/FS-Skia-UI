# Evidence policy separation (T003)

The graph gate and the merge-gate audit are **separate** stages with separate authority.

| Stage | Authoritative command | Artifact | What it decides | Failure class | Next action |
|-------|-----------------------|----------|-----------------|---------------|-------------|
| Graph | `./fake.sh build -t EvidenceGraph` | `readiness/evidence-graph.md`, `readiness/task-graph.{md,json}` | DAG is acyclic, no dangling refs, `[S*]` propagation, `skillist` metadata + visible mirrors valid | cycle / dangling dep / invalid metadata | fix `tasks.deps.yml` / `tasks.md` and re-run the graph gate |
| Audit | `./fake.sh build -t EvidenceAudit` | `readiness/evidence-audit.md` | merge verdict: unaccepted-synthetic, auto-synthetic, late-`[SEH]`, diff-scan blocking, readiness-contract blocking | any non-zero blocking count → `verdict=FAIL` | resolve the named blocking count; never relabel `[S]`→`[SEH]` at implementation time |

Separation rule: the graph gate **never** emits a merge verdict, and the audit **never**
re-derives the graph topology silently — each consumes the same in-process compiled-F#
evidence engine inputs but answers a different question. This feature ships **zero**
synthetic evidence, so the audit's synthetic/`[SEH]` counts are all expected to be 0
(SC-010).
