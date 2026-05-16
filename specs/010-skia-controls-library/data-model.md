# Data Model: Skia Controls Library

## Controls Capability

- **Fields**: capability id, display name, package id, project path, public
  contract files, test projects, package skill source path, generated skill
  destination, template fragment path, dependency capability ids, supported
  profiles, default app inclusion, evidence classes, surface baseline path,
  docs path, owner notes
- **Relationships**: Belongs to `CapabilityCatalog`; owns `ControlCatalog`,
  `ControlDefinition` entries, `WidgetsSkill`, template fragment, package
  surface baseline, generated product example, and validation reports; depends
  on Scene, Layout, and KeyboardInput where needed.
- **Validation Rules**: Must be included in the default generated app. Must
  replace Charts in active capability selection. Must declare contracts, tests,
  skill, fragment, dependencies, docs, and evidence. Dependency references must
  exist and must not create cycles.

## Control

- **Fields**: id/key, control kind, attributes, content, children, layout
  participation, style references, visual state, accessibility metadata,
  diagnostics, optional catalog id
- **Relationships**: Created by a control module; composed into a `ControlTree`;
  emits `ControlEvent` values; renders to Scene/Layout output; owns
  `TransientInteractionState` by key only.
- **Validation Rules**: Supported controls must have catalog metadata,
  documented attributes and events, examples, semantic tests, interaction tests
  where applicable, accessibility metadata, and rendering/layout evidence.
  Persistent values must be supplied by the application model.

## Control Attribute

- **Fields**: name, owning control or common group, value type, required flag,
  category, default behavior, validation rule, documentation text
- **Relationships**: Belongs to a `ControlDefinition`; configures content,
  children, layout, style, theme, state, validation, accessibility, or events.
- **Validation Rules**: Public attributes must appear in `.fsi` signatures,
  catalog metadata, examples, and surface baselines. Required attributes must
  produce diagnostics when missing.

## Control Tree

- **Fields**: root control, ordered children, keyed state map, layout result,
  render result, diagnostics, pending event messages
- **Relationships**: Produced by the Elmish-style view function; consumes
  application model values; is evaluated by layout/render/input validation.
- **Validation Rules**: Child order must be stable. Key collisions must be
  diagnosed. Layout conflicts, clipping, hit-test ambiguity, and missing
  accessibility metadata must be reported with control ids.

## Control Catalog

- **Fields**: schema version, catalog version, supported control ids,
  categories, required fields, example paths, test paths, evidence paths,
  compatibility notes
- **Relationships**: Owns many `ControlDefinition` entries; drives reference
  gallery, docs, examples, validation, generated product sample selection, and
  readiness reports.
- **Validation Rules**: Must contain at least 30 supported controls or variants
  for the first release. Every supported row must declare purpose, attributes,
  events, visual states, accessibility metadata, examples, tests, and evidence.

## Control Definition

- **Fields**: control id, display name, category, purpose, supported variants,
  required attributes, common attributes, supported events, visual states,
  accessibility role/name/state metadata, keyboard behavior, example path,
  test path, evidence path, compatibility impact
- **Relationships**: Belongs to `ControlCatalog`; maps to public control module
  members and examples; contributes to `ReferenceGallery`.
- **Validation Rules**: A row marked supported must compile through public
  `.fsi`, appear in the gallery, have tests, and have evidence. Unsupported or
  experimental rows must not be counted toward success criteria.

## Control Event

- **Fields**: event kind, source control id, payload, message factory or
  message value, dispatch policy, keyboard/pointer/text/focus origin,
  diagnostics
- **Relationships**: Declared by event attributes; consumed by Elmish dispatch;
  covered by interaction tests.
- **Validation Rules**: Interactive controls dispatch exactly one expected
  application message for the tested action. Disabled/read-only controls must
  not dispatch disallowed events.

## Persistent Control State

- **Fields**: committed text, selected values, validation results, checked or
  toggled values, slider/numeric values, expanded or active model-owned state
- **Relationships**: Owned by the application `Model`; provided to controls as
  attributes; updated by application `Msg` handling.
- **Validation Rules**: Must not be stored as durable state inside controls.
  Re-rendering from a changed model must update displayed values and states.

## Transient Interaction State

- **Fields**: hover state, pressed state, focus state, caret location,
  selection drag, active drag value, in-progress composition text,
  pointer capture, last environment diagnostic
- **Relationships**: Owned internally by the Controls runtime and keyed by
  stable control identity; reset when interaction ends or key/type changes.
- **Validation Rules**: Must not contain committed application values.
  Missing keys for controls that need durable transient interaction must be
  diagnosed.

## Theme

- **Fields**: colors, typography, spacing, density, strokes, fills, corner
  treatment, state variants, contrast policy
- **Relationships**: Applied globally or overridden per control; contributes to
  contrast evidence and reference gallery output.
- **Validation Rules**: Theme overrides must be deterministic and must not
  require duplicating style definitions across controls. Contrast failures must
  be reported for supported interactive controls.

## Accessibility Metadata

- **Fields**: role, accessible name source, state metadata, focus order,
  keyboard operation behavior, contrast evidence, diagnostics
- **Relationships**: Attached to supported controls and catalog rows; validated
  by accessibility tests and readiness reports.
- **Validation Rules**: Interactive supported controls require role, name
  source, state metadata, keyboard behavior, focus order, and contrast evidence.
  Missing or invalid metadata fails validation.

## Text Input Session

- **Fields**: control id, mode, committed text, caret index, selection range,
  pending composition text, validation result, clipboard command, environment
  support diagnostics
- **Relationships**: Backed by transient state and model-owned committed value;
  consumes KeyboardInput/viewer edge events; emits text change or validation
  messages.
- **Validation Rules**: Supports plain single-line and multi-line editing,
  cursor movement, selection, clipboard actions, validation, commit/cancel
  behavior, and environment-aware IME/composition diagnostics.

## Large Data Viewport

- **Fields**: total item count, visible range, scroll offset, selected ids,
  focused row, item update set, empty state, diagnostics
- **Relationships**: Used by list and table-like controls; consumes model-owned
  items and selection; contributes to rendering and interaction evidence.
- **Validation Rules**: Must handle 10,000 items with responsive scrolling,
  selection, empty state, and item update behavior without requiring all items
  to be rendered at once.

## Chart And Graph Control

- **Fields**: chart or graph type, data series, axes, labels, legends,
  selection/highlight state, interaction events, accessibility summary,
  compatibility note
- **Relationships**: Owned by Controls; replaces active Charts capability
  ownership; appears in catalog, generated examples, tests, and evidence.
- **Validation Rules**: Must not require generated products to select a
  separate Charts capability or `fs-skia-charts` skill. Compatibility impact
  must document the replacement path.

## Custom Control Wrapper

- **Fields**: wrapper id, render function, layout function, hit-test function,
  event mapper, accessibility metadata, diagnostics, supported state
- **Relationships**: Lets product-specific widgets participate in the same
  control tree as built-in controls; appears in tests and diagnostics.
- **Validation Rules**: Missing layout, input, accessibility, or diagnostic
  metadata must fail validation before the wrapper is treated as supported.

## Reference Gallery

- **Fields**: gallery project path, catalog rows covered, example screens,
  viewport sizes, scale factors, screenshots or render evidence, interaction
  scripts, diagnostics
- **Relationships**: Generated from or validated against `ControlCatalog`;
  produces readiness evidence and documentation examples.
- **Validation Rules**: Every supported control has a rendered example and at
  least one interaction path where applicable. Gallery validation covers three
  viewport sizes and two scale factors.

## Widgets Skill

- **Fields**: skill name, source path, generated destination, scope, public
  contract guidance, build commands, test commands, evidence rules, generated
  product guidance, package boundary notes
- **Relationships**: Owned by Controls; copied into generated products when
  Controls or widget guidance is selected; replaces generated chart and
  layout-control guidance skills.
- **Validation Rules**: Generated products must receive `fs-skia-ui-widgets`
  and must not receive `fs-skia-charts` or `fs-skia-layout` after this feature.
  Related skills must direct widget/control work to this skill.

## Validation Report

- **Fields**: report class, path, produced by command, covered control or
  capability, pass/fail verdict, observed files, missing metadata, unexpected
  references, environment diagnostics, notes
- **Relationships**: Evidence for Controls, catalog, interactions,
  accessibility, rendering, generated products, dependency ownership, skills,
  template drift, and compatibility impact.
- **Validation Rules**: Failures must identify the control/capability/profile
  and missing or unexpected artifact. Unsupported environment reports must be
  explicit and distinguish environment limitations from implementation defects.
