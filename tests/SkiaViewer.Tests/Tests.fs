module SkiaViewerCapabilityTests

open Expecto
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

[<Tests>]
let tests =
    testList "SkiaViewer MVU contract" [
        test "init emits window-open effect" {
            let model, effects = Viewer.init { Title = "Product"; InitialSize = { Width = 640; Height = 480 } }
            Expect.isFalse model.IsRunning "viewer starts stopped"
            Expect.equal effects [ OpenWindow("Product", { Width = 640; Height = 480 }) ] "init emits open effect"
        }

        test "render updates model and emits render effect" {
            let model, _ = Viewer.init { Title = "Product"; InitialSize = { Width = 640; Height = 480 } }
            let scene = Group []
            let next, effects = Viewer.update (Render scene) model
            Expect.equal next.LastScene (Some scene) "last scene is stored"
            Expect.equal effects [ RenderScene scene ] "render effect is emitted"
        }
    ]
