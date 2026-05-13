# Public API Contract: Keyboard Input Framework

This contract sketches the intended `.fsi` surface for `src/Lib/KeyboardInput.fsi`. Names may be adjusted during FSI validation, but the behavioral surface must remain equivalent.

```fsharp
namespace FS.Skia.UI

open System

type CommandId = string
type ModeId = string
type StateId = string
type LayoutId = string
type KeyPositionId = string

type InputSeverity =
    | InputInfo
    | InputWarning
    | InputError
    | InputFatal

type InputDiagnosticCode =
    | InvalidYaml
    | UnsupportedSchemaVersion
    | DuplicateBinding
    | UnknownMode
    | UnknownCommand
    | InvalidModeState
    | AmbiguousSequence
    | StaleInputEvent
    | LostKeyReleaseRecovered
    | HostActionRejected
    | UnsatisfiedCommandIntent

type InputDiagnostic =
    { Severity: InputSeverity
      Code: InputDiagnosticCode
      Message: string
      ModeId: ModeId option
      CommandId: CommandId option
      KeyPositionId: KeyPositionId option }

type CommandDefinition =
    { Id: CommandId
      DisplayName: string
      Category: string option }

type CommandRegistry =
    { Commands: CommandDefinition list }

type ModeKind =
    | StandardMode
    | StatefulMode
    | PopupMode
    | TemporaryHeldMode

type ModeDefinition =
    { Id: ModeId
      DisplayName: string
      Kind: ModeKind
      States: StateId list
      DefaultState: StateId option
      CancelKeys: KeyPositionId list }

type Hand =
    | LeftHand
    | RightHand
    | EitherHand
    | UnknownHand

type Finger =
    | Thumb
    | Index
    | Middle
    | Ring
    | Pinky
    | UnknownFinger

type KeyPosition =
    { Id: KeyPositionId
      Hand: Hand
      Finger: Finger
      Row: int
      Column: int }

type LayoutProfile =
    { Id: LayoutId
      DisplayName: string
      Positions: KeyPosition list
      Labels: Map<KeyPositionId, string> }

type KeyChord =
    { Position: KeyPositionId
      RequiredHeld: KeyPositionId list }

type BindingOutcome =
    | EmitCommand of CommandId
    | SetState of ModeId * StateId
    | PushPopup of ModeId
    | PushTemporary of ModeId
    | CancelTopMode
    | NoInputOp

type BindingDefinition =
    { ModeId: ModeId
      Sequence: KeyChord list
      WhenState: StateId option
      Outcome: BindingOutcome
      Weight: float option }

type DisambiguationPolicy =
    { TimeoutMilliseconds: int }

type BigramWeight =
    { First: CommandId
      Second: CommandId
      Weight: float }

type BigramProfile =
    { Weights: BigramWeight list
      SuggestionLimit: int }

type DisplayOptions =
    { ShowLayoutState: bool
      ShowPendingSequence: bool }

type InputConfiguration =
    { Version: int
      DefaultLayout: LayoutId
      DefaultMode: ModeId
      Layouts: LayoutProfile list
      Modes: ModeDefinition list
      Bindings: BindingDefinition list
      Disambiguation: DisambiguationPolicy
      BigramProfile: BigramProfile option
      Display: DisplayOptions }

type CanonicalInputModel =
    { Configuration: InputConfiguration
      Registry: CommandRegistry }

type ModeFrame =
    { ModeId: ModeId
      State: StateId option
      EnteredBy: KeyPositionId option }

type InputEventId = Guid

type InputEvent =
    { Id: InputEventId
      OccurredAt: DateTimeOffset
      Description: string }

type PendingSequence =
    { StartedAt: DateTimeOffset
      Chords: KeyChord list }

type InputRuntime =
    { Model: CanonicalInputModel
      ModeStack: ModeFrame list
      PressedKeys: Set<KeyPositionId>
      PendingSequence: PendingSequence option
      ActiveLayout: LayoutId
      Events: InputEvent list
      Diagnostics: InputDiagnostic list }

type InputMsg =
    | KeyDown of KeyPositionId
    | KeyUp of KeyPositionId
    | FocusLost
    | Timeout
    | SetLayout of LayoutId
    | Cancel

type LayoutStateView =
    { ActiveModeStack: ModeFrame list
      HeldModes: ModeFrame list
      PendingSequence: PendingSequence option
      ActiveLayout: LayoutProfile
      ActiveLabels: Map<KeyPositionId, string> }

type ResolvedCommand =
    { CommandId: CommandId
      ModeStack: ModeFrame list
      SourceKey: KeyPositionId }

type InputEffect =
    | CommandResolved of ResolvedCommand
    | LayoutStateChanged of LayoutStateView
    | InputDiagnosticEmitted of InputDiagnostic
    | InputEventRecorded of InputEvent

type BigramRiskKind =
    | SameFinger
    | LongTravel
    | AwkwardHold
    | SameHandRepeat

type BigramRisk =
    { First: CommandId
      Second: CommandId
      Weight: float
      Kind: BigramRiskKind
      Description: string }

type BigramSuggestion =
    { First: CommandId
      Second: CommandId
      Description: string
      ExpectedScoreDelta: float }

type BigramReport =
    { LayoutId: LayoutId
      TopPairs: BigramWeight list
      Risks: BigramRisk list
      Suggestions: BigramSuggestion list }

type CommandIntent =
    { Id: string
      CommandId: CommandId
      Constraints: string list }

type CommandPlanStatus =
    | Planned
    | AwaitingApproval
    | Executing
    | Completed
    | Failed
    | Cancelled

type CommandPlan =
    { Id: string
      Intent: CommandIntent
      Steps: string list
      Status: CommandPlanStatus
      Failure: string option }

module KeyboardInput =
    val commandRegistry : commands: CommandDefinition list -> Result<CommandRegistry, InputDiagnostic list>

    val parseYaml : yaml: string -> Result<InputConfiguration, InputDiagnostic list>

    val validate :
        registry: CommandRegistry ->
        configuration: InputConfiguration ->
            Result<CanonicalInputModel, InputDiagnostic list>

    val init :
        activeLayout: LayoutId ->
        model: CanonicalInputModel ->
            Result<InputRuntime * InputEffect list, InputDiagnostic list>

    val update : msg: InputMsg -> runtime: InputRuntime -> InputRuntime * InputEffect list

    val layoutState : runtime: InputRuntime -> LayoutStateView

    val replay :
        initial: InputRuntime ->
        messages: InputMsg list ->
            InputRuntime * InputEffect list

    val analyzeBigrams : model: CanonicalInputModel -> layout: LayoutId -> BigramReport
```

## Contract Requirements

- `parseYaml` performs syntax/schema parsing only. It must not activate untrusted commands.
- `validate` is the only path from parsed configuration to `CanonicalInputModel`.
- `validate` rejects unregistered command identifiers and host-action-like entries.
- `init` fails if the active layout or base stateful mode cannot be established.
- `update` is pure and must not perform file I/O, process execution, rendering, or mutable global state changes.
- `replay` must be deterministic for the same initial runtime and message list.
- `analyzeBigrams` must not mutate the model or return replacement configuration.
- Applications map `ResolvedCommand.CommandId` to domain messages outside this module.
