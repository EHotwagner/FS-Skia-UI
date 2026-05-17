module DependencyGovernanceTests

open System
open System.IO
open System.Xml.Linq
open Expecto
open GovernanceTestSupport

let xname name = XName.Get name

let projectReferences relativePath =
    let doc = readXml relativePath

    doc.Descendants(xname "ProjectReference")
    |> Seq.choose (fun reference ->
        reference.Attribute(xname "Include")
        |> Option.ofObj
        |> Option.map (fun attr -> attr.Value.Replace('\\', '/')))
    |> Seq.toList

let packageReferencesIn relativePath =
    let doc = readXml relativePath

    doc.Descendants(xname "PackageReference")
    |> Seq.choose (fun reference ->
        reference.Attribute(xname "Include")
        |> Option.ofObj
        |> Option.map (fun attr -> attr.Value))
    |> Seq.toList

[<Tests>]
let dependencyGovernanceTests =
    testList "Dependency governance" [
        test "Central Package Management declares direct external versions" {
            let doc = readXml "Directory.Packages.props"
            let content = doc.ToString()

            Expect.stringContains content "<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>" "CPM is enabled"

            [ "Expecto"
              "Fable.Elmish"
              "FSharp.Core"
              "Microsoft.NET.Test.Sdk"
              "Silk.NET.Vulkan"
              "SkiaSharp"
              "YamlDotNet"
              "Yoga.Net"
              "YoloDev.Expecto.TestSdk" ]
            |> List.iter (fun packageId -> Expect.stringContains content $"Include=\"{packageId}\"" $"{packageId} has central version")
        }

        test "repo-owned external PackageReference entries are versionless" {
            projectFiles ()
            |> List.iter (fun project ->
                let relative =
                    project.Substring(repositoryRoot.Length)
                        .TrimStart(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar)
                        .Replace('\\', '/')

                let doc = XDocument.Load project

                doc.Descendants(xname "PackageReference")
                |> Seq.iter (fun reference ->
                    let includeValue =
                        reference.Attribute(xname "Include")
                        |> Option.ofObj
                        |> Option.map (fun attr -> attr.Value)
                        |> Option.defaultValue ""

                    let versionAttr =
                        reference.Attribute(xname "Version")
                        |> Option.ofObj

                    let condition =
                        reference.Parent
                        |> Option.ofObj
                        |> Option.bind (fun parent -> parent.Attribute(xname "Condition") |> Option.ofObj)
                        |> Option.map (fun attr -> attr.Value)
                        |> Option.defaultValue ""

                    let allowed =
                        condition.Contains("UsePackedPackage")
                        && relative.StartsWith("samples/", StringComparison.Ordinal)

                    Expect.isTrue (versionAttr.IsNone || allowed) $"{relative} keeps {includeValue} under central package management")
                )
        }

        test "dependency docs include metadata fields and validation-only policy" {
            expectFileContains
                "docs/dependencies.md"
                [ "Purpose"
                  "Owner"
                  "License posture"
                  "Upgrade expectation"
                  "Preview risk"
                  "Validation-Only Exceptions"
                  "SkiaSharp"
                  "UsePackedPackage" ]
        }

        test "V3 dependency report names package ownership rules" {
            if directoryExists "specs/009-v3-modular-framework" then
                let reportPath =
                    if fileExists "specs/011-controls-boundary-refactor/readiness/dependency-report.md" then
                        "specs/011-controls-boundary-refactor/readiness/dependency-report.md"
                    elif fileExists "specs/010-skia-controls-library/readiness/dependency-report.md" then
                        "specs/010-skia-controls-library/readiness/dependency-report.md"
                    else
                        "specs/009-v3-modular-framework/readiness/dependency-report.md"

                expectFileContains
                    reportPath
                    [ "Scene has no Elmish, Silk.NET, SkiaSharp, Yoga.Net, or YamlDotNet dependency"
                      "SkiaViewer owns Silk.NET and SkiaSharp"
                      "Elmish owns Fable.Elmish"
                      "KeyboardInput owns YamlDotNet"
                      "Layout owns Yoga.Net"
                      "Controls"
                      "Testing" ]
            else
                Expect.isFalse (directoryExists ".template.config") "generated products do not carry source-only V3 readiness reports"
        }

        test "Controls boundary dependency report captures adapter and removed Charts package placement" {
            if fileExists "specs/011-controls-boundary-refactor/readiness/dependencies.md" then
                expectFileContains
                    "specs/011-controls-boundary-refactor/readiness/dependencies.md"
                    [ "Controls Boundary Package Placement"
                      "FS.Skia.UI.Controls"
                      "FS.Skia.UI.KeyboardInput"
                      "FS.Skia.UI.Controls.Elmish"
                      "Fable.Elmish"
                      "YamlDotNet"
                      "Legacy Charts package/project references are absent" ]

            expectFileContains
                "docs/dependencies.md"
                [ "The Controls boundary refactor does not add a third-party runtime dependency."
                  "FS.Skia.UI.Controls.Elmish"
                  "Legacy Charts package"
                  "replacement authoring lives under `FS.Skia.UI.Controls`" ]
        }

        test "V3 project references stay within declared package ownership" {
            expectFileContains
                "src/Scene/Scene.fsproj"
                [ "<PackageId>FS.Skia.UI.Scene</PackageId>"
                  "<IsPackable>true</IsPackable>" ]

            let scene = read "src/Scene/Scene.fsproj"

            [ "Fable.Elmish"
              "Silk.NET"
              "SkiaSharp"
              "Yoga.Net"
              "YamlDotNet" ]
            |> List.iter (fun forbidden -> Expect.isFalse (scene.Contains forbidden) $"Scene excludes {forbidden}")

            expectFileContains "src/SkiaViewer/SkiaViewer.fsproj" [ "FS.Skia.UI.SkiaViewer"; "Silk.NET"; "SkiaSharp"; "..\\Scene\\Scene.fsproj" ]
            expectFileContains "src/Elmish/Elmish.fsproj" [ "FS.Skia.UI.Elmish"; "Fable.Elmish"; "..\\Scene\\Scene.fsproj"; "..\\SkiaViewer\\SkiaViewer.fsproj" ]
            expectFileContains "src/KeyboardInput/KeyboardInput.fsproj" [ "FS.Skia.UI.KeyboardInput"; "YamlDotNet"; "..\\Scene\\Scene.fsproj" ]
        }

        test "Controls boundary dependencies and runtime ownership are explicit" {
            let controlsRefs = projectReferences "src/Controls/Controls.fsproj" |> Set.ofList

            Expect.equal
                controlsRefs
                (Set.ofList [ "../Scene/Scene.fsproj"; "../Layout/Layout.fsproj"; "../KeyboardInput/KeyboardInput.fsproj" ])
                "Controls depends only on Scene, Layout, and KeyboardInput"

            let controlsPackages = packageReferencesIn "src/Controls/Controls.fsproj"
            Expect.isEmpty controlsPackages "Controls has no direct external package references"

            [ "src/Lib/Lib.fsproj"
              "../Lib/Lib.fsproj"
              "../SkiaViewer/SkiaViewer.fsproj"
              "../Elmish/Elmish.fsproj" ]
            |> List.iter (fun forbidden ->
                Expect.isFalse (controlsRefs.Contains forbidden) $"Controls excludes hidden coupling {forbidden}")

            let adapterRefs = projectReferences "src/Controls.Elmish/Controls.Elmish.fsproj" |> Set.ofList
            let adapterPackages = packageReferencesIn "src/Controls.Elmish/Controls.Elmish.fsproj" |> Set.ofList

            Expect.equal
                adapterRefs
                (Set.ofList [ "../Controls/Controls.fsproj"; "../KeyboardInput/KeyboardInput.fsproj" ])
                "Controls.Elmish adapter references only Controls and KeyboardInput"
            Expect.equal adapterPackages (Set.ofList [ "Fable.Elmish" ]) "Fable.Elmish is isolated in the adapter package"

            let keyboardRuntimeDefinitions =
                [ "src/Controls"; "src/Controls.Elmish"; "src/KeyboardInput" ]
                |> List.collect (fun relative ->
                    Directory.EnumerateFiles(fullPath relative, "*.fs*", SearchOption.TopDirectoryOnly)
                    |> Seq.filter (fun file -> File.ReadAllText(file).Contains("type KeyboardModel", StringComparison.Ordinal))
                    |> Seq.map (fun file -> file.Substring(repositoryRoot.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace('\\', '/'))
                    |> Seq.toList)
                |> Set.ofList

            Expect.equal
                keyboardRuntimeDefinitions
                (Set.ofList [ "src/KeyboardInput/KeyboardInput.fs"; "src/KeyboardInput/KeyboardInput.fsi" ])
                "KeyboardInput owns the single rich keyboard runtime contract and implementation"
        }
    ]
