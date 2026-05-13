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
    let padding =
        { Left = 0.0
          Top = 0.0
          Right = 0.0
          Bottom = 0.0 }

    let sizing =
        { DesiredWidth = None
          DesiredHeight = None
          HorizontalAlignment = HorizontalAlignment.Stretch
          VerticalAlignment = VerticalAlignment.Stretch }

    let bounds width height =
        { X = 0.0
          Y = 0.0
          Width = width
          Height = height }

    let stackConfig width height : StackConfig =
        { Bounds = bounds width height
          Padding = padding
          Spacing = 0.0 }

    let dockConfig width height : DockConfig =
        { Bounds = bounds width height
          Padding = padding
          Spacing = 0.0 }

    let graphConfig kind width height =
        { Kind = kind
          Bounds = bounds width height }

    let child content =
        { Content = content
          Sizing = sizing
          Dock = None }
