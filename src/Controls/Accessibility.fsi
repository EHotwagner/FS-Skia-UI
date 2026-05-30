namespace FS.Skia.UI.Controls

/// Public contract module exposed by this FS.Skia.UI package.
module Accessibility =
    /// Public contract function exposed by this FS.Skia.UI package.
    val keyboard: focusable: bool -> activationKeys: string list -> navigationKeys: string list -> KeyboardOperation
    /// Public contract function exposed by this FS.Skia.UI package.
    val contrast: foreground: FS.Skia.UI.Scene.Color -> background: FS.Skia.UI.Scene.Color -> ratio: float -> requiredRatio: float -> ContrastEvidence
    /// Public contract function exposed by this FS.Skia.UI package.
    val metadata:
        role: AccessibilityRole ->
        nameSource: string ->
        state: string list ->
        focusOrder: int option ->
        keyboard: KeyboardOperation ->
        contrast: ContrastEvidence option ->
            AccessibilityMetadata

    /// Public contract function exposed by this FS.Skia.UI package.
    val defaultFor: kind: ControlKind -> label: string -> AccessibilityMetadata
    /// Public contract function exposed by this FS.Skia.UI package.
    val validate: control: Control<'msg> -> ControlDiagnostic list
