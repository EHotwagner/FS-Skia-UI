# Quickstart: Typed Controls Front Door

Author controls with the F# compiler as a guardrail. Open the typed namespace,
build with record literals over `defaults`, and finish with `Widget.toControl`.

```fsharp
open FS.Skia.UI.Controls          // Widget, Control, render
open FS.Skia.UI.Controls.Typed    // Button, TextBlock, Stack, CheckBox, ...

type Msg =
    | Save
    | NameChanged of string

// Compose typed controls; children are type-checked as part of the same surface.
let view : Widget<Msg> =
    Stack.view
        { Stack.defaults with
            Orientation = Vertical
            Spacing = 8.0
            Children =
                [ TextBlock.view { TextBlock.defaults with Text = "Sign in" }
                  Button.view
                      { Button.defaults with
                          Text = "Submit"
                          Intent = Primary
                          OnClick = Some Save } ] }

// Finish for the renderer / Elmish adapter — no adapter change needed (FR-009).
let result : ControlRenderResult<Msg> = Widget.render theme view
// or: let control = Widget.toControl view
```

## What the compiler now catches (US-1)

```fsharp
// ❌ wrong field type — does not compile
Button.view { Button.defaults with Text = 42 }
// ❌ wrong message type for OnClick — does not compile
Button.view { Button.defaults with OnClick = Some "save" }
// ✅ omit an optional field — documented default from `defaults` is used
Button.view { Button.defaults with Text = "OK" }   // Intent = Primary, Enabled = true
```

## Stateful control (US-3)

```fsharp
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed

let props = { TextBox.defaults "email" with Mode = SingleLine; Value = "" }
let model, effects = TextBox.init props          // delegates to TextInput.init
let model', effects' = TextBox.update (InsertText "a@b") model   // delegates to TextInput.update
let widget = TextBox.view props model'
```

## Migrating legacy controls (US-2)

```fsharp
// Drop an existing legacy Control<'msg> into a typed container during migration.
let legacy : Control<Msg> = Badge.create [ Badge.text "beta" ]
Stack.view
    { Stack.defaults with
        Children = [ Widget.ofControl legacy
                     TextBlock.view { TextBlock.defaults with Text = "Welcome" } ] }
```

## Guarantee

Each typed `view` lowers to a `Control<'msg>` structurally equal to the legacy
authoring call (parity-tested, SC-002), so render, layout, accessibility, and
event dispatch behave identically. The legacy string-keyed API keeps working
unchanged (SC-003).
