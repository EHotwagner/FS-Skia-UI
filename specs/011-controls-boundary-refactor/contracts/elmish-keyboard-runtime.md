# Contract: Elmish Adapter And Keyboard Runtime

## Purpose

Define how stateful keyboard/control workflows remain MVU-shaped while direct
Elmish command/program integration stays outside ordinary Controls
declarations.

## KeyboardInput Package Surface

- Package id: `FS.Skia.UI.KeyboardInput`
- Namespace: `FS.Skia.UI.KeyboardInput`
- Required concepts:
  - keyboard runtime model
  - keyboard messages for key down/up, focus loss, mode changes, sequence
    input, explicit reset, and diagnostics
  - keyboard effects for command resolution, key state change, mode
    transition, diagnostics, and host/control effect requests
  - pure `init` and `update`
  - state display data derived from current runtime state and recent effects
  - diagnostics for focus recovery, stale input, unsupported environment, and
    conflicting bindings

## Keyboard Runtime Rules

- Product models own the keyboard runtime state.
- Key down/up transitions update pressed keys and emit inspectable effects.
- Focus loss clears pressed keys and temporary held layers.
- Persistent mode state survives focus loss unless the product resets the
  runtime.
- State display is renderable from public runtime/effect values.
- Controls and the Elmish adapter consume this package surface instead of
  defining duplicate runtime types.

## Control Runtime Rules

- Product models own the control runtime state.
- Control runtime update is pure and returns next runtime plus effects and
  diagnostics.
- Control events produce product messages, control effects, or explicit host
  effect requests at the boundary.
- Persistent control values stay in product domain model fields.

## Elmish Adapter Surface

The adapter may live in `FS.Skia.UI.Controls.Elmish` or in
`FS.Skia.UI.Elmish`, but it must expose a distinct public surface for:

- interpreting keyboard effects into Elmish commands or product messages
- interpreting control runtime effects into Elmish commands or product
  messages
- wiring subscriptions for keyboard/control input where the product chooses
  Elmish program integration
- producing diagnostics without executing effects inside base Controls update
  logic

## Validation

- KeyboardInput semantic tests cover pressed keys, layout, mode stack,
  persistent mode state, temporary held layers, pending sequence, effects,
  focus-loss recovery, and state display.
- Controls tests cover control runtime update and stale/cancelled interaction
  recovery.
- Elmish adapter tests cover command/subscription/program integration.
- FSI transcripts exercise KeyboardInput runtime and adapter public contracts.
