# Implementation Plan: Keyboard State Display Element

**Branch**: `004-keyboard-state-display` | **Date**: 2026-05-13 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/004-keyboard-state-display/spec.md`

## Summary

Add a standard keyboard state display facility to `FS.Skia.UI.KeyboardInput` that exposes a pure structured display model and a reusable Skia scene element. The display builds on the existing keyboard input runtime, layout state, diagnostics, resolved-command effects, and scene primitives. Applications can render a hidden, compact, or expanded state element without custom visualization code while tests and alternate renderers can assert the same display data through the public `.fsi` surface.

## Technical Context

**Language/Version**: F# on .NET `net10.0` with `LangVersion=latest`  
**Primary Dependencies**: Existing `FS.Skia.UI` dependencies; no new package dependency planned beyond the current `YamlDotNet` and `Fable.Elmish` dependencies already used by the keyboard input framework  
**Storage**: N/A for runtime; filesystem only for checked-in sample YAML, replay/readiness evidence, FSI transcripts, and surface-area baselines  
**Testing**: `dotnet test`; Expecto semantic tests through `KeyboardInput.fsi`; FSI/prelude transcript for public API shape; package surface-area baseline tests; sample smoke evidence for the gallery display  
**Target Platform**: Windows and Linux desktop, aligned with existing Skia/Vulkan viewer support  
**Project Type**: Packable F# library plus sample applications  
**Performance Goals**: Display model creation completes in under 1 ms for representative runtime snapshots; compact scene construction avoids text overlap for stack depth up to 4 and up to 12 active top-context labels; state display updates are emitted for 100% of layout, stack, state, held-layer, pending-sequence, resolved-command, and diagnostic changes covered by tests  
**Constraints**: Pure display model must be independently testable; rendered element must use existing `Scene` primitives; compact mode prioritizes active layout, active top context, condensed stack, and active state before hints; diagnostics show only the most recent actionable diagnostic; key labels are limited to bindings available in the active top context; hidden mode must not require rendering; every public module requires `.fsi`; no top-level visibility modifiers in `.fs`  
**Scale/Scope**: Extend one existing public module, update one sample gallery, add focused tests and surface baseline evidence, generate one public API contract document

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Principle I - Spec -> FSI -> Semantic Tests -> Implementation**: PASS. This is Tier 1 because it extends public API and observable UI behavior. The plan requires `KeyboardInput.fsi` updates, contract tests through the public surface, FSI/prelude evidence, baseline updates, and then `.fs` implementation.
- **Principle II - Visibility Lives in `.fsi`**: PASS. The new public records, discriminated unions, and functions are declared in `src/Lib/KeyboardInput.fsi`; implementation symbols omitted from `.fsi` remain private by compiler enforcement.
- **Principle III - Idiomatic Simplicity**: PASS. The design uses plain records, discriminated unions, lists, maps, and pure functions. No custom operators, SRTP, reflection, dynamic dispatch, type providers, or non-trivial computation expressions are planned.
- **Principle IV - Elmish/MVU Boundary**: PASS. The display is derived from the existing pure `InputRuntime` and `InputEffect` boundary. Rendering remains a pure `Scene` value; viewer sample integration stays at the Elmish host edge.
- **Principle V - Synthetic Evidence Disclosure**: PASS. No synthetic evidence is planned. Tests use checked-in keyboard configurations and constructed runtime snapshots from the real public API.
- **Principle VI - Test Evidence Is Mandatory**: PASS. Tests must cover compact/expanded model generation, top-context labels, stack condensation, recent actionable diagnostic selection, pending/resolved feedback, hidden mode behavior, invalid-layout partial rendering, scene element output, and sample smoke evidence.
- **Principle VII - Observability and Safe Failure**: PASS. Display diagnostics are selected from first-class `InputDiagnostic` data and invalid/partial runtime state renders explicitly instead of silently disappearing.
- **Change Classification**: PASS. Tier 1 contracted change: public API extension and user-visible rendering behavior.
- **Engineering Constraints**: PASS. F#/.NET only; no new dependency; `.fsi` surface and package baselines remain required; Vulkan viewer constraints are unaffected.

## Project Structure

### Documentation (this feature)

```text
specs/004-keyboard-state-display/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── public-api.md
├── checklists/
│   └── requirements.md
└── tasks.md             # Phase 2 output from /speckit-tasks
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

specs/004-keyboard-state-display/readiness/
├── fsi/
│   └── keyboard-state-display-prelude.txt
├── sample-smoke/
│   └── keyboard-input-gallery-state-display.txt
└── surface-baselines/
    └── FS.Skia.UI.txt
```

**Structure Decision**: Extend the existing `src/Lib/KeyboardInput.fsi` and `.fs` module because keyboard state display is a direct projection of the keyboard input runtime, and the existing `LayoutStateView`/`renderLayoutState` APIs already live there. Keep the rendered element in the core package as a `Scene` builder, not a separate sample-only overlay. Update `KeyboardInputGallery` only as a consumer demonstration.

## Complexity Tracking

No constitution violations require justification. No new package dependency is planned.

## Phase 0: Research

See [research.md](./research.md). Decisions cover public API placement, display model shape, compact versus expanded behavior, top-context label filtering, diagnostic selection, invalid/partial state rendering, and rendering approach.

## Phase 1: Design & Contracts

See [data-model.md](./data-model.md), [contracts/public-api.md](./contracts/public-api.md), and [quickstart.md](./quickstart.md).

Design summary:

- `KeyboardInput.fsi` exposes display density, visibility, option, model, stack-entry, label, hint, and diagnostic display records.
- A pure builder converts `InputRuntime` plus recent effects/options into a `KeyboardStateDisplayModel`.
- Compact display keeps active layout, active top context, condensed stack, and active state visible before optional hints.
- Expanded display preserves full stack, top-context labels, pending sequence, recent command, and the most recent actionable diagnostic.
- Label hints are derived from bindings in the active top context, not from the full keymap.
- Invalid or missing layout state produces a partial display model with an actionable diagnostic instead of failing scene construction.
- The standard renderer consumes the display model and returns a `Scene`; hidden mode returns an empty scene.

## Constitution Check - Post Design

- **Principle I**: PASS. `contracts/public-api.md` defines the intended `.fsi` additions before implementation.
- **Principle II**: PASS. The public surface remains in the existing companion `.fsi`; no `.fs` visibility modifiers are required.
- **Principle III**: PASS. The model uses simple F# records/unions and pure functions.
- **Principle IV**: PASS. Display state is projected from the existing pure keyboard runtime and effects; sample event handling remains Elmish at the edge.
- **Principle V**: PASS. Planned tests use real checked-in configurations and runtime transitions, not mocks or hardcoded substitutes for behavior under test.
- **Principle VI**: PASS. Each user story maps to automated tests plus sample smoke evidence.
- **Principle VII**: PASS. The most recent actionable diagnostic is first-class display data and partial-state rendering is explicit.
- **Engineering Constraints**: PASS. No dependency change; package and surface baseline work are explicit.
