module BuildTemplateValidation

open System
open System.IO
open System.IO.Compression
open BuildPaths

let latestTemplatePackage artifactDir =
    let packages =
        if Directory.Exists artifactDir then
            Directory.GetFiles(artifactDir, "FS.Skia.UI.Template.*.nupkg")
        else
            Array.empty

    packages
    |> Array.sortByDescending File.GetLastWriteTimeUtc
    |> Array.tryHead

let validateTemplatePackageEntries artifactDir outputPath required forbiddenPrefixes =
    let package =
        latestTemplatePackage artifactDir
        |> Option.defaultWith (fun () -> failwithf "No template package found in %s" artifactDir)

    use archive = ZipFile.OpenRead package

    let entries =
        archive.Entries
        |> Seq.map (fun entry -> entry.FullName.Replace('\\', '/'))
        |> Seq.toList

    required
    |> List.iter (fun requiredEntry ->
        if entries |> List.contains requiredEntry |> not then
            failwithf "Template package is missing %s" requiredEntry)

    entries
    |> List.iter (fun entry ->
        forbiddenPrefixes
        |> List.iter (fun prefix ->
            if entry.StartsWith(prefix, StringComparison.Ordinal) then
                failwithf "Template package contains excluded source-only artifact %s" entry))

    let report =
        [ "# Template Package Contents"
          ""
          $"Package: `{package}`"
          ""
          "Required entries verified:"
          yield! required |> List.map (fun entry -> $"- `{entry}`")
          ""
          $"Total entries: {entries.Length}" ]

    ensureParent outputPath
    File.WriteAllText(outputPath, String.concat Environment.NewLine report + Environment.NewLine)
