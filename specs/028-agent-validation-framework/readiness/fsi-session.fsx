#r "../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../src/Layout/bin/Debug/net10.0/FS.Skia.UI.Layout.dll"
#r "../../../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../../../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"
#r "../../../src/Controls/bin/Debug/net10.0/FS.Skia.UI.Controls.dll"

open System
open FS.Skia.UI.AgentValidation
open FS.Skia.UI.Controls

let model, effects = ValidationSelection.init (Some "028-agent-validation-framework")
printfn "ValidationSelection.init Feature=%A Effects=%A" model.Feature effects

let selected, selectedEffects =
    ValidationSelection.update
        (ActiveFeatureMetadataLoaded("028-agent-validation-framework", [ "src/Controls/Types.fsi"; "build.fsx" ]))
        model

printfn "ValidationSelection.update ChangedPathSource=%A Effects=%A" selected.ChangedPathSource selectedEffects

let verdict =
    { Status = Degraded
      Authority = AgentReadyAuthority
      ChangedPathSource =
        { Kind = ActiveFeatureMetadata
          Feature = Some "028-agent-validation-framework"
          MergeBase = None
          Paths = [ "src/Controls/Types.fsi" ]
          Diagnostics = [] }
      SelectedRuleIds = [ "controls-public-surface" ]
      RequiredGates = [ "PackageSurfaceCheck"; "FsiTranscripts"; "EvidenceGraph" ]
      CompletedGates = [ "EvidenceGraph" ]
      MissingGates = [ "PackageSurfaceCheck"; "FsiTranscripts" ]
      SkippedGates = []
      MissingArtifacts = []
      FailureOwner = Governance
      FailureClass = MissingEvidenceFailure
      NextCommand = Some "./fake.sh build -t AgentReady"
      Artifacts = []
      Diagnostics = [ "draft transcript" ]
      TimestampUtc = DateTimeOffset.Parse("2026-05-28T17:30:00+02:00") }

printfn "AgentVerdict JSON=%s" (AgentVerdict.toJson verdict)
printfn "AgentVerdict Markdown=%s" (AgentVerdict.toMarkdown verdict)

let typedButton : Control<string> =
    Control.standard
        StandardControlKind.Button
        [ Attr.standardAttribute StandardAttributeName.Text (StandardText "Save")
          Attr.standardEvent StandardEventKind.Click "Clicked" ]

let custom : Control<string> =
    Control.customControl
        "vendor-widget"
        [ Attr.customAttribute "vendor-mode" (box "compact")
          Attr.customEvent "vendor-activated" "Activated" ]

let dataGridMissingRows : Control<string> =
    Control.standard
        StandardControlKind.DataGrid
        [ Attr.standardAttribute StandardAttributeName.Columns (StandardUntyped [ "Name"; "Value" ]) ]

printfn "StandardControlKind Button -> Kind=%s Attributes=%d" typedButton.Kind typedButton.Attributes.Length
printfn "StandardEventKind Click customControl customAttribute customEvent -> %s %s %A" typedButton.Kind custom.Kind custom.Attributes
printfn "DataGrid validateStandardControl diagnostics=%A" (Catalog.validateStandardControl dataGridMissingRows)
printfn "Catalog knownControlKinds=%A" (Catalog.knownControlKinds ())
printfn "validateStandardControl Button diagnostics=%A" (Catalog.validateStandardControl typedButton)
