// Feature 071, T017: capture deterministic render-only viewport evidence for the typed
// gallery panel. Reuses the exact `ControlsTypedGalleryPanel.panel` the US2 suites render,
// lowers it through the existing `Control.render` IR path, and prints the deterministic
// readback hash at >=2 viewports. Pure (no wall-clock/random/I/O), so re-running prints
// byte-identical hashes.
#r "../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../src/Controls/bin/Debug/net10.0/FS.Skia.UI.Controls.dll"
#r "../tests/Controls.Tests/bin/Debug/net10.0/Controls.Tests.dll"
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls

let panel = ControlsTypedGalleryPanel.panel
let covered = ControlsTypedGalleryPanel.coveredCategories panel
let kinds = ControlsTypedGalleryPanel.kindsPresent panel

printfn "covered-categories=%s" (covered |> Set.toList |> String.concat ",")
printfn "control-kinds=%s" (kinds |> Set.toList |> String.concat ",")

for (w, h) in [ 320, 240; 1024, 768 ] do
    let rendered = Control.render Theme.light panel
    let evidence = Scene.renderReadbackEvidence { Width = w; Height = h } rendered.Scene
    printfn "viewport=%dx%d deterministic-hash=%s diagnostics=%d node-count=%d" w h evidence.DeterministicHash rendered.Diagnostics.Length rendered.NodeCount
