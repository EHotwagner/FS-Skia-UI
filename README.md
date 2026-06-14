[Archived for now. This is an exploratory repo and I explored. I do think the basic premises are sound and can bring real gains to agent based F# development processes. Spec based with task -> .fsi -> tests -> implementation is an easy win in my opinion. Why stop? It just got too heavy and should be split up. Dog-fooding governance mechanisms is no fun.]

# FS.Skia.UI

[![NuGet](https://img.shields.io/nuget/vpre/FS.Skia.UI.Scene?logo=nuget&label=nuget)](https://www.nuget.org/packages/FS.Skia.UI.Scene)
[![Template](https://img.shields.io/nuget/vpre/FS.Skia.UI.Template?logo=nuget&label=template)](https://www.nuget.org/packages/FS.Skia.UI.Template)
[![Downloads](https://img.shields.io/nuget/dt/FS.Skia.UI.Scene?logo=nuget&label=downloads)](https://www.nuget.org/packages/FS.Skia.UI.Scene)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A Spec Kit-first F# desktop UI framework. You describe the app as specifications and drive a
governed, agent-run workflow that produces an
[Elmish](https://elmish.github.io/elmish/) (Model-View-Update) application rendered with
[SkiaSharp](https://github.com/mono/SkiaSharp) on [Vulkan](https://www.vulkan.org/), with
evidence recorded at each step.

You don't hand-write the window plumbing, the render loop, or the MVU wiring. You author
specifications, drive the [Spec Kit](https://github.com/github/spec-kit) workflow with a coding
agent, and review the evidence it produces. The framework owns the host edge (window, input,
Vulkan/Skia setup, frame rendering, screenshots, diagnostics, shutdown); the workflow produces
the model, messages, `update`, and `view` that returns immutable `Scene` values.

## How the process is governed

The workflow is structured so an agent's output is reviewable and verifiable at each step:

- **Spec-driven.** Every feature goes spec → plan → dependency-ordered tasks → implementation.
  Intent is written and reviewable before any code exists.
- **Contract-first: task → `.fsi` → tests → implementation.** Public surface is sketched as an
  `.fsi` signature and exercised in FSI before the `.fs` body exists; semantic tests run
  through that same surface and surface baselines are validated, so the contract does not drift.
- **Testable Elmish boundary.** Stateful / I/O work goes through an MVU boundary with a pure
  `update`, tested on both sides — pure transitions and interpreter tests against real I/O — so
  logic is verified without a window.
- **Per-task skills.** Each generated task is tagged with the skills needed to implement it; the
  agent loads them before touching that task's code.
- **Evidence gates.** A task cannot be marked done without supporting evidence. `EvidenceGraph`
  tracks unproven (synthetic) tasks; `EvidenceAudit` is a merge gate that blocks on unjustified
  synthetic tasks or block-severity findings.
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

## Documentation

Full documentation — getting started, the generated API reference, the
architecture (one page per subsystem with a candid analysis), the governance
system, and the typed-control / design-token workflow — lives on the
documentation site:

### 📖 **https://ehotwagner.github.io/FS-Skia-UI/**

Start there and follow the role-based entry points, or jump straight to a topic:

### Using the library

- [Get started](https://ehotwagner.github.io/FS-Skia-UI/) — install the template, generate a project, drive the workflow.
- [API reference](https://ehotwagner.github.io/FS-Skia-UI/reference/index.html) — every supported public type and member, by package.
- [Typed controls & the MVU front door](https://ehotwagner.github.io/FS-Skia-UI/controls-design/typed-front-door.html)
- [Design tokens & the Penpot flow](https://ehotwagner.github.io/FS-Skia-UI/controls-design/design-tokens-penpot.html)
- Runnable examples: [typed control / MVU](https://ehotwagner.github.io/FS-Skia-UI/examples/typed-control-mvu.html), [design-token flow](https://ehotwagner.github.io/FS-Skia-UI/examples/design-token-flow.html)

### Contributing to the framework

- [Architecture overview](https://ehotwagner.github.io/FS-Skia-UI/architecture/host-skiaviewer.html) — host, scene, layout, input, Elmish/MVU, controls, testing, governance.
- [Governance system](https://ehotwagner.github.io/FS-Skia-UI/governance/index.html) — [routing & gates](https://ehotwagner.github.io/FS-Skia-UI/governance/routing-and-gates.html), [evidence & audit](https://ehotwagner.github.io/FS-Skia-UI/governance/evidence-and-audit.html), [single-source generation](https://ehotwagner.github.io/FS-Skia-UI/governance/single-source-generation.html).
- [Developing FS.Skia.UI itself](https://ehotwagner.github.io/FS-Skia-UI/development.html)

### Spec Kit practitioners

- [Governance & speckit placement](https://ehotwagner.github.io/FS-Skia-UI/governance/speckit-placement.html) — which speckit phase governs each touchpoint, and how to respond.
- [The Spec Kit process](https://ehotwagner.github.io/FS-Skia-UI/speckit/process.html) — where custom FS Skia UI components are created and consumed.

### Releases & distribution

- [Releases & distribution](https://ehotwagner.github.io/FS-Skia-UI/distribution.html) — nuget.org install/upgrade and the Trusted-Publishing CI flow.

> **Preview status.** First-version preview on the `-preview` channel. Requires a
> **Vulkan-capable GPU** on a Windows/Linux desktop host (macOS, mobile, browser,
> and headless production are out of scope; no software/CPU fallback renderer) and
> a **coding agent** for the Spec Kit workflow.

## Building & validating the framework

Maintainers work through the same Spec Kit loop. Run **`./fake.sh build -t Route`** first to get
the minimal gate list for your change, then run only the gates it prints. Those gates are
FAKE-backed and share `.fake` state, so they are **not safe to run concurrently** — run them
**sequentially**, one at a time, never in parallel (safe non-FAKE file reads and checks may still
run in parallel):

- `./fake.sh build -t Dev` — the inner-loop gate (restore, build, test).
- `./fake.sh build -t Verify` and `./fake.sh build -t Ci` — the broad aggregate gates.

The full maintainer flow, technology stack, and dev container are covered in
[`docs/reports/build.md`](docs/reports/build.md) and on the
[development docs](https://ehotwagner.github.io/FS-Skia-UI/development.html).

## License

Licensed under the [MIT License](LICENSE).
