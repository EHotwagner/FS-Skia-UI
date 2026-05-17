#r "../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"

open FS.Skia.UI.KeyboardInput

let model, initEffects =
    Keyboard.init [ { Key = "K"; Command = "open" } ]

let afterKey, keyEffects =
    Keyboard.update (KeyDown "K") model

let recovered, recoveryEffects =
    Keyboard.update FocusLost afterKey

printfn "keyboard-package-init-effects=%A" initEffects
printfn "keyboard-package-command=%A" afterKey.LastCommand
printfn "keyboard-package-key-effects=%A" keyEffects
printfn "keyboard-package-recovered-pressed=%A" recovered.PressedKeys
printfn "keyboard-package-recovery-effects=%A" recoveryEffects
printfn "keyboard-package-display=%A" (Keyboard.stateDisplay recovered)
