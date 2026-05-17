---
name: fs-skia-testing
description: Work on generated product and package validation helper contracts.
---

# Testing Capability

## Scope

Owns `src/Testing/`, testing helper contracts, `template/fragments/testing/`, and generated product validation helper guidance.

## Public Contract

The supported API lives in `src/Testing/Testing.fsi`. Surface changes require `readiness/surface-baselines/FS.Skia.UI.Testing.txt`.

## Build Commands

Run `./fake.sh build -t CapabilityCheck`, `./fake.sh build -t PackageSurfaceCheck`, and `./fake.sh build -t PackLocal`.

## Test Commands

Run `dotnet test tests/Testing.Tests/Testing.Tests.fsproj` and `./fake.sh build -t GeneratedProductCheck`.

## Evidence

Record helper surface evidence under the active feature readiness
package-surface reports. Stable public surface baselines live under
`readiness/surface-baselines/`.

## Package Boundary

Testing helpers must not pull broad framework implementation projects into generated products.

## Generated Product

Testing is available to governed products when selected and should stay product-validation focused.
