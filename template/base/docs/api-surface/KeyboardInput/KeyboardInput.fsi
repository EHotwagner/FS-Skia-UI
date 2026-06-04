namespace FS.Skia.UI.KeyboardInput

/// Public contract type exposed by this FS.Skia.UI package.
type CommandId = string
/// Public contract type exposed by this FS.Skia.UI package.
type KeyId = string

/// Public contract type exposed by this FS.Skia.UI package.
type ViewerKey =
    | ArrowLeft
    | ArrowRight
    | ArrowUp
    | ArrowDown
    | Enter
    | Space
    | Escape
    | Backspace
    | Letter of char
    | Digit of int
    | Function of int
    | Unknown of raw: string

/// Public contract type exposed by this FS.Skia.UI package.
type ViewerKeyDirection =
    | KeyDown
    | KeyUp

/// Public contract type exposed by this FS.Skia.UI package.
type ViewerKeyEvent =
    { RawKey: string
      Direction: ViewerKeyDirection }

/// Public contract type exposed by this FS.Skia.UI package.
type KeyboardBinding =
    { Key: KeyId
      Command: CommandId }

/// Public contract type exposed by this FS.Skia.UI package.
type KeyboardDiagnostic =
    { Code: string
      Severity: string
      Message: string
      Key: KeyId option }

/// Public contract type exposed by this FS.Skia.UI package.
type KeyboardStateDisplay =
    { PressedKeys: KeyId list
      ActiveLayout: string
      ActiveModeStack: string list
      PendingSequence: KeyId list
      LastCommand: CommandId option }

/// Public contract type exposed by this FS.Skia.UI package.
type KeyboardEffect =
    | CommandResolved of CommandId
    | KeyStateChanged of KeyId list
    | LayoutChanged of string
    | ModeChanged of string list
    | PendingSequenceChanged of KeyId list
    | StateDisplayChanged of KeyboardStateDisplay
    | ReportKeyboardDiagnostic of KeyboardDiagnostic
    | RequestHostKeyCapture of KeyId

/// Public contract type exposed by this FS.Skia.UI package.
type KeyboardModel =
    { Bindings: KeyboardBinding list
      PressedKeys: Set<KeyId>
      LastCommand: CommandId option
      ActiveLayout: string
      ActiveModeStack: string list
      PersistentModeState: Map<string, string>
      PendingSequence: KeyId list
      Diagnostics: KeyboardDiagnostic list
      RecentEffects: KeyboardEffect list
      StateDisplay: KeyboardStateDisplay }

/// Public contract type exposed by this FS.Skia.UI package.
type KeyboardMsg =
    | KeyDown of KeyId
    | KeyUp of KeyId
    | FocusLost
    | Reset
    | SetActiveLayout of string
    | PushTemporaryMode of string
    | PopTemporaryMode
    | SetPersistentMode of key: string * value: string
    | ResolvePendingSequence of KeyId list

/// Public contract module exposed by this FS.Skia.UI package.
module Keyboard =
    /// Public contract function exposed by this FS.Skia.UI package.
    val init: bindings: KeyboardBinding list -> KeyboardModel * KeyboardEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val update: msg: KeyboardMsg -> model: KeyboardModel -> KeyboardModel * KeyboardEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val stateDisplay: model: KeyboardModel -> KeyboardStateDisplay

/// Public contract module exposed by this FS.Skia.UI package.
module ViewerKeyboard =
    /// Public contract function exposed by this FS.Skia.UI package.
    val normalize: raw: string -> ViewerKey
    /// Public contract function exposed by this FS.Skia.UI package.
    val normalizeEvent: event: ViewerKeyEvent -> ViewerKey * bool
    /// Public contract function exposed by this FS.Skia.UI package.
    val toKeyId: key: ViewerKey -> KeyId
