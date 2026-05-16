module SceneCapabilityTests

open Expecto
open FS.Skia.UI.Scene

[<Tests>]
let tests =
    testList "Scene public contract" [
        test "rectangle descriptions are stable" {
            let node =
                Scene.rectangle
                    "root"
                    { X = 0.0
                      Y = 0.0
                      Width = 10.0
                      Height = 20.0 }
                    (Colors.rgb 1uy 2uy 3uy)

            Expect.stringContains (Scene.describe node) "Rectangle root" "scene description includes id"
        }
    ]
