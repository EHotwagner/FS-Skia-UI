# Research: Template Packaging and Drift Governance

## Decision: Upgrade this repository in place before splitting an external template repository

**Rationale**: The user selected the in-place upgrade path during clarification, and the current repository already owns the canonical FAKE workflow, Spec Kit presets, evidence extension, docs, samples, and governance tests. Keeping V2 in this repository lets drift checks compare source, docs, templates, and build targets without introducing cross-repository synchronization risk.

**Alternatives considered**: Creating a new template repository now would make package validation look cleaner but would duplicate governance before the source repo has executable template drift checks. Publishing a separate template package without keeping this repo as source of truth would create the same drift problem V2 is meant to solve.

## Decision: Use standard `dotnet new` template metadata and local NuGet template packaging

**Rationale**: The repository is already .NET/F# and the prior template analysis identifies `dotnet new` as the native project creation mechanism. A root `.template.config/template.json` supports source-directory installation. A local template package project can pack the same template-owned source into `artifacts/templates/` for packaged-artifact validation.

**Alternatives considered**: A custom generator script would give full control but would create another command surface to govern. Copying directories manually would be hard to validate and would not exercise the template mechanism that users will actually run.

## Decision: Validate both source-directory installation and local packaged artifact installation

**Rationale**: Clarification selected both artifact boundaries. Source-directory installation catches template metadata and source modifier issues quickly. Packaged-artifact installation catches packaging omissions and validates the artifact shape future distribution will rely on.

**Alternatives considered**: Source-only validation would miss packaging defects. Package-only validation would slow local iteration and obscure source template errors behind packaging.

## Decision: Model default and minimal starter as explicit template profiles

**Rationale**: Clarification selected default plus one minimal starter profile. A `profile` choice symbol keeps generated project behavior explicit and testable. The minimal starter must include the core library, one basic sample, core tests, package checks, docs, and Spec Kit governance assets, while excluding optional layout, charts, parity, and visual sample scope.

**Alternatives considered**: Boolean-only options such as `--include-charts` and `--include-layout` are still useful under the default profile, but they do not give maintainers a stable minimal acceptance path. A single full profile would make generated projects heavier and would not prove the smallest governed product starting point.

## Decision: Extend the existing FAKE target graph instead of adding standalone validation commands

**Rationale**: V1 made FAKE the canonical command surface. Extending `build.fsx` keeps humans, CI, docs, Spec Kit hooks, and agents on one path. Because template validation is I/O-bearing, new work must extend the existing `BuildModel` / `BuildMsg` / `BuildEffect` / `update` / interpreter boundary.

**Alternatives considered**: Standalone shell scripts would be faster to write but would duplicate restore/build/template command ordering. A separate tool project would be more formal but not justified until the command surface proves too complex for the existing build graph.

## Decision: Use NuGet Central Package Management for direct dependency governance

**Rationale**: The repository already uses project files with many inline package versions. `Directory.Packages.props` is the simplest .NET-native way to move direct package versions into one reviewable policy while keeping project files readable and compatible with existing restore/build flows.

**Alternatives considered**: Paket was considered in the prior analysis but would add a new dependency workflow and lock-file model. Keeping inline versions would fail the core governance requirement.

## Decision: Treat local package smoke version properties as validation-only exceptions

**Rationale**: Some sample projects intentionally reference locally packed `FS.Skia.UI` packages through an overridable `FsSkiaUiPackageVersion` property. Those references validate package consumption rather than external dependency policy. V2 should document them as validation-only exceptions and fail on any other unmanaged inline version.

**Alternatives considered**: Forcing every local package smoke reference into central package management would make local package validation less flexible. Allowing arbitrary inline versions would weaken the dependency gate.

## Decision: Harden generated spec and plan templates through project templates and preset-owned overrides

**Rationale**: Generated features need governance prompts before tasks are created. The base `.specify/templates/spec-template.md` and `plan-template.md` are currently generic, and the local preset does not yet override spec or plan templates. V2 should harden both the active project templates and the preset-owned templates that generated products inherit.

**Alternatives considered**: Updating only `tasks-template.md` was sufficient for V1 but too late in the workflow for V2. Documentation-only guidance would rely on manual memory.

## Decision: Add a template drift gate backed by a template ownership profile and structured deferrals

**Rationale**: V2's key risk is silent divergence between source conventions and generated starter projects. Drift verification should inspect template-owned paths and command-surface changes, then fail unless corresponding template/docs/policy/guidance updates or accepted deferrals are present. Clarification requires every accepted deferral to include rationale, owner, and target phase.

**Alternatives considered**: Relying on reviewer judgment would not be repeatable. Treating every drift as a hard failure without deferrals would make future-roadmap and source-only work unnecessarily difficult.

## Decision: Keep visual evidence, release validation, external repo split, and distribution automation deferred

**Rationale**: The V2 spec explicitly focuses on template packaging/instantiation, dependency governance, generated artifact hardening, and template drift detection. Visual and release work need separate evidence policies and environment handling.

**Alternatives considered**: Folding visual and release gates into V2 would increase risk and obscure whether the template packaging milestone itself works.
