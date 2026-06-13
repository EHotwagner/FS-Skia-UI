# Governance risk levels (feature 116)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.Controls (internal RetainedRender damage-set + widened picture-cache correctness key + bounded cross-frame LRU + PictureCacheEnabled always-miss flag + offscreen-effect diagnostic + WorkReductionRecord carriers; Types additive ControlDiagnosticCode.OffscreenComposition case) + FS.Skia.UI.Controls.Elmish (six public FrameMetrics fields)
public-api-impact=breaking FrameMetrics .fsi (six additive public fields) + additive Types ControlDiagnosticCode case; internal RetainedRender/WorkReductionRecord/PictureCache seam reached via InternalsVisibleTo
mvu-applicability=no change (Update/effects/subscriptions/interpreter untouched; dispatch OUTCOMES byte-identical, FR-014; the picture cache is interpreter-edge mutation confined to the retained step, constitution III, exactly as the 113 memo cache)
route-tier=agent-ready (controls-public-surface)

## Risk classification

- **small** — a test-only edit that does not touch any `.fsi`.
- **medium** — the damage-set accumulation + metric threading + bounded picture cache + offscreen
  detector: `RefreshSurfaceBaselines` + `Dev` + the per-package surface diff.
- **broad** — the full escalated controls-public-surface set Route prints, required because the Controls
  `Types` AND Controls.Elmish `FrameMetrics` package `.fsi` surfaces change. **broad validation** is
  mandatory before merge. Non-authoritative aggregate results are advisory only in
  [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the authoritative verdict is the
  focused per-target rerun.

THIS feature is **broad** (a Controls + Controls.Elmish package `.fsi` surface change).

## Authoritative gate list (Route, run sequentially — shared `.fake` state, never concurrent)

`./fake.sh build -t Route` was run against the real diff and escalates to **controls-public-surface**.
Route printed: `Dev`, `PackageSurfaceCheck`, `PerPackageSurfaceDiff`, `FsiTranscripts`,
`GeneratedProductCheck`, `ControlsCatalogCheck`, `ControlsCatalogGenerationCheck`, `DesignTokenDrift`,
`ContrastCheck`, `ControlsDocCoverageCheck`, `ControlsInteractionCheck`, `ControlsRenderingCheck`,
`GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`. `RefreshSurfaceBaselines`
regenerated the per-package Controls + Controls.Elmish surfaces and the top-level public surface baseline
(the `FrameMetrics` fields + the new `ControlDiagnosticCode` case). All printed gates were run
sequentially and PASS.

## Required evidence per risk level

- **required evidence** (broad / Controls + Controls.Elmish `.fsi`): recaptured per-package + top-level
  surface baselines + the new fields/case `///` docs (doc-preservation gate).
- **required evidence** (US1 damage): `Feature116DamageTests` (small hover vs frame-spanning theme vs
  idle-zero, deterministic integers) + the 109 corpus goldens (SC-001).
- **required evidence** (US2 picture cache): `Feature116PictureCacheTests` (hit byte-identity +
  per-keyed-input miss + always-miss oracle, SC-002/SC-003).
- **required evidence** (US3 bounded cache): `Feature116CacheBoundTests` (EntryCount <= cap, deterministic
  eviction, evicted re-miss, SC-004).
- **required evidence** (US4 offscreen diagnostic): `Feature116OffscreenDiagTests` (fires/does-not-fire,
  output byte-identical, SC-005).
- **required evidence** (US5 metrics): `Feature116MetricsTests` + the regenerated 109 corpus goldens incl.
  the new `picture-cache-reuse` / `picture-cache-eviction` scenarios (SC-006/SC-007).
- **required evidence** (byte-identity, FR-014): the standing Scene-parity golden suite under `Dev`.
- **required evidence** (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` (0 synthetic).
