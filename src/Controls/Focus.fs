namespace FS.Skia.UI.Controls

type FocusStop =
    { Control: ControlId
      Role: AccessibilityRole
      Keyboard: KeyboardOperation
      FocusOrder: int option }

type TabOrder =
    { Stops: FocusStop list }

type FocusMove =
    | Next
    | Previous

type KeyRouting =
    | Activate
    | Navigate
    | Traverse of FocusMove
    | Fallthrough

module Focus =

    // The id scheme every host seam uses for a lowered control: the authored Key, else the Kind.
    // (Matches `node.Control.Key |> Option.defaultValue node.Control.Kind` at the retained seam.)
    let private controlId (c: Control<'msg>) : ControlId =
        c.Key |> Option.defaultValue c.Kind

    // FR-001: pre-order walk that emits a FocusStop for each focusable control and does NOT descend
    // into a focusable control's subtree (a composite is a single tab stop, clarified). A
    // non-focusable container is descended so its focusable descendants are found. `docIndex` is the
    // pre-order visit index, threaded so the sort tiebreak is stable document order.
    let order (control: Control<'msg>) : TabOrder =
        let stops = System.Collections.Generic.List<int * FocusStop>()
        let mutable docIndex = 0

        let rec walk (c: Control<'msg>) =
            let here = docIndex
            docIndex <- docIndex + 1

            match c.Accessibility with
            | Some metadata when metadata.Keyboard.Focusable ->
                stops.Add(
                    here,
                    { Control = controlId c
                      Role = metadata.Role
                      Keyboard = metadata.Keyboard
                      FocusOrder = metadata.FocusOrder }
                )
            // Focusable -> single stop; do not descend into its subtree.
            | _ ->
                // Non-focusable (or no metadata) -> descend to find focusable descendants.
                for child in c.Children do
                    walk child

        walk control

        // Stable sort by (FocusOrder ?? +inf, docIndex). List.sortBy is stable, and the docIndex
        // component makes the order fully deterministic even within an equal FocusOrder bucket.
        let ordered =
            stops
            |> List.ofSeq
            |> List.sortBy (fun (docIndex, stop) ->
                (match stop.FocusOrder with
                 | Some n -> n
                 | None -> System.Int32.MaxValue),
                docIndex)
            |> List.map snd

        { Stops = ordered }

    // FR-002: cyclic traversal reduction. Total/deterministic over the closed FocusMove set.
    let traverse (order: TabOrder) (current: ControlId option) (move: FocusMove) : ControlId option =
        let stops = order.Stops
        let n = List.length stops

        if n = 0 then
            None
        else
            let idOf (s: FocusStop) = s.Control
            let first () = Some(idOf stops.[0])
            let last () = Some(idOf stops.[n - 1])

            match current with
            | None ->
                match move with
                | Next -> first ()
                | Previous -> last ()
            | Some id ->
                match stops |> List.tryFindIndex (fun s -> idOf s = id) with
                | Some i ->
                    let j =
                        match move with
                        | Next -> (i + 1) % n
                        | Previous -> (i - 1 + n) % n

                    Some(idOf stops.[j])
                // Stale target: the current id is absent from the order (it was removed between
                // frames). Recover to the first stop on Next / last stop on Previous (the next stop
                // at the former start position) — never throws.
                | None ->
                    match move with
                    | Next -> first ()
                    | Previous -> last ()

    // FR-003/FR-007: classify a normalized key against the focused control's KeyboardOperation.
    // The control's own consumption (Activate/Navigate) is tested BEFORE the Tab test, so a control
    // that lists a traversal key in its own keys consumes it (never Traverse). Pure, total.
    let route (keyboard: KeyboardOperation) (key: string) (isTab: bool) (shift: bool) : KeyRouting =
        if List.contains key keyboard.ActivationKeys then
            Activate
        elif List.contains key keyboard.NavigationKeys then
            Navigate
        elif isTab then
            Traverse(if shift then Previous else Next)
        else
            Fallthrough
