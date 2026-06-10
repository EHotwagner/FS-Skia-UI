// CatalogGen.fsi — the typed single-source generator for the six 065-typed control
// catalog rows (feature 066, FR-001/FR-002/FR-004/FR-005). Build-tooling scope
// (`build/Governance`, net10.0); NOT a tracked runtime surface baseline (same status as
// SkillTreeGen.fsi / GovernedBlocks). Pure generation + currency over in-memory text;
// every filesystem read/write stays at the Engine/Interpret.fs edge (Principle IV).
//
// Mirrors the ContractView (Routing.fs -> validation.contract.yml) precedent: a canonical
// typed value here is the one place each fact is declared, and the renderers splice it into
// per-control `typed-catalog/<id>` marked regions in catalog.yml / Catalog.fs. The 41
// hand-authored rows lie outside every marker and are never read or written by generation.
module FS.Skia.UI.Build.CatalogGen

/// The single-source catalog-relevant facts for one 065 typed control. Field names mirror
/// the catalog row so the mapping is mechanical and reviewable (data-model §`TypedCatalogFact`).
type TypedCatalogFact =
    { Id: string
      DisplayName: string
      Category: string
      Module: string
      /// Feature 089 (TYPED-SURFACE-1): the `FS.Skia.UI.Controls.Typed` module
      /// name that realizes this control — the single-source id → typed-module
      /// pointer rendered into `catalog.yml` (`typedModule:`). Equals `Module`
      /// for most controls; the shared-builder collection/menu rows name their
      /// distinct typed module. Every value names a module declared in an
      /// enrolled `src/Controls/Widgets/*.fsi` (published api-surface), never a
      /// copy of the Props field names.
      TypedModule: string
      Purpose: string
      RequiredAttributes: string list
      Events: string list
      AccessibilityRole: string }

/// Status of one generated region relative to a fresh render.
type RegionStatus =
    | Current
    | Stale
    | Missing

/// Currency result for one (control, file) region.
type CatalogCurrency =
    { ControlId: string
      FilePath: string
      Status: RegionStatus }

/// The authoritative six-control fact table — the single source (FR-001). Covers exactly
/// { text-block, button, check-box, stack, text-box, data-grid }, in catalog file order.
val catalogFacts: TypedCatalogFact list

/// Repo-relative home files the six rows are spliced into.
val catalogYmlRel: string
val catalogFsRel: string

/// Render one fact to its on-disk row text for each target (byte-identical to today's
/// hand-authored row — FR-004). Pure. The F# render carries the 10-space list indentation
/// and appends `|> withChartDataGridEvidence` for `data-grid`; the YAML render carries the
/// `- id:` block indentation and appends the chart/data-grid evidence path for `data-grid`.
val renderFSharpRow: fact: TypedCatalogFact -> string
val renderYamlRow: fact: TypedCatalogFact -> string

/// Splice every fact's rendered row into its `typed-catalog/<id>` marked region in the
/// given file text, leaving every byte outside the markers untouched (FR-003). Unknown
/// marker ids are left untouched. Idempotent on an already-current file. Pure.
val spliceFSharp: fileText: string -> string
val spliceYaml: fileText: string -> string

/// Compare each on-disk region (both files) against a fresh render. Clean iff every
/// returned status is Current. Pure.
val currency: catalogYmlText: string -> catalogFsText: string -> CatalogCurrency list

/// True when every region is current.
val isCurrent: currency: CatalogCurrency list -> bool

/// Actionable drift diagnostics — one per stale/missing region, naming the divergent
/// control, the file, and `./fake.sh build -t RefreshSurfaceBaselines` (FR-005). Empty
/// when current.
val currencyDrift: currency: CatalogCurrency list -> string list
