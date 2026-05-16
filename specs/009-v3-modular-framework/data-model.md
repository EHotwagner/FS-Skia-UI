# Data Model: V3 Modular Framework

## Capability

- **Fields**: id, display name, package id, project path, public contract files,
  source roots, test projects, local skill path, template fragment path,
  dependency capability ids, default app inclusion, supported profiles, evidence
  classes, surface baseline path, owner notes
- **Relationships**: Belongs to `CapabilityCatalog`; owns one `CapabilitySkill`;
  owns zero or more `TemplateFragment` entries; depends on zero or more other
  `Capability` records; produces `PackageSurfaceBaseline` and
  `CapabilityValidationResult`.
- **Validation Rules**: Every selectable capability must declare package,
  contracts, tests, skill, fragment, dependencies, docs, and validation path.
  Dependency references must exist in the catalog and must not create cycles.

## Capability Catalog

- **Fields**: catalog version, schema version, default app capabilities,
  supported profiles, capability entries, evidence expectations
- **Relationships**: Drives template composition, selected skill copying,
  generated product validation, dependency checks, and package surface checks.
- **Validation Rules**: Default app capabilities must equal Scene, SkiaViewer,
  Elmish, KeyboardInput, Layout, and Charts. Every profile must resolve to a
  closed set of capability dependencies. The catalog must be readable by FAKE
  validation without relying on prose.

## Distributable Library

- **Fields**: package id, assembly name, project path, version, title,
  description, public signature files, direct dependencies, packable flag,
  package tags, surface baseline path
- **Relationships**: Implements one primary `Capability`; consumes dependency
  `DistributableLibrary` outputs; has one `PackageSurfaceBaseline`.
- **Validation Rules**: Public modules require curated `.fsi` files. Scene must
  not depend on Elmish, Silk.NET, SkiaSharp, Yoga.Net, or YamlDotNet. Public
  package exports must match the package-specific baseline.

## Capability Skill

- **Fields**: skill id, source path, generated destination path, owning
  capability id, scope, public contract guidance, build commands, test commands,
  evidence rules, agent boundary guidance
- **Relationships**: Owned by one `Capability`; copied into `GeneratedProduct`
  when selected directly or required as a dependency.
- **Validation Rules**: A skill must name owned areas, public contract checks,
  verification commands, and evidence expectations. Generated products must not
  receive skills for unselected/unrequired capabilities.

## Template Fragment

- **Fields**: fragment id, source path, target path, owning capability id,
  included files, excluded files, profile conditions, generated parameters
- **Relationships**: Selected by `TemplateProfile` through `CapabilityCatalog`;
  contributes to `GeneratedProduct`.
- **Validation Rules**: Fragments must not copy framework implementation source
  into default consumer products. Fragment inclusion must be deterministic for a
  profile and capability set.

## Template Profile

- **Fields**: name, description, default capability ids, optional capability
  ids, governance level, sample inclusion, source-framework mode, validation
  commands
- **Relationships**: Resolves capabilities through `CapabilityCatalog`; creates
  one or more `GeneratedProduct` validation rows.
- **Validation Rules**: Default app profile includes Scene, SkiaViewer, Elmish,
  KeyboardInput, Layout, and Charts with full product governance and no samples.
  Sample pack profile is the only profile that may include samples by default.

## Generated Product

- **Fields**: name, profile, selected capabilities, resolved capabilities,
  generated root, package references, copied skills, generated docs, command
  targets, file list, validation logs
- **Relationships**: Created from `TemplateProfile`; validated by
  `GeneratedProductValidation`; owns product-level Spec Kit governance assets.
- **Validation Rules**: Default generated products contain exactly one product
  app and one product test suite unless the profile explicitly allows more.
  They must not contain framework samples, galleries, historical specs,
  framework readiness evidence, framework docs, framework README content, or
  framework implementation projects.

## Generated Product Governance

- **Fields**: governance level, included Spec Kit assets, evidence gate
  commands, drift check commands, generated guidance check commands, readiness
  paths, excluded framework maintenance checks
- **Relationships**: Belongs to a `GeneratedProduct`; contributes to
  `GeneratedProductValidation`.
- **Validation Rules**: Default generated products include full product
  governance: evidence gates, drift checks, generated guidance checks, and
  readiness workflow. They must not run framework gallery, parity,
  framework package-surface maintenance, or framework template packaging checks.

## Package Surface Baseline

- **Fields**: package id, baseline path, captured public modules/types/values,
  capture command, comparison command, approval notes
- **Relationships**: Belongs to one `DistributableLibrary`; supports
  `ValidationReport`.
- **Validation Rules**: Every public V3 capability package must have a baseline
  or an explicit no-public-surface record. Diffs require feature evidence and
  reviewer approval.

## Validation Report

- **Fields**: report class, path, produced by command, covered profile or
  capability, pass/fail verdict, observed files, unexpected files, missing
  metadata, dependency leaks, notes
- **Relationships**: Evidence for `Capability`, `GeneratedProduct`,
  `CapabilitySkill`, `PackageSurfaceBaseline`, and template drift checks.
- **Validation Rules**: Failures must identify the capability/profile/path that
  caused the issue and the missing or unexpected artifact class.

## Compatibility Impact Record

- **Fields**: path, affected packages, affected generated products, public
  surface impact, package identity impact, V2 migration stance, reviewer notes
- **Relationships**: Required readiness artifact for this Tier 1 change.
- **Validation Rules**: Must state that V2 migration support is out of scope and
  must not prescribe implementation tasks for V2 migration.
