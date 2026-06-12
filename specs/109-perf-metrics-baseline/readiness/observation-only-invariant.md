# Observation-only invariant (feature 109, T027, FR-020 / SC-008)

Feature 109 changes the **observability surface only**. The `FrameMetrics` field set changes (remove
`ViewRebuilt`; add `ProductModelChanged`, `ViewCalled`, `FullRenderCount`) and the live loop now reports
those facts + a real `FrameDuration`, but **no rendered pixel, layout box, or dispatch outcome changes**.

## What was preserved byte-identically

- **Render gating unchanged.** In `Perf.runScript` the `renderStep` call is still gated EXACTLY as
  before (`if hasMsgs then renderStep ()` on the message arms; `if rebuilt || hadAnimation` on the tick
  arm). The metric now REPORTS that pre-existing fact instead of re-deriving a single conflated boolean;
  it does not change WHEN any render runs. So the retained-state evolution, the Scene, and the layout
  boxes are identical to the pre-feature path.
- **No model/scene side effects.** `ProductModelChanged` is computed by reference-comparing the model
  the pure fold already produced; the metric reads state, it never mutates it. The `OnFrameMetrics`
  default stays `ignore` (inert), so a host that does not observe metrics is byte-identical to its
  pre-109 behaviour.
- **Default host path untouched.** No render / layout / hit-test / dispatch code path is edited. The
  live `emitFrameMetrics` is the additive best-effort sink; the default `OnFrameMetrics = ignore` makes
  it a no-op.

## Assertion

`tests/Elmish.Tests/Feature109MetricsHonestyTests.fs` →
"observation-only: the production render path is byte-identical regardless of metric observation":

- The production `Control.renderTree` Scene is byte-identical whether the host's `OnFrameMetrics` is the
  inert default or a recording sink (the sink does not touch the render path).
- The at-rest production render is byte-stable across repeated renders.
- The deterministic count/boolean surface and the model fold are identical for an observing host vs the
  default host (observation perturbs neither).

The DataGrid 10000-row scenario runs on the NON-virtualized path ON PURPOSE — it is the
pre-virtualization baseline, not a behaviour to "fix" here.
