# Serialized escalated / dogfood gate run (T018, SC-007)

The escalated set `Route` selected for this feature (`Dev`, `GeneratedGuidanceCheck`,
`TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`) **plus** the full dogfood six-target
extras (`TemplateCheck`, `GeneratedProductCheck`), run **sequentially** (FAKE shares
`.fake` state — never concurrently). Captured at the pinned SHA `4276bd0`. Raw `.log`
files live under the gitignored `readiness/logs/`; this authored note is the committed
evidence.

| # | Gate | Result | Duration | Note |
|---|---|---|---|---|
| 1 | `Dev` | **Ok** | ~2m41s | Restore, Build, Test, SampleContractSmoke, SkillSyncCheck all green |
| 2 | `GeneratedGuidanceCheck` | **Ok** | <1s | guidance/skillist/constitution prompts valid |
| — | `TemplateDrift` (Route-required) | **Ok** | ~2s | template-owned path classification clean |
| 3 | `TemplateCheck` | **Ok** | full chain | TemplatePack → InstallSource → Build → InstallPackage → SampleContractSmoke → TemplateInstantiate → Test → TemplateSmoke all green |
| 4 | `GeneratedProductCheck` | **Ok** | ~1m28s | CapabilityCheck, SkillCheck, generate + verify V3 products, semantic tests, bounded smoke, scene evidence all green |
| 5 | `EvidenceGraph` | **verdict=ok** | <1s | DAG acyclic, no dangling refs, no `[S*]`, skillist metadata + mirrors valid |
| 6 | `EvidenceAudit` | **verdict=PASS** | <1s | 0 unaccepted-synthetic, 0 auto-synthetic, 0 late-seh, 0 diff-scan, 0 readiness-contract; total-blockers=0 |

**Aggregate authority note:** aggregate FAKE results are recorded as a non-authoritative
aggregate; this run encountered **no** race-like or environment-flaky failure (no
`SkiaViewer.Tests` headless crash, no `FsiTranscripts` Class-C exclusion needed) — every
gate passed in its serialized turn, so the aggregate result here is corroborated by the
individual focused passes.

**Verdict:** the full escalated/dogfood serialized six-target set is **green** (SC-007).
`EvidenceGraph` `verdict=ok`, `EvidenceAudit` `verdict=PASS` with **zero synthetic
evidence**.
