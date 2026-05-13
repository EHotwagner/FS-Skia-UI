# Quickstart: Keyboard State Display Element

## Goal

Use the standard keyboard state display with the existing keyboard input runtime, first as pure display data and then as a rendered Skia `Scene`.

## Example Usage

```fsharp
open FS.Skia.UI

let options =
    { KeyboardInput.compactStateDisplayOptions with
        ShowKeyLabels = true
        ShowPendingSequence = true
        ShowRecentCommand = true
        ShowDiagnostic = true }

let displayModel =
    KeyboardInput.keyboardStateDisplay options lastInputEffects runtime

let overlayScene =
    KeyboardInput.renderKeyboardStateDisplayAt (48.0, 152.0) options lastInputEffects runtime
```

Applications that need a richer inspector can switch to expanded options:

```fsharp
let expanded =
    KeyboardInput.keyboardStateDisplay
        KeyboardInput.expandedStateDisplayOptions
        lastInputEffects
        runtime
```

Applications that want to keep keyboard input active but hide the element can use hidden visibility:

```fsharp
let hiddenOptions =
    { KeyboardInput.compactStateDisplayOptions with
        Visibility = KeyboardStateDisplayHidden }

let scene = KeyboardInput.renderKeyboardStateDisplay hiddenOptions [] runtime
```

## Validation Steps

1. Sketch the public additions in `src/Lib/KeyboardInput.fsi`.
2. Update `scripts/input-prelude.fsx` or add a feature transcript that constructs compact and expanded display models through the packed/public surface.
3. Add semantic tests in `tests/Lib.Tests/KeyboardInputTests.fs` for:
   - active layout and top context display,
   - permanent/stateful/popup/held stack entries,
   - compact omission rules,
   - expanded full details,
   - top-context-only labels,
   - pending sequence and timeout state,
   - most recent resolved command,
   - most recent actionable diagnostic,
   - partial invalid-layout rendering,
   - hidden mode empty scene.
4. Update `samples/KeyboardInputGallery/Program.fs` to consume `renderKeyboardStateDisplayAt`.
5. Refresh package surface baselines.
6. Run:

```bash
dotnet test
dotnet fsi scripts/input-prelude.fsx
dotnet run --project samples/KeyboardInputGallery/KeyboardInputGallery.fsproj
```

## Expected Results

- Compact display identifies active layout, active top context, condensed stack, and state without text overlap.
- Expanded display exposes full stack details, active top-context labels, pending sequence, recent command, and the latest actionable diagnostic.
- The gallery can toggle or display the standard element without custom state visualization logic.
- Tests assert structured display data instead of relying only on rendered scene inspection.
