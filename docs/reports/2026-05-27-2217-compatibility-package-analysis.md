---
title: Compatibility Package Analysis
index: 10
description: Detailed phase 5 analysis for deciding the long-term role of the FS.Skia.UI compatibility package.
---

# Compatibility Package Analysis

This report expands phase 5 from the refactoring analysis: decide what to do
with the `FS.Skia.UI` compatibility package after the lower-risk cleanup phases
are complete. The package is a meaningful design problem because it is both a
public surface and a historical container for capabilities that now have more
focused packages.

The recommendation is not to restructure the package during the first
behavior-preserving refactor. Treat compatibility package direction as its own
design feature with explicit consumer inventory, migration policy, package
surface evidence, and release notes.

## Executive Position

`FS.Skia.UI` should remain stable until the repository can answer three
questions with evidence:

1. Which current consumers still open `FS.Skia.UI` directly?
2. Which public types in `FS.Skia.UI` are compatibility aliases, and which are
   still the only available public contract for a capability?
3. Is the intended future of `FS.Skia.UI` to be a permanent broad package, a
   facade over split packages, or a deprecated migration bridge?

The safest near-term decision is to keep the package as a compatibility
surface, stop adding new primary concepts to it unless compatibility requires
them, and move new authoring guidance toward the focused packages. That
preserves existing consumers while preventing the compatibility package from
becoming the default home for new framework work.

## Current Package Shape

The package project is [src/Lib/Lib.fsproj](../src/Lib/Lib.fsproj). It produces
the package identity `FS.Skia.UI` and assembly name `FS.Skia.UI`.

The current compile set is:

| File | Role |
|------|------|
| [InternalsVisibleTo.fs](../src/Lib/InternalsVisibleTo.fs) | Test and internal access policy. |
| [VulkanResources.fsi](../src/Lib/VulkanResources.fsi) and [VulkanResources.fs](../src/Lib/VulkanResources.fs) | Vulkan resource contracts and implementation. |
| [VulkanStartup.fsi](../src/Lib/VulkanStartup.fsi) and [VulkanStartup.fs](../src/Lib/VulkanStartup.fs) | Vulkan startup contracts and implementation. |
| [Library.fsi](../src/Lib/Library.fsi) and [Library.fs](../src/Lib/Library.fs) | Broad public UI, scene, viewer, diagnostics, parity, and runtime surface. |
| [KeyboardInput.fsi](../src/Lib/KeyboardInput.fsi) and [KeyboardInput.fs](../src/Lib/KeyboardInput.fs) | Compatibility keyboard input surface and implementation. |

The package references `Fable.Elmish`, Silk.NET window/input/Vulkan packages,
SkiaSharp native assets, and `YamlDotNet`. That dependency set is much broader
than the newer focused authoring packages need.

The focused packages already separate major responsibilities elsewhere:

| Focused package | Current role |
|-----------------|--------------|
| `FS.Skia.UI.Scene` | Immutable scene primitives and deterministic evidence. |
| `FS.Skia.UI.SkiaViewer` | Viewer host, persistent window behavior, screenshot and visual evidence. |
| `FS.Skia.UI.Elmish` | Elmish integration for viewer programs. |
| `FS.Skia.UI.KeyboardInput` | Keyboard runtime, command configuration, diagnostics, and state display. |
| `FS.Skia.UI.Layout` | Layout evaluation and graph layout support. |
| `FS.Skia.UI.Controls` | Controls, charts, graph views, DataGrid, and rich rendering. |
| `FS.Skia.UI.Controls.Elmish` | Controls-specific Elmish adapter. |
| `FS.Skia.UI.Testing` | Generated product validation and evidence helpers. |

This means `FS.Skia.UI` is no longer the only logical package boundary for
most capabilities, but it may still be the easiest package for older samples,
consumers, or compatibility smoke tests.

## Why Phase 5 Must Be Separate

Compatibility work carries a different risk profile from file extraction.
Moving internals behind unchanged signatures is mostly a maintainability
exercise. Changing what `FS.Skia.UI` means is a product and ecosystem decision.

Phase 5 affects:

- package identity expectations,
- dependency closure for downstream consumers,
- namespace and open-module habits in existing code,
- generated template guidance,
- sample project references,
- package surface baselines,
- migration documentation,
- release compatibility promises.

Those concerns need explicit acceptance criteria. They should not ride along
with template cleanup, build script decomposition, or viewer internal module
splits.

## Consumer Inventory Needed

Before choosing a direction, collect a concrete inventory of current usage.

### Repository Consumers

Known repository references include:

| Consumer | Current signal |
|----------|----------------|
| [samples/BasicViewer](../samples/BasicViewer/) | References `src/Lib/Lib.fsproj` locally and package `FS.Skia.UI` for packaged mode, then opens `FS.Skia.UI`. |
| [samples/ScreenshotGallery](../samples/ScreenshotGallery/) | References `src/Lib/Lib.fsproj` locally and package `FS.Skia.UI` for packaged mode. |
| Documentation | Architecture and subsystem docs describe `src/Lib` as the compatibility core package. |
| Older design docs | V3 design material frames split packages as the preferred long-term structure. |

The next pass should produce a table of every repository `ProjectReference`,
`PackageReference`, namespace open, sample, template fragment, and docs page
that still depends on `FS.Skia.UI`.

### External Consumers

External usage is harder to prove from the repository. The design decision
should still define a migration posture for unknown consumers:

- keep package identity stable for at least one migration window,
- preserve documented public members until replacement guidance exists,
- keep obsolete guidance explicit if deprecation is chosen,
- avoid silent dependency or behavior removals in preview packages.

## Public Surface Classification

The compatibility package should be classified member-by-member before any
migration plan is accepted. A useful classification is:

| Classification | Meaning | Default action |
|----------------|---------|----------------|
| Primary-only compatibility member | Public member exists only in `FS.Skia.UI`. | Keep until a focused replacement exists and is documented. |
| Duplicate of focused package concept | Public member has an equivalent in a focused package. | Prefer focused package for new docs; keep compatibility member stable. |
| Facade candidate | Compatibility member can delegate to a focused package without behavior change. | Consider internal delegation after tests prove parity. |
| Deprecated candidate | Member has a better replacement and low active usage. | Deprecate only with migration guidance and surface evidence. |
| Permanent compatibility surface | Member is intentionally retained for old consumers. | Document as compatibility-owned and freeze semantics. |

This inventory matters because the file size in [Library.fs](../src/Lib/Library.fs)
is not enough evidence to remove or migrate public members. Large public files
can still represent valid compatibility commitments.

## Strategic Options

### Option A: Keep `FS.Skia.UI` As A Permanent Broad Package

Under this option, `FS.Skia.UI` remains a fully supported package that exposes a
large integrated surface. Focused packages still exist for lighter consumers,
but the broad package is not deprecated.

Advantages:

- lowest disruption for existing consumers,
- easiest package choice for small samples,
- avoids migration churn while the framework is still evolving,
- keeps old docs and examples mostly valid.

Costs:

- the package keeps a broad dependency closure,
- maintainers must preserve overlapping concepts indefinitely,
- new contributors may treat `FS.Skia.UI` as the preferred package rather than
  a compatibility surface,
- drift risk remains between broad and focused package concepts.

This option is reasonable if external compatibility matters more than package
minimalism. It still needs a rule that new primary concepts start in focused
packages unless the broad package intentionally re-exports them.

### Option B: Make `FS.Skia.UI` A Facade Over Focused Packages

Under this option, `FS.Skia.UI` remains the public package identity but its
implementation delegates to or re-exports focused package concepts where
possible.

Advantages:

- preserves the package name for existing consumers,
- reduces duplicate implementation over time,
- can align behavior with focused packages without forcing immediate migration,
- creates a bridge toward a cleaner package graph.

Costs:

- facade design can introduce dependency cycles if package directions are not
  controlled,
- F# type identity matters; aliases, wrappers, and duplicate record shapes are
  not interchangeable by accident,
- preserving binary/source compatibility may require old types to remain even
  when focused package equivalents exist,
- package size and dependency closure may remain broad if the facade references
  many focused packages.

This is likely the best long-term direction if type identity and dependency
direction can be handled cleanly. It should start with low-risk delegation
behind unchanged signatures, not with public type substitution.

### Option C: Deprecate `FS.Skia.UI` In Favor Of Focused Packages

Under this option, `FS.Skia.UI` becomes a migration bridge and new users are
directed to focused packages only.

Advantages:

- clearest long-term package story,
- lighter dependency closure for new consumers,
- less conceptual duplication in documentation,
- focused packages become the obvious authoring path.

Costs:

- highest migration burden for existing users,
- requires strong replacement coverage for every documented scenario,
- may force sample and docs churn before the focused packages have fully
  absorbed old functionality,
- risks breaking consumers if deprecation turns into removal too early.

This option should not be chosen until replacement docs, samples, and package
surface baselines prove that focused packages cover the current compatibility
scenarios.

### Option D: Freeze `FS.Skia.UI` And Move Only New Work Elsewhere

Under this option, the compatibility package remains supported but receives no
new primary capabilities unless needed to preserve existing behavior. Focused
packages become the home for new authoring.

Advantages:

- low immediate risk,
- prevents the broad package from growing further,
- gives maintainers time to inventory public members,
- supports either a future facade or deprecation decision.

Costs:

- duplicate implementation remains for now,
- docs must be clear or users may still choose the broad package by default,
- maintainers need discipline when adding features that feel "core".

This is the recommended next posture before a full phase 5 design project. It
does not solve the compatibility package, but it stops making the problem worse.

## Recommended Decision Path

Use a staged decision, not a one-step migration.

### Stage 1: Freeze And Inventory

Keep `FS.Skia.UI` behavior stable. Add no new primary feature surface to the
compatibility package unless a compatibility scenario requires it.

Produce:

- a repository consumer inventory,
- a public member classification table,
- a focused replacement map,
- a dependency closure report,
- package surface baseline status.

Acceptance:

- every current repository consumer of `FS.Skia.UI` is named,
- every public compatibility area has an owner classification,
- unknown external consumers are handled by a conservative migration policy.

### Stage 2: Replacement Coverage

For each compatibility capability, identify whether a focused package already
covers it.

Produce:

- sample migration examples for representative scenarios,
- docs that tell new users which package to choose,
- tests proving focused packages cover the intended replacement paths.

Acceptance:

- common sample flows can be written without `FS.Skia.UI`,
- generated templates do not require the broad package for new profiles unless
  deliberately selected,
- docs explain when `FS.Skia.UI` is compatibility-only.

### Stage 3: Facade Feasibility

Try internal delegation for low-risk areas where public signatures can remain
unchanged.

Acceptance:

- no public signature changes,
- no package cycles,
- no changed diagnostics or rendering behavior,
- surface baselines remain stable,
- focused package tests and compatibility package tests both pass.

### Stage 4: Deprecation Decision

Only after replacement coverage and facade feasibility are known, decide
whether to keep, facade, or deprecate `FS.Skia.UI`.

Acceptance:

- migration guide exists,
- release notes define the compatibility window,
- samples and templates point new users to the intended packages,
- package surface and FSI transcript evidence are updated deliberately.

## Technical Risk Areas

### Type Identity

F# records and discriminated unions with the same fields are still different
types when they come from different assemblies or namespaces. A migration from
`FS.Skia.UI.Rect` to `FS.Skia.UI.Scene.Rect` is not just a namespace edit unless
the public contract is designed around aliases or conversion helpers.

Risk response:

- avoid public type substitution in compatibility cleanup,
- prefer adapters or conversion helpers when preserving old signatures,
- test source compatibility with representative consumer projects.

### Dependency Direction

The broad package currently owns a wide dependency set. A facade strategy could
make `FS.Skia.UI` depend on focused packages, but focused packages must not
depend back on `FS.Skia.UI` in a way that creates cycles or makes the split
meaningless.

Risk response:

- document allowed package dependency direction before facade work,
- use dependency reports as a gate,
- avoid moving shared concepts into the compatibility package.

### Runtime Behavior

The compatibility package includes Vulkan startup, viewer behavior, diagnostics,
and keyboard input. Even if focused packages offer equivalent capabilities, old
runtime behavior may have exact diagnostic text, unsupported-host behavior, or
startup failure semantics that tests and users rely on.

Risk response:

- treat diagnostics and unsupported-host behavior as observable behavior,
- run compatibility smoke tests before and after each internal delegation,
- preserve failure categories and report wording unless a separate feature
  authorizes a change.

### Documentation Drift

If docs call `FS.Skia.UI` both "core" and "compatibility", users will receive
conflicting guidance.

Risk response:

- update the design map with a single package-selection story,
- mark old examples as compatibility examples when they intentionally use the
  broad package,
- keep focused-package authoring guidance near generated template docs.

## Evidence Required For A Phase 5 Feature

A future phase 5 implementation plan should require these readiness artifacts:

| Evidence file | Purpose |
|---------------|---------|
| `readiness/compatibility-consumer-inventory.md` | Lists repository consumers and package/reference usage. |
| `readiness/compatibility-public-surface-map.md` | Classifies public members and replacement coverage. |
| `readiness/compatibility-dependency-report.md` | Shows dependency closure before and after any change. |
| `readiness/compatibility-sample-migration.md` | Demonstrates representative sample migration paths. |
| `readiness/compatibility-surface-baseline.md` | Records package surface baseline status and intentional differences. |
| `readiness/compatibility-release-policy.md` | States whether the package is kept, facaded, frozen, or deprecated. |

These artifacts should be real review inputs. A phase 5 plan should not rely on
"file got smaller" as proof of a correct compatibility decision.

## Acceptance Criteria For Compatibility Work

Compatibility package work is acceptable only when:

- existing repository consumers either keep working unchanged or have an
  explicit migration commit,
- package surface changes are intentional and documented,
- focused replacement packages are named for every migrated scenario,
- generated templates do not accidentally regain a broad-package dependency,
- samples continue to build in both local project-reference and packaged modes,
- unsupported-host and viewer diagnostic behavior remains explicit,
- release notes explain the compatibility posture in user-facing language,
- dependency reports show no accidental package cycles or broad dependency
  spread into focused packages.

## Anti-Goals

Do not use compatibility package review to:

- remove old public APIs just because focused packages now exist,
- make `FS.Skia.UI.Scene` or other focused packages depend on the broad
  compatibility package,
- hide breaking changes behind internal refactoring language,
- weaken package surface baselines,
- collapse generated profiles into a single broad dependency set,
- rewrite runtime behavior while claiming a package cleanup,
- change the migration story without docs and release notes.

## Final Recommendation

For the current refactoring feature, phase 5 should remain deferred. The next
responsible action is to freeze the compatibility package as a compatibility
surface, inventory its consumers and public members, and direct new authoring
guidance toward focused packages.

After generated evidence cleanup, template splitting, build decomposition, and
viewer internal boundaries are stable, open a dedicated compatibility package
feature. That feature should decide between permanent broad package, facade,
freeze, or deprecation based on consumer inventory and replacement coverage, not
on file size alone.
