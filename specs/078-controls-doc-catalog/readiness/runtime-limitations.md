# Runtime limitations and failure diagnostics (078)

## Unsupported-scope handling / failure diagnostics

- **Catalog docs currency drift**: `ControlsCatalogDocsCheck`
  (`CatalogDocsGen.catalogDocsCurrency` / `currencyDrift`) names each drift
  finding (`IndexStale`, `MissingDetailPage`, `StaleDetailHeader`,
  `OrphanDetailPage`, `MissingPreview`, `UndecodablePreview`, `OrphanPreview`,
  `DeadLink`) **and** the regeneration command
  `./fake.sh build -t RefreshSurfaceBaselines` on any drift (FR-005 / SC-004). A
  removed control's surviving page/preview is reported as an orphan (loud), never
  silently passed.
- **Preview honesty**: each required preview is validated with
  `Testing.readPngArtifact` (decodable, non-1×1 dimensions, non-trivial content).
  A control that cannot be honestly rendered carries an explicit unsupported note
  on its page and **no** asset — never a 1×1 / metadata-only / fabricated image.
- **Render evidence is render-only**: control preview PNGs are produced through
  the existing deterministic render-only evidence path as committed source
  assets — **no GPU window**. The docs CI consumes the committed PNGs and stays
  GPU-free.

## Platform runtime boundary

The render-only preview path and the supported control surface run on the
supported runtime only:

- **.NET 10 desktop** host (Windows and Linux desktop).
- **Vulkan**-backed GPU path for the windowed host; the render-only preview path
  exercises the raster output without a persistent window.
- **SkiaSharp preview** is the pinned rendering dependency.
- **unsupported macOS/mobile/browser**: these targets are out of runtime scope.
- **no software-renderer fallback**: there is no software rasterizer substitute;
  unsupported hosts/controls are reported as unsupported, not silently
  downgraded.

## Deferred-scope boundary

No new controls, no control-behavior or visual-redesign work, no docs-theme
redesign, no API-reference-generator redesign, no release/distribution/platform
changes, and no live Penpot/MCP integration (documented only). Any preview that
cannot be honestly produced this iteration is a bounded, disclosed follow-up, not
a silent omission.
