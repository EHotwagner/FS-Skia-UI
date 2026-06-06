# Governance risk levels

This feature's change is classified **medium** risk.

- **small** — a single framework-internal edit with no public-surface or
  consumer-contract impact (e.g. one `src/Scene/**/*.fs` change). Focused
  validation: `Dev` only (inner-loop tier).
- **medium** — an **additive** public `.fsi` surface change confined to one
  shipped package (`src/Controls/**`): the 41 new typed modules under
  `FS.Skia.UI.Controls.Typed`, grouped by mechanic into nine new
  `Widgets/*.fsi`/`*.fs` files. The legacy string-keyed API is frozen, so the
  diff is additive-only (81 added / 0 removed per-package surface lines).
  **Required evidence**: the regenerated surface baseline +
  `PackageSurfaceCheck` / `PerPackageSurfaceDiff` clean diff, the 41-control
  structural lowering-parity matrix, the stateful-delegation equality tests, the
  FSI transcript, and the catalog cross-check. **This feature sits here.**
- **broad** — a consumer-contract change to `template/**`, `.specify/**`,
  `build.fsx`/`scripts/build/**`, or governance paths, where **broad validation**
  (`TemplateCheck` / `GeneratedProductCheck`) is the required evidence before
  merge. This feature touches none of those for the controls migration; the one
  new `.agents/skills/fs-skia-typed-controls/SKILL.md` routes through the skill
  gate set and its generated `.claude` peer.

FAKE-backed gates run **sequentially** (shared `.fake` state); the authoritative
verdict is the per-target result, with `EvidenceAudit verdict=PASS` as the merge
gate. Any aggregate umbrella result is non-authoritative.
