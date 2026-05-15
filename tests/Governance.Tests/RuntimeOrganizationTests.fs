module RuntimeOrganizationTests

open System
open System.IO
open Expecto
open GovernanceTestSupport

let helperFiles =
    [ "src/Lib/VulkanResources.fsi"
      "src/Lib/VulkanResources.fs"
      "src/Lib/VulkanStartup.fsi"
      "src/Lib/VulkanStartup.fs" ]

[<Tests>]
let runtimeOrganizationTests =
    testList "Runtime organization guardrails" [
        test "accepted internal helper files have paired signatures and compile before Library.fs" {
            helperFiles
            |> List.iter (fun relative -> Expect.isTrue (fileExists relative) $"{relative} exists")

            let project = read "src/Lib/Lib.fsproj"
            let resourcesIndex = project.IndexOf("VulkanResources.fsi", StringComparison.Ordinal)
            let startupIndex = project.IndexOf("VulkanStartup.fsi", StringComparison.Ordinal)
            let libraryIndex = project.IndexOf("Library.fsi", StringComparison.Ordinal)

            Expect.isGreaterThanOrEqual resourcesIndex 0 "VulkanResources signature appears in project"
            Expect.isGreaterThanOrEqual startupIndex 0 "VulkanStartup signature appears in project"
            Expect.isLessThan resourcesIndex libraryIndex "resource helper compiles before public facade"
            Expect.isLessThan startupIndex libraryIndex "startup helper compiles before public facade"
        }

        test "runtime implementation files do not use top-level visibility modifiers in fs files" {
            [ "src/Lib/VulkanResources.fs"
              "src/Lib/VulkanStartup.fs"
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
