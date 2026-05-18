module TestingCapabilityTests

open System
open Expecto
open FS.Skia.UI.Testing

[<Tests>]
let tests =
    testList "Testing helper contract" [
        test "summaries include profile and packages" {
            let summary =
                GeneratedProductAssertions.summarize
                    { Profile = "app"
                      RequiredFiles = [ "src/Product/Product.fsproj" ]
                      ForbiddenPrefixes = [ "samples/" ]
                      PackageReferences = [ { PackageId = "FS.Skia.UI.Scene"; Required = true } ] }

            Expect.stringContains summary "app" "profile is included"
            Expect.stringContains summary "FS.Skia.UI.Scene" "package is included"
        }

        test "local consumer package reports include feed snippets restore command and drift" {
            let expected =
                [ { PackageId = "FS.Skia.UI.Scene"; Version = "1.2.3"; FeedPath = "/tmp/feed" }
                  { PackageId = "FS.Skia.UI.SkiaViewer"; Version = "1.2.3"; FeedPath = "/tmp/feed" } ]

            let actual =
                [ { PackageId = "FS.Skia.UI.Scene"; Version = "1.2.2"; FeedPath = "/tmp/feed" } ]

            let drift = LocalConsumerPackages.classifyDrift expected actual
            Expect.hasLength drift 2 "stale and missing packages are both reported"

            let report = LocalConsumerPackages.report "/tmp/feed" expected
            Expect.equal report.FeedPath "/tmp/feed" "feed path is recorded"
            Expect.equal (report.Packages |> List.map _.PackageId) [ "FS.Skia.UI.Scene"; "FS.Skia.UI.SkiaViewer" ] "generated consumer package set is recorded"
            Expect.stringContains report.ConsumerConfigSnippet "FS.Skia.UI.Scene" "package snippet names identities"
            Expect.stringContains report.ConsumerConfigSnippet "1.2.3" "package snippet names versions"
            Expect.isSome report.NuGetConfigSnippet "optional NuGet.config snippet is provided"
            Expect.stringContains (report.NuGetConfigSnippet |> Option.defaultValue "") "/tmp/feed" "NuGet.config snippet names feed path"
            Expect.stringContains report.RestoreCommand "dotnet restore" "restore command is included"
            Expect.exists drift (fun item -> item.PackageId = "FS.Skia.UI.Scene" && item.ActualVersion = Some "1.2.2") "stale package drift is reported before generated build failures"
            Expect.exists drift (fun item -> item.PackageId = "FS.Skia.UI.SkiaViewer" && item.ActualVersion = None) "missing package drift is reported before generated build failures"
            drift
            |> List.iter (fun item ->
                Expect.stringContains item.RemediationCommand "PackLocal" "drift diagnostics name PackLocal remediation")
        }

        test "generated consumer validation summaries expose category elapsed command and evidence" {
            let result =
                { Category = Completed
                  Elapsed = TimeSpan.FromSeconds 3.0
                  CommandContext = "./fake.sh build -t GeneratedProductCheck"
                  EvidencePath = Some "readiness/generated-consumer-validation.md"
                  Diagnostics = [ "scene evidence captured" ] }

            let summary = GeneratedConsumerValidation.summarize result
            Expect.stringContains summary "Completed" "category is present"
            Expect.stringContains summary "GeneratedProductCheck" "command context is present"
            Expect.stringContains summary "readiness/generated-consumer-validation.md" "evidence path is present"
            Expect.stringContains summary "scene evidence captured" "scene evidence diagnostics are present"

            let unsupported =
                GeneratedConsumerValidation.summarize
                    { result with
                        Category = UnsupportedHost
                        Diagnostics = [ "bounded viewer smoke unsupported"; "headless scene evidence captured" ] }

            Expect.stringContains unsupported "UnsupportedHost" "unsupported host category is preserved"
            Expect.stringContains unsupported "bounded viewer smoke unsupported" "bounded smoke unsupported diagnostic is summarized"
        }
    ]
