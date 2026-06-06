// T032 helper: regenerate the per-package surface baselines for the two packages
// that gained a new top-level `.fsi` (Scene/Animation.fsi, Elmish/AnimationTick.fsi).
// RefreshSurfaceBaselines does NOT touch per-package `.fsi.txt` snapshots, so this
// invokes PerPackageSurface.captureCurrent and writes the normalized surfaces.
#r "../../../build/bin/Debug/net10.0/FS.Skia.UI.Build.dll"

open System.IO
open FS.Skia.UI.Build.PerPackageSurface

let packages = [ "FS.Skia.UI.Scene"; "FS.Skia.UI.Elmish" ]

for surface in captureCurrent packages do
    let path = Path.Combine("readiness", "per-package-surface", surface.PackageId + ".fsi.txt")
    File.WriteAllText(path, surface.NormalizedText)
    printfn "wrote %s (%d chars)" path surface.NormalizedText.Length
