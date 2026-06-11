# Governance risk levels — feature 100 (general navigation-key delivery, R5, T004)

This Tier-1 change moves public `.fsi` surface in three Controls modules (`Focus.fsi` —
`Direction`/`NavIntent`/widened `route`; `Types.fsi` — `NavRange`/`NavPayload`/`ControlEvent.Nav`/
`AccessibilityMetadata.Navigation`; `Accessibility.fsi` — widened `metadata`), so `./fake.sh build -t
Route` escalates it to the **controls-public-surface** rule. Run `Route` first and run exactly the
gates it prints (`tier=agent-ready`; matched-rules include `controls-public-surface`). `Route`
escalates only **after** the `.fsi` edits exist — this record states the **expected** escalation;
T022/T023/T024 verify it on the real diff (`--enforce` for missing evidence).

## small

The pure `Focus.route` -> `NavIntent` classification (the single role-specific branch) and the closed
`NavIntent`/`NavPayload` exhaustiveness — totality, role -> one intent class, value-role-without-range
-> `Fallthrough`, non-navigable role -> `Fallthrough`, Home/End fold to First/Last / Min/Max.
- required evidence: targeted `Controls.Tests/Feature100*` — the per-role route classification
  (selection/value/grid), the no-op fallthrough cases, and the FsCheck `Check.One` closed-set /
  one-to-one `NavIntent`<->`NavPayload` proof (>=1000) (T006/T018).
- gate: `./fake.sh build -t Dev`.

## medium

The host per-intent resolver in `routeFocusedKey` (selected-then-changed binding match, declared-step
value clamp, linear-selection index clamp, grid 2-D clamp, dual-set `Payload`+`Nav`, empty/unset/
boundary no-ops) driven through the live adapter.
- required evidence: `Elmish.Tests/Feature100*` — selection-move (T009/T010), declared-step +
  byte-identical default-step golden (T012/T013), grid 2-D move (T015/T016), boundary-clamp and
  non-navigable-button no-ops, driven through the real `RetainedRender` + `routeFocusedKey` seam, plus
  the captured `responds-vs-renders.md` / `declared-step.md` / `role-coverage.md`.
- gate: `./fake.sh build -t Dev` (Elmish.Tests).

## broad

The `src/Controls/**/*.fsi` surface change (Focus/Types/Accessibility) forces the controls-public-
surface escalation. The **public** `runInteractiveApp` / `InteractiveAppHost` surface is **unchanged**
(the resolver stays module-internal); `Payload : string option` is retained on `ControlEvent`.
- required evidence: recaptured api-surface + per-package `.fsi.txt` baselines for `FS.Skia.UI.Controls`
  showing exactly the `Focus`/`Types`/`Accessibility` surface moves with no other drift
  (`surface-baseline.md`, T020), plus `EvidenceGraph` + `EvidenceAudit verdict=PASS`.
- broad validation: the `Route`-printed escalated controls-public-surface set, run **sequentially**
  (shared `.fake` state) in deterministic order; aggregate results are recorded as
  **non-authoritative** unless re-confirmed sequentially (see `aggregate-hang-diagnostics.md`).

authoritative-gate-list=Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
