# Governance risk levels (T003)

This feature (080-control-render-fidelity) is **Tier 1 (contracted, escalated
`maintainer-verify`)**. Affected packages: `FS.Skia.UI.Controls`,
`FS.Skia.UI.SkiaViewer`, `FS.Skia.UI.Build`, `FS.Skia.UI.Scene`. Public-API
impact: **no public `.fsi` delta expected** (charts reuse existing `Scene`
primitives; `Control.render` signature is stable). Principle IV (MVU) is **N/A**:
pure `Control -> Scene` rendering plus a pure decode-and-assert gate; no
`Model`/`Msg`/`Effect`/interpreter is introduced.

Each change is matched to a risk level with its focused validation. The level
sets which gates are **required evidence**; only `broad` triggers the full
escalated serialized order.

## small

Renderer/extraction internals (`src/Controls/Control.fs` `chartValues`/`renderNode`,
`src/SkiaViewer/SceneRenderer.fs`). Focused validation: `./fake.sh build -t Dev`
plus the render-capable harness suite
(`dotnet run --project tests/ControlsPreview.Harness -- --sequenced`).

## medium

Harness fidelity gate + retained fixtures + per-control sample data/signatures
(`tests/ControlsPreview.Harness/**`). Focused validation: `ControlFidelityCheck`
(render-capable) plus the harness `--sequenced` suite. Required evidence:
`readiness/control-fidelity.md` (decoded-content report + fixture matrix).

## broad

New build target + routing rule + regenerated `validation.contract.yml` +
preview-asset regeneration (`docs/img/controls/**`, `docs/controls/**`). This
registers a contract surface (FR-012), so **broad validation** is required: the
escalated serialized order `Route --enforce` → `Dev` → `ControlFidelityCheck` →
`GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
`EvidenceGraph` → `EvidenceAudit`. `GeneratedProductCheck` is a known
non-authoritative local environment failure (see `aggregate-hang-diagnostics.md`).
Required evidence for the broad level: regenerated `validation.contract.yml`
(via `RefreshSurfaceBaselines`, currency-enforced by `TargetMetadataDrift`),
`readiness/control-fidelity.md`, `readiness/real-image-evidence.md`,
`readiness/usage-coherence.md`.
