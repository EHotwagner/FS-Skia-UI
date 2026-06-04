# US1 — Always-on feedback capture + deterministic hook precedence (FR-001/002)

Independent validation path for User Story 1. All evidence is real (template source +
generated project + a repo-side regression gate).

## FR-001 — feedback hook is mandatory (auto-fires, no manual trigger)

- **Canonical source.** `template/feedback/extensions/feedback.yml` — all six
  `after_<phase>` entries (`after_specify`, `after_clarify`, `after_plan`, `after_tasks`,
  `after_analyze`, `after_implement`) now register `optional: false` (T012). This file is
  copied into generated projects via `.template.config/template.json`
  (`template/feedback/extensions/` → `.specify/extensions/feedback/` under
  `--feedback true`).
- **Generated-project check (T011).** `dotnet new fs-skia-ui --feedback true`; inspect
  `.specify/extensions/feedback/feedback.yml` — every `after_<phase>` is `optional: false`,
  and completing a phase auto-writes `specs/<feature>/feedback/<phase>-<date>.md` with no
  manual nudge (the hook is mandatory, so the precedence rule auto-runs it under
  `auto_execute_hooks: true`).
- **Repo-side regression gate (T010/T015).** `Guidance.feedbackHookOptionalFindings`
  asserts no `optional: true` survives under any feedback hook, folded into
  `GeneratedGuidanceCheck` (`validateFeedbackHookPolicy`). Unit-tested in
  `tests/Governance.Tests/Feature062GovernanceTests.fs` (a seeded `optional: true` is
  flagged; `optional: false` passes; the shipped file is clean). Low-cost, deterministic.

## FR-001/002 — precedence rule + effective-hooks notice (D1/D2)

The precedence rule + the consolidated effective-hooks notice were added to the hook step
of every phase skill **that has one** — the five hook-bearing skills
`speckit-{specify,clarify,plan,analyze,checklist}` (T013). `speckit-tasks` and
`speckit-implement` have **no** hook-discovery step (consistent with feature 061), so the
rule has no hook step to attach to there.

- **Precedence rule (D1, contract C2):** `auto_execute_hooks: true` scopes the **mandatory**
  set only — a `optional: false` hook auto-runs; a `optional: true` hook is **always
  surfaced** and is never force-run by `auto_execute_hooks`; a `condition`-guarded hook is
  never evaluated by the skill (left to the executor). Under `auto_execute_hooks: false`,
  even mandatory hooks are surfaced for confirmation.
- **Effective-hooks notice (D2, contract C3):** after the merge + dedup by
  `(extension, command)`, each skill emits **one** consolidated `## Effective hooks for
  <phase>` notice listing each hook's resolved decision (`auto-run` / `surfaced` /
  `skipped (disabled)` / `condition-deferred`), so the operator never hand-reconciles the
  extension files. The promoted feedback hook appears as `auto-run`.

The `.claude/**` mirror is regenerated from these `.agents/**` edits via
`RefreshSurfaceBaselines` (T014); `SkillSyncCheck`/`TargetMetadataDrift`/`SkillQualityCheck`
stay green.
