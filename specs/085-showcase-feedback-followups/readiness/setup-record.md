# Setup record (Phase 1: T001, T003, T004)

## T001 — feature directory + classification

- **Feature**: `085-showcase-feedback-followups`
- **Branch**: `085-showcase-feedback-followups` (current)
- **Spec**: [spec.md](../spec.md) · **Plan**: [plan.md](../plan.md)
- **Classification**: **Tier 1 — escalated `maintainer-verify`**, triggered by the
  *implementation* diff (new public `src/Controls/Control.fsi` +
  `src/SkiaViewer/SkiaViewer.fsi` surface, `template/**` doc edits, new
  `.agents/skills/fs-skia-viewer-host`, governance-template edits). The spec-only
  baseline routes lower (see T004) and escalates once the contract-bearing edits land.

## T003 — affected layer, API impact, MVU applicability, evidence obligations

- **Affected layers / packages**: `FS.Skia.UI.Controls` (`Control.renderTree`),
  `FS.Skia.UI.SkiaViewer` (`InteractiveAppHost` + `Viewer.runInteractiveApp`,
  size-aware `View`), `FS.Skia.UI.KeyboardInput` (`normalize` behavior). Reuses
  `FS.Skia.UI.Controls.Elmish` pointer pipeline; no new dependency.
- **Additive public-API impact**: `Control.renderTree` (FR-001),
  `InteractiveAppHost` + `Viewer.runInteractiveApp` (FR-004); `Control.render` /
  `Widget.render` and `GeneratedAppHost` / `Viewer.runApp` preserved byte-for-byte
  (FR-003/FR-006); size-aware `View: Size -> 'model -> SceneNode` (FR-009);
  `KeyboardInput.fsi` / `ViewerKey` unchanged.
- **MVU/effect applicability**: **US2 is I/O-bearing** (pointer input + host loop):
  `Update` pure, pointer events become `PointerInteraction` data before `MapPointer`,
  interpreter edge = `runInteractiveApp`. US1/US3/US4 are pure (Principle IV N/A).
- **Evidence obligations** (detail in `governance-risk-levels.md`): SC-001
  render-distinctness PNGs + diff (`evidence/render-distinctness/`); SC-002
  pointer-dispatch (`evidence/pointer-dispatch.md`) + durable visible-window class;
  SC-003 `evidence/normalize-mapping.md`; SC-004 `evidence/size-aware-render/` +
  blur-workaround note; surface baselines + per-package `.fsi.txt`;
  `RefreshSurfaceBaselines` log; `EvidenceGraph`/`EvidenceAudit` output.

## T004 — baseline Route (spec-only working tree)

`./fake.sh build -t Route` on the pre-implementation diff:

```
developer-class=framework-author
tier=agent-ready
gates=Dev, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
dogfood-forced=false
matched-rules=evidence-governance, specify-catchall, docs-only
```

This is the expected pre-edit baseline (no `src/**/*.fsi`, `template/**`, or new
`.agents/skills/**` touched yet). T033 re-runs `Route` after the contract-bearing
edits and confirms escalation to `maintainer-verify`.
