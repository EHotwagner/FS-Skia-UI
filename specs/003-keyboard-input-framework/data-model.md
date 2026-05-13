# Data Model: Keyboard Input Framework

## InputConfiguration

User-authored configuration after YAML decoding but before activation.

**Fields**:

- `Version`: schema version.
- `Layouts`: available layout profiles.
- `DefaultLayoutId`: selected layout at startup.
- `DefaultModeId`: base mode used to initialize the mode stack.
- `Modes`: declared modes.
- `Bindings`: key bindings grouped by mode.
- `BigramProfile`: optional usage weights and ergonomic scoring inputs.
- `Display`: optional layout-state display preferences.
- `CommandIntent`: optional advanced intent policy/template section.

**Validation rules**:

- Version must be supported.
- Default layout must exist.
- Default mode must exist and be a standard or stateful mode.
- Every binding must reference an existing mode.
- Every command binding must reference a registered command identifier.
- YAML cannot declare arbitrary host actions.
- Stateful modes must have a non-empty state set and default state.
- Popup and temporary modes must have clear cancellation/release behavior.

## CommandRegistry

Application-provided list of commands the input framework is allowed to emit.

**Fields**:

- `Commands`: identifiers and optional display names/categories.

**Validation rules**:

- Identifiers are unique.
- Identifiers are stable strings intended for configuration and test fixtures.
- Missing or duplicate identifiers reject activation.

## CanonicalInputModel

Validated runtime-ready input model.

**Fields**:

- `ConfigurationVersion`
- `CommandRegistry`
- `Layouts`
- `ActiveLayoutId`
- `DefaultModeId`
- `Modes`
- `Bindings`
- `Disambiguation`
- `BigramProfile`
- `Display`

**Relationships**:

- Created from `InputConfiguration` plus `CommandRegistry`.
- Used by `InputRuntime` for all input resolution.

## Mode

Named input context.

**Fields**:

- `Id`
- `Kind`: standard, stateful, popup, or temporary held.
- `DefaultState`: required for stateful modes.
- `States`: valid states for stateful modes.
- `CancelKeys`
- `Timeout`
- `DisplayName`

**Validation rules**:

- Stateful modes always have a current state.
- Popup modes must define bounded next-input or cancellation behavior.
- Temporary held modes must be releasable by key-up and focus-loss cleanup.

## ModeStack

Ordered active input contexts.

**Fields**:

- `Frames`: base-to-top stack.

**State transitions**:

- Initialize with base mode and valid state.
- Push popup mode on popup key press.
- Push temporary mode on held key press.
- Pop popup after command, cancellation, or timeout.
- Pop temporary mode on matching key release.
- Pop all temporary modes on focus loss.
- Preserve underlying stateful mode unless a resolved action changes it.

## ModeState

Current value of a stateful mode.

**Fields**:

- `ModeId`
- `StateId`
- `UpdatedBy`: optional input event id or command id.

**Validation rules**:

- State id must be valid for the mode.
- There is no undefined state for stateful modes.

## Binding

Mapping from a key condition to an input outcome.

**Fields**:

- `ModeId`
- `Sequence`: one or more key chords/positions.
- `Outcome`: command, state transition, push popup, push temporary, cancel, or no-op.
- `WhenState`: optional state guard.
- `Weight`: optional usage weight.

**Validation rules**:

- Duplicate sequences in the same mode/state are invalid unless an explicit disambiguation rule exists.
- Command outcomes must reference registered commands.
- Temporary outcomes must reference temporary modes.
- Popup outcomes must reference popup modes.

## KeyPosition

Layout-independent key identity.

**Fields**:

- `Id`: stable physical position id.
- `Hand`
- `Finger`
- `Row`
- `Column`

**Validation rules**:

- Position ids are unique inside a layout profile.
- Physical metadata must be sufficient for same-finger and travel-distance analysis.

## LayoutProfile

Keyboard layout metadata.

**Fields**:

- `Id`
- `DisplayName`
- `Positions`
- `Labels`
- `SupportedPlatforms`: optional.

**Validation rules**:

- Labels must map to known positions.
- Missing labels are allowed only when display falls back to position id.

## BigramProfile

Usage-weighted command-pair and key-pair data.

**Fields**:

- `CommandPairWeights`
- `RiskRules`
- `SuggestionLimit`

**Validation rules**:

- Weights must be non-negative.
- Unknown command ids are invalid.
- Reports are read-only and do not mutate configuration.

## BigramReport

Analysis output.

**Fields**:

- `GeneratedForLayout`
- `TopPairs`
- `Risks`
- `Suggestions`
- `ScoreSummary`

**Validation rules**:

- Report must identify the source layout and configuration.
- Suggestions must be non-mutating and explicit about expected score impact.

## InputRuntime

Current runtime state.

**Fields**:

- `Model`: canonical input model.
- `ModeStack`
- `PressedKeys`
- `PendingSequence`
- `ActiveLayoutId`
- `EventLog`
- `Diagnostics`

**State transitions**:

- `KeyDown`: update pressed keys, resolve binding or pending sequence, push modes when needed.
- `KeyUp`: release matching temporary held modes and update pressed keys.
- `Timeout`: cancel or resolve pending sequence according to disambiguation.
- `FocusLost`: clear pressed keys and pop temporary held modes.
- `SetLayout`: change active layout if the layout exists.
- `Replay`: fold recorded messages through `update`.

## InputMsg

Runtime message.

**Cases**:

- `KeyDown`
- `KeyUp`
- `FocusLost`
- `Timeout`
- `SetLayout`
- `Cancel`
- `ReplayEvent`

## InputEffect

Pure transition output for host handling.

**Cases**:

- `ResolvedCommand`
- `ModeChanged`
- `LayoutStateChanged`
- `DiagnosticEmitted`
- `EventRecorded`
- `CommandIntentStatusChanged`

## InputDiagnostic

Structured diagnostic.

**Fields**:

- `Severity`
- `Code`
- `Message`
- `ModeId`
- `BindingId`
- `CommandId`
- `EventId`

**Validation rules**:

- Invalid configuration diagnostics block activation.
- Runtime diagnostics do not throw; they are emitted as effects and stored in state.

## LayoutStateView

Optional display data.

**Fields**:

- `ActiveModeStack`
- `ActiveState`
- `HeldModes`
- `PendingPopup`
- `PendingSequence`
- `ActiveLayout`
- `KeyLabels`

## CommandIntent

Optional advanced intent data.

**Fields**:

- `Id`
- `Source`
- `CommandId`
- `Constraints`

**Scope**:

- Data contract only in v1.
- No standard command grammar required.

## CommandPlan

Optional advanced plan data.

**Fields**:

- `Id`
- `IntentId`
- `Steps`
- `Status`
- `Failure`

**Scope**:

- Used for future opt-in command-intent flows and samples.
- Standard key input does not depend on it.
