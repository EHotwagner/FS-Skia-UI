# FS.Skia.UI

FS.Skia.UI is an experimental F# desktop UI toolkit built on
[SkiaSharp](https://github.com/mono/SkiaSharp) and
[Vulkan](https://www.vulkan.org/), using an [Elmish](https://elmish.github.io/elmish/)
(Model-View-Update) application model. It is **two things at once**: a
composable UI library, and a spec-driven, evidence-governed framework for
building it.

Application code owns the model, messages, `update` function, and `view`
function. The `view` function returns immutable `Scene` values. `Viewer.run`
owns the host edge: window creation, input events, Vulkan/Skia setup, frame
rendering, screenshots, diagnostics, and shutdown. Higher-level packages only
produce data, diagnostics, hit-test results, or `Scene` output — they never
create windows or take over the application loop.

It is **not** a general renderer abstraction, a browser/mobile UI framework, or
a retained widget toolkit that owns application state. The runtime host is
deliberately narrow: desktop, SkiaSharp, Silk.NET, and Vulkan.

---

## Architecture

The packages form a layered stack. `Scene` is the shared vocabulary that every
layer ultimately returns; the host edge lives in `Lib`/`SkiaViewer`.

```
Scene  ──────────────  pure scene primitives (FSharp.Core only)
 ├─ Layout            (+ Yoga.Net)    flex/grid layout, graphs, hit testing
 ├─ KeyboardInput     (+ YamlDotNet)  key bindings, reducer, effects
 └─ Lib (FS.Skia.UI)  (+ Silk.NET, SkiaSharp, Elmish)   Vulkan desktop host
     └─ SkiaViewer                    viewer host workflow contracts
         └─ Elmish                    Elmish ↔ viewer adapter
             ├─ Controls              TextBlock/Button/charts/DataGrid/graphs
             │   └─ Controls.Elmish   command + subscription adapters
             └─ Testing               generated-product validation helpers
```

| Package | Purpose |
|---------|---------|
| `FS.Skia.UI.Scene` | Core scene primitives and paint/path/text/image declarations. Zero dependencies beyond FSharp.Core. |
| `FS.Skia.UI.Layout` | Yoga-backed layout, graph validation, graph layout, rendering, and hit testing. |
| `FS.Skia.UI.KeyboardInput` | Keyboard input runtime, reducer, effects, diagnostics, and state-display contracts. |
| `FS.Skia.UI` | Elmish-only Vulkan desktop viewer primitives (the live SkiaSharp/Silk.NET host). |
| `FS.Skia.UI.SkiaViewer` | Viewer runtime contracts, structured diagnostics, screenshots, and desktop host boundaries. |
| `FS.Skia.UI.Elmish` | Elmish/MVU adapter contracts for product applications. |
| `FS.Skia.UI.Controls` | Declarative controls, rich rendering, chart/graph controls, and DataGrid. |
| `FS.Skia.UI.Controls.Elmish` | Elmish command, subscription, and program adapters for Controls and KeyboardInput. |
| `FS.Skia.UI.Charts` | Pure chart and DataGrid builders that return core `Scene` values. |
| `FS.Skia.UI.Testing` | Generated-product and package validation helpers. |

Design invariants that gates enforce: public visibility lives in `.fsi`
signature files (not `.fs`); the contract chain is **spec → `.fsi` → failing
tests → implementation → surface baseline**; package boundaries are strict
(e.g. `Controls` may not reference Silk.NET/SkiaSharp/Elmish directly); stateful
workflows go through the MVU/effect boundary.

---

## Minimal App Shape

```fsharp
open Elmish
open FS.Skia.UI

type Model = { Title: string }

type Msg =
    | NoOp

let init () =
    { Title = "Hello FS.Skia.UI" }, Cmd.none

let update msg model =
    match msg with
    | NoOp -> model, Cmd.none

let view model =
    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 18uy 24uy 32uy 255uy)
        Scene.text (48.0, 88.0) model.Title Colors.white
    ]

let config =
    Viewer.defaultConfiguration "Hello" { Width = 640; Height = 480 }

let program =
    Viewer.create config init update view

[<EntryPoint>]
let main _ =
    match Viewer.run program with
    | Ok() -> 0
    | Error diagnostic ->
        eprintfn "%s" diagnostic.Message
        1
```

---

## Runtime Boundary

The live desktop host currently requires:

- .NET SDK with `net10.0` support.
- Windows or Linux desktop.
- Vulkan-capable GPU, driver, and presentation surface.
- NuGet access for SkiaSharp 4 preview and Silk.NET dependencies.

macOS, mobile, browser, and headless production targets are out of scope. The
public API does not expose renderer selection and provides no OpenGL, CPU,
software, or fallback renderer. Startup validates the supported OS, presentation
surface, Vulkan instance, device, swapchain, and Skia Vulkan context before
rendering; failures return `Result<unit, RenderDiagnostic>` from `Viewer.run`.

This is a first-version, preview package. Treat the SkiaSharp 4 preview
dependency as preview-risk: Vulkan driver behavior, native assets, and package
shape may change before SkiaSharp 4 reaches stable release.

---

## Quickstart

Restore local tools and run the default validation:

```bash
dotnet tool restore
./fake.sh build -t Dev
```

Pack the preview packages locally, install the template, and generate a product:

```bash
./fake.sh build -t PackLocal
dotnet new install .
dotnet new fs-skia-ui --name MyProduct --profile app --allow-scripts yes
cd MyProduct
./fake.sh build -t Dev
```

`PackLocal` writes packages to `~/.local/share/nuget-local`; add that folder as
a NuGet source (or use an equivalent feed) before restoring the generated
product. Pass `--skipGitInit true` when generating inside an existing repository.

> **FAKE concurrency:** FAKE-backed commands (`./fake.sh`, `fake.cmd`,
> `dotnet fake`) share repository `.fake` state and are **not safe to run
> concurrently**. When more than one FAKE target is needed, run them
> sequentially in a deterministic order. Non-FAKE reads and checks may still run
> in parallel.

---

## Build, Test, and Governance

Run FAKE-backed validation targets one at a time:

1. `./fake.sh build -t Dev` — restore, build, test
2. `./fake.sh build -t GeneratedGuidanceCheck` — spec/plan template governance prompts
3. `./fake.sh build -t TemplateCheck` — template pack/install/instantiate/smoke
4. `./fake.sh build -t GeneratedProductCheck` — generated-product matrix compiles and passes
5. `./fake.sh build -t EvidenceGraph` — validate the task DAG, count synthetic tasks
6. `./fake.sh build -t EvidenceAudit` — merge gate: synthetic propagation + diff-scan

`Verify` and `Ci` are the broad aggregate gates. Use `fake.cmd build -t Dev`
from Windows command prompts. See [docs/build.md](docs/build.md),
[docs/testing.md](docs/testing.md), and [docs/evidence.md](docs/evidence.md).

### Spec Kit and evidence governance

Feature work runs through [Spec Kit](https://github.com/github/spec-kit):
numbered folders under `specs/` carry `spec.md → plan.md → tasks.md →
research.md → data-model.md → contracts/ → readiness/`. The distinctive layer is
**evidence/readiness**:

- **EvidenceGraph** validates the task dependency graph and counts synthetic
  tasks — it reports but does not block.
- **EvidenceAudit** is the merge gate: it runs synthetic propagation plus a
  diff-scan and **hard-blocks** on unjustified synthetic tasks (`[S]`/`[S*]`) or
  block-severity findings.

`validation.contract.yml` routes changed paths to required gates, expected
readiness artifacts, and a failure owner. The project constitution in
[.specify/memory/constitution.md](.specify/memory/constitution.md) governs
public-contract impact, `.fsi` visibility, MVU boundaries, test evidence, and
synthetic-evidence disclosure. Project-local `speckit-*` skills live in
`.claude/skills/`, mirrored by Codex peers in `.agents/skills/`.

---

## Project Template

`dotnet new fs-skia-ui` generates governed products through explicit capability
profiles: `app`, `headless-scene`, `governed`, and `sample-pack`. Profiles are
composed from `template/base/` plus `template/fragments/`. Generated products
inherit the Spec Kit prompts and evidence discipline.

```bash
dotnet new install .
dotnet new fs-skia-ui --name MyProduct          --profile app            --allow-scripts yes
dotnet new fs-skia-ui --name MyProduct.Scene    --profile headless-scene --allow-scripts yes
dotnet new fs-skia-ui --name MyProduct.Governed --profile governed       --allow-scripts yes
dotnet new fs-skia-ui --name MyProduct.Samples  --profile sample-pack    --allow-scripts yes
```

Maintainers validate source and packaged template paths with `TemplateCheck`
and `GeneratedProductCheck`. See
[docs/template-profile.md](docs/template-profile.md),
[docs/dependencies.md](docs/dependencies.md), and [docs/speckit.md](docs/speckit.md).

---

## Samples

The repository includes runnable samples. Each exposes a public-API contract
smoke path (`--contract-smoke`) that does not open a live window.

| Sample | Focus |
|--------|-------|
| `BasicViewer` | Core scene composition, diagnostics, and screenshot requests. |
| `ChartsGallery` | Line, bar, scatter, area, pie/donut, and histogram charts. |
| `DataGridGallery` | Data grid sorting, viewport state, fixed headers, hit testing. |
| `DemoReel` | Animated showcase: geometry, shaders, layout, charts, graphs, effects. |
| `EffectsGallery` | Gradients, path effects, blend modes, clipping, perspective, color spaces. |
| `InteractiveViewer` | Keyboard/pointer state, timer updates, JPEG screenshot requests. |
| `KeyboardInputGallery` | Keyboard layouts, command resolution, keyboard state display. |
| `LayoutGraphGallery` | Automatic layout, graph rendering, validation, hit testing. |
| `ParityGallery` | Skia feature-parity coverage for shapes, paths, clips, regions, images. |
| `ScreenshotGallery` | Screenshot/render effects, recoverable diagnostics, shutdown effects. |

```bash
dotnet run --project samples/BasicViewer/BasicViewer.fsproj
dotnet run --project samples/BasicViewer/BasicViewer.fsproj -- --contract-smoke
```

---

## Built With

### Runtime dependencies

| Library | Version | Role |
|---------|---------|------|
| [F# / FSharp.Core](https://github.com/dotnet/fsharp) | 10.1.300 | Language and core library. |
| [.NET](https://dotnet.microsoft.com/) (`net10.0`) | — | Target framework. |
| [SkiaSharp](https://github.com/mono/SkiaSharp) | 4.147.0-preview.3.1 | 2D graphics / rendering. |
| [SkiaSharp.NativeAssets.Linux / .Win32](https://github.com/mono/SkiaSharp) | 4.147.0-preview.3.1 | Native Skia binaries. |
| [Silk.NET](https://github.com/dotnet/Silk.NET) (Input, Vulkan, Vulkan.Extensions.KHR, Windowing, Windowing.Extensions) | 2.23.0 | Windowing, input, and [Vulkan](https://www.vulkan.org/) bindings. |
| [Fable.Elmish](https://github.com/elmish/elmish) | 4.2.0 | Model-View-Update application model. |
| [Yoga.Net](https://www.nuget.org/packages/Yoga.Net) | 3.2.3 | .NET binding for [Yoga](https://www.yogalayout.dev/) flexbox layout. |
| [YamlDotNet](https://github.com/aaubry/YamlDotNet) | 17.1.0 | YAML parsing for input/config contracts. |

### Build, test, and governance tooling

| Tool | Version | Role |
|------|---------|------|
| [FAKE](https://fake.build/) | — | F# Make build automation (`build.fsx`). |
| [Spec Kit](https://github.com/github/spec-kit) | — | Spec-driven development and evidence governance. |
| [Expecto](https://github.com/haf/expecto) | 10.2.2 | F# test framework. |
| [YoloDev.Expecto.TestSdk](https://github.com/YoloDev/YoloDev.Expecto.TestSdk) | 0.15.3 | Expecto adapter for `dotnet test`. |
| [Microsoft.NET.Test.Sdk](https://github.com/microsoft/vstest) | 17.11.1 | Test host/runner. |

Exact pins are centralized in
[Directory.Packages.props](Directory.Packages.props).

---

## License

Licensed under the [MIT License](LICENSE).
