# Governance risk levels

This feature's change is classified **medium** risk.

- **small** — a single framework-internal edit with no public-surface or
  consumer-contract impact (e.g. one `src/Scene/**/*.fs` change). Focused
  validation: `Dev` only (inner-loop tier).
- **medium** — an **additive** public `.fsi` surface change confined to one
  shipped package (`src/Controls/**`): the new `Widget` seam and the six typed
  modules under `FS.Skia.UI.Controls.Typed`. The legacy string-keyed API is
  frozen, so the diff is additive-only (18 added / 0 removed type-level lines;
  7 added / 0 removed per-package surface lines). **Required evidence**: the
  regenerated surface baselines + `PackageSurfaceCheck` / `PerPackageSurfaceDiff`
  clean diff, the six-control structural-parity matrix, FSI transcript, and the
  render/accessibility parity capture. **This feature sits here.**
- **broad** — a consumer-contract change to `template/**`, `.specify/**`,
  `build.fsx`/`scripts/build/**`, or governance paths, where **broad validation**
  (`TemplateCheck` / `GeneratedProductCheck`) is the required evidence before
  merge. This feature touches **none** of those: `GeneratedProductCheck` runs as a
  Route-printed gate and is expected to pass unchanged because the generated
  product does not consume the typed surface in this feature.

FAKE-backed gates run **sequentially** (shared `.fake` state); the authoritative
verdict is the per-target result, with `EvidenceAudit verdict=PASS` as the merge
gate. Any aggregate umbrella result is non-authoritative.
