module Feature060GovernanceTests

(* Feature 060 — failing-first (Principle I/VI) tests for the three new governance
   surfaces: the api-surface single-source generator (FR-003), SkillContractPathCheck
   (FR-004), and TemplateUpdateSkillPackageCheck (FR-009). Each exercises the pure
   library function over literal inputs AND asserts the real repo state is clean, so
   the negative case fails before the generator/edits exist and passes after. *)

open System
open System.IO
open Expecto
open FS.Skia.UI.Build
open GovernanceTestSupport

let private realCapabilities () =
    Capabilities.readCatalog (Path.Combine(repositoryRoot, "template", "capabilities.yml"))

let private readBytesAt (rel: string) =
    let full = Path.Combine(repositoryRoot, rel.Replace('/', Path.DirectorySeparatorChar))
    if File.Exists full then Some(File.ReadAllBytes full) else None

let private skillFiles (subs: string list) =
    subs
    |> List.collect (fun sub ->
        let dir = Path.Combine(repositoryRoot, sub.Replace('/', Path.DirectorySeparatorChar))

        if Directory.Exists dir then
            Directory.EnumerateFiles(dir, "SKILL.md", SearchOption.AllDirectories) |> List.ofSeq
        else
            [])
    |> List.filter (fun f ->
        let n = f.Replace('\\', '/')
        not (n.Contains "/obj/" || n.Contains "/bin/"))
    |> List.distinct

let private realPackableLeaves () =
    [ "src"; "build" ]
    |> List.collect (fun sub ->
        let dir = Path.Combine(repositoryRoot, sub)

        if Directory.Exists dir then
            Directory.EnumerateFiles(dir, "*.fsproj", SearchOption.AllDirectories) |> List.ofSeq
        else
            [])
    |> List.filter (fun f ->
        let n = f.Replace('\\', '/')

        if n.Contains "/obj/" || n.Contains "/bin/" then
            false
        else
            let t = File.ReadAllText f
            t.Contains "<IsPackable>true</IsPackable>" || t.Contains "<PackageId>")
    |> List.map (fun f -> TemplateUpdatePackage.leafOfProject ((Path.GetRelativePath(repositoryRoot, f)).Replace('\\', '/')))
    |> List.distinct

[<Tests>]
let apiSurfaceGenTests =
    testList "Feature060 ApiSurfaceGen" [
        test "plan emits each capability contract under docs/api-surface/<Leaf>/<file>.fsi" {
            let entries = ApiSurfaceGen.plan (realCapabilities ())

            Expect.exists
                entries
                (fun e -> e.ProjectRelPath = "docs/api-surface/Scene/Scene.fsi" && e.SourceFsi = "src/Scene/Scene.fsi")
                "scene surface is planned from its capability contract"

            Expect.exists
                entries
                (fun e -> e.ProjectRelPath = "docs/api-surface/KeyboardInput/KeyboardInput.fsi")
                "keyboard-input surface is planned"

            Expect.exists
                entries
                (fun e -> e.EmittedRelPath = "template/base/docs/api-surface/Scene/Scene.fsi")
                "emitted path lives under the template base"

            Expect.isFalse
                (entries |> List.exists (fun e -> e.SourceFsi = "no-public-surface"))
                "the non-runtime samples capability emits no surface"
        }

        test "currency passes on byte-identity and fails on a missing/drifted emitted file" {
            let entry: ApiSurfaceGen.ApiSurfaceEntry =
                { PackageId = "FS.Skia.UI.X"
                  PkgLeaf = "X"
                  SourceFsi = "src/X.fsi"
                  EmittedRelPath = "template/base/docs/api-surface/X/X.fsi"
                  ProjectRelPath = "docs/api-surface/X/X.fsi"
                  Profiles = [] }

            let bytes = Text.Encoding.UTF8.GetBytes "module X"

            let readMatch p =
                if p = "src/X.fsi" || p = "template/base/docs/api-surface/X/X.fsi" then Some bytes else None

            Expect.isEmpty
                (ApiSurfaceGen.currency [ entry ] readMatch [ "template/base/docs/api-surface/X/X.fsi" ])
                "byte-identical emitted file is current"

            let readMissing p = if p = "src/X.fsi" then Some bytes else None
            Expect.isNonEmpty (ApiSurfaceGen.currency [ entry ] readMissing []) "missing emitted file fails currency"

            let readDrift p =
                if p = "src/X.fsi" then Some bytes
                elif p = "template/base/docs/api-surface/X/X.fsi" then Some(Text.Encoding.UTF8.GetBytes "stale")
                else None

            Expect.isNonEmpty
                (ApiSurfaceGen.currency [ entry ] readDrift [ "template/base/docs/api-surface/X/X.fsi" ])
                "drifted emitted bytes fail currency"
        }

        test "the committed api-surface tree is current against the real capability catalog" {
            let entries = ApiSurfaceGen.plan (realCapabilities ())
            let emittedRootFull = Path.Combine(repositoryRoot, "template", "base", "docs", "api-surface")

            let existing =
                if Directory.Exists emittedRootFull then
                    Directory.EnumerateFiles(emittedRootFull, "*", SearchOption.AllDirectories)
                    |> Seq.map (fun f -> (Path.GetRelativePath(repositoryRoot, f)).Replace('\\', '/'))
                    |> Seq.toList
                else
                    []

            Expect.isEmpty
                (ApiSurfaceGen.currency entries readBytesAt existing)
                "committed docs/api-surface tree is byte-identical to source .fsi (regenerate via RefreshSurfaceBaselines)"
        }
    ]

[<Tests>]
let skillContractPathTests =
    testList "Feature060 SkillContractPath" [
        test "parseClaims extracts api-surface paths and detects the no-reflection promise" {
            let text = "Read `docs/api-surface/Scene/Scene.fsi` locally — no DLL reflection needed."
            let claims = SkillContractPath.parseClaims "x/SKILL.md" text

            Expect.hasLength claims 1 "one api-surface claim parsed"
            Expect.equal claims.[0].ClaimedPath "docs/api-surface/Scene/Scene.fsi" "claimed path captured"
            Expect.isTrue claims.[0].ClaimsNoReflection "no-reflection promise detected"
        }

        test "check fails a claimed path absent from the emitted tree" {
            let entries: ApiSurfaceGen.ApiSurfaceEntry list =
                [ { PackageId = "FS.Skia.UI.Scene"
                    PkgLeaf = "Scene"
                    SourceFsi = "src/Scene/Scene.fsi"
                    EmittedRelPath = "template/base/docs/api-surface/Scene/Scene.fsi"
                    ProjectRelPath = "docs/api-surface/Scene/Scene.fsi"
                    Profiles = [] } ]

            let resolved: SkillContractPath.SkillContractClaim list =
                [ { SkillPath = "s"; ClaimedPath = "docs/api-surface/Scene/Scene.fsi"; ClaimsNoReflection = true } ]

            let absent: SkillContractPath.SkillContractClaim list =
                [ { SkillPath = "s"; ClaimedPath = "docs/api-surface/Ghost/Ghost.fsi"; ClaimsNoReflection = true } ]

            Expect.isEmpty (SkillContractPath.check entries resolved) "a resolved claim passes"
            Expect.isNonEmpty (SkillContractPath.check entries absent) "an absent claim fails"
        }

        test "every real product/capability skill api-surface claim resolves to the emitted tree" {
            let entries = ApiSurfaceGen.plan (realCapabilities ())

            let claims =
                skillFiles [ "template/product-skills"; "src"; ".agents/skills" ]
                |> List.collect (fun full ->
                    let rel = (Path.GetRelativePath(repositoryRoot, full)).Replace('\\', '/')
                    SkillContractPath.parseClaims rel (File.ReadAllText full))

            Expect.isEmpty (SkillContractPath.check entries claims) "all real skill api-surface claims resolve to emitted files"
        }
    ]

[<Tests>]
let templateUpdatePackageTests =
    testList "Feature060 TemplateUpdatePackage" [
        test "leafOfProject derives the package leaf from the project path" {
            Expect.equal (TemplateUpdatePackage.leafOfProject "build/Governance/FS.Skia.UI.Build.fsproj") "Build" "build engine leaf"
            Expect.equal (TemplateUpdatePackage.leafOfProject "src/Controls.Elmish/Controls.Elmish.fsproj") "Controls.Elmish" "controls.elmish leaf"
            Expect.equal (TemplateUpdatePackage.leafOfProject "src/Input/Input.fsproj") "Input" "input leaf"
        }

        test "feedLoopLeaves parses the step-5 for-loop enumeration" {
            let text = "```bash\nfor pkg in Build Scene Input SkillSupport; do\n  echo $pkg\ndone\n```"
            Expect.equal (TemplateUpdatePackage.feedLoopLeaves text |> List.sort) [ "Build"; "Input"; "Scene"; "SkillSupport" ] "feed-loop leaves parsed"
        }

        test "check fails the stale pre-060 skill enumeration (failing-first)" {
            let stale =
                "for p in Build Scene SkiaViewer Elmish KeyboardInput Layout Controls Controls.Elmish Testing; do\n"
                + "  test -f x && echo OK\n"
                + "done\n"
                + "# FS.Skia.UI (the Lib package) has no suffix\n"
                + "entry to the current package version (nine repo packages)\n"

            let packable = [ "Build"; "Scene"; "SkiaViewer"; "Elmish"; "KeyboardInput"; "Input"; "Layout"; "Controls"; "Controls.Elmish"; "Testing"; "SkillSupport" ]
            let diagnostics = TemplateUpdatePackage.check packable "x" stale

            Expect.isNonEmpty diagnostics "stale enumeration fails the check"
            Expect.isTrue (diagnostics |> List.exists (fun d -> d.Contains "Input")) "missing Input is flagged"
            Expect.isTrue (diagnostics |> List.exists (fun d -> d.Contains "SkillSupport")) "missing SkillSupport is flagged"
            Expect.isTrue (TemplateUpdatePackage.hasPhantomLib stale) "phantom bare-Lib check is detected"
            Expect.isTrue (TemplateUpdatePackage.hasStaleCount stale) "stale nine-package count is detected"
        }

        test "the corrected fs-skia-template-update skill equals the real packable set" {
            let skillText = read ".agents/skills/fs-skia-template-update/SKILL.md"

            Expect.isEmpty
                (TemplateUpdatePackage.check (realPackableLeaves ()) ".agents/skills/fs-skia-template-update/SKILL.md" skillText)
                "the corrected skill enumeration equals the packable set (zero phantom, zero missing)"
        }
    ]
