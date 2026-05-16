namespace FS.Skia.UI.Controls

open FS.Skia.UI

module Accessibility =
    let keyboard focusable activationKeys navigationKeys =
        { Focusable = focusable
          ActivationKeys = activationKeys
          NavigationKeys = navigationKeys }

    let contrast foreground background ratio requiredRatio =
        { Foreground = foreground
          Background = background
          Ratio = ratio
          RequiredRatio = requiredRatio }

    let metadata role nameSource state focusOrder keyboard contrast =
        { Role = role
          NameSource = nameSource
          State = state
          FocusOrder = focusOrder
          Keyboard = keyboard
          Contrast = contrast }

    let roleFor kind =
        match kind with
        | "text-block"
        | "label"
        | "badge"
        | "validation-message" -> StaticText
        | "button"
        | "icon-button" -> Button
        | "text-box"
        | "text-area"
        | "numeric-input" -> TextBox
        | "check-box"
        | "switch" -> CheckBox
        | "radio-group" -> RadioGroup
        | "slider" -> Slider
        | "list-view"
        | "list-box"
        | "multi-select-list"
        | "combo-box"
        | "tree-view" -> List
        | "data-grid"
        | "table" -> Grid
        | "menu"
        | "context-menu"
        | "toolbar" -> Menu
        | "tabs" -> Tab
        | "dialog"
        | "overlay" -> Dialog
        | "progress-bar"
        | "spinner" -> Progress
        | "image"
        | "icon" -> Image
        | "line-chart"
        | "bar-chart"
        | "pie-chart"
        | "scatter-plot" -> Chart
        | "graph-view" -> Graph
        | _ -> Custom

    let defaultFor kind label =
        let role = roleFor kind
        let focusable =
            match role with
            | StaticText
            | Image
            | Progress -> false
            | _ -> true

        metadata
            role
            label
            [ "normal" ]
            None
            (keyboard focusable [ "Enter"; "Space" ] [ "Tab"; "Shift+Tab" ])
            (Some(contrast Colors.black Colors.white 7.0 4.5))

    let validate control =
        match control.Accessibility with
        | None -> [ Diagnostics.missingAccessibility control.Key control.Kind ]
        | Some metadata ->
            [ if metadata.Keyboard.Focusable && metadata.Keyboard.NavigationKeys.IsEmpty then
                  yield FS.Skia.UI.Controls.Diagnostics.create control.Key control.Kind MissingAccessibilityMetadata ControlDiagnosticSeverity.Error "Focusable control is missing keyboard navigation metadata."
              match metadata.Contrast with
              | Some evidence when evidence.Ratio < evidence.RequiredRatio ->
                  yield FS.Skia.UI.Controls.Diagnostics.create control.Key control.Kind ContrastFailure ControlDiagnosticSeverity.Error "Contrast evidence is below the required ratio."
              | _ -> () ]
