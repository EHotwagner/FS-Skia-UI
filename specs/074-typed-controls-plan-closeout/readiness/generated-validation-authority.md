# Generated-Validation Authority — Typed-Controls Plan Closeout (074)

The authoritative verdict for this feature is **each focused gate's own result**, run
sequentially on the real repository working tree. Aggregate/multi-target runs and the generated
product's own Verify are recorded as **non-authoritative**.

## Routed gate list (T017, SC-006)

`./fake.sh build -t Route` → `developer-class=framework-author`, `tier=agent-ready`,
matched-rules `generated-template, evidence-governance, specify-catchall, docs-only,
skill-quality`. Printed gates and their authoritative verdicts:

| Gate | Verdict | Authority |
| --- | --- | --- |
| `Dev` | ✅ PASS | authoritative |
| `SkillSyncCheck` | ✅ PASS (zero drift, both skills — SC-002) | authoritative |
| `SkillQualityCheck` | ✅ PASS (new skill carries all rubric sections) | authoritative |
| `SkillContractPathCheck` | ✅ PASS (referenced contract paths resolve) | authoritative |
| `TemplateUpdateSkillPackageCheck` | ✅ PASS | authoritative |
| `GeneratedGuidanceCheck` | ✅ PASS | authoritative |
| `TemplateCheck` | ✅ PASS (pack + install + instantiate + smoke) | authoritative |
| `TemplateDrift` | ✅ PASS | authoritative |
| `GeneratedProductCheck` | ❌ FAIL | **non-authoritative** (environment) |
| `EvidenceGraph` | ✅ PASS | authoritative (see `evidence-graph.md`) |
| `EvidenceAudit` | ✅ PASS | authoritative (see `evidence-audit.md`) |

## GeneratedProductCheck — non-authoritative

The only failing gate. The generated product's own `Dev`, `GeneratedGuidanceCheck`, and
`TemplateDrift` complete; it fails only at the evidence-graph step because the generated
product's `.specify/feature.json` has no usable `feature_directory` entry (it never falls back
to a bundled sample). This is a documented **environment failure**, independent of this
documentation/governance change. See `aggregate-hang-diagnostics.md` and
`generated-product-verify/app-source/verify.log`.

## Public package surface delta (T018, SC-005)

**Zero delta attributable to this feature.** This is a documentation/governance-only change:
no `.fsi` signature, no curated public surface, no per-package or cross-package surface baseline
is touched. The only changed files are skill `SKILL.md` sources, their regenerated `.claude`
peers, the regenerated `skillist-reference.md` index, and the historical implementation-plan
report — none of which is package public surface. `Reconcile` remains `module internal` (FR-010).
