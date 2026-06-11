# Quickstart: an unkeyed `Button.onClick` that actually dispatches (R3)

R3 makes the **documented, obvious** authoring work in the live host. No `withKey`
required.

## Before R3

```fsharp
type Msg = Clicked

let view _model =
    Stack.vertical [
        Button.create [ Button.text "Save"; Button.onClick Clicked ]   // no withKey
    ]
```

The button renders, but a click does nothing: recovery (`nearestAuthored`) only saw
*keyed* nodes, returned `None`, and the interaction fell through to an unmapped
`MapPointer`. A dead button.

## After R3

Exactly the same view now dispatches `Clicked` on click. Under the hood:

1. `renderTree` emits `BoundIds` — the canonical ids (`Key ?? path`) of every bound node.
   The unkeyed button's id is its structural path, e.g. `"0.0"`.
2. A click resolves to that path id; `nearestAuthored` now treats the node as authored
   because its id is in `BoundIds`, and returns `Some "0.0"`.
3. `bindingMessagesFor` looks up `"0.0"` in `EventBindings` (same unified scheme), finds
   the `onClick` binding, and dispatches `Clicked`. `MapPointer` is not consulted.

## Same-kind siblings disambiguate

```fsharp
let view _ =
    Stack.horizontal [
        Button.create [ Button.text "A"; Button.onClick ClickedA ]   // id "0.0"
        Button.create [ Button.text "B"; Button.onClick ClickedB ]   // id "0.1"
    ]
```

Each button now mints a distinct canonical id (its path), so a click on B dispatches
`ClickedB` — not the colliding shared `"button"` id of the old `Kind` scheme.

## When you still need a stable id

Add `withKey` to pin the reported `ControlId` regardless of position:

```fsharp
Button.create [ Button.text "Save"; Button.onClick Clicked ] |> Control.withKey "save"
```

Keyed authoring is unchanged by R3 — the reported id stays `"save"`.

## Reading `ControlEvent.ControlId` / public `Bounds`

For **unkeyed** controls the reported id is now the structural path (`"0.1"`) rather than
the `Kind` string (`"button"`). Match on the path, or add a `Key` for a stable label.
Keyed controls report their `Key`, unchanged.

## Verify locally

The live-adapter routing seam exercises the exact path `runInteractiveApp` wires:

```fsharp
// Controls.Elmish.Tests — deliver a press+release Click over the button bounds and assert
// the authored message is dispatched (and MapPointer is not consulted).
let state', msgs = ControlsElmish.routeInteractivePointer host state size model pressInput
// ... release ...
// Expect: msgs contains Clicked
```
