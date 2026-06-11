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
