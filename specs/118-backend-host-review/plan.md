# Implementation Plan: Backend and Host Mode Review

**Branch**: `118-backend-host-review` | **Date**: 2026-06-13 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/118-backend-host-review/spec.md`

## Summary

Phase 9 (final rung) of the controls-performance roadmap. The live present path
(`SkiaViewer/Host/Vulkan.fs` `renderFrame`) renders every frame to an **offscreen**
Skia surface, does a GPU→CPU `SKSurface.ReadPixels` readback (`renderSceneToPixels`,
Vulkan.fs:904), re-uploads via a **per-frame** staging buffer + command pool
(`copyPixelsToSwapchainImage`, Vulkan.fs:945), and stalls on a per-frame
`vkQueueWaitIdle` (Vulkan.fs:1054) before `vkQueuePresentKHR` (Vulkan.fs:1067). The
live present path and the evidence/screenshot path share that same readback routine,
so ordinary live frames pay evidence-mode cost.

Technical approach: add an **opt-in** present-mode selector to the public
`ViewerOptions` record (default = today's readback path, so existing behavior is
byte-identical), thread it through the internal `ViewerConfiguration` into
`renderFrame`, and add a **direct-to-swapchain** present path that wraps the acquired
swapchain image in a `GRBackendRenderTarget` (`GRVkImageInfo`) and renders the Skia
scene straight onto it — no readback, no per-frame staging buffer/command pool, no
per-frame `vkQueueWaitIdle`. The direct path degrades safely to the readback path with
a `Warning` diagnostic on any init failure (FR-005), recreates its per-image render
targets on swapchain recreation (FR-006), emits a **live-only, non-golden** backend
diagnostic over the existing `ViewerDiagnosticEvent` channel reporting mode + whether
ordinary frames read back (FR-007), and adds **no** `FrameMetrics` field (FR-008).
Evidence/screenshot capture keeps using the offscreen readback routine **on demand**
only (FR-004). US3 ships the audit + hosting-mode tradeoff document (FR-009).

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: SkiaSharp 4 preview (`GRBackendRenderTarget`, `GRVkImageInfo`,
`SKSurface.Create(context, backendRenderTarget, origin, colorType)`), Silk.NET Vulkan
(`Vk`, `KhrSwapchain`) — all already referenced by `FS.Skia.UI.SkiaViewer`. No new
package, no version/identity change.
**Testing**: Expecto (`tests/SkiaViewer.Tests`), FAKE routed gate set, FSI preludes,
live smoke + on-demand screenshot equivalence (the direct path's proof is live visual
evidence, not deterministic `Perf.runScript` goldens — the headless driver has no
backend).
**Target Platform**: Windows and Linux desktop (GPU passthrough expected per the
constitution's Vulkan-smoke clause). Headless/`Perf.runScript` has no backend; present
mode is moot there.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

This is a **Tier 1 (contracted change)**: it adds public API surface (a new
`ViewerOptions` field and a new present-mode DU), so it requires the full artifact
chain — `.fsi` updates, surface-area baseline updates, test evidence, docs. No new
dependency, no inter-project contract change. Re-checked after Phase 1: design holds;
no new violations; complexity is idiomatic (a record field + a closed DU + a backend
branch), no constitution-flagged feature (no SRTP/reflection/CE/type-provider/custom
operator) is introduced.

### Repository Governance Decisions

- **Template ownership**: **Required.** `ViewerOptions` is a public record; adding a
  field is a breaking record-shape change, so every construction site in the
  `dotnet new fs-skia-ui` template must add `PresentMode = ViewerPresentMode.OffscreenReadback`.
  Concretely `template/base/src/Product/EvidenceCommands.fs` (two `ViewerOptions`
  literals at ~:242 and ~:352). The generated product is then re-validated by
  `TemplateCheck` / `GeneratedProductCheck` (which Route is expected to add). The
  `.template.config/template.json` package set is unchanged (no package identity
  change), so no template manifest edit beyond the source literals; verify via the
  `fs-skia-template-update` skill on merge if pins move.
- **Dependency impact**: **N/A — no dependency change.** `GRBackendRenderTarget`,
  `GRVkImageInfo`, and `SKSurface.Create(backendRenderTarget…)` are already provided by
  the pinned SkiaSharp 4 preview that `SkiaViewer` references. No edit to
  `Directory.Packages.props`, `docs/dependencies.md`, or `DependencyReport` coverage.
- **Command-surface impact**: **N/A — no build-target definitions change.** Run exactly
  what `./fake.sh build -t Route` prints (expected: escalated SkiaViewer
  public-surface set because `SkiaViewer.fsi` changes, **plus** `TemplateCheck` /
  `GeneratedProductCheck` because the template/generated product constructs
  `ViewerOptions`). FAKE-backed targets run sequentially in the deterministic order
  (Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck → EvidenceGraph
  → EvidenceAudit); safe non-FAKE reads may parallelize. No change to the *definitions*
  of `Dev`, `Verify`, `Ci`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`,
  `TemplateDrift`, `EvidenceGraph`, or `EvidenceAudit`.
- **Generated project impact**: **Required (literals only).** The generated product's
  `ViewerOptions` construction sites gain the defaulted `PresentMode` field; default
  value keeps generated `Dev`/evidence behavior byte-identical. No change to selected
  Controls guidance, local skills, validation logs, placeholder/excluded-history scans,
  or generated `Dev` semantics beyond the field addition.
- **Evidence paths**: `specs/118-backend-host-review/readiness/` —
  `audit/present-path-audit.md` (FR-009 audit with concrete `Vulkan.fs` call sites),
  `audit/hosting-mode-tradeoffs.md` (FR-009 mode enumeration + "evidence is not live
  perf proof" statement), `smoke/direct-mode-smoke.md` + on-demand screenshots
  (FR-003 equivalence, FR-007 zero-readback live diagnostic), `smoke/default-byte-identity.md`
  (FR-001 visual + window-diagnostics unchanged), `smoke/safe-fallback.md` (FR-005),
  `fsi/*-prelude transcript` (FSI surface exercise), `readiness/generated-validation.md`
  (`package-resolution=resolved`, `package-mismatch=false`), `readiness/evidence-audit.md`
  (verdict). Golden-absence note: `Perf.runScript` metric goldens are unchanged (FR-008)
  — that absence is itself an evidence point.
- **`.fsi` / contract impact**: **Required.** `SkiaViewer.fsi` gains
  `[<RequireQualifiedAccess>] type ViewerPresentMode` (closed DU:
  `OffscreenReadback | DirectToSwapchain`) and a `PresentMode: ViewerPresentMode` field
  on `ViewerOptions`. `Host/Diagnostics.fsi` `ViewerConfiguration` gains a matching
  internal `PresentMode` field. XML-doc gate: `///` before each new public field/type,
  attribute-before-doc-before-type ordering. Per-package and top-level surface baselines
  update via `RefreshSurfaceBaselines`. Compatibility note: additive opt-in; default
  preserves today's behavior, but the record-shape change requires recompiling
  construction sites (mechanical: add one defaulted field).
- **MVU/effect boundary**: The viewer already owns an MVU boundary
  (`ViewerModel`/`ViewerMsg`/`ViewerEffect`, pure `Viewer.update`, backend interpreter
  in `Vulkan.fs run`). `PresentMode` is **configuration** carried in
  `ViewerModel.Options` (`Options: ViewerOptions` already exists) — **no new `Msg`,
  no new `Effect`, no `update` change.** The present-mechanism switch and the
  safe-fallback both live in the backend **interpreter edge** (`renderFrame`/`run`),
  which is the correct place for I/O; the fallback emits an existing-style `Warning`
  diagnostic event. `update` stays pure; effect ordering is unobservable to consumers.
- **Synthetic evidence**: **None planned.** All proof is real: real Vulkan backend live
  smoke, real on-demand screenshots, real diagnostic-sink capture, real default-mode
  byte-identity. The safe-fallback path (FR-005) is exercised by forcing a direct-path
  init failure on a real backend (mismatched color-type / unsupported wrap) — a real
  error path, not a mock. If a CI host lacks a Vulkan backend, the unsupported-environment
  classification already applies and is **not** synthetic substitution; any unavoidable
  `[S]` would be disclosed at all five surfaces per Principle V (not anticipated).
- **Test evidence**: Failing-first semantic tests in `tests/SkiaViewer.Tests`: (1) a
  `ViewerOptions` default-value test asserting `PresentMode = OffscreenReadback` (fails
  before the field exists); (2) configuration-threading test asserting the internal
  `ViewerConfiguration.PresentMode` mirrors the option; (3) present-mode → diagnostic
  category mapping test (Swapchain/Frame, not Renderer) for FR-007; (4) the live
  smoke/screenshot equivalence + zero-readback evidence (real backend). Governance:
  routed SkiaViewer public-surface gates + Template/GeneratedProduct gates.
- **Observability**: FR-007 live-only `ViewerDiagnosticEvent` (`Category = Swapchain` or
  `Frame`) naming the active present mode and whether ordinary frames read back, via the
  consumer `ViewerDiagnosticsOptions.Sink`. FR-005 emits a `Warning` diagnostic on
  direct-path fallback with actionable cause. Note plumbing gap (Phase 0 R3): today
  `LegacyDiagnosticReported` hardcodes `Category = Renderer` (SkiaViewer.fs:1290) — the
  present-mode diagnostic needs a category-carrying path so Swapchain/Frame survives.
- **Deferred scope**: Out of scope and explicitly deferred (FR-010/FR-011, report Phase 9
  task 5 + "do not do yet"): render-thread / compositor split, layer / scene-submission
  diffing, scene-graph caching, GPU/layer caches, any timing-based pass/fail gate.
  Backend timing is a human/diagnostic signal only; deterministic gating stays on counts
  and booleans. This feature closes the controls-performance roadmap; no successor phase.

## Project Structure

Source (framework-internal — `FS.Skia.UI.SkiaViewer`):

```
src/SkiaViewer/
  SkiaViewer.fsi          # + ViewerPresentMode DU; + ViewerOptions.PresentMode field (public surface)
  SkiaViewer.fs           # thread PresentMode into ViewerConfiguration; category-carrying diagnostic mapping
  Host/
    Diagnostics.fsi/.fs   # + internal ViewerConfiguration.PresentMode field
    Viewer.fs             # defaultConfiguration threads PresentMode
    Vulkan.fsi/.fs        # renderFrame branches on PresentMode; new direct-to-swapchain present
                          #   path (GRBackendRenderTarget/GRVkImageInfo), per-image render-target
                          #   cache on SwapchainState, safe fallback, present-mode live diagnostic
  skill/SKILL.md          # fs-skia-skiaviewer — refresh if host-seam guidance shifts
```

Construction-site updates (breaking record-shape — add `PresentMode = ViewerPresentMode.OffscreenReadback`):

```
template/base/src/Product/EvidenceCommands.fs   # ~:242, ~:352
samples/{BasicViewer,EffectsGallery,ParityGallery,DemoReel}/Program.fs
tests/SkiaViewer.Tests/Tests.fs + Feature*Tests.fs   # ~30 literals
tests/Elmish.Tests/Tests.fs ; tests/ControlsPreview.Harness/PreviewRender.fs
specs/085*/readiness/{harness,fsi}/* ; specs/086*/...; specs/090*/...   # readiness harnesses + .fsx preludes
src/SkiaViewer/SkiaViewer.fs:~2899   # internal "Generated App" literal
```
(`RefreshSurfaceBaselines` Build catches any missed sample/FSI site; with-expressions
that reuse a base options value do **not** need editing.)

Tests / evidence:

```
tests/SkiaViewer.Tests/Feature118*Tests.fs   # default value, config threading, diagnostic category
specs/118-backend-host-review/readiness/**   # audit, hosting-mode doc, smoke, screenshots, audit verdict
```

## Phase 0 — Research

See [research.md](./research.md). Resolves: (R1) direct-to-swapchain Skia/Vulkan wrap
mechanics + image-layout/present ownership; (R2) per-swapchain-image render-target
caching + recreation on resize; (R3) the diagnostic category-carrying plumbing (current
`Renderer` hardcode) for the FR-007 Swapchain/Frame event; (R4) safe-fallback trigger
points + color-type/sample-count match; (R5) construction-site inventory + whether a
defaulted smart-constructor reduces churn.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md) — `ViewerPresentMode` DU, `ViewerOptions` /
  `ViewerConfiguration` field additions, per-image render-target cache shape, state flow.
- [contracts/skiaviewer-surface-delta.md](./contracts/skiaviewer-surface-delta.md) —
  exact `.fsi` additions (public + internal), default value, doc-ordering.
- [quickstart.md](./quickstart.md) — FSI exercise of the new surface and how a consumer
  opts into the direct path + attaches a diagnostics sink.
- Agent context: `AGENTS.md` SPECKIT marker repointed to this plan.
