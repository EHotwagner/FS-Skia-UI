# US1 independent validation path (078) — T020

**Story**: Discover and understand every control (SC-001/SC-002/SC-003/SC-004).

Reproduce on the built `output/` site:

1. Open the site index (`output/index.html`). The consumer entry-point nav lists
   **Controls Catalog** → one click to `output/controls/catalog.html` (SC-001).
2. The catalog lists **52** controls grouped by category (display, input, selection,
   data, layout, navigation, overlay, feedback, chart, graph, custom). The count and
   entries are generated from `CatalogGen.catalogFacts`, so they exactly match the
   shipped control set (SC-002), enforced by `ControlsCatalogDocsCheck`.
3. Click any control (e.g. **Button**) → its detail page
   (`output/controls/button.html`) shows the generated header (name, category,
   purpose) plus a resolving **API reference** link
   (`output/reference/fs-skia-ui-controls-button.html`) and a back-to-catalog link
   (SC-003). Authored prose explains the control, names its required attributes,
   events, accessibility role, and the typed module to build it with.
4. Add / rename / remove a control in `catalogFacts` and run
   `./fake.sh build -t RefreshSurfaceBaselines` → the index and detail headers update
   with **zero** hand-edits; a stale index, a missing detail page, or an orphan page
   is caught by `ControlsCatalogDocsCheck` (`IndexStale` / `MissingDetailPage` /
   `OrphanDetailPage`) (SC-004).

**Outcome**: PASS — the path is reachable in one step from the index, the catalog is
complete and current, and every detail page resolves its API link. Evidence:
`docs-build.md` (strict site build), `controls-catalog-docs.md` (gate PASS).

**Caveat**: per-control preview images are deferred this iteration (disclosed in
`controls-preview-evidence.md`); each detail page carries an honest no-preview note
in place of an image, so the path is fully navigable with no broken links.
