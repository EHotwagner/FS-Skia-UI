# Tier 1 Scope And Evidence Obligations

This feature is a Tier 1 dependency/governance change.

Affected file groups:

- `Directory.Packages.props` and dependency documentation for SkiaSharp package-family movement.
- `.specify/*`, template-owned Spec Kit copies, generated guidance, and selected local skills for Spec Kit version posture.
- `template/base/Directory.Packages.props` and `.template.package/FS.Skia.UI.Template.fsproj` for generated product package posture.
- Compatibility readiness artifacts for `FS.Skia.UI` consumer inventory, surface classification, sample decisions, and release policy.

Public API default: no `.fsi` change is authorized. `PackageSurfaceCheck` and `readiness/package-surface-baseline.md` are the required evidence. A detected compatibility-package surface delta pauses implementation for `.fsi` sketch, semantic tests, FSI/package evidence, docs, and approval.

MVU/effect-boundary applicability: no new product MVU workflow is introduced. Existing build, dependency, package-surface, template, and evidence workflows remain the I/O boundary and must keep observable command paths, report fields, and actionable failures.

Synthetic evidence restrictions: synthetic evidence is not acceptable for version selection, dependency reports, package-surface status, generated template alignment, or compatibility consumer inventory. Unsupported-host facts count only when they come from real command output with preserved failure reasons.

Risk levels:

- Small: readiness-only wording, one focused documentation clarification, or one validator expectation.
- Medium: generated template pins, Spec Kit copied assets, package guidance, compatibility inventory tooling.
- Broad: central package version movement, public `.fsi` or package-surface difference, template package metadata, generated profile behavior, or native/viewer validation outcome.

