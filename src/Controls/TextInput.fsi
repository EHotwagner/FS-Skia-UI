namespace FS.Skia.UI.Controls

type TextInputMode =
    | SingleLine
    | MultiLine

type TextSelection =
    { Start: int
      End: int }

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

type TextInputEffect =
    | RequestClipboardText of ControlId
    | CommitText of ControlId * string
    | ReportTextInputDiagnostic of ControlDiagnostic

module TextInput =
    val init: controlId: ControlId -> mode: TextInputMode -> value: string -> TextInputModel * TextInputEffect list
    val update: msg: TextInputMsg -> model: TextInputModel -> TextInputModel * TextInputEffect list
    val interpretEffect: effect: TextInputEffect -> TextInputMsg option
    val diagnostics: model: TextInputModel -> ControlDiagnostic list
