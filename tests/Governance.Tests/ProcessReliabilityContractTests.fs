module ProcessReliabilityContractTests

open System
open System.IO
open Expecto
open GovernanceTestSupport

let focusedGates =
    [ "PackageSurfaceCheck"
      "FsiTranscripts"
      "ControlsCatalogCheck"
      "ControlsInteractionCheck"
      "ControlsRenderingCheck"
      "DependencyReport"
      "TemplateCheck"
      "GeneratedProductCheck"
      "GeneratedGuidanceCheck"
      "TemplateDrift"
      "EvidenceGraph"
      "EvidenceAudit" ]

let buildGovernanceSource () =
    readBuildGovernanceSources ()

[<Tests>]
let processReliabilityContractTests =
    testList "Process reliability command contract" [
        test "build workflow carries process health bootstrap verdict and focused gate data through MVU boundary" {
            expectFileContains
                "build.fsx"
                [ "type ProcessHealthSnapshot"
                  "type ProcessHealthThreshold"
                  "type BootstrapValidation"
                  "type VerificationVerdict"
                  "type FocusedGateContract"
                  "ProcessHealthPath"
                  "BootstrapRunnerPath"
                  "VerificationVerdictsPath"
                  "FocusedGatesReportPath"
                  "CollectProcessHealth"
                  "ValidateRunnerBootstrap"
                  "WriteVerificationVerdict"
                  "WriteFocusedGateSummary"
                  "CheckFocusedGateAssumptions"
                  "VerificationVerdictWritten"
                  "FocusedGateCompleted" ]
        }

        test "broad aggregate targets emit preflight and bootstrap effects before broad work" {
            let content = read "build.fsx"

            [ "\"VerifyPreflight\", []"
              "\"CiPreflight\", []"
              "\"Verify\","
              "\"Ci\", [ \"CiPreflight\"; \"Verify\" ]"
              "StartTarget \"VerifyPreflight\""
              "StartTarget \"CiPreflight\""
              "CollectProcessHealth(\"Verify\""
              "CollectProcessHealth(\"Ci\""
              "ValidateRunnerBootstrap(\"Verify\""
              "ValidateRunnerBootstrap(\"Ci\"" ]
            |> List.iter (fun needle -> Expect.stringContains content needle $"build.fsx contains {needle}")
        }

        test "build helper contracts cover path process report scan package template and health behavior" {
            let source = buildGovernanceSource ()

            [ "let path"
              "let ensureParent"
              "let cleanDirectoryContents"
              "let appendLine"
              "let runProcessWithAllowedExitCodes"
              "let runProcess"
              "let runDotnetAction"
              "File.AppendAllText(outputPath"
              "exit-code={proc.ExitCode}"
              "timed out. See"
              "let parseScalar"
              "let parseInlineList"
              "WriteStructuredReport"
              "ScanV3GeneratedProducts"
              "GeneratedGuidanceScan"
              "let latestTemplatePackage"
              "let validateTemplatePackage"
              "expectedPackages"
              "ProcessHealthThreshold"
              "ProcessHealthSnapshot"
              "malformed threshold override" ]
            |> List.iter (fun needle -> Expect.stringContains source needle $"build helper source preserves {needle}")
        }

        test "verification verdict reports expose environment product degraded and success categories" {
            expectFileContains
                "build.fsx"
                [ "VerificationSuccess"
                  "VerificationProductFailure"
                  "VerificationEnvironmentFailure"
                  "VerificationDegraded"
                  "authoritative-product-evidence"
                  "recommended-rerun-environment"
                  "fresh shell, fresh container, or CI runner" ]
        }

        test "process health snapshot contract covers runner signals thresholds startup and elapsed timing" {
            expectFileContains
                "build.fsx"
                [ "AvailableMemoryMb"
                  "ProcessCount"
                  "ZombieProcessCount"
                  "ThreadLimit"
                  "ThreadHeadroom"
                  "FileDescriptorLimit"
                  "FileDescriptorHeadroom"
                  "DotnetStartup"
                  "FakeBootstrap"
                  "UnsupportedSignals"
                  "Thresholds"
                  "PreflightElapsedMs"
                  "process-health.available-memory"
                  "process-health.process-count"
                  "process-health.zombie-count"
                  "process-health.file-descriptor-headroom" ]
        }

        test "threshold overrides require value source and reason diagnostics" {
            expectFileContains
                "build.fsx"
                [ "FS_SKIA_PROCESS_MIN_AVAILABLE_MEMORY_MB"
                  "FS_SKIA_PROCESS_MAX_PROCESS_COUNT"
                  "FS_SKIA_PROCESS_MAX_ZOMBIE_COUNT"
                  "FS_SKIA_PROCESS_MIN_FD_HEADROOM"
                  "malformed threshold override"
                  "_REASON"
                  "OverrideSource"
                  "OverrideReason" ]
        }

        test "interpreter diagnostics classify bootstrap startup process and warning failures as environment evidence" {
            expectFileContains
                "build.fsx"
                [ "dotnet startup failed"
                  "CoreCLR/VSTest/socket/thread startup failures"
                  "bootstrap validation failed with environment-failure"
                  "process-health preflight failed with environment-failure"
                  "runner-warning-classification"
                  "warning-noise unless target exits nonzero"
                  "FAKE runner did not start after tool restore"
                  "dotnet fake --version"
                  "fresh shell, fresh container, or CI runner" ]
        }

        test "focused gates are direct entry points and avoid broad aggregate dependencies" {
            let content = read "build.fsx"

            focusedGates
            |> List.iter (fun gate ->
                expectFakeTarget gate
                let dependencyPrefix = $"\"{gate}\", ["
                let dependencyStart = content.IndexOf(dependencyPrefix, StringComparison.Ordinal)
                Expect.isGreaterThanOrEqual dependencyStart 0 $"{gate} has dependency entry"
                let dependencyEnd = content.IndexOf("]", dependencyStart, StringComparison.Ordinal)
                let dependencyText = content.Substring(dependencyStart, dependencyEnd - dependencyStart + 1)
                Expect.isFalse (dependencyText.Contains("\"Verify\"")) $"{gate} does not depend on Verify"
                Expect.isFalse (dependencyText.Contains("\"Ci\"")) $"{gate} does not depend on Ci")
        }

        test "focused gate diagnostics name stale build and restore remediation" {
            expectFileContains
                "build.fsx"
                [ "stale-build-restore-assumption"
                  "dotnet restore"
                  "dotnet build"
                  "affected-gate"
                  "remediation-command" ]
        }

        test "smoke tests run through direct Expecto executable instead of VSTest adapter" {
            let content = buildGovernanceSource ()

            Expect.stringContains
                content
                "EndsWith(\"tests/Smoke.Tests/Smoke.Tests.fsproj\""
                "FAKE detects the Smoke.Tests project explicitly"

            Expect.stringContains
                content
                "processEffect $\"dotnet run {project}\""
                "FAKE labels Smoke.Tests as direct executable smoke"

            Expect.stringContains
                content
                "run --project {project} --no-restore"
                "main Test target runs Smoke.Tests through dotnet run"

            Expect.stringContains
                content
                "run --project {quote project} --no-restore"
                "shared dotnet action helper also routes Smoke.Tests through dotnet run"

            let forbidden =
                [ "test tests/Smoke.Tests/Smoke.Tests.fsproj" ]

            forbidden
            |> List.iter (fun needle ->
                Expect.isFalse (content.Contains(needle, StringComparison.Ordinal)) $"build.fsx must not contain {needle}")
        }

        test "scanner fixtures can create XML metadata generated product guidance stale and readiness inputs" {
            use fixture = new TempFixtureDirectory("process-reliability")
            let project = writeProjectFixture fixture.Root "src/Product/Product.fsproj" [ "FS.Skia.UI.Controls" ] [ "../Scene/Scene.fsproj" ]
            let capability = writeCapabilityMetadataFixture fixture.Root "- id: controls\n  displayName: Controls\n"
            let generated = writeGeneratedProductFixture fixture.Root "sample-pack" [ "samples/README.md", "# Samples\n" ]
            let guidance = writeGeneratedGuidanceFixture fixture.Root "docs/generated.md" [ "RichText.create" ] [ "ControlsElmish.program" ]
            let stale = writeStaleReferenceFixture fixture.Root "docs/reports/architecture.md" "FS.Skia.UI.Charts" "historical replacement"
            let readiness = writeReadinessReportFixture fixture.Root "readiness/verification-verdicts.md" "environment-failure" [ "process-health.md" ]

            [ project; capability; guidance; stale; readiness ]
            |> List.iter (fun path -> Expect.isTrue (System.IO.File.Exists path) $"{path} exists")

            Expect.isNonEmpty generated "generated product fixture files were written"
        }

        test "workflow self-check exercises process reliability transition and effect assertions" {
            let exitCode, stdout, stderr = runFakeTarget "BuildWorkflowCheck"
            let output = stdout + stderr
            Expect.equal exitCode 0 output
            Expect.stringContains output "process reliability self-check" "self-check covered process reliability assertions"
        }
    ]
