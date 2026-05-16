---
name: fs-skia-skiaviewer
description: Work on viewer host contracts and generated product viewer usage.
---

# SkiaViewer Capability

## Scope

Owns `src/SkiaViewer/`, viewer tests, `template/fragments/skiaviewer/`, and generated product viewer startup guidance.

## Public Contract

The supported API lives in `src/SkiaViewer/SkiaViewer.fsi`. Surface changes require `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`.

## Build Commands

Run `./fake.sh build -t CapabilityCheck`, `./fake.sh build -t DependencyReport`, and `./fake.sh build -t PackLocal`.

## Test Commands

Run `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` and `./fake.sh build -t GeneratedProductCheck`.

## Evidence

Capture real viewer command or package evidence under `specs/009-v3-modular-framework/readiness/package-surfaces/`. Disclose synthetic native evidence if a platform window system is unavailable.

## Package Boundary

Keep native window and render effects at the interpreter edge. Scene descriptions stay in Scene; Elmish adapter behavior stays in Elmish.

## Generated Product

Products that select SkiaViewer receive viewer package references, this skill, and product commands that avoid framework gallery checks.
