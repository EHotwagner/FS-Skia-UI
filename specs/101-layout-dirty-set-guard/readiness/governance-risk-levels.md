# Governance risk levels — feature 101 (Layout Dirty-Set Anti-Drift Guard, R7, T004)

R7 is a **Tier-2 (internal)** change: it adds an anti-drift guard (pure drift report + behavioral
probe) as ordinary Controls Expecto tests, light name-token single-sourcing of three string literals
in `src/Controls/Control.fs`, and a comment correction in `src/Controls/Control.fs` /
`src/Controls/RetainedRender.fs`. **No `.fsi` signature change** (public or internal) is made — the
report/probe are test-local and the name tokens are `private`.

`./fake.sh build -t Route` was run against the working-tree diff. Because the change touches
`src/Controls/**/*.fs`, Route matches the **controls-public-surface** rule and escalates to
`tier=agent-ready` (this repo treats any `src/Controls` source edit as consumer-contract-bearing — the
same escalation features 096–100 ran). The plan's "inner-loop → Dev only" prediction was optimistic;
**Route is authoritative** and the escalated gate set below is what was run, sequentially (shared
`.fake` state, never concurrently). Note vs the precedent: `PerPackageSurfaceDiff` is **absent** here
because R7 makes **no** `.fsi` edit, so no per-package internal baseline moves (zero surface delta,
SC-005 / T015).

## small

The pure `layoutDriftReport (discovered) (covered) : DriftFinding list` (exact set-difference both
directions) + `formatDrift` (human-legible, names attribute + direction; empty → "no drift") —
totality, order-stable findings, both drift directions.
- required evidence: targeted `Controls.Tests/Feature101*` drift-report units — under-coverage
  (`{w;h;padding}` vs `{w;h}` → `[Uncovered "padding"]`), over-coverage (`{w}` vs `{w;orientation}` →
  `[OverBroad "orientation"]`), both-directions sorted, shipping-state `[]`, and the `formatDrift`
  naming assertions (T007/T009).
- gate: `./fake.sh build -t Dev`.

## medium

The behavioral **probe** over the REAL `ControlInternals.evaluateLayout` (corpus × fixtures discovery,
structural `LayoutNode` comparison via `%A`, union) and the FR-004 category-honoring units asserted
through the EXPOSED `RetainedRender.step` (`WorkReductionRecord.RemeasuredNodeCount`, which the real
`layoutDirtySet` drives).
- required evidence: `Controls.Tests/Feature101*` — the load-bearing gate
  (`layoutDriftReport (discoverLayoutDrivingNames size) layoutAffectingAttrNames = []`), the
  `discovered = {width;height;orientation}` assertion, the non-layout-names exclusion, and the three
  category-channel units (Layout-category dirties; AttrRemoved-of-prev-Layout dirties; Style change
  re-measures nothing) (T007/T008/T009); plus the **unchanged** re-run of
  `Layout.Tests/Feature097IncrementalTests.fs` and `Controls.Tests/Feature097WiringTests.fs`
  (T013/T014).
- gate: `./fake.sh build -t Dev`.

## broad

The `src/Controls/**/*.fs` edit forces the controls-public-surface escalation even though **no** `.fsi`
or consumer-observable behavior changes (byte-identical render; R2 INV-1 preserved). The public
`runInteractiveApp` / `InteractiveAppHost` surface and the per-package internal `.fsi.txt` baseline are
**unchanged**.
- required evidence: the per-package `FS.Skia.UI.Controls` internal `.fsi.txt` baseline shows **no**
  drift vs the pre-change reference (`surface-baseline.md`, T006/T015), plus `EvidenceGraph` +
  `EvidenceAudit verdict=PASS` with **no** synthetic work.
- broad validation: the `Route`-printed escalated controls-public-surface set, run **sequentially**
  (shared `.fake` state) in deterministic order; aggregate results are recorded as
  **non-authoritative** unless re-confirmed sequentially (see `aggregate-hang-diagnostics.md`).

authoritative-gate-list=Dev, PackageSurfaceCheck, FsiTranscripts, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
