# Runtime Limitations & Evidence Obligations — 079-doc-preview-examples

Authoritative command: `./fake.sh build -t Route` (tier/gate selection).
Artifact path: this file. Failure class: `tier-or-evidence-misclassification`.
Next action: keep this aligned with plan §Constitution Check.

## Tier and surface

- **Tier 2 (internal change)** with a consumer-contract *generation* surface
  (`docs/**` consumed assets + governance build). `Route` escalates it to the
  `maintainer-verify` serialized six-target path because it touches governance
  paths and `docs/**`.
- **No public product `.fsi` / API / behavior change.** The preview machinery and
  the strengthened currency gate **read** the existing public control surface and
  `CatalogGen.catalogFacts`; they do not redefine it. The per-control sample
  source and render harness are **build/harness-internal** (Tier-2 internal).
- Affected layers: a new render harness project (`tests/ControlsPreview.Harness/`,
  compile-checked in the solution), the governance build pure core + edge
  (`build/Governance/CatalogDocsGen.fs(/.fsi)`, `Engine/Update.fs`), `docs/**`
  (regenerated previews + `categoryindex` renumber), and readiness evidence.

## MVU / effect boundary

- **N/A as a runtime concern.** No framework `Model`/`Msg`/`Effect`/`update`
  change. The generator and currency check keep the governance engine's pure-core /
  edge-interpreter shape: pure functions compute the rendered sample IR, currency
  findings, and the trivial-content verdict over in-memory values; file reads/writes
  and `FailWith` happen only at the `Engine/Update.fs` edge. Stateful sample
  controls (slider, list-box, data-grid, text-box) are initialized via their typed
  `init` with **fixed** sample models at the render edge only.

## Rendering vs consumption (the hard limitation)

- **Preview rendering requires a render-capable host** (Skia native libs on the
  loader path). It is performed on such a host (this feature's renders were
  produced on Linux x64 with the SkiaSharp `linux-x64` native asset) and the PNGs
  are **committed as source assets**.
- **The docs/site build and the currency gate stay GPU-free.** `dotnet fsdocs
  build --strict --eval` consumes the committed PNGs unchanged (FR-009); the
  `ControlsCatalogDocsCheck` gate validates committed bytes **structurally**
  (PNG signature + IHDR dimensions + byte-floor), never decoding pixels, so the
  governance build takes **no** SkiaSharp dependency.
- Consequently the render-harness tests (`ControlsPreview.Harness`) are **not** in
  `defaultTestProjects`: they need a render host and would fail GPU-free CI `Dev`.
  They are compile-checked by the solution `Build` and exercised on a
  render-capable host with evidence under this `readiness/` dir.

## Real evidence obligations

- **Preview evidence** — `controls-preview-evidence.md`: per-control honesty
  ledger (id, name, render-only mode, decodable, dimensions, bytes, content
  classification) + reconciled `rendered = N / unsupported = M`, `N + M ==
  |catalog|`.
- **Catalog currency** — `controls-catalog-docs.md`: `ControlsCatalogDocsCheck`
  PASS on the demonstrative tree; FAIL on blanked/trivial/missing/undecodable/
  orphan/stale.
- **Docs build** — `docs-build.md`: strict eval build with all previews present,
  links resolving, Examples → Controls → Guides nav order.

## Platform runtime boundary

The render-only preview path and the supported control surface run on the supported runtime
only:

- **.NET 10 desktop** host (Windows and Linux desktop).
- **Vulkan**-backed GPU path for the windowed host; the render-only preview path exercises
  the raster output without a persistent window.
- **SkiaSharp preview** is the pinned rendering dependency (linux-x64 native asset on this
  render-capable host).
- **unsupported macOS/mobile/browser**: these targets are out of runtime scope.
- **no software-renderer fallback**: there is no software rasterizer substitute; unsupported
  hosts/controls are reported as unsupported, not silently downgraded (e.g. `custom-control`
  is declared unsupported, never a fabricated image).

## Known non-authoritative environment failure

- `GeneratedProductCheck` fails locally by design (generated Verify cannot resolve
  a feature; no template `feature.json` + empty env). Recorded as a
  non-authoritative **environment-failure** in `aggregate-hang-diagnostics.md`, not
  a product defect (see memory `generated-product-check-env-failure`).
