# Contract: Feedback capture record (4-prompt schema)

**Feature**: `061-breakout-consumer-friction-followups` · **FR-003**
**Sources of truth**: `template/feedback/skill/SKILL.md` (canonical skill, shipped
under `--feedback true`) and `specs/058-skills-quality-feedback/contracts/feedback-capture.md`
(the 058 sourcing contract). This contract restates the 061 delta only.

## Prompts (exact wording, `{phase}` substituted)

1. "During the *{phase}* phase, did anything go wrong or cause friction in the
   fs-skia-ui / Spec Kit process — and what would have helped you?"
2. "Did you write any F# code on a skill topic this phase that could be
   generalized into the support library? If yes, name the skill family/topic and
   the candidate helper (and link any external docs/research used)."
3. **(NEW — 061)** "What additional or new skills would have been helpful during
   the *{phase}* phase? Name the topic and what the missing skill should have
   covered, or 'none'."
4. "How blocking was the friction — none / minor / major / blocker?"

## Record schema

```markdown
---
phase: <phase>
date: <YYYY-MM-DD>
severity: <none|minor|major|blocker>   # answer to prompt 4
---

## Process friction
<answer to prompt 1>

## Generalizable code
<answer to prompt 2, or "none">

## Skill gaps
<answer to prompt 3 — topic + what the missing skill should cover, or "none">

## Research links
<official-docs-first then community links; offline: "research blocked — <why>">
```

## Conformance

| ID | Assertion |
|----|-----------|
| FB-1 | The skill enumerates **exactly four** prompts `1.`–`4.` with the wording above. *(SC-002)* |
| FB-2 | The record schema contains a `## Skill gaps` section. |
| FB-3 | Prompt 3 = "none" still writes a well-formed `## Skill gaps` section. *(edge)* |
| FB-4 | No surviving "three prompts" reference in the skill, its template example, the 058 contract, or 058 spec/readiness/research/plan/tasks. *(SC-002)* |
| FB-5 | A governance/template check fails if the four-prompt set or `## Skill gaps` is dropped. *(D6)* |

## Notes

- The feedback skill is **template-only** (`template/feedback/skill/SKILL.md`);
  it is not part of the `.agents`/`.claude` skill tree, so `SkillSyncCheck` does
  not govern it — but `TemplateCheck` / `GeneratedProductCheck` may pin its
  content, and the D6 check enforces the prompt count.
