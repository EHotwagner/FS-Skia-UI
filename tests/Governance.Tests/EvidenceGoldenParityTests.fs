module EvidenceGoldenParityTests

// T009 (US1): golden-fixture byte-diff parity. The compiled F# evidence engine
// MUST reproduce the Python engine's task-graph.json / task-graph.md /
// audit-counts.txt and the five scan outputs byte-for-byte across the committed
// 036/037/038 golden fixtures (SC-001/SC-001a). DiffPlex renders the first
// divergence when a diff is non-zero.

open System.IO
open System.Text
open Expecto
open DiffPlex.DiffBuilder
open DiffPlex.DiffBuilder.Model
open GovernanceTestSupport
open FS.Skia.UI.Build.Evidence

let private repoRoot = repositoryRoot

let private buildInputs (feature: string) : EvidenceInputs =
    let featDir = Path.Combine(repoRoot, "specs", feature)
    let readinessDir = Path.Combine(featDir, "readiness")
    let readinessFiles =
        if Directory.Exists readinessDir then
            Directory.GetFiles(readinessDir, "*", SearchOption.AllDirectories)
            |> Array.map (fun p -> p.Substring(readinessDir.Length + 1).Replace('\\', '/'), File.ReadAllText p)
            |> List.ofArray
        else
            []
    let featureText =
        [ "spec.md"; "plan.md"; "tasks.md" ]
        |> List.map (fun n ->
            let p = Path.Combine(featDir, n)
            if File.Exists p then File.ReadAllText p else "")
        |> String.concat "\n"
    let auditStatusFiles =
        readinessFiles
        |> List.filter (fun (rel, c) ->
            rel.EndsWith ".md"
            && c.Contains "```audit-status"
            && not (rel.ToLowerInvariant().StartsWith "audit-fixtures/")
            && not (rel.ToLowerInvariant().StartsWith "audit-rejections/"))
    let slePath = Path.Combine(readinessDir, "skill-loading-evidence.md")
    { FeatureName = feature
      TasksMd = File.ReadAllText(Path.Combine(featDir, "tasks.md"))
      DepsYml = File.ReadAllText(Path.Combine(featDir, "tasks.deps.yml"))
      Registry = SkillRegistry.build repoRoot
      SkillLoadingEvidence = (if File.Exists slePath then Some(File.ReadAllText slePath) else None)
      ResolvedExists = (fun p -> File.Exists(if Path.IsPathRooted p then p else Path.Combine(repoRoot, p)))
      Canonicalize = (fun p -> Path.GetFullPath(if Path.IsPathRooted p then p else Path.Combine(repoRoot, p)))
      RecordedFeature = Some feature
      Scan = { ReadinessDir = readinessDir; FeatureText = featureText; ReadinessFiles = readinessFiles }
      AuditStatusFiles = auditStatusFiles
      PatternsYml = File.ReadAllText(Path.Combine(repoRoot, ".specify/extensions/evidence/audit-patterns.yml"))
      UnifiedDiff = "" }

let private firstDivergence (golden: string) (produced: string) : string =
    let diff = InlineDiffBuilder.Diff(golden, produced)
    diff.Lines
    |> Seq.filter (fun l -> l.Type <> ChangeType.Unchanged)
    |> Seq.truncate 6
    |> Seq.map (fun l -> sprintf "%A: %s" l.Type l.Text)
    |> String.concat "\n"

let private assertByteParity (feature: string) (artifactName: string) (goldenRel: string) (produced: string) =
    let goldenBytes = File.ReadAllBytes(fullPath goldenRel)
    let producedBytes = Encoding.UTF8.GetBytes produced
    if goldenBytes <> producedBytes then
        let golden = Encoding.UTF8.GetString goldenBytes
        failtestf
            "%s/%s diverged from golden (gold=%d bytes, produced=%d bytes):\n%s"
            feature artifactName goldenBytes.Length producedBytes.Length (firstDivergence golden produced)

let private features =
    [ "036-archive-readiness-api-docs"; "037-authoring-audit-robustness"; "038-authoring-guidance-consistency" ]

[<Tests>]
let evidenceGoldenParityTests =
    testList
        "US1 evidence engine byte-parity"
        [ for feature in features do
              test (sprintf "%s graph + audit artifacts are byte-identical to the Python golden fixtures" feature) {
                  let inputs = buildInputs feature
                  let _, graphArts = Engine.runGraph inputs
                  let _, auditArts = Engine.runAudit inputs
                  let goldDir = sprintf "tests/Governance.Tests/fixtures/evidence-golden/%s" feature
                  assertByteParity feature "task-graph.json" (goldDir + "/task-graph.json") graphArts.TaskGraphJson
                  assertByteParity feature "task-graph.md" (goldDir + "/task-graph.md") graphArts.TaskGraphMd
                  assertByteParity feature "audit-counts.txt" (goldDir + "/audit-counts.txt") auditArts.AuditCounts
                  assertByteParity feature "readiness-contract-hits.json" (goldDir + "/scans/readiness-contract-hits.json") auditArts.ReadinessContractHits
                  assertByteParity feature "persistent-launch-hits.json" (goldDir + "/scans/persistent-launch-hits.json") auditArts.PersistentLaunchHits
                  assertByteParity feature "persistent-gui-runtime-hits.json" (goldDir + "/scans/persistent-gui-runtime-hits.json") auditArts.PersistentGuiRuntimeHits
                  assertByteParity feature "window-visibility-hits.json" (goldDir + "/scans/window-visibility-hits.json") auditArts.WindowVisibilityHits
                  assertByteParity feature "diff-scan-hits.json" (goldDir + "/scans/diff-scan-hits.json") auditArts.DiffScanHits
              } ]
