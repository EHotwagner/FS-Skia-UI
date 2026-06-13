# Tasks: OpenGL Present Backend (Direct GPU Rendering)

**Feature branch**: `119-opengl-present-backend`
**Spec**: `specs/119-opengl-present-backend/spec.md`
**Plan**: `specs/119-opengl-present-backend/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written. `[SEH]` is a design-approved synthetic
error-handling annotation that remains `[S]` when completed. No `[SEH]` tasks are planned for
this feature — the GL-unavailable path is reproducible on a real headless/no-GPU shell, so its
evidence is real (target: 0 synthetic).

## Vertical-slice rule (US phases)

A `[US*]` task may be `[X]` only when reachable from a user-facing entry point and that path was
actually exercised (FSI against the packed library, a real host launch, or a `readiness/`
capture). This is an I/O-bearing host feature: `[X]` on a `[US*]` task also requires the
Elmish/MVU evidence — the viewer `Model`/`Msg`/`Effect` edge was exercised, `update` stayed
pure, and the GL interpreter edge was run against a real GL context where safe.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US4]** — user-story scope
- Overall feature tier: **Tier 1 (contracted change)** — breaking `.fsi`, dependency swap,
  constitution amendment. Per-task `[T1]`/`[T2]` omitted where it matches the overall tier.

## MVU/effect applicability

Principle IV **applies** (I/O-bearing host loop). The viewer Elmish edge
(`Model`/`Msg`/`Effect`/`init`/pure `update`) is **preserved unchanged**; only the
interpreter-edge present mechanism (Vulkan → OpenGL) changes. T011 records this; US1 evidence
exercises the real GL interpreter.

## Risk levels

- **small**: source-internal GL host body and tests (`src/SkiaViewer/Host/**`) — focused
  validation `./fake.sh build -t Dev`.
- **medium**: dependency swap + generated-product token seed — focused `DependencyReport`,
  `GeneratedProductCheck`.
- **broad**: breaking public `.fsi` + governance-token + constitution — broad validation
  required (`PackageSurfaceCheck`/`PerPackageSurfaceDiff`, `GeneratedGuidanceCheck`,
  `EvidenceGraph`, `EvidenceAudit`). Non-authoritative aggregate runs are recorded in
  `readiness/aggregate-hang-diagnostics.md` with verdict/stage/elapsed/last-command/focused-rerun.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold the feature directory and link spec + plan; confirm `.specify/feature.json` resolves `119-opengl-present-backend`
- [X] T002 [P] [skillist: []] Add `readiness/` scaffolding with audit-enforced placeholder files discoverable before implementation: `runtime-limitations.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `visual-evidence-honesty.md`, `window-visibility.md`, `real-image-evidence.md`, `generated-validation.md`, `evidence-audit.md`, `migration.md`, and `smoke/`/`sample-smoke/`/`fsi/` dirs — each naming its authoritative command, artifact path, failure class, next action
- [X] T003 [P] [skillist: []] Record feature Tier (1), affected layer (`FS.Skia.UI.SkiaViewer` host), public-API impact (breaking `Host/Vulkan.fsi` + diagnostic DUs), MVU applicability (edge-only swap), and evidence obligations into the readiness notes
- [X] T004 [skillist: fs-skia-skiaviewer] Swap the dependency manifest (FR-008): remove `Silk.NET.Vulkan` + `Silk.NET.Vulkan.Extensions.KHR` from `Directory.Packages.props` and `src/SkiaViewer/SkiaViewer.fsproj`; add `Silk.NET.OpenGL` `2.23.0`; keep `Silk.NET.Windowing*`/`Silk.NET.Input`/`SkiaSharp*`

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-skiaviewer] Draft the GL host public surface as `.fsi` (FSI-first, Principle I): `Host/OpenGl.fsi` (`GlResources`/`GlStartup`/`GlHost.run`), reconcile `ViewerDiagnosticCategory` (`Vulkan`→`OpenGl`, `Swapchain`→`Framebuffer`) and `ViewerRunBlockedStage` (`Swapchain`→`GlContext`, retain `Readback`) in `SkiaViewer.fsi`, and re-document `PresentMode.fsi` cases for GL (FR-007) — per `contracts/gl-host-surface.md`
- [X] T006 [skillist: speckit-constitution] Amend the constitution (FR-011) via `/speckit-constitution`: replace the Vulkan-backend mandate and the "Vulkan smoke" clause in `Project-specific constraints` with the OpenGL backend; keep `build/Governance/GovernedBlocks.fs` fragment in sync
- [X] T007 [skillist: fsharp-code-generation] Update governance tokens (FR-010): flip the single-sourced `runtime-limitations.md` token `"Vulkan"`→`"OpenGL"` in `build/Governance/Evidence/EvidenceFormatSchema.fs` `readinessContractChecks`, the generated seed in `GeneratedProduct.fs:970`, `build/Governance/README.md`, and the docs/ADR/architecture/report prose; regenerate `.claude` peers via `RefreshSurfaceBaselines`
- [X] T008 [skillist: fs-skia-skiaviewer] Exercise the draft `.fsi` from FSI against the packed/loaded surface (`GlResources`/`GlStartup`, each `ViewerPresentMode`, `ViewerOptions`) and capture the transcript to `readiness/fsi/gl-host-session.txt`
- [X] T009 [skillist: fs-skia-skiaviewer] Record surface-area baselines for the changed public modules (`RefreshSurfaceBaselines`) — top-level + per-package `SkiaViewer` surface
- [X] T010 [skillist: fs-skia-evidence-mode] Author `readiness/runtime-limitations.md` with the GL token set (`.NET 10 desktop`, `OpenGL`, `SkiaSharp preview`, `unsupported macOS/mobile/browser`, `no software-renderer fallback`) and record unsupported-scope handling + GL failure-diagnostic classification
- [X] T011 [skillist: fs-skia-elmish] Confirm and document the MVU/effect boundary is preserved: viewer `Model`/`Msg`/`Effect`/`init`/pure `update` unchanged, only the interpreter edge swapped; note the injected-`Tick` clock and retained-render step are untouched

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — Direct, readback-free live presentation (P1)

### Tests First (Principle I, VI)

- [X] T012 [P] [US1] [skillist: fs-skia-skiaviewer] Add `Feature119` semantic tests: GL present-mode mapping (`DirectToSwapchain` default, `OffscreenReadback` fallback) and the zero-readback assertion (per-frame readback count == 0 in direct mode) — failing-first before the GL host exists (SC-001)

### Implementation

- [X] T013 [US1] [skillist: fs-skia-skiaviewer] Implement the GL host body `Host/OpenGl.fs`: create the GL context (`GRGlInterface`/`GRContext.CreateGl`), wrap FBO 0 (`GRGlFramebufferInfo` → `GRBackendRenderTarget` → `SKSurface`, `GRSurfaceOrigin.BottomLeft`/`Rgba8888`), draw the existing `SceneRenderer` scene, `Flush`, then toolkit `SwapBuffers` — no readback/staging/command-pool/queue-stall
- [X] T014 [US1] [skillist: fs-skia-skiaviewer] Wire `Host/Viewer.fs` to route to `GlHost.run` instead of `VulkanHost.run` and flip the applied default `ViewerOptions.PresentMode` to `DirectToSwapchain`; high-level entry points keep their shape
- [X] T015 [P] [US1] [skillist: fs-skia-skiaviewer] Handle window/framebuffer resize and GL context loss (FR-006): recreate the render-target + `SKSurface` at the new framebuffer pixel size with no GPU-resource leak; classify context loss honestly (high-DPI/Wayland sized from framebuffer pixels)
- [X] T016 [US1] [skillist: fs-skia-skiaviewer] Capture the **persistent** interactive graphical launch on a GPU-passthrough machine (default executable path, real window, continuous render, working pointer/keyboard) → `readiness/supported-host-persistent-launch.txt`
- [X] T017 [US1] [skillist: fs-skia-evidence-mode] Capture the zero-readback present-path proof in direct mode using counts/booleans (no timing gate) → `readiness/smoke/zero-readback-present.md` (unblocks feature 118 SC-002)
- [X] T018 [US1] [skillist: fs-skia-skiaviewer] Document US1's independent validation path (how to reproduce direct present + the zero-readback proof) in the readiness notes

**Checkpoint**: US1 functional — readback-free direct present demonstrated on real hardware.

---

## Phase 4: User Story 2 (US2) — Unchanged visual output and interaction (P1)

### Tests First

- [X] T019 [P] [US2] [skillist: fs-skia-skiaviewer] Add interaction-parity semantic tests: pointer/keyboard routing, focus handling, and the animation clock behave identically across the backend swap (SC-003)

### Implementation

- [X] T020 [US2] [skillist: fs-skia-skiaviewer] Run the existing controls/charts/datagrid sample-smoke and screenshot/evidence captures under the GL backend and confirm they match the established baselines → `readiness/sample-smoke/*` (SC-002)
- [X] T021 [P] [US2] [skillist: fs-skia-evidence-mode] Preserve feature 118's capture/present decoupling (FR-004): the on-demand screenshot/evidence routine stays the `OffscreenReadback` offscreen path, independent of the live direct-present path; assert identical capture results
- [X] T022 [US2] [skillist: fs-skia-evidence-mode] Author `readiness/visual-evidence-honesty.md` + `readiness/real-image-evidence.md` (artifact paths, sha/byte-parity vs baseline, intentional-deviation log)
- [X] T023 [US2] [skillist: fs-skia-skiaviewer] Author `readiness/window-visibility.md` and document US2's independent validation path

**Checkpoint**: US2 functional — visual + interaction parity proven against baselines.

---

## Phase 5: User Story 3 (US3) — Safe behavior where GL is unavailable (P2)

### Tests First

- [X] T024 [P] [US3] [skillist: fs-skia-evidence-mode] Add semantic tests for the classified GL-unavailable diagnostic: missing/broken GL context → benign `UnsupportedEnvironment`; post-context failure → blocking defect; never a false success (SC-004)

### Implementation

- [X] T025 [US3] [skillist: fs-skia-evidence-mode] Implement GL context/FBO-acquisition failure classification in `Host/Diagnostics.fs` (GL stages), reusing the existing benign/blocking host-warning classifier; emit a structured diagnostic naming the failed GL stage (Principle VII)
- [-] T026 [US3] [skillist: fs-skia-evidence-mode] Capture the classified diagnostic on a **real** GL-unavailable environment (headless/no-passthrough shell) → `readiness/smoke/unsupported-gl-diagnostic.md` (real evidence, no synthetic)

**Checkpoint**: US3 functional — honest, classified GL-unavailable behavior on real hardware.

---

## Phase 6: User Story 4 (US4) — Consumer migration off the removed Vulkan surface (P2)

- [X] T027 [P] [US4] [skillist: fs-skia-skiaviewer] Author `readiness/migration.md` naming every removed/renamed public member with its GL replacement: `VulkanResources`/`VulkanStartup`/`VulkanHost`, `ViewerDiagnosticCategory.Vulkan`/`Swapchain`, `ViewerRunBlockedStage.Swapchain`, and the re-mapped `ViewerPresentMode` semantics (FR-009)
- [X] T028 [US4] [skillist: fs-skia-skiaviewer] Confirm the high-level consumer entry points (`runInteractiveApp` / `runInteractiveViewer` / `ViewerOptions`) compile unchanged against the new surface, with FSI/compile evidence (SC-005)

**Checkpoint**: US4 functional — breaking change documented; consumer front door source-stable.

---

## Phase 7: Integration & Polish

- [X] T029 [skillist: fsharp-build-orchestration] Run `./fake.sh build -t Dev` (host build + Controls/Elmish/Feature119 tests) and record the red→green evidence log
- [X] T030 [skillist: fsharp-build-orchestration] Run `./fake.sh build -t DependencyReport` — confirm `Silk.NET.Vulkan*` gone, `Silk.NET.OpenGL` present; regenerated `docs/reports/dependencies.md` (FR-008); note the two archived 085/086 harness `.fsproj`s are out of scope
- [X] T031 [skillist: fsharp-code-generation] Run `GeneratedGuidanceCheck` (regenerated `evidence-formats.md` token currency) then `GeneratedProductCheck` (generated `runtime-limitations.md` = OpenGL) sequentially
- [X] T032 [skillist: fs-skia-template-update] Run `./fake.sh build -t TemplateCheck` — record the expected template pin-lag failure pre-merge and document the deferral to the `/fs-skia-template-update` follow-up track (not in this merge scope)
- [X] T033 [skillist: fs-skia-skiaviewer] Refresh surface baselines and run `PackageSurfaceCheck`/`PerPackageSurfaceDiff` (Tier 1 breaking delta = intended `SkiaViewer` change and nothing else); author `readiness/governance-risk-levels.md` + `readiness/aggregate-hang-diagnostics.md`
- [X] T034 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; record graph before/after paths
- [X] T035 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS, 0 synthetic, no diff-scan hits; `generated-validation.md` package-resolution=resolved (SC-006)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. None planned (target: 0 synthetic;
the GL-unavailable path is reproducible on real headless hardware, so it is real evidence).

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
