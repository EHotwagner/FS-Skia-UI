module FS.Skia.UI.Build.Front.Governance

open System
open System.IO
open System.Net.Http
open System.Text.RegularExpressions
open BuildPaths
open BuildProcess
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Engine.Model
open FS.Skia.UI.Build.Engine.Update
open FS.Skia.UI.Build.Front.Helpers

// Relocated verbatim from build.fsx (feature 045): workflow self-check, skill-tree +
// constitution regeneration, route selection, and the evidence graph/audit gate drivers.

let workflowSelfCheck (root: string) =
    let model, initEffects = init root
    let _, restoreEffects = update (StartTarget Targets.Restore) model
    let _, verifyPreflightEffects = update (StartTarget Targets.VerifyPreflight) model
    let _, ciPreflightEffects = update (StartTarget Targets.CiPreflight) model
    let _, verifyEffects = update (StartTarget Targets.Verify) model
    let _, focusedEffects = update (StartTarget Targets.ControlsRenderingCheck) model
    let _, templatePackEffects = update (StartTarget Targets.TemplatePack) model
    let _, templateSmokeEffects = update (StartTarget Targets.TemplateSmoke) model

    if initEffects |> List.exists (function EnsureDirectory path when path = model.LogDir -> true | _ -> false) |> not then
        failwith "init must request log directory creation"

    if restoreEffects |> List.exists (function RunDotnetAction(label, _, _, _, _, _) when label = "dotnet restore" -> true | _ -> false) |> not then
        failwith "Restore must emit a dotnet restore workflow effect"

    if verifyPreflightEffects |> List.exists (function CollectProcessHealth("Verify", _, _) -> true | _ -> false) |> not then
        failwith "VerifyPreflight must emit process-health collection before broad work"

    if verifyPreflightEffects |> List.exists (function ValidateRunnerBootstrap("Verify", _, _) -> true | _ -> false) |> not then
        failwith "VerifyPreflight must emit bootstrap validation before broad work"

    if verifyPreflightEffects |> List.exists (function RequireFiles("verify readiness preflight artifact set", _) -> true | _ -> false) |> not then
        failwith "VerifyPreflight must require readiness impact files before broad work"

    if ciPreflightEffects |> List.exists (function CollectProcessHealth("Ci", _, _) -> true | _ -> false) |> not then
        failwith "CiPreflight must emit process-health collection before broad work"

    if focusedEffects |> List.exists (function CheckFocusedGateAssumptions contract when contract.TargetName = "ControlsRenderingCheck" -> true | _ -> false) |> not then
        failwith "Focused gates must emit stale build/restore assumption checks"

    if focusedEffects |> List.exists (function WriteFocusedGateSummary contract when contract.TargetName = "ControlsRenderingCheck" -> true | _ -> false) |> not then
        failwith "Focused gates must emit focused gate summaries"

    if templatePackEffects |> List.exists (function ValidateTemplatePackage _ -> true | _ -> false) |> not then
        failwith "TemplatePack must validate the local template package artifact"

    if templateSmokeEffects |> List.exists (function ScanGeneratedProjects _ -> true | _ -> false) |> not then
        failwith "TemplateSmoke must scan generated projects and run generated Dev"

    if verifyEffects |> List.exists (function RequireFiles("v1 plus v2 verification artifact set", _) -> true | _ -> false) |> not then
        failwith "Verify must require the v1 plus v2 artifact set"

    let _, completedEffects = update (TargetCompleted "Restore") model

    if completedEffects <> [] then
        failwith "TargetCompleted must be a pure state transition with no effects"

    printfn "process reliability self-check: preflight, bootstrap, verdict, and focused gate effects are pure"

// BUILD SECTION: interpreter

// Feature 044 — skill-tree enumeration edge (US1). Reads every file under a tree root
// into a SkillTreeGen.SkillFile (repo-relative, forward-slash RelPath + raw bytes), sorted
// for determinism. This is the only file I/O for the generator; the pure plan/currency
// live in the library (Principle IV). No diff/cmp/sha256sum/symlink shelling — in-process.
let enumerateSkillTree (root: string) (treeRelRoot: string) : FS.Skia.UI.Build.SkillTreeGen.SkillFile list =
    let fullRoot = path (root :: (treeRelRoot.Split('/') |> List.ofArray))

    if not (Directory.Exists fullRoot) then
        []
    else
        Directory.EnumerateFiles(fullRoot, "*", SearchOption.AllDirectories)
        |> Seq.sort
        |> Seq.map (fun full ->
            let rel = (Path.GetRelativePath(root, full)).Replace('\\', '/')
            let segments = rel.Split('/')

            let slug =
                match segments |> Array.tryFindIndex (fun s -> s = "skills") with
                | Some i when i + 1 < segments.Length -> segments.[i + 1]
                | _ -> rel

            let file: FS.Skia.UI.Build.SkillTreeGen.SkillFile =
                { Slug = slug
                  RelPath = rel
                  Bytes = File.ReadAllBytes full }

            file)
        |> Seq.toList

// Feature 044 — SkillSyncCheck (reframed): generation-currency over the whole tree.
// `.claude/skills` MUST be a current regeneration of canonical `.agents/skills` across
// every enumerated file (coverage by enumeration, no allowlist). Fails with an actionable
// "regenerate via RefreshSurfaceBaselines" diagnostic, never a bare "A and B differ".
let runSkillSyncGate (model: BuildModel) =
    let canonical = enumerateSkillTree model.RepositoryRoot FS.Skia.UI.Build.SkillTreeGen.canonicalRoot
    let derived = enumerateSkillTree model.RepositoryRoot FS.Skia.UI.Build.SkillTreeGen.derivedRoot
    let plan = FS.Skia.UI.Build.SkillSync.planFromCanonical canonical
    let currency = FS.Skia.UI.Build.SkillSync.currency plan derived
    let report = FS.Skia.UI.Build.SkillSync.renderReport plan currency
    let reportPath = path [ model.ReadinessDir; "skill-sync-check.md" ]
    let logPath = path [ model.LogDir; "skill-sync-check.txt" ]
    ensureParent reportPath
    File.WriteAllText(reportPath, report)
    ensureParent logPath
    File.WriteAllText(logPath, report)

    if not (FS.Skia.UI.Build.SkillSync.isCurrent currency) then
        failwith (FS.Skia.UI.Build.SkillSync.renderFailureMessage currency)

// Feature 058 — SkillQualityCheck (US1, FR-001/FR-003): enumerate every FS-authored
// SKILL.md across the in-scope skill homes, check each against the section rubric, write
// the per-skill report, and fail loud naming each skill + missing section. The vendored
// `speckit-*` tree is excluded by `SkillQuality.isInScope` (FR-004).
let runSkillQualityCheck (model: BuildModel) =
    let root = model.RepositoryRoot

    let subRoots =
        [ ".agents/skills"
          "src"
          "template/product-skills"
          "template/fragments"
          "template/feedback"
          "template/base/.agents/skills" ]

    let files =
        subRoots
        |> List.collect (fun sub ->
            let dir = path (root :: (sub.Split('/') |> List.ofArray))

            if Directory.Exists dir then
                Directory.EnumerateFiles(dir, "SKILL.md", SearchOption.AllDirectories)
                |> List.ofSeq
            else
                [])
        |> List.distinct

    let parsed =
        files
        |> List.map (fun full ->
            let rel = (Path.GetRelativePath(root, full)).Replace('\\', '/')
            SkillQuality.parse rel (File.ReadAllText full))
        |> List.filter (fun s -> s.InScope)
        |> List.distinctBy (fun s -> s.RelPath)

    let report = SkillQuality.renderReport parsed
    let reportPath = path [ model.ReadinessDir; "skill-quality-check.md" ]
    let logPath = path [ model.LogDir; "skill-quality-check.txt" ]
    ensureParent reportPath
    File.WriteAllText(reportPath, report)
    ensureParent logPath
    File.WriteAllText(logPath, report)

    let findings = SkillQuality.checkCorpus parsed

    if not (List.isEmpty findings) then
        failwith (
            sprintf
                "SkillQualityCheck failed: %d missing rubric section(s) across in-scope skills.\n%s"
                (List.length findings)
                (Findings.renderDetail findings)
        )

// Feature 060 (FR-003) — api-surface regeneration edge. Plan the emitted tree from the
// capability catalog `contracts:`, write each emitted `.fsi` byte-identical to its source,
// and prune any orphan emitted file/dir with no capability source so the surface a consumer
// reads in a generated project never drifts from the framework signatures.
let regenerateApiSurface (model: BuildModel) =
    let root = model.RepositoryRoot
    let toFull (rel: string) = repoRelPath root rel
    let entries = ApiSurfaceGen.plan (Capabilities.readCatalog model.CapabilityCatalogPath)

    let expected =
        entries |> List.map (fun e -> (toFull e.EmittedRelPath).Replace('\\', '/')) |> Set.ofList

    for entry in entries do
        let sourceFull = toFull entry.SourceFsi

        if File.Exists sourceFull then
            let emittedFull = toFull entry.EmittedRelPath
            ensureParent emittedFull
            File.WriteAllBytes(emittedFull, File.ReadAllBytes sourceFull)

    let emittedRootFull = toFull ApiSurfaceGen.emittedRoot

    if Directory.Exists emittedRootFull then
        for existing in Directory.EnumerateFiles(emittedRootFull, "*", SearchOption.AllDirectories) do
            if not (expected.Contains(existing.Replace('\\', '/'))) then
                File.Delete existing

        for dir in Directory.EnumerateDirectories(emittedRootFull, "*", SearchOption.AllDirectories) |> Seq.sortDescending do
            if Directory.Exists dir && Seq.isEmpty (Directory.EnumerateFileSystemEntries dir) then
                Directory.Delete dir

// Feature 062 (FR-006) — regenerate docs/skillist-reference.md from the live
// SkillRegistry + the closed owns vocabulary. The registry build + file write are
// the I/O edge; the render is pure (currency in TargetMetadataDrift).
let regenerateSkillistReference (model: BuildModel) =
    let registry = FS.Skia.UI.Build.Evidence.SkillRegistry.build model.RepositoryRoot
    let doc =
        FS.Skia.UI.Build.SkillistReference.render registry FS.Skia.UI.Build.Evidence.Audit.ownsVocabulary
    let full = repoRelPath model.RepositoryRoot FS.Skia.UI.Build.SkillistReference.referenceDocPath
    ensureParent full
    File.WriteAllText(full, doc)

// Feature 060 (FR-004) — SkillContractPathCheck. Every capability/product skill that names
// a `docs/api-surface/...fsi` contract source must name a path the emitted tree actually
// provides; a "no DLL reflection needed" claim against an absent path is a hard failure.
let runSkillContractPathCheck (model: BuildModel) =
    let root = model.RepositoryRoot
    let entries = ApiSurfaceGen.plan (Capabilities.readCatalog model.CapabilityCatalogPath)

    let skillFiles =
        [ "template/product-skills"; "src"; ".agents/skills" ]
        |> List.collect (fun sub ->
            let dir = path (root :: (sub.Split('/') |> List.ofArray))

            if Directory.Exists dir then
                Directory.EnumerateFiles(dir, "SKILL.md", SearchOption.AllDirectories) |> List.ofSeq
            else
                [])
        |> List.filter (fun f ->
            let n = f.Replace('\\', '/')
            not (n.Contains "/obj/" || n.Contains "/bin/"))
        |> List.distinct

    let claims =
        skillFiles
        |> List.collect (fun full ->
            let rel = (Path.GetRelativePath(root, full)).Replace('\\', '/')
            SkillContractPath.parseClaims rel (File.ReadAllText full))

    let diagnostics = SkillContractPath.check entries claims
    let orphans = SkillContractPath.orphans entries claims

    let report =
        [ "# SkillContractPathCheck"
          ""
          if List.isEmpty diagnostics then
              sprintf
                  "PASS: every `docs/api-surface/...fsi` path claimed across %d skill file(s) resolves to the emitted api-surface tree (%d entries)."
                  (List.length skillFiles)
                  (List.length entries)
          else
              "FAIL: a skill names a contract source the generated project does not emit."
              ""
              yield! diagnostics |> List.map (fun d -> sprintf "- %s" d)
          if not (List.isEmpty orphans) then
              ""
              "Advisory (emitted but unclaimed):"
              yield! orphans |> List.map (fun o -> sprintf "- %s" o) ]
        |> String.concat Environment.NewLine

    let reportPath = path [ model.ReadinessDir; "skill-contract-path-check.md" ]
    let logPath = path [ model.LogDir; "skill-contract-path-check.txt" ]
    ensureParent reportPath
    File.WriteAllText(reportPath, report)
    ensureParent logPath
    File.WriteAllText(logPath, report)

    if not (List.isEmpty diagnostics) then
        failwith (
            sprintf
                "SkillContractPathCheck failed: %d skill-claimed api-surface path(s) are not emitted.\n%s"
                (List.length diagnostics)
                (String.concat Environment.NewLine diagnostics)
        )

// Feature 063 (FR-003) — SymbolCrossCheck. Run the existing analyzer
// (build/Governance/SymbolCrossCheck.fs) over the active feature's plan/data-model/tasks
// (paths derived from the feature dir — the DependencyReport pattern), print the
// `## Symbol consistency (analyze pass G)` markdown, and write it to
// readiness/symbol-cross-check.md. A command/diagnostic, not a hard merge gate:
// design-only symbols are reported for human judgment, never a false-fail.
let runSymbolCrossCheck (model: BuildModel) =
    let readArtifact name =
        let p = path [ model.FeatureDir; name ]
        if File.Exists p then File.ReadAllText p else ""

    let findings =
        SymbolCrossCheck.diff (readArtifact "plan.md") (readArtifact "data-model.md") (readArtifact "tasks.md")

    let markdown = SymbolCrossCheck.render findings

    printfn "%s" markdown

    let reportPath = path [ model.ReadinessDir; "symbol-cross-check.md" ]
    let logPath = path [ model.LogDir; "symbol-cross-check.txt" ]
    ensureParent reportPath
    File.WriteAllText(reportPath, markdown)
    ensureParent logPath
    File.WriteAllText(logPath, markdown)

// Feature 060 (FR-009) — TemplateUpdateSkillPackageCheck. The `fs-skia-template-update`
// skill's enumerated package set must equal the packable `.fsproj` set so it cannot drift
// (no phantom bare-Lib, no missing SkillSupport/Input). The packable-set discovery stays
// here; TemplateUpdatePackage.check is pure.
let runTemplateUpdatePackageCheck (model: BuildModel) =
    let root = model.RepositoryRoot

    let packableProjects =
        [ "src"; "build" ]
        |> List.collect (fun sub ->
            let dir = path [ root; sub ]

            if Directory.Exists dir then
                Directory.EnumerateFiles(dir, "*.fsproj", SearchOption.AllDirectories) |> List.ofSeq
            else
                [])
        |> List.filter (fun f ->
            let n = f.Replace('\\', '/')

            if n.Contains "/obj/" || n.Contains "/bin/" then
                false
            else
                let text = File.ReadAllText f
                text.Contains "<IsPackable>true</IsPackable>" || text.Contains "<PackageId>")
        |> List.distinct

    let packableLeaves =
        packableProjects
        |> List.map (fun f -> TemplateUpdatePackage.leafOfProject ((Path.GetRelativePath(root, f)).Replace('\\', '/')))
        |> List.distinct

    let skillRel = ".agents/skills/fs-skia-template-update/SKILL.md"
    let skillFull = repoRelPath root skillRel
    let skillText = if File.Exists skillFull then File.ReadAllText skillFull else ""

    let diagnostics = TemplateUpdatePackage.check packableLeaves skillRel skillText

    let report =
        [ "# TemplateUpdateSkillPackageCheck"
          ""
          sprintf "Packable set (%d): %s" (List.length packableLeaves) (packableLeaves |> List.sort |> String.concat ", ")
          sprintf
              "Skill step-5 feed loop (%d): %s"
              (TemplateUpdatePackage.feedLoopLeaves skillText |> List.length)
              (TemplateUpdatePackage.feedLoopLeaves skillText |> List.sort |> String.concat ", ")
          ""
          if List.isEmpty diagnostics then
              "PASS: the fs-skia-template-update skill's package enumeration equals the packable set (zero phantom, zero missing)."
          else
              "FAIL: the fs-skia-template-update skill's package enumeration has drifted from the packable set."
              ""
              yield! diagnostics |> List.map (fun d -> sprintf "- %s" d) ]
        |> String.concat Environment.NewLine

    let reportPath = path [ model.ReadinessDir; "template-update-package-check.md" ]
    let logPath = path [ model.LogDir; "template-update-package-check.txt" ]
    ensureParent reportPath
    File.WriteAllText(reportPath, report)
    ensureParent logPath
    File.WriteAllText(logPath, report)

    if not (List.isEmpty diagnostics) then
        failwith (
            sprintf
                "TemplateUpdateSkillPackageCheck failed: %d package-enumeration drift(s).\n%s"
                (List.length diagnostics)
                (String.concat Environment.NewLine diagnostics)
        )

// Feature 044 — RefreshSurfaceBaselines regeneration edge (US1). Plan the derived tree
// from canonical, write each derived file's bytes + the provenance manifest, and remove
// any orphan derived file with no canonical source so the mirror stays exact.
let regenerateSkillTree (model: BuildModel) =
    let root = model.RepositoryRoot
    let canonical = enumerateSkillTree root FS.Skia.UI.Build.SkillTreeGen.canonicalRoot
    let plan = FS.Skia.UI.Build.SkillSync.planFromCanonical canonical

    let toFull (rel: string) = path (root :: (rel.Split('/') |> List.ofArray))

    let expected =
        plan.ManifestRelPath :: (plan.Entries |> List.map (fun e -> e.DerivedRelPath))
        |> List.map (fun rel -> (toFull rel).Replace('\\', '/'))
        |> Set.ofList

    // Write the manifest + every derived file (byte-identical to canonical, FR-003).
    let manifestFull = toFull plan.ManifestRelPath
    ensureParent manifestFull
    File.WriteAllBytes(manifestFull, plan.ManifestBytes)

    for entry in plan.Entries do
        let full = toFull entry.DerivedRelPath
        ensureParent full
        File.WriteAllBytes(full, entry.Bytes)

    // Remove orphan derived files (a canonical skill removed upstream must vanish here),
    // then prune any directory left empty by that removal so no stale skill dir lingers.
    let derivedFullRoot = toFull FS.Skia.UI.Build.SkillTreeGen.derivedRoot

    if Directory.Exists derivedFullRoot then
        for existing in Directory.EnumerateFiles(derivedFullRoot, "*", SearchOption.AllDirectories) do
            if not (expected.Contains(existing.Replace('\\', '/'))) then
                File.Delete existing

        for dir in Directory.EnumerateDirectories(derivedFullRoot, "*", SearchOption.AllDirectories) |> Seq.sortDescending do
            if Directory.Exists dir && Seq.isEmpty (Directory.EnumerateFileSystemEntries dir) then
                Directory.Delete dir

// Feature 044 — RefreshSurfaceBaselines regeneration edge (US3). Extract the principle
// fragments from the constitution and splice them into each governed template's BEGIN/END
// GENERATED regions, preserving every out-of-marker byte (FR-010).
let regenerateConstitutionFragments (model: BuildModel) =
    let constitutionPath = repoRelPath model.RepositoryRoot constitutionRelPath

    if not (File.Exists constitutionPath) then
        failwithf "RefreshSurfaceBaselines: %s is missing — cannot derive principle fragments (Principle VII)." constitutionRelPath

    let fragments = ConstitutionFragments.extract (File.ReadAllText constitutionPath)

    for (relTemplate, _ids) in constitutionTemplateRegions do
        let templatePath = repoRelPath model.RepositoryRoot relTemplate

        if not (File.Exists templatePath) then
            failwithf "RefreshSurfaceBaselines: %s is missing — cannot splice constitution fragments (Principle VII)." relTemplate

        let spliced = ConstitutionFragments.splice fragments (File.ReadAllText templatePath)
        File.WriteAllText(templatePath, spliced)

// Feature 057 — RefreshSurfaceBaselines regeneration edge (US1). Splice every canonical
// GovernedBlock into each of its home files' `BEGIN/END GENERATED: gov/<id>` regions,
// preserving every out-of-marker byte (FR-006). A home file missing its marker region is
// left untouched here; TargetMetadataDrift reports it as Missing (loud, not silent).
let regenerateGovernedBlocks (model: BuildModel) =
    for block in GovernedBlocks.governedBlocks do
        for (relTarget, mode) in block.Targets do
            let targetPath = repoRelPath model.RepositoryRoot relTarget

            if File.Exists targetPath then
                let spliced = GovernedBlocks.splice block mode (File.ReadAllText targetPath)
                File.WriteAllText(targetPath, spliced)

    // Feature 057 (class 4, FR-007): render the concrete `constitution.md` and the preset
    // twin from the canonical placeholder-bearing twin. Rendered here (before the
    // skill-tree / constitution-fragment regen in RefreshSurfaceBaselines) so the
    // fragment extraction reads the fresh constitution.md.
    let canonicalConst = repoRelPath model.RepositoryRoot GovernedBlocks.constitutionCanonicalRel

    if File.Exists canonicalConst then
        let canonicalText = File.ReadAllText canonicalConst
        let concretePath = repoRelPath model.RepositoryRoot GovernedBlocks.constitutionConcreteRel
        File.WriteAllText(concretePath, GovernedBlocks.renderConstitution GovernedBlocks.Concrete canonicalText)
        let twinPath = repoRelPath model.RepositoryRoot GovernedBlocks.constitutionTwinRel
        File.WriteAllText(twinPath, GovernedBlocks.renderConstitution GovernedBlocks.Twin canonicalText)

// Feature 066 — RefreshSurfaceBaselines regeneration edge (US1, FR-002). Splice the six
// typed-catalog rows into both `src/Controls/catalog.yml` and `src/Controls/Catalog.fs`
// from the single `CatalogGen.catalogFacts` source, in ONE operation, preserving every
// byte outside the per-control `typed-catalog/<id>` markers (FR-003). A home file missing
// its marker region is left untouched here; ControlsCatalogGenerationCheck reports it as
// Missing (loud, not silent). Both files are written together so the two generated outputs
// cannot diverge (partial-regeneration edge case cannot occur).
let regenerateCatalog (model: BuildModel) =
    let ymlPath = repoRelPath model.RepositoryRoot CatalogGen.catalogYmlRel
    let fsPath = repoRelPath model.RepositoryRoot CatalogGen.catalogFsRel

    if File.Exists ymlPath then
        File.WriteAllText(ymlPath, CatalogGen.spliceYaml (File.ReadAllText ymlPath))

    if File.Exists fsPath then
        File.WriteAllText(fsPath, CatalogGen.spliceFSharp (File.ReadAllText fsPath))

// Feature 069 — RefreshSurfaceBaselines regeneration edge (US1, FR-002). Regenerate the
// whole `src/Controls/DesignTokens.fs` module from the single DTCG source
// `src/Controls/design-tokens.tokens.json`, so the DTCG document is the one edit point and
// DesignTokenDrift cannot trip on drift. The read/write is the only filesystem effect, here
// at the interpret edge; `DesignTokenGen.splice` is pure (Principle IV). A malformed/cyclic/
// incomplete DTCG source raises a loud generation failure (no partial emit) before any write.
let regenerateDesignTokens (model: BuildModel) =
    let jsonPath = repoRelPath model.RepositoryRoot DesignTokenGen.tokensJsonRel
    let fsPath = repoRelPath model.RepositoryRoot DesignTokenGen.designTokensFsRel

    if File.Exists jsonPath then
        File.WriteAllText(fsPath, DesignTokenGen.splice (File.ReadAllText jsonPath))

// Feature 042 (FR-002a, research R2): the git union-diff is read here at the `Route`
// interpreter edge so the Routing selector stays pure and unit-testable without git.
let routeGitCapture root (arguments: string) =
    try
        let startInfo = System.Diagnostics.ProcessStartInfo("git", arguments)
        startInfo.WorkingDirectory <- root
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false

        match System.Diagnostics.Process.Start startInfo with
        | null -> Error(sprintf "git %s could not be started (no process handle)" arguments)
        | started ->
            use proc = started
            let stdout = proc.StandardOutput.ReadToEnd()
            let stderr = proc.StandardError.ReadToEnd()
            proc.WaitForExit() |> ignore

            if proc.ExitCode = 0 then Ok stdout else Error(stderr.Trim())
    with ex ->
        Error ex.Message

let routeWorkingTreePaths (porcelain: string) =
    porcelain.Replace("\r\n", "\n").Split('\n')
    |> Array.toList
    |> List.collect (fun line ->
        let trimmed = line.TrimEnd()

        if trimmed.Length <= 3 then
            []
        else
            let payload = trimmed.Substring(3)
            // `git status --porcelain` renders renames as "old -> new"; take the new path.
            let arrow = payload.IndexOf(" -> ", StringComparison.Ordinal)
            if arrow >= 0 then [ payload.Substring(arrow + 4) ] else [ payload ])

let runRouteSelection root =
    let argv = System.Environment.GetCommandLineArgs() |> Array.toList
    let enforce = argv |> List.exists (fun arg -> arg = "--enforce")

    let developerClass =
        if argv |> List.exists (fun arg -> arg = "consumer-agent") then
            Routing.ConsumerAgent
        else
            Routing.FrameworkAuthor

    let mergeBaseAgainst ref =
        match routeGitCapture root (sprintf "merge-base HEAD %s" ref) with
        | Ok value when value.Trim() <> "" -> Some(value.Trim())
        | _ -> None

    let mergeBase =
        // Trunk is `main` (renamed from `master`); fall back to `master` for old checkouts.
        match mergeBaseAgainst "main" with
        | Some _ as found -> found
        | None ->
            match mergeBaseAgainst "master" with
            | Some _ as found -> found
            | None ->
                printfn "Route: could not resolve 'git merge-base HEAD main' (or master); using working-tree changes only (no branch baseline)."
                None

    let committedPaths =
        match mergeBase with
        | Some baseCommit ->
            match routeGitCapture root (sprintf "diff --name-only %s...HEAD" baseCommit) with
            | Ok value -> value.Replace("\r\n", "\n").Split('\n') |> Array.toList
            | Error message ->
                printfn "Route: 'git diff' against the merge-base failed (%s); continuing with working-tree changes." message
                []
        | None -> []

    let workingPaths =
        match routeGitCapture root "status --porcelain --untracked-files=all" with
        | Ok value -> routeWorkingTreePaths value
        | Error message ->
            printfn "Route: 'git status' failed (%s)." message
            []

    let changedPaths =
        committedPaths @ workingPaths
        |> List.map (fun p -> p.Trim().Trim('"'))
        |> List.filter (fun p -> p <> "")
        |> List.distinct

    let selection =
        Routing.selectForFeature developerClass featureId { Routing.Diff.ChangedPaths = changedPaths }

    printfn "%s" (Routing.renderSelection selection)

    if enforce then
        let present =
            selection.ExpectedArtifacts
            |> List.filter (fun artifact -> File.Exists(path [ root; artifact ]))
            |> Set.ofList

        let missing = Routing.unmetArtifacts present selection

        if not (List.isEmpty missing) then
            failwith (Routing.enforceDiagnostic selection missing)

// Feature 043 (FR-009/FR-012, Principle IV): the EvidenceGraph/EvidenceAudit gates compute
// the task DAG and merge-gate audit in-process via FS.Skia.UI.Build.Evidence. Every read
// (tasks.md, tasks.deps.yml, readiness files, the skill registry, the git diff) and every
// artifact write stays at this interpreter edge so the engine itself performs no I/O.
// Feature 048 (FR-007/008, Principle IV/VII): the PerPackageSurfaceDiff gate runs the
// additive per-package surface diff in-process. The pure diff lives in the library
// (`PerPackageSurface`); every file read (the eight `.fsi` surfaces, the committed
// baselines) and the report write stays at this interpreter edge.
let runPerPackageSurfaceDiff (model: BuildModel) =
    let baselineDir = path [ model.RepositoryRoot; "readiness"; "per-package-surface" ]
    let baselines = FS.Skia.UI.Build.PerPackageSurface.loadBaselines baselineDir
    let current = FS.Skia.UI.Build.PerPackageSurface.captureCurrent FS.Skia.UI.Build.PerPackageSurface.packagesInScope
    let outcome = FS.Skia.UI.Build.PerPackageSurface.diff baselines current
    let reportPath = path [ model.ReadinessDir; "per-package-surface-diff.md" ]
    let clean = FS.Skia.UI.Build.PerPackageSurface.runReport reportPath outcome

    if not clean then
        let drifted =
            outcome.Drifted
            |> List.map (fun d -> d.PackageId)
            |> String.concat ", "

        let missing = outcome.MissingBaselines |> String.concat ", "

        failwithf
            "PerPackageSurfaceDiff: surface drift detected (drifted: [%s]; missing baselines: [%s]). See %s and update readiness/per-package-surface/<PackageId>.fsi.txt."
            drifted
            missing
            reportPath

let private evidenceReadAll p = if File.Exists p then File.ReadAllText p else ""

let buildEvidenceInputs (model: BuildModel) (unifiedDiff: string) : FS.Skia.UI.Build.Evidence.EvidenceInputs =
    let featDir = model.FeatureDir
    let readinessDir = model.ReadinessDir
    let repoRoot = model.RepositoryRoot
    let featureName =
        Path.GetFileName(featDir.TrimEnd('/', '\\')) |> Option.ofObj |> Option.defaultValue ""
    let readinessFiles =
        if Directory.Exists readinessDir then
            Directory.GetFiles(readinessDir, "*", SearchOption.AllDirectories)
            |> Array.map (fun p -> p.Substring(readinessDir.Length + 1).Replace('\\', '/'), File.ReadAllText p)
            |> List.ofArray
        else
            []
    let featureText =
        [ "spec.md"; "plan.md"; "tasks.md" ]
        |> List.map (fun n -> evidenceReadAll (Path.Combine(featDir, n)))
        |> String.concat "\n"
    let auditStatusFiles =
        readinessFiles
        |> List.filter (fun (rel, c) ->
            rel.EndsWith ".md"
            && c.Contains "```audit-status"
            && not (rel.ToLowerInvariant().StartsWith "audit-fixtures/")
            && not (rel.ToLowerInvariant().StartsWith "audit-rejections/"))
    let slePath = Path.Combine(readinessDir, "skill-loading-evidence.md")
    { FeatureName = featureName
      TasksMd = evidenceReadAll (Path.Combine(featDir, "tasks.md"))
      DepsYml = evidenceReadAll (Path.Combine(featDir, "tasks.deps.yml"))
      Registry = FS.Skia.UI.Build.Evidence.SkillRegistry.build repoRoot
      SkillLoadingEvidence = (if File.Exists slePath then Some(File.ReadAllText slePath) else None)
      ResolvedExists = (fun p -> File.Exists(if Path.IsPathRooted p then p else Path.Combine(repoRoot, p)))
      Canonicalize = (fun p -> Path.GetFullPath(if Path.IsPathRooted p then p else Path.Combine(repoRoot, p)))
      RecordedFeature = Some featureName
      Scan = { ReadinessDir = readinessDir; FeatureText = featureText; ReadinessFiles = readinessFiles }
      AuditStatusFiles = auditStatusFiles
      PatternsYml = evidenceReadAll (Path.Combine(repoRoot, ".specify/extensions/evidence/audit-patterns.yml"))
      UnifiedDiff = unifiedDiff }

let private evidenceWrite (p: string) (content: string) =
    ensureParent p
    File.WriteAllText(p, content)

let runEvidenceGraphCheck (model: BuildModel) =
    let inputs = buildEvidenceInputs model ""
    let gr, arts = FS.Skia.UI.Build.Evidence.Engine.runGraph inputs
    evidenceWrite (path [ model.ReadinessDir; "task-graph.json" ]) arts.TaskGraphJson
    evidenceWrite (path [ model.ReadinessDir; "task-graph.md" ]) arts.TaskGraphMd
    // FR-007 (061): emit an explicit, greppable terminal verdict line so a clean
    // pass is self-evident without inspecting the exit code.
    let verdictLine = FS.Skia.UI.Build.Evidence.Render.graphVerdictLine gr
    evidenceWrite
        (path [ model.LogDir; "evidence-graph.txt" ])
        (sprintf "=== speckit.evidence.graph (in-process) ===\nfeature: %s\ntasks: %d\n%s\n" inputs.FeatureName (List.length gr.Tasks) verdictLine)
    match gr.Verdict with
    | FS.Skia.UI.Build.Evidence.GraphVerdict.Error ->
        failwithf "Evidence graph validation failed (%d errors); see %s" (List.length gr.Errors) (path [ model.ReadinessDir; "task-graph.md" ])
    | _ -> ()

let private resolveBaseRef root =
    let hasRef name =
        match routeGitCapture root (sprintf "show-ref --verify --quiet refs/heads/%s" name) with
        | Ok _ -> true
        | Error _ -> false
    if hasRef "main" then "main"
    elif hasRef "master" then "master"
    else "HEAD~1"

let runEvidenceAuditCheck root (model: BuildModel) =
    let baseRef = resolveBaseRef root
    let mergeBase =
        match routeGitCapture root (sprintf "merge-base %s HEAD" baseRef) with
        | Ok s when s.Trim() <> "" -> s.Trim()
        | _ -> baseRef
    let unifiedDiff =
        match routeGitCapture root (sprintf "diff %s --unified=0" mergeBase) with
        | Ok s -> s
        | Error _ -> ""
    let inputs = buildEvidenceInputs model unifiedDiff
    let gr, graphArts = FS.Skia.UI.Build.Evidence.Engine.runGraph inputs
    evidenceWrite (path [ model.ReadinessDir; "task-graph.json" ]) graphArts.TaskGraphJson
    evidenceWrite (path [ model.ReadinessDir; "task-graph.md" ]) graphArts.TaskGraphMd
    match gr.Verdict with
    | FS.Skia.UI.Build.Evidence.GraphVerdict.Error ->
        failwithf "Evidence graph compute failed; see %s" (path [ model.ReadinessDir; "task-graph.md" ])
    | _ ->
        let res, arts = FS.Skia.UI.Build.Evidence.Engine.runAudit inputs
        let r = model.ReadinessDir
        evidenceWrite (path [ r; "seh-audit-summary.json" ]) arts.SehAuditSummary
        evidenceWrite (path [ r; "readiness-contract-hits.json" ]) arts.ReadinessContractHits
        evidenceWrite (path [ r; "persistent-launch-hits.json" ]) arts.PersistentLaunchHits
        evidenceWrite (path [ r; "persistent-gui-runtime-hits.json" ]) arts.PersistentGuiRuntimeHits
        evidenceWrite (path [ r; "window-visibility-hits.json" ]) arts.WindowVisibilityHits
        evidenceWrite (path [ r; "audit-status-hits.json" ]) arts.AuditStatusHits
        evidenceWrite (path [ r; "diff-scan-hits.json" ]) arts.DiffScanHits
        let verdictStr =
            match res.Verdict with
            | FS.Skia.UI.Build.Evidence.AuditVerdict.Pass -> "PASS"
            | _ -> "FAIL"
        let log =
            sprintf
                "=== speckit.evidence.audit (in-process) ===\nfeature: %s\nverdict=%s\nreal-tasks=%d\naccepted-seh-tasks=%d\nunaccepted-synthetic-tasks=%d\nauto-synthetic-tasks=%d\nlate-seh-tasks=%d\ndiff-scan-hits=%d\nreadiness-contract-hits=%d\npersistent-launch-hits=%d\npersistent-gui-runtime-hits=%d\nwindow-visibility-hits=%d\naudit-status-hits=%d\ntotal-blockers=%d\n"
                inputs.FeatureName verdictStr res.RealTasks (List.length res.SehSummary.AcceptedSehTasks)
                (List.length res.SehSummary.UnacceptedSyntheticTasks) (List.length res.SehSummary.AutoSyntheticTasks)
                (List.length res.SehSummary.LateSehTasks) res.DiffBlocking res.ReadinessContract res.PersistentLaunch
                res.PersistentGuiRuntime res.WindowVisibility res.AuditStatus res.TotalBlockers
        // FR-004 (061): when the readiness-contract scan blocks, print the full
        // required shape per failing file so the contract is recoverable from
        // the audit's own output (no decompiling, no sibling copy). Sourced from
        // the same enforced token list, so it cannot drift (RC-1/RC-2).
        let readinessDiagnostics =
            FS.Skia.UI.Build.Evidence.Render.readinessContractDiagnostics
                (FS.Skia.UI.Build.Evidence.Scans.readinessContract inputs.Scan)
        let log =
            if readinessDiagnostics = "" then log
            else log + "\n--- readiness-contract required shapes (FR-004) ---\n" + readinessDiagnostics
        // FR-005 (062): for each OTHER failing evidence-format class, append the
        // complete required shape (single-sourced from EvidenceFormatSchema) so the
        // contract is recoverable from the audit's own output — no decompiling, no
        // sibling copy. window-visibility prints when it blocks; SEH prints when an
        // unaccepted-synthetic or invalid-SEH diagnostic is present.
        let log =
            if res.WindowVisibility > 0 then
                log
                + "\n--- window-visibility required shapes (FR-005) ---\n"
                + FS.Skia.UI.Build.Evidence.Scans.windowVisibilitySchemaText ()
            else
                log
        let log =
            if not (List.isEmpty res.SehSummary.UnacceptedSyntheticTasks)
               || not (List.isEmpty res.SehSummary.Diagnostics) then
                log
                + "\n--- seh-acceptance required shapes (FR-005) ---\n"
                + FS.Skia.UI.Build.Evidence.TaskParser.sehAcceptanceSchemaText ()
            else
                log
        evidenceWrite (path [ model.LogDir; "evidence-audit.txt" ]) log
        match res.Verdict with
        | FS.Skia.UI.Build.Evidence.AuditVerdict.Fail ->
            failwithf "Evidence audit FAIL (%d blockers); see %s" res.TotalBlockers (path [ model.LogDir; "evidence-audit.txt" ])
        | _ -> ()

// ---------------------------------------------------------------------------
// Feature 064 (FR-001/FR-002/FR-006): the publish + pre-publish interpret edge.
// The pure plan/rules live in Publish.fs / PrePublish.fs; everything here is I/O
// (env reads, the anonymous feed read, `dotnet nuget push`, file reads/writes).
// ---------------------------------------------------------------------------

let private templatePackageProject = ".template.package/FS.Skia.UI.Template.fsproj"

/// The full shipped (packageId, version) set: 11 packProjects libs + the template package.
let private shippedPackages root =
    (packProjects |> List.map (fun (project, packageId) -> packageId, projectVersion root project))
    @ [ "FS.Skia.UI.Template", projectVersion root templatePackageProject ]

let private nupkgPathFor (model: BuildModel) (packageId: string) (version: string) =
    let fileName = sprintf "%s.%s.nupkg" packageId version

    if packageId = "FS.Skia.UI.Template" then
        path [ model.TemplateArtifactDir; fileName ]
    else
        path [ model.LocalPackageDir; fileName ]

let private httpGetString (url: string) =
    try
        use client = new HttpClient()
        let response = client.GetAsync(url).GetAwaiter().GetResult()

        if response.IsSuccessStatusCode then
            Some(response.Content.ReadAsStringAsync().GetAwaiter().GetResult())
        else
            None
    with _ ->
        None

/// An anonymous-read predicate: does the target feed already have <id>@<version>? Local feed ⇒
/// directory listing; nuget.org/http ⇒ flat-container index.json (404 ⇒ not published).
let private feedHasVersionProbe (config: PublishConfig) : string -> string -> bool =
    if config.IsLocalFeed then
        fun packageId version ->
            [ sprintf "%s.%s.nupkg" packageId version
              sprintf "%s.%s.nupkg" (packageId.ToLowerInvariant()) version ]
            |> List.exists (fun fileName -> File.Exists(path [ config.ReadUrl; fileName ]))
    else
        fun packageId version ->
            let url =
                sprintf "%s/%s/index.json" (config.ReadUrl.TrimEnd('/')) (packageId.ToLowerInvariant())

            match httpGetString url with
            | Some json -> (Publish.parseFlatContainerVersions json).Contains(version.ToLowerInvariant())
            | None -> false

// Feature 064: optional GitHub Release artifact upload — an ARCHIVAL supplement to the nuget.org
// push (nuget.org remains the consumer feed; GitHub Releases is not a NuGet feed). gh authenticates
// via the ambient `gh auth` token, so no credential appears on the command line. Best-effort: a gh
// failure is surfaced but never fails the (already-completed, irreversible) nuget push.
let private uploadGitHubReleaseAssets (model: BuildModel) (tag: string) (assets: string list) =
    let log = path [ model.LogDir; "publish-gh-release.txt" ]

    if List.isEmpty assets then
        printfn "publish: gh release skipped — no .nupkg assets present"
    else
        let quoted = assets |> List.map quote |> String.concat " "

        try
            // `gh release create` creates the release with all assets; if the tag already has a
            // release it errors, and we fall back to `gh release upload --clobber`.
            let created =
                try
                    runProcess
                        "gh release create"
                        "gh"
                        (sprintf
                            "release create %s %s --title %s --notes %s"
                            (quote tag)
                            quoted
                            (quote (sprintf "FS.Skia.UI %s" tag))
                            (quote "FS.Skia.UI package artifacts. The consumer feed is nuget.org; these .nupkg are an archival supplement."))
                        model.RepositoryRoot
                        log
                        Map.empty

                    true
                with _ ->
                    false

            if not created then
                runProcessWithAllowedExitCodes
                    "gh release upload"
                    "gh"
                    (sprintf "release upload %s %s --clobber" (quote tag) quoted)
                    model.RepositoryRoot
                    log
                    Map.empty
                    (Set.singleton 0)

            printfn "publish: gh release %s updated with %d asset(s)" tag (List.length assets)
        with ex ->
            printfn
                "publish: WARNING gh release upload to tag %s failed (%s); the nuget.org push is unaffected. See %s"
                tag
                ex.Message
                log

let runPublishPackages (model: BuildModel) =
    let lookup name =
        Environment.GetEnvironmentVariable name |> Option.ofObj

    let config = Publish.configFromEnv lookup

    // Validation (FR-002 edge): a real push with no credential aborts fast, pushing nothing.
    match Publish.validateConfig config with
    | Some err -> failwith err
    | None -> ()

    let packages = shippedPackages model.RepositoryRoot
    let rows = Publish.buildPlan packages (feedHasVersionProbe config)
    let planReport = Publish.renderPlan config rows

    ensureParent (path [ model.LogDir; "publish-plan.md" ])
    File.WriteAllText(path [ model.LogDir; "publish-plan.md" ], planReport)
    printfn "%s" planReport

    if List.length rows <> 12 then
        failwithf "Publish plan must cover exactly 12 packages (11 libs + template); got %d" (List.length rows)

    if config.DryRun then
        printfn "publish: dry-run — no network push performed (credential not required)."
    else
        let nonBlank = Option.filter (fun (s: string) -> not (String.IsNullOrWhiteSpace s))

        let apiKey =
            match (lookup "FSSKIA_PUBLISH_API_KEY" |> nonBlank) |> Option.orElse (lookup "NUGET_API_KEY" |> nonBlank) with
            | Some key -> key
            | None -> failwith "No publish credential set (NUGET_API_KEY or FSSKIA_PUBLISH_API_KEY); nothing was pushed."

        // Feature 064 (FR-001): pass the credential to `dotnet nuget push` via the NUGET_API_KEY
        // environment variable (NuGet 7.6+ / SDK 10.0.300) — the same ambient-credential model
        // `gh` uses with GH_TOKEN. The key is therefore NEVER on the command line, so it cannot
        // appear in any log; redaction of captured output stays as belt-and-braces.
        let pushEnv = Map.ofList [ "NUGET_API_KEY", apiKey ]

        for row in rows do
            match row.Decision with
            | Skip -> printfn "publish: skip %s@%s (already on the feed)" row.PackageId row.Version
            | Push ->
                let nupkg = nupkgPathFor model row.PackageId row.Version

                if not (File.Exists nupkg) then
                    failwithf "publish: package artifact %s is missing; run PackLocal/TemplatePack first" nupkg

                let args =
                    sprintf "nuget push %s -s %s --skip-duplicate" (quote nupkg) (quote config.FeedUrl)

                runProcessRedacted (sprintf "push %s" row.PackageId) "dotnet" args model.RepositoryRoot (path [ model.LogDir; "publish.txt" ]) pushEnv [ apiKey ]

        // Optional archival upload of the .nupkg to a GitHub Release (opt-in via
        // FSSKIA_PUBLISH_GH_RELEASE_TAG). Supplement only — nuget.org is the consumer feed.
        match lookup "FSSKIA_PUBLISH_GH_RELEASE_TAG" with
        | Some tag when not (String.IsNullOrWhiteSpace tag) ->
            rows
            |> List.map (fun r -> nupkgPathFor model r.PackageId r.Version)
            |> List.filter File.Exists
            |> uploadGitHubReleaseAssets model tag
        | _ -> ()

let private xmlTagValue (xml: string) (tag: string) =
    let m =
        Regex.Match(xml, sprintf "<%s>([^<]*)</%s>" tag tag, RegexOptions.CultureInvariant)

    if m.Success && not (String.IsNullOrWhiteSpace m.Groups.[1].Value) then
        Some(m.Groups.[1].Value.Trim())
    else
        None

let private resolvePackageMetadata root (packageId: string) (project: string) : PrePublish.PackageMetadata =
    let fsproj =
        let p = path [ root; project ]
        if File.Exists p then File.ReadAllText p else ""

    let directoryBuildProps =
        let p = path [ root; "Directory.Build.props" ]
        if File.Exists p then File.ReadAllText p else ""

    // fsproj wins; Directory.Build.props is the shared fallback for license/authors/repo-url.
    let pick tag =
        xmlTagValue fsproj tag |> Option.orElse (xmlTagValue directoryBuildProps tag)

    // FR-010: the README must physically exist next to the packable project (a configured
    // PackageReadmeFile with no file fails `dotnet pack`), so check the file, not just the tag.
    let projectDir =
        Path.GetDirectoryName(path [ root; project ]) |> Option.ofObj |> Option.defaultValue root

    let readmeFile =
        if File.Exists(path [ projectDir; "README.md" ]) then Some "README.md" else None

    { PackageId = packageId
      LicenseExpression = pick "PackageLicenseExpression"
      RepositoryUrl = pick "RepositoryUrl"
      Authors = pick "Authors"
      Description = pick "Description"
      ReadmeFile = readmeFile }

let runPrePublishCheck (model: BuildModel) =
    let root = model.RepositoryRoot
    let templateProps = path [ root; "template"; "base"; "Directory.Packages.props" ]
    let templatePropsContent = if File.Exists templateProps then File.ReadAllText templateProps else ""

    // The version build.fsx resolves at runtime: a literal `#r "nuget: FS.Skia.UI.Build, X"`
    // when present (pre-T024), otherwise the <FsSkiaUiVersion> property build.fsx reads (post-T024).
    let buildFsx = path [ root; "template"; "base"; "build.fsx" ]
    let buildFsxContent = if File.Exists buildFsx then File.ReadAllText buildFsx else ""

    let engineVersionFromBuildFsx =
        let literal =
            Regex.Match(
                buildFsxContent,
                "#r\\s+\"nuget:\\s*FS\\.Skia\\.UI\\.Build\\s*,\\s*([^\"]+)\"",
                RegexOptions.CultureInvariant)

        if literal.Success then
            Some(literal.Groups.[1].Value.Trim())
        else
            let prop =
                Regex.Match(templatePropsContent, "<FsSkiaUiVersion>([^<]+)</FsSkiaUiVersion>", RegexOptions.CultureInvariant)

            if prop.Success then Some(prop.Groups.[1].Value.Trim()) else None

    let metadataProjects =
        (packProjects |> List.map (fun (project, packageId) -> packageId, project))
        @ [ "FS.Skia.UI.Template", templatePackageProject ]

    let inputs: PrePublish.PrePublishInputs =
        { ShippedVersions = shippedPackages root
          EngineShippedVersion = projectVersion root "build/Governance/FS.Skia.UI.Build.fsproj"
          TemplateProps = templatePropsContent
          EngineVersionFromBuildFsx = engineVersionFromBuildFsx
          ConsumerNuGetConfig = GeneratedProduct.consumerNuGetConfigContent model
          Metadata = metadataProjects |> List.map (fun (packageId, project) -> resolvePackageMetadata root packageId project) }

    let findings = PrePublish.check inputs
    let report = PrePublish.render findings

    ensureParent (path [ model.LogDir; "prepublish-check.txt" ])
    File.WriteAllText(path [ model.LogDir; "prepublish-check.txt" ], report)
    printfn "%s" report

    if not (List.isEmpty findings) then
        let summary =
            findings
            |> List.map (fun f -> sprintf "%s: %s/%s — %s" (PrePublish.ruleName f.Rule) f.Package f.Field f.Detail)
            |> String.concat Environment.NewLine

        failwithf "PrePublishCheck FAIL (%d finding(s)); the publish is aborted:%s%s" (List.length findings) Environment.NewLine summary
