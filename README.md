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

## Why it holds up

The point of FS.Skia.UI is that an **agent can build your app and you can trust
the result**, because the process is engineered for robustness rather than speed
alone:

- **A spec-driven workflow.** Every feature starts as a specification, becomes a
  plan, then a dependency-ordered task list, and only then an implementation.
  Intent is written down and reviewable before any code exists.
- **Contract-first: task → `.fsi` → tests → implementation.** Public surface
  changes are sketched as an `.fsi` signature and exercised in F# Interactive
  *before* the `.fs` body exists — "FSI is the honest audience." Semantic tests
  then exercise the API through that same surface, and surface-area baselines are
  validated automatically, so the public contract can't drift silently.
- **Elmish that is actually testable.** Stateful and I/O workflows must go
  through an MVU boundary with a pure `update`. Tests are required on *both
  sides*: pure `Model + Msg → Model + effects` transitions, interpreter tests
  against real I/O, and FSI transcripts. Logic is verified without a window.
- **Per-task skill assignment at generation time.** When tasks are generated,
  each one is tagged with the local skills required to implement it (`skillist`
  metadata). The agent loads the declared skills before touching code for that
  task, so the right guidance is in context for the work at hand.
- **Evidence that can't be faked.** Work cannot claim "done" without proof.
  `EvidenceGraph` tracks unproven (synthetic) tasks; `EvidenceAudit` is a merge
  gate that **hard-blocks** on unjustified synthetic tasks or block-severity
  findings. An agent can't hand-wave a feature past the gate.
- **Extensive, markdown-driven governance via a compiled F# build.** A
  dedicated [FAKE](https://fake.build/) build front-end (`build/Build.fsproj`,
  run via `./fake.sh`) drives the test suites, package-surface checks,
  template/generated-product validation, dependency ownership, and the evidence
  audits — emitting human-readable Markdown readiness reports. All rules live in
  one compiled library, `FS.Skia.UI.Build`, so a mistyped gate is a compile
  error; generated artifacts (the routing contract, the `.claude` skill mirror)
  are generated from a single source, not hand-synced.
- **A two-tier process with one entry point.** Run **`./fake.sh build -t Route`**
  first: it reads your change and prints the authoritative *tier* and the
  *minimal gate list* to run. Routine framework work routes to a light
  `inner-loop` tier (`Dev`); consumer-contract changes escalate automatically.
  The full serialized gate order is the escalated path, not the default — a new
  contributor runs `Route` and proceeds without reading the whole governance
  corpus. (See `docs/adr/0006-foundations-programme-closeout.md` and the
  before/after measurement in `docs/reports/_baselines/`.)

---

## How you build with it

Spec Kit is the primary interface. Every project you generate carries the
`.specify/` install and project-local `speckit-*` skills, so you build features
through a governed loop driven by a coding agent:

```
specify  →  plan  →  tasks  →  implement  →  evidence
```

1. **`$speckit-specify`** — describe the feature in plain language; the workflow
   writes `spec.md`.
2. **`$speckit-plan`** — turn the spec into an implementation plan and design
   artifacts.
3. **`$speckit-tasks`** — break the plan into a dependency-ordered task list,
   each task tagged with the skills needed to implement it.
4. **`$speckit-implement`** — the agent works each task contract-first
   (`.fsi` → tests → implementation), producing the F# MVU code and the
   readiness evidence.
5. **evidence** — `EvidenceGraph` / `EvidenceAudit` gate the result.

Each feature lives in a numbered folder (`spec.md → plan.md → tasks.md →
research.md → data-model.md → contracts/ → readiness/`). Your job is to describe
intent, decide when the workflow asks, and review evidence.

The workflow is driven by coding agents, with synchronized skill peers for
[Claude Code](https://www.anthropic.com/claude-code) (`.claude/skills/`) and
[Codex](https://openai.com/codex/) (`.agents/skills/`). It is currently
developed and tested against **Claude Opus 4.8** and **Codex 5.5**.

---

## Get started

### 1. Generate a Spec Kit-enabled project

Install the project template from **public nuget.org** — no repository clone, no
local feed — then generate a governed product:

```bash
dotnet new install FS.Skia.UI.Template      # from nuget.org
dotnet new fs-skia-ui --name MyApp --profile governed --allow-scripts yes
cd MyApp
dotnet restore                              # resolves FS.Skia.UI.* from nuget.org only
```

The generated `NuGet.config` references the public feed only, and a single
`<FsSkiaUiVersion>` pins every `FS.Skia.UI.*` package plus the build engine — so a
consumer upgrade is one edit (see the generated `docs/UPGRADING.md`). The full
consumer install/upgrade flow and the maintainer release/publish sequence live in
[`docs/distribution.md`](docs/distribution.md).

**Framework developers** working from a clone instead validate against the local feed:
`dotnet tool restore && ./fake.sh build -t PackLocal` writes the packages to
`~/.local/share/nuget-local` (registered as a user-level NuGet source), and
`dotnet new install .` installs the template from source.

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
Because `update` is pure, its behavior is tested without ever opening a window.
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

## Development environment

The repository ships a ready-made development container that bundles every
prerequisite — the .NET SDKs (6.0/8.0/10.0), F# tooling (`fsautocomplete`,
`fantomas`, FAKE, Paket, Fable), the native graphics libraries SkiaSharp and
Vulkan need, and the local NuGet feed — so you don't assemble them by hand. From
the repository root:

```bash
./Container/create-fs-skia-ui-dev.sh --workspace="$PWD" --rebuild
```

This builds the image, starts a rootless Podman container with your workspace
mounted at `/workspace`, forwards GPU/display where available, and drops you into
a shell. See
[Container/fs-skia-ui-container.md](Container/fs-skia-ui-container.md) for the
full prerequisite list and options.

You still bring two things the container can't: a **Vulkan-capable GPU** on a
Windows or Linux desktop host (macOS, mobile, browser, and headless production
are out of scope), and a **coding agent** for the Spec Kit workflow (currently
Claude Opus 4.8 or Codex 5.5).

> **Preview status.** This is a first-version, preview toolkit. There are no
> stable published packages yet — consume it from this repository using the
> local-feed flow above. Treat the SkiaSharp 4 preview dependency as
> preview-risk: Vulkan driver behavior, native assets, and package shape may
> change before SkiaSharp 4 is stable. The public API exposes no renderer
> selection and provides no OpenGL/CPU/software/fallback renderer.

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

Package boundaries are enforced: e.g. `Controls` may not reference
Silk.NET/SkiaSharp/Elmish directly, keeping the layers honest.

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
| [FAKE](https://fake.build/) | 6.1.4 | F# Make build automation — the `Fake.Core.Target` library, compiled into the `build/Build.fsproj` front-end (no FSX runner). |
| [Spec Kit](https://github.com/github/spec-kit) | — | Spec-driven development and evidence governance. |
| [Expecto](https://github.com/haf/expecto) | 10.2.2 | F# test framework. |
| [YoloDev.Expecto.TestSdk](https://github.com/YoloDev/YoloDev.Expecto.TestSdk) | 0.15.3 | Expecto adapter for `dotnet test`. |
| [Microsoft.NET.Test.Sdk](https://github.com/microsoft/vstest) | 17.11.1 | Test host/runner. |

Exact pins are centralized in
[Directory.Packages.props](Directory.Packages.props).

---

## Developing FS.Skia.UI itself

Maintainers work in this repository through the same Spec Kit process, validated
by FAKE targets run **one at a time, sequentially** (FAKE-backed commands share
`.fake` state and are not safe to run concurrently; run them in the deterministic
order below):

1. `./fake.sh build -t Dev` — restore, build, test
2. `./fake.sh build -t GeneratedGuidanceCheck` — spec/plan template governance
3. `./fake.sh build -t TemplateCheck` — template pack/install/instantiate/smoke
4. `./fake.sh build -t GeneratedProductCheck` — generated-product matrix
5. `./fake.sh build -t EvidenceGraph` — validate the task DAG
6. `./fake.sh build -t EvidenceAudit` — merge gate

`./fake.sh build -t Verify` and `./fake.sh build -t Ci` are the broad aggregate
gates. Public visibility lives in
`.fsi` signature files (the contract chain is Spec → `.fsi` → semantic tests →
implementation → surface baseline). See [docs/reports/build.md](docs/reports/build.md),
[docs/reports/testing.md](docs/reports/testing.md), [docs/reports/evidence.md](docs/reports/evidence.md),
[docs/reports/speckit.md](docs/reports/speckit.md), and
[.specify/memory/constitution.md](.specify/memory/constitution.md).

---

## License

Licensed under the [MIT License](LICENSE).
