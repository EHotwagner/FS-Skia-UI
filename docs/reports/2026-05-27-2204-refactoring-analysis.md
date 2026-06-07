---
title: Refactoring Analysis
index: 9
description: Maintainability assessment, duplication inventory, and phased refactoring plan.
---

# Refactoring Analysis

This report assesses whether the repository warrants refactoring after the
Breakout demo feedback work. The answer is yes, but the refactor should be
targeted. The repository already has a useful package split and strong
governance gates; the main problem is not architectural absence, it is localized
accumulation in orchestration files, generated template source, compatibility
runtime code, and evidence/report helper duplication.

## Executive Assessment

Refactoring is warranted in three areas:

1. Evidence and report-writing helpers are repeated across generated product
   code, viewer code, testing helpers, build automation, and test support.
2. Several files have become coordination hubs that carry unrelated
   responsibilities in one compilation unit.
3. Generated template source is doing too much in one `Program.fs`, which makes
   generated guidance harder to audit and increases risk when adding evidence
   commands.

Refactoring is not warranted as a broad rewrite. The package boundaries are
mostly coherent, test coverage is broad, and many apparent repetitions are the
result of deliberate package or template isolation. The correct first move is a
behavior-preserving extraction pass with public API stability as a hard
constraint.

## Evidence Snapshot

The largest implementation files are concentrated in a small number of areas:

| File | Lines | Primary concern |
|------|------:|-----------------|
| [build.fsx](../build.fsx) | 4071 | Target model, process execution, template packaging, generated scanning, package validation, evidence writing, and process-health policy all live in one script. |
| [src/Lib/Library.fs](../src/Lib/Library.fs) | 2408 | Compatibility package combines scene helpers, parity reporting, Vulkan host implementation, and legacy viewer runtime. |
| [src/SkiaViewer/SkiaViewer.fs](../src/SkiaViewer/SkiaViewer.fs) | 2381 | Public viewer implementation, legacy conversions, diagnostics, window lifecycle, app hosting, visual evidence, and screenshot evidence are co-located. |
| [tests/SkiaViewer.Tests/Tests.fs](../tests/SkiaViewer.Tests/Tests.fs) | 1452 | Large behavioral test surface mirrors the large viewer implementation surface. |
| [src/Lib/KeyboardInput.fs](../src/Lib/KeyboardInput.fs) | 1398 | YAML parsing, validation, runtime update, display model, rendering, and bigram analysis share one module. |
| [template/base/src/Product/Program.fs](../template/base/src/Product/Program.fs) | 1276 | Multiple template profiles, product app state, layout evidence, visual evidence, launch evidence, screenshot evidence, and CLI dispatch live together. |
| [src/Testing/Testing.fs](../src/Testing/Testing.fs) | 879 | Generated product assertions, consumer validation, layout validation, warning classification, readiness discovery, and report helpers share one file. |
| [src/Scene/Scene.fs](../src/Scene/Scene.fs) | 773 | Scene DSL, deterministic evidence, shape evidence, and layout evidence classification are combined. |

The concentration itself is not automatically wrong. The issue is that several
files now combine type contracts, infrastructure, validation policy, host
effects, and generated workflow details. That makes future feature work more
expensive because a small evidence or template change requires reading a large
unrelated surface.

## Current Architecture Strengths

The existing structure gives a good base for incremental refactoring:

- Package directories are already separated by public capability:
  [Scene](../src/Scene/), [SkiaViewer](../src/SkiaViewer/),
  [Testing](../src/Testing/), [Layout](../src/Layout/),
  [Controls](../src/Controls/), [KeyboardInput](../src/KeyboardInput/), and
  [Controls.Elmish](../src/Controls.Elmish/).
- Public package contracts are declared through `.fsi` files and protected by
  surface baselines in [readiness/surface-baselines](../readiness/surface-baselines/).
- Template validation, generated guidance checks, package surface checks, FSI
  transcripts, and evidence graph/audit targets already exist behind FAKE.
- The generated app guidance now distinguishes deterministic scene evidence,
  persistent viewer evidence, and screenshot evidence, which is the correct
  conceptual boundary.
- `FS.Skia.UI.Testing` now has normalized evidence report helpers, so one
  canonical report shape exists and should become the anchor for further cleanup.

These strengths argue for a conservative refactor. Preserve contracts, move
responsibilities, and let existing verification detect accidental behavior
change.

## Debt Classes

### 1. Evidence And Report Duplication

There are multiple ways to create parent directories, write key-value reports,
detect images, and classify evidence results:

| Helper family | Examples |
|---------------|----------|
| Parent directory creation | `ensureParent` in [build.fsx](../build.fsx), `ensureParentDirectory` in [SkiaViewer.fs](../src/SkiaViewer/SkiaViewer.fs), local write helpers in generated product code. |
| Report writing | `EvidenceReports.write` in [Testing.fs](../src/Testing/Testing.fs), `writeEvidenceReport`, `writeLaunchEvidenceReport`, `writeLaunchFailureReport`, and `writeBoundedSmokeReport` in [template/base/src/Product/Program.fs](../template/base/src/Product/Program.fs), markdown/verdict writers in [build.fsx](../build.fsx). |
| Image checks | `isPngFile` in generated product tests and template source, `isPngPath` and `imageDecodable` in viewer implementation, output-field validation in testing helpers. |
| Geometry checks | Rectangle intersection and containment helpers appear in [Scene.fs](../src/Scene/Scene.fs) and generated product code. |
| Parsing helpers | `parseScalar` and `parseInlineList` exist in both [build.fsx](../build.fsx) and [tests/Governance.Tests/TestSupport.fs](../tests/Governance.Tests/TestSupport.fs). |
| Process execution | `runProcess` variants exist in [build.fsx](../build.fsx), [template/base/build.fsx](../template/base/build.fsx), smoke tests, and governance test support. |

Some duplication is acceptable. Generated projects should not be forced to
depend on repository-only test support, and package boundaries prevent every
helper from moving to one place. The problem is that there is no documented
policy that distinguishes intentional local copies from accidental copies.

Recommended policy:

- Runtime packages should use package-local internal helpers when the behavior
  is package-specific.
- Generated product code may keep a tiny local writer only when the generated
  profile does not reference `FS.Skia.UI.Testing`.
- Governed generated profiles that already reference `FS.Skia.UI.Testing`
  should use public report helper concepts instead of reimplementing report
  semantics.
- Build/test support should share repository-local helpers where possible,
  especially for scalar/list parsing and process execution.

### 2. Oversized Coordination Modules

The largest files are not just long; they cross responsibility boundaries.

[build.fsx](../build.fsx) is the highest-value extraction target. It currently
contains:

- target state and update model,
- process execution,
- process-health thresholds,
- focused gate reports,
- template package validation,
- generated project instantiation and scanning,
- capability catalog parsing,
- generated package resolution,
- consumer validation,
- evidence report writing,
- task graph and audit orchestration.

The FAKE entrypoint should remain the stable public command surface, but helper
logic should move into loaded script modules. This keeps `build.fsx` readable as
the target graph rather than a repository-wide implementation container.

[src/SkiaViewer/SkiaViewer.fs](../src/SkiaViewer/SkiaViewer.fs) is the main
runtime hotspot. It contains:

- conversion between `FS.Skia.UI.Scene` and legacy `FS.Skia.UI` scene types,
- diagnostic filtering and dispatch,
- window behavior validation,
- desktop session detection,
- lifecycle state classification,
- Silk.NET window operations,
- bounded run evidence,
- visual evidence artifact generation,
- generated app host interpretation,
- screenshot evidence results.

The implementation has clear subdomains that can be split without changing the
public `.fsi`: diagnostics, host capability detection, visual evidence,
window-behavior validation, and generated app host adapter.

[template/base/src/Product/Program.fs](../template/base/src/Product/Program.fs)
is the main generated-source hotspot. It mixes profile-conditional source,
sample product behavior, command-line evidence generation, and report writing.
The template should generate multiple source files so each profile still
produces simple code. A generated `Program.fs` should read as a product
entrypoint, not as a mini framework.

### 3. Compatibility Package Accumulation

[src/Lib/Library.fs](../src/Lib/Library.fs) is large because it remains the
compatibility package for lower-level paths. It includes public scene-like
types, parity reporting, Vulkan host runtime, and legacy viewer behavior.

This file is a real hotspot, but it is not the safest first refactor. It is
close to public API compatibility, and the newer split packages already depend
on it in some places. The better sequence is:

1. stabilize evidence/template cleanup,
2. split SkiaViewer implementation internals,
3. only then consider a compatibility migration plan for `FS.Skia.UI`.

### 4. Generated Product Complexity

The generated product template currently supports multiple profiles from a
single source file using conditional comments. This creates two problems:

- readers see code that is irrelevant to their chosen profile,
- governance checks must scan a large file to validate one behavior.

The app profile also contains a small duplicated transition case:

```fsharp
| Paused, Escape -> Main, ...
| Paused, Escape -> Main, ...
```

That specific duplicate is low-risk to remove, but it is a symptom of the
larger issue: when the generated product file carries too many unrelated
examples, small mistakes become harder to see.

### 5. Test Support Mirrors Production Sprawl

Large tests are not inherently bad here because the repo depends on governance
and evidence checks. However, test helper duplication increases maintenance
cost:

- smoke tests, governance support, and generated product tests each run
  processes with local wrappers,
- scalar/list parsing helpers are duplicated between build logic and test
  support,
- image validation helpers appear in both viewer tests and generated tests.

Repository-local test support should be extracted where it does not blur package
boundaries. This is especially valuable for process execution and fixture
parsing because those helpers encode governance behavior.

## Refactoring Priorities

### Priority 1: Generated Evidence And Template Cleanup

This is the best first slice because it directly addresses duplication surfaced
by the Breakout feedback and has strong existing validation.

Actions:

- Split [template/base/src/Product/Program.fs](../template/base/src/Product/Program.fs)
  into generated files such as `Model.fs`, `View.fs`, `LayoutEvidence.fs`,
  `EvidenceCommands.fs`, `WindowOptions.fs`, and `Program.fs`.
- Keep `Program.fs` as the only CLI entrypoint.
- Keep generated command names and output fields stable.
- Route all generated evidence commands through one local report writer.
- Delete separate launch/image failure writers if the shared writer can express
  the same fields.
- Remove the duplicated `Paused, Escape` match case.
- Update [template/base/src/Product/Product.fsproj](../template/base/src/Product/Product.fsproj)
  compile order deliberately because F# source order is semantic.
- Update generated product tests only where they assert source-shape details
  that intentionally changed.

Expected payoff:

- generated source becomes inspectable by profile,
- report conventions have one local implementation point,
- future evidence commands are less likely to fork report behavior.

Primary risks:

- F# compile order errors,
- generated profile conditionals accidentally excluding a needed file,
- guidance checks tied to old source layout.

### Priority 2: Build Script Decomposition

The build script should remain the command surface, but its helper
implementation should be modular.

Actions:

- Introduce loaded scripts under `scripts/build/`, for example:
  - `Paths.fsx`
  - `Process.fsx`
  - `Reports.fsx`
  - `TemplateValidation.fsx`
  - `GeneratedScanning.fsx`
  - `PackageResolution.fsx`
  - `ProcessHealth.fsx`
- Move types and helpers only when they do not need FAKE target declarations.
- Keep target names, target dependencies, output paths, and readiness file names
  unchanged.
- Keep [build.fsx](../build.fsx) responsible for target registration,
  dependency wiring, and final command orchestration.

Expected payoff:

- reviewers can inspect target graph separately from validation internals,
- duplicated parsing/report/process helpers become easier to reuse,
- future target additions have a smaller blast radius.

Primary risks:

- F# script load order mistakes,
- hidden coupling through shared top-level values,
- subtle changes to active feature path discovery.

### Priority 3: SkiaViewer Internal Boundary Split

This should be done after template/evidence cleanup because it touches runtime
behavior.

Actions:

- Keep [src/SkiaViewer/SkiaViewer.fsi](../src/SkiaViewer/SkiaViewer.fsi)
  unchanged.
- Add internal implementation files before `SkiaViewer.fs` in
  [src/SkiaViewer/SkiaViewer.fsproj](../src/SkiaViewer/SkiaViewer.fsproj).
- Move legacy scene conversion into an internal conversion module.
- Move visual evidence functions into an internal visual evidence module.
- Move desktop session and runtime capability detection into an internal host
  capability module.
- Move window behavior validation and option result creation into an internal
  window behavior module.
- Keep the public `Viewer` module facade in `SkiaViewer.fs`.

Expected payoff:

- screenshot and visual evidence work becomes easier to modify,
- window lifecycle logic is easier to reason about,
- public API remains stable while implementation ownership improves.

Primary risks:

- internal module ordering and visibility,
- accidental behavior changes in host classification,
- test updates required where tests inspect exact diagnostic text.

### Priority 4: Testing Helper Consolidation

Actions:

- Extract governance process execution helpers from
  [tests/Governance.Tests/TestSupport.fs](../tests/Governance.Tests/TestSupport.fs)
  into smaller modules within the same test project.
- Share scalar/list parsing test helpers with fixture creation logic in that
  test project.
- Keep product-facing test assertions separate from repository governance
  assertions.

Expected payoff:

- governance tests become easier to extend,
- process execution semantics become easier to audit,
- generated product validation tests stop absorbing unrelated helper logic.

### Priority 5: Compatibility Package Review

This is a later, explicit design project, not a cleanup task.

Actions:

- Inventory which consumers still require the compatibility package
  [src/Lib](../src/Lib/).
- Decide whether `FS.Skia.UI` remains a compatibility package indefinitely or
  becomes a facade over split packages.
- If changing public compatibility behavior, write a migration guide and update
  package surface baselines deliberately.

Expected payoff:

- clearer long-term package story,
- less duplicated scene/viewer concept surface over time.

Primary risks:

- public API churn,
- package dependency cycles,
- disruption to generated templates and samples.

## Detailed Recommendations

### Prefer Extraction Over Abstraction

Most of the current debt comes from co-location, not from bad algorithms.
Extract files and modules first. Add new abstractions only when there are two or
more real callers and the contract is obvious.

Good first extractions:

- build process execution,
- build report writing,
- generated product evidence command helpers,
- SkiaViewer visual evidence helpers,
- SkiaViewer window behavior validation.

Avoid first:

- a universal repository utility package,
- a cross-package internal helper package,
- broad unification of all report formats,
- public API changes to support cleanup.

### Make Duplication Policy Explicit

Before deleting every duplicate helper, classify it:

| Classification | Meaning | Action |
|----------------|---------|--------|
| Intentional template copy | Needed so generated products are standalone or profile-light. | Keep, but keep tiny and documented. |
| Package-boundary copy | Prevents an inappropriate dependency between runtime packages. | Keep or move to the lower-level package only if dependency direction stays correct. |
| Repository-local duplication | Same behavior repeated in build/test support. | Consolidate. |
| Drift-prone semantic copy | Same report or evidence semantics repeated with different fields or status behavior. | Consolidate first. |

This policy matters because the repository intentionally produces standalone
generated products. Not every repeated helper is a bug.

### Keep Public API Stable During Cleanup

The first refactoring pass should not change:

- `.fsi` public signatures,
- package IDs,
- generated command names,
- generated evidence field names,
- readiness artifact paths,
- FAKE target names,
- target dependency semantics,
- generated profile names.

If a cleanup requires any of those changes, promote it to a separate feature
with explicit package surface and template migration evidence.

### Use The Existing Verification System As Refactor Guardrails

The repository already has the right gates for this work. A refactor should be
accepted only when the relevant gates prove behavior stability:

| Change area | Required checks |
|-------------|-----------------|
| Generated product source split | `TemplateCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, generated product tests |
| Public package internals only | Targeted package tests, `PackageSurfaceCheck`, `FsiTranscripts` |
| Build script decomposition | `Dev`, focused target checks, `Verify` if target graph or readiness paths change |
| Evidence/report behavior | `Testing.Tests`, generated evidence command tests, readiness convention checks |
| SkiaViewer internals | `SkiaViewer.Tests`, bounded smoke where host supports it, unsupported-host evidence checks |

## Proposed Implementation Sequence

### Phase 0: Baseline

- Capture current `git status`.
- Run the smallest relevant checks before editing:
  - `dotnet test tests/Testing.Tests/Testing.Tests.fsproj`
  - `dotnet test tests/Scene.Tests/Scene.Tests.fsproj`
  - `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj`
  - `./fake.sh build -t TemplateCheck`
- Record any pre-existing failure before refactoring so cleanup is not blamed
  for unrelated breakage.

### Phase 1: Generated Product Report Cleanup

- Keep all code in the same generated source file initially.
- Replace specialized report writers with one generated report writer.
- Preserve every existing command and field.
- Remove the duplicated paused-screen transition.
- Run generated product tests and `TemplateCheck`.

This phase gives a quick payoff with minimal file movement.

### Phase 2: Generated Product File Split

- Split generated source files by responsibility.
- Update `Product.fsproj` compile order.
- Keep `Program.fs` as the entrypoint and command dispatcher.
- Re-run generated template matrix checks.

This phase reduces the most visible template bloat.

### Phase 3: Build Script Internal Modules

- Extract script modules one responsibility at a time.
- After each extraction, run the focused FAKE target that owns the moved logic.
- Do not rename targets or readiness outputs.

This phase reduces the largest single repository maintenance hotspot.

### Phase 4: SkiaViewer Internals

- Extract internal modules behind the existing public facade.
- Keep tests focused on behavior, not private module structure.
- Run targeted viewer tests before broad verification.

This phase is higher risk and should wait until template/report cleanup is
stable.

### Phase 5: Compatibility Package Decision

- Treat this as a separate design decision.
- Decide whether compatibility remains a permanent public facade or begins a
  migration to split packages.
- Update docs and baselines only after the decision is explicit.

## Acceptance Criteria

A refactoring pass should be considered successful only if:

- public surface baselines are unchanged unless intentionally updated,
- generated commands produce the same required fields,
- generated templates instantiate and build for the same profiles,
- evidence reports preserve status vocabulary and exit-code semantics,
- unsupported screenshot behavior remains explicit and does not claim screenshot
  proof,
- FAKE target names and readiness paths remain stable,
- deleted duplication is covered by tests or generated validation.

## Anti-Goals

Do not use this refactor to:

- redesign the UI model,
- replace Skia/Silk runtime behavior,
- change public package signatures,
- remove compatibility package APIs,
- rewrite FAKE target semantics,
- introduce a shared utility package just for cleanup,
- collapse generated product profiles into one runtime dependency set,
- weaken evidence requirements to make cleanup easier.

## Final Recommendation

Proceed with a phased refactor, starting with generated evidence/report cleanup
and template file splitting. That path directly addresses the bloat visible to
template consumers and has the strongest verification coverage. Defer
SkiaViewer internal decomposition until the generated evidence path is stable.
Defer compatibility package restructuring until it has its own design decision
and migration plan.

The success metric is not fewer lines by itself. The goal is that future
evidence, screenshot, generated app, or template-profile changes can be reviewed
inside a small owned module instead of requiring a scan through several
thousand lines of mixed responsibilities.
