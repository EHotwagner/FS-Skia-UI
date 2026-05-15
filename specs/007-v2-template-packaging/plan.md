# Implementation Plan: Template Packaging and Drift Governance

**Branch**: `007-v2-template-packaging` | **Date**: 2026-05-14 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/007-v2-template-packaging/spec.md`

## Summary

Deliver V2 by turning this repository into the governed source for a .NET project template while preserving the V1 FAKE workflow. The implementation adds a `dotnet new` template profile with default and minimal starter profiles, validates both source-directory installation and a locally packaged template artifact, centralizes package version governance through NuGet Central Package Management, hardens generated specification and planning templates, and adds drift verification that fails when template-owned files change without matching template, documentation, policy, or deferral updates.

V2 upgrades this repository in place. A separate external template repository, full visual evidence, release validation, and broader distribution automation remain deferred roadmap work.

## Technical Context

**Language/Version**: F# on .NET `net10.0`; local SDK observed as `10.0.300`; FAKE build script in F#; Bash and Windows command wrappers  
**Primary Dependencies**: Existing .NET SDK, FAKE repo-local tool, NuGet template packaging, `dotnet new`, Expecto test projects, existing Spec Kit extensions, and existing runtime packages governed by NuGet Central Package Management  
**Storage**: Filesystem only: `.template.config/template.json`, template package project metadata, `Directory.Packages.props`, docs, Spec Kit templates/presets, readiness artifacts, generated project temp roots, local template package output under `artifacts/templates/`, and local package output under `~/.local/share/nuget-local/`  
**Testing**: Expecto governance tests, FAKE workflow self-checks, `dotnet new` source install, local template package install, generated default/minimal project `Dev` verification, placeholder and excluded-history scans, no-inline-package-version checks, generated spec/plan guidance checks, and template drift checks  
**Target Platform**: Windows and Linux developer/CI environments that can run the current .NET solution and `dotnet new`; V2 remains non-visual and does not require GPU/window availability  
**Project Type**: Governed F# library/template repository with samples, tests, scripts, local build workflow, and Spec Kit workflow assets  
**Performance Goals**: Each fresh default-profile and minimal-starter generated project completes its fast verification workflow in 15 minutes or less on a supported CI/developer baseline matching the Windows/Linux target platform, local SDK constraints, and non-visual validation scope; validation evidence records per-project elapsed time  
**Constraints**: No runtime library public API changes are planned; V1 `Dev`, `Verify`, and `Ci` remain available; V2 must validate both source-directory and locally packaged template artifacts; visual evidence, release validation, external repo split, and public distribution automation are deferred; package smoke/local package version exceptions must be documented validation-only exceptions  
**Scale/Scope**: One repository source of truth; two generated profiles; two template artifact boundaries; all direct package references in repo-owned project files; generated spec and plan templates; template-owned files, docs, presets, dependency policy, samples, and command-surface drift

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Principle I - Spec -> FSI -> Semantic Tests -> Implementation**: PASS. This is a Tier 1 governance/template contract change, not a runtime `.fsi` API expansion. The user-facing surface is documented in [contracts/v2-template-workflow.md](./contracts/v2-template-workflow.md), [contracts/dependency-governance.md](./contracts/dependency-governance.md), and [contracts/generated-guidance.md](./contracts/generated-guidance.md). Tests must be added before implementation for template generation, dependency governance, guidance hardening, drift diagnostics, and explicit no-op runtime `.fsi`/surface-baseline impact.
- **Principle II - Visibility Lives in `.fsi`**: PASS. No new public runtime F# modules are planned. If implementation introduces reusable F# source under `src/`, it must include `.fsi`; build/test helper code remains repository-local automation. Governance evidence must confirm that no public runtime module or surface baseline changed without the corresponding signature and baseline update.
- **Principle III - Idiomatic Simplicity**: PASS. The plan uses standard `dotnet new` template metadata, NuGet Central Package Management, existing FAKE targets, simple file/process effects, and Expecto governance tests. No advanced F# features require justification.
- **Principle IV - Elmish/MVU Boundary**: PASS. Template validation, dependency reporting, and drift detection are I/O-bearing workflows, so `build.fsx` must extend the existing local MVU/effect algebra (`BuildModel`, `BuildMsg`, `BuildEffect`, `init`, pure `update`, interpreter at the edge) rather than burying process/file work in ad hoc command sequences.
- **Principle V - Synthetic Evidence Disclosure**: PASS. V2 requires real generated projects from both source and package artifacts, real `dotnet new` installation, real restore/build/test execution, real dependency scans, and real drift checks. Synthetic-only evidence is not planned.
- **Principle VI - Test Evidence Is Mandatory**: PASS. Tasks must add failing-first governance tests for template profile metadata, template validation workflow, no-inline dependency versions, dependency metadata, generated spec/plan prompts, drift failure diagnostics, and deferral field validation.
- **Principle VII - Observability and Safe Failure**: PASS. New validation targets must emit actionable logs under `specs/007-v2-template-packaging/readiness/` and fail clearly on missing placeholders, excluded history, restore/network failures, unmanaged dependencies, missing guidance prompts, missing artifact classes, and invalid drift deferrals.
- **Change Classification**: PASS. Tier 1 template contract and governance change; no runtime library public API impact.
- **Engineering Constraints**: PASS. F#/.NET remains the exclusive stack. SkiaSharp preview package use remains governed and documented. Pack output remains `~/.local/share/nuget-local/`; template package output is feature-owned under `artifacts/templates/`.

## Project Structure

### Documentation (this feature)

```text
specs/007-v2-template-packaging/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── dependency-governance.md
│   ├── generated-guidance.md
│   └── v2-template-workflow.md
├── checklists/
│   └── requirements.md
└── tasks.md             # Phase 2 output from /speckit-tasks
```

### Source Code (repository root)

```text
.template.config/
└── template.json                    # dotnet new profile and source modifiers

.template.package/
└── FS.Skia.UI.Template.fsproj        # local NuGet template package metadata

Directory.Packages.props             # central package versions and CPM enablement
Directory.Build.props                # remains shared build/package metadata

build.fsx                            # extend target graph and MVU effect algebra
fake.sh
fake.cmd

docs/
├── template-profile.md              # template ownership, profiles, options
├── dependencies.md                  # dependency owner/license/upgrade policy
├── speckit.md                       # generated artifact guidance and governance
├── build.md                         # add V2 targets and artifact paths
├── testing.md                       # add V2 validation matrix
└── evidence.md                      # add V2 readiness artifact classes

readiness/
├── surface-baselines/
└── template-deferrals.yml           # accepted drift deferrals with required fields

scripts/
├── dependency-report.fsx            # dependency metadata and no-inline scan
└── template-drift.fsx               # template ownership and deferral validation

.specify/
├── templates/
│   ├── spec-template.md             # hardened generated spec prompts
│   └── plan-template.md             # hardened generated planning prompts
├── presets/fsharp-opinionated/templates/
│   ├── spec-template.md             # preset-owned override for generated products
│   └── plan-template.md             # preset-owned override for generated products
└── workflows/speckit/workflow.yml   # keep delegating to canonical FAKE targets

tests/Governance.Tests/
├── TemplateProfileTests.fs
├── DependencyGovernanceTests.fs
├── GeneratedGuidanceTests.fs
└── TemplateDriftTests.fs
```

**Structure Decision**: Keep the source repository as the V2 source of truth. Add `.template.config/template.json` at the repository root for source-directory installation, and add a template package project to produce the local packaged artifact without requiring a separate repository. Extend the existing FAKE command surface so humans, CI, and agents use the same V2 targets. Keep generated project validation evidence under the active feature readiness folder and stable cross-feature policy artifacts under root-level `readiness/`.

## Complexity Tracking

No constitution violations require justification.

## Phase 0: Research

See [research.md](./research.md). Decisions cover in-repo `dotnet new` packaging, default/minimal starter profiles, source and packaged artifact validation, NuGet Central Package Management, validation-only package version exceptions, FAKE target integration, generated spec/plan hardening, template drift verification, deferral records, and deferred roadmap boundaries.

## Phase 1: Design & Contracts

See [data-model.md](./data-model.md), [contracts/v2-template-workflow.md](./contracts/v2-template-workflow.md), [contracts/dependency-governance.md](./contracts/dependency-governance.md), [contracts/generated-guidance.md](./contracts/generated-guidance.md), and [quickstart.md](./quickstart.md).

Design summary:

- `.template.config/template.json` defines a `fs-skia-ui` template with a `profile` choice supporting `default` and `minimal`, plus product identity symbols for project name, root namespace, package prefix, authors, repository URL, and target framework.
- The default profile preserves the governed framework surface expected by current repository conventions. The minimal starter profile includes only the core library, one basic sample, core tests, package checks, docs, and Spec Kit governance assets while excluding optional layout, charts, parity, and visual sample scope.
- `.template.package/FS.Skia.UI.Template.fsproj` packages the template-owned source into `artifacts/templates/FS.Skia.UI.Template.*.nupkg`.
- `TemplateCheck` installs and instantiates from both the source directory and the local packaged artifact, then validates default and minimal generated projects in isolated temp roots.
- Generated project validation includes placeholder scans, excluded-history scans, generated `Dev` verification, dependency governance checks, guidance checks, and drift output capture.
- `Directory.Packages.props` enables NuGet Central Package Management and owns direct dependency versions. Project files use versionless `PackageReference` entries except documented validation-only package smoke/version properties.
- `DependencyReport` writes dependency metadata and fails on unmanaged inline versions, missing owner/purpose/license/upgrade/preview-risk metadata, or undocumented validation-only exceptions.
- Generated specification and planning prompts are hardened in both core templates and preset-owned overrides so future products inherit package impact, public contract impact, state workflow impact, layout/rendering impact, evidence obligations, unsupported scope, template ownership, dependency impact, command-surface impact, generated project impact, and evidence path prompts.
- `TemplateDrift` scans template-owned diffs and fails unless matching template, docs, dependency policy, guidance, command-surface, or deferral updates are present.
- Root-level `readiness/template-deferrals.yml` records accepted drift deferrals with `id`, changed paths, rationale, owner, and target phase. Drift verification rejects records missing any required field.
- `Verify` and `Ci` are extended to include V2 validation targets while `Dev` remains the fast local restore/build/test path.

## Constitution Check - Post Design

- **Principle I**: PASS. Command, dependency, and guidance contracts define the public governance surface before implementation tasks.
- **Principle II**: PASS. No runtime public module is introduced by the design.
- **Principle III**: PASS. Template metadata, central package props, and FAKE target extensions are straightforward and avoid advanced F# features.
- **Principle IV**: PASS. New process/file work is represented through the existing build workflow model/effect/update/interpreter boundary.
- **Principle V**: PASS. Planned evidence is real generated output, real command execution, and real scan output.
- **Principle VI**: PASS. The design identifies failing-first tests for all V2 governance behavior.
- **Principle VII**: PASS. Every validation target has explicit readiness outputs and actionable failure responsibilities.
- **Engineering Constraints**: PASS. F#/.NET stack, local package output conventions, and deferred visual/release boundaries are preserved.
