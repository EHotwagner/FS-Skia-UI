# Keyboard Input Elmish Evidence

Status: setup placeholder, awaiting US1 implementation.

## Required Evidence

- Product model stores `FS.Skia.UI.KeyboardInput` runtime state.
- Key down/up messages route through public `update`.
- Emitted effects are asserted and interpreted through the adapter boundary.
- Focus loss clears pressed keys and temporary held layers while preserving
  persistent mode state.
- State display renders from current runtime state without hidden host state.

## US1 T033 Evidence

- `readiness/logs/t033-keyboardinput-fsi.txt`: FSI exercises the public
  `FS.Skia.UI.KeyboardInput` `Keyboard.init`, `Keyboard.update`, focus-loss
  recovery, emitted effects, and state display.
- `readiness/logs/t033-controls-elmish-fsi.txt`: FSI exercises adapter
  interpretation of `KeyboardEffect.CommandResolved` into a product message
  command and `ControlRuntimeEffect.FocusChanged` into runtime/product
  messages.
- `readiness/logs/t033-keyboard-runtime-definition-scan.txt`: the rich
  keyboard runtime types are defined only by the dedicated KeyboardInput
  package, not duplicated in Controls or the adapter.

## US1 Readiness Capture

| Evidence | Path | Verdict |
|----------|------|---------|
| Adapter FSI interpreting keyboard/control effects | `readiness/logs/t033-controls-elmish-fsi.txt` | PASS |
| ControlsGallery combined runtime and adapter sample | `readiness/logs/t034-controlsgallery-contract-smoke.txt` | PASS |
| KeyboardInputGallery state-display and adapter sample | `readiness/logs/t034-keyboardinputgallery-contract-smoke.txt` | PASS |

Both US1 samples store runtime state in product models and route emitted
effects through `ControlsElmish.interpretKeyboardEffect` or
`ControlsElmish.interpretControlEffect`.
