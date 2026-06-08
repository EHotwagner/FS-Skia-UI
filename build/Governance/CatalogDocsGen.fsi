// CatalogDocsGen.fsi — the single-source generator + currency check for the published
// "Controls" docs section (feature 078). Build-tooling scope (`build/Governance`, net10.0);
// NOT a tracked runtime surface baseline (same status as CatalogGen.fsi / DesignTokenGen.fsi).
//
// Reuses the CatalogGen single source (`catalogFacts`) and the keyed BEGIN/END GENERATED
// splice-marker precedent, projected onto docs targets:
//   - the catalog index region in `docs/controls/catalog.md` (key `catalog-docs/index`), and
//   - one canonical header region per control in `docs/controls/<id>.md` (key `catalog-docs/<id>`).
// Hand-authored prose/usage/previews live OUTSIDE every marker and are never read or written by
// generation. Pure render/splice/currency over in-memory text + file listings; every filesystem
// read/write stays at the Front/Governance.fs + Engine/Update.fs interpreter edge (Principle IV).
module FS.Skia.UI.Build.CatalogDocsGen

open FS.Skia.UI.Build.CatalogGen

/// One drift finding produced by the currency check. Empty list ⇒ the docs section is a
/// current, complete, honest projection of `catalogFacts` (data-model §"Validation state").
type DocFinding =
    /// The generated catalog-index region no longer matches a fresh render of `catalogFacts`.
    | IndexStale
    /// A supported control has no `docs/controls/<id>.md` detail page.
    | MissingDetailPage of controlId: string
    /// A detail page's generated header region no longer matches a fresh render.
    | StaleDetailHeader of controlId: string
    /// A `docs/controls/<id>.md` exists for an id not in `catalogFacts`.
    | OrphanDetailPage of controlId: string
    /// A required preview is absent and the detail page carries no honest unsupported note.
    | MissingPreview of controlId: string
    /// A preview is present but fails PNG structural validation (undecodable / 1x1).
    | UndecodablePreview of controlId: string
    /// A decodable preview whose committed byte size is below `trivialPreviewFloorBytes` —
    /// its content has regressed to empty/near-empty (Feature 079, FR-004/FR-005). Fails
    /// like a missing preview.
    | TrivialPreview of controlId: string
    /// A `docs/img/controls/<id>.png` exists for an id not in `catalogFacts`.
    | OrphanPreview of controlId: string
    /// A generated detail→API-reference link does not resolve in the built site.
    | DeadLink of controlId: string * target: string

/// A present detail page: its control id (filename stem) and full file text.
type DetailPage = { ControlId: string; Text: string }

/// A present preview asset: its control id, whether it passed PNG structural validation
/// (decodable, non-1x1 dimensions), and its committed byte size (Feature 079) for the
/// trivial-content byte floor — a real structural property read without decoding pixels.
type PreviewAsset = { ControlId: string; Decodable: bool; Bytes: int64 }

/// The observed docs tree the currency check reads (gathered at the interpreter edge).
type DocsTree =
    { /// Full text of `docs/controls/catalog.md` ("" when missing).
      CatalogIndexText: string
      /// Every `docs/controls/<id>.md` except `catalog.md` / `spec-kit-workflow.md`.
      DetailPages: DetailPage list
      /// Every `docs/img/controls/<id>.png` with its validation result.
      Previews: PreviewAsset list
      /// The reference-page slug set from `output/reference/` (without the `.html`),
      /// or `None` when no built site is present — then API-link resolution is deferred
      /// to the authoritative `dotnet fsdocs build --strict` step (DeadLink is not raised). */
      AvailableReferenceSlugs: Set<string> option }

/// Repo-relative locations of the generated docs artifacts.
val catalogDocsRelDir: string
val catalogIndexRel: string
val previewRelDir: string
/// `docs/controls/<id>.md` for a control id.
val detailPageRel: id: string -> string
/// `docs/img/controls/<id>.png` for a control id.
val previewRel: id: string -> string

/// The honest-unsupported-note marker a detail page must carry when its control has no
/// preview asset (so an honest omission is distinguishable from an accidental one).
val previewUnsupportedMarker: string

/// The pinned trivial-content byte floor `T` (Feature 079, R3/T012). A committed
/// demonstrative preview MUST exceed this; it sits between the ~363-byte near-empty
/// 320×160 canvas and the smallest committed demonstrative render (486 bytes), so a preview
/// regressing to empty/near-empty drops below it and fails. Read without decoding pixels.
val trivialPreviewFloorBytes: int64

/// The fsdocs reference-page slug (without `reference/` prefix or `.html`) for a control's
/// API page, derived from its `Module` (research R2). The 072 typed-front-door-only controls
/// resolve to the `…-controls-typed-<module>` page; every other control to `…-controls-<module>`.
val apiReferenceSlug: fact: TypedCatalogFact -> string

/// The relative href a detail page (`docs/controls/<id>.md`) uses to link its API page.
val apiReferenceHref: fact: TypedCatalogFact -> string

/// Render the catalog-index region body: controls grouped by `Category` (category order =
/// first appearance in `catalogFacts`; within-category order = `catalogFacts` order), each row
/// linking `DisplayName` → `<id>.html` with the one-line `Purpose`, plus the total count.
/// Deterministic, invariant-culture, byte-stable. Pure.
val renderCatalogIndex: facts: TypedCatalogFact list -> string

/// Render one control's canonical detail-page header region body: H1 `DisplayName`, a Category
/// line, the `Purpose`, the resolving API-reference link, and a back-to-index link.
/// Deterministic, byte-stable. Pure.
val renderDetailHeader: fact: TypedCatalogFact -> string

/// The render map keyed by region key: `"index"` → the index body, and each control id →
/// its detail-header body. Pure.
val renderMap: facts: TypedCatalogFact list -> Map<string, string>

/// Replace the body of every `<!-- BEGIN GENERATED: catalog-docs/<key> -->` …
/// `<!-- END GENERATED: catalog-docs/<key> -->` region whose key is present in `renders`,
/// leaving every byte outside the markers untouched. Never invents markers; idempotent on an
/// already-current file (memory `catalog-splice-marker-insertion`). Pure.
val spliceCatalogDocs: renders: Map<string, string> -> fileText: string -> string

/// Compute the drift findings for the observed docs tree against `catalogFacts`. Empty ⇒ PASS.
/// Pure (data-model §"Validation state").
val catalogDocsCurrency: facts: TypedCatalogFact list -> tree: DocsTree -> DocFinding list

/// Actionable drift diagnostics — one line per finding, naming the control/target and the
/// remedy (`./fake.sh build -t RefreshSurfaceBaselines`, author the page, render/declare the
/// preview, fix the slug). Empty when current. Pure.
val currencyDrift: findings: DocFinding list -> string list
