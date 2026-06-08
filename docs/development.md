---
title: Developing FS.Skia.UI
category: Guides
categoryindex: 10
---

# Developing FS.Skia.UI

This page covers the development container, the technology stack, and the maintainer
validation flow for working **on the framework itself**. Consuming the framework needs none of
this — see the [README](../README.md) "Get started" section and
[distribution.md](distribution.md).

## Development environment

The repository ships a ready-made development container that bundles every prerequisite — the
.NET SDKs (6.0/8.0/10.0), F# tooling (`fsautocomplete`, `fantomas`, FAKE, Paket, Fable), the
native graphics libraries SkiaSharp and Vulkan need, and the local NuGet feed — so you don't
assemble them by hand. From the repository root:

```bash
./Container/create-fs-skia-ui-dev.sh --workspace="$PWD" --rebuild
```

This builds the image, starts a rootless Podman container with your workspace mounted at
`/workspace`, forwards GPU/display where available, and drops you into a shell. See
[Container/fs-skia-ui-container.md](../Container/fs-skia-ui-container.md) for the full
prerequisite list and options.

You still bring two things the container can't: a **Vulkan-capable GPU** on a Windows or Linux
desktop host (macOS, mobile, browser, and headless production are out of scope), and a **coding
agent** for the Spec Kit workflow (currently Claude Opus 4.8 or Codex 5.5).

## Maintainer validation flow

Maintainers work in this repository through the same Spec Kit process. Run
**`./fake.sh build -t Route`** first — it reads your change and prints the authoritative *tier*
and the *minimal gate list* to run. Routine framework work routes to the light `inner-loop`
tier (`Dev` only); consumer-contract changes escalate automatically.

FAKE-backed commands share `.fake` state and are **not** safe to run concurrently — run them
one at a time. When a change escalates, the serialized order is:

1. `./fake.sh build -t Dev` — restore, build, test
2. `./fake.sh build -t GeneratedGuidanceCheck` — spec/plan template governance
3. `./fake.sh build -t TemplateCheck` — template pack/install/instantiate/smoke
4. `./fake.sh build -t GeneratedProductCheck` — generated-product matrix
5. `./fake.sh build -t EvidenceGraph` — validate the task DAG
6. `./fake.sh build -t EvidenceAudit` — merge gate

`./fake.sh build -t Verify` and `./fake.sh build -t Ci` are the broad aggregate gates. Public
visibility lives in `.fsi` signature files (the contract chain is Spec → `.fsi` → semantic
tests → implementation → surface baseline). See
[docs/reports/build.md](reports/build.md), [docs/reports/testing.md](reports/testing.md),
[docs/reports/evidence.md](reports/evidence.md), [docs/reports/speckit.md](reports/speckit.md),
and [.specify/memory/constitution.md](../.specify/memory/constitution.md).

Releasing to nuget.org is described in [distribution.md](distribution.md) (CI trusted
publishing via `.github/workflows/publish.yml`).

## Built with

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

Exact pins are centralized in [Directory.Packages.props](../Directory.Packages.props).
