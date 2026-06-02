# Data Model: V3 Stage 3–4 Residual

This feature **relocates** an existing model without changing it. The "entities" below are the
governance/structural artifacts that move, plus the preserved runtime model (carried verbatim).

## Structural entities (what moves)

| Entity | Before | After |
|---|---|---|
| Rich keyboard-input module | `src/Lib/KeyboardInput.fs(i)`, namespace `FS.Skia.UI` | `src/Input/KeyboardInput.fs(i)`, namespace `FS.Skia.UI.Input`, package `FS.Skia.UI.Input` |
| `Parity` evidence helper | `src/Lib/Library.fs(i)` (`ParityStatus`/`EvidenceType`/`ParityEvidenceItem`/`ParityReport` + `Parity` module) | removed once `Parity.Tests` retires |
| Rich keyboard-input tests | `tests/Lib.Tests/KeyboardInputTests.fs` (refs `Lib`) | `tests/Input.Tests/KeyboardInputTests.fs` (refs `FS.Skia.UI.Input`) |
| Parity bridge | `tests/Parity.Tests` (refs `Lib`) | retired; valuable assertions → `SkiaViewer.Tests`/`Scene.Tests` |
| `InteractiveViewer` refs | `Scene` + `SkiaViewer` + `Lib` + `FS.Skia.UI` pkg | `Scene` + `SkiaViewer` + `FS.Skia.UI.Input` (pkg on the packed path) |
| `Package.Tests` refs | conditional `Lib.fsproj` | none to `Lib` |
| Per-package surface baseline | `FS.Skia.UI.fsi.txt` only (incl. rich KB) | `+ FS.Skia.UI.Input.fsi.txt` (new); `FS.Skia.UI.fsi.txt` shrinks |

## Preserved runtime model (carried verbatim — no behaviour change)

The rich input runtime's public surface (38 types + the `KeyboardInput` module) moves unchanged.
Key Elmish-shaped pieces (relevant to the MVU/effect-boundary governance prompt):

- **Model:** `InputRuntime`, `CanonicalInputModel`, `ModeFrame`, `PendingSequence`,
  `InputConfiguration` (with `CommandRegistry`, `ModeDefinition`, `BindingDefinition`,
  `LayoutProfile`, `BigramProfile`, `DisplayOptions`).
- **Msg:** `InputMsg`, `InputEvent` (`InputEventId = Guid`).
- **Effect:** `InputEffect`; outputs `ResolvedCommand`, `CommandPlan`/`CommandPlanStatus`.
- **Diagnostics:** `InputDiagnostic`, `InputDiagnosticCode`, `InputSeverity` — preserved verbatim.
- **State-display projection:** `KeyboardStateDisplay*` family (visibility/density/layout/labels/
  pending-sequence/recent-command/diagnostic/omission/model).
- **Analysis:** `BigramWeight`/`BigramProfile`/`BigramRisk`/`BigramSuggestion`/`BigramReport`.
- **Identifiers:** `CommandId`/`ModeId`/`StateId`/`LayoutId`/`KeyPositionId` (string aliases),
  `KeyChord`, `KeyPosition`, `Hand`, `Finger`, `BindingOutcome`, `DisambiguationPolicy`,
  `CommandIntent`, `CommandDefinition`, `ModeKind`/`ModeDefinition`.

**Invariant:** every `val`/`type`/field/case in `src/Lib/KeyboardInput.fsi` appears, unchanged,
in `src/Input/KeyboardInput.fsi` (only the `namespace` line differs). Verified by diffing the two
`.fsi` files modulo the namespace line.

## Validation rules

- New package per-package baseline `FS.Skia.UI.Input.fsi.txt` must equal the normalized
  post-move `.fsi`; `PerPackageSurfaceDiff` clean.
- `FS.Skia.UI.fsi.txt` (monolith) loses exactly the rich-KB lines (and the `Parity` lines once
  the helper retires); no other monolith surface change.
- No project outside `src/Lib` references `Lib.fsproj` or `FS.Skia.UI` (monolith pkg).
