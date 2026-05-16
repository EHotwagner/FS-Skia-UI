---
name: fs-skia-elmish
description: Work on Elmish adapter contracts and generated product Elmish wiring.
---

# Elmish Capability

## Scope

Owns `src/Elmish/`, Elmish adapter tests, `template/fragments/elmish/`, and generated product Elmish entry points.

## Public Contract

The supported API lives in `src/Elmish/Elmish.fsi`. Surface changes require `readiness/surface-baselines/FS.Skia.UI.Elmish.txt`.

## Build Commands

Run `./fake.sh build -t CapabilityCheck`, `./fake.sh build -t DependencyReport`, and `./fake.sh build -t PackLocal`.

## Test Commands

Run `dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj` and `./fake.sh build -t GeneratedProductCheck`.

## Evidence

Record transition and effect evidence under `specs/009-v3-modular-framework/readiness/package-surfaces/` when adapter behavior changes.

## Package Boundary

Keep `Model`, `Msg`, `Effect`, `init`, and `update` pure. Native viewer I/O belongs to SkiaViewer interpreter code.

## Generated Product

Products that select Elmish receive Scene and SkiaViewer prerequisites plus this skill.
