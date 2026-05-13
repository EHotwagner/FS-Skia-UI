namespace FS.Skia.UI.Layout

open FS.Skia.UI

type LayoutBounds =
    { X: float
      Y: float
      Width: float
      Height: float }

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
    val sizing : LayoutSizing
    val bounds : width: float -> height: float -> LayoutBounds
    val stackConfig : width: float -> height: float -> StackConfig
    val dockConfig : width: float -> height: float -> DockConfig
    val graphConfig : kind: GraphKind -> width: float -> height: float -> GraphConfig
    val child : content: Scene -> LayoutChild
