# Governance risk levels (feature 113)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.Controls (internal RetainedRender memo seam + cache/entry types, the DataGrid projection site, the public Diagnostics.stabilityReport val, the ControlDiagnosticCode UnstableReuseInput case) + FS.Skia.UI.Controls.Elmish (public FrameMetrics MemoHitCount/MemoMissCount)
public-api-impact=breaking FrameMetrics .fsi (two additive public fields) + new public Diagnostics.stabilityReport val + new ControlDiagnosticCode case; internal memo seam (MemoEntry/MemoCache/MemoOutcome/memoize) reached via InternalsVisibleTo
mvu-applicability=no change (Update/effects/subscriptions/interpreter untouched; dispatch OUTCOMES byte-identical, FR-014 — only WHETHER a pure subtree is recomputed or reused changes)
route-tier=agent-ready (controls-public-surface)

## Risk classification

- **small** — a test-only edit that does not touch any `.fsi`: focused validation is `Dev` plus the
  affected test list.
- **medium** — the memo seam + the `FrameMetrics` fields + the live threading: focused validation is
  `RefreshSurfaceBaselines` + `Dev` + the per-package surface diff.
- **broad** — the full escalated controls-public-surface set Route prints, required because the Controls
  AND Controls.Elmish package `.fsi` surfaces change (new vals/types/fields move the per-package
  baselines). **broad validation** is mandatory before merge. Non-authoritative aggregate results are
  advisory only in [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the authoritative
  verdict is the focused per-target rerun.

THIS feature is **broad** (a Controls + Controls.Elmish package `.fsi` surface change).

## Authoritative gate list (Route, run sequentially — shared `.fake` state, never concurrent)

Run `./fake.sh build -t Route` against the real diff and obey its printed minimal list. The Controls /
Controls.Elmish `.fsi` change escalates to **controls-public-surface**; Route prints the
controls-public-surface set (`Dev`, `PackageSurfaceCheck`, `PerPackageSurfaceDiff`, `FsiTranscripts`,
`GeneratedProductCheck`, `ControlsCatalogCheck`, `ControlsCatalogGenerationCheck`, `DesignTokenDrift`,
`ContrastCheck`, `ControlsCatalogDocsCheck`, `ControlsDocCoverageCheck`, `ControlFidelityCheck`,
`ControlsInteractionCheck`, `ControlsRenderingCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`,
`EvidenceGraph`, `EvidenceAudit`). `RefreshSurfaceBaselines` regenerates the per-package Controls +
Controls.Elmish surfaces (Controls gains the public `Diagnostics.stabilityReport`/`dataGridCells` vals,
the `UnstableReuseInput` case, the internal memo seam/types; Controls.Elmish gains the two
`FrameMetrics` fields); the top-level type-level public surface baseline is unchanged (the memo types are
internal, the additions are members/fields/a fieldless DU case).

## Required evidence per risk level

- **required evidence** (broad / Controls + Controls.Elmish `.fsi`): recaptured per-package surface
  baselines + the new vals/fields/types' `///` docs (doc-preservation gate); the top-level public surface
  baseline shows no new type symbol.
- **required evidence** (US1 memo seam): hit/miss/cold + reference-reuse on a hit
  (`Feature113MemoSeamTests`, SC-001).
- **required evidence** (US2 parity): memo-on ≡ memo-off scene byte-identity + no staleness on a real
  input change (`Feature113MemoParityTests`, SC-002/SC-003).
- **required evidence** (US3 metrics): deterministic `MemoHitCount`/`MemoMissCount` goldens — steady-state
  hits, perturbed/cold misses, idle 0/0 (`Feature113MemoMetricsTests` + the regenerated 109 corpus
  goldens, SC-004).
- **required evidence** (US4 diagnostic): a stable tree → no findings, an injected always-new input →
  flagged (`Feature113StabilityDiagTests`, SC-005) + the stable-props guidance page.
- **required evidence** (byte-identity): at-rest rendered output + geometry byte-identical — the standing
  Scene-parity golden suite under `Dev` ([byte-identity-authority.md](./byte-identity-authority.md),
  FR-014/SC-002).
- **required evidence** (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` (0 synthetic).
