---
title: V3 Design
category: Design
categoryindex: 4
index: 8
description: Modular package, skill, and lean template design for the next FS.Skia.UI generation.
---

# V3 Design

V3 should turn FS.Skia.UI from a governed framework repository that is copied
into new projects into a modular framework distribution that new projects
consume. Each reusable capability should be a compiled, packable F# library
with its own public `.fsi` contract, tests, package identity, and local agent
skill. The `dotnet new` template should generate a clean application shell that
references selected framework packages. It should not copy samples, galleries,
parity code, historical readiness artifacts, framework documentation, or the
framework repository README into a new product.

## Goals

The V3 design has five goals:

| Goal | Meaning |
|------|---------|
| Modular packages | Runtime capabilities ship as independently packable libraries instead of one broad core package. |
| Local skills by capability | Agent workflows live beside the capability they govern and are copied only when that capability is selected. |
| Lean generated projects | `dotnet new` creates product code, product tests, governance hooks, and package references, not a framework clone. |
| Optional framework surface | Applications opt into Skia viewer, Elmish integration, keyboard input, layout, charts, and samples explicitly. |
| Stronger ownership | Each package has its own contract, tests, smoke target, docs, dependency policy, and template fragment. |

This is a V3 direction, not a change to the current
`008-targeted-refactor-governance` feature. The current feature should finish
its bounded internal refactor and public-surface stability work first. V3 is
allowed to change package boundaries because that is the point of the design.

## Core Decision

The source repository remains the framework development repository. Generated
projects become consumers of the framework.

In V2, the template profiles still copy large parts of this repository. That is
useful for proving governance, but it makes a new project inherit framework
samples, framework docs, framework test suites, and implementation details that
most application teams do not need.

In V3, the default template should create this kind of product:

```text
.
|-- .agents/
|   `-- skills/
|       |-- fs-skia-project/
|       |-- fs-skia-elmish/
|       `-- fs-skia-skiaviewer/
|-- .config/
|   `-- dotnet-tools.json
|-- .specify/
|   |-- memory/
|   |-- presets/
|   `-- templates/
|-- docs/
|   `-- product.md
|-- src/
|   `-- MyProduct/
|       |-- MyProduct.fsproj
|       `-- Program.fs
|-- tests/
|   `-- MyProduct.Tests/
|       |-- MyProduct.Tests.fsproj
|       `-- Tests.fs
|-- Directory.Build.props
|-- Directory.Packages.props
|-- MyProduct.sln
|-- README.md
|-- build.fsx
|-- fake.cmd
`-- fake.sh
```

The generated `README.md` should describe the generated product, its commands,
and its selected framework packages. It should not be the FS.Skia.UI framework
README. Framework architecture documents stay in the framework repository and
published documentation site. A generated project may link to them, but should
not carry a local copy by default.

## Package Model

Every reusable part with runtime API should be a compiled library. Template
fragments and samples may exist, but they are not the primary distribution unit
for runtime behavior.

| Package | Current source | V3 responsibility | Dependencies |
|---------|----------------|-------------------|--------------|
| `FS.Skia.UI.Scene` | `src/Lib/Library.fsi` scene subset | Immutable scene, paint, geometry, image, text, effects, diagnostics primitives, and pure render descriptions. | `FSharp.Core` |
| `FS.Skia.UI.SkiaViewer` | `src/Lib/Library.fs` viewer and Vulkan code | Persistent window host, generated app host, Vulkan/Skia startup, frame rendering, screenshots, viewer diagnostics, shutdown, and bounded platform smoke helpers. | `FS.Skia.UI.Scene`, Silk.NET, SkiaSharp |
| `FS.Skia.UI.Elmish` | `src/Lib/Library.fsi` Elmish viewer subset | Elmish program adapter, event mapper, effect mapper, subscriptions, and `ViewerProgram` construction helpers. | `FS.Skia.UI.Scene`, `FS.Skia.UI.SkiaViewer`, `Fable.Elmish` |
| `FS.Skia.UI.KeyboardInput` | `src/Lib/KeyboardInput.*` | Keyboard configuration, command registry, runtime reducer, replay, bigram analysis, and optional state display scenes. | `FS.Skia.UI.Scene`, `YamlDotNet` |
| `FS.Skia.UI.Layout` | `src/Layout` | Yoga-backed layout, computed bounds, hit testing, layout diagnostics, and graph layout support if graph remains coupled. | `FS.Skia.UI.Scene`, `Yoga.Net` |
| `FS.Skia.UI.Charts` | `src/Charts` | Chart and DataGrid scene builders, hit testing, and finite data rendering helpers. | `FS.Skia.UI.Scene` |
| `FS.Skia.UI.Testing` | selected `tests` helpers | Contract smoke helpers, generated app assertions, surface baseline utilities, and package consumer checks. | Selected packages only |

The important split is `Scene` from host/runtime adapters. A product that only
wants to build reusable widgets should not need Silk.NET, Vulkan, Skia native
assets, or Elmish. A product that wants Elmish should opt into the Elmish
adapter. A product that wants direct host control should be able to use
`FS.Skia.UI.SkiaViewer` without inheriting keyboard input or charts.

The package graph should stay acyclic:

```text
FS.Skia.UI.Charts        --> FS.Skia.UI.Scene
FS.Skia.UI.Layout        --> FS.Skia.UI.Scene
FS.Skia.UI.KeyboardInput --> FS.Skia.UI.Scene

FS.Skia.UI.SkiaViewer    --> FS.Skia.UI.Scene
FS.Skia.UI.Elmish        --> FS.Skia.UI.Scene
FS.Skia.UI.Elmish        --> FS.Skia.UI.SkiaViewer

FS.Skia.UI.Testing       --> selected packages
```

`FS.Skia.UI.Scene` must not depend on Elmish, Silk.NET, SkiaSharp, Yoga.Net, or
YamlDotNet. This keeps the base vocabulary cheap to reference and makes the
rest of the framework genuinely optional.

## Skill Model

Each package should own a local agent skill. A skill is not compiled into the
library, but it is versioned, reviewed, and validated with the package it
governs.

Recommended source layout:

```text
src/
|-- Scene/
|   |-- Scene.fsproj
|   |-- ...
|   `-- skill/SKILL.md
|-- SkiaViewer/
|   |-- SkiaViewer.fsproj
|   |-- ...
|   `-- skill/SKILL.md
|-- Elmish/
|   |-- Elmish.fsproj
|   |-- ...
|   `-- skill/SKILL.md
|-- KeyboardInput/
|   |-- KeyboardInput.fsproj
|   |-- ...
|   `-- skill/SKILL.md
|-- Layout/
|   |-- Layout.fsproj
|   |-- ...
|   `-- skill/SKILL.md
`-- Charts/
    |-- Charts.fsproj
    |-- ...
    `-- skill/SKILL.md
```

At template generation time, selected skills are copied into root
`.agents/skills/`:

```text
.agents/skills/
|-- fs-skia-project/
|-- fs-skia-scene/
|-- fs-skia-skiaviewer/
|-- fs-skia-elmish/
|-- fs-skia-keyboard-input/
|-- fs-skia-layout/
`-- fs-skia-charts/
```

Each package skill should contain:

| Section | Purpose |
|---------|---------|
| Scope | Which files, packages, and generated project paths the skill owns. |
| Public contract | Which `.fsi` files define the supported API and how to check surface changes. |
| Build commands | The smallest FAKE targets or `dotnet` commands needed for that package. |
| Test commands | Package-specific semantic tests, smoke tests, and generated product checks. |
| Evidence rules | Required readiness output when the package changes. |
| Agent guidance | How to add features without crossing package boundaries. |

For example, the Skia viewer skill should tell an agent to inspect
`FS.Skia.UI.SkiaViewer` contracts, run native startup cleanup tests, disclose
synthetic native evidence, and keep window/GPU lifetime at the host edge. The
keyboard input skill should tell an agent to update command registry tests,
YAML fixtures, replay tests, and state display scene checks. The Elmish skill
should keep `Model`, `Msg`, `init`, `update`, `view`, event mapping, and effect
mapping separated from host-side side effects.

## Capability Manifest

V3 should add a machine-readable capability manifest so packages, template
fragments, skills, docs, tests, and build targets do not drift apart.

Suggested shape:

```yaml
capabilities:
  skiaviewer:
    package: FS.Skia.UI.SkiaViewer
    project: src/SkiaViewer/SkiaViewer.fsproj
    skill: src/SkiaViewer/skill/SKILL.md
    templateFragment: template/fragments/skiaviewer
    tests:
      - tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj
    smokeTargets:
      - SkiaViewerSmoke
    dependencies:
      - scene
  elmish:
    package: FS.Skia.UI.Elmish
    project: src/Elmish/Elmish.fsproj
    skill: src/Elmish/skill/SKILL.md
    templateFragment: template/fragments/elmish
    tests:
      - tests/Elmish.Tests/Elmish.Tests.fsproj
    dependencies:
      - scene
      - skiaviewer
```

The manifest should drive template composition and validation. If a capability
declares a package, it must declare a skill, tests, a docs entry, and package
dependencies. `TemplateCheck` should fail when a selected capability cannot
generate, restore, build, and run its minimal tests from both source and packed
template paths.

## Template Profiles

The default V3 template should be lean. It should generate a product that can
run immediately, but it should not generate galleries or framework internals.

Recommended profiles:

| Profile | Purpose | Includes |
|---------|---------|----------|
| `app` | Default product starter. | One app project, one test project, `Scene`, `SkiaViewer`, `Elmish`, product README, selected skills, light Spec Kit governance. |
| `headless-scene` | Library/widget authoring. | One library project, tests, `Scene`, optional `Charts` or `Layout`, no viewer host. |
| `full-governed` | Teams that want the current governance weight. | Product app, tests, Spec Kit templates, evidence gates, drift checks, selected skills. |
| `sample-pack` | Optional examples outside product template. | Gallery apps and smoke projects for framework learning or regression checks. |

Recommended switches:

```bash
dotnet new fs-skia-ui \
  --name MyProduct \
  --profile app \
  --with-skiaviewer true \
  --with-elmish true \
  --with-keyboard-input false \
  --with-layout false \
  --with-charts false \
  --with-samples false \
  --governance light
```

Samples should default to `false`. When samples are requested, they should come
from a sample template or a separate `fs-skia-ui-samples` package so product
repositories do not confuse framework demos with application code.

## Generated Project Content Policy

The V3 template should include only files that a new product is expected to
edit or run.

Include by default:

- Product solution, app project, and focused test project.
- `Directory.Build.props` and `Directory.Packages.props` with selected package
  references.
- FAKE wrappers and a small `build.fsx` with `Dev`, `Test`, `Verify`, and
  `Pack` targets relevant to the generated project.
- Product README with commands and selected framework package list.
- Minimal `docs/product.md` for product architecture notes.
- Project-level Spec Kit constitution and templates when governance is enabled.
- Project skill plus selected capability skills.

Exclude by default:

- Framework samples and galleries.
- Framework parity tests, package surface baselines, historical readiness
  folders, and synthetic evidence logs.
- Framework documentation set, including architecture, V2 analysis, subsystem
  design, dependency policy, and template profile docs.
- Framework root README content.
- Framework implementation projects unless `--source-framework true` is an
  explicit development option.
- Ancillary scripts that exist only to maintain the framework repository.

The generated product may still have a link table pointing to the framework
documentation site or source repository. That keeps the project clean while
making framework help discoverable.

### Persistent Viewer Migration

Generated apps that previously used only bounded viewer smoke, first-frame,
frame-count, or scene metadata paths must choose one of these migration paths:

- adopt the persistent generated host and make `Viewer.runApp viewerOptions generatedHost` the default executable path
- declare the product headless or non-interactive in its spec, task list, and readiness evidence
- record the missing persistent viewer capability as a blocking product/package gap

Bounded-only generated apps must not claim interactive graphical readiness.
Unsupported-host diagnostics can explain local execution limits, but they do not
replace a supported-host persistent launch artifact.

## Framework Repository Shape

The source repository should remain broader than generated projects because it
owns framework development, samples, template composition, and regression
coverage.

Recommended V3 source shape:

```text
.
|-- src/
|   |-- Scene/
|   |-- SkiaViewer/
|   |-- Elmish/
|   |-- KeyboardInput/
|   |-- Layout/
|   |-- Charts/
|   `-- Testing/
|-- tests/
|   |-- Scene.Tests/
|   |-- SkiaViewer.Tests/
|   |-- Elmish.Tests/
|   |-- KeyboardInput.Tests/
|   |-- Layout.Tests/
|   |-- Charts.Tests/
|   |-- Package.Tests/
|   `-- Governance.Tests/
|-- samples/
|   |-- BasicViewer/
|   |-- KeyboardInputGallery/
|   |-- LayoutGallery/
|   `-- ChartsGallery/
|-- template/
|   |-- base/
|   |-- fragments/
|   `-- capabilities.yml
|-- docs/
|-- scripts/
|-- .agents/
|   `-- skills/
`-- .specify/
```

Samples stay in the framework repository because they are useful regression and
learning assets. They should not be copied into product repositories unless a
user asks for a sample pack.

## Build And Validation

The V3 build should validate both package modularity and generated project
cleanliness.

Recommended targets:

| Target | Responsibility |
|--------|----------------|
| `Dev` | Restore, build, and test changed framework packages. |
| `Verify` | Run all package tests, governance tests, package surface checks, docs checks, and template checks. |
| `PackFramework` | Pack all selected framework libraries with dependency metadata. |
| `CapabilityCheck` | Validate each capability manifest entry has package, skill, tests, docs, and template fragment. |
| `TemplateCheck` | Generate lean app, headless-scene, full-governed, and sample-pack variants from source and package template paths. |
| `GeneratedProjectCheck` | Assert generated product file lists exclude framework samples, framework docs, historical specs, and framework README text. |
| `SkillCheck` | Parse every capability skill and ensure it names owned files, tests, commands, and evidence rules. |

The generated product's own build should be much smaller:

```bash
./fake.sh build -t Dev
./fake.sh build -t Test
./fake.sh build -t Verify
```

`Verify` in a generated product should validate the product and selected
framework package usage. It should not run framework gallery, parity, package
surface, or template packaging tests unless the product explicitly opts into
framework-source development.

## Public API Policy

V3 package splitting will create breaking package boundaries even if many type
names remain the same. The design should treat that honestly:

- Keep `.fsi` files as the authoritative public contract for every package.
- Prefer namespace moves that make ownership obvious, such as
  `FS.Skia.UI.Scene`, `FS.Skia.UI.SkiaViewer`, and
  `FS.Skia.UI.KeyboardInput`.
- Provide migration documentation from V2 package names to V3 package names.
- Add compatibility shims only when they are cheap and do not keep unwanted
  dependencies alive.
- Keep dependency-heavy adapters out of base packages.
- Publish package surface baselines per package, not only for the aggregate
  framework.

The base scene package must be the most stable API. Viewer, Elmish, keyboard,
layout, and charts can evolve more independently if they do not leak into the
base package.

## Migration Plan

V3 should be delivered as a staged feature set.

1. Define `template/capabilities.yml` and add `CapabilityCheck`.
2. Split `FS.Skia.UI.Scene` from the current core library behind stable `.fsi`
   contracts.
3. Move host runtime into `FS.Skia.UI.SkiaViewer` and keep Vulkan/Skia native
   ownership tests with that package.
4. Move Elmish adapters into `FS.Skia.UI.Elmish` so the base scene package no
   longer depends on `Fable.Elmish`.
5. Move keyboard input into `FS.Skia.UI.KeyboardInput`, with its own skill,
   tests, YAML fixtures, and package references.
6. Keep layout and charts as separate packages, but retarget them to
   `FS.Skia.UI.Scene` instead of the old broad core package.
7. Add package-owned skills and `SkillCheck`.
8. Replace copy-the-repo template sources with base and capability fragments.
9. Change the default template to `app`, with samples excluded by default.
10. Add migration docs and generated project cleanliness tests.

The first implementation should not try to preserve every V2 template profile.
It is better to introduce one excellent lean `app` profile and one optional
sample pack than to carry the current default/minimal split forward unchanged.

## Acceptance Criteria

V3 is ready when these conditions are true:

- A generated default app contains no `samples/` directory, no framework docs
  set, no historical `specs/`, and no framework README copy.
- The generated app references framework packages rather than copying framework
  implementation projects.
- `FS.Skia.UI.Scene` builds without Elmish, Silk.NET, SkiaSharp, Yoga.Net, or
  YamlDotNet package references.
- Skia viewer, Elmish, keyboard input, layout, and charts each have a packable
  project, `.fsi` contracts, semantic tests, and a local agent skill.
- Template generation copies only the skills for selected capabilities.
- Generated app `Dev` and `Verify` targets run without framework gallery or
  template packaging tests.
- Framework `TemplateCheck` proves generated products are lean from both source
  and packed template paths.
- Package surface baselines exist per public package.
- Migration docs explain how to move V2 applications to V3 package references.

## Non-Goals

V3 should not become a plugin runtime, a dynamic package loader, or a general UI
framework rewrite. The goal is modular distribution and cleaner project
generation, not a new rendering architecture. Vulkan/Skia host work can remain
Vulkan/Skia host work, but it should live in `FS.Skia.UI.SkiaViewer` instead of
being bundled with scene primitives, keyboard input, and Elmish adapters.

V3 also should not remove governance. It should make governance proportional.
The framework repository keeps heavy checks. Generated products get the checks
and skills that match the capabilities they selected.
