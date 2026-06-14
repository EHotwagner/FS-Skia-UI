# Governance risk levels (feature 122)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.SkiaViewer (present-path buffer-fill in `GlHost.renderFrame` + exposed pure `GlHost.planPresent`/`PresentAction` test seam) + FS.Skia.UI.Controls.Elmish (additive public `runInteractiveAppWithWindowBehavior` overload) + FS.Skia.UI.Controls (internal `CustomControl` null-guard; regenerated catalog description) + template/governance docs + skill tree
public-api-impact=additive `ControlsElmish.runInteractiveAppWithWindowBehavior` + `GlHost.PresentAction`/`planPresent` (test seam, attr→doc→type); `runInteractiveApp` unchanged; CustomControl `.fsi` unchanged
mvu-applicability=no change (Update/effects/subscriptions/interpreter untouched; present-path + window-behavior threading are host-loop/launch concerns; `planPresent` is pure)
route-tier=agent-ready (controls-public-surface + generated-template)

## Risk classification

- **small** — a test-only or doc-only edit that touches no `.fsi`.
- **medium** — the additive `.fsi` members + the present-path internals: `RefreshSurfaceBaselines` +
  `Dev` + the per-package surface diff.
- **broad** — the full escalated set Route prints, required because public `.fsi` surface changes
  (`Controls.Elmish` overload + `SkiaViewer.Host` test seam), the template `Program.fs`/docs change, and
  the skill tree changes. **broad validation** is mandatory before merge. Non-authoritative aggregate
  results are advisory only in [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the
  authoritative verdict is the focused per-target rerun.

THIS feature is **broad** (public `.fsi` additions + template + skill-tree changes).

## Authoritative gate list (Route, run sequentially — shared `.fake` state, never concurrent)

`./fake.sh build -t Route` was run against the real diff and escalates to **agent-ready**
(controls-public-surface + generated-template + evidence-governance + generated-guidance +
controls-catalog-docs + package-surface + skill-quality). Route printed: `Dev, PackageSurfaceCheck,
PerPackageSurfaceDiff, FsiTranscripts, TemplateCheck, GeneratedProductCheck, ControlsCatalogCheck,
ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsCatalogDocsCheck,
ControlsDocCoverageCheck, ControlFidelityCheck, ControlsInteractionCheck, ControlsRenderingCheck,
GeneratedGuidanceCheck, SkillSyncCheck, SkillQualityCheck, PhaseHookParityCheck, SkillContractPathCheck,
TemplateUpdateSkillPackageCheck, TemplateDrift, EvidenceGraph, EvidenceAudit`.
`RefreshSurfaceBaselines` regenerated the SkiaViewer + Controls.Elmish per-package surfaces, the
typed-catalog block + `docs/controls/*`, `evidence-formats.md`, and the `.claude` skill mirror. All
printed gates were run sequentially; `TemplateCheck` / `GeneratedProductCheck` show the **expected
template-pin-lag** (the new overload is not in the published package), scoped OUT of merge and resolved
by the `/fs-skia-template-update` follow-up after the post-merge version bump (see
[generated-validation.md](./generated-validation.md)). All other printed gates PASS.

## Required evidence per risk level

- **required evidence** (broad / public `.fsi`): recaptured per-package SkiaViewer + Controls.Elmish
  surface baselines + the new members' `///` docs (doc-preservation gate).
- **required evidence** (US1 present path, FR-001/002): `Feature122PresentPathTests` — `planPresent`
  paints on change, re-presents to fill `bufferFillDepth` buffers, then idles; no buffer left undrawn;
  continuous animation never re-presents; a size change forces a repaint.
- **required evidence** (US2 window-behavior threading, FR-003/005): `Feature122PresentPathTests` default
  parity + `Feature122TemplateThreadingTests` (generated `Program.fs` routes a window flag through
  `runInteractiveAppWithWindowBehavior`).
- **required evidence** (US3 CustomControl, FR-006/007): `Feature122CustomControlTests` (null Id/effect →
  diagnostic, no NRE) + the regenerated honest catalog/docs text.
- **required evidence** (byte-identity, FR-002/SC-004): the offscreen/readback path is untouched; the
  standing Scene-parity golden suites under `Dev` carry it.
- **required evidence** (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` (0 synthetic).
