module ControlsGallery.Program

open System
open System.Diagnostics
open Elmish
open FS.Skia.UI
open FS.Skia.UI.Controls

type Model =
    { Count: int
      Name: string
      CanSave: bool
      SelectedTab: string
      Items: string list
      Collection: CollectionModel }

type Msg =
    | Increment
    | NameChanged of string
    | ToggleSave
    | TabChanged of string
    | SelectItem of string
    | CollectionMsg of CollectionMsg
    | HostEffect of ViewerEffect<Msg>
    | NoOp

type Effect =
    | RequestHostRender
    | PersistDraft of string

let init () : Model * Cmd<Msg> =
    let collection, _ = Collections.init "orders" 10_000 24.0 240.0

    { Count = 0
      Name = "Ada"
      CanSave = true
      SelectedTab = "Form"
      Items = [ "Orders"; "Invoices"; "Customers" ]
      Collection = collection },
    Cmd.none

let update msg (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Increment -> { model with Count = model.Count + 1 }, Cmd.none
    | NameChanged value -> { model with Name = value }, Cmd.none
    | ToggleSave -> { model with CanSave = not model.CanSave }, Cmd.none
    | TabChanged tab -> { model with SelectedTab = tab }, Cmd.none
    | SelectItem _ -> model, Cmd.none
    | CollectionMsg collectionMsg ->
        let collection, _ = Collections.update collectionMsg model.Collection
        { model with Collection = collection }, Cmd.none
    | HostEffect _
    | NoOp -> model, Cmd.none

let controlView model =
    Stack.create [
        Stack.children [
            TextBlock.create [ TextBlock.text "Controls Gallery" ]
            Tabs.create [
                Tabs.items [ "Form"; "Dashboard"; "Data" ]
                Tabs.selected model.SelectedTab
                Tabs.onChanged TabChanged
            ]
            TextBox.create [
                TextBox.value model.Name
                TextBox.onChanged NameChanged
                TextBox.validation Valid
            ]
            Button.create [
                Button.text $"Save {model.Count}"
                Button.enabled model.CanSave
                Button.onClick Increment
            ]
            CheckBox.create [
                CheckBox.text "Can save"
                CheckBox.checked' model.CanSave
                CheckBox.onChanged (fun _ -> ToggleSave)
            ]
            ProgressBar.create [ ProgressBar.value (float model.Count / 10.0) ]
            LineChart.create [
                LineChart.series [
                    { Name = "count"
                      Points = [ for index in 0 .. 9 -> { X = float index; Y = float (index + model.Count); Label = None } ] }
                ]
            ]
            GraphView.create [ GraphView.nodes [ "form"; "dashboard"; "data" ] ]
            ValidationMessage.create [ ValidationMessage.text $"Visible rows: {model.Collection.VisibleRange.Count}" ]
        ]
    ]

let view model =
    (Control.render Theme.light (controlView model)).Scene

let configuration =
    { Viewer.defaultConfiguration "Controls Gallery" { Width = 800; Height = 600 } with
        ClearColor = Some Theme.light.Background
        TargetFrameRate = Some 60
        Diagnostics = { Verbose = true } }

let program =
    Viewer.create configuration init update view
    |> Viewer.withEventMapping (fun _ -> Some NoOp)
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let runContractSmoke () =
    let model, _ = init ()
    let root = controlView model
    let rendered = Control.render Theme.light root
    let click =
        { Kind = "click"
          ControlId = Some "button"
          Origin = Pointer
          Payload = None }

    printfn "status=ok"
    printfn "sample=ControlsGallery"
    printfn "control-count=%d" rendered.NodeCount
    printfn "catalog-count=%d" (Catalog.supportedCount ())
    printfn "visible-range=%A" model.Collection.VisibleRange
    printfn "diagnostics=%A" rendered.Diagnostics
    printfn "scene-kinds=%A" (Scene.describe rendered.Scene)
    printfn "manual-click-path=%A" (Control.dispatch click root)
    0

let runSmoke () =
    let stopwatch = Stopwatch.StartNew()

    match Viewer.run program with
    | Ok() ->
        stopwatch.Stop()
        printfn "status=ok"
        printfn "sample=ControlsGallery"
        printfn "renderer=Vulkan"
        printfn "first-frame-ms=%d" stopwatch.ElapsedMilliseconds
        0
    | Result.Error diagnostic ->
        stopwatch.Stop()
        printfn "status=error"
        printfn "sample=ControlsGallery"
        printfn "diagnostic-stage=%A" diagnostic.Stage
        printfn "diagnostic-message=%s" diagnostic.Message
        2

[<EntryPoint>]
let main argv =
    if argv |> Array.contains "--contract-smoke" then
        runContractSmoke ()
    else
        runSmoke ()
