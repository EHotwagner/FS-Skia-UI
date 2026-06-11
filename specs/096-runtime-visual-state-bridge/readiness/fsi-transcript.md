# FSI transcript — public `ControlRuntime.deriveVisualState` (feature 096, T007)

evidence-kind=fsi-transcript
renderer-mode=DeterministicRenderOnly
status=pass

The public projection exercised against the built `FS.Skia.UI.Controls.dll` through its public
entry point (Principle I), matching the contract's FSI block
(`contracts/control-runtime-bridge.md`).

```fsharp
#r "FS.Skia.UI.Controls.dll"
open FS.Skia.UI.Controls
let m = { fst (ControlRuntime.init ()) with HoveredControl = Some "btn" }
ControlRuntime.deriveVisualState m "btn"        // Hover
ControlRuntime.deriveVisualState m "other"      // Normal
let pressed = { m with PressedControls = Set.ofList [ "btn" ] }
ControlRuntime.deriveVisualState pressed "btn"  // Pressed (out-ranks Hover)
let focused = { fst (ControlRuntime.init ()) with FocusedControl = Some "btn" }
ControlRuntime.deriveVisualState focused "btn"  // Focused
```

Observed session output:

```
deriveVisualState m "btn"   = Hover
deriveVisualState m "other" = Normal
deriveVisualState pressed "btn" = Pressed  // Pressed out-ranks Hover
deriveVisualState focused "btn" = Focused
```

result=pass — the public projection resolves the expected highest-ranked runtime-derivable state for
each input; an id named by no interaction state resolves to `Normal`.
authoritative-test=Feature096RuntimeBridgeTests/Feature 096 runtime visual-state bridge
