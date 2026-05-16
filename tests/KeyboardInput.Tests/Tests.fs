module KeyboardInputCapabilityTests

open Expecto
open FS.Skia.UI.KeyboardInput

[<Tests>]
let tests =
    testList "Keyboard input MVU contract" [
        test "key down emits command and key-state effects" {
            let model, _ = Keyboard.init [ { Key = "K"; Command = "open" } ]
            let next, effects = Keyboard.update (KeyDown "K") model
            Expect.equal next.LastCommand (Some "open") "last command is stored"
            Expect.equal effects [ KeyStateChanged [ "K" ]; CommandResolved "open" ] "effects are emitted"
        }
    ]
