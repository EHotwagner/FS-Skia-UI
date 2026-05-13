# Research: Keyboard Input Framework

## Decision: Implement in the Core Package

**Decision**: Add `FS.Skia.UI.KeyboardInput` to `src/Lib` rather than creating a separate package.

**Rationale**: Keyboard input is a core viewer and application-integration concern. Consumers should not need Charts or Layout packages to resolve key events, validate input configuration, or display layout state. Keeping the feature in core also preserves the existing Elmish viewer integration path.

**Alternatives considered**:

- Separate `FS.Skia.UI.Input` package: rejected for v1 because it would add package/project overhead before there is a proven independent lifecycle.
- Sample-only helper: rejected because the spec requires a reusable framework and public contracts.

## Decision: Use YamlDotNet 17.1.0 for YAML Parsing

**Decision**: Add `YamlDotNet` `17.1.0` as a pinned dependency in `src/Lib/Lib.fsproj`.

**Rationale**: YAML configuration is a product requirement. `YamlDotNet` is the established .NET YAML parser, supports .NET 10.0, and avoids implementing unsafe ad hoc parsing. The dependency is hidden behind typed records and validation; raw YAML objects are not part of the public runtime model.

**Alternatives considered**:

- Hand-written parser: rejected because YAML edge cases are easy to mishandle and would create unnecessary maintenance risk.
- JSON or TOML only: rejected because the spec explicitly requires YAML.
- Configuration-as-code only: rejected because the feature requires human-editable YAML.

## Decision: Declarative YAML Trust Boundary

**Decision**: YAML may reference only application-registered command identifiers and validated input policies. YAML must not define arbitrary host actions such as shell or process execution.

**Rationale**: Input configuration is user-editable and may be shared. Treating it as executable would create a security boundary problem and would conflict with the requirement that applications own domain behavior. A command registry makes validation deterministic and gives applications explicit control over what bindings can emit.

**Alternatives considered**:

- Built-in executable commands: rejected for v1 because it blurs ownership between the library and host application.
- Shell/process commands: rejected as unsafe for a reusable UI library.

## Decision: Stack-Based Mode Composition

**Decision**: Use an explicit mode stack. The base stateful mode remains underneath popup and temporary held modes. Closing or releasing the top mode restores the previous context.

**Rationale**: The stack model is deterministic, replayable, and matches the spec language around restoring prior context. It also gives a clear layout-state display: show stack from base to top.

**Alternatives considered**:

- Priority layers: rejected because simultaneous active modes make conflict resolution harder to test and explain.
- Replacement mode: rejected because popup/held modes would lose stateful context unless extra restoration rules were invented.

## Decision: Pure MVU Runtime Boundary

**Decision**: Model the runtime as `InputRuntime` plus `InputMsg` and a pure `update` function returning `InputRuntime * InputEffect list`.

**Rationale**: The constitution requires an MVU boundary for stateful workflows. The input framework is inherently stateful, but key resolution itself can be pure when the host supplies input events and configuration. This gives deterministic semantic tests and replay.

**Alternatives considered**:

- Callback-based mutable manager: rejected because it hides state transitions and makes replay difficult.
- Direct integration into `ViewerEvent` only: rejected because the framework must also support tests, samples, and host applications that want to resolve events before mapping to domain messages.

## Decision: Physical Key Positions Are First-Class

**Decision**: Bindings identify physical key positions and may include display labels resolved through a layout profile.

**Rationale**: Ki-inspired positional keymaps keep command identity stable across keyboard layouts. The layout profile separates physical command placement from user-facing key labels.

**Alternatives considered**:

- Label-only bindings: rejected because they become layout-specific and undermine positional ergonomics.
- Scan-code-only model: rejected for v1 because it is too host-specific for a public library contract.

## Decision: Bigram Optimization Is Analysis-Only in v1

**Decision**: Generate reports with scores, risks, and suggestions, but do not modify YAML or rewrite keymaps.

**Rationale**: Automatic optimization could surprise users and make examples harder to reason about. Analysis-only reports still satisfy the need for bigram optimization evidence while preserving author control.

**Alternatives considered**:

- Proposed YAML patches: deferred because it requires patch formatting and conflict handling.
- Automatic rewrite: rejected for v1 because it changes behavior without explicit user approval.

## Decision: Replay and Diagnostics Are Contract Data

**Decision**: Record input events and diagnostics as public typed values that can be serialized by hosts or tests.

**Rationale**: The referenced command-framework report emphasizes event logs and inspectable status. The input framework needs the same property for ambiguous sequences, lost key-up recovery, focus loss, invalid config, and replay determinism.

**Alternatives considered**:

- Logging-only diagnostics: rejected because tests and applications need structured data, not strings.
- No replay model: rejected because stack and timing behavior require deterministic evidence.

## Decision: Optional Command Intent Is Data-Only in v1

**Decision**: Define minimal opt-in data contracts for command intent, plan state, and failure reporting, but do not implement a command grammar or planner.

**Rationale**: The spec says command grammar is optional advanced functionality. Data contracts allow future work to align with the referenced stabilization report without forcing grammar parsing into the standard input schema.

**Alternatives considered**:

- Full command grammar now: rejected because it would expand scope and is not required for standard input.
- Exclude command intent entirely: rejected because the spec asks for advanced optional support and status/failure concepts.
