---
title: V2 Analysis
category: Design
categoryindex: 4
index: 7
description: Comprehensive analysis of the V2 template packaging and drift governance work.
---

# V2 Analysis

FS.Skia.UI has two overlapping intentions: it is a first-version F# UI toolkit
for declarative Skia scenes rendered through a Vulkan-only desktop host, and it
is becoming a governed project template so future products inherit the same
build, test, documentation, dependency, and Spec Kit discipline. The V2 work
mostly achieves its stated template-governance goals. The implementation is not
spaghetti code; it is mostly plain F# with strong public-contract discipline.
The main risks are concentrated in a few monolithic files and in governance
checks that prove the expected happy path but are still coarse in places.

## Research Scope

This analysis is based on the current source tree, the V2 plan and contracts,
the repository constitution, readiness evidence, generated template outputs,
source modules, tests, and targeted verification commands.

Reviewed inputs:

| Area | Files and evidence |
|------|--------------------|
| Governance intent | `.specify/memory/constitution.md`, `specs/007-v2-template-packaging/plan.md`, `specs/007-v2-template-packaging/spec.md`, `specs/007-v2-template-packaging/tasks.md` |
| Template implementation | [.template.config/template.json](../.template.config/template.json), [.template.package/FS.Skia.UI.Template.fsproj](../.template.package/FS.Skia.UI.Template.fsproj), generated projects under `artifacts/template-check/007-v2-template-packaging/` |
| Build and checks | [build.fsx](../build.fsx), [fake.sh](../fake.sh), [fake.cmd](../fake.cmd), [scripts/dependency-report.fsx](../scripts/dependency-report.fsx), [scripts/template-drift.fsx](../scripts/template-drift.fsx) |
| Runtime code | [src/Lib](../src/Lib/), [src/Charts](../src/Charts/), [src/Layout](../src/Layout/) |
| Tests | [tests/Lib.Tests](../tests/Lib.Tests/), [tests/Charts.Tests](../tests/Charts.Tests/), [tests/Layout.Tests](../tests/Layout.Tests/), [tests/Package.Tests](../tests/Package.Tests/), [tests/Governance.Tests](../tests/Governance.Tests/), [tests/Smoke.Tests](../tests/Smoke.Tests/), [tests/Parity.Tests](../tests/Parity.Tests/) |
| Readiness evidence | `specs/007-v2-template-packaging/readiness/*`, especially template scans, dependency report, generated guidance report, drift report, and merge summary |

Targeted checks run during this review:

| Command | Result |
|---------|--------|
| `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --no-build` | PASS, 37 tests |
| `dotnet test tests/Lib.Tests/Lib.Tests.fsproj --no-build` | PASS, 42 tests |
| `dotnet test tests/Layout.Tests/Layout.Tests.fsproj --no-build` | PASS, 22 tests |
| `dotnet test tests/Charts.Tests/Charts.Tests.fsproj --no-build` | PASS, 10 tests |
| `dotnet test tests/Package.Tests/Package.Tests.fsproj --no-build` | PASS, 6 tests |
| `dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj --no-build` | PASS, 2 tests |
| `dotnet test tests/Parity.Tests/Parity.Tests.fsproj --no-build` | PASS, 5 tests |
| `dotnet fsi scripts/dependency-report.fsx /tmp/fs-skia-ui-dependency-report-analysis.md` | PASS |
| `dotnet fsi scripts/template-drift.fsx /tmp/fs-skia-ui-template-drift-analysis.md` | FAIL in the current dirty worktree because changed template-owned docs/README paths are not aligned or deferred |
| `./fake.sh build --list` | Lists the expected targets, but emits a stale FAKE runner warning and a build-script FS0052 warning |

The recorded V2 readiness evidence says `Dev`, `TemplateCheck`,
`DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, `Verify`, `Ci`,
and both evidence audits passed at the time that evidence was produced. The
current worktree is no longer in that exact state: several readiness files,
`README.md`, and untracked design docs are modified, so a fresh drift scan
currently fails.

## Project Intentions

The project is trying to provide a governed, F#-first desktop UI framework.
At runtime, the intended user model is:

1. Define an Elmish-style model, messages, `init`, `update`, and `view`.
2. Return immutable `Scene` values from `view`.
3. Let `Viewer.run` own window creation, input mapping, Vulkan setup, Skia
   rendering, screenshot effects, shutdown, and diagnostics.
4. Keep charts, DataGrid, layout, graph, keyboard input, and sample behavior
   as pure or mostly pure public APIs that can be tested without a live window.

The V2 intention is separate but related: make this repository the source of
truth for a reusable `dotnet new fs-skia-ui` template. A generated product
should not be a casual copy of the repo. It should inherit a curated starter
shape, central dependency policy, build wrappers, tests, docs, surface
baselines, Spec Kit prompts, and drift governance.

The constitution reinforces this intent with seven governing ideas:

- Public features are specified before implementation.
- Runtime visibility lives in `.fsi` files.
- Simple F# is preferred over clever language features.
- Stateful and I/O workflows use an Elmish/MVU boundary.
- Synthetic evidence must be loudly disclosed.
- Behavior-changing work needs tests.
- Operational failures should produce structured diagnostics.

## How V2 Tries To Achieve The Intent

V2 uses standard .NET template infrastructure plus repository-local governance.
The main mechanism is [.template.config/template.json](../.template.config/template.json),
which defines:

- Template identity: `FS.Skia.UI.Template`.
- Short name: `fs-skia-ui`.
- Profiles: `default` and `minimal`.
- Product identity substitutions for namespace, authors, repository URL, and
  target framework.
- Source exclusions for history, `specs/**`, build outputs, `.git`, artifacts,
  and template package source.
- Minimal-profile exclusions for charts, layout, optional samples, parity,
  smoke tests, and visual/sample scope beyond `BasicViewer`.

The local package boundary is
[.template.package/FS.Skia.UI.Template.fsproj](../.template.package/FS.Skia.UI.Template.fsproj).
It packages template-owned content into `artifacts/templates/` without
requiring an external template repository.

The FAKE workflow in [build.fsx](../build.fsx) adds V2 targets:

| Target | Role |
|--------|------|
| `TemplatePack` | Builds and validates the local NuGet template package. |
| `TemplateInstallSource` | Installs the template from the source directory. |
| `TemplateInstallPackage` | Installs the template from the local package artifact. |
| `TemplateInstantiate` | Creates source/default, source/minimal, package/default, and package/minimal generated projects. |
| `TemplateSmoke` | Scans generated projects and runs their `Dev` target. |
| `TemplateCheck` | Requires the template evidence artifact set. |
| `DependencyReport` | Enforces Central Package Management and dependency docs. |
| `GeneratedGuidanceCheck` | Checks active and preset Spec Kit spec/plan prompts. |
| `TemplateDrift` | Checks template-owned dirty paths against alignment or deferral rules. |

The build script models this as a local MVU/effect algebra:

- `BuildModel` stores paths and workflow state.
- `BuildMsg` starts, completes, or fails targets.
- `BuildEffect` describes filesystem, process, template, scan, and report work.
- `update` maps messages to effects.
- `interpret` executes effects at the edge.

This is a good fit for the constitution. It keeps target intent testable as
data while still letting the interpreter run real `dotnet`, `git`, and
filesystem commands.

V2 also centralizes package versions in
[Directory.Packages.props](../Directory.Packages.props) and documents package
metadata in [docs/dependencies.md](dependencies.md). Repo-owned external
`PackageReference` entries are versionless, except documented local package
validation paths for `FS.*` packages under `UsePackedPackage`.

## Does V2 Achieve It?

Mostly yes, for the scoped V2 goals.

The recorded readiness evidence is strong for the non-visual template scope:

- Source-directory and local-package template installs were both exercised.
- Default and minimal generated projects were both created from both artifact
  boundaries.
- Generated projects passed placeholder scans.
- Historical `specs/00*` feature history was excluded.
- Minimal profile exclusions were verified.
- Generated `Dev` completed for all four rows.
- Dependency governance passed.
- Generated guidance checks passed.
- Evidence audit reported no synthetic tasks.

The recorded generated-project scan times were far below the 15 minute target:

| Artifact | Profile | Generated `Dev` elapsed |
|----------|---------|--------------------------|
| source | default | 35.5 seconds |
| source | minimal | 17.1 seconds |
| package | default | 35.8 seconds |
| package | minimal | 17.2 seconds |

The current implementation also preserves the runtime package boundary:

- `FS.Skia.UI` owns renderer, scene, diagnostics, keyboard input, and viewer
  host behavior through [src/Lib](../src/Lib/).
- `FS.Skia.UI.Charts` references core and returns scene values from
  [src/Charts](../src/Charts/).
- `FS.Skia.UI.Layout` references core, wraps Yoga layout, and returns layout
  results or scene values from [src/Layout](../src/Layout/).
- Samples are consumers.
- Tests cover semantic behavior, packaging, smoke contracts, and governance.

The caveats are important:

1. A fresh `TemplateDrift` scan fails in the current dirty worktree because
   `README.md` and several untracked docs are template-owned changes without
   recognized alignment or deferral. That does not invalidate the recorded V2
   evidence, but it means the current checkout is not drift-clean.
2. `TemplateDrift` is intentionally coarse. It checks dirty path classes and
   whether any recognized alignment path changed. It does not prove that the
   alignment is semantically correct.
3. `GeneratedGuidanceCheck` is a substring check. It proves that required prompt
   phrases exist, not that generated specs and plans are well structured or
   usable.
4. `TemplateSmoke` is real enough for V2, but it remains non-visual. It does
   not prove live Vulkan rendering quality, window behavior, or image output.
5. The runtime renderer is complex and environment-sensitive. Non-visual tests
   exercise public contracts well, but they cannot fully replace real GPU,
   window, swapchain, and screenshot validation.

## Alternatives

### Separate Template Repository

A separate template repository could contain only generated-product starter
files.

Pros:

- Cleaner template package with less source-only history to exclude.
- Lower risk that repository-analysis docs, readiness artifacts, or internal
  scripts accidentally enter generated products.
- Simpler drift mental model for template consumers.

Cons:

- Requires synchronization between framework source and template source.
- Increases release and maintenance overhead.
- Makes in-place V2 validation harder because source changes must propagate
  across repos before the template can be proven.

This is a reasonable later-phase option once the framework surface stabilizes.
For V2, keeping the template in-repo is pragmatic.

### Source-Only Template Without Local Package Validation

The project could stop at `dotnet new install .`.

Pros:

- Simpler implementation.
- Faster validation.
- Fewer package metadata concerns.

Cons:

- Does not prove the distributable template artifact.
- Misses package content problems.
- Gives less confidence that a future published template will behave like the
  source checkout.

V2 made the better choice by validating both source and package boundaries.

### Custom Copier Script Instead Of `dotnet new`

A custom FAKE or shell copier could generate projects.

Pros:

- Full control over excludes, renames, prompts, and validation.
- Easier to encode project-specific policies.

Cons:

- Non-standard for .NET users.
- More custom code to maintain.
- Less discoverable than `dotnet new`.

Using `dotnet new` is the right baseline unless template engine limitations
become a real blocker.

### External Scaffolding Tool

Tools such as Yeoman, Cookiecutter, or a bespoke CLI could provide richer
generation.

Pros:

- Potentially better interactive prompts and conditional content.
- Easier to add custom generation logic.

Cons:

- Adds a non-.NET dependency or new distribution surface.
- Weakens the "standard .NET project template" story.
- More complex for generated products and CI.

This would be overkill for the current V2 scope.

### Different Runtime Architecture

The runtime could choose a broader or more established UI host, such as
Avalonia, WPF/WinUI, SkiaSharp.Views, SDL, GLFW, or a CPU renderer.

Pros:

- Broader platform support or simpler renderer lifecycle.
- Less direct Vulkan interop in the project.
- Easier visual testing in some host environments.

Cons:

- Less control over the Vulkan-only evidence boundary.
- Different public API and dependency posture.
- May conflict with the project's explicit "no fallback renderer" design.

The current Vulkan-only path is coherent, but it puts a high maintenance burden
on `Library.fs`.

## Pros Of The Current Approach

- The repository has a clear governance spine. Specs, plans, tasks, tests,
  docs, build targets, surface baselines, and readiness evidence all reinforce
  each other.
- Public API visibility is explicit and compiler-backed through `.fsi` files.
- The template is validated through real generated projects, not just metadata
  inspection.
- Generated projects inherit real workflows instead of prose-only instructions.
- Central Package Management is a strong dependency governance improvement.
- The default/minimal split is useful: default demonstrates the whole framework,
  while minimal gives a smaller governed starting point.
- The code mostly uses plain F#: records, discriminated unions, modules,
  functions, lists, maps, results, and options.
- The tests are broad for non-visual behavior: core scene semantics, keyboard
  input, charts, DataGrid, layout, graph, package surface, samples, and
  governance all have coverage.

## Cons And Risks Of The Current Approach

- [build.fsx](../build.fsx) is almost 1,000 lines and combines path configuration, target
  graph, process execution, template installation, generated-project scanning,
  guidance scanning, package validation, and self-checks in one file.
- [src/Lib/Library.fs](../src/Lib/Library.fs) is over 2,300 lines and combines public data types,
  scene construction, diagnostics, parity reporting, Skia drawing, Vulkan
  setup, swapchain handling, screenshot encoding, event loop, and viewer API.
- The renderer path necessarily uses mutation, native pointers, `GCHandle`,
  `Marshal`, `Unchecked`, explicit disposal, and nested resource lifetimes.
  This is the most complex part of the codebase.
- Several governance checks are lexical or path-level. They catch many errors,
  but they do not prove intent-level correctness.
- Some fallback or error paths are quiet. For example, Yoga layout failure in
  `tryYogaLayout` falls back to the pure layout path without preserving the
  exception as a diagnostic.
- Public records expose many states that consumers can construct manually. This
  is idiomatic and convenient, but it weakens invariants unless all public
  functions defensively handle invalid records.
- Current `./fake.sh build --list` output includes a stale FAKE runner warning
  and an FS0052 warning from the build script. It still exits successfully, but
  the warning conflicts with the repository's general warning discipline.
- Raw `dotnet fsi build.fsx --list` fails because [build.fsx](../build.fsx) uses FAKE/Paket
  script package management. The wrapper is the intended entry point, but this
  is still worth documenting for contributors who try raw FSI.
- Adding arbitrary docs under `docs/` is a template-owned change. Without an
  explicit exclusion, alignment update, or deferral, it will be picked up by
  drift checks and may be included in generated projects.

## Code Quality State

Overall quality is good for a young framework, with some hotspots.

The best parts:

- Package boundaries are clean.
- Public signatures exist for every runtime source file.
- No top-level `private`, `internal`, or `public` access modifiers were found
  in `src/**/*.fs`.
- The source projects compile `.fsi` before `.fs` explicitly.
- [Directory.Build.props](../Directory.Build.props) promotes `FS0078` to an error, reinforcing the
  "visibility lives in `.fsi`" rule.
- Tests exercise behavior through public APIs rather than mostly through
  private helpers.
- Charts and graph modules are compact and readable.
- The dependency report and drift scripts are straightforward and easy to
  inspect.

The weaker parts:

- [src/Lib/Library.fs](../src/Lib/Library.fs) is too broad. It is readable locally, but the file has too many
  responsibilities.
- [src/Lib/KeyboardInput.fs](../src/Lib/KeyboardInput.fs) is understandable but large; parsing, validation, runtime
  update, display projection, rendering, replay, and bigram analysis could be
  separated without changing the public module.
- [src/Layout/Layout.fs](../src/Layout/Layout.fs) mixes a pure fallback layout algorithm, Yoga translation, Yoga
  execution, diagnostics, workflow update, rendering, snapping, and simple
  stack helpers. It is not unmanageable, but it is a natural next refactoring
  candidate after [src/Lib/Library.fs](../src/Lib/Library.fs).
- [build.fsx](../build.fsx) has a good MVU shape, but operational code and target contract
  code are interleaved. This makes changes safer than ad hoc shell, but still
  harder to review than smaller workflow modules would be.

This is not organically grown spaghetti code. It looks like a deliberately
grown codebase with clear rules and some monolithic early-version files. The
current abstractions are generally lean, but the renderer and build workflow
have outgrown their single-file homes.

## Abstractions And Structure

The abstractions are strongest at package and public API boundaries:

| Boundary | Assessment |
|----------|------------|
| `Scene` | Good. Publicly opaque in [src/Lib/Library.fsi](../src/Lib/Library.fsi), inspectable through `Scene.describe`, and composable by subsystems. |
| `ViewerProgram` | Good. It mirrors Elmish and keeps application state separate from host effects in [src/Lib/Library.fsi](../src/Lib/Library.fsi). |
| `ViewerEffect` | Good. It makes host-side work explicit: render, screenshot, shutdown, diagnostics, dispatch. |
| Diagnostics | Good direction. Stages and severities are public data, and unsupported environments are explicit. |
| Keyboard input runtime | Useful but broad. It exposes a full data model and pure update surface, but many records can be invalid if constructed directly. |
| Charts | Lean. Most chart modules in [src/Charts](../src/Charts/) are thin scene builders over shared helpers. |
| DataGrid | Lean. Sorting, viewport, and hit testing are simple and public in [src/Charts/DataGrid.fs](../src/Charts/DataGrid.fs). |
| Layout | Mixed. Public workflow/effect abstraction is good; [src/Layout/Layout.fs](../src/Layout/Layout.fs) mixes pure and Yoga-backed paths. |
| Graph | Lean. Validation, layout, rendering, and hit testing are easy to understand in [src/Layout/GraphValidation.fs](../src/Layout/GraphValidation.fs) and [src/Layout/Graph.fs](../src/Layout/Graph.fs). |
| Build workflow | Good concept, large implementation. MVU effects are the right abstraction, but [build.fsx](../build.fsx) needs subdivision. |
| Template governance | Pragmatic. Uses standard `dotnet new` and real generated projects, with coarse but useful drift checks. |

The main abstraction concern is invariant control. The project favors public
records and unions. That is idiomatic F# and good for FSI, tests, and
documentation, but it means functions such as `layoutState` or `analyzeBigrams`
must assume either validated inputs or be defensive against empty lists and
missing IDs. A future compatibility pass should decide where records are
intentionally open and where construction should move behind validated helper
functions.

## Language Feature Complexity

The general state is simple F#, with one advanced interop island.

Common, low-complexity features used throughout:

- Records.
- Discriminated unions.
- Modules.
- Options and results.
- Lists, maps, sets, arrays, and sequences.
- Pattern matching.
- Pipelining.
- FSI scripts.
- Explicit function values for Elmish update/view/subscription hooks.

Moderate-complexity features:

- Mutually recursive functions in `VulkanHost.run` for dispatch, effect
  interpretation, screenshot flushing, and event-loop integration.
- Recursive tree and graph walks in layout and graph validation.
- Downcasts from `YamlNode` to `YamlMappingNode`, `YamlSequenceNode`, and
  `YamlScalarNode` in keyboard YAML parsing.
- Reflection in package surface tests and baseline refresh scripts.
- XML parsing in dependency governance.
- Process orchestration with `ProcessStartInfo` in build and governance tests.
- ZIP inspection for template package validation.

High-complexity features:

- Native interop in [src/Lib/Library.fs](../src/Lib/Library.fs): `nativeptr`, `voidptr`, `fixed`,
  `GCHandle`, `Marshal.Copy`, Vulkan handles, command buffers, fences,
  swapchains, image barriers, and explicit destruction.
- Mutable event-loop and renderer state in `VulkanHost.run`.
- Skia GPU context setup and readback.
- Yoga.Net interop in `Layout.tryYogaLayout`.

Complex features that are notably absent:

- No custom operators beyond standard F# use.
- No SRTP-heavy generic tricks.
- No type providers.
- No non-trivial custom computation expressions.
- No broad object-oriented hierarchy.
- No reflection-based runtime dispatch in production code.

The complexity is therefore localized rather than systemic. Most of the code
uses the simple language subset the constitution asks for. The renderer is the
exception, and its complexity is inherent to the chosen Vulkan/Skia host.

## Visibility And Access Restrictions

The repository follows the constitution's `.fsi` visibility rule well.

Observed facts:

- Every real source file under [src](../src/) has a companion `.fsi`.
- Project files compile each `.fsi` before its `.fs`.
- `rg` found no `private`, `internal`, or `public` access modifiers in
  `src/**/*.fs` or `src/**/*.fsi`.
- [Directory.Build.props](../Directory.Build.props) includes this policy:
  `FS0078` is promoted to an error so top-level visibility modifiers in files
  with companion signatures are rejected.
- Package surface baselines exist under [readiness/surface-baselines](../readiness/surface-baselines/) for
  `FS.Skia.UI`, `FS.Skia.UI.Charts`, and `FS.Skia.UI.Layout`.
- Package surface tests verify expected exported contract names.

So the project is not relying on scattered private access modifiers. It is
using `.fsi` visibility as intended. Internal implementation details such as
`SceneNode` and `VulkanHost` are omitted from
[src/Lib/Library.fsi](../src/Lib/Library.fsi), so the compiler
keeps them out of the public API.

One subtle point: `.fsi` controls exported symbols, not all semantic
invariants. Many exported records remain freely constructible. That is not a
visibility violation; it is an API design tradeoff.

## Refactoring Usefulness

Refactoring would be useful, but it should be targeted. A broad rewrite would
be wasteful because the current structure is already understandable and tested.

High-value refactors:

1. Split [src/Lib/Library.fs](../src/Lib/Library.fs) internally.
   Keep the public [src/Lib/Library.fsi](../src/Lib/Library.fsi) stable, but move implementation concerns into
   files such as `SceneModel.fs`, `SceneDiagnostics.fs`, `SkiaDrawing.fs`,
   `VulkanResources.fs`, `VulkanFrame.fs`, `Screenshot.fs`, and
   `ViewerHost.fs`. This would reduce review risk around native lifetime code.

2. Introduce small internal resource helpers for Vulkan handles.
   The code currently uses careful `try/finally` in places, but resource
   ownership is manually spread across functions. Small helpers for fence,
   command pool, staging buffer, swapchain, surface, device, and instance
   cleanup would make failures easier to audit.

3. Flatten the deep `bind` nesting in `VulkanHost.run`.
   A `result` computation expression or a small pipeline of initialization
   steps would make startup order and cleanup obligations easier to read. A
   result CE is allowed by the constitution's complexity rules.

4. Split [build.fsx](../build.fsx) by concern if FAKE loading supports it cleanly.
   Keep one entry script, but separate path model, effects, interpreter,
   template validation, dependency governance, guidance checking, and target
   graph. If FAKE script loading makes this awkward, at least group the file
   into clearly named sections.

5. Convert `GeneratedGuidanceCheck` from substring checks to structured
   section checks.
   The current check is useful but shallow. A Markdown-aware check could verify
   section names, required prompts, deferred-scope placement, and active/preset
   parity.

6. Make `TemplateDrift` in [scripts/template-drift.fsx](../scripts/template-drift.fsx) more semantic.
   Path-level drift is useful. A stronger version would map changed path
   classes to specific required alignment classes and verify that the alignment
   file mentions the changed path or affected feature area.

7. Add diagnostics for Yoga fallback.
   `tryYogaLayout` should report a structured diagnostic when Yoga execution
   fails and the pure fallback path is used.

8. Review public record invariants.
   Decide where free record construction is desired and where helper
   constructors or validation-first APIs should become the recommended path.

Lower-value refactors:

- Rewriting charts or graph code. Those modules are already lean.
- Introducing classes or interfaces for most data models. The current record
  and DU style is clearer.
- Replacing `dotnet new` in V2. The standard template infrastructure is doing
  enough for the current scope.

## Possible Improvements

Short-term improvements:

- Add a [docs/V2Analysis.md](V2Analysis.md) template ownership decision: include it in
  generated products intentionally, or exclude source-repository analysis docs
  from the template profile.
- Resolve the current drift failure by aligning or deferring the changed
  template-owned docs/README paths.
- Fix the [build.fsx](../build.fsx) FS0052 warning at elapsed time capture.
- Document that [build.fsx](../build.fsx) is intended to run through
  [fake.sh](../fake.sh)/[fake.cmd](../fake.cmd),
  not raw `dotnet fsi`.
- Add a generated-project check that confirms source-analysis docs are either
  intentionally included or intentionally absent.

Medium-term improvements:

- Split the renderer implementation into smaller files under the same package.
- Add live renderer smoke evidence behind an explicit opt-in target that
  records OS, GPU, driver, presentation surface, frame hash, and screenshot
  artifacts.
- Add a CPU/offscreen test helper only if it can be clearly marked as
  non-runtime evidence and does not imply fallback-renderer support.
- Expand template package validation to compare package contents against
  template ownership inventory.
- Make dependency metadata machine-readable rather than Markdown-only, then
  render the Markdown from the same source.
- Add negative generated-project tests for broken minimal references beyond
  path absence.

Long-term improvements:

- Consider an external template repository once template consumption becomes a
  release product rather than a local governed starter.
- Add release validation and distribution automation.
- Decide whether additional renderer backends are in scope. If yes, treat that
  as a major public contract feature, not an internal implementation detail.
- Reassess the public API breadth after real consumers use the package. The
  current surface is broad for a first version.

## Current Approach Summary

The project is disciplined and ambitious. It is not a minimal UI experiment; it
is a framework plus governance system. V2 succeeds at making template generation
real and testable, not just documented. It also keeps the V1 command workflow
intact while adding template packaging, dependency governance, generated
guidance checks, and drift detection.

The codebase is closer to "lean with hotspots" than to "organically grown
spaghetti." The public package structure is clean, the `.fsi` discipline is
excellent, tests are substantial, and most modules use ordinary F#. The main
technical debt is concentration: the renderer host and the build workflow are
doing too much in single files. Those areas should be refactored before the
project adds another renderer, a broader release workflow, or more template
profiles.

The V2 answer is therefore: yes, it achieves the scoped non-visual template
packaging and governance goals; no, it does not yet prove full visual runtime
quality or release/distribution readiness; and the most valuable next work is
to keep the governance model while reducing the size and lifetime complexity of
the renderer and build-script implementation.
