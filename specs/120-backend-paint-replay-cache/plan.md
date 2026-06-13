# Implementation Plan: Backend Paint Replay & Performance Honesty

**Branch**: `120-backend-paint-replay-cache` | **Date**: 2026-06-13 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/120-backend-paint-replay-cache/spec.md`

## Summary

Make the OpenGL backend paint path do honest, observable, work-skipping rendering, in the
report's own evidence-led order. Today the backend re-clears the surface and re-walks the
entire cached scene into SkiaSharp draw calls on **every** present (`SceneRenderer.paintNode`
via `GlHost.drawScene`), allocating an `SKPaint`/`SKPath` per primitive — even on idle frames
— while the controls-level fragment reuse never crosses into the backend, and feature 116's
"picture cache" only *counts* hits behind a truncation-prone `sprintf "%A"` key.

This feature delivers, in priority order:

1. **(US1, P1)** Per-frame paint-phase and present/compose-phase timing diagnostics, so the
   bottleneck is measured, not inferred (the report's stated precondition, never built).
2. **(US2, P1)** An unchanged-frame skip in the present interpreter: a frame with no dirty
   cause performs no clear/walk/draw-call re-issue.
3. **(US3, P2)** A load-bearing backend `SKPicture` record/replay cache keyed by a
   **collision-resistant structural fingerprint** (replacing `%A`), carried into the Scene IR
   via an additive transparent `CachedSubtree` node, gated on prior-frame reuse stability, and
   proven byte-identical by cache-on/off pixel readback parity.
4. **(US4, P3)** Honesty/correctness cleanups the audit surfaced: `DirtyArea` sum→union, stale
   `PresentMode` docstring, DemoReel live sample present mode, and the dead
   `lastRuntimeStateTouched` reference.

The governing invariant throughout: presented pixels and deterministic count goldens stay
byte-identical at rest; every render-path addition is additive and gated behind an
always-direct oracle.

**Change Tier**: **Tier 1 (contracted)** — public `.fsi` surface changes (`Scene.fsi` new
`CachedSubtree` case; `ControlsElmish.fsi` new `FrameMetrics` timing + replay fields;
`SkiaViewer.fsi` docstring; a new internal SkiaViewer cache module `.fsi`). Surface-area
baselines update.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: SkiaSharp 4 preview (`SKPictureRecorder` / `SKPicture` / `SKCanvas.DrawPicture`,
already transitively present via the GL host), Silk.NET GL (existing), Yoga (existing). No new package.
**Testing**: Expecto (unit/property/parity), FSI prelude transcripts, real-hardware pixel
readback on the Linux AMD/Mesa OpenGL reference environment, perf-corpus count goldens + the
non-golden live timing baseline generator.
**Target Platform**: Linux and Windows desktop OpenGL (per constitution). Live pixel-readback
and timing evidence captured on Linux/Mesa (feature 119 reference env); Windows GL portability
asserted by code path, not launch-verified (declared out of scope, see Deferred scope).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**I. Spec → FSI → Semantic Tests → Implementation** — Honored. Phase 1 drafts every `.fsi`
delta (Scene `CachedSubtree`, FrameMetrics fields, the SkiaViewer backend cache module, the
docstring) as contracts before `.fs` bodies; semantic tests exercise the packed surface (FSI
prelude + Expecto) and assert pixel/byte parity, not internals.

**II. Visibility in `.fsi`** — Honored. New public case lives in `Scene.fsi`; the backend
`SKPicture` cache is an internal SkiaViewer module with its own curated `.fsi`; the structural
fingerprint helper is `internal` in `RetainedRender.fsi`. No access modifiers in `.fs`. Both
top-level and per-package surface baselines regenerate (`RefreshSurfaceBaselines`).

**III. Idiomatic Simplicity** — Honored, with two disclosed hot-path mutation uses
(Principle III's measured-hot-path allowance): (a) the structural fingerprint accumulator
folds with a `mutable`/loop over scene nodes (`// mutable: hot path` at the use site); (b) the
backend cache is a bounded mutable dictionary of native `SKPicture` handles (native lifetime
demands mutation + explicit `Dispose`). No SRTP/reflection/type-providers/custom operators.

**IV. Elmish/MVU boundary** — Honored. The behavioral change lives at the viewer's existing
MVU/effect edge: the unchanged-frame skip is a pure decision in `update` (do not emit
`RenderFrame` when no dirty cause), and record/replay executes inside the `RenderFrame`
interpreter (`interpretEffect`/`renderFrameDirect`), never inside `update`. Model gains a
frame-dirty signal; no new consumer-facing `Msg`/`Cmd` contract. Pure-transition tests assert
"unchanged + no cause ⇒ no RenderFrame effect"; interpreter tests assert real readback parity.

**V. Synthetic evidence** — Target **0 synthetic**. All proof is real: real `SKPicture`
record/replay readback on Mesa, real `Stopwatch`/GC timing, real corpus goldens. The only
candidate `[SEH]` is a forced-staleness negative test (constructed structural-collision input
to prove the new key misses where `%A` would have falsely hit) — classified at task time, not
implementation time, if it qualifies.

**VI. Test evidence** — Honored. Each FR gets a failing-first semantic test (idle zero-redraw,
cache-on/off parity, forced-staleness re-record, DirtyArea union, docstring/sample, dead-ref
removal). Parity uses the always-direct oracle (FR-011) mirroring the existing
`PictureCacheEnabled`/`TextCacheEnabled` pattern.

**VII. Observability & safe failure** — Honored and extended: paint/compose durations, replay
hit/miss/record/skipped counts, native cache bytes, the host's existing `readback=false`
present diagnostic. A failed `SKPicture` record degrades explicitly to the direct walk (never a
blank or stale subtree); the idle-skip degrades to painting if the dirty signal is uncertain.

### Repository Governance Decisions

- **Template ownership**: `.template.config/template.json` content is **unchanged this
  feature** — no new source files ship *into* generated projects (the backend cache and Scene
  case live in framework packages the template already consumes). A package **version bump +
  template re-pin** is a required post-merge follow-up via `/fs-skia-template-update` (same
  pattern as 116–119), not a template-content edit here. N/A for new template fragments.
- **Dependency impact**: **N/A — no dependency change.** `SKPictureRecorder`/`SKPicture` are
  already in the pinned SkiaSharp 4 preview surface; `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` coverage are untouched.
- **Command-surface impact**: No `build.fsx`/wrapper/target *definitions* change. The routed
  gate set runs (escalated by `.fsi` changes): `Dev`, controls/package public-surface gates,
  `GeneratedProductCheck`, `TemplateCheck`, `EvidenceGraph`, `EvidenceAudit`. FAKE-backed
  targets run **sequentially** in the documented order; `Route` is run first and only its
  printed gates are run. Safe non-FAKE reads may parallelize.
- **Generated project impact**: Default/minimal generated contents are **unchanged**; the
  feature changes framework package internals + additive public surface the generated app
  consumes after re-pin. Generated `Dev`, placeholder scans, and excluded-history scans are
  unaffected. The interactive **DemoReel sample** present mode changes (FR-017) — a sample, not
  generated-project default content.
- **Evidence paths**: `specs/120-backend-paint-replay-cache/readiness/` —
  `smoke/replay-readback-parity.md` (cache on/off pixel parity on Mesa, per corpus scene),
  `smoke/idle-zero-redraw.md` (paint diagnostic shows 0 paint work on unchanged frames),
  `smoke/forced-staleness.md` (render-affecting change re-records), `smoke/present-mode.md`
  (sample launches readback=false + docstring), `perf-corpus/*.golden.txt` updates (new replay
  counters; `dirty-area` union), `docs/reports/_baselines/2026-06-13-paint-replay-{before,after}.md`
  (live paint/compose timing), `skill-loading-evidence.md`, `evidence-audit.md`,
  `evidence-graph.md`, `focused-gates.md`.
- **`.fsi` / contract impact**: **Tier 1.** Changes: `src/Scene/Scene.fsi` (+`CachedSubtree`
  case, transparent fallback documented); `src/Controls.Elmish/ControlsElmish.fsi`
  (+`PaintDuration`, `+ComposeDuration` non-golden `TimeSpan`; `+ReplayHitCount`,
  `+ReplayMissCount`, `+ReplayRecordCount`, `+ReplaySkippedNodeCount`, `+ReplayCacheNativeBytes`);
  `src/SkiaViewer/SkiaViewer.fsi` (corrected `PresentMode` docstring; possible replay-enable
  seam on `ViewerOptions` for the parity test); new `src/SkiaViewer/PictureReplayCache.fsi`
  (internal); `src/Controls/RetainedRender.fsi` (internal structural-fingerprint helper;
  `CachedSubtree` emission). Surface baselines (top-level + per-package) regenerate; migration
  note: additive only, no removals, no signature breaks.
- **MVU/effect boundary**: The viewer host is the boundary. `Model` gains a **frame-dirty
  signal** (set by model message / resize / theme / active animation clock; cleared after a
  present). `Msg`: existing `RenderTick`/`LegacyRenderTick` reused; no new public message.
  `Effect`: existing `RenderFrame scene` reused; emitted **conditionally** (skip when not
  dirty and no animation). `init`: unchanged (first frame always paints). `update`: pure
  decision to emit-or-skip `RenderFrame`. **Interpreter** (`interpretEffect`/`renderFrameDirect`):
  executes record/replay against the real GL `SKSurface`. Evidence: pure-transition test
  (no-cause ⇒ no `RenderFrame`) + real interpreter readback parity.
- **Synthetic evidence**: None planned (`0` target). Any forced-collision negative test is
  evaluated for `[SEH]` (`synthetic-error-handling-approved`) at task generation with a named
  Synthetic-Evidence-Inventory row; no implementation-time relabeling. Convenience mocks are
  forbidden — readback runs on real hardware.
- **Test evidence**: Failing-first semantic tests per FR (idle zero-redraw; replay parity
  on/off; forced re-record; structural-collision miss; DirtyArea union; docstring/sample;
  dead-ref absence). Governance: surface-baseline tests updated for the new public fields/case.
  Host smoke: Mesa readback parity + present diagnostic. Target-level: `EvidenceGraph` +
  `EvidenceAudit` PASS.
- **Observability**: New live diagnostics — `PaintDuration`/`ComposeDuration` (non-golden),
  replay hit/miss/record/skipped counts and native cache bytes (golden counters), retained
  `readback=false` present line. Missing-artifact behavior: the timing baseline generator
  records a `MissingCounters:` line if a phase can't be timed (honesty, per 109 precedent). A
  failed picture record logs and falls back to the direct walk (no silent blank).
- **Deferred scope** (bounded follow-ups, explicit): (1) damage-rect **GPU clip** of the
  redraw (depends on FR-015's union; this feature ships the union metric only); (2)
  render-thread/compositor split (report "do later", gated on CPU-bound metrics); (3) **Windows
  GL launch evidence** (no Windows reference host; portability asserted by code path, residual
  risk noted); (4) broadening the replay-boundary selection heuristic beyond the prior-frame
  stability gate.

## Project Structure

```
src/Scene/
  Scene.fsi              # + CachedSubtree case (public, additive); doc: transparent when replay off
  Scene.fs               # + CachedSubtree; describe/diagnostics/measure see through it
src/Controls/
  RetainedRender.fsi     # internal structural-fingerprint helper; CachedSubtree emission seam
  RetainedRender.fs      # replace %A digest -> structural hash (memoized on fragment);
                         #   emit CachedSubtree for prior-frame-stable subtrees; DirtyArea union;
                         #   reduction/virtual walks see through CachedSubtree
src/Controls.Elmish/
  ControlsElmish.fsi     # + PaintDuration/ComposeDuration (non-golden) + Replay* counters
  ControlsElmish.fs      # thread the new fields; remove dead lastRuntimeStateTouched ref (FR-018);
                         #   conditional RenderFrame emission feeds the metrics
src/SkiaViewer/
  PictureReplayCache.fsi # NEW internal: bounded LRU of SKPicture by CacheId+Fingerprint
  PictureReplayCache.fs  # NEW: record-on-miss / replay-on-hit; Dispose on evict/replace
  SceneRenderer.fs       # paintNode: CachedSubtree -> replay-or-record (transparent when off)
  SkiaViewer.fsi         # corrected PresentMode docstring; optional replay-enable seam
  SkiaViewer.fs          # frame-dirty signal; conditional RenderFrame; phase timing capture
  Host/OpenGl.fs         # renderFrameDirect/drawScene: wire cache + paint/compose Stopwatch
samples/DemoReel/
  Program.fs             # FR-017: live viewerOptions -> DirectToSwapchain
tests/
  Controls.Tests/        # fingerprint structural-equality + collision-miss; DirtyArea union
  Elmish.Tests/          # FrameMetrics fields; idle-skip pure transition; corpus golden updates
  SkiaViewer.Tests/      # PictureReplayCache LRU/dispose; Mesa readback parity (on/off); present
specs/120-backend-paint-replay-cache/
  spec.md  plan.md  research.md  data-model.md  quickstart.md  contracts/  readiness/
```

**Capability skills in play** (full `skillist` is a `/speckit-tasks` gate): `fs-skia-scene`
(Scene IR case), `fs-skia-skiaviewer` + `fs-skia-evidence-mode` (backend present/readback
parity, host-warning honesty), `fs-skia-ui-widgets` + `fs-skia-reconciliation` (RetainedRender
fingerprint/emission), `fs-skia-controls-host` (interactive host metrics/dirty signal),
`fs-skia-samples` (DemoReel), `fs-skia-testing` (packed-surface validation).

## Phase 0 — Research

See [research.md](./research.md): resolves the fingerprint algorithm, the `CachedSubtree`
placement (IR wrapper vs. side-channel) and its golden/byte-identity impact, the idle-skip
dirty-signal mechanism on a double-buffered GL surface, the backend replay-enable seam for the
parity oracle, and the `DirtyArea` union computation. No unresolved NEEDS CLARIFICATION remain.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md) — entities: `CachedSubtree` node, subtree fingerprint,
  `PictureReplayCache` + entry, frame paint record, frame-dirty signal; with validity rules.
- [contracts/contracts.md](./contracts/contracts.md) — the exact `.fsi` deltas and the backend
  cache module contract.
- [quickstart.md](./quickstart.md) — how to run the parity readback, the idle-skip proof, and
  the timing baseline locally.
- Agent context: `AGENTS.md` SPECKIT marker updated to point at this plan.

## Phase 2 — Implementation ordering (preview; `/speckit-tasks` expands)

US1 (timing) → US4-metric (`DirtyArea` union, cheap, unblocks honest damage) → US2 (idle-skip)
→ US3 (fingerprint fix → `CachedSubtree` IR → backend cache → parity proof) → US4-cleanups
(docstring, sample, dead ref). US3's fingerprint fix lands before the backend cache so the key
is validated against the old `%A` behavior first (a Controls-only diff) before any backend
work depends on it.
