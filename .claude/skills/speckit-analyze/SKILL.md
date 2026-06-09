---
name: "speckit-analyze"
description: "Perform a non-destructive cross-artifact consistency and quality analysis across spec.md, plan.md, and tasks.md after task generation."
compatibility: "Requires spec-kit project structure with .specify/ directory"
metadata:
  author: "github-spec-kit"
  source: "templates/commands/analyze.md"
---


## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Pre-Execution Checks

**Check for extension hooks (before analysis)**:
- Discover hooks across **all** extension files (multi-file discovery), not just the central file:
  - Read `.specify/extensions.yml` from the project root (if present) and collect entries under the `hooks.before_analyze` key.
  - Then enumerate every `.specify/extensions/*/*.yml` file in sorted order, parse each, and collect its `hooks.before_analyze` entries too — so a hook registered only in a per-extension file (e.g. the `feedback` extension at `.specify/extensions/feedback/feedback.yml`) is still discovered and runs.
  - Merge all collected entries and dedupe by `(extension, command)` (first occurrence wins, so a hook declared in both files runs once).
  - If a file is absent, no hooks are registered, or its YAML cannot be parsed/is invalid, skip that file silently and continue.
- For every `optional: true` hook that is discovered but not executed this phase, emit one line so the skip is a visible decision: `Note: optional hook {extension}:{command} is registered but was not run (skipped).`
- Filter out hooks where `enabled` is explicitly `false`. Treat hooks without an `enabled` field as enabled by default.
- Do **not** interpret or evaluate hook `condition` expressions:
  - If the hook has no `condition` field, or it is null/empty, treat the hook as executable
  - If the hook defines a non-empty `condition`, skip the hook and leave condition evaluation to the HookExecutor implementation
- For each executable hook, output the following based on its `optional` flag:
  - **Optional hook** (`optional: true`):
    ```
    ## Extension Hooks

    **Optional Pre-Hook**: {extension}
    Command: `/{command}`
    Description: {description}

    Prompt: {prompt}
    To execute: `/{command}`
    ```
  - **Mandatory hook** (`optional: false`):
    ```
    ## Extension Hooks

    **Automatic Pre-Hook**: {extension}
    Executing: `/{command}`
    EXECUTE_COMMAND: {command}

    Wait for the result of the hook command before proceeding to the Goal.
    ```

## Goal

Identify inconsistencies, duplications, ambiguities, and underspecified items across the three core artifacts (`spec.md`, `plan.md`, `tasks.md`) before implementation. This command MUST run only after `/speckit-tasks` has successfully produced a complete `tasks.md`.

## Operating Constraints

**STRICTLY READ-ONLY**: Do **not** modify any files. Output a structured analysis report. Offer an optional remediation plan (user must explicitly approve before any follow-up editing commands would be invoked manually).

**Constitution Authority**: The project constitution (`.specify/memory/constitution.md`) is **non-negotiable** within this analysis scope. Constitution conflicts are automatically CRITICAL and require adjusting the spec, plan, or tasks — not diluting, reinterpreting, or silently ignoring the principle. Changing a principle itself must occur in a separate, explicit constitution update outside `/speckit-analyze`.

## Execution Steps

### 1. Initialize Analysis Context

Run `.specify/scripts/bash/check-prerequisites.sh --json --require-tasks --include-tasks` once from repo root and parse JSON for FEATURE_DIR and AVAILABLE_DOCS. Derive absolute paths:

- SPEC = FEATURE_DIR/spec.md
- PLAN = FEATURE_DIR/plan.md
- TASKS = FEATURE_DIR/tasks.md

Abort with an error message if any required file is missing (instruct the user to run missing prerequisite command).
For single quotes in args like "I'm Groot", use escape syntax: e.g 'I'\''m Groot' (or double-quote if possible: "I'm Groot").

### 2. Load Artifacts (Progressive Disclosure)

Load only the minimal necessary context from each artifact:

**From spec.md:**

- Overview/Context
- Functional Requirements
- Success Criteria (measurable outcomes — e.g., performance, security, availability, user success, business impact)
- User Stories
- Edge Cases (if present)

**From plan.md:**

- Architecture/stack choices
- Data Model references
- Phases
- Technical constraints

**From tasks.md:**

- Task IDs
- Descriptions
- Phase grouping
- Parallel markers [P]
- Referenced file paths

**From constitution:**

- Load `.specify/memory/constitution.md` for principle validation

### 3. Build Semantic Models

Create internal representations (do not include raw artifacts in output):

- **Requirements inventory**: For each Functional Requirement (FR-###) and Success Criterion (SC-###), record a stable key — the explicit FR-/SC- identifier as primary key when present, optionally plus an imperative-phrase slug for readability (e.g., "User can upload file" → `user-can-upload-file`). Include only Success Criteria requiring buildable work (e.g., load-testing infrastructure, security audit tooling); exclude post-launch outcome metrics and business KPIs (e.g., "Reduce support tickets by 50%").
- **User story/action inventory**: Discrete user actions with acceptance criteria
- **Task coverage mapping**: Map each task to one or more requirements or stories (inference by keyword / explicit reference patterns like IDs or key phrases)
- **Constitution rule set**: Extract principle names and MUST/SHOULD normative statements

### 4. Detection Passes (Token-Efficient Analysis)

Focus on high-signal findings. Limit to 50 findings total; aggregate remainder in overflow summary.

#### A. Duplication Detection

- Identify near-duplicate requirements
- Mark lower-quality phrasing for consolidation

#### B. Ambiguity Detection

- Flag vague adjectives (fast, scalable, secure, intuitive, robust) lacking measurable criteria
- Flag unresolved placeholders (TODO, TKTK, ???, `<placeholder>`, etc.)

#### C. Underspecification

- Requirements with verbs but missing object or measurable outcome
- User stories missing acceptance criteria alignment
- Tasks referencing files or components not defined in spec/plan

#### D. Constitution Alignment

- Any requirement or plan element conflicting with a MUST principle
- Missing mandated sections or quality gates from constitution

#### E. Coverage Gaps

- Requirements with zero associated tasks
- Tasks with no mapped requirement/story
- Success Criteria requiring buildable work (performance, security, availability) not reflected in tasks

#### F. Inconsistency

- Terminology drift (same concept named differently across files)
- Data entities referenced in plan but absent in spec (or vice versa)
- Task ordering contradictions (e.g., integration tasks before foundational setup tasks without dependency note)
- Conflicting requirements (e.g., one requires Next.js while other specifies Vue)

#### G. Cross-Artifact Symbol Consistency (mechanical, FR-008)

Run the **compiled, deterministic** symbol set-difference as a real command — do
**not** eyeball it and do **not** hand-derive the set-difference:

```bash
./fake.sh build -t SymbolCrossCheck
```

**Probe target availability first; skip-with-documented-notice when it is absent.**
`SymbolCrossCheck` is a framework-repo-only governance target — **generated projects
do not ship it**, so the command fails there with `Unknown … target:
SymbolCrossCheck`. Before invoking, probe whether the target resolves (e.g.
`./fake.sh build --list` / a target-resolution check), mirroring how the
evidence-graph step resolves the active feature from `.specify/feature.json` and
fails loudly only when nothing resolves. When `SymbolCrossCheck` resolves, run it and
fold its output in. When it is **absent**, do **not** fail the analysis: record a
documented skip notice in this report — `Symbol consistency (analyze pass G): skipped
— SymbolCrossCheck target not available in this project; manual cross-check is
non-authoritative` — and continue. The skip is a visible decision, not a silent
omission, and never blocks the invocation.

The target takes **no file arguments**: it resolves the active feature directory
(the `DependencyReport` pattern), reads `plan.md`, `data-model.md`, and `tasks.md`
from it, runs the `SymbolCrossCheck` analyzer in `FS.Skia.UI.Build`, **prints** the
markdown, and **writes** it to `readiness/symbol-cross-check.md`. Consume that output
and fold its findings into this report. The analyzer extracts named symbols by kind —
`Msg` cases, union/`Screen` variants, entity record names (backtick-quoted PascalCase
tokens on lines naming the kind), and `FR-`/`SC-` IDs — and reports each symbol whose
presence set is a **proper subset** of the three artifacts (present in some, missing
from others). It renders as:

```
## Symbol consistency (analyze pass G)
- msg-case ViewerKeyEventReceived — in {data-model, tasks}, missing from {plan}
- sc-id SC-009 — in {plan}, missing from {data-model, tasks}
```

Treatment: report set-differences as findings at the appropriate severity for
human judgment. A symbol present **only in design** (e.g. a start-state in
`data-model.md` with no matching spec FR) is flagged `[design-only? human
judgment]` and is **never hard-failed** — design-ahead-of-spec is a legitimate
edge case (the cross-check is guidance, not a gate).

### 5. Severity Assignment

Use this heuristic to prioritize findings:

- **CRITICAL**: Violates constitution MUST, missing core spec artifact, or requirement with zero coverage that blocks baseline functionality
- **HIGH**: Duplicate or conflicting requirement, ambiguous security/performance attribute, untestable acceptance criterion
- **MEDIUM**: Terminology drift, missing non-functional task coverage, underspecified edge case
- **LOW**: Style/wording improvements, minor redundancy not affecting execution order

### 6. Produce Compact Analysis Report

Output a Markdown report (no file writes) with the following structure:

## Specification Analysis Report

| ID | Category | Severity | Location(s) | Summary | Recommendation |
|----|----------|----------|-------------|---------|----------------|
| A1 | Duplication | HIGH | spec.md:L120-134 | Two similar requirements ... | Merge phrasing; keep clearer version |

(Add one row per finding; generate stable IDs prefixed by category initial.)

**Coverage Summary Table:**

| Requirement Key | Has Task? | Task IDs | Notes |
|-----------------|-----------|----------|-------|

**Constitution Alignment Issues:** (if any)

**Unmapped Tasks:** (if any)

**Metrics:**

- Total Requirements
- Total Tasks
- Coverage % (requirements with >=1 task)
- Ambiguity Count
- Duplication Count
- Critical Issues Count

### 7. Provide Next Actions

At end of report, output a concise Next Actions block:

- If CRITICAL issues exist: Recommend resolving before `/speckit-implement`
- If only LOW/MEDIUM: User may proceed, but provide improvement suggestions
- Provide explicit command suggestions: e.g., "Run /speckit-specify with refinement", "Run /speckit-plan to adjust architecture", "Manually edit tasks.md to add coverage for 'performance-metrics'"

### 8. Offer Remediation

Ask the user: "Would you like me to suggest concrete remediation edits for the top N issues?" (Do NOT apply them automatically.)

### 9. Check for extension hooks

After reporting, discover hooks across **all** extension files (multi-file discovery), not just the central file:
- Read `.specify/extensions.yml` from the project root (if present) and collect entries under the `hooks.after_analyze` key.
- Then enumerate every `.specify/extensions/*/*.yml` file in sorted order, parse each, and collect its `hooks.after_analyze` entries too — so a hook registered only in a per-extension file (e.g. the `feedback` extension at `.specify/extensions/feedback/feedback.yml`) is still discovered and runs on phase completion.
- Merge all collected entries and dedupe by `(extension, command)` (first occurrence wins, so a hook declared in both files runs once).
- **Hook execution precedence** (D1): when `settings.auto_execute_hooks: true` in `.specify/extensions.yml`, a **mandatory** hook (`optional: false`) **auto-runs** with no confirmation; an **optional** hook (`optional: true`) is **always surfaced** ("To execute: `/{command}`") and is **never force-run** by `auto_execute_hooks`; a hook with a non-empty `condition` is **never** evaluated by this skill — evaluation is left to the executor and the notice reports the resolved decision. When `auto_execute_hooks: false`, even mandatory hooks are surfaced for confirmation.
- **Effective-hooks notice** (D2): after the merge + dedup by `(extension, command)`, emit **one** consolidated notice for the phase so the operator never hand-reconciles files — the promoted feedback hook (`optional: false`) appears as `auto-run`, never as a surfaced optional:
  ```
  ## Effective hooks for analyze
  - {extension}:{command} — auto-run        (mandatory; auto_execute_hooks=true)
  - {extension}:{command} — surfaced        (optional)
  - {extension}:{command} — skipped         (enabled: false)
  - {extension}:{command} — condition-deferred
  ```
- If a file is absent, no hooks are registered, or its YAML cannot be parsed/is invalid, skip that file silently and continue.
- For every `optional: true` hook that is discovered but not executed this phase, emit one line so the skip is a visible decision: `Note: optional hook {extension}:{command} is registered but was not run (skipped).`
- Filter out hooks where `enabled` is explicitly `false`. Treat hooks without an `enabled` field as enabled by default.
- Do **not** interpret or evaluate hook `condition` expressions:
  - If the hook has no `condition` field, or it is null/empty, treat the hook as executable
  - If the hook defines a non-empty `condition`, skip the hook and leave condition evaluation to the HookExecutor implementation
- For each executable hook, output the following based on its `optional` flag:
  - **Optional hook** (`optional: true`):
    ```
    ## Extension Hooks

    **Optional Hook**: {extension}
    Command: `/{command}`
    Description: {description}

    Prompt: {prompt}
    To execute: `/{command}`
    ```
  - **Mandatory hook** (`optional: false`):
    ```
    ## Extension Hooks

    **Automatic Hook**: {extension}
    Executing: `/{command}`
    EXECUTE_COMMAND: {command}
    ```

## Operating Principles

### Context Efficiency

- **Minimal high-signal tokens**: Focus on actionable findings, not exhaustive documentation
- **Progressive disclosure**: Load artifacts incrementally; don't dump all content into analysis
- **Token-efficient output**: Limit findings table to 50 rows; summarize overflow
- **Deterministic results**: Rerunning without changes should produce consistent IDs and counts

### Analysis Guidelines

- **NEVER modify files** (this is read-only analysis)
- **NEVER hallucinate missing sections** (if absent, report them accurately)
- **Prioritize constitution violations** (these are always CRITICAL)
- **Use examples over exhaustive rules** (cite specific instances, not generic patterns)
- **Report zero issues gracefully** (emit success report with coverage statistics)

## Context

$ARGUMENTS
