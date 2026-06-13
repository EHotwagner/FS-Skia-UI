# Governance risk levels (feature 119)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI.SkiaViewer (breaking: Host/Vulkan.fsi → Host/OpenGl.fsi replacement — GlResources/GlStartup/GlHost; reconciled ViewerDiagnosticCategory OpenGl/Framebuffer + ViewerRunBlockedStage GlContext; retained-but-re-documented ViewerPresentMode; host DiagnosticStage GL stages); FS.Skia.UI.Build (governance tokens Vulkan→OpenGL); Directory.Packages.props dependency swap (Silk.NET.Vulkan* → Silk.NET.OpenGL)
public-api-impact=breaking .fsi surface change confined to the SkiaViewer host modules + diagnostic DUs; high-level entry points (runInteractiveApp / runInteractiveViewer / ViewerOptions / Viewer.runApp) source-stable (SC-005); ViewerPresentMode DU retained
mvu-applicability=no change to Elmish update/command/effect/subscription semantics; only the interpreter-edge present mechanism (Vulkan → OpenGL) changed; viewer Model/Msg/Effect/init/pure update unchanged
route-tier=agent-ready (Route printed: Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit)
constitution=amended (FR-011): the Vulkan-backend mandate and "Vulkan smoke" clause replaced with OpenGL in .specify/memory/constitution.md and build/Governance/GovernedBlocks.fs

## Risk classification

- **small** — a framework-internal host-body edit with no `.fsi` delta: focused `Dev` only.
- **medium** — a public `SkiaViewer.fsi` surface change: the routed package-surface gate set.
- **broad** — governance/contract-home + constitution + dependency edits: **broad validation**.

THIS feature is **broad** — it changes the public host `.fsi`, the dependency manifest, governance
tokens (FR-010), and the constitution (FR-011). The full routed gate set Route printed is run.

## Authoritative gate list (Route, run sequentially — shared `.fake` state, never concurrent)

`./fake.sh build -t Route` was run against the real diff. Route printed:
`Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedGuidanceCheck,
TemplateDrift, EvidenceGraph, EvidenceAudit`
(matched-rules: evidence-governance, specify-catchall, docs-only, package-surface). Only those
gates are run, sequentially. Non-authoritative aggregate results are advisory only
(`aggregate-hang-diagnostics.md`); the authoritative verdict is the focused per-target rerun.

## Required evidence per risk level

- **required evidence** (broad / `SkiaViewer.fsi` + dependency + governance + constitution):
  regenerated per-package + top-level surface baselines (`RefreshSurfaceBaselines`); the breaking
  delta shown by `PerPackageSurfaceDiff` (intended SkiaViewer change, nothing else); `FsiTranscripts`
  exercising the GL host surface; `DependencyReport` shows Silk.NET.Vulkan* gone / Silk.NET.OpenGL
  present; `GeneratedGuidanceCheck` / generated `runtime-limitations.md` = OpenGL; constitution
  amended.
- **required evidence** (US1): `Feature119OpenGlHostTests` + live launch + zero-readback proof
  (`supported-host-persistent-launch.txt`, `smoke/zero-readback-present.md`).
- **required evidence** (US2): visual + interaction parity (`real-image-evidence.md`,
  `visual-evidence-honesty.md`, `window-visibility.md`; 73 SkiaViewer.Tests green).
- **required evidence** (US3): classified GL-unavailable diagnostic (`smoke/unsupported-gl-diagnostic.md`;
  unit-verified — live unavailable-GL not reproducible on this Mesa host, documented).
- **required evidence** (US4): `migration.md` naming every removed/renamed member.
- **required evidence** (merge gate): `EvidenceGraph` + `EvidenceAudit verdict=PASS` (0 synthetic).
