# Runtime limitations & failure diagnostics (feature 115)

## Documented evidence path

Feature 115 is a **dependency-version + governance-asset** change, not a source-behavior change. It is
proven by the standing Expecto + FsCheck suites and the FAKE gate set staying green on the bumped pins,
with **zero** surface-baseline / golden / generated-product diff. A live Vulkan window is **not required**
(there is no scene, window, or screenshot surface in this feature). The asserted surfaces:

- `Directory.Packages.props` safe bumps (FSharp.Core 10.1.301, Microsoft.Extensions.FileSystemGlobbing
  10.0.9) — compile + test green under `Dev` with no source change;
- `.specify/init-options.json` `speckit_version` recorded-version edit — `GeneratedGuidanceCheck` /
  `TemplateDrift` green; the `.claude` tree is generated from the canonical `.agents` tree, not vendored
  from upstream, so the bump pulls no upstream assets;
- each held major bump (US2) — the full routed gate set is the drop-in proof; not-drop-in → full revert;
- a freshly generated `dotnet new fs-skia-ui` project restores + builds against the pins
  (`TemplateCheck` / `GeneratedProductCheck`).

## Failure diagnostics

- A missing required evidence artifact fails `Route --enforce` (it names the artifact + requiring tier).
- A safe bump that produces ANY source / golden / surface diff is reclassified as not-actually-safe and
  reverted (FR-002/FR-003).
- A held bump that fails any routed gate, or would require a source change to compile, is fully reverted
  via `git checkout -- Directory.Packages.props` and recorded `deferred(<gate + symptom>)` (FR-004/FR-005).
- A FAKE-backed failure that looks race-like is rerun sequentially before product debugging
  (see aggregate-hang-diagnostics.md).

## Platform / runtime support boundary

The framework runs on a **.NET 10 desktop** host rendering through **Vulkan** via the **SkiaSharp preview**
native binding, on Windows and Linux desktop (`net10.0`). **unsupported macOS/mobile/browser** — there is
**no software-renderer fallback**; those targets are out of scope. Feature 115's evidence is build/test/
generated-project gate output (deterministic, GPU-free), so it does not depend on the live Vulkan surface;
the SkiaSharp / Silk.NET / Yoga.Net pins are unchanged, so no platform/visual-output behavior changes.
