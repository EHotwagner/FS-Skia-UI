# Data Model: Controls Boundary Refactor

## Controls Capability

- **Fields**: capability id, package id, project path, public contract files,
  test projects, dependencies, template fragment, generated guidance, package
  skill, docs path, surface baseline, evidence classes
- **Relationships**: Owns control catalog, standard controls, rich rendering
  controls, chart controls, graph views, DataGrid, control runtime, generated
  controls examples, and controls readiness evidence; depends on Scene,
  Layout, and KeyboardInput
- **Validation Rules**: Must be the active home for ordinary controls, rich
  text, charts, graph views, and DataGrid. Must not depend on the monolithic
  viewer/runtime surface unless a contract and dependency report justify the
  coupling. Must appear in generated products that include application
  controls.

## Stable Control Record

- **Fields**: control id/key, kind, typed attributes, children/content,
  accessibility metadata, layout participation, visual state, diagnostics
- **Relationships**: Declared by product view functions; maps to catalog rows;
  emits product messages through event attributes; renders to Scene/Layout
  output
- **Validation Rules**: Public declarations are governed by `.fsi` contracts
  and surface baselines. Ordinary controls must not require direct Skia
  callbacks, Elmish `Cmd`, or host-loop ownership.

## Skia Escape Hatch

- **Fields**: rendering context descriptor, measurement inputs, draw operation,
  clipping/effects options, hit-test mapping, diagnostics, accessibility
  metadata, evidence path
- **Relationships**: Attached to advanced/custom controls; consumes Skia
  rendering concepts; contributes rich rendering evidence
- **Validation Rules**: Must be explicit in public API and docs. Must not imply
  renderer-neutral portability. Missing layout, hit-test, diagnostics, or
  accessibility metadata fails advanced-control validation.

## Control Runtime

- **Fields**: focused control id, hovered control id, pressed control ids,
  caret/selection state, active text composition, active drag, recent events,
  emitted effects, diagnostics, stale target records
- **Relationships**: Owned by product model; updated by explicit control/input
  messages; consumed by view rendering, diagnostics, and adapter effects
- **Validation Rules**: Contains only transient interaction state. Product
  values such as text contents, selected rows, active tabs, chart data, and
  DataGrid data remain outside the runtime. Focus loss, removed controls,
  cancelled drag, and interrupted composition must produce deterministic
  recovery behavior and diagnostics.

## Control Runtime Message

- **Fields**: source control id, event origin, event kind, payload, timestamp or
  ordering token where needed, recovery intent
- **Relationships**: Drives `ControlRuntime.update`; can be produced by
  pointer, keyboard, focus, text, composition, selection, and drag event
  adapters
- **Validation Rules**: Update must be pure and return next runtime plus
  inspectable effects/diagnostics. Stale target and disabled/read-only paths
  must not mutate product business values.

## Control Runtime Effect

- **Fields**: effect kind, target control id, product message mapping,
  diagnostic payload, host effect request, recovery metadata
- **Relationships**: Interpreted by product update workflow or Elmish adapter;
  recorded in readiness evidence
- **Validation Rules**: Effects are data until interpreted at the edge. Host
  effects must not be executed inside base Controls update logic.

## Keyboard Input Runtime

- **Fields**: pressed keys, active layout, active mode stack, persistent mode
  state, temporary held layers, pending sequence, recent events, emitted
  effects, diagnostics
- **Relationships**: Owned by product model; exposed from
  `FS.Skia.UI.KeyboardInput`; consumed by Controls and Elmish adapter; rendered
  by keyboard state display
- **Validation Rules**: Key down/up and focus loss are pure transitions.
  Focus loss clears pressed keys and temporary held layers while preserving
  persistent mode state unless the product explicitly resets the runtime.

## Keyboard Input Effect

- **Fields**: resolved command id, key state change, mode transition,
  diagnostic, host effect request, control message bridge
- **Relationships**: Returned by KeyboardInput update; interpreted by product
  update workflow or Elmish adapter
- **Validation Rules**: Effects must be inspectable. Product workflows decide
  whether they become commands, product messages, control messages,
  diagnostics, or explicit host effects.

## Keyboard State Display

- **Fields**: visible pressed keys, active layout label, mode stack label,
  persistent mode state summary, pending sequence, recent effects, diagnostics
- **Relationships**: Rendered from current KeyboardInput runtime state and
  recent effects; may appear in samples/generated examples
- **Validation Rules**: Must not depend on hidden mutable state, renderer
  callbacks, or ownership of the application loop.

## Elmish Adapter

- **Fields**: adapter package or module id, input effect interpreter, control
  effect interpreter, command bridge, subscription bridge, program wiring
  helpers, diagnostics
- **Relationships**: Consumes Controls and KeyboardInput public contracts;
  depends on Fable.Elmish if command/program types are exposed
- **Validation Rules**: Direct command, subscription, or program integration
  lives here rather than in ordinary Controls declarations. Tests cover generic
  message-based flow and adapter flow separately.

## Chart Control

- **Fields**: chart type, data series, axes, labels, legend, selection or
  highlight state, interaction events, accessibility summary, compatibility
  notes
- **Relationships**: Owned by Controls catalog and public API; replaces legacy
  Charts package modules; appears in examples, generated guidance, and
  readiness evidence
- **Validation Rules**: Must not require `FS.Skia.UI.Charts` package,
  `charts` capability, or chart-specific generated skill. Migration guidance
  documents the replacement path.

## Graph View Control

- **Fields**: graph nodes, edges, layout options, selected/highlighted ids,
  interaction events, accessibility summary, diagnostics
- **Relationships**: Owned by Controls catalog; may consume lower-level Layout
  graph helpers where appropriate
- **Validation Rules**: Must appear as Controls-owned guidance and examples,
  not as a separate chart capability.

## DataGrid Control

- **Fields**: column definitions, row identity, visible range, selected rows,
  focused cell/row, sort/filter metadata, cell renderers, accessibility role,
  diagnostics
- **Relationships**: Owned by Controls as a data/collection control; consumes
  product-owned row data and selection state
- **Validation Rules**: Must be categorized as data or collection control.
  Must not remain discoverable only through chart terminology. Large-row
  validation must avoid rendering all rows as live scene nodes.

## Legacy Charts Package

- **Fields**: package id, project path, solution entry, tests, samples, surface
  baseline, generated package references, capability/guidance references
- **Relationships**: Replaced by Controls-owned chart/DataGrid contracts
- **Validation Rules**: Must be removed from active capability selection,
  generated products, package surface checks, dependency reports, and guidance.
  Migration documentation remains, but no compatibility package or automated
  migration is promised.

## Generated Product Profile

- **Fields**: selected capability ids, package references, copied skills,
  product source files, product tests, generated guidance, validation logs
- **Relationships**: Produced by template; consumes Controls, KeyboardInput,
  Elmish adapter, Scene, SkiaViewer, and Layout according to profile
- **Validation Rules**: Controls profiles include form plus data/chart example
  usage. Generated products exclude framework implementation source, samples,
  historical specs, and readiness evidence. Stale Charts references fail
  generated guidance checks.

## Boundary Evidence Record

- **Fields**: evidence path, producer command, covered package/capability,
  observed files, pass/fail verdict, stale references, dependency leaks,
  unsupported environment diagnostics, notes
- **Relationships**: Written under
  `specs/011-controls-boundary-refactor/readiness/`; referenced by tasks,
  contracts, quickstart, and evidence audit
- **Validation Rules**: Failures identify the affected control, package,
  capability, profile, adapter contract, runtime state, or guidance file.
  Synthetic evidence must follow constitution disclosure rules.
