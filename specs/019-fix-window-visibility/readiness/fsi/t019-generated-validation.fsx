#r "../../../../src/Testing/bin/Debug/net10.0/FS.Skia.UI.Testing.dll"

open FS.Skia.UI.Testing

let firstFrameOnly =
    "match List.ofArray args with\n| _ ->\n    printfn \"mode=interactive-window first-frame-only=true exit after first frame\"\n    0"

let metadataOnly =
    "match List.ofArray args with\n| _ ->\n    printfn \"mode=interactive-window accessible-window=true print metadata\"\n    0"

let missingAccessible =
    "match List.ofArray args with\n| _ ->\n    match Viewer.runApp viewerOptions generatedHost with\n    | Result.Ok outcome ->\n        printfn \"status=%s mode=interactive-window\" outcome.Status\n        0\n    | Result.Error _ -> 1"

for name, source in
    [ "first-frame-only", firstFrameOnly
      "metadata-only", metadataOnly
      "missing-accessible-window", missingAccessible ] do
    let result = GeneratedProductAssertions.validateDefaultInteractiveLaunch source
    printfn "case=%s accepted=%b diagnostics=%s" name result.InteractiveLaunchRequired (String.concat ";" result.Diagnostics)
