# Public API Contract: Keyboard State Display Element

This contract extends the existing `FS.Skia.UI.KeyboardInput` public surface in `src/Lib/KeyboardInput.fsi`. Exact names may be adjusted during FSI sketching, but the implemented public surface must preserve these capabilities.

## New Public Types

```fsharp
type KeyboardStateDisplayVisibility =
    | KeyboardStateDisplayHidden
    | KeyboardStateDisplayVisible

type KeyboardStateDisplayDensity =
    | KeyboardStateDisplayCompact
    | KeyboardStateDisplayExpanded

type KeyboardStateDisplayOptions =
    { Visibility: KeyboardStateDisplayVisibility
      Density: KeyboardStateDisplayDensity
      ShowKeyLabels: bool
      ShowPendingSequence: bool
      ShowRecentCommand: bool
      ShowDiagnostic: bool
      MaxCompactLabels: int
      MaxExpandedLabels: int }

type KeyboardStateDisplayLayout =
    { Id: LayoutId
      DisplayName: string option
      IsAvailable: bool }

type KeyboardStateDisplayContextKind =
    | DisplayPermanentContext
    | DisplayStatefulContext
    | DisplayPopupContext
    | DisplayTemporaryHeldContext
    | DisplayUnknownContext

type KeyboardStateDisplayStackEntry =
    { ModeId: ModeId
      DisplayName: string option
      Kind: KeyboardStateDisplayContextKind
      State: StateId option
      EnteredBy: KeyPositionId option
      IsTop: bool
      IsPersistent: bool }

type KeyboardStateDisplayLabel =
    { KeyPositionId: KeyPositionId
      Label: string
      CommandId: CommandId option
      Outcome: string }

type KeyboardStateDisplayPendingSequence =
    { Chords: KeyChord list
      StartedAt: DateTimeOffset
      IsTimed: bool
      TimeoutMilliseconds: int option }

type KeyboardStateDisplayRecentCommand =
    { CommandId: CommandId
      DisplayName: string option
      SourceKey: KeyPositionId }

type KeyboardStateDisplayDiagnostic =
    { Severity: InputSeverity
      Code: InputDiagnosticCode
      Message: string
      ModeId: ModeId option
      CommandId: CommandId option
      KeyPositionId: KeyPositionId option }

type KeyboardStateDisplayOmission =
    | OmittedLabels of omittedCount: int
    | OmittedPendingSequence
    | OmittedRecentCommand
    | OmittedDiagnostic
    | OmittedStackEntries of omittedCount: int

type KeyboardStateDisplayModel =
    { Visibility: KeyboardStateDisplayVisibility
      Density: KeyboardStateDisplayDensity
      Layout: KeyboardStateDisplayLayout option
      Stack: KeyboardStateDisplayStackEntry list
      TopContext: KeyboardStateDisplayStackEntry option
      ActiveState: StateId option
      Labels: KeyboardStateDisplayLabel list
      PendingSequence: KeyboardStateDisplayPendingSequence option
      RecentCommand: KeyboardStateDisplayRecentCommand option
      Diagnostic: KeyboardStateDisplayDiagnostic option
      Omitted: KeyboardStateDisplayOmission list
      IsPartial: bool }
```

## New Public Functions

```fsharp
module KeyboardInput =
    val defaultStateDisplayOptions : KeyboardStateDisplayOptions

    val compactStateDisplayOptions : KeyboardStateDisplayOptions

    val expandedStateDisplayOptions : KeyboardStateDisplayOptions

    val keyboardStateDisplay :
        options: KeyboardStateDisplayOptions ->
        recentEffects: InputEffect list ->
        runtime: InputRuntime ->
            KeyboardStateDisplayModel

    val renderKeyboardStateDisplay :
        options: KeyboardStateDisplayOptions ->
        recentEffects: InputEffect list ->
        runtime: InputRuntime ->
            Scene

    val renderKeyboardStateDisplayAt :
        position: float * float ->
        options: KeyboardStateDisplayOptions ->
        recentEffects: InputEffect list ->
        runtime: InputRuntime ->
            Scene
```

## Compatibility Contract

- Existing `layoutState`, `renderLayoutState`, and `renderLayoutStateAt` remain available for compatibility.
- Existing render functions may delegate to the new expanded state display renderer.
- No new dependency is introduced.
- Surface-area baseline `FS.Skia.UI.txt` must be updated.

## Behavioral Contract

- Hidden visibility returns a hidden model and an empty scene.
- Compact density prioritizes active layout, top context, condensed stack, and active state.
- Expanded density shows full available stack details and all allowed hints up to configured limits.
- Label hints include only bindings available in the active top context.
- Diagnostic display includes at most one diagnostic: the most recent actionable diagnostic.
- Missing or invalid active layout produces a partial model and still renders available stack, state, and diagnostic data.
- Scene output must be stable enough for `Scene.describe` tests to assert panel/text primitives without depending on exact pixel placement.
