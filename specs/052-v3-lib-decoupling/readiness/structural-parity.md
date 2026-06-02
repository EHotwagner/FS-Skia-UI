# Structural parity

command: `git diff --cached -M --summary | grep rename`
artifact path: this file.
failure class: StructuralParity.
next action: none — the rename similarity confirms a behaviour-neutral move.

## Rename detection (git -M)

```
rename src/{Lib => Input}/KeyboardInput.fs (99%)
rename src/{Lib => Input}/KeyboardInput.fsi (99%)
rename tests/{Lib.Tests => Input.Tests}/KeyboardInputTests.fs (99%)
```

- The rich input runtime moved `src/Lib` → `src/Input` (package `FS.Skia.UI.Input`) at **99%**
  similarity — the only change is the `namespace` line (`FS.Skia.UI` → `FS.Skia.UI.Input`). Zero
  `val`/`type`/field/case added, removed, or retyped (mirrors the Stage-2 rename discipline).
- The migrated test moved `tests/Lib.Tests` → `tests/Input.Tests` at **99%** — the only change is its
  `open` (`FS.Skia.UI` → `FS.Skia.UI.Input`); every fixture and assertion is preserved.

## Behavioural parity

- `dotnet test tests/Input.Tests/Input.Tests.fsproj` → 12 tests, 12 passed, identical to the pre-move
  `KeyboardInputTests` (binding/mode/sequence semantics, command intents, diagnostics, state-display
  projection). The relocated runtime behaves identically; this is a change of home, not of semantics
  (FR-002, SC-004).
- The `FS.Skia.UI.Input` per-package baseline equals the post-move `.fsi` modulo the namespace line
  (`PerPackageSurfaceDiff` clean across nine packages).
