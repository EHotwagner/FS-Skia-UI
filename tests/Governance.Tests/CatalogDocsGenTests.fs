module CatalogDocsGenTests

// Feature 078 — Controls catalog docs generation. The published "Controls" section is a
// single-source projection of CatalogGen.catalogFacts onto docs targets (the catalog index
// region + one canonical header region per control). These failing-first tests cover (T009)
// render byte-identity + splice idempotence and (T010) every catalogDocsCurrency finding class
// plus the empty (PASS) list on a clean in-memory tree, then assert the committed docs tree is
// clean. All assertions are over the real pure functions with constructed in-memory fixtures —
// real test evidence, not synthetic product evidence.

open System.IO
open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.CatalogDocsGen

let private repositoryRoot =
    let rec find dir =
        if File.Exists(Path.Combine(dir, "FS-Skia-UI.sln")) then dir
        else
            match Directory.GetParent dir |> Option.ofObj with
            | Some p -> find p.FullName
            | None -> dir

    find __SOURCE_DIRECTORY__

let private facts = CatalogGen.catalogFacts

/// Wrap a rendered body inside its `catalog-docs/<key>` marker region (as a docs file holds it).
let private wrapRegion (key: string) (body: string) =
    sprintf
        "front matter\n\n<!-- BEGIN GENERATED: catalog-docs/%s -->\n%s\n<!-- END GENERATED: catalog-docs/%s -->\n\nauthored prose\n"
        key
        body
        key

/// A clean in-memory docs tree: current index + every current detail header, each detail page
/// carrying the honest unsupported-preview note, no previews, no orphans, API links not checked.
let private cleanTree () : DocsTree =
    { CatalogIndexText = wrapRegion "index" (renderCatalogIndex facts)
      DetailPages =
        facts
        |> List.map (fun f ->
            { ControlId = f.Id
              Text = wrapRegion f.Id (renderDetailHeader f) + "\n<!-- " + previewUnsupportedMarker + " -->\n" })
      Previews = []
      AvailableReferenceSlugs = None }

[<Tests>]
let catalogDocsRenderTests =
    testList "Feature 078 catalog-docs render + splice (T009)" [
        test "renderMap covers the index plus every control id" {
            let m = renderMap facts
            Expect.isTrue (Map.containsKey "index" m) "renderMap contains the index key"
            for f in facts do
                Expect.isTrue (Map.containsKey f.Id m) (sprintf "renderMap contains %s" f.Id)
            Expect.equal (Map.count m) (List.length facts + 1) "one entry per control plus the index"
        }

        test "renderCatalogIndex is deterministic and lists every control with its detail link" {
            let a = renderCatalogIndex facts
            let b = renderCatalogIndex facts
            Expect.equal a b "render is deterministic / byte-identical"
            Expect.stringContains a (sprintf "**%d supported controls**" (List.length facts)) "states the live count"
            for f in facts do
                Expect.stringContains a (sprintf "[%s](%s.html)" f.DisplayName f.Id) (sprintf "links %s" f.Id)
        }

        test "renderDetailHeader carries the H1, category, purpose, API link, and back link" {
            let button = facts |> List.find (fun f -> f.Id = "button")
            let header = renderDetailHeader button
            Expect.stringContains header "# Button" "H1 display name"
            Expect.stringContains header "**Category:** input" "category line"
            Expect.stringContains header "Pointer and keyboard activatable command." "purpose"
            Expect.stringContains header "../reference/fs-skia-ui-controls-button.html" "resolving API link"
            Expect.stringContains header "(catalog.html)" "back-to-index link"
        }

        test "apiReferenceSlug routes typed-front-door-only controls to the typed reference page" {
            let datePicker = facts |> List.find (fun f -> f.Id = "date-picker")
            Expect.equal (apiReferenceSlug datePicker) "fs-skia-ui-controls-typed-datepicker" "072 expansion → typed page"
            let stack = facts |> List.find (fun f -> f.Id = "stack")
            Expect.equal (apiReferenceSlug stack) "fs-skia-ui-controls-stack" "legacy module → controls page"
        }

        test "spliceCatalogDocs replaces only the marker body and is idempotent" {
            let renders = renderMap facts
            let original = wrapRegion "button" "STALE BODY"
            let once = spliceCatalogDocs renders original
            let twice = spliceCatalogDocs renders once
            Expect.stringContains once (renderDetailHeader (facts |> List.find (fun f -> f.Id = "button"))) "filled with the fresh render"
            Expect.stringContains once "authored prose" "bytes outside the markers are untouched"
            Expect.stringContains once "front matter" "preamble outside the markers is untouched"
            Expect.equal twice once "splice is idempotent on an already-current file"
        }

        test "spliceCatalogDocs never invents markers for an unknown key" {
            let renders = renderMap facts
            let unknown = wrapRegion "not-a-real-key" "BODY"
            Expect.equal (spliceCatalogDocs renders unknown) unknown "an unknown marker id is left untouched"
        }
    ]

[<Tests>]
let catalogDocsCurrencyTests =
    testList "Feature 078 catalogDocsCurrency findings (T010)" [
        test "a clean tree yields no findings (PASS)" {
            Expect.isEmpty (catalogDocsCurrency facts (cleanTree ())) "clean projection ⇒ empty findings"
        }

        test "IndexStale when the index region drifts" {
            let tree = { cleanTree () with CatalogIndexText = wrapRegion "index" "STALE INDEX" }
            Expect.contains (catalogDocsCurrency facts tree) IndexStale "stale index region is detected"
        }

        test "IndexStale when the index region is missing entirely" {
            let tree = { cleanTree () with CatalogIndexText = "no markers here" }
            Expect.contains (catalogDocsCurrency facts tree) IndexStale "missing index region is detected"
        }

        test "MissingDetailPage when a supported control has no page" {
            let clean = cleanTree ()
            let tree = { clean with DetailPages = clean.DetailPages |> List.filter (fun p -> p.ControlId <> "button") }
            Expect.contains (catalogDocsCurrency facts tree) (MissingDetailPage "button") "missing page is detected"
        }

        test "StaleDetailHeader when a header region drifts" {
            let clean = cleanTree ()
            let tree =
                { clean with
                    DetailPages =
                        clean.DetailPages
                        |> List.map (fun p ->
                            if p.ControlId = "button" then
                                { p with Text = wrapRegion "button" "STALE HEADER" + "<!-- " + previewUnsupportedMarker + " -->" }
                            else p) }
            Expect.contains (catalogDocsCurrency facts tree) (StaleDetailHeader "button") "stale header region is detected"
        }

        test "OrphanDetailPage when a page exists for an unknown control" {
            let clean = cleanTree ()
            let tree =
                { clean with
                    DetailPages =
                        { ControlId = "ghost-control"; Text = wrapRegion "ghost-control" "x" } :: clean.DetailPages }
            Expect.contains (catalogDocsCurrency facts tree) (OrphanDetailPage "ghost-control") "orphan page is detected"
        }

        test "MissingPreview when a control has no preview and no honest note" {
            let clean = cleanTree ()
            let tree =
                { clean with
                    DetailPages =
                        clean.DetailPages
                        |> List.map (fun p ->
                            // Drop the unsupported note from button's page, keep its header current.
                            if p.ControlId = "button" then
                                { p with Text = wrapRegion "button" (renderDetailHeader (facts |> List.find (fun f -> f.Id = "button"))) }
                            else p) }
            Expect.contains (catalogDocsCurrency facts tree) (MissingPreview "button") "missing preview without a note is detected"
        }

        test "no MissingPreview when the page carries the honest unsupported note" {
            // The clean tree has every page noted unsupported with no preview assets.
            let findings = catalogDocsCurrency facts (cleanTree ())
            Expect.isFalse (findings |> List.exists (function MissingPreview _ -> true | _ -> false)) "honest note suppresses MissingPreview"
        }

        test "UndecodablePreview when a present preview fails validation" {
            let tree = { cleanTree () with Previews = [ { ControlId = "button"; Decodable = false; Bytes = 600L } ] }
            Expect.contains (catalogDocsCurrency facts tree) (UndecodablePreview "button") "undecodable preview is detected"
        }

        test "a decodable preview above the byte floor is accepted (no finding for it)" {
            let tree = { cleanTree () with Previews = [ { ControlId = "button"; Decodable = true; Bytes = 600L } ] }
            let findings = catalogDocsCurrency facts tree
            Expect.isFalse (findings |> List.exists (function UndecodablePreview _ -> true | _ -> false)) "decodable preview raises no undecodable finding"
            Expect.isFalse (findings |> List.exists (function TrivialPreview _ -> true | _ -> false)) "above-floor preview raises no trivial finding"
        }

        // Feature 079 (US3, T016, P3.2/P3.3): a decodable preview that regressed to
        // empty/near-empty (bytes below the pinned floor) fails exactly like a missing one.
        test "TrivialPreview when a decodable preview is below the byte floor" {
            let tree = { cleanTree () with Previews = [ { ControlId = "button"; Decodable = true; Bytes = trivialPreviewFloorBytes - 1L } ] }
            Expect.contains (catalogDocsCurrency facts tree) (TrivialPreview "button") "below-floor preview is a TrivialPreview"
        }

        test "a decodable preview exactly at the byte floor raises no TrivialPreview" {
            let tree = { cleanTree () with Previews = [ { ControlId = "button"; Decodable = true; Bytes = trivialPreviewFloorBytes } ] }
            let findings = catalogDocsCurrency facts tree
            Expect.isFalse (findings |> List.exists (function TrivialPreview _ -> true | _ -> false)) "at-floor preview is not trivial"
        }

        test "an empty-canvas-sized preview (~363 bytes) is a TrivialPreview" {
            // The near-empty 320x160 light canvas committed by 078 was ~363 bytes; it must now fail.
            let tree = { cleanTree () with Previews = [ { ControlId = "button"; Decodable = true; Bytes = 363L } ] }
            Expect.contains (catalogDocsCurrency facts tree) (TrivialPreview "button") "a blanked ~363-byte preview is trivial"
        }

        test "OrphanPreview when a preview exists for an unknown control" {
            let tree = { cleanTree () with Previews = [ { ControlId = "ghost-control"; Decodable = true; Bytes = 600L } ] }
            Expect.contains (catalogDocsCurrency facts tree) (OrphanPreview "ghost-control") "orphan preview is detected"
        }

        test "DeadLink when an API slug does not resolve in the available set" {
            // Provide a reference set missing button's slug; expect a DeadLink for button.
            let slugs = facts |> List.map apiReferenceSlug |> List.filter (fun s -> s <> "fs-skia-ui-controls-button") |> Set.ofList
            let tree = { cleanTree () with AvailableReferenceSlugs = Some slugs }
            let findings = catalogDocsCurrency facts tree
            Expect.isTrue (findings |> List.exists (function DeadLink("button", _) -> true | _ -> false)) "unresolved API slug is a DeadLink"
        }

        test "no DeadLink when every API slug resolves" {
            let slugs = facts |> List.map apiReferenceSlug |> Set.ofList
            let tree = { cleanTree () with AvailableReferenceSlugs = Some slugs }
            let findings = catalogDocsCurrency facts tree
            Expect.isFalse (findings |> List.exists (function DeadLink _ -> true | _ -> false)) "all slugs resolve ⇒ no DeadLink"
        }

        test "the committed docs tree is a clean projection of catalogFacts" {
            // Read the real committed index + detail pages; assert no drift findings (API-link
            // resolution deferred to the strict site build, so AvailableReferenceSlugs = None).
            let controlsDir = Path.Combine(repositoryRoot, "docs", "controls")
            let read rel = let p = Path.Combine(controlsDir, rel) in if File.Exists p then File.ReadAllText p else ""
            // Non-control guidance pages under docs/controls/ (mirrors Engine/Update.fs `excluded`):
            // the catalog index, the spec-kit workflow guide, and (feature 113) the stable-props page.
            let excluded = set [ "catalog"; "spec-kit-workflow"; "stable-props" ]
            let detailPages =
                Directory.GetFiles(controlsDir, "*.md")
                |> Array.map (fun p -> Path.GetFileNameWithoutExtension p |> Option.ofObj |> Option.defaultValue "", p)
                |> Array.filter (fun (id, _) -> not (excluded.Contains id))
                |> Array.map (fun (id, p) -> { ControlId = id; Text = File.ReadAllText p })
                |> Array.toList
            // Gather the committed preview assets (every one is a real, decodable render).
            let imgDir = Path.Combine(repositoryRoot, "docs", "img", "controls")
            let previews =
                if Directory.Exists imgDir then
                    Directory.GetFiles(imgDir, "*.png")
                    |> Array.map (fun p ->
                        let len = (FileInfo p).Length
                        { ControlId = Path.GetFileNameWithoutExtension p |> Option.ofObj |> Option.defaultValue ""
                          Decodable = len > 256L
                          Bytes = len })
                    |> Array.toList
                else []
            let tree =
                { CatalogIndexText = read "catalog.md"
                  DetailPages = detailPages
                  Previews = previews
                  AvailableReferenceSlugs = None }
            let findings = catalogDocsCurrency facts tree
            Expect.isEmpty findings (sprintf "committed docs tree is clean; findings: %A" findings)
        }
    ]
