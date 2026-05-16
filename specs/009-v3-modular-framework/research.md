# Research: V3 Modular Framework

No unresolved clarifications remain from the specification. The decisions below
resolve the planning unknowns and set implementation boundaries.

## Capability Catalog As Source Of Truth

**Decision**: Introduce `template/capabilities.yml` as the maintained catalog
for every selectable capability. Each entry records package id, project path,
public contract files, test projects, local skill path, template fragment,
dependency capabilities, default-profile inclusion, generated-product evidence,
and surface baseline path.

**Rationale**: V3 adds multiple moving parts per capability. A manifest gives
tests and template generation one structured source of truth and makes drift
detectable before implementation review.

**Alternatives considered**: Encoding capability ownership only in project
files was rejected because skills and template fragments would remain implicit.
Encoding it only in documentation was rejected because validation needs
machine-readable data.

## Package Boundary Order

**Decision**: Split from the current broad core package in dependency order:
Scene first, then SkiaViewer, Elmish, KeyboardInput, Layout, Charts, and Testing
helpers. Existing Layout and Charts packages are retargeted to Scene ownership
after Scene exists.

**Rationale**: Scene is the dependency-light base that all other runtime
packages consume. Splitting adapter and widget packages before Scene would keep
heavy dependencies and public surface ownership tangled.

**Alternatives considered**: A one-shot package rewrite was rejected because it
would be difficult to review and would obscure public surface regressions.
Keeping the current broad core package as the base was rejected because it keeps
Elmish/native/input concerns coupled to every consumer.

## Default App Capability Set

**Decision**: The default generated app includes Scene, SkiaViewer, Elmish,
KeyboardInput, Layout, and Charts. Samples remain opt-in.

**Rationale**: The user selected the complete application capability set for
the default app. This makes the default generated product feature-complete for
normal UI application work while still keeping generated source and samples
lean.

**Alternatives considered**: Scene-only, Scene plus SkiaViewer, and Scene plus
SkiaViewer plus Elmish defaults were rejected by clarification. Including
samples by default was rejected because samples are framework learning and
regression assets, not product-owned code.

## Full Product Governance By Default

**Decision**: Generated products include full product governance by default:
Spec Kit artifacts, evidence gates, generated guidance checks, drift checks,
readiness workflow, and local selected skills. Generated products do not run
framework-source maintenance checks such as framework galleries, parity suite,
framework package-surface maintenance, or template packaging.

**Rationale**: The user selected full governance for generated products. The
important distinction is product governance versus framework maintenance:
generated products should be strict about their own work without inheriting
framework repository upkeep.

**Alternatives considered**: Light governance was rejected by clarification.
No governance was rejected because it would discard the framework's operating
model. Running all framework maintenance checks in generated products was
rejected because it violates the lean generated-product goal.

## Generated Product Content Policy

**Decision**: Generate products from a small base plus capability fragments.
Default output includes one product app, one product test suite, product docs,
command wrappers, full product governance assets, selected local skills, and
package references for the default capabilities. Default output excludes
framework samples, galleries, historical specs, readiness evidence, framework
docs, framework README content, and framework implementation projects.

**Rationale**: The core V3 value is avoiding framework-repo copies. File-list
validation can prove the generated output stays clean.

**Alternatives considered**: Continuing the current source-copy template was
rejected because it preserves the exact problem. A generated product with no
governance was rejected by clarification.

## Local Skill Distribution

**Decision**: Every capability owns a package-local `skill/SKILL.md`, and
template generation copies only the project skill plus skills for selected or
required capabilities.

**Rationale**: Skills are part of capability governance. Keeping them beside
their packages makes ownership reviewable, while selected copying keeps
generated products focused.

**Alternatives considered**: A single monolithic framework skill was rejected
because it would recreate broad-framework coupling. Copying all skills into all
generated products was rejected because it gives agents irrelevant instructions.

## Package-Specific Public Surface Baselines

**Decision**: Maintain public surface baselines per public capability package.
Package surface validation fails on accidental exports, missing `.fsi` coverage,
or dependency leaks into the wrong package.

**Rationale**: V3 changes package boundaries, so an aggregate baseline is too
coarse. Reviewers need to see which package's contract changed.

**Alternatives considered**: Keeping only the existing aggregate baseline was
rejected because package ownership changes would be hidden. Relying on compile
alone was rejected because compile success does not prove public surface intent.

## Sample Pack Handling

**Decision**: Samples are excluded from the default app and supplied through an
explicit sample profile or sample-pack selection.

**Rationale**: Samples are useful framework assets but should not be mistaken
for product-owned code.

**Alternatives considered**: Including samples in the default app was rejected
because it conflicts with lean product generation. Removing samples from the
framework repository was rejected because they remain useful regression and
learning assets.

## V2 Migration Exclusion

**Decision**: Do not implement V2 migration support in this feature. Create a
compatibility-impact readiness record that states V3 changes package/template
ownership and that no V2 migration path is provided here.

**Rationale**: The user explicitly clarified that V2 migration is unnecessary.
The constitution still requires compatibility impact to be visible, so the
record is kept without expanding scope into migration implementation.

**Alternatives considered**: Providing a full migration guide was rejected by
clarification. Omitting compatibility impact entirely was rejected because this
is a Tier 1 public package/template change.
