# Governance risk levels — feature 105 (housekeeping code-quality)

Feature 105 is a **Tier-2 (internal)** behaviour-preserving refactor of `src/**` `.fs`
bodies: de-duplicate the typed-widget lowering helpers (US1), drop redundant in-source
`private` qualifiers the `.fsi` already enforces (US2), and route internal closed-set
identifiers through typed keys / internal DUs with string boundaries (US3). There is **zero
public `.fsi` delta** under the default choices, so no per-package or cross-package baseline
moves. `Route` is authoritative; the `controls-public-surface` set may be selected
empirically even with zero `.fsi` delta (the 101/102 precedent), which is gate selection,
not a surface delta.

## small

A single widget module's helper rewire (e.g. swapping a `view`'s `XLowering.withKeyOpt`
reference to the shared `WidgetLowering.withKeyOpt`).
- required evidence: the file compiles; the Feature 105 parity guard and the per-control
  `TypedLoweringTests` stay green; the diff shows only a reference swap.

## medium

**This feature's level.** The cross-module consolidation + the internal DU introductions
(US1/US3) touching `FS.Skia.UI.Controls` (`Widgets/*.fs`, `Control.fs`, `Reconcile.fs`,
`RetainedRender.fs`, `DataGrid.fs`, new `Widgets/WidgetLowering.fs`), `FS.Skia.UI.Scene`
(`Scene.fs`), and `FS.Skia.UI.SkiaViewer` (`SkiaViewer.fs`) `.fs` bodies.
- required evidence: the gate set `Route` prints, run **sequentially**; the Controls +
  Controls.Elmish suites green with no parity/golden row moves (SC-005); the
  `Feature105ParityTests` parity guard green (SC-006); `git diff -- 'src/**/*.fsi'` empty
  (SC-007); `EvidenceGraph` + `EvidenceAudit` PASS with 0 synthetic.

## broad

Required only if `Route` escalates beyond the controls-public-surface set, or a FAKE-backed
failure looks race-like. Then rerun the affected FAKE-backed commands **sequentially**
before any product-regression claim.
- broad validation: the full `Route`-printed gate set executed sequentially (shared `.fake`
  state, never concurrently) in deterministic order; aggregate-suite results obtained outside
  the routed focused set are recorded as **non-authoritative** (see
  `aggregate-hang-diagnostics.md`) and the per-suite Expecto outcomes are authoritative.
