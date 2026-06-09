// T007 — FSI exercise of the feature-085 foundation:
//   * Control.renderTree distinctness (SC-001) + nested-children paint.
//   * InteractiveAppHost construction + the pointer-routing wiring used by runInteractiveApp.
#I "/home/developer/projects/FS-Skia-UI/src/Controls.Elmish/bin/Debug/net10.0"
#I "/home/developer/projects/FS-Skia-UI/samples/ControlsGallery/bin/Debug/net10.0"
#r "Yoga.Net.dll"
#r "FS.Skia.UI.Scene.dll"
#r "FS.Skia.UI.Layout.dll"
#r "FS.Skia.UI.KeyboardInput.dll"
#r "FS.Skia.UI.Controls.dll"
#r "FS.Skia.UI.SkiaViewer.dll"
#r "FS.Skia.UI.Controls.Elmish.dll"

open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.SkiaViewer

type Msg = Clicked

let theme = Theme.light
let size = { Width = 640; Height = 480 }

// --- 1. renderTree distinctness (SC-001) ------------------------------------------------
let pageA =
    Stack.create [ Stack.children [ TextBlock.create [ TextBlock.text "Alpha" ]
                                    Button.create [ Button.text "Go"; Button.onClick Clicked ] ] ]

let pageB: Control<Msg> =
    Stack.create [ Stack.children [ TextBlock.create [ TextBlock.text "Beta" ]
                                    Stack.create [ Stack.children [ TextBlock.create [ TextBlock.text "Nested" ] ] ] ] ]

let a = Control.renderTree theme size pageA
let b = Control.renderTree theme size pageB

printfn "renderTree A node-count=%d  bindings=%d" a.NodeCount (List.length a.EventBindings)
printfn "renderTree B node-count=%d" b.NodeCount
printfn "SC-001 scenes differ = %b" (a.Scene <> b.Scene)
printfn "A scene top-level nodes = %d (nested children laid out + painted)" (List.length a.Scene.Nodes)

// --- 2. InteractiveAppHost construction + pointer routing -------------------------------
let host: InteractiveAppHost<int, Msg> =
    { Init = fun () -> 0, []
      Update = fun Clicked model -> model + 1, []
      View = fun _ _ -> Button.create [ Button.text "Go"; Button.onClick Clicked ] |> Control.withKey "go"
      Theme = theme
      MapKey = fun _ _ -> None
      MapPointer = fun _ -> Some Clicked
      Tick = fun _ -> None
      Diagnostics = Viewer.defaultDiagnostics }

printfn "InteractiveAppHost constructed; runInteractiveApp is %s"
    (if (box ControlsElmish.runInteractiveApp <> null) then "bound" else "missing")

// Exercise the SAME hit-test + route the live window uses, with a synthetic press at the button.
let rendered = Control.renderTree theme size (host.View size 0)
let available: FS.Skia.UI.Layout.AvailableSpace =
    { Width = float size.Width; WidthMode = FS.Skia.UI.Layout.Exactly
      Height = float size.Height; HeightMode = FS.Skia.UI.Layout.Exactly }
let layoutResult = FS.Skia.UI.Layout.Layout.evaluate available rendered.Layout
let policy = FS.Skia.UI.Layout.Defaults.pixelSnapPolicy 1.0
// hit-test the centre of the button's computed bounds
let goBounds = layoutResult.Bounds |> List.tryFind (fun cb -> cb.NodeId = "go")
match goBounds with
| Some cb ->
    let cx = cb.Bounds.X + cb.Bounds.Width / 2.0
    let cy = cb.Bounds.Y + cb.Bounds.Height / 2.0
    let _, downInteractions, _ = Pointer.update policy layoutResult (PointerMsg.Down(PointerButton.Primary, cx, cy)) (Pointer.init ())
    printfn "synthetic Down at (%g,%g) -> %d interaction(s): %A" cx cy (List.length downInteractions) downInteractions
| None -> printfn "go bounds not found"

printfn "FSI exercise complete."
