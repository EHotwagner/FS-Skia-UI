(**
---
title: Typed controls & the MVU front door
category: Examples
categoryindex: 7
index: 1
---

# Typed controls & the MVU front door

This literate script is **evaluated at documentation-build time** (`fsdocs build
--eval`), so the code here is compiler-verified against the real
`FS.Skia.UI.Controls` API — if it stopped compiling, the docs build would surface
it.

It shows the *typed Props/MVU front door* under `FS.Skia.UI.Controls.Typed`: you
author an immutable **Props** record per control, compose `Widget<'msg>` values,
and the front door **lowers** them to the same legacy `Control<'msg>` IR the
framework already renders. The lowering is the single seam — everything below the
seam is unchanged.

We reference the locally built assemblies (the same ones the API reference is
generated from). `FS.Skia.UI.Scene` / `FS.Skia.UI.Layout` are copied next to
`FS.Skia.UI.Controls` in its build output, so one include path covers all three.
*)

#I "../../src/Controls/bin/Debug/net10.0"
#r "FS.Skia.UI.Scene.dll"
#r "FS.Skia.UI.Layout.dll"
#r "FS.Skia.UI.Controls.dll"

open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed

(**
## The MVU pieces

A control's interactions are expressed as your own `Msg` type — the typed front
door carries `'msg` end to end. `OnClick`/`OnChanged` are `'msg option`, so
*omitting* a handler lowers to **no** event binding rather than a default message.
*)

type Msg =
    | Increment
    | ToggleDone of bool

type Model = { Count: int; Done: bool }

let init = { Count = 0; Done = false }

/// A pure `update` — no I/O, no mutation (Principle IV).
let update (msg: Msg) (model: Model) : Model =
    match msg with
    | Increment -> { model with Count = model.Count + 1 }
    | ToggleDone v -> { model with Done = v }

(**
## Authoring with typed Props

Each control exposes a `defaults` record and a `view`. You override only the
fields you care about with F#'s record-`with` syntax — the compiler checks every
field name and type. Composing children in a `Stack` unifies the `'msg` type
across the whole tree.
*)

let view (model: Model) : Widget<Msg> =
    Stack.view
        { Stack.defaults with
            Orientation = Vertical
            Spacing = 8.0
            Children =
                [ TextBlock.view { TextBlock.defaults with Text = $"Count: {model.Count}" }
                  // OnClick = Some -> a real event binding to your Msg.
                  Button.view
                      { Button.defaults with
                          Text = "+1"
                          Intent = Primary
                          OnClick = Some Increment }
                  CheckBox.view
                      { CheckBox.defaults with
                          Text = "Done"
                          Checked = model.Done
                          OnChanged = Some ToggleDone }
                  // OnClick = None -> lowers to NO binding, not a default message.
                  Button.view { Button.defaults with Text = "(disabled)"; Enabled = false } ] }

(**
## Lowering to the legacy IR

`Widget.toControl` is the explicit seam to the existing `Control<'msg>` record
that the renderer and the Elmish adapter consume. We can inspect it directly —
it is a plain immutable record (`Kind`, `Children`, `Content`, `Attributes`), so
no rendering / GPU is involved.
*)

let lowered : Control<Msg> = Widget.toControl (view init)

printfn "root kind     : %s" lowered.Kind
printfn "child count   : %d" (List.length lowered.Children)

lowered.Children
|> List.iteri (fun i child ->
    printfn "  child %d -> kind=%s content=%A attrs=%d" i child.Kind child.Content (List.length child.Attributes))

(**
The `+1` button lowers with an extra event-binding attribute, while the disabled
button (no `OnClick`) does not — the typed front door faithfully reflects
"omitted handler = no binding". Driving the model is ordinary MVU:
*)

let afterClick = update Increment init
printfn "after one +1  : Count=%d Done=%b" afterClick.Count afterClick.Done

(**
That is the whole front door: typed Props in, `Widget<'msg>` composed, lowered to
the legacy IR for rendering — proven structurally equal to the hand-written
builders by per-control parity tests. See the
[typed front-door deep dive](../controls-design/typed-front-door.html) and the
[API reference](../reference/index.html).
*)
