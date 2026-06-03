# Template `--feedback true` Evidence (T032, SC-005)

Under `dotnet new fs-skia-ui --feedback true`, three conditional sources fire (template.json):

- `template/feedback/skill/` → `.agents/skills/fs-skia-feedback-capture/SKILL.md` and
  `.claude/skills/fs-skia-feedback-capture/SKILL.md` — the `fs-skia-feedback-capture`
  command skill, carrying the **three exact prompts** (with `{phase}` substitution) and the
  feedback **record schema** (`specs/<feature>/feedback/<phase>-<date>.md`, severity
  mandatory, generalizable-code candidate captured for `FS.Skia.UI.SkillSupport` triage).
- `template/feedback/extensions/` → `.specify/extensions/feedback/feedback.yml` — the six
  `after_*` hook entries (`after_specify`/`after_clarify`/`after_plan`/`after_tasks`/
  `after_analyze`/`after_implement`) invoking `speckit.feedback.capture`. `after_*`
  semantics ⇒ they fire only on phase completion; an aborted phase writes nothing (FR-016).

The shipped `fs-skia-feedback-capture` skill is itself in-scope for `SkillQualityCheck`
(the gate's `isInScope` covers `template/feedback/**/skill/SKILL.md`) and PASSES the rubric
(part of the 25/25 PASS).

## Empirical verification — DONE (2026-06-03)

Generated `dotnet new fs-skia-ui -n SameApp --feedback true --allow-scripts yes` (exit 0) and
confirmed all three conditional sources fired:

- `.agents/skills/fs-skia-feedback-capture/SKILL.md` and
  `.claude/skills/fs-skia-feedback-capture/SKILL.md` — present; the skill carries the
  **three exact prompts** under the heading `## The three prompts (exact wording, {phase}
  substituted)`.
- `.specify/extensions/feedback/feedback.yml` — present, carrying all **six** `after_*` hook
  entries (`after_specify`, `after_clarify`, `after_plan`, `after_tasks`, `after_analyze`,
  `after_implement`), each invoking `command: speckit.feedback.capture` with `optional: true`.
  The `sourceName` token resolved correctly (`speckit.feedback.capture` for the *SameApp*
  project). `after_*` semantics ⇒ they fire only on phase completion; an aborted phase writes
  nothing (FR-016).

This is the empirical complement to the `template-feedback-false.md` zero-diff result: the
`--feedback true` branch adds exactly these three sources over the default and nothing else.
