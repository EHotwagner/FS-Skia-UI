# Target Metadata — 061-breakout-consumer-friction-followups

## Feature classification (T002)

- **Tier**: **Tier 2 (internal/content change)**. No package identities change and no public
  `.fsi` surface is added (D8 documents the arcade helpers as conventions, not shipped API).
  The change is consumer-contract-bearing (template / skill / governance output), so Route
  **escalates** the gate list even though no `.fsi` / surface-baseline updates are required.
- **Affected layers**: governance (`build/Governance/Evidence/{Scans,Render}.fs(i)`,
  `build/Governance/Front/Governance.fs` — FR-004/FR-005/FR-007 self-describing output),
  Spec Kit phase skills (`.agents/skills/speckit-{specify,clarify,plan,analyze,checklist}/SKILL.md`
  → regenerated `.claude/**` — FR-001/FR-002), the template-only feedback skill
  (`template/feedback/skill/SKILL.md` + `template/feedback/extensions/feedback.yml` — FR-003),
  authoring templates (`.specify/{presets/fsharp-opinionated/,}templates/*.md`,
  `.agents/skills/speckit-tasks/SKILL.md` — FR-006/008/009), the shipped keyboard skill
  (`template/product-skills/fs-skia-keyboard-input/SKILL.md` — FR-010), and the capability
  skills (`src/Elmish/skill/SKILL.md`, `.agents/skills/fs-skia-layout-readability/SKILL.md`
  — FR-011). **No product runtime, layout, rendering, Vulkan, or Skia change.**
- **Public-API impact**: **none** to framework signatures (Principle II). The only `.fsi`
  edit is `build/Governance/Evidence/Render.fsi`, adding two build-internal render helpers
  (`graphVerdictLine`, `readinessContractDiagnostics`) — governance tooling surface, not a
  shipped product package. No `src/**/*.fsi` signature changes, so surface baselines are
  unchanged.
- **Elmish/MVU applicability (Principle IV)**: **N/A**. No new `Model`/`Msg`/`Effect`/
  `init`/`update`/interpreter on any product surface. The FR-004/FR-007 additions are pure
  render-string producers. The arcade helpers (FR-011) are documented as pure `update`-side
  conventions, not implemented as runtime.
- **Synthetic evidence (Principle V)**: **none**. All evidence is real — passing Governance
  unit tests, real gate runs, and a regenerated `.claude` mirror. No mocks/placeholders; no
  `[S]`/`[SEH]` tasks.

## Required evidence obligations

- `feedback-hook-autofire.md` — FR-001/002/003 (multi-file discovery present + mirrored,
  four prompts + `## Skill gaps`, no-surviving-"three prompts" grep). **Real.**
- `readiness-recoverability.md` — FR-004/005 (self-describing readiness diagnostic + single
  `product-defect` spelling, proven by Governance unit tests). **Real.**
- `arcade-helper-triage.md` — FR-011 per-helper document-vs-ship decision (all four
  documented). **Real.**
- `governance-risk-levels.md`, `runtime-limitations.md`, `aggregate-hang-diagnostics.md` —
  readiness-contract files (authored to the enforced token lists). **Real.**
- `skill-loading-evidence.md` — per-task ISO-8601 skill-loading record.
- `target-metadata.md`, `agent-ready-verdict.md`, `focused-gates.md`.

## Per-target verdicts (run individually + sequentially, 2026-06-04)

| Target | Verdict | Notes |
| --- | --- | --- |
| `RefreshSurfaceBaselines` | **Ok** | regenerated the `.claude` skill mirror from the edited `.agents` sources (FR-001/010/011) + `validation.contract.yml` |
| `Dev` (Governance.Tests) | **Ok** | 427/427 incl. the new `Feature061GovernanceTests` (FR-004/007 render, FR-003 prompt count, FR-005/006/008/009/010 content) |
| `GeneratedGuidanceCheck` | **Ok** | generated guidance current |
| `SkillSyncCheck` | **Ok** | `.agents` → `.claude` currency, no drift |
| `SkillQualityCheck` | **Ok** | in-scope skills (incl. the edited `fs-skia-elmish` / `fs-skia-layout-readability` / feedback skill) PASS |
| `SkillContractPathCheck` | **Ok** | skill-claimed api-surface paths resolve |
| `TemplateUpdateSkillPackageCheck` | **Ok** | template-update skill enumeration == packable set |
| `TemplateDrift` | **Ok** | template alignment current |
| `TemplateCheck` | **Ok** | pack → install → instantiate → Test(24s) → TemplateSmoke green across profiles (2m07s); exercises the FR-003 fourth feedback prompt, the FR-006 README/product Dev-vs-Test guidance, and the FR-010 keyboard skill in generated projects |
| `GeneratedProductCheck` | **Expected-fail (not a regression)** | the generated scaffold's own `Dev` / `GeneratedGuidanceCheck` / `TemplateDrift` PASS; its `EvidenceGraph` loud-fails with "Cannot resolve the feature to validate … no usable feature_directory" — 059's `resolveFeatureDir` working as designed on a feature-less fresh scaffold (the 060 precedent, classified identically there). This feature touches no feature-resolution code; the authoritative template validation is `TemplateCheck` (PASS) and the merge gate is `EvidenceAudit` (PASS). |
| `EvidenceGraph` | **Ok** | no cycles / dangling / `[S*]`; new terminal `verdict=ok (no cycles, no dangling refs, no [S*])` line prints (FR-007) |
| `EvidenceAudit` | **PASS** | merge gate — see `logs/evidence-audit.txt` (0 blockers) |

Aggregate `Verify`/`Ci` cannot bootstrap in this sandbox (see `runtime-limitations.md`); the
authoritative verdict is `EvidenceAudit` PASS, with every constituent gate run gate-by-gate.
`GeneratedProductCheck`'s generated-scaffold `EvidenceGraph` step is the intended 059 loud
feature-resolution failure on a feature-less scaffold, not a 061 regression.
