---
name: speckit-implement
description: Execute all tasks from the task breakdown to build the feature.
compatibility: Requires spec-kit project structure with .specify/ directory
metadata:
  author: github-spec-kit
  source: preset:fsharp-opinionated
---

# Speckit Implement Skill

# /speckit.implement

Execute the feature's tasks against the plan. Update `tasks.md` as you go.

## Pre-Execution Checks

**Check for extension hooks (before implement)**:
- Discover hooks across **all** extension files (multi-file discovery), not just the central file:
  - Read `.specify/extensions.yml` from the project root (if present) and collect entries under the `hooks.before_implement` key.
  - Then enumerate every `.specify/extensions/*/*.yml` file in sorted order, parse each, and collect its `hooks.before_implement` entries too — so a hook registered only in a per-extension file (e.g. the `feedback` extension at `.specify/extensions/feedback/feedback.yml`) is still discovered and runs.
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

    Wait for the result of the hook command before proceeding to the implementation workflow.
    ```
- If no hooks are registered or `.specify/extensions.yml` does not exist, skip silently

## Status marking discipline

Use the status legend from the template exactly:

- `[ ]` — not started.
- `[X]` — **done with real evidence.** The production code paths were
  exercised against real dependencies (real DB, filesystem, network) or a
  previously-approved synthetic fixture already listed as acceptable in the
  Synthetic-Evidence Inventory.
- `[S]` — **done with synthetic evidence only.** Use this whenever the
  task's "pass" depends on ANY of:
  - a mock, stub, fake, or in-memory substitute
  - a `NotImplementedException`, `failwith "TODO"`, `raise ()`, or
    equivalent placeholder
  - hardcoded literals where production code will need a real data source
  - a test that exercises only synthetic fixtures
  - a dependency on another `[S]` task whose synthetic nature propagates
    (you do not need to detect this manually — the evidence audit
    computes `[S*]` propagation — but BE HONEST about the direct cases).
- `[F]` — **failed.** Implementation attempted and did not pass. Leave the
  diagnostics in place; do not quietly retry.
- `[-]` — **skipped.** Requires written rationale either in the task line
  or in the Synthetic-Evidence Inventory / Deferral Notes section.

**Never mark a task `[X]` if any of the `[S]` conditions apply.** The
evidence audit catches many such cases via diff-scan, but be honest about
direct declarations — dishonesty undermines the whole synthetic-evidence
regime (Principle V).

## Vertical-slice rule (US phases)

A `[US*]` task may only be marked `[X]` when the user-facing surface was
actually exercised end-to-end. "Exercised" means one of:

- An FSI transcript captured under `readiness/` that drives the new
  behavior through its public entry point — not through internal helpers.
- A smoke run of the host application (CLI invocation, GUI launch, HTTP
  request) that touches the new code path, with the artifact (log,
  screenshot, response body) saved under `readiness/`.
- A semantic test that loads the **packed** library or runs the host
  binary and exercises the user-reachable path — not a unit test against
  domain modules in isolation.

A diff that touches only `Domain/`, `Core/`, `Models/`, or equivalent
internal layers is **never** sufficient evidence for `[X]` on a `[US*]`
task. The story is done when the user can reach it, not when the model
compiles. If wire-up to the UI / CLI / API surface is missing, the honest
status is `[ ]` (continue working) or `[S]` (disclose the gap and create a
tracking issue for the real wire-up).

This rule is in addition to the synthetic-evidence checks above: a task
can fail the vertical-slice rule without any mocks — domain code that
nothing calls is its own failure mode.

## Elmish/MVU discipline (Principle IV)

For any task whose spec, plan, or task line identifies stateful workflow or
I/O, implement through the Elmish/MVU boundary:

- `Model` captures owned workflow state.
- `Msg` captures user actions, external responses, and internal transitions.
- `Effect` or `Cmd<Msg>` captures requested I/O.
- `init` returns initial state plus startup effects.
- `update` is pure: it may inspect `Msg` and `Model`, but it MUST NOT touch
  filesystem, network, database, process state, wall clock, random source, or
  mutable global state.
- An interpreter at the edge executes effects and maps results back to `Msg`.

Before marking an MVU-bearing `[US*]` task `[X]`, verify all of the following:

- FSI or packed-library tests exercise public `init` / `update` paths.
- Tests assert both next `Model` and emitted effects for representative
  messages.
- The interpreter path has real evidence where safe (real filesystem,
  process, network, database, or host entry point). If it uses a fake,
  in-memory substitute, canned response, or unconnected interpreter, mark
  the task `[S]` and disclose it.
- The user-facing entry point is wired through the interpreter boundary, not
  around it.

Simple pure functions do not need an MVU shell. If a task does not involve
stateful workflow or I/O, note that Principle IV is not applicable and use the
ordinary spec → FSI → semantic tests → implementation path.

## Synthetic-evidence disclosures (Principle V)

`[SEH]` is a **design/task-generation** classification, never an
**implementation-time relabeling** step. The `EvidenceAudit` gate rejects late
`[SEH]` classification, so do not add `[SEH]` or
`synthetic-error-handling-approved` during readiness cleanup or after an audit
failure — send any newly-discovered synthetic error-handling need back to the
design phase. `[SEH]` tasks still complete as `[S]`, and the inventory row keeps
the approval label, design source, synthetic input class, expected error
behavior, and `accepted-seh` status.

When you emit an `[S]` task, you MUST also:

1. **Code-level disclosure.** Add a `// SYNTHETIC:` comment at the use
   site, naming the reason and (if known) the real-evidence path. Example:
   ```fsharp
   let userRepo = InMemoryUserRepo()  // SYNTHETIC: staging DB not provisioned; real repo in US-17
   ```
2. **Test-level disclosure.** Test names exercising the synthetic surface
   contain the token `Synthetic`. Example:
   ```fsharp
   [<Test>] let ``Signup.createUser_Synthetic_persists in-memory`` () = ...
   ```
   For whole test files that are synthetic-only, open with a banner:
   ```fsharp
   (* SYNTHETIC FIXTURE: all tests in this file use canned SMTP responses. *)
   ```
3. **Inventory update.** Add a row to the Synthetic-Evidence Inventory
   table in `tasks.md`:
   - Task id
   - Reason
   - Real-evidence path (or "infeasible, see spec §X")
   - Tracking issue (create one if the real-evidence path is a future
     feature)

## Workflow, per task

**Before you author any readiness/evidence file**, read the generated
`docs/evidence-formats.md` (it ships into every generated project). It is the
single source for the exact shape each readiness/evidence artifact must take —
read it first so you author against the contract instead of reverse-engineering
the format from a gate failure.

1. Mark the task `in_progress` in your own head (not in `tasks.md` — the
   file uses the five-state legend only).
2. Read the task's deps and structured `skillist` from `tasks.deps.yml`.
   Existing bare-list task metadata is invalid; stop and require migration or
   regeneration before implementation.
3. Resolve every declared skill id to exactly one readable `SKILL.md` from
   `.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`, template capability
   skill paths, or active generated product skill destinations. Load those
   skills in declared order before code changes for the task begin. Record
   the loaded paths in `skill-loading-evidence.md` — read and written from the
   **feature** readiness dir `specs/<feature>/readiness/skill-loading-evidence.md`
   (NOT a repo-root `readiness/`) — or in a task-specific verification log before
   marking the task complete. Write **one row per (task, declared-skill)** pair;
   each record must include task id, skill id, resolved path (the
   `.agents/skills/<id>/SKILL.md` or `src/*/skill/SKILL.md` home), load result,
   `loaded_at`, `work_started_at` (with `loaded_at` strictly **before**
   `work_started_at`), evidence path, and reviewer exception fields. The
   skill-loading contract is **enforced only once a task flips to `[X]`**, so it
   surfaces late — author the rows as you load skills, not retroactively at audit
   time. If a declared skill is missing, unreadable,
   ambiguous, late-loaded, or missing evidence, block the task and report the task
   id plus unresolved skill id.
   implementation batch records must preserve the red-green evidence log,
   graph before/after paths before and after every status change, and the
   skill-loading evidence used for the batch.
4. Confirm all deps are `[X]` or `[S]`. If any dep is `[ ]`, `[F]`, or
   `[-]`, stop and raise it.
5. Implement the task per the plan, applying the loaded capability skill
   guidance before generic guidance whenever both match.
6. Run the verification appropriate for the phase (tests, baseline check,
   FSI exercise, …). `[US*]` tasks MUST include a user-reachable exercise
   (see the Vertical-slice rule); MVU-bearing tasks MUST include
   transition/effect assertions and interpreter evidence. A green unit test
   on the domain layer is not enough.
7. Update the status in `tasks.md`. Before writing `[X]` on a `[US*]`
   task, confirm the vertical-slice rule is satisfied; if not, the
   honest status is `[ ]` or `[S]`. If `[S]`, add the code-level,
   test-level, and inventory disclosures before moving on.
8. **Re-run `speckit.evidence.graph`** after every status change. This
   refreshes `readiness/task-graph.json` and recomputes `[S*]`
   propagation. It's cheap (milliseconds).
9. Move to the next task.

## Visibility discipline (Principle II)

- Never write `private`, `internal`, or `public` on a top-level F#
  binding. Visibility lives in the `.fsi` signature file.
- If a task needs to change the public surface, the `.fsi` update is part
  of the same task — not a follow-up.

## Simplicity discipline (Principle III)

Before reaching for a "clever" F# feature (custom operators, SRTP,
reflection, non-trivial computation expressions, type providers, non-
obvious active patterns), confirm it is justified in the spec or plan.
If not, either simplify or stop and raise the justification gap.

## Stop conditions

Stop and ask the user when:

- A task's spec guidance conflicts with its code. The spec wins (Principle
  III: complex features require justification).
- A test fails in a way that would require weakening an assertion or
  adding `[<Skip>]` to pass. Never weaken; surface the failure.
- A dependency in `tasks.deps.yml` points to a task that doesn't exist.
  Fix the yml before proceeding.

## Sequential FAKE Commands

FAKE-backed commands (`./fake.sh`, `fake.cmd`, or `dotnet fake`) share
repository `.fake` state and are not safe to run concurrently. You may
parallelize safe non-FAKE reads and checks, but run multiple FAKE-backed tests
or targets sequentially:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

If a FAKE-backed failure looks race-like or concurrent context is unknown,
rerun the affected FAKE-backed commands sequentially before product debugging.

## Post-Execution Checks

**Check for extension hooks (after implement)**: After implementation completes, discover hooks across **all** extension files (multi-file discovery), not just the central file:
- Read `.specify/extensions.yml` from the project root (if present) and collect entries under the `hooks.after_implement` key.
- Then enumerate every `.specify/extensions/*/*.yml` file in sorted order, parse each, and collect its `hooks.after_implement` entries too — so a hook registered only in a per-extension file (e.g. the `feedback` extension at `.specify/extensions/feedback/feedback.yml`) is still discovered and runs on phase completion.
- Merge all collected entries and dedupe by `(extension, command)` (first occurrence wins, so a hook declared in both files runs once).
- **Hook execution precedence** (D1): when `settings.auto_execute_hooks: true` in `.specify/extensions.yml`, a **mandatory** hook (`optional: false`) **auto-runs** with no confirmation; an **optional** hook (`optional: true`) is **always surfaced** ("To execute: `/{command}`") and is **never force-run** by `auto_execute_hooks`; a hook with a non-empty `condition` is **never** evaluated by this skill — evaluation is left to the executor and the notice reports the resolved decision. When `auto_execute_hooks: false`, even mandatory hooks are surfaced for confirmation.
- **Effective-hooks notice** (D2): after the merge + dedup by `(extension, command)`, emit **one** consolidated notice for the phase so the operator never hand-reconciles files — a promoted feedback hook (`optional: false`) appears as `auto-run`, never as a surfaced optional:
  ```
  ## Effective hooks for implement
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
