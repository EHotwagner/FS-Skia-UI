# Phase 1 Data Model — Controls.Elmish Command Model (Widget View + Cmd Alignment)

This feature adds **no new data types** — it adds **functions** over existing types. The
"model" here is the additive API algebra and the laws each function obeys. All additions live
in `module ControlsElmish` / a new `module AdapterCmd`, both in
`src/Controls.Elmish/ControlsElmish.fsi` / `.fs`. `'msg` and `'model` are product-owned and
threaded through unchanged.

## Existing types (unchanged — shown for context)

From `src/Controls.Elmish/ControlsElmish.fsi` (FR-002 — none of these change):

```
type AdapterDiagnostic       = { Code: string; Message: string; Source: string }
type AdapterEffect<'msg> =
    | DispatchProductMessage        of 'msg
    | DispatchControlRuntimeMessage of ControlRuntimeMsg
    | DispatchKeyboardMessage       of KeyboardMsg
    | DispatchHostCommand           of string
    | ReportAdapterDiagnostic       of AdapterDiagnostic
type AdapterCommand<'msg>      = AdapterEffect<'msg> list
type AdapterSubscription<'msg> = { Id: string; Subscribe: unit -> AdapterCommand<'msg> }
type AdapterProgram<'model,'msg> =
    { Init: unit -> 'model * AdapterCommand<'msg>
      Update: 'msg -> 'model -> 'model * AdapterCommand<'msg>
      View: 'model -> Control<'msg>          // <-- still Control<'msg>; NOT changed
      Subscriptions: 'model -> AdapterSubscription<'msg> list }
```

From `src/Controls/Widget.fsi` (065, unchanged): the opaque `Widget<'msg>` and the lowering
seam `Widget.toControl : Widget<'msg> -> Control<'msg>` (invariant `toControl (ofControl c) = c`).

From `Fable.Elmish` (already referenced): `Cmd<'msg>` and `Cmd.none`.

## New functions

### `ControlsElmish.widgetView`
```
val widgetView : view:('model -> Widget<'msg>) -> ('model -> Control<'msg>)
```
- **Definition**: `view >> Widget.toControl` (pure).
- **Law (parity, FR-004/SC-002)**: `widgetView view model = Widget.toControl (view model)`,
  structurally — feeding it to the existing `program` produces a `View` byte-identical to the
  hand-written `view >> Widget.toControl` boundary.

### `ControlsElmish.programOfWidget`
```
val programOfWidget :
    init: (unit -> 'model * AdapterCommand<'msg>) ->
    update: ('msg -> 'model -> 'model * AdapterCommand<'msg>) ->
    view: ('model -> Widget<'msg>) ->
    subscriptions: ('model -> AdapterSubscription<'msg> list) ->
        AdapterProgram<'model,'msg>
```
- **Definition**: `program init update (widgetView view) subscriptions` (pure).
- **Law (FR-001)**: `(programOfWidget i u v s).View model = Widget.toControl (v model)` and
  the `Init`/`Update`/`Subscriptions` fields are exactly `i`/`u`/`s` — i.e. identical to
  `program i u (v >> Widget.toControl) s`. No product code calls `Widget.toControl` (SC-001).

### `module AdapterCmd`
```
val none            : Cmd<'msg>
val ofMessage       : msg:'msg -> AdapterCommand<'msg>
val productMessages : command:AdapterCommand<'msg> -> 'msg list
val toCmd           : route:(AdapterEffect<'msg> -> 'msg) -> command:AdapterCommand<'msg> -> Cmd<'msg>
```

- **`none`** = `Cmd.none`. Law: `toCmd route [] = none` (empty-command edge).
- **`ofMessage msg`** = `[ DispatchProductMessage msg ]`. Law:
  `productMessages (ofMessage m) = [ m ]`.
- **`productMessages command`** = `command |> List.choose (function DispatchProductMessage m -> Some m | _ -> None)`
  — ordered, no other case contributes.
- **`toCmd route command`** — one Elmish sub per effect, in list order, each dispatching
  `route effect`; **total** over every `AdapterEffect` case (FR-003). Pure to construct.
  - **Law (round-trip, FR-008/SC-003)**: dispatching `toCmd (fun e -> match e with DispatchProductMessage m -> m | other -> route other) command`
    through a recording dispatcher yields, for the product-message cases, exactly
    `productMessages command` in order. For a command of only `DispatchProductMessage`
    payloads: `dispatchedMessages (toCmd projectProduct command) = productMessages command`.
  - **Law (order)**: dispatch order equals `List.map route command` order (FR-003 ordering).
  - **Totality (SC-007-style)**: never throws for any `command`/`route`; `route`'s totality
    is the type-level guarantee no effect is dropped.

## Validation / invariants summary

| Invariant | Source | Where proven |
| --- | --- | --- |
| `programOfWidget` ≡ `program (view >> Widget.toControl)` | FR-001, FR-004, SC-001/002 | US1 parity test |
| `toCmd route []  = Cmd.none` | FR-003 empty edge | AdapterCmd unit test |
| dispatch order = effect-list order | FR-003 | AdapterCmd unit test |
| product-message round-trip (≥1000 cases) | FR-008, SC-003 | FsCheck property |
| every existing signature unchanged | FR-002, SC-004 | contract test + compile |
| base Controls has no `Fable.Elmish` | FR-006, SC-005 | dependency guard test |
| surface delta additive, this package only | FR-007, SC-006 | PackageSurfaceCheck / PerPackageSurfaceDiff |

No state transitions: every addition is a pure function; the adapter owns no new runtime
state (the MVU state stays product-owned, threaded through `init`/`update` unchanged).
