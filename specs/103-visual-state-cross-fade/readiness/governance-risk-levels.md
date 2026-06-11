# Governance risk levels — feature 103 (True Visual-State Cross-Fade, R6, T018)

R6 is a **Tier-1 (contracted)** change: it alters observable behavior (mid-transition paint becomes a
genuine cross-fade) and touches an internal-but-baselined `.fsi` (the internal `AnimationClock` gains
a `From` prior-snapshot field; its doc is reconciled, FR-009). The **public**
`runInteractiveApp`/consumer surface is **unchanged** — only the internal `RetainedRender` `.fsi`
moves, so the per-package internal baseline recaptures while the cross-package public baseline does not.

`./fake.sh build -t Route` was run against the working-tree diff (see
[generated-validation.md](./generated-validation.md) for the printed tier + minimal gate list). Because
the change edits `src/Controls/RetainedRender.fs(i)`, Route escalates to the `controls-public-surface`
gate set — the same escalation features 096–102 ran — even though the public consumer surface is
unchanged (the feature-101 rule: ANY `src/Controls/**/*.fs` edit escalates). The escalated set is run
**sequentially** (shared `.fake` state, never concurrently).

## small

The `RetainedRender.fsi` internal doc-comment reconciliation (FR-009) — the `AnimationClock` doc drops
the unfulfilled standalone Scene-`Color`-tween claim and describes the snapshot-composite cross-fade.
- required evidence: the reconciled doc names exactly the channels the implementation drives (the
  opacity tween + the two-snapshot composite); confirmed by T015 against `RetainedRender.fsi`.
- gate: doc-only; subsumed by the escalated set below.

## medium

The `RetainedRender.fs` behavior change — `updateClockForState` captures the prior own-scene snapshot
as the clock's `From`, `sampleOnPaint` composites the prior layer (fade-out) under the next (fade-in),
and the assemble walk threads the prior snapshot by `RetainedId` — plus the internal `AnimationClock`
`From` field.
- required evidence: the mid-flight strictly-between proof ([mid-flight-interpolation.md](./mid-flight-interpolation.md),
  SC-001), the at-rest / final-frame byte-identity proofs ([at-rest-byte-identity.md](./at-rest-byte-identity.md),
  [final-frame-identity.md](./final-frame-identity.md), SC-002/SC-003), [determinism.md](./determinism.md)
  (SC-004), and the held-state single-scoped-repaint + return-to-Normal-drop edge tests (SC-006). The
  Controls + Elmish suites and the 099/101 property + unit suites stay **green**.
- gate: `./fake.sh build -t Dev` (escalated as below).

## broad

The `src/Controls/**/*.fs` edit forces the controls-public-surface escalation. The **public** surface
baseline is unchanged; the **per-package** internal `FS.Skia.UI.Controls.fsi.txt` baseline moves
(the `AnimationClock` `From` field + reconciled doc) and is recaptured via
`PerPackageSurface.captureCurrent` (T016) — `RefreshSurfaceBaselines` does **not** regenerate
per-package snapshots.
- required evidence: `PackageSurfaceCheck` shows only the expected internal per-package move (no
  **public** surface drift), plus `EvidenceGraph` + `EvidenceAudit verdict=PASS` with **0 synthetic**.
- broad validation: the `Route`-printed escalated controls-public-surface set, run **sequentially**
  (shared `.fake` state) in deterministic order; aggregate results are recorded as
  **non-authoritative** unless re-confirmed sequentially (see [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md)).

authoritative-gate-list=Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
