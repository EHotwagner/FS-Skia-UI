namespace FS.Skia.UI.Controls

open FS.Skia.UI
open FS.Skia.UI.Layout

type CustomControlDefinition<'msg> =
    { Id: ControlId
      Render: unit -> Scene
      Layout: unit -> LayoutNode
      HitTest: float -> float -> bool
      Event: ControlEvent -> 'msg option
      Accessibility: AccessibilityMetadata option
      Diagnostics: ControlDiagnostic list }

module CustomControl =
    let validate definition =
        [ if definition.Id.Trim() = "" then
              yield Diagnostics.missingRequired None "custom-control" "id"
          if definition.Accessibility.IsNone then
              yield Diagnostics.missingAccessibility (Some definition.Id) "custom-control"
          yield! definition.Diagnostics ]

    let create (definition: CustomControlDefinition<'msg>) (attrs: Attr<'msg> list) =
        Control.create "custom-control" (Attr.accessibility (definition.Accessibility |> Option.defaultValue (Accessibility.defaultFor "custom-control" definition.Id)) :: attrs)
        |> Control.withKey definition.Id
