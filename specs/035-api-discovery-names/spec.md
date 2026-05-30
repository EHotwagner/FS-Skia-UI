# Feature Specification: Package API Discovery And Name Safety

**Feature Branch**: `035-api-discovery-names`  
**Created**: 2026-05-30  
**Status**: Draft  
**Input**: User description: "create specs for package consumer API discoverability and Scene/Controls name collision ergonomics, maybe require fully qualified names attributes"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Discover F# Authoring Shapes From Packages (Priority: P1)

A package consumer or generated-product agent needs to author a scene using only installed FS.Skia.UI packages. They can discover the supported F#-shaped public API, including union cases, record fields, constructors, modules, factory functions, parameter names, and common examples, without reflecting over assemblies or reading repository source files.

**Independent Test**: In a clean generated consumer project that references packaged FS.Skia.UI artifacts, a reviewer can identify the correct source-shaped spelling for Scene primitives, `Paint` helpers, key viewer types, geometry records, and records or discriminated unions from packaged documentation or generated authoring guidance alone.

### User Story 2 - Avoid Scene And Controls Name Collisions (Priority: P1)

A consumer opens Scene and Controls namespaces in the same file while building a product view. Collision-prone names such as text primitives, event origins, record fields, or builder helpers are either unambiguous by default or called out by generated guidance so the consumer writes stable, explicit names rather than relying on namespace open order.

**Independent Test**: A generated consumer sample containing both Scene and Controls authoring compiles without open-order-sensitive errors, and guidance explains the required qualification pattern for collision-prone names.

### User Story 3 - Classify API Ergonomics Feedback Correctly (Priority: P2)

A maintainer receives feedback that an agent had to use reflection or change open order to author a generated product. They can classify the feedback as package documentation discoverability, public contract ergonomics, generated template guidance, or consumer authoring guidance, with a clear next action and evidence path.

**Independent Test**: Given the reported reflection and name-collision findings, the feature evidence records the owner category, whether the public contract must change, whether generated guidance must change, and whether any runtime behavior is out of scope.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Packaged FS.Skia.UI consumers MUST have a discoverable source-shaped public API reference for every packable framework package, covering public modules, types, union cases, records, fields, values, parameters, returns, and common construction patterns.
- **FR-002**: The source-shaped API reference MUST use F# authoring names rather than compiled reflection names when they differ.
- **FR-003**: Generated product guidance MUST tell agents where to find packaged API reference material before they resort to reflection or repository source inspection.
- **FR-004**: Generated product guidance MUST include a stable authoring rule for files that combine Scene primitives and Controls APIs.
- **FR-005**: Collision-prone public names shared by Scene, Controls, and related packages MUST be identified and either made unambiguous by the public contract or documented with explicit qualification guidance.
- **FR-006**: Consumer examples that combine Scene and Controls MUST use explicit, stable naming for collision-prone members instead of relying on namespace open order.
- **FR-007**: Validation MUST include at least one clean package-consumer scenario that proves a consumer can author Scene primitives, `Paint` helpers, and basic Controls-adjacent code without reflection.
- **FR-008**: Validation MUST include at least one mixed Scene/Controls scenario that would expose open-order sensitivity if guidance or public contracts are insufficient.
- **FR-009**: Feedback classification guidance MUST distinguish package documentation discoverability, public contract ergonomics, generated template workflow, and consumer authoring issues.
- **FR-010**: The feature MUST preserve existing XML documentation guarantees while adding any missing source-shaped discovery or name-safety guarantees.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package contents may change to include source-shaped API reference material for packaged consumers. Package identities and package versions are expected to be reviewed during planning; release publishing remains out of scope.
- **Public contract impact**: Public `.fsi` signatures and surface baselines may change if planning chooses contract-level qualification for collision-prone names. Documented public APIs and generated package consumer guidance are in scope.
- **State workflow impact**: Stateful workflow, I/O commands, effects, subscriptions, and interpreter behavior are out of scope unless needed only for validation fixtures.
- **Layout/rendering impact**: Runtime layout, rendering, screenshot capture, Vulkan, Skia rasterization, and visual output behavior are out of scope. Authoring examples may include simple visual scenes only to prove API discovery and name safety.
- **Evidence obligations**: Required real evidence paths include `specs/035-api-discovery-names/readiness/api-discovery.md`, `specs/035-api-discovery-names/readiness/name-collision-safety.md`, `specs/035-api-discovery-names/readiness/generated-consumer-validation.md`, `specs/035-api-discovery-names/readiness/feedback-classification.md`, `specs/035-api-discovery-names/readiness/package-reference-material.md`, `specs/035-api-discovery-names/readiness/package-surface-baseline.md`, `specs/035-api-discovery-names/readiness/evidence-graph.md`, and `specs/035-api-discovery-names/readiness/evidence-audit.md`.
- **Unsupported scope**: Runtime rendering fixes, release publishing, external documentation hosting, new game demos, and broad API redesign beyond collision-prone discoverability are out of scope.
- **Build-target impact**: `Dev`, `PackLocal`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, and `EvidenceAudit` are expected to be relevant. `DependencyReport`, `TemplateDrift`, `Verify`, and `Ci` should change only if planning identifies affected surfaces.

## Success Criteria *(mandatory)*

- **SC-001**: A clean generated consumer project can identify the correct source-shaped authoring form for at least 95% of public Scene, Controls, viewer, geometry, and paint members sampled by validation without assembly reflection.
- **SC-002**: Mixed Scene/Controls generated examples compile successfully with namespace opens in more than one order, or the examples avoid open-order dependence through explicit qualification.
- **SC-003**: 100% of collision-prone names identified by validation have either a public-contract safety decision or explicit consumer guidance.
- **SC-004**: Feedback reports for reflection-based discovery and open-order name collisions can be classified into the correct owner category with a documented next action using a maintainer checklist or transcript whose recorded elapsed time is under 5 minutes.
- **SC-005**: Existing packaged XML documentation validation remains passing while the new source-shaped discovery checks pass for every packable framework package.

## Assumptions

- Source-shaped discoverability means consumers can see F# authoring names and shapes from package artifacts, generated guidance, or package-adjacent reference material without cloning this repository.
- Contract-level qualification for collision-prone public names is preferred when it can be introduced without excessive breakage; otherwise generated guidance and examples must use explicit qualification.
- The first validation target is generated FS.Skia.UI package consumers, not arbitrary external IDE behavior.
