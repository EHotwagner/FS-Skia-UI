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
    val create: definition: CustomControlDefinition<'msg> -> attrs: Attr<'msg> list -> Control<'msg>
    val validate: definition: CustomControlDefinition<'msg> -> ControlDiagnostic list
