# Governance risk levels — feature 099 (animation clock on retained identity, R4, T004)

This Tier-1 change generalizes the **internal** carried slot `RetainedUiState.Animation` in
`src/Controls/RetainedRender.fsi` (from `AnimationState<Transform> option` to the feature-073
multi-channel `AnimationClock option`), so `./fake.sh build -t Route` escalates it to the
**controls-public-surface** rule. Run `Route` first and run exactly the gates it prints
(`tier=agent-ready`; matched-rules include `controls-public-surface`).

## small

The pure clock core in `RetainedRender.fs` — `advance` / `updateClockForState` / `sampleOnPaint` /
`clockActive` (totality, non-positive no-op, settled-end clamp, settled-`Normal` drop, retarget from
the current sampled value, determinism).
- required evidence: targeted `Controls.Tests` — the FsCheck determinism property (≥1000 cases),
  the delta/trigger edge cases, and identity-at-rest byte-identity / zero-recompute (T013/T014/T015).
- gate: `./fake.sh build -t Dev`.

## medium

The host seam wiring in `runInteractiveApp` (wrap `Tick` → advance before render; sample on paint;
retarget from the stamped `VisualState`) and the survival / GC / scoped-repaint behaviors through the
live adapter.
- required evidence: `Elmish.Tests` — animates-vs-snaps (T008/T009), seam-driven survival (T011),
  removed-identity GC (T017), scoped-repaint (T019), driven through `RetainedRender.advance`/`step`
  with `ControlRuntime.applyRuntimeVisualState`.
- gate: `./fake.sh build -t Dev` (Elmish.Tests) + the captured `us1-animates-vs-snaps.md` /
  `us2-survival.md` / `us4-gc.md` / `scoped-repaint.md`.

## broad

The internal `src/Controls/**/*.fsi` slot-type change (controls-public-surface escalation). The
**public** `runInteractiveApp` / `InteractiveAppHost` surface is **unchanged**.
- required evidence: recaptured surface baselines (controls-public-surface api-surface + per-package
  `.fsi.txt`) showing exactly the internal slot-type generalization and the `internal` helper
  signatures, with no public-surface drift.
- broad validation: the `Route`-printed escalated controls-public-surface set, run **sequentially**
  (shared `.fake` state) in deterministic order; aggregate results are recorded as
  **non-authoritative** unless re-confirmed sequentially (see `aggregate-hang-diagnostics.md`).

authoritative-gate-list=Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
