# Implementation Plan: Controls Boundary Refactor

**Branch**: `011-controls-boundary-refactor` | **Date**: 2026-05-17 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/011-controls-boundary-refactor/spec.md`

## Summary

Refactor the controls boundary so Controls is openly Skia-rendered and
Elmish-shaped, not renderer-neutral. Ordinary controls remain stable record
declarations generic over product messages, while advanced controls expose
explicit Skia escape hatches for rich text, measurement, custom drawing,
clipping, effects, and diagnostics. Charts, graph views, and DataGrid move
fully under Controls ownership; the legacy Charts package, capability,
generated guidance, and active package references are removed rather than kept
as a compatibility shim.

Keyboard input becomes a richer package-owned runtime in
`FS.Skia.UI.KeyboardInput` that Controls and `FS.Skia.UI.Controls.Elmish`
consume instead of duplicating input state. Persistent product values remain in
product models. Transient control state becomes an explicit product-owned
`ControlRuntime` submodel. Direct `Cmd`, subscription, and program wiring is
isolated in the dedicated `FS.Skia.UI.Controls.Elmish` adapter so the base
Controls package does not own the host loop.

## Technical Context

**Language/Version**: F# on .NET `net10.0`; SDK-style projects; existing FAKE
`build.fsx`; Bash and Windows command wrappers; generated products reference
packages rather than copying framework implementation source.
**Primary Dependencies**: Existing Fable.Elmish 4.2.0, SkiaSharp
4.147.0-preview.2.1, Silk.NET 2.23.0, Yoga.Net 3.2.3, YamlDotNet 17.1.0,
Expecto 10.2.2, FAKE, and Spec Kit assets. No new third-party runtime
dependency is planned for this refactor. Any new dependency for rich text,
clipboard, IME, rendering diagnostics, or adapter behavior must be justified in
`docs/dependencies.md`, `Directory.Packages.props`, and
`readiness/dependency-report.md`.
**Storage**: Filesystem only: source projects, `.fsi` contracts, catalog data,
template fragments, local/package skills, generated validation roots under
`artifacts/`, public surface baselines under `readiness/surface-baselines/`,
and feature evidence under
`specs/011-controls-boundary-refactor/readiness/`.
**Testing**: Expecto semantic tests, FSI transcripts against packed packages,
package surface baselines, catalog contract tests, keyboard input runtime
tests, control runtime tests, interaction dispatch tests, rich rendering
evidence, generated product verification, dependency reports, generated
guidance checks, template drift, evidence graph, and evidence audit.
**Target Platform**: Windows and Linux developer/CI environments that can
restore, build, test, pack, and instantiate generated products. Native visual,
clipboard, font, IME, and GPU smoke tests must report unsupported environment
conditions separately from implementation defects.
**Project Type**: Governed F# framework/template repository with multiple
packable libraries, examples, samples, tests, Spec Kit assets, local/package
agent skills, and generated product validation.
**Performance Goals**: Control runtime update and stale-target recovery should
be deterministic and bounded by explicit event/control sets. DataGrid and
table-like controls must preserve the existing 10,000 item validation
expectation from the Controls plan without rendering all rows as live scene
nodes. Rich rendering evidence records observed durations and unsupported
environment diagnostics rather than silently skipping.
**Constraints**: Public surface is owned by `.fsi` files and package surface
baselines. Top-level `.fs` files do not use visibility keywords to define the
public contract. Persistent business values remain product-model-owned.
Controls and KeyboardInput may expose product-owned runtime submodels, but host
effects and command execution stay at the product or adapter edge. Rich text is
in scope as a Skia-specific rendering capability; renderer-neutral widgets,
new renderer backends, platform-native wrappers, formal accessibility
certification, automatic external-app migration, and release publishing
automation are out of scope.
**Scale/Scope**: Tier 1 contracted refactor across Controls, KeyboardInput,
`FS.Skia.UI.Controls.Elmish`, template/capability metadata, generated guidance,
surface baselines, tests, docs, and readiness evidence. Existing lower-level
Scene, Layout, KeyboardInput, SkiaViewer, and Elmish paths remain available for
applications that do not select Controls.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: PASS. The refactor changes active generated package
  selection and generated guidance. It must update `template/capabilities.yml`,
  default app/package references, template fragments, product examples,
  generated guidance scans, and template drift evidence so Controls is the
  active home for controls, rich text, chart controls, graph views, and
  DataGrid.
- **Dependency impact**: PASS. Controls already depends on Scene, Layout, and
  KeyboardInput, but it currently references `src/Lib/Lib.fsproj`; this feature
  must remove monolithic viewer/runtime coupling unless a public contract
  explicitly requires it. The legacy `FS.Skia.UI.Charts` package is removed.
  The dedicated `FS.Skia.UI.Controls.Elmish` adapter package must record its
  Fable.Elmish dependency. Any package-version change must update central
  package versions, dependency docs, and dependency-report evidence.
- **Command-surface impact**: PASS. `Dev`, `Verify`, `Ci`, `PackLocal`,
  `PackageSurfaceCheck`, `FsiTranscripts`, `TemplateCheck`,
  `CapabilityCheck`, `SkillCheck`, `GeneratedProductCheck`,
  `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`,
  `EvidenceGraph`, and `EvidenceAudit` must include controls boundary,
  removed Charts ownership, KeyboardInput runtime, Elmish adapter, and
  generated guidance checks.
- **Generated project impact**: PASS. Generated products that select Controls
  must reference Controls as the single authoring path for forms, rich text,
  charts, graph views, and DataGrid. Generated products must not reference the
  legacy Charts package/capability or copy framework samples, historical specs,
  readiness evidence, framework implementation projects, or stale chart-only
  skills.
- **Evidence paths**: PASS. Required readiness files are listed in Project
  Structure. Evidence covers public surface, package boundary, adapter
  behavior, KeyboardInput package ownership, catalog metadata, control runtime,
  rich rendering, keyboard input Elmish flow, chart/DataGrid Controls
  ownership, generated product usage, dependency impact, template drift,
  compatibility impact, evidence graph, and evidence audit.
- **`.fsi` / contract impact**: PASS. This is a Tier 1 public contract change.
  Controls, KeyboardInput, and the Elmish adapter require curated `.fsi`
  signatures, FSI transcript coverage, package surface baselines, and
  compatibility guidance. The removed Charts package/baseline is a deliberate
  breaking boundary change documented by migration guidance.
- **MVU/effect boundary**: PASS. Base Controls remains generic over product
  messages. `KeyboardInput` exposes product-owned runtime state plus pure
  update and explicit effects. Controls exposes a product-owned
  `ControlRuntime` for transient interaction state plus pure updates and
  inspectable effects/diagnostics. Elmish `Cmd`, subscription, and program
  integration belong in the adapter.
- **Synthetic evidence**: PASS. Synthetic evidence is not planned as primary
  proof. Any fakes for clipboard, IME, text composition, GPU, font, or
  unsupported environment paths must be marked under the repository synthetic
  disclosure policy and paired with real evidence where available.
- **Test evidence**: PASS. Failing-first tests are required for public surface
  drift, package boundary, Charts removal, catalog ownership, DataGrid
  category, rich rendering contracts, control runtime transitions, keyboard
  input runtime transitions, Elmish adapter behavior, generated product usage,
  dependency report, template drift, and guidance scans.
- **Observability**: PASS. Validation failures must name the stale reference,
  package, capability, control, catalog entry, generated profile, adapter
  contract, input/control runtime state, unsupported environment condition, or
  migration guidance gap.
- **Deferred scope**: PASS. Renderer-neutral widget abstraction, new renderer
  backends, browser/mobile support, platform-native wrappers, formal
  accessibility certification, release publishing automation, and automatic
  migration of external applications remain out of scope.

### Constitution Gate Result

PASS. No unresolved clarifications remain. The plan carries two explicit
compatibility obligations: remove the legacy Charts package/capability without
a compatibility shim, and preserve lower-level Scene, Layout, KeyboardInput,
SkiaViewer, and Elmish usage paths for applications that do not choose the
higher-level Controls capability.

## Project Structure

### Documentation (this feature)

```text
specs/011-controls-boundary-refactor/
|-- spec.md
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   |-- controls-public-api.md
|   |-- package-capability-boundary.md
|   |-- elmish-keyboard-runtime.md
|   |-- generated-guidance-validation.md
|   `-- readiness-evidence.md
|-- checklists/
|   `-- requirements.md
`-- readiness/
    |-- public-surface.md
    |-- package-boundary.md
    |-- elmish-adapter.md
    |-- keyboardinput-package.md
    |-- control-catalog.md
    |-- control-runtime.md
    |-- rich-rendering.md
    |-- keyboard-input-elmish.md
    |-- chart-datagrid-controls.md
    |-- generated-product-usage.md
    |-- dependency-report.md
    |-- template-drift.md
    |-- compatibility-impact.md
    |-- evidence-graph.md
    `-- evidence-audit.md
```

### Source Code (repository root)

```text
src/
|-- Controls/
|   |-- Controls.fsproj
|   |-- Types.fsi / Types.fs
|   |-- Control.fsi / Control.fs
|   |-- ControlRuntime.fsi / ControlRuntime.fs
|   |-- Attributes.fsi / Attributes.fs
|   |-- Theme.fsi / Theme.fs
|   |-- Accessibility.fsi / Accessibility.fs
|   |-- Diagnostics.fsi / Diagnostics.fs
|   |-- Catalog.fsi / Catalog.fs
|   |-- RichText.fsi / RichText.fs
|   |-- TextInput.fsi / TextInput.fs
|   |-- Collections.fsi / Collections.fs
|   |-- Charts.fsi / Charts.fs
|   |-- DataGrid.fsi / DataGrid.fs
|   |-- CustomControl.fsi / CustomControl.fs
|   |-- catalog.yml
|   `-- skill/SKILL.md
|-- KeyboardInput/
|   |-- KeyboardInput.fsproj
|   |-- KeyboardInput.fsi
|   |-- KeyboardInput.fs
|   `-- skill/SKILL.md
|-- Elmish/
|   |-- Elmish.fsproj
|   |-- Elmish.fsi
|   `-- Elmish.fs
|-- Controls.Elmish/
|   |-- Controls.Elmish.fsproj
|   |-- ControlsElmish.fsi
|   `-- ControlsElmish.fs
|-- Scene/
|-- SkiaViewer/
|-- Layout/
`-- Testing/

samples/
|-- ControlsGallery/
|-- DataGridGallery/
|-- KeyboardInputGallery/
`-- ScreenshotGallery/

template/
|-- capabilities.yml
|-- fragments/
|   |-- controls/
|   |-- keyboard-input/
|   `-- elmish/
`-- base/

tests/
|-- Controls.Tests/
|-- KeyboardInput.Tests/
|-- Elmish.Tests/
|-- Package.Tests/
|-- Governance.Tests/
`-- Smoke.Tests/

readiness/
`-- surface-baselines/
    |-- FS.Skia.UI.Controls.txt
    |-- FS.Skia.UI.KeyboardInput.txt
    |-- FS.Skia.UI.Elmish.txt
    `-- FS.Skia.UI.Controls.Elmish.txt
```

## Phase 0: Research

Research decisions are recorded in [research.md](./research.md). All planning
unknowns from the Technical Context are resolved.

## Phase 1: Design And Contracts

Design entities are recorded in [data-model.md](./data-model.md). Public and
governance contracts are recorded in [contracts/](./contracts/). Validation
workflow is recorded in [quickstart.md](./quickstart.md).

## Post-Design Constitution Check

PASS. The design artifacts preserve the required Spec -> FSI -> Semantic Tests
-> Implementation order, keep public visibility in `.fsi` contracts, make
stateful input/control workflows explicit MVU boundaries, require test and
readiness evidence, and document synthetic evidence handling without planning
synthetic evidence as primary proof.
