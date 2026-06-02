module RuntimeOrganizationTests

open System
open System.IO
open Expecto
open GovernanceTestSupport

// V3 Stage 1 (050): the Vulkan/Skia host moved out of the FS.Skia.UI monolith into the
// FS.Skia.UI.SkiaViewer package under Host/. The resource/startup helpers travel with it and keep
// their paired signatures, compiling before the package facade (SkiaViewer.fs).
let helperFiles =
    [ "src/SkiaViewer/Host/Diagnostics.fsi"
      "src/SkiaViewer/Host/Diagnostics.fs"
      "src/SkiaViewer/Host/Vulkan.fsi"
      "src/SkiaViewer/Host/Vulkan.fs"
      "src/SkiaViewer/Host/Viewer.fsi"
      "src/SkiaViewer/Host/Viewer.fs" ]

[<Tests>]
let runtimeOrganizationTests =
    testList "Runtime organization guardrails" [
        test "accepted internal helper files have paired signatures and compile before the package facade" {
            helperFiles
            |> List.iter (fun relative -> Expect.isTrue (fileExists relative) $"{relative} exists")

            let project = read "src/SkiaViewer/SkiaViewer.fsproj"
            let resourcesIndex = project.IndexOf("Host/Vulkan.fsi", StringComparison.Ordinal)
            let diagnosticsIndex = project.IndexOf("Host/Diagnostics.fsi", StringComparison.Ordinal)
            let facadeIndex = project.IndexOf("SkiaViewer.fsi", StringComparison.Ordinal)

            Expect.isGreaterThanOrEqual resourcesIndex 0 "Vulkan host signature appears in project"
            Expect.isGreaterThanOrEqual diagnosticsIndex 0 "host diagnostics signature appears in project"
            Expect.isLessThan resourcesIndex facadeIndex "Vulkan host compiles before package facade"
            Expect.isLessThan diagnosticsIndex facadeIndex "host diagnostics compiles before package facade"
        }

        test "runtime implementation files do not use top-level visibility modifiers in fs files" {
            [ "src/SkiaViewer/Host/Diagnostics.fs"
              "src/SkiaViewer/Host/Vulkan.fs"
              "src/SkiaViewer/Host/Viewer.fs"
              "src/Lib/Library.fs" ]
            |> List.iter (fun relative ->
                let lines = read relative |> fun content -> content.Replace("\r\n", "\n").Split('\n')

                lines
                |> Array.iteri (fun index line ->
                    let trimmed = line.TrimStart()
                    let isTopLevelVisibility =
                        (trimmed.StartsWith("private ", StringComparison.Ordinal)
                         || trimmed.StartsWith("internal ", StringComparison.Ordinal)
                         || trimmed.StartsWith("public ", StringComparison.Ordinal))
                        && (line.Length = trimmed.Length)

                    Expect.isFalse isTopLevelVisibility $"{relative}:{index + 1} has no top-level visibility modifier"))
        }

        test "runtime responsibility map records helper split and named-section fallback" {
            let readinessMap = "specs/008-targeted-refactor-governance/readiness/runtime-responsibility-map.md"

            if fileExists readinessMap then
                expectFileContains
                    readinessMap
                    [ "VulkanResources.fsi"
                      "VulkanStartup.fsi"
                      "Named-section fallback"
                      "Frame flow and screenshots"
                      "Viewer hosting" ]
            else
                Expect.isFalse (directoryExists "specs/008-targeted-refactor-governance") "feature readiness map is source-only and absent from generated projects"
        }
    ]
