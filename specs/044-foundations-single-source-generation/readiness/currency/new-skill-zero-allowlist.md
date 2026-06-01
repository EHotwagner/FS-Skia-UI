# New Skill, Zero Allowlist Edits (SC-002)

Coverage is by **enumeration** over the canonical tree, not a hardcoded slug list (the
old `SkillSync.expectedSlugs` 6-entry allowlist is **deleted**). Adding a skill therefore
needs **zero** allowlist edits.

## Demo

```
$ mkdir -p .agents/skills/zzz-demo-new-skill
$ cat > .agents/skills/zzz-demo-new-skill/SKILL.md <<'MD'
---
name: zzz-demo-new-skill
description: temporary demo skill proving zero-allowlist enumeration coverage.
---
# zzz-demo-new-skill
Demo body.
MD

$ ls .claude/skills/zzz-demo-new-skill 2>/dev/null      # → NO (derived copy absent before regen)

$ ./fake.sh build -t RefreshSurfaceBaselines            # exit 0 — NO code/allowlist edit
$ ls .claude/skills/zzz-demo-new-skill/                 # → SKILL.md  (the derived tree gained it)
$ ./fake.sh build -t SkillSyncCheck                     # exit 0 — PASS
```

The new skill was reproduced at its `.claude/skills` mirror path purely by enumeration —
no per-skill allowlist or hardcoded slug list was touched. Proof there is no allowlist to
edit:

```
$ grep -rniE 'expectedSlugs|allowlist|"fsharp-parsing"|hardcoded' build/Governance/SkillTreeGen.fs build/Governance/SkillSync.fs
# (only textual hit is the literal word "allowlist" inside a PASS message — no slug list exists)
```

## Cleanup (orphan removal, the reverse direction)

```
$ rm -rf .agents/skills/zzz-demo-new-skill
$ ./fake.sh build -t RefreshSurfaceBaselines            # exit 0
$ ls .claude/skills/zzz-demo-new-skill/SKILL.md         # → gone: regeneration removes the orphan derived file
```

`regenerateSkillTree` deletes any derived file with no canonical source and prunes the
emptied directory, so removing a canonical skill makes its derived copy vanish on the next
regeneration.

**Verdict: PASS** — adding (and removing) a skill flows through enumeration with zero
allowlist edits (SC-002).
