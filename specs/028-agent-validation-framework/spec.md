# Feature Specification: Agent Validation Framework

**Feature Branch**: `028-agent-validation-framework`  
**Created**: 2026-05-28  
**Status**: Draft  
**Input**: User description: `create specs for docs/2026-05-28-1557-agent-consumer-framework-analysis.md`

## Clarifications

### Session 2026-05-28

- Q: Should the first implementation include the build-runner migration itself or only create the manifest/metadata layer that makes a later migration safe? → A: Include native build target registration migration in this feature's implementation scope.
- Q: What controls typing scope should this feature cover? → A: Type every existing standard controls module in this feature.
- Q: What source should agent-ready validation use to determine changed paths? → A: Prefer active feature metadata; fall back to git diff from the merge base.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Route Agent Validation Deliberately (Priority: P1)

As a Spec Kit agent working on FS.Skia.UI, I want a clear validation contract that maps changed product, template, governance, and documentation areas to the smallest authoritative gates, using active feature metadata first and git merge-base diff as fallback, so that I can prove the work without guessing or defaulting to broad validation for every change.

**Independent Test**: Given representative changed paths from active feature metadata and, when metadata is unavailable, from git merge-base diff for controls, templates, evidence governance, generated app guidance, documentation, and package surfaces, the validation contract identifies the required validation tier, required gates, expected evidence artifacts, and default failure owner for each change area.

### User Story 2 - Produce One Agent Verdict (Priority: P1)

As a reviewer or autonomous agent, I want validation runs to produce one compact verdict that states status, authority, selected rules, required gates, completed gates, missing gates, failure ownership, next command, and evidence artifacts, so that the next action is auditable from one place.

**Independent Test**: Run the agent-ready validation path for a focused change. The resulting verdict records the selected validation rules, all required and completed gates, any skipped or missing gates, the authority level, and a next command when the run is incomplete or degraded.

### User Story 3 - Separate Normal Product Launch From Evidence Policy (Priority: P2)

As a generated app author, I want normal generated app launch to stay focused on running the product while evidence policy, report wording, and command orchestration are separated into explicit evidence workflows, so that generated app code is easier to inspect and evidence claims remain governed.

**Independent Test**: Inspect a generated app and confirm that normal launch remains persistent and interactive, while evidence commands are explicit, separately discoverable, and produce governed reports without changing everyday product execution.

### User Story 4 - Add Typed Control Guardrails (Priority: P2)

As a controls author or generated app agent, I want typed front doors for every existing standard controls module, including standard control kinds, event kinds, attribute names, chart data, and grid data while preserving custom extension escape hatches, so that standard generated controls fail early when the contract is misspelled or semantically invalid.

**Independent Test**: Attempt to define standard controls with misspelled standard control kinds, standard event kinds, or standard data attributes. The typed path rejects or prevents the misuse across every existing standard controls module, while a deliberate custom extension path remains available and visibly marked as custom.

### User Story 5 - Align Build Targets With Discoverable Metadata (Priority: P3)

As a maintainer integrating FS.Skia.UI validation with external tooling, I want build targets to use native target registration while preserving testable planning metadata, and I want target metadata, validation tiers, dependencies, costs, authority levels, outputs, and stale-prerequisite assumptions to be discoverable and checked for drift, so that command behavior and documentation stay aligned.

**Independent Test**: Compare the runnable native target surface with the target metadata and validation contract. The validation fails when a documented target has no runnable target, a runnable validation target has no metadata, or a focused validation rule lacks outputs and failure ownership.

### Edge Cases

- Changed-path detection is unavailable or ambiguous; the verdict must degrade explicitly and name the broad fallback command instead of silently claiming focused authority.
- A change matches multiple validation rules; the selected gates must be the union of required gates without duplicate execution claims.
- A gate is unsupported on the current host; the verdict must classify the result as unsupported or degraded and name the environment reason rather than product failure.
- A validation rule requires a generated consumer package or template artifact that has not been prepared; the verdict must classify the stale prerequisite and name the command needed to refresh it.
- Custom controls and custom attributes must remain possible, but they must not masquerade as known typed controls or known typed attributes.
- Evidence commands must not make screenshot, visual, package, or final-readiness claims stronger than the completed validation supports.
- Existing command names used by generated projects and repository workflows must remain stable unless the migration explicitly documents a compatibility transition.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST provide a machine-readable validation contract that maps changed paths, feature concerns, and risk categories to required validation tiers, gates, expected artifacts, timeout class, and default failure owner.
- **FR-002**: The validation contract MUST include rules for controls public surface changes, generated template changes, evidence-governance changes, generated app guidance changes, documentation-only changes, package-surface changes, and build-target contract changes.
- **FR-003**: The system MUST define validation tiers for fast inner-loop checks, focused authoritative checks, agent-ready proof, maintainer broad verification, and automation-final verification.
- **FR-004**: The system MUST provide an agent-ready validation path that selects required focused gates from the validation contract, includes evidence graph and evidence audit obligations, and avoids broad verification when focused authority is sufficient.
- **FR-005**: The agent-ready validation path MUST determine changed paths by preferring active feature metadata and falling back to git diff from the merge base when feature metadata is unavailable.
- **FR-006**: When focused validation cannot be selected confidently from active feature metadata or merge-base diff, the agent-ready validation path MUST report an explicit degraded status and identify the broad fallback command.
- **FR-007**: Every agent-ready validation run MUST emit one consolidated verdict containing status, authority level, selected rule identifiers, required gates, completed gates, missing gates, failure owner, next command, evidence artifacts, and diagnostics.
- **FR-008**: The consolidated verdict MUST classify environment failures, unsupported hosts, stale prerequisites, product failures, template failures, governance failures, and missing evidence as distinct outcomes.
- **FR-009**: Every focused and broad validation gate in scope MUST declare metadata for description, tier, dependencies, direct prerequisites, expected outputs, stale assumptions, timeout class, cost, authority level, and default failure owner.
- **FR-010**: Validation MUST fail when target metadata, runnable target names, documented target names, and validation contract references drift from one another.
- **FR-011**: Normal generated app launch MUST remain separate from explicit evidence commands and MUST NOT run audits, close windows, or write evidence artifacts unless an evidence command is invoked.
- **FR-012**: Generated app evidence workflows MUST keep product-owned facts separate from policy-owned command orchestration and report wording.
- **FR-013**: Evidence reports MUST avoid success-only completion claims and MUST state the authority level of the evidence that was actually produced.
- **FR-014**: The standard controls authoring path MUST expose typed or otherwise constrained front doors for every existing standard controls module, covering known control kinds, event kinds, attribute names, chart data, and grid data.
- **FR-015**: The controls contract MUST preserve a deliberate custom extension path for custom controls, custom events, and custom values while making custom usage distinguishable from known framework contracts.
- **FR-016**: Controls diagnostics MUST be able to report missing or unsupported required attributes from a shared control schema instead of relying on scattered string vocabulary.
- **FR-017**: Generated templates and guidance MUST prefer typed standard controls paths for all existing standard controls and reserve custom paths for deliberate extension scenarios.
- **FR-018**: The build target surface MUST support external discovery of target names and metadata without requiring maintainers or agents to infer command behavior from prose.
- **FR-019**: The build target surface MUST migrate in-scope validation targets to native target registration while preserving pure planning metadata for testability.
- **FR-020**: Existing stable command names used by repository workflows and generated consumers MUST remain available during migration.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package identities and package versions are not required to change by this specification. Package contents may change where validation metadata, generated evidence workflows, generated template files, or controls front-door assets are shipped to generated consumers.
- **Public contract impact**: Public controls contracts may change by adding typed or constrained front doors for every existing standard controls module, control schema exposure, and compatibility-preserving custom extension paths. Existing sample contracts, generated templates, documentation, and surface baselines must be reviewed and updated if public controls names or signatures change.
- **State workflow impact**: Build and evidence command workflows change. Normal generated app state, product reducers, everyday launch behavior, and viewer interaction semantics remain separate from evidence policy.
- **Layout/rendering impact**: Core layout and rendering behavior are not changed by default. Controls diagnostics, generated control authoring guidance, and evidence claim wording for visual proof are in scope where needed to support typed guardrails and agent verdicts.
- **Evidence obligations**: Required real evidence paths are `specs/028-agent-validation-framework/readiness/validation-contract.md`, `specs/028-agent-validation-framework/readiness/agent-ready-verdict.md`, `specs/028-agent-validation-framework/readiness/target-metadata.md`, `specs/028-agent-validation-framework/readiness/evidence-policy-separation.md`, `specs/028-agent-validation-framework/readiness/typed-controls-front-door.md`, `specs/028-agent-validation-framework/readiness/environment-failure-classification.md`, `specs/028-agent-validation-framework/readiness/evidence-graph.md`, and `specs/028-agent-validation-framework/readiness/evidence-audit.md`.
- **Unsupported scope**: New game mechanics, renderer redesign, package publishing, new platform support, browser or mobile screenshot capture, replacement of the screenshot capture contract, and removal of existing compatibility APIs are out of scope.
- **Build-target impact**: Native target registration migration is in scope for the validation target surface. `GeneratedGuidanceCheck`, `TemplateCheck`, `PackageSurfaceCheck`, `FsiTranscripts`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`, `Verify`, and `Ci` may need contract, metadata, registration, or aggregation updates. A new `AgentReady` target or equivalent agent-ready command is in scope. `Dev`, `PackLocal`, `DependencyReport`, and `TemplateDrift` change only if planning identifies direct metadata, package-content, or drift-validation requirements.

### Key Entities

- **Validation Contract**: A machine-readable rule set that maps changed paths and feature concerns to validation tiers, required gates, expected artifacts, timeout class, and failure ownership.
- **Validation Tier**: A named authority and cost level for checks, such as inner-loop, focused-authority, agent-ready, maintainer-verify, or automation-final.
- **Agent Verdict**: A compact validation outcome that records status, authority, selected rules, required and completed gates, missing work, failure ownership, next command, artifacts, and diagnostics.
- **Target Metadata**: The discoverable description of a natively registered runnable validation target, including dependencies, prerequisites, outputs, cost, authority, timeout class, and failure owner.
- **Evidence Policy Workflow**: The explicit evidence command path that orchestrates validation and report wording separately from normal generated product launch.
- **Typed Control Front Door**: The standard controls authoring path that constrains every existing standard controls module, including known control kinds, events, attributes, chart data, and grid data before lowering to the generic controls representation.
- **Control Schema**: A shared description of known controls, required attributes, supported events, accessibility expectations, and diagnostics vocabulary.
- **Environment Failure Classification**: Verdict fields and report wording that distinguish unsupported hosts, missing desktop sessions, stale prerequisites, and retryable environment issues from product defects.

### Assumptions

- The source analysis document is treated as the product direction for this feature, and its recommendations are grouped into one planning feature because they all serve agent-consumable validation and framework guardrails.
- Existing command names should remain stable for generated consumers while internal target registration migrates to native target registration.
- The typed controls work should cover every existing standard controls module while remaining additive and compatibility-preserving; removal of flexible string or custom-value APIs would require a separate migration decision.
- The agent-ready validation path should select focused gates from active feature metadata first, fall back to git merge-base diff when metadata is unavailable, and degrade to an explicit broad fallback when neither source can provide confident path context.
- Evidence graph and audit remain required for final agent-ready proof because they govern readiness and synthetic-evidence disclosure.

## Success Criteria *(mandatory)*

- **SC-001**: For at least six representative changed-path scenarios sourced from active feature metadata or git merge-base diff, the validation contract selects the expected focused validation rules, required gates, evidence artifacts, and failure owner with 100% accuracy.
- **SC-002**: An agent-ready validation run emits a consolidated verdict in 100% of completed, failed, unsupported, and degraded scenarios.
- **SC-003**: When required focused gates are omitted, the verdict names all missing gates and provides a next command in a single run.
- **SC-004**: When changed-path context is unavailable from both active feature metadata and git merge-base diff, the agent-ready validation path reports degraded authority and names a broad fallback command instead of claiming focused authority.
- **SC-005**: Metadata drift validation catches 100% of seeded mismatches between natively registered runnable targets, documented targets, target metadata, and validation contract references.
- **SC-006**: Generated app inspection confirms normal launch remains interactive and evidence-free unless an explicit evidence command is invoked.
- **SC-007**: Typed standard controls authoring prevents or rejects seeded misspellings for standard control kinds, standard event kinds, and standard chart or grid data attributes across every existing standard controls module while preserving a visibly custom extension path.
- **SC-008**: Environment-related validation failures are classified separately from product defects in at least four representative scenarios: unsupported desktop host, stale package prerequisite, missing generated artifact, and unavailable changed-path context.
