# Implementation Plan: Frame Scheduler & Phase-Invalidation Model (Explain and Schedule Frames by Cause)

**Branch**: `111-frame-scheduler-invalidation` | **Date**: 2026-06-12 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/111-frame-scheduler-invalidation/spec.md`

## Summary

The live host and the deterministic `Perf.runScript` driver produce a per-frame
`FrameMetrics`, but the record cannot say **why** a frame ran or **which phases** it
needed, and the live repaint (`renderRetained`, `ControlsElmish.fs:867`) and the
corpus driver's animation tick (`renderStep`, `ControlsElmish.fs:1175`) still call
`host.View size model` even when the **product model did not change** — re-running
the consumer view and full-tree stamp for a host-owned hover / focus / animation
change (source report §"Gaps").

**Technical approach (Phase 3 of the performance report, "Do next" #1):**

1. Add a public, closed `FrameCause` discriminated union
   (`Idle | PointerMove | PointerDiscrete | Key | Tick | Resize | Theme`,
   `RequireQualifiedAccess`) and three boolean **phase** fields (`DiffRan`,
   `LayoutRan`, `PaintRan`) to `FrameMetrics`; the existing `ViewCalled` is the
   **view** phase, so the phase record is `{ ViewCalled, DiffRan, LayoutRan,
   PaintRan }` (clarified: hit-test is **not** a phase field). Breaking public
   `.fsi` change (FR-001/FR-002/FR-010).
2. Wire a **frame scheduler** that classifies each produced frame's cause and runs
   **only the phases the cause requires** — concretely, a frame whose cause did
   **not** change the product model **skips `host.View`** and reuses the view tree
   already produced for the unchanged model:
   - **Perf driver**: an animation-only tick reuses `prev.Root.Control` (the
     retained tree, which equals `host.View` of the unchanged model) and steps it to
     re-sample the overlay — `ViewCalled = false`, `PaintRan = true`
     (`ControlsElmish.fs:1273-1306`, the `[ FrameInput.Tick delta ]` branch).
   - **Live loop**: `renderRetained` caches the **un-stamped** `host.View size model`
     output keyed by `(model-reference, size)`; on a paint where the model instance
     and size are unchanged it reuses the cached tree, still re-applies the full-tree
     `applyRuntimeVisualState` stamp and `RetainedRender.step`, and skips only the
     `host.View` call (`ViewCalled = false`).
3. Set the `FrameCause` + phase fields at **every** `FrameMetrics` construction site
   (Perf `zero`/move/tick/key/discrete; live `emitFrameMetrics`) and add them to the
   golden `serialize()`; regenerate the feature-109 corpus goldens so animation/tick
   frames become view-free and every frame carries its cause + phase record (FR-007/
   FR-010).
4. Keep the full-tree runtime visual-state stamp (Phase 4 deferred, FR-009) and the
   feature-110 retained routing + oracle/fallback unchanged.

This is a **scheduling/observability change only** (FR-008): at-rest rendered
output, geometry, focus/keyboard semantics, and every dispatch outcome stay
byte-identical because `host.View` is a pure function of `(model, size)` — reusing
its output on an unchanged `(model, size)` is identical to re-running it. The only
intended observable deltas are the new cause/phase fields and the elimination of
redundant `host.View` work on model-unchanged frames (`ViewCalled`/`FullRenderCount`
drop to `false`/`0` on animation ticks; the overlay fact moves to `PaintRan`).

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: No new dependencies. Consumes the existing internal
`FS.Skia.UI.Controls` retained surface (`RetainedRender.step`/`init`, the retained
tree `RetainedRender<'msg>.Root.Control`); edits `FS.Skia.UI.Controls.Elmish`.
**Testing**: Expecto + FsCheck (cause/phase classification, view-skip byte-identity,
the deterministic `Perf.runScript` corpus goldens), FAKE targets. Tests reach
internal `RetainedRender` via `InternalsVisibleTo "Elmish.Tests"`.
**Target Platform**: Windows and Linux (no platform-specific code; no
Vulkan/Skia/visual-output change).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification — Tier 1 (contracted change).** `FrameMetrics` gains a
public `FrameCause` field + three phase booleans, and a **new public `FrameCause`
DU** is added, in `ControlsElmish.fsi`, so the full artifact chain applies: `.fsi`
update, surface + per-package baseline regeneration, test evidence, XML-doc. `Route`
escalates to the **controls-public-surface** tier.

**Principle compliance.**
- *I (Spec→FSI→Tests→Impl)*: the new `FrameCause` DU and `FrameMetrics` fields are
  drafted in signature form first and exercised from FSI; the cause/phase classifier
  and the view-skip are proven through the public `Perf.runScript` surface.
- *II (Visibility in `.fsi`)*: `FrameCause` + the new fields are declared in
  `ControlsElmish.fsi`; any internal scheduler/cache seam stays internal (its
  visibility lives in the `.fsi`). No access modifiers in `.fs`.
- *III (Idiomatic simplicity)*: a closed DU + boolean fields + a `mutable`/`ref`
  view-tree cache on the hot loop (the established idiom here — `pendingMove`,
  `retained`, `lastRender` refs — disclosed at the use site). No SRTP/reflection/
  type-providers introduced. `RequireQualifiedAccess` on `FrameCause` is justified
  (its case names `Key`/`Tick`/`Idle` would shadow a consumer's `Msg` on `open`,
  exactly as `FrameInput` already requires).
- *IV (Elmish/MVU boundary)*: unchanged — `Update`, effects, subscriptions,
  commands, interpreter are untouched; only the *scheduling* of which phases run, and
  the cause/phase observability, change. Dispatch outcomes byte-identical (FR-008).
- *V (Synthetic disclosure)*: none expected — cause/phase classification and the
  view-skip are proven on the real `Perf.runScript`/retained pipeline; byte-identity
  is proven by structural scene comparison against the real pre-feature render. If
  any task needs a stub it is marked `[S]` with full disclosure.
- *VI (Test evidence)*: failing-first cause/phase tests + view-skip byte-identity +
  regenerated goldens fail before / pass after; the feature-109 honesty test is
  updated to the new phase record (scope narrowed, not assertion weakened).
- *VII (Observability)*: `FrameCause` + the phase record make every non-idle frame
  explain itself by cause and skipped phases; a regression that reintroduces a
  skipped phase fails the deterministic golden rather than passing silently.

### Repository Governance Decisions

- **Template ownership**: N/A — no `template/**`, sample, or command-surface change;
  the framework-internal scheduler/observability change does not alter
  `.template.config/template.json`. (The merge-time template package-pin bump is the
  standard post-merge step, not a content change in this feature.)
- **Dependency impact**: N/A — no new package; `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` are unchanged.
- **Command-surface impact**: No new gate. Escalated controls-public-surface set
  because of the `ControlsElmish.fsi` change (new `FrameCause` type + `FrameMetrics`
  fields); run `Route` first and obey its printed list. `RefreshSurfaceBaselines`
  must regenerate the surface **and** per-package baselines — note the top-level
  surface baseline `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt`
  **does** change this time (a new public `FrameCause` type + its cases), unlike
  feature 110 (which added only a field + internal vals). FAKE-backed commands run
  sequentially in deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: N/A — generated default/minimal contents, selected
  Controls guidance, and generated `Dev` behaviour are unchanged; the live host
  internals are not surfaced into generated projects beyond the (additive)
  `FrameMetrics`/`FrameCause` observability already part of the public contract.
- **Evidence paths**: cause/phase + view-skip tests under
  `tests/Elmish.Tests/Feature111*.fs`; regenerated corpus goldens under
  `specs/109-perf-metrics-baseline/readiness/perf-corpus/*.golden.txt`
  (regenerated via `PERF_CORPUS_REGEN=1`); before/after view-free delta recorded in
  `specs/111-frame-scheduler-invalidation/readiness/`; skill-loading evidence in
  `specs/111-frame-scheduler-invalidation/readiness/skill-loading-evidence.md`;
  `specs/111-frame-scheduler-invalidation/readiness/evidence-audit.md` (verdict
  token); generated-validation package-resolution tokens; surface/per-package
  baselines under `readiness/surface-baselines/` + `readiness/per-package-surface/`.
- **`.fsi` / contract impact**: `ControlsElmish.fsi` gains a public
  `[<RequireQualifiedAccess>] type FrameCause` and `FrameMetrics` gains
  `FrameCause: FrameCause` + `DiffRan: bool` + `LayoutRan: bool` + `PaintRan: bool`,
  all XML-doc'd (doc-preservation gate). `ViewCalled`'s XML-doc narrows (precise
  meaning kept; value flips to `false` on model-unchanged frames). No public function
  signature gains an internal-typed parameter; any view-tree cache / classifier seam
  is internal. Surface baseline + per-package baseline files update.
- **MVU/effect boundary**: Unchanged (preserved, not modified). `Model`/`Msg`/
  `Effect`/`init`/`update`/interpreter are untouched; this feature changes only how
  the host schedules per-frame phases and reports cause/phase, not the transition
  algebra. No new effect, command, or subscription.
- **Synthetic evidence**: None planned. Cause/phase are read from the real
  `Perf.runScript` fold; the view-skip byte-identity is proven by comparing the real
  rendered scene before/after; the regenerated goldens come from the real corpus. Any
  unavoidable stub returns to task review for `[S]` disclosure.
- **Test evidence**: failing-first cause-classification test (every frame's
  `FrameCause` matches its trigger), phase-record test (idle = all phases false;
  animation tick = view false / paint true; model frame = view+diff+layout+paint),
  view-skip byte-identity test (animation tick + hover frame render byte-identically
  while `host.View` did not run), regenerated goldens proving the new fields +
  view-free tick frames, and the updated feature-109 honesty test asserting the new
  phase record on the animation tick.
- **Observability**: `FrameCause` (closed DU) + `DiffRan`/`LayoutRan`/`PaintRan`
  (deterministic bools, golden-asserted) + the narrowed `ViewCalled`/`FullRenderCount`
  semantics (both `false`/`0` on a model-unchanged frame). Live `OnFrameMetrics`
  continues as the best-effort sink; `Perf.runScript` remains the authoritative
  byte-stable surface. No unsupported-environment message change.
- **Deferred scope**: Phase 4+ is OUT — narrowed per-identity runtime visual-state
  stamping (Phase 4; Phase 3 keeps the full-tree stamp, FR-009), view/control
  memoization (Phase 5), viewport virtualization (Phase 6), damage rects / picture /
  paint caches (Phase 7), text / layout-boundary caches (Phase 8), `SkiaViewer`
  backend review (Phase 9). No granular per-phase node-count fields beyond the
  ran/skipped record + the counts that already exist. No renderer rewrite, no
  Avalonia/WPF redesign, no platform/release/distribution scope. Feature 110's
  retained routing + full-render oracle/fallback are unchanged.

**Gate result: PASS.** No unjustified violations. Tier 1 obligations (`.fsi`,
baselines, tests, docs) are enumerated above and carried into Phase 1.

## Project Structure

Edited / added paths for this feature:

```
src/Controls.Elmish/
  ControlsElmish.fsi          # NEW FrameCause DU; FrameMetrics gains FrameCause + DiffRan/LayoutRan/PaintRan; ViewCalled doc narrowed
  ControlsElmish.fs           # mirror the types; classify FrameCause + phases per frame; reuse cached view tree on model-unchanged frames
                              #   (renderRetained live cache; renderStep/tick branch in Perf); set cause/phases at every construction site

readiness/surface-baselines/
  FS.Skia.UI.Controls.Elmish.txt   # regenerated — gains the FrameCause type + its cases (RefreshSurfaceBaselines)
readiness/per-package-surface/
  FS.Skia.UI.Controls.Elmish.fsi.txt  # regenerated — FrameMetrics fields + FrameCause

specs/109-perf-metrics-baseline/readiness/perf-corpus/
  *.golden.txt                # regenerated (PERF_CORPUS_REGEN=1): +FrameCause +DiffRan/LayoutRan/PaintRan; tick frames view-free

tests/Elmish.Tests/
  Feature111FrameCauseTests.fs        # FR-001 cause classification (US1)
  Feature111PhaseRecordTests.fs       # FR-002 phase ran/skipped (US2)
  Feature111ViewSkipTests.fs          # FR-003/FR-004 view-free model-unchanged frames + byte-identity (US3)
  Feature109CorpusTests.fs            # serialize() gains FrameCause + phase bools
  Feature109MetricsHonestyTests.fs    # animation-tick ViewCalled flip → assert the new phase record

specs/111-frame-scheduler-invalidation/
  spec.md  plan.md  research.md  data-model.md  quickstart.md
  contracts/frame-cause-phases.md  contracts/frame-scheduler.md
  readiness/   # evidence-audit.md, skill-loading-evidence.md, before/after view-free delta, byte-identity authority
```

**Key seams (file:line anchors, post-feature-110):**
- `FrameMetrics` record: `ControlsElmish.fsi:48`, `ControlsElmish.fs:35`.
- Live repaint to make view-skipping: `renderRetained` `ControlsElmish.fs:867-878`
  (caches `lastRender` already; add the un-stamped view-tree cache + model/size key).
- Live metrics emit: `emitFrameMetrics` `ControlsElmish.fs:918`; classify cause in
  `mapPointer` `ControlsElmish.fs:973` and `wrappedTick` `ControlsElmish.fs:1102`.
- Perf driver: `renderStep` `ControlsElmish.fs:1175`; frame branches `zero`
  `ControlsElmish.fs:1231`, move `1247`, tick `1273` (the animation-only view-skip),
  key `1307`, discrete `1325`.
- Golden serialize: `Feature109CorpusTests.fs:153`.
- Animation-tick honesty assertion to update: `Feature109MetricsHonestyTests.fs`
  (the SC-011 "animation-only tick runs the view" test).

## Phase 0: Research

See [research.md](./research.md). Resolves: (a) the exact `FrameCause` taxonomy and
how each Perf frame branch + live input maps to a cause; (b) the four phase booleans'
precise semantics and why `ViewCalled` serves as the view phase (no duplicate
`ViewRan`); (c) the byte-identity argument for reusing the un-stamped view tree on a
model-unchanged frame (live) and `prev.Root.Control` on an animation tick (Perf);
(d) why `FullRenderCount`/`ViewCalled` drop on animation ticks and how the
feature-109 SC-011 contract is re-expressed via `PaintRan`; (e) which causes the
deterministic corpus exercises vs which are live-only (`Resize`/`Theme`).

## Phase 1: Design & Contracts

- [data-model.md](./data-model.md): `FrameCause` (closed DU), the extended
  `FrameMetrics` (with the phase record), the reused-view-tree cache entity, and the
  per-frame cause/phase classification table.
- [contracts/frame-cause-phases.md](./contracts/frame-cause-phases.md): the breaking
  `.fsi` shape (new DU + fields), the cause taxonomy, the phase-bool semantics, and
  every construction/read site to update.
- [contracts/frame-scheduler.md](./contracts/frame-scheduler.md): the internal
  scheduler contract — classify cause, run only required phases, reuse the view tree
  on model-unchanged frames — its byte-identity obligation and the live-vs-Perf
  view-skip mechanisms.
- [quickstart.md](./quickstart.md): how to run the cause/phase tests, the view-skip
  byte-identity test, regenerate goldens, and run the escalated gate set.
- Agent context update: `AGENTS.md` SPECKIT marker repointed to this plan.

## Phase 2: Planning complete

Stop after design. `tasks.md` is produced by `/speckit.tasks`.
