---
title: Input
category: Architecture
categoryindex: 4
index: 4
description: The FS.Skia.UI input subsystem — the keyboard-binding/command/mode runtime (FS.Skia.UI.Input), the lightweight keyboard package (FS.Skia.UI.KeyboardInput), and how host pointer events reach controls.
---

# Input

FS.Skia.UI's input story is built from **pure, host-independent reducers**: the
raw event stream from the [SkiaViewer host](./host-skiaviewer.html) is translated
into typed messages, fed to an Elmish-style `update`, and turned into a list of
*effects* that the application interprets. Nothing in the input packages opens a
window, polls a device, or draws to the screen on its own. This page covers the
two keyboard packages that share the `input` slug — the richer
`FS.Skia.UI.Input` runtime and the lightweight `FS.Skia.UI.KeyboardInput`
package — and explains where pointer/mouse input actually lives. See the
[API reference](../reference/index.html) for the full surface.

> **Naming caveat (read this first).** Despite the landing-page table describing
> `FS.Skia.UI.Input` as "Pointer/mouse input events", the package's public
> surface today (namespace `FS.Skia.UI.Input`) is a **keyboard** binding,
> command, mode, sequence, and bigram-analysis runtime — there are no pointer or
> mouse types in it. The actual pointer/mouse contract is the host's
> `ViewerEvent` cases plus a pointer front door that lives in
> [`FS.Skia.UI.Controls`](./controls.html), described at the end of this page.
> This documentation reflects the code, not the label.

## Two keyboard packages, two scopes

| Package | Namespace | Scope |
|---|---|---|
| [`FS.Skia.UI.KeyboardInput`](../reference/fs-skia-ui-keyboardinput.html) | `FS.Skia.UI.KeyboardInput` | A small key→command binding reducer with mode-stack/layout state and a state-display snapshot. Depends only on [Scene](./scene.html). |
| [`FS.Skia.UI.Input`](../reference/fs-skia-ui-input.html) | `FS.Skia.UI.Input` | A full keyboard *configuration* runtime: YAML-driven bindings, command registry, modes/states, multi-chord sequences, command intents, diagnostics, bigram ergonomics analysis, and on-host state-display rendering. Depends on [Scene](./scene.html) and [SkiaViewer](./host-skiaviewer.html). |

The two are independent — `Input` is not built on top of `KeyboardInput`; they
are separate packages with overlapping concepts (both have `KeyDown`/`KeyUp`
messages, a pressed-key set, a mode stack, and a state-display projection).

## FS.Skia.UI.KeyboardInput — the lightweight reducer

This package is deliberately minimal. You give
[`Keyboard.init`](../reference/fs-skia-ui-keyboardinput-keyboard.html) a list of
[`KeyboardBinding`](../reference/fs-skia-ui-keyboardinput-keyboardbinding.html)
records (`{ Key; Command }`) and it returns a
[`KeyboardModel`](../reference/fs-skia-ui-keyboardinput-keyboardmodel.html) plus
startup effects. [`Keyboard.update`](../reference/fs-skia-ui-keyboardinput-keyboard.html)
is the reducer over
[`KeyboardMsg`](../reference/fs-skia-ui-keyboardinput-keyboardmsg.html)
(`KeyDown`, `KeyUp`, `FocusLost`, `Reset`, `SetActiveLayout`,
`PushTemporaryMode`/`PopTemporaryMode`, `SetPersistentMode`,
`ResolvePendingSequence`). When a `KeyDown` matches a binding it emits a
`CommandResolved` [`KeyboardEffect`](../reference/fs-skia-ui-keyboardinput-keyboardeffect.html);
other effects report key-state, layout, mode, pending-sequence, and diagnostic
changes.

A second module,
[`ViewerKeyboard`](../reference/fs-skia-ui-keyboardinput-viewerkeyboard.html),
normalizes raw host key strings into a typed
[`ViewerKey`](../reference/fs-skia-ui-keyboardinput-viewerkey.html)
(`ArrowLeft`, `Enter`, `Letter of char`, `Digit of int`, `Function of int`,
`Unknown of raw`, …) and converts it to the `KeyId` the bindings use. That is the
adapter between the host's stringly-typed key names and the package's vocabulary.

[`Keyboard.stateDisplay`](../reference/fs-skia-ui-keyboardinput-keyboard.html)
projects the model into a
[`KeyboardStateDisplay`](../reference/fs-skia-ui-keyboardinput-keyboardstatedisplay.html)
snapshot (pressed keys, active layout, mode stack, pending sequence, last
command) for HUD/state-overlay rendering.

## FS.Skia.UI.Input — the configured keyboard runtime

The `Input` package is a much larger system aimed at apps that want a *data-driven
key map* with modes and chords (think of a modal editor's keybinding file). The
pipeline has four stages, each returning a `Result<_, InputDiagnostic list>` so
configuration errors are explicit values, not exceptions:

```fsharp
result {
    let! registry      = KeyboardInput.commandRegistry commands       // CommandRegistry
    let! configuration = KeyboardInput.parseYaml yaml                  // InputConfiguration (YamlDotNet)
    let! model         = KeyboardInput.validate registry configuration // CanonicalInputModel
    let! runtime, _fx  = KeyboardInput.init configuration.DefaultLayout model
    return runtime
}
```

1. **Registry.**
   [`KeyboardInput.commandRegistry`](../reference/fs-skia-ui-input-keyboardinput.html)
   takes the
   [`CommandDefinition`](../reference/fs-skia-ui-input-commanddefinition.html) list
   the bindings may emit and rejects duplicate command ids.
2. **Parse.** `parseYaml` reads a YAML document (via `YamlDotNet`) into an
   [`InputConfiguration`](../reference/fs-skia-ui-input-inputconfiguration.html) —
   layouts (`LayoutProfile` with `KeyPosition`s carrying `Hand`/`Finger`/row/
   column), modes (`ModeDefinition` of kind `StandardMode`/`StatefulMode`/
   `PopupMode`/`TemporaryHeldMode`), bindings, disambiguation timeout, an optional
   bigram profile, display options, and command intents.
3. **Validate.** `validate` cross-checks the configuration against the registry —
   unknown modes/commands/layouts, duplicate bindings, invalid mode states — and
   produces a
   [`CanonicalInputModel`](../reference/fs-skia-ui-input-canonicalinputmodel.html).
4. **Init.** `init` builds the
   [`InputRuntime`](../reference/fs-skia-ui-input-inputruntime.html): the
   canonical model plus a mode stack, a pressed-key set, an optional pending
   sequence, the active layout, an event log, and accumulated diagnostics.

### The runtime reducer

[`KeyboardInput.update`](../reference/fs-skia-ui-input-keyboardinput.html) folds an
[`InputMsg`](../reference/fs-skia-ui-input-inputmsg.html)
(`KeyDown`, `KeyUp`, `FocusLost`, `Timeout`, `SetLayout`, `Cancel`) into a new
`InputRuntime` and a list of
[`InputEffect`](../reference/fs-skia-ui-input-inputeffect.html)
(`CommandResolved`, `LayoutStateChanged`, `InputDiagnosticEmitted`,
`InputEventRecorded`). The control flow is honest about ambiguity and error
recovery:

- **`KeyDown`** filters bindings that match the pressed key in the current mode/
  state. *Zero* matches emits an informational `StaleInputEvent` diagnostic;
  *more than one* match emits an `AmbiguousSequence` error; exactly one resolves
  its [`BindingOutcome`](../reference/fs-skia-ui-input-bindingoutcome.html) —
  `EmitCommand` (popping a popup mode if one is on top), `SetState`,
  `SetLayoutOutcome`, `PushPopup`/`PushTemporary`, `CancelTopMode`, or `NoInputOp`.
- **`KeyUp`** removes the key and pops the held mode it entered; if no held mode
  matches it records a `LostKeyReleaseRecovered` warning rather than failing.
- **`FocusLost`** clears pressed keys, drops `TemporaryHeldMode` frames, clears
  any pending sequence, and (if keys were down) emits a recovery diagnostic — the
  defence against the classic "stuck modifier after alt-tab" bug.
- **`Timeout`** abandons a pending multi-chord sequence (popping a popup mode if
  present) and reports it.
- **`SetLayout`/`Cancel`** switch the active layout (rejecting unknown ids) or pop
  the top mode and clear the pending sequence.

[`KeyboardInput.viewerInputMsg`](../reference/fs-skia-ui-input-keyboardinput.html)
maps a host `ViewerEvent` (`KeyDown`/`KeyUp`/`CloseRequested`) to an `InputMsg`,
and `updateFromViewerEvent` steps the runtime straight from a host event.
[`KeyboardInput.replay`](../reference/fs-skia-ui-input-keyboardinput.html) folds a
whole `InputMsg list`, which makes recorded-input replay deterministic.

### Projection, rendering, and analysis

[`KeyboardInput.layoutState`](../reference/fs-skia-ui-input-keyboardinput.html)
projects the runtime into a
[`LayoutStateView`](../reference/fs-skia-ui-input-layoutstateview.html), and a
family of `keyboardStateDisplay` / `renderKeyboardStateDisplay(At)` /
`renderLayoutState(At)` functions build a
[`KeyboardStateDisplayModel`](../reference/fs-skia-ui-input-keyboardstatedisplaymodel.html)
and render it to a [`Scene`](./scene.html) for an on-host overlay, with default,
compact, and expanded option presets.
[`KeyboardInput.analyzeBigrams`](../reference/fs-skia-ui-input-keyboardinput.html)
produces a
[`BigramReport`](../reference/fs-skia-ui-input-bigramreport.html) — top command
pairs, ergonomic `BigramRisk`s (same-finger, long-travel, awkward-hold,
same-hand-repeat), and reassignment `BigramSuggestion`s — using the layout's
hand/finger metadata.

## Where pointer / mouse input lives

Pointer interaction is *not* in either input package. It is split across two
other subsystems:

- **The host contract.** The [SkiaViewer host](./host-skiaviewer.html) publishes
  raw pointer events on its `ViewerEvent` type: `PointerMoved (x, y)`,
  `PointerPressed (x, y, button)` and `PointerReleased (x, y, button)` (carrying
  a `ViewerPointerButton`), `PointerScrolled (x, y, deltaX, deltaY)`, and
  `PointerExited` (pointer left the window / focus lost — drives cancel). These
  were extended for feature 075 (Mouse Input & Pointer Events).
- **The coordination front door.** A pure pointer reducer lives in
  [`FS.Skia.UI.Controls`](./controls.html). It speaks a neutral `PointerSample`
  (x/y/button/phase/delta) — deliberately *not* `ViewerEvent`, so `Controls`
  stays host-independent — and turns a pointer sample into ordered interactions
  (hover enter/leave, press/release/click, drag begin/move/end, secondary click,
  scroll) by hit-testing through
  [`Layout.hitTestComputed`](./layout.html). The `ViewerEvent.Pointer* →
  PointerSample` translation is done by the consumer (the sample/host glue),
  exactly as keyboard events are translated through `viewerInputMsg`.

So the data flow for a click is: host raw event → `ViewerEvent.PointerPressed` →
consumer maps to `PointerSample` → the `Controls` pointer reducer hit-tests via
`Layout` and emits a click interaction → the
[`Controls.Elmish`](./controls.html) bridge lowers it to a `Cmd<'msg>` for the
[Elmish runtime](./elmish-mvu.html). The keyboard packages follow the same
shape with `ViewerEvent.KeyDown/KeyUp`.

## How input fits the framework

Both keyboard packages are pure reducers that sit at the same architectural
position: the [host](./host-skiaviewer.html) emits raw events, an adapter
normalizes them into typed messages, `update` produces a new immutable runtime
plus effects, and the [Elmish/MVU runtime](./elmish-mvu.html) interprets those
effects as commands. The packages render *into* [scenes](./scene.html) (for state
overlays) but never own the frame loop, and they reach controls only through the
neutral hit-test geometry that [Layout](./layout.html) computes.

## Analysis

### Implementation strengths

- **Error recovery is explicit and tested-by-design.** The `Input` runtime names
  every failure mode as a diagnostic code — `StaleInputEvent`,
  `AmbiguousSequence`, `LostKeyReleaseRecovered` — and `FocusLost` actively clears
  pressed keys and temporary-held modes, directly defending against stuck-key and
  ambiguous-binding bugs instead of ignoring them.
- **Pure reducers with deterministic replay.** Both `Keyboard.update` and
  `KeyboardInput.update` are pure folds over an immutable model, and the `Input`
  package's `replay` re-runs an `InputMsg list` to identical results, which makes
  recorded-input tests trivial and the behaviour reproducible.
- **Configuration errors are values, not crashes.** The `Input` pipeline
  (`commandRegistry`/`parseYaml`/`validate`/`init`) returns
  `Result<_, InputDiagnostic list>` at every stage, so a malformed key map yields
  a structured diagnostic list rather than an exception at runtime.

### Implementation weaknesses

- **The package label contradicts the code.** `FS.Skia.UI.Input` is advertised
  (in `index.md`) as pointer/mouse input but contains only keyboard
  configuration/runtime types; the actual pointer surface lives in `Controls`.
  This mislabelling is itself a defect that will mislead newcomers.
- **Two overlapping keyboard runtimes.** `KeyboardInput` (lightweight) and
  `Input` (configured) duplicate concepts — pressed-key sets, mode stacks,
  `KeyDown`/`KeyUp`, state-display projection — without a shared core, so the same
  ideas are implemented twice and a consumer must choose between them with little
  guidance.
- **Stringly-typed keys and ids.** Keys are `string`s (`KeyId`,
  `KeyPositionId`) normalized by ad-hoc `match` tables (e.g.
  `normalizeViewerKey` enumerates a handful of keys and falls back to a `Key{X}`
  heuristic), which is brittle and incomplete relative to a typed key enumeration.

### Design pros

- **Host-independent by construction.** Input reducers depend only on Scene (and,
  for `Input`, the host event *type*), never on the windowing/Vulkan layer, so
  they are portable, unit-testable without a GPU, and reusable across hosts.
- **Data-driven keybindings.** The `Input` package's YAML configuration with
  modes, multi-chord sequences, command intents, and bigram ergonomics analysis
  is a genuinely powerful model for editor-style apps — keymaps become data a user
  can edit, not code that must be recompiled.
- **One consistent event-to-effect shape across input kinds.** Keyboard and
  pointer both follow the same `host event → typed sample/msg → pure update →
  effects → Elmish Cmd` pipeline, which keeps the whole input story coherent and
  predictable for consumers.

### Design cons

- **Pointer input is spread across three packages.** The pointer contract is
  divided between `SkiaViewer` (the `ViewerEvent` cases), `Controls` (the
  `Pointer` reducer), and `Controls.Elmish` (the bridge), with no single "input"
  home — discoverability suffers and the `input` architecture slug does not own
  its own subsystem.
- **The consumer owns the host-to-message glue.** Because `Controls` must stay
  host-independent, the `ViewerEvent → PointerSample` (and `ViewerEvent →
  InputMsg`) translation is the consumer's responsibility rather than a provided
  adapter, which is principled but pushes boilerplate onto every app.
- **Concept overlap with no unifying abstraction.** Having two keyboard packages
  and a separate pointer front door means "input" is a family of similar-but-
  distinct designs rather than one model, raising the conceptual cost of learning
  the framework's input story.
