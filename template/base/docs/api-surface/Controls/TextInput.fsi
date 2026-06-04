namespace FS.Skia.UI.Controls

/// Public contract type exposed by this FS.Skia.UI package.
type TextInputMode =
    | SingleLine
    | MultiLine

/// Public contract type exposed by this FS.Skia.UI package.
type TextSelection =
    { Start: int
      End: int }

/// Public contract type exposed by this FS.Skia.UI package.
type TextInputModel =
    { ControlId: ControlId
      Mode: TextInputMode
      CommittedText: string
      DraftText: string
      CaretIndex: int
      Selection: TextSelection option
      Composition: string option
      Validation: ValidationState
      Focused: bool }

/// Public contract type exposed by this FS.Skia.UI package.
type TextInputMsg =
    | Focus
    | Blur
    | InsertText of string
    | MoveCaret of int
    | SelectRange of int * int
    | RequestClipboardPaste
    | ClipboardTextReceived of string
    | Commit
    | Cancel
    | CompositionStarted of string
    | CompositionCommitted of string
    | ApplyValidation of ValidationState

/// Public contract type exposed by this FS.Skia.UI package.
type TextInputEffect =
    | RequestClipboardText of ControlId
    | CommitText of ControlId * string
    | ReportTextInputDiagnostic of ControlDiagnostic

/// Public contract module exposed by this FS.Skia.UI package.
module TextInput =
    /// Public contract function exposed by this FS.Skia.UI package.
    val init: controlId: ControlId -> mode: TextInputMode -> value: string -> TextInputModel * TextInputEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val update: msg: TextInputMsg -> model: TextInputModel -> TextInputModel * TextInputEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val interpretEffect: effect: TextInputEffect -> TextInputMsg option
    /// Public contract function exposed by this FS.Skia.UI package.
    val diagnostics: model: TextInputModel -> ControlDiagnostic list
