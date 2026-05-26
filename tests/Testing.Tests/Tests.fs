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
                        Category = GeneratedValidationCategory.UnsupportedHost
                        Diagnostics = [ "bounded viewer smoke unsupported"; "headless scene evidence captured" ] }

            Expect.stringContains unsupported "UnsupportedHost" "unsupported host category is preserved"
            Expect.stringContains unsupported "bounded viewer smoke unsupported" "bounded smoke unsupported diagnostic is summarized"
        }

        test "generated package verification fails NU1603 exact-version drift and missing sources" {
            let requested =
                [ { PackageId = "FS.Skia.UI.SkiaViewer"; Version = "0.1.16-persistent.1"; FeedPath = "/tmp/feed" } ]

            let resolved =
                [ { PackageId = "FS.Skia.UI.SkiaViewer"; Version = "0.1.16-preview.1"; FeedPath = "/tmp/feed" } ]

            let result =
                GeneratedConsumerValidation.verifyPackageResolution
                    { RequestedPackages = requested
                      ResolvedPackages = resolved
                      PackageSources = []
                      RestoreWarnings = [ "NU1603: FS.Skia.UI.SkiaViewer 0.1.16-persistent.1 was not found" ] }

            Expect.isFalse result.ExactMatch "NU1603 prevents exact package verification"
            Expect.equal result.FailureReason (Some "NU1603") "NU1603 is the primary failure class"
            Expect.exists result.Diagnostics (fun item -> item.Contains "missing package sources") "missing sources are reported"
            Expect.exists result.Diagnostics (fun item -> item.Contains "package mismatch") "requested/resolved drift is reported"
        }

        test "generated package verification accepts exact requested resolved versions and configured sources" {
            let requested =
                [ { PackageId = "FS.Skia.UI.Scene"; Version = "0.1.16-persistent.1"; FeedPath = "/tmp/feed" }
                  { PackageId = "FS.Skia.UI.SkiaViewer"; Version = "0.1.16-persistent.1"; FeedPath = "/tmp/feed" }
                  { PackageId = "FS.Skia.UI.Testing"; Version = "0.1.16-persistent.1"; FeedPath = "/tmp/feed" } ]

            let result =
                GeneratedConsumerValidation.verifyPackageResolution
                    { RequestedPackages = requested
                      ResolvedPackages = requested
                      PackageSources = [ "/tmp/feed"; "https://api.nuget.org/v3/index.json" ]
                      RestoreWarnings = [] }

            Expect.isTrue result.ExactMatch "exact requested/resolved package versions are authoritative"
            Expect.isNone result.FailureReason "no package failure class is reported for exact resolution"
            Expect.isEmpty result.Diagnostics "exact package resolution with configured sources has no diagnostics"
        }

        test "generated package verification reports version mismatch failure when NU1603 is absent" {
            let requested =
                [ { PackageId = "FS.Skia.UI.Testing"; Version = "0.1.16-persistent.1"; FeedPath = "/tmp/feed" } ]

            let resolved =
                [ { PackageId = "FS.Skia.UI.Testing"; Version = "0.1.16-preview.1"; FeedPath = "/tmp/feed" } ]

            let result =
                GeneratedConsumerValidation.verifyPackageResolution
                    { RequestedPackages = requested
                      ResolvedPackages = resolved
                      PackageSources = [ "/tmp/feed" ]
                      RestoreWarnings = [] }

            Expect.isFalse result.ExactMatch "requested/resolved drift blocks exact package verification"
            Expect.equal result.FailureReason (Some "version-mismatch") "version mismatch has its own failure class"
            Expect.exists result.Diagnostics (fun item -> item.Contains "requested=0.1.16-persistent.1") "requested version is reported"
            Expect.exists result.Diagnostics (fun item -> item.Contains "resolved=0.1.16-preview.1") "resolved version is reported"
        }

        test "generated verification is non-authoritative when tests exist but do not run" {
            let result =
                GeneratedConsumerValidation.verifyGeneratedTests
                    { TestsExist = true
                      TestsRan = false
                      VerifyRan = true }

            Expect.isFalse result.Authoritative "verify is not authoritative when generated tests are skipped"
            Expect.equal result.NonAuthoritativeReason (Some "missing-generated-test-execution") "missing generated tests are classified"
            Expect.exists result.Diagnostics (fun item -> item.Contains "did not run") "diagnostic explains skipped tests"
        }

        test "generated verification is authoritative only when generated tests run through Verify" {
            let result =
                GeneratedConsumerValidation.verifyGeneratedTests
                    { TestsExist = true
                      TestsRan = true
                      VerifyRan = true }

            Expect.isTrue result.Authoritative "generated tests run through Verify are authoritative"
            Expect.isNone result.NonAuthoritativeReason "authoritative generated verification has no failure class"
            Expect.isEmpty result.Diagnostics "authoritative generated verification has no diagnostics"
        }

        test "generated verification is non-authoritative when tests bypass Verify" {
            let result =
                GeneratedConsumerValidation.verifyGeneratedTests
                    { TestsExist = true
                      TestsRan = true
                      VerifyRan = false }

            Expect.isFalse result.Authoritative "generated tests outside Verify do not prove generated Verify coverage"
            Expect.equal result.NonAuthoritativeReason (Some "verify-target-not-authoritative") "Verify bypass has its own failure class"
            Expect.exists result.Diagnostics (fun item -> item.Contains "outside generated Verify") "diagnostic names Verify bypass"
        }

        test "generated product validation requires interactive default launch and rejects bounded-only substitutes" {
            let validSource =
                """
[<EntryPoint>]
let main args =
    match List.ofArray args with
    | "--launch-evidence" :: path :: _ -> launchEvidence path
    | _ ->
        match Viewer.runApp viewerOptions generatedHost with
        | Result.Ok outcome ->
            printfn "status=%s mode=interactive-window" outcome.Status
            0
        | Result.Error _ -> 1
"""

            let invalidSource =
                """
[<EntryPoint>]
let main args =
    match List.ofArray args with
    | _ ->
        let evidence = Viewer.runBounded request viewerOptions scene
        printfn "mode=persistent-evidence self-closed-for-evidence=true print metadata"
        0
"""

            let valid = GeneratedProductAssertions.validateDefaultInteractiveLaunch validSource
            Expect.isTrue valid.InteractiveLaunchRequired "interactive runApp default path is accepted"
            Expect.isEmpty valid.Diagnostics "valid default launch has no diagnostics"

            let invalid = GeneratedProductAssertions.validateDefaultInteractiveLaunch invalidSource
            Expect.isFalse invalid.InteractiveLaunchRequired "bounded-only default path is rejected"
            Expect.exists invalid.Diagnostics (fun item -> item.Contains "Viewer.runApp") "missing runApp is diagnostic"
            Expect.exists invalid.Diagnostics (fun item -> item.Contains "Viewer.runBounded") "bounded substitute is diagnostic"
            Expect.exists invalid.Diagnostics (fun item -> item.Contains "self-close") "evidence self-close is diagnostic"
            Expect.exists invalid.Diagnostics (fun item -> item.Contains "metadata-only") "metadata-only default is diagnostic"
        }

        test "visual evidence prefers screenshots and preserves board input progress fields" {
            let result =
                GeneratedConsumerValidation.selectVisualEvidence
                    { ScreenshotAvailable = true
                      PixelReadbackAvailable = true
                      BoardReadable = Some true
                      InputOrProgressObserved = Some true
                      UnsupportedReason = None }

            Expect.equal result.EvidenceKind Screenshot "screenshot is preferred when available"
            Expect.equal result.BoardReadable (Some true) "board readability is preserved"
            Expect.equal result.InputOrProgressObserved (Some true) "input/progress observation is preserved"
            Expect.isNone result.FallbackReason "screenshot path does not need fallback"
        }

        test "visual evidence uses pixel-readback fallback only when screenshots are unavailable" {
            let result =
                GeneratedConsumerValidation.selectVisualEvidence
                    { ScreenshotAvailable = false
                      PixelReadbackAvailable = true
                      BoardReadable = Some true
                      InputOrProgressObserved = Some true
                      UnsupportedReason = None }

            Expect.equal result.EvidenceKind PixelReadback "pixel-readback is selected as fallback"
            Expect.equal result.FallbackReason (Some "screenshot unavailable; pixel-readback selected") "fallback reason is explicit"
            Expect.exists result.Diagnostics (fun item -> item.Contains "screenshot unavailable") "diagnostics name screenshot fallback"
        }

        test "visual evidence reports unsupported host when screenshot and readback are unavailable" {
            let result =
                GeneratedConsumerValidation.selectVisualEvidence
                    { ScreenshotAvailable = false
                      PixelReadbackAvailable = false
                      BoardReadable = None
                      InputOrProgressObserved = None
                      UnsupportedReason = Some "headless session has no display socket" }

            Expect.equal result.EvidenceKind VisualEvidenceKind.UnsupportedHost "unsupported host is explicit"
            Expect.equal result.UnsupportedReason (Some "headless session has no display socket") "unsupported reason is retained"
            Expect.isNone result.BoardReadable "unsupported host cannot claim readable board"
            Expect.isNone result.InputOrProgressObserved "unsupported host cannot claim input/progress"
        }
    ]
