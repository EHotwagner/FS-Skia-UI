# Governance risk levels — feature 102 (Documented-Narrowing Reconciliation, R8, T020)

R8 is a **Tier-2 (internal/documentation)** change: a pure honesty pass that reconciles roadmap report
prose with the shipped code and adds descriptive in-source comments. It makes **no `.fsi` signature
change** (public or internal), no logic change, and no behavior change — every edit is report prose or a
descriptive comment. There is **zero surface delta** (SC-005), so no per-package or cross-package
baseline moves.

`./fake.sh build -t Route` was run against the working-tree diff and printed `tier=agent-ready`
(matched-rules: `controls-public-surface`, `evidence-governance`, `specify-catchall`, `docs-only`).
Because the change touches `src/Controls/**/*.fs`, Route escalates to the controls-public-surface gate
set — the same escalation features 096–101 ran — even though only comments change and no `.fsi` moves.
The spec's "inner-loop → Dev only" prediction was optimistic; **Route is authoritative** and the
escalated gate set below is what was run, sequentially (shared `.fake` state, never concurrently). Note
vs the precedent: `PackageSurfaceCheck` finds **no** per-package baseline drift here because R8 makes
**no** `.fsi` edit (zero surface delta, SC-005).

## small

The roadmap report prose edits (`docs/reports/…-roadmap.md`: §10.3 two-function visual-state split,
§10.4 shipped-`Bounds`-cache wording, the "segmented" selection-role correction) and the
`src/Layout/Layout.fs` Yoga blast-radius comment.
- required evidence: the report/source diff is itself the reconciliation evidence — each reconciled
  section now matches its cited source (T007–T010, T012). No package surface is touched.
- gate: routes via `docs-only` + inner-loop `Dev`; subsumed by the escalated set below.

## medium

The `src/Controls/**/*.fs` descriptive comments — `ControlRuntime.fs` dead-`Selected` annotation,
`Focus.fs` value-role classed-but-not-routed note, `Control.fs:1131` legacy-080 preview-id note.
- required evidence: each cited site carries an accurate, purely descriptive comment (T011/T013/T014);
  FR-010 verified — no comment is a gate-significant token or literal evidence filename (T015); the
  existing R1/R2/R4/R5 Controls / Elmish / Layout suites stay **green and unchanged** under `Dev`
  (T018), and arrow-key routing for `Chart`/`Graph`/`Progress` is unchanged (T019).
- gate: `./fake.sh build -t Dev` (escalated as below).

## broad

The `src/Controls/**/*.fs` edit forces the controls-public-surface escalation even though **no** `.fsi`
or consumer-observable behavior changes (byte-identical render). The public surface and the per-package
internal `.fsi.txt` baselines are **unchanged**.
- required evidence: `PackageSurfaceCheck` shows **no** drift vs the pre-change baseline, plus
  `EvidenceGraph` + `EvidenceAudit verdict=PASS` with **no** synthetic work.
- broad validation: the `Route`-printed escalated controls-public-surface set, run **sequentially**
  (shared `.fake` state) in deterministic order; aggregate results are recorded as
  **non-authoritative** unless re-confirmed sequentially (see `aggregate-hang-diagnostics.md`).

authoritative-gate-list=Dev, PackageSurfaceCheck, FsiTranscripts, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
