module GovernanceTestSupport

open System
open System.Diagnostics
open System.IO
open System.Text.Json
open System.Xml.Linq
open Expecto

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

let read (relativePath: string) =
    File.ReadAllText(fullPath relativePath)

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
    let executable, processArguments =
        if fileName = "./fake.sh" || fileName = "fake.sh" then
            let scriptPath = fullPath "fake.sh"
            "bash", $"\"{scriptPath}\" {arguments}"
        elif fileName.StartsWith("./", StringComparison.Ordinal) then
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
    if fileName = "./fake.sh" || fileName = "fake.sh" then
        lock fakeProcessLock (fun () -> runProcessUnlocked fileName arguments)
    else
        runProcessUnlocked fileName arguments

let runFakeTarget target =
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

let expectPathsExist context paths =
    paths
    |> List.iter (fun relative -> Expect.isTrue (File.Exists(fullPath relative) || Directory.Exists(fullPath relative)) $"{context}: {relative} exists")

let expectFakeTarget target =
    Expect.stringContains (read "build.fsx") $"\"{target}\"" $"build.fsx declares {target}"

let expectGeneratedProductFileList profile (required: string list) (forbidden: string list) =
    let reportPath = $"specs/009-v3-modular-framework/readiness/generated-file-lists/{profile}.txt"
    Expect.isTrue (fileExists reportPath) $"{profile} file-list report exists"
    let content = read reportPath

    required
    |> List.iter (fun item -> Expect.stringContains content item $"{profile} includes {item}")

    forbidden
    |> List.iter (fun item -> Expect.isFalse (content.Contains item) $"{profile} excludes {item}")
