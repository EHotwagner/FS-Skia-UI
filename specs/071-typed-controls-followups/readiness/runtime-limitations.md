# Runtime limitations and failure diagnostics (071) — T005

## Unsupported-scope handling / failure diagnostics

- **Catalog currency drift**: `ControlsCatalogGenerationCheck`
  (`CatalogGen.currency` / `currencyDrift`) names the stale
  `typed-catalog/<id>` region **and** the regeneration command
  `./fake.sh build -t RefreshSurfaceBaselines` on any drift (FR-003 / SC-002). A
  removed region is reported `Missing` (loud), never silently passed.
- **Fixture gap**: the `066` fixture-iteration cross-check
  (`tests/Controls.Tests/CatalogTests.fs`) names the missing fixture id when a
  `Catalog.fs.<id>.txt` / `catalog.yml.<id>.txt` pair is absent for a fact.
- **Render evidence is render-only**: typed-gallery viewport evidence is captured
  headless through the `Widget.toControl` → `Control.render` IR path — **no GPU
  window**. It is a render/interaction smoke, not a substitute for the persistent
  gallery launch (T015).

## Platform runtime boundary

The render/accessibility coverage and evidence run on the supported runtime only:

- **.NET 10 desktop** host (Windows and Linux desktop).
- **Vulkan**-backed GPU path for the windowed host; the headless render path used
  here exercises the same IR without a window.
- **SkiaSharp preview** is the pinned rendering dependency.
- **unsupported macOS/mobile/browser**: these targets are out of runtime scope.
- **no software-renderer fallback**: there is no software rasterizer substitute;
  unsupported hosts are reported as unsupported, not silently downgraded.

## Deferred-scope boundary (FR-011)

Catalog **expansion** (controls beyond the 47 rows), overlays/virtualization,
motion/animation, live Penpot/MCP, design-token value changes, legacy-API
deprecation, `067` keyed-reconciliation, and `068` `Controls.Elmish` changes all
remain deferred to a later `071+` feature. The typed gallery panel is
deliberately representative (≥1 per mechanic group), not exhaustive.
