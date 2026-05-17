namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene
open FS.Skia.UI.Layout

type CustomControlDefinition<'msg> =
    { Id: ControlId
      Measure: unit -> float * float
      Render: unit -> Scene
      Draw: unit -> Scene
      Layout: unit -> LayoutNode
      Clip: (float * float * float * float) option
      Effects: string list
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
          for effect in definition.Effects do
              if effect.Trim() = "" then
                  yield Diagnostics.missingRequired (Some definition.Id) "custom-control" "effect"
          yield! definition.Diagnostics ]

    let create (definition: CustomControlDefinition<'msg>) (attrs: Attr<'msg> list) =
        Control.create "custom-control" (Attr.accessibility (definition.Accessibility |> Option.defaultValue (Accessibility.defaultFor "custom-control" definition.Id)) :: attrs)
        |> Control.withKey definition.Id
