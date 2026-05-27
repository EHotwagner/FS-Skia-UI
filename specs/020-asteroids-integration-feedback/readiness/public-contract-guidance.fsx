#r "../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../src/Testing/bin/Debug/net10.0/FS.Skia.UI.Testing.dll"

open FS.Skia.UI.Scene
open FS.Skia.UI.Testing

let readable =
    { Scene = Scene.text (16.0, 28.0) "Score 100" Colors.white
      OutputSize = { Width = 1280; Height = 720 }
      ProofLevel = ReadableLayout
      HudRegion = Some { Name = "hud"; Bounds = { X = 0.0; Y = 0.0; Width = 1280.0; Height = 96.0 } }
      GameplayRegion = Some { Name = "gameplay"; Bounds = { X = 0.0; Y = 96.0; Width = 1280.0; Height = 624.0 } }
      TextBounds = [ { Name = "score"; Text = "Score 100"; Bounds = { X = 16.0; Y = 12.0; Width = 92.0; Height = 24.0 }; MeasurementMode = ExactTextBounds } ]
      GameplayBounds = [ { Name = "ship"; Bounds = { X = 620.0; Y = 420.0; Width = 28.0; Height = 28.0 } } ]
      OverlapStatus = NoLayoutOverlap
      MeasurementMode = ExactTextBounds
      UnsupportedReasons = []
      Diagnostics = [ "hud-readable=true"; "gameplay-contained=true" ]
      RenderEvidence = None }

let deterministicOnly =
    { readable with
        ProofLevel = DeterministicRenderOnly
        HudRegion = None
        GameplayRegion = None
        TextBounds = []
        GameplayBounds = []
        Diagnostics = [ "deterministic render metadata only" ]
        RenderEvidence = Some(Scene.renderReadbackEvidence { Width = 640; Height = 480 } Scene.empty) }

let unsupported =
    { readable with
        ProofLevel = UnsupportedLayoutInspection
        TextBounds = []
        MeasurementMode = UnsupportedTextBounds
        UnsupportedReasons = [ { Fact = "font-metrics"; Reason = "host metrics unavailable"; Diagnostic = "unsupported layout inspection" } ]
        Diagnostics = [ "unsupported fact: font-metrics" ] }

let validationCheck =
    { Report = readable
      RequireReadableLayout = true }

let validationResult =
    { Accepted = true
      FailureClass = None
      Diagnostics = [] }

printfn "readable-proof=%A text-bounds=%d gameplay-bounds=%d" readable.ProofLevel readable.TextBounds.Length readable.GameplayBounds.Length
printfn "deterministic-proof=%A render-hash=%s" deterministicOnly.ProofLevel deterministicOnly.RenderEvidence.Value.DeterministicHash
printfn "unsupported-proof=%A reasons=%d" unsupported.ProofLevel unsupported.UnsupportedReasons.Length
printfn "validation-check-requires-readable=%b accepted=%b" validationCheck.RequireReadableLayout validationResult.Accepted
