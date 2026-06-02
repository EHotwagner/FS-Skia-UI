# Contract: `FS.Skia.UI.Input` package public surface

**Type:** F# library package public contract (the `.fsi` is the sole surface declaration —
Constitution Principle II). This contract is a **namespace-rename relocation** of the existing
`src/Lib/KeyboardInput.fsi`; it introduces no new public surface.

## Package identity

- **Package id:** `FS.Skia.UI.Input`
- **Project:** `src/Input/Input.fsproj`
- **Namespace:** `FS.Skia.UI.Input`
- **Public module:** `KeyboardInput`
- **Package dependencies:** `FS.Skia.UI.Scene`, `FS.Skia.UI.SkiaViewer` (no new external deps)
- **Consumed via:** `open FS.Skia.UI.Input`

## Surface (authoritative source = `src/Input/KeyboardInput.fsi`)

The public surface equals `src/Lib/KeyboardInput.fsi` verbatim except `namespace FS.Skia.UI` →
`namespace FS.Skia.UI.Input`. It comprises 38 public types and the `KeyboardInput` module:

- Identifier aliases: `CommandId`, `ModeId`, `StateId`, `LayoutId`, `KeyPositionId`,
  `InputEventId`.
- Diagnostics: `InputSeverity`, `InputDiagnosticCode`, `InputDiagnostic`.
- Commands & modes: `CommandDefinition`, `CommandRegistry`, `ModeKind`, `ModeDefinition`,
  `CommandIntent`.
- Layout & keys: `Hand`, `Finger`, `KeyPosition`, `LayoutProfile`, `KeyChord`.
- Bindings: `BindingOutcome`, `BindingDefinition`, `DisambiguationPolicy`.
- Bigram analysis: `BigramWeight`, `BigramProfile`, `BigramRiskKind`, `BigramRisk`,
  `BigramSuggestion`, `BigramReport`.
- Display options: `DisplayOptions`, `KeyboardStateDisplayVisibility`,
  `KeyboardStateDisplayDensity`, `KeyboardStateDisplayOptions`.
- Configuration & model: `InputConfiguration`, `CanonicalInputModel`, `ModeFrame`,
  `InputEvent`, `PendingSequence`, `InputRuntime`.
- Messages, effects, outputs: `InputMsg`, `LayoutStateView`, `ResolvedCommand`, `InputEffect`,
  `CommandPlanStatus`, `CommandPlan`.
- State-display projection: `KeyboardStateDisplayLayout`, `KeyboardStateDisplayContextKind`,
  `KeyboardStateDisplayStackEntry`, `KeyboardStateDisplayLabel`,
  `KeyboardStateDisplayPendingSequence`, `KeyboardStateDisplayRecentCommand`,
  `KeyboardStateDisplayDiagnostic`, `KeyboardStateDisplayOmission`, `KeyboardStateDisplayModel`.
- `module KeyboardInput` — the init/update/parse/project functions.

## Compatibility / migration note

Consumers of the old monolith surface migrate `open FS.Skia.UI` (for keyboard input) →
`open FS.Skia.UI.Input`, and replace the `FS.Skia.UI` package reference with `FS.Skia.UI.Input`.
No member is renamed, removed, or retyped. (A full V2→V3 surface-mapping table is Stage 5.)

## Contract tests

- `tests/Input.Tests` exercises this surface through `open FS.Skia.UI.Input` (Principle I/III),
  carrying the migrated `KeyboardInputTests.fs` assertions.
- `PerPackageSurfaceDiff` enforces the surface against `readiness/per-package-surface/FS.Skia.UI.Input.fsi.txt`.
