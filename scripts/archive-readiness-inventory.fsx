open System
open System.IO
open System.IO.Compression

let repositoryRoot =
    Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, ".."))

let repoPath (relative: string) =
    Path.Combine(repositoryRoot, relative.Replace('/', Path.DirectorySeparatorChar))

let featureId = "036-archive-readiness-api-docs"
let readinessRoot = repoPath $"specs/{featureId}/readiness"

Directory.CreateDirectory readinessRoot |> ignore

type ArchiveRow =
    { FeatureId: string
      Path: string
      Classification: string
      ArchivalMarker: string
      Rationale: string
      PreservationStatus: string
      Owner: string
      ReplacementPath: string }

let historicalFeatureDirectories =
    Directory.GetDirectories(repoPath "specs")
    |> Array.map (fun path -> DirectoryInfo(path).Name)
    |> Array.filter (fun name -> name <> featureId && Char.IsDigit(name[0]))
    |> Array.sort

let archivedFeatureDirectories =
    let archiveRoot = repoPath "specs/archive"

    if Directory.Exists archiveRoot then
        Directory.GetFiles(archiveRoot, "readiness.zip", SearchOption.AllDirectories)
        |> Array.map (fun zipPath -> DirectoryInfo(Path.GetDirectoryName zipPath).Name)
        |> Set.ofArray
    else
        Set.empty

let liveReadinessRows =
    historicalFeatureDirectories
    |> Array.collect (fun feature ->
        let readiness = repoPath $"specs/{feature}/readiness"
        if archivedFeatureDirectories.Contains feature then
            [||]
        elif Directory.Exists readiness then
            Directory.EnumerateFiles(readiness, "*", SearchOption.AllDirectories)
            |> Seq.filter (fun path -> Path.GetFileName(path) <> "README.md")
            |> Seq.truncate 80
            |> Seq.map (fun path ->
                let relative = Path.GetRelativePath(repositoryRoot, path).Replace('\\', '/')
                { FeatureId = feature
                  Path = relative
                  Classification = "archived"
                  ArchivalMarker = "historical-readiness-audit-context"
                  Rationale = "Historical feature evidence is retained for traceability but excluded from current gate pass/fail evidence."
                  PreservationStatus = "retained in place"
                  Owner = "feature"
                  ReplacementPath = $"specs/{featureId}/readiness/current-evidence-map.md" })
            |> Seq.toArray
        else
            [||])
    |> Array.toList

let archivedZipRows =
    let archiveRoot = repoPath "specs/archive"

    if Directory.Exists archiveRoot then
        Directory.GetFiles(archiveRoot, "readiness.zip", SearchOption.AllDirectories)
        |> Array.collect (fun zipPath ->
            let feature = DirectoryInfo(Path.GetDirectoryName zipPath).Name
            use archive = ZipFile.OpenRead zipPath

            archive.Entries
            |> Seq.filter (fun entry -> not (String.IsNullOrWhiteSpace entry.Name))
            |> Seq.map (fun entry ->
                let archiveRelative = Path.GetRelativePath(repositoryRoot, zipPath).Replace('\\', '/')

                { FeatureId = feature
                  Path = $"{archiveRelative}!/{entry.FullName}"
                  Classification = "archived"
                  ArchivalMarker = "compressed-historical-readiness-audit-context"
                  Rationale = "Historical feature evidence is compressed for checkout/search hygiene and retained for traceability, but excluded from current gate pass/fail evidence."
                  PreservationStatus = "compressed in place"
                  Owner = "feature"
                  ReplacementPath = $"specs/{featureId}/readiness/current-evidence-map.md" })
            |> Seq.toArray)
        |> Array.toList
    else
        []

let readinessRows =
    archivedZipRows @ liveReadinessRows

let retainedRows =
    [ { FeatureId = "035-api-discovery-names"
        Path = "specs/035-api-discovery-names/readiness/package/api-reference/"
        Classification = "retained"
        ArchivalMarker = "source-shaped-package-api-reference"
        Rationale = "The package API reference introduced by feature 035 remains authoritative supporting material for agent authoring until this feature records a replacement decision."
        PreservationStatus = "retained in place"
        Owner = "package"
        ReplacementPath = "none" }
      { FeatureId = "035-api-discovery-names"
        Path = "specs/035-api-discovery-names/readiness/package-surfaces/"
        Classification = "retained"
        ArchivalMarker = "package-surface-supporting-evidence"
        Rationale = "Package surface outputs support package API reference validation but do not satisfy current feature gates by themselves."
        PreservationStatus = "retained in place"
        Owner = "package"
        ReplacementPath = $"specs/{featureId}/readiness/api-reference-generator-evaluation.md" } ]

let currentRows =
    [ "archive-inventory.md", "Archive classification and preservation policy", "./fake.sh build -t EvidenceAudit"
      "current-evidence-map.md", "Authoritative current-gate evidence map", "./fake.sh build -t EvidenceAudit"
      "stale-reference-scan.md", "Active-surface stale reference scan", "./fake.sh build -t GeneratedGuidanceCheck"
      "api-reference-generator-evaluation.md", "API reference generator decision", "dotnet test tests/Package.Tests/Package.Tests.fsproj"
      "fsharp-formatting-spike.md", "FSharp.Formatting/fsdocs spike or blocker", "dotnet test tests/Package.Tests/Package.Tests.fsproj"
      "generated-guidance-check.md", "Generated guidance validation", "./fake.sh build -t GeneratedGuidanceCheck"
      "evidence-graph.md", "Task graph validation summary", "./fake.sh build -t EvidenceGraph"
      "evidence-audit.md", "Evidence audit summary", "./fake.sh build -t EvidenceAudit" ]

let table rows =
    [ yield "| feature-id | path | classification | archival-marker | rationale | preservation-status | owner | replacement-path |"
      yield "|------------|------|----------------|-----------------|-----------|---------------------|-------|------------------|"
      yield!
          rows
          |> List.map (fun row ->
              $"| {row.FeatureId} | `{row.Path}` | {row.Classification} | {row.ArchivalMarker} | {row.Rationale} | {row.PreservationStatus} | {row.Owner} | `{row.ReplacementPath}` |") ]

let archiveInventory =
    [ yield "# Archive Inventory"
      yield ""
      yield "## Classification Policy"
      yield ""
      yield "Historical readiness files are retained as audit context. When compressed archives exist under `specs/archive/<feature>/readiness.zip`, archived rows point to zip members instead of live checkout files so normal repository search does not scan old logs. Archived rows must not be presented as satisfying current package, template, generated-product, or audit gates. Retained package reference material remains supporting evidence when the current evidence map names it explicitly."
      yield ""
      yield "## Current Evidence"
      yield ""
      yield "| current-path | purpose | verification-command |"
      yield "|--------------|---------|----------------------|"
      yield!
          currentRows
          |> List.map (fun (fileName, purpose, command) ->
              $"| `specs/{featureId}/readiness/{fileName}` | {purpose} | `{command}` |")
      yield ""
      yield "## Archived In Place"
      yield ""
      yield! table readinessRows
      yield ""
      yield "## Replaced Or Retained"
      yield ""
      yield! table retainedRows
      yield ""
      yield "## Removable Candidates"
      yield ""
      yield "| feature-id | path | classification | archival-marker | rationale | preservation-status | owner | replacement-path |"
      yield "|------------|------|----------------|-----------------|-----------|---------------------|-------|------------------|"
      yield $"| {featureId} | `none` | removable | no-removal-candidate | No current artifact is identified as audit-safe to delete. | retained in place | governance | `none` |"
      yield ""
      yield "## Informational Historical Findings"
      yield ""
      yield "- Historical readiness evidence may be cited to explain prior decisions, unsupported-host classifications, synthetic disclosures, command logs, or previous audit reports."
      yield "- Current validation must cite the current evidence map and current feature readiness files." ]
    |> String.concat Environment.NewLine

File.WriteAllText(Path.Combine(readinessRoot, "archive-inventory.md"), archiveInventory + Environment.NewLine)

let currentEvidenceMap =
    [ yield "# Current Evidence Map"
      yield ""
      yield "Historical readiness cannot satisfy current package, template, generated-product, or audit gates. Archived material may be cited only as audit context unless this map marks it as supporting evidence."
      yield ""
      yield "| gate | current-path | supporting-paths | archived-path-policy | required-fields | verification-command |"
      yield "|------|--------------|------------------|----------------------|-----------------|----------------------|"
      yield $"| Archive classification | `specs/{featureId}/readiness/archive-inventory.md` | `specs/{featureId}/contracts/archive-classification-contract.md` | Historical readiness is archived in place and excluded from current gates. | feature id, path, classification, archival marker, rationale, owner, preservation status, replacement path | `dotnet fsi scripts/archive-readiness-inventory.fsx` |"
      yield $"| Current evidence routing | `specs/{featureId}/readiness/current-evidence-map.md` | `specs/{featureId}/plan.md` | Historical references are supporting only when named here. | gate, current path, supporting paths, verification command, archived policy | `dotnet fsi scripts/archive-readiness-inventory.fsx` |"
      yield $"| Stale reference scan | `specs/{featureId}/readiness/stale-reference-scan.md` | `specs/{featureId}/readiness/stale-reference-scan.json` | Active-surface citations to archived readiness as pass/fail evidence are blocking. | source path, referenced path, scan area, severity, reason, replacement path, line, next action | `dotnet fsi scripts/stale-readiness-reference-scan.fsx` |"
      yield $"| Generated guidance | `specs/{featureId}/readiness/generated-guidance-check.md` | `docs/generated-apps.md`, `docs/template-profile.md`, `template/base/README.md`, `template/base/docs/product.md` | Generated guidance must describe archived readiness as audit context only. | guidance surface, required term, forbidden advice, replacement instruction | `./fake.sh build -t GeneratedGuidanceCheck` |"
      yield $"| API reference decision | `specs/{featureId}/readiness/api-reference-generator-evaluation.md` | `specs/035-api-discovery-names/readiness/package/api-reference/` | Feature 035 package API references are retained supporting evidence, not current feature pass/fail evidence. | package id, generator, dimensions, decision, next action | `dotnet test tests/Package.Tests/Package.Tests.fsproj --filter Package` |"
      yield $"| FSharp.Formatting spike | `specs/{featureId}/readiness/fsharp-formatting-spike.md` | `specs/{featureId}/readiness/logs/fsdocs-spike.txt` | fsdocs output is secondary/hybrid unless the decision record marks it authoritative. | command, log path, blocker or sample path, reason, next action | local fsdocs spike command or documented blocker |"
      yield $"| Evidence graph | `specs/{featureId}/readiness/evidence-graph.md` | `specs/{featureId}/readiness/task-graph.json` | Historical task graphs are informational only. | command, artifact path, graph result, synthetic propagation note | `./fake.sh build -t EvidenceGraph` |"
      yield $"| Evidence audit | `specs/{featureId}/readiness/evidence-audit.md` | `specs/{featureId}/readiness/task-graph.json` | Historical audit reports are informational only. | command, artifact path, diff-scan result, next action | `./fake.sh build -t EvidenceAudit` |" ]
    |> String.concat Environment.NewLine

File.WriteAllText(Path.Combine(readinessRoot, "current-evidence-map.md"), currentEvidenceMap + Environment.NewLine)

printfn "Wrote archive inventory and current evidence map to %s" readinessRoot
