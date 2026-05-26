# Feature Specification: Synthetic Error Evidence

**Feature Branch**: `017-synthetic-error-evidence`  
**Created**: 2026-05-26  
**Status**: Draft  
**Input**: User description: "synthetic input is often necessary to test malformed input/error handling. it is not feasable to expect real input there. lets give these error handling tasks a special tag/label and accept the synthetic status. this classification must be done in the design phase/tasks and MUST NOT be done in the implementation phase to fix synthetic status."

## Clarifications

### Session 2026-05-26

- Q: What audit verdict should apply when all synthetic tasks are valid design-approved `[SEH]` tasks? → A: Audit verdict is PASS when all synthetic tasks are valid design-approved `[SEH]`; report lists accepted synthetic counts.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Classify Necessary Synthetic Error Tests During Design (Priority: P1)

As a maintainer planning a feature that includes malformed input, hostile payload, missing field, invalid state, timeout, or parser failure behavior, I want the design and task breakdown to mark those error-handling tasks as intentionally synthetic when real input is infeasible, so the work is reviewed honestly before implementation begins.

**Independent Test**: Given a plan and task list with malformed-input or error-path validation tasks, review the generated design artifacts before implementation starts. Each accepted synthetic error-handling task is marked with the task tag `[SEH]`, keeps synthetic status `[S]` when completed with synthetic-only evidence, and includes the label `synthetic-error-handling-approved` with a design-phase rationale explaining why real input is infeasible or inappropriate.

### User Story 2 - Accept Approved Synthetic Error-Handling Evidence Without Hiding It (Priority: P1)

As a reviewer, I want approved malformed-input and error-handling tasks to pass readiness without being converted into real-evidence tasks, so the audit accepts necessary synthetic status while still making the synthetic boundary visible.

**Independent Test**: Given a task graph containing completed `[S]` tasks that were tagged `[SEH]` and labeled `synthetic-error-handling-approved` in the design/task phase, the evidence audit reports them as accepted synthetic error-handling evidence rather than merge-blocking synthetic evidence. The same report still lists those tasks as synthetic and links the design rationale.

### User Story 3 - Reject Late Synthetic Reclassification During Implementation (Priority: P1)

As a maintainer enforcing evidence discipline, I want implementation-time attempts to add the error-handling synthetic label after an audit failure to be rejected, so contributors cannot use the new label to launder unplanned synthetic evidence.

**Independent Test**: Given a task that was not tagged `[SEH]` or labeled `synthetic-error-handling-approved` before implementation began, complete it with synthetic-only evidence and then add the label during implementation or readiness cleanup. The audit rejects the task as late-classified synthetic evidence and names the missing design-phase classification.

### User Story 4 - Separate Error-Handling Synthetic Evidence From Convenience Fixtures (Priority: P2)

As a planner, I want the synthetic error-handling exception to apply only to malformed-input and error-path cases where real input is not feasible, so ordinary mocks, shortcuts, unavailable integrations, or incomplete product behavior remain governed by the existing synthetic evidence rules.

**Independent Test**: Given representative tasks for malformed parser input, external-service convenience mocks, unavailable display hosts, and ordinary in-memory substitutes, only the malformed-input or explicit error-handling cases can receive the `[SEH]` tag. The remaining synthetic tasks continue to fail readiness unless handled by existing synthetic override policy.

### Edge Cases

- A task uses a tiny hand-authored malformed payload that could never come from a real successful user flow; it may be `[SEH]` if the design rationale names the exact error behavior being validated.
- A task uses generated invalid data merely because real integration setup is inconvenient; it must not receive `[SEH]`.
- A task validates an unsupported environment diagnostic; it may receive `[SEH]` only when the test input itself is malformed or an error-path fixture and the feature does not require real host evidence.
- A task is split after planning; derived tasks inherit `[SEH]` only when the split preserves the same approved error-handling scope and rationale.
- A reviewer discovers during implementation that a task needs synthetic malformed input but no design-phase classification exists; the task must return to planning/task update before it can be accepted under `[SEH]`.
- A `[SEH]` task depends on an ordinary `[S]` task; the ordinary synthetic dependency remains visible and is not automatically accepted by the error-handling exception.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The design and task-generation workflow MUST define a dedicated task tag `[SEH]` for synthetic error-handling tasks that intentionally require malformed, impossible, hostile, or otherwise non-real input.
- **FR-002**: The workflow MUST pair the `[SEH]` task tag with the label `synthetic-error-handling-approved` in task metadata or the Synthetic-Evidence Inventory.
- **FR-003**: A task MAY receive `[SEH]` only during design, planning, clarification, or task generation before implementation work for that task begins.
- **FR-004**: A `[SEH]` task MUST name the error-handling behavior under test, the synthetic input class, and the reason real input is infeasible, unsafe, impossible, or not representative of the error path.
- **FR-005**: Completed `[SEH]` tasks that rely only on synthetic evidence MUST keep synthetic status `[S]`; the workflow MUST NOT convert them to `[X]` solely because the synthetic evidence is accepted.
- **FR-006**: Evidence graph and audit reporting MUST distinguish accepted synthetic error-handling evidence from unaccepted synthetic evidence while still counting and displaying accepted tasks as synthetic.
- **FR-007**: Evidence audit MUST return a passing verdict when every synthetic task in scope is a design-approved `[SEH]` task with the required rationale and label; the report MUST still list accepted synthetic counts separately from real-evidence tasks.
- **FR-008**: Evidence audit MUST reject any task whose `[SEH]` tag or `synthetic-error-handling-approved` label first appears during implementation, readiness cleanup, or after an audit failure.
- **FR-009**: Task generation guidance MUST state that `[SEH]` classification is a design/task responsibility and MUST NOT be used during implementation to fix synthetic status.
- **FR-010**: Planning guidance MUST include examples of eligible `[SEH]` tasks, including malformed parser input, corrupt file content, invalid command arguments, protocol violations, missing required data, hostile payloads, and forced error-result fixtures.
- **FR-011**: Planning guidance MUST include non-eligible examples, including convenience mocks, incomplete integrations, unavailable product capability, missing host support, placeholder outputs, and tests that avoid real behavior for speed alone.
- **FR-012**: Synthetic-Evidence Inventory entries for `[SEH]` tasks MUST include the task identifier, label, design-phase source location, rationale, synthetic input class, expected error behavior, and reviewer-visible acceptance status.
- **FR-013**: If a task is added, split, renamed, or rescoped after task generation, the workflow MUST preserve or invalidate `[SEH]` classification based on whether the approved error-handling rationale still applies.
- **FR-014**: Implementation guidance MUST require contributors to send newly discovered synthetic error-handling needs back to the task/design phase rather than applying `[SEH]` locally during implementation.
- **FR-015**: Existing synthetic-evidence disclosures at task, code, test, inventory, and PR surfaces MUST remain required for `[SEH]` tasks unless explicitly superseded by a stronger disclosure in this feature.
- **FR-016**: The feature MUST NOT weaken evidence requirements for product readiness artifacts that require real behavior, supported-host execution, package validation, or public contract proof.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, package contents, package versions, or generated package consumers are expected to change. This is a governance workflow change for specification, planning, task metadata, and evidence audit acceptance.
- **Public contract impact**: No `.fsi` signatures, documented runtime public APIs, sample contracts, or surface baselines are expected to change. Spec Kit task and evidence contracts change by adding `[SEH]` and `synthetic-error-handling-approved`.
- **State workflow impact**: Product state workflows are out of scope. Governance workflow state changes because task classification must record when `[SEH]` was assigned, where the design rationale lives, and whether audit acceptance is allowed.
- **Layout/rendering impact**: No layout, charts, DataGrid, rendering, screenshots, Vulkan, Skia, visual output, or unsupported environment diagnostics change. Unsupported environment diagnostics remain governed by their feature-specific real-evidence requirements.
- **Evidence obligations**: Required real evidence paths should include `specs/017-synthetic-error-evidence/readiness/seh-classification-rules.md`, `specs/017-synthetic-error-evidence/readiness/task-generation-seh.md`, `specs/017-synthetic-error-evidence/readiness/audit-accepted-seh.md`, `specs/017-synthetic-error-evidence/readiness/audit-rejects-late-seh.md`, `specs/017-synthetic-error-evidence/readiness/non-eligible-synthetic-cases.md`, `specs/017-synthetic-error-evidence/readiness/generated-guidance-check.md`, `specs/017-synthetic-error-evidence/readiness/evidence-graph.md`, and `specs/017-synthetic-error-evidence/readiness/evidence-audit.md`.
- **Unsupported scope**: This feature does not approve arbitrary synthetic evidence, remove `[S]` or `[S*]` reporting, replace the Synthetic-Evidence Inventory, remove PR disclosure, change runtime product behavior, or allow implementation-time synthetic status cleanup.
- **Build-target impact**: `EvidenceGraph`, `EvidenceAudit`, task generation, planning guidance, implementation guidance, and generated governance checks may need updates. `Dev`, `Verify`, and `Ci` may change only if they aggregate those checks. `PackLocal`, `TemplateCheck`, `DependencyReport`, and `TemplateDrift` should change only if they currently inspect task or evidence guidance.

### Key Entities

- **Synthetic Error-Handling Task**: A task tagged `[SEH]` whose purpose is to validate malformed input or an explicit error path using synthetic evidence that cannot reasonably be replaced by real input.
- **Accepted Synthetic Error-Handling Label**: The metadata label `synthetic-error-handling-approved`, assigned before implementation to authorize audit acceptance while preserving synthetic disclosure.
- **Design-Phase Rationale**: Reviewer-visible explanation recorded in planning or task artifacts that justifies why the task needs synthetic malformed or error-path input.
- **Late Reclassification Attempt**: Any attempt to add `[SEH]` or `synthetic-error-handling-approved` after implementation for that task has begun or after a synthetic audit failure has been observed.
- **Synthetic-Evidence Inventory Entry**: The task-level disclosure record that lists accepted and unaccepted synthetic tasks with their rationale, status, and audit treatment.

### Assumptions

- Malformed input and error-path behavior often cannot be tested with real successful user input because the value of the test is the impossible, invalid, hostile, or corrupt condition itself.
- The existing `[S]` synthetic status remains the honest task status for synthetic-only evidence; this feature changes audit acceptance for a narrow approved class rather than redefining synthetic evidence as real.
- The design/task phase is the correct decision point because reviewers can evaluate scope and evidence expectations before implementation incentives exist.
- Current `--accept-synthetic` behavior remains available for broader exceptional cases, but `[SEH]` provides a narrower pre-approved path for malformed-input and error-handling work that can produce a passing audit verdict when no other blocking synthetic evidence remains.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of accepted synthetic malformed-input or error-handling tasks include `[SEH]`, `synthetic-error-handling-approved`, and a design-phase rationale before implementation begins.
- **SC-002**: Evidence audit returns PASS for 100% of fixture task graphs where every synthetic task is valid design-approved `[SEH]`, while still reporting those tasks as synthetic `[S]`.
- **SC-003**: Evidence audit rejects 100% of fixture tasks where `[SEH]` or `synthetic-error-handling-approved` is added only during implementation or after an audit failure.
- **SC-004**: Planning guidance lets a reviewer correctly classify at least eight representative examples as eligible or non-eligible for `[SEH]` in under 10 minutes.
- **SC-005**: No ordinary synthetic fixture, convenience mock, unsupported-host substitute, or speed-only shortcut passes audit under the `[SEH]` exception in covered validation cases.
- **SC-006**: Reviewers can identify all accepted synthetic error-handling tasks, their rationale, and their audit acceptance status from the task list or Synthetic-Evidence Inventory within 2 minutes.
