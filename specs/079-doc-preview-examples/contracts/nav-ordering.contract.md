# Contract: Controls Nav Repositioning

Scope: repositioning the Controls category in the published-docs sidebar (FR-011). Mechanism
is fsdocs `categoryindex` renumbering only (R6) — no page nesting, no file relocation.

## N1 — Built-nav order (FR-011, SC-006)

In the built site sidebar, the **Controls** category renders **immediately below Examples**
and **above Guides**.
- **N1.1** Controls remains its own top-level category (`category: Controls`).
- **N1.2** Concrete indices (R6): Examples=7, **Controls=8**, Roadmap=9, Guides=10; nothing
  renders between Examples and Controls.
- (Test/verify: inspect `dotnet fsdocs build` output sidebar order; recorded in
  `docs-build.md`.)

## N2 — No file relocation, no page nesting (FR-011)

Detail pages and preview assets stay under `docs/controls/` and `docs/img/controls/`; only
`categoryindex` frontmatter values change. Within-category `index` values are unchanged
(narrative=1, catalog=2, detail=3..54). URLs/slugs are unchanged.
- (Test: git diff confirms only `categoryindex` lines change in `docs/controls/*` and the
  renumbered peers; no file renames/moves.)

## N3 — All cross-links resolve (FR-011)

Every existing cross-link into the Controls section (from `docs/architecture/controls.md`,
`docs/controls-design/*`, `docs/index.md`, and the catalog/detail pages) continues to
resolve after the renumber.
- (Test: `ControlsCatalogDocsCheck` `DeadLink` finding stays clean; `--strict` site build
  reports no broken links.)
