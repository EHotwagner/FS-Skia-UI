# Byte-Identity Across All Skill Pairs (SC-001) — in-process proof

In-process F# (System.IO + Array.forall2 byte compare; no diff/cmp/sha256sum/symlink).
Enumerates every file under .agents/skills/** and asserts byte-identity at the
.claude/skills/** mirror path. Covers all 25 SKILL.md plus nested non-SKILL files.

```
canonical files enumerated: 26
byte-identical derived files: 26
of which SKILL.md pairs: 25
mismatches: 0
VERDICT: PASS — every canonical file is byte-identical at its .claude mirror
```
