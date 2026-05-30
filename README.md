# FS.Skia.UI

Build native desktop apps in **F#** with an [Elmish](https://elmish.github.io/elmish/)
(Model-View-Update) model, rendered with [SkiaSharp](https://github.com/mono/SkiaSharp)
on [Vulkan](https://www.vulkan.org/) — and develop them through a spec-driven
workflow that ships in the box.

You write the model, messages, `update`, and `view`. Your `view` returns
immutable `Scene` values; the framework owns the host edge (window, input,
Vulkan/Skia setup, frame rendering, screenshots, diagnostics, shutdown). You
never manage the window or the render loop.

```fsharp
open Elmish
open FS.Skia.UI

type Model = { Title: string }
type Msg = NoOp

let init () = { Title = "Hello FS.Skia.UI" }, Cmd.none
let update msg model = match msg with NoOp -> model, Cmd.none

let view model =
    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 18uy 24uy 32uy 255uy)
        Scene.text (48.0, 88.0) model.Title Colors.white
    ]

let program =
    Viewer.create (Viewer.defaultConfiguration "Hello" { Width = 640; Height = 480 })
                  init update view

[<EntryPoint>]
let main _ =
    match Viewer.run program with
    | Ok () -> 0
    | Error d -> eprintfn "%s" d.Message; 1
```

---

## What you get

- **A pure view model** — compose immutable `Scene` values; the host renders
  them. No retained widget tree to keep in sync.
- **Ready-made controls** — `TextBlock`, `Button`, `TextBox`, charts
  (line/bar/pie/scatter), a `DataGrid`, and graph views, with theming,
  accessibility roles, and hit testing.
- **Layout** — Yoga-backed flex/grid layout, graph layout, and hit testing.
- **Keyboard input** — bindings, command resolution, modes, and a live
  state-display contract.
- **Diagnostics & screenshots** — structured render diagnostics and PNG/JPEG
  capture from the last good frame.
- **A development process in the box** — every generated app inherits the
  [Spec Kit](https://github.com/github/spec-kit) specify → plan → tasks →
  implement → evidence workflow (see below).

---

## Requirements

- .NET SDK with `net10.0` support
- Windows or Linux desktop (macOS, mobile, browser, and headless production are
  out of scope)
- A Vulkan-capable GPU, driver, and presentation surface
- NuGet access for the SkiaSharp 4 preview and Silk.NET dependencies

> **Preview status.** This is a first-version, preview toolkit. There are no
> stable published packages yet — consume it from this repository using the
> local-feed flow below. Treat the SkiaSharp 4 preview dependency as
> preview-risk: Vulkan driver behavior, native assets, and package shape may
> change before SkiaSharp 4 is stable. The public API exposes no renderer
> selection and provides no OpenGL/CPU/software/fallback renderer.

---

## Get started

### 1. Scaffold an app from the template

From a clone of this repository, pack the preview packages to a local feed and
install the project template:

```bash
dotnet tool restore
./fake.sh build -t PackLocal          # writes packages to ~/.local/share/nuget-local
dotnet new install .                  # installs the `fs-skia-ui` template
```

Add `~/.local/share/nuget-local` as a NuGet source (e.g. via `nuget.config` or
`dotnet nuget add source`), then generate and run a product:

```bash
dotnet new fs-skia-ui --name MyApp --profile app --allow-scripts yes
cd MyApp
./fake.sh build -t Dev                # restore, build, test
dotnet run --project src/MyApp/MyApp.fsproj
```

Pass `--skipGitInit true` when generating inside an existing repository. The
`.NET` CLI prompts before running template scripts unless you pass
`--allow-scripts yes`.

### 2. Pick a profile

Generated products are scaffolded through capability profiles:

| Profile | Use it for |
|---------|-----------|
| `app` | A full consumer application. |
| `headless-scene` | Scene composition without a live window (tests, evidence). |
| `governed` | The full Spec Kit governance framework. |
| `sample-pack` | A demo/gallery-style starting point. |

```bash
dotnet new fs-skia-ui --name MyApp --profile governed --allow-scripts yes
```

---

## Writing your app

Your application owns the MVU four-tuple; higher-level packages only **produce
data, diagnostics, hit-test results, or `Scene` output** — they never open a
window or take over the loop. Compose with the packages you need:

| Package | What it gives your app |
|---------|------------------------|
| `FS.Skia.UI.Scene` | The scene vocabulary — shapes, paths, text, images, paint. |
| `FS.Skia.UI` | The Elmish Vulkan viewer (`Viewer.create`, `Viewer.run`). |
| `FS.Skia.UI.Controls` | Buttons, text, charts, `DataGrid`, graph views, theming. |
| `FS.Skia.UI.Controls.Elmish` | Wires control + keyboard runtime effects into your Elmish program. |
| `FS.Skia.UI.Layout` | Flex/grid layout, graph layout, hit testing. |
| `FS.Skia.UI.KeyboardInput` | Key bindings, command resolution, modes, state display. |
| `FS.Skia.UI.Charts` | Pure chart and `DataGrid` builders that return `Scene` values. |
| `FS.Skia.UI.Elmish` | The adapter that bridges your Elmish program to the viewer. |
| `FS.Skia.UI.SkiaViewer` | Viewer host workflow contracts (window behavior, close reasons). |
| `FS.Skia.UI.Testing` | Helpers for validating your generated product. |

When something can't start (unsupported OS, no Vulkan surface, swapchain/context
failure), `Viewer.run` returns `Result<unit, RenderDiagnostic>` with a stage you
can report — it does not throw or silently fall back.

---

## Building with Spec Kit

Every generated app carries the `.specify/` install and project-local
`speckit-*` skills, so you develop features through the same governed loop the
framework uses on itself:

```
specify  →  plan  →  tasks  →  implement  →  evidence
```

Each feature lives in a numbered folder (`spec.md → plan.md → tasks.md →
research.md → data-model.md → contracts/ → readiness/`). The skills
`$speckit-specify`, `$speckit-plan`, `$speckit-tasks`, and `$speckit-implement`
drive each stage, and an **evidence** layer keeps work honest:

- **EvidenceGraph** validates the task dependency graph and counts synthetic
  (not-yet-proven) tasks.
- **EvidenceAudit** is the merge gate — it hard-blocks on unjustified synthetic
  tasks or block-severity findings, so a feature can't merge claiming evidence
  it doesn't have.

The workflow is driven by coding agents, with synchronized skill peers for
[Claude Code](https://www.anthropic.com/claude-code) (`.claude/skills/`) and
[Codex](https://openai.com/codex/) (`.agents/skills/`). It is currently
developed and tested against **Claude Opus 4.8** and **Codex 5.5**. See
[docs/speckit.md](docs/speckit.md).

---

## Learn from the samples

Each sample runs as a window, or as a non-visual contract smoke (`--contract-smoke`):

```bash
dotnet run --project samples/BasicViewer/BasicViewer.fsproj
dotnet run --project samples/DemoReel/DemoReel.fsproj -- --contract-smoke
```

| Sample | Shows |
|--------|-------|
| `BasicViewer` | Core scene composition, diagnostics, screenshots. |
| `ChartsGallery` | Line, bar, scatter, area, pie/donut, histogram charts. |
| `DataGridGallery` | Sorting, viewport state, fixed headers, hit testing. |
| `DemoReel` | Animated showcase: geometry, shaders, layout, charts, graphs, effects. |
| `EffectsGallery` | Gradients, path effects, blend modes, clipping, perspective. |
| `InteractiveViewer` | Keyboard/pointer state, timer updates, JPEG screenshots. |
| `KeyboardInputGallery` | Keyboard layouts, command resolution, state display. |
| `LayoutGraphGallery` | Automatic layout, graph rendering, validation, hit testing. |
| `ParityGallery` | Skia feature parity: shapes, paths, clips, regions, images. |
| `ScreenshotGallery` | Screenshot/render effects, recoverable diagnostics. |

---

## How the packages fit together

`Scene` is the shared vocabulary everything returns; the host edge lives in
`Lib`/`SkiaViewer`:

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

---

## Built With

### Runtime

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

### Build, test, and process tooling

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

## Developing FS.Skia.UI itself

Maintainers work in this repository through the same Spec Kit process, validated
by FAKE targets run **one at a time** (FAKE-backed commands share `.fake` state
and are not safe to run concurrently):

1. `./fake.sh build -t Dev` — restore, build, test
2. `./fake.sh build -t GeneratedGuidanceCheck` — spec/plan template governance
3. `./fake.sh build -t TemplateCheck` — template pack/install/instantiate/smoke
4. `./fake.sh build -t GeneratedProductCheck` — generated-product matrix
5. `./fake.sh build -t EvidenceGraph` — validate the task DAG
6. `./fake.sh build -t EvidenceAudit` — merge gate

`Verify` and `Ci` are the broad aggregate gates. Public visibility lives in
`.fsi` signature files (the contract chain is spec → `.fsi` → failing tests →
implementation → surface baseline), and package boundaries are enforced (e.g.
`Controls` may not reference Silk.NET/SkiaSharp/Elmish directly). See
[docs/build.md](docs/build.md), [docs/testing.md](docs/testing.md),
[docs/evidence.md](docs/evidence.md), and
[.specify/memory/constitution.md](.specify/memory/constitution.md).

---

## License

Licensed under the [MIT License](LICENSE).
