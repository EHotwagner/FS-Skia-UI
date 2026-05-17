namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene

type RichTextWeight =
    | Regular
    | Medium
    | Bold

type RichTextStyle =
    { FontFamily: string option
      FontSize: float
      Weight: RichTextWeight
      Foreground: Color
      Background: Color option
      Underline: bool
      Italic: bool }

type RichTextRun =
    { Text: string
      Style: RichTextStyle
      Diagnostics: ControlDiagnostic list }

type RichTextBlock =
    { Runs: RichTextRun list
      MaxWidth: float option
      Clip: bool
      Effects: string list
      Accessibility: AccessibilityMetadata option }

type RichTextMeasurement =
    { Width: float
      Height: float
      LineCount: int
      Diagnostics: ControlDiagnostic list }

module RichText =
    val defaultStyle: Theme -> RichTextStyle
    val run: text: string -> style: RichTextStyle -> RichTextRun
    val block: runs: RichTextRun list -> RichTextBlock
    val measure: block: RichTextBlock -> RichTextMeasurement
    val create: block: RichTextBlock -> Attr<'msg> list -> Control<'msg>
