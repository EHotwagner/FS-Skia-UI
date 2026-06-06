# Quickstart — Controls.Elmish Command Model (feature 068)

Two additive entry points on `FS.Skia.UI.Controls.Elmish` let a product (a) author its view
with the typed front door and hand it to the adapter **without** a `Widget.toControl` shim,
and (b) fold adapter effects into a standard Elmish `Cmd<'msg>`.

## 1. Widget-returning view — no boundary shim

**Before (065 — the shim every typed product writes by hand):**

```fsharp
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish

let view (model: Model) : Control<Msg> =
    Typed.Stack.view
        { Typed.Stack.defaults with
            Children =
              [ Typed.TextBlock.view { Typed.TextBlock.defaults with Text = "Typed" }
                Typed.Button.view    { Typed.Button.defaults with Text = "Save"; OnClick = Some Save } ] }
    |> Widget.toControl                       // <-- the shim 068 removes

let program = ControlsElmish.program init update view subscriptions
```

**After (068 — `view` returns `Widget<'msg>`; the adapter lowers internally):**

```fsharp
let view (model: Model) : Widget<Msg> =       // returns Widget — no Widget.toControl here
    Typed.Stack.view
        { Typed.Stack.defaults with
            Children =
              [ Typed.TextBlock.view { Typed.TextBlock.defaults with Text = "Typed" }
                Typed.Button.view    { Typed.Button.defaults with Text = "Save"; OnClick = Some Save } ] }

let program = ControlsElmish.programOfWidget init update view subscriptions
```

`programOfWidget i u v s` is exactly `program i u (v >> Widget.toControl) s` — the rendered
`Control<'msg>` tree is structurally identical (parity, SC-002), and no `Widget.toControl`
appears in product code (SC-001). If you prefer to keep using `program`, compose the shim
once via `ControlsElmish.widgetView view` instead.

## 2. Adapter effects in a standard Elmish `Cmd<'msg>`

`AdapterCmd.toCmd` converts the adapter's effect list to an Elmish command. `route` is a
**total** mapping you supply for every effect case, so nothing is silently dropped:

```fsharp
open FS.Skia.UI.Controls.Elmish
open Elmish

// Fold every adapter effect into the product's own Msg space.
let route (effect: AdapterEffect<Msg>) : Msg =
    match effect with
    | DispatchProductMessage m        -> m
    | DispatchControlRuntimeMessage r -> RuntimeChanged r
    | DispatchKeyboardMessage k       -> KeyboardChanged k
    | DispatchHostCommand cmd         -> HostRequested cmd
    | ReportAdapterDiagnostic d       -> DiagnosticRaised d

// An AdapterCommand produced by the interpreters or by `update`:
let command : AdapterCommand<Msg> =
    ControlsElmish.interpretControlEffect RuntimeChanged someControlEffect

let cmd : Cmd<Msg> = AdapterCmd.toCmd route command   // dispatch order = effect order; [] -> Cmd.none
```

Round-trip (the property `068` proves, FR-008/SC-003): for a command of product messages,

```fsharp
let command = [ DispatchProductMessage A; DispatchProductMessage B ]
// dispatching `AdapterCmd.toCmd (function DispatchProductMessage m -> m | e -> route e) command`
// delivers exactly  AdapterCmd.productMessages command  =  [ A; B ]  in order.
```

## 3. What did NOT change

- `AdapterProgram`, `AdapterCommand`, `AdapterEffect`, `AdapterSubscription`, `program`, and
  the effect interpreters are byte-for-byte unchanged — existing programs compile and behave
  identically (SC-004).
- The base `FS.Skia.UI.Controls` package still has **no** `Fable.Elmish` reference (SC-005).
- The legacy `Control<'msg>`-returning `program` path stays as a permanent peer; the `Widget`
  path is simply the preferred one.

## Where the tests live

`tests/Elmish.Tests/` — `TypedControlsAdapterTests.fs` (US1 `programOfWidget` parity, next to
the existing 065 `widgetView` test), `AdapterCmdTests.fs` (US2 round-trip unit tests + FsCheck
property), and `ControlsElmishAdapterContractTests.fs` (the new `.fsi` surface assertion + the
retained base-Controls "no Fable.Elmish" dependency guard).
