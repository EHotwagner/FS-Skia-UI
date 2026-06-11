# Governance risk levels (feature 098, R3)

Run `./fake.sh build -t Route` first and run exactly the gates it prints.

route-tier=agent-ready
route-gates=Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, TemplateCheck, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, SkillContractPathCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
matched-rules=controls-public-surface, generated-template, evidence-governance, specify-catchall, docs-only, package-surface

## small

scope=the pure `nearestAuthored` one-predicate widening (authored = keyed OR `node.Id ∈ BoundIds`) and the
`boundIdsOf` derivation.
required evidence=`Dev` + the Controls.Tests recovery / single-scheme-agreement / FsCheck-distinctness suites
(Feature090RecoveryTests, Feature098UnifiedSchemeTests).
status=pass (Controls.Tests 282/282).

## medium

scope=the id-scheme unification (`eventBindings`/`collectBoundsWith`/`Control.dispatch` path-threading), the
`RetainedRender` `BoundIds` population, and the live-adapter dispatch seam.
required evidence=`Dev` + the Controls.Tests `Control.dispatch` keyed-regression suite (InteractionTests) and
the Elmish.Tests routing-seam dispatch / fallback suites (Feature098DispatchTests).
status=pass (Controls.Tests 282/282, Elmish.Tests 55/55).

## broad

scope=escalation applies because the public `src/Controls/**/*.fsi` changed (the `BoundIds` field +
`val boundIdsOf` + the documented unkeyed canonical-id change `Kind → path`). The full
controls-public-surface gate set applies.
broad validation=the escalated path — surface baselines regenerated (RefreshSurfaceBaselines; per-package
+ api-surface tree recaptured), then the gates `Route` printed run **sequentially** (shared `.fake` state):
Dev -> PackageSurfaceCheck -> PerPackageSurfaceDiff -> FsiTranscripts -> TemplateCheck ->
GeneratedProductCheck -> ControlsCatalogCheck -> ControlsCatalogGenerationCheck -> DesignTokenDrift ->
ContrastCheck -> ControlsInteractionCheck -> ControlsRenderingCheck -> GeneratedGuidanceCheck ->
SkillContractPathCheck -> TemplateDrift -> EvidenceGraph -> EvidenceAudit. Aggregate results are recorded
as non-authoritative unless re-confirmed sequentially.
