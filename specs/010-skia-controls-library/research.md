# Research: Skia Controls Library

No unresolved clarifications remain from the specification. The decisions below
resolve the planning unknowns and set implementation boundaries.

## FuncUI-Inspired Authoring Shape

**Decision**: Controls use module-per-control creation functions and
declarative attributes: for example `Button.create [ Button.text "...";
Button.onClick Msg ]` and container attributes such as `Stack.children [...]`.
This follows the documented FuncUI pattern where control modules expose
`create` functions accepting attribute lists, attributes cover properties and
events, and content properties cover single views or child lists:
https://funcui.avaloniaui.net/view-basics/creating-views and
https://funcui.avaloniaui.net/view-basics/attributes.

**Rationale**: The requested inspiration is Avalonia.FuncUI. The pattern is
familiar to F# Elmish users, keeps views as values, and gives every control a
consistent creation/content/event style.

**Alternatives considered**: Object-oriented builders were rejected because they
would hide public surface in mutable objects. A single untyped attribute bag was
rejected because `.fsi` contracts and FSI usage would be weaker. Copying
Avalonia-specific backing-control concepts directly was rejected because this
project renders Skia scene/layout data, not Avalonia controls.

## View Lifetime And Transient State

**Decision**: A control view is a pure description. Persistent values live in
the application model. Controls retain only keyed transient state for active
interaction: hover, pressed, focus, caret, drag, and in-progress text
composition. Stable keys are required for controls whose transient state must
survive view recreation.

**Rationale**: FuncUI documents patching and keyed views as the mechanism that
preserves backing view state across structural updates:
https://funcui.avaloniaui.net/view-basics/lifetime. FS.Skia.UI needs the same
practical concept, adapted to a Skia control tree and model-owned persistent
state.

**Alternatives considered**: Recreating all state on every render was rejected
because text editing, focus, and drag interactions would break. Allowing
controls to own committed values was rejected by clarification and would
violate Elmish/MVU expectations.

## Event Contract

**Decision**: Interactive attributes produce application messages or message
factories, not imperative callbacks that mutate widget instances. Event
bindings must be refreshed from the current view description so handlers cannot
silently capture stale model values.

**Rationale**: FuncUI attributes treat events as attributes and warns about
captured state in event handlers. This project can avoid that failure mode by
making message production explicit and validating dispatch exactly once per
exercised interaction.

**Alternatives considered**: Raw callback subscriptions were rejected because
they make stale captured state and duplicate dispatch harder to detect. A
global event bus was rejected because it obscures control ownership and weakens
semantic tests.

## Controls Package Boundary

**Decision**: Introduce `src/Controls/Controls.fsproj` with package id
`FS.Skia.UI.Controls`. The package owns control nodes, attributes, themes,
accessibility metadata, diagnostics, text-entry support, collection controls,
chart/graph controls, catalog metadata, custom wrappers, examples, package
skill, tests, and surface baseline.

**Rationale**: Controls is a large public authoring surface and should be
reviewable as one capability. It can depend on lower-level capabilities without
forcing users to learn lower-level primitives first.

**Alternatives considered**: Adding controls to Scene was rejected because
Scene must remain dependency-light. Adding controls to Elmish was rejected
because controls can remain generic over message type and should not require a
specific Elmish runtime package. Keeping Charts separate was rejected by
clarification.

## Layout And Input Dependencies

**Decision**: Controls may depend on Scene, Layout, and KeyboardInput. Layout
stays a separate runtime package. SkiaViewer and Elmish remain selected by the
default generated app profile but are not required as direct Controls package
dependencies unless implementation proves the public API needs them.

**Rationale**: Controls need scene output, layout participation, focus and
keyboard behavior, and text-input diagnostics. Keeping the viewer and Elmish
runtime out of the Controls package avoids coupling the reusable control DSL to
one host loop.

**Alternatives considered**: Making Controls depend on every default app
capability was rejected because it would create unnecessary dependency leaks.
Moving Layout into Controls was rejected by clarification.

## Chart And Graph Absorption

**Decision**: Move chart, graph, and table-like data display ownership into
Controls. The generated capability catalog must no longer expose `charts`, the
default generated app must reference `FS.Skia.UI.Controls` instead of
`FS.Skia.UI.Charts`, and generated products must receive `fs-skia-ui-widgets`
instead of `fs-skia-charts`.

**Rationale**: The user clarified that Controls fully absorbs charts and
graphs now. Keeping Charts selectable would leave two public owners for the
same widget category.

**Alternatives considered**: Keeping Charts as an optional add-on was rejected
by clarification. Providing only adapter modules was rejected because
generated capability selection and skills would still expose stale ownership.

## Text Entry Scope

**Decision**: The first text-entry release supports plain single-line and
multi-line text entry, cursor movement, text selection, clipboard commands,
validation feedback, committed value changes, cancellation or rejection of
invalid input, and environment-aware IME/composition diagnostics.

**Rationale**: This matches the clarified feature scope while avoiding rich
text, formatting spans, and platform-native widget wrappers.

**Alternatives considered**: Deferring text entry was rejected because forms
are a core controls use case. Rich text was rejected as explicitly out of
scope.

## Accessibility And Diagnostics

**Decision**: Every supported interactive control declares role, accessible
name source, state metadata, focus order, keyboard operation, and contrast
evidence. Validation reports missing metadata, unreachable focus paths,
keyboard-only operation gaps, and contrast failures.

**Rationale**: The first release requires accessibility metadata and
diagnostics, not formal certification. Diagnostics make unsupported or missing
coverage visible before readiness approval.

**Alternatives considered**: Treating accessibility as documentation-only was
rejected because it would not catch catalog drift. Formal certification was
rejected as out of scope.

## Large Data Controls

**Decision**: List and table-like controls must virtualize or otherwise limit
work to visible ranges and update changed items predictably while supporting up
to 10,000 items in reference validation.

**Rationale**: The spec requires responsive scrolling, selection, and item
updates at 10,000 items. A visible-range contract lets tests assert behavior
without requiring a specific UI virtualization implementation detail.

**Alternatives considered**: Rendering all 10,000 items as scene nodes was
rejected because it risks predictable performance failures. Deferring table
support was rejected by clarified scale requirements.

## Catalog And Evidence Ownership

**Decision**: Maintain a machine-readable control catalog under Controls
ownership. A supported control is not considered supported until the catalog
declares purpose, attributes, events, visual states, accessibility metadata,
example path, tests, evidence paths, and compatibility notes where applicable.

**Rationale**: A comprehensive catalog will drift without a structured source
of truth. Catalog validation gives maintainers a concrete review surface.

**Alternatives considered**: Markdown-only catalog entries were rejected
because tests need structured metadata. Inferring catalog data from source was
rejected because examples, screenshots, and evidence paths are not reliably
derivable from signatures alone.

## Generated Product Example

**Decision**: The default generated app profile includes Controls and a small
product-owned example view that demonstrates representative controls. The
example must live in generated product source, not copied framework samples or
gallery code.

**Rationale**: Generated products need immediate proof that package references,
skills, and basic control authoring work without inheriting framework sample
ownership.

**Alternatives considered**: Copying the full reference gallery was rejected
because generated products must stay lean and product-owned. Omitting an
example was rejected by clarification.

## Widget Skill Consolidation

**Decision**: Add one `fs-skia-ui-widgets` skill sourced from Controls
ownership. Generated products with Controls or layout-oriented widgets receive
this skill and must not receive separate `fs-skia-charts` or `fs-skia-layout`
skills. Framework-internal Layout package guidance may remain for engine work,
but generated product widget/control guidance is consolidated.

**Rationale**: The user clarified that local agent guidance should match the
widget structure and avoid separate chart/layout-control guidance in generated
products.

**Alternatives considered**: Keeping existing generated `fs-skia-charts` and
`fs-skia-layout` skills was rejected because it preserves stale ownership.
Merging all project skills into a monolithic skill was rejected because Scene,
SkiaViewer, Elmish, KeyboardInput, and Testing still have distinct boundaries.

## Deferred Scope

**Decision**: Do not add a new renderer backend, new platform support promise,
designer tool, rich text editor, platform-native widget wrapper set, release
publishing automation, or V2 migration implementation.

**Rationale**: These are explicitly outside the feature and would obscure the
controls capability work.

**Alternatives considered**: Including a native widget layer or designer was
rejected because the requested surface is a Skia/Elmish control library for the
view function.
