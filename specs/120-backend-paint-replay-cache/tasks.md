# Tasks: Backend Paint Replay & Performance Honesty

**Feature branch**: `120-backend-paint-replay-cache`
**Spec**: `specs/120-backend-paint-replay-cache/spec.md`
**Plan**: `specs/120-backend-paint-replay-cache/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written. `[SEH]` is a design-approved synthetic
error-handling annotation that remains `[S]` when completed. **No `[SEH]` tasks are planned**
(target: 0 synthetic). The forced-staleness / structural-collision test (T020, T028) feeds a
real constructed structural input through the real fingerprint and asserts a real cache *miss*
plus real readback pixels — that is real evidence, not a synthetic error path, so it does not
qualify for `[SEH]`.

## Vertical-slice rule (US phases)

A `[US*]` task may be `[X]` only when reachable from a user-facing entry point and that path was
actually exercised (FSI against the packed library, a real host launch, or a `readiness/`
capture). This is an I/O-bearing host/render feature: `[X]` on a `[US*]` task also requires the
Elmish/MVU evidence — the viewer `Model`/`Msg`/`Effect` edge was exercised, the unchanged-frame
skip stayed a **pure** `update` decision, and the record/replay + timing executed in the real GL
**interpreter** against a real GL context on the Linux/Mesa reference environment.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US4]** — user-story scope
- Overall feature tier: **Tier 1 (contracted change)** — additive public `.fsi` deltas
  (`Scene` `CachedSubtree` case, `FrameMetrics` timing + replay fields, corrected `SkiaViewer`
  docstring, new internal `PictureReplayCache.fsi`). Per-task `[T1]`/`[T2]` omitted where it
  matches the overall tier.

## MVU/effect applicability

Principle IV **applies** (I/O-bearing host loop). The behavioral change lives at the viewer's
existing MVU/effect edge: `Model` gains an internal **frame-dirty signal**; `update` makes a
pure emit-or-skip decision on the existing `RenderFrame` effect (FR-004/FR-006); record/replay
and per-phase timing execute in the `RenderFrame` interpreter, never in `update`. No new
consumer-facing `Msg`/`Cmd` contract. T010 records this; US2 evidence proves the pure transition
+ the real interpreter idle skip.

## Functional-requirement → task mapping (forward traceability)

Most FRs are traced via the success-criterion map below; these two are otherwise implicit:

- FR-007 (cache recorded backend draw commands + replay) → T025 (`PictureReplayCache.fs` record-on-miss / replay-on-hit, wired into `SceneRenderer.paintNode`); effects gated by SC-003/SC-004.
- FR-019 (at-rest byte-identity of presented output + deterministic goldens) → T012 (count goldens byte-identical) + T023 (`CachedSubtree` see-through preserves at-rest byte-identity) + T038 (deterministic surface byte-identical); a cross-cutting invariant, not a single task.

## Success-criterion → assertion mapping

- SC-001 → T012 (distinct paint/compose durations; deterministic count goldens byte-identical).
- SC-002 → T016 (pure no-cause ⇒ no `RenderFrame`) + T019 (live zero-redraw + byte-identical forced repaint).
- SC-003 → T027 (cache-on/off readback byte-identity per corpus scene).
- SC-004 → T027 (10000-row grid `ReplaySkippedNodeCount` majority + lower `PaintDuration`).
- SC-005 → T020 (collision-miss the truncating key would falsely hit) + T028 (forced-staleness re-record).
- SC-006 → T021 (LRU bound + native dispose) + T038 (`ReplayCacheNativeBytes` bounded golden).
- SC-007 → T029 (DirtyArea = union area, ≤ frame area).
- SC-008 → T031/T032 + T034 (docstring names default; sample persistent-launches readback=false).
- SC-009 → T033 (dead ref absent) + T040/T041 (routed set + EvidenceAudit PASS, 0 synthetic).

## Risk levels

- **small**: framework-internal `.fs` bodies + their tests (`src/Scene/Scene.fs`,
  `src/Controls/RetainedRender.fs`, `src/SkiaViewer/PictureReplayCache.fs`, `SceneRenderer.fs`,
  `Host/OpenGl.fs`, `tests/**`) — focused validation `./fake.sh build -t Dev`.
- **medium**: live sample present-mode + perf-corpus golden updates — focused
  `GeneratedProductCheck`, `TemplateCheck`, corpus golden diff.
- **broad**: additive public `.fsi` (Scene case + FrameMetrics fields + docstring) — broad
  validation required (`PackageSurfaceCheck`/`PerPackageSurfaceDiff`, `EvidenceGraph`,
  `EvidenceAudit`). Non-authoritative aggregate runs are recorded in
  `readiness/aggregate-hang-diagnostics.md` with verdict/stage/elapsed/last-command/focused-rerun.

## Canonical verification targets

Run `./fake.sh build -t Route` first and obey its printed gate list. FAKE-backed targets share
`.fake` state and run **sequentially** in the documented order: `Dev`, controls/package
public-surface gates, `GeneratedProductCheck`, `TemplateCheck`, `EvidenceGraph`, `EvidenceAudit`.
Safe non-FAKE reads may parallelize.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold the feature directory and link spec + plan; confirm `.specify/feature.json` resolves `specs/120-backend-paint-replay-cache`
- [X] T002 [P] [skillist: []] Add `readiness/` scaffolding with audit-enforced placeholder files discoverable before implementation: `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation.md`, `visual-evidence-honesty.md`, `window-visibility.md`, `real-image-evidence.md`, `skill-loading-evidence.md`, `focused-gates.md`, `evidence-graph.md`, `evidence-audit.md`, and the `smoke/`, `fsi/`, `perf-corpus/` dirs — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [P] [skillist: []] Record feature Tier (1), affected layers (`FS.Skia.UI` Scene IR, `FS.Skia.UI.Controls` RetainedRender, `FS.Skia.UI.Controls.Elmish` FrameMetrics, `FS.Skia.UI.SkiaViewer` backend), public-API impact (additive Scene case + FrameMetrics fields + docstring + new internal cache `.fsi`), MVU applicability (frame-dirty signal + conditional `RenderFrame`), and the six evidence obligations into the readiness notes

---

## Phase 2: Foundation

- [X] T004 [skillist: fs-skia-scene] Draft the additive public Scene IR contract in `src/Scene/Scene.fsi` (Principle I, FSI-first): new `SceneNode.CachedSubtree of CacheBoundary` case + `CacheBoundary` record (`CacheId: uint64`, `Fingerprint: uint64`, `Scene: Scene`), documented **transparent** to every consumer except the backend painter — per `contracts/contracts.md` §1
- [X] T005 [skillist: fs-skia-elmish] Draft the additive `FrameMetrics` delta in `src/Controls.Elmish/ControlsElmish.fsi`: non-golden `PaintDuration`/`ComposeDuration: TimeSpan` + golden counters `ReplayHitCount`/`ReplayMissCount`/`ReplayRecordCount`/`ReplaySkippedNodeCount`/`ReplayCacheNativeBytes: int`, and correct the `DirtyArea` docstring to "union of distinct damage rectangles, never exceeds frame area" — per `contracts/contracts.md` §2
- [X] T006 [skillist: fs-skia-skiaviewer] Draft `src/SkiaViewer/PictureReplayCache.fsi` (NEW `module internal`: `create`/`paintBoundary`/`stats`/`dispose`, replay-disable oracle seam) and correct the `ViewerOptions.PresentMode` docstring in `src/SkiaViewer/SkiaViewer.fsi` to name the shipped `DirectToSwapchain` default — per `contracts/contracts.md` §3, §4
- [X] T007 [skillist: fs-skia-reconciliation] Draft the internal `src/Controls/RetainedRender.fsi` seam (no public surface delta): `val internal hashScene` structural fingerprint, `PictureCacheKey` → `{ Box; Fingerprint: uint64 }`, `Fragment` gains internal `Fingerprint`, and the `CachedSubtree` emission seam — per `contracts/contracts.md` §5
- [X] T008 [skillist: fs-skia-skiaviewer] Exercise the drafted `.fsi` from FSI against the loaded surface (construct a `CachedSubtree`, confirm transparent describe/measure; read the new `FrameMetrics` fields; sketch the `PictureReplayCache` create/stats shape) and capture the transcript to `readiness/fsi/session.txt`
- [X] T009 [skillist: fs-skia-skiaviewer] Record surface-area baselines for the changed public modules (`./fake.sh build -t RefreshSurfaceBaselines`) — top-level + per-package `Scene`, `Controls.Elmish`, `SkiaViewer` surfaces (additive delta only)
- [X] T010 [skillist: fs-skia-elmish] Confirm and document the MVU/effect boundary: the internal frame-dirty signal on the viewer `Model`, the pure emit-or-skip `RenderFrame` decision in `update` (FR-004/FR-006), and the record/replay + phase-timing in the interpreter edge — existing `Msg`/`Cmd` contract preserved, no new public message
- [X] T011 [skillist: fs-skia-evidence-mode] Author `readiness/runtime-limitations.md` and record unsupported-scope handling + safe-failure diagnostics: no damage-rect GPU clip, no render-thread/compositor split, **Windows GL not launch-verified** (Linux/Mesa-only evidence), a failed `SKPicture` record degrades to the direct walk, idle-skip degrades to painting on an uncertain dirty signal

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — Per-phase paint cost is observable (P1)

### Tests First (Principle I, VI)

- [X] T012 [P] [US1] [skillist: fs-skia-elmish] Add `Feature120` semantic tests (failing-first): the live timing path reports a distinct `PaintDuration` (scene→canvas walk) and `ComposeDuration` (flush + buffer-swap), and the deterministic count goldens are byte-identical to before the timing fields were added (SC-001 / FR-002)

### Implementation

- [X] T013 [US1] [skillist: fs-skia-skiaviewer] Capture per-phase durations with `Stopwatch` in `Host/OpenGl.fs` (`renderFrameDirect`/`drawScene`): time the scene paint-walk separately from flush + `SwapBuffers`, thread both into `FrameMetrics.PaintDuration`/`ComposeDuration` (`TimeSpan.Zero` on the deterministic `Perf.runScript` path)
- [X] T014 [US1] [skillist: fs-skia-evidence-mode] Wire the non-golden live timing baseline generator to capture the new per-phase durations for the existing performance corpus scenarios into `docs/reports/_baselines/2026-06-13-paint-replay-after.md` (FR-003), emitting a `MissingCounters:` line if a phase cannot be timed (109 honesty precedent)
- [X] T015 [US1] [skillist: fs-skia-skiaviewer] Capture the before/after live timing baseline on the Mesa corpus, confirm paint and present/compose durations are separated, and document US1's independent validation path in the readiness notes

**Checkpoint**: US1 functional — per-phase paint cost is observable; the cache win is now measurable, not asserted.

---

## Phase 4: User Story 2 (US2) — Unchanged frames do no paint work (P1)

### Tests First

- [X] T016 [P] [US2] [skillist: fs-skia-elmish] Add the pure-transition test (failing-first): an unchanged frame with **no** dirty cause emits **no** `RenderFrame` effect, while an active animation clock / resize / theme / model change / work-requiring tick **does** emit it (FR-004 / FR-006)

### Implementation

- [X] T017 [US2] [skillist: fs-skia-controls-host] Add the internal frame-dirty signal to the viewer `Model` (set by product message / resize / theme / active animation clock; cleared after present) and make `update` emit `RenderFrame` only when set or an animation clock is live — pure decision, no new public `Msg`
- [X] T018 [US2] [skillist: fs-skia-skiaviewer] Implement the present-path idle skip in the interpreter: a skipped frame performs no surface clear, no scene walk, and no draw-call re-issue, and keeps the previously presented front buffer valid (re-present treated as "no scene work", FR-005) on the double-buffered GL surface
- [X] T019 [US2] [skillist: fs-skia-evidence-mode] Drive the interactive host on Mesa with a steady stream of no-op frames after the first render; capture the zero-redraw proof (subsequent unchanged frames report zero paint work / zero draw-call re-issue) and confirm a forced repaint afterward is byte-identical to the first frame's readback → `readiness/smoke/idle-zero-redraw.md` (SC-002)

**Checkpoint**: US2 functional — idle/unchanged frames skip all scene work on real hardware with no flicker.

---

## Phase 5: User Story 3 (US3) — Stable subtrees replay with a key safe to depend on (P2)

### Tests First

- [X] T020 [P] [US3] [skillist: fs-skia-reconciliation] Add `Feature120` `Controls.Tests` fingerprint tests (failing-first): `hashScene` differs for any render-affecting change (geometry, color, path, text, font, opacity, transform, clip) and — the key proof — a constructed subtree that stringifies identically under the old truncating `%A` digest but differs structurally produces a **different** fingerprint (SC-005 / FR-008)
- [X] T021 [P] [US3] [skillist: fs-skia-skiaviewer] Add `Feature120` `SkiaViewer.Tests` for `PictureReplayCache`: hit on matching fingerprint, miss/re-record on changed fingerprint or eviction, LRU bound never exceeded, native `SKPicture` disposed on evict/replace, and the replay-disable oracle never records/replays (FR-011 / FR-013)

### Implementation

- [X] T022 [US3] [skillist: fs-skia-reconciliation] Implement `hashScene` (collision-resistant structural fold; `// mutable: hot path` accumulator) in `RetainedRender.fs`, replace the `sprintf "%A"`-based `PictureCacheKey.Picture` with `{ Box; Fingerprint }`, and memoize `Fingerprint` on the `Fragment` (computed in `paintFresh`/`buildFresh`, carried unchanged on `Keep` — cost ∝ damage, not tree size)
- [X] T023 [US3] [skillist: fs-skia-scene] Implement the `CachedSubtree` node in `src/Scene/Scene.fs`: `describe`/diagnostics/`measure` and all IR traversals **see through** it (recurse into `Scene`), preserving deterministic goldens and at-rest byte-identity
- [X] T024 [US3] [skillist: fs-skia-reconciliation] Emit `CachedSubtree` from `RetainedRender` for subtrees reuse-stable on the **prior** frame only (FR-012 churny-gate; `CacheId` from `RetainedId`, `Fingerprint` from T022), and update all internal reduction / virtual-items / damage walks to see through the boundary
- [X] T025 [US3] [skillist: fs-skia-skiaviewer] Implement `PictureReplayCache.fs` (record-on-miss via `SKPictureRecorder` at the boundary box, replay-on-hit via `DrawPicture`, `Dispose` on evict/replace, deterministic min-`Stamp` LRU eviction, native-byte accounting, replay-disable oracle) and wire `SceneRenderer.paintNode` `CachedSubtree → replay-or-record` (transparent when disabled)
- [X] T026 [US3] [skillist: fs-skia-elmish] Thread the per-frame replay counters (`ReplayHitCount`/`ReplayMissCount`/`ReplayRecordCount`/`ReplaySkippedNodeCount`/`ReplayCacheNativeBytes`) from the cache stats into `FrameMetrics` (FR-014)
- [X] T027 [US3] [skillist: fs-skia-evidence-mode] Capture cache-on / cache-off **pixel readback parity** on Mesa for **every** performance corpus scene (byte-identical, FR-009/FR-011), and on the 10000-row DataGrid small-change frame confirm `ReplaySkippedNodeCount` is the large majority of subtree paint nodes with a lower `PaintDuration` than the replay-off baseline → `readiness/smoke/replay-readback-parity.md` (SC-003 / SC-004)
- [X] T028 [US3] [skillist: fs-skia-evidence-mode] Capture the forced-staleness proof on Mesa (a render-affecting change — theme/text/geometry/opacity/clip — flips the fingerprint, forces re-record, and never presents stale pixels) → `readiness/smoke/forced-staleness.md`, and document US3's independent validation path (SC-005 / FR-010)

**Checkpoint**: US3 functional — reuse-stable subtrees replay byte-identically with a collision-free key, proven on real hardware.

---

## Phase 6: User Story 4 (US4) — Audit honesty & correctness defects resolved (P3)

### Tests First

- [X] T029 [P] [US4] [skillist: fs-skia-ui-widgets] Add the `DirtyArea` union test (failing-first): for two overlapping repainted regions the metric equals the **union** area (not the sum) and never exceeds the frame area (SC-007 / FR-015)

### Implementation

- [X] T030 [US4] [skillist: fs-skia-reconciliation] Implement the `DirtyArea` sum → union computation in `RetainedRender.fs` (FR-015), clamped to the frame area
- [X] T031 [P] [US4] [skillist: fs-skia-skiaviewer] Finalize the corrected `ViewerOptions.PresentMode` docstring in `SkiaViewer.fsi` so it names the shipped `DirectToSwapchain` default (FR-016) — the `.fsi` body matching the T006 draft
- [X] T032 [P] [US4] [skillist: fs-skia-samples] Set the DemoReel live window `viewerOptions` to `ViewerPresentMode.DirectToSwapchain` (FR-017) in `samples/DemoReel/Program.fs`, leaving the evidence/screenshot capture path on `OffscreenReadback`
- [X] T033 [P] [US4] [skillist: fs-skia-controls-host] Remove the dead, written-never-read `lastRuntimeStateTouched` reference from the interactive host (`ControlsElmish.fs`, FR-018) — behavior-neutral, deterministic goldens unchanged
- [X] T034 [US4] [skillist: fs-skia-skiaviewer] Launch the **persistent** DemoReel interactive window from the default executable path on a GPU-passthrough machine and confirm via the host's own present diagnostic that it presents with `readback=false` in `DirectToSwapchain` → `readiness/smoke/present-mode.md` + `readiness/window-visibility.md` (SC-008)

**Checkpoint**: US4 functional — present-mode docs/sample honest, damage metric is a true union, dead bookkeeping gone.

---

## Phase 7: Integration & Polish

- [X] T035 [skillist: fsharp-build-orchestration] Run `./fake.sh build -t Route` then `./fake.sh build -t Dev` (Scene/Controls/Elmish/SkiaViewer + Feature120 tests); record the red→green evidence log and the selected governance risk level
- [X] T036 [skillist: fs-skia-template-update] Run `./fake.sh build -t GeneratedProductCheck` then `./fake.sh build -t TemplateCheck` sequentially; record the expected template pin-lag failure pre-merge and document the deferral to the post-merge `/fs-skia-template-update` re-pin (116–119 pattern), not in this merge scope
- [X] T037 [skillist: fs-skia-skiaviewer] Refresh surface baselines and run the controls/package public-surface gates (`PackageSurfaceCheck`/`PerPackageSurfaceDiff`) — additive Tier-1 delta only (Scene case + FrameMetrics fields + docstring), nothing removed; author `readiness/governance-risk-levels.md` + `readiness/aggregate-hang-diagnostics.md`
- [X] T038 [skillist: fs-skia-evidence-mode] Update the perf-corpus count goldens for the new replay counters (`ReplayHit/Miss/Record/SkippedNode/CacheNativeBytes`) and confirm the existing count lines and the timing fields stay out of goldens — deterministic surface byte-identical (FR-002)
- [X] T039 [skillist: fsharp-code-generation] Author `readiness/skill-loading-evidence.md` (one row per task/skill load) and `readiness/focused-gates.md` (the focused gate per risk level), so skill-loading and focused-validation evidence is complete before the audit
- [X] T040 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; record graph before/after paths
- [X] T041 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS, **0 synthetic**, no diff-scan hits, `generated-validation.md` package-resolution=resolved (SC-009)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. **None planned** (target: 0
synthetic). All proof is real: real `SKPicture` record/replay readback on Mesa, real
`Stopwatch`/GC timing, real corpus goldens. The structural-collision and forced-staleness tests
(T020, T028) use constructed-but-real structural inputs producing real cache misses and real
readback pixels — real evidence, not a synthetic error path, so no `[SEH]` row applies.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
