# Governance risk levels (feature 121)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.SkiaViewer (public `ViewerOptions.FrameRateCap` additive field + `GlHost.shouldAdvanceFrame` extracted pacing decision + `runEventLoop` render-cadence gate) + FS.Skia.UI.Controls (internal `RetainedRender.advanceStateClocks` no-alloc idle-tick seam; published `Controls/Pointer.fsi` api-surface) + FS.Skia.UI.Controls.Elmish (internal `wrappedTick` rewire — no `.fsi` change)
public-api-impact=additive `ViewerOptions.FrameRateCap: int option` (defaulted, byte-identical when unset); new published `docs/api-surface/Controls/Pointer.fsi`; internal RetainedRender/GlHost surface reached via InternalsVisibleTo
mvu-applicability=no change (Update/effects/subscriptions/interpreter untouched; the already-shipped `CloseWindow` quit path is documented, not modified; the native render-cadence gate is host-loop, not MVU)
route-tier=agent-ready (controls-public-surface)

## Risk classification

- **small** — a test-only or doc-only edit that touches no `.fsi`.
- **medium** — the additive `ViewerOptions` field + the api-surface publish: `RefreshSurfaceBaselines` +
  `Dev` + the per-package surface diff.
- **broad** — the full escalated controls-public-surface set Route prints, required because the public
  `SkiaViewer` `ViewerOptions` `.fsi` surface changes and the Controls api-surface tree gains
  `Pointer.fsi`. **broad validation** is mandatory before merge. Non-authoritative aggregate results are
  advisory only in [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the authoritative
  verdict is the focused per-target rerun.

THIS feature is **broad** (a public `SkiaViewer` `.fsi` field addition + an api-surface tree addition).

## Authoritative gate list (Route, run sequentially — shared `.fake` state, never concurrent)

`./fake.sh build -t Route` was run against the real diff and escalates to **controls-public-surface**.
Route printed: `Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, TemplateCheck,
GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift,
ContrastCheck, ControlsCatalogDocsCheck, ControlsDocCoverageCheck, ControlFidelityCheck,
ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, SkillSyncCheck,
SkillQualityCheck, PhaseHookParityCheck, SkillContractPathCheck, TemplateUpdateSkillPackageCheck,
TemplateDrift, EvidenceGraph, EvidenceAudit`. `RefreshSurfaceBaselines` regenerated the per-package
SkiaViewer + Controls surfaces and emitted `Pointer.fsi` into the api-surface tree. All printed gates
were run sequentially; `TemplateCheck` / `GeneratedProductCheck` show the **expected template-pin-lag**
(the new field is not in the published 0.1.127 package), scoped OUT of merge and resolved by the
`/fs-skia-template-update` follow-up after the post-merge version bump (see
[generated-validation.md](./generated-validation.md)). All other printed gates PASS.

## Required evidence per risk level

- **required evidence** (broad / public `ViewerOptions` `.fsi`): recaptured per-package SkiaViewer surface
  baseline + the new field's `///` doc (doc-preservation gate); emitted `Controls/Pointer.fsi` api-surface
  byte-current with source (Feature060/Feature089 currency tests).
- **required evidence** (US1 frame-cap): `Feature121LiveHostPacingTests` — `GlHost.shouldAdvanceFrame`
  gates after the interval, a tighter cap yields strictly fewer advances (FR-002/SC-001), and a
  non-positive cap is rejected at option validation (FR-003/SC-005).
- **required evidence** (US2 idle-tick no-alloc): `Feature121IdleTickTests` —
  `RetainedRender.advanceStateClocks` returns the state reference-equal when no clock is active (SC-003)
  and advances active clocks exactly as the `advance` oracle (099/103 unchanged).
- **required evidence** (US3 surface honesty): the published `docs/api-surface/Controls/Pointer.fsi` +
  the viewer-host skill present-mode/pacing/reconciliation sections + `runtime-limitations.md`.
- **required evidence** (byte-identity, FR-008): the standing Scene-parity golden suite under `Dev` (the
  cadence gate and idle-tick change cost, not pixels).
- **required evidence** (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` (0 synthetic).
</content>
