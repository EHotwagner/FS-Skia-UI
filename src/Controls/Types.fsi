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
      Diagnostics: ControlDiagnostic list
      EventBindings: ControlEventBinding<'msg> list
      NodeCount: int }
