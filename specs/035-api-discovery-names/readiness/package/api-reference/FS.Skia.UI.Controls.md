# FS.Skia.UI.Controls Source-Shaped API Reference

package-id: FS.Skia.UI.Controls
package-version: local
generated-from: curated-fsi
assembly-reflection: false
repository-source-authoring-fallback: false
symbol-count: 762
xml-summary-count: 643
source-fsi-paths:
- src/Controls/Accessibility.fsi
- src/Controls/Attributes.fsi
- src/Controls/Catalog.fsi
- src/Controls/Charts.fsi
- src/Controls/Collections.fsi
- src/Controls/Control.fsi
- src/Controls/ControlRuntime.fsi
- src/Controls/CustomControl.fsi
- src/Controls/DataGrid.fsi
- src/Controls/Diagnostics.fsi
- src/Controls/RichText.fsi
- src/Controls/TextInput.fsi
- src/Controls/Theme.fsi
- src/Controls/Types.fsi
sampled-symbols:
- type Control<'msg>
- KnownControl.TextBlock
- StandardAttributeName.VisibleRange
- DataGrid.create
- LineChart.series
- TextBox.onChanged
omitted-symbol-reasons:
- none
unsupported-symbols:
- none
diagnostics:
- none

## Common Samples
- `type Control<'msg>`
- `KnownControl.TextBlock`
- `StandardAttributeName.VisibleRange`
- `DataGrid.create`
- `LineChart.series`
- `TextBox.onChanged`

## Curated Signatures
```fsharp
namespace FS.Skia.UI.Controls

/// Build and validate a control's accessibility contract: `metadata`/`defaultFor` plus `keyboard`/`contrast` evidence.
module Accessibility =
    /// Describe a control's keyboard contract: whether it is `focusable` and its activation/navigation keys.
    val keyboard: focusable: bool -> activationKeys: string list -> navigationKeys: string list -> KeyboardOperation
    /// Record `ContrastEvidence` for a foreground/background pair against the `requiredRatio`.
    val contrast: foreground: FS.Skia.UI.Scene.Color -> background: FS.Skia.UI.Scene.Color -> ratio: float -> requiredRatio: float -> ContrastEvidence
    /// Assemble full `AccessibilityMetadata` from role, name source, state, focus order, keyboard, contrast, and nav range.
    val metadata:
        role: AccessibilityRole ->
        nameSource: string ->
        state: string list ->
        focusOrder: int option ->
        keyboard: KeyboardOperation ->
        contrast: ContrastEvidence option ->
        navRange: NavRange option ->
            AccessibilityMetadata

    /// Build default `AccessibilityMetadata` for a `ControlKind` with the given accessible `label`.
    val defaultFor: kind: ControlKind -> label: string -> AccessibilityMetadata
    /// Check a `control`'s accessibility contract and return any `ControlDiagnostic` violations.
    val validate: control: Control<'msg> -> ControlDiagnostic list

namespace FS.Skia.UI.Controls

/// Builder functions (`Attr`) for constructing the typed `Attr<'msg>` values that
/// configure a control — covering content, layout, state, style, theme, and event attributes.
module Attr =
    /// Low-level escape hatch (`create`) building an `Attr<'msg>` from an explicit `name`,
    /// `AttrCategory`, and `AttrValue<'msg>`; the foundation the typed builders below wrap.
    val create: name: string -> category: AttrCategory -> value: AttrValue<'msg> -> Attr<'msg>
    /// Schema-checked builder (`standardAttribute`) producing an `Attr<'msg>` from a typed
    /// `StandardAttributeName` and `StandardAttributeValue<'msg>`, keeping the attribute within
    /// the recognised contract surface.
    val standardAttribute: name: StandardAttributeName -> value: StandardAttributeValue<'msg> -> Attr<'msg>
    /// Builder (`customAttribute`) for a consumer-defined attribute outside the standard set:
    /// a free-form `name` carrying an untyped `obj` value, lowered under the `Data` category.
    val customAttribute: name: string -> value: obj -> Attr<'msg>
    /// Event builder (`standardEvent`) wiring a typed `StandardEventKind` to dispatch a fixed
    /// `msg` when that built-in event fires on the control.
    val standardEvent: eventKind: StandardEventKind -> msg: 'msg -> Attr<'msg>
    /// Event builder (`customEvent`) wiring a free-form `eventKind` string to dispatch a fixed
    /// `msg`, for events outside the `StandardEventKind` set.
    val customEvent: eventKind: string -> msg: 'msg -> Attr<'msg>
    /// Content builder (`text`) setting the control's display text — the label of a button or
    /// text-block, or the caption shown by content-bearing kinds.
    val text: value: string -> Attr<'msg>
    /// Content builder (`value`) setting the current value of an input control such as a
    /// text-box, carried as the `Value` attribute.
    val value: value: string -> Attr<'msg>
    /// Data builder (`items`) supplying the ordered string entries of a list-like control as
    /// the `Items` attribute.
    val items: values: string list -> Attr<'msg>
    /// Children builder (`child`) attaching a single nested `Control<'msg>` to a container.
    val child: control: Control<'msg> -> Attr<'msg>
    /// Children builder (`children`) attaching an ordered list of nested `Control<'msg>` to a
    /// container such as a stack, grid, or panel.
    val children: controls: Control<'msg> list -> Attr<'msg>
    /// State builder (`enabled`): when `false` the control is disabled (non-interactive,
    /// rendered in its `Disabled` visual state); omitted defaults to enabled.
    val enabled: value: bool -> Attr<'msg>
    /// State builder (`visible`): when `false` the control is hidden from layout and paint;
    /// omitted defaults to visible.
    val visible: value: bool -> Attr<'msg>
    /// State builder (`readOnly`): when `true` an input control such as a text-box displays
    /// its value but rejects edits; omitted defaults to editable.
    val readOnly: value: bool -> Attr<'msg>
    /// State builder (`loading`): when `true` the control shows its busy/`Loading` visual
    /// state; omitted defaults to not loading.
    val loading: value: bool -> Attr<'msg>
    /// State builder (`selected`): when `true` marks the control as selected (e.g. a toggle,
    /// radio, or list item in its `Selected` visual state); omitted defaults to unselected.
    val selected: value: bool -> Attr<'msg>
    /// Layout builder (`width`) requesting a fixed control width in device-independent pixels;
    /// omitted lets the control size to its content/container.
    val width: value: float -> Attr<'msg>
    /// Layout builder (`height`) requesting a fixed control height in device-independent
    /// pixels; omitted lets the control size to its content/container.
    val height: value: float -> Attr<'msg>
    /// Layout builder (`padding`) setting uniform inner spacing in pixels between the control's
    /// edge and its content; omitted defaults to no padding.
    val padding: value: float -> Attr<'msg>
    /// Layout builder (`margin`) setting uniform outer spacing in pixels around the control
    /// within its parent; omitted defaults to no margin.
    val margin: value: float -> Attr<'msg>
    /// Style builder (`style`) attaching a single named style class by string — the free-form
    /// counterpart to the typed `styleClasses` builder.
    val style: name: string -> Attr<'msg>
    /// Feature 093 (E3): attach an ordered list of style classes (list order = attach order).
    /// Lowers to a single `Style`-category attribute carrying `StyleClassesValue`. Absent ≡
    /// `[]` ≡ the behaviour-preserving base case (FR-005). The last `styleClasses` attribute on
    /// a control wins (the codebase's last-writer attribute convention).
    val styleClasses: classes: StyleClass list -> Attr<'msg>
    /// Feature 093 (E3): set the control's current `VisualState` for the resolver. A host wires
    /// its `ControlRuntime` Hover/Press/Focus state into this each frame; it rides the control
    /// through the keyed reconciler so a state-driven look survives a sibling shift (FR-006,
    /// SC-005). Absent ≡ `Normal` ≡ the behaviour-preserving base case.
    val visualState: state: VisualState -> Attr<'msg>
    /// Theme builder (`theme`) attaching a `Theme` palette/metrics to the control subtree,
    /// overriding the inherited theme for it and its descendants.
    val theme: theme: Theme -> Attr<'msg>
    /// Validation builder (`validation`) attaching a `ValidationState` (`Valid`/`Invalid`/`Pending`)
    /// to an input control, surfacing its validity in the resolved visual state.
    val validation: state: ValidationState -> Attr<'msg>
    /// Accessibility builder (`accessibility`) attaching explicit `AccessibilityMetadata`
    /// (role, name source, keyboard contract, contrast/navigation data) to override the
    /// control's inferred semantics.
    val accessibility: metadata: AccessibilityMetadata -> Attr<'msg>
    /// Event builder (`on`) subscribing to an event by `eventKind` string and dispatching a
    /// fixed `msg` when it fires, ignoring the event payload.
    val on: eventKind: string -> msg: 'msg -> Attr<'msg>
    /// Event builder (`onWith`) subscribing to an event by `eventKind` string and computing the
    /// dispatched message from the `ControlEvent` via `map`, giving access to the payload.
    val onWith: eventKind: string -> map: (ControlEvent -> 'msg) -> Attr<'msg>

namespace FS.Skia.UI.Controls

/// Accessibility facts a catalog entry advertises for a control: its `Role`,
/// where the accessible name comes from (`NameSource`), reported `StateMetadata`,
/// `FocusBehavior`, `KeyboardOperation`, and the `ContrastEvidence` backing it.
type CatalogAccessibility =
    { Role: string
      NameSource: string
      StateMetadata: string list
      FocusBehavior: string
      KeyboardOperation: string
      ContrastEvidence: string }

/// One control's full authoring contract as published by `Catalog`: identity
/// (`Id`/`DisplayName`/`Category`/`Module`), `Purpose`, its `RequiredAttributes`
/// and `CommonAttributes`, bindable `Events`, `VisualStates`, `Accessibility`,
/// plus `Examples`/`Tests`/`Evidence` and `SupportStatus`/`Owner` provenance.
type ControlDefinition =
    { Id: string
      DisplayName: string
      Category: string
      Module: string
      Purpose: string
      RequiredAttributes: string list
      CommonAttributes: string list
      Events: string list
      VisualStates: string list
      Accessibility: CatalogAccessibility
      Examples: string list
      Tests: string list
      Evidence: string list
      SupportStatus: string
      Owner: string }

/// Discovery surface for the standard control library: enumerate every control's
/// authoring contract (required/supported attributes, events) and validate
/// authored `Control` values against the published schema.
module Catalog =
    /// The full list of `ControlDefinition` entries — one per supported control —
    /// to enumerate the whole authoring catalog.
    val supportedControls: ControlDefinition list
    /// The machine-readable `ControlSchema` per standard control, pairing each
    /// kind with its accepted attributes and events for validation.
    val standardSchema: ControlSchema list
    /// Every `StandardControlKind` the catalog knows about — the entry point for
    /// enumerating which controls can be authored and queried.
    val knownControlKinds: unit -> StandardControlKind list
    /// The `StandardAttributeName`s a control of `kind` must carry to be valid —
    /// the mandatory subset of its authoring contract.
    val requiredAttributes: kind: StandardControlKind -> StandardAttributeName list
    /// Every `StandardAttributeName` a control of `kind` accepts (required plus
    /// optional), to discover the full attribute surface for that control.
    val supportedAttributes: kind: StandardControlKind -> StandardAttributeName list
    /// The `StandardEventKind`s a control of `kind` can raise — the bindable
    /// events a consumer may wire handlers to.
    val supportedEvents: kind: StandardControlKind -> StandardEventKind list
    /// Checks an authored `control` against the catalog schema, returning a
    /// `ControlDiagnostic` for each missing required or unsupported attribute.
    val validateStandardControl: control: Control<'msg> -> ControlDiagnostic list
    /// The number of entries in `supportedControls` — how many distinct controls
    /// the catalog documents.
    val supportedCount: unit -> int
    /// The distinct `Category` values across the catalog, to group controls when
    /// presenting a discovery index.
    val categories: unit -> string list
    /// Self-checks the catalog itself, returning any `ControlDiagnostic` for
    /// internal inconsistencies between definitions and the schema.
    val validate: unit -> ControlDiagnostic list
    /// Renders the whole catalog as a Markdown reference table — a ready-to-read
    /// summary of every control's authoring contract.
    val markdownSummary: unit -> string

namespace FS.Skia.UI.Controls

// `ChartPoint` / `ChartSeries` are declared in Types.fsi (feature 080, surface-neutral move).

/// Line-chart control plotting one or more `ChartSeries` as connected lines;
/// author it through the typed `Props` front door.
module LineChart =
    /// Builds a `LineChart` `Control` from the given attributes.
    val create: Attr<'msg> list -> Control<'msg>
    /// Attribute supplying the line-chart's `series` data to plot.
    val series: ChartSeries list -> Attr<'msg>

/// Bar-chart control rendering each `ChartSeries` as grouped bars; author it
/// through the typed `Props` front door.
module BarChart =
    /// Builds a `BarChart` `Control` from the given attributes.
    val create: Attr<'msg> list -> Control<'msg>
    /// Attribute supplying the bar-chart's `series` data to render as bars.
    val series: ChartSeries list -> Attr<'msg>

/// Pie-chart control rendering `ChartPoint` values as proportional slices;
/// author it through the typed `Props` front door.
module PieChart =
    /// Builds a `PieChart` `Control` from the given attributes.
    val create: Attr<'msg> list -> Control<'msg>
    /// Attribute supplying the pie-chart's `values`, each a slice of the whole.
    val values: ChartPoint list -> Attr<'msg>

/// Scatter-plot control rendering each `ChartSeries` as discrete points;
/// author it through the typed `Props` front door.
module ScatterPlot =
    /// Builds a `ScatterPlot` `Control` from the given attributes.
    val create: Attr<'msg> list -> Control<'msg>
    /// Attribute supplying the scatter-plot's `series` of points to plot.
    val series: ChartSeries list -> Attr<'msg>

/// Graph-view control rendering a set of named nodes and their relationships;
/// author it through the typed `Props` front door.
module GraphView =
    /// Builds a `GraphView` `Control` from the given attributes.
    val create: Attr<'msg> list -> Control<'msg>
    /// Attribute supplying the graph's `nodes` by name.
    val nodes: string list -> Attr<'msg>

namespace FS.Skia.UI.Controls

/// The slice of a virtualized list currently realized: `FirstIndex`/`Count` within `Total`.
type VisibleRange =
    { FirstIndex: int
      Count: int
      Total: int }

/// State of a virtualizing collection: scroll offset, viewport/row geometry, `SelectedKeys`,
/// and the derived `VisibleRange` keyed by `ControlId`.
type CollectionModel =
    { ControlId: ControlId
      ItemCount: int
      RowHeight: float
      ViewportHeight: float
      ScrollOffset: float
      SelectedKeys: Set<string>
      VisibleRange: VisibleRange
      RecalculationThresholdMs: int }

/// Messages that drive a `CollectionModel`: `ScrollTo`, `SelectKey`/`ToggleKey`, `ReplaceItemCount`.
type CollectionMsg =
    | ScrollTo of float
    | SelectKey of string
    | ToggleKey of string
    | ReplaceItemCount of int

/// Side effect emitted when a collection update shifts the realized window (`VisibleRangeChanged`).
type CollectionEffect =
    | VisibleRangeChanged of VisibleRange

/// Virtualization model for large scrolling lists: `visibleRange`/`init`/`update` over `CollectionModel`.
module Collections =
    /// Compute the realized `VisibleRange` from row height, viewport height, scroll offset, and item total.
    val visibleRange: rowHeight: float -> viewportHeight: float -> scrollOffset: float -> totalItems: int -> VisibleRange
    /// Build the initial `CollectionModel` for a `controlId` and emit its first `CollectionEffect` list.
    val init: controlId: ControlId -> itemCount: int -> rowHeight: float -> viewportHeight: float -> CollectionModel * CollectionEffect list
    /// Apply a `CollectionMsg` to the `CollectionModel`, returning the next model and any effects.
    val update: msg: CollectionMsg -> model: CollectionModel -> CollectionModel * CollectionEffect list

namespace FS.Skia.UI.Controls

/// Internal extraction seam (feature 080) — `internal` accessibility, no public-surface
/// entry (mirrors `module internal Reconcile`); reached from `Controls.Tests` via
/// `InternalsVisibleTo`. Only `chartValues` is exposed, for the FR-002 extraction test that
/// proves the typed-front-door `ChartSeries`/`ChartPoint` data is read (pre-080: yielded `[]`).
module internal ControlInternals =
    /// Extract the chart data points (X/Y/Label preserved) a chart-like control carries.
    val chartValues: control: Control<'msg> -> ChartPoint list

    /// Feature 097 (R2): attribute names `toLayout` reads to derive geometry (single source for the
    /// incremental dirty-set classifier; FR-003 anti-drift). See the implementation comment.
    val layoutAffectingAttrNames: Set<string>

    /// Feature 091 — the per-node measure of `Control.renderTree`, factored so the wired
    /// retained path (`module internal RetainedRender`) measures with the IDENTICAL function.
    /// Builds + evaluates the nested Yoga layout, returning the root node and the absolute
    /// bounds keyed by the collision-free structural id (`Key |> defaultValue path`).
    val evaluateLayout:
        size: FS.Skia.UI.Scene.Size ->
        control: Control<'msg> ->
            FS.Skia.UI.Layout.LayoutNode * Map<string, FS.Skia.UI.Layout.LayoutBounds> * FS.Skia.UI.Layout.LayoutResult

    /// Feature 097 (R2): incremental layout seam — re-measures only the `dirty` set (conservatively
    /// propagated inside `Layout.evaluateIncremental`) against the previous frame's `LayoutResult`,
    /// returning the same `root, boundsById` shape plus the new result to carry forward. `Bounds` are
    /// byte-identical to `evaluateLayout`.
    val evaluateLayoutIncremental:
        size: FS.Skia.UI.Scene.Size ->
        control: Control<'msg> ->
        previous: FS.Skia.UI.Layout.LayoutResult ->
        dirty: Set<FS.Skia.UI.Layout.LayoutNodeId> ->
            FS.Skia.UI.Layout.LayoutNode * Map<string, FS.Skia.UI.Layout.LayoutBounds> * FS.Skia.UI.Layout.LayoutResult

    /// Feature 091 — paint ONE node's own contribution (`here`) at its computed box; the
    /// reusable unit a retained `RenderFragment` caches. Depends only on (theme, box, the
    /// node's own Kind/Content/Attributes/has-children), never on descendants.
    val paintNode:
        theme: Theme ->
        boundsById: Map<string, FS.Skia.UI.Layout.LayoutBounds> ->
        path: string ->
        c: Control<'msg> ->
            FS.Skia.UI.Scene.Scene list

    /// Feature 091 — the evaluated absolute box of a node, by the same structural id
    /// `paintNode` looks up. `None` when the node was not laid out.
    val nodeBox:
        boundsById: Map<string, FS.Skia.UI.Layout.LayoutBounds> ->
        path: string ->
        c: Control<'msg> ->
            FS.Skia.UI.Scene.Rect option

    /// Feature 091 — the evaluated `Bounds` list `renderTree` surfaces, from a pre-evaluated
    /// `boundsById`, so the retained path emits the identical list.
    val collectBoundsWith:
        boundsById: Map<string, FS.Skia.UI.Layout.LayoutBounds> ->
        control: Control<'msg> ->
            (ControlId * FS.Skia.UI.Scene.Rect) list

    /// Feature 091 — the recursive `EventBindings` list `renderTree` surfaces, factored so the
    /// retained path emits the identical list.
    val eventBindingsOf: control: Control<'msg> -> ControlEventBinding<'msg> list

    /// Feature 098 (FR-002) — the canonical ids (`Key ?? path`) of every node carrying ≥1 event
    /// binding. The single source for `ControlRenderResult.BoundIds` at the full rebuild AND the
    /// retained frames, so the retained path is byte-identical by construction; read by
    /// `nearestAuthored` to recover an unkeyed-bound ancestor.
    val boundIdsOf: control: Control<'msg> -> Set<ControlId>

    /// Feature 093 (E3) — dispatch a rich-family control to its faithful geometry within `box`.
    /// Exposed (internal) so the migration parity tests assert the Button/CheckBox paint is
    /// structurally-`Scene`-equal to the frozen pre-refactor procedural geometry (SC-003/SC-007).
    val faithfulContent: theme: Theme -> box: FS.Skia.UI.Scene.Rect -> control: Control<'msg> -> FS.Skia.UI.Scene.Scene list

    /// Feature 093 (E3) — the ordered attached style classes carried by a control's `styleClasses`
    /// attribute (last-writer convention; absent ≡ `[]`). The resolver folds these in list order.
    val styleClassesOf: attrs: Attr<'msg> list -> StyleClass list

    /// Feature 093 (E3) — the control's current `VisualState` carried by its `visualState`
    /// attribute (absent ≡ `Normal`). Rides the control through the keyed reconciler so a
    /// state-driven look survives a sibling-shifting re-render (SC-005).
    val visualStateOf: attrs: Attr<'msg> list -> VisualState

    /// Feature 095 (E5) — build the single `Slot`-category carrier attribute from an ordered
    /// name->fill association list. `internal`: the typed `Props` views call it; there is NO public
    /// free-form slot builder (FR-001). The slot name is internal plumbing, never a consumer string.
    val slotFill: fills: (string * Control<'msg>) list -> Attr<'msg>

    /// Feature 095 (E5) — the ordered slot fills carried by a control's last `slot` attribute
    /// (last-writer convention). Absent ≡ `[]` ≡ no slot filled ≡ the byte-identical base case.
    val slotFillsOf: attrs: Attr<'msg> list -> (string * Control<'msg>) list

    /// Feature 095 (E5) — the fill for ONE named region, or `None` when that name is absent
    /// (unfilled ⇒ default chrome). A name present but empty still returns `Some` (absent ≠ empty).
    val slotFor: name: string -> attrs: Attr<'msg> list -> Control<'msg> option

    /// Feature 095 (E5) — the pure, total, deterministic slot lowering. Injects the fills into the
    /// control's `Children` ordered by region position (leading regions, intrinsic children,
    /// trailing regions) and consumes the slot carrier; with no slot attribute the control is
    /// returned verbatim (byte-identical, FR-003). Never throws for any (kind, fills) — totality
    /// (SC-005). Fills land in `Children`, inheriting E1–E4 + E2 identity by construction (FR-004).
    val lowerSlots: control: Control<'msg> -> Control<'msg>

/// Core authoring and rendering verbs for `Control<'msg>` — construction, standard/custom
/// lowering, keying, single-control preview `render` and nested `renderTree`.
module Control =
    /// Build a `Control<'msg>` from an arbitrary `ControlKind` and its attribute list — the
    /// general constructor the per-kind `*.create` builders are sugar over.
    val create: kind: ControlKind -> attrs: Attr<'msg> list -> Control<'msg>
    /// Build a control from a `StandardControlKind` (the framework's built-in catalog kinds),
    /// keeping the kind on the typed enum rather than a free-form string.
    val standard: kind: StandardControlKind -> attrs: Attr<'msg> list -> Control<'msg>
    /// Build a consumer-defined control whose kind is a free-form `kind` string, for control
    /// families outside the built-in `StandardControlKind` catalog.
    val customControl: kind: string -> attrs: Attr<'msg> list -> Control<'msg>
    /// Lower a `standard`-kind control into its primitive composition (the expansion the renderer
    /// consumes); a control whose kind needs no expansion is returned unchanged.
    val lowerStandard: control: Control<'msg> -> Control<'msg>
    /// Lower a `customControl` into its primitive composition; the custom-kind counterpart to
    /// `lowerStandard`.
    val lowerCustom: control: Control<'msg> -> Control<'msg>
    /// Stamp a stable identity `key` onto a control so the keyed reconciler tracks it across
    /// sibling-shifting re-renders (the `withKey` anchor read by `nearestAuthored`).
    val withKey: key: ControlId -> control: Control<'msg> -> Control<'msg>
    /// Render a SINGLE control to a `ControlRenderResult<'msg>` preview at intrinsic size
    /// (Feature 080); use `renderTree` to lay out and paint nested children.
    val render: theme: Theme -> control: Control<'msg> -> ControlRenderResult<'msg>
    /// Faithfully rasterize a NESTED control tree to a Scene using real Yoga layout and paint
    /// at the given output size (distinct from `render`, the Feature-080 single-control
    /// PREVIEW). Lays out and paints nested containers AND their children at their computed
    /// bounds, so two structurally different trees produce visibly different scenes. The
    /// returned `Layout`/`EventBindings` correlate by `ControlId` for host hit-testing.
    /// Additive: `render` and `Widget.render` are unchanged (FR-001/FR-002/FR-003).
    ///
    /// Feature 091 (behavioral note, signature unchanged): the interactive host loops no
    /// longer call `renderTree` afresh every frame — each next frame is produced by diffing
    /// the next lowered tree against a retained previous tree (`module internal
    /// RetainedRender`) and reusing the unchanged subtrees' cached render fragments. The
    /// per-node measure/paint here is factored into `ControlInternals.evaluateLayout` /
    /// `paintNode`, which the retained path reuses, so a full `renderTree` and the retained
    /// partial render are byte-for-byte identical (FR-005).
    val renderTree:
        theme: Theme -> size: FS.Skia.UI.Scene.Size -> control: Control<'msg> -> ControlRenderResult<'msg>
    /// Resolve which rendered control (if any) contains the point (x, y), from the public
    /// `renderTree` result alone. `None` when the point lies in a gap. Layered over
    /// `Layout.hitTestComputed` against the evaluated `Bounds` (FR-012).
    val hitTest: result: ControlRenderResult<'msg> -> x: float -> y: float -> ControlId option
    /// Resolve a structural hit `ControlId` (the id a `PointerInteraction`/`hitTest` carries — a
    /// `Key` for an authored node, else the positional path `renderTree` assigns) to the nearest
    /// ancestor (incl. self) the consumer authored with a `withKey`, as that ancestor's authored
    /// `ControlId`. A click inside a container-keyed composite recovers the container's id (so the
    /// interactive host can route its binding); a directly-keyed leaf resolves to itself. `None`
    /// when no keyed ancestor exists on the hit node's path — the host then falls back to
    /// `MapPointer` with the raw interaction, never inventing an id. Pure/total/deterministic; reads
    /// the `renderTree` layout tree only, no layout-math change (FR-004/FR-004a/FR-005, feature 090).
    val nearestAuthored: result: ControlRenderResult<'msg> -> hit: ControlId -> ControlId option
    /// Collect the `ControlDiagnostic` list a control's tree reports (e.g. authoring issues),
    /// for surfacing in tooling without rendering.
    val diagnostics: control: Control<'msg> -> ControlDiagnostic list
    /// Translate an incoming `ControlEvent` into the `'msg` list a control's bindings emit — the
    /// dispatch step the interactive host runs to feed the MVU update loop.
    val dispatch: event: ControlEvent -> control: Control<'msg> -> 'msg list
    /// Count the total nodes in a control's tree (self plus all descendants); a structural metric
    /// used by tests and tooling.
    val count: control: Control<'msg> -> int

/// Builders for the `TextBlock` control — a multi-line, wrapping run of body text.
module TextBlock =
    /// Build a `TextBlock` from its attributes; pair with `TextBlock.text` for the content. The
    /// typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the displayed text of a `TextBlock` (`Attr` carrying the run of characters to lay out
    /// and wrap).
    val text: string -> Attr<'msg>

/// Builders for the `Label` control — a short, single-line caption, typically naming an
/// adjacent field.
module Label =
    /// Build a `Label` from its attributes; pair with `Label.text` for the caption. The typed
    /// `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the caption text of a `Label` (`Attr` carrying the single-line string to display).
    val text: string -> Attr<'msg>

/// Builders for the `Image` control — a bitmap displayed from a source reference.
module Image =
    /// Build an `Image` from its attributes; pair with `Image.source` for the bitmap. The typed
    /// `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the `Image` source (`Attr` carrying the path/URI string the renderer loads the bitmap
    /// from).
    val source: string -> Attr<'msg>

/// Builders for the `Icon` control — a glyph chosen from the icon set by name.
module Icon =
    /// Build an `Icon` from its attributes; pair with `Icon.name` to choose the glyph. The typed
    /// `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Select which glyph an `Icon` shows (`Attr` carrying the icon-set name to look up).
    val name: string -> Attr<'msg>

/// Builders for the `Separator` control — a thin divider rule between adjacent content.
module Separator =
    /// Build a `Separator` divider from its attributes (takes no content of its own). The typed
    /// `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>

/// Builders for the `Badge` control — a small count/status pill overlaid on or beside content.
module Badge =
    /// Build a `Badge` pill from its attributes; pair with `Badge.text` for its label. The typed
    /// `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the label shown inside a `Badge` (`Attr` carrying the short count/status string).
    val text: string -> Attr<'msg>

/// Builders for the `Button` control — a clickable command surface with a text label.
module Button =
    /// Build a `Button` from its attributes; pair with `Button.text` and `Button.onClick`. The
    /// typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the `Button` label (`Attr` carrying the caption rendered on the command surface).
    val text: string -> Attr<'msg>
    /// Set whether a `Button` is interactive (`Attr`; `false` greys it out and suppresses click
    /// dispatch). Omitted ≡ enabled.
    val enabled: bool -> Attr<'msg>
    /// Emit a fixed `'msg` when the `Button` is clicked (`Attr.onClick`); use `onClickWith` when
    /// the message depends on the event.
    val onClick: 'msg -> Attr<'msg>
    /// Emit a `'msg` derived from the `ControlEvent` when the `Button` is clicked — the
    /// event-aware counterpart to `Button.onClick`.
    val onClickWith: (ControlEvent -> 'msg) -> Attr<'msg>

/// Builders for the `IconButton` control — a compact, glyph-only clickable command.
module IconButton =
    /// Build an `IconButton` from its attributes; pair with `IconButton.icon` and `onClick`. The
    /// typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Choose the glyph an `IconButton` shows (`Attr` carrying the icon-set name; the visual
    /// stand-in for a text label).
    val icon: string -> Attr<'msg>
    /// Emit a fixed `'msg` when the `IconButton` is clicked (`Attr.onClick`).
    val onClick: 'msg -> Attr<'msg>

/// Builders for the `CheckBox` control — a labelled boolean toggle with a tick box.
module CheckBox =
    /// Build a `CheckBox` from its attributes; pair with `CheckBox.checked'` and `onChanged`. The
    /// typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the label beside a `CheckBox` (`Attr` carrying the descriptive caption text).
    val text: string -> Attr<'msg>
    /// Set the checked state of a `CheckBox` (`Attr.checked'`; `true` ticks the box). This is a
    /// controlled value — drive it from model state and reconcile via `onChanged`.
    val checked': bool -> Attr<'msg>
    /// Emit a `'msg` carrying the new `bool` when a `CheckBox` is toggled (`Attr.onChanged`).
    val onChanged: (bool -> 'msg) -> Attr<'msg>

/// Builders for the `Switch` control — a sliding on/off toggle (the track-and-thumb form of a
/// boolean).
module Switch =
    /// Build a `Switch` from its attributes; pair with `Switch.checked'` and `onChanged`. The
    /// typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the on/off position of a `Switch` (`Attr.checked'`; `true` slides the thumb on). A
    /// controlled value driven from model state and reconciled via `onChanged`.
    val checked': bool -> Attr<'msg>
    /// Emit a `'msg` carrying the new `bool` when a `Switch` is flipped (`Attr.onChanged`).
    val onChanged: (bool -> 'msg) -> Attr<'msg>

/// Builders for the `Slider` control — a draggable thumb selecting a continuous value along a
/// track.
module Slider =
    /// Build a `Slider` from its attributes; pair with `Slider.value` and `onChanged`. The typed
    /// `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the `Slider` position (`Attr.value`; a `float` over the control's range, default 0–1).
    /// A controlled value driven from model state and reconciled via `onChanged`.
    val value: float -> Attr<'msg>
    /// Emit a `'msg` carrying the new `float` as a `Slider` is dragged (`Attr.onChanged`).
    val onChanged: (float -> 'msg) -> Attr<'msg>

/// Builders for the `NumericInput` control — a typed numeric field, typically with stepper
/// affordances.
module NumericInput =
    /// Build a `NumericInput` from its attributes; pair with `NumericInput.value` and `onChanged`.
    /// The typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the current number in a `NumericInput` (`Attr.value`; a controlled `float` driven from
    /// model state and reconciled via `onChanged`).
    val value: float -> Attr<'msg>
    /// Emit a `'msg` carrying the edited `float` when a `NumericInput` value changes
    /// (`Attr.onChanged`).
    val onChanged: (float -> 'msg) -> Attr<'msg>

/// Builders for the `TextBox` control — a single-line editable text field.
module TextBox =
    /// Build a `TextBox` from its attributes; pair with `TextBox.value` and `onChanged`. The
    /// typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the current text in a `TextBox` (`Attr.value`; a controlled `string` driven from model
    /// state and reconciled via `onChanged`).
    val value: string -> Attr<'msg>
    /// Make a `TextBox` display-only (`Attr.readOnly`; `true` shows the value but blocks editing).
    /// Omitted ≡ editable.
    val readOnly: bool -> Attr<'msg>
    /// Attach a `ValidationState` to a `TextBox` so it renders the matching valid/invalid styling
    /// (`Attr.validation`).
    val validation: ValidationState -> Attr<'msg>
    /// Emit a `'msg` carrying the edited `string` on each `TextBox` change (`Attr.onChanged`).
    val onChanged: (string -> 'msg) -> Attr<'msg>

/// Builders for the `TextArea` control — a multi-line editable text field (the wrapping
/// counterpart to `TextBox`).
module TextArea =
    /// Build a `TextArea` from its attributes; pair with `TextArea.value` and `onChanged`. The
    /// typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the current multi-line text in a `TextArea` (`Attr.value`; a controlled `string`
    /// reconciled via `onChanged`).
    val value: string -> Attr<'msg>
    /// Emit a `'msg` carrying the edited `string` on each `TextArea` change (`Attr.onChanged`).
    val onChanged: (string -> 'msg) -> Attr<'msg>

/// Builders for the `RadioGroup` control — a set of mutually-exclusive options, one selected at
/// a time.
module RadioGroup =
    /// Build a `RadioGroup` from its attributes; pair with `RadioGroup.items`, `selected` and
    /// `onChanged`. The typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is recommended.
    val create: Attr<'msg> list -> Control<'msg>
    /// Supply the option labels of a `RadioGroup` (`Attr.items`; one radio button per `string` in
    /// the list, in order).
    val items: string list -> Attr<'msg>
    /// Mark which option of a `RadioGroup` is chosen (`Attr.selected`; the `string` must match one
    /// of `items`). A controlled value reconciled via `onChanged`.
    val selected: string -> Attr<'msg>
    /// Emit a `'msg` carrying the newly-chosen option `string` when a `RadioGroup` selection
    /// changes (`Attr.onChanged`).
    val onChanged: (string -> 'msg) -> Attr<'msg>

/// Builders for the `Stack` container — lays its children single-file along one axis (vertical
/// by default; see `Stack.orientation`).
module Stack =
    /// Build a `Stack` container from its attributes; pair with `Stack.children` for content. The
    /// typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Supply the ordered child controls a `Stack` arranges (`Attr.children`).
    val children: Control<'msg> list -> Attr<'msg>
    /// Lay the stack's children along the row axis when value = "horizontal"; any other
    /// value (or omission) keeps the default vertical column (FR-007).
    val orientation: string -> Attr<'msg>

/// Builders for the `Grid` container — arranges its children in a two-dimensional row/column
/// matrix.
module Grid =
    /// Build a `Grid` container from its attributes; pair with `Grid.children` for content. The
    /// typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Supply the child controls a `Grid` places into its cells (`Attr.children`).
    val children: Control<'msg> list -> Attr<'msg>

/// Builders for the `Dock` container — pins its children to the edges, the last filling the
/// remaining centre.
module Dock =
    /// Build a `Dock` container from its attributes; pair with `Dock.children` for content. The
    /// typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Supply the child controls a `Dock` arranges against its edges (`Attr.children`).
    val children: Control<'msg> list -> Attr<'msg>

/// Builders for the `Wrap` container — flows its children along an axis, wrapping to the next
/// line when they overflow.
module Wrap =
    /// Build a `Wrap` container from its attributes; pair with `Wrap.children` for content. The
    /// typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Supply the child controls a `Wrap` flows and line-wraps (`Attr.children`).
    val children: Control<'msg> list -> Attr<'msg>

/// Builders for the `Border` container — wraps a single child in a stroked/padded frame.
module Border =
    /// Build a `Border` from its attributes; pair with `Border.child` for the wrapped content. The
    /// typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the single control a `Border` frames (`Attr.child`; a `Border` holds exactly one
    /// child, unlike the multi-child containers).
    val child: Control<'msg> -> Attr<'msg>

/// Builders for the `Panel` container — a surface grouping child controls, with optional
/// Header/Footer slots (Feature 095).
module Panel =
    /// Build a `Panel` from its attributes; pair with `Panel.children` for content. The typed
    /// `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Supply the child controls a `Panel` groups on its surface (`Attr.children`).
    val children: Control<'msg> list -> Attr<'msg>

/// Builders for the `ProgressBar` control — a horizontal fill showing completion of a task.
module ProgressBar =
    /// Build a `ProgressBar` from its attributes; pair with `ProgressBar.value` for the fill. The
    /// typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the completion of a `ProgressBar` (`Attr.value`; a `float` fraction, 0 = empty through
    /// 1 = full).
    val value: float -> Attr<'msg>

/// Builders for the `Spinner` control — an indeterminate busy/loading indicator.
module Spinner =
    /// Build a `Spinner` busy indicator from its attributes (no progress value; it animates
    /// indeterminately). The typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is recommended.
    val create: Attr<'msg> list -> Control<'msg>

/// Builders for the `ValidationMessage` control — inline error/hint text shown beneath a field.
module ValidationMessage =
    /// Build a `ValidationMessage` from its attributes; pair with `ValidationMessage.text` for the
    /// message. The typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is recommended.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the validation text shown to the user (`Attr` carrying the error/hint `string`).
    val text: string -> Attr<'msg>

/// Builders for the `Tabs` control — a row of tab headers selecting one active page.
module Tabs =
    /// Build a `Tabs` strip from its attributes; pair with `Tabs.items`, `selected` and
    /// `onChanged`. The typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is recommended.
    val create: Attr<'msg> list -> Control<'msg>
    /// Supply the tab header labels of a `Tabs` strip (`Attr.items`; one tab per `string`, in
    /// order).
    val items: string list -> Attr<'msg>
    /// Mark which tab of a `Tabs` strip is active (`Attr.selected`; the `string` must match one of
    /// `items`). A controlled value reconciled via `onChanged`.
    val selected: string -> Attr<'msg>
    /// Emit a `'msg` carrying the newly-activated tab `string` when the `Tabs` selection changes
    /// (`Attr.onChanged`).
    val onChanged: (string -> 'msg) -> Attr<'msg>

/// Builders for the `Menu` control — a list of selectable command entries.
module Menu =
    /// Build a `Menu` from its attributes; pair with `Menu.items` and `onSelected`. The typed
    /// `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Supply the entry labels of a `Menu` (`Attr.items`; one selectable row per `string`, in
    /// order).
    val items: string list -> Attr<'msg>
    /// Emit a `'msg` carrying the chosen entry `string` when a `Menu` item is selected
    /// (`Attr.onSelected`).
    val onSelected: (string -> 'msg) -> Attr<'msg>

/// Builders for the `Toolbar` container — a horizontal band of command controls (buttons,
/// icons, separators).
module Toolbar =
    /// Build a `Toolbar` from its attributes; pair with `Toolbar.children` for content. The typed
    /// `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Supply the command controls a `Toolbar` lays out left-to-right (`Attr.children`).
    val children: Control<'msg> list -> Attr<'msg>

/// Builders for the `Tooltip` control — a transient hover hint floating over content.
module Tooltip =
    /// Build a `Tooltip` from its attributes; pair with `Tooltip.text` for the hint. The typed
    /// `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the hint text a `Tooltip` shows on hover (`Attr` carrying the `string` to float).
    val text: string -> Attr<'msg>

/// Builders for the `Dialog` container — a modal surface holding a focused task's content.
module Dialog =
    /// Build a `Dialog` from its attributes; pair with `Dialog.children` for content. The typed
    /// `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended authoring path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Supply the child controls a `Dialog` hosts in its modal body (`Attr.children`).
    val children: Control<'msg> list -> Attr<'msg>

/// Builders for the `Toast` control — a brief, auto-dismissing notification banner.
module Toast =
    /// Build a `Toast` notification from its attributes; pair with `Toast.text` for the message.
    /// The typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the message a `Toast` displays (`Attr` carrying the short notification `string`).
    val text: string -> Attr<'msg>

/// Builders for the `Overlay` container — a layer drawn above the rest of the UI to host a
/// single child (scrims, popovers).
module Overlay =
    /// Build an `Overlay` from its attributes; pair with `Overlay.child` for the layered content.
    /// The typed `Props` front door (`FS.Skia.UI.Controls.Typed`) is the recommended path.
    val create: Attr<'msg> list -> Control<'msg>
    /// Set the single control an `Overlay` floats above the UI (`Attr.child`; an `Overlay` holds
    /// exactly one child).
    val child: Control<'msg> -> Attr<'msg>

namespace FS.Skia.UI.Controls

/// The text caret position (`Index`) within a focused control identified by `ControlId`.
type ControlCaret =
    { ControlId: ControlId
      Index: int }

/// A text selection range (`Start`..`End`) within the control identified by `ControlId`.
type ControlSelection =
    { ControlId: ControlId
      Start: int
      End: int }

/// In-flight IME composition `Text` being entered into the control identified by `ControlId`.
type ControlComposition =
    { ControlId: ControlId
      Text: string }

/// An active pointer drag on `ControlId`, tracking start (`StartX`/`StartY`) and current (`CurrentX`/`CurrentY`) coordinates.
type ControlDrag =
    { ControlId: ControlId
      StartX: float
      StartY: float
      CurrentX: float
      CurrentY: float }

/// An observable side effect emitted by `ControlRuntime.update` when interaction state changes (focus, hover, caret, selection, drag, diagnostics).
type ControlRuntimeEffect =
    | FocusChanged of ControlId option
    | HoverChanged of ControlId option
    | PressedControlsChanged of ControlId list
    | CaretChanged of ControlCaret option
    | SelectionChanged of ControlSelection option
    | CompositionChanged of ControlComposition option
    | DragChanged of ControlDrag option
    | StaleTarget of ControlId
    | CancelledInteraction of ControlId option
    | ReportControlRuntimeDiagnostic of ControlDiagnostic

/// The aggregate runtime interaction state: focused/hovered/pressed controls, `Caret`, `Selection`, `Composition`, `ActiveDrag`, and accumulated `Diagnostics`.
type ControlRuntimeModel =
    { FocusedControl: ControlId option
      HoveredControl: ControlId option
      PressedControls: Set<ControlId>
      Caret: ControlCaret option
      Selection: ControlSelection option
      Composition: ControlComposition option
      ActiveDrag: ControlDrag option
      Diagnostics: ControlDiagnostic list
      RecentEffects: ControlRuntimeEffect list }

/// An input message driving the runtime transition, e.g. `FocusControl`, `HoverControl`, `PressControl`, `SetCaret`, `StartDrag`, or `Reset`.
type ControlRuntimeMsg =
    | FocusControl of ControlId option
    | HoverControl of ControlId option
    | PressControl of ControlId
    | ReleaseControl of ControlId
    | SetCaret of ControlCaret option
    | SetSelection of ControlSelection option
    | StartComposition of ControlId * string
    | CommitComposition of ControlId
    | StartDrag of ControlId * float * float
    | MoveDrag of float * float
    | EndDrag
    | FocusLost
    | RemoveControl of ControlId
    | RecoverStaleTarget of ControlId
    | CancelInteraction of ControlId option
    | Reset

/// MVU runtime tracking control focus, hover, press, caret/selection, composition, drag, and derived visual state.
module ControlRuntime =
    /// Seeds an empty `ControlRuntimeModel` with no focus or interaction and its initial effects.
    val init: unit -> ControlRuntimeModel * ControlRuntimeEffect list
    /// Pure transition applying `msg` to `model`, returning the next model and the `ControlRuntimeEffect` list it raises.
    val update: msg: ControlRuntimeMsg -> model: ControlRuntimeModel -> ControlRuntimeModel * ControlRuntimeEffect list
    /// Returns the `ControlDiagnostic` list currently accumulated in `model`.
    val diagnostics: model: ControlRuntimeModel -> ControlDiagnostic list

    /// Feature 096 (R1): the pure, total, deterministic projection from live
    /// interaction state to a single VisualState. Selects the highest-ranked
    /// runtime-derivable state for `controlId` under the fixed closed order
    /// Pressed > Selected > Focused > Hover > Normal (the runtime-derivable tail of
    /// FR-002's Disabled > Validation > Loading > Pressed > Selected > Focused > Hover
    /// > Normal). A control named by no interaction state yields `Normal`. No per-kind
    /// branching; identical inputs always yield an identical result.
    val deriveVisualState: model: ControlRuntimeModel -> controlId: ControlId -> VisualState

    /// Feature 096 (R1): internal host bridge — NOT public surface. Stamps each control's derived
    /// VisualState onto the lowered Control<'msg> tree in the ControlId domain (pre-reconcile),
    /// preserving a consumer-set non-Normal attribute and emitting NOTHING at Normal (byte-identity
    /// at rest). Declared `internal` so the Controls.Elmish host and Controls.Tests / Elmish.Tests
    /// reach it via InternalsVisibleTo without enlarging the package's public contract.
    val internal applyRuntimeVisualState: model: ControlRuntimeModel -> control: Control<'msg> -> Control<'msg>

namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene
open FS.Skia.UI.Layout

/// The author-supplied definition of a custom control: measure/render/layout/hit-test/event callbacks
/// plus optional `Accessibility` and `Diagnostics`, keyed by `Id`.
type CustomControlDefinition<'msg> =
    { Id: ControlId
      Measure: unit -> float * float
      Render: unit -> Scene
      Draw: unit -> Scene
      Layout: unit -> LayoutNode
      Clip: (float * float * float * float) option
      Effects: string list
      HitTest: float -> float -> bool
      Event: ControlEvent -> 'msg option
      Accessibility: AccessibilityMetadata option
      Diagnostics: ControlDiagnostic list }

/// Author a bespoke control from a `CustomControlDefinition`: `create` it as a `Control<'msg>` and `validate` it.
module CustomControl =
    /// Build a `Control<'msg>` from a `CustomControlDefinition` and the supplied `attrs`.
    val create: definition: CustomControlDefinition<'msg> -> attrs: Attr<'msg> list -> Control<'msg>
    /// Check a `CustomControlDefinition` for authoring errors, returning any `ControlDiagnostic` list.
    val validate: definition: CustomControlDefinition<'msg> -> ControlDiagnostic list

namespace FS.Skia.UI.Controls

/// How a `DataGridColumn` renders and interprets its cells — `TextColumn`,
/// `NumericColumn`, `BooleanColumn`, or a `CustomColumn` named by its tag.
type DataGridColumnType =
    | TextColumn
    | NumericColumn
    | BooleanColumn
    | CustomColumn of string

/// A grid column definition: its stable `Key`, displayed `Header`, pixel
/// `Width`, and `ColumnType` controlling cell rendering.
type DataGridColumn =
    { Key: string
      Header: string
      Width: float
      ColumnType: DataGridColumnType }

/// A single grid cell addressed by `RowKey` and `ColumnKey`, carrying its
/// rendered string `Value`.
type DataGridCell =
    { RowKey: string
      ColumnKey: string
      Value: string }

/// A grid row identified by `Key`, holding its `Cells` in column order.
type DataGridRow =
    { Key: string
      Cells: DataGridCell list }

/// Direction of a column sort — `Ascending` or `Descending`.
type DataGridSortDirection =
    | Ascending
    | Descending

/// The active sort: which column (`ColumnKey`) is ordered and in which
/// `Direction`.
type DataGridSort =
    { ColumnKey: string
      Direction: DataGridSortDirection }

/// The currently focused cell, located by `RowKey` and `ColumnKey` for
/// keyboard navigation.
type DataGridFocusedCell =
    { RowKey: string
      ColumnKey: string }

/// The full virtualized data-grid state: identity, `Columns`, `RowCount`,
/// row/viewport metrics, the visible window, selection, focus, sort, filter,
/// and accumulated `Diagnostics`.
type DataGridModel =
    { ControlId: ControlId
      Columns: DataGridColumn list
      RowCount: int
      RowHeight: float
      ViewportHeight: float
      VisibleRange: VisibleRange
      SelectedRows: Set<string>
      FocusedCell: DataGridFocusedCell option
      Sort: DataGridSort option
      FilterText: string option
      Diagnostics: ControlDiagnostic list }

/// Messages driving a `DataGridModel` through `update`: scroll, row
/// select/toggle, cell focus, sort, filter, and row-count replacement.
type DataGridMsg =
    | ScrollRowsTo of int
    | SelectRow of string
    | ToggleRow of string
    | FocusCell of DataGridFocusedCell option
    | SortBy of string
    | ApplyFilter of string option
    | ReplaceRowCount of int

/// Outbound notifications a `DataGridModel` emits when its visible range,
/// selection, focus, sort, or filter changes, plus diagnostic reports.
type DataGridEffect =
    | DataGridVisibleRangeChanged of VisibleRange
    | DataGridSelectionChanged of string list
    | DataGridFocusChanged of DataGridFocusedCell option
    | DataGridSortChanged of DataGridSort option
    | DataGridFilterChanged of string option
    | ReportDataGridDiagnostic of ControlDiagnostic

/// Virtualized data-grid control: an MVU `init`/`update` core plus attribute
/// builders for authoring a grid against the typed `Props` front door.
module DataGrid =
    /// Builds the initial `DataGridModel` for `controlId` from its columns and
    /// row/viewport metrics, with the first visible-range effects.
    val init: controlId: ControlId -> columns: DataGridColumn list -> rowCount: int -> rowHeight: float -> viewportHeight: float -> DataGridModel * DataGridEffect list
    /// Applies a `DataGridMsg` to `model`, returning the next state and any
    /// `DataGridEffect`s the change produces.
    val update: msg: DataGridMsg -> model: DataGridModel -> DataGridModel * DataGridEffect list
    /// Authors a data-grid `Control` over the given `columns` and `attrs` — the
    /// legacy builder behind the typed `Props` front door.
    val create: columns: DataGridColumn list -> attrs: Attr<'msg> list -> Control<'msg>
    /// Attribute setting the grid's `columns` definition on a data-grid control.
    val columns: columns: DataGridColumn list -> Attr<'msg>
    /// Attribute supplying the grid's `rows` of cell data to a data-grid control.
    val rows: rows: DataGridRow list -> Attr<'msg>
    /// Attribute pinning the grid's `visibleRange` — the virtualized window of
    /// rows currently rendered.
    val visibleRange: visibleRange: VisibleRange -> Attr<'msg>
    /// Attribute marking the set of `selectedRows` (by row key) on a data-grid
    /// control.
    val selectedRows: selectedRows: Set<string> -> Attr<'msg>
    /// Attribute setting the grid's `focusedCell` for keyboard navigation, or
    /// `None` to clear focus.
    val focusedCell: focusedCell: DataGridFocusedCell option -> Attr<'msg>

namespace FS.Skia.UI.Controls

/// Constructors for `ControlDiagnostic` values reported by the controls runtime and validation passes.
module Diagnostics =
    /// Builds a `ControlDiagnostic` from an explicit `code`, `severity`, and `message`, optionally scoped to a `controlId` and `kind`.
    val create:
        controlId: ControlId option ->
        kind: ControlKind ->
        code: ControlDiagnosticCode ->
        severity: ControlDiagnosticSeverity ->
        message: string ->
            ControlDiagnostic

    /// Reports that a control of `kind` is missing a required attribute `name`.
    val missingRequired: controlId: ControlId option -> kind: ControlKind -> name: string -> ControlDiagnostic
    /// Reports that attribute `name` was supplied more than once on a control of `kind`.
    val duplicateAttribute: controlId: ControlId option -> kind: ControlKind -> name: string -> ControlDiagnostic
    /// Reports that a control of `kind` lacks the accessibility metadata it requires.
    val missingAccessibility: controlId: ControlId option -> kind: ControlKind -> ControlDiagnostic
    /// Reports that the same `key` identifies two sibling controls, a `ControlId` collision.
    val keyCollision: key: ControlId -> kind: ControlKind -> ControlDiagnostic
    /// Reports that a control of `kind` requested a `capability` the host environment does not support.
    val unsupportedEnvironment: kind: ControlKind -> capability: string -> ControlDiagnostic
    /// Reports that standard control `kind` does not support the standard attribute `name`.
    val unsupportedStandardAttribute: kind: StandardControlKind -> name: StandardAttributeName -> ControlDiagnostic
    /// Reports that standard control `kind` does not raise the standard event `eventKind`.
    val unsupportedStandardEvent: kind: StandardControlKind -> eventKind: StandardEventKind -> ControlDiagnostic
    /// Reports that standard control `kind` omits a required standard attribute `name`.
    val missingStandardAttribute: kind: StandardControlKind -> name: StandardAttributeName -> ControlDiagnostic
    /// Reports that a control `kind` declares a non-standard `extensionName` outside the standard contract.
    val customExtension: kind: string -> extensionName: string -> ControlDiagnostic
    /// Reports a `packageId` reference at `path` that no longer resolves to a current package.
    val stalePackageReference: packageId: string -> path: string -> ControlDiagnostic
    /// Reports that `packageId` exposes a transitive `dependencyPath` that should not leak across the package boundary.
    val dependencyLeak: packageId: string -> dependencyPath: string -> ControlDiagnostic
    /// Reports that the catalog entry for `controlId` omits a `requiredField`.
    val catalogOmission: controlId: string -> requiredField: string -> ControlDiagnostic
    /// Reports that `runtimeName` is defined more than once, the duplicate residing at `path`.
    val duplicateRuntimeDefinition: runtimeName: string -> path: string -> ControlDiagnostic
    /// Reports that an `eventKind` binding targets a `controlId` that no longer exists in the tree.
    val staleEventTarget: controlId: ControlId -> eventKind: string -> ControlDiagnostic
    /// Reports that `scopeName` owned by `owner` cannot be expanded as requested.
    val unsupportedScopeExpansion: scopeName: string -> owner: string -> ControlDiagnostic

namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene

/// The font weight applied to a `RichTextRun`: `Regular`, `Medium`, or `Bold`.
type RichTextWeight =
    | Regular
    | Medium
    | Bold

/// The visual styling of a `RichTextRun`: font family/size, `Weight`, foreground/background colors, underline, and italic.
type RichTextStyle =
    { FontFamily: string option
      FontSize: float
      Weight: RichTextWeight
      Foreground: Color
      Background: Color option
      Underline: bool
      Italic: bool }

/// A single span of `Text` carrying one `RichTextStyle`, the atomic unit composed into a `RichTextBlock`.
type RichTextRun =
    { Text: string
      Style: RichTextStyle
      Diagnostics: ControlDiagnostic list }

/// An ordered sequence of `Runs` with optional `MaxWidth`, clipping, effects, and accessibility metadata, forming a layout unit.
type RichTextBlock =
    { Runs: RichTextRun list
      MaxWidth: float option
      Clip: bool
      Effects: string list
      Accessibility: AccessibilityMetadata option }

/// The measured layout of a `RichTextBlock`: `Width`, `Height`, `LineCount`, and any measurement diagnostics.
type RichTextMeasurement =
    { Width: float
      Height: float
      LineCount: int
      Diagnostics: ControlDiagnostic list }

/// Builders for styled rich-text `RichTextRun`/`RichTextBlock` values and their lowering to a `Control`.
module RichText =
    /// Returns the baseline `RichTextStyle` derived from the supplied `Theme`.
    val defaultStyle: Theme -> RichTextStyle
    /// Builds a `RichTextRun` pairing `text` with a `style`.
    val run: text: string -> style: RichTextStyle -> RichTextRun
    /// Assembles a `RichTextBlock` from an ordered list of `runs`.
    val block: runs: RichTextRun list -> RichTextBlock
    /// Computes the `RichTextMeasurement` (width, height, line count) for `block`.
    val measure: block: RichTextBlock -> RichTextMeasurement
    /// Lowers a `RichTextBlock` and its `Attr` list into a renderable `Control`.
    val create: block: RichTextBlock -> Attr<'msg> list -> Control<'msg>

namespace FS.Skia.UI.Controls

/// Whether a `TextInputModel` accepts a `SingleLine` or `MultiLine` of text.
type TextInputMode =
    | SingleLine
    | MultiLine

/// A selected character range (`Start`..`End`) within a `TextInputModel`.
type TextSelection =
    { Start: int
      End: int }

/// The MVU state of a text field: committed vs. draft text, `CaretIndex`, `Selection`, in-flight `Composition`, `Validation`, and focus.
type TextInputModel =
    { ControlId: ControlId
      Mode: TextInputMode
      CommittedText: string
      DraftText: string
      CaretIndex: int
      Selection: TextSelection option
      Composition: string option
      Validation: ValidationState
      Focused: bool }

/// An input message driving `TextInput.update`, e.g. `Focus`, `InsertText`, `MoveCaret`, `Commit`, `Cancel`, or composition events.
type TextInputMsg =
    | Focus
    | Blur
    | InsertText of string
    | MoveCaret of int
    | SelectRange of int * int
    | RequestClipboardPaste
    | ClipboardTextReceived of string
    | Commit
    | Cancel
    | CompositionStarted of string
    | CompositionCommitted of string
    | ApplyValidation of ValidationState

/// A side effect raised by `TextInput.update`: a clipboard read request, a committed-text notification, or a reported diagnostic.
type TextInputEffect =
    | RequestClipboardText of ControlId
    | CommitText of ControlId * string
    | ReportTextInputDiagnostic of ControlDiagnostic

/// MVU text-field component covering caret, selection, IME composition, clipboard, and validation.
module TextInput =
    /// Seeds a `TextInputModel` for `controlId` in the given `mode` with an initial `value`, plus any startup effects.
    val init: controlId: ControlId -> mode: TextInputMode -> value: string -> TextInputModel * TextInputEffect list
    /// Pure transition applying `msg` to `model`, returning the next model and the `TextInputEffect` list it raises.
    val update: msg: TextInputMsg -> model: TextInputModel -> TextInputModel * TextInputEffect list
    /// Maps a host-fulfilled `effect` back into the `TextInputMsg` that feeds it into `update`, if any.
    val interpretEffect: effect: TextInputEffect -> TextInputMsg option
    /// Returns the `ControlDiagnostic` list implied by the current `model` state.
    val diagnostics: model: TextInputModel -> ControlDiagnostic list

namespace FS.Skia.UI.Controls

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
/// Palette and density tokens for controls: built-in `light`/`dark` themes plus `withDensity`/`withAccent`/`resolve`.
module Theme =
    /// The built-in light `Theme` (DTCG `DesignTokens.Light` palette).
    val light: Theme
    /// The built-in dark `Theme` (DTCG `DesignTokens.Dark` palette).
    val dark: Theme
    /// Return `theme` scaled by `density` (spacing/size multiplier) for compact or comfortable layouts.
    val withDensity: density: float -> theme: Theme -> Theme
    /// Return `theme` with its accent colour replaced by `accent`.
    val withAccent: accent: FS.Skia.UI.Scene.Color -> theme: Theme -> Theme
    /// Resolve the effective `Theme`: the caller's `overrides` if present, otherwise the `light` default.
    val resolve: overrides: Theme option -> Theme

namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene
open FS.Skia.UI.Layout

/// Stable string identity of a control instance (`ControlId`), used as the join key
/// across `Bounds`, `EventBindings`, and `BoundIds` for hit-testing and event dispatch.
type ControlId = string
/// String tag naming a control's kind (`ControlKind`), e.g. the lowered form of a
/// `StandardControlKind` such as `"button"` or `"line-chart"`.
type ControlKind = string

/// A single plotted datum (`ChartPoint`): `X`/`Y` coordinates plus an optional `Label`
/// for line, bar, pie, and scatter chart kinds.
type ChartPoint =
    { X: float
      Y: float
      Label: string option }

/// A named collection of points (`ChartSeries`): a display `Name` and the ordered
/// `Points` it contributes to a chart control.
type ChartSeries =
    { Name: string
      Points: ChartPoint list }

[<RequireQualifiedAccess>]
/// Closed enumeration (`KnownControl`) of the built-in control kinds the package
/// recognises by name, from `TextBlock` through the chart family to `DataGrid`.
type KnownControl =
    | TextBlock
    | Button
    | TextBox
    | LineChart
    | BarChart
    | PieChart
    | ScatterPlot
    | GraphView
    | DataGrid

[<RequireQualifiedAccess>]
/// Closed enumeration (`KnownEvent`) of the built-in event kinds controls raise,
/// e.g. `Click`, `Changed`, `Selected`, `FocusChanged`, and `SortChanged`.
type KnownEvent =
    | Click
    | Changed
    | Selected
    | FocusChanged
    | SortChanged

[<RequireQualifiedAccess>]
/// Closed enumeration (`KnownAttribute`) of the built-in attribute names controls
/// accept, spanning content (`Text`/`Value`), data (`Series`/`Items`/`Nodes`), and
/// grid state (`SelectedRows`/`FocusedCell`).
type KnownAttribute =
    | Text
    | Value
    | Children
    | Series
    | Values
    | Columns
    | Rows
    | Items
    | Nodes
    | VisibleRange
    | SelectedRows
    | FocusedCell

[<RequireQualifiedAccess>]
/// The schema-facing control kind (`StandardControlKind`): the built-in kinds plus a
/// `Custom of string` escape hatch for consumer-defined controls.
type StandardControlKind =
    | TextBlock
    | Button
    | TextBox
    | LineChart
    | BarChart
    | PieChart
    | ScatterPlot
    | GraphView
    | DataGrid
    | Custom of string

[<RequireQualifiedAccess>]
/// The schema-facing event kind (`StandardEventKind`): the built-in events plus a
/// `Custom of string` case for consumer-defined event names.
type StandardEventKind =
    | Click
    | Changed
    | Selected
    | FocusChanged
    | SortChanged
    | Custom of string

[<RequireQualifiedAccess>]
/// The schema-facing attribute name (`StandardAttributeName`): the built-in attribute
/// names plus a `Custom of string` case for consumer-defined attributes.
type StandardAttributeName =
    | Text
    | Value
    | Children
    | Series
    | Values
    | Columns
    | Rows
    | Items
    | Nodes
    | VisibleRange
    | SelectedRows
    | FocusedCell
    | Custom of string

/// Schema-facing attribute value (`StandardAttributeValue`): a typed union over the
/// primitive shapes an attribute may carry — `StandardText`/`StandardBool`/`StandardFloat`,
/// a `StandardStringList`, a `StandardMessage`/`StandardEvent` payload, or `StandardUntyped`.
type StandardAttributeValue<'msg> =
    | StandardText of string
    | StandardBool of bool
    | StandardFloat of float
    | StandardStringList of string list
    | StandardMessage of 'msg
    | StandardEvent of (string -> 'msg)
    | StandardUntyped of obj

/// Per-kind authoring contract (`ControlSchema`): the control `Kind`, its
/// `RequiredAttributes` and `SupportedAttributes`, the `SupportedEvents` it raises, and
/// whether `CustomAllowed` extension attributes are permitted.
type ControlSchema =
    { Kind: StandardControlKind
      RequiredAttributes: StandardAttributeName list
      SupportedAttributes: StandardAttributeName list
      SupportedEvents: StandardEventKind list
      CustomAllowed: bool }

/// Severity level of a `ControlDiagnostic` (`ControlDiagnosticSeverity`): `Info`,
/// `Warning`, or `Error`, ordered from advisory to authoring-blocking.
type ControlDiagnosticSeverity =
    | Info
    | Warning
    | Error

/// Closed classification (`ControlDiagnosticCode`) of an authoring or runtime defect,
/// from `MissingRequiredAttribute` and `MissingStableKey` to `ContrastFailure`,
/// `KeyCollision`, and `StaleGeneratedReference`.
type ControlDiagnosticCode =
    | MissingRequiredAttribute
    | DuplicateAttribute
    | UnsupportedStateCombination
    | MissingStableKey
    | HitTestFailed
    | LayoutConflict
    | MissingAccessibilityMetadata
    | ContrastFailure
    | UnsupportedEnvironment
    | KeyCollision
    | StaleGeneratedReference

/// Accessibility/semantic role of a control (`AccessibilityRole`), e.g. `Button`,
/// `Slider`, `Grid`, or `Chart`; drives keyboard routing and assistive-tech naming,
/// with `Custom` for roles outside the built-in set.
type AccessibilityRole =
    | StaticText
    | Button
    | TextBox
    | CheckBox
    | RadioGroup
    | Slider
    | List
    | Grid
    | Menu
    | Tab
    | Dialog
    | Progress
    | Image
    | Chart
    | Graph
    | Custom

/// Keyboard contract of a control (`KeyboardOperation`): whether it is `Focusable`,
/// the `ActivationKeys` that trigger it, and the `NavigationKeys` it consumes for
/// internal movement.
type KeyboardOperation =
    { Focusable: bool
      ActivationKeys: string list
      NavigationKeys: string list }

/// Recorded contrast measurement (`ContrastEvidence`): the `Foreground`/`Background`
/// colors, the measured `Ratio`, and the `RequiredRatio` it is checked against.
type ContrastEvidence =
    { Foreground: Color
      Background: Color
      Ratio: float
      RequiredRatio: float }

/// Declared value/range metadata (`NavRange`) for slider/progress/numeric roles.
/// Feature 100 (R5): declared range metadata for value/range roles — the SOLE source of
/// step/bounds, replacing the host's hardcoded 0.1 / 0..1 slider constant (FR-002). A
/// DEFAULT-step slider declares <c>{ Step = 0.1; Min = 0.0; Max = 1.0 }</c> so the pre-R5
/// numeric path is reproduced byte-identically (FR-007). Validation: <c>Min &lt;= Max</c>;
/// <c>Step &gt; 0</c>.
type NavRange =
    { Step: float
      Min: float
      Max: float }

/// Per-control accessibility record (`AccessibilityMetadata`): the semantic `Role`,
/// `NameSource`, current `State` flags, optional `FocusOrder`, the `Keyboard` contract,
/// optional `Contrast` evidence, and optional value-range `Navigation` metadata.
type AccessibilityMetadata =
    { Role: AccessibilityRole
      NameSource: string
      State: string list
      FocusOrder: int option
      Keyboard: KeyboardOperation
      Contrast: ContrastEvidence option
      /// Feature 100 (R5): the declared value/range step + bounds for a range role
      /// (<c>Some</c> for Slider/Progress/numeric value roles), <c>None</c> otherwise. Read by
      /// both <c>Focus.route</c> and the host per-intent resolver.
      Navigation: NavRange option }

/// Validation status of an input control (`ValidationState`): `Valid`, `Invalid` with
/// an error message, or `Pending` with an in-progress message.
type ValidationState =
    | Valid
    | Invalid of string
    | Pending of string

/// Interaction/render state of a control (`VisualState`) consumed by the style resolver:
/// `Normal`, `Disabled`, `Hover`, `Pressed`, `Focused`, `Selected`, `Loading`, or a
/// `Validation`-wrapped `ValidationState`.
type VisualState =
    | Normal
    | Disabled
    | Hover
    | Pressed
    | Focused
    | Selected
    | Loading
    | Validation of ValidationState

[<RequireQualifiedAccess>]
/// Built-in semantic style variant (`StyleVariant`): `Primary`, `Danger`, `Ghost`,
/// `Neutral`, `Success`, or `Warning`.
/// Feature 093 (E3): the typed, CLOSED set of built-in semantic style variants — the
/// compiler-checked common path for declarative styling. Closure guarantees the resolver's
/// variant layer is a total match (FR-001, FR-002, FR-004). Free-form classes live one level
/// up in <c>StyleClass.Custom</c>.
type StyleVariant =
    | Primary
    | Danger
    | Ghost
    | Neutral
    | Success
    | Warning

/// One attached style class (`StyleClass`): a typed `Variant` wrapping a `StyleVariant`,
/// or a free-form `Custom` consumer-defined class name.
/// Feature 093 (E3): one attached-class entry — either a typed <c>StyleVariant</c> or a
/// free-form, consumer-defined class. A control carries a <c>StyleClass list</c> whose list
/// position IS the attach order the resolver folds left-to-right (FR-001, FR-003).
type StyleClass =
    | Variant of StyleVariant
    | Custom of string

/// Resolved paint and typography for a control (`ResolvedStyle`): `Foreground`, `Fill`,
/// `Stroke`/`StrokeWidth`, and `FontFamily`/`FontSize`/`FontWeight`, produced by `Style.resolve`.
/// Feature 093 (E3) — the per-control output of style resolution: the concrete paint/typography
/// the migrated kinds apply. A FLAT record so the fixed precedence is last-writer-wins per field
/// and the parity proof is a plain structural record comparison. Geometry is NOT here — the
/// resolver governs paint/typography only; geometry stays computed as today (data-model R3).
/// Declared before `Theme` so the shared field names (`Foreground`/`FontFamily`/`FontSize`)
/// resolve to `Theme` for unannotated `theme.*` accesses; produced by `Style.resolve`.
type ResolvedStyle =
    { Foreground: Color
      Fill: Color
      Stroke: Color
      StrokeWidth: float
      FontFamily: string option
      FontSize: float
      FontWeight: int option }

/// Design-token palette and metrics (`Theme`): the named color roles
/// (`Foreground`/`Background`/`Accent`/`Danger`/`Muted`), typography
/// (`FontFamily`/`FontSize`), and layout metrics (`Density`/`CornerRadius`/`ContrastRequiredRatio`).
type Theme =
    { Name: string
      Foreground: Color
      Background: Color
      Accent: Color
      Danger: Color
      Muted: Color
      FontFamily: string option
      FontSize: float
      Density: float
      CornerRadius: float
      ContrastRequiredRatio: float }

[<RequireQualifiedAccess>]
/// Input source that produced a `ControlEvent` (`ControlEventOrigin`): `Pointer`,
/// `Keyboard`, `Text`, `Focus`, `Selection`, or `Clipboard`.
type ControlEventOrigin =
    | Pointer
    | Keyboard
    | Text
    | Focus
    | Selection
    | Clipboard

/// Typed navigation outcome (`NavPayload`): `SteppedValue` for a value change,
/// `MovedSelection` for a selection move, or `MovedCell` for grid cell movement.
/// Feature 100 (R5): the closed set of navigation-outcome payload shapes (FR-005, SC-005).
/// Mirrors <c>NavIntent</c> one-to-one; exhaustively matched at the host edge.
type NavPayload =
    | SteppedValue of value: float
    | MovedSelection of index: int * item: string option
    | MovedCell of row: int * col: int

/// A dispatched control event (`ControlEvent`): its `Kind`, the source `ControlId`, the
/// `Origin` input source, an optional string `Payload`, and an optional typed `Nav` outcome.
type ControlEvent =
    { Kind: string
      ControlId: ControlId option
      Origin: ControlEventOrigin
      Payload: string option
      /// Feature 100 (R5): the closed typed navigation outcome for a focused-key navigation
      /// dispatch. A selection move dual-sets <c>Payload</c> (the moved item id, for existing
      /// string consumers) AND <c>Nav</c> (the closed <c>MovedSelection</c>); non-navigation
      /// events leave it <c>None</c>. <c>Payload : string option</c> is retained for backward
      /// compatibility (research R-3).
      Nav: NavPayload option }

/// Classification (`AttrCategory`) of what an attribute affects — `Content`, `Children`,
/// `Layout`, `Style`, `Theme`, `State`, `Validation`, `Accessibility`, `Event`, `Data`, or
/// `Slot` — used to route the attribute during lowering.
type AttrCategory =
    | Content
    | Children
    | Layout
    | Style
    | Theme
    | State
    | Validation
    | Accessibility
    | Event
    | Data
    /// Feature 095 (E5): the category under which named slot fills ride the `Attr` mechanism,
    /// mirroring E3's `Style`. Closed; only the internal `ControlInternals.slotFill` builder
    /// produces it — there is NO public free-form slot builder (the typed `Props` slot fields are
    /// the only sanctioned authoring path, FR-001).
    | Slot

/// The core declarative control node (`Control<'msg>`): its `Kind`, optional stable `Key`
/// identity, its `Attributes` and `Children`, optional text `Content`, and optional
/// `Accessibility` metadata. The unit of the authoring tree and the reconciler diff.
type Control<'msg> =
    { Kind: ControlKind
      Key: ControlId option
      Attributes: Attr<'msg> list
      Children: Control<'msg> list
      Content: string option
      Accessibility: AccessibilityMetadata option }

and Attr<'msg> =
    { Name: string
      Category: AttrCategory
      Value: AttrValue<'msg> }

and AttrValue<'msg> =
    | TextValue of string
    | BoolValue of bool
    | FloatValue of float
    | StringListValue of string list
    | ValidationValue of ValidationState
    /// Feature 093 (E3): an ordered list of attached style classes (list order = attach order).
    /// Rides the existing `Attr` mechanism under `AttrCategory.Style`; absent ≡ `[]` ≡ base.
    | StyleClassesValue of StyleClass list
    /// Feature 093 (E3): the control's current `VisualState`, consumed by `Style.resolve`. Rides
    /// the `Attr` mechanism so it travels WITH the control through the keyed reconciler diff — a
    /// state-driven look therefore survives a sibling-shifting re-render under E2's retained
    /// identity (FR-006, SC-005). Absent ≡ `Normal` ≡ the behaviour-preserving base case.
    | VisualStateValue of VisualState
    /// Feature 095 (E5): an ordered association list from declared slot NAME to the consumer's
    /// fill sub-tree. Rides the existing `Attr` mechanism under `AttrCategory.Slot` (the same shape
    /// E3 used for `StyleClassesValue`); a control carries at most one `Slot`-category attribute,
    /// last-writer-wins. The slot NAME is internal plumbing — a name ABSENT from this list is an
    /// unfilled slot (renders default), a name PRESENT is filled (renders the sub-tree, even when
    /// the sub-tree is empty). A slot fill is a static `Control<'msg>` value, NOT a data-bound
    /// template (FR-008). Lowering injects the fills into the control's `Children`, so they inherit
    /// E1–E4 + E2 retained identity by construction (FR-004, FR-005).
    | SlotFillsValue of (string * Control<'msg>) list
    | AccessibilityValue of AccessibilityMetadata
    | ThemeValue of Theme
    | ChildValue of Control<'msg>
    | ChildrenValue of Control<'msg> list
    | MessageValue of 'msg
    | EventValue of (ControlEvent -> 'msg)
    | UntypedValue of obj

/// A reported authoring/runtime issue (`ControlDiagnostic`): the offending `ControlId`
/// and `ControlKind`, the diagnostic `Code` and `Severity`, a human-readable `Message`,
/// and an optional `EvidencePath`.
type ControlDiagnostic =
    { ControlId: ControlId option
      ControlKind: ControlKind
      Code: ControlDiagnosticCode
      Severity: ControlDiagnosticSeverity
      Message: string
      EvidencePath: string option }

/// A wired event handler (`ControlEventBinding<'msg>`): binds a `ControlId` and
/// `EventKind` to a `Dispatch` function turning a `ControlEvent` into a host message.
type ControlEventBinding<'msg> =
    { ControlId: ControlId
      EventKind: string
      Dispatch: ControlEvent -> 'msg }

/// Output of rendering a control tree (`ControlRenderResult<'msg>`): the painted `Scene`,
/// the `Layout` root, the per-control `Bounds`, any `Diagnostics`, the `EventBindings` and
/// their `BoundIds`, and the total `NodeCount`.
type ControlRenderResult<'msg> =
    { Scene: Scene
      Layout: LayoutNode
      /// Evaluated absolute bounds of every laid-out control, keyed by `ControlId`
      /// (one entry per laid-out control instance). Populated by `Control.renderTree`
      /// from the computed `LayoutResult`; the preview `Control.render` leaves it empty.
      /// A host joins this with `EventBindings` (also keyed by `ControlId`) for hit-testing.
      Bounds: (ControlId * Rect) list
      Diagnostics: ControlDiagnostic list
      EventBindings: ControlEventBinding<'msg> list
      /// Canonical ids (the unified `Key ?? structural-path` scheme) of every node
      /// carrying at least one event binding. The same scheme as `EventBindings` and
      /// `Bounds`, so a recovered id is a direct membership/lookup key. Populated by
      /// `renderTree` and `render` (and the retained path); read by `nearestAuthored`.
      BoundIds: Set<ControlId>
      NodeCount: int }

```
