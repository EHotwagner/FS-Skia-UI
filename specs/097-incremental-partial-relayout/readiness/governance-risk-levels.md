# Governance risk levels (feature 097, R2)

Run `./fake.sh build -t Route` first and run exactly the gates it prints.

route-tier=agent-ready
route-gates=Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
matched-rules=controls-public-surface, evidence-governance, specify-catchall, docs-only, package-surface

## small

scope=the pure `Layout.evaluateIncremental` body (propagation + cache reuse + honest `Invalidated`),
`evaluateIncremental` rounding-off config, and the `layoutDirtySet` derivation.
required evidence=`Dev` + the Layout.Tests equivalence / propagation / `Invalidated` suites (Feature097).
status=pass (Layout.Tests 28/28).

## medium

scope=`RetainedRender.step` wiring, the carried `LayoutResult` measure cache, the extended
`WorkReductionRecord.RemeasuredNodeCount`, and the `ControlInternals.evaluateLayoutIncremental` seam.
required evidence=`Dev` + Controls.Tests metric / at-rest byte-identity / E2-invariant suites on the live
path (Feature097WiringTests).
status=pass (Controls.Tests 277/277).

## broad

scope=escalation applies because internal `.fsi` (Control.fsi, RetainedRender.fsi) moved, and the
`Layout.evaluate` rounding behaviour changed (maintainer-approved). The full controls-public-surface gate
set applies.
broad validation=the serialized escalated path — surface baselines regenerated (RefreshSurfaceBaselines;
Governance.Tests 573/573), per-package surface recaptured, then Dev -> GeneratedGuidanceCheck ->
TemplateDrift -> EvidenceGraph -> EvidenceAudit. FAKE-backed targets run sequentially (shared `.fake`
state); aggregate results are recorded as non-authoritative unless re-confirmed sequentially.
