# Governance risk levels (feature 118)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.SkiaViewer (public ViewerPresentMode DU + ViewerOptions.PresentMode field; internal ViewerConfiguration.PresentMode threading; Host/Vulkan.fs direct-present seam with safe fallback; FR-007 Stage→Category diagnostic mapping; FR-004 on-demand capture decoupling)
public-api-impact=additive public surface: new [<RequireQualifiedAccess>] ViewerPresentMode + ViewerOptions.PresentMode field (breaking record-shape — every construction site updated); no other package surface changes; no FrameMetrics field (FR-008)
mvu-applicability=no change (PresentMode is configuration carried in ViewerModel.Options; no new ViewerMsg/ViewerEffect, no Viewer.update change; the present-mechanism switch + safe fallback live in the backend interpreter edge)
route-tier=agent-ready (package-surface)

## Risk classification

- **small** — a framework-internal `Vulkan.fs` edit with no `.fsi` delta: focused `Dev` only.
- **medium** — this feature's public `SkiaViewer.fsi` surface change: the routed package-surface
  gate set Route prints, plus `TemplateCheck` / `GeneratedProductCheck` because the template and
  generated product construct `ViewerOptions`.
- **broad** — governance/contract-home edits: the full serialized six-target order. Not required
  here; Route did not escalate that far.

THIS feature is **medium** (a public `SkiaViewer.fsi` surface change + template/generated
construction-site churn).

## Authoritative gate list (Route, run sequentially — shared `.fake` state, never concurrent)

`./fake.sh build -t Route` was run against the real diff. Route printed:
`Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, TemplateCheck,
GeneratedProductCheck, ControlsCatalogDocsCheck, ControlFidelityCheck, GeneratedGuidanceCheck,
SkillContractPathCheck, TemplateDrift, EvidenceGraph, EvidenceAudit`
(matched-rules: generated-template, evidence-governance, specify-catchall, docs-only,
controls-catalog-docs, package-surface). Only those gates are run, sequentially. Non-authoritative
aggregate results are advisory only (see aggregate-hang-diagnostics.md); the authoritative
verdict is the focused per-target rerun. **broad validation** (the full six-target order) is NOT
required for this tier.

## Required evidence per risk level

- **required evidence** (medium / `SkiaViewer.fsi`): recaptured per-package + top-level surface
  baselines (new `ViewerPresentMode` type) + the new public field/type `///` docs (XML-doc gate);
  `FsiTranscripts` exercising the new surface; `TemplateCheck` + `GeneratedProductCheck` green
  with the defaulted `PresentMode` construction sites.
- **required evidence** (US1): `Feature118PresentModeTests` (default = OffscreenReadback, config
  threading, distinct modes) + live smoke (`smoke/direct-mode-smoke.md`,
  `default-byte-identity.md`, `safe-fallback.md`) + byte-identical captures.
- **required evidence** (US2): the FR-007 category mapping + live diagnostic capture
  (`us2-validation.md`); `FrameMetrics` goldens unchanged (FR-008).
- **required evidence** (US3): `audit/present-path-audit.md` + `audit/hosting-mode-tradeoffs.md`
  + `audit/opengl-backend-resolution.md` (`us3-validation.md`).
- **required evidence** (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` (0 synthetic).
