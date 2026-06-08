# Quickstart: Controls Catalog Docs (feature 078)

The author + verify loop for the Controls section. All FAKE-backed commands run
**sequentially** (shared `.fake` state).

## 1. See what gates apply

```bash
./fake.sh build -t Route          # prints tier + minimal gate list for the diff
```

A docs-catalog change selects `ControlsCatalogDocsCheck` (plus governance gates for
the generator/routing edits).

## 2. Generate index + detail-page headers from the single source

```bash
./fake.sh build -t RefreshSurfaceBaselines
```

Fills every `BEGIN/END GENERATED: catalog-docs/<key>` region in
`docs/controls/catalog.md` and `docs/controls/<id>.md` from `CatalogGen.catalogFacts`.
Adding a control? Commit its detail-page stub with empty marker pair **first**, then
refresh (markers are filled, never invented).

## 3. Author prose, usage, and previews

- Write explanation/usage in each `docs/controls/<id>.md` **outside** the generated
  region.
- Place each render-only preview at `docs/img/controls/<id>.png` and embed it. For a
  control that cannot be honestly rendered, write the explicit unsupported note —
  never a placeholder/1×1 image.
- Write the narrative + Penpot subsection in `docs/controls/spec-kit-workflow.md`.

## 4. Validate currency, completeness, previews, links

```bash
./fake.sh build -t ControlsCatalogDocsCheck
```

PASS ⇒ index matches the catalog (count + entries), every supported control has a
current detail page, every required preview is present/decodable/non-orphan, and every
generated link resolves. FAIL names the finding + remedy → see
`specs/078-controls-doc-catalog/readiness/controls-catalog-docs.md`.

## 5. Build the site

```bash
dotnet tool restore
dotnet fsdocs build --strict --eval     # full site to output/
dotnet fsdocs watch                     # live-reload while authoring
```

Confirm the Controls section appears in nav, ordered narrative → Penpot subsection →
catalog index → detail pages, with previews and resolving API links.

## 6. Governance suite (when escalated)

```bash
./fake.sh build -t Dev
./fake.sh build -t ControlsCatalogDocsCheck
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## Acceptance smoke (maps to Success Criteria)

- Open the site index → reach the catalog page in one step; it lists **52** controls
  grouped by category (SC-001/SC-002).
- Click any control → its detail page shows name/category/purpose, a preview (or honest
  note), and a resolving API link (SC-003).
- Add/rename/remove one control in `catalogFacts`, refresh → index updates with zero
  hand-edits; a missing detail page or stale index fails the check (SC-004).
