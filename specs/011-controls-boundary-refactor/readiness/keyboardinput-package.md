# KeyboardInput Package Evidence

Status: setup placeholder, awaiting foundation and US1 implementation.

## Current Assets

- Contract: `src/KeyboardInput/KeyboardInput.fsi`
- Implementation: `src/KeyboardInput/KeyboardInput.fs`
- Tests: `tests/KeyboardInput.Tests/Tests.fs`
- Sample: `samples/KeyboardInputGallery/Program.fs`
- Surface baseline: `readiness/surface-baselines/FS.Skia.UI.KeyboardInput.txt`

## Required Evidence

- Runtime model, messages, effects, diagnostics, focus recovery, state display,
  and persistent mode state exercised through public `init` and `update`.
- Evidence that Controls and the Elmish adapter consume this package-owned
  runtime instead of duplicating input state.

## Red Test Evidence

- `readiness/logs/t010-keyboardinput-red.txt`: fails on missing layout, mode
  stack, persistent mode state, pending sequence, diagnostics, state display,
  focus recovery messages, and interpreter effect cases.

## Foundation Evidence

- `readiness/logs/t015-keyboardinput-contracts.txt`: KeyboardInput tests pass
  with runtime model, messages, effects, `init`, `update`, and state display.
- `readiness/logs/t031-keyboardinput-runtime.txt`: focus-loss recovery clears
  pressed keys and temporary modes, preserves persistent mode state, and emits
  a recovery diagnostic.

## US1 Sample Evidence

- `readiness/logs/t034-keyboardinputgallery-contract-smoke.txt`:
  KeyboardInputGallery stores `KeyboardModel` in the sample model, routes
  `KeyDown`, temporary mode, and `FocusLost` messages through
  `FS.Skia.UI.KeyboardInput.Keyboard.update`, prints state-display evidence,
  and interprets emitted effects through `Controls.Elmish`.
- `readiness/logs/t034-controlsgallery-contract-smoke.txt`: ControlsGallery
  combines the KeyboardInput runtime with Controls records and adapter wiring
  in one product-style sample.

## US1 Readiness Capture

| Evidence | Path | Verdict |
|----------|------|---------|
| KeyboardInput runtime tests | `readiness/logs/t031-keyboardinput-runtime.txt` | PASS |
| Public FSI runtime/effect/state-display exercise | `readiness/logs/t033-keyboardinput-fsi.txt` | PASS |
| Keyboard runtime definition ownership scan | `readiness/logs/t033-keyboard-runtime-definition-scan.txt` | PASS |
| KeyboardInputGallery package-owned sample smoke | `readiness/logs/t034-keyboardinputgallery-contract-smoke.txt` | PASS |

The rich `KeyboardModel`, `KeyboardMsg`, and `KeyboardEffect` definitions are
owned by `src/KeyboardInput/`; Controls and Controls.Elmish consume them rather
than declaring duplicate runtime types.

## T075 Runtime Check

| Evidence | Path | Verdict |
|----------|------|---------|
| Feature-specific interaction target | `readiness/logs/t075-controls-interaction-check.txt` | PASS |
| Direct serial Lib runtime tests | `readiness/logs/t075-lib-tests-direct.txt` | PASS |
| Aggregate `Verify` attempt | `readiness/logs/t075-verify.txt` | ENVIRONMENT FAIL |

`KeyboardInputCheck` is not a declared split target in the current target
graph. KeyboardInput package ownership remains covered by the direct
KeyboardInput runtime tests, public FSI transcript evidence, sample smoke
evidence, and the passing `ControlsInteractionCheck` target. The aggregate
`Verify` failure is recorded as a local VSTest testhost startup
`OutOfMemoryException`, not a KeyboardInput assertion failure.
