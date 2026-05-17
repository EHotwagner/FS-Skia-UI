# Research: Controls Boundary Refactor

No unresolved clarifications remain from the specification. The decisions below
resolve the planning unknowns and set the implementation boundary.

## Controls Positioning

**Decision**: Document and validate Controls as a Skia-rendered,
Elmish-shaped controls surface rather than a renderer-neutral widget layer.
The base surface remains generic over product messages, and direct Elmish
`Cmd`, subscription, or program integration is isolated in an adapter surface.

**Rationale**: The spec explicitly asks for Skia/Elmish-specific controls and
advanced Skia capabilities. Keeping ordinary declarations generic over product
messages preserves model-view-update ownership without forcing every caller to
use a specific Elmish `Program` pipeline.

**Alternatives considered**: A renderer-neutral abstraction was rejected
because it would hide rich Skia capabilities and contradict FR-001. Requiring
Elmish runtime integration for ordinary declarations was rejected because it
would couple controls to command/program ownership that belongs at the adapter
edge.

## Stable Records With Skia Escape Hatches

**Decision**: Ordinary controls use stable public records and module functions.
Advanced/custom controls expose explicit Skia escape hatches for rich text,
measurement, direct drawing, clipping, effects, hit testing, diagnostics, and
readback evidence.

**Rationale**: Stable records keep the public surface usable from FSI and
governable through `.fsi` signatures and baselines. Escape hatches make Skia
specificity honest without pushing low-level callbacks into simple controls.

**Alternatives considered**: An opaque untyped attribute bag was rejected
because it weakens contracts and catalog validation. Making every control a raw
Skia callback was rejected because it would lose the cataloged control model
and make ordinary examples harder to test semantically.

## Controls Package Boundary

**Decision**: Controls owns standard controls, rich text controls, chart
controls, graph views, DataGrid, the control catalog, control runtime,
diagnostics, generated controls guidance, and Controls public surface
baselines. Controls may depend on Scene, Layout, and KeyboardInput. Controls
must not depend on the monolithic `src/Lib` or SkiaViewer host loop unless a
contract documents the explicit reason.

**Rationale**: Scene and Layout provide lower-level primitives. KeyboardInput
owns input state. SkiaViewer owns host/viewer lifecycle. Controls should render
Skia output and consume input contracts without inheriting window creation,
application shutdown, or host scheduling responsibilities.

**Alternatives considered**: Keeping Controls coupled to `src/Lib` was rejected
because it hides viewer/runtime dependencies. Moving Controls into Elmish was
rejected because base controls should stay generic over product messages.
Moving Layout into Controls was rejected because lower-level layout remains a
separate usage path.

## Legacy Charts Removal

**Decision**: Remove the legacy `FS.Skia.UI.Charts` package and active Charts
capability entirely. Move or rehome chart, graph, and DataGrid public contracts
under `FS.Skia.UI.Controls`, including catalog rows, examples, tests, samples,
generated guidance, package references, and compatibility documentation.

**Rationale**: The spec clarifies that Charts should not remain a compatibility
package or separate capability. A single Controls owner avoids conflicting
support models for chart, graph, and data controls.

**Alternatives considered**: Keeping a shim package was rejected by FR-007 and
the chart migration edge case. Keeping Charts selectable as an optional
capability was rejected because generated products would keep stale ownership.
Automated migration tooling was rejected as out of scope.

## DataGrid Category

**Decision**: Treat DataGrid as a data or collection control, not as a chart
category. DataGrid catalog metadata, examples, docs, generated guidance, and
tests must all reflect data/collection ownership.

**Rationale**: DataGrid usage is tabular data interaction, selection, focus,
and collection presentation. Categorizing it as chart-only would preserve the
old package boundary and make discovery harder.

**Alternatives considered**: Leaving DataGrid under chart terminology was
rejected by FR-006. Creating a separate DataGrid capability was rejected
because the user asked for one Controls path.

## KeyboardInput Runtime Ownership

**Decision**: Consolidate rich keyboard runtime state, update contracts,
effects, diagnostics, and keyboard state display in
`FS.Skia.UI.KeyboardInput`. Controls and the Elmish adapter consume that public
package surface. The runtime tracks pressed keys, active layout, active mode
stack, persistent mode state, pending sequence, effects, events, and
diagnostics.

**Rationale**: Keyboard input is a stateful MVU submodel. Putting the rich
runtime in the dedicated package gives lower-level users the same contract as
Controls and prevents duplicated definitions.

**Alternatives considered**: Keeping rich keyboard state inside Controls was
rejected because lower-level input users would lose access. Keeping only the
current small `KeyboardModel` was rejected because it cannot satisfy the
specified state display, mode stack, focus recovery, and effect interpretation
requirements.

## Control Runtime Ownership

**Decision**: Add a product-owned `ControlRuntime` submodel for transient
control interaction state: focus, hover, pressed controls, caret/selection,
composition, active drag, recent events/effects, and diagnostics. Persistent
business values such as text, selected rows, active tab, chart data, and
DataGrid data remain outside this runtime.

**Rationale**: The repository constitution requires stateful user interaction
to be modeled explicitly through MVU-style state and effects. Product ownership
makes runtime state inspectable and testable without hiding mutable control
state behind rendering callbacks.

**Alternatives considered**: Internal hidden mutable widget state was rejected
because it weakens diagnostics and recovery tests. Storing committed product
values in `ControlRuntime` was rejected by FR-026.

## Elmish Adapter Shape

**Decision**: Provide direct `Cmd`, subscription, or `Program` integration
through a dedicated adapter. Prefer a split `FS.Skia.UI.Controls.Elmish`
package if the adapter introduces additional package dependencies or package
surface governance; otherwise expose the adapter from the existing
`FS.Skia.UI.Elmish` package with clear module ownership and surface baselines.

**Rationale**: The adapter is the only place that should know how control and
keyboard effects become Elmish commands/subscriptions. The base Controls
package can remain message-oriented and free of host-loop ownership.

**Alternatives considered**: Putting commands in ordinary control attributes
was rejected because it would make simple controls depend on runtime wiring.
Creating a new application host package was rejected because host lifecycle
changes are outside this feature.

## Rich Rendering And Rich Text

**Decision**: Support rich rendering scenarios through Skia-specific public
contracts and evidence: rich text spans or runs, measurement inputs, custom
paint/draw hooks, clipping/effects options, diagnostics, and render/readback or
screenshot evidence where the environment supports it.

**Rationale**: Rich text and precise Skia output are explicit goals of the
refactor. The public surface should not pretend those features are portable to
non-Skia renderers.

**Alternatives considered**: Deferring rich text was rejected because it is a
named reason for the refactor. Implementing a full rich text editor was
rejected because the spec asks for rich rendering capability, not platform
editing widgets.

## Generated Product Guidance

**Decision**: Generated products that select Controls receive Controls package
references, Controls guidance, representative form plus data/chart examples,
and widget guidance from Controls ownership. They must not receive stale
Charts package references, chart-specific active capability guidance, or copied
framework galleries.

**Rationale**: Generated products are the practical consumer contract for the
template. Their guidance needs to match the framework boundary so new users do
not learn a deprecated Charts path.

**Alternatives considered**: Leaving historical chart guidance in generated
products was rejected by FR-009. Copying framework samples was rejected because
generated products must own product source rather than framework evidence.

## Evidence Strategy

**Decision**: Treat this as a Tier 1 contracted refactor with readiness files
for public surface, package boundary, Elmish adapter, KeyboardInput package,
control catalog, control runtime, rich rendering, keyboard input Elmish flow,
chart/DataGrid Controls ownership, generated product usage, dependency report,
template drift, compatibility impact, evidence graph, and evidence audit.

**Rationale**: The refactor removes a package and changes multiple public
contracts. Evidence must make stale references, dependency leaks, and runtime
ownership violations visible.

**Alternatives considered**: Relying only on unit tests was rejected because
template, generated guidance, and package-boundary regressions would not be
covered. Synthetic-only evidence was rejected as primary proof by the
constitution.

## Deferred Scope

**Decision**: Do not add renderer-neutral widgets, new renderer backends,
browser/mobile support, platform-native widget wrappers, formal accessibility
certification, automatic migration of external applications, or release
publishing automation.

**Rationale**: These items are explicitly excluded by the spec and would
obscure the boundary refactor.

**Alternatives considered**: A compatibility migration package and release
automation were rejected because the feature is a repository boundary refactor,
not a published migration program.
