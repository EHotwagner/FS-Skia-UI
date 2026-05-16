namespace FS.Skia.UI.KeyboardInput

type CommandId = string
type KeyId = string

type KeyboardBinding =
    { Key: KeyId
      Command: CommandId }

type KeyboardModel =
    { Bindings: KeyboardBinding list
      PressedKeys: Set<KeyId>
      LastCommand: CommandId option }

type KeyboardMsg =
    | KeyDown of KeyId
    | KeyUp of KeyId
    | Reset

type KeyboardEffect =
    | CommandResolved of CommandId
    | KeyStateChanged of KeyId list

module Keyboard =
    val init: bindings: KeyboardBinding list -> KeyboardModel * KeyboardEffect list
    val update: msg: KeyboardMsg -> model: KeyboardModel -> KeyboardModel * KeyboardEffect list
