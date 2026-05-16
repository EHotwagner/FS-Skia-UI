---
name: fs-skia-layout
description: Work on Yoga-backed layout contracts and generated product layout usage.
---

# Layout Capability

## Scope

Owns `src/Layout/`, layout tests, and runtime layout engine guidance. Generated product layout-control and widget examples are owned by `fs-skia-ui-widgets`.

## Public Contract

The supported API lives in `src/Layout/*.fsi`. Surface changes require `readiness/surface-baselines/FS.Skia.UI.Layout.txt`.

## Build Commands

Run `./fake.sh build -t CapabilityCheck`, `./fake.sh build -t DependencyReport`, and `./fake.sh build -t PackageSurfaceCheck`.

## Test Commands

Run `dotnet test tests/Layout.Tests/Layout.Tests.fsproj` and `./fake.sh build -t GeneratedProductCheck`.

## Evidence

Record package-surface and dependency evidence under `specs/009-v3-modular-framework/readiness/package-surfaces/`.

## Package Boundary

Layout may depend on Scene and Yoga.Net. Do not introduce viewer, keyboard, controls, or chart dependencies.

## Generated Product

Products that select Controls receive `FS.Skia.UI.Layout` as a runtime dependency but use `fs-skia-ui-widgets` for generated widget guidance. Use this skill only for lower-level layout engine work.
