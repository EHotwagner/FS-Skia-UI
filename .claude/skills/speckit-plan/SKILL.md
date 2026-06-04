---
name: "speckit-plan"
description: "Execute the implementation planning workflow using the plan template to generate design artifacts."
compatibility: "Requires spec-kit project structure with .specify/ directory"
metadata:
  author: "github-spec-kit"
  source: "templates/commands/plan.md"
---


## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Pre-Execution Checks

**Check for extension hooks (before planning)**:
- Discover hooks across **all** extension files (multi-file discovery), not just the central file:
  - Read `.specify/extensions.yml` from the project root (if present) and collect entries under the `hooks.before_plan` key.
  - Then enumerate every `.specify/extensions/*/*.yml` file in sorted order, parse each, and collect its `hooks.before_plan` entries too — so a hook registered only in a per-extension file (e.g. the `feedback` extension at `.specify/extensions/feedback/feedback.yml`) is still discovered and runs.
  - Merge all collected entries and dedupe by `(extension, command)` (first occurrence wins, so a hook declared in both files runs once).
  - If a file is absent or its YAML cannot be parsed/is invalid, skip that file silently and continue.
- For every `optional: true` hook that is discovered but not executed this phase, emit one line so the skip is a visible decision: `Note: optional hook {extension}:{command} is registered but was not run (skipped).`
- Filter out hooks where `enabled` is explicitly `false`. Treat hooks without an `enabled` field as enabled.
- For each remaining hook, do **not** interpret or evaluate hook `condition` expressions:
  - If the hook has no `condition` field, or it is null/empty, treat the hook as executable
  - If the hook defines a non-empty `condition`, skip it and leave condition evaluation to the HookExecutor implementation
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

    Wait for the result of the hook command before proceeding to the Outline.
    ```
- If no hooks are registered or `.specify/extensions.yml` does not exist, skip silently

## Outline

1. **Setup**: Run `.specify/scripts/bash/setup-plan.sh --json` from repo root and parse JSON for FEATURE_SPEC, IMPL_PLAN, SPECS_DIR, BRANCH. For single quotes in args like "I'm Groot", use escape syntax: e.g 'I'\''m Groot' (or double-quote if possible: "I'm Groot").

2. **Load context**: Read FEATURE_SPEC and `.specify/memory/constitution.md`. Load IMPL_PLAN template (already copied). **When planning a generated FS Skia UI product, read `docs/scaffold-map.md` first** (it ships into every generated project) — it is the authoritative durable-vs-replaceable map of the scaffold (which files you own and design freely vs. which are framework-owned). Read it rather than reconstructing that map by hand from the file tree, and let it inform your game-model design (e.g. record-label collisions with Scene's `X`/`Y`/`Width`/`Height`).

3. **Execute plan workflow**: Follow the structure in IMPL_PLAN template to:
   - Fill Technical Context (mark unknowns as "NEEDS CLARIFICATION")
   - Fill Constitution Check section from constitution
   - Evaluate gates (ERROR if violations unjustified)
   - Phase 0: Generate research.md (resolve all NEEDS CLARIFICATION)
   - Phase 1: Generate data-model.md, contracts/, quickstart.md
   - Phase 1: Update agent context by running the agent script
   - Re-evaluate Constitution Check post-design

4. **Stop and report**: Command ends after Phase 2 planning. Report branch, IMPL_PLAN path, and generated artifacts.

5. **Check for extension hooks**: After reporting, discover hooks across **all** extension files (multi-file discovery), not just the central file:
   - Read `.specify/extensions.yml` from the project root (if present) and collect entries under the `hooks.after_plan` key.
   - Then enumerate every `.specify/extensions/*/*.yml` file in sorted order, parse each, and collect its `hooks.after_plan` entries too — so a hook registered only in a per-extension file (e.g. the `feedback` extension at `.specify/extensions/feedback/feedback.yml`) is still discovered and runs on phase completion.
   - Merge all collected entries and dedupe by `(extension, command)` (first occurrence wins, so a hook declared in both files runs once).
   - **Hook execution precedence** (D1): when `settings.auto_execute_hooks: true` in `.specify/extensions.yml`, a **mandatory** hook (`optional: false`) **auto-runs** with no confirmation; an **optional** hook (`optional: true`) is **always surfaced** ("To execute: `/{command}`") and is **never force-run** by `auto_execute_hooks`; a hook with a non-empty `condition` is **never** evaluated by this skill — evaluation is left to the executor and the notice reports the resolved decision. When `auto_execute_hooks: false`, even mandatory hooks are surfaced for confirmation.
   - **Effective-hooks notice** (D2): after the merge + dedup by `(extension, command)`, emit **one** consolidated notice for the phase so the operator never hand-reconciles files — the promoted feedback hook (`optional: false`) appears as `auto-run`, never as a surfaced optional:
     ```
     ## Effective hooks for plan
     - {extension}:{command} — auto-run        (mandatory; auto_execute_hooks=true)
     - {extension}:{command} — surfaced        (optional)
     - {extension}:{command} — skipped         (enabled: false)
     - {extension}:{command} — condition-deferred
     ```
   - If a file is absent or its YAML cannot be parsed/is invalid, skip that file silently and continue.
   - For every `optional: true` hook that is discovered but not executed this phase, emit one line so the skip is a visible decision: `Note: optional hook {extension}:{command} is registered but was not run (skipped).`
   - Filter out hooks where `enabled` is explicitly `false`. Treat hooks without an `enabled` field as enabled.
   - For each remaining hook, do **not** interpret or evaluate hook `condition` expressions:
     - If the hook has no `condition` field, or it is null/empty, treat the hook as executable
     - If the hook defines a non-empty `condition`, skip it and leave condition evaluation to the HookExecutor implementation
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
   - If no hooks are registered or `.specify/extensions.yml` does not exist, skip silently

## Phases

### Phase 0: Outline & Research

1. **Extract unknowns from Technical Context** above:
   - For each NEEDS CLARIFICATION → research task
   - For each dependency → best practices task
   - For each integration → patterns task

2. **Generate and dispatch research agents**:

   ```text
   For each unknown in Technical Context:
     Task: "Research {unknown} for {feature context}"
   For each technology choice:
     Task: "Find best practices for {tech} in {domain}"
   ```

3. **Consolidate findings** in `research.md` using format:
   - Decision: [what was chosen]
   - Rationale: [why chosen]
   - Alternatives considered: [what else evaluated]

**Output**: research.md with all NEEDS CLARIFICATION resolved

### Phase 1: Design & Contracts

**Prerequisites:** `research.md` complete

1. **Extract entities from feature spec** → `data-model.md`:
   - Entity name, fields, relationships
   - Validation rules from requirements
   - State transitions if applicable

2. **Define interface contracts** (if project has external interfaces) → `/contracts/`:
   - Identify what interfaces the project exposes to users or other systems
   - Document the contract format appropriate for the project type — e.g. public APIs for libraries, command schemas for CLI tools, endpoints for web services, grammars for parsers, UI contracts for applications
   - Skip if project is purely internal (build scripts, one-off tools, etc.)

3. **Agent context update**:
   - Update the plan reference between the `<!-- SPECKIT START -->` and `<!-- SPECKIT END -->` markers in `AGENTS.md` to point to the plan file created in step 1 (the IMPL_PLAN path)

**Output**: data-model.md, /contracts/*, quickstart.md, updated agent context file

## Key rules

- Use absolute paths for filesystem operations; use project-relative paths for references in documentation and agent context files
- ERROR on gate failures or unresolved clarifications
- Constitution Check completeness is **machine-enforced**: the `GeneratedGuidanceCheck` gate fails the build if any required *Repository Governance Decisions* area is empty, still boilerplate, or carries a `NEEDS CLARIFICATION`/`TODO` placeholder (N/A-with-rationale counts as filled)
