# Keyboard State Display Implementation Notes

## Tier and Evidence Obligations

- Classification: Tier 1 contracted change.
- Public module: `FS.Skia.UI.KeyboardInput` in `src/Lib/KeyboardInput.fsi`.
- Dependency policy: no new package dependencies.
- Evidence obligations: public `.fsi` contract, semantic tests through public `KeyboardInput` functions, FSI transcript, sample smoke output, surface baseline artifact, performance evidence, and final evidence audit.

## Elmish/MVU Applicability

The library feature does not introduce a new `Model` / `Msg` / `Effect` contract. It projects display data from the existing pure `InputRuntime` and `InputEffect` boundary.

Validation therefore exercises existing public `KeyboardInput.init`, `KeyboardInput.update`, `KeyboardInput.replay`, and emitted `InputEffect` values. The gallery remains the host Elmish edge and consumes `renderKeyboardStateDisplayAt` as a pure scene value.

## Failure-Diagnostic Expectations

- Missing or invalid active layout returns `IsPartial = true`, preserves available stack/state data, and emits a display diagnostic with `UnknownLayout`.
- Unknown mode frames are represented as `DisplayUnknownContext` and make the display partial.
- Non-actionable `InputInfo` diagnostics are filtered from `KeyboardStateDisplayModel.Diagnostic`.
- Warning, error, and fatal diagnostics are actionable; the most recent actionable diagnostic is selected.
