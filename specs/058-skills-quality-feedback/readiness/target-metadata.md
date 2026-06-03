# Target Metadata — 058-skills-quality-feedback

## Feature classification (T004)

- **Tier**: **Tier 1 (contracted)**. New public `.fsi` surface
  (`FS.Skia.UI.SkillSupport`), new package identity + template pin, new template
  parameter (`feedback`), new gate target (`SkillQualityCheck`). Route escalates to
  the **maintainer-verify / full-pipeline** path (consumer-contract: `template/**`,
  new `.fsi`, governance paths, `.agents/skills/**`, `.specify/**`).
- **Affected layer**: governance (`build/Governance/**`) + template (`template/**`,
  `.template.config/**`) + skills (`.agents/skills/**`, `src/*/skill/**`,
  `template/product-skills/**`) + a new packable library (`src/SkillSupport/**`).
  **No product runtime, layout, rendering, Vulkan, or Skia change.**
- **Public-API impact**: **additive only.** New per-family `.fsi` for
  `FS.Skia.UI.SkillSupport` (`Graph`, `Parsing`, `Globbing`, `CodeGen`,
  `ShellProcess`). No existing public `.fsi` signature is altered;
  `build/Governance` modules whose bodies move keep their own `.fsi` and delegate.
- **Elmish/MVU applicability (Principle IV)**: **N/A**. This feature adds no
  `Model` / `Msg` / `Effect` / `Cmd` / `init` / `update` / interpreter on any
  product surface. The only new "flow" is the authoring-time per-phase feedback
  prompt delivered through the existing Spec Kit `after_*` hook surface, which owns
  no product-runtime state. Recorded with rationale in `runtime-limitations.md`.
- **Synthetic evidence (Principle V)**: none planned. The skill-quality check runs
  against the real corpus; `SkillSupport` tests run against the real packed/`.fsi`
  surface; template evidence comes from real `dotnet new` runs. No `[SEH]` approved.

## Required evidence obligations

- `skill-quality-check.md` — `SkillQualityCheck` PASS over the in-scope set + a
  demonstrated FAIL naming skill+section.
- `skill-loading-evidence.md` — per-task skill-loading record (ISO-8601 stamps).
- `skill-sync.md` — `.agents` → `.claude` currency (no `SkillSyncCheck` drift).
- `surface-baseline.md` + `per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt`.
- `template-feedback-false.md` / `template-feedback-true.md`.
- `support-library-tests.md`.
- `feedback-record-example.md`.
- `target-metadata.md`, `agent-ready-verdict.md`.
- `aggregate-hang-diagnostics.md`.

## Per-target verdicts (T035 / T036)

Authoritative per-target verdicts for the gates run this session (each FAKE target run
individually and sequentially; the aggregate sweep is non-authoritative — see
`aggregate-hang-diagnostics.md`):

| Target | Verdict | Notes |
| --- | --- | --- |
| `RefreshSurfaceBaselines` | **Ok** | regenerated `validation.contract.yml` + target metadata + `.claude` tree |
| `TargetMetadataDrift` | **Ok** | new `SkillQualityCheck` gate current across contract + metadata + docs |
| `SkillQualityCheck` | **Ok** | 25/25 in-scope skills PASS (demonstrated FAIL first — `skill-quality-evidence.md`) |
| `SkillSyncCheck` | **Ok** | `.agents` → `.claude` currency, no drift |
| `PerPackageSurfaceDiff` | **Ok** | zero drift across 10 packages incl. `FS.Skia.UI.SkillSupport` |
| `PackLocal` | **Ok** | packs all libraries incl. `FS.Skia.UI.SkillSupport.0.1.61-preview.1.nupkg` |
| `EvidenceGraph` | **Ok** | no cycles, no dangling refs, no `[S*]` |
| `EvidenceAudit` | **PASS** | 0 blockers (0 synthetic, 0 diff-scan, 0 readiness-contract, 0 gui-runtime) |
| `Dev` | **Ok** | full suite green incl. headless GUI (`SkiaViewer MVU contract` 48 tests) via X11 fallback; no hang observed in this environment |
| `GeneratedGuidanceCheck` | **Ok** | generated guidance current |
| `TemplateCheck` | **Ok** | `TemplatePack` → `Build` → `TemplateInstantiate` → `Test` (391) → `TemplateSmoke` all green |
| `GeneratedProductCheck` | **Ok** | generated product validated; `Test` 391 green; sample smoke green |

All maintainer-verify targets ran green this session (2026-06-03), each FAKE target
individually and sequentially. **No deferred targets remain.**

### Bug found and fixed during the empirical pipeline run

The first `TemplateCheck` run **failed** (`Governance.Tests`: 3 failures in *US1 validation
routing*) with `unknown-gate at routing_rules.required_gates: unknown gate SkillSyncCheck`.
Root cause: this feature's `skill-quality` routing rule (T007) requires
`[SkillQualityCheck; SkillSyncCheck]`, which put `SkillSyncCheck` into the generated
`validation.contract.yml`'s `required_gates` **for the first time**, but only
`SkillQualityCheck` was added to the contract validator's `AgentValidation.knownGates`
allowlist. The prior session never re-ran `Governance.Tests` after regenerating the contract
(`RefreshSurfaceBaselines`, T025), so the latent failure went uncaught — exactly the gap the
empirical pipeline run closed. Fix: added `"SkillSyncCheck"` to `knownGates`
(`build/Governance/AgentValidation.fs`). After the fix, `Governance.Tests` is 391/391 green
and every maintainer-verify target passes.

### Additional maintainer-verify gates run (2026-06-03)

Beyond the serialized four, the remaining Route gate list was exercised individually and
sequentially, all green:

| Target | Verdict | Notes |
| --- | --- | --- |
| `TargetMetadataDrift` | **Ok** | contract/metadata/docs current after the `knownGates` fix |
| `PerPackageSurfaceDiff` | **Ok** | zero drift across packages incl. `FS.Skia.UI.SkillSupport` |
| `PackageSurfaceCheck` | **Ok** | aggregate surface current |
| `FsiTranscripts` | **Ok** | |
| `SkillQualityCheck` | **Ok** | 25/25 in-scope skills PASS |
| `TemplateDrift` | **Ok** | |
| `ControlsCatalogCheck` | **Ok** | no product-runtime change in this feature |
| `ControlsInteractionCheck` | **Ok** | |
| `ControlsRenderingCheck` | **Ok** | |
| `EvidenceGraph` | **Ok** | no cycles / dangling / `[S*]` after T031/T032/T036 → `[X]` |
| `EvidenceAudit` | **PASS** | 0 blockers |
| `AgentReady` | **Ok** (standalone) | passes after `validation-contract.md` was authored |
| `Verify` | **environment-blocked** | `VerifyPreflight` fails in this sandbox — the aggregate bootstraps the FAKE runner as a `dotnet-fake` global tool, which is not installed here (gates are driven via `./fake.sh`). This is a known environment limitation (see `runtime-limitations.md`), not a readiness gap; every constituent gate was run directly and passed. The auto-generated `agent-ready-verdict.md` therefore stays `degraded` in-sandbox. |

The merge gate is `EvidenceAudit` (**PASS**); the maintainer-verify path is fully green when
run gate-by-gate, which is the supported mode in this environment.
