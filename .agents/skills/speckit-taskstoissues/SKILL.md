---
name: "speckit-taskstoissues"
description: "Convert existing tasks into actionable, dependency-ordered GitHub issues for the feature based on available design artifacts."
compatibility: "Requires spec-kit project structure with .specify/ directory"
metadata:
  author: "github-spec-kit"
  source: "templates/commands/taskstoissues.md"
---


## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Pre-Execution Checks

**Check for extension hooks (before taskstoissues)**:
- Discover hooks across **all** extension files (multi-file discovery), not just the central file:
  - Read `.specify/extensions.yml` from the project root (if present) and collect entries under the `hooks.before_taskstoissues` key.
  - Then enumerate every `.specify/extensions/*/*.yml` file in sorted order, parse each, and collect its `hooks.before_taskstoissues` entries too — so a hook registered only in a per-extension file (e.g. the `feedback` extension at `.specify/extensions/feedback/feedback.yml`) is still discovered and runs.
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

1. Run `.specify/scripts/bash/check-prerequisites.sh --json --require-tasks --include-tasks` from repo root and parse FEATURE_DIR and AVAILABLE_DOCS list. All paths must be absolute. For single quotes in args like "I'm Groot", use escape syntax: e.g 'I'\''m Groot' (or double-quote if possible: "I'm Groot").
1. From the executed script, extract the path to **tasks**.
1. Get the Git remote by running:

```bash
git config --get remote.origin.url
```

> [!CAUTION]
> ONLY PROCEED TO NEXT STEPS IF THE REMOTE IS A GITHUB URL

1. For each task in the list, use the GitHub MCP server to create a new issue in the repository that is representative of the Git remote.

> [!CAUTION]
> UNDER NO CIRCUMSTANCES EVER CREATE ISSUES IN REPOSITORIES THAT DO NOT MATCH THE REMOTE URL

## Post-Execution Checks

**Check for extension hooks (after taskstoissues)**: After the issues are created, discover hooks across **all** extension files (multi-file discovery), not just the central file:
- Read `.specify/extensions.yml` from the project root (if present) and collect entries under the `hooks.after_taskstoissues` key.
- Then enumerate every `.specify/extensions/*/*.yml` file in sorted order, parse each, and collect its `hooks.after_taskstoissues` entries too — so a hook registered only in a per-extension file (e.g. the `feedback` extension at `.specify/extensions/feedback/feedback.yml`) is still discovered and runs on phase completion.
- Merge all collected entries and dedupe by `(extension, command)` (first occurrence wins, so a hook declared in both files runs once).
- **Hook execution precedence** (D1): when `settings.auto_execute_hooks: true` in `.specify/extensions.yml`, a **mandatory** hook (`optional: false`) **auto-runs** with no confirmation; an **optional** hook (`optional: true`) is **always surfaced** ("To execute: `/{command}`") and is **never force-run** by `auto_execute_hooks`; a hook with a non-empty `condition` is **never** evaluated by this skill — evaluation is left to the executor and the notice reports the resolved decision. When `auto_execute_hooks: false`, even mandatory hooks are surfaced for confirmation.
- **Effective-hooks notice** (D2): after the merge + dedup by `(extension, command)`, emit **one** consolidated notice for the phase so the operator never hand-reconciles files — a promoted feedback hook (`optional: false`) appears as `auto-run`, never as a surfaced optional:
  ```
  ## Effective hooks for taskstoissues
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
