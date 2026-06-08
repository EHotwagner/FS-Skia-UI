module FS.Skia.UI.Build.CatalogDocsGen

// Feature 078 (US1): the published "Controls" docs section is a single-source projection of
// CatalogGen.catalogFacts onto docs targets, on the same keyed BEGIN/END GENERATED splice +
// per-region currency precedent as CatalogGen (catalog.yml / Catalog.fs). Two generated
// regions: the catalog index (`catalog-docs/index` in docs/controls/catalog.md) and one
// canonical header per control (`catalog-docs/<id>` in docs/controls/<id>.md). Hand-authored
// prose/usage/previews live OUTSIDE every marker. Pure render/splice/currency over in-memory
// text + file listings; the filesystem read/write stays at the interpreter edge (Principle IV).

open System.Globalization
open System.Text
open System.Text.RegularExpressions
open FS.Skia.UI.Build.CatalogGen

type DocFinding =
    | IndexStale
    | MissingDetailPage of controlId: string
    | StaleDetailHeader of controlId: string
    | OrphanDetailPage of controlId: string
    | MissingPreview of controlId: string
    | UndecodablePreview of controlId: string
    // Feature 079 (US3, FR-004/FR-005): a decodable preview whose committed byte size is below
    // the pinned trivial-content floor — its content has regressed to empty/near-empty. Treated
    // as a failing preview exactly like missing/undecodable.
    | TrivialPreview of controlId: string
    | OrphanPreview of controlId: string
    | DeadLink of controlId: string * target: string

type DetailPage = { ControlId: string; Text: string }
// Feature 079: `Bytes` carries the committed PNG byte size so the SkiaSharp-free gate can apply
// the trivial-content byte floor (a real structural property of the committed file, not a decode).
type PreviewAsset = { ControlId: string; Decodable: bool; Bytes: int64 }

type DocsTree =
    { CatalogIndexText: string
      DetailPages: DetailPage list
      Previews: PreviewAsset list
      AvailableReferenceSlugs: Set<string> option }

let catalogDocsRelDir = "docs/controls"
let catalogIndexRel = "docs/controls/catalog.md"
let previewRelDir = "docs/img/controls"
let detailPageRel (id: string) = sprintf "docs/controls/%s.md" id
let previewRel (id: string) = sprintf "docs/img/controls/%s.png" id

let previewUnsupportedMarker = "preview-status: unsupported"

// Feature 079 (US1 T012, R3) — the pinned trivial-content byte floor `T`. A committed
// demonstrative preview MUST exceed this; a near-empty 320×160 light canvas compresses to
// ~363 bytes, while the smallest committed demonstrative render is 486 bytes (icon-button).
// `T = 420` sits between them with headroom on both sides (~57 above the empty baseline,
// ~66 below the smallest demonstrative) and is a real structural property of the committed
// bytes readable without decoding pixels, keeping the governance build SkiaSharp-free.
let trivialPreviewFloorBytes = 420L

// Feature 079 — the deterministic render-only harness that regenerates the demonstrative
// previews (named in trivial/undecodable remedies so a failure points at the fix).
let private rerenderCommand = "dotnet run --project tests/ControlsPreview.Harness -- --render"

let private regenCommand = "./fake.sh build -t RefreshSurfaceBaselines"

// ---------------------------------------------------------------------------------------
// API-reference slug derivation (research R2). The 072 breadth-expansion controls are typed
// front-door compositions with no dedicated legacy module, so their published reference page
// is the `…-controls-typed-<module>` page; every other control's is `…-controls-<module>`. The
// chosen slug is enforced to resolve by the link-resolution clause + the strict site build.
// ---------------------------------------------------------------------------------------
let private typedOnlyModules =
    Set.ofList [ "ToggleButton"; "SplitButton"; "DatePicker"; "TimePicker"; "ColorPicker" ]

let apiReferenceSlug (fact: TypedCatalogFact) : string =
    let m = fact.Module.ToLowerInvariant()
    if typedOnlyModules.Contains fact.Module then
        sprintf "fs-skia-ui-controls-typed-%s" m
    else
        sprintf "fs-skia-ui-controls-%s" m

let apiReferenceHref (fact: TypedCatalogFact) : string =
    sprintf "../reference/%s.html" (apiReferenceSlug fact)

// ---------------------------------------------------------------------------------------
// Renderers. Deterministic, invariant-culture, byte-stable so the splice diff is empty after a
// clean RefreshSurfaceBaselines and the currency check is meaningful.
// ---------------------------------------------------------------------------------------
let private titleCase (category: string) =
    if category.Length = 0 then category
    else string (System.Char.ToUpperInvariant category.[0]) + category.Substring 1

// Category order = first appearance in catalogFacts; within-category order = catalogFacts order.
let private categoriesInOrder (facts: TypedCatalogFact list) : string list =
    facts
    |> List.fold (fun acc f -> if List.contains f.Category acc then acc else acc @ [ f.Category ]) []

let renderCatalogIndex (facts: TypedCatalogFact list) : string =
    let sb = StringBuilder()
    let append (s: string) = sb.Append(s).Append('\n') |> ignore
    append (sprintf "**%d supported controls**, grouped by category." (List.length facts))
    for category in categoriesInOrder facts do
        let rows = facts |> List.filter (fun f -> f.Category = category)
        append ""
        append (sprintf "### %s" (titleCase category))
        append ""
        append "| Control | Purpose |"
        append "|---------|---------|"
        for f in rows do
            sb.Append(sprintf "| [%s](%s.html) | %s |" f.DisplayName f.Id f.Purpose).Append('\n')
            |> ignore
    // Trim the trailing newline so the region body is byte-stable regardless of marker spacing.
    sb.ToString().TrimEnd('\n')

let renderDetailHeader (fact: TypedCatalogFact) : string =
    [ sprintf "# %s" fact.DisplayName
      ""
      sprintf "- **Category:** %s" fact.Category
      sprintf "- **Purpose:** %s" fact.Purpose
      sprintf "- **API reference:** [FS.Skia.UI.Controls.%s](%s)" fact.Module (apiReferenceHref fact)
      ""
      "[← Back to the controls catalog](catalog.html)" ]
    |> String.concat "\n"

let renderMap (facts: TypedCatalogFact list) : Map<string, string> =
    [ yield "index", renderCatalogIndex facts
      for f in facts -> f.Id, renderDetailHeader f ]
    |> Map.ofList

// ---------------------------------------------------------------------------------------
// Keyed marked-region splice + per-region currency. Markdown HTML-comment markers
// (`<!-- BEGIN GENERATED: catalog-docs/<key> --> … <!-- END GENERATED: catalog-docs/<key> -->`).
// The leading indent before BEGIN is preserved; the END marker indent is absorbed and re-emitted.
// ---------------------------------------------------------------------------------------
let private normalizeNewlines (text: string) = text.Replace("\r\n", "\n")

let private regionRegex =
    Regex(
        @"<!-- BEGIN GENERATED: catalog-docs/(?<key>[^\s]+) -->\r?\n(?<inner>.*?)\r?\n[ \t]*<!-- END GENERATED: catalog-docs/\k<key> -->",
        RegexOptions.Singleline ||| RegexOptions.Compiled)

let spliceCatalogDocs (renders: Map<string, string>) (fileText: string) : string =
    regionRegex.Replace(
        fileText,
        (fun (m: Match) ->
            let key = m.Groups.["key"].Value

            match Map.tryFind key renders with
            | Some rendered ->
                sprintf
                    "<!-- BEGIN GENERATED: catalog-docs/%s -->\n%s\n<!-- END GENERATED: catalog-docs/%s -->"
                    key
                    rendered
                    key
            | None -> m.Value))

let private regionInner (key: string) (fileText: string) : string option =
    let m = regionRegex.Match(normalizeNewlines fileText)

    let rec scan (m: Match) =
        if not m.Success then None
        elif m.Groups.["key"].Value = key then Some(m.Groups.["inner"].Value)
        else scan (m.NextMatch())

    scan m

// ---------------------------------------------------------------------------------------
// Currency. Pure over (facts, observed tree). Empty ⇒ PASS.
// ---------------------------------------------------------------------------------------
let catalogDocsCurrency (facts: TypedCatalogFact list) (tree: DocsTree) : DocFinding list =
    let factIds = facts |> List.map (fun f -> f.Id) |> Set.ofList
    let pageById = tree.DetailPages |> List.map (fun p -> p.ControlId, p) |> Map.ofList
    let previewById = tree.Previews |> List.map (fun p -> p.ControlId, p) |> Map.ofList
    let findings = ResizeArray<DocFinding>()

    // 1. Index currency.
    match regionInner "index" tree.CatalogIndexText with
    | Some inner when inner = renderCatalogIndex facts -> ()
    | _ -> findings.Add IndexStale

    // 2. Per-control completeness / header currency / preview honesty / API-link resolution.
    for fact in facts do
        match Map.tryFind fact.Id pageById with
        | None -> findings.Add(MissingDetailPage fact.Id)
        | Some page ->
            match regionInner fact.Id page.Text with
            | Some inner when inner = renderDetailHeader fact -> ()
            | _ -> findings.Add(StaleDetailHeader fact.Id)

        match Map.tryFind fact.Id previewById with
        | Some preview ->
            // Undecodable takes priority; an otherwise-decodable preview below the byte floor is
            // a trivial/near-empty regression (Feature 079, FR-005).
            if not preview.Decodable then findings.Add(UndecodablePreview fact.Id)
            elif preview.Bytes < trivialPreviewFloorBytes then findings.Add(TrivialPreview fact.Id)
        | None ->
            // No preview asset: honest only if the detail page declares it unsupported.
            let hasHonestNote =
                match Map.tryFind fact.Id pageById with
                | Some page -> page.Text.Contains previewUnsupportedMarker
                | None -> false

            if not hasHonestNote then findings.Add(MissingPreview fact.Id)

        match tree.AvailableReferenceSlugs with
        | Some slugs when not (slugs.Contains(apiReferenceSlug fact)) ->
            findings.Add(DeadLink(fact.Id, apiReferenceHref fact))
        | _ -> ()

    // 3. Orphans (id present on disk but not in catalogFacts), sorted for stable output.
    for page in tree.DetailPages |> List.sortBy (fun p -> p.ControlId) do
        if not (factIds.Contains page.ControlId) then findings.Add(OrphanDetailPage page.ControlId)

    for preview in tree.Previews |> List.sortBy (fun p -> p.ControlId) do
        if not (factIds.Contains preview.ControlId) then findings.Add(OrphanPreview preview.ControlId)

    List.ofSeq findings

let currencyDrift (findings: DocFinding list) : string list =
    findings
    |> List.map (fun finding ->
        match finding with
        | IndexStale ->
            sprintf
                "%s catalog-docs/index region no longer matches a fresh render of CatalogGen.catalogFacts. Regenerate via %s."
                catalogIndexRel
                regenCommand
        | MissingDetailPage id ->
            sprintf
                "%s is missing for supported control %s. Author the detail page (commit the stub with its catalog-docs/%s marker pair, then %s)."
                (detailPageRel id)
                id
                id
                regenCommand
        | StaleDetailHeader id ->
            sprintf
                "%s catalog-docs/%s header region is stale or missing relative to CatalogGen.catalogFacts. Regenerate via %s."
                (detailPageRel id)
                id
                regenCommand
        | OrphanDetailPage id ->
            sprintf
                "%s is an orphan detail page — control %s is not in CatalogGen.catalogFacts. Remove the page."
                (detailPageRel id)
                id
        | MissingPreview id ->
            sprintf
                "%s is missing and %s carries no honest unsupported note. Render the preview through the render-only path, or add a '%s' note to the page (never a 1x1/placeholder image)."
                (previewRel id)
                (detailPageRel id)
                previewUnsupportedMarker
        | UndecodablePreview id ->
            sprintf
                "%s failed PNG validation (undecodable / 1x1). Re-render it through the deterministic render-only path (%s)."
                (previewRel id)
                rerenderCommand
        | TrivialPreview id ->
            sprintf
                "%s is below the %d-byte trivial-content floor — its content has regressed to empty/near-empty and fails like a missing preview. Re-render it demonstratively through the deterministic render-only path (%s)."
                (previewRel id)
                (int trivialPreviewFloorBytes)
                rerenderCommand
        | OrphanPreview id ->
            sprintf
                "%s is an orphan preview — control %s is not in CatalogGen.catalogFacts. Remove the asset."
                (previewRel id)
                id
        | DeadLink(id, target) ->
            sprintf
                "%s links to API reference '%s' which does not resolve in the built site. Fix the slug (research R2) or the target page."
                (detailPageRel id)
                target)
