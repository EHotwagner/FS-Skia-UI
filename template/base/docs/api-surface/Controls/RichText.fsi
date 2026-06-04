namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene

/// Public contract type exposed by this FS.Skia.UI package.
type RichTextWeight =
    | Regular
    | Medium
    | Bold

/// Public contract type exposed by this FS.Skia.UI package.
type RichTextStyle =
    { FontFamily: string option
      FontSize: float
      Weight: RichTextWeight
      Foreground: Color
      Background: Color option
      Underline: bool
      Italic: bool }

/// Public contract type exposed by this FS.Skia.UI package.
type RichTextRun =
    { Text: string
      Style: RichTextStyle
      Diagnostics: ControlDiagnostic list }

/// Public contract type exposed by this FS.Skia.UI package.
type RichTextBlock =
    { Runs: RichTextRun list
      MaxWidth: float option
      Clip: bool
      Effects: string list
      Accessibility: AccessibilityMetadata option }

/// Public contract type exposed by this FS.Skia.UI package.
type RichTextMeasurement =
    { Width: float
      Height: float
      LineCount: int
      Diagnostics: ControlDiagnostic list }

/// Public contract module exposed by this FS.Skia.UI package.
module RichText =
    /// Public contract function exposed by this FS.Skia.UI package.
    val defaultStyle: Theme -> RichTextStyle
    /// Public contract function exposed by this FS.Skia.UI package.
    val run: text: string -> style: RichTextStyle -> RichTextRun
    /// Public contract function exposed by this FS.Skia.UI package.
    val block: runs: RichTextRun list -> RichTextBlock
    /// Public contract function exposed by this FS.Skia.UI package.
    val measure: block: RichTextBlock -> RichTextMeasurement
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: block: RichTextBlock -> Attr<'msg> list -> Control<'msg>
