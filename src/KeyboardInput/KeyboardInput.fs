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
    let init bindings =
        { Bindings = bindings
          PressedKeys = Set.empty
          LastCommand = None },
        ([]: KeyboardEffect list)

    let update msg model =
        match msg with
        | KeyDown key ->
            let pressed = model.PressedKeys |> Set.add key

            let command =
                model.Bindings
                |> List.tryFind (fun binding -> binding.Key = key)
                |> Option.map _.Command

            let effects =
                [ KeyStateChanged(Set.toList pressed)
                  match command with
                  | Some command -> CommandResolved command
                  | None -> KeyStateChanged(Set.toList pressed) ]
                |> List.distinct

            { model with PressedKeys = pressed; LastCommand = command }, effects
        | KeyUp key ->
            let pressed = model.PressedKeys |> Set.remove key
            { model with PressedKeys = pressed }, [ KeyStateChanged(Set.toList pressed) ]
        | Reset ->
            { model with PressedKeys = Set.empty; LastCommand = None }, [ KeyStateChanged [] ]
