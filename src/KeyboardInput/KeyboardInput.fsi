namespace FS.Skia.UI.KeyboardInput

type CommandId = string
type KeyId = string

type KeyboardBinding =
    { Key: KeyId
      Command: CommandId }

type KeyboardDiagnostic =
    { Code: string
      Severity: string
      Message: string
      Key: KeyId option }

type KeyboardStateDisplay =
    { PressedKeys: KeyId list
      ActiveLayout: string
      ActiveModeStack: string list
      PendingSequence: KeyId list
      LastCommand: CommandId option }

type KeyboardEffect =
    | CommandResolved of CommandId
    | KeyStateChanged of KeyId list
    | LayoutChanged of string
    | ModeChanged of string list
    | PendingSequenceChanged of KeyId list
    | StateDisplayChanged of KeyboardStateDisplay
    | ReportKeyboardDiagnostic of KeyboardDiagnostic
    | RequestHostKeyCapture of KeyId

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

module Keyboard =
    val init: bindings: KeyboardBinding list -> KeyboardModel * KeyboardEffect list
    val update: msg: KeyboardMsg -> model: KeyboardModel -> KeyboardModel * KeyboardEffect list
    val stateDisplay: model: KeyboardModel -> KeyboardStateDisplay
