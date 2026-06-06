# Validation contract — routing + gate record (feature 069)

The new `DesignTokenDrift` target and the `controls-public-surface` routing extension are
wired into governance, and the generated `validation.contract.yml` was regenerated from the
compiled `Routing.fs` single source via `RefreshSurfaceBaselines`, so the currency checks
cannot drift.

## `./fake.sh build -t Route` for this change

```
developer-class=framework-author
tier=maintainer-verify
gates=Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, TemplateCheck, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, SkillSyncCheck, SkillQualityCheck, SkillContractPathCheck, TemplateUpdateSkillPackageCheck, TemplateDrift, EvidenceGraph, EvidenceAudit, AgentReady, TargetMetadataDrift, Verify, Ci
dogfood-forced=false
matched-rules=controls-public-surface, generated-template, evidence-governance, specify-catchall, docs-only, package-surface, skill-quality, build-target-contract
```

The `controls-public-surface` rule now lists **`DesignTokenDrift`** alongside the existing
controls-surface gates (confirmed in the printed `gates=` line above).

## Wiring

- **Targets** (`build/Governance/Targets.fs` + `.fsi`): `DesignTokenDrift` added to the `Target`
  DU, `name`, `directPrerequisites` (`[]`), `failureOwner` (`product`), and `allTargets`
  (mirroring `ControlsCatalogGenerationCheck`).
- **knownGates** (`AgentValidation.fs`): `DesignTokenDrift` added to the validation-contract
  allowlist (otherwise rule selection emits `unknown-gate`).
- **Routing** (`build/Governance/Routing.fs`): `Targets.DesignTokenDrift` added to the
  `controls-public-surface` rule's required-gate list.
- **Effect** (`Engine/Model.fs(i)`, `Interpret.fs`, `Front/Governance.fs`):
  `RegenerateDesignTokens` → `regenerateDesignTokens` (whole-file regen at the interpreter edge),
  spliced into `RefreshSurfaceBaselines`; the `DesignTokenDrift` arm added in `Engine/Update.fs`.
- **validation.contract.yml**: regenerated from `Routing.fs`; now renders `DesignTokenDrift`
  under `controls-public-surface`.

## Per-gate results (authoritative; run sequentially)

| Gate | Result |
|------|--------|
| `Route` | Ok (maintainer-verify; `DesignTokenDrift` listed) |
| `Dev` | Ok (build + all semantic/parity suites, incl. Feature 069) |
| `DesignTokenDrift` | Ok (PASS — see `design-token-drift.md`) |
| `PackageSurfaceCheck` | Ok |
| `PerPackageSurfaceDiff` | Ok (additive-only delta) |
| `ControlsCatalogCheck` | Ok |
| `ControlsCatalogGenerationCheck` | Ok |
| `ControlsInteractionCheck` | Ok |
| `ControlsRenderingCheck` | Ok |
| `FsiTranscripts` | Ok |
| `GeneratedGuidanceCheck` | Ok |
| `SkillSyncCheck` | Ok |
| `SkillQualityCheck` | Ok (incl. new `fs-skia-design-tokens` skill) |
| `SkillContractPathCheck` | Ok |
| `TargetMetadataDrift` | Ok (validation.contract.yml current vs Routing.fs) |
| `TemplateCheck` | Ok |
| `GeneratedProductCheck` | Failure — **environmental/pre-existing**, not a 069 regression (generated product's own `.specify/feature.json` unresolvable + no `SPECKIT_FEATURE_DIR`; aborts before touching Controls). See `logs/route-gates.txt`. |
| `TemplateDrift` | Ok |
| `EvidenceGraph` | Ok — see `evidence-graph.md` |
| `EvidenceAudit` | Ok (verdict=PASS, 0 blockers, accepted-seh=1 [T011]) — see `evidence-audit.md` |

Full per-gate transcript: `readiness/logs/route-gates.txt`. Aggregate `Route`/multi-gate
summaries are non-authoritative; the per-gate lines above are authoritative.
