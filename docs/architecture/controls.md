---
title: Controls Suite
category: Design
categoryindex: 4
index: 6
description: The declarative control suite and its Elmish adapter — how controls compose over scene, layout, and input, render to a scene + diagnostics, and lower control/keyboard/pointer effects into Elmish commands.
---

# Controls Suite

The control suite is two published packages that share one story.
`FS.Skia.UI.Controls` is the declarative, Elmish-shaped widget layer: you build
an immutable tree of `Control<'msg>` values and render it against a `Theme` to get
back a scene, a layout, diagnostics, and event bindings. `FS.Skia.UI.Controls.Elmish`
is the adapter that wires that tree's runtime effects — control interaction,
keyboard commands, and pointer interactions — into a standard Elmish `Cmd`/`Sub`
program. This page explains both for a newcomer: the core control vocabulary, how
controls compose over the [scene](./scene.html), [layout](./layout.html), and
[input](./input.html) layers, how rendering and event dispatch work, and how the
adapter lowers effects into messages. Detail on the *typed* front door (the
`Widget<'msg>` Props/MVU surface) and the Penpot/design-token flow lives in its
own deep dive — see
[Typed control front door & Penpot flow](../controls-design/typed-front-door.html)
— so it is kept light here.

The supported public surface is owned by the `.fsi` files under
[`src/Controls`](https://github.com/FS-Skia-UI/FS-Skia-UI/tree/main/src/Controls)
and the governed catalog
([`catalog.yml`](https://github.com/FS-Skia-UI/FS-Skia-UI/blob/main/src/Controls/catalog.yml)
plus `Catalog.supportedControls`). Background and the boundary history are in the
[Controls report](../reports/controls.html) and the
[Controls boundary refactor process report](../reports/controls-boundary-refactor-process-report.html).

## Where this sits

Controls is the high-level authoring path. It composes over three lower layers:
[Scene](./scene.html) (the immutable drawing vocabulary a render produces),
[Layout](./layout.html) (the geometry a render resolves), and
[Input](./input.html) (pointer and keyboard events that drive interaction).
Persistent application state stays in your own Elmish model and message types;
the controls layer is a *projection* of that state, not a store. For products
that do not adopt Controls, the [Controls report](../reports/controls.html)
documents the lower-level packages (`Scene`, `Layout`, `KeyboardInput`,
`SkiaViewer`, `Elmish`) that remain supported on their own.

## The core control vocabulary

Everything is built from one record. From
[`Types.fsi`](https://github.com/FS-Skia-UI/FS-Skia-UI/blob/main/src/Controls/Types.fsi):

```fsharp
type Control<'msg> =
    { Kind: ControlKind
      Key: ControlId option
      Attributes: Attr<'msg> list
      Children: Control<'msg> list
      Content: string option
      Accessibility: AccessibilityMetadata option }

and Attr<'msg> =
    { Name: string; Category: AttrCategory; Value: AttrValue<'msg> }
```

A `Control<'msg>` is a kind tag, an optional stable `Key`, a list of typed
attributes, child controls, optional text content, and optional accessibility
metadata. The
[`AttrValue<'msg>`](../reference/fs-skia-ui-controls-attrvalue-1.html) union is
where the type parameter earns its keep: alongside data cases (`TextValue`,
`BoolValue`, `FloatValue`, `StringListValue`, `ValidationValue`, `ThemeValue`,
child/children cases) it carries `MessageValue of 'msg` and `EventValue of
(ControlEvent -> 'msg)` — so an attribute can hold a message to dispatch on
interaction. There is also a deliberate escape hatch, `UntypedValue of obj`, for
custom controls.

Authors rarely touch the record directly. Each control has a module with a
`create: Attr<'msg> list -> Control<'msg>` plus attribute builders. From
[`Control.fsi`](https://github.com/FS-Skia-UI/FS-Skia-UI/blob/main/src/Controls/Control.fsi)
the suite spans display (`TextBlock`, `Label`, `Image`, `Icon`, `Badge`,
`Separator`), input (`Button`, `CheckBox`, `Switch`, `Slider`, `NumericInput`,
`TextBox`, `TextArea`, `RadioGroup`), layout containers (`Stack`, `Grid`, `Dock`,
`Wrap`, `Border`, `Panel`), feedback (`ProgressBar`, `Spinner`,
`ValidationMessage`), navigation (`Tabs`, `Menu`, `Toolbar`), and overlays
(`Tooltip`, `Dialog`, `Toast`, `Overlay`). Charts (`LineChart`, `BarChart`,
`PieChart`, `ScatterPlot`, `GraphView`) and a virtualized `DataGrid` round out
the data controls. The [Controls report](../reports/controls.html) records the
governed catalog at 47 supported rows across these categories.

Authoring follows one uniform shape — read persistent state from the model,
emit messages from events:

```fsharp
let view model =
    Stack.create [
        Stack.children [
            TextBlock.create [ TextBlock.text model.Title ]
            TextBox.create [
                TextBox.value model.Name
                TextBox.validation model.NameValidation
                TextBox.onChanged NameChanged ]
            Button.create [
                Button.text "Save"
                Button.enabled model.CanSave
                Button.onClick SaveRequested ] ] ]
```

Under all the typed modules sit the lower-level builders in
[`Control`](../reference/fs-skia-ui-controls-control.html): `create`,
`standard` (over the typed
[`StandardControlKind`](../reference/fs-skia-ui-controls-standardcontrolkind.html)),
`customControl`, `withKey`, and the lowering helpers `lowerStandard` /
`lowerCustom`. These are the structural seam the typed front door and the catalog
build on.

## Rendering: from tree to scene

[`Control.render`](../reference/fs-skia-ui-controls-control.html) takes a `Theme`
and a control and returns a
[`ControlRenderResult<'msg>`](../reference/fs-skia-ui-controls-controlrenderresult-1.html):

```fsharp
type ControlRenderResult<'msg> =
    { Scene: Scene
      Layout: LayoutNode
      Diagnostics: ControlDiagnostic list
      EventBindings: ControlEventBinding<'msg> list
      NodeCount: int }
```

This is the composition point over the lower layers. The render produces:

- a `Scene` (from the [Scene](./scene.html) layer) — the drawing vocabulary the
  viewer host turns into Vulkan/Skia operations;
- a `LayoutNode` (from the [Layout](./layout.html) layer) — the resolved geometry;
- a list of
  [`ControlDiagnostic`](../reference/fs-skia-ui-controls-controldiagnostic.html)
  with codes such as `MissingRequiredAttribute`, `MissingStableKey`,
  `ContrastFailure`, and `KeyCollision`, surfaced rather than thrown;
- `EventBindings` mapping a `ControlId` and event kind to a `ControlEvent ->
  'msg` dispatcher, which is how input is routed back to your messages;
- a `NodeCount` for cheap size diagnostics.

The [`Theme`](../reference/fs-skia-ui-controls-theme.html) record carries
foreground/background/accent/danger/muted colors, font, density, corner radius,
and the contrast ratio required for the `ContrastFailure` check. The
[`Theme`](../reference/fs-skia-ui-controls-theme.html) module supplies built-in
`light` and `dark` palettes plus `withDensity`, `withAccent`, and `resolve` for
overrides. The deeper design-token pipeline that *generates* the theme primitives
is covered in the
[typed front door & Penpot deep dive](../controls-design/typed-front-door.html).

Beyond `render`, [`Control`](../reference/fs-skia-ui-controls-control.html)
exposes `dispatch: ControlEvent -> Control<'msg> -> 'msg list` (compute the
messages an event would produce), `diagnostics` (inspect without rendering), and
`count`.

## Interaction state and runtime effects

Persistent values — text, selected items, validation, committed values — live in
your model. *Transient* interaction state (focus, hover, pressed, caret,
selection, composition, drag) lives in a product-owned
[`ControlRuntimeModel`](../reference/fs-skia-ui-controls-controlruntimemodel.html),
an ordinary Elmish sub-model from
[`ControlRuntime.fsi`](https://github.com/FS-Skia-UI/FS-Skia-UI/blob/main/src/Controls/ControlRuntime.fsi):

```fsharp
module ControlRuntime =
    val init: unit -> ControlRuntimeModel * ControlRuntimeEffect list
    val update: msg: ControlRuntimeMsg -> model: ControlRuntimeModel -> ControlRuntimeModel * ControlRuntimeEffect list
    val diagnostics: model: ControlRuntimeModel -> ControlDiagnostic list
```

[`ControlRuntimeMsg`](../reference/fs-skia-ui-controls-controlruntimemsg.html)
covers `FocusControl`, `HoverControl`, `PressControl`/`ReleaseControl`,
`SetCaret`, `SetSelection`, composition start/commit, drag start/move/end,
`RemoveControl`, `RecoverStaleTarget`, `CancelInteraction`, and `Reset`. Each
turn emits
[`ControlRuntimeEffect`](../reference/fs-skia-ui-controls-controlruntimeeffect.html)
values such as `FocusChanged`, `HoverChanged`, `DragChanged`, `StaleTarget`, and
`ReportControlRuntimeDiagnostic` — descriptions, not actions. Virtualized
collections have a parallel pure sub-model in
[`Collections`](../reference/fs-skia-ui-controls-collections.html)
(`CollectionModel`/`CollectionMsg`/`CollectionEffect`), which computes a
`VisibleRange` from row height, viewport height, and scroll offset and emits
`VisibleRangeChanged`.

## The Elmish adapter

`FS.Skia.UI.Controls.Elmish` turns those runtime effects into a standard Elmish
program. Its effect envelope and program record come from
[`ControlsElmish.fsi`](https://github.com/FS-Skia-UI/FS-Skia-UI/blob/main/src/Controls.Elmish/ControlsElmish.fsi):

```fsharp
type AdapterEffect<'msg> =
    | DispatchProductMessage of 'msg
    | DispatchControlRuntimeMessage of ControlRuntimeMsg
    | DispatchKeyboardMessage of KeyboardMsg
    | DispatchHostCommand of string
    | ReportAdapterDiagnostic of AdapterDiagnostic

type AdapterCommand<'msg> = AdapterEffect<'msg> list
```

The adapter is a set of total, pure lowering functions on
[`ControlsElmish`](../reference/fs-skia-ui-controls-elmish-controlselmish.html):

- `interpretKeyboardEffect` lowers a `KeyboardEffect` into an
  `AdapterCommand<'msg>`, mapping each fired `CommandId` through your function.
- `interpretControlEffect` lowers a `ControlRuntimeEffect`, mapping
  `ControlRuntimeMsg` values through your router.
- `interpretPointerEffect` / `interpretPointerOutcome` (feature 075) lower
  pointer interactions: diagnostics become `ReportAdapterDiagnostic`, and every
  other interaction is offered to a consumer router that may return `None` (a
  no-op). `interpretPointerOutcome` applies the `ControlRuntime` messages first
  to keep runtime state consistent, then the interactions.
- `subscriptions` merges keyboard and control subscription lists; `program`
  assembles an
  [`AdapterProgram<'model,'msg>`](../reference/fs-skia-ui-controls-elmish-adapterprogram-2.html)
  from `init`/`update`/`view`/`subscriptions`; `diagnostic` builds an
  `AdapterDiagnostic`.

The bridge to Elmish proper is the
[`AdapterCmd`](../reference/fs-skia-ui-controls-elmish-adaptercmd.html) module
(feature 068): `toCmd` converts an `AdapterCommand<'msg>` to an Elmish
`Cmd<'msg>` by routing *every* effect case — product and non-product — through a
caller-supplied function, preserving order, with `[]` mapping to `Cmd.none`. Its
laws (`toCmd route [] = none`, `productMessages (ofMessage m) = [ m ]`) make the
round trip checkable. So the full flow is: a pointer/keyboard/control event
produces runtime effects → an `interpret*` function lowers them to an
`AdapterCommand` → `AdapterCmd.toCmd` lifts that to an Elmish `Cmd` → the message
flows through your `update` → `view` re-projects the model to a `Control<'msg>` →
`Control.render` produces the next scene.

## The typed front door (pointer to the deep dive)

There is a typed authoring surface, `Widget<'msg>`, that wraps the lowered
`Control<'msg>` IR behind a sealed type with `Widget.ofControl` / `Widget.toControl`
/ `Widget.render`. The adapter integrates it directly: `widgetView` adapts a
`'model -> Widget<'msg>` view to the `Control<'msg>` the program expects (via
`Widget.toControl`), and `programOfWidget` builds a program whose view is authored
with the typed front door. The typed Props/MVU per-control surface, its parity
guarantees, and the design-token / Penpot integration are deliberately **out of
scope here** — see
[Typed control front door & Penpot flow](../controls-design/typed-front-door.html).

## Related pages

- [Namespace reference: `FS.Skia.UI.Controls`](../reference/fs-skia-ui-controls.html)
  · [`FS.Skia.UI.Controls.Elmish`](../reference/fs-skia-ui-controls-elmish.html)
- [API reference index](../reference/index.html)
- [Scene](./scene.html) · [Layout](./layout.html) · [Input](./input.html) ·
  [Elmish / MVU runtime](./elmish-mvu.html)
- [Typed control front door & Penpot flow](../controls-design/typed-front-door.html)
- [Controls report](../reports/controls.html) ·
  [Controls boundary refactor process report](../reports/controls-boundary-refactor-process-report.html)

## Analysis

### Implementation strengths

- A single generic record, `Control<'msg>`, expresses every control; the typed
  per-control modules (`Button`, `TextBox`, `Stack`, …) are thin builders over it,
  so the kernel that `render`, `dispatch`, and the catalog reason about stays
  small and uniform.
- Rendering returns errors as data: `ControlRenderResult.Diagnostics` carries
  typed `ControlDiagnosticCode` values (contrast, missing key, key collision)
  rather than throwing, which matches the framework's "report, don't silently
  fall back" stance from the [Controls report](../reports/controls.html).
- The adapter's lowering functions are documented as total and pure, with stated
  algebraic laws on `AdapterCmd` (`toCmd route [] = none`,
  `productMessages (ofMessage m) = [ m ]`), making the Elmish bridge testable
  without a host.
- Transient interaction state is a separate, ordinary Elmish sub-model
  (`ControlRuntime`, `Collections`), so focus/hover/drag and virtualization are
  unit-testable in isolation and never hidden inside mutable widget objects.

### Implementation weaknesses

- `AttrValue<'msg>` includes `UntypedValue of obj` and `StandardUntyped of obj`
  escape hatches; convenient for custom controls, but they punch a hole in the
  otherwise typed attribute surface that the compiler cannot police.
- Attributes are an untyped-by-name `Attr<'msg>` list (`Name: string`,
  `Category`), so an attribute applied to the wrong control kind is caught only at
  render time as a diagnostic, not at compile time.
- The
  [Controls boundary refactor process report](../reports/controls-boundary-refactor-process-report.html)
  records that the focused `ControlsRenderingCheck` was historically coupled to a
  broad `Build` and failed in the aggregate under memory pressure even when serial
  rendering tests passed — evidence that the verification path around this surface
  has been fragile.
- Re-rendering rebuilds the full `Control` tree to a `Scene` on each turn; there
  is no caching or diffing at this layer, so render cost tracks the whole view's
  complexity every update.

### Design pros

- Persistent state in the model, transient state in `ControlRuntime`, and the
  view as a pure projection give a clean separation that keeps product code
  declarative and the controls layer stateless about application data.
- The adapter adds no new effect case for keyboard, control-runtime, or pointer
  lowering — pointer support (075) reused the existing `AdapterEffect` cases — so
  the integration surface stayed stable as capabilities were added.
- A governed catalog (`catalog.yml` + `Catalog.supportedControls`, 47 rows) makes
  the supported surface explicit and machine-checkable, with deliberate custom
  escape hatches (`customControl`, custom attribute/event) for extension.
- Folding `Controls` and `Controls.Elmish` into one suite — and routing the typed
  front door through the same lowering seam — means there is one IR
  (`Control<'msg>`) and one render path, not parallel stacks to keep in sync.

### Design cons

- The layer leans on Elmish and on its sibling packages (`Scene`, `Layout`,
  `Input`, `KeyboardInput`); a consumer must understand the whole stack and own
  the `ControlRuntime` and keyboard sub-models, which is more wiring than a
  batteries-included widget toolkit.
- Splitting controls into `Controls` (pure tree + render) and `Controls.Elmish`
  (effect lowering) is principled but means a fully interactive app spans two
  packages and several `interpret*` calls before a click becomes a message.
- The string-keyed attribute and control-kind model trades compile-time safety
  for a small, uniform IR; the typed front door exists precisely to recover some
  of that safety, but it is a separate, opt-in surface rather than the default.
- Pushing transient state ownership onto the product is flexible but shifts a
  recurring boilerplate burden — initialising and threading `ControlRuntime`,
  keyboard, and collection sub-models — onto every consuming application.
