namespace FS.Skia.UI.Layout

open FS.Skia.UI

type LayoutBounds =
    { X: float
      Y: float
      Width: float
      Height: float }

type LayoutNodeId = string

type HorizontalAlignment =
    | Left
    | Center
    | Right
    | Stretch

type VerticalAlignment =
    | Top
    | Middle
    | Bottom
    | Stretch

type DockPosition =
    | Top
    | Bottom
    | Left
    | Right
    | Fill

type LayoutPadding =
    { Left: float
      Top: float
      Right: float
      Bottom: float }

type MeasureMode =
    | Undefined
    | Exactly
    | AtMost

type LayoutDirection =
    | Row
    | Column

type LayoutWrap =
    | NoWrap
    | Wrap

type LayoutAlign =
    | Auto
    | Start
    | Center
    | End
    | Stretch
    | SpaceBetween
    | SpaceAround
    | SpaceEvenly

type LayoutVisibility =
    | Visible
    | Hidden
    | Collapsed

type LayoutSize =
    { Width: float option
      Height: float option }

type LayoutGap =
    { Row: float
      Column: float }

type DiagnosticSeverity =
    | Info
    | Warning
    | Error

type LayoutDiagnosticCode =
    | InvalidAvailableSpace
    | InvalidLayoutValue
    | DuplicateLayoutNodeId
    | UnsatisfiedConstraint
    | UnmeasurableContent
    | FallbackBoundsApplied
    | UnsupportedLayoutIntent

type LayoutDiagnostic =
    { NodeId: LayoutNodeId option
      Code: LayoutDiagnosticCode
      Severity: DiagnosticSeverity
      Message: string
      Constraint: string option
      FallbackApplied: bool }

type LayoutIntent =
    { Direction: LayoutDirection
      Wrap: LayoutWrap
      AlignItems: LayoutAlign
      AlignSelf: LayoutAlign option
      JustifyContent: LayoutAlign
      Padding: LayoutPadding
      Margin: LayoutPadding
      Gap: LayoutGap
      Size: LayoutSize
      MinSize: LayoutSize
      MaxSize: LayoutSize
      FlexGrow: float
      FlexShrink: float
      FlexBasis: float option }

type MeasureRequest =
    { AvailableWidth: float
      WidthMode: MeasureMode
      AvailableHeight: float
      HeightMode: MeasureMode }

type MeasureResponse =
    { Width: float
      Height: float
      Diagnostics: LayoutDiagnostic list }

type ContentMeasure = MeasureRequest -> MeasureResponse

type LayoutNode =
    { Id: LayoutNodeId
      Intent: LayoutIntent
      Visibility: LayoutVisibility
      Measure: ContentMeasure option
      Content: Scene option
      Children: LayoutNode list }

type AvailableSpace =
    { Width: float
      WidthMode: MeasureMode
      Height: float
      HeightMode: MeasureMode }

type ComputedBounds =
    { NodeId: LayoutNodeId
      Bounds: LayoutBounds
      Visibility: LayoutVisibility }

type LayoutResult =
    { Bounds: ComputedBounds list
      Diagnostics: LayoutDiagnostic list
      Invalidated: LayoutNodeId list
      Revision: int64 }

type SnapMode =
    | Floor
    | Round
    | Expand

type PixelSnapPolicy =
    { ScaleFactor: float
      Mode: SnapMode }

type LayoutWorkflowModel =
    { Root: LayoutNode
      Available: AvailableSpace
      Result: LayoutResult option
      LastChangedNodeIds: LayoutNodeId list
      PixelSnapPolicy: PixelSnapPolicy }

type LayoutWorkflowMsg =
    | LayoutHostResized of AvailableSpace
    | LayoutVisibilityChanged of LayoutNodeId * LayoutVisibility
    | LayoutIntentChanged of LayoutNodeId * LayoutIntent
    | LayoutMeasurementChanged of LayoutNodeId
    | LayoutEvaluationCompleted of LayoutResult

type LayoutWorkflowEffect =
    | EvaluateLayout
    | EvaluateIncrementalLayout of LayoutNodeId list

type LayoutSizing =
    { DesiredWidth: float option
      DesiredHeight: float option
      HorizontalAlignment: HorizontalAlignment
      VerticalAlignment: VerticalAlignment }

type LayoutChild =
    { Content: Scene
      Sizing: LayoutSizing
      Dock: DockPosition option }

type StackConfig =
    { Bounds: LayoutBounds
      Padding: LayoutPadding
      Spacing: float }

type DockConfig =
    { Bounds: LayoutBounds
      Padding: LayoutPadding
      Spacing: float }

type GraphKind =
    | Directed
    | Undirected

type GraphNode =
    { Id: string
      Label: string
      Style: Color option }

type GraphEdge =
    { Source: string
      Target: string
      Weight: float option
      Label: string option }

type GraphConfig =
    { Kind: GraphKind
      Bounds: LayoutBounds }

type GraphDefinition =
    { Config: GraphConfig
      Nodes: GraphNode list
      Edges: GraphEdge list }

type GraphNodeLayout =
    { Node: GraphNode
      Bounds: LayoutBounds }

type GraphLayoutResult =
    { Nodes: GraphNodeLayout list
      Edges: GraphEdge list }

module Defaults =
    val padding : LayoutPadding
    val layoutGap : LayoutGap
    val layoutSize : LayoutSize
    val layoutIntent : LayoutIntent
    val layoutNode : id: LayoutNodeId -> LayoutNode
    val availableSpace : width: float -> height: float -> AvailableSpace
    val pixelSnapPolicy : scaleFactor: float -> PixelSnapPolicy
    val sizing : LayoutSizing
    val bounds : width: float -> height: float -> LayoutBounds
    val stackConfig : width: float -> height: float -> StackConfig
    val dockConfig : width: float -> height: float -> DockConfig
    val graphConfig : kind: GraphKind -> width: float -> height: float -> GraphConfig
    val child : content: Scene -> LayoutChild
