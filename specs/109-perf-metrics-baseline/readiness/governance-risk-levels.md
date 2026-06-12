# Governance risk levels (feature 109)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.Controls.Elmish
public-api-impact=yes (breaking: remove `ViewRebuilt`; add `ProductModelChanged`, `ViewCalled`, `FullRenderCount`)
mvu-applicability=no change (observation only — Update/effects/interpreter untouched; the pure
`Perf.runScript` fold only records different FACTS)
route-tier=agent-ready (controls-public-surface, maintainer-verify)

## Risk classification

- **small** — a test-only or corpus/golden edit that does not touch the public `.fsi`: focused
  validation is `Dev` plus the affected test list. (e.g. adding a corpus scenario.)
- **medium** — the `FrameMetrics` `.fsi`/`.fs` field change and construction-site updates: focused
  validation is `RefreshSurfaceBaselines` + `Dev` + `GeneratedProductCheck`.
- **broad** — full escalated controls-public-surface (maintainer-verify) order, required because the
  public `ControlsElmish.fsi` surface changes breakingly. **broad validation** is mandatory before
  merge. Non-authoritative aggregate results are advisory only in
  [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the authoritative verdict is the
  focused per-target rerun.

THIS feature is **broad** (a breaking public `.fsi` change to `FrameMetrics`).

## Authoritative gate list (Route, run sequentially — shared `.fake` state, never concurrent)

Run `./fake.sh build -t Route` against the real diff and obey its printed minimal list. The escalated
controls-public-surface order is:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

`RefreshSurfaceBaselines` regenerates the surface + per-package baselines after the field change.

## Required evidence per risk level

- **required evidence** (broad / public `.fsi`): recaptured surface + per-package baselines
  (`PackageSurfaceCheck` / `PerPackageSurfaceDiff` green over the regenerated baselines showing ONLY
  the `FrameMetrics` field delta) and the new fields' `///` docs (doc-preservation gate).
- **required evidence** (US1 metric honesty): the three scripted frames + idle (SC-001/004) and the
  animation-only-tick divergence (SC-011), asserted in `Feature109MetricsHonestyTests`.
- **required evidence** (US2 corpus): a byte-stable per-scenario metrics golden under
  `readiness/perf-corpus/` that re-runs identically (SC-005) and answers counts (SC-006).
- **required evidence** (US3 coalescing): received=N / processed≤1 (SC-002), no discrete drop (SC-003),
  drag-path retained (FR-011) — `Feature109MetricsHonestyTests`.
- **required evidence** (US4 baselines): the non-golden before/after coalescing timing+allocation under
  `docs/reports/_baselines/` (FR-019/SC-007), count-first thresholds (FR-018), no timing in goldens
  (SC-009).
- **required evidence** (observation-only): at-rest output + default host path byte-identical (FR-020 /
  SC-008).
- **required evidence** (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` (0 synthetic).
