# Serialized Escalated Six-Target Gate Run (T028)

`Route` escalates this `.specify/**` + skill-tree + governance-path change to the full
serialized FAKE gate set (FR-013/FR-015). FAKE-backed gates were run **sequentially**
(shared `.fake` state — never concurrently), in the deterministic order. Aggregate results
below; any race-like/environment-flaky failure would be rerun focused under a stash control
as the authoritative result (none were needed — every gate passed clean on the first run).

| # | Gate | Result | Notes |
|---|------|--------|-------|
| 1 | `Dev` | **PASS** (exit 0) | Restore + Build + non-visual Test + the reframed `SkillSyncCheck` (now Dev's only skill gate after `SkillExamplesCheck` retirement) |
| 2 | `GeneratedGuidanceCheck` | **PASS** (exit 0) | Generated prompt / task-skill / guidance governance unaffected |
| 3 | `TemplateCheck` | **PASS** (exit 0) | Template pack + install (source/package) + instantiate + smoke; the `BEGIN/END GENERATED` marker regions added to `plan-template.md` / `tasks-template.md` are governance-only and did not change generated-product content |
| 4 | `GeneratedProductCheck` | **PASS** (exit 0) | Default/minimal generated-project validation — no generated `Dev` behavior change |
| 5 | `EvidenceGraph` | **PASS** (exit 0) | Acyclic DAG, no dangling refs, valid skillist metadata + mirrors; the reframed active-feature skillist currency check is green for the committed state |
| 6 | `EvidenceAudit` | **PASS** (exit 0) | `verdict=PASS`; `unaccepted-synthetic=0`, `auto-synthetic=0`, `accepted-seh=0`, `late-seh=0`, `diff-scan-hits=0`, `readiness-contract-hits=0` |

## EvidenceAudit verdict (authoritative)

```
verdict=PASS
accepted-seh-tasks=0
unaccepted-synthetic-tasks=0
auto-synthetic-tasks=0
late-seh-tasks=0
diff-scan-hits=0
readiness-contract-hits=0
```

## Coherence follow-through (T023)

`SkillExamplesCheck` was retired from the typed `Targets` DU, so the runnable-target
registry dropped 38 → 37. `RefreshSurfaceBaselines` regenerated `validation.contract.yml`
from `Routing.fs`; the file is byte-unchanged (it carries no per-target references), and
`TargetMetadataDrift` stays green (the `target-metadata.json` golden fixture + the
`CommandContractTests` Dev-prereq expectation were updated to match). The full
`Governance.Tests` suite (326/326) — including the `build.fsx`-compiling `workflow
self-check` — passes, confirming the target-set change is coherent.

**Verdict: PASS** — all six escalated gates green on the first sequential run; zero
non-authoritative flakes required a focused rerun.
