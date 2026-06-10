# FS.Skia.UI.Controls Source-Shaped API Reference

package-id: FS.Skia.UI.Controls
package-version: local
generated-from: curated-fsi
assembly-reflection: false
repository-source-authoring-fallback: false
symbol-count: 710
xml-summary-count: 344
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

/// Public contract module exposed by this FS.Skia.UI package.
module Accessibility =
    /// Public contract function exposed by this FS.Skia.UI package.
    val keyboard: focusable: bool -> activationKeys: string list -> navigationKeys: string list -> KeyboardOperation
    /// Public contract function exposed by this FS.Skia.UI package.
    val contrast: foreground: FS.Skia.UI.Scene.Color -> background: FS.Skia.UI.Scene.Color -> ratio: float -> requiredRatio: float -> ContrastEvidence
    /// Public contract function exposed by this FS.Skia.UI package.
    val metadata:
        role: AccessibilityRole ->
        nameSource: string ->
        state: string list ->
        focusOrder: int option ->
        keyboard: KeyboardOperation ->
        contrast: ContrastEvidence option ->
            AccessibilityMetadata

    /// Public contract function exposed by this FS.Skia.UI package.
    val defaultFor: kind: ControlKind -> label: string -> AccessibilityMetadata
    /// Public contract function exposed by this FS.Skia.UI package.
    val validate: control: Control<'msg> -> ControlDiagnostic list

namespace FS.Skia.UI.Controls

/// Public contract module exposed by this FS.Skia.UI package.
module Attr =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: name: string -> category: AttrCategory -> value: AttrValue<'msg> -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val standardAttribute: name: StandardAttributeName -> value: StandardAttributeValue<'msg> -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val customAttribute: name: string -> value: obj -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val standardEvent: eventKind: StandardEventKind -> msg: 'msg -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val customEvent: eventKind: string -> msg: 'msg -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val text: value: string -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val value: value: string -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val items: values: string list -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val child: control: Control<'msg> -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val children: controls: Control<'msg> list -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val enabled: value: bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val visible: value: bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val readOnly: value: bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val loading: value: bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val selected: value: bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val width: value: float -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val height: value: float -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val padding: value: float -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val margin: value: float -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val style: name: string -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val theme: theme: Theme -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val validation: state: ValidationState -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val accessibility: metadata: AccessibilityMetadata -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val on: eventKind: string -> msg: 'msg -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onWith: eventKind: string -> map: (ControlEvent -> 'msg) -> Attr<'msg>

namespace FS.Skia.UI.Controls

/// Public contract type exposed by this FS.Skia.UI package.
type CatalogAccessibility =
    { Role: string
      NameSource: string
      StateMetadata: string list
      FocusBehavior: string
      KeyboardOperation: string
      ContrastEvidence: string }

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract module exposed by this FS.Skia.UI package.
module Catalog =
    /// Public contract function exposed by this FS.Skia.UI package.
    val supportedControls: ControlDefinition list
    /// Public contract function exposed by this FS.Skia.UI package.
    val standardSchema: ControlSchema list
    /// Public contract function exposed by this FS.Skia.UI package.
    val knownControlKinds: unit -> StandardControlKind list
    /// Public contract function exposed by this FS.Skia.UI package.
    val requiredAttributes: kind: StandardControlKind -> StandardAttributeName list
    /// Public contract function exposed by this FS.Skia.UI package.
    val supportedAttributes: kind: StandardControlKind -> StandardAttributeName list
    /// Public contract function exposed by this FS.Skia.UI package.
    val supportedEvents: kind: StandardControlKind -> StandardEventKind list
    /// Public contract function exposed by this FS.Skia.UI package.
    val validateStandardControl: control: Control<'msg> -> ControlDiagnostic list
    /// Public contract function exposed by this FS.Skia.UI package.
    val supportedCount: unit -> int
    /// Public contract function exposed by this FS.Skia.UI package.
    val categories: unit -> string list
    /// Public contract function exposed by this FS.Skia.UI package.
    val validate: unit -> ControlDiagnostic list
    /// Public contract function exposed by this FS.Skia.UI package.
    val markdownSummary: unit -> string

namespace FS.Skia.UI.Controls

// `ChartPoint` / `ChartSeries` are declared in Types.fsi (feature 080, surface-neutral move).

/// Public contract module exposed by this FS.Skia.UI package.
module LineChart =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val series: ChartSeries list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module BarChart =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val series: ChartSeries list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module PieChart =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val values: ChartPoint list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module ScatterPlot =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val series: ChartSeries list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module GraphView =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val nodes: string list -> Attr<'msg>

namespace FS.Skia.UI.Controls

/// Public contract type exposed by this FS.Skia.UI package.
type VisibleRange =
    { FirstIndex: int
      Count: int
      Total: int }

/// Public contract type exposed by this FS.Skia.UI package.
type CollectionModel =
    { ControlId: ControlId
      ItemCount: int
      RowHeight: float
      ViewportHeight: float
      ScrollOffset: float
      SelectedKeys: Set<string>
      VisibleRange: VisibleRange
      RecalculationThresholdMs: int }

/// Public contract type exposed by this FS.Skia.UI package.
type CollectionMsg =
    | ScrollTo of float
    | SelectKey of string
    | ToggleKey of string
    | ReplaceItemCount of int

/// Public contract type exposed by this FS.Skia.UI package.
type CollectionEffect =
    | VisibleRangeChanged of VisibleRange

/// Public contract module exposed by this FS.Skia.UI package.
module Collections =
    /// Public contract function exposed by this FS.Skia.UI package.
    val visibleRange: rowHeight: float -> viewportHeight: float -> scrollOffset: float -> totalItems: int -> VisibleRange
    /// Public contract function exposed by this FS.Skia.UI package.
    val init: controlId: ControlId -> itemCount: int -> rowHeight: float -> viewportHeight: float -> CollectionModel * CollectionEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val update: msg: CollectionMsg -> model: CollectionModel -> CollectionModel * CollectionEffect list

namespace FS.Skia.UI.Controls

/// Internal extraction seam (feature 080) — `internal` accessibility, no public-surface
/// entry (mirrors `module internal Reconcile`); reached from `Controls.Tests` via
/// `InternalsVisibleTo`. Only `chartValues` is exposed, for the FR-002 extraction test that
/// proves the typed-front-door `ChartSeries`/`ChartPoint` data is read (pre-080: yielded `[]`).
module internal ControlInternals =
    /// Extract the chart data points (X/Y/Label preserved) a chart-like control carries.
    val chartValues: control: Control<'msg> -> ChartPoint list

    /// Feature 091 — the per-node measure of `Control.renderTree`, factored so the wired
    /// retained path (`module internal RetainedRender`) measures with the IDENTICAL function.
    /// Builds + evaluates the nested Yoga layout, returning the root node and the absolute
    /// bounds keyed by the collision-free structural id (`Key |> defaultValue path`).
    val evaluateLayout:
        size: FS.Skia.UI.Scene.Size ->
        control: Control<'msg> ->
            FS.Skia.UI.Layout.LayoutNode * Map<string, FS.Skia.UI.Layout.LayoutBounds>

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

/// Public contract module exposed by this FS.Skia.UI package.
module Control =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: kind: ControlKind -> attrs: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val standard: kind: StandardControlKind -> attrs: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val customControl: kind: string -> attrs: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val lowerStandard: control: Control<'msg> -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val lowerCustom: control: Control<'msg> -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val withKey: key: ControlId -> control: Control<'msg> -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
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
    /// Public contract function exposed by this FS.Skia.UI package.
    val diagnostics: control: Control<'msg> -> ControlDiagnostic list
    /// Public contract function exposed by this FS.Skia.UI package.
    val dispatch: event: ControlEvent -> control: Control<'msg> -> 'msg list
    /// Public contract function exposed by this FS.Skia.UI package.
    val count: control: Control<'msg> -> int

/// Public contract module exposed by this FS.Skia.UI package.
module TextBlock =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val text: string -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Label =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val text: string -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Image =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val source: string -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Icon =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val name: string -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Separator =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Badge =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val text: string -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Button =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val text: string -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val enabled: bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onClick: 'msg -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onClickWith: (ControlEvent -> 'msg) -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module IconButton =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val icon: string -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onClick: 'msg -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module CheckBox =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val text: string -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val checked': bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onChanged: (bool -> 'msg) -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Switch =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val checked': bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onChanged: (bool -> 'msg) -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Slider =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val value: float -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onChanged: (float -> 'msg) -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module NumericInput =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val value: float -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onChanged: (float -> 'msg) -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module TextBox =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val value: string -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val readOnly: bool -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val validation: ValidationState -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onChanged: (string -> 'msg) -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module TextArea =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val value: string -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onChanged: (string -> 'msg) -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module RadioGroup =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val items: string list -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val selected: string -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onChanged: (string -> 'msg) -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Stack =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val children: Control<'msg> list -> Attr<'msg>
    /// Lay the stack's children along the row axis when value = "horizontal"; any other
    /// value (or omission) keeps the default vertical column (FR-007).
    val orientation: string -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Grid =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val children: Control<'msg> list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Dock =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val children: Control<'msg> list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Wrap =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val children: Control<'msg> list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Border =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val child: Control<'msg> -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Panel =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val children: Control<'msg> list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module ProgressBar =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val value: float -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Spinner =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module ValidationMessage =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val text: string -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Tabs =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val items: string list -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val selected: string -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onChanged: (string -> 'msg) -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Menu =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val items: string list -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val onSelected: (string -> 'msg) -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Toolbar =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val children: Control<'msg> list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Tooltip =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val text: string -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Dialog =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val children: Control<'msg> list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Toast =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val text: string -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module Overlay =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val child: Control<'msg> -> Attr<'msg>

namespace FS.Skia.UI.Controls

/// Public contract type exposed by this FS.Skia.UI package.
type ControlCaret =
    { ControlId: ControlId
      Index: int }

/// Public contract type exposed by this FS.Skia.UI package.
type ControlSelection =
    { ControlId: ControlId
      Start: int
      End: int }

/// Public contract type exposed by this FS.Skia.UI package.
type ControlComposition =
    { ControlId: ControlId
      Text: string }

/// Public contract type exposed by this FS.Skia.UI package.
type ControlDrag =
    { ControlId: ControlId
      StartX: float
      StartY: float
      CurrentX: float
      CurrentY: float }

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract module exposed by this FS.Skia.UI package.
module ControlRuntime =
    /// Public contract function exposed by this FS.Skia.UI package.
    val init: unit -> ControlRuntimeModel * ControlRuntimeEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val update: msg: ControlRuntimeMsg -> model: ControlRuntimeModel -> ControlRuntimeModel * ControlRuntimeEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val diagnostics: model: ControlRuntimeModel -> ControlDiagnostic list

namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene
open FS.Skia.UI.Layout

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract module exposed by this FS.Skia.UI package.
module CustomControl =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: definition: CustomControlDefinition<'msg> -> attrs: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val validate: definition: CustomControlDefinition<'msg> -> ControlDiagnostic list

namespace FS.Skia.UI.Controls

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridColumnType =
    | TextColumn
    | NumericColumn
    | BooleanColumn
    | CustomColumn of string

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridColumn =
    { Key: string
      Header: string
      Width: float
      ColumnType: DataGridColumnType }

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridCell =
    { RowKey: string
      ColumnKey: string
      Value: string }

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridRow =
    { Key: string
      Cells: DataGridCell list }

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridSortDirection =
    | Ascending
    | Descending

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridSort =
    { ColumnKey: string
      Direction: DataGridSortDirection }

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridFocusedCell =
    { RowKey: string
      ColumnKey: string }

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridMsg =
    | ScrollRowsTo of int
    | SelectRow of string
    | ToggleRow of string
    | FocusCell of DataGridFocusedCell option
    | SortBy of string
    | ApplyFilter of string option
    | ReplaceRowCount of int

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridEffect =
    | DataGridVisibleRangeChanged of VisibleRange
    | DataGridSelectionChanged of string list
    | DataGridFocusChanged of DataGridFocusedCell option
    | DataGridSortChanged of DataGridSort option
    | DataGridFilterChanged of string option
    | ReportDataGridDiagnostic of ControlDiagnostic

/// Public contract module exposed by this FS.Skia.UI package.
module DataGrid =
    /// Public contract function exposed by this FS.Skia.UI package.
    val init: controlId: ControlId -> columns: DataGridColumn list -> rowCount: int -> rowHeight: float -> viewportHeight: float -> DataGridModel * DataGridEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val update: msg: DataGridMsg -> model: DataGridModel -> DataGridModel * DataGridEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: columns: DataGridColumn list -> attrs: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val columns: columns: DataGridColumn list -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val rows: rows: DataGridRow list -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val visibleRange: visibleRange: VisibleRange -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val selectedRows: selectedRows: Set<string> -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val focusedCell: focusedCell: DataGridFocusedCell option -> Attr<'msg>

namespace FS.Skia.UI.Controls

/// Public contract module exposed by this FS.Skia.UI package.
module Diagnostics =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create:
        controlId: ControlId option ->
        kind: ControlKind ->
        code: ControlDiagnosticCode ->
        severity: ControlDiagnosticSeverity ->
        message: string ->
            ControlDiagnostic

    /// Public contract function exposed by this FS.Skia.UI package.
    val missingRequired: controlId: ControlId option -> kind: ControlKind -> name: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val duplicateAttribute: controlId: ControlId option -> kind: ControlKind -> name: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val missingAccessibility: controlId: ControlId option -> kind: ControlKind -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val keyCollision: key: ControlId -> kind: ControlKind -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val unsupportedEnvironment: kind: ControlKind -> capability: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val unsupportedStandardAttribute: kind: StandardControlKind -> name: StandardAttributeName -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val unsupportedStandardEvent: kind: StandardControlKind -> eventKind: StandardEventKind -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val missingStandardAttribute: kind: StandardControlKind -> name: StandardAttributeName -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val customExtension: kind: string -> extensionName: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val stalePackageReference: packageId: string -> path: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val dependencyLeak: packageId: string -> dependencyPath: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val catalogOmission: controlId: string -> requiredField: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val duplicateRuntimeDefinition: runtimeName: string -> path: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val staleEventTarget: controlId: ControlId -> eventKind: string -> ControlDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val unsupportedScopeExpansion: scopeName: string -> owner: string -> ControlDiagnostic

namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene

/// Public contract type exposed by this FS.Skia.UI package.
type RichTextWeight =
    | Regular
    | Medium
    | Bold

/// Public contract type exposed by this FS.Skia.UI package.
type RichTextStyle =
    { FontFamily: string option
      FontSize: float
      Weight: RichTextWeight
      Foreground: Color
      Background: Color option
      Underline: bool
      Italic: bool }

/// Public contract type exposed by this FS.Skia.UI package.
type RichTextRun =
    { Text: string
      Style: RichTextStyle
      Diagnostics: ControlDiagnostic list }

/// Public contract type exposed by this FS.Skia.UI package.
type RichTextBlock =
    { Runs: RichTextRun list
      MaxWidth: float option
      Clip: bool
      Effects: string list
      Accessibility: AccessibilityMetadata option }

/// Public contract type exposed by this FS.Skia.UI package.
type RichTextMeasurement =
    { Width: float
      Height: float
      LineCount: int
      Diagnostics: ControlDiagnostic list }

/// Public contract module exposed by this FS.Skia.UI package.
module RichText =
    /// Public contract function exposed by this FS.Skia.UI package.
    val defaultStyle: Theme -> RichTextStyle
    /// Public contract function exposed by this FS.Skia.UI package.
    val run: text: string -> style: RichTextStyle -> RichTextRun
    /// Public contract function exposed by this FS.Skia.UI package.
    val block: runs: RichTextRun list -> RichTextBlock
    /// Public contract function exposed by this FS.Skia.UI package.
    val measure: block: RichTextBlock -> RichTextMeasurement
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: block: RichTextBlock -> Attr<'msg> list -> Control<'msg>

namespace FS.Skia.UI.Controls

/// Public contract type exposed by this FS.Skia.UI package.
type TextInputMode =
    | SingleLine
    | MultiLine

/// Public contract type exposed by this FS.Skia.UI package.
type TextSelection =
    { Start: int
      End: int }

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract type exposed by this FS.Skia.UI package.
type TextInputEffect =
    | RequestClipboardText of ControlId
    | CommitText of ControlId * string
    | ReportTextInputDiagnostic of ControlDiagnostic

/// Public contract module exposed by this FS.Skia.UI package.
module TextInput =
    /// Public contract function exposed by this FS.Skia.UI package.
    val init: controlId: ControlId -> mode: TextInputMode -> value: string -> TextInputModel * TextInputEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val update: msg: TextInputMsg -> model: TextInputModel -> TextInputModel * TextInputEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val interpretEffect: effect: TextInputEffect -> TextInputMsg option
    /// Public contract function exposed by this FS.Skia.UI package.
    val diagnostics: model: TextInputModel -> ControlDiagnostic list

namespace FS.Skia.UI.Controls

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
/// Public contract module exposed by this FS.Skia.UI package.
module Theme =
    /// Public contract function exposed by this FS.Skia.UI package.
    val light: Theme
    /// Public contract function exposed by this FS.Skia.UI package.
    val dark: Theme
    /// Public contract function exposed by this FS.Skia.UI package.
    val withDensity: density: float -> theme: Theme -> Theme
    /// Public contract function exposed by this FS.Skia.UI package.
    val withAccent: accent: FS.Skia.UI.Scene.Color -> theme: Theme -> Theme
    /// Public contract function exposed by this FS.Skia.UI package.
    val resolve: overrides: Theme option -> Theme

namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene
open FS.Skia.UI.Layout

/// Public contract type exposed by this FS.Skia.UI package.
type ControlId = string
/// Public contract type exposed by this FS.Skia.UI package.
type ControlKind = string

/// Public contract type exposed by this FS.Skia.UI package.
type ChartPoint =
    { X: float
      Y: float
      Label: string option }

/// Public contract type exposed by this FS.Skia.UI package.
type ChartSeries =
    { Name: string
      Points: ChartPoint list }

[<RequireQualifiedAccess>]
/// Public contract type exposed by this FS.Skia.UI package.
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
/// Public contract type exposed by this FS.Skia.UI package.
type KnownEvent =
    | Click
    | Changed
    | Selected
    | FocusChanged
    | SortChanged

[<RequireQualifiedAccess>]
/// Public contract type exposed by this FS.Skia.UI package.
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
/// Public contract type exposed by this FS.Skia.UI package.
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
/// Public contract type exposed by this FS.Skia.UI package.
type StandardEventKind =
    | Click
    | Changed
    | Selected
    | FocusChanged
    | SortChanged
    | Custom of string

[<RequireQualifiedAccess>]
/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract type exposed by this FS.Skia.UI package.
type StandardAttributeValue<'msg> =
    | StandardText of string
    | StandardBool of bool
    | StandardFloat of float
    | StandardStringList of string list
    | StandardMessage of 'msg
    | StandardEvent of (string -> 'msg)
    | StandardUntyped of obj

/// Public contract type exposed by this FS.Skia.UI package.
type ControlSchema =
    { Kind: StandardControlKind
      RequiredAttributes: StandardAttributeName list
      SupportedAttributes: StandardAttributeName list
      SupportedEvents: StandardEventKind list
      CustomAllowed: bool }

/// Public contract type exposed by this FS.Skia.UI package.
type ControlDiagnosticSeverity =
    | Info
    | Warning
    | Error

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract type exposed by this FS.Skia.UI package.
type KeyboardOperation =
    { Focusable: bool
      ActivationKeys: string list
      NavigationKeys: string list }

/// Public contract type exposed by this FS.Skia.UI package.
type ContrastEvidence =
    { Foreground: Color
      Background: Color
      Ratio: float
      RequiredRatio: float }

/// Public contract type exposed by this FS.Skia.UI package.
type AccessibilityMetadata =
    { Role: AccessibilityRole
      NameSource: string
      State: string list
      FocusOrder: int option
      Keyboard: KeyboardOperation
      Contrast: ContrastEvidence option }

/// Public contract type exposed by this FS.Skia.UI package.
type ValidationState =
    | Valid
    | Invalid of string
    | Pending of string

/// Public contract type exposed by this FS.Skia.UI package.
type VisualState =
    | Normal
    | Disabled
    | Hover
    | Pressed
    | Focused
    | Selected
    | Loading
    | Validation of ValidationState

/// Public contract type exposed by this FS.Skia.UI package.
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
/// Public contract type exposed by this FS.Skia.UI package.
type ControlEventOrigin =
    | Pointer
    | Keyboard
    | Text
    | Focus
    | Selection
    | Clipboard

/// Public contract type exposed by this FS.Skia.UI package.
type ControlEvent =
    { Kind: string
      ControlId: ControlId option
      Origin: ControlEventOrigin
      Payload: string option }

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract type exposed by this FS.Skia.UI package.
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
    | AccessibilityValue of AccessibilityMetadata
    | ThemeValue of Theme
    | ChildValue of Control<'msg>
    | ChildrenValue of Control<'msg> list
    | MessageValue of 'msg
    | EventValue of (ControlEvent -> 'msg)
    | UntypedValue of obj

/// Public contract type exposed by this FS.Skia.UI package.
type ControlDiagnostic =
    { ControlId: ControlId option
      ControlKind: ControlKind
      Code: ControlDiagnosticCode
      Severity: ControlDiagnosticSeverity
      Message: string
      EvidencePath: string option }

/// Public contract type exposed by this FS.Skia.UI package.
type ControlEventBinding<'msg> =
    { ControlId: ControlId
      EventKind: string
      Dispatch: ControlEvent -> 'msg }

/// Public contract type exposed by this FS.Skia.UI package.
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
      NodeCount: int }

```
