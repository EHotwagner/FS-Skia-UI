module Feature089GovernanceTests

// Feature 089 (Typed Front-Door Discoverability & Spec-Kit Workflow Followups) —
// failing-first governance tests over the real typed values:
//   * TYPED-SURFACE-1 (T006/T007): the 14 typed `src/Controls/Widgets/*.fsi`
//     enrolled in the Controls api-surface, and the `TypedModule` token rendered
//     into `catalog.yml` with the E1⟂E2 single-source cross-check + total 52-id
//     coverage (custom-control excepted).
//   * EVGRAPH-ECHO-1 (T016): the pure `Render.skillistResolution` helper, which
//     resolves / alias-flags / ambiguous-flags / unresolved-flags each distinct
//     skillist id using the same registry the Audit validator consults.
// All assertions are over the real pure functions and the committed source tree —
// real test evidence, no synthetic product fixtures.

open System.IO
open System.Text.RegularExpressions
open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Evidence
open GovernanceTestSupport

// --- shared fixtures --------------------------------------------------------

let private facts = CatalogGen.catalogFacts

/// The 14 typed Widgets `.fsi` enrolled by this feature (data-model E2).
let private typedWidgetFsi =
    [ "src/Controls/Widgets/Primitives.fsi"
      "src/Controls/Widgets/Buttons.fsi"
      "src/Controls/Widgets/Containers.fsi"
      "src/Controls/Widgets/Input.fsi"
      "src/Controls/Widgets/TextBoxWidget.fsi"
      "src/Controls/Widgets/TextAreaWidget.fsi"
      "src/Controls/Widgets/Display.fsi"
      "src/Controls/Widgets/CollectionsWidgets.fsi"
      "src/Controls/Widgets/DataGridWidget.fsi"
      "src/Controls/Widgets/Navigation.fsi"
      "src/Controls/Widgets/Overlay.fsi"
      "src/Controls/Widgets/ChartsWidgets.fsi"
      "src/Controls/Widgets/CustomControlWidget.fsi"
      "src/Controls/Widgets/Pickers.fsi" ]

/// Parse `module X =` declarations and whether each declares a `val view` from a
/// typed widget `.fsi` body. Returns (moduleName, hasView) per module.
let private modulesWithView (fsiText: string) : (string * bool) list =
    let lines = fsiText.Replace("\r\n", "\n").Split('\n')
    let mutable current = None
    let result = ResizeArray<string * bool>()
    let mutable hasView = false
    let flush () =
        match current with
        | Some m -> result.Add(m, hasView)
        | None -> ()
    for line in lines do
        let m = Regex.Match(line, "^module (\\w+) =")
        if m.Success then
            flush ()
            current <- Some(m.Groups.[1].Value)
            hasView <- false
        elif Regex.IsMatch(line, "\\bval view\\b") then
            hasView <- true
    flush ()
    List.ofSeq result

/// Every typed module declared across the 14 enrolled `.fsi`, with its view flag.
let private enrolledModules () : Map<string, bool> =
    typedWidgetFsi
    |> List.collect (fun rel -> modulesWithView (read rel))
    |> Map.ofList

// --- TYPED-SURFACE-1 (T006): api-surface enrolment + currency ---------------

[<Tests>]
let feature089TypedSurfaceTests =
    testList "feature 089 TYPED-SURFACE-1 published typed front-door" [

        // T006 — capabilities enrolment + ApiSurfaceGen currency over the typed .fsi
        test "the 14 typed Widgets .fsi are enrolled in the Controls api-surface contracts" {
            let rows = Capabilities.readCatalog (fullPath "template/capabilities.yml")
            let controls =
                rows |> List.tryFind (fun r -> r.PackageId = Some "FS.Skia.UI.Controls")
            Expect.isSome controls "the Controls capability row is present"
            let contracts = controls.Value.Contracts
            for f in typedWidgetFsi do
                Expect.contains contracts f (sprintf "%s enrolled in Controls contracts (additive)" f)
            // additive: the legacy builder surface stays published (FR-003)
            Expect.contains contracts "src/Controls/Control.fsi" "legacy Control.fsi still published"
            Expect.contains contracts "src/Controls/Catalog.fsi" "legacy Catalog.fsi still published"
        }

        test "ApiSurfaceGen plans an emitted Controls/<file>.fsi for each typed widget .fsi" {
            let rows = Capabilities.readCatalog (fullPath "template/capabilities.yml")
            let entries = ApiSurfaceGen.plan rows
            for f in typedWidgetFsi do
                let leaf = Path.GetFileName f
                let hit =
                    entries
                    |> List.exists (fun e ->
                        e.SourceFsi = f
                        && e.EmittedRelPath.Replace("\\", "/").EndsWith(sprintf "docs/api-surface/Controls/%s" leaf))
                Expect.isTrue hit (sprintf "%s plans an emitted Controls/%s entry" f leaf)
        }

        test "the emitted Controls api-surface is byte-current with the source .fsi (post-regen)" {
            // Failing-first: before RefreshSurfaceBaselines the typed .fsi are absent
            // from the emitted tree, so currency reports `missing`. After regen the
            // emitted copies are byte-identical and currency is empty.
            let rows = Capabilities.readCatalog (fullPath "template/capabilities.yml")
            let entries = ApiSurfaceGen.plan rows
            let readBytes (rel: string) =
                let p = fullPath rel
                if File.Exists p then Some(File.ReadAllBytes p) else None
            let emittedRootAbs = fullPath ApiSurfaceGen.emittedRoot
            let existing =
                if Directory.Exists emittedRootAbs then
                    Directory.GetFiles(emittedRootAbs, "*.fsi", SearchOption.AllDirectories)
                    |> Array.map (fun p -> p.Substring(repositoryRoot.Length + 1).Replace("\\", "/"))
                    |> List.ofArray
                else []
            let drift = ApiSurfaceGen.currency entries readBytes existing
            Expect.isEmpty drift "emitted Controls api-surface is current (run RefreshSurfaceBaselines if not)"
        }
    ]

// --- TYPED-SURFACE-1 (T007): TypedModule render + cross-check + coverage -----

[<Tests>]
let feature089TypedModuleTests =
    testList "feature 089 TYPED-SURFACE-1 catalog TypedModule index" [

        test "renderYamlRow emits the typedModule token after module" {
            let f = facts |> List.find (fun x -> x.Id = "list-view")
            let row = CatalogGen.renderYamlRow f
            Expect.stringContains row "    module: Collections" "legacy module token preserved"
            Expect.stringContains row "    typedModule: ListView" "typed module pointer rendered"
            // ordering: typedModule immediately follows module
            Expect.stringContains row "    module: Collections\n    typedModule: ListView" "typedModule renders directly after module"
        }

        test "CatalogGen.currency is clean on the committed catalog and fails on a TypedModule edit" {
            let yml = read CatalogGen.catalogYmlRel
            let fs = read CatalogGen.catalogFsRel
            let clean = CatalogGen.currency yml fs
            Expect.isTrue (CatalogGen.isCurrent clean) "committed catalog is current (run RefreshSurfaceBaselines if not)"
            // tamper one typedModule token → the region for that id goes stale
            let tampered = yml.Replace("    typedModule: ListView", "    typedModule: Wrong")
            Expect.notEqual tampered yml "the tamper actually changed the text"
            let drift = CatalogGen.currency tampered fs
            Expect.isFalse (CatalogGen.isCurrent drift) "a TypedModule edit fails the currency gate"
            let stale =
                drift |> List.exists (fun c -> c.ControlId = "list-view" && c.Status = CatalogGen.Stale)
            Expect.isTrue stale "list-view region is reported stale after the TypedModule edit"
        }

        test "E1⟂E2: every TypedModule names a module declared in an enrolled typed .fsi" {
            let declared = enrolledModules ()
            for f in facts do
                Expect.isTrue
                    (declared.ContainsKey f.TypedModule)
                    (sprintf "%s → TypedModule %s is declared in some enrolled Widgets/*.fsi" f.Id f.TypedModule)
        }

        test "total coverage: all 52 control ids map to a typed module exposing view (custom-control excepted)" {
            let declared = enrolledModules ()
            Expect.equal (List.length facts) 52 "the catalog has 52 control facts"
            for f in facts do
                match f.Id with
                | "custom-control" ->
                    Expect.equal f.TypedModule "CustomControl" "custom-control points at the bridge module"
                    Expect.equal (declared.TryFind "CustomControl") (Some false) "CustomControl is bridge-typed (no view) — excepted"
                    Expect.isEmpty f.RequiredAttributes "custom-control carries no fabricated required attribute"
                | _ ->
                    Expect.equal
                        (declared.TryFind f.TypedModule)
                        (Some true)
                        (sprintf "%s → %s exposes a typed `view` in the published surface" f.Id f.TypedModule)
        }
    ]

// --- EVGRAPH-ECHO-1 (T016): skillistResolution helper -----------------------

[<Tests>]
let feature089SkillistResolutionTests =
    testList "feature 089 EVGRAPH-ECHO-1 skillist resolution echo" [

        test "skillistResolution resolves, aliases, ambiguates and flags-unresolved distinctly" {
            let registry: SkillRegistry =
                { Skills =
                    Map.ofList
                        [ "speckit-tasks", [ ".agents/skills/speckit-tasks/SKILL.md" ]
                          "some-dup", [ "a/SKILL.md"; "b/SKILL.md" ] ]
                  DirectoryAliases =
                    Map.ofList [ "controlsshowcase1-widgets", ("fs-skia-ui-widgets", "src/Controls/skill/SKILL.md") ]
                  Warnings = [] }
            let ids = [ "speckit-tasks"; "some-dup"; "controlsshowcase1-widgets"; "made-up-skill" ]
            let out = Render.skillistResolution registry ids

            let resolvedHdr = "## Skillist id → SKILL.md path"
            let flaggedHdr = "## Skillist id → unresolved / flagged"
            Expect.stringContains out resolvedHdr "resolved section header present"
            Expect.stringContains out flaggedHdr "flagged section header present (distinct)"

            // split into the two sections to assert membership distinctly
            let idx = out.IndexOf flaggedHdr
            let resolvedSection = out.Substring(0, idx)
            let flaggedSection = out.Substring(idx)

            Expect.stringContains resolvedSection "speckit-tasks → .agents/skills/speckit-tasks/SKILL.md" "resolved id → path under resolved header"
            Expect.isFalse (resolvedSection.Contains "made-up-skill") "unresolved id is NOT among the resolved lines"

            Expect.stringContains flaggedSection "made-up-skill → UNRESOLVED (not registered/readable)" "unresolved flagged"
            Expect.stringContains flaggedSection "some-dup → ambiguous: a/SKILL.md, b/SKILL.md" "ambiguous flagged"
            Expect.stringContains flaggedSection "controlsshowcase1-widgets → directory name for src/Controls/skill/SKILL.md (accepted id: fs-skia-ui-widgets)" "alias flagged with accepted id"
        }

        test "skillistResolution agrees with the live registry for this feature's own skillist ids" {
            // The echo must use the SAME registry the Audit validator consults; on
            // the live tree every declared id (e.g. speckit-evidence-graph) resolves.
            let registry = SkillRegistry.build repositoryRoot
            let out = Render.skillistResolution registry [ "speckit-evidence-graph"; "fsharp-code-generation" ]
            Expect.stringContains out "speckit-evidence-graph → .agents/skills/speckit-evidence-graph/SKILL.md" "live resolved path echoed"
            Expect.stringContains out "fsharp-code-generation → .agents/skills/fsharp-code-generation/SKILL.md" "live resolved path echoed"
            let idx = out.IndexOf "## Skillist id → unresolved / flagged"
            let flaggedSection = out.Substring(idx)
            Expect.stringContains flaggedSection "_(none" "no flagged ids for these resolvable skills"
        }
    ]
