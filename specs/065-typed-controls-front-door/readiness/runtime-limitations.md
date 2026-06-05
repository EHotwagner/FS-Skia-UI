# Runtime limitations

## Feature scope: additive authoring surface only

This feature adds a compiler-checked **authoring** surface
(`Widget<'msg>` + the `FS.Skia.UI.Controls.Typed` modules) that lowers, by
construction, to the **same** `Control<'msg>` IR the legacy string-keyed builders
already produce. It introduces no new runtime model, interpreter, effect,
subscription, layout, rendering, screenshot, Vulkan, or Skia behavior — typed
views reuse the existing render path byte-for-byte (proven by the
render-parity capture in [controls-rendering.md](./controls-rendering.md)).

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux and
renders through **Vulkan** on a **SkiaSharp preview** native build.
Platforms remain **unsupported macOS/mobile/browser**, and there is **no software-renderer fallback**.
This feature changes none of that — it adds a typed front door inside the existing
`FS.Skia.UI.Controls` package and does not alter how the runtime executes or which
platforms it supports.

## Sandbox limitation: GeneratedProductCheck evidence-graph sub-step

`GeneratedProductCheck` scaffolds a product into
`artifacts/generated-products/065-typed-controls-front-door/app-source/` and runs
its `Verify`. In this sandbox the generated product's `Dev`, `GeneratedGuidanceCheck`,
and `TemplateDrift` all **complete**, and `Product.dll` plus `Product.Tests`
**build and test cleanly** — but the generated product's own evidence-graph
sub-step aborts with: *"Cannot resolve the feature to validate: no
`SPECKIT_FEATURE_DIR` override is set and `…/app-source/.specify/feature.json` has
no usable `feature_directory` entry."* The generated scaffold ships an empty
`.specify/feature.json`, so it cannot self-resolve a feature in a headless run.

This is a **pre-existing, sandbox-wide environmental condition**, not a regression
from the additive typed-controls surface: the prior **merged** feature
`064-publish-nuget-distribution` hit the **identical** failure
(`specs/064-publish-nuget-distribution/readiness/generated-product-verify/app-source/verify.log`).
The typed front door is additive and the generated product does not consume it, so
the generated `Product` compiles and its tests pass — only the generated
evidence-graph step, which needs `SPECKIT_FEATURE_DIR`/a populated generated
`feature.json`, fails. Classification: **environment-degraded**, not a product
defect. The authoritative merge gate remains `EvidenceAudit verdict=PASS`.

## Stateful controls reuse existing pure models

The two stateful typed controls (`TextBox`, `DataGrid`) delegate `init`/`update`
to the existing pure `TextInput`/`DataGrid` models; the edge interpreter
(`TextInput.interpretEffect`) is unchanged and reused. No I/O is introduced into
`update`, and no new effect interpreter or host capability is added.
