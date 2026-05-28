# Feature Specification: Upgrade SkiaSharp And Spec Kit

**Feature Branch**: `025-upgrade-skia-speckit`  
**Created**: 2026-05-28  
**Status**: Draft  
**Input**: User description: "docs/2026-05-27-2217-compatibility-package-analysis.md and upgrade skiasharp and speckit versions"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Govern Dependency Upgrade Readiness (Priority: P1)

A maintainer reviews the compatibility package analysis, upgrades the SkiaSharp and Spec Kit version inputs, and can prove the resulting package graph, template output, generated governance assets, and documentation still describe the intended package posture.

**Independent Test**: Starting from the completed feature, a reviewer can inspect the readiness evidence and confirm that the selected SkiaSharp and Spec Kit versions are recorded, dependency reports are refreshed, generated template pins are aligned, and no unapproved compatibility-package surface or dependency changes occurred.

### User Story 2 - Preserve Compatibility Package Direction (Priority: P1)

A maintainer deciding what to do with `FS.Skia.UI` can rely on concrete consumer inventory, public surface classification, replacement coverage, dependency evidence, and release policy instead of file size or incidental refactoring pressure.

**Independent Test**: A reviewer can open the readiness artifacts and verify that every repository consumer of `FS.Skia.UI` is listed, public compatibility areas are classified, focused-package replacements are named where available, and the release posture is documented before any package-surface migration is accepted.

### User Story 3 - Keep Generated Users On Supported Package Pins (Priority: P2)

A user who creates a new generated project receives package and Spec Kit assets that match the repository's approved package story, without accidentally depending on the broad compatibility package or stale governance templates.

**Independent Test**: Generated project validation demonstrates that package references, generated Spec Kit files, selected local skills, template docs, and guidance are synchronized with the upgraded repository inputs and still build or validate under the supported profiles.

### User Story 4 - Document Upgrade And Compatibility Outcomes (Priority: P2)

A release reviewer can understand which versions changed, why they were accepted, what compatibility risks were checked, and which compatibility-package decisions remain deferred.

**Independent Test**: Documentation and release notes state the upgraded inputs, compatibility-package posture, known migration boundaries, and any unsupported or deferred decisions in user-facing language.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST identify the approved target versions for SkiaSharp runtime packages and Spec Kit assets at implementation time, using current source-of-truth package and governance metadata rather than hard-coded assumptions from this specification.
- **FR-002**: System MUST update every repository-owned declaration, generated template pin, generated Spec Kit metadata file, and package guidance page that is required to keep SkiaSharp and Spec Kit versions consistent for supported consumers.
- **FR-003**: System MUST keep all SkiaSharp package variants version-aligned unless documented compatibility evidence proves that a deliberate mismatch is required.
- **FR-004**: System MUST refresh dependency documentation and dependency-report evidence so reviewers can compare the package graph before and after the upgrade.
- **FR-005**: System MUST preserve existing public compatibility package behavior unless a change is explicitly classified, documented, and covered by package surface evidence.
- **FR-006**: System MUST produce a repository consumer inventory for `FS.Skia.UI` covering project references, package references, namespace usage, samples, templates, and documentation that still depend on the broad compatibility package.
- **FR-007**: System MUST classify the public compatibility package surface into primary-only compatibility members, duplicates of focused package concepts, facade candidates, deprecated candidates, and permanent compatibility-owned surface.
- **FR-008**: System MUST map focused replacement packages for representative compatibility scenarios where replacements already exist, and clearly mark scenarios where no focused replacement is ready.
- **FR-009**: System MUST document the accepted near-term compatibility posture for `FS.Skia.UI`, including whether it remains frozen, permanent broad, facade-oriented, deprecated, or still deferred pending evidence.
- **FR-010**: System MUST validate that generated templates do not accidentally regain or expand a broad `FS.Skia.UI` dependency when focused packages are the intended authoring path.
- **FR-011**: System MUST preserve supported sample behavior in local project-reference mode and packaged mode, or document each intentional migration with replacement guidance.
- **FR-012**: System MUST preserve unsupported-host and viewer diagnostic behavior as observable compatibility behavior unless an intentional change is documented with test and release evidence.
- **FR-013**: System MUST update user-facing docs and release notes so new users are directed toward focused packages while existing compatibility-package users receive conservative migration guidance.
- **FR-014**: System MUST run and record the relevant package, template, generated-guidance, dependency, surface, and evidence-governance checks before the feature is considered ready for implementation completion.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package versions and package contents change for SkiaSharp-related dependencies and Spec Kit-generated assets. The `FS.Skia.UI` compatibility package identity must remain stable unless a later planning artifact explicitly authorizes a documented posture change. Generated package consumers and template pins must be reviewed.
- **Public contract impact**: `.fsi` signatures and public APIs should not change for this upgrade by default. If package-surface or compatibility members change, surface baselines, public documentation, representative sample contracts, and release notes must record the intentional difference.
- **State workflow impact**: No product state workflow, command, effect, subscription, or interpreter behavior is expected to change. Build, package, template, and evidence workflows may change only to support upgraded version metadata and validation.
- **Layout/rendering impact**: SkiaSharp version changes may affect rendering, screenshots, Vulkan/Skia startup, native asset loading, or unsupported environment diagnostics. Visual and screenshot evidence must distinguish dependency-upgrade regressions from pre-existing host limitations.
- **Evidence obligations**: Required real evidence paths are `specs/025-upgrade-skia-speckit/readiness/version-selection.md`, `specs/025-upgrade-skia-speckit/readiness/dependency-report.md`, `specs/025-upgrade-skia-speckit/readiness/template-version-alignment.md`, `specs/025-upgrade-skia-speckit/readiness/compatibility-consumer-inventory.md`, `specs/025-upgrade-skia-speckit/readiness/compatibility-public-surface-map.md`, `specs/025-upgrade-skia-speckit/readiness/compatibility-sample-migration.md`, `specs/025-upgrade-skia-speckit/readiness/compatibility-release-policy.md`, `specs/025-upgrade-skia-speckit/readiness/package-surface-baseline.md`, and `specs/025-upgrade-skia-speckit/readiness/evidence-audit.md`.
- **Unsupported scope**: Do not remove public compatibility APIs solely because focused packages exist. Do not make focused packages depend on the broad compatibility package. Do not redesign renderer architecture, add new desktop platform support, collapse generated profiles into a single broad dependency set, or publish packages as part of this feature unless release automation is explicitly invoked later.
- **Build-target impact**: `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`, `Verify`, `Ci`, and package surface checks must be reviewed. `Dev` and `PackLocal` semantics should remain unchanged unless version validation requires a documented update.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of repository-owned SkiaSharp package declarations and generated template pins reference the same approved SkiaSharp version family after the upgrade.
- **SC-002**: 100% of repository-owned Spec Kit metadata, generated templates, command assets, and selected skills that require version updates are aligned with the approved Spec Kit version or documented compatibility range.
- **SC-003**: Reviewers can trace every `FS.Skia.UI` repository consumer to a recorded classification and migration posture in under 10 minutes using the readiness inventory.
- **SC-004**: Package surface evidence shows zero accidental public compatibility-package changes, or every difference is explicitly approved with migration guidance.
- **SC-005**: Generated template validation passes for supported profiles and shows no unintended broad compatibility-package dependency.
- **SC-006**: Dependency-report evidence identifies no new package cycles and no unexplained dependency spread from focused packages into the compatibility package.
- **SC-007**: Release-facing documentation states the version upgrade, compatibility posture, and deferred decisions clearly enough that a user can choose between focused packages and the compatibility package without conflicting guidance.
- **SC-008**: Required package, template, generated-guidance, dependency, surface, and evidence-governance checks complete with recorded logs or explicit unsupported-host facts.

## Key Entities

- **Version Upgrade Decision**: Records the selected SkiaSharp and Spec Kit versions, source of truth, approval rationale, affected files, and known compatibility risks.
- **Compatibility Consumer Inventory**: Lists every repository consumer of `FS.Skia.UI`, including project/package references, namespace usage, samples, templates, docs, and packaged-mode behavior.
- **Public Surface Classification**: Classifies each compatibility-package public area by ownership posture and focused-package replacement status.
- **Focused Replacement Map**: Connects compatibility scenarios to focused package alternatives where available and records gaps where compatibility must remain primary.
- **Dependency Evidence Report**: Captures package graph changes, dependency closure, cycle checks, and before/after comparison for upgraded dependencies.
- **Template Alignment Evidence**: Shows generated package pins, generated Spec Kit assets, docs, and selected skills are synchronized with the repository version decisions.
- **Release Policy Note**: States the accepted user-facing compatibility posture, migration window, and deferred decisions for this feature.

## Assumptions

- The upgrade should target the newest compatible approved versions available during implementation, not the versions current on the day this specification is written.
- The compatibility package remains stable by default; this feature inventories and governs its future direction but does not remove public APIs unless later evidence explicitly justifies the change.
- Generated templates continue to prefer focused packages for new authoring unless a profile deliberately selects compatibility-package behavior.
- External consumer usage cannot be fully proven from the repository, so unknown users receive conservative compatibility and migration guidance.
