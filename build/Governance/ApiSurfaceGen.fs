module FS.Skia.UI.Build.ApiSurfaceGen

open System
open FS.Skia.UI.Build.Capabilities

type ApiSurfaceEntry =
    { PackageId: string
      PkgLeaf: string
      SourceFsi: string
      EmittedRelPath: string
      ProjectRelPath: string
      Profiles: string list }

let emittedRoot = "template/base/docs/api-surface"
let projectRoot = "docs/api-surface"

// `FS.Skia.UI.Scene` -> `Scene`; a packageId without the prefix degrades to its last
// dotted segment so an unexpected id still produces a stable directory name.
let private leafOf (packageId: string) =
    let prefix = "FS.Skia.UI."

    if packageId.StartsWith(prefix, StringComparison.Ordinal) then
        packageId.Substring(prefix.Length)
    else
        match packageId.LastIndexOf('.') with
        | -1 -> packageId
        | index -> packageId.Substring(index + 1)

let private fileName (contract: string) =
    let normalized = contract.Replace('\\', '/')

    match normalized.LastIndexOf('/') with
    | -1 -> normalized
    | index -> normalized.Substring(index + 1)

// A capability contributes api-surface entries when it ships a real public `.fsi`:
// it names a package, is not flagged non-runtime, and its contracts are real paths
// (not the `no-public-surface` sentinel the samples capability uses).
let private hasEmittableSurface (row: CapabilityRow) =
    row.PackageId.IsSome
    && not row.NonRuntime
    && not (row.Contracts |> List.exists (fun c -> c = "no-public-surface"))

let plan (capabilities: CapabilityRow list) : ApiSurfaceEntry list =
    capabilities
    |> List.filter hasEmittableSurface
    |> List.collect (fun row ->
        let packageId = row.PackageId |> Option.defaultValue row.Id
        let leaf = leafOf packageId

        row.Contracts
        |> List.map (fun contract ->
            let name = fileName contract

            { PackageId = packageId
              PkgLeaf = leaf
              SourceFsi = contract.Replace('\\', '/')
              EmittedRelPath = sprintf "%s/%s/%s" emittedRoot leaf name
              ProjectRelPath = sprintf "%s/%s/%s" projectRoot leaf name
              Profiles = row.Profiles }))
    |> List.sortBy (fun e -> e.EmittedRelPath)
    |> List.distinctBy (fun e -> e.EmittedRelPath)

let currency
    (entries: ApiSurfaceEntry list)
    (readBytes: string -> byte[] option)
    (existingEmitted: string list)
    : string list =
    let expected = entries |> List.map (fun e -> e.EmittedRelPath) |> Set.ofList

    let driftPerEntry =
        entries
        |> List.collect (fun entry ->
            match readBytes entry.SourceFsi with
            | None -> [ sprintf "api-surface source missing: %s (required by %s)" entry.SourceFsi entry.PackageId ]
            | Some sourceBytes ->
                match readBytes entry.EmittedRelPath with
                | None ->
                    [ sprintf
                          "api-surface stale: %s is not emitted — regenerate via ./fake.sh build -t RefreshSurfaceBaselines"
                          entry.EmittedRelPath ]
                | Some emittedBytes when emittedBytes <> sourceBytes ->
                    [ sprintf
                          "api-surface stale: %s differs from %s — regenerate via ./fake.sh build -t RefreshSurfaceBaselines"
                          entry.EmittedRelPath
                          entry.SourceFsi ]
                | Some _ -> [])

    let orphans =
        existingEmitted
        |> List.filter (fun path -> not (expected.Contains path))
        |> List.map (fun path ->
            sprintf
                "api-surface orphan: %s has no capability source — regenerate via ./fake.sh build -t RefreshSurfaceBaselines"
                path)

    driftPerEntry @ orphans
