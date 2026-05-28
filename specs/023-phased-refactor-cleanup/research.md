# Research: Phased Refactor Cleanup

## Decision: Treat This As A Tier 2 Behavior-Preserving Refactor

The cleanup will preserve public `.fsi` signatures, surface baselines, package
IDs, generated profile names, command names, evidence report fields, status
vocabulary, output paths, exit-code meanings, FAKE target names, and readiness
artifact paths.

**Rationale**: The refactoring analysis identifies co-location and duplication
as the primary debt, not a missing public model. Keeping contracts stable lets
existing verification prove that extraction did not change behavior.

**Alternatives considered**: Combine cleanup with compatibility package
restructuring. Rejected because compatibility behavior is close to public API
and requires a separate migration design.

## Decision: Classify Duplication Before Consolidating Helpers

Duplicate helpers will be classified as intentional template copy,
package-boundary copy, repository-local duplication, or drift-prone semantic
copy before deletion or consolidation.

**Rationale**: Generated products must remain standalone and package boundaries
must not be crossed casually. A classification step prevents cleanup from
creating inappropriate dependencies.

**Alternatives considered**: Move all helpers to one shared utility package.
Rejected because the feature explicitly avoids new shared packages and public
dependency churn.

## Decision: Clean Generated Evidence Reports Before Splitting Files

Generated product evidence commands will first route through one local report
writer while the source layout is still familiar. File splitting follows after
report behavior is stable.

**Rationale**: Report semantics are the highest-risk generated behavior. Keeping
file movement separate from semantic consolidation makes regressions easier to
isolate.

**Alternatives considered**: Split all generated files first. Rejected because
compile-order and conditional-profile failures would obscure report behavior
changes.

## Decision: Split Generated Product Source By Responsibility

Generated product source will move toward responsibility-specific files:
model/update state, rendering description, layout evidence, evidence commands,
window options, and entrypoint/dispatch.

**Rationale**: `template/base/src/Product/Program.fs` has become the most
visible template-consumer hotspot. A generated entrypoint should show launch and
command dispatch without unrelated evidence and profile logic.

**Alternatives considered**: Leave the single file and add region comments.
Rejected because F# compile order and generated profile ownership are clearer
when responsibilities are represented as files.

## Decision: Decompose Build Governance Into Loaded Scripts

`build.fsx` will remain the stable FAKE command surface while helper logic moves
incrementally into loaded scripts under `scripts/build/`.

**Rationale**: Target names, dependencies, and final orchestration are the
public build contract. Helper extraction improves maintainability without
renaming commands or changing user-facing behavior.

**Alternatives considered**: Rewrite build orchestration as a compiled tool.
Rejected as too broad for behavior-preserving cleanup and likely to change
operational behavior.

## Decision: Split SkiaViewer Internals Behind The Existing Facade

SkiaViewer diagnostics, visual evidence, host capability checks, scene
conversion, and window behavior validation can move to internal implementation
files while `SkiaViewer.fsi` and the public `Viewer` facade remain unchanged.

**Rationale**: Screenshot and visual evidence changes are risky because
diagnostics and host classification are co-located with window runtime code.
Internal file boundaries reduce future review scope without changing contracts.

**Alternatives considered**: Change public viewer contracts to expose the new
subdomains. Rejected because no public behavior change is authorized.

## Decision: Defer Compatibility Package Review

`src/Lib` compatibility package restructuring is not part of this cleanup.

**Rationale**: The compatibility package is public-facing and migration-sensitive.
It should be handled only after generated/template and viewer-internal cleanup
are stable, with separate compatibility guidance.

**Alternatives considered**: Include compatibility cleanup as the final phase.
Rejected because the current feature must not change public package signatures
or package strategy.
