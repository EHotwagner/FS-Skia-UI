# Feedback Hook Auto-Fire + Fourth Prompt (FR-001/002/003 · SC-001/SC-002)

## FR-001/002 — multi-file hook discovery (BD-1)

**Defect.** Every `/speckit-*` phase skill's "Check for extension hooks" step scanned only
the single central file `.specify/extensions.yml`, while the `feedback` extension ships its
hooks separately under `.specify/extensions/feedback/feedback.yml`. A hook in the
per-extension file was therefore invisible, and being `optional: true` the omission was
silent — so the feedback capture never auto-fired.

**Fix.** The "Check for extension hooks" block was rewritten to **multi-file discovery** in
every canonical phase-skill source that carries one — `.agents/skills/speckit-{specify,
clarify,plan,analyze,checklist}/SKILL.md`, both the `before_<phase>` and `after_<phase>`
blocks. (`speckit-tasks` and `speckit-implement` carry no hook-discovery block in their
canonical sources, so there is nothing to convert there.) Each rewritten block now:

1. reads `.specify/extensions.yml` (if present) and collects `hooks.<dir>_<phase>`;
2. enumerates every `.specify/extensions/*/*.yml` (sorted), parses each, and collects its
   `hooks.<dir>_<phase>` too — so the per-extension `feedback` hook is discovered;
3. merges and dedupes by `(extension, command)` (first wins → no double-run when a hook is
   declared in both files — HD-2);
4. drops `enabled: false`; does **not** evaluate `condition` (HD-3);
5. **FR-002**: for every `optional: true` hook discovered-but-not-run, emits one line —
   `Note: optional hook {extension}:{command} is registered but was not run (skipped).` (HD-4).

`.claude/**` was regenerated from the edited `.agents` sources via `RefreshSurfaceBaselines`;
the `Claude Code readiness` sync test (`.claude` mirrors `.agents` byte-for-byte) is **green**
(HD-6). Verification:

```
$ for s in specify clarify plan analyze checklist; do grep -c "multi-file discovery" .claude/skills/speckit-$s/SKILL.md; done
2  2  2  2  2     # before_<phase> + after_<phase> blocks, each phase skill, mirrored into .claude
```

**Scope note (honest).** The fix lands in the canonical phase-skill sources this repository's
own agents execute (the spec's "the same gap exists in this repo's own phase skills and is
fixed there too"). A generated consumer project auto-fires the hook only when its Spec Kit
command set carries this multi-file prose; the discovery algorithm is also captured normatively
in `contracts/hook-discovery.md` for downstream adoption. An end-to-end pack/install/`dotnet new
--feedback true`/run-a-phase capture is **not reproducible in this governance sandbox** (the
generated project's phase commands come from the upstream `specify` CLI, and the aggregate
`Verify` path cannot bootstrap `dotnet-fake` here — see `runtime-limitations.md`); the
behaviour is instead pinned by the prose rewrite + the regenerated mirror + the contract.

## FR-003 — fourth feedback prompt + `## Skill gaps` (USER ask · SC-002)

The `fs-skia-feedback-capture` skill (`template/feedback/skill/SKILL.md`) enumerates exactly
**four** prompts; the fourth asks "What additional or new skills would have been helpful
during the *{phase}* phase? … or 'none'", and the record schema gained a matching
`## Skill gaps` section (line 54). The 058 sourcing contract
(`specs/058-skills-quality-feedback/contracts/feedback-capture.md`) states four prompts and
credits feature 061 for the skill-gaps addition.

Pinned by `tests/Governance.Tests/Feature061GovernanceTests.fs` ("FR-003 the feedback skill
enumerates exactly four prompts and a Skill gaps section", FB-1/FB-2) — **green**.

**No surviving "three prompts" reference (SC-002 / FB-4):**

```
$ grep -rn "three prompts\|three exact prompts\|three feedback prompts" specs/058-skills-quality-feedback/ template/
NONE — clean
```

(The only remaining "three" in 058 is "three conditional feedback **sources**" — the count of
template files the `--feedback true` branch adds, which is accurate and unrelated to prompts.)
The stale references were swept across 058 `spec.md`, `research.md`, `plan.md`, `tasks.md`,
`readiness/template-feedback-true.md`, and `readiness/task-graph.{json,md}`, each updated to
"four prompts (4th = skill-gaps, added by 061)".
