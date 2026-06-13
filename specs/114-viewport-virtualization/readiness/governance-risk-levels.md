# Governance risk levels (feature 114)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.Controls (Collections.visibleRange overscan param + CollectionModel/DataGridModel Overscan field, DataGrid offscreen ScrollRowsTo relocation + create-site a11y Collection, Types CollectionPosition + AccessibilityMetadata.Collection, internal WorkReductionRecord VirtualMaterialized/VirtualTotal) + FS.Skia.UI.Controls.Elmish (public FrameMetrics VirtualItemsMaterialized/VirtualItemsTotal)
public-api-impact=breaking FrameMetrics .fsi (two additive public fields) + additive Collections/DataGrid/Types surface (overscan param/field, CollectionPosition type, AccessibilityMetadata.Collection field); internal WorkReductionRecord count seam reached via InternalsVisibleTo
mvu-applicability=no change (Update/effects/subscriptions/interpreter untouched; dispatch OUTCOMES for materialized rows byte-identical, FR-016 — offscreen targeting is a newly reachable capability, not a changed outcome)
route-tier=agent-ready (controls-public-surface)

## Risk classification

- **small** — a test-only edit that does not touch any `.fsi`.
- **medium** — the overscan model + metric threading + offscreen relocation: `RefreshSurfaceBaselines` +
  `Dev` + the per-package surface diff.
- **broad** — the full escalated controls-public-surface set Route prints, required because the Controls
  AND Controls.Elmish package `.fsi` surfaces change. **broad validation** is mandatory before merge.
  Non-authoritative aggregate results are advisory only in
  [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the authoritative verdict is the
  focused per-target rerun.

THIS feature is **broad** (a Controls + Controls.Elmish package `.fsi` surface change).

## Authoritative gate list (Route, run sequentially — shared `.fake` state, never concurrent)

`./fake.sh build -t Route` was run against the real diff and escalates to **controls-public-surface**.
Route printed: `Dev`, `PackageSurfaceCheck`, `PerPackageSurfaceDiff`, `FsiTranscripts`,
`GeneratedProductCheck`, `ControlsCatalogCheck`, `ControlsCatalogGenerationCheck`, `DesignTokenDrift`,
`ContrastCheck`, `ControlsDocCoverageCheck`, `ControlsInteractionCheck`, `ControlsRenderingCheck`,
`GeneratedGuidanceCheck`, `SkillSyncCheck`, `SkillQualityCheck`, `PhaseHookParityCheck`,
`SkillContractPathCheck`, `TemplateUpdateSkillPackageCheck`, `TemplateDrift`, `EvidenceGraph`,
`EvidenceAudit`. `RefreshSurfaceBaselines` regenerated the per-package Controls + Controls.Elmish surfaces
and the top-level public surface baseline (the `FrameMetrics` fields + the new `CollectionPosition` type).
All printed gates were run sequentially and PASS.

## Required evidence per risk level

- **required evidence** (broad / Controls + Controls.Elmish `.fsi`): recaptured per-package + top-level
  surface baselines + the new params/fields/types' `///` docs (doc-preservation gate).
- **required evidence** (US1 bounded materialization): `Feature114OverscanTests` + `Feature114VirtualMetricsTests`
  + 109 datagrid goldens (SC-001).
- **required evidence** (US2 parity): `Feature114OverscanParityTests` + the Scene-parity suite under `Dev`
  (SC-002/SC-003/SC-007).
- **required evidence** (US3 offscreen + a11y): `Feature114OffscreenTests` + `Feature114AccessibilityTests`
  (SC-004/SC-005).
- **required evidence** (US4 metrics): `Feature114VirtualMetricsTests` + the regenerated 109 corpus goldens
  (SC-006).
- **required evidence** (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` (0 synthetic).
