---
name: speckit-constitution
description: Create or update project governing principles and development guidelines.
compatibility: Requires spec-kit project structure with .specify/ directory
metadata:
  author: github-spec-kit
  source: preset:fsharp-opinionated
---

# Speckit Constitution Skill

# /speckit.constitution

## Pre-Execution Checks

**Check for extension hooks (before constitution)**:
- Discover hooks across **all** extension files (multi-file discovery), not just the central file:
  - Read `.specify/extensions.yml` from the project root (if present) and collect entries under the `hooks.before_constitution` key.
  - Then enumerate every `.specify/extensions/*/*.yml` file in sorted order, parse each, and collect its `hooks.before_constitution` entries too — so a hook registered only in a per-extension file (e.g. the `feedback` extension at `.specify/extensions/feedback/feedback.yml`) is still discovered and runs.
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

    Wait for the result of the hook command before proceeding to constitution setup.
    ```
  The registered `before_constitution` `git.initialize` hook (`optional: false`) is **mandatory** — under `auto_execute_hooks: true` it auto-runs and you wait for the repository to be initialized before populating the constitution.
- If no hooks are registered or `.specify/extensions.yml` does not exist, skip silently

Populate `.specify/memory/constitution.md` from the preset's
`constitution-template.md`. The template contains three kinds of sections,
marked by HTML comments:

- `<!-- REQUIRED -->` — placeholders MUST be filled. Ask the user for each.
- `<!-- TAILORABLE -->` — tune per project. Ask the user what's appropriate
  for this project's domain and stack. You MAY add or reword bullets within
  these sections.
- `<!-- LOCKED -->` — do NOT modify these sections. The seven Core Principles,
  Change Classification, Workflow & Quality Gates, and Governance sections
  are shared doctrine across every project using this preset. If the user
  explicitly asks to modify a locked section, pause and confirm: *"This
  section is marked LOCKED because it's shared across every project using
  the fsharp-opinionated preset. Are you sure you want a per-project
  amendment, or should this become an upstream preset change?"*

## Placeholders to fill

The REQUIRED section uses these placeholders. Ask for each:

- `[PROJECT_NAME]` — the project's display name (often matches the repo name).
- `[CONSTITUTION_VERSION]` — start at `1.0.0` for a new project.
- `[RATIFICATION_DATE]` — today's date in `YYYY-MM-DD`.
- `[LAST_AMENDED_DATE]` — same as ratification on first write.

The TAILORABLE section uses these placeholders. Offer reasonable defaults
and let the user accept or override:

- `[PACK_OUTPUT_PATH]` — default `~/.local/share/nuget-local/`.
- `[LOGGING_LIBRARY]` — default "not yet selected; see ADR when chosen."
  Common F# choices: Serilog, Microsoft.Extensions.Logging, Logary.
- `[PROJECT_CONSTRAINTS]` — any project-specific rules (runtime target,
  deployment target, supported OS). If none, write "None beyond the
  defaults above."

## Output

- Write the filled constitution to `.specify/memory/constitution.md`.
- Remove the HTML comment markers (they're authoring aids, not reader content).
- Do NOT remove the LOCKED-section text itself — only the `<!-- LOCKED -->`
  marker lines go.
- Produce a short summary naming (a) what placeholders were filled, (b) what
  TAILORABLE choices were made, and (c) confirming no LOCKED sections changed.

## If the constitution file already exists

Do not overwrite silently. Diff the existing file against the current
template and offer three options:

1. Update only REQUIRED/TAILORABLE placeholders in place (preserve any
   existing tailoring).
2. Refresh from the current template, preserving the existing project's
   TAILORABLE choices (warn if LOCKED sections have diverged — that's a
   sign the preset was amended upstream).
3. Abort and let the user edit manually.

## Post-Execution Checks

**Check for extension hooks (after constitution)**: After the constitution is populated, discover hooks across **all** extension files (multi-file discovery), not just the central file:
- Read `.specify/extensions.yml` from the project root (if present) and collect entries under the `hooks.after_constitution` key.
- Then enumerate every `.specify/extensions/*/*.yml` file in sorted order, parse each, and collect its `hooks.after_constitution` entries too — so a hook registered only in a per-extension file (e.g. the `feedback` extension at `.specify/extensions/feedback/feedback.yml`) is still discovered and runs on phase completion.
- Merge all collected entries and dedupe by `(extension, command)` (first occurrence wins, so a hook declared in both files runs once).
- **Hook execution precedence** (D1): when `settings.auto_execute_hooks: true` in `.specify/extensions.yml`, a **mandatory** hook (`optional: false`) **auto-runs** with no confirmation; an **optional** hook (`optional: true`) is **always surfaced** ("To execute: `/{command}`") and is **never force-run** by `auto_execute_hooks`; a hook with a non-empty `condition` is **never** evaluated by this skill — evaluation is left to the executor and the notice reports the resolved decision. When `auto_execute_hooks: false`, even mandatory hooks are surfaced for confirmation.
- **Effective-hooks notice** (D2): after the merge + dedup by `(extension, command)`, emit **one** consolidated notice for the phase so the operator never hand-reconciles files — a promoted feedback hook (`optional: false`) appears as `auto-run`, never as a surfaced optional:
  ```
  ## Effective hooks for constitution
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
