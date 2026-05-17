module SceneCapabilityTests

open Expecto
open FS.Skia.UI.Scene

[<Tests>]
let tests =
    testList "Scene public contract" [
        test "rectangle descriptions are stable" {
            let node =
                Scene.rectangle
                    (0.0, 0.0, 10.0, 20.0)
                    (Colors.rgb 1uy 2uy 3uy)

            Expect.contains (Scene.describe node) RectangleElement "scene description includes rectangle kind"
        }
    ]
