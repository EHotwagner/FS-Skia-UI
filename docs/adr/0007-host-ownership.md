# ADR 0007 — Host ownership

- **Status**: Accepted
- **Date**: 2026-06-02
- **Decision source**: the V3 modular-distribution implementation plan
  (`docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md`, §0.4 + Stage 1)
  and feature `048-v3-retirement-baseline` (research.md D8).

## Context

The Vulkan/Skia host — `Viewer`, `VulkanStartup`, `VulkanResources`, and the runtime
diagnostics — lives inside the monolith `src/Lib` (`PackageId = FS.Skia.UI`). The split
package `FS.Skia.UI.SkiaViewer` is *supposed* to be the host package, but it
**project-references the monolith** (`src/SkiaViewer/SkiaViewer.fsproj → ..\Lib\Lib.fsproj`)
and bridges types through `SceneConversion.fs`. So today the host still physically lives
in the monolith, and any consumer of `SkiaViewer` (or `Elmish → SkiaViewer`) transitively
resolves `FS.Skia.UI` (see `docs/reports/_baselines/2026-06-02-v3-before.md` §5).

## Decision

**`FS.Skia.UI.SkiaViewer` owns the Vulkan/Skia host.** The host modules (`Viewer`,
`VulkanStartup`, `VulkanResources`, runtime diagnostics) move out of `src/Lib` into
`src/SkiaViewer`, and the `Lib.fsproj` project reference is deleted. `SkiaViewer` depends
only on the split packages it needs (`Scene`, `KeyboardInput`) — never on the monolith.

## Alternatives considered

- **Leave the host in the monolith and ship `SkiaViewer` as a thin façade (rejected).**
  Keeps the monolith on every host consumer's transitive graph; defeats the retirement.
- **Create a new `FS.Skia.UI.Host` package distinct from `SkiaViewer` (rejected).**
  Adds a package without need; `SkiaViewer` is already the named host package and its
  consumers already reference it.

## Rationale

The host is the single largest block of monolith code that has a natural split-package
home. Moving it is the keystone that lets the monolith be deleted. Owning it in
`SkiaViewer` makes the host-package's name honest and removes the `SkiaViewer → Lib` leak.

## Affected stages

- **Stage 1** (keystone): host extraction into `SkiaViewer`; `Lib.fsproj` reference removed.
- Gated by the **parity oracle** (ADR 0011) so the move is provably behaviour-preserving.
