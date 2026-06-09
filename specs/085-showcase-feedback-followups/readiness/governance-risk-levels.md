# Governance risk levels (T003)

This feature (085-showcase-feedback-followups) is **Tier 1 (escalated
`maintainer-verify`)**, triggered by the implementation diff (new public
`src/Controls/Control.fsi` + `src/SkiaViewer/SkiaViewer.fsi` surface,
`template/**` doc edits, new `.agents/skills/fs-skia-viewer-host`, governance
templates). Affected packages: `FS.Skia.UI.Controls`, `FS.Skia.UI.SkiaViewer`,
`FS.Skia.UI.KeyboardInput`. Public-API impact: **additive** `Control.renderTree`,
`InteractiveAppHost`, `Viewer.runInteractiveApp` (FR-001/FR-004/FR-006/FR-009);
`KeyboardInput` `.fsi`/`ViewerKey` unchanged (behavior-only `normalize` fix).
Principle IV (MVU): **applicable** to US2 (I/O-bearing pointer input + host loop);
N/A to US1/US3/US4 pure functions.

Each change is matched to a risk level with its focused validation. The level
sets which gates are **required evidence**; only `broad` triggers the full
escalated serialized order.

## small

The `normalize` behavior fix (US3, `src/KeyboardInput/KeyboardInput.fs`) and the
doc/skill edits (US5: `.agents/skills/**`, `template/base/docs/**`,
`.specify/templates/spec-template.md`). Focused validation: the relevant unit
tests (`KeyboardInput.Tests`) + `GeneratedGuidanceCheck` / `SkillSyncCheck` /
`SkillQualityCheck`. Required evidence: `evidence/normalize-mapping.md`,
`readiness/framework-guidance.md`.

## medium

The size-aware `View` wiring (US4, `src/SkiaViewer/SkiaViewer.fs`): host render
tests + `./fake.sh build -t Dev`. Required evidence:
`evidence/size-aware-render/*.png` + the windowed-fullscreen blur workaround note
in `readiness/runtime-limitations.md`.

## broad

New public `.fsi` surface (US1 `Control.renderTree`, US2 `InteractiveAppHost` +
`Viewer.runInteractiveApp`). This registers a contract surface, so
**broad validation** is required: the escalated serialized order
`Route --enforce` → `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` →
`GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`.
`GeneratedProductCheck` is a known **non-authoritative aggregate** local
environment failure (see `aggregate-hang-diagnostics.md`). Required evidence for
the broad level: regenerated surface baselines + per-package `.fsi.txt` (via
`RefreshSurfaceBaselines`, currency-enforced by `TargetMetadataDrift`),
`evidence/render-distinctness/*.png`, `readiness/real-image-evidence.md`,
`readiness/interactive-visible-window.md`, `evidence/pointer-dispatch.md`.
