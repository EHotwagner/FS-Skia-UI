# Research: Upgrade SkiaSharp And Spec Kit

## Version Selection Policy

**Decision**: Treat SkiaSharp `4.147.0-preview.3.1` and Spec Kit `0.8.15` as
planning candidates observed on 2026-05-28, then re-check official package and
release sources immediately before implementation edits.

**Rationale**: Package versions are time-sensitive. The repository currently
uses SkiaSharp `4.147.0-preview.2.1` and Spec Kit `0.8.11`, while current
official sources show newer candidates. Capturing the observed versions keeps
planning concrete without making stale data a permanent requirement.

**Alternatives considered**: Pin exact versions in the spec and skip
implementation-time verification. Rejected because the upgrade target can
change after planning and before implementation.

**Sources checked**:

- NuGet Gallery `SkiaSharp`: https://www.nuget.org/packages/SkiaSharp/
- NuGet Gallery `SkiaSharp` `4.147.0-preview.3.1`: https://www.nuget.org/packages/SkiaSharp/4.147.0-preview.3.1
- GitHub `github/spec-kit` releases: https://github.com/github/spec-kit/releases

## SkiaSharp Package Family Alignment

**Decision**: Update `SkiaSharp`, `SkiaSharp.NativeAssets.Linux`, and
`SkiaSharp.NativeAssets.Win32` as one aligned package family unless official
package compatibility evidence requires an explicit mismatch.

**Rationale**: The renderer and viewer depend on native assets matching the
managed SkiaSharp package. Misaligned previews can produce runtime failures
that are hard to distinguish from Vulkan/windowing host issues.

**Alternatives considered**: Update only `SkiaSharp` and leave native assets on
the older preview. Rejected because it weakens dependency governance and makes
host evidence ambiguous.

## Spec Kit Asset Update Scope

**Decision**: Treat Spec Kit as a governed asset set covering
`.specify/init-options.json`, extensions, presets, templates, workflows,
generated template copies under `template/base`, and project-local skills
included in generated products.

**Rationale**: This repository vendors and customizes Spec Kit behavior. A
version field update alone would leave generated projects and command behavior
on stale templates or skill copies.

**Alternatives considered**: Change only `.specify/init-options.json`.
Rejected because generated users would still receive old assets and governance
checks could drift from the repository root.

## Compatibility Package Posture During Upgrade

**Decision**: Freeze `FS.Skia.UI` as a compatibility surface during this
upgrade. Inventory consumers, classify public surface, and document release
policy, but do not remove APIs or choose deprecation/facade migration as part
of the version bump.

**Rationale**: The compatibility package analysis concluded that package
direction requires consumer inventory, public-surface classification, focused
replacement coverage, dependency reports, and release notes. Bundling removal
or deprecation into a dependency upgrade would hide product risk behind
maintenance work.

**Alternatives considered**: Deprecate `FS.Skia.UI` while upgrading package
versions. Rejected because replacement coverage and external migration posture
are not yet proven.

## Evidence and Test Strategy

**Decision**: Reuse existing FAKE governance/report targets and add focused
governance tests for missing assertions: SkiaSharp family alignment, Spec Kit
asset alignment, generated template pin alignment, compatibility consumer
inventory coverage, and accidental broad-package dependency detection.

**Rationale**: The repository already has dependency, template, guidance,
surface, graph, and audit targets. Extending those checks keeps evidence in the
existing review flow.

**Alternatives considered**: Add a separate standalone upgrade verifier.
Rejected because it would duplicate existing targets and make readiness harder
to audit.
