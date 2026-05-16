# Feature Specification: Skia Controls Library

**Feature Branch**: `010-skia-controls-library`  
**Created**: 2026-05-16  
**Status**: Draft  
**Input**: User description: "i want to add a comprehensive skia widget/controls library. it should be for elmish to be put in the view function. take inspiration by https://github.com/fsprojects/Avalonia.FuncUI https://funcui.avaloniaui.net/"

## Clarifications

### Session 2026-05-16

- Q: Should the controls capability be included in the default generated app profile? -> A: Include the controls capability in the default generated app profile, with a small product-owned example view.
- Q: Who owns persistent control values and selection state? -> A: Model-owned persistent state; controls dispatch messages and keep only transient interaction state internally.
- Q: What text-entry scope is required for the first supported catalog? -> A: Plain single-line and multi-line text entry, with cursor, selection, clipboard, validation, and environment-aware IME/composition diagnostics.
- Q: What accessibility scope is required for the first controls release? -> A: Require role/name/state metadata, focus order, keyboard operation, contrast checks, and diagnostics; no formal certification.
- Q: What list/table data scale must the first release support? -> A: Support up to 10,000 list/table items with responsive scrolling, selection, and item updates.
- Q: How should charts and graphs be owned after controls are introduced? -> A: Controls fully absorbs charts and graphs now; remove the separate Charts capability, package, template fragment, and skill.
- Q: How should local agent skills adapt to the controls/widget structure? -> A: Add one broader `fs-skia-ui-widgets` skill that replaces Charts, Layout, and most control-related guidance.
- Q: Should Layout remain a separate runtime capability after widget skill consolidation? -> A: Layout package/capability remains separate; `fs-skia-ui-widgets` replaces only layout-control guidance and generated skill selection.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Compose Screens in the View Function (Priority: P1)

An application developer can describe a complete interactive screen directly in
their Elmish-style view function using reusable Skia controls. The view stays a
pure description of the current model and dispatchable user actions, while the
library handles visual states, layout participation, hit testing, focus, and
rendering behavior.

**Why this priority**: This is the core value of the feature. If controls cannot
be placed naturally in the view function, the library does not solve the
authoring problem.

**Independent Test**: Build a reference counter/form screen from the control
library using only a model, messages, update logic, and a view function. The
screen is acceptable when it renders, responds to pointer and keyboard input,
updates through dispatched messages, and requires no app-owned widget lifecycle
management.

**Acceptance Scenarios**:

1. **Given** a developer has an Elmish application model and dispatch function,
   **When** they compose text, buttons, input fields, toggles, panels, and chart
   controls in the view, **Then** the screen renders as a Skia UI and all user
   actions are delivered as application messages.
2. **Given** the model changes after an update, **When** the view is evaluated
   again, **Then** displayed control values, enabled states, focus indicators,
   selection states, and validation feedback reflect the new model.
3. **Given** a control receives pointer or keyboard interaction, **When** the
   event maps to a declared action, **Then** the action is dispatched once with
   the expected payload and without requiring mutable UI state in user code.
4. **Given** a control has hover, pressed, focus, caret, or active drag state,
   **When** the interaction ends or the view is recreated from the model,
   **Then** only transient interaction state may be retained by the control and
   all persistent values continue to come from the application model.

---

### User Story 2 - Use a Comprehensive Control Catalog (Priority: P1)

A developer can choose from a broad catalog of ready-made controls that cover
common application screens: text, buttons, toggles, editable inputs, selection,
lists, layout containers, scrolling, progress, tabs, menus, overlays, drawing
surfaces, data display, charts, and graph controls.

**Why this priority**: A small set of primitives would still force each product
to rebuild routine widgets. The feature needs enough coverage to let generated
and handwritten applications build real product screens.

**Independent Test**: Review and run the control catalog sample. The catalog is
acceptable when every supported control has a rendered example, documented
properties, documented events, supported visual states, and at least one
interaction test where applicable.

**Acceptance Scenarios**:

1. **Given** a developer needs a standard application form, **When** they open
   the catalog, **Then** they can find text labels, text entry, numeric entry,
   buttons, checkboxes, radio choices, switches, sliders, validation messages,
   and layout containers.
2. **Given** a developer needs data and navigation UI, **When** they open the
   catalog, **Then** they can find list, menu, tab, scroll, progress, chart,
   graph, and table-like display controls.
3. **Given** a catalog example is marked supported, **When** the example runs
   in the reference viewer, **Then** it renders without overlap, missing text,
   unhandled input, or undocumented required setup.
4. **Given** a supported interactive control appears in the catalog, **When**
   maintainers inspect its accessibility metadata, **Then** they can identify
   its role, accessible name source, state metadata, focus behavior, keyboard
   operation, contrast evidence, and diagnostics.

---

### User Story 3 - Configure Controls Declaratively (Priority: P2)

A developer can configure controls with predictable declarative attributes for
content, children, layout, styling, state, validation, and events. Control
composition follows consistent naming and behavior so developers can learn one
pattern and apply it across the catalog.

**Why this priority**: The requested inspiration is a declarative UI style where
control modules expose creation functions and attributes. Consistency matters
more than a large but irregular surface.

**Independent Test**: Pick five unrelated controls from different categories and
build a screen using the same attribute patterns for values, children, style,
layout, and events. The test passes when the controls compose without special
case wiring and their behavior matches their documentation.

**Acceptance Scenarios**:

1. **Given** a developer knows how to create one control, **When** they create a
   different control, **Then** the creation, content, child, style, and event
   patterns remain recognizable.
2. **Given** a control can contain one child or many children, **When** the
   developer supplies content or children, **Then** the library preserves order,
   layout intent, and event delivery for nested controls.
3. **Given** a developer configures styling and layout attributes, **When** the
   same control appears in different containers, **Then** sizing, alignment,
   margins, padding, colors, typography, and visual states are applied
   consistently.

---

### User Story 4 - Maintain Quality Through Tests and Examples (Priority: P2)

A framework maintainer can verify the public control surface, control catalog,
interaction behavior, layout behavior, and generated product usage before
approving changes. New controls cannot be added without examples, tests,
documentation, and public contract evidence.

**Why this priority**: A comprehensive controls library will become a public
surface area. Without governance, control behavior, examples, and documented
contracts will drift quickly.

**Independent Test**: Add a representative control to the catalog and run the
control validation workflow. The workflow passes only when the control has a
public contract, catalog entry, usage example, semantic tests, rendering/layout
evidence, and generated product evidence where applicable.

**Acceptance Scenarios**:

1. **Given** a new supported control is proposed, **When** maintainers review
   it, **Then** they can see its purpose, public surface, visual states,
   events, examples, tests, and compatibility impact.
2. **Given** a control changes behavior, **When** validation runs, **Then** the
   change reports any public surface drift, catalog drift, interaction
   regression, or visual/layout regression.
3. **Given** generated products include the controls capability, **When**
   generated verification runs, **Then** it proves that a product can reference
   and use representative controls without copying framework samples or
   framework implementation projects.
4. **Given** a developer creates the default generated app profile, **When**
   generation completes, **Then** the product includes the controls capability
   and a small product-owned example view that demonstrates representative
   controls.
5. **Given** generated products include the controls capability, **When**
   selected local skills are copied, **Then** widget, layout-control, chart,
   and graph guidance comes from `fs-skia-ui-widgets` instead of separate
   chart or layout guidance skills.
6. **Given** controls use layout behavior, **When** package and skill ownership
   are reviewed, **Then** the Layout runtime capability remains separate while
   generated product layout-control guidance comes from `fs-skia-ui-widgets`.

---

### User Story 5 - Extend With Custom Controls (Priority: P3)

An advanced developer can wrap custom Skia scene elements or product-specific
widgets so they participate in the same declarative composition, layout, event,
focus, and testing conventions as built-in controls.

**Why this priority**: A comprehensive catalog cannot anticipate every product
need. Extension points prevent users from leaving the library's composition
model for custom widgets.

**Independent Test**: Build a custom control wrapper with custom rendering and
one interaction event, then place it beside built-in controls in a reference
screen. The test passes when the custom control lays out, renders, receives
input, dispatches messages, and appears in test diagnostics like built-in
controls.

**Acceptance Scenarios**:

1. **Given** a product has a custom visual element, **When** a developer wraps
   it as a control, **Then** it can be composed with built-in controls in the
   same view function.
2. **Given** a custom control declares input behavior, **When** users interact
   with it, **Then** the control can dispatch application messages and expose
   diagnostics for tests.
3. **Given** a custom control is missing required layout or input metadata,
   **When** validation runs, **Then** diagnostics identify the missing metadata
   before the control is treated as supported.

### Edge Cases

- A control is disabled, hidden, read-only, or loading while still present in
  the view tree.
- Text is empty, very long, multi-line, right-to-left, or larger than its
  container.
- Plain text entry uses single-line and multi-line editing, cursor movement,
  text selection, clipboard commands, validation feedback, and
  environment-aware IME or composition diagnostics.
- Nested containers contain overlapping children, zero-size children, or
  children with conflicting layout requests.
- Pointer events occur near control boundaries or on overlapping controls.
- Keyboard focus moves through disabled controls, nested controls, popups, and
  editable text.
- A control has no accessible name, invalid role/state metadata, unreachable
  focus order, keyboard-only operation gap, or insufficient contrast.
- A list or table contains up to 10,000 items and changes scrolling, selection,
  or item content while the model updates.
- A handler depends on model values that change between renders.
- A control has transient hover, pressed, focus, caret, drag, or composition
  state while persistent values remain model-owned.
- A platform lacks expected GPU, font, clipboard, text input, or window-system
  support.
- Default generated products include the controls capability and product-owned
  controls example while excluding optional framework samples.
- A user or generated product previously selected Charts as a separate
  capability after Controls has become the public owner for charts and graphs.
- A generated product previously received separate chart or layout guidance
  skills after widget guidance has been consolidated into `fs-skia-ui-widgets`.
- A generated product uses controls that depend on the Layout runtime
  capability while receiving widget guidance rather than a separate layout
  skill.
- Visual examples run at different viewport sizes, DPI scale factors, and color
  themes.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST provide a declarative control catalog that can be
  used from an Elmish-style view function.
- **FR-002**: The control catalog MUST include supported controls for text,
  buttons, toggles, editable text, numeric input, selection, lists, panels,
  borders, scrolling, progress, sliders, tabs, menus, overlays, data display,
  charts, graphs, image or drawing surfaces, and layout composition.
- **FR-003**: Every supported control MUST have a documented purpose, supported
  attributes, supported events, supported visual states, examples, and tests.
- **FR-004**: Every supported control MUST expose a consistent creation pattern
  that accepts declarative attributes.
- **FR-005**: Controls that display content MUST support single content or child
  collection composition where appropriate.
- **FR-006**: Controls that accept user input MUST expose message-oriented event
  hooks that can dispatch application messages without app-owned widget
  lifecycle management.
- **FR-007**: Controls MUST reflect model-driven state changes for displayed
  values, selected values, enabled state, visibility, validation state, focus
  state, hover state, pressed state, and loading state where applicable.
- **FR-007a**: Persistent control values, selected values, validation state, and
  committed text MUST be owned by the application model; controls MAY retain
  only transient interaction state such as hover, pressed, focus, caret, active
  drag, or in-progress text composition.
- **FR-008**: The library MUST provide consistent layout attributes for sizing,
  alignment, margins, padding, ordering, clipping, and container participation.
- **FR-009**: The library MUST provide consistent styling attributes for color,
  typography, stroke, fill, corner treatment, spacing, density, and visual
  state variants where applicable.
- **FR-010**: The library MUST support application-level themes and per-control
  overrides without forcing users to duplicate style definitions across
  controls.
- **FR-011**: Interactive controls MUST support pointer interaction, keyboard
  interaction, focus traversal, focus indicators, and disabled/read-only
  behavior where applicable.
- **FR-012**: Editable text controls MUST support plain single-line and
  multi-line text entry, cursor movement, text selection, clipboard actions,
  validation feedback, committed value changes, cancellation or rejection of
  invalid input, and environment-aware diagnostics for unavailable IME or
  composition support.
- **FR-013**: Selection controls MUST support single-selection and
  multiple-selection scenarios where the control category requires them.
- **FR-014**: List and table-like controls MUST support up to 10,000 items with
  responsive scrolling, predictable selection, empty state, and item update
  behavior.
- **FR-015**: Controls MUST provide diagnostics that identify missing required
  attributes, unsupported state combinations, failed hit testing, layout
  conflicts, and unsupported environment conditions.
- **FR-016**: Supported controls MUST expose accessibility role, accessible name
  source, state metadata, focus order, keyboard operation behavior, and contrast
  evidence; validation MUST report missing metadata, keyboard-only operation
  gaps, and contrast failures.
- **FR-017**: The control catalog MUST include a runnable reference gallery that
  exercises each control, its common states, and at least one interaction path
  for interactive controls.
- **FR-018**: The library MUST provide a supported way to wrap custom controls
  so they participate in composition, layout, rendering, input, focus, and test
  diagnostics.
- **FR-019**: The controls capability MUST be selectable by generated products
  without copying framework samples, framework galleries, historical specs,
  framework readiness evidence, or framework implementation projects.
- **FR-020**: The default generated app profile MUST include the controls
  capability, concise controls guidance, and at least one product-owned example
  view that demonstrates representative controls.
- **FR-021**: The controls capability MUST declare its prerequisites and must
  not require unrelated optional capabilities unless a selected control category
  depends on them.
- **FR-022**: The public control surface MUST be tracked with contract evidence
  so maintainers can review additions, removals, and behavior changes.
- **FR-023**: Charts and graphs MUST be owned by the controls capability. The
  separate Charts capability, Charts package, chart template fragment, and
  chart-specific local skill MUST be removed from generated capability
  selection and replaced by controls-owned chart and graph contracts, examples,
  tests, generated guidance, and evidence.
- **FR-024**: Local agent skill guidance for controls, widgets,
  layout-oriented controls, charts, and graphs MUST be consolidated into a
  single `fs-skia-ui-widgets` skill. Generated products MUST NOT receive
  separate `fs-skia-charts` or `fs-skia-layout` skills after this feature, and
  related Scene, SkiaViewer, Elmish, KeyboardInput, and Testing skills MUST
  point widget/control work to `fs-skia-ui-widgets` where applicable.
- **FR-025**: Control validation MUST cover semantic behavior, interaction
  dispatch, layout participation, rendering output, catalog examples, and
  generated product usage.
- **FR-026**: The library MUST preserve compatibility guidance for existing
  users of lower-level scene, viewer, and chart primitives by documenting how
  controls compose with, replace, or absorb those primitives.
- **FR-027**: The feature MUST NOT introduce a new renderer backend, a new
  platform support promise, a designer tool, rich text editing, or release
  publishing automation.

### Change Classification

- **Tier**: Tier 1 (contracted public surface and visual behavior change).
- **Rationale**: This feature introduces a broad reusable controls capability,
  expands the public authoring surface, changes generated product capability
  selection, and adds visual, interaction, layout, and documentation evidence.
- **Public API impact**: New public control contracts and surface baselines are
  expected for the controls capability, along with representative generated
  product usage.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: This feature is expected to add a controls/widgets
  capability package or equivalent capability-owned package contents, default
  generated app references, package metadata, and package surface baselines. It
  must absorb chart and graph ownership from the existing Charts capability and
  remove the separate Charts package, chart template fragment, chart capability
  catalog entry, and chart-specific generated skill from active capability
  selection. Controls must declare dependencies on scene, layout, viewer,
  Elmish, or input capabilities only where required by the selected control set.
  The Layout runtime capability and package remain separate; only
  layout-control guidance and generated skill selection move under
  `fs-skia-ui-widgets`.
- **Public contract impact**: This feature changes documented public APIs,
  `.fsi` signatures, sample contracts, generated product examples, and surface
  baselines for every supported control and custom-control extension point.
- **State workflow impact**: This feature changes user interaction workflow by
  adding message-oriented control events, focus behavior, input state,
  validation state, and subscriptions or effects needed by interactive controls.
  Persistent application state must remain owned by the Elmish model/update
  loop; controls may retain only transient interaction state needed to process
  the active interaction.
- **Layout/rendering impact**: This feature changes Skia visual output, widget
  layout, hit testing, focus rendering, chart and graph display usage,
  screenshot evidence, contrast evidence, unsupported environment diagnostics,
  and reference catalog output.
- **Evidence obligations**: Required real evidence includes
  `specs/010-skia-controls-library/readiness/control-catalog.md`,
  `specs/010-skia-controls-library/readiness/public-surface.md`,
  `specs/010-skia-controls-library/readiness/semantic-tests.md`,
  `specs/010-skia-controls-library/readiness/interaction-tests.md`,
  `specs/010-skia-controls-library/readiness/layout-rendering.md`,
  `specs/010-skia-controls-library/readiness/generated-product-usage.md`,
  `specs/010-skia-controls-library/readiness/local-skills.md`,
  `specs/010-skia-controls-library/readiness/dependency-report.md`,
  `specs/010-skia-controls-library/readiness/generated-guidance.md`,
  `specs/010-skia-controls-library/readiness/template-drift.md`, and evidence
  graph/audit reports.
- **Unsupported scope**: New renderer backends, new operating-system support
  promises, a visual designer, rich text editing, platform-native widget
  wrappers, release publishing automation, formal accessibility certification,
  and wholesale replacement of lower-level scene/viewer APIs are out of scope.
- **Build-target impact**: `Dev`, `Verify`, `Ci`, `PackLocal`,
  `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`,
  `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` are expected to include
  controls-specific validation or evidence once this feature is implemented.
  Skill validation must also prove that `fs-skia-ui-widgets` replaces separate
  chart and layout guidance in generated products.

### Key Entities

- **Control**: A reusable visual and interactive UI element that can be
  composed in the view tree and configured with declarative attributes.
- **Control Attribute**: A declarative value that configures content, children,
  layout, style, visual state, validation, or event behavior for a control.
- **Control Catalog**: The maintained list of supported controls, their
  categories, examples, public contracts, visual states, events, tests, and
  evidence.
- **Chart And Graph Controls**: Controls-owned data visualization widgets that
  replace the separate Charts capability and appear in the control catalog,
  generated product examples, public contracts, tests, and evidence.
- **Widgets Skill**: The `fs-skia-ui-widgets` local agent skill that owns
  guidance for controls, widgets, layout-oriented controls, chart controls,
  graph controls, catalog examples, generated product widget guidance, and
  related evidence.
- **Layout Runtime Capability**: The existing Layout package/capability that
  remains separate for layout engine behavior while controls own user-facing
  layout-control guidance through `fs-skia-ui-widgets`.
- **Large Data Control**: A list or table-like control that must maintain
  responsive scrolling, selection, empty state, and item update behavior with up
  to 10,000 items.
- **Reference Gallery**: A runnable product-like view that demonstrates every
  supported control and common state for review, tests, and documentation.
- **Control Event**: A user interaction declaration that maps pointer, keyboard,
  text, focus, or selection activity to application messages.
- **Persistent Control State**: Model-owned values such as committed text,
  selected items, validation results, and checked or toggled values.
- **Transient Interaction State**: Control-owned short-lived state such as
  hover, pressed, focus, caret, active drag, or in-progress text composition.
- **Theme**: A reusable set of visual choices for colors, typography, spacing,
  density, strokes, fills, and state variants.
- **Plain Text Entry**: Single-line or multi-line text input without rich text
  formatting spans, with cursor, selection, clipboard, validation, and
  environment-aware composition diagnostics.
- **Accessibility Metadata**: Control role, accessible name source, state
  metadata, focus order, keyboard operation behavior, contrast evidence, and
  diagnostics for missing or invalid metadata.
- **Custom Control Wrapper**: A supported extension that lets product-specific
  widgets participate in the same composition, layout, rendering, input, and
  diagnostics model as built-in controls.

### Assumptions

- The controls library is a selectable capability that is included in the
  default generated app profile and builds on the V3 modular framework
  direction rather than replacing Scene, SkiaViewer, Elmish, Layout,
  or KeyboardInput.
- Charts and graphs are no longer a separate public capability after this
  feature; their public ownership moves into the controls library.
- Local skill structure follows the new widget ownership model: one
  `fs-skia-ui-widgets` skill replaces separate chart and layout guidance for
  generated products, while non-widget skills continue to own Scene,
  SkiaViewer, Elmish, KeyboardInput, and Testing boundaries.
- Layout remains a separate runtime capability and package; the consolidation
  affects generated-product guidance and widget/control authoring, not the
  lower-level layout engine boundary.
- The first comprehensive release should prioritize common product application
  controls and a coherent catalog over pixel-perfect parity with any existing
  desktop UI framework.
- Inspiration from Avalonia.FuncUI means consistent declarative creation,
  property/event attributes, content/children composition, and catalog-driven
  examples, adapted to this project's Skia and Elmish model.
- Persistent control values remain model-owned so the view function is a pure
  projection of the current model plus dispatchable actions.
- Accessibility role/name/state metadata, focus order, keyboard operation,
  contrast checks, and diagnostics are required; formal accessibility
  certification is outside this feature.
- Visual validation can distinguish product defects from unsupported local GPU,
  font, text-input, clipboard, or window-system environments.
- Rich text editing and formatting spans are deferred beyond the first
  supported catalog.

## Success Criteria *(mandatory)*

- **SC-001**: A developer can build a representative form-and-dashboard screen
  with at least 10 distinct control types, 3 nested layout regions, and 5 user
  interactions in under 30 minutes using the catalog documentation.
- **SC-002**: The initial supported catalog contains at least 30 documented
  controls or control variants across input, display, selection, navigation,
  layout, feedback, data, chart, and graph categories.
- **SC-003**: 100% of supported controls have a catalog entry with purpose,
  required attributes, common attributes, events, visual states, and at least
  one runnable example.
- **SC-004**: 95% of catalog interaction tests dispatch exactly the expected
  application message and payload for the exercised user action.
- **SC-005**: List and table-like validation proves 10,000-item data sets use
  bounded visible-range rendering, complete visible-range recalculation within
  an implementation-defined threshold recorded in readiness evidence, and
  preserve correct scrolling, selection, empty-state, and item-update behavior.
- **SC-006**: Reference gallery validation passes at three viewport sizes and
  two scale factors with no unintended text clipping, uncontrolled overlap, or
  missing visual states in supported examples.
- **SC-007**: Default generated products verify successfully with the controls
  capability, a product-owned example view, and no copied framework samples or
  framework implementation projects.
- **SC-008**: Public surface review identifies every added, changed, or removed
  supported control member before the feature can be marked ready for
  implementation completion.
- **SC-009**: Generated capability validation no longer exposes Charts as a
  selectable capability and instead verifies chart and graph widgets through
  controls-owned package references, examples, tests, skills, and evidence.
- **SC-010**: Selected-skill validation proves that generated products with
  controls receive `fs-skia-ui-widgets` and do not receive separate
  `fs-skia-charts` or `fs-skia-layout` skills.
- **SC-011**: Dependency validation proves that Layout remains a separate
  runtime capability when controls require layout engine behavior, while
  generated local guidance still comes from `fs-skia-ui-widgets`.
- **SC-012**: 100% of supported interactive controls declare accessibility
  metadata and keyboard operation expectations, and validation reports missing
  metadata or contrast failures before readiness approval.
- **SC-013**: A maintainer-review walkthrough with at least 5 first-time
  evaluators records whether each evaluator can locate the correct control and
  event pattern for a simple form task without reading source files; at least 4
  of 5 must succeed, and the walkthrough notes are captured in readiness
  evidence.
