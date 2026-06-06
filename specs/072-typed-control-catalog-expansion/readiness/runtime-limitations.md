# Runtime Limitations & Unsupported Scope — Expansion (072)

## Unsupported scope (deferred)

This feature ships a **representative reference slice** (5 controls spanning the
three named families), not exhaustive family coverage. Out of scope and deferred
to later `071+` features: every other button/picker/date-time control, overlays as
a standalone feature, list/grid virtualization, motion/animation, Penpot/MCP
design-sync. A full color-wheel/gradient `ColorPicker` is out of scope (it would
need new rendering); this feature ships the palette/swatch picker only.

## Unsupported-scope handling / failure diagnostics

- **Catalog currency drift**: `ControlsCatalogGenerationCheck` names the stale
  `typed-catalog/<id>` region **and** the regeneration command
  `./fake.sh build -t RefreshSurfaceBaselines` on any drift; a removed region is
  reported `Missing` (loud), never silently passed.
- **Render evidence is render-only**: new-control viewport evidence is captured
  headless through the `Widget.toControl` → `Control.render` IR path — **no GPU
  window** — and is byte-identical on re-capture.
- **No new state primitive**: popup visibility (`IsOpen`) and selection values are
  product-owned `Props`, reapplied each render; there is no framework-owned
  ephemeral popup state.

## Platform runtime boundary

The render/accessibility coverage and evidence run on the supported runtime only:

- **.NET 10 desktop** host (Windows and Linux desktop).
- **Vulkan**-backed GPU path for the windowed host; the headless render path used
  here exercises the same IR without a window.
- **SkiaSharp preview** is the pinned rendering dependency.
- **unsupported macOS/mobile/browser**: these targets are out of runtime scope.
- **no software-renderer fallback**: there is no software rasterizer substitute;
  unsupported hosts are reported as unsupported, not silently downgraded.

## Non-authoritative aggregate

`GeneratedProductCheck` fails locally for an environment reason (no template
`feature.json` resolution + `Map.empty` env), independent of this change. It is a
**non-authoritative environment failure**, not a product regression; the
authoritative per-gate controls/surface checks all pass.
