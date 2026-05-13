# Data Model: Keyboard State Display Element

## Keyboard State Display Options

Controls whether and how the standard element is produced.

Fields:

- `Visibility`: hidden or visible.
- `Density`: compact or expanded.
- `ShowKeyLabels`: whether active top-context key labels may be included.
- `ShowPendingSequence`: whether pending sequence hints may be included.
- `ShowRecentCommand`: whether the most recent resolved command may be included.
- `ShowDiagnostic`: whether the most recent actionable diagnostic may be included.
- `MaxCompactLabels`: maximum label hints shown in compact density.
- `MaxExpandedLabels`: maximum label hints shown in expanded density.

Validation:

- Hidden visibility produces a hidden display model and an empty scene.
- Label limits must be non-negative.
- Compact density must preserve layout, top context, condensed stack, and state before optional hints.

## Keyboard State Display Model

Pure structured representation consumed by tests, alternate renderers, and the standard scene renderer.

Fields:

- `Visibility`: hidden or visible.
- `Density`: compact or expanded.
- `Layout`: active layout display summary when available.
- `Stack`: ordered context stack entries from persistent base to active top.
- `TopContext`: active top context when available.
- `ActiveState`: selected state for the active or nearest stateful context when present.
- `Labels`: current-context label hints.
- `PendingSequence`: optional pending sequence hint.
- `RecentCommand`: optional recent resolved command hint.
- `Diagnostic`: optional most recent actionable diagnostic.
- `Omitted`: details omitted because of density or limits.
- `IsPartial`: true when layout or stack data is incomplete but renderable.

Validation:

- `TopContext` must match the last stack entry when the stack is non-empty.
- Compact density may have omitted hints but must not omit layout, top context, condensed stack, or active state if those values exist.
- Expanded density should preserve all available stack entries and allowed hints.
- Partial models must include either available context data or a diagnostic explaining what is incomplete.

## Display Layout Summary

User-facing layout identity.

Fields:

- `Id`: active layout identifier.
- `DisplayName`: active layout display name when known.
- `IsAvailable`: false when the runtime active layout is missing from configuration.

Validation:

- `Id` is retained even when display name is unavailable.
- Missing layout data sets `IsAvailable = false` and contributes to `IsPartial`.

## Display Stack Entry

One visible context in the active keyboard stack.

Fields:

- `ModeId`: mode identifier.
- `DisplayName`: configured display name when known.
- `Kind`: permanent/stateful, popup, temporary held, or unknown.
- `State`: selected state when present.
- `EnteredBy`: source key for held/pushed temporary context when known.
- `IsTop`: true for the active top context.
- `IsPersistent`: true for standard and stateful base contexts.

Validation:

- Exactly one entry is top when stack is non-empty.
- Unknown mode definitions are retained as partial entries instead of being dropped.

## Display Label Hint

Visible key label for a binding in the active top context.

Fields:

- `KeyPositionId`: physical key position.
- `Label`: layout-specific label or fallback key identifier.
- `CommandId`: resolved command if the binding emits a command.
- `Outcome`: concise display text for non-command outcomes.

Validation:

- Only bindings whose `ModeId` matches the top context are eligible.
- State-specific bindings are included only when their `WhenState` matches the top context state or no state is required.
- Labels are capped according to display options.

## Display Pending Sequence

Hint for in-progress input.

Fields:

- `Chords`: pending chords.
- `StartedAt`: sequence start time.
- `IsTimed`: true when disambiguation timeout applies.
- `TimeoutMilliseconds`: configured timeout when available.

Validation:

- Present only when runtime has `PendingSequence` and options allow pending sequence display.

## Display Recent Command

Short-term feedback for the latest resolved command.

Fields:

- `CommandId`: command identifier.
- `DisplayName`: registry display name when known.
- `SourceKey`: key that resolved the command.

Validation:

- Selected from recent `CommandResolved` effects supplied by the caller.
- Most recent command wins.

## Display Diagnostic

Most recent actionable diagnostic.

Fields:

- `Severity`: warning, error, or fatal preferred; informational used only when no stronger actionable diagnostic exists and the caller opts in.
- `Code`: diagnostic code.
- `Message`: user-facing actionable message.
- `ModeId`, `CommandId`, `KeyPositionId`: optional context.

Validation:

- At most one diagnostic is displayed.
- The selected diagnostic must be the newest eligible diagnostic from runtime diagnostics or recent effects.

## State Transitions

- Hidden -> visible compact/expanded: model becomes visible and scene renders current runtime state.
- Compact -> expanded: omitted hints/details become available when present.
- Layout change: layout summary updates and previous invalid-layout partial flag clears if the new layout exists.
- Stack push/pop: stack entries and top context update in order.
- Stateful mode change: active state updates without losing persistent context.
- Held mode key-up/focus-loss: temporary held entries are removed and any recovery diagnostic can become the displayed diagnostic.
- Pending sequence start/timeout: pending hint appears or clears; timeout diagnostic may become the displayed diagnostic.
- Command resolved: recent command hint updates from effects.
