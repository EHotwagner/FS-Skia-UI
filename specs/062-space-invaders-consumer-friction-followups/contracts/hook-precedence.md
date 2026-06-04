# Contract: Hook Execution Precedence & Effective-Hooks Notice (FR-001/002)

**Surface type:** Spec Kit phase-skill behavior (authoring/UI contract) + template
hook registration. No `.fsi`. Canonical edits in `.agents/skills/**` →
regenerated `.claude/**`.

## C1 — Feedback hook registration (FR-001)

`template/feedback/extensions/feedback.yml`: **every** `after_<phase>` entry
(`after_specify`, `after_clarify`, `after_plan`, `after_tasks`, `after_analyze`,
`after_implement`) MUST declare `optional: false`. (Currently all six are
`optional: true`.)

**Check (low-cost, D12):** `TemplateCheck`/`GeneratedGuidanceCheck` asserts no
`optional: true` remains under any feedback hook entry. Regression-guarded.

## C2 — Precedence rule (FR-001), stated in every phase skill

> When `settings.auto_execute_hooks: true` in `.specify/extensions.yml`:
> - a **mandatory** hook (`optional: false`) **auto-runs** (no confirmation);
> - an **optional** hook (`optional: true`) is **always surfaced** ("To execute:
>   `/command`") — `auto_execute_hooks` does **not** force-run it;
> - a hook with a non-empty `condition` is **never** evaluated by the skill;
>   evaluation is left to the executor, and the notice reports the resolved
>   decision rather than forcing a run.
> When `auto_execute_hooks: false`, even mandatory hooks are surfaced for
> confirmation.

This rule MUST appear in the hook step of every phase skill that has one
(`speckit-specify/clarify/plan/tasks/analyze/checklist/implement`).

## C3 — Effective-hooks notice (FR-002)

After multi-file discovery (merge `.specify/extensions.yml` + every
`.specify/extensions/*/*.yml`, dedup by `(extension, command)`, first wins), the
skill emits **one consolidated notice** for the phase:

```
## Effective hooks for <phase>
- {extension}:{command} — auto-run        (mandatory; auto_execute_hooks=true)
- {extension}:{command} — surfaced        (optional)
- {extension}:{command} — skipped         (enabled: false)
- {extension}:{command} — condition-deferred
```

The promoted feedback hook appears as `auto-run`. The operator does not
hand-reconcile files.

## Acceptance (SC-001)

- Generated project: completing a phase auto-writes
  `specs/<feature>/feedback/<phase>-<date>.md` with **no** explicit user nudge
  (registration is `optional: false`).
- With an optional hook registered, the skill applies C2 and prints the C3 notice
  in a single pass — no clarifying round-trip.
