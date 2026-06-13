# Unsupported scope & deferral (feature 114)

This feature is **Phase 6 only** (viewport virtualization for repeated controls). Explicitly OUT this
rung:

- **variable / measured row heights** and **row/column/text measurement caches** — Phase 8 (FR-018);
  this rung keeps **uniform fixed `RowHeight`** (as the model already is).
- **paint caches / damage rectangles / Skia picture boundaries** — Phase 7.
- **layout hot-path / text-measurement caches & layout-boundary hints** — Phase 8.
- **`SkiaViewer` backend / render-thread / compositor review** — Phase 9.
- **generalizing virtualization to non-DataGrid list/collection surfaces** — DataGrid is the
  representative this rung; the shared `Collections` model MAY carry overscan for future reuse (and does:
  `CollectionModel.Overscan`), but only the DataGrid path is wired/validated here.
- **horizontal / column virtualization** — rows only.

No renderer rewrite, no Avalonia/WPF redesign, no platform/release/distribution scope. Features
110/111/112/113 are unchanged (FR-017); feature 113's DataGrid `gridGeom` memoization continues to work
over the (now overscan-widened) virtualized row set (memo keys are downstream of materialization).

## Applicability of cross-cutting principles

- **Principle IV (Elmish/MVU boundary)** — N/A: `Update`, effects, subscriptions, commands, and the
  interpreter are unchanged. Offscreen focus/selection reuses the existing `DataGridMsg` set
  (`ScrollRowsTo`/`SelectRow`/`ToggleRow`/`FocusCell`) over logical keys/indices; dispatch outcomes for
  materialized rows are byte-identical (FR-016).
- **Interactive-UI run-and-use gate** — N/A: the feature delivers an internal virtualization contract +
  deterministic metrics observable via `ControlsElmish.Perf.runScript`, plus offscreen addressability /
  a11y totals on the logical model — not a new interactive surface. No new persistent/graphical entry
  point is introduced; the existing `runInteractiveApp` window-launch contract is unchanged.
