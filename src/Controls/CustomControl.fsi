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
    val create: definition: CustomControlDefinition<'msg> -> attrs: Attr<'msg> list -> Control<'msg>
    val validate: definition: CustomControlDefinition<'msg> -> ControlDiagnostic list
