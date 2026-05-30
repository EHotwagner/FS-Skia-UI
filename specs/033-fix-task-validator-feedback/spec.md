# Feature Specification: Task Validator Feedback Follow-ups

**Feature Branch**: `033-fix-task-validator-feedback`  
**Created**: 2026-05-29  
**Status**: Draft  
**Input**: User description: "Mailbox/2026-05-29T22-16-55+0200-asteroids-demo-speckit-task-generation-analysis.md"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Avoid False Skill Requirements From Required Filenames (Priority: P1)

A Spec Kit task author creates setup or readiness tasks that mention mandated evidence filenames. The task graph validator does not infer an unrelated required skill from a substring inside a filename, so the author can keep task skill metadata semantically accurate.

**Independent Test**: Create a task title that names the required skill-loading readiness workflow file without declaring the implementation skill, run task graph validation, and confirm the task is accepted when no implementation workflow is actually requested.

### User Story 2 - Discover Validator Escape Hatches And Trigger Tokens (Priority: P1)

A Spec Kit task author can read task-generation guidance before validation and learn which title phrases trigger required skills, when the readiness-notes title prefix suppresses those checks, and how to phrase setup tasks without source-diving.

**Independent Test**: Starting from generated task guidance only, identify the readiness-notes prefix, the enforced trigger-token groups, and at least one example of safe wording for setup tasks that cite required readiness filenames.

### User Story 3 - Resolve Skill Registry Names Without Guesswork (Priority: P2)

A generated-project author chooses task skill ids from the same registry the validator uses. When a visible skill directory and its declared skill id differ, guidance and diagnostics make the accepted id clear before task graph validation blocks the author.

**Independent Test**: Inspect skill guidance for a skill whose directory name differs from its declared id, then confirm the author-facing guidance names the id that task metadata must declare.

### User Story 4 - Keep Task Guidance Aligned With Enforced Validation Rules (Priority: P2)

A Spec Kit maintainer updates validator expectations and the task authoring guidance stays aligned with the same enforced token set, preventing documentation from warning about stale examples while omitting live blocking phrases.

**Independent Test**: Compare the published task guidance against the validator expectations and confirm all enforced Spec Kit title-trigger groups are represented, with no obsolete-only examples presented as enforced failures.

### User Story 5 - Improve Non-Blocking Capability And Mode Signals (Priority: P3)

A task author and reviewer get clearer feedback about FS.Skia.UI capability skill choices and graph-only validation mode without turning advisory capability guidance into new blocking workflow friction.

**Independent Test**: Run graph-only validation and confirm its output is labeled as graph validation, while task guidance or diagnostics provide advisory help for FS.Skia.UI capability skill choices without failing otherwise valid task metadata.

### Synthetic Evidence Disclosure

This feature includes design-approved synthetic error-handling fixtures for
validator error paths where real task inputs are malformed by definition or
would require contrived repository skill states.

- **US1 synthetic dependency**: Filename-bound trigger-token fixtures use
  synthetic task titles to reproduce the false-positive validator behavior.
  Real evidence is captured through
  `specs/033-fix-task-validator-feedback/readiness/title-trigger-validation.md`,
  including failing-first and passing validator output.
- **US3 synthetic dependency**: Directory/id mismatch diagnostics use synthetic
  registry metadata or fixture task metadata to isolate the mismatch path. Real
  evidence is captured through
  `specs/033-fix-task-validator-feedback/readiness/skill-registry-diagnostics.md`,
  including the accepted declared id and source path shown to authors.
- **Replacement path**: Synthetic fixtures are limited to explicit validator
  error-path coverage approved during planning as `[SEH]`; repository guidance
  scans and command-output captures remain real evidence.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Task title capability matching MUST avoid treating trigger tokens found only as substrings inside longer filenames or words as high-confidence required-skill signals.
- **FR-002**: Task title capability matching MUST avoid requiring an implementation skill solely because a setup or readiness task cites a mandated skill-loading evidence filename.
- **FR-003**: Task-generation guidance MUST document the readiness-notes title prefix that suppresses capability expectation checks for setup/readiness aggregation tasks.
- **FR-004**: Task-generation guidance MUST document the actual enforced Spec Kit title-trigger groups for graph validation, evidence audit, task generation, implementation loading, and constitution-related work.
- **FR-005**: Task-generation guidance MUST distinguish blocking title-trigger rules from advisory examples and non-enforced authoring suggestions.
- **FR-006**: Skill registry guidance MUST identify the authoritative skill registry used by task validation and explain that declared skill ids come from each skill's declared name, not necessarily its directory name.
- **FR-007**: Validator diagnostics SHOULD identify the accepted skill id when an author declares a directory-like skill name that exists but resolves to a different declared id.
- **FR-008**: Task guidance SHOULD provide advisory FS.Skia.UI capability selection hints for common rendering, scene, viewer, input, layout, and evidence tasks without converting those hints into hard failures.
- **FR-009**: Graph-only validation output MUST clearly identify that only graph validation is running and MUST not imply that the full evidence audit is executing.
- **FR-010**: Follow-up guidance MUST classify each item as validator behavior, task-author guidance, skill-registry guidance, advisory capability guidance, or cosmetic output labeling so backlog scope remains explicit.
- **FR-011**: The updated workflow MUST preserve existing graph validation protections for cycles, dangling dependencies, skill metadata mirror mismatches, unreadable skills, and required Spec Kit skill ordering.
- **FR-012**: The updated workflow MUST preserve accepted validation for correctly authored task lists that already declare required Spec Kit skills and use the expected task dependency shape.

Each functional requirement is accepted when a targeted failing-first validation or guidance scan demonstrates the previous feedback case and then passes after the update.

### Change Classification

- **Tier**: Tier 1 (contracted governance behavior change)
- **Rationale**: This feature changes observable Spec Kit validator behavior,
  generated task-authoring guidance, diagnostics, and command output labels. It
  does not change public FS.Skia.UI runtime `.fsi` APIs, package identities,
  runtime rendering behavior, or package dependencies.
- **Public API impact**: No public F# runtime API or `.fsi` surface changes are
  planned. Markdown contracts, generated guidance, validator diagnostics, and
  governance command output are the contracted surfaces in scope.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package identities, package versions, generated package consumers, and runtime package contents are expected to remain unchanged. Package contents may change only for shipped Spec Kit templates, generated guidance fragments, or validation scripts included with generated consumers.
- **Public contract impact**: Public `.fsi` signatures and documented runtime APIs are not expected to change. Sample contracts and guidance baselines for task generation and evidence validation are in scope if they describe the validator behavior.
- **State workflow impact**: Runtime state workflow, I/O, commands, effects, subscriptions, and interpreter behavior are out of scope. Task-generation validation workflow and generated guidance workflow are in scope.
- **Layout/rendering impact**: Runtime layout, charts, DataGrid, Skia rendering, Vulkan behavior, screenshot output, and unsupported environment diagnostics are out of scope except where advisory FS.Skia.UI capability guidance names rendering-related task categories.
- **Evidence obligations**: Required real evidence paths include `specs/033-fix-task-validator-feedback/readiness/title-trigger-validation.md`, `specs/033-fix-task-validator-feedback/readiness/task-guidance-scan.md`, `specs/033-fix-task-validator-feedback/readiness/skill-registry-diagnostics.md`, `specs/033-fix-task-validator-feedback/readiness/advisory-capability-guidance.md`, and `specs/033-fix-task-validator-feedback/readiness/graph-only-output-label.md`.
- **Unsupported scope**: New game features, runtime rendering changes, release publishing, broad Spec Kit replacement, new package families, and generated demo implementation work are out of scope.
- **Build-target impact**: `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, and `EvidenceAudit` may need validation coverage. `Dev`, `Verify`, `Ci`, `PackLocal`, `DependencyReport`, and `TemplateDrift` should change only if existing touched artifacts require their normal validation.

## Success Criteria *(mandatory)*

- **SC-001**: A setup task title containing the mandated skill-loading readiness workflow filename validates without declaring the implementation skill when no implementation workflow is requested.
- **SC-002**: Guidance scans confirm that 100% of enforced Spec Kit title-trigger groups are documented before authors run graph validation.
- **SC-003**: Guidance scans confirm the readiness-notes prefix and at least three safe task-title examples are discoverable from task-generation guidance.
- **SC-004**: Skill registry diagnostics or guidance enable an author to identify the validator-accepted skill id for a directory/id mismatch in under 2 minutes.
- **SC-005**: Existing valid task lists with cycles absent, dependencies resolved, skill mirrors aligned, readable skills, and required Spec Kit skill ordering continue to pass graph validation.
- **SC-006**: Graph-only validation output is labeled clearly enough that reviewers can distinguish graph-only validation from full evidence audit execution in one log scan.
- **SC-007**: Advisory FS.Skia.UI capability guidance covers at least five common task categories without introducing new hard validation failures for otherwise valid task metadata.

## Assumptions

- The mailbox report accurately describes task-generation validation behavior observed during the Asteroids demo task-generation workflow.
- The follow-up should prioritize the blocking false positive and documentation drift before lower-impact advisory capability improvements.
- The authoritative validator and generated task guidance should remain synchronized through reviewable repository artifacts, even if the exact synchronization mechanism is chosen during planning.
- Advisory FS.Skia.UI capability guidance is useful only if it stays non-blocking unless a later specification explicitly expands validator enforcement.
