module PointerInteractionGallery

// Feature 075 sample — demonstrates hover / click / drag / secondary-click / wheel
// driven entirely through the pointer front door. The application code below
// references ONLY ControlId-level messages (Activate / OpenContext / ScrollBy /
// DragTo) and performs NO coordinate math or hit-testing of its own (SC-007); the
// only host-coupled glue is the `ViewerEvent -> PointerSample` translation.

open System.Diagnostics
open FS.Skia.UI.Scene
open FS.Skia.UI.Layout
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.SkiaViewer.Host

// --- product messages (ControlId-level only, SC-007) ------------------------

type Msg =
    | ViewerInput of ViewerEvent
    | Activate of ControlId
    | OpenContext of ControlId
    | ScrollBy of ControlId * float
    | DragTo of ControlId * float * float
    | NoOp

type Model =
    { Pointer: PointerState
      Hovered: ControlId option
      Activations: int
      Contexts: int
      ScrollOffset: float
      Thumb: (float * float) option
      LastAction: string }

// --- the framework-computed layout (a real LayoutResult; the consumer never
//     hit-tests it directly — Pointer.update does, inside the framework) -------

let private leaf id w h : LayoutNode =
    { Defaults.layoutNode id with
        Intent = { Defaults.layoutIntent with Size = { Width = Some w; Height = Some h } } }

let private gallery: LayoutNode =
    { Defaults.layoutNode "root" with
        Intent = { Defaults.layoutIntent with Direction = Row }
        Children = [ leaf "buttonA" 100.0 40.0; leaf "buttonB" 100.0 40.0; leaf "thumb" 60.0 40.0; leaf "scroll" 140.0 40.0 ] }

let policy: PixelSnapPolicy = Defaults.pixelSnapPolicy 1.0
let layout: LayoutResult = Layout.evaluate (Defaults.availableSpace 420.0 60.0) gallery

// --- host glue: neutral PointerSample from the host ViewerEvent --------------

let private toButton =
    function
    | PrimaryButton -> PointerButton.Primary
    | SecondaryButton -> PointerButton.Secondary
    | MiddleButton -> PointerButton.Middle

let toSample event : PointerSample option =
    match event with
    | PointerMoved(x, y) -> Some { Phase = PointerPhase.Moved; X = x; Y = y; Button = None; DeltaX = 0.0; DeltaY = 0.0 }
    | PointerPressed(x, y, b) -> Some { Phase = PointerPhase.Pressed; X = x; Y = y; Button = Some(toButton b); DeltaX = 0.0; DeltaY = 0.0 }
    | PointerReleased(x, y, b) -> Some { Phase = PointerPhase.Released; X = x; Y = y; Button = Some(toButton b); DeltaX = 0.0; DeltaY = 0.0 }
    | PointerScrolled(x, y, dx, dy) -> Some { Phase = PointerPhase.Wheel; X = x; Y = y; Button = None; DeltaX = dx; DeltaY = dy }
    | PointerExited -> Some { Phase = PointerPhase.Exited; X = 0.0; Y = 0.0; Button = None; DeltaX = 0.0; DeltaY = 0.0 }
    | _ -> None

// --- the consumer router: meaningful interactions -> product messages --------

let routeInteraction: PointerInteraction -> Msg option =
    function
    | Click(id, PointerButton.Primary, _, _) -> Some(Activate id)
    | Click(id, PointerButton.Secondary, _, _) -> Some(OpenContext id)
    | Scroll(id, _, dy, _, _) -> Some(ScrollBy(id, dy))
    | DragMove(id, _, x, y) -> Some(DragTo(id, x, y))
    | _ -> None

// --- MVU (pure update; effects lowered via the Elmish bridge) ----------------

let init () : Model * AdapterCommand<Msg> =
    { Pointer = Pointer.init ()
      Hovered = None
      Activations = 0
      Contexts = 0
      ScrollOffset = 0.0
      Thumb = None
      LastAction = "none" },
    []

let update (msg: Msg) (model: Model) : Model * AdapterCommand<Msg> =
    match msg with
    | ViewerInput event ->
        match toSample event |> Option.bind Pointer.toMsg with
        | Some pmsg ->
            let pointer, interactions, runtimeMsgs = Pointer.update policy layout pmsg model.Pointer

            let hovered =
                interactions
                |> List.fold
                    (fun current ->
                        function
                        | HoverEnter(c, _, _) -> Some c
                        | HoverLeave _ -> None
                        | _ -> current)
                    model.Hovered

            { model with Pointer = pointer; Hovered = hovered },
            ControlsElmish.interpretPointerOutcome routeInteraction interactions runtimeMsgs
        | None -> model, []
    | Activate id -> { model with Activations = model.Activations + 1; LastAction = $"activate:{id}" }, []
    | OpenContext id -> { model with Contexts = model.Contexts + 1; LastAction = $"context:{id}" }, []
    | ScrollBy(id, dy) -> { model with ScrollOffset = model.ScrollOffset + dy; LastAction = $"scroll:{id}:{dy}" }, []
    | DragTo(id, x, y) -> { model with Thumb = Some(x, y); LastAction = $"drag:{id}" }, []
    | NoOp -> model, []

let view (model: Model) : Control<Msg> =
    let hovered = model.Hovered |> Option.defaultValue "none"

    Stack.create
        [ Stack.children
              [ TextBlock.create [ TextBlock.text "Pointer Interaction Gallery" ]
                TextBlock.create [ TextBlock.text $"hovered: {hovered}" ]
                TextBlock.create [ TextBlock.text $"activations: {model.Activations}" ]
                TextBlock.create [ TextBlock.text $"contexts: {model.Contexts}" ]
                TextBlock.create [ TextBlock.text $"scroll-offset: {model.ScrollOffset}" ]
                TextBlock.create [ TextBlock.text $"last-action: {model.LastAction}" ] ] ]

let adapterProgram = ControlsElmish.program init update view (fun _ -> [])

// --- deterministic smoke (the authoritative evidence; the GUI screenshot is
//     render-only per evidence-mode honesty rules) ---------------------------

/// Apply a host event and drain the one level of routed product messages it
/// produces (product handlers emit no further commands).
let private dispatch (model: Model) (event: ViewerEvent) : Model =
    let next, command = update (ViewerInput event) model
    AdapterCmd.productMessages command |> List.fold (fun m pm -> fst (update pm m)) next

let private centerOf id =
    let b = layout.Bounds |> List.find (fun item -> item.NodeId = id) |> fun item -> item.Bounds
    b.X + b.Width / 2.0, b.Y + b.Height / 2.0

let runContractSmoke () =
    let stopwatch = Stopwatch.StartNew()
    let model, _ = adapterProgram.Init()

    let ax, ay = centerOf "buttonA"
    let bx, by = centerOf "buttonB"
    let tx, ty = centerOf "thumb"
    let sx, sy = centerOf "scroll"

    let model =
        model
        // US1 hover: A then B
        |> fun m -> dispatch m (PointerMoved(ax, ay))
        |> fun m -> dispatch m (PointerMoved(bx, by))
        // US2 click: primary press+release on A
        |> fun m -> dispatch m (PointerPressed(ax, ay, PrimaryButton))
        |> fun m -> dispatch m (PointerReleased(ax, ay, PrimaryButton))
        // US4 secondary: right-click on B
        |> fun m -> dispatch m (PointerPressed(bx, by, SecondaryButton))
        |> fun m -> dispatch m (PointerReleased(bx, by, SecondaryButton))
        // US3 drag: press thumb, cross the threshold (DragBegin), drag (DragMove), release (DragEnd)
        |> fun m -> dispatch m (PointerPressed(tx, ty, PrimaryButton))
        |> fun m -> dispatch m (PointerMoved(tx + 12.0, ty))
        |> fun m -> dispatch m (PointerMoved(tx + 24.0, ty))
        |> fun m -> dispatch m (PointerReleased(tx + 24.0, ty, PrimaryButton))
        // US5 wheel over the scroll region
        |> fun m -> dispatch m (PointerScrolled(sx, sy, 0.0, -3.0))

    let rendered = Control.render Theme.light (adapterProgram.View model)
    stopwatch.Stop()

    printfn "status=ok"
    printfn "sample=PointerInteractionGallery"
    printfn "hovered=%A" model.Hovered
    printfn "activations=%d" model.Activations
    printfn "contexts=%d" model.Contexts
    printfn "scroll-offset=%g" model.ScrollOffset
    printfn "thumb=%A" model.Thumb
    printfn "last-action=%s" model.LastAction
    printfn "scene-kinds=%A" (Scene.describe rendered.Scene)
    printfn "elapsed-ms=%d" stopwatch.ElapsedMilliseconds
    0

[<EntryPoint>]
let main argv =
    if argv |> Array.contains "--contract-smoke" then
        runContractSmoke ()
    else
        runContractSmoke ()
