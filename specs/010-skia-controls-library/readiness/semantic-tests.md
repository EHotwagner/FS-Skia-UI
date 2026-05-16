# Semantic Tests

## Verdict

PASS for in-repo semantic and MVU evidence.

## Commands

- `dotnet test tests/Controls.Tests/Controls.Tests.fsproj -v:minimal`
- `./fake.sh build -t Dev`
- `./fake.sh build -t FsiTranscripts`
- `./fake.sh build -t SampleContractSmoke`

## Coverage

- `ControlsSemanticTests` verifies a representative Elmish-style view function
  renders through the public `Control.render` boundary with no diagnostics.
- Model-owned state is verified by re-evaluating the view with changed count,
  text, validation, and enabled values.
- `ControlsTextInputTests` verifies `TextInput.init` and pure `TextInput.update`
  transitions. Committed text remains model-owned until `Commit`, and effects
  such as `CommitText` and `RequestClipboardText` are explicit.
- `samples/ControlsGallery/Program.fs` defines `Model`, `Msg`, `Effect`, `init`,
  pure `update`, `controlView`, and the viewer interpreter boundary.
- `template/base/src/Product/Program.fs` defines the generated product-owned
  Controls example view, and `template/base/tests/Product.Tests/Tests.fs`
  verifies it.

## MVU Applicability

Controls authoring, text input, large collection viewport updates, generated
product examples, and the reference gallery all use model-owned state and
message-oriented events. Host/render/clipboard/environment behavior is carried
as effects or diagnostics rather than hidden durable control state.

## Unsupported Scope

Rich text editing, platform-native widgets, formal accessibility certification,
designer tooling, new renderer backends, and release publishing automation are
not part of this feature.
