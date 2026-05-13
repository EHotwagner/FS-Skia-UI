# Implementation Plan: Keyboard Input Framework

**Branch**: `003-keyboard-input-framework` | **Date**: 2026-05-13 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/003-keyboard-input-framework/spec.md`

## Summary

Add a keyboard-centric input framework to the core `FS.Skia.UI` package. The framework provides a validated YAML configuration model, application-registered command identifiers, stack-based mode composition, stateful selection-like modes, one-shot popup modes, temporary held modes, positional keymaps, deterministic input replay, diagnostics, optional layout-state display data, and analysis-only bigram ergonomics reports. Command grammar and declarative command planning remain optional advanced concepts and are represented only as opt-in contracts for future extension.

The implementation approach is a pure typed input runtime with an Elmish/MVU-shaped boundary:

- `.fsi` first for the public `FS.Skia.UI.KeyboardInput` surface.
- Pure `init`/`update` style functions for key events, mode stack transitions, diagnostics, and replay.
- YAML parsing converts text into typed values and rejects unregistered commands before activation.
- Applications continue to own domain state; the input framework emits resolved command identifiers and diagnostics only.
- Rendering/layout display is exposed as data so host applications decide how to draw it.

## Technical Context

**Language/Version**: F# on .NET `net10.0` with `LangVersion=latest`  
**Primary Dependencies**: Existing `FS.Skia.UI` dependencies plus `YamlDotNet` `17.1.0` for YAML parsing in the core package; `Fable.Elmish` `4.2.0` remains the public MVU dependency  
**Storage**: Filesystem only for YAML examples, replay transcripts, readiness evidence, surface-area baselines, and package artifacts; no runtime persistence owned by the framework  
**Testing**: `dotnet test`; Expecto semantic tests; FSI/prelude transcript for public API shape; package and surface-area baseline tests; sample smoke tests where practical  
**Target Platform**: Windows and Linux desktop, aligned with existing Vulkan-only viewer support  
**Project Type**: Packable F# library suite plus sample applications  
**Performance Goals**: 95% of key press/release events resolve in under 16 ms in automated tests; replay of 10,000 recorded input events completes in under 1 second on a development workstation; bigram report for 500 bindings and 2,000 weighted command pairs completes in under 1 second  
**Constraints**: Core feature is pure and host-agnostic; YAML is declarative and cannot execute host actions; configuration may reference only registered command identifiers; mode composition is stack-based; bigram optimization is analysis-only; application/domain state remains outside the framework; every public module requires `.fsi`; no top-level visibility modifiers in `.fs`  
**Scale/Scope**: One new core public module, one YAML parser dependency, one sample gallery, one FSI prelude, contract tests for configuration validation, state transitions, replay, diagnostics, layout state, and bigram reports

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Principle I - Spec -> FSI -> Semantic Tests -> Implementation**: PASS. This is Tier 1 because it adds public API and a dependency. The plan requires `KeyboardInput.fsi`, prelude transcript, semantic tests, surface baseline updates, and then implementation.
- **Principle II - Visibility Lives in `.fsi`**: PASS. The new public surface is isolated in `src/Lib/KeyboardInput.fsi`; `KeyboardInput.fs` must not use top-level `private`, `internal`, or `public` modifiers.
- **Principle III - Idiomatic Simplicity**: PASS. Contracts use records, discriminated unions, modules, and pure functions. Mutable code is not expected outside local parsing/report loops; any measured hot-path mutation must be commented.
- **Principle IV - Elmish/MVU Boundary**: PASS. The runtime is stateful, so the public surface exposes a model, messages, pure transition function, and effects/diagnostic outputs. File I/O is kept at the host edge.
- **Principle V - Synthetic Evidence Disclosure**: PASS. No synthetic evidence is planned. Sample fixtures are real YAML/replay examples checked into the repository.
- **Principle VI - Test Evidence Is Mandatory**: PASS. Tests must cover failures before implementation completion: invalid YAML, unregistered command rejection, stack restoration, held-mode release, lost key-up recovery, replay determinism, and bigram report stability.
- **Principle VII - Observability and Safe Failure**: PASS. Diagnostics are first-class entities and include invalid config, ambiguous input, unknown command, dropped/stale event, focus-loss cleanup, and unsatisfied optional command intent.
- **Change Classification**: PASS. Tier 1 contracted change: new public API surface and new dependency.
- **Engineering Constraints**: PASS. F#/.NET only; net10.0; `.fsi` surface; package output remains local NuGet; Vulkan viewer constraints are unaffected.

## Project Structure

### Documentation (this feature)

```text
specs/003-keyboard-input-framework/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── public-api.md
├── checklists/
│   └── requirements.md
├── tasks.md
└── tasks.deps.yml
```

### Source Code (repository root)

```text
src/
└── Lib/
    ├── KeyboardInput.fsi
    ├── KeyboardInput.fs
    ├── Library.fsi
    ├── Library.fs
    └── Lib.fsproj

samples/
└── KeyboardInputGallery/
    ├── KeyboardInputGallery.fsproj
    └── Program.fs

tests/
├── Lib.Tests/
│   ├── KeyboardInputTests.fs
│   ├── Program.fs
│   └── Lib.Tests.fsproj
├── Package.Tests/
│   └── SurfaceAreaTests.fs
└── Smoke.Tests/
    └── Tests.fs

scripts/
└── input-prelude.fsx

specs/003-keyboard-input-framework/readiness/
├── input-replay/
│   └── keyboard-modal-stack.json
├── sample-configs/
│   ├── modal-input.yaml
│   ├── invalid-duplicate-binding.yaml
│   ├── invalid-unregistered-command.yaml
│   └── invalid-host-action.yaml
└── surface-baselines/
    └── FS.Skia.UI.txt
```

**Structure Decision**: Add the framework to the existing `FS.Skia.UI` core package because keyboard input is a cross-cutting viewer capability and must be available without depending on Charts or Layout. Keep Charts/Layout unchanged. Add a sample gallery to demonstrate optional layout-state display and sample YAML, while tests remain in `Lib.Tests` and package baseline coverage remains in `Package.Tests`.

## Complexity Tracking

No constitution violations require justification. `YamlDotNet` is a new dependency, but it is required by the product requirement for YAML configuration and is isolated behind typed public contracts.

## Phase 0: Research

See [research.md](./research.md). Decisions cover the core package boundary, YamlDotNet dependency, declarative YAML trust boundary, stack-based mode composition, MVU runtime shape, physical key model, analysis-only bigram reports, replay/diagnostic evidence, and optional advanced command intent.

## Phase 1: Design & Contracts

See [data-model.md](./data-model.md), [contracts/public-api.md](./contracts/public-api.md), and [quickstart.md](./quickstart.md).

Design summary:

- `KeyboardInput.fsi` owns all public input entities and pure runtime functions.
- Applications provide a `CommandRegistry` before YAML activation.
- YAML parses into an `InputConfiguration`, validates against the registry, and produces a `CanonicalInputModel`.
- `KeyboardInput.init` creates an `InputRuntime` with an explicit base stateful mode and active layout.
- `KeyboardInput.update` consumes input messages and returns a new runtime plus resolved commands, diagnostics, and optional layout-state updates.
- Mode composition is a stack. Popup and held modes push; completion, cancellation, key release, or focus loss pops.
- Bigram reports are non-mutating. They rank risks and suggestions but never rewrite YAML.
- Optional command intent has data contracts only; grammar parsing and execution engines are out of v1 scope.

## Constitution Check - Post Design

- **Principle I**: PASS. `contracts/public-api.md` defines the intended `.fsi` public surface before implementation.
- **Principle II**: PASS. The design adds exactly one public module with a companion `.fsi`; surface baseline updates are required.
- **Principle III**: PASS. The model uses plain F# records, discriminated unions, and modules. No advanced language features are required.
- **Principle IV**: PASS. The stateful runtime exposes `InputRuntime`, `InputMsg`, `InputEffect`, `init`, and `update`; effects are values and host I/O stays outside the core transition.
- **Principle V**: PASS. Planned evidence uses checked-in real YAML fixtures and replay transcripts, not mocks.
- **Principle VI**: PASS. Each user story maps to semantic tests and quickstart verification.
- **Principle VII**: PASS. Diagnostics and failure reports are public contract data.
- **Engineering Constraints**: PASS. Dependency addition is justified and pinned; package and baseline work are explicit.
