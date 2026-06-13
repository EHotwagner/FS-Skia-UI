# Governance risk levels (feature 117)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.Controls (internal ControlInternals text-measure hook over the six Scene.measureText call sites + fittedFontSize; internal RetainedRender bounded TextMeasureCache + TextCacheEnabled always-miss flag + measureTextCached pure helper + WorkReductionRecord TextMeasureCacheHits/Misses + LayoutInvalidatedNodeCount carriers) + FS.Skia.UI.Controls.Elmish (three public FrameMetrics fields)
public-api-impact=breaking FrameMetrics .fsi (three additive public fields); internal RetainedRender/WorkReductionRecord/TextMeasureCache + ControlInternals measureText hook reached via InternalsVisibleTo
mvu-applicability=no change (Update/effects/subscriptions/interpreter untouched; dispatch OUTCOMES byte-identical, FR-004; the text-measure cache is interpreter-edge mutation confined to the retained step, constitution III, exactly as the 113 memo cache and the 116 picture cache)
route-tier=agent-ready (controls-public-surface)

## Risk classification

- **small** — a test-only edit that does not touch any `.fsi`.
- **medium** — the text-measure cache + metric threading + the layout-invalidated count:
  `RefreshSurfaceBaselines` + `Dev` + the per-package surface diff.
- **broad** — the full escalated controls-public-surface set Route prints, required because the
  Controls.Elmish `FrameMetrics` package `.fsi` surface changes. **broad validation** is mandatory before
  merge. Non-authoritative aggregate results are advisory only in
  [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the authoritative verdict is the
  focused per-target rerun.

THIS feature is **broad** (a Controls.Elmish package `.fsi` surface change; the Controls surface delta is
internal-only, reached via `InternalsVisibleTo`).

## Authoritative gate list (Route, run sequentially — shared `.fake` state, never concurrent)

`./fake.sh build -t Route` was run against the real diff and escalates to **controls-public-surface**.
Route printed: `Dev`, `PackageSurfaceCheck`, `PerPackageSurfaceDiff`, `FsiTranscripts`,
`GeneratedProductCheck`, `ControlsCatalogCheck`, `ControlsCatalogGenerationCheck`, `DesignTokenDrift`,
`ContrastCheck`, `ControlsDocCoverageCheck`, `ControlsInteractionCheck`, `ControlsRenderingCheck`,
`GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`. `RefreshSurfaceBaselines`
regenerated the per-package Controls + Controls.Elmish surfaces (the top-level public surface baseline
tracks type/member names, which are unchanged — the delta is three record fields). All printed gates were
run sequentially and PASS.

## Required evidence per risk level

- **required evidence** (broad / Controls.Elmish `.fsi`): recaptured per-package surface baselines + the
  three new fields' `///` docs (doc-preservation gate).
- **required evidence** (US1 text cache): `Feature117TextCacheTests` (cold→warm + per-keyed-input miss +
  hit byte-identity + always-miss oracle + empty/whitespace + fitted-caption keys, SC-001/SC-002/SC-004)
  and `Feature117CacheBoundTests` (Entries.Count <= cap, deterministic eviction, evicted re-miss, SC-005).
- **required evidence** (US2 layout-invalidated): `Feature117LayoutInvalidatedTests` (idle = 0, style-only
  = 0/0, geometry bounded with `LayoutInvalidatedNodeCount <= RemeasuredNodeCount`, drift-guard set
  unchanged, SC-006) — see [layout-invalidated-authority.md](./layout-invalidated-authority.md) for the
  spec-direction correction (the framework guarantees `<=`, not `>=`).
- **required evidence** (US3 style-only zero work): the style-only/visual-state assertions in
  `Feature117LayoutInvalidatedTests` + `Feature117MetricsTests` (zero re-measure / zero invalidated / zero
  text-cache miss over warm text, SC-003).
- **required evidence** (US1/US2/US3 metrics): `Feature117MetricsTests` + the regenerated 109 corpus
  goldens incl. the new `text-heavy-cold-warm` / `text-cache-eviction` scenarios (FR-005/FR-006/FR-010).
- **required evidence** (byte-identity, FR-004): the standing Scene-parity golden suite under `Dev` + the
  cache-on ≡ cache-off oracle (`Feature117TextCacheTests`).
- **required evidence** (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` (0 synthetic).
