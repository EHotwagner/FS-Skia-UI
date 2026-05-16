# Implementation Plan: Skia Controls Library

**Branch**: `010-skia-controls-library` | **Date**: 2026-05-16 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/010-skia-controls-library/spec.md`

## Summary

Deliver a comprehensive declarative Skia controls capability for Elmish-style
view functions. The implementation introduces an `FS.Skia.UI.Controls`
capability package, a catalog-driven control surface, control contracts and
examples, input/focus/accessibility diagnostics, a reference gallery, generated
product support, and validation evidence. Charts and graphs move into Controls
ownership; the separate Charts capability, package, template fragment, and
generated chart skill are removed from active capability selection. Layout
remains a separate runtime package, but generated product guidance for
layout-oriented controls moves under the new `fs-skia-ui-widgets` skill.

## Technical Context

**Language/Version**: F# on .NET `net10.0`; SDK-style projects; existing FAKE
`build.fsx`; Bash and Windows command wrappers; generated products remain
consumer projects that reference packages rather than copying framework source.
**Primary Dependencies**: Existing Fable.Elmish, SkiaSharp 4 preview packages,
Silk.NET, Yoga.Net, YamlDotNet, Expecto, FAKE, and Spec Kit assets. No new
third-party runtime dependency is planned for the controls library. Any new
dependency for text input, clipboard, IME diagnostics, accessibility metadata,
or virtualization must be justified in `docs/dependencies.md`,
`Directory.Packages.props`, and `readiness/dependency-report.md`.
**Storage**: Filesystem only: source projects, `.fsi` contracts, catalog data,
template fragments, local skills, generated validation roots under
`artifacts/`, public surface baselines under `readiness/surface-baselines/`,
and feature evidence under `specs/010-skia-controls-library/readiness/`.
**Testing**: Expecto semantic tests, packed-library FSI transcripts, catalog
contract tests, interaction dispatch tests, keyboard/focus/text-entry tests,
layout/rendering evidence, accessibility/contrast diagnostics, generated
product verification, dependency reports, generated guidance checks, template
drift, evidence graph, and evidence audit.
**Target Platform**: Windows and Linux developer/CI environments that can
restore, build, test, pack, and instantiate generated projects. Native visual
and input smoke tests must distinguish implementation defects from unsupported
GPU, font, clipboard, text-input, IME, or window-system conditions.
**Project Type**: Governed F# framework and template repository with multiple
packable libraries, examples, samples, tests, Spec Kit assets, local agent
skills, and generated product validation.
**Performance Goals**: List and table-like controls support 10,000 items with
responsive scrolling, predictable selection, and item updates in the reference
catalog. Reference gallery validation covers three viewport sizes and two scale
factors. Observed durations for catalog, generated product, and visual
validation are recorded as evidence rather than silently ignored.
**Constraints**: The public surface is owned by `.fsi` files and package
surface baselines. Persistent control values remain model-owned. Controls may
retain only transient interaction state. Rich text editing, new renderer
backends, new platform support promises, platform-native widget wrappers,
designer tooling, formal accessibility certification, and release publishing
automation are out of scope.
**Scale/Scope**: Initial supported catalog contains at least 30 documented
controls or variants across display, input, selection, navigation, layout,
feedback, data, chart, and graph categories. Every supported control requires
contract, catalog, example, test, visual state, accessibility metadata, and
evidence coverage before readiness approval.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: PASS. This is a template-owned Tier 1 change. It must
  update `template/capabilities.yml`, default generated app references,
  profile behavior, generated guidance, selected skills, template fragments,
  and template drift evidence. The default app capability set changes from
  Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Charts to Scene,
  SkiaViewer, Elmish, KeyboardInput, Layout, Controls.
- **Dependency impact**: PASS. The feature introduces the
  `FS.Skia.UI.Controls` package and removes `FS.Skia.UI.Charts` from active
  generated capability selection. Controls may depend on Scene, Layout, and
  KeyboardInput, while generated app profiles also select SkiaViewer and
  Elmish. Central package versions, dependency docs, package metadata, and
  `DependencyReport` evidence must be updated if any dependency changes.
- **Command-surface impact**: PASS. `Dev`, `Verify`, `Ci`, `PackLocal`,
  `PackageSurfaceCheck`, `FsiTranscripts`, `TemplateCheck`,
  `CapabilityCheck`, `SkillCheck`, `GeneratedProductCheck`,
  `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`,
  `EvidenceGraph`, and `EvidenceAudit` must include controls-specific
  validation or evidence. A dedicated controls/catalog target is allowed when
  it improves failure messages.
- **Generated project impact**: PASS. Default generated products include the
  controls capability, package reference, concise controls guidance,
  `fs-skia-ui-widgets`, and a product-owned representative controls example.
  Generated products must not copy framework samples, framework galleries,
  historical specs, readiness evidence, framework implementation projects, or
  separate chart/layout guidance skills.
- **Evidence paths**: PASS. Required readiness paths are listed under Project
  Structure and contracts. Evidence includes control catalog, public surface,
  semantic tests, interaction tests, layout/rendering evidence, generated
  product usage, local skill consolidation, dependency report, generated
  guidance, template drift, evidence graph, evidence audit, and compatibility
  impact.
- **`.fsi` / contract impact**: PASS. This is an approved Tier 1 public
  contract change. Controls require curated `.fsi` signatures, package surface
  baselines, FSI transcripts, public docs, sample contracts, and compatibility
  guidance for lower-level Scene, SkiaViewer, Layout, KeyboardInput, and Charts
  users.
- **MVU/effect boundary**: PASS. Control authoring is model-view-update
  oriented: persistent values live in the app model, events produce application
  messages, update remains pure, and host/text/clipboard/environment effects
  are represented explicitly at the viewer or input edge. Control runtime
  internals may keep transient hover, pressed, focus, caret, drag, and
  in-progress composition state.
- **Synthetic evidence**: PASS. Synthetic evidence is not planned as primary
  proof. If visual, text-input, clipboard, IME, environment, or accessibility
  failure paths require fixtures or fakes, they must follow the repository
  synthetic disclosure policy and be paired with real evidence where available.
- **Test evidence**: PASS. Failing-first tests are required for public surface
  baselines, catalog completeness, control examples, semantic behavior,
  interaction dispatch, text entry, focus traversal, layout participation,
  accessibility metadata, generated product usage, capability selection, skill
  copying, and chart capability removal.
- **Observability**: PASS. Validation failures must name the control,
  capability, package, generated path, event, missing metadata, unsupported
  environment condition, layout conflict, contrast failure, public surface
  drift, or unexpected generated skill/package reference.
- **Deferred scope**: PASS. Rich text, new backends, new OS support promises,
  platform-native widget wrappers, designer tooling, formal certification,
  release publishing automation, and wholesale replacement of lower-level APIs
  are explicitly deferred.

### Constitution Gate Result

PASS with one explicit compatibility obligation: the feature removes active
Charts capability/package ownership and therefore must include compatibility
guidance for existing chart users. The feature may document the breaking change
and replacement path through Controls, but it must not expand into V2 migration
implementation or release automation.

## Project Structure

### Documentation (this feature)

```text
specs/010-skia-controls-library/
|-- spec.md
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   |-- controls-public-api.md
|   |-- control-catalog.md
|   |-- controls-capability-and-template.md
|   |-- interaction-accessibility-validation.md
|   `-- widgets-skill-consolidation.md
|-- checklists/
|   `-- requirements.md
`-- readiness/
    |-- control-catalog.md
    |-- public-surface.md
    |-- semantic-tests.md
    |-- interaction-tests.md
    |-- layout-rendering.md
    |-- generated-product-usage.md
    |-- local-skills.md
    |-- dependency-report.md
    |-- generated-guidance.md
    |-- template-drift.md
    |-- evidence-graph.md
    |-- evidence-audit.md
    `-- compatibility-impact.md
```

### Source Code (repository root)

```text
src/
|-- Controls/
|   |-- Controls.fsproj
|   |-- Types.fsi
|   |-- Types.fs
|   |-- Control.fsi
|   |-- Control.fs
|   |-- Attributes.fsi
|   |-- Attributes.fs
|   |-- Theme.fsi
|   |-- Theme.fs
|   |-- Accessibility.fsi
|   |-- Accessibility.fs
|   |-- Diagnostics.fsi
|   |-- Diagnostics.fs
|   |-- Catalog.fsi
|   |-- Catalog.fs
|   |-- TextInput.fsi
|   |-- TextInput.fs
|   |-- Collections.fsi
|   |-- Collections.fs
|   |-- Charts.fsi
|   |-- Charts.fs
|   |-- CustomControl.fsi
|   |-- CustomControl.fs
|   `-- skill/SKILL.md
|-- Scene/
|-- SkiaViewer/
|-- Elmish/
|-- KeyboardInput/
|-- Layout/
`-- Testing/

samples/
`-- ControlsGallery/
    |-- ControlsGallery.fsproj
    `-- Program.fs

template/
|-- capabilities.yml
|-- fragments/
|   `-- controls/
|       |-- README.md
|       `-- skill/SKILL.md
`-- profiles/
    |-- app.yml
    |-- governed.yml
    |-- headless-scene.yml
    `-- sample-pack.yml

tests/
|-- Controls.Tests/
|   |-- Controls.Tests.fsproj
|   |-- CatalogTests.fs
|   |-- PublicSurfaceTests.fs
|   |-- SemanticTests.fs
|   |-- InteractionTests.fs
|   |-- TextInputTests.fs
|   |-- AccessibilityTests.fs
|   `-- RenderingTests.fs
|-- Governance.Tests/
`-- Package.Tests/
```

**Structure Decision**: Controls is the public owner for built-in controls,
custom control wrapping, charts, graphs, and generated widget guidance. Layout
remains a separate runtime capability for layout engine behavior. The existing
Charts package/capability may be used as source material during implementation,
but the accepted end state removes `charts` from active capability selection,
package references, template fragments, and generated skills.

## Complexity Tracking

No constitution gate violation is required. The scope is broad, so the
implementation must be staged through contract-first `.fsi` work, focused
semantic tests, and catalog subsets while preserving the accepted end state.
Any partial catalog milestone must be represented honestly in tasks and must
not mark unsupported controls as supported.

## Phase 0: Research

See [research.md](./research.md). Decisions cover control DSL shape, model-owned
state, package boundaries, chart/graph absorption, Layout and KeyboardInput
dependencies, text entry scope, accessibility diagnostics, 10,000-item
collection behavior, validation evidence, generated product examples, widget
skill consolidation, and deferred scope.

## Phase 1: Design & Contracts

See [data-model.md](./data-model.md),
[contracts/controls-public-api.md](./contracts/controls-public-api.md),
[contracts/control-catalog.md](./contracts/control-catalog.md),
[contracts/controls-capability-and-template.md](./contracts/controls-capability-and-template.md),
[contracts/interaction-accessibility-validation.md](./contracts/interaction-accessibility-validation.md),
[contracts/widgets-skill-consolidation.md](./contracts/widgets-skill-consolidation.md),
and [quickstart.md](./quickstart.md).

Design summary:

- `FS.Skia.UI.Controls` becomes the public package for controls, widgets,
  chart controls, graph controls, catalog metadata, custom control wrappers,
  and widget diagnostics.
- Public authoring follows a FuncUI-inspired F# shape: modules named after
  controls expose `create` functions and declarative attributes for content,
  children, layout, style, state, validation, accessibility, and events.
- Events produce application messages. Controls do not require app-owned
  widget lifecycle objects, and persistent values stay in the Elmish model.
- Transient interaction state is keyed by stable control identity and limited
  to hover, pressed, focus, caret, drag, and active composition behavior.
- The control catalog is a machine-readable source of truth for supported
  controls, examples, states, events, accessibility metadata, tests, and
  evidence.
- Layout remains a runtime capability. Controls depend on Layout where layout
  engine behavior is required, but generated local guidance for
  layout-oriented controls comes from `fs-skia-ui-widgets`.
- Charts and graphs move into Controls. Generated products no longer expose or
  select Charts as a separate capability or `fs-skia-charts` skill.
- Default generated products include Controls by default, one product-owned
  example view, controls package references, and the widgets skill while still
  excluding framework samples and implementation projects.
- Validation fails on public surface drift, catalog drift, missing examples,
  interaction regressions, layout/rendering regressions, missing accessibility
  metadata, contrast failures, generated product usage gaps, and stale chart or
  layout guidance selection.

## Constitution Check - Post Design

- **Principle I - Spec -> FSI -> Semantic Tests -> Implementation**: PASS.
  Contracts require `.fsi` signatures, FSI transcripts, semantic tests, and
  public surface baselines before implementation is accepted.
- **Principle II - Visibility Lives in `.fsi`**: PASS. Controls modules expose
  only curated signature members and package-specific baselines.
- **Principle III - Idiomatic Simplicity**: PASS. The design uses records,
  discriminated unions, plain modules, structured catalog data, FAKE targets,
  and Expecto tests. No advanced F# feature is required by the plan.
- **Principle IV - Elmish/MVU Boundary**: PASS. Control events map to messages,
  persistent state is model-owned, update remains pure, and host/input effects
  stay at the edge.
- **Principle V - Synthetic Evidence Disclosure**: PASS. Synthetic evidence is
  not planned as primary proof. Any fixtures or fakes must use the repository
  disclosure policy.
- **Principle VI - Test Evidence Is Mandatory**: PASS. Contracts define
  failing-first tests and readiness evidence for public API, catalog, behavior,
  interaction, rendering, accessibility, templates, generated products, skills,
  dependencies, and charts removal.
- **Principle VII - Observability and Safe Failure**: PASS. Diagnostics are
  required for missing metadata, invalid state, unsupported environments,
  layout conflicts, hit-test failures, surface drift, generated selection drift,
  and validation gaps.
- **Change Classification**: PASS. Tier 1 public package/template/governance
  change with compatibility impact and no V2 migration implementation.
- **Engineering Constraints**: PASS. F#/.NET, `.fsi` contracts, MVU boundaries,
  central dependency governance, local package output, structured evidence, and
  existing FAKE command surface remain governing constraints.
