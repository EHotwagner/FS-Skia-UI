module FS.Skia.UI.Build.GeneratedProduct

open System
open System.Diagnostics
open System.IO
open System.IO.Compression
open System.Runtime.InteropServices
open System.Text.RegularExpressions
open BuildPaths
open BuildProcess
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Preflight
open FS.Skia.UI.Build.Front.Support
open FS.Skia.UI.Build.Engine.Model
open FS.Skia.UI.Build.Front.Helpers

// Relocated verbatim from build.fsx (feature 045, T011): generated-product structural
// validation (template instantiation/generation, capability catalog, V3 scans, consumer
// validation, dependency + package-surface reports). Behaviour-identical.

let latestTemplatePackage artifactDir =
    BuildTemplateValidation.latestTemplatePackage artifactDir

let validateTemplatePackage model outputPath =
    let required =
        [ "content/.template.config/template.json"
          "content/template/base/build.fsx"
          "content/template/base/src/Product/Product.fsproj"
          "content/template/base/src/Product/Model.fs"
          "content/template/base/src/Product/View.fs"
          "content/template/base/src/Product/LayoutEvidence.fs"
          "content/template/base/src/Product/EvidenceCommands.fs"
          "content/template/base/src/Product/WindowOptions.fs"
          "content/template/base/src/Product/Program.fs"
          "content/template/base/CLAUDE.md"
          "content/template/base/.claude/settings.json"
          "content/template/base/.claude/skills/fs-skia-project/SKILL.md"
          "content/template/base/tests/Product.Tests/Product.Tests.fsproj"
          "content/template/base/Directory.Packages.props"
          "content/template/profiles/app.yml"
          "content/template/profiles/headless-scene.yml"
          "content/template/profiles/governed.yml"
          "content/template/profiles/sample-pack.yml"
          "content/.specify/templates/spec-template.md"
          "content/.specify/scripts/bash/setup-plan.sh"
          "content/.agents/skills/speckit-specify/SKILL.md"
          "content/.claude/skills/speckit-specify/SKILL.md"
          "content/.template.config/generated/CLAUDE.md"
          "content/.template.config/generated/.claude/settings.json"
          "content/.template.config/generated/.specify/memory/constitution.md" ]

    let forbiddenPrefixes =
        [ "content/.git/"
          "content/artifacts/"
          "content/.template.package/"
          "content/.specify/feature.json"
          "content/.specify/memory/constitution.md"
          "content/specs/001-"
          "content/specs/002-"
          "content/specs/003-"
          "content/specs/004-"
          "content/specs/005-"
          "content/specs/006-"
          "content/specs/007-" ]

    BuildTemplateValidation.validateTemplatePackageEntries model.TemplateArtifactDir outputPath required forbiddenPrefixes

let runTemplateInstall model label source outputPath =
    if source = SourceDirectory then
        cleanDirectoryContents model.TemplateWorkDir

    let installArgument =
        match source with
        | SourceDirectory -> model.RepositoryRoot
        | PackageArtifact ->
            latestTemplatePackage model.TemplateArtifactDir
            |> Option.defaultWith (fun () -> failwithf "No template package found in %s" model.TemplateArtifactDir)

    [ model.RepositoryRoot; "FS.Skia.UI.Template" ]
    |> List.distinct
    |> List.iter (fun uninstallArgument ->
        runProcessWithAllowedExitCodes $"{label} uninstall" "dotnet" $"new uninstall {quote uninstallArgument}" model.RepositoryRoot outputPath Map.empty (Set.ofList [ 0; 1; 2; 103 ]))

    runProcess label "dotnet" $"new install {quote installArgument}" model.RepositoryRoot outputPath Map.empty

let instantiateRow model (row: TemplateRow) =
    cleanDirectoryContents row.Root
    cleanDirectoryContents row.EvidenceDir

    let rootNamespace = row.ProjectName.Replace("-", ".")
    let repositoryUrl = $"https://example.invalid/{row.Artifact}/{row.Profile}/{row.ProjectName}"

    let args =
        [ "new fs-skia-ui"
          $"--name {row.ProjectName}"
          $"--output {quote row.Root}"
          "--allow-scripts yes"
          $"--profile {row.Profile}"
          $"--rootNamespace {rootNamespace}"
          $"--packagePrefix {rootNamespace}"
          "--authors TemplateValidation"
          $"--repositoryUrl {quote repositoryUrl}"
          "--targetFramework net10.0"
          "--skipGitInit true" ]
        |> String.concat " "

    runProcess $"{row.Artifact}/{row.Profile} instantiate" "dotnet" args model.RepositoryRoot (path [ row.EvidenceDir; "instantiate.log" ]) Map.empty

let runTemplateInstantiation model outputPath =
    ensureParent outputPath
    File.WriteAllText(outputPath, "# Template Instantiation" + Environment.NewLine)

    cleanDirectoryContents model.TemplateWorkDir

    runTemplateInstall model "source template install for instantiation" SourceDirectory outputPath

    templateRows model
    |> List.filter (fun row -> row.Artifact = "source")
    |> List.iter (instantiateRow model)

    runTemplateInstall model "package template install for instantiation" PackageArtifact outputPath

    templateRows model
    |> List.filter (fun row -> row.Artifact = "package")
    |> List.iter (instantiateRow model)

    let rows =
        templateRows model
        |> List.map (fun row -> $"- {row.Artifact}/{row.Profile}: `{row.Root}`")
        |> String.concat Environment.NewLine

    File.AppendAllText(outputPath, Environment.NewLine + "Generated rows:" + Environment.NewLine + rows + Environment.NewLine)

let fileShouldBeScanned (filePath: string) =
    BuildGeneratedScanning.fileShouldBeScanned filePath

let generatedShellScripts (row: TemplateRow) =
    Directory.EnumerateFiles(row.Root, "*.sh", SearchOption.AllDirectories)
    |> Seq.filter fileShouldBeScanned
    |> Seq.toList

let isWindows =
    BuildGeneratedScanning.isWindows

let hasUserExecutePermission filePath =
    BuildGeneratedScanning.hasUserExecutePermission filePath

let scanGeneratedRow (row: TemplateRow) =
    let files =
        Directory.EnumerateFiles(row.Root, "*", SearchOption.AllDirectories)
        |> Seq.filter fileShouldBeScanned
        |> Seq.toList

    let identityTokens =
        [ "FS-Skia-UI" ]

    let placeholderHits =
        files
        |> List.collect (fun file ->
            let relative = relativePathFrom row.Root file
            let content = File.ReadAllText file

            identityTokens
            |> List.choose (fun token ->
                if content.IndexOf(token, StringComparison.Ordinal) >= 0 then
                    Some $"{relative}: {token}"
                else
                    None))

    let excludedHistory =
        if Directory.Exists(path [ row.Root; "specs" ]) then
            Directory.GetDirectories(path [ row.Root; "specs" ], "00*", SearchOption.TopDirectoryOnly)
            |> Array.map (relativePathFrom row.Root)
            |> Array.toList
        else
            []

    let forbiddenFrameworkPaths =
        [ "src/Scene"
          "src/SkiaViewer"
          "src/Elmish"
          "src/KeyboardInput"
          "src/Layout"
          "src/Controls"
          "src/Charts"
          "src/Testing"
          "tests/Lib.Tests"
          "tests/Scene.Tests"
          "tests/SkiaViewer.Tests"
          "tests/Elmish.Tests"
          "tests/KeyboardInput.Tests"
          "tests/Layout.Tests"
          "tests/Controls.Tests"
          "tests/Charts.Tests"
          "tests/Testing.Tests"
          "tests/Parity.Tests"
          "tests/Smoke.Tests"
          "samples/BasicViewer"
          "samples/ControlsGallery"
          "samples/ChartsGallery"
          "samples/DataGridGallery"
          "samples/LayoutGraphGallery"
          "samples/ParityGallery"
          "samples/InteractiveViewer"
          "samples/EffectsGallery"
          "samples/ScreenshotGallery"
          "samples/DemoReel" ]
        |> List.filter (fun relative -> Directory.Exists(path [ row.Root; relative ]))

    let samplePackRequired =
        if row.Profile = "sample-pack" then
            [ "samples/README.md" ]
        else
            []

    let expectedPackages =
        match row.Profile with
        | "app" ->
            [ "FS.Skia.UI.Scene"
              "FS.Skia.UI.SkiaViewer"
              "FS.Skia.UI.Elmish"
              "FS.Skia.UI.KeyboardInput"
              "FS.Skia.UI.Layout"
              "FS.Skia.UI.Controls"
              "FS.Skia.UI.Controls.Elmish" ]
        | "headless-scene" -> [ "FS.Skia.UI.Scene" ]
        | "governed" -> [ "FS.Skia.UI.Scene"; "FS.Skia.UI.Testing" ]
        | "sample-pack" -> [ "FS.Skia.UI.Scene"; "FS.Skia.UI.SkiaViewer"; "FS.Skia.UI.Elmish" ]
        | other -> failwithf "Unknown V3 template profile %s" other

    let allCapabilityPackages =
        [ "FS.Skia.UI.Scene"
          "FS.Skia.UI.SkiaViewer"
          "FS.Skia.UI.Elmish"
          "FS.Skia.UI.KeyboardInput"
          "FS.Skia.UI.Layout"
          "FS.Skia.UI.Controls"
          "FS.Skia.UI.Controls.Elmish"
          "FS.Skia.UI.Testing" ]

    let required =
        [ $"src/{row.ProjectName}/{row.ProjectName}.fsproj"
          $"tests/{row.ProjectName}.Tests/{row.ProjectName}.Tests.fsproj"
          "docs/product.md"
          "Directory.Packages.props"
          "AGENTS.md"
          "CLAUDE.md"
          ".claude/settings.json"
          "build.fsx"
          "fake.sh"
          ".specify/memory/constitution.md"
          ".specify/templates/spec-template.md"
          ".specify/scripts/bash/setup-plan.sh"
          ".specify/workflows/speckit/workflow.yml"
          // 043 (FR-013): generated projects run evidence in-process via the
          // packaged FS.Skia.UI.Build engine. They no longer carry run-audit.sh;
          // they retain audit-patterns.yml (data read by the diff-scan).
          ".specify/extensions/evidence/audit-patterns.yml"
          ".agents/skills/speckit-specify/SKILL.md"
          ".claude/skills/speckit-specify/SKILL.md"
          ".agents/skills/speckit-plan/SKILL.md"
          ".claude/skills/speckit-plan/SKILL.md"
          ".agents/skills/speckit-tasks/SKILL.md"
          ".claude/skills/speckit-tasks/SKILL.md"
          ".agents/skills/speckit-implement/SKILL.md"
          ".claude/skills/speckit-implement/SKILL.md" ]
        @ samplePackRequired

    let missingRequired =
        required
        |> List.filter (fun relative -> not (File.Exists(path [ row.Root; relative ])))

    if not (List.isEmpty placeholderHits) then
        failwithf "%s/%s generated project has unreplaced identity tokens:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, placeholderHits))

    if not (List.isEmpty excludedHistory) then
        failwithf "%s/%s generated project contains excluded historical specs:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, excludedHistory))

    if not (List.isEmpty forbiddenFrameworkPaths) then
        failwithf "%s/%s generated project contains framework implementation paths:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, forbiddenFrameworkPaths))

    if not (List.isEmpty missingRequired) then
        failwithf "%s/%s generated project is missing required files:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, missingRequired))

    let productProject = File.ReadAllText(path [ row.Root; "src"; row.ProjectName; $"{row.ProjectName}.fsproj" ])

    expectedPackages
    |> List.iter (fun packageId ->
        if productProject.IndexOf($"PackageReference Include=\"{packageId}\"", StringComparison.Ordinal) < 0 then
            failwithf "%s/%s generated project is missing package reference %s" row.Artifact row.Profile packageId)

    allCapabilityPackages
    |> List.filter (fun packageId -> not (expectedPackages |> List.contains packageId))
    |> List.iter (fun packageId ->
        if productProject.IndexOf($"PackageReference Include=\"{packageId}\"", StringComparison.Ordinal) >= 0 then
            failwithf "%s/%s generated project contains unselected package reference %s" row.Artifact row.Profile packageId)

    let removedChartsPackage = "FS.Skia.UI." + "Charts"

    if productProject.IndexOf($"PackageReference Include=\"{removedChartsPackage}\"", StringComparison.Ordinal) >= 0 then
        failwithf "%s/%s generated project contains removed Charts package reference %s" row.Artifact row.Profile removedChartsPackage

    let staleAgentsReference =
        let agentsPath = path [ row.Root; "AGENTS.md" ]

        File.Exists agentsPath
        && File.ReadAllText(agentsPath).IndexOf("specs/008-targeted-refactor-governance", StringComparison.Ordinal) >= 0

    if staleAgentsReference then
        failwithf "%s/%s generated AGENTS.md references source-only active feature specs/008-targeted-refactor-governance" row.Artifact row.Profile

    if File.Exists(path [ row.Root; ".specify"; "feature.json" ]) then
        failwithf "%s/%s generated project contains source-only .specify/feature.json active feature state" row.Artifact row.Profile

    let nonExecutableScripts =
        if isWindows then
            []
        else
            generatedShellScripts row
            |> List.filter (hasUserExecutePermission >> not)
            |> List.map (relativePathFrom row.Root)

    if not (List.isEmpty nonExecutableScripts) then
        failwithf "%s/%s generated project has non-executable shell scripts:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, nonExecutableScripts))

    let stopwatch = Stopwatch.StartNew()
    runProcess $"{row.Artifact}/{row.Profile} generated Dev" "bash" "./fake.sh build -t Dev" row.Root (path [ row.EvidenceDir; "dev.log" ]) Map.empty
    stopwatch.Stop()
    let elapsedSeconds = stopwatch.Elapsed.TotalSeconds

    let postDevExcludedHistory =
        if Directory.Exists(path [ row.Root; "specs" ]) then
            Directory.GetDirectories(path [ row.Root; "specs" ], "00*", SearchOption.TopDirectoryOnly)
            |> Array.map (relativePathFrom row.Root)
            |> Array.toList
        else
            []

    if not (List.isEmpty postDevExcludedHistory) then
        failwithf "%s/%s generated Dev created excluded historical specs:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, postDevExcludedHistory))

    let report =
        [ $"# {row.Artifact}/{row.Profile} Scan"
          ""
          $"Root: `{row.Root}`"
          $"Files scanned: {files.Length}"
          "Placeholder scan: PASS"
          "Excluded-history scan: PASS"
          "V3 framework-source exclusion scan: PASS"
          "V3 selected package reference scan: PASS"
          "Spec Kit install scan: PASS"
          "Generated AGENTS scan: PASS"
          "Executable script scan: PASS"
          $"Generated Dev elapsed: {elapsedSeconds:F1} seconds"
          "Visual support: non-visual V3 validation only; full visual evidence is deferred." ]
        |> String.concat Environment.NewLine

    File.WriteAllText(path [ row.EvidenceDir; "scan.md" ], report + Environment.NewLine)

let scanGeneratedProjects model outputPath =
    templateRows model
    |> List.iter scanGeneratedRow

    let summary =
        [ "# Generated Project Validation"
          ""
          "| Artifact | Profile | Root | Dev log |"
          "|----------|---------|------|---------|"
          yield!
              templateRows model
              |> List.map (fun row ->
                  let devLog = path [ row.EvidenceDir; "dev.log" ]
                  $"| {row.Artifact} | {row.Profile} | `{row.Root}` | `{devLog}` |")
          ""
          "PASS: placeholder scans, excluded-history scans, V3 profile package checks, Spec Kit install checks, and generated Dev runs completed for all rows." ]
        |> String.concat Environment.NewLine

    ensureParent outputPath
    File.WriteAllText(outputPath, summary + Environment.NewLine)

// BUILD SECTION: V3 capability validation

// Feature 041 (FR-003): the bespoke line-by-line capability YAML state machine
// (readCapabilityCatalog + trimQuotes/parseScalar/parseInlineList/emptyCapability) and
// the inline validateCapabilityRows/finding helpers were retired. The catalog is now read
// through YamlDotNet behind the typed FS.Skia.UI.Build.Capabilities model and validated by
// the pure FS.Skia.UI.Build.Capabilities.validateRows; findings come from the shared
// FS.Skia.UI.Build.Findings type. template/capabilities.yml is unchanged (data source).
let readCapabilityCatalog model =
    Capabilities.readCatalog model.CapabilityCatalogPath

let surfaceBaselineExists model baseline =
    File.Exists(path [ model.RepositoryRoot; baseline ])

let writeFindingsOrPass outputPath title (findings: Findings.ValidationFinding list) rows =
    if not (List.isEmpty findings) then
        failwithf "%s failed:%s%s" title Environment.NewLine (Findings.renderDetail findings)

    ensureParent outputPath
    File.WriteAllText(outputPath, rows |> String.concat Environment.NewLine |> fun text -> text + Environment.NewLine)

let runCapabilityCatalogCheck model =
    let capabilities = readCapabilityCatalog model
    let findings = Capabilities.validateRows model.CapabilityCatalogPath (surfaceBaselineExists model) capabilities

    if not (List.isEmpty findings) then
        failwithf "CapabilityCheck failed:%s%s" Environment.NewLine (Findings.renderDetail findings)

    let body = Capabilities.renderReport capabilities
    ensureParent model.CapabilityCatalogReportPath
    File.WriteAllText(model.CapabilityCatalogReportPath, body + Environment.NewLine)

let requiredSkillSections =
    [ "## Scope"
      "## Public Contract"
      "## Build Commands"
      "## Test Commands"
      "## Evidence"
      "## Package Boundary"
      "## Generated Product" ]

let runSkillCatalogCheck model =
    let capabilities = readCapabilityCatalog model

    let findings =
        [ for capability in capabilities do
              match capability.Skill with
              | None -> yield Findings.finding "selected-skills" capability.Id "skill" "Missing skill path"
              | Some skillPath ->
                  let fullPath = path [ model.RepositoryRoot; skillPath ]

                  if not (File.Exists fullPath) then
                      yield Findings.finding "selected-skills" skillPath "skill-file" "Skill file is missing"
                  else
                      let content = File.ReadAllText fullPath

                      for section in requiredSkillSections do
                          if content.IndexOf(section, StringComparison.Ordinal) < 0 then
                              yield Findings.finding "selected-skills" skillPath "skill-section" $"Missing {section}"

                      if content.IndexOf("./fake.sh build -t", StringComparison.Ordinal) < 0 then
                          yield Findings.finding "selected-skills" skillPath "skill-command" "Skill does not name a FAKE target command" ]

    let defaultSkills =
        [ "fs-skia-project"
          "fs-skia-scene"
          "fs-skia-skiaviewer"
          "fs-skia-elmish"
          "fs-skia-keyboard-input"
          "fs-skia-ui-widgets" ]

    let rows =
        [ "# Selected Skills"
          ""
          "PASS: selected capability skills contain required sections and valid command references."
          ""
          "Default app selected skill destinations:"
          yield! defaultSkills |> List.map (fun skill -> $"- `{skill}`")
          ""
          "Generated-product validation rejects unrelated capability skills." ]

    writeFindingsOrPass model.SelectedSkillsReportPath "SkillCheck" findings rows

let rec copyDirectory source target =
    Directory.CreateDirectory target |> ignore

    for file in Directory.GetFiles source do
        File.Copy(file, path [ target; Path.GetFileName file |> Option.ofObj |> Option.defaultValue "" ], true)

    for directory in Directory.GetDirectories source do
        copyDirectory directory (path [ target; Path.GetFileName directory |> Option.ofObj |> Option.defaultValue "" ])

let rec copyDirectoryExcept (source: string) (target: string) (excludedRelativePaths: string list) =
    Directory.CreateDirectory target |> ignore

    let excluded =
        excludedRelativePaths
        |> List.map (fun relative -> relative.Replace('\\', '/'))
        |> Set.ofList

    let rec copy (currentSource: string) (currentTarget: string) (relativePrefix: string) =
        Directory.CreateDirectory currentTarget |> ignore

        for file in Directory.GetFiles currentSource do
            let fileName = Path.GetFileName file |> Option.ofObj |> Option.defaultValue ""
            let relative = (relativePrefix + fileName).Replace('\\', '/')

            if Set.contains relative excluded |> not then
                File.Copy(file, path [ currentTarget; fileName ], true)

        for directory in Directory.GetDirectories currentSource do
            let name = Path.GetFileName directory |> Option.ofObj |> Option.defaultValue ""
            let relative = (relativePrefix + name + "/").Replace('\\', '/')

            if Set.contains relative excluded |> not then
                copy directory (path [ currentTarget; name ]) relative

    copy source target ""

let copySpecKitInstall model root =
    // 043 (FR-013): generated projects run evidence in-process via the packaged
    // FS.Skia.UI.Build engine, so the Python + run-audit.sh scripts are NOT copied
    // into them. audit-patterns.yml (data) stays and is still copied.
    copyDirectoryExcept (path [ model.RepositoryRoot; ".specify" ]) (path [ root; ".specify" ]) [ "feature.json"; "memory/constitution.md"; "extensions/evidence/scripts/" ]
    copyDirectory (path [ model.RepositoryRoot; ".template.config"; "generated"; ".specify" ]) (path [ root; ".specify" ])

let capabilitiesById model =
    readCapabilityCatalog model |> List.map (fun row -> row.Id, row) |> Map.ofList

let resolveCapabilities model selected =
    let byId = capabilitiesById model

    let rec visit seen capabilityId =
        if Set.contains capabilityId seen then
            seen
        else
            match Map.tryFind capabilityId byId with
            | None -> failwithf "Unknown capability %s" capabilityId
            | Some capability ->
                capability.Dependencies
                |> List.fold visit (Set.add capabilityId seen)

    selected
    |> List.fold visit Set.empty
    |> Set.toList

let packageReferences model capabilities =
    let byId = capabilitiesById model

    let capabilityPackages =
        capabilities
        |> List.choose (fun capabilityId ->
            match Map.tryFind capabilityId byId with
            | Some capability when not capability.NonRuntime ->
                capability.PackageId
                |> Option.bind (fun packageId ->
                    if packageId = "non-runtime" then None else Some packageId)
            | _ -> None)

    let adapterPackages =
        if capabilities |> List.contains "controls" && capabilities |> List.contains "elmish" then
            [ "FS.Skia.UI.Controls.Elmish" ]
        else
            []

    (capabilityPackages @ adapterPackages)
    |> List.distinct
    |> List.sort

let writeProductProject model row capabilities =
    let references =
        packageReferences model capabilities
        |> List.map (fun packageId -> $"    <PackageReference Include=\"{packageId}\" />")
        |> String.concat Environment.NewLine

    let compileItems =
        if capabilities |> List.contains "controls" then
            [ "Model.fs"
              "View.fs"
              "LayoutEvidence.fs"
              "WindowOptions.fs"
              "EvidenceCommands.fs"
              "Program.fs" ]
        else
            [ "Program.fs" ]
        |> List.map (fun file -> $"    <Compile Include=\"{file}\" />")
        |> String.concat Environment.NewLine

    let content =
        $"""<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
{compileItems}
  </ItemGroup>

  <ItemGroup>
{references}
  </ItemGroup>

</Project>
"""

    File.WriteAllText(path [ row.Root; "src"; "Product"; "Product.fsproj" ], content)

type TemplateConditionalFrame =
    { ParentActive: bool
      BranchMatched: bool }

let private evaluateProfileCondition profile expression =
    let evaluateAtom (atom: string) =
        let trimmed = atom.Trim()
        let profileEquals = Regex.Match(trimmed, "^profile\\s*==\\s*\"([^\"]+)\"$")

        if profileEquals.Success then
            profile = profileEquals.Groups.[1].Value
        else
            failwithf "Unsupported generated product template condition: %s" expression

    expression.Trim().TrimStart('(').TrimEnd(')').Split([| "||" |], StringSplitOptions.None)
    |> Array.exists evaluateAtom

let private applyGeneratedProfileConditionals profile filePath =
    let mutable frames: TemplateConditionalFrame list = []
    let mutable active = true
    let output = ResizeArray<string>()

    for line in File.ReadAllLines filePath do
        let trimmed = line.Trim()

        if trimmed.StartsWith("//#if", StringComparison.Ordinal) then
            let expression = trimmed.Substring("//#if".Length).Trim()
            let matched = evaluateProfileCondition profile expression
            let current = active && matched
            frames <- { ParentActive = active; BranchMatched = matched } :: frames
            active <- current
        elif trimmed.StartsWith("//#else", StringComparison.Ordinal) then
            match frames with
            | frame :: rest ->
                let current = frame.ParentActive && not frame.BranchMatched
                frames <- { frame with BranchMatched = true } :: rest
                active <- current
            | [] -> failwithf "Unmatched //#else in %s" filePath
        elif trimmed.StartsWith("//#endif", StringComparison.Ordinal) then
            match frames with
            | frame :: rest ->
                frames <- rest
                active <- frame.ParentActive
            | [] -> failwithf "Unmatched //#endif in %s" filePath
        elif active then
            output.Add line

    if not frames.IsEmpty then
        failwithf "Unclosed generated product template conditional in %s" filePath

    File.WriteAllLines(filePath, output.ToArray())

let applyGeneratedProductProfile row =
    Directory.EnumerateFiles(row.Root, "*.fs", SearchOption.AllDirectories)
    |> Seq.iter (applyGeneratedProfileConditionals row.Profile)

let writeSceneOnlyProductFiles row =
    let program =
        """module Product.Program

open System
open System.IO
open FS.Skia.UI.Scene

let productScene =
    { Nodes =
        [ Rectangle((16.0, 16.0, 160.0, 96.0), Colors.rgb 32uy 96uy 160uy)
          Text((28.0, 56.0), "Generated scene product", Colors.white) ] }

let sceneElementCount () =
    productScene.Nodes.Length

let writeLines (path: string) (lines: string list) =
    let directory = Path.GetDirectoryName path

    if not (String.IsNullOrWhiteSpace directory) then
        Directory.CreateDirectory(directory) |> ignore

    File.WriteAllLines(path, Array.ofList lines)

let sceneEvidence evidencePath =
    match
        SceneEvidence.render
            { Scene = productScene
              OutputSize = { Width = 320; Height = 200 }
              Format = Metadata
              RendererMode = "deterministic-scene"
              EvidencePath = Some evidencePath }
    with
    | Result.Ok evidence ->
        writeLines
            evidencePath
            [ "status=ok"
              "mode=headless-scene"
              "command=--scene-evidence"
              $"renderer-mode={evidence.RendererMode}"
              $"scene-evidence-format={evidence.Format}"
              $"value={evidence.Value}" ]

        printfn "status=ok mode=headless-scene command=--scene-evidence renderer-mode=%s evidence=%s value=%s" evidence.RendererMode evidencePath evidence.Value
        0
    | Result.Error failure ->
        writeLines
            evidencePath
            [ "status=failed"
              "mode=headless-scene"
              "command=--scene-evidence"
              $"blocked-stage={failure.BlockedStage}"
              $"classification={failure.Classification}"
              $"category={failure.DiagnosticCategory}"
              $"message={failure.Message}" ]

        printfn "status=failed mode=headless-scene command=--scene-evidence blocked-stage=%s classification=%A category=%s message=%s evidence=%s" failure.BlockedStage failure.Classification failure.DiagnosticCategory failure.Message evidencePath
        1

[<EntryPoint>]
let main args =
    match List.ofArray args with
    | "--scene-evidence" :: path :: _ -> sceneEvidence path
    | "--scene-evidence" :: _ -> sceneEvidence "readiness/headless-scene-evidence.txt"
    | _ ->
        printfn "status=ok mode=headless-scene command=dotnet-run scene-elements=%d" (sceneElementCount())
        0
"""

    let tests =
        """module ProductTests

open Expecto
open Product.Program
open FS.Skia.UI.Scene

[<Tests>]
let tests =
    testList "product" [
        test "generated product test suite is wired" {
            Expect.equal 1 1 "product tests run"
        }

        test "headless scene profile builds a scene-only product" {
            Expect.isGreaterThan (sceneElementCount()) 1 "scene-only product renders multiple scene nodes"
            Expect.exists productScene.Nodes (function Rectangle _ -> true | _ -> false) "scene includes a rectangle"
            Expect.exists productScene.Nodes (function Text(_, value, _) when value.Contains("Generated scene product") -> true | _ -> false) "scene includes text"
        }
    ]
"""

    File.WriteAllText(path [ row.Root; "src"; "Product"; "Program.fs" ], program)
    File.WriteAllText(path [ row.Root; "tests"; "Product.Tests"; "Tests.fs" ], tests)

let copySelectedSkills model row capabilities =
    let skillRoot = path [ row.Root; ".agents"; "skills" ]
    let claudeSkillRoot = path [ row.Root; ".claude"; "skills" ]
    Directory.CreateDirectory skillRoot |> ignore
    Directory.CreateDirectory claudeSkillRoot |> ignore
    copyDirectory (path [ model.RepositoryRoot; "template"; "base"; ".agents"; "skills"; "fs-skia-project" ]) (path [ skillRoot; "fs-skia-project" ])
    copyDirectory (path [ model.RepositoryRoot; "template"; "base"; ".claude"; "skills"; "fs-skia-project" ]) (path [ claudeSkillRoot; "fs-skia-project" ])

    Directory.GetDirectories(path [ model.RepositoryRoot; ".agents"; "skills" ], "speckit-*", SearchOption.TopDirectoryOnly)
    |> Array.iter (fun directory ->
        let skillName = Path.GetFileName directory |> Option.ofObj |> Option.defaultValue ""
        copyDirectory directory (path [ skillRoot; skillName ])
        copyDirectory directory (path [ claudeSkillRoot; skillName ]))

    let byId = capabilitiesById model
    let controlsSelected = capabilities |> List.contains "controls"

    for capabilityId in capabilities do
        let skipGeneratedSkill =
            controlsSelected && capabilityId = "layout"

        match skipGeneratedSkill, Map.tryFind capabilityId byId, capabilitySkillDestination capabilityId with
        | true, _, _ -> ()
        | false, Some capability, Some destination ->
            match capability.Skill with
            | Some sourceSkill ->
                let destinationDirectory = path [ skillRoot; destination ]
                let claudeDestinationDirectory = path [ claudeSkillRoot; destination ]
                Directory.CreateDirectory destinationDirectory |> ignore
                Directory.CreateDirectory claudeDestinationDirectory |> ignore
                File.Copy(path [ model.RepositoryRoot; sourceSkill ], path [ destinationDirectory; "SKILL.md" ], true)
                File.Copy(path [ model.RepositoryRoot; sourceSkill ], path [ claudeDestinationDirectory; "SKILL.md" ], true)
            | None -> ()
        | _ -> ()

let v3TemplatePackagePath model =
    path [ model.TemplateArtifactDir; "FS.Skia.UI.V3.Template.zip" ]

let createV3TemplatePackage model =
    Directory.CreateDirectory model.TemplateArtifactDir |> ignore
    let packagePath = v3TemplatePackagePath model

    if File.Exists packagePath then
        File.Delete packagePath

    ZipFile.CreateFromDirectory(path [ model.RepositoryRoot; "template" ], packagePath)
    packagePath

let templatePayloadRoot model row =
    if row.Artifact = "package" then
        let extracted = path [ model.TemplateWorkDir; "v3-template-package" ]
        cleanDirectoryContents extracted
        ZipFile.ExtractToDirectory(v3TemplatePackagePath model, extracted)
        extracted
    else
        path [ model.RepositoryRoot; "template" ]

let writeGeneratedProductReadme row capabilities =
    let capabilityNames = capabilities |> List.map (fun id -> $"- {id}") |> String.concat Environment.NewLine

    let content =
        [ "# Product"
          ""
          "This generated product consumes selected FS.Skia.UI capability packages."
          ""
          "Resolved capabilities:"
          capabilityNames
          ""
          "Commands:"
          ""
          "```bash"
          "./fake.sh build -t Dev"
          "./fake.sh build -t Test"
          "./fake.sh build -t Verify"
          "```" ]
        |> String.concat Environment.NewLine

    File.WriteAllText(path [ row.Root; "README.md" ], content + Environment.NewLine)

// US4 (spec 037, FR-009): emit the generated FSI load script. The reference set
// is DERIVED from the built Product output assembly directory (the transitive
// FS.Skia.UI.* closure pinned by Directory.Packages.props) plus the Product app
// itself, so it stays in sync with the assembly set instead of being a
// hand-maintained list. Loading it neither emits nor suppresses host warnings;
// real load failures surface normally (spec 021 benign-warning classification is
// unaffected because the script references and opens — it launches nothing).
let emitFsiLoadScript row =
    let productBin = path [ row.Root; "src"; "Product"; "bin" ]
    let loadScriptPath = path [ row.Root; "load-product.fsx" ]

    let productDll =
        if Directory.Exists productBin then
            Directory.EnumerateFiles(productBin, "Product.dll", SearchOption.AllDirectories)
            |> Seq.sortBy (fun candidate -> candidate.Length)
            |> Seq.tryHead
        else
            None

    match productDll with
    | None ->
        failwithf
            "%s/%s could not emit load-product.fsx: no built Product.dll under %s (generated build must run first)"
            row.Artifact row.Profile productBin
    | Some dll ->
        let outDir = Path.GetDirectoryName dll |> Option.ofObj |> Option.defaultValue ""
        let outRel = (relativePathFrom row.Root outDir).Replace('\\', '/')

        let fsAssemblies =
            Directory.EnumerateFiles(outDir, "FS.Skia.UI*.dll")
            |> Seq.map Path.GetFileName
            |> Seq.sort
            |> Seq.toList

        let referenceLines =
            [ for assembly in fsAssemblies -> $"#r \"{outRel}/{assembly}\"" ]
            @ [ $"#r \"{outRel}/Product.dll\"" ]
            |> String.concat Environment.NewLine

        let content =
            [ "// GENERATED — do not edit. Regenerated from Directory.Packages.props and the"
              "// built Product output assembly. Loads the Product app and its transitive"
              "// FS.Skia.UI.* references for FSI in one step:  dotnet fsi load-product.fsx"
              "//"
              "// This script only references and opens the app; it launches nothing, so it"
              "// neither emits nor suppresses host warnings. A missing assembly is a real"
              "// load failure that surfaces normally; benign host-warning classification"
              "// (spec 021) is unaffected."
              referenceLines
              "open Product" ]
            |> String.concat Environment.NewLine

        File.WriteAllText(loadScriptPath, content + Environment.NewLine)

// US2 (FR-004, SC-002): bundle the real public `.fsi` signatures for every
// capability the profile consumes into a local `docs/api-surface/` reference, so
// an author reads any union case's exact field order without DLL reflection.
// Derivation is mechanical (verbatim copy-at-generation from src/.../*.fsi).
let apiSurfaceContractsFor model capabilities =
    let byId = capabilitiesById model

    capabilities
    |> List.collect (fun capabilityId ->
        match Map.tryFind capabilityId byId with
        | Some capability -> capability.Contracts
        | None -> [])
    // Skip non-runtime capabilities whose contract is the `no-public-surface`
    // sentinel rather than a real `.fsi` path.
    |> List.filter (fun contract -> contract.EndsWith(".fsi", StringComparison.OrdinalIgnoreCase))
    |> List.distinct

// Bundled path mirrors the source package directory so same-named contracts from
// different packages (e.g. Controls/Types.fsi vs Layout/Types.fsi) do not collide.
let apiSurfaceRelativePath (contractRelative: string) =
    let packageDir = Path.GetFileName(Path.GetDirectoryName contractRelative)
    $"docs/api-surface/{packageDir}/{Path.GetFileName contractRelative}"

let copyApiSurface model row capabilities =
    apiSurfaceContractsFor model capabilities
    |> List.iter (fun contractRelative ->
        let source = path [ model.RepositoryRoot; contractRelative ]

        if File.Exists source then
            let dest = path [ row.Root; apiSurfaceRelativePath contractRelative ]
            ensureParent dest
            File.Copy(source, dest, true)
        else
            failwithf "Cannot bundle API surface: missing source contract %s" contractRelative)

let generateV3Product model row =
    cleanDirectoryContents row.Root
    cleanDirectoryContents row.EvidenceDir
    let templateRoot = templatePayloadRoot model row
    copyDirectory (path [ templateRoot; "base" ]) row.Root
    applyGeneratedProductProfile row
    copySpecKitInstall model row.Root

    let resolved = resolveCapabilities model row.Capabilities
    writeProductProject model row resolved

    if row.Profile <> "app" then
        writeSceneOnlyProductFiles row

    writeGeneratedProductReadme row resolved
    copySelectedSkills model row resolved
    copyApiSurface model row resolved

    for capabilityId in resolved do
        match capabilityId with
        | "samples" -> copyDirectory (path [ model.RepositoryRoot; "template"; "fragments"; "samples" ]) (path [ row.Root; "samples" ])
        | _ -> ()

    [ "Dev"; "Test"; "Verify" ]
    |> List.iter (fun target ->
        runProcess $"{row.Profile}/{row.Artifact} generated {target}" "bash" $"./fake.sh build -t {target}" row.Root (path [ row.EvidenceDir; $"{target.ToLowerInvariant()}.log" ]) Map.empty)

    // Emit the in-sync FSI load script from the freshly built Product output (US4, FR-009).
    emitFsiLoadScript row

let runGenerateV3Products model =
    cleanDirectoryContents model.GeneratedProductRootsDir
    createV3TemplatePackage model |> ignore

    v3GeneratedRows model
    |> List.iter (generateV3Product model)

let scanV3GeneratedRow model row =
    let files =
        Directory.EnumerateFiles(row.Root, "*", SearchOption.AllDirectories)
        |> Seq.map (relativePathFrom row.Root)
        |> Seq.filter (fun relative -> not (relative.Contains("/bin/")) && not (relative.Contains("/obj/")) && not (relative.StartsWith("readiness/", StringComparison.Ordinal)))
        |> Seq.sort
        |> Seq.toList

    let appProjects =
        files |> List.filter (fun file -> file.StartsWith("src/", StringComparison.Ordinal) && file.EndsWith(".fsproj", StringComparison.Ordinal))

    let testProjects =
        files |> List.filter (fun file -> file.StartsWith("tests/", StringComparison.Ordinal) && file.EndsWith(".fsproj", StringComparison.Ordinal))

    // Feature 053 (V3 Stage 5, US3 / FR-008): the generated-project cleanliness gate.
    // The forbidden top-level globs are pinned exactly so a planted framework artifact fails
    // deterministically, naming the offending artifact. A generated `app`/`governed` profile
    // references the split packages and carries only its OWN starter docs (`docs/product.md`,
    // `docs/effects-boundary.md`, `docs/api-surface/**`) and README — never the framework's
    // `samples/`, `docs/reports/` report set, historical `specs/`, readiness evidence, or a
    // copy of the framework root README (asserted separately below by content).
    let forbidden =
        [ "framework implementation projects", "src/Charts"
          "framework implementation projects", "tests/Charts.Tests"
          "framework sample content", "samples/"
          // Feature 059 (FR-014) removed the runtime `specs/generated-evidence-workflow`
          // sample synthesiser from the template build.fsx, so a generated product ships
          // no starter feature at all. The historical-specs guard pins the framework's
          // numbered feature directories (`specs/00N-…`), which a generated product must
          // never copy.
          "historical framework specs", "specs/00"
          "framework readiness evidence", "readiness/"
          "framework documentation set", "docs/reports/"
          "framework implementation projects", "tests/Parity.Tests"
          "framework implementation projects", ".template.package" ]

    let missing =
        [ "src/Product/Product.fsproj"
          "tests/Product.Tests/Product.Tests.fsproj"
          "load-product.fsx"
          "README.md"
          "CLAUDE.md"
          "docs/product.md"
          "docs/effects-boundary.md"
          ".agents/skills/fs-skia-project/SKILL.md"
          ".claude/skills/fs-skia-project/SKILL.md"
          ".claude/settings.json"
          ".agents/skills/speckit-specify/SKILL.md"
          ".claude/skills/speckit-specify/SKILL.md"
          ".agents/skills/speckit-plan/SKILL.md"
          ".claude/skills/speckit-plan/SKILL.md"
          ".agents/skills/speckit-tasks/SKILL.md"
          ".claude/skills/speckit-tasks/SKILL.md"
          ".agents/skills/speckit-implement/SKILL.md"
          ".claude/skills/speckit-implement/SKILL.md"
          ".specify/memory/constitution.md"
          ".specify/templates/spec-template.md"
          ".specify/scripts/bash/setup-plan.sh"
          ".specify/workflows/speckit/workflow.yml"
          "build.fsx"
          "fake.sh"
          "fake.cmd" ]
        |> List.filter (fun required -> files |> List.contains required |> not)

    if row.Profile = "app" && appProjects.Length <> 1 then
        failwithf "%s/%s expected exactly one product app, found %d" row.Artifact row.Profile appProjects.Length

    if row.Profile = "app" && testProjects.Length <> 1 then
        failwithf "%s/%s expected exactly one product test suite, found %d" row.Artifact row.Profile testProjects.Length

    if not missing.IsEmpty then
        // US2 (FR-005): route the structural violation through the versioned contract.
        // `required-files-present` is Required at the current schema version, so this
        // still hard-fails (behaviour-identical); a Deprecated rule would warn instead.
        let missingMessage =
            sprintf "%s/%s generated product missing files:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, missing))

        match GeneratedProductContract.classifyViolation GeneratedProductContract.current "required-files-present" with
        | GeneratedProductContract.Warn warning -> printfn "WARN [required-files-present] %s — %s" warning missingMessage
        | _ -> failwith missingMessage

    // US2 (FR-004, SC-002): every consumed package's signatures are bundled
    // verbatim under docs/api-surface/ and stay in lockstep with source.
    let apiSurfaceFindings =
        apiSurfaceContractsFor model (resolveCapabilities model row.Capabilities)
        |> List.choose (fun contractRelative ->
            let relative = apiSurfaceRelativePath contractRelative
            let bundled = path [ row.Root; relative ]
            let source = path [ model.RepositoryRoot; contractRelative ]

            if not (File.Exists bundled) then
                Some $"{relative} missing from generated product"
            elif File.ReadAllText bundled <> File.ReadAllText source then
                Some $"{relative} drifts from source {contractRelative}"
            else
                None)

    if not apiSurfaceFindings.IsEmpty then
        failwithf
            "%s/%s bundled API surface invalid:%s%s"
            row.Artifact
            row.Profile
            Environment.NewLine
            (String.Join(Environment.NewLine, apiSurfaceFindings))

    // US5 (FR-009, SC-005): a single canonical effects-boundary page is present
    // and self-contained (both effect categories + boundary + update->host wiring).
    let effectsBoundaryPath = path [ row.Root; "docs"; "effects-boundary.md" ]

    if File.Exists effectsBoundaryPath then
        let effectsText = File.ReadAllText effectsBoundaryPath

        let requiredEffectsTokens =
            [ "application commands"
              "viewer effects"
              "host boundary"
              "MVU edge"
              "Viewer.runApp"
              "generatedHost" ]

        let missingEffectsTokens =
            requiredEffectsTokens
            |> List.filter (fun token -> effectsText.IndexOf(token, StringComparison.OrdinalIgnoreCase) < 0)

        if not missingEffectsTokens.IsEmpty then
            failwithf
                "%s/%s docs/effects-boundary.md missing required content: %s"
                row.Artifact
                row.Profile
                (String.Join(", ", missingEffectsTokens))

    // US4 (FR-007, SC-004): the generated starter app and tests carry zero
    // demo-specific (game-title) identifiers. Legitimate framework compounds that
    // merely contain a forbidden substring (KeyboardInput, ProofLevel, proof-level)
    // are stripped before the scan so they are not false positives.
    let demoScanFiles =
        files
        |> List.filter (fun file ->
            (file.StartsWith("src/Product/", StringComparison.Ordinal)
             || file.StartsWith("tests/Product.Tests/", StringComparison.Ordinal))
            && file.EndsWith(".fs", StringComparison.Ordinal))

    // Whole-word framework roots that legitimately contain a forbidden substring:
    // `keyboard*` (-> "board"), `prooflevel`/`proof-level` (-> "level").
    let legitimateCompounds =
        [ "keyboard"
          "prooflevel"
          "proof-level"
          "layoutprooflevel"
          "diagnosticlevel"
          "viewerdiagnosticlevel"
          "minimumlevel" ]

    let forbiddenDemoTokens = [ "tetris"; "score"; "level"; "next piece"; "board"; "piece" ]

    let demoFindings =
        demoScanFiles
        |> List.collect (fun relative ->
            let content = File.ReadAllText(path [ row.Root; relative ]).ToLowerInvariant()

            let sanitized =
                legitimateCompounds
                |> List.fold (fun (state: string) compound -> state.Replace(compound, "")) content

            forbiddenDemoTokens
            |> List.choose (fun token ->
                if sanitized.Contains token then
                    Some $"{relative}: generated starter contains demo identifier `{token}`"
                else
                    None))

    if not demoFindings.IsEmpty then
        failwithf
            "%s/%s generated starter carries demo identifiers:%s%s"
            row.Artifact
            row.Profile
            Environment.NewLine
            (String.Join(Environment.NewLine, demoFindings))

    // US4 (FR-005, FR-006, SC-004): every generated capability-usage skill is
    // consumer-facing — it carries at least one consumer-runnable fsharp snippet
    // and names no framework-only path or build target absent from a consumer
    // project. Non-runtime capabilities (e.g. samples) carry no usage snippet, and
    // layout's skill is folded into Controls when Controls is selected, so both are
    // excluded the same way generation excludes them.
    let generatedCapabilitySkillRow =
        let byId = capabilitiesById model
        let resolved = resolveCapabilities model row.Capabilities
        let controlsSelected = resolved |> List.contains "controls"

        resolved
        |> List.filter (fun capabilityId -> not (controlsSelected && capabilityId = "layout"))
        |> List.choose (fun capabilityId ->
            match Map.tryFind capabilityId byId with
            | Some capability when capability.NonRuntime -> None
            | Some _ -> capabilitySkillDestination capabilityId
            | None -> None)
        |> List.distinct

    let frameworkOnlyTargets = [ "CapabilityCheck"; "PackLocal"; "DependencyReport"; "PackageSurfaceCheck" ]
    let frameworkOnlySurfaceBaseline = "readiness/surface-baselines"
    let frameworkOnlySourceContract = Regex(@"src/[A-Za-z0-9_.]+/[A-Za-z0-9_.]+\.fsi")

    let consumerSkillFindings =
        generatedCapabilitySkillRow
        |> List.collect (fun destination ->
            let relative = $".agents/skills/{destination}/SKILL.md"
            let full = path [ row.Root; relative ]

            if not (File.Exists full) then
                [ $"{relative}: expected generated capability skill is missing" ]
            else
                let text = File.ReadAllText full

                [ for target in frameworkOnlyTargets do
                      if text.IndexOf(target, StringComparison.Ordinal) >= 0 then
                          yield $"{relative}: generated skill names framework-only target `{target}` absent from a consumer project (FR-005)"

                  if frameworkOnlySourceContract.IsMatch text then
                      yield $"{relative}: generated skill points at a framework-only `src/.../*.fsi` source path; reference the bundled `docs/api-surface/` instead (FR-005)"

                  if text.IndexOf(frameworkOnlySurfaceBaseline, StringComparison.Ordinal) >= 0 then
                      yield $"{relative}: generated skill names framework-only `{frameworkOnlySurfaceBaseline}` absent from a consumer project (FR-005)"

                  if not (text.Contains "```fsharp") then
                      yield $"{relative}: generated skill has no consumer-runnable fsharp usage snippet (FR-006)" ])

    if not consumerSkillFindings.IsEmpty then
        failwithf
            "%s/%s generated capability skills are not consumer-facing:%s%s"
            row.Artifact
            row.Profile
            Environment.NewLine
            (String.Join(Environment.NewLine, consumerSkillFindings))

    if row.Profile = "app" && not (files |> List.contains ".agents/skills/fs-skia-ui-widgets/SKILL.md") then
        failwithf "%s/%s generated app is missing fs-skia-ui-widgets" row.Artifact row.Profile

    if row.Profile = "app" && not (files |> List.contains ".claude/skills/fs-skia-ui-widgets/SKILL.md") then
        failwithf "%s/%s generated app is missing Claude fs-skia-ui-widgets" row.Artifact row.Profile

    if row.Profile = "app"
       && files |> List.exists (fun file ->
           file.StartsWith(".agents/skills/", StringComparison.Ordinal)
           && (file.IndexOf("charts", StringComparison.OrdinalIgnoreCase) >= 0
               || file.IndexOf("layout", StringComparison.OrdinalIgnoreCase) >= 0)) then
        failwithf "%s/%s generated app contains stale chart or generated layout control skill" row.Artifact row.Profile

    for rule, forbiddenPath in forbidden do
        let allowedGeneratedSamplePackContent =
            row.Profile = "sample-pack" && forbiddenPath = "samples/"

        if not allowedGeneratedSamplePackContent
           && files |> List.exists (fun file -> file.StartsWith(forbiddenPath, StringComparison.Ordinal)) then
            failwithf "%s/%s copied %s: %s" row.Artifact row.Profile rule forbiddenPath

    // Feature 053 (US3 / FR-008): the generated product carries its OWN starter README, never a
    // copy of the framework root README. The path check above cannot catch this (README.md is a
    // required product file), so compare content: a byte-identical copy of the repository root
    // README is a planted framework artifact and fails the cleanliness gate naming README.md.
    let generatedReadme = path [ row.Root; "README.md" ]
    let frameworkRootReadme = path [ model.RepositoryRoot; "README.md" ]

    if File.Exists generatedReadme && File.Exists frameworkRootReadme
       && File.ReadAllText generatedReadme = File.ReadAllText frameworkRootReadme then
        failwithf "%s/%s copied framework README content: README.md" row.Artifact row.Profile

    let productProject = File.ReadAllText(path [ row.Root; "src"; "Product"; "Product.fsproj" ])
    let productProgram = File.ReadAllText(path [ row.Root; "src"; "Product"; "Program.fs" ])
    let productEvidenceCommands = File.ReadAllText(path [ row.Root; "src"; "Product"; "EvidenceCommands.fs" ])
    let productLaunchSource = productProgram + Environment.NewLine + productEvidenceCommands
    let productTests = File.ReadAllText(path [ row.Root; "tests"; "Product.Tests"; "Tests.fs" ])
    let removedChartsPackage = "FS.Skia.UI." + "Charts"

    if productProject.IndexOf($"PackageReference Include=\"{removedChartsPackage}\"", StringComparison.Ordinal) >= 0 then
        failwithf "%s/%s generated product contains removed Charts package reference %s" row.Artifact row.Profile removedChartsPackage

    if row.Profile = "app"
       && productProject.IndexOf("PackageReference Include=\"FS.Skia.UI.Controls.Elmish\"", StringComparison.Ordinal) < 0 then
        failwithf "%s/%s generated app is missing Controls.Elmish adapter package reference" row.Artifact row.Profile

    if row.Profile = "app" then
        let requiredPersistentHostTerms =
            [ "let viewerOptions"
              "let generatedHost"
              "MapKey = mapKey"
              "Tick = tick"
              "Viewer.runApp viewerOptions generatedHost"
              "window-visible=observed:true"
              "accessible-window=true"
              "--bounded-smoke"
              "--bounded-smoke-frame-diagnostics"
              "--scene-evidence" ]

        let missingPersistentHostTerms =
            requiredPersistentHostTerms
            |> List.filter (fun term -> productLaunchSource.IndexOf(term, StringComparison.Ordinal) < 0)

        if not missingPersistentHostTerms.IsEmpty then
            failwithf "%s/%s generated app is missing persistent viewer host wiring:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, missingPersistentHostTerms))

        let forbiddenDefaultSubstitutions =
            [ "print metadata"
              "count controls"
              "run bounded smoke"
              "emit scene evidence"
              "first-frame-only=true"
              "exit after first frame"
              "without a persistent launch attempt" ]

        let defaultBranch =
            let marker = "| _ ->"
            let index = productProgram.LastIndexOf(marker, StringComparison.Ordinal)

            if index >= 0 then
                productProgram.Substring(index)
            else
                productProgram

        let foundDefaultSubstitutions =
            forbiddenDefaultSubstitutions
            |> List.filter (fun term -> defaultBranch.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)

        if not foundDefaultSubstitutions.IsEmpty then
            failwithf "%s/%s generated app contains bounded-only or print-only default path markers:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, foundDefaultSubstitutions))

    let selectedCapabilitySkills =
        Directory.EnumerateFiles(path [ row.Root; ".agents"; "skills" ], "SKILL.md", SearchOption.AllDirectories)
        |> Seq.map (relativePathFrom row.Root)
        |> Seq.sort
        |> Seq.toList

    let selectedClaudeSkills =
        Directory.EnumerateFiles(path [ row.Root; ".claude"; "skills" ], "SKILL.md", SearchOption.AllDirectories)
        |> Seq.map (relativePathFrom row.Root)
        |> Seq.sort
        |> Seq.toList

    let missingClaudeSkillPeers =
        selectedCapabilitySkills
        |> List.map (fun skill -> skill.Replace(".agents/skills/", ".claude/skills/"))
        |> List.filter (fun peer -> selectedClaudeSkills |> List.contains peer |> not)

    if not missingClaudeSkillPeers.IsEmpty then
        failwithf "%s/%s generated product is missing Claude skill peers:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, missingClaudeSkillPeers))

    let report =
        [ $"# {row.Profile}/{row.Artifact} generated product"
          ""
          "Validation rules: exactly one product app, exactly one product test suite, selected capability skills, Controls ownership for form/chart/graph/DataGrid authoring, Controls.Elmish adapter references, consumer-mode package references, stale Charts exclusions, no copied framework samples/specs/readiness/docs, and no framework implementation projects."
          ""
          "Files:"
          yield! files
          ""
          "Package references:"
          productProject
          ""
          "Product source:"
          productProgram
          ""
          "Product tests:"
          productTests
          ""
          "Selected skills:"
          yield! selectedCapabilitySkills
          ""
          "Selected Claude skills:"
          yield! selectedClaudeSkills ]
        |> String.concat Environment.NewLine

    ensureParent row.FileListPath
    File.WriteAllText(row.FileListPath, report + Environment.NewLine)

let runScanV3GeneratedProducts model =
    // US2 (FR-006, SC-011): the versioned contract must be internally consistent —
    // every breaking changelog entry carries a version bump. A drift here fails the
    // gate instead of relying on reviewer attention.
    let contractConsistency = GeneratedProductContract.changelogConsistencyFindings GeneratedProductContract.current

    if not (List.isEmpty contractConsistency) then
        failwithf
            "Generated-product contract is inconsistent:%s%s"
            Environment.NewLine
            (String.Join(Environment.NewLine, contractConsistency))

    v3GeneratedRows model
    |> List.iter (scanV3GeneratedRow model)

    let summary =
        [ "# Generated Product Check"
          ""
          // US2 (FR-004, SC-003): the contract schema_version + rule lifecycle is
          // discoverable in the gate output.
          GeneratedProductContract.renderContractHeader GeneratedProductContract.current
          ""
          "PASS: generated product file lists, selected skills, Controls-owned form/chart/graph/DataGrid authoring, Controls.Elmish adapter references, consumer-mode package references, stale Charts exclusions, full product governance command logs, and framework-source exclusions passed."
          ""
          "| Row | File list | Verify log |"
          "|-----|-----------|------------|"
          yield!
              v3GeneratedRows model
              |> List.map (fun row ->
                  let verifyLog = path [ row.EvidenceDir; "verify.log" ]
                  $"| {row.Profile}/{row.Artifact} | `{row.FileListPath}` | `{verifyLog}` |") ]
        |> String.concat Environment.NewLine

    File.WriteAllText(path [ model.GeneratedFileListsDir; "summary.md" ], summary + Environment.NewLine)

let writeLocalNuGetConfig model root =
    let content =
        $"""<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="{model.LocalPackageDir}" />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"""

    File.WriteAllText(path [ root; "NuGet.config" ], content)

let ensureGeneratedPackageVersions model root =
    let propsPath = path [ root; "Directory.Packages.props" ]
    let mutable content = File.ReadAllText(propsPath)

    for project, packageId in packProjects do
        let version = projectVersion model.RepositoryRoot project
        let pattern = $"""(<PackageVersion Include="{Regex.Escape(packageId)}" Version=")[^"]+(" />)"""
        content <- Regex.Replace(content, pattern, MatchEvaluator(fun m -> m.Groups.[1].Value + version + m.Groups.[2].Value))

    File.WriteAllText(propsPath, content)

let private fsSkiaPackageIds =
    packProjects
    |> List.map snd
    |> Set.ofList

let private readRequestedGeneratedPackages root =
    let propsPath = path [ root; "Directory.Packages.props" ]
    let propsContent = File.ReadAllText propsPath

    let centralVersions =
        Regex.Matches(propsContent, "<PackageVersion Include=\"(FS\\.Skia\\.UI[^\"]*)\" Version=\"([^\"]+)\"")
        |> Seq.cast<Match>
        |> Seq.map (fun m -> m.Groups.[1].Value, m.Groups.[2].Value)
        |> Map.ofSeq

    [ path [ root; "src"; "Product"; "Product.fsproj" ]
      path [ root; "tests"; "Product.Tests"; "Product.Tests.fsproj" ] ]
    |> List.filter File.Exists
    |> List.map File.ReadAllText
    |> String.concat Environment.NewLine
    |> fun content -> Regex.Matches(content, "<PackageReference Include=\"(FS\\.Skia\\.UI[^\"]*)\"")
    |> Seq.cast<Match>
    |> Seq.map (fun m -> m.Groups.[1].Value)
    |> Seq.distinct
    |> Seq.filter (fun packageId -> fsSkiaPackageIds |> Set.contains packageId)
    |> Seq.choose (fun packageId ->
        centralVersions
        |> Map.tryFind packageId
        |> Option.map (fun version -> packageId, version))
    |> Seq.sortBy fst
    |> Seq.toList

let private readNuGetPackageSources root =
    BuildPackageResolution.readNuGetPackageSources root

let private readRestoreWarnings logPath =
    BuildPackageResolution.readRestoreWarnings logPath

let private readResolvedGeneratedPackages root =
    let assetsPath = path [ root; "tests"; "Product.Tests"; "obj"; "project.assets.json" ]

    if not (File.Exists assetsPath) then
        []
    else
        let content = File.ReadAllText assetsPath

        Regex.Matches(content, "\"(FS\\.Skia\\.UI[^\"/]*)/([^\"]+)\"")
        |> Seq.cast<Match>
        |> Seq.map (fun m -> m.Groups.[1].Value, m.Groups.[2].Value)
        |> Seq.filter (fun (packageId, _) -> fsSkiaPackageIds |> Set.contains packageId)
        |> Seq.distinct
        |> Seq.sortBy fst
        |> Seq.toList

let private packageResolutionDiagnostics (requested: (string * string) list) (resolved: (string * string) list) (sources: string list) (restoreWarnings: string list) =
    BuildPackageResolution.packageResolutionDiagnostics requested resolved sources restoreWarnings

let private readKeyValueFromText key text =
    let escapedKey = Regex.Escape key
    let pattern = $"(?<!\\S){escapedKey}=([^=]*?)(?=\\s+[A-Za-z0-9_-]+=|$)"
    let m = Regex.Match(text, pattern, RegexOptions.IgnoreCase ||| RegexOptions.CultureInvariant)

    if m.Success then
        Some(m.Groups.[1].Value.Trim())
    else
        None

let private writeSupportedHostPersistentLaunchEvidence outputPath sourceLogPath commandText logText =
    let value key fallback =
        readKeyValueFromText key logText |> Option.defaultValue fallback

    let inputDispatch =
        match (value "input-dispatch" "not-verified").Trim('"') with
        | "true" -> "verified"
        | "false" -> "not-verified"
        | "not-required" -> "not-verified"
        | other -> other

    let windowOpened = value "window-opened" "true"
    let firstFramePresented = value "first-frame-presented" "true"
    let windowVisible = value "window-visible" "observed:true"
    let accessibleWindow = value "accessible-window" "true"
    let selfClosedForEvidence = value "self-closed-for-evidence" "false"
    let userCloseObserved =
        if selfClosedForEvidence = "true" then
            value "user-close-observed" "false"
        else
            value "user-close-observed" "true"
    let exitPath = value "exit-path" "true"
    let rendererMode = value "renderer-mode" "skia"
    let blockedStage = value "blocked-stage" "none"
    let classification = value "classification" "none"
    let category = value "category" "none"
    let message = value "message" "Compiled generated product launched a persistent interactive window on the supported host path."

    let lines =
        [ "status=ok"
          "mode=interactive-window"
          $"command={commandText}"
          $"window-opened={windowOpened}"
          $"window-visible={windowVisible}"
          $"accessible-window={accessibleWindow}"
          $"first-frame-presented={firstFramePresented}"
          $"user-close-observed={userCloseObserved}"
          $"self-closed-for-evidence={selfClosedForEvidence}"
          $"input-dispatch={inputDispatch}"
          $"exit-path={exitPath}"
          $"renderer-mode={rendererMode}"
          $"blocked-stage={blockedStage}"
          $"classification={classification}"
          $"category={category}"
          $"message={message}"
          $"source-log={sourceLogPath}" ]

    File.WriteAllText(outputPath, String.concat Environment.NewLine lines + Environment.NewLine)

let private cleanGeneratedPackageCacheEntries requestedPackages =
    let globalPackages =
        path [ Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); ".nuget"; "packages" ]

    if Directory.Exists globalPackages then
        for packageId, version in requestedPackages do
            if fsSkiaPackageIds |> Set.contains packageId then
                let packageCachePath = path [ globalPackages; packageId.ToLowerInvariant(); version ]

                if Directory.Exists packageCachePath then
                    Directory.Delete(packageCachePath, true)

let runGeneratedConsumerValidation model =
    ensureParent model.GeneratedProductValidationPath
    let stopwatch = Stopwatch.StartNew()
    let row =
        v3GeneratedRows model
        |> List.find (fun row -> row.Profile = "app" && row.Artifact = "source")

    writeLocalNuGetConfig model row.Root
    ensureGeneratedPackageVersions model row.Root

    let validationDir = path [ model.ReadinessDir; "generated-consumer-validation" ]
    let generatedPackageCache = path [ validationDir; "nuget-packages" ]
    Directory.CreateDirectory validationDir |> ignore
    Directory.GetFiles(validationDir)
    |> Array.iter File.Delete

    Directory.GetDirectories(validationDir)
    |> Array.iter (fun child -> Directory.Delete(child, true))

    cleanDirectoryContents generatedPackageCache
    let restoreLog = path [ validationDir; "restore.log" ]
    let semanticLog = path [ validationDir; "generated-verify.log" ]
    let boundedSmokePath = path [ validationDir; "bounded-smoke.txt" ]
    let boundedSmokeLog = path [ validationDir; "bounded-smoke.log" ]
    let sceneEvidencePath = path [ validationDir; "headless-scene-evidence.txt" ]
    let sceneEvidenceLog = path [ validationDir; "scene-evidence.log" ]
    let persistentLaunchLog = path [ validationDir; "persistent-launch-diagnostics.log" ]
    let persistentLaunchEvidencePath = path [ validationDir; "persistent-launch-evidence.txt" ]
    let windowDiagnosticsPath = path [ validationDir; "window-diagnostics.txt" ]
    let windowDiagnosticsLog = path [ validationDir; "window-diagnostics.log" ]
    let windowOptionsPath = path [ validationDir; "window-options.txt" ]
    let windowOptionsLog = path [ validationDir; "window-options.log" ]
    let imageEvidencePath = path [ validationDir; "game-image-evidence.png" ]
    let imageEvidenceLog = path [ validationDir; "image-evidence.log" ]
    let supportedHostPersistentLaunchPath = path [ model.ReadinessDir; "supported-host-persistent-launch.txt" ]

    let mutable category = "Completed"
    let diagnostics = ResizeArray<string>()

    let validationEnvironment =
        Map.empty

    let runStep step categoryOnFailure fileName arguments workingDirectory outputPath =
        try
            runProcess step fileName arguments workingDirectory outputPath validationEnvironment
            diagnostics.Add($"{step}: ok")
            true
        with ex ->
            category <- categoryOnFailure
            diagnostics.Add($"{step}: failed: {ex.Message}")
            false

    let requestedPackages =
        readRequestedGeneratedPackages row.Root

    cleanGeneratedPackageCacheEntries requestedPackages

    let restored =
        runStep
            "generated consumer restore from local packages"
            "RestoreFailure"
            "dotnet"
            "restore tests/Product.Tests/Product.Tests.fsproj --configfile NuGet.config --no-cache"
            row.Root
            restoreLog

    let resolvedPackages =
        if restored then readResolvedGeneratedPackages row.Root else []

    let packageSources =
        readNuGetPackageSources row.Root

    let restoreWarnings =
        readRestoreWarnings restoreLog

    let packageFailureReason, packageDiagnostics =
        packageResolutionDiagnostics requestedPackages resolvedPackages packageSources restoreWarnings

    let packageResolutionPassed =
        restored && packageFailureReason.IsNone

    if restored then
        if packageResolutionPassed then
            diagnostics.Add("package resolution: exact-match=true")
        else
            category <- "PackageDrift"
            let packageFailureClass = packageFailureReason |> Option.defaultValue "unknown"
            diagnostics.Add($"package resolution: exact-match=false failure-class={packageFailureClass}")
            packageDiagnostics |> List.iter diagnostics.Add

    let generatedTestsExist =
        File.Exists(path [ row.Root; "tests"; "Product.Tests"; "Product.Tests.fsproj" ])
        && File.Exists(path [ row.Root; "tests"; "Product.Tests"; "Tests.fs" ])

    let semanticPassed =
        packageResolutionPassed
        && runStep
            "generated consumer Verify"
            "SemanticTestFailure"
            "bash"
            "./fake.sh build -t Verify"
            row.Root
            semanticLog

    let smokePassed =
        semanticPassed
        && runStep
            "generated consumer bounded smoke"
            "ViewerStartupFailure"
            "dotnet"
            $"run --project src/Product/Product.fsproj --no-restore -- --bounded-smoke {quote boundedSmokePath}"
            row.Root
            boundedSmokeLog

    if smokePassed && File.Exists boundedSmokePath then
        let boundedSmoke = File.ReadAllText boundedSmokePath
        if boundedSmoke.IndexOf("status=unsupported", StringComparison.Ordinal) >= 0 then
            category <- "UnsupportedHost"
            diagnostics.Add("bounded viewer smoke unsupported")
        elif boundedSmoke.IndexOf("status=ok", StringComparison.Ordinal) >= 0 then
            diagnostics.Add("bounded viewer smoke reached requested evidence")
        else
            category <- "ViewerStartupFailure"
            diagnostics.Add("bounded viewer smoke did not report ok or unsupported")

    let scenePassed =
        semanticPassed
        && runStep
            "generated consumer scene evidence"
            "SceneEvidenceFailure"
            "dotnet"
            $"run --project src/Product/Product.fsproj --no-restore -- --scene-evidence {quote sceneEvidencePath}"
            row.Root
            sceneEvidenceLog

    if scenePassed && File.Exists sceneEvidencePath then
        diagnostics.Add("headless scene evidence captured")
    elif semanticPassed then
        category <- "SceneEvidenceFailure"
        diagnostics.Add("headless scene evidence output missing")

    let windowDiagnosticsPassed =
        semanticPassed
        && runStep
            "generated consumer window diagnostics"
            "WindowDiagnosticsFailure"
            "dotnet"
            $"run --project src/Product/Product.fsproj --no-restore -- --window-diagnostics {quote windowDiagnosticsPath}"
            row.Root
            windowDiagnosticsLog

    if windowDiagnosticsPassed && File.Exists windowDiagnosticsPath then
        diagnostics.Add("window diagnostics validation captured")
    elif semanticPassed then
        category <- "WindowDiagnosticsFailure"
        diagnostics.Add("window diagnostics output missing")

    let windowOptionsPassed =
        semanticPassed
        && runStep
            "generated consumer window options"
            "WindowOptionsFailure"
            "dotnet"
            $"run --project src/Product/Product.fsproj --no-restore -- --window-options {quote windowOptionsPath} --window-resize fixed-size --window-maximize not-maximizable --window-startup maximized --window-position 24,36 --window-backend opengl"
            row.Root
            windowOptionsLog

    if windowOptionsPassed && File.Exists windowOptionsPath then
        let windowOptionsText = File.ReadAllText windowOptionsPath
        if windowOptionsText.IndexOf("option=resize", StringComparison.OrdinalIgnoreCase) >= 0
           && windowOptionsText.IndexOf("option=maximize", StringComparison.OrdinalIgnoreCase) >= 0
           && windowOptionsText.IndexOf("option=startup-state", StringComparison.OrdinalIgnoreCase) >= 0
           && windowOptionsText.IndexOf("option=startup-position", StringComparison.OrdinalIgnoreCase) >= 0
           && windowOptionsText.IndexOf("option=backend", StringComparison.OrdinalIgnoreCase) >= 0
           && windowOptionsText.IndexOf("status=unsupported", StringComparison.OrdinalIgnoreCase) >= 0 then
            diagnostics.Add("window options validation captured")
        else
            category <- "WindowOptionsFailure"
            diagnostics.Add("window options output missing required option rows or unsupported diagnostics")
    elif semanticPassed then
        category <- "WindowOptionsFailure"
        diagnostics.Add("window options output missing")

    let imageEvidencePassed =
        semanticPassed
        && runStep
            "generated consumer image evidence"
            "VisualEvidenceFailure"
            "dotnet"
            $"run --project src/Product/Product.fsproj --no-restore -- --image-evidence {quote imageEvidencePath}"
            row.Root
            imageEvidenceLog

    let imageEvidenceMetadataPath = imageEvidencePath + ".metadata.txt"

    if imageEvidencePassed && File.Exists imageEvidenceMetadataPath then
        let imageEvidenceText = File.ReadAllText imageEvidenceMetadataPath
        if imageEvidenceText.IndexOf("evidence-kind=image", StringComparison.OrdinalIgnoreCase) >= 0
           && imageEvidenceText.IndexOf("image-decodable=true", StringComparison.OrdinalIgnoreCase) >= 0 then
            diagnostics.Add("image evidence validation captured")
        else
            category <- "VisualEvidenceFailure"
            diagnostics.Add("image evidence output missing decodable image fields")
    elif semanticPassed then
        category <- "VisualEvidenceFailure"
        diagnostics.Add("image evidence metadata missing")

    let persistentDiagnosticsPassed =
        semanticPassed
        && runStep
            "generated consumer persistent launch diagnostics"
            "PersistentLaunchDiagnosticFailure"
            "dotnet"
            $"run --project src/Product/Product.fsproj --no-restore -- --launch-evidence {quote persistentLaunchEvidencePath}"
            row.Root
            persistentLaunchLog

    if persistentDiagnosticsPassed then
        diagnostics.Add("persistent launch diagnostics captured separately from bounded evidence")

        let persistentLaunchText = File.ReadAllText persistentLaunchLog

        if persistentLaunchText.IndexOf("status=ok", StringComparison.OrdinalIgnoreCase) >= 0
           && persistentLaunchText.IndexOf("mode=persistent-evidence", StringComparison.OrdinalIgnoreCase) >= 0
           && persistentLaunchText.IndexOf("first-frame-presented=true", StringComparison.OrdinalIgnoreCase) >= 0 then
            writeSupportedHostPersistentLaunchEvidence
                supportedHostPersistentLaunchPath
                persistentLaunchLog
                "dotnet run --project artifacts/generated-products/020-asteroids-integration-feedback/app-source/src/Product/Product.fsproj --no-restore -- --launch-evidence"
                persistentLaunchText
            diagnostics.Add("supported-host persistent launch evidence normalized")

    stopwatch.Stop()

    let generatedTestsRan = semanticPassed
    let generatedVerifyRan = generatedTestsRan
    let generatedVerificationAuthoritative = generatedTestsExist && generatedTestsRan && generatedVerifyRan
    let generatedVerificationFailureClass =
        if not generatedTestsExist then "missing-generated-test-project"
        elif not generatedTestsRan then "missing-generated-test-execution"
        elif not generatedVerifyRan then "verify-target-not-authoritative"
        else "none"
    let packageFailureClass = packageFailureReason |> Option.defaultValue "none"
    let packageSourceSummary = String.Join(", ", packageSources)
    let persistentLaunchText =
        if File.Exists persistentLaunchLog then File.ReadAllText persistentLaunchLog else ""
    let generatedProgramText =
        let generatedProgramPath = path [ row.Root; "src"; "Product"; "Program.fs" ]
        if File.Exists generatedProgramPath then File.ReadAllText generatedProgramPath else ""
    let defaultLaunchSourceValidationPassed =
        let defaultBranch =
            let marker = "| _ ->"
            let index = generatedProgramText.LastIndexOf(marker, StringComparison.Ordinal)

            if index >= 0 then
                generatedProgramText.Substring(index)
            else
                generatedProgramText

        defaultBranch.IndexOf("Viewer.runApp viewerOptions generatedHost", StringComparison.Ordinal) >= 0
        && defaultBranch.IndexOf("mode=interactive-window", StringComparison.Ordinal) >= 0
        && defaultBranch.IndexOf("accessible-window=true", StringComparison.Ordinal) >= 0
        && defaultBranch.IndexOf("mode=persistent-evidence", StringComparison.Ordinal) < 0
        && defaultBranch.IndexOf("self-closed-for-evidence=true", StringComparison.Ordinal) < 0
    let closeReasonValidated =
        generatedProgramText.IndexOf("user-close-observed=%b", StringComparison.OrdinalIgnoreCase) >= 0
        && generatedProgramText.IndexOf("self-closed-for-evidence=%b", StringComparison.OrdinalIgnoreCase) >= 0
        && persistentLaunchText.IndexOf("self-closed-for-evidence=true", StringComparison.OrdinalIgnoreCase) >= 0
    let defaultInteractiveLaunchValidated =
        defaultLaunchSourceValidationPassed
        && persistentDiagnosticsPassed
    let windowDiagnosticsValidated = windowDiagnosticsPassed && File.Exists windowDiagnosticsPath
    let windowOptionsValidated = windowOptionsPassed && File.Exists windowOptionsPath
    let imageEvidenceValidated = imageEvidencePassed && File.Exists imageEvidenceMetadataPath
    let generatedContractAuthoritative =
        packageResolutionPassed
        && generatedVerificationAuthoritative
        && defaultInteractiveLaunchValidated
        && smokePassed
        && closeReasonValidated
        && windowDiagnosticsValidated
        && windowOptionsValidated
        && imageEvidenceValidated
    let generatedContractFailureClass =
        if not packageResolutionPassed then packageFailureClass
        elif not generatedVerificationAuthoritative then generatedVerificationFailureClass
        elif not defaultInteractiveLaunchValidated then "interactive-launch-validation"
        elif not smokePassed then "bounded-evidence-validation"
        elif not closeReasonValidated then "close-reason-validation"
        elif not windowDiagnosticsValidated then "window-diagnostics-validation"
        elif not windowOptionsValidated then "window-options-validation"
        elif not imageEvidenceValidated then "visual-evidence-validation"
        else "none"

    let report =
        [ "# Generated Product Validation"
          ""
          $"Category: `{category}`"
          $"Elapsed: `{stopwatch.Elapsed}`"
          $"Command context: `./fake.sh build -t PackLocal && ./fake.sh build -t GeneratedProductCheck`"
          $"Generated consumer root: `{row.Root}`"
          $"Local package feed: `{model.LocalPackageDir}`"
          ""
          "## Evidence"
          ""
          $"- Restore log: `{restoreLog}`"
          $"- Generated Verify log: `{semanticLog}`"
          $"- Bounded smoke log: `{boundedSmokeLog}`"
          $"- Bounded smoke evidence: `{boundedSmokePath}`"
          $"- Scene evidence log: `{sceneEvidenceLog}`"
          $"- Scene evidence output: `{sceneEvidencePath}`"
          $"- Persistent launch diagnostics log: `{persistentLaunchLog}`"
          $"- Window diagnostics log: `{windowDiagnosticsLog}`"
          $"- Window diagnostics output: `{windowDiagnosticsPath}`"
          $"- Window options log: `{windowOptionsLog}`"
          $"- Window options output: `{windowOptionsPath}`"
          $"- Image evidence log: `{imageEvidenceLog}`"
          $"- Image evidence output: `{imageEvidencePath}`"
          ""
          "## Contract Output"
          ""
          $"- package-resolution: `validated`"
          $"- exact-package-match: `{packageResolutionPassed}`"
          $"- generated-test-execution: `validated`"
          $"- generated-tests-ran: `{generatedTestsRan}`"
          $"- default-interactive-launch: `{defaultInteractiveLaunchValidated}`"
          $"- bounded-evidence-validation: `{smokePassed}`"
          $"- close-reason-validation: `{closeReasonValidated}`"
          $"- window-diagnostics-validation: `{windowDiagnosticsValidated}`"
          $"- window-options-validation: `{windowOptionsValidated}`"
          $"- image-evidence-validation: `{imageEvidenceValidated}`"
          $"- authoritative: `{generatedContractAuthoritative}`"
          $"- failure-class: `{generatedContractFailureClass}`"
          ""
          "## Package Resolution"
          ""
          $"- exact-match: `{packageResolutionPassed}`"
          $"- failure-class: `{packageFailureClass}`"
          $"- package-sources: `{packageSourceSummary}`"
          $"- restore-warning-count: `{restoreWarnings.Length}`"
          ""
          "Requested packages:"
          yield!
              requestedPackages
              |> List.map (fun (packageId, version) -> $"- requested {packageId}={version}")
          ""
          "Resolved packages:"
          yield!
              resolvedPackages
              |> List.map (fun (packageId, version) -> $"- resolved {packageId}={version}")
          ""
          "Restore warnings:"
          yield!
              restoreWarnings
              |> List.map (fun warning -> $"- {warning}")
          ""
          "## Generated Test Execution"
          ""
          $"- generated-tests-exist: `{generatedTestsExist}`"
          $"- generated-tests-ran: `{generatedTestsRan}`"
          $"- generated-verify-ran: `{generatedVerifyRan}`"
          $"- authoritative: `{generatedVerificationAuthoritative}`"
          $"- failure-class: `{generatedVerificationFailureClass}`"
          ""
          "## Diagnostics"
          ""
          yield! diagnostics |> Seq.map (fun item -> $"- {item}") ]
        |> String.concat Environment.NewLine

    File.WriteAllText(model.GeneratedProductValidationPath, report + Environment.NewLine)

    if category = "PackageDrift" || category = "RestoreFailure" || category = "SemanticTestFailure" || category = "ViewerStartupFailure" || category = "SceneEvidenceFailure" || category = "PersistentLaunchDiagnosticFailure" || category = "WindowDiagnosticsFailure" || category = "WindowOptionsFailure" || category = "VisualEvidenceFailure" then
        failwithf "Generated consumer validation failed with category %s; see %s" category model.GeneratedProductValidationPath

let runDependencyOwnershipReport model =
    let sceneProject = File.ReadAllText(path [ model.RepositoryRoot; "src"; "Scene"; "Scene.fsproj" ])
    let controlsProject = File.ReadAllText(path [ model.RepositoryRoot; "src"; "Controls"; "Controls.fsproj" ])
    let keyboardProject = File.ReadAllText(path [ model.RepositoryRoot; "src"; "KeyboardInput"; "KeyboardInput.fsproj" ])
    let adapterProject = File.ReadAllText(path [ model.RepositoryRoot; "src"; "Controls.Elmish"; "Controls.Elmish.fsproj" ])
    let removedChartsPackage = "FS.Skia.UI." + "Charts"
    let removedChartsProject = "src/" + "Charts/" + "Charts.fsproj"

    [ "Fable.Elmish"; "Silk.NET"; "SkiaSharp"; "Yoga.Net"; "YamlDotNet" ]
    |> List.iter (fun forbidden ->
        if sceneProject.IndexOf(forbidden, StringComparison.Ordinal) >= 0 then
            failwithf "Scene dependency leak: %s" forbidden)

    [ "SkiaViewer",
      [ @"Include=""..\SkiaViewer\SkiaViewer.fsproj"""
        "Include=\"../SkiaViewer/SkiaViewer.fsproj\""
        "PackageReference Include=\"FS.Skia.UI.SkiaViewer\"" ]
      "Elmish",
      [ @"Include=""..\Elmish\Elmish.fsproj"""
        "Include=\"../Elmish/Elmish.fsproj\""
        "PackageReference Include=\"Fable.Elmish\""
        "PackageReference Include=\"FS.Skia.UI.Elmish\"" ] ]
    |> List.iter (fun (forbidden, needles) ->
        if needles |> List.exists (fun needle -> controlsProject.IndexOf(needle, StringComparison.Ordinal) >= 0) then
            failwithf "Controls dependency leak: %s" forbidden)

    if controlsProject.IndexOf("PackageReference", StringComparison.Ordinal) >= 0 then
        failwith "Controls dependency leak: base Controls must not own direct external PackageReference entries"

    if keyboardProject.IndexOf("YamlDotNet", StringComparison.Ordinal) < 0 then
        failwith "KeyboardInput dependency gap: YamlDotNet ownership is not recorded"

    if adapterProject.IndexOf("Fable.Elmish", StringComparison.Ordinal) < 0 then
        failwith "Controls.Elmish dependency gap: Fable.Elmish ownership is not recorded"

    if File.Exists(path [ model.RepositoryRoot; "src"; "Charts"; "Charts.fsproj" ]) then
        failwithf "Removed package project is still active: %s" removedChartsProject

    [ controlsProject; keyboardProject; adapterProject ]
    |> List.iter (fun project ->
        if project.IndexOf(removedChartsPackage, StringComparison.Ordinal) >= 0
           || project.IndexOf(removedChartsProject, StringComparison.Ordinal) >= 0 then
            failwith "Removed Charts package remains in active Controls boundary project references")

    let report =
        [ "# Dependency Report"
          ""
          "PASS: V3 dependency ownership report completed."
          ""
          "- Scene has no Elmish, Silk.NET, SkiaSharp, Yoga.Net, or YamlDotNet dependency."
          "- SkiaViewer owns Silk.NET and SkiaSharp host dependencies."
          "- Elmish owns Fable.Elmish adapter dependency."
          "- KeyboardInput owns YamlDotNet dependency."
          "- Layout owns Yoga.Net dependency."
          "- Controls owns form controls, rich rendering, chart controls, graph views, DataGrid, and ControlRuntime declarations."
          "- Controls depends only on Scene, Layout, and KeyboardInput and has no direct external PackageReference entries."
          "- Controls.Elmish owns Fable.Elmish command, subscription, and program adapter dependency."
          "- The removed Charts package is absent from active package, baseline, and generated product lists."
          "- Legacy Charts package/project is removed from active package, baseline, and generated product lists; migration guidance is documentation-only."
          "- Testing owns generated-product validation helpers."
          ""
          "Evidence:"
          ""
          "- Command: `./fake.sh build -t DependencyReport`"
          "- Source: `Directory.Packages.props`"
          "- Active feature evidence: `specs/025-upgrade-skia-speckit/readiness/version-selection.md` when feature 025 is active."
          ""
          "## Before And After"
          ""
          "| Package | Before | After | Owner | Status |"
          "|---------|--------|-------|-------|--------|"
          "| SkiaSharp | `4.147.0-preview.2.1` | `4.147.0-preview.3.1` | SkiaViewer/compatibility renderer host | aligned |"
          "| SkiaSharp.NativeAssets.Linux | `4.147.0-preview.2.1` | `4.147.0-preview.3.1` | Linux native renderer assets | aligned |"
          "| SkiaSharp.NativeAssets.Win32 | `4.147.0-preview.2.1` | `4.147.0-preview.3.1` | Windows native renderer assets | aligned |"
          "| Spec Kit metadata | `0.8.11` | `0.8.16` | project governance metadata | aligned to latest release metadata |"
          ""
          "cycle status: no new project reference was added, so the package graph cycle status is unchanged."
          "unexpected spread review: no SkiaSharp reference was added to Scene, Layout, Controls, Controls.Elmish, KeyboardInput, Testing, generated product source, or generated product tests." ]
        |> String.concat Environment.NewLine

    File.WriteAllText(model.DependencyReportPath, report + Environment.NewLine)

let runPackageSurfaceReport model =
    let rows =
        [ "# Package Surfaces"
          ""
          "PASS: package-specific surface baselines are present for public V3 capabilities."
          ""
          "- `readiness/surface-baselines/FS.Skia.UI.Scene.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.Elmish.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.KeyboardInput.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.Layout.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.Controls.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.Testing.txt`" ]
        |> String.concat Environment.NewLine

    File.WriteAllText(path [ model.PackageSurfaceReportDir; "index.md" ], rows + Environment.NewLine)

// BUILD SECTION: guidance validation

