namespace FS.Skia.UI.Controls.Elmish

open System
open FS.Skia.UI.Controls
open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer
open Elmish

type AdapterDiagnostic =
    { Code: string
      Message: string
      Source: string }

type AdapterEffect<'msg> =
    | DispatchProductMessage of 'msg
    | DispatchControlRuntimeMessage of ControlRuntimeMsg
    | DispatchKeyboardMessage of KeyboardMsg
    | DispatchHostCommand of string
    | ReportAdapterDiagnostic of AdapterDiagnostic

type AdapterCommand<'msg> = AdapterEffect<'msg> list

type AdapterSubscription<'msg> =
    { Id: string
      Subscribe: unit -> AdapterCommand<'msg> }

type AdapterProgram<'model, 'msg> =
    { Init: unit -> 'model * AdapterCommand<'msg>
      Update: 'msg -> 'model -> 'model * AdapterCommand<'msg>
      View: 'model -> Control<'msg>
      Subscriptions: 'model -> AdapterSubscription<'msg> list }

type InteractiveAppHost<'model, 'msg> =
    { Init: unit -> 'model * ViewerEffect list
      Update: 'msg -> 'model -> 'model * ViewerEffect list
      View: Size -> 'model -> Control<'msg>
      Theme: Theme
      MapKey: ViewerKey -> bool -> 'msg option
      MapPointer: PointerInteraction -> 'msg option
      Tick: TimeSpan -> 'msg option
      Diagnostics: ViewerDiagnosticsOptions }

/// Verdict of a responds-proof (feature 090, FR-006): `Responsive` when a real input applied to the
/// running host produced a visible change in the rendered output (`before` ≠ `after`), `Inert` when
/// it did not. An inert host (renders but does not respond) can only yield `Inert`.
type RespondsVerdict =
    | Responsive
    | Inert

/// A captured input→visible-change responds-proof (feature 090, FR-006/FR-007): the `Before` frame,
/// the `After` frame produced by applying a real dispatched interaction (route → `host.Update` fold →
/// re-render, exactly as the live repaint loop), and the `Verdict`. A distinct evidence class from a
/// render-only screenshot (one frame, no interaction) and from the offscreen `runInteractivePointerOnce`
/// route probe (model layer only): an app that renders but does not respond yields identical frames and
/// an `Inert` verdict, so "renders" cannot be passed off as "responds".
type RespondsProof =
    { Before: Scene
      After: Scene
      Verdict: RespondsVerdict }

module AdapterCmd =
    let none: Cmd<'msg> = Cmd.none

    let ofMessage (msg: 'msg) : AdapterCommand<'msg> = [ DispatchProductMessage msg ]

    let productMessages (command: AdapterCommand<'msg>) : 'msg list =
        command
        |> List.choose (function
            | DispatchProductMessage msg -> Some msg
            | _ -> None)

    let toCmd (route: AdapterEffect<'msg> -> 'msg) (command: AdapterCommand<'msg>) : Cmd<'msg> =
        command
        |> List.map (fun effect -> (fun (dispatch: Dispatch<'msg>) -> dispatch (route effect)))

module ControlsElmish =
    let diagnostic source code message =
        { Source = source
          Code = code
          Message = message }

    let interpretKeyboardEffect mapCommand effect =
        match effect with
        | CommandResolved command -> [ DispatchProductMessage(mapCommand command) ]
        | KeyStateChanged _
        | LayoutChanged _
        | ModeChanged _
        | PendingSequenceChanged _
        | StateDisplayChanged _ -> []
        | RequestHostKeyCapture key -> [ DispatchHostCommand $"capture-key:{key}" ]
        | ReportKeyboardDiagnostic keyboardDiagnostic ->
            [ ReportAdapterDiagnostic(diagnostic "keyboard-input" keyboardDiagnostic.Code keyboardDiagnostic.Message) ]

    let interpretControlEffect mapRuntime effect =
        match effect with
        | FocusChanged controlId ->
            [ DispatchControlRuntimeMessage(FocusControl controlId)
              DispatchProductMessage(mapRuntime (FocusControl controlId)) ]
        | HoverChanged controlId ->
            [ DispatchControlRuntimeMessage(HoverControl controlId)
              DispatchProductMessage(mapRuntime (HoverControl controlId)) ]
        | PressedControlsChanged _
        | CaretChanged _
        | SelectionChanged _
        | CompositionChanged _
        | DragChanged _
        | CancelledInteraction _ -> []
        | StaleTarget controlId ->
            [ ReportAdapterDiagnostic(diagnostic "control-runtime" "StaleTarget" $"Stale control target '{controlId}' was ignored by the Controls adapter.") ]
        | ReportControlRuntimeDiagnostic controlDiagnostic ->
            [ ReportAdapterDiagnostic(diagnostic "control-runtime" (string controlDiagnostic.Code) controlDiagnostic.Message) ]

    let interpretPointerEffect (mapInteraction: PointerInteraction -> 'msg option) (interaction: PointerInteraction) =
        match interaction with
        | Diagnostic pointerDiagnostic ->
            [ ReportAdapterDiagnostic(diagnostic "pointer" (string pointerDiagnostic.Code) pointerDiagnostic.Message) ]
        | meaningful ->
            match mapInteraction meaningful with
            | Some msg -> [ DispatchProductMessage msg ]
            | None -> []

    let interpretPointerOutcome
        (mapInteraction: PointerInteraction -> 'msg option)
        (interactions: PointerInteraction list)
        (runtimeMessages: ControlRuntimeMsg list)
        =
        (runtimeMessages |> List.map DispatchControlRuntimeMessage)
        @ (interactions |> List.collect (interpretPointerEffect mapInteraction))

    let subscriptions (keyboard: AdapterSubscription<'msg> list) (controls: AdapterSubscription<'msg> list) =
        keyboard @ controls

    let program init update view subscriptions =
        { Init = init
          Update = update
          View = view
          Subscriptions = subscriptions }

    let widgetView (view: 'model -> Widget<'msg>) : 'model -> Control<'msg> =
        view >> Widget.toControl

    let programOfWidget init update view subscriptions =
        program init update (widgetView view) subscriptions

    // FR-001/FR-003 (feature 090): a pointer Click is binding-eligible for an authored control's
    // click-equivalent bindings (`onClick`→"click", a click-driven toggle `onChanged`→"changed", a
    // click-driven `onSelected`→"selected"). Other interactions (hover/drag/scroll) are not
    // binding-eligible here and go straight to `MapPointer`.
    let clickEquivalentKinds = [ "click"; "changed"; "selected" ]

    // Resolve the authored bindings (if any) a single interaction should dispatch. `Some msgs` means
    // an authored binding consumed the interaction (MapPointer is NOT consulted for it); `None` means
    // no authored binding matched, so the host falls back to `MapPointer` with the raw interaction.
    let bindingMessagesFor (rendered: ControlRenderResult<'msg>) (interaction: PointerInteraction) : 'msg list option =
        match interaction with
        | Click(control, _, _, _) ->
            match Control.nearestAuthored rendered control with
            | Some authored ->
                let matched =
                    rendered.EventBindings
                    |> List.filter (fun binding ->
                        binding.ControlId = authored
                        && List.contains binding.EventKind clickEquivalentKinds)

                match matched with
                | [] -> None
                | bindings ->
                    bindings
                    |> List.map (fun binding ->
                        binding.Dispatch
                            { Kind = binding.EventKind
                              ControlId = Some authored
                              Origin = ControlEventOrigin.Pointer
                              Payload = None })
                    |> Some
            | None -> None
        | _ -> None

    // The single pointer-routing step the interactive host performs per native sample: render the
    // current Control tree at the live extent, hit-test the laid-out bounds via the shipped 075
    // pipeline (Pointer.update over the LayoutResult, incl. the 4px click/drag fold), then route the
    // emitted interactions through interpretPointerOutcome host.MapPointer to product messages.
    // Returns the advanced PointerState (threaded across samples) + the product messages. Exposed so
    // a headless test exercises the EXACT routing runInteractiveApp wires (research D6 honest bar).
    let routeInteractivePointer
        (host: InteractiveAppHost<'model, 'msg>)
        (state: PointerState)
        (size: Size)
        (model: 'model)
        (input: ViewerPointerInput)
        : PointerState * 'msg list =
        let toSample (input: ViewerPointerInput) : PointerSample =
            let phase =
                match input.Phase with
                | ViewerPointerPhaseKind.Moved -> PointerPhase.Moved
                | ViewerPointerPhaseKind.Pressed -> PointerPhase.Pressed
                | ViewerPointerPhaseKind.Released -> PointerPhase.Released
                | ViewerPointerPhaseKind.Wheel -> PointerPhase.Wheel
                | ViewerPointerPhaseKind.Exited -> PointerPhase.Exited

            let button =
                input.Button
                |> Option.map (fun b ->
                    match b with
                    | ViewerPointerButtonKind.Primary -> PointerButton.Primary
                    | ViewerPointerButtonKind.Secondary -> PointerButton.Secondary
                    | ViewerPointerButtonKind.Middle -> PointerButton.Middle)

            { Phase = phase
              X = input.X
              Y = input.Y
              Button = button
              DeltaX = input.DeltaX
              DeltaY = input.DeltaY }

        match Pointer.toMsg (toSample input) with
        | None -> state, []
        | Some pointerMsg ->
            let rendered = Control.renderTree host.Theme size (host.View size model)

            let available: FS.Skia.UI.Layout.AvailableSpace =
                { Width = float size.Width
                  WidthMode = FS.Skia.UI.Layout.Exactly
                  Height = float size.Height
                  HeightMode = FS.Skia.UI.Layout.Exactly }

            let layoutResult = FS.Skia.UI.Layout.Layout.evaluate available rendered.Layout
            let policy = FS.Skia.UI.Layout.Defaults.pixelSnapPolicy 1.0

            let state', interactions, _runtimeMessages =
                Pointer.update policy layoutResult pointerMsg state

            // FR-001/FR-003 (feature 090): authored EventBindings win; MapPointer is the fallback.
            // For each interaction the host (1) recovers the authored control id via
            // `Control.nearestAuthored` (so a hit on an inner positional node inside a container-keyed
            // composite resolves to the authored container id), (2) looks up `rendered.EventBindings`
            // for a binding on that id whose `EventKind` is click-equivalent, and (3) dispatches the
            // bound message — WITHOUT also offering the interaction to `MapPointer` (no double-advance).
            // An interaction with no consuming binding (no match, or recovery `None`) falls back to
            // `MapPointer` with the raw interaction exactly as before, so existing `MapPointer`-only
            // consumers are bit-for-bit unchanged (additive). Interaction order is preserved.
            let messages =
                interactions
                |> List.collect (fun interaction ->
                    match bindingMessagesFor rendered interaction with
                    | Some msgs -> msgs
                    | None ->
                        interpretPointerEffect host.MapPointer interaction
                        |> AdapterCmd.productMessages)

            state', messages

    /// Focus-aware text-routing seam (feature 090, FR-008): deliver a `TextInputMsg` (a keystroke /
    /// committed or composed text) to the CURRENTLY FOCUSED text control's existing `TextInput` model,
    /// and fold that control's authored `onChanged` binding (if any) into product messages — so
    /// TextBox/TextArea/NumericInput are typeable. Only the focused control's model advances (`models`,
    /// the caller-held map keyed by `ControlId`, holds one `TextInputModel` per text control); an
    /// unfocused control's model is returned unchanged. Reuses `ControlRuntime.FocusedControl` +
    /// `TextInput.update` — no parallel text model (FR-008). When `focused` is `None` or names no
    /// model, nothing is delivered (the host's unchanged `MapKey` path handles the key) and the models
    /// are returned unchanged. Scope: routing seam only — caret/selection/IME-UX/undo and general
    /// focus/tab-traversal across all control kinds are trajectory item E4 (FR-008a).
    let routeFocusedText
        (rendered: ControlRenderResult<'msg>)
        (focused: ControlId option)
        (models: Map<ControlId, TextInputModel>)
        (msg: TextInputMsg)
        : Map<ControlId, TextInputModel> * 'msg list =
        match focused with
        | Some id when Map.containsKey id models ->
            let model', _effects = TextInput.update msg models.[id]
            let models' = Map.add id model' models

            let text =
                if String.IsNullOrEmpty model'.CommittedText then
                    model'.DraftText
                else
                    model'.CommittedText

            let productMessages =
                rendered.EventBindings
                |> List.filter (fun binding -> binding.ControlId = id && binding.EventKind = "changed")
                |> List.map (fun binding ->
                    binding.Dispatch
                        { Kind = "changed"
                          ControlId = Some id
                          Origin = ControlEventOrigin.Text
                          Payload = Some text })

            models', productMessages
        | _ -> models, []

    /// Build a responds-proof verdict from a before/after frame pair (feature 090, FR-006):
    /// `Responsive` when the frames differ (a real input produced a visible change), `Inert` when
    /// identical. The reusable core the pointer and text responds-proof captures share.
    let respondsProofOf (before: Scene) (after: Scene) : RespondsProof =
        { Before = before
          After = after
          Verdict = (if before <> after then Responsive else Inert) }

    /// Capture an input→visible-change responds-proof for a pointer interaction on the running host
    /// (feature 090, FR-006/FR-007): render the BEFORE frame, route the interaction through the real
    /// `routeInteractivePointer` adapter path, fold the produced messages with `host.Update`, render
    /// the AFTER frame, and emit both frames + a verdict. A host whose live window is inert (an
    /// authored binding dropped, so the route produces no message) yields identical frames and an
    /// `Inert` verdict — it cannot be passed off as a responds-proof. Reuses the production render
    /// path (`Control.renderTree`); no live Vulkan window required (render-only capture).
    let captureRespondsProof
        (host: InteractiveAppHost<'model, 'msg>)
        (state: PointerState)
        (size: Size)
        (model: 'model)
        (input: ViewerPointerInput)
        : RespondsProof =
        let before = (Control.renderTree host.Theme size (host.View size model)).Scene
        let _, messages = routeInteractivePointer host state size model input
        let model' = messages |> List.fold (fun current msg -> fst (host.Update msg current)) model
        let after = (Control.renderTree host.Theme size (host.View size model')).Scene
        respondsProofOf before after

    // Map a native key (key-down) to the text-seam message it inserts. Only printable keys produce
    // text; editing keys (Backspace, arrows, …) are E4 scope (FR-008a) and fall through to MapKey.
    let textMsgOfKey (key: ViewerKey) : TextInputMsg option =
        match key with
        | Letter c -> Some(InsertText(string c))
        | Digit n -> Some(InsertText(string n))
        | Space -> Some(InsertText " ")
        | _ -> None

    let runInteractiveApp (options: ViewerOptions) (host: InteractiveAppHost<'model, 'msg>) =
        // Durable pointer coordination state (hover/press/4px-fold), threaded across samples.
        let pointerState = ref (Pointer.init ())
        // Focus-aware text-seam state (feature 090, FR-008): the focused text control and one
        // TextInput model per text control the user has focused, threaded across native events. The
        // latest (size, model) seen by the pointer path lets the stateless `MapKey` seam re-render the
        // tree to resolve the focused control's bindings.
        let focusedText = ref (None: ControlId option)
        let textModels = ref (Map.empty: Map<ControlId, TextInputModel>)
        let latest = ref (None: (Size * 'model) option)

        let mapPointer (input: ViewerPointerInput) (size: Size) (model: 'model) : 'msg list =
            latest.Value <- Some(size, model)

            // Focus-on-click (FR-008/T3): a press over an authored control that exposes an `onChanged`
            // binding focuses it, so a subsequent keystroke reaches it through the text seam.
            (match input.Phase with
             | ViewerPointerPhaseKind.Pressed ->
                 let rendered = Control.renderTree host.Theme size (host.View size model)

                 match Control.hitTest rendered input.X input.Y |> Option.bind (Control.nearestAuthored rendered) with
                 | Some authored when
                     rendered.EventBindings
                     |> List.exists (fun b -> b.ControlId = authored && b.EventKind = "changed")
                     ->
                     focusedText.Value <- Some authored

                     if not (Map.containsKey authored textModels.Value) then
                         let initial, _ = TextInput.init authored SingleLine ""
                         textModels.Value <- Map.add authored initial textModels.Value
                 | _ -> ()
             | _ -> ())

            let state', messages = routeInteractivePointer host pointerState.Value size model input
            pointerState.Value <- state'
            messages

        let mapKey (key: ViewerKey) (pressed: bool) : 'msg option =
            // Focus-aware text delivery (FR-008): when a text control is focused, deliver the keystroke
            // to its `TextInput` model and fold its `onChanged` binding; otherwise the host's `MapKey`
            // path is unchanged. Only key-down (`pressed`) keystrokes produce text.
            let delivered =
                if pressed then
                    match textMsgOfKey key, focusedText.Value, latest.Value with
                    | Some textMsg, Some focused, Some(size, model) ->
                        let rendered = Control.renderTree host.Theme size (host.View size model)
                        let models', msgs = routeFocusedText rendered (Some focused) textModels.Value textMsg
                        textModels.Value <- models'
                        List.tryHead msgs
                    | _ -> None
                else
                    None

            match delivered with
            | Some _ -> delivered
            | None -> host.MapKey key pressed

        let viewerHost: InteractiveViewerHost<'model, 'msg> =
            { Init = host.Init
              Update = host.Update
              View = fun size model -> SceneNode.Group [ (Control.renderTree host.Theme size (host.View size model)).Scene ]
              MapKey = mapKey
              MapPointer = mapPointer
              Tick = host.Tick
              Diagnostics = host.Diagnostics }

        Viewer.runInteractiveViewer options viewerHost
