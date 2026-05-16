---
name: fs-skia-keyboard-input
description: Work on keyboard input contracts and generated product keyboard guidance.
---

# KeyboardInput Capability

## Scope

Owns `src/KeyboardInput/`, keyboard input tests, `template/fragments/keyboard-input/`, and generated product keyboard reducer usage.

## Public Contract

The supported API lives in `src/KeyboardInput/KeyboardInput.fsi`. Surface changes require `readiness/surface-baselines/FS.Skia.UI.KeyboardInput.txt`.

## Build Commands

Run `./fake.sh build -t CapabilityCheck`, `./fake.sh build -t DependencyReport`, and `./fake.sh build -t PackLocal`.

## Test Commands

Run `dotnet test tests/KeyboardInput.Tests/KeyboardInput.Tests.fsproj` and `./fake.sh build -t GeneratedProductCheck`.

## Evidence

Capture reducer transition and emitted effect evidence under `specs/009-v3-modular-framework/readiness/package-surfaces/`.

## Package Boundary

Keyboard input may depend on Scene and YamlDotNet only. Keep viewer hosting, controls, charting, graphing, and layout concerns out of this package; use `fs-skia-ui-widgets` for widget authoring.

## Generated Product

Products that select keyboard input receive the keyboard skill only when selected directly or as a prerequisite.
