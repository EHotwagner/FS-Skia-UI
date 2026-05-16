---
name: fs-skia-charts
description: Work on chart scene builders and generated product chart usage.
---

# Charts Capability

## Scope

Owns `src/Charts/`, chart tests, `template/fragments/charts/`, and generated product chart examples.

## Public Contract

The supported API lives in `src/Charts/*.fsi`. Surface changes require `readiness/surface-baselines/FS.Skia.UI.Charts.txt`.

## Build Commands

Run `./fake.sh build -t CapabilityCheck`, `./fake.sh build -t DependencyReport`, and `./fake.sh build -t PackageSurfaceCheck`.

## Test Commands

Run `dotnet test tests/Charts.Tests/Charts.Tests.fsproj` and `./fake.sh build -t GeneratedProductCheck`.

## Evidence

Record package-surface and generated-product evidence under `specs/009-v3-modular-framework/readiness/package-surfaces/`.

## Package Boundary

Charts may depend on Scene only. Keep data grid and chart builders pure and avoid viewer or keyboard coupling.

## Generated Product

Products that select charts receive the charts package reference and this skill.
