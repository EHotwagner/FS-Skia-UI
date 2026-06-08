# Tier & Impact Record (078) — T003

- **Tier**: Tier 2 (internal). This feature changes **no** public product `.fsi`
  surface, package identity, or runtime behavior. It escalates on the
  **governance / generated-guidance** path because it adds a governance gate and
  generated docs artifacts; `Route` selects the authoritative gate list for the
  actual diff.
- **Affected layers**: `build/Governance/**` (new `CatalogDocsGen` generator +
  `ControlsCatalogDocsCheck` gate + routing/target wiring) and `docs/**` (new
  Controls section: catalog index, per-control detail pages, previews, narrative).
- **Public-API / `.fsi` impact**: none. The generator/gate **read** the existing
  public control surface and `CatalogGen.catalogFacts`; they redefine nothing.
  (The build-tool `FS.Skia.UI.Build` internal `.fsi` gains the new module's
  signature — build-tool internal surface, not product public contract.)
- **MVU/Elmish applicability**: N/A as a runtime concern. The generator and
  currency check are pure functions; all file I/O lives at the `Engine/Update.fs`
  / `Front/Governance.fs` interpreter edge (Principle IV honored by shape).
- **Evidence obligations**:
  - generated catalog index current vs `catalogFacts` (`ControlsCatalogDocsCheck`)
  - one detail page per supported control, header region current
  - per-control preview present/decodable, or an honest unsupported note
  - `ControlsCatalogDocsCheck` PASS report (`controls-catalog-docs.md`)
  - site build (`dotnet fsdocs build --strict --eval`, `docs-build.md`)
