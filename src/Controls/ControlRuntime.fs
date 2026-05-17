namespace FS.Skia.UI.Controls

type ControlCaret =
    { ControlId: ControlId
      Index: int }

type ControlSelection =
    { ControlId: ControlId
      Start: int
      End: int }

type ControlComposition =
    { ControlId: ControlId
      Text: string }

type ControlDrag =
    { ControlId: ControlId
      StartX: float
      StartY: float
      CurrentX: float
      CurrentY: float }

type ControlRuntimeEffect =
    | FocusChanged of ControlId option
    | HoverChanged of ControlId option
    | PressedControlsChanged of ControlId list
    | CaretChanged of ControlCaret option
    | SelectionChanged of ControlSelection option
    | CompositionChanged of ControlComposition option
    | DragChanged of ControlDrag option
    | StaleTarget of ControlId
    | CancelledInteraction of ControlId option
    | ReportControlRuntimeDiagnostic of ControlDiagnostic

type ControlRuntimeModel =
    { FocusedControl: ControlId option
      HoveredControl: ControlId option
      PressedControls: Set<ControlId>
      Caret: ControlCaret option
      Selection: ControlSelection option
      Composition: ControlComposition option
      ActiveDrag: ControlDrag option
      Diagnostics: ControlDiagnostic list
      RecentEffects: ControlRuntimeEffect list }

type ControlRuntimeMsg =
    | FocusControl of ControlId option
    | HoverControl of ControlId option
    | PressControl of ControlId
    | ReleaseControl of ControlId
    | SetCaret of ControlCaret option
    | SetSelection of ControlSelection option
    | StartComposition of ControlId * string
    | CommitComposition of ControlId
    | StartDrag of ControlId * float * float
    | MoveDrag of float * float
    | EndDrag
    | FocusLost
    | RemoveControl of ControlId
    | RecoverStaleTarget of ControlId
    | CancelInteraction of ControlId option
    | Reset

module ControlRuntime =
    let empty =
        { FocusedControl = None
          HoveredControl = None
          PressedControls = Set.empty
          Caret = None
          Selection = None
          Composition = None
          ActiveDrag = None
          Diagnostics = []
          RecentEffects = [] }

    let init () =
        empty, ([]: ControlRuntimeEffect list)

    let withEffects effects model =
        { model with RecentEffects = effects }, effects

    let staleDiagnostic controlId =
        Diagnostics.create
            (Some controlId)
            "control-runtime"
            StaleGeneratedReference
            ControlDiagnosticSeverity.Warning
            $"Stale interaction target '{controlId}' was recovered by ControlRuntime."

    let cancelledDiagnostic controlId =
        Diagnostics.create
            controlId
            "control-runtime"
            HitTestFailed
            ControlDiagnosticSeverity.Info
            "Control interaction was cancelled before completion."

    let clearTarget controlId model =
        { model with
            FocusedControl = model.FocusedControl |> Option.filter ((<>) controlId)
            HoveredControl = model.HoveredControl |> Option.filter ((<>) controlId)
            PressedControls = model.PressedControls.Remove controlId
            Caret = model.Caret |> Option.filter (fun caret -> caret.ControlId <> controlId)
            Selection = model.Selection |> Option.filter (fun selection -> selection.ControlId <> controlId)
            Composition = model.Composition |> Option.filter (fun composition -> composition.ControlId <> controlId)
            ActiveDrag = model.ActiveDrag |> Option.filter (fun drag -> drag.ControlId <> controlId) }

    let update msg model =
        match msg with
        | FocusControl controlId ->
            { model with FocusedControl = controlId }
            |> withEffects [ FocusChanged controlId ]
        | HoverControl controlId ->
            { model with HoveredControl = controlId }
            |> withEffects [ HoverChanged controlId ]
        | PressControl controlId ->
            let pressed = model.PressedControls.Add controlId
            { model with PressedControls = pressed }
            |> withEffects [ PressedControlsChanged(Set.toList pressed) ]
        | ReleaseControl controlId ->
            let pressed = model.PressedControls.Remove controlId
            { model with PressedControls = pressed }
            |> withEffects [ PressedControlsChanged(Set.toList pressed) ]
        | SetCaret caret ->
            { model with Caret = caret }
            |> withEffects [ CaretChanged caret ]
        | SetSelection selection ->
            { model with Selection = selection }
            |> withEffects [ SelectionChanged selection ]
        | StartComposition(controlId, text) ->
            let composition = Some { ControlId = controlId; Text = text }
            { model with Composition = composition }
            |> withEffects [ CompositionChanged composition ]
        | CommitComposition controlId ->
            let composition =
                model.Composition
                |> Option.filter (fun current -> current.ControlId <> controlId)

            { model with Composition = composition }
            |> withEffects [ CompositionChanged composition ]
        | StartDrag(controlId, x, y) ->
            let drag =
                Some
                    { ControlId = controlId
                      StartX = x
                      StartY = y
                      CurrentX = x
                      CurrentY = y }

            { model with ActiveDrag = drag }
            |> withEffects [ DragChanged drag ]
        | MoveDrag(x, y) ->
            let drag =
                model.ActiveDrag
                |> Option.map (fun current -> { current with CurrentX = x; CurrentY = y })

            { model with ActiveDrag = drag }
            |> withEffects [ DragChanged drag ]
        | EndDrag ->
            { model with ActiveDrag = None }
            |> withEffects [ DragChanged None ]
        | FocusLost ->
            { model with
                FocusedControl = None
                HoveredControl = None
                PressedControls = Set.empty
                ActiveDrag = None }
            |> withEffects [ FocusChanged None; HoverChanged None; PressedControlsChanged []; DragChanged None ]
        | RemoveControl controlId ->
            let next = clearTarget controlId model
            let diagnostic = staleDiagnostic controlId

            { next with Diagnostics = diagnostic :: next.Diagnostics }
            |> withEffects [ StaleTarget controlId; ReportControlRuntimeDiagnostic diagnostic ]
        | RecoverStaleTarget controlId ->
            let diagnostic = staleDiagnostic controlId

            { model with Diagnostics = diagnostic :: model.Diagnostics }
            |> withEffects [ StaleTarget controlId; ReportControlRuntimeDiagnostic diagnostic ]
        | CancelInteraction controlId ->
            let diagnostic = cancelledDiagnostic controlId

            { model with
                PressedControls = Set.empty
                Caret = None
                Selection = None
                Composition = None
                ActiveDrag = None
                Diagnostics = diagnostic :: model.Diagnostics }
            |> withEffects [ CancelledInteraction controlId; DragChanged None; ReportControlRuntimeDiagnostic diagnostic ]
        | Reset ->
            empty |> withEffects []

    let diagnostics model =
        model.Diagnostics
