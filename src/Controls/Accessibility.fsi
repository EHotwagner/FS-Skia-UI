namespace FS.Skia.UI.Controls

module Accessibility =
    val keyboard: focusable: bool -> activationKeys: string list -> navigationKeys: string list -> KeyboardOperation
    val contrast: foreground: FS.Skia.UI.Color -> background: FS.Skia.UI.Color -> ratio: float -> requiredRatio: float -> ContrastEvidence
    val metadata:
        role: AccessibilityRole ->
        nameSource: string ->
        state: string list ->
        focusOrder: int option ->
        keyboard: KeyboardOperation ->
        contrast: ContrastEvidence option ->
            AccessibilityMetadata

    val defaultFor: kind: ControlKind -> label: string -> AccessibilityMetadata
    val validate: control: Control<'msg> -> ControlDiagnostic list
