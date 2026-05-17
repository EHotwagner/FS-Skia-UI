# Control Runtime Evidence

Status: setup placeholder, awaiting foundation and US1 implementation.

## Required Evidence

- Product-owned `ControlRuntime` model, messages, effects, diagnostics, pure
  `init` and `update`.
- Transition tests for focus, hover, pressed controls, caret and selection,
  composition, drag lifecycle, focus loss, removed controls, cancelled
  interactions, and stale targets.
- FSI or packed-package evidence through public entry points.

## Setup Observation

No `src/Controls/ControlRuntime.fsi` or `src/Controls/ControlRuntime.fs` file
exists at setup time.

## Red Test Evidence

- `readiness/logs/t011-controlruntime-red.txt`: fails because the Controls
  `ControlRuntime` contract and transient recovery surface do not exist yet.

## Foundation Evidence

- `readiness/logs/t014-controls-contracts.txt`: Controls package builds after
  adding the `ControlRuntime` contract and pure update implementation.
- `readiness/logs/t030-controlruntime.txt`: cancellation clears caret,
  selection, composition, drag state, and emits a named cancellation effect.

## US1 Sample Evidence

- `readiness/logs/t034-controlsgallery-contract-smoke.txt`: ControlsGallery
  stores `ControlRuntimeModel` in the sample product model, routes
  `FocusControl` through the public update path, interprets the emitted
  `FocusChanged` effect through `Controls.Elmish`, and renders the focused
  control through the public Controls surface.

## US1 Readiness Capture

| Evidence | Path | Verdict |
|----------|------|---------|
| Pure runtime cancellation and cleanup test | `readiness/logs/t030-controlruntime.txt` | PASS |
| Public FSI focus and stale-target recovery | `readiness/logs/t033-controls-fsi.txt` | PASS |
| Product-style sample focus routing through adapter | `readiness/logs/t034-controlsgallery-contract-smoke.txt` | PASS |

ControlRuntime remains product-owned in the sample model. No persistent
business values are stored in `ControlRuntimeModel`; the sample keeps `Name`,
`CanSave`, collection data, and selected tab in its product model.

## T075 Runtime Check

| Evidence | Path | Verdict |
|----------|------|---------|
| Feature-specific interaction target | `readiness/logs/t075-controls-interaction-check.txt` | PASS |
| Direct serial Lib runtime tests | `readiness/logs/t075-lib-tests-direct.txt` | PASS |
| Aggregate `Verify` attempt | `readiness/logs/t075-verify.txt` | ENVIRONMENT FAIL |

`ControlsRuntimeCheck` is not a declared target in the current target graph.
The available split target, `ControlsInteractionCheck`, passed and exercises
the Controls runtime and adapter interaction contract. The aggregate `Verify`
attempt failed while VSTest was creating the `Lib.Tests` testhost socket
engine under local memory/process pressure; the same `Lib.Tests` suite passed
as a direct serial command.
