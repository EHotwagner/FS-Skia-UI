# Feature Specification: Phased Refactor Cleanup

**Feature Branch**: `023-phased-refactor-cleanup`  
**Created**: 2026-05-27  
**Status**: Draft  
**Input**: User description: "docs/2026-05-27-2204-refactoring-analysis.md"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Simplify Generated Product Evidence (Priority: P1)

As a framework maintainer, I want generated product evidence and report-writing
behavior consolidated without changing generated command names or report fields,
so that future evidence commands can be reviewed in one clearly owned place.

**Independent Test**: Generate every supported product profile, run its existing
evidence commands, and verify that each command still emits the same required
field names, status vocabulary, output paths, and exit-code meaning as before.

### User Story 2 - Split Generated Product Responsibilities (Priority: P1)

As a generated template consumer, I want generated source files organized by
product responsibility, so that the entrypoint is readable and profile-specific
behavior is easier to inspect.

**Independent Test**: Instantiate the same generated profiles as the current
template supports, inspect that the generated entrypoint contains only command
dispatch and launch responsibilities, and verify that each profile builds and
passes its generated tests.

### User Story 3 - Make Build Governance Easier To Maintain (Priority: P2)

As a repository maintainer, I want build and governance helper responsibilities
separated while preserving the public command surface, so that target behavior
can be reviewed without scanning unrelated helper logic.

**Independent Test**: Run the existing focused governance targets and confirm
that target names, dependencies, evidence paths, report outputs, and failure
messages remain stable for users.

### User Story 4 - Reduce Viewer Runtime Coordination Hotspots (Priority: P3)

As a runtime maintainer, I want viewer diagnostics, visual evidence, host
capability checks, and window behavior responsibilities separated behind the
same public viewer behavior, so that screenshot and visual evidence changes are
less risky.

**Independent Test**: Run the viewer behavior tests and supported or unsupported
host evidence checks, confirming that observable viewer behavior and evidence
classification are unchanged.

### Edge Cases

- A generated profile that does not reference product testing helpers must still
  remain standalone and must not gain an unnecessary dependency.
- Unsupported screenshot hosts must continue to produce explicit unsupported
  evidence rather than claiming screenshot proof.
- Refactoring must preserve all existing generated command names, report field
  names, target names, readiness paths, package identities, profile names, and
  public surface baselines unless a later feature explicitly authorizes a
  contract change.
- Any duplicate helper removed during cleanup must have equivalent behavior
  covered by generated, package, or governance checks.
- Pre-existing test failures must be recorded before refactoring so they are not
  attributed to behavior-preserving cleanup.

## Change Classification *(mandatory)*

This feature is **Tier 2 (internal change)**. It is a behavior-preserving
refactor that must not change public `.fsi` signatures, public package
identities, generated command names, generated profile names, report fields,
status vocabulary, output paths, exit-code meanings, FAKE target names, surface
baselines, or readiness artifact paths.

Verification is by baseline capture before each phase, focused phase checks
after each phase, unchanged public surface evidence where relevant, and final
`EvidenceGraph` / `EvidenceAudit` validation.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The refactor MUST preserve all externally visible generated
  command names, report fields, status vocabulary, output paths, exit-code
  meanings, generated profile names, public package identities, and governance
  target names unless a separate approved feature changes them.
- **FR-002**: Generated product evidence commands MUST use one local report
  writing approach per generated product so equivalent status and field behavior
  is not reimplemented through multiple specialized writers.
- **FR-003**: Generated product source MUST separate product model, rendering
  description, layout evidence, evidence commands, window options, and entrypoint
  responsibilities enough that each responsibility can be reviewed without
  unrelated command or profile logic.
- **FR-004**: Generated profile outputs MUST remain buildable and testable for
  every profile supported before the refactor.
- **FR-005**: Build and governance command behavior MUST remain available under
  the same user-facing target names and MUST continue to write the same
  readiness artifacts and report paths.
- **FR-006**: Repository-local duplicated helper behavior for process execution,
  report writing, scalar or list parsing, generated scanning, package
  resolution, and process-health policy MUST be classified as intentional or
  consolidated when consolidation does not cross package or template isolation
  boundaries.
- **FR-007**: Viewer runtime cleanup MUST preserve the public viewer contract
  and all observable behavior for diagnostics, window behavior validation,
  desktop or unsupported-host classification, visual evidence, and screenshot
  evidence.
- **FR-008**: The first refactoring pass MUST NOT change public signatures,
  package IDs, generated evidence field names, target dependency semantics, or
  readiness artifact paths.
- **FR-009**: Any later compatibility package restructuring MUST be treated as a
  separate design decision with migration guidance rather than included in this
  behavior-preserving cleanup.
- **FR-010**: Refactoring work MUST proceed in phases, starting with generated
  evidence/report cleanup, then generated source splitting, then build
  governance decomposition, then viewer internals, with compatibility package
  review deferred.
- **FR-011**: Before a phase begins, maintainers MUST capture current repository
  status and run the smallest relevant checks needed to identify pre-existing
  failures for that phase.
- **FR-012**: A phase MUST be accepted only when the checks relevant to that
  phase prove that user-facing behavior, generated outputs, and evidence
  semantics remain stable.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package identities and package versions are not expected
  to change. Package contents may change only through internal file movement or
  generated-template source organization. Generated package consumers must
  observe the same commands, fields, profiles, and behavior.
- **Public contract impact**: Public signatures, documented public APIs, sample
  contracts, and surface baselines must remain unchanged for the first cleanup
  pass. If any public contract change becomes necessary, it must be moved to a
  separate feature.
- **State workflow impact**: Product state workflows, commands, effects,
  subscriptions, and host interpretation semantics must remain behaviorally
  unchanged. The work may reorganize where responsibilities live.
- **Layout/rendering impact**: Layout, rendering, visual output, screenshot
  classification, and unsupported-environment diagnostics must remain
  behaviorally unchanged. Visual evidence may be reorganized internally but must
  preserve observable results.
- **Evidence obligations**: Required real evidence paths are
  `specs/023-phased-refactor-cleanup/readiness/baseline-status.md`,
  `specs/023-phased-refactor-cleanup/readiness/generated-evidence-cleanup.md`,
  `specs/023-phased-refactor-cleanup/readiness/template-split-validation.md`,
  `specs/023-phased-refactor-cleanup/readiness/build-governance-decomposition.md`,
  and `specs/023-phased-refactor-cleanup/readiness/viewer-internal-boundary.md`.
- **Unsupported scope**: Out of scope are UI model redesign, runtime replacement,
  public package signature changes, compatibility package API removal, release
  automation rewrite, new shared utility packages, generated profile collapse,
  weakened evidence requirements, and broad compatibility package migration.
- **Build-target impact**: `TemplateCheck`, `GeneratedGuidanceCheck`,
  `TemplateDrift`, package surface checks, focused package tests,
  `EvidenceGraph`, and `EvidenceAudit` must validate preserved behavior for
  touched phases. `Dev`, `Verify`, `Ci`, `PackLocal`, and `DependencyReport`
  must remain user-facing stable and change only if decomposition requires
  internal wiring updates without behavior changes.

### Assumptions

- The refactoring analysis document is the authoritative feature description
  for this specification.
- The first delivery slice is behavior-preserving cleanup, not a public API or
  package strategy redesign.
- Existing generated profiles, checks, and readiness conventions are sufficient
  to prove stability when run at the phase boundaries named above.
- Some duplication is intentional because generated products and package
  boundaries require isolation; the feature consolidates only drift-prone or
  repository-local duplication.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of generated profiles supported before the refactor still
  instantiate, build, and pass their generated validation checks after the
  generated cleanup phases.
- **SC-002**: 100% of generated evidence commands affected by the cleanup emit
  the same required field names, status vocabulary, output path behavior, and
  exit-code meanings as before.
- **SC-003**: The generated product entrypoint is reduced to launch and command
  dispatch responsibilities, with product model, view description, evidence
  commands, layout evidence, and window options inspectable in separate owned
  areas.
- **SC-004**: All user-facing build and governance target names affected by the
  cleanup remain unchanged, and focused target checks confirm the same readiness
  artifact paths and success/failure outcomes.
- **SC-005**: Viewer runtime cleanup completes with zero intentional changes to
  public surface baselines and zero observable changes in viewer diagnostics,
  host classification, visual evidence, or screenshot evidence behavior.
- **SC-006**: Each refactoring phase records baseline status and final evidence
  in its readiness file, including any pre-existing failures and the checks used
  to prove behavior stability.
- **SC-007**: Maintainers can review future generated evidence or template
  profile changes by inspecting a smaller responsibility-specific area instead
  of a single mixed-responsibility generated entrypoint.
