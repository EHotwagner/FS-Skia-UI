namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene
open FS.Skia.UI.Layout

type ControlId = string
type ControlKind = string

[<RequireQualifiedAccess>]
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
type KnownEvent =
    | Click
    | Changed
    | Selected
    | FocusChanged
    | SortChanged

[<RequireQualifiedAccess>]
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
type StandardEventKind =
    | Click
    | Changed
    | Selected
    | FocusChanged
    | SortChanged
    | Custom of string

[<RequireQualifiedAccess>]
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

type StandardAttributeValue<'msg> =
    | StandardText of string
    | StandardBool of bool
    | StandardFloat of float
    | StandardStringList of string list
    | StandardMessage of 'msg
    | StandardEvent of (string -> 'msg)
    | StandardUntyped of obj

type ControlSchema =
    { Kind: StandardControlKind
      RequiredAttributes: StandardAttributeName list
      SupportedAttributes: StandardAttributeName list
      SupportedEvents: StandardEventKind list
      CustomAllowed: bool }

type ControlDiagnosticSeverity =
    | Info
    | Warning
    | Error

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

type KeyboardOperation =
    { Focusable: bool
      ActivationKeys: string list
      NavigationKeys: string list }

type ContrastEvidence =
    { Foreground: Color
      Background: Color
      Ratio: float
      RequiredRatio: float }

type AccessibilityMetadata =
    { Role: AccessibilityRole
      NameSource: string
      State: string list
      FocusOrder: int option
      Keyboard: KeyboardOperation
      Contrast: ContrastEvidence option }

type ValidationState =
    | Valid
    | Invalid of string
    | Pending of string

type VisualState =
    | Normal
    | Disabled
    | Hover
    | Pressed
    | Focused
    | Selected
    | Loading
    | Validation of ValidationState

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
type ControlEventOrigin =
    | Pointer
    | Keyboard
    | Text
    | Focus
    | Selection
    | Clipboard

type ControlEvent =
    { Kind: string
      ControlId: ControlId option
      Origin: ControlEventOrigin
      Payload: string option }

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

type ControlDiagnostic =
    { ControlId: ControlId option
      ControlKind: ControlKind
      Code: ControlDiagnosticCode
      Severity: ControlDiagnosticSeverity
      Message: string
      EvidencePath: string option }

type ControlEventBinding<'msg> =
    { ControlId: ControlId
      EventKind: string
      Dispatch: ControlEvent -> 'msg }

type ControlRenderResult<'msg> =
    { Scene: Scene
      Layout: LayoutNode
      Diagnostics: ControlDiagnostic list
      EventBindings: ControlEventBinding<'msg> list
      NodeCount: int }
