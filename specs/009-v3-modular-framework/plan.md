# Implementation Plan: V3 Modular Framework

**Branch**: `009-v3-modular-framework` | **Date**: 2026-05-16 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/009-v3-modular-framework/spec.md`

## Summary

Deliver a V3 modular framework shape where generated products consume selected
compiled FS.Skia.UI capability packages instead of copying the framework
repository. The implementation introduces a capability catalog, package-owned
local agent skills, template base/fragments, generated product cleanliness
validation, full generated-product governance by default, and per-capability
surface/evidence reporting. The default generated app includes Scene,
SkiaViewer, Elmish, KeyboardInput, Layout, and Charts, but still excludes
framework samples, galleries, historical specs, framework docs, framework README
content, and framework-source maintenance checks. V2 migration support is out of
scope; the feature records compatibility impact but does not provide a V2
migration path.

## Technical Context

**Language/Version**: F# on .NET `net10.0`; existing FAKE `build.fsx`; Bash and Windows command wrappers; current solution and generated products remain SDK-style .NET projects  
**Primary Dependencies**: Existing Fable.Elmish, Silk.NET, SkiaSharp preview packages, Yoga.Net, YamlDotNet, Expecto, FAKE, and Spec Kit assets. Dependency ownership changes are expected because packages move from broad core ownership to capability ownership. No new third-party runtime dependency is required by the plan itself.  
**Storage**: Filesystem only: source projects, template base/fragments, capability catalog, local agent skills, generated product validation roots under `artifacts/`, surface baselines under `readiness/surface-baselines/`, and feature evidence under `specs/009-v3-modular-framework/readiness/`  
**Testing**: Expecto semantic tests per package, package surface baseline checks, generated product file-list tests, generated product `Dev`/`Test`/`Verify` runs, full product governance checks in generated products, template source/package validation, dependency reports, selected-skill copy reports, generated guidance checks, template drift, evidence graph, and evidence audit  
**Target Platform**: Windows and Linux developer/CI environments that can restore, build, test, pack, and instantiate generated projects. Native Skia/Vulkan smoke remains environment-aware and must distinguish implementation defects from missing GPU/window-system setup.  
**Project Type**: Governed F# framework and template repository with multiple packable libraries, samples, tests, Spec Kit assets, local agent skills, and generated product validation  
**Performance Goals**: Generated default product creation and non-visual `Dev` validation are observational, not gating, for this feature. Package split and generated-product validation must record observed command durations in readiness evidence, and reviewers decide whether follow-up performance work is needed.  
**Constraints**: V2 migration support is out of scope; generated products are consumers by default, not source forks; default generated app includes Scene, SkiaViewer, Elmish, KeyboardInput, Layout, and Charts; samples remain opt-in; generated products receive full product governance by default but not framework-source maintenance checks; `.fsi` remains the public contract owner for every public package; public surface baselines must be package-specific.  
**Scale/Scope**: One framework repository; current broad `src/Lib` package split into capability packages over staged implementation; existing `src/Layout` and `src/Charts` retargeted to Scene package ownership; template moves from copy-repo source selection to base/fragments; current default/minimal profiles replaced or superseded by `app`, `headless-scene`, `governed`, and `sample-pack` profiles.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: PASS. This is a template-owned Tier 1 change. It must update template composition, template docs, generated guidance, generated product validation, and template drift alignment. The plan replaces copy-repo template ownership with `template/base`, `template/fragments`, and `template/capabilities.yml`.
- **Dependency impact**: PASS. Package ownership changes require `Directory.Packages.props`, project references/package references, dependency docs, and `DependencyReport` evidence to be updated. The base Scene package must prove it has no Elmish, Silk.NET, SkiaSharp, Yoga.Net, or YamlDotNet dependency.
- **Command-surface impact**: PASS. Framework targets must grow capability, skill, generated-product cleanliness, and package surface validation. Generated products must include full product governance by default but exclude framework gallery, parity, template packaging, and framework-source maintenance checks.
- **Generated project impact**: PASS. Default generated app content changes materially. Validation must prove no framework samples, galleries, historical specs, framework readiness, framework docs, framework README copy, or framework implementation projects appear in default output.
- **Evidence paths**: PASS. Required readiness paths are listed under Project Structure and contracts. Evidence includes capability catalog, generated file lists, generated command logs, package surfaces, selected skills, dependency reports, guidance reports, drift reports, evidence graph, and evidence audit.
- **`.fsi` / contract impact**: PASS. This is an approved Tier 1 public contract change. Every public package must have curated `.fsi` files and package-specific surface baselines. Compatibility impact is documented; V2 migration support is explicitly out of scope by clarification.
- **MVU/effect boundary**: PASS. Elmish and viewer workflow ownership changes must keep stateful/I/O behavior behind explicit Model/Msg/Effect or Cmd boundaries. Generated product build/governance commands must keep the existing build MVU/effect boundary or an equivalent pure update plus edge interpreter.
- **Synthetic evidence**: PASS. Synthetic evidence is not required by the plan. If native viewer failure injection or generated product placeholders are used in implementation tests, they must be marked under the repository synthetic evidence policy and paired with real evidence where available.
- **Test evidence**: PASS. Failing-first tests are required for generated product cleanliness, capability catalog completeness, selected-skill copy behavior, package dependency separation, generated product full governance, and surface baselines.
- **Observability**: PASS. Validation failures must name missing capability metadata, unexpected generated paths, unrelated copied skills, dependency leaks, package surface drift, and generated-product governance gaps.
- **Deferred scope**: PASS. V2 migration support, dynamic plugin loading, new renderer backends, new platform support, release publishing automation, and full visual quality validation are deferred/out of scope.

### Constitution Gate Result

PASS with one explicit compatibility note: the constitution expects public API changes to document compatibility impact and migration guidance. This feature is a new V3 distribution shape and the user clarified that V2 migration implementation is not necessary. The plan therefore requires a compatibility-impact record with migration/non-migration guidance for existing consumers but forbids V2 migration implementation in this feature.

## Project Structure

### Documentation (this feature)

```text
specs/009-v3-modular-framework/
|-- spec.md
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   |-- capability-catalog.md
|   |-- generated-product-content.md
|   |-- local-agent-skills.md
|   |-- package-surface-and-dependencies.md
|   `-- governance-validation.md
|-- checklists/
|   `-- requirements.md
`-- readiness/
    |-- capability-catalog.md
    |-- generated-file-lists/
    |-- generated-product-verify/
    |-- selected-skills.md
    |-- package-surfaces/
    |-- dependency-report.md
    |-- generated-guidance.md
    |-- template-drift.md
    |-- evidence-graph.md
    |-- evidence-audit.md
    `-- compatibility-impact.md
```

### Source Code (repository root)

```text
src/
|-- Scene/
|   |-- Scene.fsproj
|   |-- *.fsi
|   |-- *.fs
|   `-- skill/SKILL.md
|-- SkiaViewer/
|   |-- SkiaViewer.fsproj
|   |-- *.fsi
|   |-- *.fs
|   `-- skill/SKILL.md
|-- Elmish/
|   |-- Elmish.fsproj
|   |-- *.fsi
|   |-- *.fs
|   `-- skill/SKILL.md
|-- KeyboardInput/
|   |-- KeyboardInput.fsproj
|   |-- *.fsi
|   |-- *.fs
|   `-- skill/SKILL.md
|-- Layout/
|   |-- Layout.fsproj
|   |-- *.fsi
|   |-- *.fs
|   `-- skill/SKILL.md
|-- Charts/
|   |-- Charts.fsproj
|   |-- *.fsi
|   |-- *.fs
|   `-- skill/SKILL.md
`-- Testing/
    |-- Testing.fsproj
    |-- *.fsi
    |-- *.fs
    `-- skill/SKILL.md

template/
|-- capabilities.yml
|-- base/
|-- fragments/
|   |-- scene/
|   |-- skiaviewer/
|   |-- elmish/
|   |-- keyboard-input/
|   |-- layout/
|   |-- charts/
|   |-- full-governance/
|   `-- samples/
`-- profiles/
    |-- app.yml
    |-- headless-scene.yml
    |-- governed.yml
    `-- sample-pack.yml

tests/
|-- Scene.Tests/
|-- SkiaViewer.Tests/
|-- Elmish.Tests/
|-- KeyboardInput.Tests/
|-- Layout.Tests/
|-- Charts.Tests/
|-- Testing.Tests/
|-- Package.Tests/
`-- Governance.Tests/
```

**Structure Decision**: Move toward capability-owned source directories and template fragments while preserving working repository validation at every stage. The implementation may use compatibility project names temporarily if needed, but the accepted end state is capability packages with package-specific `.fsi`, tests, skills, and surface baselines. Generated products are created from template base/fragments and package references, not copied framework implementation projects.

## Complexity Tracking

The only constitution tension is the deliberate exclusion of V2 migration support. This is accepted because the user clarified scope and because the feature is a V3 distribution design rather than a supported upgrade workflow. A compatibility-impact readiness record is still required.

## Phase 0: Research

See [research.md](./research.md). Decisions cover capability catalog ownership, package boundary order, default app capability set, full product governance by default, generated project content policy, local skill packaging, package surface baselines, sample-pack handling, and V2 migration exclusion.

## Phase 1: Design & Contracts

See [data-model.md](./data-model.md), [contracts/capability-catalog.md](./contracts/capability-catalog.md), [contracts/generated-product-content.md](./contracts/generated-product-content.md), [contracts/local-agent-skills.md](./contracts/local-agent-skills.md), [contracts/package-surface-and-dependencies.md](./contracts/package-surface-and-dependencies.md), [contracts/governance-validation.md](./contracts/governance-validation.md), and [quickstart.md](./quickstart.md).

Design summary:

- `template/capabilities.yml` becomes the source of truth for capability name, package, project, public contracts, tests, skill path, template fragment, dependencies, generated-product defaults, and evidence classes.
- Capability packages are split so Scene is the dependency-light base package, SkiaViewer owns native host rendering, Elmish owns the Elmish adapter, KeyboardInput owns input configuration/reducer/display, Layout owns Yoga-backed layout and graph support, Charts owns chart/DataGrid builders, and Testing owns generated-product/package helper APIs.
- The default generated app includes Scene, SkiaViewer, Elmish, KeyboardInput, Layout, and Charts, plus full product governance, selected local skills, one product app, and one product test suite.
- Generated products must not include framework samples/galleries, historical specs/readiness, framework docs, framework README copy, or framework implementation projects.
- Samples are supplied only through an explicit sample profile or sample-pack selection.
- Generated products receive full product governance by default, including evidence gates, drift checks, generated guidance checks, and readiness workflow. They do not run framework-source maintenance checks.
- Every public capability package has `.fsi` contracts and package-specific surface baselines.
- V2 migration support is out of scope. A compatibility-impact record explains the break and records that no V2 migration path is provided by this feature.

## Constitution Check - Post Design

- **Principle I - Spec -> FSI -> Semantic Tests -> Implementation**: PASS. The plan is Tier 1 and requires package-specific `.fsi`, semantic tests, generated product checks, surface baselines, and evidence before implementation can be merge-ready.
- **Principle II - Visibility Lives in `.fsi`**: PASS. Every public package requires curated `.fsi` files and package-specific surface baselines.
- **Principle III - Idiomatic Simplicity**: PASS. The design uses plain package boundaries, YAML/structured catalog data, FAKE targets, and Expecto tests. No advanced F# feature is required by the plan.
- **Principle IV - Elmish/MVU Boundary**: PASS. Elmish, viewer, keyboard input, and generated-product governance workflows must keep state and I/O behind explicit model/message/effect or equivalent build MVU boundaries.
- **Principle V - Synthetic Evidence Disclosure**: PASS. Synthetic evidence is not planned as the primary proof. Any native failure injection or placeholder/generated fixture evidence must be disclosed and paired with real evidence where available.
- **Principle VI - Test Evidence Is Mandatory**: PASS. Contracts require failing-first tests for catalog completeness, generated project content, selected skill copying, package surfaces, dependencies, and generated product governance.
- **Principle VII - Observability and Safe Failure**: PASS. Validation failures must be structured and actionable for missing metadata, unexpected generated paths, dependency leaks, package surface drift, and governance gaps.
- **Change Classification**: PASS. Tier 1 public package/template/governance change. Compatibility impact is documented; V2 migration implementation remains out of scope.
- **Engineering Constraints**: PASS. F#/.NET, `.fsi` contracts, MVU boundaries, package baseline evidence, central dependency governance, and FAKE command surface remain governing constraints.
