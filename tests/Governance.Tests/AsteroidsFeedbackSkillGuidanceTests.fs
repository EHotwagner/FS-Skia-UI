module AsteroidsFeedbackSkillGuidanceTests

open System
open System.IO
open System.IO.Compression
open System.Xml.Linq
open Expecto
open GovernanceTestSupport

let featureDir = "specs/034-asteroids-feedback-skills"

let tasksGuidancePaths =
    [ ".specify/templates/tasks-template.md"
      ".specify/presets/fsharp-opinionated/templates/tasks-template.md"
      ".specify/presets/fsharp-opinionated/commands/speckit.tasks.md"
      ".agents/skills/speckit-tasks/SKILL.md" ]

let generatedProductGuidancePaths =
    [ "template/base/README.md"
      "template/base/docs/product.md"
      "template/base/tests/Product.Tests/Tests.fs" ]

let layoutEvidenceGuidancePaths =
    [ ".agents/skills/fs-skia-layout-evidence/SKILL.md"
      "template/base/docs/product.md"
      "template/base/tests/Product.Tests/Tests.fs" ]

let packableProjectPaths =
    [ "src/Lib/Lib.fsproj", "FS.Skia.UI", [ "src/Lib/Library.fsi" ]
      "src/Scene/Scene.fsproj", "FS.Skia.UI.Scene", [ "src/Scene/Scene.fsi" ]
      "src/SkiaViewer/SkiaViewer.fsproj", "FS.Skia.UI.SkiaViewer", [ "src/SkiaViewer/SkiaViewer.fsi"; "src/SkiaViewer/Host/Diagnostics.fsi"; "src/SkiaViewer/Host/Vulkan.fsi"; "src/SkiaViewer/Host/Viewer.fsi" ]
      "src/Elmish/Elmish.fsproj", "FS.Skia.UI.Elmish", [ "src/Elmish/Elmish.fsi" ]
      "src/KeyboardInput/KeyboardInput.fsproj", "FS.Skia.UI.KeyboardInput", [ "src/KeyboardInput/KeyboardInput.fsi" ]
      "src/Input/Input.fsproj", "FS.Skia.UI.Input", [ "src/Input/KeyboardInput.fsi" ]
      "src/Layout/Layout.fsproj", "FS.Skia.UI.Layout", [ "src/Layout/Layout.fsi"; "src/Layout/Types.fsi"; "src/Layout/Graph.fsi"; "src/Layout/GraphValidation.fsi" ]
      "src/Controls/Controls.fsproj", "FS.Skia.UI.Controls", [ "src/Controls/Accessibility.fsi"; "src/Controls/Attributes.fsi"; "src/Controls/Catalog.fsi"; "src/Controls/Charts.fsi"; "src/Controls/Collections.fsi"; "src/Controls/Control.fsi"; "src/Controls/ControlRuntime.fsi"; "src/Controls/CustomControl.fsi"; "src/Controls/DataGrid.fsi"; "src/Controls/Diagnostics.fsi"; "src/Controls/RichText.fsi"; "src/Controls/TextInput.fsi"; "src/Controls/Theme.fsi"; "src/Controls/Types.fsi" ]
      "src/Controls.Elmish/Controls.Elmish.fsproj", "FS.Skia.UI.Controls.Elmish", [ "src/Controls.Elmish/ControlsElmish.fsi" ]
      "src/Testing/Testing.fsproj", "FS.Skia.UI.Testing", [ "src/Testing/Testing.fsi" ] ]

let declarationLines relativePath =
    read relativePath
    |> fun text -> text.Replace("\r\n", "\n").Split('\n')
    |> Array.mapi (fun index line -> index, line)
    |> Array.filter (fun (_, line) ->
        let trimmed = line.TrimStart()
        trimmed.StartsWith("type ", StringComparison.Ordinal)
        || trimmed.StartsWith("module ", StringComparison.Ordinal)
        || trimmed.StartsWith("val ", StringComparison.Ordinal))

let hasDocCommentBefore (lines: string array) index =
    if index = 0 then
        false
    else
        let rec previousMeaningful i =
            if i < 0 then None
            elif String.IsNullOrWhiteSpace lines[i] then previousMeaningful (i - 1)
            else Some(lines[i].TrimStart())

        match previousMeaningful (index - 1) with
        | Some line -> line.StartsWith("///", StringComparison.Ordinal)
        | None -> false

[<Tests>]
let asteroidsFeedbackSkillGuidanceTests =
    testList "Asteroids feedback skill guidance" [
        test "generated task guidance assigns visual demo skills and preserves ordered mirrors" {
            tasksGuidancePaths
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "scene rendering -> fs-skia-scene"
                      "screenshot capture -> fs-skia-skiaviewer"
                      "layout readability -> fs-skia-layout-evidence"
                      "persistent viewer launch -> fs-skia-skiaviewer"
                      "deterministic evidence mode -> fs-skia-layout-evidence"
                      "generated-package validation -> fs-skia-template-update"
                      "graph validation -> speckit-evidence-graph"
                      "audit validation -> speckit-evidence-audit"
                      "implementation-before-evidence"
                      "graph-before-audit"
                      "debug-before-broad-rerun"
                      "[skillist: speckit-tasks, fs-skia-layout-evidence]" ])
        }

        // Feature 038 (FR-001, US1): the `speckit-debug-loop` advertised id was
        // dangling — no skill declares that `name:` — so it is removed from the
        // speckit-tasks guidance. Assert it no longer advertises the dangling id.
        test "speckit-tasks guidance does not advertise the dangling speckit-debug-loop id" {
            [ ".agents/skills/speckit-tasks/SKILL.md"
              ".claude/skills/speckit-tasks/SKILL.md" ]
            |> List.iter (fun path ->
                Expect.isFalse
                    ((read path).Contains "speckit-debug-loop")
                    $"{path} must not advertise the dangling `speckit-debug-loop` id (FR-001)")
        }

        test "generated guidance enumerates readiness scaffolds and field cues before audit" {
            tasksGuidancePaths
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "readiness/visual-evidence-honesty.md"
                      "readiness/window-visibility.md"
                      "readiness/governance-risk-levels.md"
                      "readiness/aggregate-hang-diagnostics.md"
                      "readiness/runtime-limitations.md"
                      "readiness/generated-guidance-validation.md"
                      "readiness/real-image-evidence.md"
                      "authoritative command"
                      "artifact path"
                      "failure class"
                      "next action" ])
        }

        test "visual evidence guidance rejects metadata fallback and layout-only proof" {
            layoutEvidenceGuidancePaths
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "decodable image"
                      "image dimensions"
                      "non-trivial content"
                      "renderer mode"
                      "fallback classification"
                      "unsupported reason"
                      "metadata-only reports do not satisfy visual proof"
                      "1x1 fallback images do not satisfy visual proof"
                      "layout-only bounds claims do not satisfy visual proof" ])
        }

        test "feedback classification guidance names owners and host warning outcomes" {
            layoutEvidenceGuidancePaths
            |> List.iter (fun path ->
                expectFileContains
                    path
                    [ "framework runtime"
                      "generated template workflow"
                      "documentation discoverability"
                      "consumer authoring"
                      "persistent-window blocking"
                      "display/session availability"
                      "auto-close smoke"
                      "benign warning"
                      "blocking warning"
                      "deferred warning"
                      "name-collision guidance" ])
        }

        test "feature readiness evidence records scan commands and owner dispositions" {
            [ "skill-assignment-guidance.md"
              "readiness-scaffold-coverage.md"
              "visual-evidence-honesty.md"
              "feedback-classification.md"
              "generated-guidance-validation.md"
              "xml-documentation-validation.md" ]
            |> List.iter (fun file ->
                expectFileContains
                    $"{featureDir}/readiness/{file}"
                    [ "command"
                      "scanned files"
                      "observed"
                      "missing"
                      "failure class"
                      "next action" ])
        }

        test "public fsi declarations have XML documentation comments" {
            packableProjectPaths
            |> List.collect (fun (_, _, fsiFiles) -> fsiFiles)
            |> List.iter (fun fsi ->
                let text = read fsi
                let lines = text.Replace("\r\n", "\n").Split('\n')

                declarationLines fsi
                |> Array.iter (fun (index, declaration) ->
                    Expect.isTrue (hasDocCommentBefore lines index) $"{fsi}:{index + 1} has XML documentation before {declaration.Trim()}"))
        }

        test "documentation validation covers generated XML and packed nupkg entries" {
            let evidence = read $"{featureDir}/readiness/xml-documentation-validation.md"

            packableProjectPaths
            |> List.iter (fun (project, packageId, fsiFiles) ->
                Expect.stringContains evidence project $"evidence names {project}"
                Expect.stringContains evidence packageId $"evidence names {packageId}"

                fsiFiles
                |> List.iter (fun fsi -> Expect.stringContains evidence fsi $"evidence names {fsi}")

                Expect.stringContains evidence $"{packageId}.xml" $"evidence names packed XML entry for {packageId}")
        }
    ]
