---
name: fs-skia-scene
description: Work on dependency-light scene primitives and generated product scene usage.
---

# Scene Capability

## Scope

Owns `src/Scene/`, Scene package tests, `template/fragments/scene/`, and generated product code that builds pure scene descriptions.

## Public Contract

The supported API lives in `src/Scene/Scene.fsi`. Surface changes require `readiness/surface-baselines/FS.Skia.UI.Scene.txt` and package-surface evidence.

## Build Commands

Run `./fake.sh build -t CapabilityCheck`, `./fake.sh build -t PackageSurfaceCheck`, and `./fake.sh build -t PackLocal` when this capability changes.

## Test Commands

Run `dotnet test tests/Scene.Tests/Scene.Tests.fsproj` and `./fake.sh build -t GeneratedProductCheck`.

## Evidence

Update `specs/009-v3-modular-framework/readiness/package-surfaces/` and `capability-catalog.md` for contract or catalog changes.

## Package Boundary

Scene must not reference Elmish, Silk.NET, SkiaSharp, Yoga.Net, or YamlDotNet. Keep host, input, layout, and chart concerns outside this package.

## Generated Product

Scene is included in every app, governed, headless scene, and sample-pack product as the base capability.
