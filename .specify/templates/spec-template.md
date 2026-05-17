# Feature Specification: [FEATURE NAME]

**Feature Branch**: `[###-feature-name]`  
**Created**: [DATE]  
**Status**: Draft  
**Input**: User description: "$ARGUMENTS"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - [Brief Title] (Priority: P1)

[Describe the user journey and independent test.]

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST [specific capability]

### Framework Governance Prompts *(mandatory)*

- **Package impact**: State whether package identities, package contents,
  package versions, or generated package consumers change. For controls,
  chart, graph, or DataGrid authoring changes, name the active package path
  and any legacy Charts package migration guidance.
- **Public contract impact**: State whether `.fsi` signatures, documented public
  APIs, sample contracts, or surface baselines change.
- **State workflow impact**: State whether stateful workflow, I/O, commands,
  effects, subscriptions, or interpreter behavior changes.
- **Layout/rendering impact**: State whether layout, charts, DataGrid,
  rendering, screenshots, Vulkan, Skia, visual output, or unsupported
  environment diagnostics change.
- **Evidence obligations**: Name the required real evidence paths.
- **Unsupported scope**: Name visual, release, platform, distribution, or
  roadmap boundaries that are out of scope.
- **Build-target impact**: State whether `Dev`, `Verify`, `Ci`, `PackLocal`,
  `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`,
  `TemplateDrift`, `EvidenceGraph`, or `EvidenceAudit` must change.

## Success Criteria *(mandatory)*

- **SC-001**: [Measurable outcome]
