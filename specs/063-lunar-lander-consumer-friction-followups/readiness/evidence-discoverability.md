# Evidence-format discoverability + readiness-diagnostic legibility (FR-004/005, SC-004)

## Diagnostic relabel — distinct labels for one absent token (FR-004, SC-004)

`build/Governance/Evidence/Render.fs` `readinessContractDiagnostics` now labels the
**full required set** and the **absent subset** distinctly:

- `full-required-set:` — the complete enforced token list (recovery shape), and
- `absent-from-file:` — only the tokens actually missing from the file.

So a failure with **one** absent token no longer reads as "all missing." Proven by:

- `tests/Governance.Tests/Feature063GovernanceTests.fs` — "FR-004 one absent token
  prints full-required-set and absent-from-file under distinct labels": with
  `Required = [task; skill id; loaded_at]` and `MissingTerms = [loaded_at]`, the render
  contains `full-required-set: task, skill id, loaded_at` **and**
  `absent-from-file: loaded_at`, and **neither** old ambiguous label (`required-tokens:`,
  `missing:`) remains.
- `tests/Governance.Tests/Feature061GovernanceTests.fs` RC-2 (re-pointed to the new
  vocabulary): a partial `governance-risk-levels.md` missing one token still prints the
  full enforced list under `full-required-set:` and the single gap under
  `absent-from-file:`.

Failing-first → green: the relabel test failed against the pre-change `required-tokens:`
output (`Expected … to contain 'full-required-set: …'`), then passed after the Render.fs
label swap. 7/7 FR-004 tests (061 + 063) pass.

No data shape change: `Required = Some terms` (full set) and `MissingTerms` (absent
subset) already existed in `Scans.fs`; only the two labels in the renderer changed. The
separate `EvidenceFormatSchema.renderSchema` `required-tokens:` line (the format-schema
description) is a different context and is intentionally left unchanged.

## Authoring discoverability — speckit-implement pointers (FR-004/005)

`.agents/skills/speckit-implement/SKILL.md` (canonical; regenerated to `.claude/**` via
`RefreshSurfaceBaselines`) now:

- opens the per-task **Workflow** with a "Before you author any readiness/evidence file,
  read the generated `docs/evidence-formats.md`" pointer — author against the contract,
  not against a gate failure; and
- enriches the skill-loading step to state that `skill-loading-evidence.md` is read/written
  from the **feature** readiness dir `specs/<feature>/readiness/` (not a repo-root
  `readiness/`), needs **one row per (task, declared-skill)** with the resolved
  `.agents/skills/<id>/SKILL.md` path and `loaded_at` strictly **before**
  `work_started_at`, and is **enforced only once a task flips to `[X]`** (so it surfaces
  late — author rows as you load, not retroactively).
