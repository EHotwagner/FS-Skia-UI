module ControlsRenderingTests

open Expecto
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls

[<Tests>]
let renderingTests =
    testList "Controls rendering and collections" [
        test "large data visible range stays bounded for ten thousand items" {
            let model, effects = Collections.init "orders" 10_000 24.0 240.0
            Expect.equal model.VisibleRange.Count 11 "visible range includes only the viewport plus one buffer row"
            Expect.equal effects [ VisibleRangeChanged model.VisibleRange ] "initial range effect is emitted"

            let scrolled, _ = Collections.update (ScrollTo(24.0 * 250.0)) model
            Expect.equal scrolled.VisibleRange.FirstIndex 250 "scroll offset maps to first visible row"
            Expect.isLessThan scrolled.VisibleRange.Count 30 "visible range remains bounded"
        }

        test "render output covers viewport sizes and scale factors without diagnostics" {
            let screen =
                Stack.create [
                    Stack.children [
                        TextBlock.create [ TextBlock.text "Catalog" ]
                        ProgressBar.create [ ProgressBar.value 0.4 ]
                        GraphView.create [ GraphView.nodes [ "a"; "b"; "c" ] ]
                    ]
                ]

            for width, height in [ 320, 240; 640, 480; 1024, 768 ] do
                for scale in [ 1.0; 2.0 ] do
                    let theme = Theme.light |> Theme.withDensity scale
                    let rendered = Control.render theme screen
                    let evidence = Scene.renderReadbackEvidence { Width = width; Height = height } rendered.Scene
                    Expect.isEmpty rendered.Diagnostics $"no rendering diagnostics at {width}x{height}@{scale}"
                    Expect.isNonEmpty evidence.DeterministicHash "render evidence has deterministic hash"
        }

        test "rich text reports unsupported Skia effect diagnostics during measurement" {
            let block =
                { RichText.block [ RichText.run "Hello" (RichText.defaultStyle Theme.light) ] with
                    MaxWidth = Some 32.0
                    Clip = true
                    Effects = [ "drop-shadow" ] }

            let measurement = RichText.measure block
            let rendered = Control.render Theme.light (RichText.create block [])
            let evidence = Scene.renderReadbackEvidence { Width = 160; Height = 90 } rendered.Scene

            Expect.isLessThanOrEqual measurement.Width 32.0 "measurement respects max width"
            Expect.exists measurement.Diagnostics (fun item -> item.Code = UnsupportedEnvironment && item.Message.Contains "drop-shadow") "unsupported effect is diagnosed"
            Expect.isNonEmpty evidence.DeterministicHash "rich text render produces readback evidence"
        }
    ]
