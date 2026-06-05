# Runtime limitations

## Feature scope: pure internal diff; no runtime surface

This feature adds a **pure, internal** keyed reconciler (`module internal Reconcile`)
over the immutable `Control<'msg>` IR. It introduces no runtime model, interpreter,
effect, subscription, layout, rendering, screenshot, Vulkan, or Skia behavior, and it is
**not wired into the live render path** (FR-012). The reconciler `describes` the
difference between two control trees; it does not render them, so no render/screenshot/
host evidence applies. The honest audience is the in-assembly Expecto/FsCheck tests
(round-trip + determinism), not a screenshot or host smoke run. The shipped
`FS.Skia.UI.Controls` package gains no product dependency and no observable behavior
change.

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux and renders
through **Vulkan** on a **SkiaSharp preview** native build.
Platforms remain **unsupported macOS/mobile/browser**, and there is **no software-renderer fallback**.
This feature changes none of that — it adds assembly-internal framework code unreachable
from package consumers, not a change to how the runtime executes or which platforms it
supports.

## Sandbox limitation: GeneratedProductCheck evidence-graph sub-step

`GeneratedProductCheck` scaffolds a product into
`artifacts/generated-products/067-keyed-reconciliation/app-source/` and runs its `Verify`.
The product's `Dev`, `GeneratedGuidanceCheck`, and `TemplateDrift` steps all complete; the
run then aborts in `runGeneratedEvidenceGraph` with *"Cannot resolve the feature to
validate: no `SPECKIT_FEATURE_DIR` override is set and
`…/app-source/.specify/feature.json` has no usable `feature_directory` entry."* The
generated scaffold ships an empty `.specify/feature.json`, so it cannot self-resolve a
feature in a headless run.

This is a **pre-existing, sandbox-wide environmental condition**, not a regression from
this feature: the prior **merged** features `064-publish-nuget-distribution`,
`065-typed-controls-front-door`, and `066-typed-catalog-generation` hit the **identical**
failure. The reconciler lives only in `src/Controls/**` as assembly-internal code that the
generated product does not (and cannot) consume — only the product's own evidence-graph
step, which needs `SPECKIT_FEATURE_DIR` / a populated generated `feature.json`, fails.
Classification: **environment-degraded**, not a product defect. The authoritative merge
gate remains `EvidenceAudit verdict=PASS`.

## No visual/host evidence is applicable

Because the deliverable is a pure, deterministic, internal data-structure-and-algorithm
with no render-path wiring, there is no interactive-window, first-frame, or
environment-session diagnostic to capture for this feature. The authoritative evidence is
the round-trip / determinism FsCheck properties (≥1000 cases each), the US1–US4 + edge
unit tests, and the escalated Route-printed gate set.
