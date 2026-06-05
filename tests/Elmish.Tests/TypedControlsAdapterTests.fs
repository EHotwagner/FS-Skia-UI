module ControlsTypedAdapterTests

open System.IO
open Expecto
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish

let repositoryRoot =
    let rec find dir =
        if File.Exists(Path.Combine(dir, "FS-Skia-UI.sln")) then
            dir
        else
            match Directory.GetParent dir |> Option.ofObj with
            | Some parent -> find parent.FullName
            | None -> dir

    find __SOURCE_DIRECTORY__

type Msg = Save

type Model = { Saved: bool }

// A product view authored entirely through the typed front door, terminated with
// `Widget.toControl` so it satisfies the existing `AdapterProgram.View: 'model ->
// Control<'msg>` contract with no adapter edit (FR-009).
let widgetView (model: Model) : Control<Msg> =
    FS.Skia.UI.Controls.Typed.Stack.view
        { FS.Skia.UI.Controls.Typed.Stack.defaults with
            Children =
                [ FS.Skia.UI.Controls.Typed.TextBlock.view
                      { FS.Skia.UI.Controls.Typed.TextBlock.defaults with Text = "Typed" }
                  FS.Skia.UI.Controls.Typed.Button.view
                      { FS.Skia.UI.Controls.Typed.Button.defaults with
                          Id = Some "save"
                          Text = "Save"
                          OnClick = Some Save } ] }
    |> Widget.toControl

[<Tests>]
let typedAdapterTests =
    testList "Typed controls Elmish boundary and dependency guard" [
        test "Widget.toControl-terminated view runs through AdapterProgram unchanged" {
            let init () = { Saved = false }, []
            let update msg model =
                match msg with
                | Save -> { model with Saved = true }, []

            let program = ControlsElmish.program init update widgetView (fun _ -> [])

            let model, initCommands = program.Init()
            let updated, _ = program.Update Save model
            let control = program.View updated
            let rendered = Control.render Theme.light control

            Expect.isTrue updated.Saved "adapter update ran"
            Expect.isEmpty initCommands "no startup commands required"
            Expect.isGreaterThan rendered.NodeCount 0 "typed view renders through the adapter"

            let click =
                { Kind = "click"; ControlId = Some "save"; Origin = ControlEventOrigin.Pointer; Payload = None }

            Expect.equal (Control.dispatch click control) [ Save ] "typed event dispatches through the adapter view"
        }

        test "base Controls package gains no Fable.Elmish dependency (FR-011, SC-004)" {
            let controlsProject = Path.Combine(repositoryRoot, "src", "Controls", "Controls.fsproj")
            let text = File.ReadAllText controlsProject

            Expect.isFalse (text.Contains "Fable.Elmish") "Controls.fsproj does not reference Fable.Elmish"
            Expect.isFalse (text.Contains "PackageReference") "Controls.fsproj adds no NuGet package dependency"

            [ @"..\Scene\Scene.fsproj"; @"..\Layout\Layout.fsproj"; @"..\KeyboardInput\KeyboardInput.fsproj" ]
            |> List.iter (fun reference -> Expect.stringContains text reference $"existing reference {reference} retained")
        }
    ]
