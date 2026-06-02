module UpgradeSkiaSpecKitTests

open System
open System.IO
open System.Xml.Linq
open Expecto
open GovernanceTestSupport

let private xname name = XName.Get name

let private packageVersions relativePath =
    let doc = readXml relativePath

    doc.Descendants(xname "PackageVersion")
    |> Seq.choose (fun node ->
        let includeAttr = node.Attribute(xname "Include") |> Option.ofObj
        let versionAttr = node.Attribute(xname "Version") |> Option.ofObj

        match includeAttr, versionAttr with
        | Some packageId, Some version -> Some(packageId.Value, version.Value)
        | _ -> None)
    |> Map.ofSeq

let private projectPackageIds relativePath =
    let doc = readXml relativePath

    doc.Descendants(xname "PackageReference")
    |> Seq.choose (fun node -> node.Attribute(xname "Include") |> Option.ofObj |> Option.map _.Value)
    |> Set.ofSeq

let private projectVersions () =
    Directory.EnumerateFiles(fullPath "src", "*.fsproj", SearchOption.AllDirectories)
    |> Seq.choose (fun project ->
        let doc = XDocument.Load project
        let packageId = doc.Descendants(xname "PackageId") |> Seq.tryHead |> Option.map _.Value
        let version = doc.Descendants(xname "Version") |> Seq.tryHead |> Option.map _.Value
        Option.map2 (fun id value -> id, value) packageId version)
    |> Map.ofSeq

let private requiredReadinessArtifacts =
    [ "version-selection.md"
      "dependency-report.md"
      "template-version-alignment.md"
      "compatibility-consumer-inventory.md"
      "compatibility-public-surface-map.md"
      "compatibility-sample-migration.md"
      "compatibility-release-policy.md"
      "package-surface-baseline.md"
      "evidence-audit.md" ]
    |> List.map (sprintf "specs/025-upgrade-skia-speckit/readiness/%s")

[<Tests>]
let upgradeSkiaSpecKitTests =
    testList "SkiaSharp and Spec Kit upgrade governance" [
        test "SkiaSharp package family is version-aligned and recorded in readiness" {
            let versions = packageVersions "Directory.Packages.props"

            let skiaPackages =
                [ "SkiaSharp"
                  "SkiaSharp.NativeAssets.Linux"
                  "SkiaSharp.NativeAssets.Win32" ]

            let selected = skiaPackages |> List.map (fun packageId -> versions[packageId]) |> Set.ofList
            Expect.equal selected (Set.singleton "4.147.0-preview.3.1") "managed and native SkiaSharp packages share the selected implementation-time version"

            expectFileContains
                "specs/025-upgrade-skia-speckit/readiness/version-selection.md"
                [ "4.147.0-preview.3.1"
                  "https://api.nuget.org/v3-flatcontainer/skiasharp/index.json"
                  "https://api.nuget.org/v3-flatcontainer/skiasharp.nativeassets.linux/index.json"
                  "SkiaSharp.NativeAssets.Win32"
                  "Preview package" ]
        }

        test "Spec Kit root metadata and generated guidance posture record selected release" {
            expectFileContains ".specify/init-options.json" [ "\"speckit_version\": \"0.8.16\"" ]

            expectFileContains
                "specs/025-upgrade-skia-speckit/readiness/version-selection.md"
                [ "0.8.16"
                  "https://api.github.com/repos/github/spec-kit/releases/latest"
                  "published 2026-05-27T21:35:15Z"
                  ".specify/init-options.json" ]

            expectFileContains
                "specs/025-upgrade-skia-speckit/readiness/template-version-alignment.md"
                [ "Spec Kit"
                  "0.8.16"
                  "GeneratedGuidanceCheck"
                  "TemplateCheck" ]
        }

        test "dependency readiness records before after versions and package graph status" {
            expectFileContains
                "specs/025-upgrade-skia-speckit/readiness/dependency-report.md"
                [ "Before"
                  "After"
                  "4.147.0-preview.2.1"
                  "4.147.0-preview.3.1"
                  "DependencyReport"
                  "cycle status"
                  "unexpected spread"
                  "SkiaSharp.NativeAssets.Linux"
                  "SkiaSharp.NativeAssets.Win32" ]
        }

        test "template package pins match current repository package posture" {
            let templateVersions = packageVersions "template/base/Directory.Packages.props"
            let repoVersions = projectVersions ()

            [ "FS.Skia.UI.Scene"
              "FS.Skia.UI.SkiaViewer"
              "FS.Skia.UI.Elmish"
              "FS.Skia.UI.KeyboardInput"
              "FS.Skia.UI.Layout"
              "FS.Skia.UI.Controls"
              "FS.Skia.UI.Controls.Elmish"
              "FS.Skia.UI.Testing" ]
            |> List.iter (fun packageId ->
                Expect.equal templateVersions[packageId] repoVersions[packageId] $"{packageId} generated pin matches repository package version")

            expectFileContains
                "specs/025-upgrade-skia-speckit/readiness/template-version-alignment.md"
                [ "app"
                  "governed"
                  "headless-scene"
                  "sample-pack"
                  "broad-package dependency status" ]
        }

        test "focused generated profiles do not expand broad FS.Skia.UI dependency" {
            // V3 Stage 5: the broad compatibility package is retired; its id is named via parts
            // so the SC-001 no-consumer grep stays clean while the guard still bites.
            let monolith = "FS.Skia." + "UI"
            let productPackages = projectPackageIds "template/base/src/Product/Product.fsproj"
            let testPackages = projectPackageIds "template/base/tests/Product.Tests/Product.Tests.fsproj"

            Expect.isFalse (productPackages.Contains monolith) "generated product source does not reference broad compatibility package"
            Expect.isFalse (testPackages.Contains monolith) "generated product tests do not reference broad compatibility package"
            Expect.isFalse ((read "template/base/Directory.Packages.props").Contains($"Include=\"{monolith}\"")) "template central pins omit broad compatibility package"
        }

        test "compatibility consumer inventory covers repository consumer classes" {
            expectFileContains
                "specs/025-upgrade-skia-speckit/readiness/compatibility-consumer-inventory.md"
                [ "ProjectReference"
                  "PackageReference"
                  "namespace open"
                  "samples/BasicViewer"
                  "samples/ScreenshotGallery"
                  "docs/"
                  "template"
                  "packaged-mode guidance"
                  "focused replacement" ]
        }

        test "compatibility public surface and release posture are explicit" {
            expectFileContains
                "specs/025-upgrade-skia-speckit/readiness/compatibility-public-surface-map.md"
                [ "primary-only"
                  "duplicate"
                  "facade candidate"
                  "deprecated candidate"
                  "permanent compatibility surface"
                  "FS.Skia.UI.Scene"
                  "FS.Skia.UI.SkiaViewer" ]

            expectFileContains
                "specs/025-upgrade-skia-speckit/readiness/package-surface-baseline.md"
                [ "PackageSurfaceCheck"
                  "no `.fsi` change"
                  ("FS.Skia." + "UI")
                  "unchanged" ]

            expectFileContains
                "specs/025-upgrade-skia-speckit/readiness/compatibility-release-policy.md"
                [ "near-term"
                  "stable compatibility surface"
                  "focused packages"
                  "unknown external consumers"
                  "deferred decisions" ]
        }

        test "documentation and guidance carry selected version and conservative compatibility posture" {
            expectFileContains
                "docs/reports/dependencies.md"
                [ "4.147.0-preview.3.1"
                  // V3 Stage 5: the broad compatibility monolith is retired; the dependency doc
                  // now records the retirement and steers consumers to the split packages.
                  "compatibility monolith was retired"
                  "reference the focused split packages"
                  "docs/migration/v2-to-v3.md" ]

            expectFileContains
                "specs/025-upgrade-skia-speckit/readiness/compatibility-sample-migration.md"
                [ "BasicViewer"
                  "ScreenshotGallery"
                  "keep unchanged"
                  "supported sample behavior" ]
        }

        test "viewer native diagnostic evidence remains observable" {
            expectFileContains
                "specs/025-upgrade-skia-speckit/readiness/logs/skia-native-viewer-validation.md"
                [ "platform"
                  "command"
                  "failure reason"
                  "blocks acceptance" ]
        }

        test "required readiness artifacts contain real evidence provenance" {
            requiredReadinessArtifacts
            |> List.iter (fun path ->
                Expect.isTrue (fileExists path) $"{path} exists"
                Expect.isFalse ((read path).Contains("Status: pending", StringComparison.OrdinalIgnoreCase)) $"{path} is finalized"
                Expect.stringContains (read path) "Evidence" $"{path} names evidence provenance")
        }
    ]
