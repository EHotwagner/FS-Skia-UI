# Gate Self-Tests — 040 (T020 / T025)

Both deliberate-break self-tests are **gate self-tests, not shipped synthetic
fixtures** (plan: Synthetic evidence — None). The break is introduced, observed,
and reverted within the same run.

## SC-002 — `SkillSyncCheck` (T020): PASS → FAIL(names slug) → PASS

1. **PASS** — refined six pairs byte-identical; `readiness/skill-sync-check.md`
   lists six matching SHA-256 digests. `Status: Ok`.
2. **FAIL** — appended one newline byte to
   `.agents/skills/fsharp-io-globbing/SKILL.md`. Gate failed naming the slug and
   both digests:
   ```
   SkillSyncCheck FAILED — byte-identity drift between the two skill trees:
   SkillSyncCheck drift: fsharp-io-globbing —
     .claude/.../SKILL.md=da76c5b5… .agents/.../SKILL.md=0170cd2f…
   Status: Failure
   ```
3. **PASS** — re-synced `.agents` copy from `.claude`; gate returned
   `Status: Ok` with six matching digests restored.

## SC-007 — `SkillExamplesCheck` (T025): PASS → FAIL(names skill/block) → PASS

1. **PASS** — all 26 ` ```fsharp ` blocks compiled; report lists per-skill block
   counts (parsing 8, graph 5, code-gen 4, globbing 3, shell 2, orchestration 4).
2. **FAIL** — replaced `Proc.run` with a non-existent `Proc.runThisDoesNotExist`
   in the `fsharp-shell-process` C17 block (both trees, identically, so the
   prerequisite `SkillSyncCheck` still passed). The gate mapped the generated
   diagnostic back to the owning skill + block:
   ```
   Offending skill/block(s):
     Generated/fsharp-shell-process.fs(33,21): error FS0039: The value …
       'runThisDoesNotExist' is not defined.   [// source: .claude/skills/fsharp-shell-process/SKILL.md:68]
   Status: Failure
   ```
3. **PASS** — restored `Proc.run` in both trees; gate returned `Status: Ok`,
   all 26 blocks compiling again.

Logs: `readiness/logs/skill-sync-check.txt`, `readiness/logs/skill-examples-check.txt`.
