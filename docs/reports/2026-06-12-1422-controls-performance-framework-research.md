---
title: Controls Performance Framework Research and Implementation Plan
category: Reports
categoryindex: 9
---

# Controls Performance Framework Research and Implementation Plan

- Generated: 2026-06-12 14:22:02 CEST (+0200)
- Status: Analysis and planning only. No product code changes are proposed or made by this report.
- Scope: `FS.Skia.UI.Controls` and `FS.Skia.UI.Controls.Elmish` live controls performance, with emphasis on how mature declarative and retained UI frameworks avoid avoidable per-frame work.
- Local context: Feature 108 (`specs/108-focus-and-perf-feedback/plan.md`) is active and already targets focus visibility, per-frame metrics, deterministic perf scripting, and pointer-move coalescing. This report treats that work as in-progress context and plans follow-up performance architecture beyond it.

## Executive Summary

The controls stack has real performance risk. The current retained path is directionally correct: stable identities, a keyed diff, incremental layout, cached paint fragments, per-identity animation clocks, and work-reduction metrics are exactly the foundations used by higher-performance UI frameworks. The problem is that the framework still has several immediate-mode escape hatches around that retained core:

- Pointer routing can still rebuild/render a full control tree just to hit-test and dispatch.
- The host can still call `host.View` and restamp runtime state more broadly than the actual visual change requires.
- Fragment reuse is CPU-side scene-list reuse, not a full retained render/compositor scene graph with damage regions, layers, and backend scheduling.
- Large repeated controls do not have a framework-level virtualization story.
- Metrics are beginning to exist, but the performance model is not yet phase-complete enough to explain where a frame spent time or why work was scheduled.

Research across React, Flutter, Jetpack Compose, SwiftUI, Avalonia/WPF, Qt Quick, and browser rendering points to the same architecture: keep stable identity, split work into phases, track dependencies and invalidations at the smallest useful scope, coalesce high-rate input to frame boundaries, virtualize large collections, cache draw/layer work, and profile before optimizing. FS.Skia.UI should continue evolving toward that model rather than replacing the MVU/control architecture.

The highest-return plan is:

1. Finish and harden feature 108 metrics/coalescing, but verify that metrics describe the real work done.
2. Remove full-render pointer routing from the hot path by routing hit-testing and bindings from the retained frame.
3. Add a phase-complete frame work record: view, diff, layout, paint, compose, hit-test, allocations, and scheduling cause.
4. Add a frame scheduler that batches input and only advances work needed for the frame cause.
5. Add viewport virtualization for repeated controls, especially DataGrid/ListView-like surfaces.
6. Add retained draw/layer caches and damage rectangles once metrics identify paint/composition as the bottleneck.

## Research Method

I used primary or official framework documentation where possible, then mapped the recurring patterns back to the current FS.Skia.UI implementation. Key sources reviewed:

- React reconciliation and memoization: [Reconciliation](https://legacy.reactjs.org/docs/reconciliation.html), [memo](https://react.dev/reference/react/memo), [useMemo](https://react.dev/reference/react/useMemo)
- Flutter rendering and performance: [Inside Flutter](https://docs.flutter.dev/resources/inside-flutter), [Performance best practices](https://docs.flutter.dev/perf/best-practices)
- Jetpack Compose performance: [Compose performance](https://developer.android.com/develop/ui/compose/performance), [Compose phases](https://developer.android.com/develop/ui/compose/phases), [Strong skipping](https://developer.android.com/develop/ui/compose/performance/stability/strongskipping), [Stability](https://developer.android.com/develop/ui/compose/performance/stability), [Best practices](https://developer.android.com/develop/ui/compose/performance/bestpractices)
- SwiftUI performance: [Demystify SwiftUI](https://developer.apple.com/videos/play/wwdc2021/10022/), [Demystify SwiftUI performance](https://developer.apple.com/videos/play/wwdc2023/10160/), [Optimize SwiftUI performance with Instruments](https://developer.apple.com/videos/play/wwdc2025/306/)
- Avalonia and WPF: [Avalonia architecture](https://docs.avaloniaui.net/docs/fundamentals/architecture), [Avalonia performance optimization](https://docs.avaloniaui.net/docs/app-development/performance), [WPF layout performance](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/optimizing-performance-layout-and-design), [WPF 2D graphics and imaging](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/optimizing-performance-2d-graphics-and-imaging)
- Qt Quick: [Qt Quick scene graph](https://doc.qt.io/qt-6/qtquick-visualcanvas-scenegraph.html), [Qt Quick performance](https://doc.qt.io/qt-6/qtquick-performance.html), [QQuickWidget performance considerations](https://doc.qt.io/qt-6/qquickwidget.html), [Qt embedded performance guidance](https://doc.qt.io/qt-6/embedded-linux.html)
- Browser/input rendering: [web.dev rendering performance](https://web.dev/articles/rendering-performance), [Jank busting for better rendering performance](https://web.dev/articles/speed-rendering), [MDN PointerEvent.getCoalescedEvents](https://developer.mozilla.org/en-US/docs/Web/API/PointerEvent/getCoalescedEvents), [W3C Pointer Events](https://www.w3.org/TR/pointerevents/)
- Skia display-list precedent: [React Native Skia Pictures](https://shopify.github.io/react-native-skia/docs/shapes/pictures/), [Skia documentation](https://skia.org/docs/)

## What Other Frameworks Do

### React: O(n) Diffing, Keys, and Measured Memoization

React avoids an optimal tree-edit distance algorithm because generic tree diffing is too expensive for UI. Its reconciliation docs describe an O(n) heuristic based on element type and developer-supplied keys. The key lesson for FS.Skia.UI is not "copy React"; it is that identity must be explicit enough to make the cheap heuristic correct. If keys are unstable or positional, updates become slow and state can attach to the wrong item.

React's modern `memo`, `useMemo`, and compiler story is equally relevant. React normally re-renders children when a parent re-renders; `memo` only helps when props are unchanged and the render is expensive. React's own docs warn that one always-new prop can defeat memoization. That maps directly to `Control<'msg>` attributes: if event bindings, untyped values, or derived lists are recreated every frame and compare unequal, the retained path cannot skip work.

Implication for FS.Skia.UI:

- Keep stable keys as a required performance contract for repeated controls.
- Make attribute equality and event binding stability visible in diagnostics.
- Avoid a blanket memoization feature until metrics prove expensive view construction; when added, memoization must be keyed by explicit dependencies, not object identity accidents.

### Flutter: Three Trees, One Layout Pass, Repaint Boundaries, and Lazy Lists

Flutter separates the declarative Widget tree from the Element tree and the RenderObject tree. The docs explain that this separation lets layout walk only relevant render objects instead of all composition nodes. Flutter also designs layout for one pass per frame and sublinear layout on updates. Animations can directly invalidate paint rather than causing build plus layout.

Flutter's practical guidance is also concrete: avoid expensive build work, use lazy list/grid builders, avoid intrinsic layout passes, use opacity/clipping/saveLayer cautiously, and use repaint isolation where appropriate. Those are not micro-optimizations; they are phase-boundary optimizations.

Implication for FS.Skia.UI:

- The current retained tree is the right analog to Flutter's Element/RenderObject retained state, but pointer routing must stop falling back to full render.
- `VisualState` animation should stay paint-only when possible; it should not rebuild the consumer view or relayout unaffected ancestors.
- Large collection controls need builder/viewport semantics, not just a faster diff over a huge fully-materialized control tree.
- Expensive paint features need explicit layer/cache boundaries, not accidental `Scene.group` reuse alone.

### Jetpack Compose: Phases, Stability, Skipping, and Deferred State Reads

Compose splits a frame into composition, layout, and drawing. Its performance docs center on stability and skippability: when inputs are stable and unchanged, Compose skips recomposition. Strong skipping makes more restartable composables skippable, and Compose compares parameters differently depending on stability. Compose also recommends `remember`, stable keys for lazy layouts, `derivedStateOf` for rapidly changing state, and deferring state reads to later phases when possible.

The strong lesson is phase-local dependency tracking. If state is only needed for drawing, do not read it during composition. If pointer position only changes hover paint, do not rerun the application view and layout.

Implication for FS.Skia.UI:

- Introduce explicit frame causes and dependency classes: model, size, theme, runtime visual state, animation clock, pointer position, text draft.
- Teach metrics to say whether composition/view, layout, paint, or hit-test was invalidated.
- Treat stable F# immutable records as a strength, but expose unstable `UntypedValue`, closures, and rebuilt lists as potential skip breakers.

### SwiftUI: Identity, Lifetime, Dependencies, and Instruments

Apple's SwiftUI talks frame performance through identity, lifetime, and dependencies. SwiftUI performance tooling highlights update groups, long view body updates, and causes/effects of updates. The practical recommendation is to split views so dependencies are local and avoid long body work on every update.

Implication for FS.Skia.UI:

- The framework needs first-class instrumentation that answers "why did this frame update?" and "which dependency caused which phase?"
- `host.View` duration and allocation count should be visible, even if not part of deterministic goldens.
- Focus, hover, pressed, and animation state should be host-retained dependencies, not reasons to rebuild arbitrary consumer view bodies.

### Avalonia and WPF: Retained Trees, Invalidation, Virtualization, and Visual Tree Cost

Avalonia documents a retained-mode pipeline: input, property changes that invalidate layout or render, measure/arrange for controls that need it, dirty visual render, scene graph diff, and GPU composition. It also explicitly coalesces layout invalidations so only one layout pass runs per frame. Avalonia performance guidance emphasizes UI virtualization, visual tree depth, simpler templates, and minimizing layout invalidations.

WPF's older docs make the cost model blunt: layout is mathematically intensive, tree construction order matters, and lighter drawing primitives perform better than full framework elements when interactivity/layout are not needed.

Implication for FS.Skia.UI:

- Do not make every visual primitive a full interactive control if it only needs drawing.
- Provide simplified/lightweight templates for high-density surfaces.
- Add virtualization and recycling before attempting global renderer rewrites.
- Coalesce invalidations per frame, and report which invalidation categories were coalesced.

### Qt Quick: Scene Graph, Render Thread, Batching, and Avoiding Extra Passes

Qt Quick's scene graph retains renderable primitives independently from QML items. It can batch similar primitives into fewer draw calls and often renders on a dedicated thread while the GUI thread prepares the next frame. Qt also warns that QQuickWidget pays for flexibility with an extra offscreen render pass and loss of the threaded render loop.

Implication for FS.Skia.UI:

- CPU-side retained `Scene list` reuse is useful but not the full story; batching, layers, and backend-aware composition matter once scenes get large.
- Avoid unnecessary offscreen layers/readbacks in evidence and live paths.
- Be explicit when a convenience hosting mode sacrifices render-thread or compositor advantages.

### Browser Rendering and Pointer Events: Frame Budget and Coalesced Input

Browser guidance centers on the 16 ms frame budget at 60 Hz, moving heavy work out of input handlers, using `requestAnimationFrame` as the frame boundary, and chunking work. Pointer Events explicitly support coalescing multiple raw pointer moves into one dispatched event while still allowing apps to inspect the raw path via `getCoalescedEvents`.

Implication for FS.Skia.UI:

- Feature 108 pointer coalescing follows the right precedent: continuous moves are frame-rate work, not sample-rate work.
- The plan must preserve path fidelity for drawing/dragging while avoiding per-sample hit-test/render work.
- Input handlers should enqueue and classify, not rebuild view, relayout, paint, and dispatch synchronously for each native sample.

### Skia and React Native Skia: Pictures and Display Lists

Skia itself is a drawing engine, not a UI framework. React Native Skia's Picture docs show the useful distinction: retained mode is good for animating property values cheaply, while immutable pictures are useful when replaying a recorded list of drawing commands. This is directly relevant because FS.Skia.UI already lowers controls to a `Scene` display description.

Implication for FS.Skia.UI:

- Add picture/display-list cache boundaries after the framework can prove paint is the bottleneck.
- Cache immutable drawing operations for stable subtrees, but keep interactive hit-test/accessibility state separate from cached pixels.
- Use explicit invalidation keys for pictures; do not hide stale-paint risk behind opaque cache reuse.

## Cross-Framework Principles

The recurring performance principles are:

1. Stable identity is the base requirement. Fast diffing, focus retention, animation continuity, virtualization, and state reuse all depend on it.
2. Work must be phase-separated. View construction, diff, layout, paint, composition, hit-test, and backend presentation are different costs and need different invalidation rules.
3. Invalidations should be narrower than updates. A hover color change should not imply a model update, full view rebuild, full layout, full paint, and full hit-test.
4. High-rate input is frame-rate work. Pointer move bursts should be coalesced at the frame boundary, with raw paths preserved only for consumers that need them.
5. Large repeated UI must be virtualized. Optimizing a 10000-row fully materialized tree is the wrong first fight.
6. Rendering caches need explicit correctness keys. Theme, size, layout box, visual state, text/font inputs, clip, opacity, and transform all affect reuse.
7. Tooling is not optional. Mature frameworks expose profilers that show why work happened.
8. Avoid expensive graphics effects by default. Opacity groups, clipping, offscreen layers, readbacks, and intrinsic layout have real costs.

## FS.Skia.UI Current State

### Strengths Already Present

The existing retained path has meaningful foundations:

- `RetainedRender` holds stable `RetainedId`, a retained node tree, `StateByIdentity`, prior `Layout`, and cached render fragments.
- `Reconcile.diff` uses keyed/positional matching and compares `VisualState` structurally, which allows held visual state to avoid repeated changes.
- `RetainedRender.layoutDirtySet` computes a dirty set from the reconcile patch and feeds incremental layout.
- `WorkReductionRecord` already reports baseline, recomputed, changed-bound, shifted, and remeasured work.
- Animation clocks are per identity, injected-delta driven, and sampled during paint.
- Feature 108 in-progress work adds `FrameMetrics`, `Perf.runScript`, and pointer-move coalescing.

These are not throwaway mechanisms. They are the right direction.

### Gaps and Likely Hot Spots

The hot-path gaps are also clear:

- `routeInteractivePointer` renders the current control tree with `Control.renderTree host.Theme size (host.View size model)` to route a pointer sample. This bypasses the retained frame for pointer dispatch and can turn input sampling into full view/render work.
- `renderRetained` still calls `host.View size model` to produce the next tree for diffing. That is unavoidable for model changes, but it is too broad for host-owned hover/focus/animation changes if the consumer model did not change.
- `FrameMetrics.ViewRebuilt` is currently a semantic approximation in parts of the code path. It can mean "product messages were produced" rather than "host.View actually ran." Those must be separated.
- Fragment reuse stores `OwnScene`, `SubtreeScene`, and `Box`, but there is no backend layer, picture, damage-rect, or draw-call batching layer. Reusing F# scene lists avoids some CPU paint construction; it does not guarantee cheap backend presentation.
- Theme changes invalidate all cached paint. That is correct, but it suggests token/theme churn needs to be rare or scoped by future render-role caches.
- `SubtreeScene` assembly can still rebuild ancestor lists when child paint changes. This is likely acceptable for small trees but becomes a scaling issue for large repeated controls.
- Hit-testing is retained-aware for focus via `retainedHitTest`, but event binding dispatch still has paths that recover from full rendered output.
- There is no framework-level virtualization contract for DataGrid/list surfaces.
- There is no phase-complete profiler record or allocation accounting, so performance work can be misprioritized.

## Performance Model FS.Skia.UI Should Expose

Feature 108 should be treated as the first metrics slice, not the final metrics model. A durable frame record should split deterministic counts from non-deterministic timing:

Deterministic/golden-friendly fields:

- `FrameCause`: idle, model message, pointer move, pointer discrete, key, tick, resize, theme.
- `ViewCalled`: whether `host.View` actually ran.
- `DiffNodeCount`, `PatchedNodeCount`, `ReusedNodeCount`, `RepaintedNodeCount`.
- `RemeasuredNodeCount`, `LayoutInvalidatedNodeCount`.
- `HitTestCount`.
- `PointerSamplesReceived`, `PointerMovesProcessed`, `PointerMovesCoalesced`.
- `SceneNodeCountBefore`, `SceneNodeCountAfter`.
- `VirtualItemsMaterialized`, `VirtualItemsTotal`.
- `DirtyRectCount` and integer-rounded dirty area if damage tracking is added.

Timing/diagnostic fields, excluded from goldens:

- `ViewDuration`, `DiffDuration`, `LayoutDuration`, `PaintDuration`, `ComposeDuration`, `DispatchDuration`, `FrameDuration`.
- Allocated bytes per phase where .NET runtime APIs make this practical.
- Backend present/readback timing when running live.

The rule should be: deterministic evidence asserts counts and booleans; human perf reports inspect timing and allocations.

## Implementation Plan

This plan is intentionally staged. It preserves current feature 108 work and avoids a renderer rewrite until metrics prove the bottleneck.

### Phase 0: Baseline and Guardrails

Goal: Establish honest baseline evidence before changing performance behavior.

Tasks:

1. Create a controls performance scenario corpus:
   - Hover across 100, 1000, and 5000 simple controls.
   - DataGrid with 100, 1000, and 10000 rows.
   - Deep nested layout with repeated labels and buttons.
   - Text entry in a focused field while unrelated controls animate.
   - Theme switch across a moderate dashboard.
   - Continuous drag/freehand path with hundreds of raw samples.
2. Extend `Perf.runScript` evidence so each scenario has deterministic metrics goldens.
3. Add a non-golden local benchmark command or report generator that records timing and allocation fields.
4. Capture current before numbers and keep them in `docs/reports/_baselines` or the feature readiness area.
5. Define regression thresholds in counts first, timing second.

Acceptance criteria:

- The report can answer how many times `host.View`, full render, layout, paint, and hit-test happened per scripted interaction.
- A hover burst has a recorded baseline before and after feature 108 coalescing.
- No optimization is accepted solely on anecdotal smoothness.

### Phase 1: Finish and Correct Feature 108 Metrics and Coalescing

Goal: Make existing in-progress work truthful and load-bearing.

Tasks:

1. Verify `PointerSamplesReceived` counts raw native samples, including deferred moves.
2. Verify `PointerMovesProcessed <= 1` for a burst frame, while drag path fidelity is retained.
3. Split `ViewRebuilt` into at least two concepts internally:
   - `ProductModelChanged`: a product message changed model.
   - `ViewCalled`: `host.View size model` actually ran.
4. Keep public `ViewRebuilt` only if its meaning is precise. If not, rename before the contract hardens.
5. Make `FrameDuration` real timing for live diagnostics while keeping it out of goldens.
6. Ensure `OnFrameMetrics` fires once per produced frame, not once per incidental flush boundary with ambiguous counts.

Acceptance criteria:

- Metrics match code-path facts under tests that deliberately produce no product messages, product messages with no visual change, and visual host-state changes with no product message.
- Coalesced input never drops press/release/click/scroll.
- Idle frames report zero work unless an active animation or explicit tick requires work.

### Phase 2: Remove Full-Render Pointer Routing from the Hot Path

Goal: Pointer input should use the retained frame for hit-test and binding dispatch.

Tasks:

1. Store enough event binding and bound-id data on retained nodes or retained frame output to route pointer events without calling `Control.renderTree`.
2. Route hit-testing through `RetainedRender.retainedHitTest` and cached boxes.
3. Add a retained-id to authored-control-id lookup so composite controls still dispatch authored bindings correctly.
4. Replace `routeInteractivePointer` hot-path full render with retained-frame dispatch.
5. Keep the existing full-render path only as a test oracle or fallback diagnostic, not the normal live path.
6. Add a metric `FullRenderFallbackCount`; require zero for normal scripted pointer scenarios.

Acceptance criteria:

- A pointer move/click after initial render performs no `Control.renderTree` full rebuild for routing.
- Binding dispatch results match the previous full-render path in parity tests.
- Unkeyed same-kind siblings remain distinguishable through retained identity.

### Phase 3: Add a Frame Scheduler and Phase Invalidation Model

Goal: Batch work at the frame boundary and only run phases required by the cause.

Tasks:

1. Introduce an internal `FrameCause` and `FrameInvalidation` model.
2. Queue native input samples and flush on the viewer frame/tick boundary.
3. Coalesce move samples inside the queue, preserving raw drag path.
4. Mark invalidations by phase:
   - `InputOnly`
   - `RuntimeVisualState`
   - `ProductModel`
   - `Layout`
   - `Paint`
   - `AnimationPaint`
   - `Theme`
   - `Resize`
5. Make animation clocks request paint-only frames while active.
6. Make hover/focus visual state request diff/paint for the affected retained identities, not whole-app view work when possible.

Acceptance criteria:

- Input handlers enqueue and return quickly.
- Continuous pointer movement produces frame-rate work, not sample-rate work.
- Metrics identify skipped phases explicitly.

### Phase 4: Narrow Runtime Visual-State Updates

Goal: Hover/focus/press changes should not require stamping the entire tree.

Tasks:

1. Replace full-tree `ControlRuntime.applyRuntimeVisualState` stamping in the hot path with retained-id targeted visual-state changes where possible.
2. For the first implementation, target only previous-hover/current-hover, previous-focus/current-focus, and pressed identities.
3. Preserve the full-tree pure stamp as a parity oracle and fallback.
4. Add tests proving targeted stamping produces the same final rendered scene as full stamping.
5. Track `RuntimeStateTouchedNodeCount`.

Acceptance criteria:

- Moving hover between two controls touches the old and new hover identities plus necessary ancestors, not every node.
- At-rest output stays byte-identical.
- Disabled/consumer-set visual state precedence remains unchanged.

### Phase 5: View Memoization and Stable Dependency Contracts

Goal: Avoid recomputing expensive pure control subtrees when their declared dependencies have not changed.

Tasks:

1. Add diagnostics that identify always-new attributes/events that break equality, especially `UntypedValue`, event closures, rebuilt row lists, and unstable keys.
2. Document a stable-props guidance page for control authors.
3. Consider an explicit `Control.memo` or `Widget.memo` primitive:
   - Keyed by `ControlId`.
   - Uses a deterministic dependency value supplied by the caller.
   - Reuses the prior lowered subtree when dependencies compare equal.
   - Never hides semantic changes.
4. Prefer high-level control-internal memoization first, especially DataGrid column/row transforms and style resolution.
5. Add `MemoHitCount` and `MemoMissCount`.

Acceptance criteria:

- Memoization is opt-in or control-owned, testable, and deterministic.
- A bad dependency key fails visibly through diagnostics or tests.
- No correctness depends on memoization; it is only a performance optimization.

### Phase 6: Viewport Virtualization for Repeated Controls

Goal: Large repeated controls should materialize visible items plus overscan, not every logical item.

Tasks:

1. Define an internal virtualization model for DataGrid/List-like controls:
   - Total item count.
   - Viewport size and scroll offset.
   - Estimated or measured row height.
   - Overscan count.
   - Stable item keys.
2. Add row/column measurement caches keyed by item key, column key, theme density, and width constraints.
3. Ensure keyboard focus and selection can target offscreen logical items without requiring their visual controls to exist.
4. Render placeholders/spacers for total scroll extent.
5. Add evidence scenarios for 10000-row DataGrid.

Acceptance criteria:

- `VirtualItemsMaterialized <= visible + overscan`.
- Scrolling reuses row containers where possible.
- Keyboard navigation across visible/offscreen boundaries remains correct.
- Accessibility metadata can still describe total counts and current position.

### Phase 7: Paint Cache, Damage Rects, and Optional Picture Boundaries

Goal: Repaint and backend presentation should scale with changed visual area when possible.

Tasks:

1. Add retained damage rectangles from changed/repainted retained nodes.
2. Track dirty area and dirty node counts in metrics.
3. Add explicit cache boundaries for expensive stable subtrees.
4. Explore Skia picture/display-list recording for stable subtrees after paint metrics prove benefit.
5. Keep theme, box, clip, opacity, transform, font/text, and visual-state values in cache keys.
6. Avoid hidden offscreen layer costs. Add diagnostics when a control uses an expensive effect that requires offscreen composition.

Acceptance criteria:

- A hover state change reports a small dirty region.
- Cached picture reuse is invalidated on all relevant inputs.
- Memory growth from caches is bounded and observable.

### Phase 8: Layout Hot-Path Improvements

Goal: Keep incremental layout correct while reducing unnecessary measure work.

Tasks:

1. Keep the existing layout dirty-set drift guard; extend it when new layout attributes are added.
2. Add text measurement caches keyed by text, font family, size, weight, and constraints.
3. Add fixed-size or layout-boundary hints for controls whose descendants cannot affect ancestor size.
4. Flatten structural wrappers in high-density controls where it does not alter semantics.
5. Add metrics for intrinsic or multi-pass layout if such paths are introduced.

Acceptance criteria:

- Style-only and visual-state-only updates remeasure zero layout nodes unless geometry truly changed.
- Text-heavy scenarios show cache hits.
- Deep layout scenarios have clear dirty propagation counts.

### Phase 9: Backend and Host Mode Review

Goal: Ensure the Skia/viewer backend does not defeat retained controls performance.

Tasks:

1. Audit `SkiaViewer` frame scheduling, present path, and any readback/evidence code used in live mode.
2. Separate live rendering from evidence readback paths so proof tooling does not impose live costs.
3. Identify whether scene submission can skip unchanged layers.
4. Document hosting modes and their performance tradeoffs.
5. Consider a future render-thread/compositor split only after CPU metrics show the main thread is the limiting factor.

Acceptance criteria:

- Live mode has no accidental readback in ordinary frames.
- Evidence mode remains deterministic but is explicitly not used as live performance proof.
- Backend limitations are documented in the report/evidence artifacts.

## Priority Recommendations

Do first:

1. Verify feature 108 metrics semantics.
2. Remove full-render pointer routing from the live hot path.
3. Add phase-complete metrics and before/after baselines.
4. Add DataGrid/list virtualization.

Do next:

1. Target runtime visual-state stamping to changed identities.
2. Add stable-dependency diagnostics and control-owned memoization.
3. Add text measurement caches and layout boundary hints.

Do later:

1. Damage rectangles and picture/layer caches.
2. Backend composition/layer submission changes.
3. Render-thread or compositor architecture work.

Do not do yet:

- A wholesale rewrite to Avalonia/WPF architecture.
- Blind memoization around every control.
- GPU/layer caching before paint/composition metrics show it is the bottleneck.
- Timing-only pass/fail gates that vary by machine.

## Risks

- Cache invalidation correctness: A stale visual cache is worse than a slow frame. Cache keys must include every render-affecting input.
- Public API churn: Metrics names harden quickly. The semantics must be precise before they become contract surface.
- Memory pressure: Retained fragments, pictures, row caches, and text metrics can improve CPU while increasing memory. Track memory from the start.
- Determinism: Timing is useful for humans but unsuitable for goldens. Keep deterministic counts separate.
- Input fidelity: Coalescing must never lose discrete interactions and must preserve drag/freehand paths.
- Accessibility: Virtualization must preserve logical item counts, focus traversal, and keyboard navigation even when visuals are not materialized.

## Success Criteria

The performance work should be considered successful only when these are true:

- Idle frames do no view, diff, layout, paint, or hit-test work.
- A burst of pointer moves processes at most one move per frame and performs zero full renders for routing.
- A hover/focus state change touches only affected identities and necessary ancestors.
- A 10000-row DataGrid materializes only the visible rows plus overscan.
- Metrics explain every non-idle frame by cause and phase.
- Deterministic perf scripts fail when a regression reintroduces full-tree work.
- Live diagnostic reports show timing and allocation improvements on the same scenarios that deterministic counts improved.

## Final Recommendation

The project should continue the retained-MVU architecture. The current direction matches the successful parts of React, Flutter, Compose, SwiftUI, Avalonia, and Qt Quick. The next step is not a redesign; it is to remove the remaining immediate-mode hot-path bypasses and make frame work observable by phase.

The most important concrete follow-up after feature 108 is retained pointer routing. As long as pointer input can call `host.View` plus `Control.renderTree` for routing, controls will still feel slow under movement regardless of how good `RetainedRender.step` becomes.
