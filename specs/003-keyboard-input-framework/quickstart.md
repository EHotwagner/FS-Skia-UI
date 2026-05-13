# Quickstart: Keyboard Input Framework

## Goal

Validate the public API from FSI, semantic tests, sample smoke, and readiness evidence before implementation work is accepted.

## Expected Files

```text
src/Lib/KeyboardInput.fsi
src/Lib/KeyboardInput.fs
tests/Lib.Tests/KeyboardInputTests.fs
scripts/input-prelude.fsx
samples/KeyboardInputGallery/Program.fs
specs/003-keyboard-input-framework/readiness/sample-configs/modal-input.yaml
```

## Sample YAML

```yaml
version: 1
default_layout: qwerty
default_mode: selection

layouts:
  - id: qwerty
    display_name: QWERTY
    positions:
      - { id: home-left-index, hand: left, finger: index, row: 0, column: 3, label: f }
      - { id: home-right-index, hand: right, finger: index, row: 0, column: 6, label: j }
      - { id: thumb-space, hand: either, finger: thumb, row: 1, column: 5, label: space }
      - { id: escape, hand: either, finger: unknown, row: 2, column: 0, label: escape }

modes:
  - id: selection
    kind: stateful
    default_state: character
    states: [character, word, line]
  - id: space
    kind: popup
    cancel_keys: [escape]
  - id: copy
    kind: temporary_held

bindings:
  - mode: selection
    key: home-right-index
    command: move.right
  - mode: selection
    key: thumb-space
    push_popup: space
  - mode: selection
    key: home-left-index
    push_temporary: copy
  - mode: copy
    key: home-right-index
    command: edit.copy

bigram_profile:
  suggestion_limit: 20
  weights:
    - first: edit.copy
      second: move.right
      weight: 0.8

display:
  show_layout_state: true
  show_pending_sequence: true
```

## FSI Transcript Shape

Run after `.fsi` is drafted:

```bash
dotnet fsi scripts/input-prelude.fsx
```

The prelude should:

1. Register `move.right`, `edit.copy`, and `edit.delete`.
2. Parse the sample YAML text.
3. Validate it against the command registry.
4. Initialize the runtime with the `qwerty` layout.
5. Press the popup key and verify `space` is pushed onto the mode stack.
6. Resolve one popup command and verify the stack restores to `selection`.
7. Hold the temporary copy key and verify it is popped on key release.
8. Run `analyzeBigrams` and verify the configured keymap is unchanged.

## Test Commands

```bash
dotnet build src/Lib/Lib.fsproj
dotnet fsi scripts/input-prelude.fsx
dotnet test tests/Lib.Tests/Lib.Tests.fsproj
dotnet test tests/Package.Tests/Package.Tests.fsproj
dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj
dotnet run --project samples/KeyboardInputGallery/KeyboardInputGallery.fsproj -- --contract-smoke
dotnet run --project samples/KeyboardInputGallery/KeyboardInputGallery.fsproj
dotnet test
```

## Required Test Coverage

- Valid YAML parses into `InputConfiguration`.
- Invalid YAML returns `InvalidYaml`.
- Duplicate binding returns `DuplicateBinding`.
- Unknown mode returns `UnknownMode`.
- Unregistered command returns `UnknownCommand`.
- Host-action-like YAML returns `HostActionRejected`.
- Stateful mode without default state returns `InvalidModeState`.
- Popup mode push/pop restores prior state.
- Temporary held mode pushes on key-down and pops on key-up.
- Focus loss clears pressed keys and pops temporary held modes.
- Ambiguous sequence emits a diagnostic or waits according to timeout policy.
- Replay produces the same final runtime and resolved command effects.
- Layout-state display data includes active mode stack, held modes, pending sequence, active layout, and labels.
- Bigram report lists weighted command pairs and does not mutate configuration.

## Sample Smoke

Run the sample gallery after implementation:

```bash
dotnet run --project samples/KeyboardInputGallery/KeyboardInputGallery.fsproj -- --contract-smoke
```

The smoke output is captured in `readiness/sample-smoke/keyboard-input-gallery.txt`. It proves the author-facing sample loads YAML, validates the command registry, initializes the runtime, replays popup and temporary held-mode input, exposes layout-state data, and runs bigram analysis.

The live Skia app starts on `colemacs-dh` / `Colemak-DH`, captures window `KeyDown` / `KeyUp` events, renders the active layout overlay in the scene, and documents layout switching for QWERTY, Dvorak, Colemak-DH, Workman, and a custom Symbols layout.

## Readiness Evidence

- FSI transcript: `readiness/fsi/input-prelude.txt`
- Replay fixture: `readiness/input-replay/keyboard-modal-stack.json`
- YAML fixtures: `readiness/sample-configs/*.yaml`
- Surface baseline: `readiness/surface-baselines/FS.Skia.UI.txt`
- Performance evidence: `readiness/performance/input-performance.txt`
