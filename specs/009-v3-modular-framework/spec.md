# Feature Specification: V3 Modular Framework

**Feature Branch**: `009-v3-modular-framework`  
**Created**: 2026-05-16  
**Status**: Draft  
**Input**: User description: "create specs for docs/v3Design.md"

## Clarifications

### Session 2026-05-16

- Q: Is V2 migration required for this feature? → A: No; V2 migration is out of scope.
- Q: Which capabilities are included in the default generated app? → A: Scene, SkiaViewer, Elmish, KeyboardInput, Layout, and Charts.
- Q: What governance level should generated products receive by default? → A: Full governance by default, including evidence gates, drift checks, generated guidance checks, and readiness workflow.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Generate a Lean Product (Priority: P1)

An application developer can create a new FS.Skia.UI product that contains only
product-owned application code, product tests, full product governance assets,
and references to the default framework capabilities: Scene, SkiaViewer,
Elmish, KeyboardInput, Layout, and Charts. The generated product does not look
like a copy of the framework repository.

**Why this priority**: This is the main user value of V3. A template that still
copies framework samples, framework docs, historical specifications, or
framework implementation projects does not solve the modularity problem.

**Independent Test**: Generate the default product profile and inspect the
result. The output is acceptable when it contains a runnable product shell and
does not contain framework samples, galleries, historical readiness artifacts,
or framework documentation copies.

**Acceptance Scenarios**:

1. **Given** a developer creates a default V3 product, **When** generation
   completes, **Then** the product contains one product application, one product
   test suite, product documentation, command wrappers, full Spec Kit governance
   assets, and references for Scene, SkiaViewer, Elmish, KeyboardInput, Layout,
   and Charts.
2. **Given** a developer creates a default V3 product, **When** they inspect the
   file tree, **Then** the product contains no framework sample gallery,
   framework parity suite, historical feature specification, readiness evidence,
   or copied framework README content.
3. **Given** a developer runs the generated product's standard verification
   command, **When** only the default capabilities are selected, **Then**
   verification checks the product, selected capability usage, evidence gates,
   drift checks, generated guidance checks, and readiness workflow without
   running framework gallery, parity, template packaging, or framework-source
   checks.

---

### User Story 2 - Select Framework Capabilities Explicitly (Priority: P2)

A developer can choose which FS.Skia.UI capabilities their product needs, such
as scene composition, Skia viewer hosting, Elmish integration, keyboard input,
layout, charts, testing helpers, or samples. The generated project includes
only the chosen capabilities and their required prerequisites.

**Why this priority**: Modularity is only useful if capability selection is
predictable. Users should not inherit keyboard input, charts, layout, native
viewer dependencies, or samples unless they chose a profile or option that
requires them.

**Independent Test**: Generate products with several capability selections and
compare their declared references, copied skills, command surface, and generated
files against the selected capability set.

**Acceptance Scenarios**:

1. **Given** a developer selects scene-only authoring, **When** the product is
   generated, **Then** the output excludes viewer hosting, Elmish integration,
   keyboard input, charts, layout, and sample assets unless separately selected.
2. **Given** a developer selects the default application profile, **When** the
   product is generated, **Then** Scene, SkiaViewer, Elmish, KeyboardInput,
   Layout, and Charts are included automatically.
3. **Given** a developer requests sample content, **When** the product is
   generated, **Then** sample assets are supplied through an explicit sample
   profile or sample pack rather than appearing in the default app profile.

---

### User Story 3 - Maintain Capability Ownership (Priority: P2)

A framework maintainer can see the owner, public contract, tests, local agent
skill, documentation entry, dependencies, and template fragment for every
reusable capability before approving a V3 change.

**Why this priority**: The design moves responsibility out of one broad package.
Without explicit ownership, modular packages and generated template fragments
will drift apart.

**Independent Test**: Review the capability catalog and run the capability
validation workflow. The workflow passes only when every selectable capability
has a complete ownership record and validation path.

**Acceptance Scenarios**:

1. **Given** a reusable capability is listed for V3, **When** maintainers review
   its ownership record, **Then** they can identify its distributable unit,
   public contract, test coverage, local agent skill, documentation, template
   fragment, and dependency list.
2. **Given** a capability is missing one of those ownership elements, **When**
   validation runs, **Then** the change is reported as incomplete before
   planning or implementation proceeds.
3. **Given** a capability depends on another capability, **When** generated
   products select it, **Then** the prerequisite capability is included in a
   predictable and reviewable way.

---

### User Story 4 - Guide Agents With Selected Local Skills (Priority: P3)

An AI-assisted contributor working inside a generated product receives only the
local agent skills that match the generated product and its selected framework
capabilities. The skills explain ownership, allowed changes, required tests, and
evidence expectations for those capabilities.

**Why this priority**: V3 should keep governance useful without forcing every
generated product to inherit the entire framework repository's workflow surface.

**Independent Test**: Generate products with different capability selections and
inspect the copied local skills. Each output contains the project skill plus
only the capability skills selected directly or required as prerequisites.

**Acceptance Scenarios**:

1. **Given** a product selects keyboard input, **When** generation completes,
   **Then** the product includes a keyboard input skill that names keyboard
   ownership, tests, fixtures, evidence, and boundary rules.
2. **Given** a product does not select charts, **When** generation completes,
   **Then** no chart-specific local skill is copied into the generated product.
3. **Given** a contributor opens a selected capability skill, **When** they read
   it, **Then** the skill names the owned files or product areas, public
   contract, verification commands, and evidence rules needed for safe changes.

### Edge Cases

- A selected capability requires another capability that the user did not
  explicitly select.
- A user selects a headless scene profile that should not include a live viewer
  host.
- A user requests samples while also expecting a lean product tree.
- A capability has package metadata but no local agent skill, no tests, or no
  template fragment.
- A generated product opts into framework-source development and therefore needs
  heavier checks than a normal product consumer.
- A generated product's verification command accidentally runs framework
  repository checks that are unrelated to the product.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST define a V3 capability catalog that names every
  selectable reusable framework capability and its ownership metadata.
- **FR-002**: Each selectable runtime capability MUST have an independently
  distributable compiled library or be explicitly marked as non-runtime
  generated content.
- **FR-003**: Each selectable capability MUST declare its public contract,
  semantic tests, local agent skill, documentation entry, template fragment,
  dependencies, and validation path.
- **FR-004**: The base scene capability MUST be independently selectable without
  requiring viewer hosting, Elmish integration, keyboard input, layout, charts,
  samples, or native viewer dependencies.
- **FR-005**: Viewer hosting, Elmish integration, keyboard input, layout,
  charts, testing helpers, and samples MUST be selectable separately from the
  base scene capability.
- **FR-006**: Capability selection MUST include required prerequisites
  automatically and report those inclusions in generated product output.
- **FR-007**: The default generated product MUST contain a product application,
  product tests, product documentation, command wrappers, full Spec Kit
  governance assets, selected local skills, and references for Scene,
  SkiaViewer, Elmish, KeyboardInput, Layout, and Charts.
- **FR-008**: The default generated product MUST NOT contain framework samples,
  framework galleries, framework parity suites, historical feature
  specifications, readiness evidence, copied framework documentation, copied
  framework README content, or framework implementation projects.
- **FR-009**: Samples MUST be excluded by default and available only through an
  explicit sample-oriented profile or sample selection.
- **FR-010**: Generated product documentation MUST describe the generated
  product and its selected framework capabilities, not the framework repository
  as a whole.
- **FR-011**: Generated products MUST receive only the project-level local skill
  and the local skills for selected or required capabilities.
- **FR-012**: Each capability skill MUST state its scope, public contract,
  verification commands, evidence rules, and guidance for avoiding unrelated
  package-boundary changes.
- **FR-013**: Framework validation MUST detect when capability metadata,
  package ownership, skills, docs, tests, or template fragments drift out of
  alignment.
- **FR-014**: Framework validation MUST generate representative products from
  source and packaged template paths and prove that their content matches the
  selected profiles and capabilities.
- **FR-015**: Generated product verification MUST focus on product behavior and
  selected framework capability usage, not framework repository gallery,
  parity, packaging, or template maintenance workflows.
- **FR-016**: Public surface tracking MUST be maintained per public capability
  so maintainers can review package-specific contract changes.
- **FR-017**: V3 generated products MUST include full product governance by
  default, including evidence gates, drift checks, generated guidance checks,
  and readiness workflow, while excluding framework-source maintenance checks
  that do not apply to the generated product.

### Change Classification

- **Tier**: Tier 1 (contracted change).
- **Rationale**: This feature changes public package boundaries, generated
  product behavior, template composition, package dependencies, public contract
  ownership, and governance validation.
- **Public API impact**: Public capability packages and package-specific `.fsi`
  contracts are introduced or retargeted for Scene, SkiaViewer, Elmish,
  KeyboardInput, Layout, Charts, and Testing.

### Framework Governance Prompts *(mandatory for this repository)*

- **Package impact**: This feature changes package boundaries, package
  contents, package dependencies, and generated package consumers. It is
  expected to introduce separate capability-owned public packages and
  per-package surface baselines.
- **Public contract impact**: This feature changes public contract ownership by
  moving from one broad core contract toward capability-specific contracts.
  Each affected public contract and generated product reference must be reviewed
  with package-specific surface evidence.
- **State workflow impact**: This feature changes ownership boundaries for
  stateful workflow concerns. Viewer hosting, Elmish adapters, keyboard input,
  generated product build commands, and local skills must remain explicit about
  messages, effects, subscriptions, and interpreter behavior.
- **Layout/rendering impact**: This feature changes ownership of layout,
  charts, viewer hosting, screenshots, Skia, and native viewer dependencies by
  making them separate selectable capabilities. It does not authorize a new
  rendering architecture or new platform support.
- **Evidence obligations**: Required evidence includes capability catalog
  validation, generated product file-list reports, source and packaged template
  generation logs, generated product verification logs, per-package surface
  baselines, selected-skill copy reports, dependency reports, and template
  drift reports under the feature readiness directory.
- **Unsupported scope**: Dynamic plugin loading, a general UI framework rewrite,
  new renderer backends, new platform support, release publishing automation,
  V2 migration support, and full visual quality validation are out of scope
  unless a later feature explicitly authorizes them.
- **Build-target impact**: The feature is expected to add or change framework
  validation targets for capability ownership, selected-skill validation,
  generated product cleanliness, template validation, dependency reporting,
  package surface checks, generated guidance checks, template drift, evidence
  graph, and evidence audit. Generated product `Dev`, `Test`, and `Verify`
  targets must include full product governance while excluding framework gallery,
  parity, template packaging, and framework-source maintenance checks.

### Key Entities

- **Capability**: A selectable framework concern such as scene composition,
  viewer hosting, Elmish integration, keyboard input, layout, charts, testing
  helpers, or samples. It has ownership metadata and dependency rules.
- **Capability Catalog**: The maintained record that connects each capability
  to its package ownership, tests, local skill, documentation, template
  fragment, dependencies, and validation path.
- **Distributable Library**: A compiled reusable framework unit that product
  projects can reference without copying framework implementation source.
- **Local Agent Skill**: A capability-specific workflow guide copied into a
  generated product only when that capability is selected or required.
- **Template Profile**: A named generation mode. The canonical V3 profile IDs
  are `app`, `headless-scene`, `governed`, and `sample-pack`.
- **Governed Profile**: A product-generation profile that uses the same full
  product governance baseline as the default app but may omit runtime app
  capabilities when selected for governance-only or library-oriented product
  scaffolding. It must not introduce framework-source maintenance checks.
- **Generated Product**: The output repository or project created from the V3
  template for an application or library team.
- **Sample Pack**: Optional example content that demonstrates framework
  capabilities without being part of the default product template.
- **Validation Report**: Evidence proving that capability ownership, generated
  product contents, selected skills, dependencies, and public surfaces remain
  aligned.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A default generated product contains zero framework sample
  directories, zero framework gallery applications, zero historical feature
  specification directories, zero framework readiness evidence directories, and
  zero copied framework README sections.
- **SC-002**: A default generated product contains exactly one product
  application and exactly one product test suite unless the user selects a
  profile that explicitly asks for more.
- **SC-003**: 100% of selectable capabilities in the V3 catalog declare an
  owner, public contract, validation path, local skill, documentation entry,
  generated template fragment, and dependency list.
- **SC-004**: For at least four representative capability selections, generated
  products contain only the selected or required capability skills and no
  unrelated capability skills.
- **SC-005**: For at least four representative capability selections, generated
  product verification includes evidence gates, drift checks, generated guidance
  checks, and readiness workflow, and completes without running framework
  sample-gallery, parity-suite, framework package-surface maintenance, or
  framework template packaging checks.
- **SC-006**: Framework template validation proves both source-based and
  package-based generation for the `app`, `headless-scene`, `governed`, and
  `sample-pack` profiles.
- **SC-007**: 100% of public V3 capability packages have a package-specific
  surface baseline or an explicit record explaining why no public surface is
  exposed.
- **SC-008**: Generated product documentation identifies Scene, SkiaViewer,
  Elmish, KeyboardInput, Layout, Charts, and product commands in one page
  without including framework architecture, V2 analysis, subsystem design, or
  template framework analysis documents.

## Assumptions

- The source design intent is the V3 modular framework direction captured in
  `docs/v3Design.md`.
- The V3 design is a future modularization feature and does not block completion
  of the current targeted refactor and governance diagnostics work.
- The framework repository remains the source of truth for framework
  development, heavy governance, samples, docs, and regression evidence.
- Generated products are normal consumers of the framework by default, not
  framework-source forks.
- Some package and namespace changes are acceptable in V3 when surface evidence
  is explicit; V2 migration support is not part of this feature.
- Samples are still valuable framework assets, but they should be opt-in for
  generated products.
- The default product experience should prioritize a complete application
  capability set and full product governance while keeping generated source,
  samples, and docs lean.
