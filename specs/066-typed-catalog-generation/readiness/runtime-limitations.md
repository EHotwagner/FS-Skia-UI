# Runtime limitations

## Feature scope: build-time-only transform; no runtime surface

This feature changes only **how** the six `065`-typed control catalog rows are authored —
they become generated outputs of the build-front `CatalogGen.catalogFacts` single source
rather than hand-authored text. It introduces no runtime model, interpreter, effect,
subscription, layout, rendering, screenshot, Vulkan, or Skia behavior. The catalog
**describes** controls; it does not render them, so no render/screenshot/host evidence
applies. The generator, gate, and `RegenerateCatalog` effect live entirely in
`FS.Skia.UI.Build`; the shipped `FS.Skia.UI.Controls` package gains no dependency and no
behavior change (the six rows are byte-identical).

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux and renders
through **Vulkan** on a **SkiaSharp preview** native build.
Platforms remain **unsupported macOS/mobile/browser**, and there is **no software-renderer fallback**.
This feature changes none of that — it changes the authoring mechanism of six
framework-internal catalog rows, not how the runtime executes or which platforms it supports.

## Sandbox limitation: GeneratedProductCheck evidence-graph sub-step

`GeneratedProductCheck` scaffolds a product into
`artifacts/generated-products/066-typed-catalog-generation/app-source/` and runs its
`Verify`. In this sandbox that step aborts with *"Cannot resolve the feature to validate:
no `SPECKIT_FEATURE_DIR` override is set and `…/app-source/.specify/feature.json` has no
usable `feature_directory` entry."* The generated scaffold ships an empty
`.specify/feature.json`, so it cannot self-resolve a feature in a headless run.

This is a **pre-existing, sandbox-wide environmental condition**, not a regression from this
feature: the prior **merged** features `064-publish-nuget-distribution` and
`065-typed-controls-front-door` hit the **identical** failure
(`specs/065-typed-controls-front-door/readiness/generated-product-verify/app-source/verify.log`).
The typed-catalog generator and drift gate live only in `FS.Skia.UI.Build` (which the
template does not ship), and the catalog content the generated product reads is
byte-identical, so the generated product does not consume this feature — only its own
evidence-graph step, which needs `SPECKIT_FEATURE_DIR`/a populated generated
`feature.json`, fails. Classification: **environment-degraded**, not a product defect. The
authoritative merge gate remains `EvidenceAudit verdict=PASS`.

## No visual/host evidence is applicable

Because generation is a pure, deterministic build-time text transform, the honest audience is
the currency gate (`ControlsCatalogGenerationCheck`), the parity/drift/correspondence tests,
and `TargetMetadataDrift` — not a screenshot or a host smoke run. There is no
interactive-window, first-frame, or environment-session diagnostic to capture for this
feature.
