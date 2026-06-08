module ControlsPreview.RendererFidelityTests

// Feature 080 (T008/T009, US1, FR-002/003/004/005/011) — the faithful renderer produces
// control-specific geometry, lowered to existing Scene primitives, WITHIN the preview canvas
// bounds. These are pure `Control.render Theme.light` + `Scene.describe` assertions (no native
// host needed); they live in the harness alongside the rest of the 080 fidelity surface and run
// under `-- --sequenced`. Pre-080 every rich control rendered one filled rect + one clipped label
// (no Path/Arc/Points/per-row geometry) and the chart node painted off-canvas at y≈180–400.

open Expecto
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed

let private canvasW, canvasH = 320.0, 160.0

let private render (w: Widget<unit>) =
    (Control.render Theme.light (Widget.toControl w)).Scene

let private kinds (w: Widget<unit>) = render w |> Scene.describe
let private count k ks = ks |> List.filter ((=) k) |> List.length

/// Collect every geometry coordinate the scene references, so we can prove bounds-safety.
let rec private coords (scene: Scene) : (float * float) list =
    scene.Nodes
    |> List.collect (fun node ->
        match node with
        | Group scenes -> scenes |> List.collect coords
        | ClipNode(_, s) -> coords s
        | ColorSpaceNode(_, s) -> coords s
        | PerspectiveNode(_, s) -> coords s
        | Rectangle((x, y, w, h), _) -> [ x, y; x + w, y + h ]
        | PaintedRectangle(r, _) -> [ r.X, r.Y; r.X + r.Width, r.Y + r.Height ]
        | Circle(c, rad, _) -> [ c.X - rad, c.Y - rad; c.X + rad, c.Y + rad ]
        | FilledEllipse(r, _) | Ellipse(r, _) -> [ r.X, r.Y; r.X + r.Width, r.Y + r.Height ]
        | Line(a, b, _) -> [ a.X, a.Y; b.X, b.Y ]
        | Arc(r, _, _, _) -> [ r.X, r.Y; r.X + r.Width, r.Y + r.Height ]
        | Points(ps, _) -> ps |> List.map (fun p -> p.X, p.Y)
        | TextRun t -> [ t.Position.X, t.Position.Y ]
        | SceneNode.Text((x, y), _, _) -> [ x, y ]
        | SceneNode.Image((x, y, w, h), _) -> [ x, y; x + w, y + h ]
        | Path(spec, _) ->
            spec.Commands
            |> List.collect (function
                | MoveTo p | LineTo p -> [ p.X, p.Y ]
                | QuadTo(c, p) -> [ c.X, c.Y; p.X, p.Y ]
                | CubicTo(c1, c2, p) -> [ c1.X, c1.Y; c2.X, c2.Y; p.X, p.Y ]
                | ArcTo(b, _, _) -> [ b.X, b.Y; b.X + b.Width, b.Y + b.Height ]
                | Close -> [])
        | _ -> [])

let private withinCanvas (w: Widget<unit>) =
    coords (render w)
    |> List.forall (fun (x, y) -> x >= -1.0 && x <= canvasW + 1.0 && y >= -1.0 && y <= canvasH + 1.0)

let private series: ChartSeries list =
    [ { Name = "S"
        Points =
          [ { X = 0.0; Y = 3.0; Label = None }
            { X = 1.0; Y = 7.0; Label = None }
            { X = 2.0; Y = 5.0; Label = None }
            { X = 3.0; Y = 9.0; Label = None } ] } ]

let private items4 = [ "Alpha"; "Beta"; "Gamma"; "Delta" ]

[<Tests>]
let chartGeometryTests =
    testList "Feature 080 chart-family geometry (T008, US1)" [
        test "line chart renders a polyline PathElement within bounds" {
            let w = LineChart.view { LineChart.defaults with Series = series }
            Expect.isTrue (kinds w |> List.contains PathElement) "line chart contributes a Path polyline"
            Expect.isTrue (withinCanvas w) "line chart geometry stays within the 320x160 canvas (no off-canvas chartTop)"
            Expect.isFalse (kinds w |> List.contains ChartElement) "no opaque Chart node is emitted (T012)"
        }
        test "bar chart renders one RectangleElement per point" {
            let w = BarChart.view { BarChart.defaults with Series = series }
            Expect.isGreaterThanOrEqual (kinds w |> count RectangleElement) 4 "a bar per series point"
            Expect.isTrue (withinCanvas w) "bar geometry within canvas"
        }
        test "pie chart renders one ArcElement per slice" {
            let values = [ { X = 0.0; Y = 30.0; Label = Some "A" }; { X = 1.0; Y = 50.0; Label = Some "B" }; { X = 2.0; Y = 20.0; Label = Some "C" } ]
            let w = PieChart.view { PieChart.defaults with Values = values }
            Expect.isGreaterThanOrEqual (kinds w |> count ArcElement) 3 "an arc per slice"
            Expect.isTrue (withinCanvas w) "pie geometry within canvas"
        }
        test "scatter plot renders one CircleElement per point" {
            let w = ScatterPlot.view { ScatterPlot.defaults with Series = series }
            Expect.isGreaterThanOrEqual (kinds w |> count CircleElement) 4 "a mark per point"
            Expect.isTrue (withinCanvas w) "scatter geometry within canvas"
        }
        test "graph view renders node circles and connecting edges" {
            let w = GraphView.view { GraphView.defaults with Nodes = [ "A"; "B"; "C"; "D" ] }
            Expect.isGreaterThanOrEqual (kinds w |> count CircleElement) 4 "a circle per node"
            Expect.isTrue (kinds w |> List.contains LineElement) "edges between nodes"
            Expect.isTrue (withinCanvas w) "graph geometry within canvas"
        }
    ]

[<Tests>]
let collectionAndValueGeometryTests =
    testList "Feature 080 collection / value-control geometry (T009, US1)" [
        test "list box renders >=3 distinct item rows with text" {
            let p = { (ListBox.defaults "list-box") with Items = items4 }
            let m, _ = ListBox.init p
            let m2, _ = ListBox.update (SelectKey "Beta") m
            let w = ListBox.view p m2
            Expect.isGreaterThanOrEqual (kinds w |> count RectangleElement) 3 ">=3 item rows"
            Expect.isTrue (kinds w |> List.contains TextRunElement) "rows carry item text"
            Expect.isTrue (withinCanvas w) "list rows within canvas"
        }
        test "slider renders a track plus a thumb" {
            let w = Slider.view { Slider.defaults with Value = 0.5 }
            Expect.isGreaterThanOrEqual (kinds w |> count RectangleElement) 2 "track + filled track"
            Expect.isTrue (kinds w |> List.contains CircleElement) "a draggable thumb"
            Expect.isTrue (withinCanvas w) "slider within canvas"
        }
        test "progress bar renders a track and a partial fill" {
            let w = ProgressBar.view { ProgressBar.defaults with Value = 0.6 }
            Expect.isGreaterThanOrEqual (kinds w |> count RectangleElement) 2 "track + fill rectangles"
            Expect.isTrue (withinCanvas w) "progress within canvas"
        }
        test "radio group renders a circle per option, one marked" {
            let w = RadioGroup.view { RadioGroup.defaults with Items = [ "Low"; "Medium"; "High" ]; SelectedKey = Some "Medium" }
            Expect.isGreaterThanOrEqual (kinds w |> count CircleElement) 3 "a radio circle per option"
            Expect.isTrue (withinCanvas w) "radio group within canvas"
        }
        test "tabs render a tab strip rectangle per tab" {
            let w = Tabs.view { Tabs.defaults with Items = [ "Home"; "Profile"; "Settings" ]; SelectedKey = Some "Profile" }
            Expect.isGreaterThanOrEqual (kinds w |> count RectangleElement) 3 "a tab per item"
            Expect.isTrue (withinCanvas w) "tabs within canvas"
        }
        test "check box renders a frame plus a tick when checked" {
            let w = CheckBox.view { CheckBox.defaults with Text = "Enable"; Checked = true }
            Expect.isTrue (kinds w |> List.contains RectangleElement) "a check frame"
            Expect.isTrue (kinds w |> List.contains LineElement) "a tick mark when checked"
            Expect.isTrue (withinCanvas w) "check box within canvas"
        }
        test "switch renders a track plus a thumb in the on position" {
            let w = Switch.view { Switch.defaults with Checked = true }
            Expect.isTrue (kinds w |> List.contains RectangleElement) "a switch track"
            Expect.isTrue (kinds w |> List.contains CircleElement) "a switch thumb"
            Expect.isTrue (withinCanvas w) "switch within canvas"
        }
        test "image renders a framed placeholder, not a path-less label box" {
            let w = Image.view { Image.defaults with Value = "logo.png" }
            Expect.isTrue (kinds w |> List.contains RectangleElement) "a placeholder frame"
            Expect.isTrue (kinds w |> List.contains LineElement) "diagonal placeholder lines"
            Expect.isTrue (withinCanvas w) "image within canvas"
        }
        test "icon renders a font-independent Path glyph (no .notdef box)" {
            let w = Icon.view { Icon.defaults with Text = "home" }
            Expect.isTrue (kinds w |> List.contains PathElement) "a vector glyph that never falls back to .notdef"
            Expect.isTrue (withinCanvas w) "icon within canvas"
        }
    ]
