#r "../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"

open FS.Skia.UI.Scene

let outputSize = { Width = 640; Height = 480 }
let fill = Colors.rgb 64uy 196uy 255uy

let circleScene = Scene.circle { X = 32.0; Y = 32.0 } 16.0 fill
let ellipseScene = Scene.filledEllipse { X = 600.0; Y = 456.0; Width = 72.0; Height = 36.0 } fill
let scene = Scene.group [ circleScene; ellipseScene ]

let circleFacts = Scene.circleEvidence outputSize { X = 32.0; Y = 32.0 } 16.0 fill
let ellipseFacts = Scene.ellipseEvidence outputSize { X = 600.0; Y = 456.0; Width = 72.0; Height = 36.0 } fill
let renderFacts = Scene.renderReadbackEvidence outputSize scene

printfn "status=ok"
printfn "command=dotnet fsi specs/022-breakout-demo-feedback/readiness/scene-shape-evidence.fsx"
printfn "evidence-kind=deterministic-scene-shape"
printfn "circle-center=%f,%f" circleFacts.Center.X circleFacts.Center.Y
printfn "circle-radius=%f" circleFacts.Radius
printfn "circle-bounds=%f,%f,%f,%f" circleFacts.Bounds.X circleFacts.Bounds.Y circleFacts.Bounds.Width circleFacts.Bounds.Height
printfn "circle-fill=%A" circleFacts.Fill
printfn "circle-placement=%A" circleFacts.Placement
printfn "ellipse-bounds=%f,%f,%f,%f" ellipseFacts.Bounds.X ellipseFacts.Bounds.Y ellipseFacts.Bounds.Width ellipseFacts.Bounds.Height
printfn "ellipse-fill=%A" ellipseFacts.Fill
printfn "ellipse-placement=%A" ellipseFacts.Placement
printfn "capabilities=%s" (renderFacts.Capabilities |> String.concat ",")
printfn "deterministic-hash=%s" renderFacts.DeterministicHash

