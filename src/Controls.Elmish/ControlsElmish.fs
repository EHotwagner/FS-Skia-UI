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

    // 092 (FR-004): resolve a click to the stable RetainedId of the control under it, via the
    // retained tree's per-node boxes — replaces the 090 `ControlId` `hitTest |> nearestAuthored`
    // path, which collapses unkeyed same-kind siblings onto one id and disagrees with
    // `nearestAuthored`'s scheme. `None` for a true gap / outside the root.
    let resolveFocus (retained: RetainedRender<'msg>) (x: float) (y: float) : RetainedId option =
        RetainedRender.retainedHitTest x y retained

    // Find a retained node by its stable identity (the focused control's node).
    let rec private tryFindNode (id: RetainedId) (n: RetainedNode<'msg>) : RetainedNode<'msg> option =
        if n.Identity = id then
            Some n
        else
            n.Children |> List.tryPick (tryFindNode id)

    // Read a control's current text value (the first-focus seed, FR-005): the `text`/`value`
    // attribute, else its `Content`, else empty.
    let private controlTextValue (c: Control<'msg>) : string =
        let fromAttr name =
            c.Attributes
            |> List.tryPick (fun a ->
                if a.Name = name then
                    match a.Value with
                    | TextValue v -> Some v
                    | _ -> None
                else
                    None)

        fromAttr "text"
        |> Option.orElseWith (fun () -> fromAttr "value")
        |> Option.orElseWith (fun () -> c.Content)
        |> Option.defaultValue ""

    // Kind-derived line mode (FR-005): a `text-area` is multi-line; every other kind single-line.
    // Fixes the 090 hard-coded-`SingleLine` defect that truncated multi-line fields.
    let private lineModeOf (c: Control<'msg>) : TextInputMode =
        if c.Kind = "text-area" then MultiLine else SingleLine

    /// 092 focus-aware text routing on the RETAINED structure (FR-005/FR-006), replacing the 090
    /// `ControlId`-keyed seam: deliver `msg` to the focused control's `RetainedId`-keyed `TextInput`
    /// state held in `retained.StateByIdentity[id].Text`. On the FIRST keystroke after focus (no
    /// existing `Text` entry) the model is seeded from the control's current value + kind-derived
    /// line mode, so the keystroke APPENDS to the pre-filled value instead of discarding it (fixes
    /// the 090 empty-seed / hard-coded-`SingleLine` defects). Returns the next retained structure
    /// (with the advanced text state, which `step` carries across a positional shift) and ALL of the
    /// focused control's matched `onChanged` product messages — every binding, not just the first
    /// (FR-006). When `focused` is `None` or names no live node, nothing is delivered and the
    /// structure is returned unchanged. Scope: routing seam only — caret/selection/IME-UX/undo and
    /// general focus/tab-traversal are trajectory item E4.
    let routeFocusedText
        (retained: RetainedRender<'msg>)
        (focused: RetainedId option)
        (msg: TextInputMsg)
        : RetainedRender<'msg> * 'msg list =
        match focused with
        | Some id ->
            match tryFindNode id retained.Root with
            | Some node ->
                let priorState = retained.StateByIdentity |> Map.tryFind id

                // The carried draft is authoritative while focused; the model value re-seeds the
                // draft ONLY on the focus-acquisition transition (no existing Text entry), never on
                // an ordinary re-render — so a same-frame model change cannot overwrite typing.
                let model0 =
                    match priorState |> Option.bind (fun s -> s.Text) with
                    | Some existing -> existing
                    | None ->
                        let controlId = node.Control.Key |> Option.defaultValue node.Control.Kind
                        fst (TextInput.init controlId (lineModeOf node.Control) (controlTextValue node.Control))

                let model', _effects = TextInput.update msg model0

                let newState =
                    { (priorState |> Option.defaultValue { Animation = None; Text = None }) with
                        Text = Some model' }

                let retained' =
                    { retained with
                        StateByIdentity = Map.add id newState retained.StateByIdentity }

                // FR-006: dispatch EVERY matched `onChanged` binding on the focused control (the 090
                // path dropped all but the first via `List.tryHead`).
                let productMessages =
                    ControlInternals.eventBindingsOf node.Control
                    |> List.filter (fun binding -> binding.EventKind = "changed")
                    |> List.map (fun binding ->
                        binding.Dispatch
                            { Kind = "changed"
                              ControlId = Some binding.ControlId
                              Origin = ControlEventOrigin.Text
                              Payload = Some model'.DraftText })

                retained', productMessages
            | None -> retained, []
        | None -> retained, []

    // Read a control's current numeric `value` (the slider/numeric step base), defaulting to the
    // renderer's own default (sliderGeom uses 0.5) when absent.
    let private controlFloatValue (c: Control<'msg>) (deflt: float) : float =
        c.Attributes
        |> List.tryPick (fun a ->
            if a.Name = "value" then
                match a.Value with
                | FloatValue v -> Some v
                | TextValue t ->
                    match Double.TryParse(t, Globalization.CultureInfo.InvariantCulture) with
                    | true, v -> Some v
                    | _ -> None
                | _ -> None
            else
                None)
        |> Option.defaultValue deflt

    // E4 arrow-key value step for a navigation control (slider/numeric). Right/Up increment,
    // Left/Down decrement, clamped to the normalized [0, 1] slider range.
    let private navStep = 0.1

    let private steppedValue (c: Control<'msg>) (key: string) : float =
        let current = controlFloatValue c 0.5

        let delta =
            match key with
            | "ArrowRight"
            | "ArrowUp" -> navStep
            | "ArrowLeft"
            | "ArrowDown" -> -navStep
            | _ -> 0.0

        Math.Clamp(current + delta, 0.0, 1.0)

    // Normalize a host `ViewerKey` (+ a leading `Shift+` on an `Unknown` raw) to the (keyName, isTab)
    // pair `Focus.route` matches against `Activation`/`NavigationKeys`. A bare/`Shift+`-prefixed "Tab"
    // is the traversal candidate (isTab = true); every other key is a plain name (isTab = false).
    let private normalizeFocusKey (key: ViewerKey) : string * bool =
        match key with
        | Enter -> "Enter", false
        | Space -> "Space", false
        | ArrowLeft -> "ArrowLeft", false
        | ArrowRight -> "ArrowRight", false
        | ArrowUp -> "ArrowUp", false
        | ArrowDown -> "ArrowDown", false
        | ViewerKey.Unknown raw ->
            let bare =
                if raw.StartsWith("Shift+", StringComparison.OrdinalIgnoreCase) then
                    raw.Substring 6
                else
                    raw

            if String.Equals(bare, "Tab", StringComparison.OrdinalIgnoreCase) then
                "Tab", true
            else
                raw, false
        | other -> ViewerKeyboard.toKeyId other, false

    /// E4 (FR-003/FR-006/FR-007): route a delivered key to the current `FocusedControl` over the
    /// RETAINED tree, generalizing the 092 `routeFocusedText` text seam to all interactive kinds.
    /// Resolves the focused control via its stable `RetainedId`, reads its `KeyboardOperation`, and
    /// applies `Focus.route`: `Activate` fires the control's authored activation bindings (the same
    /// message a pointer activation dispatches, once); `Navigate` steps a value control and fires its
    /// `onChanged` bindings; `Traverse` emits a `FocusControl` for `Focus.traverse`; `Fallthrough`
    /// emits nothing (the host then consults `host.MapKey`). The E1 text seam is consulted by the
    /// host BEFORE this, so text delivery is unchanged (SC-003). Total; never throws.
    let routeFocusedKey
        (retained: RetainedRender<'msg>)
        (focused: RetainedId option)
        (order: TabOrder)
        (key: ViewerKey)
        (shift: bool)
        : RetainedRender<'msg> * ControlRuntimeMsg list * 'msg list =
        match focused with
        | None -> retained, [], []
        | Some id ->
            match tryFindNode id retained.Root with
            | None -> retained, [], []
            | Some node ->
                let nodeId = node.Control.Key |> Option.defaultValue node.Control.Kind

                let keyboard =
                    node.Control.Accessibility
                    |> Option.map (fun m -> m.Keyboard)
                    |> Option.defaultValue
                        { Focusable = false
                          ActivationKeys = []
                          NavigationKeys = [] }

                let keyName, isTab = normalizeFocusKey key

                // The focused control's OWN authored bindings (a focusable composite is a single
                // stop, so descendant bindings are excluded by the id filter).
                let ownBindings =
                    ControlInternals.eventBindingsOf node.Control
                    |> List.filter (fun b -> b.ControlId = nodeId)

                match Focus.route keyboard keyName isTab shift with
                | Activate ->
                    // The pointer-equivalent activation message(s) — the same click-equivalent
                    // bindings the pointer path dispatches — fired ONCE each (no double-dispatch).
                    let messages =
                        ownBindings
                        |> List.filter (fun b -> List.contains b.EventKind clickEquivalentKinds)
                        |> List.map (fun b ->
                            b.Dispatch
                                { Kind = b.EventKind
                                  ControlId = Some nodeId
                                  Origin = ControlEventOrigin.Keyboard
                                  Payload = None })

                    retained, [], messages
                | Navigate ->
                    // A value control's arrow step: dispatch its `onChanged` bindings with the new
                    // value, mirroring the pointer-driven value change.
                    let payload =
                        (steppedValue node.Control keyName)
                            .ToString(Globalization.CultureInfo.InvariantCulture)

                    let messages =
                        ownBindings
                        |> List.filter (fun b -> b.EventKind = "changed")
                        |> List.map (fun b ->
                            b.Dispatch
                                { Kind = "changed"
                                  ControlId = Some nodeId
                                  Origin = ControlEventOrigin.Keyboard
                                  Payload = Some payload })

                    retained, [], messages
                | Traverse move ->
                    let next = Focus.traverse order (Some nodeId) move
                    retained, [ FocusControl next ], []
                | Fallthrough -> retained, [], []

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
        // Feature 092 (E2): focus is now keyed by the STABLE `RetainedId` (was `ControlId`), and the
        // focused control's `TextInput` state lives in `RetainedRender.StateByIdentity[id].Text` — no
        // parallel `ControlId`-keyed text-model map. Because `step` carries `StateByIdentity` to the
        // matched identity across the diff, focus + in-progress text + the per-control animation clock
        // survive an unrelated re-render even when the control's position shifts (FR-001/2/3). This is
        // the half 091 left unwired: 091 carried the map but the host never read/wrote it.
        // Feature 094 (E4): generalized from the 092 text-only `focusedText` to the host's single
        // focus identity (still a stable `RetainedId`). The E1 text seam, `routeFocusedKey`
        // activation/navigation, and Tab-traversal all read/write this one ref.
        let focused = ref (None: RetainedId option)
        // The retained render structure (the wired keyed reconciler, 067), the single home of
        // per-control UI state. Mutation is confined to this closure at the interpreter edge
        // (constitution III); the consumer `view` stays pure.
        let retained = ref (None: RetainedRender<'msg> option)
        // Diff/first-frame diagnostics (e.g. KeyCollision from duplicate sibling keys) surfaced
        // through the host's diagnostics stderr channel — never silently dropped; de-duped so a
        // standing collision is reported once, not every frame. The path stays total in their presence.
        let surfacedDiagnostics = ref (Set.empty: Set<string>)

        let surface (diags: ControlDiagnostic list) =
            for d in diags do
                let key = sprintf "%A|%A|%s" d.Code d.ControlId d.Message

                if not (Set.contains key surfacedDiagnostics.Value) then
                    surfacedDiagnostics.Value <- Set.add key surfacedDiagnostics.Value
                    eprintfn "[ControlDiagnostic %A] %s" d.Severity d.Message

        // Produce the production scene for (size, model) through the retained reconciler. The first
        // frame seeds the retained structure and paints ONCE (FR-009 — no second `Control.renderTree`,
        // first-frame collisions surfaced immediately); later frames diff + reuse. Output is
        // byte-identical to a full rebuild (FR-005, proven by the wired round-trip property suite).
        let renderRetained (size: Size) (model: 'model) : Scene =
            let next = host.View size model

            match retained.Value with
            | None ->
                let r0 = RetainedRender.init host.Theme size next
                surface r0.Diagnostics
                retained.Value <- Some r0.Retained
                r0.Render.Scene
            | Some prev ->
                let s = RetainedRender.step host.Theme size prev next
                surface s.Diagnostics
                retained.Value <- Some s.Retained
                s.Render.Scene

        // A focused node is a TEXT control (the E1 seam owns its printable keys); every other
        // focusable kind routes through `routeFocusedKey`.
        let isTextNode (node: RetainedNode<'msg>) : bool =
            (match node.Control.Accessibility with
             | Some m -> m.Role = AccessibilityRole.TextBox
             | None -> false)
            || List.contains node.Control.Kind [ "text-box"; "text-area"; "numeric-input" ]

        // FR-006: a press sets focus to the focusable control under it (its accessibility metadata
        // declares `Focusable = true`). Resolve a `FocusControl next` ControlId back to a stable
        // `RetainedId` so traversal keeps tracking the moved focus across frames.
        let retainedIdOfControl (r: RetainedRender<'msg>) (controlId: ControlId) : RetainedId option =
            let rec find (n: RetainedNode<'msg>) =
                let nId = n.Control.Key |> Option.defaultValue n.Control.Kind

                if nId = controlId then
                    Some n.Identity
                else
                    n.Children |> List.tryPick find

            find r.Root

        let mapPointer (input: ViewerPointerInput) (size: Size) (model: 'model) : 'msg list =
            // Focus-on-click (FR-004/FR-006): a press resolves to the `RetainedId` under the point via
            // the retained tree's per-node boxes (distinguishing unkeyed same-kind siblings); if that
            // control is FOCUSABLE (per its accessibility metadata) it becomes the focus target, so a
            // later key reaches it through the text seam or `routeFocusedKey`. A press on a
            // non-focusable region leaves the current focus UNCHANGED (it is not silently cleared).
            (match input.Phase, retained.Value with
             | ViewerPointerPhaseKind.Pressed, Some r ->
                 match resolveFocus r input.X input.Y with
                 | Some id ->
                     match tryFindNode id r.Root with
                     | Some node when
                         node.Control.Accessibility
                         |> Option.exists (fun m -> m.Keyboard.Focusable)
                         -> focused.Value <- Some id
                     | _ -> ()
                 | None -> ()
             | _ -> ())

            let state', messages = routeInteractivePointer host pointerState.Value size model input
            pointerState.Value <- state'
            messages

        let mapKey (key: ViewerKey) (pressed: bool) : 'msg list =
            // Feature 094 (E4) focus-first key routing. Only key-down (`pressed`) is routed; key-up
            // falls straight through. Precedence (R3): (1) E1 text seam for a focused TEXT control's
            // printable keys, (2) `routeFocusedKey` for activation / navigation / Tab-traversal,
            // (3) `host.MapKey` for anything no focused control and no traversal consumed.
            if not pressed then
                host.MapKey key false |> Option.toList
            else
                match retained.Value with
                | None -> host.MapKey key true |> Option.toList
                | Some r ->
                    let focusedNode = focused.Value |> Option.bind (fun id -> tryFindNode id r.Root)

                    // (1) E1 text seam — unchanged delivery for a focused text control's printable keys.
                    let textHandled =
                        match textMsgOfKey key, focused.Value, focusedNode with
                        | Some textMsg, Some id, Some node when isTextNode node ->
                            let r', msgs = routeFocusedText r (Some id) textMsg
                            retained.Value <- Some r'
                            Some msgs
                        | _ -> None

                    match textHandled with
                    | Some msgs -> msgs
                    | None ->
                        // (2) routeFocusedKey — the tab order is derived from the retained tree's
                        // root control (the lowered view), so no model/size is needed here.
                        let order = Focus.order r.Root.Control

                        let shift =
                            match key with
                            | ViewerKey.Unknown raw -> raw.StartsWith("Shift+", StringComparison.OrdinalIgnoreCase)
                            | _ -> false

                        let r', controlMsgs, productMsgs = routeFocusedKey r focused.Value order key shift
                        retained.Value <- Some r'

                        // Apply focus-update messages to the host's focus identity (map the next
                        // ControlId back to its stable RetainedId).
                        for cm in controlMsgs do
                            match cm with
                            | FocusControl next ->
                                focused.Value <- next |> Option.bind (retainedIdOfControl r')
                            | _ -> ()

                        // (3) Fall through to host.MapKey only when nothing was consumed.
                        match productMsgs, controlMsgs with
                        | [], [] -> host.MapKey key true |> Option.toList
                        | _ -> productMsgs

        let viewerHost: InteractiveViewerHost<'model, 'msg> =
            { Init = host.Init
              Update = host.Update
              View = fun size model -> SceneNode.Group [ renderRetained size model ]
              MapKey = mapKey
              MapPointer = mapPointer
              Tick = host.Tick
              Diagnostics = host.Diagnostics }

        Viewer.runInteractiveViewer options viewerHost
