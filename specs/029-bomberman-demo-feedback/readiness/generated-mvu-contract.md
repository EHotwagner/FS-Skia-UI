# Generated MVU Contract

Task: T007
Captured: 2026-05-29T11:48:32+02:00

## App-Owned Contract

Generated game apps must own:

- `Model`: pure game state.
- `Msg`: user, tick, and internal transitions.
- `Effect`: app-owned requests, not viewer/native/file effects.
- `init`: returns initial `Model` plus app-owned effects.
- `update`: pure transition from `Msg` and `Model` to next `Model` plus app-owned effects.
- `view`: pure `Model -> SceneNode`.
- `mapKey`: viewer key plus pressed/released state to optional app `Msg`.
- `tick`: elapsed time to optional app `Msg`.

## Host Boundary

The generated host owns:

- mapping viewer key events to `mapKey`
- mapping elapsed frame time to `tick`
- rendering through `view`
- adapting app-owned effects into viewer effects
- executing viewer launch, filesystem, native, screenshot, and process work

## Default Launch

`runApp` or the generated executable default must attempt persistent interactive launch. Bounded evidence launch must remain explicit, for example `runAppEvidence` or a named evidence command.
