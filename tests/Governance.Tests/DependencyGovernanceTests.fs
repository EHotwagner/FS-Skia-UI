module DependencyGovernanceTests

open System
open System.Xml.Linq
open Expecto
open GovernanceTestSupport

let xname name = XName.Get name

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
    ]
