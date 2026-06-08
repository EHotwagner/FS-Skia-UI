# Contract: the canonical modern hook-discovery block

Each in-scope phase skill (`implement`, `tasks`, `taskstoissues`, `constitution`,
and — already compliant — `specify`, `plan`, `clarify`, `analyze`, `checklist`)
must carry **two** discovery blocks plus a consolidated notice. The authoritative
template is the existing `speckit-plan` SKILL.md; the text below is the shape the
guard's strict markers verify. `<phase>` = the skill's phase; `<Anchor>` = the
skill's first post-hook section (e.g. "Outline" for plan, "Workflow" for
implement, the task-generation step for tasks).

## Pre-hook block (`before_<phase>`)

> **Check for extension hooks (before `<phase>`)**:
> - Read `.specify/extensions.yml` from the project root (if present) and collect
>   entries under the `hooks.before_<phase>` key.
> - Then enumerate every `.specify/extensions/*/*.yml` file in sorted order, parse
>   each, and collect its `hooks.before_<phase>` entries too — so a hook
>   registered only in a per-extension file (e.g. the `feedback` extension at
>   `.specify/extensions/feedback/feedback.yml`) is still discovered and runs.
> - Merge all collected entries and dedupe by `(extension, command)` (first
>   occurrence wins).
> - For every `optional: true` hook discovered but not executed this phase, emit
>   `Note: optional hook {extension}:{command} is registered but was not run (skipped).`
> - Filter out `enabled: false` hooks (absent `enabled` ⇒ enabled). Do **not**
>   evaluate `condition` expressions (non-empty `condition` ⇒ leave to executor).
> - Optional hook ⇒ surface (`## Extension Hooks` / "To execute: `/{command}`");
>   mandatory hook ⇒ `EXECUTE_COMMAND` and wait for the result before `<Anchor>`.
> - If a file is absent/invalid, skip silently. If no hooks registered, skip
>   silently.

## Post-hook block (`after_<phase>`)

Same multi-file discovery (central + every `.specify/extensions/*/*.yml`, sorted,
parse-tolerant, deduped by `(extension, command)`), the D1 hook-execution
precedence (mandatory auto-runs under `auto_execute_hooks: true`; optional always
surfaced never force-run; non-empty `condition` deferred; `enabled:false`
skipped), **plus** the consolidated notice:

> ## Effective hooks for <phase>
> - {extension}:{command} — auto-run        (mandatory; auto_execute_hooks=true)
> - {extension}:{command} — surfaced        (optional)
> - {extension}:{command} — skipped         (enabled: false)
> - {extension}:{command} — condition-deferred

## Strict markers the guard keys on (must all be present)

| # | Literal substring | Where it comes from |
| --- | --- | --- |
| 1 | `.specify/extensions/*/*.yml` (≥ 2 occurrences) | both blocks' multi-file enumeration |
| 2 | `(extension, command)` | dedupe language in both blocks |
| 3 | `## Effective hooks for <phase>` | the post-hook consolidated notice |

## No-op / behavior-preservation guarantee (FR-005, FR-009, edge cases)

- **No matching hook registered** ⇒ discovery is a silent no-op; phase completes
  normally. This repo currently registers only `git`/`evidence` hooks (no
  feedback), so the new blocks must not error or prompt here.
- **Aborted/failed phase** ⇒ `after_*` fires only on completion; no record.
- **Optional vs mandatory** ⇒ optional surfaced, never force-run; mandatory
  auto-run under `auto_execute_hooks: true`.
- **Disabled / condition-bearing** ⇒ skipped-with-note / condition-deferred.
- **Multi-file-only hook** ⇒ a hook present only in a per-extension file is
  discovered, deduped, honored.

The four edited skills must read identically (modulo `<phase>`/`<Anchor>`) to the
five compliant siblings so behavior is provably the same.
