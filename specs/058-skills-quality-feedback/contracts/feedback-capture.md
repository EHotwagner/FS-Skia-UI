# Contract: Feedback Parameter, Prompts & Record

## Template parameter (FR-011/FR-012)

```json
// .template.config/template.json  (symbols)
"feedback": {
  "type": "parameter",
  "datatype": "bool",
  "defaultValue": "false",
  "description": "Capture per-phase Spec Kit feedback into specs/<feature>/feedback/."
}
```

- `--feedback false` (default): **no** new files, hooks, or prompts; generated
  output byte-identical to today (verified by a generation-diff test, SC-006).
- `--feedback true`: emit the `after_*` feedback hooks + the
  `fs-skia-feedback-capture` command skill + the `feedback/` destination.

All `feedback == true` content lives in the template's conditional branch
(`#if`/conditional `sources`); the false branch emits no markers/whitespace.

## Hook wiring (FR-013, D5) — generated `.specify/extensions.yml` (feedback branch)

For each of `after_specify`, `after_clarify`, `after_plan`, `after_tasks`,
`after_analyze`, `after_implement`, append:

```yaml
  - extension: feedback
    command: speckit.feedback.capture     # → skill fs-skia-feedback-capture
    enabled: true
    optional: true
    prompt: Capture fs-skia-ui feedback for the {phase} phase?
    description: Per-phase fs-skia-ui / Spec Kit feedback capture
    condition: null
```

These fire only on phase **completion** (after_* semantics), so an aborted/failed
phase writes nothing (FR-016).

## The three prompts (FR-013 — exact wording, `{phase}` substituted)

1. "During the *{phase}* phase, did anything go wrong or cause friction in the
   fs-skia-ui / Spec Kit process — and what would have helped you?"
2. "Did you write any F# code on a skill topic this phase that could be
   generalized into the support library? If yes, name the skill family/topic and
   the candidate helper (and link any external docs/research used)."
3. "How blocking was the friction — none / minor / major / blocker?"

## Record schema (FR-014/FR-015) — `specs/<feature>/feedback/<phase>-<date>.md`

```markdown
---
phase: plan
date: 2026-06-03
severity: minor            # none | minor | major | blocker
---

## Process friction
<answer to prompt 1 — what went wrong + what would have helped>

## Generalizable code
<answer to prompt 2 — skill family/topic + candidate helper, or "none">

## Research links
<official-docs-first then community links, when created after a hard problem;
 offline: "research blocked — <why>">
```

- One record per phase (FR-014).
- A record naming generalizable code MUST capture the skill topic + candidate
  helper so it can be triaged into `FS.Skia.UI.SkillSupport` (FR-015 → US2).
- Severity is mandatory (FR-015).
- After a hard problem the research links MUST be present, official docs first
  then community (FR-015/FR-017); offline degrades to "research blocked + why"
  (FR-018).

## Command skill `fs-skia-feedback-capture`

A new FS-authored authoring command skill (canonical `.agents/skills/`, shipped in
the template's feedback branch). It instructs the agent to surface the three
prompts with `{phase}` substituted and write the record above. It is itself
in-scope for the quality bar.
