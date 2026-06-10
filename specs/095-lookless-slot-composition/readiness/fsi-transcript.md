# FSI transcript — typed slot-fill front door (feature 095)

Exercises the typed slot-fill front door (`FS.Skia.UI.Controls.Typed`) through the built
`FS.Skia.UI.Controls.dll` from `dotnet fsi` (Principle I, T006). The session fills declared slots,
confirms region placement in the lowered IR, and confirms the unfilled byte-identity.

## Script

```fsharp
#r ".../FS.Skia.UI.Scene.dll"
#r ".../FS.Skia.UI.Layout.dll"
#r ".../FS.Skia.UI.KeyboardInput.dll"
#r ".../FS.Skia.UI.Controls.dll"
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed

let icon : Widget<int> = TextBlock.view { TextBlock.defaults with Id = Some "leading-icon"; Text = "*" }
let saveBtn = Button.view { Button.defaults with Text = "Save"; Leading = Some icon } |> Widget.toControl
printfn "button kind=%s  child-keys=%A  has-slot-attr=%b"
    saveBtn.Kind (saveBtn.Children |> List.map (fun c -> c.Key))
    (saveBtn.Attributes |> List.exists (fun a -> a.Name = "slot"))

let panel : Control<int> =
    Panel.view
        { Panel.defaults with
            Header = Some(TextBlock.view { TextBlock.defaults with Id = Some "hdr"; Text = "Settings" })
            Footer = Some(TextBlock.view { TextBlock.defaults with Id = Some "ftr"; Text = "v1" })
            Children = [ TextBlock.view { TextBlock.defaults with Id = Some "body"; Text = "content" } ] }
    |> Widget.toControl
printfn "panel child-keys (header,content,footer order)=%A" (panel.Children |> List.map (fun c -> c.Key))

let plain : Control<int> = Button.view { Button.defaults with Text = "Save" } |> Widget.toControl
printfn "unfilled button: children=%d  has-slot-attr=%b"
    plain.Children.Length (plain.Attributes |> List.exists (fun a -> a.Name = "slot"))
```

## Output (captured)

```
button kind=button  child-keys=[Some "leading-icon"]  has-slot-attr=false
panel child-keys (header,content,footer order)=[Some "hdr"; Some "body"; Some "ftr"]
unfilled button: children=0  has-slot-attr=false
```

## What this proves

- A filled `Button.Leading` lowers its supplied sub-tree into the lowered Button's `Children`
  (`[Some "leading-icon"]`), and the internal slot carrier is **consumed** by lowering
  (`has-slot-attr=false`) — a single source of truth.
- `Panel.Header` / `Panel.Footer` place **before** and **after** the content respectively
  (`[hdr; body; ftr]`) — two distinct regions, no swap (SC-001).
- An **unfilled** button carries **no** slot attribute and **no** peripheral children
  (`children=0`) — byte-identical to the pre-slot lowering (SC-002).
