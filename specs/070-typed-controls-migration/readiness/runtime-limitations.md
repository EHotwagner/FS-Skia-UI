# Runtime limitations

## Feature scope: additive authoring surface only

This feature adds a compiler-checked **authoring** surface (the 41 remaining
catalog controls under `FS.Skia.UI.Controls.Typed`) that lowers, by construction,
to the **same** `Control<'msg>` IR the legacy string-keyed builders already
produce. Each typed `view` calls the exact legacy `*.create`/`Attr` builder (or
`Control.standard (Custom <id>)` where no dedicated builder exists) and lifts the
result through `Widget.ofControl`, so the lowered IR is structurally equal to the
legacy authoring call — proven per control by the parity matrix in
[typed-lowering-parity.md](./typed-lowering-parity.md). It introduces no new
runtime model, interpreter, effect, subscription, layout, rendering, screenshot,
Vulkan, or Skia behavior — typed views reuse the existing render path byte-for-byte.

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux and
renders through **Vulkan** on a **SkiaSharp preview** native build. Platforms
remain **unsupported macOS/mobile/browser**, with **no software-renderer fallback**.
This feature changes none of that — it adds a typed front door inside
the existing `FS.Skia.UI.Controls` package and does not alter how the runtime
executes or which platforms it supports.

## Sandbox limitation: GeneratedProductCheck evidence-graph sub-step

`GeneratedProductCheck` scaffolds a product into
`artifacts/generated-products/070-typed-controls-migration/app-source/` and runs its `Verify`.
In this sandbox the generated product's `Dev` **completes**, and `Product.dll` plus
`Product.Tests` **build and test cleanly** (28/28 passed) — but the generated product's own
evidence-graph sub-step aborts with: *"Cannot resolve the feature to validate: no
`SPECKIT_FEATURE_DIR` override is set and `…/app-source/.specify/feature.json` has no usable
`feature_directory` entry."* The generated scaffold ships an empty `.specify/feature.json`, so it
cannot self-resolve a feature in a headless run.

This is a **pre-existing, sandbox-wide environmental condition**, not a regression from the
additive typed-controls surface: the prior **merged** features `064-publish-nuget-distribution`
and `065-typed-controls-front-door` hit the **identical** failure. The typed front door is
additive and the generated product does not consume it, so the generated `Product` compiles and
its tests pass — only the generated evidence-graph step, which needs `SPECKIT_FEATURE_DIR`/a
populated generated `feature.json`, fails. Classification: **environment-degraded**, not a product
defect. The authoritative merge gate remains `EvidenceAudit verdict=PASS` (achieved, zero
blockers, zero synthetic).

## Stateful controls reuse existing pure models

The stateful typed controls delegate `init`/`update` to the existing pure models
with no fork: `text-area` → `TextInput` (multi-line); the five selection
collections (`list-view`, `list-box`, `multi-select-list`, `combo-box`,
`tree-view`) → `Collections`. The edge interpreters are unchanged and reused. No
I/O is introduced into `update`, and no new effect interpreter or host capability
is added. Charts/graph remain pure `Props -> Widget` (no chart runtime model
exists to fork). `custom-control` is the existing `Widget.ofControl` bridge — no
fabricated schema, no new runtime path.
