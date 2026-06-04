# Target Metadata — 062-space-invaders-consumer-friction-followups

## Feature classification (T003)

- **Tier**: **Tier 1**, driven **solely by FR-010** (new public `FS.Skia.UI.SkillSupport`
  `.fsi` surface — `Random` + `Hud` modules — and a new per-package surface baseline
  `readiness/surface-baselines/FS.Skia.UI.SkillSupport.txt`). Every other workstream is
  Tier 2 content/governance (template, skill, docs, self-describing diagnostics) and
  consumer-contract-bearing, so Route **escalates** the gate list regardless.
- **Affected layers**: the canonical feedback hook source
  (`template/feedback/extensions/feedback.yml` — FR-001), Spec Kit phase skills
  (`.agents/skills/speckit-{specify,clarify,plan,tasks,analyze,checklist,implement}/SKILL.md`
  → regenerated `.claude/**` — FR-001/002/008), governance output
  (`build/Governance/Engine/Update.fs` FR-004, `build/Governance/Evidence/{Audit,Scans,TaskParser,Render}.fs`
  FR-005/007, a symbol-diff helper + generators FR-006/008), the capability skills
  (`src/SkiaViewer/skill/SKILL.md` FR-009, `src/Elmish/skill/SKILL.md` +
  `.agents/skills/fs-skia-layout-readability/SKILL.md` FR-010/011), the new public helpers
  (`src/SkillSupport/{Random,Hud}.fsi|.fs` FR-010), and new generated docs
  (`template/base/docs/{scaffold-map,evidence-formats,skillist-reference}.md` +
  `.template.config/template.json`). **No product runtime, layout, rendering, Vulkan, or
  Skia change.**
- **Public-API impact**: **FR-010 only** — new curated `.fsi` for
  `FS.Skia.UI.SkillSupport.Random` / `.Hud` and a new per-package surface baseline. No other
  framework `.fsi` signature changes. SI-8/SI-9 are resolved by pitfalls/map content, **not**
  by renaming any framework or consumer DU case (out of scope).
- **Elmish/MVU applicability (Principle IV)**: **N/A** for the framework — no
  interpreter/effects/host change. The seeded RNG is a pure value-type utility a consumer
  threads through *their* pure `update`, not a host runtime addition.
- **Synthetic evidence (Principle V)**: **none planned**. All evidence is real — real gate
  runs, a real generated project (FR-001/004/005), real RNG/band/symbol-diff unit tests. No
  mocks/placeholders; no `[S]`/`[SEH]` tasks anticipated.

## Route result (T004)

`./fake.sh build -t Route` against the spec-only baseline diff (2026-06-04):

```
developer-class=framework-author
tier=agent-ready
gates=Dev, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
dogfood-forced=false
matched-rules=evidence-governance, specify-catchall, docs-only
```

**This escalates as change-sets land.** Re-run `./fake.sh build -t Route` after each
change-set for the authoritative list. Expected escalation as governance/template/skill/
`SkillSupport` change-sets land: `TemplateCheck`, `GeneratedProductCheck`, `SkillSyncCheck`,
`TargetMetadataDrift`, `SkillQualityCheck`; and — FR-010 only —
`PackageSurfaceCheck`/`PerPackageSurfaceDiff`.

## Required evidence obligations

- `target-metadata.md` / `agent-ready-verdict.md` — Route escalated-tier artifacts. **Real.**
- `skill-loading-evidence.md` — per `(task, skill)` loading discipline. **Real.**
- `readiness-recoverability.md` — FR-005 proof (a generated project reaches passing
  `EvidenceAudit` for every format class with no `strings -el` and no sibling copy). **Real.**
- `governance-risk-levels.md`, `runtime-limitations.md`, `aggregate-hang-diagnostics.md` —
  readiness-contract required files. **Real.**
- RNG/band/symbol-diff unit-test output and the updated
  `readiness/surface-baselines/FS.Skia.UI.SkillSupport.txt` (FR-010). **Real.**

## Per-target verdict table

Stamped as each Route-printed gate is run individually and sequentially (shared `.fake`
state). The authoritative merge verdict is `EvidenceAudit` `verdict=PASS`.

| gate | verdict | note |
|---|---|---|
| EvidenceGraph | PASS | acyclic, no dangling refs, no `[S*]`; effective-DAG render incl. injected edges + skillist set |
| EvidenceAudit | **PASS** | `verdict=PASS`, 43 real tasks, 0 blockers (the authoritative merge verdict) |
| GeneratedGuidanceCheck | PASS | incl. the FR-001 feedback-hook regression check |
| SkillSyncCheck | PASS | `.claude` mirrors `.agents` after RefreshSurfaceBaselines |
| SkillQualityCheck | PASS | skill edits clear the quality bar |
| SkillContractPathCheck | PASS | |
| TemplateUpdateSkillPackageCheck | PASS | |
| TargetMetadataDrift / TemplateDrift | PASS | incl. evidence-formats.md + skillist-reference.md currency |
| PackageSurfaceCheck | PASS | (SkillSupport not in reflected-type scope) |
| PerPackageSurfaceDiff | PASS | new `FS.Skia.UI.SkillSupport.fsi.txt` baseline matches the `.fsi` (FR-010) |
| FsiTranscripts | PASS | |
| TemplateCheck | PASS | generated projects ship the three new docs + flipped feedback.yml |
| Dev | PASS | self-describing verdict ("does not compile; use Test/Verify"); writes the generic aggregate-hang report |
| GeneratedProductCheck | **EXPECTED-FAIL (non-regression)** | feature-less scaffold: generated `.specify/feature.json` has no `feature_directory`, so generated Verify cannot resolve a feature (059-documented limitation; unrelated to this feature's changes). Aggregate result is **non-authoritative** — the authoritative verdict is `EvidenceAudit verdict=PASS`. |
| Governance.Tests (full suite) | PASS | 444/444 (incl. 17 new feature-062 tests + regenerated 036/037/038 golden task-graph.md for the FR-007 render) |
| SkillSupport.Tests | PASS | 24/24 (incl. new RNG determinism/replay/bounds + HUD clamp/partition) |
