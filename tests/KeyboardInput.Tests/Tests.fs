module KeyboardInputCapabilityTests

open Microsoft.FSharp.Reflection
open Expecto
open FS.Skia.UI.KeyboardInput

let recordFields<'T> =
    FSharpType.GetRecordFields(typeof<'T>)
    |> Array.map _.Name
    |> Set.ofArray

let unionCases<'T> =
    FSharpType.GetUnionCases(typeof<'T>)
    |> Array.map _.Name
    |> Set.ofArray

[<Tests>]
let tests =
    testList "Keyboard input MVU contract" [
        test "key down emits command and key-state effects" {
            let model, _ = Keyboard.init [ { Key = "K"; Command = "open" } ]
            let next, effects = Keyboard.update (KeyDown "K") model
            Expect.equal next.LastCommand (Some "open") "last command is stored"
            Expect.equal effects [ KeyStateChanged [ "K" ]; CommandResolved "open" ] "effects are emitted"
        }

        test "runtime model exposes layout modes pending sequence diagnostics and state display state" {
            let fields = recordFields<KeyboardModel>

            [ "PressedKeys"
              "ActiveLayout"
              "ActiveModeStack"
              "PersistentModeState"
              "PendingSequence"
              "Diagnostics"
              "RecentEffects"
              "StateDisplay" ]
            |> List.iter (fun field ->
                Expect.isTrue (Set.contains field fields) $"KeyboardModel exposes {field}")
        }

        test "messages and effects cover focus recovery mode behavior and interpreter data" {
            let messages = unionCases<KeyboardMsg>
            let effects = unionCases<KeyboardEffect>

            [ "KeyDown"
              "KeyUp"
              "FocusLost"
              "Reset"
              "SetActiveLayout"
              "PushTemporaryMode"
              "PopTemporaryMode"
              "SetPersistentMode"
              "ResolvePendingSequence" ]
            |> List.iter (fun caseName ->
                Expect.isTrue (Set.contains caseName messages) $"KeyboardMsg exposes {caseName}")

            [ "CommandResolved"
              "KeyStateChanged"
              "LayoutChanged"
              "ModeChanged"
              "PendingSequenceChanged"
              "StateDisplayChanged"
              "ReportKeyboardDiagnostic"
              "RequestHostKeyCapture" ]
            |> List.iter (fun caseName ->
                Expect.isTrue (Set.contains caseName effects) $"KeyboardEffect exposes {caseName}")
        }

        test "init emits inspectable state-display evidence for interpreters" {
            let _, effects = Keyboard.init []

            Expect.exists
                (effects |> List.map string)
                (fun effect -> effect.Contains("StateDisplay"))
                "initialization publishes state-display data through an effect"
        }

        test "focus loss clears temporary state preserves persistent modes and reports diagnostics" {
            let model, _ = Keyboard.init []
            let withMode, _ = Keyboard.update (SetPersistentMode("layout", "qwerty")) model
            let withTemporary, _ = Keyboard.update (PushTemporaryMode "symbols") withMode
            let withKey, _ = Keyboard.update (KeyDown "K") withTemporary
            let recovered, effects = Keyboard.update FocusLost withKey

            Expect.isEmpty recovered.PressedKeys "focus loss clears pressed keys"
            Expect.isEmpty recovered.ActiveModeStack "focus loss clears temporary modes"
            Expect.equal recovered.PersistentModeState["layout"] "qwerty" "persistent mode state survives focus loss"
            Expect.exists effects (function ReportKeyboardDiagnostic diagnostic when diagnostic.Code = "FocusLostRecovered" -> true | _ -> false) "focus loss reports recovery diagnostic"
        }
    ]
