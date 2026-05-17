# Feature Specification: Controls Boundary Refactor

**Feature Branch**: `011-controls-boundary-refactor`  
**Created**: 2026-05-17  
**Status**: Draft  
**Input**: User description: "write specs for this refactoring: controls should be openly Skia/Elmish-specific, support advanced Skia capabilities such as rich text without renderer-neutral abstractions, consolidate charts and DataGrid under Controls ownership, and clean the package boundary so controls are not coupled to the monolithic viewer/runtime surface unless explicitly required."

## Clarifications

### Session 2026-05-17

- Q: How should KeyboardInput participate in the controls boundary refactor? -> A: Elmish submodel.
- Q: How should public controls expose Skia-specific rendering? -> A: Hybrid stable records with explicit Skia escape hatches.
- Q: How should Controls depend on Elmish? -> A: Hybrid generic base controls with dedicated Elmish adapter.
- Q: What happens to the legacy Charts package and capability? -> A: Remove the legacy Charts package and capability entirely.
- Q: How should transient control interaction state be handled? -> A: Product-owned ControlRuntime submodel.
- Q: How should KeyboardInput package organization change? -> A: Consolidate rich runtime into the dedicated KeyboardInput package consumed by Controls and the Elmish adapter.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Build Skia/Elmish Controls Directly (Priority: P1)

As a product application developer, I want the controls capability to be explicit
that it is for Skia-rendered Elmish-style applications, so that I can use rich
text, precise drawing, visual effects, and Skia-native diagnostics without
guessing whether the control layer is hiding renderer-specific features.

**Independent Test**: A representative product view can declare ordinary form
controls, rich text content, and a custom Skia-specific visual extension through
the controls surface. The view remains model-driven, emits product messages for
events, renders deterministic visual output, and does not require a
renderer-neutral replacement path.

Ordinary controls use stable control records for common declarations. Advanced
or custom controls may use explicit Skia-specific escape hatches when direct
rendering access is required for text quality, drawing precision, effects, or
diagnostics.

Keyboard input follows the same flow: a representative product model stores
keyboard input runtime state, routes key down and key up events through keyboard
input update, records emitted effects, and renders the keyboard state display
from the current model. Persistent keyboard mode state survives ordinary
command resolution, temporary held layers clear on key release or focus loss,
and the view reflects the latest runtime state.

The base controls surface remains Elmish-shaped and generic over product
messages. Direct command or program integration is provided through a dedicated
Elmish adapter when product workflows need it.

Transient control interaction state follows the same submodel pattern as
keyboard input. Product applications own an inspectable control runtime for
focus, hover, pressed controls, caret, text composition, drag, and similar
short-lived interaction state, while business values remain in the product
domain model.

### User Story 2 - Use Charts and DataGrid as Controls (Priority: P1)

As a product application developer, I want charts, graph views, and DataGrid to
belong to the same controls catalog as buttons, inputs, lists, and tables, so
that I can discover, configure, validate, and document all interactive visual
components through one capability.

**Independent Test**: The controls catalog and examples expose chart, graph, and
DataGrid entries under Controls ownership. DataGrid is categorized as a data or
collection control rather than a chart. A product view can combine form inputs,
a chart, and a DataGrid without selecting a separate active Charts capability.

### User Story 3 - Generate Coherent Product Guidance (Priority: P2)

As a template user, I want generated product profiles and local guidance to
describe one controls path for forms, rich text, charts, graph views, and data
controls, so that new projects start from the same mental model as the
framework.

**Independent Test**: Generated product profiles that include Controls provide
Controls package references, controls guidance, and representative usage for
form controls plus at least one data/chart control. Generated products do not
copy framework implementation code or require stale chart-specific guidance for
new control usage.

### User Story 4 - Validate the Boundary as a Maintainer (Priority: P2)

As a framework maintainer, I want public contracts, examples, dependency
reports, and readiness evidence to make the controls boundary auditable, so that
future work does not accidentally reintroduce a generic renderer promise or a
hidden dependency on the desktop host loop.

**Independent Test**: Governance checks report package contents, public
contracts, generated guidance, dependency impact, compatibility impact, and
visual/control evidence for the refactored boundary. Failures identify the
specific stale reference, unsupported scope expansion, catalog omission, or
boundary violation.

### Edge Cases

- Existing chart consumers need migration guidance because the legacy Charts
  package and capability are removed rather than retained as a compatibility
  shim.
- Advanced Skia-specific features must remain compatible with model-owned
  persistent state and message-producing control events.
- Control examples must distinguish renderer-specific richness from host-loop
  ownership; controls can require Skia output without owning window creation,
  update scheduling, or application shutdown.
- DataGrid must not remain discoverable only through chart terminology.
- Generated products must not copy framework samples, historical specs,
  readiness evidence, or implementation projects.
- Keyboard input focus loss must clear pressed keys and temporary held layers
  without losing persistent mode state.
- Keyboard state display must reflect current runtime state and recent effects
  without relying on hidden renderer or host-loop state.
- Focus, hover, pressed, caret, composition, and drag state must recover from
  cancelled interactions, focus loss, removed controls, and stale event targets
  without mutating product business values.

## Requirements *(mandatory)*

### Change Classification

- **Tier 1 (contracted change)**: This refactor changes public package/API
  surface, `.fsi` signatures, package and capability ownership, generated
  product guidance, package surface baselines, and compatibility guidance. It
  requires curated `.fsi` updates, semantic and FSI tests, baseline refreshes,
  documentation, migration guidance, and readiness evidence before merge
  readiness.

### Functional Requirements

- **FR-001**: The controls capability MUST be documented and validated as a
  Skia-rendered, Elmish-style controls surface, not as a renderer-neutral or
  general-purpose widget abstraction.
- **FR-002**: The controls surface MUST allow Skia-specific visual capabilities
  where they are needed for control quality, including rich text, text
  measurement, custom drawing, clipping, visual effects, and diagnostic
  evidence.
- **FR-002a**: Ordinary controls MUST remain declarable through stable control
  records, while advanced or custom controls MAY expose explicit Skia-specific
  escape hatches for direct rendering scenarios.
- **FR-003**: Persistent control values MUST remain owned by the product
  application model. Controls MAY expose transient interaction state only when
  it is needed for interaction fidelity and is observable through explicit
  control evidence or diagnostics.
- **FR-004**: Control events MUST produce product messages or explicit effects
  at the application boundary. Controls MUST NOT require ownership of the
  application update loop to perform ordinary interaction.
- **FR-005**: Charts, graph views, and DataGrid MUST be represented under the
  Controls catalog, examples, generated guidance, validation evidence, and
  public support model.
- **FR-006**: DataGrid MUST be treated as a data or collection control, not as a
  chart category, in catalog metadata, examples, docs, and generated guidance.
- **FR-007**: The legacy Charts package and capability MUST be removed during
  this refactor; new chart, graph, and DataGrid authoring MUST go through
  Controls.
- **FR-008**: Generated product profiles that include application controls MUST
  select Controls as the active home for standard controls, rich text, charts,
  graph views, and data controls.
- **FR-009**: Generated product guidance MUST avoid stale chart-specific or
  renderer-neutral instructions for new controls work.
- **FR-010**: Public documentation MUST explain when a product should use the
  controls capability, lower-level scene composition, layout primitives, input
  helpers, or viewer host APIs.
- **FR-011**: Public contracts and examples MUST include at least one
  Skia-specific rich rendering scenario, at least one chart scenario, at least
  one graph or DataGrid scenario, and at least one ordinary form-control
  scenario.
- **FR-012**: Validation failures MUST identify the affected control, catalog
  entry, generated profile, package reference, public contract, or unsupported
  scope expansion.
- **FR-013**: Migration guidance MUST describe the supported path for existing
  chart users to move to Controls ownership without retaining a Charts
  compatibility package, promising automated migration, or promising release
  publishing.
- **FR-014**: Readiness evidence MUST demonstrate that the refactored controls
  boundary is usable from generated products and from repository samples without
  relying on copied framework implementation source.
- **FR-015**: The feature MUST preserve existing lower-level scene, layout,
  input, viewer, and Elmish usage paths for applications that do not choose the
  higher-level controls capability.
- **FR-016**: Keyboard input MUST be documented and validated as an
  Elmish-shaped submodel that product applications can store in their model,
  update from input events, and render from the current model.
- **FR-017**: Keyboard input runtime state MUST expose pressed keys, active
  layout, active mode stack, persistent mode state, pending sequence, effects,
  events, and diagnostics through inspectable records or equivalent public
  contracts.
- **FR-018**: Keyboard state display MUST be renderable from current keyboard
  input runtime state and recent keyboard input effects without requiring hidden
  mutable state, renderer callbacks, or ownership of the application loop.
- **FR-019**: Keyboard input effects MUST be interpretable by the product update
  workflow into product commands, control messages, diagnostics, or explicit
  host effects.
- **FR-020**: Keyboard input focus recovery MUST clear pressed keys and
  temporary held layers while preserving persistent mode state unless the
  product explicitly resets the input runtime.
- **FR-020a**: The rich keyboard input runtime, state display, diagnostics, and
  update contracts MUST be consolidated into the dedicated KeyboardInput package
  so Controls and the Elmish adapter consume one package-owned input surface.
- **FR-021**: The base Controls surface MUST remain generic over product
  messages while following Elmish model-view-update ownership rules.
- **FR-022**: Direct command, subscription, or program integration MUST be
  exposed through a dedicated Elmish adapter rather than being required by
  ordinary control declarations.
- **FR-023**: Generated products that use Elmish program integration MUST show
  the adapter path, while simpler examples MAY use the generic message-based
  controls surface directly.
- **FR-024**: Controls MUST expose a product-owned runtime submodel for
  transient interaction state such as focused control, hovered control, pressed
  controls, caret or selection state, text composition, active drag, and stale
  interaction diagnostics.
- **FR-025**: Control runtime updates MUST be driven by explicit input or
  control messages and MUST produce inspectable effects or diagnostics for
  focus changes, activation, text composition, drag lifecycle, and recovery
  paths.
- **FR-026**: Product business values such as text contents, selected rows,
  active tabs, chart data, and DataGrid data MUST remain outside the transient
  control runtime unless explicitly represented as product model state.
- **FR-027**: Controls MUST NOT depend on the monolithic `src/Lib`
  viewer/runtime surface unless a public contract explicitly requires that
  coupling and the dependency report documents the justification. Ordinary
  controls, chart controls, DataGrid, KeyboardInput integration, and the Elmish
  adapter MUST keep host-loop and viewer ownership outside the base Controls
  package.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package contents and generated package consumers change.
  Controls becomes the active package home for controls, chart controls, graph
  views, and DataGrid. The legacy Charts package identity and active capability
  are removed. The dedicated `FS.Skia.UI.Controls.Elmish` adapter package owns
  command and program integration for Controls. The rich keyboard input runtime
  moves into the dedicated KeyboardInput package as the single package-owned
  input surface consumed by Controls and the adapter. Package versions and
  package surface baselines must be refreshed.
- **Public contract impact**: `.fsi` signatures, documented public APIs, sample
  contracts, catalog metadata, generated guidance, and surface baselines change.
  Contracts must reflect the intended Skia/Elmish-specific controls model and
  the Controls-owned chart/DataGrid path. Public contracts must distinguish the
  generic message-based controls surface from the
  `FS.Skia.UI.Controls.Elmish` adapter surface and expose keyboard input runtime
  contracts from the dedicated KeyboardInput package.
- **State workflow impact**: Stateful workflows change at the control boundary
  and keyboard input boundary. Persistent values and keyboard input runtime
  state remain product-model-owned, transient control runtime state is an
  explicit product-owned submodel, control and keyboard events produce messages
  or explicit effects, adapter-owned command integration is explicit, and
  host/runtime effects stay outside ordinary control declarations.
- **Layout/rendering impact**: Layout, charts, DataGrid, rich text, custom
  Skia-specific visual output, screenshots or render-readback evidence, and
  unsupported environment diagnostics are in scope. New renderer backends and
  new platform support promises are out of scope.
- **Evidence obligations**: Required real evidence paths include
  `specs/011-controls-boundary-refactor/readiness/public-surface.md`,
  `specs/011-controls-boundary-refactor/readiness/package-boundary.md`,
  `specs/011-controls-boundary-refactor/readiness/elmish-adapter.md`,
  `specs/011-controls-boundary-refactor/readiness/keyboardinput-package.md`,
  `specs/011-controls-boundary-refactor/readiness/control-catalog.md`,
  `specs/011-controls-boundary-refactor/readiness/control-runtime.md`,
  `specs/011-controls-boundary-refactor/readiness/rich-rendering.md`,
  `specs/011-controls-boundary-refactor/readiness/keyboard-input-elmish.md`,
  `specs/011-controls-boundary-refactor/readiness/chart-datagrid-controls.md`,
  `specs/011-controls-boundary-refactor/readiness/generated-product-usage.md`,
  `specs/011-controls-boundary-refactor/readiness/dependency-report.md`,
  `specs/011-controls-boundary-refactor/readiness/template-drift.md`,
  `specs/011-controls-boundary-refactor/readiness/compatibility-impact.md`,
  `specs/011-controls-boundary-refactor/readiness/evidence-graph.md`, and
  `specs/011-controls-boundary-refactor/readiness/evidence-audit.md`.
- **Unsupported scope**: Renderer-neutral widget abstraction, new renderer
  backends, browser/mobile support, platform-native widget wrappers, formal
  accessibility certification, release publishing automation, and automatic
  migration of external applications are out of scope.
- **Build-target impact**: `Dev`, `Verify`, `Ci`, `PackLocal`,
  `PackageSurfaceCheck`, `TemplateCheck`, `CapabilityCheck`, `SkillCheck`,
  `GeneratedProductCheck`, `DependencyReport`, `GeneratedGuidanceCheck`,
  `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` must include the
  refactored controls boundary or its readiness evidence where applicable.

## Key Entities

- **Controls Capability**: The active authoring surface for ordinary controls,
  rich text, chart controls, graph views, and data controls.
- **Elmish Adapter**: The optional integration surface that translates generic
  controls and keyboard input effects into direct Elmish command, subscription,
  or program workflows.
- **Control Catalog Entry**: A documented control record with category,
  supported states, required attributes, interaction metadata, accessibility
  metadata, examples, and evidence expectations.
- **Control Runtime**: Product-owned transient interaction state for controls,
  including focus, hover, pressed controls, caret or selection state, text
  composition, active drag, events, effects, and diagnostics.
- **Rich Rendering Scenario**: A documented use case where control quality
  depends on Skia-specific text, measurement, drawing, clipping, or visual
  effects.
- **Skia Escape Hatch**: An explicit advanced-control path that allows direct
  Skia-specific rendering access without making ordinary controls depend on
  direct rendering callbacks.
- **Keyboard Input Runtime**: Product-owned input state that tracks active
  layout, pressed keys, mode stack, persistent mode state, pending sequences,
  diagnostics, events, and effects.
- **KeyboardInput Package**: The dedicated package that owns the rich keyboard
  input runtime, state display, diagnostics, update contracts, and public
  package surface consumed by Controls and the Elmish adapter.
- **Keyboard State Display**: A renderable view of keyboard input runtime state
  and recent effects, used for overlays, diagnostics, and user-visible input
  feedback.
- **DataGrid Control**: A data or collection control that supports tabular
  presentation and interaction, independent from chart terminology.
- **Chart Migration Guidance**: Documentation and evidence that explain how
  existing chart users move to Controls after the legacy Charts package and
  capability are removed.
- **Generated Product Profile**: A template profile whose selected capabilities,
  package references, guidance, and samples must align with Controls ownership.
- **Boundary Evidence Record**: Readiness output proving public surface,
  dependency impact, catalog completeness, generated usage, and compatibility
  impact.

## Assumptions

- "Elmish-specific" means product applications follow model-view-update message
  ownership; keyboard input runtime is treated as a product-owned submodel, and
  direct command or program integration belongs in a dedicated Elmish adapter.
- Keyboard input is expected to have one rich package-owned runtime surface in
  the dedicated KeyboardInput package; Controls should consume that package
  rather than duplicating or owning input infrastructure.
- "Skia-specific" means the controls capability may expose Skia rendering
  concepts when doing so improves quality or diagnosability; it does not imply
  controls own the desktop host lifecycle.
- Ordinary controls are expected to use stable control records by default;
  direct Skia-specific rendering access is reserved for advanced/custom-control
  scenarios.
- Base controls are expected to remain generic over product messages; generated
  Elmish products may opt into adapter helpers for command or program
  integration.
- Control runtime is expected to contain transient interaction state only;
  product business values remain in domain-specific product model fields.
- Existing chart users receive migration guidance, but a retained compatibility
  package, automated migration tooling, and release publishing are outside this
  feature.
- This refactor is allowed to revise active generated capability selection and
  generated guidance for new products.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of generated product profiles that include Controls present
  Controls as the active home for ordinary controls, rich text, chart controls,
  graph views, and DataGrid.
- **SC-002**: 100% of new Controls examples and generated-product examples avoid
  requiring a separate active Charts capability for chart, graph, or DataGrid
  usage.
- **SC-003**: The Controls catalog includes chart, graph, and DataGrid entries
  with category, required attributes, interaction metadata, accessibility
  metadata, example references, and evidence links.
- **SC-004**: DataGrid appears in data or collection control documentation and
  catalog metadata in all maintained guidance, with zero remaining new-authoring
  references that classify it only as a chart.
- **SC-005**: Validation evidence covers at least one ordinary form flow, one
  rich rendering or rich text flow, one chart flow, and one graph or DataGrid
  flow.
- **SC-006**: Public surface and generated guidance checks fail when a stale
  chart-only active capability reference, renderer-neutral controls promise, or
  unsupported host-loop dependency is introduced.
- **SC-007**: Existing lower-level scene, layout, input, viewer, and Elmish
  samples remain represented in validation evidence after the controls boundary
  refactor.
- **SC-008**: Migration guidance identifies the supported replacement path for
  existing chart users in one maintained document and one generated-product
  guidance scan, with zero retained Charts package references in active
  generated products.
- **SC-009**: Validation evidence includes one product-style flow where
  keyboard input runtime state is stored in the application model, updated from
  key down and key up events, interpreted through emitted effects, and rendered
  as a keyboard state display.
- **SC-010**: Keyboard input evidence demonstrates pressed-key tracking,
  persistent mode state, temporary held layer release, and focus-loss recovery
  in deterministic tests or sample smoke output.
- **SC-010a**: Package and public-surface evidence shows the rich keyboard input
  runtime and state display are exposed from the dedicated KeyboardInput package
  and consumed by Controls or the Elmish adapter without duplicate runtime
  definitions.
- **SC-011**: Public examples and contract evidence include both an ordinary
  stable-record control declaration and an advanced/custom control that uses an
  explicit Skia-specific escape hatch.
- **SC-012**: Public examples and generated-product evidence include both a
  generic message-based controls flow and an Elmish adapter flow for command or
  program integration.
- **SC-013**: Validation evidence includes one product-style flow where control
  runtime state is stored in the application model, updated from focus,
  pointer, keyboard, text composition, or drag events, and rendered from the
  current model without hidden mutable state.
- **SC-014**: Control runtime evidence demonstrates recovery from at least two
  stale or cancelled interaction paths, such as focus loss, removed control
  target, cancelled drag, or interrupted text composition.
