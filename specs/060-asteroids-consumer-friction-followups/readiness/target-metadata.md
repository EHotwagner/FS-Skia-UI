# Target Metadata — 060-asteroids-consumer-friction-followups

## Feature classification (T004)

- **Tier**: **Tier 1 (consumer-contract)**. No framework `.fsi` *signatures* change, but
  the consumer contract changes — generated `docs/api-surface/**` tree, split generated
  tests, corrected capability/template-update skills — and two new governance gates are
  added. Route escalates to the **maintainer-verify** path (matched rules:
  `generated-template`, `evidence-governance`, `specify-catchall`, `docs-only`,
  `skill-quality`, `build-target-contract`).
- **Affected layers**: governance (`build/Governance/**` — `ApiSurfaceGen`,
  `SkillContractPath`, `TemplateUpdatePackage`, `Routing`, `Targets`, `Update`,
  `Interpret`, `Front/Governance`, `AgentValidation`), template (`template/base/**`,
  `.template.config/template.json`, `.template.package/**`), and skills
  (`.agents/skills/**`, `template/product-skills/**`, regenerated `.claude/**`).
  **No product runtime, layout, rendering, Vulkan, or Skia change.**
- **Public-API impact**: **none** to framework signatures. The feature *surfaces* existing
  `.fsi` into generated projects (byte-identical copies via the api-surface generator) and
  adds checks that the copies match source. No `src/**/*.fsi` signature is altered, so
  `readiness/surface-baselines/*` and `readiness/per-package-surface/*` are unchanged.
- **Elmish/MVU applicability (Principle IV)**: **N/A**. No new `Model`/`Msg`/`Effect`/
  `init`/`update`/interpreter on any product surface. The generated evidence-runner's
  feature resolution is existing pure path resolution (059, shipped). The new governance
  effects (`RegenerateApiSurface`, `SkillContractPathScan`, `TemplateUpdatePackageScan`)
  follow the existing build MVU edge (pure decision in `Update`, I/O in `Interpret`).
- **Synthetic evidence (Principle V)**: **none**. FR-001/FR-003/FR-005 evidence is real
  generated-project output (see `generated-project/*.log`). No mocks/placeholders; no
  `[S]`/`[SEH]`.

## Required evidence obligations

- `generated-project/feature-resolution.log` — FR-001/SC-001 (echoed `feature-directory=`/
  `tasks=32` + loud-failure path). **Real.**
- `generated-project/api-surface.log` — FR-003/SC-002 (byte-identical api-surface in a
  freshly generated project). **Real.**
- `generated-project/test-split.log` — FR-005/SC-003 (governance vs behavior separation
  survives a model swap). **Real (structural).**
- `template/template-pack.log`, `template/template-package-contents.md` — FR-002 pack/install.
- `skill-contract-path-check.md`, `template-update-package-check.md`, `skill-quality-check.md`,
  `skill-sync-check.md`, `target-metadata-drift.md` — gate reports (written by the gates).
- `skill-loading-evidence.md` — per-task ISO-8601 skill-loading record.
- `target-metadata.md`, `agent-ready-verdict.md`, `aggregate-hang-diagnostics.md`,
  `governance-risk-levels.md`, `runtime-limitations.md`.

## Per-target verdicts (run individually + sequentially, 2026-06-04)

| Target | Verdict | Notes |
| --- | --- | --- |
| `RefreshSurfaceBaselines` | **Ok** | regenerated `validation.contract.yml` (new gates), the `docs/api-surface/**` tree, and the `.claude` mirror |
| `TargetMetadataDrift` | **Ok** | contract + api-surface + constitution + governed-block currency all green; new gates carry metadata |
| `SkillContractPathCheck` | **Ok** | every skill-claimed `docs/api-surface/...fsi` resolves to the emitted tree |
| `TemplateUpdateSkillPackageCheck` | **Ok** | template-update skill enumeration == packable set (11), zero phantom/missing |
| `SkillSyncCheck` | **Ok** | `.agents` → `.claude` currency, no drift |
| `SkillQualityCheck` | **Ok** | in-scope skills PASS |
| `GeneratedGuidanceCheck` | **Ok** | generated guidance current |
| `TemplatePack` | **Ok** | packed FS.Skia.UI.Template.0.1.82-preview.1; installed; generated FrictionCheck |
| `Dev` (Governance.Tests) | **Ok** | 415/415 incl. the new `Feature060GovernanceTests` + the resolver FR-001 test |
| `EvidenceGraph` | **Ok** | no cycles / dangling / `[S*]` |
| `EvidenceAudit` | **PASS** | merge gate — see `logs/evidence-audit.txt` |

Aggregate `Verify`/`Ci` cannot bootstrap in this sandbox (see `runtime-limitations.md`); the
authoritative verdict is `EvidenceAudit` PASS, with every constituent gate run gate-by-gate.
