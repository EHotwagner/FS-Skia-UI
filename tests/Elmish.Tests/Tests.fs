module ElmishCapabilityTests

open Expecto
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer
open FS.Skia.UI.Elmish

[<Tests>]
let tests =
    testList "Elmish adapter contract" [
        test "init maps viewer effects" {
            let scene = Scene.empty
            let _, effects = ElmishAdapter.init { Title = "Product"; InitialSize = { Width = 320; Height = 240 } } 0 scene
            Expect.equal effects [ DispatchViewer(OpenWindow("Product", { Width = 320; Height = 240 })) ] "viewer effect is mapped"
        }
    ]
