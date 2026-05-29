# Feature Specification: Serialize FAKE Runs

**Feature Branch**: `031-serialize-fake-runs`  
**Created**: 2026-05-29  
**Status**: Draft  
**Input**: User description: "don’t run FAKE-backed tests and FAKE targets concurrently in this repo; they can race on .fake. Sequential runs are clean."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Run Repository Validation Reliably (Priority: P1)

As a repository maintainer, I want repository validation guidance and automation to avoid running FAKE-backed tests and FAKE targets at the same time, so local and agent-driven validation does not fail intermittently because of shared `.fake` state.

**Independent Test**: From a clean checkout, follow the documented validation sequence for a change that requires both FAKE-backed tests and FAKE targets. The sequence runs each FAKE-backed operation one after another and completes without a `.fake` race.

### User Story 2 - Guide Agents Away From Unsafe Parallelism (Priority: P1)

As an AI coding agent working in this repository, I want explicit instructions that FAKE-backed commands are not safe to run concurrently, so I do not parallelize test and target execution in a way that creates avoidable failures.

**Independent Test**: Inspect the agent-facing workflow instructions and confirm that they name FAKE-backed tests, FAKE targets, the `.fake` race risk, and the requirement to run them sequentially.

### User Story 3 - Diagnose Race-Like Failures Clearly (Priority: P2)

As a contributor, I want validation failure notes to distinguish suspected concurrent FAKE execution from real product failures, so I can rerun the affected commands sequentially before investigating unrelated causes.

**Independent Test**: When evidence or readiness notes mention a failed FAKE-backed run, they include enough context to tell whether another FAKE-backed command was running at the same time and recommend a sequential rerun if concurrency is suspected.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Repository validation guidance MUST state that FAKE-backed tests and FAKE targets are not safe to run concurrently in this repository because they can race on shared `.fake` state.
- **FR-002**: Agent-facing instructions MUST require FAKE-backed tests and FAKE targets to be run sequentially, including when multiple validation commands appear independent.
- **FR-003**: Workflows, task instructions, and readiness guidance that ask for both FAKE-backed tests and FAKE targets MUST present them in a deterministic sequence rather than as parallel work.
- **FR-004**: Validation evidence MUST record the order of FAKE-backed commands when more than one such command is required for a feature or readiness claim.
- **FR-005**: Failure triage guidance MUST instruct contributors to rerun suspected race-affected FAKE-backed commands sequentially before treating the failure as a product regression.
- **FR-006**: Documentation MUST preserve the distinction between FAKE-backed commands, which require serialization, and unrelated non-FAKE checks, which may still be parallelized when otherwise safe.
- **FR-007**: The feature MUST avoid changing package identities, runtime behavior, public framework APIs, or visual output.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identities, package contents, package versions, or generated package consumers should change. No controls, chart, graph, or DataGrid package migration guidance is required.
- **Public contract impact**: No `.fsi` signatures, documented public APIs, sample contracts, or surface baselines should change.
- **State workflow impact**: Validation workflow guidance changes. Product stateful workflow, I/O commands, effects, subscriptions, and interpreter behavior should not change.
- **Layout/rendering impact**: No layout, charts, DataGrid, rendering, screenshots, Vulkan, Skia, visual output, or unsupported environment diagnostics should change.
- **Evidence obligations**: Required real evidence paths should include the updated spec artifacts and readiness evidence showing sequential FAKE-backed validation guidance, such as `specs/031-serialize-fake-runs/readiness/sequential-fake-validation.md`.
- **Unsupported scope**: Runtime UI behavior, visual demos, release packaging, platform support expansion, package publishing, and unrelated build-target redesign are out of scope.
- **Build-target impact**: Existing FAKE-backed targets may need guidance or validation ordering updates, but `Dev`, `Verify`, `Ci`, `PackLocal`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` should not be changed to run concurrently with other FAKE-backed work.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of updated agent-facing validation instructions that mention FAKE-backed tests or FAKE targets also state that those commands must run sequentially.
- **SC-002**: A maintainer can complete the required validation path for this feature using a sequential command order without encountering a `.fake` race.
- **SC-003**: Readiness evidence for this feature records the order of all FAKE-backed commands that were run.
- **SC-004**: Contributors reviewing a FAKE-backed failure can identify within 2 minutes whether the documented next step is a sequential rerun or normal defect investigation.
- **SC-005**: No public API, package identity, or user-facing rendering behavior changes are introduced by this feature.

## Assumptions

- FAKE-backed commands share `.fake` state in this repository and therefore are unsafe to run at the same time.
- Sequential FAKE-backed command execution is reliable for the affected validation workflows.
- Non-FAKE file reads, searches, and unrelated checks may still run concurrently when they do not invoke FAKE or depend on `.fake`.
