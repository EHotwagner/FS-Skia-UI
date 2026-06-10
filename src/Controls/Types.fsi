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

[<RequireQualifiedAccess>]
/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract type exposed by this FS.Skia.UI package.
/// Feature 093 (E3): one attached-class entry — either a typed <c>StyleVariant</c> or a
/// free-form, consumer-defined class. A control carries a <c>StyleClass list</c> whose list
/// position IS the attach order the resolver folds left-to-right (FR-001, FR-003).
type StyleClass =
    | Variant of StyleVariant
    | Custom of string

/// Public contract type exposed by this FS.Skia.UI package.
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
    /// Feature 095 (E5): the category under which named slot fills ride the `Attr` mechanism,
    /// mirroring E3's `Style`. Closed; only the internal `ControlInternals.slotFill` builder
    /// produces it — there is NO public free-form slot builder (the typed `Props` slot fields are
    /// the only sanctioned authoring path, FR-001).
    | Slot

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
