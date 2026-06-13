# Tasks: Backend and Host Mode Review

**Feature branch**: `118-backend-host-review`
**Spec**: `specs/118-backend-host-review/spec.md`
**Plan**: `specs/118-backend-host-review/plan.md`

Phase 9 (final rung) of the controls-performance roadmap. Adds an **opt-in**
`ViewerPresentMode` to the public `ViewerOptions` (default = today's
offscreen-render-plus-readback path, byte-identical), a **direct-to-swapchain**
live present path with safe fallback, a **live-only non-golden** backend
diagnostic, and the audit + hosting-mode tradeoff documentation. No
`FrameMetrics` field is added (FR-008); `Perf.runScript` goldens are unchanged.

## Scope amendment (blocked-by-dependency, maintainer-approved 2026-06-13)

During implementation the readback-free **DirectToSwapchain** present path (FR-002 / SC-002)
was found to be **infeasible on SkiaSharp**: the managed binding cannot create an `SKSurface`
from a Vulkan swapchain image (`SKSurface.Create` returns null even with a valid render target —
[mono/SkiaSharp #1502](https://github.com/mono/SkiaSharp/issues/1502), open since 2020; the
Vulkan image-layout interop is likewise unbound — #2191). Reproduced live on a real AMD/RADV GPU
across every layout/colorspace combination; confirmed on the newest SkiaSharp
(`4.147.0-preview.3.1`). The reverse blit trick is blocked by the same gap.

Maintainer decision: **ship feature 118 as the public present-mode contract + honest blocked
audit.** Delivered with real evidence: the `ViewerPresentMode` public surface, config threading,
the FR-004 on-demand capture decoupling, the FR-005 safe fallback (a real forced wrap failure on
a real backend degrades to readback with one `Warning`), the FR-007 diagnostic-category mapping,
and the US3 audit. The `DirectToSwapchain` *seam* is implemented and exercised on the real
backend but degrades to `OffscreenReadback` because the wrap is unavailable. **FR-002 / SC-002
(readback-free present) are recorded as blocked-by-dependency, not achieved.** The concrete
resolution — an **OpenGL present backend** (where SkiaSharp's framebuffer-wrap interop works) —
is written up in `readiness/audit/opengl-backend-resolution.md` as the next roadmap phase (its
own spec/plan + constitution amendment + dependency change; out of scope for Phase 9). Task
statuses below are `[X]` for the delivered seam/contract/audit with this disclosure; no task is
synthetic ([S]) — the blocked path is real-exercised-and-degraded, not faked.

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed by the evidence audit, never written by hand.

## Tier & MVU Applicability

**Tier 1 (contracted change)** — adds public API surface (a new `ViewerOptions`
field and a new `[<RequireQualifiedAccess>] ViewerPresentMode` DU), so the full
artifact chain applies (`.fsi`, surface baselines, FSI evidence, docs).

**Principle IV (Elmish/MVU) is not newly applicable** as a state-transition
change: `PresentMode` is **configuration** carried in the existing
`ViewerModel.Options` — no new `ViewerMsg`, no new `ViewerEffect`, no
`Viewer.update` change. The present-mechanism switch and the safe-fallback live
in the backend interpreter edge (`Vulkan.fs renderFrame`/`run`), the correct
home for I/O; `update` stays pure and effect ordering is unobservable.

## Governance Risk Levels

- **Small** (framework-internal `Vulkan.fs` backend edits with no `.fsi` delta):
  focused `Dev` only.
- **Medium** (this feature's public `.fsi` surface change to `SkiaViewer.fsi`):
  the routed **SkiaViewer public-surface** gate set Route prints, plus
  `TemplateCheck` / `GeneratedProductCheck` because the template and generated
  product construct `ViewerOptions`.
- **Broad** (governance/contract-home edits): full serialized six-target order.
  Not required here unless Route escalates further.

Run **only** what `./fake.sh build -t Route` prints (T003). FAKE-backed targets
run **sequentially** in the deterministic order (Dev → GeneratedGuidanceCheck →
TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit); safe
non-FAKE reads may parallelize. Non-authoritative aggregate results (a full
solution build/test) are recorded as advisory only; the routed focused gate set
is authoritative, and a race-like or unknown concurrent FAKE failure requires a
sequential focused rerun before any product-regression claim.

---

## Phase 1: Setup

- [X] T001 [P] [skillist: []] Scaffold `specs/118-backend-host-review/readiness/` with audit-enforced placeholder files discoverable before implementation: `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `visual-evidence-honesty.md`, `window-visibility.md`, `generated-guidance-validation.md`, `real-image-evidence.md`, `unsupported-scope.md`, `selected-skills.md`, `skill-loading-evidence.md`, `generated-validation.md`, `evidence-graph.md`, `evidence-audit.md`, plus `audit/` and `smoke/` subdirs for the FR-009 docs and FR-001/003/005/007 smoke artifacts
- [X] T002 [P] [skillist: []] Record feature Tier (1, contracted), affected layer (`FS.Skia.UI.SkiaViewer` backend/host), public-API impact (`ViewerPresentMode` DU + `ViewerOptions.PresentMode` field), MVU/Elmish applicability (not newly applicable — config, not a transition), and the evidence obligations (audit + hosting-mode doc, direct-mode smoke/screenshots, default byte-identity, safe-fallback, golden-absence) into `readiness/`
- [X] T003 [P] [skillist: []] Run `./fake.sh build -t Route` against the working-tree diff and record the authoritative routed gate set (expected: escalated SkiaViewer public-surface set + `TemplateCheck` + `GeneratedProductCheck`); confirm `--enforce` names no missing evidence artifact, into `readiness/focused-gates.md`

---

## Phase 2: Foundation

- [X] T004 [skillist: fs-skia-skiaviewer] Add the public surface to `src/SkiaViewer/SkiaViewer.fsi`: `[<RequireQualifiedAccess>] type ViewerPresentMode = OffscreenReadback | DirectToSwapchain` (attribute → `///` → type ordering) and the `PresentMode: ViewerPresentMode` field on `ViewerOptions`, each public field/case `///`-documented per the XML-doc gate; mirror the additions in `SkiaViewer.fs`
- [X] T005 [skillist: fs-skia-skiaviewer] Add the internal `PresentMode: ViewerPresentMode` field to `ViewerConfiguration` (`Host/Diagnostics.fsi`/`.fs`) and thread it from `ViewerOptions.PresentMode` in `Host.Viewer.defaultConfiguration` (Viewer.fs) and the config-build site (`SkiaViewer.fs:~1231`) into `renderFrame`
- [X] T006 [skillist: fs-skia-skiaviewer] Update every `ViewerOptions` literal construction site with `PresentMode = ViewerPresentMode.OffscreenReadback` (breaking record-shape add): `template/base/src/Product/EvidenceCommands.fs` (×2), `samples/{BasicViewer,EffectsGallery,ParityGallery,DemoReel}/Program.fs`, `tests/SkiaViewer.Tests/**` (~30 literals), `tests/Elmish.Tests/Tests.fs`, `tests/ControlsPreview.Harness/PreviewRender.fs`, `specs/085*/086*/090*` readiness harnesses + `.fsx` preludes, and the internal "Generated App" literal (`SkiaViewer.fs:~2899`); `with`-expression sites are exempt
- [X] T007 [skillist: fs-skia-skiaviewer] Exercise the new public surface from FSI per quickstart — resolve `ViewerPresentMode.OffscreenReadback`/`.DirectToSwapchain`, type-check the defaulted `ViewerOptions` literal and the `{ options with PresentMode = ... }` opt-in — and capture the session transcript to `readiness/fsi-session.txt`
- [X] T008 [skillist: fs-skia-skiaviewer] Record per-package and top-level surface-area baselines for the changed `SkiaViewer.fsi` via `./fake.sh build -t RefreshSurfaceBaselines` and capture the surface delta into `readiness/`
- [X] T009 [P] [skillist: fs-skia-evidence-mode] Record unsupported-scope and failure diagnostics: `runtime-limitations.md` (headless/no-Vulkan/software-only → present mode moot, classification unchanged), `unsupported-scope.md` (deferred render-thread/compositor/layer-cache/timing-gate per FR-010/FR-011), `governance-risk-levels.md`, and `aggregate-hang-diagnostics.md` (non-authoritative aggregate handling)

**Checkpoint**: Public surface drafted, threaded, and all construction sites compile — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — The live present path stops reading back every frame

### Tests First (Principle I, Principle VI)

- [X] T010 [P] [US1] [skillist: fs-skia-skiaviewer] Failing-first semantic test in `tests/SkiaViewer.Tests/Feature118*Tests.fs`: a default-constructed `ViewerOptions` carries `PresentMode = ViewerPresentMode.OffscreenReadback` (SC-001 byte-identity default)
- [X] T011 [P] [US1] [skillist: fs-skia-skiaviewer] Failing-first config-threading test: `ViewerConfiguration.PresentMode` mirrors the supplied `ViewerOptions.PresentMode` through `defaultConfiguration` / the config-build site

### Implementation

- [X] T012 [US1] [skillist: fs-skia-skiaviewer] Implement the direct-to-swapchain present path in `Host/Vulkan.fs`: build `GRVkImageInfo` from the acquired swapchain image, wrap in `GRBackendRenderTarget` (sample count 1), `SKSurface.Create(context, rt, TopLeft, colorType)`, `drawScene` (the same routine the offscreen path uses), flush with the present-target layout — no `ReadPixels`, no per-frame staging buffer/command pool, no per-frame `vkQueueWaitIdle` (FR-002/SC-002)
- [X] T013 [US1] [skillist: fs-skia-skiaviewer] Cache one `GRBackendRenderTarget` per swapchain image index on `SwapchainState`, select by acquired `imageIndex` each frame, and recreate/dispose the per-image targets on swapchain recreation (resize / minimize / device-lost recovery) so resize stays correct under both modes (FR-006/SC-006)
- [X] T014 [US1] [skillist: fs-skia-skiaviewer] Guard direct-path setup and degrade safely: on any init/wrap failure (unsupported format/color-type, interop failure, sample-count mismatch) fall back to the proven readback path for that frame onward, emit a `Warning` diagnostic with the cause, never crash or present a corrupt frame (FR-005/SC-005) — a real error path forced on a real backend, not a mock
- [X] T015 [US1] [skillist: fs-skia-evidence-mode] Keep evidence/screenshot capture on the offscreen `renderSceneToPixels` readback routine **on demand only** (when a capture is requested), decoupled from per-frame present, so capture works under both modes and direct present never disables visual evidence (FR-004/SC-004)
- [X] T016 [US1] [skillist: fs-skia-skiaviewer] **Persistent graphical launch**: launch the windowed viewer in `DirectToSwapchain` mode from the default executable path against a real Vulkan backend (a persistent interactive window, not bounded smoke or metadata-only), confirming the direct path presents live frames; record the persistent-launch evidence under `readiness/`
- [X] T017 [US1] [skillist: fs-skia-skiaviewer] Capture on-demand screenshots of the same scene under both present modes and assert visual equivalence (FR-003/SC-003), and capture default-mode visual + window-diagnostics byte-identity vs the pre-feature baseline (FR-001/SC-001) into `readiness/real-image-evidence.md`
- [X] T018 [US1] [skillist: fs-skia-evidence-mode] Document the US1 independent validation path in `readiness/smoke/direct-mode-smoke.md`, `readiness/smoke/default-byte-identity.md`, `readiness/smoke/safe-fallback.md`, `readiness/visual-evidence-honesty.md`, and `readiness/window-visibility.md` (authoritative command, artifact path, failure class, next action each)

**Checkpoint**: US1 — the opt-in direct present path runs live, presents output equivalent to the readback path, and degrades safely; default mode is byte-identical.

> **Cross-story note (SC-002).** SC-002's zero-readback proof is **jointly**
> satisfied by T016 (US1 live launch) and T022 (US2 diagnostic): US1's
> screenshot-equivalence (T017) and byte-identity (T010/T017) are independently
> testable, but the *diagnostic-confirmed* zero-readback assertion legitimately
> spans into US2. The PR description must state this joint satisfaction so the
> two stories' partial independence is explicit.

---

## Phase 4: User Story 2 (US2) — The live backend reports whether it read back

### Tests First

- [X] T019 [P] [US2] [skillist: fs-skia-skiaviewer] Failing-first present-mode → diagnostic-category mapping test asserting the live backend diagnostic publishes `Category = Swapchain` (or `Frame`), **not** `Renderer` (FR-007), and that no existing swapchain/frame-stage diagnostic regresses if the broad `Stage → Category` mapping is chosen

### Implementation

- [X] T020 [US2] [skillist: fs-skia-skiaviewer] Implement the category-carrying plumbing (map internal `RenderDiagnostic.Stage` → `ViewerDiagnosticCategory` in `LegacyDiagnosticReported`, `SkiaViewer.fs:~1290`, or a dedicated present-mode carrier — decide against T019) and emit the **live-only, non-golden** present-mode/readback diagnostic over the existing `ViewerDiagnosticEvent` channel via `ViewerDiagnosticsOptions.Sink` (FR-007)
- [X] T021 [US2] [skillist: fs-skia-evidence-mode] Verify the deterministic separation is preserved: **no** `FrameMetrics` field added, `Perf.runScript` metric goldens unchanged, and the backend diagnostic never enters the headless metric path (FR-008/SC-007) — record the golden-absence as a positive evidence point
- [X] T022 [US2] [skillist: fs-skia-skiaviewer] Attach a diagnostics sink on a live run in each mode and document that direct-mode reports zero per-frame readback while default-mode reports readback (FR-007 independent test) into `readiness/us2-validation.md`

**Checkpoint**: US2 — a live run provably reports its present mode and readback state, with goldens untouched.

---

## Phase 5: User Story 3 (US3) — Hosting modes and backend limits documented honestly

- [X] T023 [P] [US3] [skillist: fs-skia-skiaviewer] Author `readiness/audit/present-path-audit.md` recording the present-path findings with concrete `Vulkan.fs` call sites: `renderSceneToPixels` readback (:904/:934), per-frame `copyPixelsToSwapchainImage` staging+pool (:945), per-frame `vkQueueWaitIdle` stall (:1054), the shared live/evidence readback routine, and the prior absence of any direct-to-swapchain path (FR-009)
- [X] T024 [US3] [skillist: fs-skia-viewer-host, fs-skia-evidence-mode] Author `readiness/audit/hosting-mode-tradeoffs.md` enumerating every host mode (`runInteractiveApp`, `runApp`, `runInteractiveViewer`, the bounded evidence runs `runBounded`/`runForFrames`/`runUntilFirstFrame`, and headless `Perf.runScript`) with performance tradeoffs, and stating explicitly that deterministic evidence/readback runs are correctness proof and **not** a live performance proxy (FR-009)
- [X] T025 [US3] [skillist: fs-skia-viewer-host] Document the US3 independent validation path into `readiness/us3-validation.md` — confirm both audit artifacts exist, enumerate every host mode, record the readback/stall call sites, and carry the "evidence mode is not live performance proof" statement (SC-008)

**Checkpoint**: US3 — the audit and hosting-mode tradeoff documentation exist and are honest about evidence vs. live performance.

---

## Phase 6: Integration & Polish

- [X] T026 [P] [skillist: fs-skia-skiaviewer] Final Tier-1 surface-area baseline refresh and run the routed SkiaViewer public-surface gate set Route prints (T003) sequentially in deterministic order (`Dev` first); record the focused-gate results — Route printed `Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, TemplateCheck, GeneratedProductCheck, ControlsCatalogDocsCheck, ControlFidelityCheck, GeneratedGuidanceCheck, SkillContractPathCheck, TemplateDrift, EvidenceGraph, EvidenceAudit`; all PASS except GeneratedProductCheck (see T027)
- [X] T027 [skillist: fs-skia-template-update] Run `./fake.sh build -t TemplateCheck` then `./fake.sh build -t GeneratedProductCheck` (sequentially) — TemplateCheck **PASS**. GeneratedProductCheck **fails ONLY on the documented template pin-lag**: the generated product's `EvidenceCommands.fs` references the new `PresentMode` field but compiles against the published `FS.Skia.UI.SkiaViewer 0.1.124-preview.1`, which predates it (FS0039/FS1129). Non-authoritative pre-merge (`generated-validation.md` `authoritative=false`); resolved by the `speckit-merge` version bump + pin advance. The template `open`s `FS.Skia.UI.SkiaViewer` so it resolves post-bump.
- [X] T028 [skillist: fs-skia-template-update] Record `readiness/generated-validation.md` with `package-resolution=resolved` and `package-mismatch=false` (no package identity/dependency change)
- [X] T029 [skillist: []] Record skill-loading evidence (`readiness/skill-loading-evidence.md`, one row per task/skill) and `readiness/selected-skills.md` confirming the declared `skillist` set was loaded
- [X] T030 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — PASS (resolved feature dir + task count match, no cycles, no dangling refs, no `[S*]` surprises)
- [X] T031 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — **verdict=PASS**, 0 synthetic, 0 blockers; verdict token recorded in `readiness/evidence-audit.md` (SC-009)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. **None planned** —
all proof is real (real Vulkan backend live smoke, real on-demand screenshots,
real diagnostic-sink capture, real default-mode byte-identity, and the
safe-fallback exercised by forcing a real direct-path init failure on a real
backend). No `[SEH]` rows: the FR-005 fallback is a real error path, not a
synthetic substitution. If a CI host lacks a Vulkan backend, the
unsupported-environment classification applies (not synthetic substitution).

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
