# Runtime limitations

## Feature scope: pure additive adapter API; no runtime/render surface

This feature adds a **pure** Widget-returning view path (`ControlsElmish.widgetView` /
`programOfWidget`, = `view >> Widget.toControl`) and a **pure, total** bridge between the
adapter's `AdapterCommand<'msg>` effect list and Elmish `Cmd<'msg>` (the `AdapterCmd`
module). It introduces no new layout, rendering, screenshot, Vulkan, or Skia behavior, and
no new effect-interpreter semantics — `interpretKeyboardEffect`, `interpretControlEffect`,
and `subscriptions` are byte-unchanged (FR-009). The `Cmd` bridge performs **no I/O**: it
constructs an Elmish command that dispatches through the standard Elmish dispatcher at
runtime, exactly as any `Cmd` does. The honest audience for this feature is the in-package
Expecto/FsCheck tests (lowering parity + command round-trip) and the
`controls-elmish-prelude.fsx` FSI transcript, not a screenshot or host smoke run.

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux and renders
through **Vulkan** on a **SkiaSharp preview** native build.
Platforms remain **unsupported macOS/mobile/browser**, and there is **no software-renderer fallback**.
This feature changes none of that — it adds additive, pure adapter API to
`FS.Skia.UI.Controls.Elmish`, not a change to how the runtime executes or which platforms
it supports. The base `FS.Skia.UI.Controls` package gains no dependency and no observable
behavior change.

## No visual/host evidence is applicable

Because the deliverable is pure function composition (`Widget` lowering) and a pure,
deterministic effect-list↔`Cmd<'msg>` mapping with no render-path wiring, there is no
interactive-window, first-frame, or environment-session diagnostic to capture for this
feature. The authoritative evidence is the lowering-parity test (US1, the Widget path
renders structurally equal to `view >> Widget.toControl`), the command round-trip FsCheck
property (≥1000 cases), the totality/order property (≥1000 cases over every `AdapterEffect`
case), the US3 legacy-unchanged + dependency-guard tests, and the escalated Route-printed
gate set.
