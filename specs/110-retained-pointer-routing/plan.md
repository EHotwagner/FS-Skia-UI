# Implementation Plan: Retained-Frame Pointer Routing (Remove Full-Render Pointer Hot Path)

**Branch**: `110-retained-pointer-routing` | **Date**: 2026-06-12 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/110-retained-pointer-routing/spec.md`

## Summary

Pointer routing on the live host currently rebuilds a full immediate-mode control
tree per routed sample: `routeInteractivePointer` calls
`Control.renderTree host.Theme size (host.View size model)`, evaluates layout,
hit-tests, and dispatches from that throwaway tree
(`src/Controls.Elmish/ControlsElmish.fs:241`). The deterministic corpus driver
does the same inside `Perf.runScript`'s `routeInteraction`
(`ControlsElmish.fs:1062`). This defeats the retained pipeline (features
091/092/096–103): the retained frame already carries stable per-node `RetainedId`,
cached per-node boxes (`retainedHitTest`), the already-evaluated `LayoutResult`
(`RetainedRender<'msg>.Layout`), and the frame's `ControlRenderResult`
(`EventBindings`, `BoundIds`, `Bounds`) — but only `s.Render.Scene` is consumed at
the step site (`ControlsElmish.fs:765,773`); `s.Render` itself is discarded.

**Technical approach (Phase 2 of the performance report, "Do first" #2):** route
pointer hit-testing and event-binding dispatch *from the retained frame* instead
of a fresh render. Concretely:

1. Retain the step's `ControlRenderResult` (`s.Render`) in a host-loop ref and as
   a carried value in `Perf.runScript`, so its `EventBindings`/`BoundIds`/`Bounds`
   are available to route without re-rendering.
2. Add an internal **retained-id → authored-control-id lookup** on the retained
   frame output so a `retainedHitTest` `RetainedId` resolves to the authored
   `ControlId` whose binding must fire (composite controls bind above the hit
   node).
3. Add an internal retained-aware route that runs `Pointer.update` over the
   retained frame's **cached** `LayoutResult` (not a freshly evaluated one),
   resolves each interaction via `retainedHitTest` + the new lookup +
   `EventBindings`, and falls back to `MapPointer` exactly as today — performing
   **zero** `host.View`/`Control.renderTree` rebuilds for routing.
4. Preserve the public `routeInteractivePointer` as the **parity oracle /
   counted fallback** (FR-007); wire `runInteractiveApp` and `Perf.runScript`
   onto the retained route.
5. Add a deterministic int `FullRenderFallbackCount` to `FrameMetrics` (breaking
   public `.fsi` change, FR-009); regenerate the feature-109 corpus goldens so
   their routing full-render counts drop to zero (FR-010).

This is a **hot-path mechanism change only** (FR-011): at-rest rendered output,
geometry, focus/keyboard semantics, and every dispatch outcome stay
byte-identical; the only intended observable deltas are fewer routing full-renders
and the new fallback metric.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: No new dependencies. Consumes the existing internal
`FS.Skia.UI.Controls` retained surface (`RetainedRender.step`/`retainedHitTest`/
`RetainedId`/`RetainedNode`/`RetainedRender<'msg>.Layout`) and the
`ControlRenderResult` (`Types.fsi:420`); edits `FS.Skia.UI.Controls.Elmish`.
**Testing**: Expecto + FsCheck (parity oracle, forced-fallback), the deterministic
`Perf.runScript` corpus goldens, FAKE targets. Tests reach internal
`RetainedRender` via `InternalsVisibleTo "Elmish.Tests"`
(`src/Controls.Elmish/Controls.Elmish.fsproj`, `src/Controls/Controls.fsproj`).
**Target Platform**: Windows and Linux (no platform-specific code; no
Vulkan/Skia/visual-output change).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification — Tier 1 (contracted change).** `FrameMetrics` gains a
public field (`FullRenderFallbackCount`) in `ControlsElmish.fsi`, so the full
artifact chain applies: `.fsi` update, surface + per-package baseline regeneration,
test evidence, XML-doc. `Route` escalates to the **controls-public-surface** tier.

**Principle compliance.**
- *I (Spec→FSI→Tests→Impl)*: the new `.fsi` field and any new internal seam are
  drafted in signature form first; parity tests exercise the public
  `routeInteractivePointer` oracle vs. the wired retained route through the
  `Perf.runScript` / adapter surface.
- *II (Visibility in `.fsi`)*: `RetainedRender` stays `internal` (its `.fsi` hides
  the surface); the new retained-id→authored-id lookup is internal; the only
  public surface delta is the `FrameMetrics` field. No access modifiers in `.fs`.
- *III (Idiomatic simplicity)*: plain functions over the existing retained data; a
  `mutable`/`ref` on the hot loop is already the established idiom here
  (`pendingMove`, `retained`, `lastWorkReduction` refs) and is disclosed at the
  use site. No SRTP/reflection/type-providers introduced.
- *IV (Elmish/MVU boundary)*: unchanged — `Update`, effects, subscriptions,
  commands, interpreter are untouched; only the routing *mechanism* that produces
  dispatched messages changes. Dispatch outcomes are byte-identical (FR-006/011).
- *V (Synthetic disclosure)*: none expected — parity uses the real preserved
  oracle, goldens are regenerated from the real corpus, and the forced-fallback
  test constructs a *real* unroutable case (not a mock). If any task can only be
  proven with a stub it is marked `[S]` with full disclosure.
- *VI (Test evidence)*: parity + forced-fallback + regenerated goldens fail before
  / pass after; no assertion weakening.
- *VII (Observability)*: `FullRenderFallbackCount` makes any deviation from the
  zero-full-render hot path observable rather than silent; the fallback degrades
  to the correct oracle dispatch rather than mis-dispatching.

### Repository Governance Decisions

- **Template ownership**: N/A — no `template/**`, sample, or command-surface
  change; the framework-internal routing mechanism does not alter
  `.template.config/template.json`. (The merge-time template package-pin bump is
  the standard post-merge step, not a content change in this feature.)
- **Dependency impact**: N/A — no new package; `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` are unchanged.
- **Command-surface impact**: No new gate. Escalated controls-public-surface set
  because of the `ControlsElmish.fsi` change; run `Route` first and obey its
  printed list. `RefreshSurfaceBaselines` must regenerate the surface +
  per-package baselines after the `FullRenderFallbackCount` addition. FAKE-backed
  commands run sequentially in the deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: N/A — generated default/minimal contents, selected
  Controls guidance, and generated `Dev` behavior are unchanged; the live host
  internals are not surfaced into generated projects beyond the (additive)
  `FrameMetrics` field already part of the public observability contract.
- **Evidence paths**: parity + forced-fallback tests under
  `tests/Elmish.Tests/Feature110*.fs`; regenerated corpus goldens under
  `specs/109-perf-metrics-baseline/readiness/perf-corpus/*.golden.txt`
  (regenerated via `PERF_CORPUS_REGEN=1`); before/after routing-full-render delta
  recorded in `specs/110-retained-pointer-routing/readiness/`; skill-loading
  evidence in `specs/110-retained-pointer-routing/readiness/skill-loading-evidence.md`;
  `specs/110-retained-pointer-routing/readiness/evidence-audit.md` (verdict token);
  generated-validation package-resolution tokens in the readiness area;
  surface/per-package baselines under `readiness/surface-baselines/`.
- **`.fsi` / contract impact**: `ControlsElmish.fsi` `FrameMetrics` gains
  `FullRenderFallbackCount: int` with XML-doc (doc-preservation gate). The public
  `routeInteractivePointer` signature is **retained unchanged** (oracle/fallback).
  Any retained-aware route is an **internal** seam (consuming internal
  `RetainedRender`), so no public signature gains an internal-typed parameter.
  Surface baseline + per-package baseline files update.
- **MVU/effect boundary**: Unchanged. `Model`/`Msg`/`Effect`/`init`/`update`/
  interpreter are untouched; this feature changes only how the host resolves which
  message to dispatch from a pointer sample, not the transition algebra. No new
  effect, command, or subscription. (Boundary identified as *preserved, not
  modified* — recorded here per the gate rather than left blank.)
- **Synthetic evidence**: None planned. Parity oracle = the real preserved
  full-render path; goldens = real corpus; forced-fallback = a real unroutable
  construction. Any unavoidable stub returns to task review for `[S]` disclosure.
- **Test evidence**: failing-first parity test (retained route vs. oracle: message
  lists, matched identity, focus outcome) across keyed / unkeyed-same-kind-sibling
  / composite / nested scenes; forced-fallback test (counter increments + oracle
  dispatch matches); regenerated goldens proving routing full-render counts → 0;
  a metrics test asserting `FullRenderFallbackCount = 0` for normal scenarios and
  that `FullRenderCount` no longer increments for routing.
- **Observability**: `FullRenderFallbackCount` (deterministic int, golden-asserted)
  + the narrowed `FullRenderCount` semantics (routing never increments it). Live
  `OnFrameMetrics` continues as the best-effort sink; `Perf.runScript` remains the
  authoritative byte-stable surface. No unsupported-environment message change.
- **Deferred scope**: Phase 3+ of the report is OUT — frame scheduler
  (`FrameCause`/`FrameInvalidation`), narrowed visual-state stamping, view/control
  memoization, viewport virtualization, damage rects / picture caches, text /
  layout-boundary caches, and `SkiaViewer` backend review. The full-render path is
  **not** removed (preserved as oracle/fallback). No renderer rewrite, no
  Avalonia/WPF redesign, no platform/release/distribution scope.

**Gate result: PASS.** No unjustified violations. Tier 1 obligations (`.fsi`,
baselines, tests, docs) are enumerated above and carried into Phase 1.

## Project Structure

Edited / added paths for this feature:

```
src/Controls.Elmish/
  ControlsElmish.fsi          # FrameMetrics gains FullRenderFallbackCount (+ XML-doc)
  ControlsElmish.fs           # retain s.Render in a ref; internal retained route;
                              #   wire runInteractiveApp + Perf.runScript onto it;
                              #   emit FullRenderFallbackCount; route via cached LayoutResult
src/Controls/
  RetainedRender.fsi          # (internal) retained-id -> authored-control-id lookup seam
  RetainedRender.fs           # build/expose the lookup from the step output

readiness/surface-baselines/
  FS.Skia.UI.Controls.Elmish.txt   # regenerated (RefreshSurfaceBaselines)
  (+ per-package baselines as the generator updates them)

specs/109-perf-metrics-baseline/readiness/perf-corpus/
  *.golden.txt                # regenerated (PERF_CORPUS_REGEN=1): routing renders -> 0

tests/Elmish.Tests/
  Feature110RetainedRoutingParityTests.fs   # FR-006 parity oracle vs retained route
  Feature110FallbackTests.fs                # FR-007/009 forced fallback + counter
  Feature109CorpusTests.fs                  # FrameMetrics serialize() gains the field
  Feature109MetricsHonestyTests.fs          # FullRenderCount narrowing / fallback=0

specs/110-retained-pointer-routing/
  spec.md  plan.md  research.md  data-model.md  quickstart.md
  contracts/frame-metrics.md  contracts/retained-routing.md
  readiness/   # evidence-audit.md, skill-loading-evidence.md, before/after delta
```

**Key seams (file:line anchors):**
- Retained step site that must retain `s.Render`: `ControlsElmish.fs:763-773`.
- Live route to replace: `routeInteractivePointer` `ControlsElmish.fs:207-273`
  (call site `ControlsElmish.fs:835`).
- Corpus route to replace: `routeInteraction` `ControlsElmish.fs:1058-1066`.
- Retained hit-test already used for focus: `resolveFocus` `ControlsElmish.fs:279`.
- Authored-binding resolution to mirror on retained identity:
  `bindingMessagesFor` / `Control.nearestAuthored` `ControlsElmish.fs:175-199`.
- `FrameMetrics` construction sites to update: `ControlsElmish.fs:804`, `1076`,
  `1107`, `1144`, `1162`, `1178`; test serialize `Feature109CorpusTests.fs:153`.

## Phase 0: Research

See [research.md](./research.md). Resolves: (a) how to bridge `RetainedId` →
authored `ControlId` to reproduce `nearestAuthored` resolution from retained
identity; (b) whether the retained frame's cached `LayoutResult` is the exact
input `Pointer.update` needs (so no fresh layout eval is required); (c) where the
fallback boundary lies and how the counter is threaded through the live loop and
the corpus driver; (d) the byte-identity argument for FR-011.

## Phase 1: Design & Contracts

- [data-model.md](./data-model.md): `FrameMetrics` (post-feature, with
  `FullRenderFallbackCount`), the retained-id→authored-id lookup entity, the
  preserved full-render oracle, and the retained route's read set.
- [contracts/frame-metrics.md](./contracts/frame-metrics.md): the breaking
  `.fsi` shape, field semantics, and every construction/read site to update.
- [contracts/retained-routing.md](./contracts/retained-routing.md): the internal
  retained-route contract, its parity obligation vs. the oracle, and the fallback
  rule.
- [quickstart.md](./quickstart.md): how to run the parity test, the forced-fallback
  test, regenerate goldens, and run the escalated gate set.
- Agent context update: `AGENTS.md` SPECKIT marker repointed to this plan.

## Phase 2: Planning complete

Stop after design. `tasks.md` is produced by `/speckit.tasks`.
