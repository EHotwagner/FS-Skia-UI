module GovernanceTestSupport

open System
open System.Diagnostics
open System.IO
open System.Text.Json
open System.Xml.Linq
open Expecto
open FS.Skia.UI.Build.Evidence

let rec findRepositoryRoot (directory: string) =
    if Directory.GetFiles(directory, "*.sln").Length > 0 || File.Exists(Path.Combine(directory, "build.fsx")) then
        directory
    else
        match Directory.GetParent directory |> Option.ofObj with
        | Some parent -> findRepositoryRoot parent.FullName
        | None -> failwithf "Could not locate repository root from %s" directory

let repositoryRoot = findRepositoryRoot AppContext.BaseDirectory

let fullPath (relativePath: string) =
    Path.Combine(repositoryRoot, relativePath.Replace("/", string Path.DirectorySeparatorChar))

let fileExists relativePath =
    File.Exists(fullPath relativePath)

let directoryExists relativePath =
    Directory.Exists(fullPath relativePath)

// Feature 045: build.fsx was deleted; the entire build front-end (the MEL engine, the
// relocated validators, the helper modules) now lives in compiled build/Governance modules
// (recursively, incl. Front/ + Engine/ subdirs). The governance-source contract aggregates
// those plus the still-present scripts/build/*.fsx helpers. `read "build.fsx"` is redirected
// to this aggregate so the contract tests that historically asserted build.fsx text keep
// asserting the same tokens against their relocated home (behaviour/intent preserved).
let private readSourceTree relative =
    let dir = fullPath relative

    if Directory.Exists dir then
        [ Directory.GetFiles(dir, "*.fsi", SearchOption.AllDirectories)
          Directory.GetFiles(dir, "*.fs", SearchOption.AllDirectories) ]
        |> Array.concat
        |> Array.filter (fun p ->
            let n = p.Replace('\\', '/')
            not (n.Contains "/bin/" || n.Contains "/obj/"))
        |> Array.sort
        |> Array.map File.ReadAllText
        |> String.concat Environment.NewLine
    else
        ""

let readBuildFrontEnd () =
    let scriptsBuild =
        let dir = fullPath "scripts/build"

        if Directory.Exists dir then
            Directory.GetFiles(dir, "*.fsx")
            |> Array.sort
            |> Array.map File.ReadAllText
            |> String.concat Environment.NewLine
        else
            ""

    [ readSourceTree "build/Governance"; scriptsBuild ]
    |> List.filter (fun text -> text <> "")
    |> String.concat Environment.NewLine

let read (relativePath: string) =
    if relativePath = "build.fsx" then
        readBuildFrontEnd ()
    else
        File.ReadAllText(fullPath relativePath)

let readBuildGovernanceSources () = readBuildFrontEnd ()

let readJson (relativePath: string) =
    JsonDocument.Parse(read relativePath)

let readXml (relativePath: string) =
    XDocument.Load(fullPath relativePath)

let expectContains (content: string) (needle: string) (context: string) =
    Expect.stringContains content needle context

let expectFileContains (relativePath: string) (needles: string list) =
    let content = read relativePath

    needles
    |> List.iter (fun needle -> expectContains content needle $"{relativePath} contains {needle}")

// Feature 057 (FR-006): single-source the in-file scanner echoes. The "Exact … phrases
// for scans:" echo lines existed only so a literal-substring scanner would match a phrase
// the surrounding natural prose already states but line-wraps. The faithful fix is to make
// the scanner read the canonical prose with incidental line-wrapping collapsed, then delete
// the echo. This still fails on a genuinely-absent concept (drift detection preserved); it
// only ignores newline/indent runs introduced by Markdown wrapping.
let private collapseWhitespace (s: string) =
    System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ")

let expectFileContainsNormalized (relativePath: string) (needles: string list) =
    let content = collapseWhitespace (read relativePath)

    needles
    |> List.iter (fun needle ->
        expectContains content (collapseWhitespace needle) $"{relativePath} contains (whitespace-normalized) {needle}")

type MarkdownSection =
    { Heading: string
      Level: int
      StartLine: int
      EndLine: int
      Content: string }

let headingLevel (line: string) =
    let trimmed = line.TrimStart()

    if trimmed.StartsWith("#") then
        let level = trimmed |> Seq.takeWhile ((=) '#') |> Seq.length
        if level > 0 && trimmed.Length > level && trimmed[level] = ' ' then
            Some(level, trimmed.Substring(level).Trim())
        else
            None
    else
        None

let markdownSections (content: string) =
    let lines = content.Replace("\r\n", "\n").Split('\n')

    lines
    |> Array.mapi (fun index line -> index, line, headingLevel line)
    |> Array.choose (function
        | index, _, Some(level, heading) -> Some(index, level, heading)
        | _ -> None)
    |> Array.mapi (fun headingIndex (startIndex, level, heading) ->
        let endIndex =
            lines
            |> Array.mapi (fun index line -> index, headingLevel line)
            |> Array.skip (startIndex + 1)
            |> Array.tryPick (function
                | index, Some(nextLevel, _) when nextLevel <= level -> Some(index - 1)
                | _ -> None)
            |> Option.defaultValue (lines.Length - 1)

        let sectionContent =
            lines[startIndex..endIndex]
            |> String.concat "\n"

        { Heading = heading
          Level = level
          StartLine = startIndex + 1
          EndLine = endIndex + 1
          Content = sectionContent })
    |> Array.toList

let trySection (heading: string) (content: string) =
    markdownSections content
    |> List.tryFind (fun section -> section.Heading.Contains(heading, StringComparison.OrdinalIgnoreCase))

let requireSection (heading: string) (content: string) context =
    match trySection heading content with
    | Some section -> section
    | None -> failtestf "%s is missing Markdown section containing '%s'" context heading

let expectPromptInSection relativePath sectionHeading (prompt: string) =
    let section = requireSection sectionHeading (read relativePath) relativePath
    Expect.stringContains (section.Content.ToLowerInvariant()) (prompt.ToLowerInvariant()) $"{relativePath} has {prompt} in {sectionHeading}"

let markdownTableRows relativePath =
    read relativePath
    |> fun content -> content.Replace("\r\n", "\n").Split('\n')
    |> Array.choose (fun line ->
        let trimmed = line.Trim()

        if trimmed.StartsWith("|") && trimmed.EndsWith("|") && not (trimmed.Contains("---")) then
            Some(trimmed.Trim('|').Split('|') |> Array.map (fun cell -> cell.Trim()) |> Array.toList)
        else
            None)
    |> Array.toList

let fakeProcessLock = obj ()

let runProcessUnlocked (fileName: string) (arguments: string) =
    if fileName = "./fake.sh" || fileName = "fake.sh" then
        let scriptPath = fullPath "fake.sh"
        let outputPath = Path.GetTempFileName()
        let command = $"\"{scriptPath}\" {arguments} > \"{outputPath}\" 2>&1"
        let startInfo = ProcessStartInfo("bash")
        startInfo.ArgumentList.Add("-c")
        startInfo.ArgumentList.Add(command)
        startInfo.WorkingDirectory <- repositoryRoot
        startInfo.UseShellExecute <- false

        try
            use proc =
                match Process.Start(startInfo) |> Option.ofObj with
                | Some proc -> proc
                | None -> failwithf "Could not start %s %s" fileName arguments

            if proc.WaitForExit(240000) then
                proc.ExitCode, File.ReadAllText outputPath, ""
            else
                proc.Kill()
                -1, File.ReadAllText outputPath, ""
        finally
            if File.Exists outputPath then
                File.Delete outputPath
    else
        let executable, processArguments =
            if fileName.StartsWith("./", StringComparison.Ordinal) then
                fullPath (fileName.Substring 2), arguments
            else
                fileName, arguments

        let startInfo: ProcessStartInfo = ProcessStartInfo(executable, processArguments)
        startInfo.WorkingDirectory <- repositoryRoot
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false

        use proc =
            match Process.Start(startInfo) |> Option.ofObj with
            | Some proc -> proc
            | None -> failwithf "Could not start %s %s" fileName arguments

        let stdout = proc.StandardOutput.ReadToEnd()
        let stderr = proc.StandardError.ReadToEnd()

        if proc.WaitForExit(240000) then
            proc.ExitCode, stdout, stderr
        else
            proc.Kill()
            -1, stdout, stderr

let runProcess (fileName: string) (arguments: string) =
    if fileName = "./fake.sh"
       || fileName = "fake.sh"
       || (fileName = "dotnet" && arguments.StartsWith("fake ", StringComparison.OrdinalIgnoreCase)) then
        lock fakeProcessLock (fun () -> runProcessUnlocked fileName arguments)
    else
        runProcessUnlocked fileName arguments

let runFakeTarget target =
    // Feature 045: the front-end is the compiled exe behind ./fake.sh, not `dotnet fake`.
    runProcess "./fake.sh" $"build -t {target}"

let projectFiles () =
    Directory.EnumerateFiles(repositoryRoot, "*.fsproj", SearchOption.AllDirectories)
    |> Seq.filter (fun file ->
        let relative =
            file.Substring(repositoryRoot.Length)
                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Replace('\\', '/')

        (relative.StartsWith("src/", StringComparison.Ordinal)
         || relative.StartsWith("tests/", StringComparison.Ordinal)
         || relative.StartsWith("samples/", StringComparison.Ordinal))
        && not (relative.Contains("/bin/"))
        && not (relative.Contains("/obj/")))
    |> Seq.toList

type CatalogCapability =
    { Id: string
      DisplayName: string
      PackageId: string option
      Project: string option
      Contracts: string list
      Tests: string list
      Skill: string option
      TemplateFragment: string option
      Dependencies: string list
      Profiles: string list
      DefaultApp: bool
      Evidence: string list
      SurfaceBaseline: string option
      Docs: string option
      OwnerNotes: string option
      NonRuntime: bool }

let private emptyCapability id =
    { Id = id
      DisplayName = ""
      PackageId = None
      Project = None
      Contracts = []
      Tests = []
      Skill = None
      TemplateFragment = None
      Dependencies = []
      Profiles = []
      DefaultApp = false
      Evidence = []
      SurfaceBaseline = None
      Docs = None
      OwnerNotes = None
      NonRuntime = false }

let private trimQuotes (value: string) =
    value.Trim().Trim('"').Trim('\'')

let private parseScalar (line: string) =
    match line.IndexOf(':') with
    | index when index >= 0 -> line.Substring(index + 1) |> trimQuotes
    | _ -> ""

let private parseInlineList (value: string) =
    let trimmed = value.Trim()

    if trimmed.StartsWith("[") && trimmed.EndsWith("]") then
        trimmed.Trim('[', ']').Split(',', StringSplitOptions.RemoveEmptyEntries)
        |> Array.map trimQuotes
        |> Array.toList
    elif String.IsNullOrWhiteSpace trimmed then
        []
    else
        [ trimQuotes trimmed ]

let readCatalogCapabilities () =
    let lines =
        read "template/capabilities.yml"
        |> fun text -> text.Replace("\r\n", "\n").Split('\n')

    let mutable current: CatalogCapability option = None
    let mutable currentList: string option = None
    let capabilities = ResizeArray<CatalogCapability>()

    let commitCurrent () =
        match current with
        | Some capability -> capabilities.Add capability
        | None -> ()

    let setCurrent value =
        current <- current |> Option.map value

    for raw in lines do
        let line = raw.TrimEnd()
        let trimmed = line.Trim()

        if trimmed.StartsWith("- id:") then
            commitCurrent ()
            current <- Some(emptyCapability (parseScalar trimmed))
            currentList <- None
        elif trimmed.StartsWith("- ") && currentList.IsSome then
            let item = trimmed.Substring(2) |> trimQuotes

            match currentList.Value with
            | "contracts" -> setCurrent (fun c -> { c with Contracts = c.Contracts @ [ item ] })
            | "tests" -> setCurrent (fun c -> { c with Tests = c.Tests @ [ item ] })
            | "dependencies" -> setCurrent (fun c -> { c with Dependencies = c.Dependencies @ [ item ] })
            | "profiles" -> setCurrent (fun c -> { c with Profiles = c.Profiles @ [ item ] })
            | "evidence" -> setCurrent (fun c -> { c with Evidence = c.Evidence @ [ item ] })
            | _ -> ()
        elif current.IsSome && trimmed.Contains(":") then
            let field = trimmed.Substring(0, trimmed.IndexOf(':')).Trim()
            let value = parseScalar trimmed
            currentList <- None

            match field with
            | "displayName" -> setCurrent (fun c -> { c with DisplayName = value })
            | "packageId" -> setCurrent (fun c -> { c with PackageId = Some value })
            | "project" -> setCurrent (fun c -> { c with Project = Some value })
            | "contracts" ->
                setCurrent (fun c -> { c with Contracts = parseInlineList value })
                currentList <- Some "contracts"
            | "tests" ->
                setCurrent (fun c -> { c with Tests = parseInlineList value })
                currentList <- Some "tests"
            | "skill" -> setCurrent (fun c -> { c with Skill = Some value })
            | "templateFragment" -> setCurrent (fun c -> { c with TemplateFragment = Some value })
            | "dependencies" ->
                setCurrent (fun c -> { c with Dependencies = parseInlineList value })
                currentList <- Some "dependencies"
            | "profiles" ->
                setCurrent (fun c -> { c with Profiles = parseInlineList value })
                currentList <- Some "profiles"
            | "defaultApp" -> setCurrent (fun c -> { c with DefaultApp = value.Equals("true", StringComparison.OrdinalIgnoreCase) })
            | "evidence" ->
                setCurrent (fun c -> { c with Evidence = parseInlineList value })
                currentList <- Some "evidence"
            | "surfaceBaseline" -> setCurrent (fun c -> { c with SurfaceBaseline = Some value })
            | "docs" -> setCurrent (fun c -> { c with Docs = Some value })
            | "ownerNotes" -> setCurrent (fun c -> { c with OwnerNotes = Some value })
            | "nonRuntime" -> setCurrent (fun c -> { c with NonRuntime = value.Equals("true", StringComparison.OrdinalIgnoreCase) })
            | _ -> ()

    commitCurrent ()
    capabilities |> Seq.toList

// Feature 043 (T025): typed evidence-engine helpers so tests exercise the
// compiled F# graph/audit engine in-process instead of shelling
// `python3 compute-task-graph.py` / `bash run-audit.sh`. Every read the build.fsx
// interpreter performs at the edge (tasks.md, tasks.deps.yml, readiness files,
// the skill registry, audit-patterns.yml) is reproduced here as data; the engine
// stays pure. `featDir` is an absolute feature/fixture directory.
let evidenceInputsForDir (featDir: string) (recordedFeature: string option) : EvidenceInputs =
    let readinessDir = Path.Combine(featDir, "readiness")
    let readinessFiles =
        if Directory.Exists readinessDir then
            Directory.GetFiles(readinessDir, "*", SearchOption.AllDirectories)
            |> Array.map (fun p -> p.Substring(readinessDir.Length + 1).Replace('\\', '/'), File.ReadAllText p)
            |> List.ofArray
        else
            []
    let readFeatureFile name =
        let p = Path.Combine(featDir, name)
        if File.Exists p then File.ReadAllText p else ""
    let featureText =
        [ "spec.md"; "plan.md"; "tasks.md" ] |> List.map readFeatureFile |> String.concat "\n"
    let auditStatusFiles =
        readinessFiles
        |> List.filter (fun (rel, c) ->
            rel.EndsWith ".md"
            && c.Contains "```audit-status"
            && not (rel.ToLowerInvariant().StartsWith "audit-fixtures/")
            && not (rel.ToLowerInvariant().StartsWith "audit-rejections/"))
    let slePath = Path.Combine(readinessDir, "skill-loading-evidence.md")
    let dirName =
        Path.GetFileName(featDir.TrimEnd('/', '\\')) |> Option.ofObj |> Option.defaultValue ""
    { FeatureName = (match recordedFeature with Some f -> f | None -> dirName)
      TasksMd = readFeatureFile "tasks.md"
      DepsYml = readFeatureFile "tasks.deps.yml"
      Registry = SkillRegistry.build repositoryRoot
      SkillLoadingEvidence = (if File.Exists slePath then Some(File.ReadAllText slePath) else None)
      ResolvedExists = (fun p -> File.Exists(if Path.IsPathRooted p then p else Path.Combine(repositoryRoot, p)))
      Canonicalize = (fun p -> Path.GetFullPath(if Path.IsPathRooted p then p else Path.Combine(repositoryRoot, p)))
      RecordedFeature = recordedFeature
      Scan = { ReadinessDir = readinessDir; FeatureText = featureText; ReadinessFiles = readinessFiles }
      AuditStatusFiles = auditStatusFiles
      PatternsYml = File.ReadAllText(Path.Combine(repositoryRoot, ".specify/extensions/evidence/audit-patterns.yml"))
      BaseRef = None
      UnifiedDiff = "" }

/// Run the in-process merge-gate audit over an absolute feature/fixture dir.
let runEvidenceAuditAt (featDir: string) : AuditResult * AuditArtifacts =
    Engine.runAudit (evidenceInputsForDir featDir None)

/// Run the in-process graph compute over an absolute feature/fixture dir.
let runEvidenceGraphAt (featDir: string) : GraphResult * GraphArtifacts =
    Engine.runGraph (evidenceInputsForDir featDir None)

let expectPathsExist context paths =
    paths
    |> List.iter (fun relative -> Expect.isTrue (File.Exists(fullPath relative) || Directory.Exists(fullPath relative)) $"{context}: {relative} exists")

// Feature 041: FAKE targets are registered off the typed Targets.dispatchTargets DU
// (FR-001/FR-013), so target identity and the dependency graph are asserted against the
// real library values rather than string-tuple literals that used to live in build.fsx.
let registeredTargetNames =
    FS.Skia.UI.Build.Targets.dispatchTargets |> List.map FS.Skia.UI.Build.Targets.name

let expectFakeTarget target =
    Expect.isTrue (List.contains target registeredTargetNames) $"build registers the {target} FAKE target"

let dependencyRow name =
    FS.Skia.UI.Build.Targets.targetDependencyRows
    |> List.tryFind (fun (target, _) -> target = name)
    |> Option.map snd

let expectDependency name expected =
    Expect.equal (dependencyRow name) (Some expected) $"{name} dependency row (Targets.targetDependencyRows)"

type TempFixtureDirectory(prefix: string) =
    let root =
        Path.Combine(Path.GetTempPath(), prefix + "-" + Guid.NewGuid().ToString("N"))

    do Directory.CreateDirectory(root) |> ignore

    member _.Root = root

    member _.Path([<ParamArray>] segments: string array) =
        segments
        |> Array.fold (fun current segment -> Path.Combine(current, segment)) root

    interface IDisposable with
        member _.Dispose() =
            if Directory.Exists root then
                Directory.Delete(root, true)

let writeFixtureFile (root: string) (relativePath: string) (content: string) =
    let destination =
        Path.Combine(root, relativePath.Replace("/", string Path.DirectorySeparatorChar))

    match Path.GetDirectoryName destination |> Option.ofObj with
    | Some directory -> Directory.CreateDirectory directory |> ignore
    | None -> ()
    File.WriteAllText(destination, content)
    destination

let writeProjectFixture root relativePath packageReferences projectReferences =
    let packageRows =
        packageReferences
        |> List.map (fun packageId -> $"    <PackageReference Include=\"{packageId}\" />")
        |> String.concat Environment.NewLine

    let projectRows =
        projectReferences
        |> List.map (fun projectPath -> $"    <ProjectReference Include=\"{projectPath}\" />")
        |> String.concat Environment.NewLine

    let content =
        $"""<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <OutputType>Library</OutputType>
  </PropertyGroup>

  <ItemGroup>
{packageRows}
{projectRows}
  </ItemGroup>

</Project>
"""

    writeFixtureFile root relativePath content

let writeCapabilityMetadataFixture root capabilitiesYaml =
    writeFixtureFile root "template/capabilities.yml" capabilitiesYaml

let writeGeneratedProductFixture root profile files =
    files
    |> List.map (fun (relativePath, content) ->
        writeFixtureFile root (Path.Combine("generated", profile, relativePath).Replace('\\', '/')) content)

let writeGeneratedGuidanceFixture root relativePath sourceMarkers testMarkers =
    let content =
        [ "# Generated Guidance Fixture"
          ""
          "Source markers:"
          yield! sourceMarkers |> List.map (fun marker -> $"- {marker}")
          ""
          "Test markers:"
          yield! testMarkers |> List.map (fun marker -> $"- {marker}") ]
        |> String.concat Environment.NewLine

    writeFixtureFile root relativePath (content + Environment.NewLine)

let writeStaleReferenceFixture root relativePath term context =
    writeFixtureFile root relativePath $"{context}: {term}{Environment.NewLine}"

let writeReadinessReportFixture root relativePath verdict evidencePaths =
    let content =
        [ "# Readiness Fixture"
          ""
          $"Verdict: {verdict}"
          ""
          "Evidence:"
          yield! evidencePaths |> List.map (fun path -> $"- `{path}`") ]
        |> String.concat Environment.NewLine

    writeFixtureFile root relativePath (content + Environment.NewLine)

let expectGeneratedProductFileList profile (required: string list) (forbidden: string list) =
    let activePath = $"specs/011-controls-boundary-refactor/readiness/generated-file-lists/{profile}.txt"
    let currentPath = $"specs/010-skia-controls-library/readiness/generated-file-lists/{profile}.txt"
    let reportPath =
        if fileExists activePath then
            activePath
        elif fileExists currentPath then
            currentPath
        else
            $"specs/009-v3-modular-framework/readiness/generated-file-lists/{profile}.txt"

    Expect.isTrue (fileExists reportPath) $"{profile} file-list report exists"
    let content = read reportPath
    let fileList =
        let marker = "Files:"
        let packageMarker = "Package references:"
        let startIndex = content.IndexOf(marker, StringComparison.Ordinal)

        if startIndex < 0 then
            content
        else
            let filesStart = startIndex + marker.Length
            let packageStart = content.IndexOf(packageMarker, filesStart, StringComparison.Ordinal)

            if packageStart < 0 then
                content.Substring(filesStart)
            else
                content.Substring(filesStart, packageStart - filesStart)

    required
    |> List.iter (fun item -> Expect.stringContains content item $"{profile} includes {item}")

    forbidden
    |> List.iter (fun item ->
        let subject =
            if item.StartsWith("PackageReference", StringComparison.Ordinal)
               || item.Contains(".fsproj", StringComparison.Ordinal)
               || item.Contains("src/Charts", StringComparison.Ordinal)
               || item.Contains("tests/Charts.Tests", StringComparison.Ordinal) then
                content
            else
                fileList

        Expect.isFalse (subject.Contains item) $"{profile} excludes {item}")
