module SceneCapabilityTests

open System.IO
open Expecto
open FS.Skia.UI.Scene

[<Tests>]
let tests =
    testList "Scene public contract" [
        test "rectangle descriptions are stable" {
            let node =
                Scene.rectangle
                    (0.0, 0.0, 10.0, 20.0)
                    (Colors.rgb 1uy 2uy 3uy)

            Expect.contains (Scene.describe node) RectangleElement "scene description includes rectangle kind"
        }

        test "scene evidence helper returns deterministic hash without a viewer window" {
            let scene = Scene.rectangle (0.0, 0.0, 10.0, 20.0) (Colors.rgb 1uy 2uy 3uy)

            let evidence =
                SceneEvidence.renderHash { Width = 64; Height = 64 } scene

            match evidence with
            | Result.Ok value ->
                Expect.equal value.Format Hash "hash evidence is returned"
                Expect.equal value.OutputSize { Width = 64; Height = 64 } "output size is recorded"
                Expect.equal value.RendererMode "deterministic-scene" "renderer mode is scene-level"
                Expect.isNonEmpty value.Value "hash value is populated"
            | Result.Error failure -> failtestf "scene evidence should not fail: %s" failure.Message
        }

        test "scene evidence writes metadata output with stable size renderer and value" {
            let scene =
                Scene.group [
                    Scene.rectangle (0.0, 0.0, 10.0, 20.0) (Colors.rgb 1uy 2uy 3uy)
                    Scene.text (4.0, 18.0) "Generated" Colors.white
                ]

            let path = Path.Combine(Path.GetTempPath(), "fs-skia-scene-evidence-metadata.txt")
            if File.Exists path then
                File.Delete path

            let result =
                SceneEvidence.render
                    { Scene = scene
                      OutputSize = { Width = 128; Height = 96 }
                      Format = Metadata
                      RendererMode = "deterministic-scene"
                      EvidencePath = Some path }

            match result with
            | Result.Ok evidence ->
                Expect.equal evidence.Format Metadata "metadata evidence is returned"
                Expect.equal evidence.OutputSize { Width = 128; Height = 96 } "metadata records output size"
                Expect.equal evidence.RendererMode "deterministic-scene" "metadata uses deterministic scene renderer"
                Expect.stringContains evidence.Value "size=128x96" "metadata names output size"
                Expect.stringContains (File.ReadAllText path) evidence.Value "metadata evidence is written to disk"
            | Result.Error failure -> failtestf "scene evidence should write metadata: %A" failure
        }

        test "scene evidence PNG bytes are deterministic and do not require viewer startup" {
            let scene = Scene.text (12.0, 24.0) "Generated app scene" Colors.white

            let first = SceneEvidence.renderPng { Width = 80; Height = 40 } scene
            let second = SceneEvidence.renderPng { Width = 80; Height = 40 } scene

            match first, second with
            | Result.Ok firstBytes, Result.Ok secondBytes ->
                Expect.sequenceEqual firstBytes secondBytes "PNG evidence bytes are stable for the same scene"
                Expect.isGreaterThan firstBytes.Length 0 "PNG evidence produces bytes"
            | other -> failtestf "expected deterministic PNG evidence, got %A" other
        }

        test "scene evidence reports unsupported renderer capabilities explicitly" {
            let result =
                SceneEvidence.render
                    { Scene = Scene.empty
                      OutputSize = { Width = 64; Height = 64 }
                      Format = Hash
                      RendererMode = "live-window"
                      EvidencePath = None }

            match result with
            | Result.Error failure ->
                Expect.equal failure.Classification UnsupportedEnvironment "unsupported renderer mode is environment/capability failure"
                Expect.equal failure.BlockedStage "renderer" "blocked renderer capability is named"
                Expect.stringContains failure.Message "live-window" "message names unsupported renderer mode"
            | Result.Ok evidence -> failtestf "expected unsupported renderer failure, got %A" evidence
        }
    ]
