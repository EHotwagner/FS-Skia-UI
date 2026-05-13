module LayoutTests

open Expecto
open System.Diagnostics
open FS.Skia.UI
open FS.Skia.UI.Layout

let child label =
    Defaults.child (Scene.text (0.0, 0.0) label Colors.white)

let overlaps (a: LayoutBounds) (b: LayoutBounds) =
    a.X < b.X + b.Width
    && a.X + a.Width > b.X
    && a.Y < b.Y + b.Height
    && a.Y + a.Height > b.Y

let node id =
    { Id = id
      Label = id
      Style = None }

let edge source target =
    { Source = source
      Target = target
      Weight = None
      Label = None }

let graph kind nodes edges =
    { Config = Defaults.graphConfig kind 640.0 360.0
      Nodes = nodes
      Edges = edges }

[<Tests>]
let contractTests =
    testList "Layout contract" [
        test "default stack config is constructible" {
            let config = Defaults.stackConfig 800.0 600.0
            Expect.equal config.Bounds.Height 600.0 "height is retained"
            Expect.equal config.Padding Defaults.padding "default padding is retained"
            Expect.equal config.Spacing 0.0 "default spacing is retained"
        }

        test "horizontal and vertical stack measurement honors padding spacing and child count" {
            let config =
                { Defaults.stackConfig 300.0 120.0 with
                    Padding = { Left = 10.0; Top = 8.0; Right = 20.0; Bottom = 12.0 }
                    Spacing = 5.0 }

            let children = [ child "a"; child "b"; child "c" ]
            let horizontal = Layout.measureHorizontal config children
            let vertical = Layout.measureVertical config children

            Expect.equal horizontal.Length 3 "horizontal measurement returns one bound per child"
            Expect.floatClose Accuracy.medium horizontal[0].X 10.0 "left padding is applied"
            Expect.floatClose Accuracy.medium horizontal[1].X (10.0 + horizontal[0].Width + 5.0) "horizontal spacing is applied"
            Expect.floatClose Accuracy.medium horizontal[0].Height 100.0 "vertical padding is applied to horizontal stack"
            Expect.floatClose Accuracy.medium vertical[0].Y 8.0 "top padding is applied"
            Expect.floatClose Accuracy.medium vertical[1].Y (8.0 + vertical[0].Height + 5.0) "vertical spacing is applied"
            Expect.floatClose Accuracy.medium vertical[0].Width 270.0 "horizontal padding is applied to vertical stack"
        }

        test "dock config and child sizing records retain public layout props" {
            let dockConfig =
                { Defaults.dockConfig 640.0 480.0 with
                    Padding = { Left = 4.0; Top = 6.0; Right = 8.0; Bottom = 10.0 }
                    Spacing = 3.0 }

            let sized =
                { Content = Scene.rectangle (0.0, 0.0, 10.0, 10.0) Colors.white
                  Sizing =
                    { DesiredWidth = Some 120.0
                      DesiredHeight = Some 48.0
                      HorizontalAlignment = Center
                      VerticalAlignment = Middle }
                  Dock = Some Left }

            let scene = Layout.dock dockConfig [ sized ]
            Expect.equal sized.Sizing.DesiredWidth (Some 120.0) "desired width is retained"
            Expect.equal sized.Sizing.HorizontalAlignment Center "horizontal alignment is retained"
            Expect.equal sized.Dock (Some Left) "dock position is retained"
            Expect.contains (Scene.describe scene) RectangleElement "dock returns child scene content"
        }

        test "zero and negative stack bounds clamp measured child sizes to non-negative values" {
            let zero = Layout.measureHorizontal (Defaults.stackConfig 0.0 0.0) [ child "zero" ]
            let negative = Layout.measureVertical (Defaults.stackConfig -20.0 -10.0) [ child "negative" ]

            Expect.equal zero[0].Width 0.0 "zero width is stable"
            Expect.equal zero[0].Height 0.0 "zero height is stable"
            Expect.equal negative[0].Width 0.0 "negative width is clamped"
            Expect.equal negative[0].Height 0.0 "negative height is clamped"
        }

        test "layout resize keeps at least ten horizontal children non-overlapping at three sizes" {
            let children = [ for index in 0 .. 9 -> child $"item-{index}" ]
            let sizes = [ 320.0, 160.0; 640.0, 240.0; 960.0, 360.0 ]

            for width, height in sizes do
                let config =
                    { Defaults.stackConfig width height with
                        Padding = { Left = 8.0; Top = 8.0; Right = 8.0; Bottom = 8.0 }
                        Spacing = 4.0 }

                let bounds = Layout.measureHorizontal config children

                Expect.equal bounds.Length 10 $"all children are measured at {width}x{height}"
                Expect.all bounds (fun item -> item.Width >= 0.0 && item.Height >= 0.0) "bounds are non-negative"

                for leftIndex in 0 .. bounds.Length - 1 do
                    for rightIndex in leftIndex + 1 .. bounds.Length - 1 do
                        Expect.isFalse (overlaps bounds[leftIndex] bounds[rightIndex]) $"children {leftIndex} and {rightIndex} do not overlap at {width}x{height}"
        }

        test "layout resize keeps at least ten vertical children non-overlapping at three sizes" {
            let children = [ for index in 0 .. 9 -> child $"row-{index}" ]
            let sizes = [ 240.0, 320.0; 360.0, 640.0; 480.0, 960.0 ]

            for width, height in sizes do
                let config =
                    { Defaults.stackConfig width height with
                        Padding = { Left = 6.0; Top = 10.0; Right = 6.0; Bottom = 10.0 }
                        Spacing = 3.0 }

                let bounds = Layout.measureVertical config children

                Expect.equal bounds.Length 10 $"all children are measured at {width}x{height}"
                Expect.all bounds (fun item -> item.Width >= 0.0 && item.Height >= 0.0) "bounds are non-negative"

                for topIndex in 0 .. bounds.Length - 1 do
                    for bottomIndex in topIndex + 1 .. bounds.Length - 1 do
                        Expect.isFalse (overlaps bounds[topIndex] bounds[bottomIndex]) $"children {topIndex} and {bottomIndex} do not overlap at {width}x{height}"
        }

        test "graph validation reports duplicates missing endpoints self-loops and cycles" {
            let invalid =
                graph
                    Directed
                    [ node "a"; node "a"; node "b"; node "c" ]
                    [ edge "a" "b"
                      edge "b" "missing"
                      edge "missing" "c"
                      edge "c" "c"
                      edge "b" "a" ]

            let issues = GraphValidation.validate invalid

            Expect.contains issues (DuplicateNodeId "a") "duplicate node id is reported"
            Expect.exists issues (function MissingTarget(1, "missing") -> true | _ -> false) "missing target is reported"
            Expect.exists issues (function MissingSource(2, "missing") -> true | _ -> false) "missing source is reported"
            Expect.exists issues (function SelfLoop(3, "c") -> true | _ -> false) "self-loop is reported"
            Expect.exists issues (function CycleDetected _ -> true | _ -> false) "directed cycle is reported"
        }

        test "graph validation reports disconnected components and accepts dense edge sets" {
            let disconnected =
                graph
                    Undirected
                    [ node "a"; node "b"; node "c"; node "d"; node "e" ]
                    [ edge "a" "b"; edge "c" "d" ]

            let components = GraphValidation.disconnectedComponents disconnected
            Expect.equal (components |> List.length) 3 "two pairs and one isolated node produce three components"

            let denseNodes = [ for index in 0 .. 11 -> node $"n{index}" ]
            let denseEdges =
                [ for source in denseNodes do
                      for target in denseNodes do
                          if source.Id <> target.Id then
                              edge source.Id target.Id ]

            let dense = graph Undirected denseNodes denseEdges
            Expect.isEmpty (GraphValidation.validate dense) "dense undirected edge set is valid"
            Expect.isFalse (GraphValidation.hasCycle dense) "undirected dense graph cycle detection is intentionally not a DAG failure"
        }

        test "graph layout handles one hundred node DAG within two seconds" {
            let nodes = [ for index in 0 .. 99 -> node $"n{index}" ]
            let edges = [ for index in 0 .. 98 -> edge $"n{index}" $"n{index + 1}" ]
            let dag = graph Directed nodes edges
            let stopwatch = Stopwatch.StartNew()
            let result = Graph.layout dag
            stopwatch.Stop()

            match result with
            | Ok layout ->
                Expect.equal layout.Nodes.Length 100 "all DAG nodes are laid out"
                Expect.equal layout.Edges.Length 99 "all DAG edges are retained"
                Expect.isLessThan stopwatch.ElapsedMilliseconds 2000L "100-node DAG layout stays under two seconds"
            | Result.Error issues -> failtestf "expected valid DAG layout, got %A" issues
        }

        test "weighted undirected graph with fifty nodes has visible components and renders a scene" {
            let nodes = [ for index in 0 .. 49 -> node $"u{index}" ]
            let edges =
                [ for index in 0 .. 49 ->
                      { Source = $"u{index}"
                        Target = $"u{(index + 7) % 50}"
                        Weight = Some(float (index % 9 + 1))
                        Label = Some $"w{index % 9 + 1}" } ]

            let graph = graph Undirected nodes edges

            match Graph.layout graph, Graph.undirected graph with
            | Ok layout, Ok scene ->
                Expect.equal layout.Nodes.Length 50 "all weighted graph nodes are visible"
                Expect.isTrue (layout.Nodes |> List.forall (fun item -> item.Bounds.Width > 0.0 && item.Bounds.Height > 0.0)) "node bounds are visible"
                Expect.contains (Scene.describe scene) GroupElement "undirected graph renders as a grouped scene"
                Expect.contains (Scene.describe scene) TextElement "undirected graph includes visible node labels"
            | Result.Error issues, _ -> failtestf "expected valid weighted graph layout, got %A" issues
            | _, Result.Error issues -> failtestf "expected valid weighted graph scene, got %A" issues
        }

        test "graph scene builders render edges labels weights and hit-test nodes and edges" {
            let graph =
                graph
                    Directed
                    [ node "a"; node "b"; node "c" ]
                    [ { Source = "a"; Target = "b"; Weight = Some 2.5; Label = Some "a-b" }
                      { Source = "b"; Target = "c"; Weight = Some 4.0; Label = None } ]

            match Graph.layout graph, Graph.directed graph with
            | Ok layout, Ok scene ->
                let kinds = Scene.describe scene
                Expect.contains kinds LineElement "graph scene includes edge lines"
                Expect.contains kinds RectangleElement "graph scene includes node boxes"
                Expect.contains kinds TextElement "graph scene includes labels and weights"

                let firstNode = layout.Nodes.Head
                let nodeCenterX = firstNode.Bounds.X + firstNode.Bounds.Width / 2.0
                let nodeCenterY = firstNode.Bounds.Y + firstNode.Bounds.Height / 2.0
                Expect.equal (Graph.hitTest layout nodeCenterX nodeCenterY) (Some(Node firstNode.Node.Id)) "node hit-test returns node target"

                let source = layout.Nodes |> List.find (fun item -> item.Node.Id = "a")
                let target = layout.Nodes |> List.find (fun item -> item.Node.Id = "b")
                let edgeX = (source.Bounds.X + source.Bounds.Width / 2.0 + target.Bounds.X + target.Bounds.Width / 2.0) / 2.0
                let edgeY = (source.Bounds.Y + source.Bounds.Height / 2.0 + target.Bounds.Y + target.Bounds.Height / 2.0) / 2.0
                Expect.equal (Graph.hitTest layout edgeX edgeY) (Some(Edge 0)) "edge hit-test returns edge target"
            | Result.Error issues, _ -> failtestf "expected graph layout, got %A" issues
            | _, Result.Error issues -> failtestf "expected graph scene, got %A" issues
        }
    ]
