# Feature scope and tier (071) — T002

- **Tier**: Tier 2 (internal change) throughout. Every phase matches this tier,
  so per-task `[T2]` is omitted.
- **Affected layer**: build-side fact table (`build/Governance/CatalogGen.fs`) +
  generated governance artifacts (`src/Controls/catalog.yml`,
  `src/Controls/Catalog.fs`) + tests (`tests/Controls.Tests/**`) + repo sample
  (`samples/ControlsGallery/Program.fs`) + parity fixtures + readiness evidence.
  **No shipped public `FS.Skia.UI.Controls` `.fsi`** is touched.
- **Public-API impact**: none. The 41 typed modules and their `.fsi` shipped in
  `070`. The per-package surface delta for `FS.Skia.UI.Controls` MUST be
  **additive-only or empty** (FR-010 / SC-007 / contract C11), verified by
  `PackageSurfaceCheck` / `PerPackageSurfaceDiff` (T018).
- **Elmish/MVU applicability**: **Principle IV not applicable.** This feature
  adds no new `Model`/`Msg`/`Effect`. Catalog generation is pure text
  render/splice/currency over in-memory strings (file read/write stays at the
  `Engine/Interpret.fs` edge). The typed gallery panel reuses the already-shipped
  `070` typed façades and their existing pure `update` models; **no I/O is added
  to any `update`**.
- **Evidence obligations**:
  - `readiness/catalog-single-source.md` — the 6→47 fact-table extension + regen
    rationale (T012).
  - `readiness/controls-rendering.md` — deterministic typed-gallery viewport
    render evidence, render-only, no `[S]` (T017).
  - Per-fact parity fixtures (41 new ids × 2 files) under
    `specs/066-typed-catalog-generation/readiness/parity-fixtures/` (T010).
  - Gate evidence: `readiness/evidence-graph.md`, `readiness/evidence-audit.md`,
    per-package surface diff, `readiness/skill-loading-evidence.md`.
