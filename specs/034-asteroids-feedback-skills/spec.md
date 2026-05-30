# Feature Specification: Asteroids Feedback Skill Guidance

**Feature Branch**: `034-asteroids-feedback-skills`  
**Created**: 2026-05-30  
**Status**: Draft  
**Input**: User description: "Mailbox/2026-05-30T07-19-26+0200-asteroids-demo-implementation-phase-fs-skia-ui-feedback.md maybe some additional/improved skills asigned to tasks could help in some regards."

## Clarifications

### Session 2026-05-30

- Q: Should this feature directly fix missing consumer API documentation by editing public `.fsi` files, or only improve guidance/classification around that gap? -> A: Expand this feature to add comprehensive XML doc comments to every public `.fsi` in packable framework packages, with validation that generated XML docs are non-empty.
- Q: What depth of XML documentation is required for public `.fsi` surfaces? -> A: Summary plus parameter/return docs where applicable, with remarks or examples for non-obvious modules, workflows, and factory functions.
- Q: Which `.fsi` files are in scope for public XML documentation? -> A: Every public `.fsi` compiled by every packable `src/*/*.fsproj`, including the root `FS.Skia.UI` package and capability packages.
- Q: Should XML documentation validation be hard-failing or advisory? -> A: Add a hard validation check that fails when public `.fsi` members lack required XML docs or generated XML doc files are missing/empty.
- Q: Should documentation validation prove XML docs are delivered in packed NuGet artifacts? -> A: Validate packed NuGet artifacts include the generated XML documentation files for each packable framework package.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Assign Implementation Skills Before Work Starts (Priority: P1)

A generated-demo implementer receives task assignments that name the relevant implementation, evidence, layout, CI-debugging, and documentation skills before starting work, so specialized guidance is available during the tasks that need it.

**Independent Test**: Generate or inspect an FS.Skia.UI demo task list with rendering, screenshot evidence, layout evidence, persistent-window, and verification tasks, then confirm each task either has the applicable skill assignment or explains why no specialized skill applies.

### User Story 2 - Scaffold Hidden Evidence Contracts (Priority: P1)

A generated-demo implementer can see every required readiness evidence file and required evidence field before running the final audit, avoiding reverse-engineering audit terms from failing validation output.

**Independent Test**: Starting from the generated implementation tasks alone, identify all readiness files required for visual evidence, window visibility, governance risk, aggregate hang diagnostics, runtime limitations, generated validation, and real-image evidence without reading the audit implementation.

### User Story 3 - Preserve Evidence Honesty For Visual Proofs (Priority: P1)

A reviewer can distinguish real decodable screenshots and rasterized scene evidence from metadata-only reports, fallback images, or layout-only bounds checks, so visual proof claims do not overstate what was actually captured.

**Independent Test**: Review generated task guidance and evidence acceptance criteria for screenshot and image evidence, then confirm they require a decodable image, expected dimensions, non-trivial content, and honest unsupported/fallback classification when those conditions are not met.

### User Story 4 - Classify Framework, Template, And Consumer Findings (Priority: P2)

A maintainer triaging Asteroids implementation feedback can separate framework-attributable rendering or host-contract issues from template guidance issues and consumer authoring choices, keeping follow-up backlogs actionable.

**Independent Test**: Given the Asteroids implementation feedback, classify each reported issue as framework behavior, generated template/evidence workflow, documentation/discoverability, or consumer authoring choice, and confirm each class maps to a bounded follow-up path.

### User Story 5 - Improve Skill Discovery And API Documentation For API And Host Friction (Priority: P3)

A generated-demo author who hits API discoverability, host-size, persistent-window, or name-collision friction can find the relevant guidance, skill assignment, and shipped XML API documentation from task metadata or package artifacts instead of relying on ad hoc reflection or trial-and-error.

**Independent Test**: Inspect tasks that involve public API discovery, host/window behavior, visual rendering, or compile-warning triage and confirm the task metadata points to a skill or guidance note that addresses the expected friction; inspect generated XML documentation for packable framework package `.fsi` files and confirm public signatures have non-empty documentation.

### Synthetic Evidence Disclosure

The feature includes design-approved synthetic error-handling coverage for invalid guidance and visual-proof inputs:

- Missing or malformed readiness key/value fields may use synthetic malformed scaffold fixtures. Real replacement evidence is `specs/034-asteroids-feedback-skills/readiness/readiness-scaffold-coverage.md`.
- Metadata-only screenshot reports, fallback or placeholder image claims, and layout-bounds-only visual proof claims may use synthetic rejection fixtures. Real replacement evidence is `specs/034-asteroids-feedback-skills/readiness/visual-evidence-honesty.md`.

These cases validate explicit error paths and must remain marked `[SEH]` / `synthetic-error-handling-approved` in tasks.

## Requirements *(mandatory)*

### Change Classification

- **Tier**: Tier 1 (contracted governance/template and documentation change).
- **Rationale**: This feature changes documented task contracts, generated guidance, readiness scaffolds, validation behavior, skill metadata expectations, and public `.fsi` XML documentation for packable framework packages. It does not change runtime API shapes, package versions, or rendering behavior.
- **Public API impact**: Public F# runtime API shapes are not expected to change, but public `.fsi` documentation comments are in scope. If implementation discovers a required public F# symbol change, work must return to `.fsi` design before implementation.

### Functional Requirements

- **FR-001**: Generated FS.Skia.UI implementation tasks MUST include skill assignments for specialized workflows when a matching local skill exists, including implementation execution, layout evidence, evidence graph validation, evidence audit validation, template updates, and debug-until-green loops.
- **FR-002**: Task-generation guidance MUST explain how to assign multiple skills to one task when a task spans implementation work and evidence validation work.
- **FR-003**: Task-generation guidance MUST include advisory skill assignment patterns for visual demo tasks that involve scene rendering, screenshot capture, layout readability, persistent window launch, deterministic evidence mode, and generated-package validation.
- **FR-004**: Task lists MUST scaffold or explicitly enumerate all audit-required readiness evidence files that an implementation-phase author is expected to produce for generated visual demos.
- **FR-005**: Readiness scaffolding MUST include expected evidence fields or acceptance cues for real-image evidence, window visibility, close-reason separation, window-state diagnostics, window options, governance risk levels, aggregate hang diagnostics, runtime limitations, and generated validation.
- **FR-006**: Evidence acceptance criteria MUST distinguish a decodable image artifact from a textual report and MUST reject metadata-only screenshot claims as complete visual proof. For visual proof, non-trivial content means the artifact has expected dimensions and evidence of rendered scene content, such as multiple non-background colors, non-empty changed-pixel regions, or an explicit classifier/scan result showing visible scene elements.
- **FR-007**: Evidence acceptance criteria MUST distinguish rasterized-pixel proof from layout-bounds proof, so layout readability evidence cannot substitute for missing visual content in screenshots.
- **FR-008**: Evidence acceptance criteria MUST classify fallback or placeholder images as unsupported or incomplete proof unless they meet the same dimensions and content checks as real captures.
- **FR-009**: Generated guidance MUST direct authors to record framework-attributable rendering gaps separately from consumer workarounds, including missing stroke rendering, text legibility problems, screenshot artifact honesty, fallback-image honesty, and host-size delivery limitations.
- **FR-010**: Generated guidance MUST direct authors to classify persistent-window blocking, display/session availability, and auto-close smoke-test needs separately so environment readiness is not confused with runtime framework failure.
- **FR-011**: Generated guidance SHOULD make API discoverability friction visible to authors by pointing API-surface investigation tasks to the best available local documentation or skill guidance.
- **FR-012**: Generated guidance SHOULD warn authors about common name-collision and overlapping-field friction in visual demo work and require any remaining warnings to be classified as benign, blocking, or deferred.
- **FR-013**: Follow-up tasks created from implementation feedback MUST identify whether the target owner is framework runtime, generated template/evidence workflow, documentation/discoverability, or consumer authoring.
- **FR-014**: The improved task guidance MUST preserve existing validation behavior for correctly authored task lists and MUST NOT make advisory skill suggestions into hard failures unless a later specification explicitly changes validation rules.
- **FR-015**: The feature MUST provide real validation evidence showing that generated task guidance exposes the skill assignment and readiness scaffolding before implementation begins.
- **FR-016**: Every public `.fsi` file compiled by every packable `src/*/*.fsproj`, including the root `FS.Skia.UI` package and all capability packages, MUST include comprehensive XML documentation comments for public modules, types, union cases, records, fields, and values so generated XML documentation is useful to package consumers.
- **FR-017**: XML documentation comments MUST include a summary for every documented public member, parameter and return documentation where applicable, and remarks or examples for non-obvious modules, workflows, and factory functions.
- **FR-018**: Documentation validation MUST fail when required XML documentation comments are missing from public `.fsi` members, or when generated XML documentation files for packable framework packages are missing, empty, or lack member documentation for public `.fsi` surfaces.
- **FR-019**: Package validation MUST prove each packed NuGet artifact for a packable framework package includes the generated XML documentation file that corresponds to its assembly.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Runtime package identities, package versions, and runtime API shapes are expected to remain unchanged. Generated template guidance, task templates, skill guidance, evidence workflow assets, generated XML documentation contents, and packed XML documentation files may change.
- **Public contract impact**: Public `.fsi` signatures and runtime API shapes are not expected to change in this feature, but public `.fsi` XML documentation comments are in scope. Documented task contracts, evidence acceptance contracts, generated guidance, and readiness scaffolds are also in scope.
- **State workflow impact**: Runtime state workflows are out of scope. Implementation-phase task workflow, evidence-production workflow, and skill-assignment workflow are in scope.
- **Layout/rendering impact**: Runtime rendering behavior is out of scope for this specification. Visual evidence requirements, screenshot honesty, layout-evidence interpretation, and rendering-gap triage guidance are in scope.
- **Evidence obligations**: Required real evidence paths include `specs/034-asteroids-feedback-skills/readiness/skill-assignment-guidance.md`, `specs/034-asteroids-feedback-skills/readiness/readiness-scaffold-coverage.md`, `specs/034-asteroids-feedback-skills/readiness/visual-evidence-honesty.md`, `specs/034-asteroids-feedback-skills/readiness/feedback-classification.md`, `specs/034-asteroids-feedback-skills/readiness/generated-guidance-validation.md`, and `specs/034-asteroids-feedback-skills/readiness/xml-documentation-validation.md`.
- **Unsupported scope**: Fixing runtime stroke rasterization, text rasterization, screenshot capture internals, host resize APIs, release publishing, package version bumps, and implementing a new Asteroids demo are out of scope.
- **Build-target impact**: `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `PackLocal`, `EvidenceGraph`, and `EvidenceAudit` may need validation coverage. `PackLocal` is expected when proving packed NuGet XML documentation inclusion for packable framework packages. `Dev`, `Verify`, `Ci`, `DependencyReport`, and `TemplateDrift` should change only if existing touched artifacts require their normal validation.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of generated visual-demo tasks that require specialized workflow guidance have a skill assignment or a documented no-skill rationale.
- **SC-002**: An implementer can identify every audit-required visual-demo readiness file from generated tasks or guidance in under 5 minutes without reading audit scripts.
- **SC-003**: Screenshot evidence is accepted only when a decodable image artifact exists at the claimed artifact path with expected dimensions and non-trivial content; metadata-only reports, 1x1 images, blank captures, and layout-bounds-only reports do not satisfy this criterion.
- **SC-004**: Fallback image use, metadata-only reports, and layout-only bounds checks are classified as incomplete or unsupported visual proof in 100% of generated evidence guidance examples.
- **SC-005**: At least four framework-attributable findings and at least three non-framework findings from the Asteroids feedback are classified into distinct follow-up categories.
- **SC-006**: Existing valid task lists continue to pass graph validation after advisory skill guidance is added.
- **SC-007**: Generated guidance validation confirms the new skill-assignment and readiness-scaffold guidance before `/speckit-implement` begins.
- **SC-008**: 100% of public `.fsi` files in packable framework packages produce generated XML documentation with non-empty summaries for public modules, types, union cases, records, fields, and values, plus parameter/return documentation where applicable.
- **SC-009**: 100% of packed NuGet artifacts for packable framework packages contain the generated XML documentation file that corresponds to the packaged assembly.

## Assumptions

- The mailbox report accurately describes implementation-phase friction from the Asteroids demo work completed on 2026-05-30.
- The immediate value is better generated task guidance, skill assignment, evidence scaffolding, and consumer-facing API documentation; runtime rendering and host-contract bugs should be tracked separately.
- Advisory skill assignment should help authors choose existing or future skills without expanding hard validator enforcement in this feature.
- Readiness scaffolding can be improved through templates, generated guidance, or task-generation rules; the exact mechanism can be selected during planning.
