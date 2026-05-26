# Feature Specification: Improve Governance Weaknesses

**Feature Branch**: `015-improve-governance-weaknesses`  
**Created**: 2026-05-26  
**Status**: Draft  
**Input**: User description: "improve on Main Weaknesses: the implementation-time load declared skills before work rule is only partly enforceable; obviously applicable skill detection is heuristic and regex-based; the build system is large and governance-heavy; aggregate Dev build intermittently hung in Smoke.Tests though the direct smoke test passed quickly; runtime product remains narrow: Vulkan-only desktop, .NET 10, SkiaSharp preview dependencies, no macOS/mobile/browser/software-renderer fallback."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Audit Skill Loading Evidence (Priority: P1)

As a framework maintainer, I want task implementation evidence to show when declared skills were loaded and used before work began, so I can distinguish compliant workflow execution from after-the-fact or missing skill claims.

**Independent Test**: Run implementation evidence review for tasks with declared skills and verify each task records the declared skill list, load timing, source path, and any reviewer-visible exception before task work is accepted. A task that lacks this evidence is marked incomplete with a diagnostic naming the task and missing evidence.

### User Story 2 - Calibrate Applicable-Skill Detection (Priority: P1)

As a maintainer reviewing task readiness, I want applicable-skill detection to report confidence and uncertainty instead of presenting heuristic matches as proof, so human reviewers can resolve missed or ambiguous skill assignments before implementation starts.

**Independent Test**: Evaluate a task list containing obvious matches, ambiguous matches, and intentionally skill-free tasks. The readiness report accepts obvious matches, flags ambiguous or low-confidence cases for review, and records reviewer disposition without treating heuristic silence as evidence that no skill applies.

### User Story 3 - Make Governance Cost Visible and Proportionate (Priority: P2)

As a contributor making a small change, I want governance checks to identify the minimum required evidence path for the risk level of the change, so simple work does not require broad operational overhead unless its scope justifies it.

**Independent Test**: Classify representative small, medium, and broad changes. The workflow names the required focused checks, explains why broad checks are or are not required, and rejects final readiness only when the selected evidence path does not satisfy the declared risk level.

### User Story 4 - Diagnose Aggregate Build Hangs (Priority: P2)

As a maintainer running broad validation, I want intermittent aggregate hangs to produce a timeout verdict, focused rerun guidance, and enough stage evidence to isolate orchestration problems from deterministic test failures.

**Independent Test**: Simulate or reproduce a broad validation hang while the focused smoke test still passes within its expected window. The aggregate records the hung stage, elapsed time, last active command, focused rerun outcome, and final verdict category without reporting a product failure unless a product check actually fails.

**Synthetic Evidence Boundary**: Simulated hang logs may be used only as validation fixtures for timeout classification behavior. They must be disclosed as synthetic evidence in task status, fixture names, readiness notes, and PR summary. Real evidence is provided by either a reproduced aggregate hang transcript or a documented focused rerun showing the aggregate verdict remains non-authoritative.

### User Story 5 - State Runtime Portability Roadmap Boundaries (Priority: P3)

As a project stakeholder, I want runtime platform constraints and fallback gaps to be visible as explicit product limitations, so governance work does not imply broader platform support than the product currently provides.

**Independent Test**: Review product readiness and roadmap notes and verify current platform constraints, dependency maturity, and unsupported fallback modes are named as limitations. Feature readiness remains bounded unless a separate platform-expansion feature is opened.

### Edge Cases

- A task declares a skill and the agent claims it was loaded, but no timestamped or task-linked evidence exists.
- A skill is relevant by capability ownership but does not match simple task text, path names, or trigger phrases.
- Multiple skills partially match a task and reviewers need to decide whether one, several, or none are required.
- A small documentation or metadata change appears in a governance-heavy area but does not affect runtime behavior or generated outputs.
- A broad aggregate run hangs while focused checks pass quickly, leaving mixed evidence that must not be overstated.
- Platform constraints may be known product limitations rather than defects in the current feature.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The implementation workflow MUST require per-task evidence that each declared skill was loaded before task work began.
- **FR-002**: Skill-loading evidence MUST identify the task, declared skill identifier, resolved skill source, load result, and timing relative to task work.
- **FR-003**: A task with declared skills MUST NOT be accepted as complete when skill-loading evidence is missing, unverifiable, or recorded only after task work is complete.
- **FR-004**: Skill-loading review MUST allow an explicit reviewer exception only when the exception names the task, skill, reason, approving reviewer, and compensating evidence.
- **FR-005**: Applicable-skill detection MUST report match confidence, matched signals, and unresolved ambiguity rather than treating heuristic results as authoritative proof.
- **FR-006**: Task readiness validation MUST surface cases where no skill was selected but task content touches a skill-owned capability, even when the match is indirect.
- **FR-007**: The workflow MUST maintain validation examples for obvious skill matches, ambiguous matches, indirect ownership matches, false positives, and valid empty skill lists.
- **FR-008**: Governance guidance MUST define risk levels for small, medium, and broad changes and name the minimum evidence expected for each level.
- **FR-009**: Governance reports MUST explain when broad aggregate validation is required, when focused validation is sufficient, and when a broad result is non-authoritative.
- **FR-010**: Broad validation MUST record timeout or hang diagnostics including the stage, elapsed duration, last observed command, and recommended focused rerun.
- **FR-011**: When focused validation passes but aggregate validation hangs, readiness evidence MUST classify the result as an orchestration or environment concern unless a product check produced a product failure.
- **FR-012**: Aggregate validation guidance MUST include a bounded timeout policy for smoke-level stages and a retry or isolation path for intermittent hangs.
- **FR-013**: Product readiness or roadmap documentation MUST explicitly state current runtime platform constraints, fallback limitations, and dependency maturity risks.
- **FR-014**: Platform limitation documentation MUST distinguish current supported scope from future expansion so governance evidence does not imply unsupported platforms or renderers are available.
- **FR-015**: The feature MUST NOT expand runtime platform support, change package identity, or replace rendering dependencies unless a separate product feature is specified.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, package contents, package version, or generated package consumer change is intended. Documentation and validation evidence may describe package and dependency maturity risks without changing distribution.
- **Public contract impact**: No `.fsi` signatures, public runtime APIs, or sample contracts are expected to change. Workflow evidence contracts for skill loading, confidence reporting, and timeout verdicts may change.
- **State workflow impact**: Product state workflows are out of scope. Governance workflow state is in scope, including task skill evidence status, reviewer exceptions, risk-level evidence selection, and aggregate timeout verdicts.
- **Layout/rendering impact**: No layout, charts, DataGrid, rendering behavior, screenshots, Vulkan behavior, Skia behavior, visual output, or unsupported environment diagnostics are changed by this feature. Runtime platform and fallback limitations are documented only.
- **Evidence obligations**: Required real evidence paths should include `specs/015-improve-governance-weaknesses/readiness/skill-loading-evidence.md`, `specs/015-improve-governance-weaknesses/readiness/skill-detection-calibration.md`, `specs/015-improve-governance-weaknesses/readiness/governance-risk-levels.md`, `specs/015-improve-governance-weaknesses/readiness/aggregate-hang-diagnostics.md`, `specs/015-improve-governance-weaknesses/readiness/runtime-limitations.md`, `specs/015-improve-governance-weaknesses/readiness/evidence-graph.md`, and `specs/015-improve-governance-weaknesses/readiness/evidence-audit.md`.
- **Unsupported scope**: Creating new capability skills, adding platform support, adding software rendering, changing dependency versions, simplifying the entire build system, replacing broad validation, or guaranteeing agent honesty without recorded evidence are out of scope.
- **Build-target impact**: `Dev`, `EvidenceGraph`, `EvidenceAudit`, `GeneratedGuidanceCheck`, and task readiness checks may need updates. `Verify`, `Ci`, `PackLocal`, `TemplateCheck`, `DependencyReport`, and `TemplateDrift` should change only if existing governance or timeout reporting depends on them.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of completed tasks with non-empty declared skills include reviewer-visible pre-work skill-loading evidence.
- **SC-002**: Task readiness reports identify obvious and ambiguous skill-selection issues for a representative validation set in under 30 seconds.
- **SC-003**: Validation examples include at least five cases covering obvious matches, ambiguous matches, indirect ownership matches, false positives, and valid empty skill lists.
- **SC-004**: Contributors can identify the minimum required evidence path for a small change within 5 minutes using the governance guidance.
- **SC-005**: An aggregate hang produces a timeout or orchestration verdict with stage, elapsed duration, and focused rerun guidance within the configured timeout window.
- **SC-006**: When the focused smoke test passes after an aggregate hang, readiness evidence clearly separates the passing focused result from the unresolved aggregate orchestration result.
- **SC-007**: Runtime limitation notes name all currently unsupported platform or fallback categories covered by this feature and do not claim support beyond the tested product scope.

## Assumptions

- Honest recording is still required from the agent or operator, but readiness can require evidence strong enough for review rather than relying only on a narrative claim.
- Heuristic skill detection remains useful as a first pass, but reviewer-visible confidence and calibration examples are required to prevent false certainty.
- The aggregate hang described by the user is treated as intermittent orchestration behavior unless new evidence shows a deterministic product failure.
- Runtime platform expansion requires separate product specifications because it affects support, dependencies, testing, and release promises beyond this governance follow-up.

## Key Entities

- **Task Skill Evidence**: Reviewer-visible record that a task's declared skills were resolved and loaded before work began.
- **Skill Match Assessment**: Readiness result describing matched signals, confidence, ambiguity, and reviewer disposition for applicable skills.
- **Governance Risk Level**: Classification that maps change scope to the minimum required evidence path.
- **Validation Verdict**: The outcome category for a focused or aggregate run, including pass, product failure, environment failure, timeout, and non-authoritative result.
- **Runtime Limitation**: A documented product boundary for supported platforms, rendering paths, dependency maturity, or fallback availability.
