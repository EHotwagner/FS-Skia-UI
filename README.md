# FS.Skia.UI

A **Spec Kit-first** F# desktop UI framework. You describe the app you want as
specifications and drive a governed, agent-run workflow — it produces an
[Elmish](https://elmish.github.io/elmish/) (Model-View-Update) application rendered with
[SkiaSharp](https://github.com/mono/SkiaSharp) on [Vulkan](https://www.vulkan.org/), backed by
evidence at every step.

You don't hand-write the window plumbing, the render loop, or the MVU wiring. You author
specifications, drive the [Spec Kit](https://github.com/github/spec-kit) workflow with a coding
agent, and review the evidence it produces. The framework owns the host edge (window, input,
Vulkan/Skia setup, frame rendering, screenshots, diagnostics, shutdown); the workflow produces
the model, messages, `update`, and `view` that returns immutable `Scene` values.

## Why it holds up

The point is that an **agent can build your app and you can trust the result**, because the
process is engineered for robustness:

- **Spec-driven.** Every feature goes spec → plan → dependency-ordered tasks → implementation.
  Intent is written and reviewable before any code exists.
- **Contract-first: task → `.fsi` → tests → implementation.** Public surface is sketched as an
  `.fsi` signature and exercised in FSI *before* the `.fs` body exists; semantic tests run
  through that same surface and surface baselines are validated, so the contract can't drift.
- **Elmish that is actually testable.** Stateful / I/O work goes through an MVU boundary with a
  pure `update`, tested on both sides — pure transitions *and* interpreter tests against real
  I/O — so logic is verified without a window.
- **Per-task skills.** Each generated task is tagged with the skills needed to implement it; the
  agent loads them before touching that task's code.
- **Evidence that can't be faked.** Work can't claim "done" without proof. `EvidenceGraph`
  tracks unproven (synthetic) tasks; `EvidenceAudit` is a merge gate that **hard-blocks** on
  unjustified synthetic tasks or block-severity findings.
- **One compiled rulebook, one entry point.** All governance lives in the compiled
  `FS.Skia.UI.Build` library (a [FAKE](https://fake.build/) front-end run via `./fake.sh`), so a
  mistyped gate is a compile error and generated artifacts are single-sourced, not hand-synced.
  Run **`./fake.sh build -t Route`** first: it prints the *tier* and *minimal gate list* for
  your change. Routine work routes to a light `inner-loop` tier; consumer-contract changes
  escalate automatically.

## How you build with it

Spec Kit is the primary interface. Every generated project carries the `.specify/` install and
project-local `speckit-*` skills, so you build features through a governed loop:

```
specify  →  plan  →  tasks  →  implement  →  evidence
```

1. **`$speckit-specify`** — describe the feature in plain language → `spec.md`.
2. **`$speckit-plan`** — turn the spec into an implementation plan and design artifacts.
3. **`$speckit-tasks`** — break the plan into dependency-ordered tasks, each tagged with skills.
4. **`$speckit-implement`** — the agent works each task contract-first (`.fsi` → tests →
   implementation), producing the F# MVU code and the readiness evidence.
5. **evidence** — `EvidenceGraph` / `EvidenceAudit` gate the result.

The workflow is driven by coding agents with synchronized skill peers for
[Claude Code](https://www.anthropic.com/claude-code) (`.claude/skills/`) and
[Codex](https://openai.com/codex/) (`.agents/skills/`). It is currently developed and tested
against **Claude Opus 4.8** and **Codex 5.5**.

## Get started

### 1. Generate a project from nuget.org

The 11 `FS.Skia.UI.*` libraries and the template are published on **public nuget.org** — no repo
clone, no local feed:

```bash
dotnet new install FS.Skia.UI.Template          # from nuget.org
dotnet new fs-skia-ui --name MyApp              # default profile: app
cd MyApp
dotnet restore                                  # resolves FS.Skia.UI.* from nuget.org only
```

The generated `NuGet.config` references the public feed only, and a single `<FsSkiaUiVersion>`
pins every `FS.Skia.UI.*` package plus the build engine — so a consumer upgrade is one edit
(see the generated `docs/UPGRADING.md`). Full install/upgrade and release detail:
[`docs/distribution.md`](docs/distribution.md).

**Template options:**

| Option | Default | Effect |
|--------|---------|--------|
| `--profile <p>` | `app` | Which product to scaffold (see table below). |
| `--feedback true` | `false` | Capture per-phase Spec Kit feedback into `specs/<feature>/feedback/` — adds the `after_*` feedback hooks and the `fs-skia-feedback-capture` skill so each completed phase records process friction, generalizable-code candidates, skill gaps, and a severity. Default `false` induces no diff. |
| `--skipGitInit true` | `false` | Don't create the initial Git commit (use when generating inside an existing repo). |

| Profile | Scaffolds |
|---------|-----------|
| `app` | Default product — Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Controls, product tests + governance. |
| `headless-scene` | Headless Scene-only product for scene/widget authoring (no live window). |
| `governed` | Scene plus Testing helpers, governance-focused. |
| `sample-pack` | Scene, SkiaViewer, Elmish + sample-pack gallery content. |

All profiles carry the Spec Kit install and `speckit-*` skills.

### 2. Drive the workflow

Open the generated project with your coding agent and hand it a feature. The
[`docs/testSpecs/`](docs/testSpecs/) folder has ready-made plain-language game specs you can use
as a quickstart — e.g. [Pong](docs/testSpecs/pong.md):

```text
$speckit-specify  Build the Pong demo described in docs/testSpecs/pong.md
$speckit-plan
$speckit-tasks
$speckit-implement
```

The agent produces the spec, plan, tasks, the F# implementation, and the readiness evidence.
Then build and run what it produced:

```bash
./fake.sh build -t Dev                          # restore, build, test
dotnet run --project src/MyApp/MyApp.fsproj
```

## What the workflow produces

An ordinary Elmish app: a model, messages, an `update`, and a `view` that returns immutable
`Scene` values. You read and review code shaped like this — you don't write it from scratch:

```fsharp
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

let view () =
    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 18uy 24uy 32uy 255uy)
        Scene.text (48.0, 88.0) "Hello FS.Skia.UI" Colors.white
    ]

let options = { Title = "Hello"; InitialSize = { Width = 640; Height = 480 } }

[<EntryPoint>]
let main _ =
    match Viewer.run options (view ()) with
    | Ok _ -> 0
    | Error d -> eprintfn "%s" d.Message; 1
```

The application owns the MVU four-tuple; the framework owns everything around it. Because
`update` is pure, its behavior is tested without ever opening a window. When something can't
start (unsupported OS, no Vulkan surface, swapchain/context failure), `Viewer.run` /
`Viewer.runApp` returns `Result<_, ViewerRunFailure>` with a stage you can report — it does not
throw or silently fall back.

The workflow can compose any of these capability packages:

| Package | Capability |
|---------|-----------|
| `FS.Skia.UI.Scene` | Dependency-light scene vocabulary — shapes, paths, text, images, paint. |
| `FS.Skia.UI.SkiaViewer` | The Vulkan/Skia viewer host (`Viewer.run`, `Viewer.runApp`) + window/close-reason contracts. |
| `FS.Skia.UI.Elmish` | The adapter that bridges the Elmish program to the viewer. |
| `FS.Skia.UI.Layout` | Pure flex/grid and graph layout scene builders, hit testing. |
| `FS.Skia.UI.KeyboardInput` | Package-owned keyboard runtime, reducer, effects, diagnostics, state display. |
| `FS.Skia.UI.Input` | Host-coupled input runtime — YAML key bindings, modes, sequences, command intents. |
| `FS.Skia.UI.Controls` | Declarative controls — buttons, text, charts, graph views, `DataGrid`, theming. |
| `FS.Skia.UI.Controls.Elmish` | Wires Controls + keyboard runtime effects into the Elmish program. |
| `FS.Skia.UI.Testing` | Generated-product and package validation helpers. |
| `FS.Skia.UI.SkillSupport` | Backing library for the authoring skills — DAG algorithms, parsing, globbing, code generation. |
| `FS.Skia.UI.Build` | The compiled governance engine (evidence graph + merge-gate audit). |

### How they fit together

`Scene` is the shared vocabulary everything returns; `SkiaViewer` is the Vulkan/Skia host:

```
Scene  ─────────────────  pure scene primitives (FSharp.Core only)
 ├─ Layout              (+ Yoga.Net)    flex/grid + graph layout, hit testing
 ├─ Testing                             generated-product validation helpers
 ├─ KeyboardInput       (+ YamlDotNet)  key bindings, reducer, effects
 │   └─ Controls                        controls, charts, DataGrid, graph views
 │       └─ Controls.Elmish             control + keyboard command/subscription adapters
 └─ SkiaViewer          (+ Silk.NET, SkiaSharp)   Vulkan desktop host  ← Viewer.run/runApp
     ├─ Elmish          (+ Fable.Elmish)          Elmish ↔ viewer adapter
     └─ Input                                     host-coupled interactive input runtime
```

Package boundaries are enforced: e.g. `Controls` may not reference Silk.NET/SkiaSharp/Elmish
directly, keeping the layers honest. (`FS.Skia.UI.SkillSupport` and `FS.Skia.UI.Build` are
authoring/build-time libraries, outside this runtime graph.)

## Releases & distribution

Packages ship on **nuget.org** on the **preview** channel (libraries `0.1.68-preview.1`,
template `0.1.87-preview.1`). The production push runs in CI with **no stored API key**, using
NuGet [Trusted Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing)
(GitHub OIDC): `.github/workflows/publish.yml` mints a short-lived key per run, the job is
gated behind a protected `release` environment requiring maintainer approval, and all 12
packages push idempotently (`--skip-duplicate`). Channel is explicit in the version value
(`-preview.N` ⇒ preview; bare `MAJOR.MINOR.PATCH` ⇒ stable). Full flow:
[`docs/distribution.md`](docs/distribution.md).

> **Preview status.** This is a first-version preview toolkit on the `-preview` channel. Treat
> the SkiaSharp 4 preview dependency as preview-risk: Vulkan driver behavior, native assets, and
> package shape may change before SkiaSharp 4 is stable. The public API exposes no renderer
> selection and provides no OpenGL/CPU/software/fallback renderer. You bring a **Vulkan-capable
> GPU** on a Windows or Linux desktop host (macOS, mobile, browser, and headless production are
> out of scope) and a **coding agent** for the Spec Kit workflow.

## Developing FS.Skia.UI itself

Working on the framework — the dev container, the technology stack, and the maintainer
validation flow — is covered in [`docs/development.md`](docs/development.md) and
[`docs/reports/build.md`](docs/reports/build.md). In short: maintainers work through the same
Spec Kit process, run `./fake.sh build -t Route` to get the minimal gate list, and run
FAKE-backed gates **one at a time** (they share `.fake` state and aren't concurrency-safe).
`./fake.sh build -t Dev` is the inner-loop gate (restore, build, test).
`./fake.sh build -t Verify` and `./fake.sh build -t Ci` are the broad aggregate gates.

## License

Licensed under the [MIT License](LICENSE).
