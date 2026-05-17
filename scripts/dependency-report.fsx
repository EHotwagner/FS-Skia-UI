open System
open System.IO
open System.Xml.Linq

let scriptDir = __SOURCE_DIRECTORY__
let repositoryRoot = Directory.GetParent(scriptDir).FullName

let path segments =
    segments |> Array.ofList |> Path.Combine

let xname name = XName.Get name

let relativePath (file: string) =
    file.Substring(repositoryRoot.Length)
        .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
        .Replace('\\', '/')

let normalizeProjectReference (includeValue: string) =
    includeValue.Replace('\\', '/')

let args =
    let raw = Environment.GetCommandLineArgs() |> Array.skip 1 |> Array.toList

    match raw with
    | script :: rest when script.EndsWith(".fsx", StringComparison.OrdinalIgnoreCase) -> rest
    | other -> other

let writeOutput (outputPath: string) content =
    if outputPath.EndsWith(".fsx", StringComparison.OrdinalIgnoreCase) then
        failwithf "Refusing to write report over script file: %s" outputPath

    match Path.GetDirectoryName outputPath |> Option.ofObj with
    | Some directory when directory <> "" -> Directory.CreateDirectory directory |> ignore
    | _ -> ()

    File.WriteAllText(outputPath, content + Environment.NewLine)

let outputPath, fixture =
    match args with
    | "--fixture" :: name :: output :: _ -> output, Some name
    | output :: _ -> output, None
    | [] -> path [ repositoryRoot; "specs"; "007-v2-template-packaging"; "readiness"; "dependencies.md" ], None

match fixture with
| Some "unmanaged-inline-version" ->
    let report =
        "# Dependency Governance Report\n\nFAIL: tests/Fixture/Fixture.fsproj declares PackageReference Include=\"Expecto\" Version=\"10.2.2\". Move the version to Directory.Packages.props."

    writeOutput outputPath report
    failwith "unmanaged inline external package version: tests/Fixture/Fixture.fsproj Expecto"
| Some other -> failwithf "Unknown dependency-report fixture: %s" other
| None -> ()

let centralPolicy = path [ repositoryRoot; "Directory.Packages.props" ]

if not (File.Exists centralPolicy) then
    failwith "Directory.Packages.props is required for Central Package Management"

let centralDoc = XDocument.Load centralPolicy

let centralVersions =
    centralDoc.Descendants(xname "PackageVersion")
    |> Seq.choose (fun node ->
        match node.Attribute(xname "Include") |> Option.ofObj with
        | Some includeAttr -> Some includeAttr.Value
        | None -> None)
    |> Set.ofSeq

let cpmEnabled =
    centralDoc.Descendants(xname "ManagePackageVersionsCentrally")
    |> Seq.exists (fun node -> node.Value.Trim().Equals("true", StringComparison.OrdinalIgnoreCase))

if not cpmEnabled then
    failwith "Directory.Packages.props must set ManagePackageVersionsCentrally to true"

let docsPath = path [ repositoryRoot; "docs"; "dependencies.md" ]
let docsContent =
    if File.Exists docsPath then File.ReadAllText docsPath else ""

let projectFiles =
    Directory.EnumerateFiles(repositoryRoot, "*.fsproj", SearchOption.AllDirectories)
    |> Seq.filter (fun file ->
        let relative = relativePath file

        (relative.StartsWith("src/", StringComparison.Ordinal)
         || relative.StartsWith("tests/", StringComparison.Ordinal)
         || relative.StartsWith("samples/", StringComparison.Ordinal))
        && not (relative.Contains("/bin/"))
        && not (relative.Contains("/obj/")))
    |> Seq.toList

let validationOnlyVersionAllowed (projectPath: string) (packageId: string) (condition: string) =
    packageId.StartsWith("FS.", StringComparison.Ordinal)
    && condition.Contains("UsePackedPackage")
    && projectPath.Replace('\\', '/').StartsWith("samples/", StringComparison.Ordinal)

let projectReferences relativeProject =
    let project = path [ repositoryRoot; relativeProject ]

    XDocument.Load(project).Descendants(xname "ProjectReference")
    |> Seq.choose (fun reference ->
        reference.Attribute(xname "Include")
        |> Option.ofObj
        |> Option.map (fun attr -> normalizeProjectReference attr.Value))
    |> Set.ofSeq

let packageReferences relativeProject =
    let project = path [ repositoryRoot; relativeProject ]

    XDocument.Load(project).Descendants(xname "PackageReference")
    |> Seq.choose (fun reference ->
        reference.Attribute(xname "Include")
        |> Option.ofObj
        |> Option.map (fun attr -> attr.Value))
    |> Set.ofSeq

let controlsBoundary =
    [ "FS.Skia.UI.Controls",
      "src/Controls/Controls.fsproj",
      Set.ofList [ "../Scene/Scene.fsproj"; "../Layout/Layout.fsproj"; "../KeyboardInput/KeyboardInput.fsproj" ],
      Set.empty,
      "Owns form controls, rich rendering, chart controls, graph views, DataGrid, and product-owned ControlRuntime declarations."
      "FS.Skia.UI.KeyboardInput",
      "src/KeyboardInput/KeyboardInput.fsproj",
      Set.ofList [ "../Scene/Scene.fsproj" ],
      Set.ofList [ "YamlDotNet" ],
      "Owns the rich keyboard input runtime, reducer/effect contracts, diagnostics, and YAML configuration parsing."
      "FS.Skia.UI.Controls.Elmish",
      "src/Controls.Elmish/Controls.Elmish.fsproj",
      Set.ofList [ "../Controls/Controls.fsproj"; "../KeyboardInput/KeyboardInput.fsproj" ],
      Set.ofList [ "Fable.Elmish" ],
      "Owns command, subscription, and program adapter integration so base Controls stays generic over product messages." ]

let removedChartsPackage = "FS.Skia.UI." + "Charts"
let removedChartsProjectSuffix = "src/" + "Charts/" + "Charts.fsproj"

let controlsBoundaryViolations =
    controlsBoundary
    |> List.collect (fun (packageId, relativeProject, expectedProjects, expectedPackages, _) ->
        let project = path [ repositoryRoot; relativeProject ]

        if not (File.Exists project) then
            [ $"{relativeProject}: {packageId} project is missing" ]
        else
            let actualProjects = projectReferences relativeProject
            let actualPackages = packageReferences relativeProject
            let expectedProjectsText = expectedProjects |> Set.toList |> String.concat ", "
            let actualProjectsText = actualProjects |> Set.toList |> String.concat ", "
            let expectedPackagesText = expectedPackages |> Set.toList |> String.concat ", "
            let actualPackagesText = actualPackages |> Set.toList |> String.concat ", "

            [ if actualProjects <> expectedProjects then
                  $"{relativeProject}: ProjectReference set changed; expected {expectedProjectsText} but found {actualProjectsText}"
              if actualPackages <> expectedPackages then
                  $"{relativeProject}: PackageReference set changed; expected {expectedPackagesText} but found {actualPackagesText}" ])

let removedChartsViolations =
    [ if File.Exists(path [ repositoryRoot; "src"; "Charts"; "Charts.fsproj" ]) then
          removedChartsProjectSuffix + ": legacy Charts package project is still active"

      yield!
          projectFiles
          |> List.collect (fun project ->
              let relativeProject = relativePath project
              let doc = XDocument.Load project

              [ yield!
                    doc.Descendants(xname "ProjectReference")
                    |> Seq.choose (fun reference ->
                        reference.Attribute(xname "Include")
                        |> Option.ofObj
                        |> Option.map (fun attr -> normalizeProjectReference attr.Value))
                    |> Seq.filter (fun includeValue -> includeValue.EndsWith(removedChartsProjectSuffix, StringComparison.Ordinal))
                    |> Seq.map (fun includeValue -> $"{relativeProject}: active ProjectReference points at removed Charts project {includeValue}")

                yield!
                    doc.Descendants(xname "PackageReference")
                    |> Seq.choose (fun reference ->
                        reference.Attribute(xname "Include")
                        |> Option.ofObj
                        |> Option.map (fun attr -> attr.Value))
                    |> Seq.filter (fun includeValue -> includeValue = removedChartsPackage)
                    |> Seq.map (fun _ -> $"{relativeProject}: active PackageReference points at removed Charts package") ]) ]

let violations =
    [ yield! controlsBoundaryViolations
      yield! removedChartsViolations
      yield!
          projectFiles
          |> List.collect (fun project ->
              let relativeProject = relativePath project

              let doc = XDocument.Load project

              doc.Descendants(xname "PackageReference")
              |> Seq.toList
              |> List.collect (fun reference ->
                  let includeAttr = reference.Attribute(xname "Include") |> Option.ofObj
                  let versionAttr = reference.Attribute(xname "Version") |> Option.ofObj
                  let itemCondition =
                      reference.Parent
                      |> Option.ofObj
                      |> Option.bind (fun parent -> parent.Attribute(xname "Condition") |> Option.ofObj)
                      |> Option.map (fun attr -> attr.Value)
                      |> Option.defaultValue ""

                  match includeAttr with
                  | None -> [ $"{relativeProject}: PackageReference missing Include" ]
                  | Some includeAttr ->
                      let packageId = includeAttr.Value
                      let hasInlineVersion = versionAttr |> Option.isSome

                      let inlineVersionViolation =
                          hasInlineVersion
                          && not (validationOnlyVersionAllowed relativeProject packageId itemCondition)

                      let centralPolicyViolation =
                          not (packageId.StartsWith("FS.", StringComparison.Ordinal))
                          && not (centralVersions.Contains packageId)

                      let docsViolation =
                          not (packageId.StartsWith("FS.", StringComparison.Ordinal))
                          && not (docsContent.Contains("| " + packageId + " |"))

                      [ if inlineVersionViolation then
                            $"{relativeProject}: unmanaged inline version on {packageId}; move it to Directory.Packages.props"
                        if centralPolicyViolation then
                            $"{relativeProject}: {packageId} missing PackageVersion in Directory.Packages.props"
                        if docsViolation then
                            $"{relativeProject}: {packageId} missing metadata row in docs/dependencies.md" ])) ]

if not (List.isEmpty violations) then
    let report =
        [ "# Dependency Governance Report"
          ""
          "FAIL"
          ""
          yield! violations |> List.map (fun violation -> "- " + violation) ]
        |> String.concat Environment.NewLine

    writeOutput outputPath report
    failwithf "Dependency governance failed:%s%s" Environment.NewLine (String.Join(Environment.NewLine, violations))

let usedPackages =
    projectFiles
    |> List.collect (fun project ->
        XDocument.Load(project).Descendants(xname "PackageReference")
        |> Seq.choose (fun reference ->
            reference.Attribute(xname "Include")
            |> Option.ofObj
            |> Option.map (fun attr -> attr.Value))
        |> Seq.toList)
    |> List.distinct
    |> List.sort

let report =
    [ "# Dependency Governance Report"
      ""
      "PASS: Central Package Management is enabled and repo-owned project files use versionless external PackageReference entries."
      ""
      "## Governed Packages"
      ""
      "| Package | Central version declared | Metadata documented |"
      "|---------|--------------------------|---------------------|"
      yield!
          usedPackages
          |> List.map (fun packageId ->
              let central =
                  if centralVersions.Contains packageId then
                      "yes"
                  elif packageId.StartsWith("FS.", StringComparison.Ordinal) then
                      "validation-only local package"
                  else
                      "no"

              let documented =
                  if docsContent.Contains("| " + packageId + " |") || packageId.StartsWith("FS.", StringComparison.Ordinal) then
                      "yes"
                  else
                      "no"

              $"| {packageId} | {central} | {documented} |")
      ""
      "## Controls Boundary Package Placement"
      ""
      "| Package | Project references | External packages | Boundary rule |"
      "|---------|--------------------|-------------------|---------------|"
      yield!
          controlsBoundary
          |> List.map (fun (packageId, _, expectedProjects, expectedPackages, rule) ->
              let projects =
                  if Set.isEmpty expectedProjects then
                      "none"
                  else
                      expectedProjects |> Set.toList |> String.concat ", "

              let packages =
                  if Set.isEmpty expectedPackages then
                      "none"
                  else
                      expectedPackages |> Set.toList |> String.concat ", "

              $"| {packageId} | {projects} | {packages} | {rule} |")
      ""
      "PASS: Legacy Charts package/project references are absent from active repo-owned project files."
      ""
      "Validation-only exception: sample package-mode PackageReference versions for `FS.*` packages are allowed only under `UsePackedPackage` conditions." ]
    |> String.concat Environment.NewLine

writeOutput outputPath report
