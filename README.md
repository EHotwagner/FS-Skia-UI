# FS.Skia.UI

A **Spec Kit-first** F# desktop UI framework. You describe the app you want as
specifications and drive a governed, agent-run workflow — it produces an
[Elmish](https://elmish.github.io/elmish/) (Model-View-Update) application
rendered with [SkiaSharp](https://github.com/mono/SkiaSharp) on
[Vulkan](https://www.vulkan.org/), backed by evidence at every step.

You don't hand-write the window plumbing, the render loop, or the MVU wiring.
You author specifications, drive the [Spec Kit](https://github.com/github/spec-kit)
workflow with a coding agent, and review the evidence it produces. The framework
owns the host edge (window, input, Vulkan/Skia setup, frame rendering,
screenshots, diagnostics, shutdown); the workflow produces the model, messages,
`update`, and `view` that returns immutable `Scene` values.

---

## How you build with it

Spec Kit is the primary interface. Every project you generate carries the
`.specify/` install and project-local `speckit-*` skills, so you build features
through a governed loop driven by a coding agent:

```
specify  →  plan  →  tasks  →  implement  →  evidence
```

1. **`$speckit-specify`** — describe the feature you want in plain language; the
   workflow writes `spec.md`.
2. **`$speckit-plan`** — turn the spec into an implementation plan and design
   artifacts.
3. **`$speckit-tasks`** — break the plan into a dependency-ordered task list.
4. **`$speckit-implement`** — the agent implements the tasks, producing the F#
   MVU code and the readiness evidence.
5. **evidence** — gates keep the work honest (see
   [Evidence keeps it honest](#evidence-keeps-it-honest)).

Each feature lives in a numbered folder (`spec.md → plan.md → tasks.md →
research.md → data-model.md → contracts/ → readiness/`). Your job is to describe
intent, make decisions when the workflow asks, and review evidence — not to
hand-author the render loop or the Elmish plumbing.

The workflow is driven by coding agents, with synchronized skill peers for
[Claude Code](https://www.anthropic.com/claude-code) (`.claude/skills/`) and
[Codex](https://openai.com/codex/) (`.agents/skills/`). It is currently
developed and tested against **Claude Opus 4.8** and **Codex 5.5**.

---

## Get started

### 1. Generate a Spec Kit-enabled project

From a clone of this repository, pack the preview packages to a local feed and
install the project template:

```bash
dotnet tool restore
./fake.sh build -t PackLocal          # writes packages to ~/.local/share/nuget-local
dotnet new install .                  # installs the `fs-skia-ui` template
```

Add `~/.local/share/nuget-local` as a NuGet source (e.g. via `nuget.config` or
`dotnet nuget add source`), then generate a governed product:

```bash
dotnet new fs-skia-ui --name MyApp --profile governed --allow-scripts yes
cd MyApp
```

The generated repository already contains the Spec Kit install, the project-local
`speckit-*` skills, an initial Git commit, and a working FS.Skia.UI app to build
on. Pass `--skipGitInit true` when generating inside an existing repository.

### 2. Drive the workflow

Open the generated project with your coding agent and describe the first feature:

```text
$speckit-specify  a start screen with a title and a "New Game" button
$speckit-plan
$speckit-tasks
$speckit-implement
```

The agent produces the spec, plan, tasks, the F# implementation, and the
readiness evidence. Then build and run what it produced:

```bash
./fake.sh build -t Dev                # restore, build, test
dotnet run --project src/MyApp/MyApp.fsproj
```

### 3. Pick the right profile

| Profile | Use it for |
|---------|-----------|
| `governed` | The full Spec Kit governance framework (recommended starting point). |
| `app` | A full consumer application. |
| `headless-scene` | Scene composition without a live window (tests, evidence). |
| `sample-pack` | A demo/gallery-style starting point. |

---

## What the workflow produces

The implementation the workflow generates is an ordinary Elmish app: a model,
messages, an `update`, and a `view` that returns immutable `Scene` values. You
read and review code shaped like this — you don't write it from scratch:

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

The application owns the MVU four-tuple; the framework owns everything around it.
When something can't start (unsupported OS, no Vulkan surface, swapchain/context
failure), `Viewer.run` returns `Result<unit, RenderDiagnostic>` with a stage you
can report — it does not throw or silently fall back.

The workflow can compose any of these capability packages into what it builds:

| Package | Capability |
|---------|-----------|
| `FS.Skia.UI.Scene` | The scene vocabulary — shapes, paths, text, images, paint. |
| `FS.Skia.UI` | The Elmish Vulkan viewer (`Viewer.create`, `Viewer.run`). |
| `FS.Skia.UI.Controls` | Buttons, text, charts, `DataGrid`, graph views, theming, accessibility. |
| `FS.Skia.UI.Controls.Elmish` | Wires control + keyboard runtime effects into the Elmish program. |
| `FS.Skia.UI.Layout` | Flex/grid layout, graph layout, hit testing. |
| `FS.Skia.UI.KeyboardInput` | Key bindings, command resolution, modes, state display. |
| `FS.Skia.UI.Charts` | Pure chart and `DataGrid` builders that return `Scene` values. |
| `FS.Skia.UI.Elmish` | The adapter that bridges the Elmish program to the viewer. |
| `FS.Skia.UI.SkiaViewer` | Viewer host workflow contracts (window behavior, close reasons). |
| `FS.Skia.UI.Testing` | Helpers for validating the generated product. |

---

## Evidence keeps it honest

The distinctive part of the Spec Kit workflow is its evidence layer. Generated
work can't claim it's done without proof:

- **EvidenceGraph** validates the task dependency graph and counts synthetic
  (not-yet-proven) tasks.
- **EvidenceAudit** is the merge gate — it hard-blocks on unjustified synthetic
  tasks or block-severity findings, so a feature can't merge claiming evidence
  it doesn't have.

`validation.contract.yml` routes changed paths to required gates and expected
readiness artifacts. The project constitution
([.specify/memory/constitution.md](.specify/memory/constitution.md)) governs
public-contract impact, MVU boundaries, test evidence, and synthetic-evidence
disclosure. See [docs/speckit.md](docs/speckit.md) and
[docs/evidence.md](docs/evidence.md).

---

## Requirements

- .NET SDK with `net10.0` support
- A coding agent for the Spec Kit workflow (currently Claude Opus 4.8 or Codex 5.5)
- Windows or Linux desktop (macOS, mobile, browser, and headless production are
  out of scope)
- A Vulkan-capable GPU, driver, and presentation surface
- NuGet access for the SkiaSharp 4 preview and Silk.NET dependencies

> **Preview status.** This is a first-version, preview toolkit. There are no
> stable published packages yet — consume it from this repository using the
> local-feed flow above. Treat the SkiaSharp 4 preview dependency as
> preview-risk: Vulkan driver behavior, native assets, and package shape may
> change before SkiaSharp 4 is stable. The public API exposes no renderer
> selection and provides no OpenGL/CPU/software/fallback renderer.

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
