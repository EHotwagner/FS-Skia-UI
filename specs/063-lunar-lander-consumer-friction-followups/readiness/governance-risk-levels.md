# Governance risk levels

- **Small** — a single content/skill/doc change-set (the FR-004/005/006/007/008 skill + doc
  edits, the FR-009 summary-discipline note). Focused validation: the Route-printed gates
  for that diff only, plus `SkillSyncCheck`/`SkillQualityCheck` after `.agents` edits.
- **Medium** — the renderer refactor (`src/SkiaViewer/**`, FR-001/002) and the
  `SymbolCrossCheck` target (`build/Governance/**`, FR-003). Focused validation: `Dev` +
  the renderer golden/pixel tests + the Evidence gates + `TargetMetadataDrift`; broad
  validation when render output or the target contract changes.
- **Broad** — the FR-010 Tier-1 `Wrap` helper change-set (new `.fsi` surface + per-package
  baseline). **Broad validation** is the **required evidence** before merge: the full
  serialized six-target order plus `PackageSurfaceCheck`/`PerPackageSurfaceDiff`, run
  **sequentially** (shared `.fake` state). Aggregate results from any broad run are recorded
  as **non-authoritative** in [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md);
  the authoritative verdict is the per-target gate (`EvidenceAudit verdict=PASS`), not the
  aggregate.
