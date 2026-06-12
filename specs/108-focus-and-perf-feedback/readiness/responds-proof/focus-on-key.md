# Interactive responds-proof — focus + pointer (T011, FR-006/007)

evidence-kind=responds-proof
path=ControlsElmish.captureRespondsProof / routeInteractivePointer
live-window=not-required

The responds-proof captures a BEFORE frame, applies a real dispatched interaction through the exact
adapter path `runInteractiveApp` wires (`routeInteractivePointer` → fold through `host.Update` →
re-render), and captures an AFTER frame, returning a `Responsive`/`Inert` verdict. An app that renders
but does not respond yields identical frames and an `Inert` verdict — "renders" cannot be passed off as
"responds". This reuses the production render path; no live Vulkan window is required (the documented
evidence path, [runtime-limitations.md](../runtime-limitations.md)).

For feature 108 the focus-on-key behaviour is exercised through the SAME `RetainedRender` + focus seam
the live host wires: a press sets focus to the focusable control under it (`resolveFocus` →
`focused`), and a subsequent key routes focus-first (`routeFocusedKey`) or, when unconsumed, through
the new `MapKeyChord`/`MapKey` `chordFallthrough`. The per-frame `FrameMetrics`/coalescing behaviour is
proven deterministically by `ControlsElmish.Perf.runScript`
([../perf-metrics/coalescing.md](../perf-metrics/coalescing.md)). The adapter-path responds behaviour
is regression-covered by `tests/Elmish.Tests/Feature090DispatchTests.fs` /
`Feature098DispatchTests.fs` (the live route), which remain green under the additive host fields.
