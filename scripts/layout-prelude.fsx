#r "nuget: Fable.Elmish, 4.2.0"
#r "../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"
#r "../src/Layout/bin/Debug/net10.0/FS.Skia.UI.Layout.dll"

open FS.Skia.UI
open FS.Skia.UI.Layout

let config = Defaults.stackConfig 800.0 600.0
let child = Defaults.child (Scene.text (10.0, 20.0) "layout" Colors.white)
let scene = Layout.horizontalStack config [ child ]
printfn "layout-prelude scene=%A" (box scene |> isNull |> not)
