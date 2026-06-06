open System
open System.Diagnostics
open System.IO
open System.IO.Compression
open System.Security.Cryptography
open System.Text
open System.Text.Json
open System.Text.RegularExpressions

let repositoryRoot =
    Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, ".."))

let repoPath (relative: string) =
    Path.Combine(repositoryRoot, relative.Replace('/', Path.DirectorySeparatorChar))

let normalizePath (path: string) = path.Replace('\\', '/')

let usage =
    """Usage:
  dotnet fsi scripts/archive-readiness.fsx --feature <feature-id> [--feature <feature-id> ...]
  dotnet fsi scripts/archive-readiness.fsx --all-unarchived

Options:
  --dry-run                 Print planned archives without writing files.
  --force                   Replace an existing archive for the feature.
  --skip-inventory-refresh  Do not run archive inventory/stale-reference scripts.
  --help                    Print this help.
"""

type Options =
    { Features: string list
      AllUnarchived: bool
      DryRun: bool
      Force: bool
      SkipInventoryRefresh: bool }

let defaultOptions =
    { Features = []
      AllUnarchived = false
      DryRun = false
      Force = false
      SkipInventoryRefresh = false }

let args =
    let raw = Environment.GetCommandLineArgs() |> Array.skip 1 |> Array.toList

    match raw with
    | script :: rest when script.EndsWith(".fsx", StringComparison.OrdinalIgnoreCase) -> rest
    | rest -> rest

let rec parse options remaining =
    match remaining with
    | [] -> options
    | "--feature" :: feature :: tail -> parse { options with Features = feature :: options.Features } tail
    | "--all-unarchived" :: tail -> parse { options with AllUnarchived = true } tail
    | "--dry-run" :: tail -> parse { options with DryRun = true } tail
    | "--force" :: tail -> parse { options with Force = true } tail
    | "--skip-inventory-refresh" :: tail -> parse { options with SkipInventoryRefresh = true } tail
    | "--help" :: _ ->
        printf "%s" usage
        Environment.Exit 0
        options
    | unknown :: _ -> failwithf "Unknown argument: %s\n%s" unknown usage

let options = parse defaultOptions args

if List.isEmpty options.Features && not options.AllUnarchived then
    failwithf "No feature selected.\n%s" usage

let featurePattern = Regex(@"^\d{3}-[A-Za-z0-9][A-Za-z0-9_-]*$", RegexOptions.Compiled)
let protectedFeatures = Set.ofList [ "036-archive-readiness-api-docs" ]

let activeFeature () =
    let featureJson = repoPath ".specify/feature.json"

    if File.Exists featureJson then
        let text = File.ReadAllText featureJson
        let matchValue = Regex.Match(text, @"""feature_directory""\s*:\s*""specs/([^""]+)""")

        if matchValue.Success then
            Some matchValue.Groups.[1].Value
        else
            None
    else
        None

let zipPath feature = repoPath $"specs/archive/{feature}/readiness.zip"
let archiveReadmePath feature = repoPath $"specs/archive/{feature}/README.md"
let readinessPath feature = repoPath $"specs/{feature}/readiness"

let sha256OfFile path =
    use stream = File.OpenRead path
    use sha = SHA256.Create()
    sha.ComputeHash stream |> Convert.ToHexString |> fun value -> value.ToLowerInvariant()

let filesInReadiness feature =
    let readiness = readinessPath feature

    if Directory.Exists readiness then
        Directory.EnumerateFiles(readiness, "*", SearchOption.AllDirectories)
        |> Seq.sort
        |> Seq.toList
    else
        []

let hasOnlyArchiveStub feature =
    let files = filesInReadiness feature

    match files with
    | [ only ] when Path.GetFileName(only) = "README.md" ->
        let text = File.ReadAllText only
        text.Contains("Archived Readiness Evidence", StringComparison.Ordinal)
    | _ -> false

let allUnarchivedFeatures () =
    let active = activeFeature ()

    Directory.GetDirectories(repoPath "specs")
    |> Seq.map (fun path -> DirectoryInfo(path).Name)
    |> Seq.filter (fun feature -> featurePattern.IsMatch feature)
    |> Seq.filter (fun feature -> not (protectedFeatures.Contains feature))
    |> Seq.filter (fun feature -> active <> Some feature)
    |> Seq.filter (fun feature -> Directory.Exists(readinessPath feature))
    |> Seq.filter (fun feature -> not (hasOnlyArchiveStub feature))
    |> Seq.toList

let selectedFeatures =
    let explicitFeatures = options.Features |> List.rev
    let allFeatures = if options.AllUnarchived then allUnarchivedFeatures () else []
    (explicitFeatures @ allFeatures) |> List.distinct |> List.sort

let validateFeature (feature: string) =
    if not (featurePattern.IsMatch feature) then
        failwithf "Invalid feature id: %s" feature

    if protectedFeatures.Contains feature then
        failwithf "Refusing to archive protected governance feature: %s" feature

    let featureDir = repoPath $"specs/{feature}"

    if not (Directory.Exists featureDir) then
        failwithf "Feature directory does not exist: specs/%s" feature

let createZip feature files =
    let archiveFile = zipPath feature
    Directory.CreateDirectory(Path.GetDirectoryName archiveFile) |> ignore

    if File.Exists archiveFile then
        if options.Force then
            File.Delete archiveFile
        else
            failwithf "Archive already exists for %s. Use --force to replace it." feature

    use archive = ZipFile.Open(archiveFile, ZipArchiveMode.Create)

    for file in files do
        let relativeInsideReadiness = Path.GetRelativePath(readinessPath feature, file) |> normalizePath
        let entryName = $"specs/{feature}/readiness/{relativeInsideReadiness}"
        let entry = archive.CreateEntry(entryName, CompressionLevel.Optimal)
        // ZIP DOS timestamps cannot represent pre-1980 dates, so a 1970 UnixEpoch
        // throws ArgumentOutOfRangeException. Use the minimum valid ZIP timestamp
        // (1980-01-01) — still a fixed value, so archives stay deterministic.
        entry.LastWriteTime <- DateTimeOffset(1980, 1, 1, 0, 0, 0, TimeSpan.Zero)

        use source = File.OpenRead file
        use target = entry.Open()
        source.CopyTo target

let writeReadmes feature fileCount archiveBytes sha =
    let archiveReadme =
        [ $"# {feature} Readiness Archive"
          ""
          $"Historical readiness artifacts for `{feature}` are compressed in `readiness.zip`."
          ""
          $"- archived-files: {fileCount}"
          $"- archive-bytes: {archiveBytes}"
          $"- sha256: `{sha}`"
          $"- restore example: `python3 -m zipfile -e specs/archive/{feature}/readiness.zip .`"
          "- policy: audit context only; does not satisfy current pass/fail gates." ]
        |> String.concat Environment.NewLine

    let liveReadme =
        [ "# Archived Readiness Evidence"
          ""
          $"The detailed readiness artifacts for `{feature}` were compressed to reduce active checkout search noise."
          ""
          $"- archive: `specs/archive/{feature}/readiness.zip`"
          $"- archive index: `specs/archive/{feature}/README.md`"
          $"- archived-files: {fileCount}"
          $"- sha256: `{sha}`"
          "- status: historical audit context only; not current gate evidence." ]
        |> String.concat Environment.NewLine

    File.WriteAllText(archiveReadmePath feature, archiveReadme + Environment.NewLine)
    Directory.CreateDirectory(readinessPath feature) |> ignore
    File.WriteAllText(Path.Combine(readinessPath feature, "README.md"), liveReadme + Environment.NewLine)

type ArchiveManifestRow =
    { feature: string
      files: int
      archive_bytes: int64
      sha256: string }

let countZipFiles path =
    use archive = ZipFile.OpenRead path

    archive.Entries
    |> Seq.filter (fun entry -> not (String.IsNullOrWhiteSpace entry.Name))
    |> Seq.length

let refreshArchiveManifest () =
    let archiveRoot = repoPath "specs/archive"

    let rows =
        if Directory.Exists archiveRoot then
            Directory.GetFiles(archiveRoot, "readiness.zip", SearchOption.AllDirectories)
            |> Array.map (fun path ->
                let feature = DirectoryInfo(Path.GetDirectoryName path).Name

                { feature = feature
                  files = countZipFiles path
                  archive_bytes = FileInfo(path).Length
                  sha256 = sha256OfFile path })
            |> Array.sortBy (fun row -> row.feature)
        else
            [||]

    let json = JsonSerializer.Serialize(rows, JsonSerializerOptions(WriteIndented = true))
    File.WriteAllText(repoPath "specs/archive/readiness-archives.json", json + Environment.NewLine)

let runProcess (command: string) (arguments: string) =
    let psi = ProcessStartInfo(command, arguments)
    psi.WorkingDirectory <- repositoryRoot
    psi.RedirectStandardOutput <- true
    psi.RedirectStandardError <- true
    psi.UseShellExecute <- false

    use proc = Process.Start psi
    let stdout = proc.StandardOutput.ReadToEnd()
    let stderr = proc.StandardError.ReadToEnd()
    proc.WaitForExit()

    if not (String.IsNullOrWhiteSpace stdout) then
        printf "%s" stdout

    if proc.ExitCode <> 0 then
        if not (String.IsNullOrWhiteSpace stderr) then
            eprintf "%s" stderr

        failwithf "%s %s exited with %d" command arguments proc.ExitCode

let archiveFeature feature =
    validateFeature feature

    let readiness = readinessPath feature
    let files = filesInReadiness feature

    if not (Directory.Exists readiness) then
        printfn "SKIP %s: no readiness directory" feature
    elif List.isEmpty files then
        printfn "SKIP %s: readiness directory is empty" feature
    elif hasOnlyArchiveStub feature && File.Exists(zipPath feature) && not options.Force then
        printfn "SKIP %s: already archived" feature
    else
        let archiveFile = zipPath feature

        if File.Exists archiveFile && not options.Force then
            failwithf "Archive already exists for %s. Use --force to replace it." feature

        printfn "%s %s: %d readiness file(s) -> %s" (if options.DryRun then "DRY-RUN" else "ARCHIVE") feature files.Length (normalizePath (Path.GetRelativePath(repositoryRoot, archiveFile)))

        if not options.DryRun then
            createZip feature files
            let sha = sha256OfFile archiveFile
            let archiveBytes = FileInfo(archiveFile).Length

            Directory.Delete(readiness, true)
            writeReadmes feature files.Length archiveBytes sha

for feature in selectedFeatures do
    archiveFeature feature

if not options.DryRun then
    refreshArchiveManifest ()

    if not options.SkipInventoryRefresh then
        runProcess "dotnet" "fsi scripts/archive-readiness-inventory.fsx"
        runProcess "dotnet" "fsi scripts/stale-readiness-reference-scan.fsx"

printfn "Readiness archive processing complete."
