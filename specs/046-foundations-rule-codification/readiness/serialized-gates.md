# Serialized escalated FAKE gate run (T024)

`Route` escalated this change (tier=agent-ready, governance + generated-product-contract +
`.agents/skills/**`). The full serialized six-target set was run **sequentially**, never
concurrently (FAKE shares `.fake` state). Aggregate results are **non-authoritative**; no
race-like failure occurred, so no focused rerun was needed (this feature touches no
runtime/viewer path).

Raw per-gate logs live under `readiness/logs/` (regenerable, gitignored).

| # | Target | Command | Status |
|---|--------|---------|--------|
| 1 | `Dev` | `./fake.sh build -t Dev` | **Ok** (1m37s) |
| 2 | `GeneratedGuidanceCheck` | `./fake.sh build -t GeneratedGuidanceCheck` | **Ok** (folds in the new Constitution-Check gate) |
| 3 | `TemplateCheck` | `./fake.sh build -t TemplateCheck` | **Ok** |
| 4 | `GeneratedProductCheck` | `./fake.sh build -t GeneratedProductCheck` | **Ok** (`schema_version: 1.0` discoverable) |
| 5 | `EvidenceGraph` | `./fake.sh build -t EvidenceGraph` | **Ok** (acyclic, no dangling refs, skill-loading evidence present) |
| 6 | `EvidenceAudit` | `./fake.sh build -t EvidenceAudit` | **Ok** — `verdict=PASS`, `total-blockers=0` |

Additionally `Route` named `TemplateDrift` → `./fake.sh build -t TemplateDrift` → **Ok**.

## EvidenceAudit verdict (SC-010 — zero synthetic)

```
verdict=PASS
real-tasks=22
accepted-seh-tasks=0
unaccepted-synthetic-tasks=0
auto-synthetic-tasks=0
late-seh-tasks=0
diff-scan-hits=0
readiness-contract-hits=0
total-blockers=0
```

All evidence is real: typed unit tests over real parsers, live seeded-violation gate
failures, the real `.agents`→`.claude` generation-currency check, real `git check-ignore`
output, and the serialized gate logs above. No `[S]`/`[SEH]` task; nothing to accept.
