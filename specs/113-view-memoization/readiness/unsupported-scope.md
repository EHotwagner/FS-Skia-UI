# Unsupported scope & failure diagnostics (feature 113, T008)

- Phase 6+ is OUT: viewport virtualization (Phase 6), damage rects / picture / paint caches (Phase 7),
  text / layout-boundary caches (Phase 8), SkiaViewer backend / render-thread / compositor review
  (Phase 9).
- No public consumer `Control.memo` / `Widget.memo` primitive (deferred, clarified 2026-06-12).
- No enforced stability gate — the stability diagnostic is **report-only** this rung.
- Only a representative memoized site (the DataGrid row/column projection) is wired; `Style.resolve` and
  the full 52-control migration are OUT (the seam is kept general enough to wrap them later, but
  `Style.resolve` lowers to a `ResolvedStyle` not a `Scene list`, so wiring it requires widening the
  stored subtree type — a later rung).
- The seam **misses** (never reuses) on an unequal/unknown dependency, so a too-coarse dependency is
  caught by the memo-on/memo-off parity test, never a stale render (FR-007).
- Features 110/111/112 are UNCHANGED (FR-015).
- Principle IV (MVU) is N/A — no Model/Msg/Effect/interpreter change; the memo cache lives in the
  retained interpreter-edge state, and dispatch OUTCOMES are byte-identical (FR-014). The interactive-UI
  run-and-use gate is N/A — the feature delivers an internal seam + deterministic metrics observable via
  `ControlsElmish.Perf.runScript` + a report-only diagnostic, not a new interactive surface.
