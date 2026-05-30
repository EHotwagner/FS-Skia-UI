# FS-Skia-UI Template Framework Proposal

Date: 2026-05-14

Status: proposal for turning this repository into a governed template framework for new F# / Elmish / Skia products.

## Executive Recommendation

FS-Skia-UI should become a template framework, but not by turning the runtime library into a broad application framework or by asking agents to copy patterns manually. The strongest shape is a governed project family with four explicit mechanisms:

1. `dotnet new` creates the initial repository shape.
2. FAKE owns the stable command surface for humans, agents, and CI.
3. Spec Kit presets and templates define artifact shape, methodology, and required evidence.
4. Spec Kit extensions and project-local skills run gates that verify contracts, evidence, layout, visuals, packages, and template drift.

The highest-value next step is still a canonical FAKE build graph, but the proposal should be sharper than "add FAKE". The build graph must become the single source of truth for restore, build, test, pack, smoke, documentation, evidence, template installation, and generated baseline workflows. CI and Speckit commands should call FAKE targets instead of recreating command order.

Recommended sequencing:

1. Add FAKE and central package governance.
2. Move evidence and surface checks behind FAKE targets.
3. Strengthen the Speckit preset so generated specs, plans, and tasks carry the repo's UI-specific obligations.
4. Add `dotnet new` template packaging and a `TemplateCheck` target.
5. Add layout, visual, package, dependency, and template-drift extensions/gates.

This order matters. A template package is much more valuable after the build and evidence behavior is already executable.

## Research Basis

This analysis combines local repository inspection with primary upstream documentation:

- Local plan: `specs/005-add-yoga-net-layout/plan.md`
- Local constitution: `.specify/memory/constitution.md`
- Local preset: `.specify/presets/fsharp-opinionated/preset.yml`
- Local evidence extension: `.specify/extensions/evidence/extension.yml`
- Local projects, tests, samples, scripts, and package references under `src/`, `tests/`, `samples/`, and `scripts/`
- Spec Kit presets: https://github.github.com/spec-kit/reference/presets.html
- Spec Kit extensions: https://github.github.com/spec-kit/reference/extensions.html
- Spec Kit spec-driven model: https://github.com/github/spec-kit/blob/main/spec-driven.md
- `dotnet new` custom templates: https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates
- `template.json` symbols and conditions: https://github.com/dotnet/templating/wiki/Reference-for-template.json
- FAKE build automation: https://fake.build/guide/what-is-fake.html
- FAKE bootstrap template: https://fake.build/guide/fake-template.html
- NuGet Central Package Management: https://learn.microsoft.com/en-gb/nuget/consume-packages/central-package-management
- NuGet lock files and locked restore: https://learn.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files
- Paket lock files: https://fsprojects.github.io/Paket/lock-file.html
- Elmish MVU model: https://elmish.github.io/elmish/
- Expecto test model: https://github.com/haf/expecto
- SkiaSharp `SKCanvas` and `SKSurface`: https://learn.microsoft.com/en-us/dotnet/api/skiasharp.skcanvas and https://learn.microsoft.com/en-us/dotnet/api/skiasharp.sksurface
- Yoga layout boundary: https://www.yogalayout.dev/docs/about-yoga
- Yoga measure callbacks: https://www.yogalayout.dev/docs/advanced/external-layout-systems
- Yoga.Net repository: https://github.com/chenrensong/Yoga.Net
- Yoga.Net NuGet metadata from `https://api.nuget.org/v3-flatcontainer/yoga.net/index.json` and `https://api.nuget.org/v3-flatcontainer/yoga.net/3.2.3/yoga.net.nuspec`

Key external findings:

- Spec Kit presets are the right mechanism for artifact shape because they override templates, commands, and terminology, and can be stacked by priority.
- Spec Kit extensions are the right mechanism for new commands, integrations, and quality gates.
- `dotnet new` templates can be created from normal source folders by adding `.template.config/template.json`; `sourceName` can replace names in file paths and contents; templates can be installed from a directory or a NuGet package.
- FAKE is a cross-platform F# build automation DSL. Its template creates `build.fsx`, `fake.sh`, and `fake.cmd`, which gives this project a typed, repo-local command surface.
- NuGet Central Package Management fits the current SDK-style project shape and removes repeated package versions from individual `.fsproj` files.
- NuGet lock files are useful for application roots and CI locked restore, but Microsoft guidance warns against treating common library lock files as authoritative for downstream consumers.
- Paket gives a stronger repository-wide `paket.lock` story, but adopting it would be a bigger workflow change than Central Package Management.
- Elmish explicitly separates immutable `Model`, discriminated-union `Message`, `Command`, pure `init`, pure `update`, and pure `view`; that matches this repo's constitution.
- SkiaSharp's own type boundaries support keeping `SKSurface` and `SKCanvas` at the host/render edge rather than in product view functions.
- Yoga is only responsible for box size and position, not drawing. Yoga measure callbacks are designed for text or externally measured leaf content. Yoga.Net 3.2.3 currently lists `net8.0`, `net9.0`, and `net10.0` target groups, MIT licensing, no package dependencies, and a GitHub repository reference.

## Local Baseline

The repository already has a strong seed for a governed product family:

- `src/Lib`, `src/Layout`, and `src/Charts` are packable F# projects.
- Public modules generally use paired `.fsi` and `.fs` files.
- `Directory.Build.props` centralizes `TargetFramework`, `LangVersion`, warnings-as-errors, preview package metadata, and the FS0078 visibility rule.
- `tests/*` includes semantic package tests, layout tests, parity tests, smoke tests, chart tests, and public surface tests.
- `samples/*` already act like gallery and smoke hosts.
- `scripts/*.fsx` already capture FSI prelude, package surface, input, parity, and layout evidence workflows.
- `.specify/memory/constitution.md` already encodes FSI-first development, MVU boundaries, synthetic evidence disclosure, mandatory tests, and safe diagnostics.
- `.specify/presets/fsharp-opinionated` proves that project-local Spec Kit behavior can be customized.
- `.specify/extensions/evidence` proves that task DAG and synthetic-evidence gates can be added.
- Existing feature directories under `specs/*/readiness` already model evidence folders with logs, transcripts, surface baselines, sample smoke output, and task graphs.

The current shape is less like a normal "starter app" and more like a governed library/product template. New projects should inherit the governance, not only the code layout.

## Current Source Links

This proposal was written as an implementation direction. The current source
files that now carry those concerns are:

| Concern | Current source |
|---------|----------------|
| Build graph and V2 targets | [build.fsx](../build.fsx), [fake.sh](../fake.sh), [fake.cmd](../fake.cmd) |
| Template metadata and package | [.template.config/template.json](../.template.config/template.json), [.template.package/FS.Skia.UI.Template.fsproj](../.template.package/FS.Skia.UI.Template.fsproj) |
| Central package versions | [Directory.Packages.props](../Directory.Packages.props) |
| Core scene, viewer, diagnostics | [src/Lib/Library.fsi](../src/Lib/Library.fsi), [src/Lib/Library.fs](../src/Lib/Library.fs) |
| Keyboard input | [src/Lib/KeyboardInput.fsi](../src/Lib/KeyboardInput.fsi), [src/Lib/KeyboardInput.fs](../src/Lib/KeyboardInput.fs) |
| Charts and DataGrid | [src/Charts](../src/Charts/) |
| Layout and graph | [src/Layout](../src/Layout/) |
| Governance scripts | [scripts/dependency-report.fsx](../scripts/dependency-report.fsx), [scripts/template-drift.fsx](../scripts/template-drift.fsx), [scripts/refresh-surface-baselines.fsx](../scripts/refresh-surface-baselines.fsx) |
| Tests | [tests](../tests/) |
| Surface baselines | [readiness/surface-baselines](../readiness/surface-baselines/) |
| Spec Kit assets | [.specify](../.specify/) |

## Current Gaps

The proposal should explicitly close these gaps:

1. There is no canonical build graph.

   The repository has `FS-Skia-UI.sln`, scripts, tests, samples, and evidence artifacts, but no `build.fsx`, `fake.sh`, or `fake.cmd`. Agents and CI currently have to infer command order.

2. Package versions are repeated in project files.

   `Expecto`, `Microsoft.NET.Test.Sdk`, `YoloDev.Expecto.TestSdk`, Silk.NET packages, SkiaSharp preview packages, Yoga.Net, YamlDotNet, and sample package validation versions are declared in individual `.fsproj` files. This is workable for one repo, but weak for a reusable template.

3. Surface baselines are anchored to a feature directory.

   `tests/Package.Tests/SurfaceAreaTests.fs` reads baselines from `specs/002-skia-feature-parity/readiness/surface-baselines`. That is acceptable historical evidence, but it is not a template-owned baseline location. A template project needs a stable current baseline path, with feature readiness folders storing feature-specific evidence.

4. The Speckit preset does not yet own enough generated artifact shape.

   The current preset provides constitution and task behavior. It does not yet replace the base spec and plan templates with UI-specific fields such as package impact, FSI impact, MVU impact, layout impact, Skia/native-resource impact, visual evidence, package impact, and build target impact.

5. The evidence extension is necessary but not sufficient.

   It validates `tasks.deps.yml` and synthetic propagation. The template also needs contract, layout, visual, package, dependency, build-health, and template-drift gates.

6. There is no `dotnet new` template metadata.

   No `.template.config/template.json` exists, so the repo cannot yet be installed, instantiated, smoke-tested, or packed as a template package.

7. Samples are useful but not uniformly contract-smokeable.

   Several samples support package-reference validation through `FsSkiaUiPackageVersion`, while others use project references. The template should define a uniform sample contract: every sample either has a non-interactive `--contract-smoke` path or is explicitly marked visual/manual.

8. Documentation is not yet a maintained docs set.

   `docs/` currently contains this analysis file only. A template framework needs persistent docs for profile, architecture, build, testing, Speckit, dependencies, evidence, rendering boundaries, layout boundaries, and ADRs.

## Design Decision Summary

| Decision | Recommendation | Rationale |
|---|---|---|
| Initial repo creation | Use `dotnet new` | It is the native .NET template mechanism, supports normal source folders, `template.json`, `sourceName`, symbols, optional content, installation from directories, and NuGet template packaging. |
| Incremental scaffolding | Use FAKE scaffold targets | `dotnet new` is good for first creation; FAKE is better for adding a package, test project, sample, baseline, or readiness folder to an existing repo. |
| Build orchestration | Use FAKE as canonical | FAKE is F#, typed enough for maintainable build logic, cross-platform, and can expose stable targets to agents and CI. |
| Package version governance | Start with NuGet Central Package Management | It fits SDK-style projects and removes duplicated versions. Paket remains a later option if repo-wide transitive lock behavior becomes more important than simpler SDK-native restore. |
| Lock policy | Use locked restore for app/template checks, not as the library contract | NuGet lock files are strongest at application roots. For packable library projects, lock files should be evidence for this repo's verification, not a promise to downstream consumers. |
| Spec Kit artifact shape | Use presets | Presets should own spec, plan, task, constitution, and command text. |
| Spec Kit executable gates | Use extensions | Extensions should add contract, layout, visual, build-health, template-drift, and release-pack commands. |
| Agent behavior | Use project-local skills | Skills are the right layer for agent workflows that call the above targets and enforce local conventions. |
| MVU and rendering boundary | Keep view pure and declarative | Elmish and the local constitution both point to pure state transitions and declarative views. Skia native resources stay at the host edge. |
| Yoga boundary | Keep Yoga.Net internal to `src/Layout` | Public APIs should be idiomatic F# records, unions, options, and results. Yoga.Net objects, callbacks, node lifetimes, and disposal remain implementation details. |

## Target Architecture

The template should make the rendering and layout pipeline explicit:

```text
Application model
  -> update Msg Model
  -> view Model dispatch
  -> declarative scene/layout/widget tree
  -> layout evaluation in logical coordinates
  -> render planning
  -> Skia host draws through SKSurface/SKCanvas
  -> input/hit-test reports messages back to update
```

Layer responsibilities:

| Layer | Owns | Must not own |
|---|---|---|
| Application/MVU | `Model`, `Msg`, `init`, `update`, effects, subscriptions | `SKCanvas`, GPU handles, long-lived native resources |
| View declaration | Pure functions from model to scene/layout/widget data | I/O, mutable host state, direct drawing as the default path |
| Layout | Logical bounds, flex intent, measurement contracts, diagnostics, invalidation | Renderer state, GPU resources, product workflow state |
| Render planning | Conversion from computed tree to render commands | Window lifetime, dependency restore, feature specs |
| Skia host | Window loop, Vulkan/GPU setup, `SKSurface`, `SKCanvas`, screenshots, disposal | Domain decisions, business state transitions |
| Evidence | FSI transcripts, baselines, smoke output, screenshots, performance reports | Hidden manual proof that cannot be regenerated |
| Governance | Constitution, Speckit preset, extensions, task graph, template drift | Runtime product code |

This separation prevents two common failure modes:

- Feature code bypasses the UI framework by drawing directly in samples.
- Feature code recreates local layout/render/test/build conventions instead of using the template.

## Template-Owned vs Product-Owned

The template should freeze framework shape, not product history.

Template-owned:

- `FS-Skia-UI.sln` shape and project naming conventions.
- `Directory.Build.props`.
- `Directory.Packages.props`.
- Optional `NuGet.config` for local package source conventions.
- `.config/dotnet-tools.json` for FAKE, Fantomas if adopted, and other local tools.
- `build.fsx`, `fake.sh`, and `fake.cmd`.
- `src/Lib`, `src/Layout`, optional `src/Charts`, and optional `src/Widgets` package skeletons.
- Canonical test projects: `Lib.Tests`, `Layout.Tests`, `Smoke.Tests`, `Package.Tests`, optional `Charts.Tests`, optional `Parity.Tests`.
- Canonical samples: `BasicViewer`, `InteractiveViewer`, `ScreenshotGallery`, optional feature galleries.
- `scripts/prelude.fsx`, `scripts/refresh-surface-baselines.fsx`, and evidence scripts.
- `.specify/memory/constitution.md`.
- `.specify/presets/fsharp-opinionated` or a renamed template preset.
- `.specify/extensions/evidence` plus new project-local extensions.
- `.agents/skills` for local Speckit and Git workflows.
- `docs/reports/template-profile.md`, `docs/reports/build.md`, `docs/reports/testing.md`, `docs/reports/speckit.md`, `docs/reports/dependencies.md`, `docs/reports/evidence.md`, `docs/rendering-boundaries.md`, and `docs/layout-boundaries.md`.
- Stable baseline directories for current public package surface.

Product-owned after instantiation:

- Product-specific feature specs under `specs/`.
- Product modules, widgets, charts, themes, assets, and sample scenarios.
- Product branding, authors, repository URL, package IDs, and package descriptions.
- Product-specific screenshots and performance expectations.
- Product-specific external integrations.
- Product-specific ADRs that supersede template defaults.

Do not put existing feature history such as `specs/001-*` through `specs/005-*` into the distributed template. Include a `specs/000-template-profile/` or docs page that explains the inherited process and shows one minimal example.

## Recommended Repository Shape

Target shape for an instantiated project:

```text
.
|-- .config/
|   `-- dotnet-tools.json
|-- .specify/
|   |-- memory/
|   |-- presets/
|   `-- extensions/
|-- .agents/
|   `-- skills/
|-- .template.config/
|   `-- template.json
|-- docs/
|   |-- template-profile.md
|   |-- architecture.md
|   |-- build.md
|   |-- testing.md
|   |-- speckit.md
|   |-- dependencies.md
|   |-- evidence.md
|   |-- rendering-boundaries.md
|   |-- layout-boundaries.md
|   `-- adr/
|-- samples/
|-- scripts/
|-- specs/
|   `-- 000-template-profile/
|-- src/
|-- tests/
|-- Directory.Build.props
|-- Directory.Packages.props
|-- FS-Skia-UI.sln
|-- NuGet.config
|-- build.fsx
|-- fake.cmd
`-- fake.sh
```

For this repository itself, keep existing feature directories. For the template package, exclude old feature directories and generated `bin/`, `obj/`, readiness logs that are not canonical examples, local caches, and machine-specific files.

## `dotnet new` Template Contract

The template package should be a normal .NET template package, not a custom generator.

Minimum `template.json` responsibilities:

- `identity`: stable package identity such as `FS.Skia.UI.Template`.
- `shortName`: short CLI name such as `fs-skia-ui`.
- `sourceName`: placeholder project name that is replaced in file names and contents.
- `preferNameDirectory`: true.
- Symbols for:
  - root namespace
  - package prefix
  - authors
  - repository URL
  - target framework, default `net10.0`
  - include charts package
  - include layout package
  - include Yoga layout adapter
  - include visual samples
  - include parity tests
  - use NuGet CPM
  - use package smoke validation
- Source modifiers to exclude optional packages, tests, or samples when a symbol is disabled.

Recommended template smoke command:

```bash
dotnet new install ./artifacts/templates/FS.Skia.UI.Template.*.nupkg
dotnet new fs-skia-ui -n TemplateSmoke --include-charts true --include-layout true
cd TemplateSmoke
./fake.sh build -t Dev
./fake.sh build -t TemplateVerify
```

`TemplateCheck` should instantiate into a temporary directory, run restore/build/tests, run at least one sample `--contract-smoke`, and verify no unreplaced template tokens remain.

## FAKE Build Graph

FAKE should be the one command language. CI YAML, Speckit hooks, local docs, and agent skills should call named targets.

Recommended target graph:

```text
Clean
  ==> Restore
  ==> Build
  ==> Test
  ==> PackageSurfaceCheck
  ==> FsiTranscripts
  ==> SampleContractSmoke
  ==> EvidenceGraph
  ==> EvidenceAudit
  ==> Verify

Build
  ==> Pack
  ==> PackLocal
  ==> PackageSmoke

Build
  ==> LayoutEvidence
  ==> VisualEvidence

Restore
  ==> DependencyReport

Restore
  ==> TemplateInstall
  ==> TemplateInstantiate
  ==> TemplateSmoke
  ==> TemplateCheck
```

Command surface:

```bash
./fake.sh build -t Dev
./fake.sh build -t Verify
./fake.sh build -t Ci
./fake.sh build -t Visual
./fake.sh build -t LayoutEvidence
./fake.sh build -t PackLocal
./fake.sh build -t PackageSmoke
./fake.sh build -t TemplateCheck
./fake.sh build -t DependencyReport
```

Target responsibilities:

| Target | Responsibility | Output |
|---|---|---|
| `Dev` | Fast local restore/build/unit tests | Console output and test result |
| `Verify` | Full non-GPU verification | Readiness logs and evidence verdict |
| `Ci` | Non-interactive CI alias for `Verify` plus locked restore where configured | CI logs |
| `Visual` | GPU/window/screenshot checks | Screenshot metadata and smoke logs |
| `LayoutEvidence` | Deterministic layout reports, Yoga version, invalidation locality | `readiness/layout/*.json` and `.txt` |
| `PackLocal` | Pack all packable projects to `~/.local/share/nuget-local/` | `.nupkg` files |
| `PackageSmoke` | Validate sample or consumer projects against packed packages | package smoke logs |
| `TemplateCheck` | Install and instantiate template in temp directory | template smoke logs |
| `DependencyReport` | Print package graph, pin owners, licenses, preview risk | `readiness/dependencies.md` |
| `EvidenceGraph` | Validate task DAG and propagation | `task-graph.json`, `task-graph.md` |
| `EvidenceAudit` | Diff scan and synthetic gate | audit verdict |

CI must call `./fake.sh build -t Ci`. It should not duplicate target order in YAML.

## Package and Dependency Policy

Use NuGet Central Package Management first.

Concrete changes:

- Add `Directory.Packages.props`.
- Set `ManagePackageVersionsCentrally` to `true`.
- Move all `PackageReference Version="..."` values into `<PackageVersion />` entries.
- Keep project files with versionless `<PackageReference Include="..." />`.
- Add `docs/reports/dependencies.md` with package owner, reason, license, pinning policy, upgrade command, and evidence target.
- Add `DependencyReport` in FAKE.
- Add a no-inline-version package test that fails if an individual project reintroduces `Version="..."` except for explicitly allowed local package smoke cases.

Recommended lock policy:

- For application/sample/package-smoke roots, use lock files and CI locked restore when the workflow requires repeatability.
- For packable library projects, do not present lock files as downstream dependency guarantees.
- For template validation, run `dotnet restore --locked-mode` only where lock files are intentionally checked in.
- Record any unlocked restore in `readiness/dependencies.md` so repeatability expectations are explicit.

Paket decision:

- Do not adopt Paket as the default first move.
- Reconsider Paket if the project needs one committed lock file to govern all direct and transitive dependencies across multiple generated products, or if dependency update workflows become too weak with NuGet CPM.
- If Paket is adopted later, make it a template-level ADR because it changes developer restore and update workflows.

## Speckit Constitution Improvements

The current constitution is strong. For template use, add these principles or subsections:

1. Template Contract Is Source-Controlled

   New projects inherit a versioned template profile. Any deviation from template-owned layout, build graph, evidence graph, package policy, or sample contract must be recorded in `docs/template-deviations.md`.

2. View Functions Produce Declarative UI Data

   `view` must return framework scene, layout, and widget data. It must not perform I/O, mutate host state, allocate long-lived native resources, or draw directly to `SKCanvas`, except through a documented custom-render escape hatch with tests and sample evidence.

3. Skia Resources Live at the Host Edge

   `SKSurface`, `SKCanvas`, Vulkan contexts, images, fonts, and native handles are owned and disposed by host or renderer modules. Domain, layout, widget, and MVU modules may describe resources but not own backend lifetimes.

4. Layout Is Logical Before Physical

   Layout operates in logical coordinates. Pixel snapping, DPI scaling, and backend rounding happen at render and hit-test boundaries and must be deterministic.

5. Build Graph Is Canonical

   `build.fsx` targets are the authoritative local and CI workflow. CI may call FAKE targets but must not reimplement restore/build/test/pack/smoke/evidence sequencing in YAML.

6. Template Drift Is a Defect

   If a feature adds a new test category, public module type, sample pattern, readiness artifact, script, dependency policy, or build target, the corresponding template preset, docs, and FAKE target must be updated or an explicit deferral recorded.

7. Golden Evidence Has Owners

   Surface baselines, screenshots, FSI transcripts, layout reports, performance files, and generated task graphs must be reproducible by named build targets. Hand-edited evidence is allowed only when explicitly marked.

8. Dependencies Have Owners

   Every dependency must have a pinning policy, maintenance owner, upgrade command, license note, preview-risk note when applicable, and evidence target.

These updates should be applied to both `.specify/memory/constitution.md` and the preset's `constitution-template.md`.

## Speckit Preset Improvements

The current `fsharp-opinionated` preset should become the project-family preset. It can remain one preset initially, but its sections should be organized as if it contains three layered concerns:

- `fsharp-library-opinionated`: FSI-first workflow, package baselines, Expecto, scripts, docs.
- `elmish-skia-ui-opinionated`: MVU boundary, declarative view, Skia host edge, samples, screenshots, layout evidence.
- `template-framework-governance`: template drift, FAKE build graph, scaffold verification, dependency governance.

Preset-owned templates should include:

- `spec-template.md`
- `plan-template.md`
- `tasks-template.md`
- `tasks-deps-template.yml`
- `constitution-template.md`
- command wrappers for `speckit.specify`, `speckit.plan`, `speckit.tasks`, and `speckit.implement` where needed.

Required generated spec fields:

- Change tier
- User-visible outcome
- Affected package(s)
- Public FSI impact
- MVU/workflow impact
- Layout impact
- Skia/native-resource impact
- Sample/screenshot impact
- Package/dependency impact
- Build target impact
- Evidence target impact
- Synthetic evidence declaration
- Template drift declaration

Required generated plan fields:

- `.fsi` contract path(s)
- Semantic test path(s)
- FSI transcript path(s)
- Surface baseline path(s)
- Package validation path(s)
- Layout evidence path(s), when relevant
- Visual evidence path(s), when relevant
- FAKE target(s) to run
- Dependency pin/owner/licensing note
- Migration and compatibility note for Tier 1
- Explicit non-goals

Required generated tasks:

- Draft/update `.fsi` before `.fs`.
- Write semantic tests against public API.
- Refresh surface baselines for Tier 1.
- Run FSI transcript.
- Run sample contract smoke.
- Run screenshot capture when visual behavior changes.
- Run layout evidence when layout behavior changes.
- Update FAKE targets if workflow changes.
- Update dependency documentation when dependencies change.
- Update template/preset/docs or record template-drift deferral.
- Emit and validate `tasks.deps.yml`.
- Run evidence graph and audit.

## Spec Kit Extensions and Skills

The current evidence extension should remain. Add the following project-local extensions.

### `speckit.contract`

Purpose: verify public API discipline before implementation and before merge.

Inputs:

- `src/**/*.fs`
- `src/**/*.fsi`
- `tests/Package.Tests`
- current surface baselines
- feature `contracts/public-api.md`

Checks:

- Every public `.fs` module has a matching `.fsi`.
- No top-level `private`, `internal`, or `public` modifiers are introduced in `.fs`.
- Tier 1 plans name `.fsi` paths and surface baseline impact.
- Surface baseline output is current.
- FSI transcript exists for public API changes.

Outputs:

- `readiness/contract/contract-audit.md`
- `readiness/surface-baselines/*.txt`

### `speckit.layout-evidence`

Purpose: make layout behavior reproducible and reviewable without screenshots.

Inputs:

- fixed layout trees
- Yoga.Net version
- measurement callbacks
- expected diagnostic cases

Checks:

- repeated evaluation produces stable logical bounds
- invalid values produce structured diagnostics
- fallback geometry stays bounded
- invalidation preserves unaffected sibling bounds
- pixel snapping and hit-test snapping use the same policy
- Yoga.Net version is recorded

Outputs:

- `readiness/layout/bounds-report.json`
- `readiness/layout/invalidation-report.json`
- `readiness/layout/pixel-snap-report.json`
- `readiness/layout/yoga-version.txt`

### `speckit.visual-evidence`

Purpose: distinguish real visual behavior from unverified sample code.

Inputs:

- sample projects
- renderer mode
- dimensions
- DPI/scale
- random seed where applicable

Checks:

- sample builds
- `--contract-smoke` exits cleanly
- screenshot capture succeeds where environment supports it
- metadata records OS, renderer, GPU mode, dimensions, DPI, fallback flags, and timestamp
- visual tests skip only with explicit environment diagnostic

Outputs:

- `readiness/visual/*.png`
- `readiness/visual/*.json`
- `readiness/sample-smoke/*.txt`

### `speckit.build-health`

Purpose: summarize canonical build health for review.

Inputs:

- FAKE `Verify`
- test logs
- package logs

Outputs:

- `readiness/build-health.md`
- `readiness/logs/*.txt`

### `speckit.template-drift`

Purpose: prevent feature work from adding one-off conventions.

Checks whether a diff added:

- new public module without `.fsi`
- new package without pack target
- new test category without FAKE target
- new sample without smoke target
- new dependency without owner/pin/license note
- new script not wired into FAKE
- new readiness artifact not documented in preset/templates
- new docs obligation not reflected in template docs

Outputs:

- `readiness/template-drift.md`

Failure rule:

- fail unless the template was updated or a bounded deferral is recorded.

### `speckit.release-pack`

Purpose: validate local packages before publishing or consumption by sample projects.

Checks:

- version bump policy
- `dotnet pack`
- pack output to `~/.local/share/nuget-local/`
- package smoke consumers
- package docs and surface baselines

Outputs:

- `readiness/package/package-smoke.txt`
- `readiness/package/package-notes.md`

Skills should wrap these extensions and FAKE targets, not duplicate their logic.

## Testing Strategy

Keep test categories explicit and template-owned:

| Category | Purpose | Default target |
|---|---|---|
| Pure domain tests | public pure functions | `Test` |
| MVU transition tests | `Model + Msg -> Model + Effect` | `Test` |
| Effect interpreter tests | real filesystem/process/window dependencies where safe | `Verify` |
| Layout tests | logical bounds, diagnostics, invalidation, snapping | `LayoutEvidence` |
| Render planning tests | scene to render commands without GPU where possible | `Test` |
| Smoke tests | sample entry points and contract smoke | `SampleContractSmoke` |
| Visual evidence tests | screenshots and metadata | `Visual` |
| Package tests | packed-library and public-surface validation | `PackageSmoke` |
| Performance checks | bounded deterministic micro-scenarios | `LayoutEvidence` or dedicated target |

Expecto implications:

- Use `testList` names that match the categories above.
- Treat tests as composable values so category filters are stable.
- Mark file-output and global-resource tests as sequenced or isolate output paths.
- Keep GPU/visual tests opt-in or environment-diagnosed.
- Keep FSI scripts as first-class validation artifacts because this repository treats FSI as the honest public API consumer.

## Layout and Rendering Policy

Yoga-backed layout:

- Yoga.Net stays behind `src/Layout`.
- Public layout API exposes F# records, unions, options, lists, and results.
- Input trees are immutable.
- Measurement callbacks are deterministic and bounded.
- Leaf measurement returns size plus diagnostics, not hidden mutable cache state.
- Logical coordinates are canonical.
- Pixel snapping happens only at render and hit-test boundaries.
- Computed bounds and diagnostics are structured output.
- Yoga version and adapter behavior are recorded in readiness evidence.

Skia rendering:

- Host owns `SKSurface`, `SKCanvas`, GPU/Vulkan context, native handles, and disposal.
- View functions create declarative scene values.
- Direct custom drawing is an explicit escape hatch, not the default view model.
- Custom drawing receives a short-lived drawing context.
- Custom drawing cannot store `SKCanvas`.
- Custom drawing reports diagnostics or resource requests through declared effects.
- Custom drawing requires semantic tests plus sample or visual smoke evidence.

## Documentation Set

Create this documentation structure:

```text
docs/
|-- template-profile.md
|-- architecture.md
|-- build.md
|-- testing.md
|-- speckit.md
|-- dependencies.md
|-- evidence.md
|-- rendering-boundaries.md
|-- layout-boundaries.md
|-- migration.md
`-- adr/
    |-- 0001-template-governance.md
    |-- 0002-fake-build-graph.md
    |-- 0003-nuget-central-package-management.md
    |-- 0004-elmish-view-boundary.md
    |-- 0005-skia-host-edge.md
    `-- 0006-yoga-layout-boundary.md
```

Docs should explain decisions and regeneration commands. Do not make docs the only source of truth for command ordering; docs should reference FAKE targets.

## Risks and Mitigations

| Risk | Impact | Mitigation |
|---|---|---|
| Template becomes too heavy | New products start slow and users bypass it | Provide `Dev`, `Verify`, and `Visual` targets; make visual/GPU evidence explicit and separable. |
| `dotnet new` is used for incremental changes | Existing repos get overwritten or drift | Use `dotnet new` only for first creation; use FAKE scaffold targets for additions. |
| Build logic duplicates in CI | Different results locally and remotely | CI calls `./fake.sh build -t Ci`; YAML contains environment setup only. |
| Package versions drift | Repeated updates and inconsistent restores | Use `Directory.Packages.props`, dependency report, and no-inline-version tests. |
| Lock files are misunderstood | False confidence for library consumers | Document app-root vs library lock semantics; use locked restore only where intentional. |
| Visual tests fail due environment | False negatives on machines without GPU/window support | Require explicit skip diagnostics with OS, renderer, GPU mode, and fallback reason. |
| Agents add one-off scripts | Template loses governance value | Template-drift gate checks scripts, targets, docs, and preset updates. |
| Yoga.Net leaks into public API | Public contract inherits C# lifecycle/disposal details | Keep Yoga.Net out of `.fsi`; expose F# data only. |
| Direct Skia drawing bypasses MVU | Tests cannot reason about behavior | Require declarative view by default and documented custom-render escape hatch. |
| Surface baselines stay feature-local | Future features patch old readiness folders | Move current baselines to a template-owned path and let feature readiness store evidence copies. |

## Success Metrics

A template framework milestone is successful when:

- A clean clone can run `./fake.sh build -t Dev`.
- CI can run `./fake.sh build -t Ci` without duplicating build order.
- `./fake.sh build -t Verify` produces evidence graph and audit output.
- `./fake.sh build -t TemplateCheck` installs, instantiates, builds, and tests a new project.
- No project file contains inline package versions except approved package-smoke overrides.
- Every public `.fs` module has a paired `.fsi`.
- Package surface baselines have a stable current location.
- Every sample either has `--contract-smoke` or is marked visual/manual with a reason.
- Every layout feature produces structured layout evidence.
- Every visual feature produces screenshot metadata or an explicit environment skip diagnostic.
- Generated Spec Kit specs, plans, tasks, and `tasks.deps.yml` contain UI-specific evidence obligations.
- Template-drift checks fail when a feature adds a new convention without updating the template or recording a deferral.

## Implementation Roadmap

### Phase 1: Codify Current Practice

Deliverables:

- Add FAKE bootstrap: `build.fsx`, `fake.sh`, `fake.cmd`.
- Add `.config/dotnet-tools.json`.
- Add targets: `Clean`, `Restore`, `Build`, `Test`, `Dev`, `Verify`.
- Wire existing FSI and surface scripts into targets.
- Add stable current baseline path for package surface.
- Add `docs/reports/build.md`, `docs/reports/testing.md`, and `docs/reports/evidence.md`.

Exit criteria:

- `./fake.sh build -t Dev` passes.
- `./fake.sh build -t Verify` runs non-GPU tests and evidence gates.

### Phase 2: Package Governance

Deliverables:

- Add `Directory.Packages.props`.
- Move package versions out of project files.
- Add `docs/reports/dependencies.md`.
- Add `DependencyReport` target.
- Add no-inline-package-version package test.
- Decide lock-file policy for samples and package-smoke consumers.

Exit criteria:

- All package versions are centralized.
- Dependency report lists owner, reason, license, pin, upgrade command, and evidence target.

### Phase 3: Speckit Hardening

Deliverables:

- Update constitution and constitution template with template principles.
- Update preset-owned spec, plan, tasks, and deps templates.
- Add `speckit.contract`.
- Add `speckit.build-health`.
- Add `speckit.template-drift`.
- Ensure `after_implement` runs evidence and drift gates.

Exit criteria:

- New generated plans include package, FSI, MVU, layout, visual, dependency, build, and evidence fields.
- New generated tasks include `tasks.deps.yml` and required evidence tasks.

### Phase 4: Deterministic Scaffolding

Deliverables:

- Add `.template.config/template.json`.
- Parameterize solution name, root namespace, package prefix, authors, repository URL, target framework, optional charts/layout/visual packages.
- Add `TemplateInstall`, `TemplateInstantiate`, `TemplateSmoke`, and `TemplateCheck` targets.
- Add FAKE scaffold targets for package, test project, sample, baseline, and feature readiness folders.

Exit criteria:

- Template can be packed and installed locally.
- `TemplateCheck` creates a temp repo and runs `Dev`.

### Phase 5: UI Evidence

Deliverables:

- Add `speckit.layout-evidence`.
- Add `speckit.visual-evidence`.
- Add screenshot metadata schema.
- Add layout bounds report schema.
- Ensure every sample has `--contract-smoke` or explicit visual/manual classification.
- Add `Visual` and `LayoutEvidence` FAKE targets.

Exit criteria:

- Layout changes produce structured bounds, invalidation, snap, diagnostics, and Yoga version reports.
- Visual changes produce screenshot metadata or explicit environment skip diagnostics.

### Phase 6: Local Package Validation and Publishing

Deliverables:

- Add `PackLocal` and `PackageSmoke`.
- Add `speckit.release-pack`.
- Add package consumer smoke projects or generated temp consumers.
- Add package release notes template.

Exit criteria:

- Local packages are produced under `~/.local/share/nuget-local/`.
- Sample or consumer projects validate against packed packages, not only project references.

## First PR Recommendation

The first PR should be intentionally narrow:

1. Add FAKE bootstrap.
2. Add `Dev`, `Verify`, `PackLocal`, `PackageSurfaceCheck`, `FsiTranscripts`, `EvidenceGraph`, and `EvidenceAudit` targets.
3. Move surface baselines to a stable current path or add a compatibility target that writes both current and feature-readiness copies.
4. Add `docs/reports/build.md` and `docs/reports/evidence.md`.
5. Update Speckit task template so generated tasks call FAKE targets.

Do not start with `.template.config/template.json`. Without the build graph, the template can create files but cannot prove that the created project is valid.

## Final Position

FS-Skia-UI should become a template framework by making its constraints executable:

- `dotnet new` for initial shape.
- FAKE for commands.
- NuGet CPM for dependency centralization.
- Speckit presets for generated artifacts.
- Speckit extensions for evidence gates.
- Project-local skills for agent workflows.
- Declarative Elmish views and host-edge Skia resources.
- Internal Yoga.Net layout adapter with public F# contracts.

The proposal should treat drift as the main enemy. The codebase already has many good conventions; the template framework work is about making those conventions reproducible, testable, and hard to bypass.
