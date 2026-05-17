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
    let stateDisplay model =
        { PressedKeys = model.PressedKeys |> Set.toList
          ActiveLayout = model.ActiveLayout
          ActiveModeStack = model.ActiveModeStack
          PendingSequence = model.PendingSequence
          LastCommand = model.LastCommand }

    let attachState effects model =
        let display = stateDisplay model
        { model with StateDisplay = display; RecentEffects = effects }, effects

    let init bindings =
        let display =
            { PressedKeys = []
              ActiveLayout = "default"
              ActiveModeStack = []
              PendingSequence = []
              LastCommand = None }

        let effects = [ StateDisplayChanged display ]

        { Bindings = bindings
          PressedKeys = Set.empty
          LastCommand = None
          ActiveLayout = "default"
          ActiveModeStack = []
          PersistentModeState = Map.empty
          PendingSequence = []
          Diagnostics = []
          RecentEffects = effects
          StateDisplay = display },
        effects

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

            { model with PressedKeys = pressed; LastCommand = command }
            |> attachState effects
        | KeyUp key ->
            let pressed = model.PressedKeys |> Set.remove key
            let effects = [ KeyStateChanged(Set.toList pressed) ]
            { model with PressedKeys = pressed } |> attachState effects
        | FocusLost ->
            let diagnostic =
                { Code = "FocusLostRecovered"
                  Severity = "Warning"
                  Message = "Focus loss cleared pressed keys and temporary modes."
                  Key = None }

            let effects =
                [ KeyStateChanged []
                  ModeChanged []
                  PendingSequenceChanged []
                  ReportKeyboardDiagnostic diagnostic ]

            { model with
                PressedKeys = Set.empty
                ActiveModeStack = []
                PendingSequence = []
                Diagnostics = diagnostic :: model.Diagnostics }
            |> attachState effects
        | Reset ->
            let effects =
                [ KeyStateChanged []
                  ModeChanged []
                  PendingSequenceChanged [] ]

            { model with
                PressedKeys = Set.empty
                LastCommand = None
                ActiveModeStack = []
                PendingSequence = []
                PersistentModeState = Map.empty
                Diagnostics = [] }
            |> attachState effects
        | SetActiveLayout layout ->
            { model with ActiveLayout = layout }
            |> attachState [ LayoutChanged layout ]
        | PushTemporaryMode mode ->
            let modes = mode :: model.ActiveModeStack
            { model with ActiveModeStack = modes }
            |> attachState [ ModeChanged modes ]
        | PopTemporaryMode ->
            let modes =
                match model.ActiveModeStack with
                | _ :: rest -> rest
                | [] -> []

            { model with ActiveModeStack = modes }
            |> attachState [ ModeChanged modes ]
        | SetPersistentMode(key, value) ->
            let state = model.PersistentModeState |> Map.add key value
            { model with PersistentModeState = state }
            |> attachState [ ModeChanged model.ActiveModeStack ]
        | ResolvePendingSequence sequence ->
            { model with PendingSequence = sequence }
            |> attachState [ PendingSequenceChanged sequence ]
