namespace FS.Skia.UI.Controls

/// Internal extraction seam (feature 080) — `internal` accessibility, no public-surface
/// entry (mirrors `module internal Reconcile`); reached from `Controls.Tests` via
/// `InternalsVisibleTo`. Only `chartValues` is exposed, for the FR-002 extraction test that
/// proves the typed-front-door `ChartSeries`/`ChartPoint` data is read (pre-080: yielded `[]`).
module internal ControlInternals =
    /// Extract the chart data points (X/Y/Label preserved) a chart-like control carries.
    val chartValues: control: Control<'msg> -> ChartPoint list

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
