# Implementation Plan: OpenGL Present Backend (Direct GPU Rendering)

**Branch**: `119-opengl-present-backend` | **Date**: 2026-06-13 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/119-opengl-present-backend/spec.md`

## Summary

Replace the Vulkan live-present host backend in `FS.Skia.UI.SkiaViewer` with an **OpenGL**
backend so the interactive host draws the existing `SKCanvas`-based scene straight onto the
window's default framebuffer (FBO 0) and presents it via the windowing toolkit's `SwapBuffers`
— **zero per-frame GPU→CPU readback, no staging buffer, no per-frame command pool, no
`vkQueueWaitIdle` stall**. This is the resolution feature 118 named and deferred
(`specs/118-backend-host-review/readiness/audit/opengl-backend-resolution.md`): SkiaSharp's GL
interop is complete (`GRGlInterface` / `GRGlFramebufferInfo` /
`GRBackendRenderTarget.GetGlFramebufferInfo` / `SKSurface.Create` over FBO 0), so the exact
operation that returns `null` on Vulkan (#1502) succeeds on GL. It unblocks feature 118
FR-002/SC-002.

**Technical approach**: the scene renderer (`SceneRenderer.fs`) and the high-level consumer
entry points (`runInteractiveApp` / `runInteractiveViewer` / `ViewerOptions`) stay source-stable;
only the present/host backend beneath them is swapped. The breaking surface is confined to
`Host/Vulkan.fsi`'s three public modules (`VulkanResources` / `VulkanStartup` / `VulkanHost`),
which are replaced by a GL host surface, plus the Vulkan/Swapchain cases in the public
`ViewerDiagnosticCategory` / `ViewerRunBlockedStage` DUs, which are reconciled to GL. The
`ViewerPresentMode` DU is **retained**, its case docs re-mapped to GL semantics (FR-007).
Dependency manifest swaps `Silk.NET.Vulkan` + `.Extensions.KHR` → `Silk.NET.OpenGL`. Governance
tokens that hard-code "Vulkan" (the single-sourced `runtime-limitations.md` rule in
`EvidenceFormatSchema.readinessContractChecks`, the generated `runtime-limitations.md` seed in
`GeneratedProduct.fs`, the constitution constraint in `GovernedBlocks.fs`) are updated to GL
(FR-010), and the constitution itself is amended via `/speckit-constitution` (FR-011).

This is a **Tier 1 (contracted change)**: breaking public `.fsi`, new/removed dependency,
constitution amendment.

## Technical Context

**Language/Version**: F# / .NET 10 (`net10.0`)
**Primary Dependencies**: SkiaSharp `4.147.0-preview.3.1` (GL interop, unchanged version);
**add** `Silk.NET.OpenGL` (pin to the existing Silk.NET `2.23.0` train), `Silk.NET.Windowing`
+ `.Windowing.Extensions` + `Silk.NET.Input` (retained); **remove** `Silk.NET.Vulkan` +
`Silk.NET.Vulkan.Extensions.KHR`.
**Testing**: Expecto + FsCheck (host/diagnostic unit + property tests), FAKE targets
(`Dev`, `DependencyReport`, `TemplateCheck`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`,
`PackageSurfaceCheck`/`PerPackageSurfaceDiff`, `EvidenceGraph`, `EvidenceAudit`), FSI prelude
transcripts, and **real live-host launch evidence on a GPU-passthrough machine** (the
zero-readback present proof, the sample-smoke captures, and the unsupported-GL classified
diagnostic).
**Target Platform**: Windows + Linux desktop with GPU passthrough (Mesa/vendor GL; Wayland via
EGL). macOS/mobile/browser remain out of scope (GL deprecation on macOS is moot).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Initial evaluation (pre-Phase 0)**: PASS with one constitution dependency. The feature is
Tier 1 and follows the Spec → FSI → Semantic Tests → Implement order (Principle I): the GL host
surface is drafted as a `.fsi` (Phase 1 contract) and exercised via an FSI prelude before the
`.fs` body. Visibility lives in the `.fsi` (Principle II); no access modifiers in `.fs`. The
host loop is an I/O-bearing workflow already modeled through the existing Elmish/MVU viewer edge
(`Host/Viewer.fs`) — the backend swap preserves that boundary (Principle IV); `update` stays
pure, GL context/draw/swap is the edge interpreter. Observability and safe failure (Principle
VII) are first-class here: FR-005 requires a classified GL-unavailable diagnostic. **The one
dependency**: Principle constraints and the "Vulkan smoke" clause currently *mandate the Vulkan
backend* (constitution `Project-specific constraints`); this plan requires the constitution
amendment (FR-011) and must not be declared merge-ready until that amendment lands — it is an
explicit, scoped obligation, not a violation. Re-check after Phase 1: no new violations
expected; the design removes complexity (a ~1,800-line Vulkan host → a much smaller GL host) and
introduces no SRTP/reflection/type-providers/custom-operators requiring justification (Principle
III).

### Repository Governance Decisions

- **Template ownership**: The `dotnet new fs-skia-ui` template pin is **deferred to a separate
  follow-up track** (`/fs-skia-template-update`), per spec Package-impact — *not* in this
  feature's merge scope. `TemplateCheck` / `GeneratedProductCheck` are expected to show the
  known pin-lag failure pre-merge (the template still points at the prior package), resolved by
  the follow-up after the bumped libs are packed. No `.template.config/template.json` change in
  this feature beyond what the follow-up handles. Template **fragment** capability skills are
  unaffected (no Samples/Controls capability change).
- **Dependency impact**: **Required.** `Directory.Packages.props` drops the two
  `Silk.NET.Vulkan*` `PackageVersion` entries and adds `Silk.NET.OpenGL` (version `2.23.0`, the
  existing Silk.NET train); `src/SkiaViewer/SkiaViewer.fsproj` swaps its two Vulkan
  `PackageReference`s for `Silk.NET.OpenGL`. The two feature-085/086 readiness harness
  `.fsproj`s that reference `Silk.NET.Vulkan` are historical/archived harnesses — they are
  **not** rebuilt by the routed gates and are left untouched (call out in research if
  `DependencyReport` flags them). `docs/reports/dependencies.md` and any `docs/dependencies*`
  regenerate via `DependencyReport`. `scripts/dependency-report.fsx` is generic (CPM
  conformance) and needs no hand-edit unless it carries an explicit Silk.NET expected-ref list
  (confirmed in Phase 0: it does not).
- **Command-surface impact**: No new build target or wrapper. The routed gate set escalates
  (breaking `.fsi` + dependency + governance paths). FAKE-backed targets run **sequentially**
  in the deterministic order (CLAUDE.md): `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` →
  `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`, plus `DependencyReport`,
  `PackageSurfaceCheck` / `PerPackageSurfaceDiff`, and `RefreshSurfaceBaselines` (regenerates
  top-level + per-package baselines for the changed surface). Authoritative list comes from
  `./fake.sh build -t Route`; run only the gates it prints.
- **Generated project impact**: **Required (governance-token only).** The generated
  `runtime-limitations.md` seed in `build/Governance/GeneratedProduct.fs:970` changes
  "Vulkan backend required" → "OpenGL backend required"; this propagates into every freshly
  generated product's readiness, checked by `GeneratedProductCheck`. No change to default/minimal
  generated *contents*, selected Controls guidance, local skills, placeholder/excluded-history
  scans, or generated `Dev` behavior — generated apps consume only the source-stable high-level
  entry points, which are unchanged.
- **Evidence paths**: Real evidence under `specs/119-opengl-present-backend/readiness/`:
  - `runtime-limitations.md` (GL tokens: `.NET 10 desktop`, `OpenGL`, `SkiaSharp preview`,
    `unsupported macOS/mobile/browser`, `no software-renderer fallback`)
  - `supported-host-persistent-launch.txt` (live persistent-window launch on GPU-passthrough)
  - `readiness/smoke/zero-readback-present.md` (present-path proof: direct mode does **zero**
    per-frame readback — instrumentation/diagnostic counts, not timing gates)
  - `readiness/sample-smoke/*` (controls/charts/datagrid captures matching baselines)
  - `readiness/smoke/unsupported-gl-diagnostic.md` (classified benign vs. blocking GL-missing
    capture)
  - `package-surfaces/*` (per-package surface diff for the breaking `SkiaViewer` change)
  - `evidence-audit.md` (verdict=PASS, 0 synthetic), `generated-validation.md`
    (package-resolution=resolved, package-mismatch=false)
  - `aggregate-hang-diagnostics.md`, `governance-risk-levels.md` (routed readiness-contract files)
  - `migration.md` (FR-009 named removed/renamed members → GL replacements)
- **`.fsi` / contract impact**: **Breaking.** `Host/Vulkan.fsi` (the three public modules) is
  replaced by the GL host `.fsi`; `ViewerDiagnosticCategory.Vulkan`/`Swapchain` and
  `ViewerRunBlockedStage.Swapchain` cases in `SkiaViewer.fsi` are reconciled to GL-meaningful
  cases. `PresentMode.fsi` (`ViewerPresentMode`) is **retained**, doc-comments re-mapped to GL
  (FR-007). High-level entry points (`runInteractiveApp`/`runInteractiveViewer`/`ViewerOptions`)
  stay source-stable (SC-005). Surface baselines regenerate via `RefreshSurfaceBaselines`;
  `migration.md` documents every removed/renamed member (Principle: public API changes document
  migration guidance).
- **MVU/effect boundary**: Reuses the existing viewer Elmish edge. `Model` =
  `ViewerModel`/program state (unchanged); `Msg` = viewer messages incl. injected `Tick`
  (unchanged); `Effect`/`Cmd` = `ViewerEffect` (unchanged); `init`/`update` pure (unchanged).
  The **interpreter edge** is the only thing that changes: the GL host owns the context, the
  FBO-0-bound `SKSurface`, the per-frame draw + `SwapBuffers`, and resize/context-loss handling
  — replacing the Vulkan interpreter body. Pure-transition tests are unaffected; new evidence
  is interpreter-level (real GL launch).
- **Synthetic evidence**: Target **0 synthetic**. The unsupported-GL diagnostic path (FR-005)
  is validated against a **real** GL-unavailable environment where feasible (headless/no-GPU
  CI shell); if a specific malformed-driver case cannot be reproduced on real hardware it is a
  candidate for `[SEH]` (`synthetic-error-handling-approved`) with a design-source row, not a
  silent `[S]`. No mocks/fakes/canned responses on the primary present path — the zero-readback
  proof and sample-smoke run on real GPU-passthrough hardware.
- **Test evidence**: Failing-first semantic tests: a `Feature119` test asserting the GL host's
  present-mode mapping and diagnostic classification (red before the backend exists). Governance
  tests updated for the token churn (`tests/Governance.Tests/*` that assert "Vulkan" tokens →
  GL). Host smoke = real live launch (persistent-launch + zero-readback + sample-smoke). FSI
  prelude transcript exercises the GL host surface through the packed library. Per-target
  evidence per the Evidence-paths list.
- **Observability**: FR-005 classified diagnostic distinguishes environment-limitation
  (`UnsupportedEnvironment`, benign) from implementation defect (blocking), reusing the existing
  benign/blocking host-warning classification (`fs-skia-evidence-mode`). Diagnostics change from
  Vulkan-specific to GL-specific category/stage. Missing-artifact-class failures and the
  GL-unavailable message are honest (never false success, never unclassified crash — Principle
  VII). Log/report paths under `readiness/` per Evidence-paths.
- **Deferred scope**: Out of scope and deferred: the template re-pin
  (`/fs-skia-template-update` follow-up), macOS/mobile/browser/WebGL/ANGLE/multi-backend, any
  new scene capability, distribution/release changes, visual redesign, render-thread/compositor
  split, and **any timing-based pass/fail gate** (backend timing is a human/diagnostic signal
  only; deterministic gating stays on counts and booleans, per 118 §6). The constitution
  amendment (FR-011) is in-scope for this feature but executed via `/speckit-constitution`.

## Project Structure

Real paths touched by this feature (repo root `/home/developer/projects/FS-Skia-UI`):

```
src/SkiaViewer/
  SkiaViewer.fsproj            # swap Silk.NET.Vulkan* PackageReference → Silk.NET.OpenGL; bump <Version>
  PresentMode.fsi / .fs        # RETAIN ViewerPresentMode; re-map case docs to GL (FR-007)
  Host/
    Vulkan.fsi / .fs           # REPLACE → GL host (rename file to Host/OpenGl.fs[i] or Host/GlHost):
                               #   GL successor to VulkanResources/VulkanStartup/VulkanHost
    Diagnostics.fsi / .fs      # Vulkan-specific stages/prose → GL
    Viewer.fsi / .fs           # route to GL host backend instead of VulkanHost.run
  SkiaViewer.fsi / .fs         # reconcile ViewerDiagnosticCategory.Vulkan/Swapchain,
                               #   ViewerRunBlockedStage.Swapchain → GL; entry points source-stable
  SceneRenderer.fs             # UNCHANGED (backend-agnostic SKCanvas renderer)

Directory.Packages.props       # drop Silk.NET.Vulkan + .Extensions.KHR; add Silk.NET.OpenGL
build/Governance/
  Evidence/EvidenceFormatSchema.fs  # readinessContractChecks: "Vulkan" → "OpenGL" (single source)
  GeneratedProduct.fs:970           # generated runtime-limitations.md seed: Vulkan → OpenGL
  GovernedBlocks.fs:267             # constitution constraint fragment: Vulkan smoke → OpenGL
  README.md                         # "Vulkan + SkiaSharp" → "OpenGL + SkiaSharp"
  FS.Skia.UI.Build.fsproj           # bump <Version> (12th packable lib)

.specify/memory/constitution.md     # amended via /speckit-constitution (FR-011)
docs/{adr,architecture,reports}/    # Vulkan→GL prose churn (FR-010), incl ADR-0007 host-ownership
.agents/skills/fs-skia-viewer-host/SKILL.md  # + generated .claude peer via RefreshSurfaceBaselines
tests/Governance.Tests/*            # update tests asserting Vulkan tokens
specs/119-opengl-present-backend/
  plan.md research.md data-model.md quickstart.md contracts/  # this planning output
  readiness/                        # real evidence (see Evidence paths)
```

All 12 packable libs share `<Version>` (currently `0.1.125-preview.1`) and bump together on
merge; the breaking surface change warrants the bump (FR-009). Reference paths above are
project-relative; filesystem operations during implementation use absolute paths.
