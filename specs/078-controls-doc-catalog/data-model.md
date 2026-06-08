# Data Model: Authoritative Controls Catalog in Published Docs

This feature introduces **no new runtime types**. The "model" is the projection from
the existing single source `CatalogGen.catalogFacts` (52 rows) onto generated docs
artifacts, plus the validation state the currency check computes.

## Authoritative source (existing — unchanged)

**`CatalogGen.catalogFacts : TypedCatalogFact list`** (`build/Governance/CatalogGen.fs`)
is the single source of truth. Each `TypedCatalogFact` carries (per existing code):

| Field | Use in docs |
|---|---|
| `Id` (kebab, e.g. `text-block`) | detail-page filename (`docs/controls/<id>.md`), preview filename (`docs/img/controls/<id>.png`), index anchor |
| `DisplayName` (e.g. `Text Block`) | index entry + detail-page H1 |
| `Category` (display, input, selection, navigation, layout, feedback, data, chart, graph, overlay, custom) | index grouping + detail-page header |
| `Module` (e.g. `TextBlock`) | API-reference link slug derivation (R2) |
| `Purpose` (one line) | index one-line purpose + detail-page header |
| `RequiredAttributes`, `Events`, `AccessibilityRole` | available context for authored prose (not required in generated header) |

**Counts (current)**: 52 controls — display 7, input 10, selection 7, navigation 4,
layout 8, feedback 4, data 3, chart 4, graph 1, overlay 3, custom 1. The displayed
total MUST equal this supported count and track it (FR-010/SC-002).

## Generated / authored docs entities

### 1. Controls Catalog index (`docs/controls/catalog.md`)
- **Generated region** (`BEGIN/END GENERATED: catalog-docs/index`): a table/list of
  all controls grouped by category, each row = DisplayName (linking to its detail
  page) + one-line Purpose, plus the total count. Rendered purely from `catalogFacts`.
- **Authored region**: surrounding intro prose and cross-links. Never hand-edit the
  generated region.
- **Validation**: byte-identical to the render of `catalogFacts` (FR-004, FR-005 clause (a),
  SC-004).

### 2. Control detail page (`docs/controls/<id>.md`) — one per control
- **Generated header region** (`BEGIN/END GENERATED: catalog-docs/<id>`): H1
  DisplayName, Category, Purpose, and the resolving API-reference link derived from
  `Module`.
- **Authored body**: explanation/usage prose; the catalog usage example where one
  exists (else honest omission, never fabricated); the preview embed (or honest
  unsupported note).
- **Validation**: page MUST exist for every supported control; header region current;
  no orphan page for a removed control (FR-002, FR-005 clause (b), SC-003).

### 3. Control preview asset (`docs/img/controls/<id>.png`)
- A render-only PNG, or **absent** with an honest unsupported note on the page.
- **Validation**: required preview present, decodable, non-trivial content (reuse
  `Testing.readPngArtifact`), not stale, not orphaned (FR-003a, SC-003). 1×1 /
  metadata-only / fabricated images are rejected.

### 4. Usage narrative (`docs/controls/spec-kit-workflow.md`)
- Hand-authored. Covers control selection/authoring/validation across specify → plan
  → tasks → implement, linking to authoring guidance/skills (typed-controls,
  design-tokens). Includes a **Penpot/design-tokens `##` subsection** describing the
  token→theme path and linking the design-token single source.
- **Validation**: all cross-links resolve (FR-009).

## Validation state (computed by `ControlsCatalogDocsCheck`)

A pure function over (`catalogFacts`, file contents/listing) produces a list of
drift findings; empty ⇒ PASS. Finding classes:

| Class | Trigger | Remedy in report |
|---|---|---|
| `IndexStale` | generated index region ≠ render of `catalogFacts` | `RefreshSurfaceBaselines` |
| `MissingDetailPage` | a supported control has no `<id>.md` | author the page |
| `StaleDetailHeader` | detail header region ≠ render | `RefreshSurfaceBaselines` |
| `OrphanDetailPage` | `<id>.md` for an id not in `catalogFacts` | remove the page |
| `MissingPreview` | required preview absent and no honest unsupported note | render or declare unsupported |
| `UndecodablePreview` | preview present but fails PNG validation | re-render |
| `OrphanPreview` | preview for an id not in `catalogFacts` | remove the asset |
| `DeadLink` | a generated index→detail or detail→API link does not resolve | fix slug / target |

## State transitions (control lifecycle vs. docs)

Adding a control to `catalogFacts` → on `RefreshSurfaceBaselines`, the index gains
the entry and a detail-page header region; the page-existence + preview obligations
become enforced by the gate (page authored, preview rendered or declared
unsupported). Renaming → index/header update + page/preview rename. Removing → entry
disappears; surviving page/preview becomes an orphan finding. The published site
therefore never lists a control that no longer ships (Edge cases, FR-005).
