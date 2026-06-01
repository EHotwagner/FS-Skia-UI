# Skills Currency: edit-without-regenerate fails, regenerate passes (SC-001 / SC-008)

`SkillSyncCheck` is reframed (feature 044) from a six-slug byte-identity peer check into a
**generation-currency** check over the whole tree: `.claude/skills/**` must be a current,
byte-identical regeneration of canonical `.agents/skills/**`, covered by **enumeration**
(all 26 files / 25 SKILL.md — no allowlist). The slug used below, `speckit-merge`, is one
of the **19 slugs the old 6-entry allowlist did not guard**.

## Baseline — derived tree current → PASS

```
$ ./fake.sh build -t SkillSyncCheck    # exit 0
PASS: the generated `.claude/skills` tree is a current, byte-identical regeneration of the
canonical `.agents/skills` source across all 26 enumerated file(s) (coverage by enumeration,
no allowlist).
```

## SC-008 — edit the DERIVED copy directly → reported as drift (FAIL)

```
$ printf '\n<!-- tampered derived edit, no regen -->\n' >> .claude/skills/speckit-merge/SKILL.md
$ ./fake.sh build -t SkillSyncCheck    # exit 1
Finished (Failed) 'SkillSyncCheck'
SkillSyncCheck: the generated .claude/skills tree is not a current reproduction of the
canonical .agents/skills source — stale derived skills (bytes differ from canonical):
speckit-merge. Regenerate via ./fake.sh build -t RefreshSurfaceBaselines.
```

The failure **names the offending slug** (`speckit-merge`, previously unguarded) and the
**actionable regeneration command** (FR-012) — not a bare "A and B differ".

```
$ ./fake.sh build -t RefreshSurfaceBaselines   # exit 0 — regenerates derived from canonical
$ ./fake.sh build -t SkillSyncCheck            # exit 0 — byte-identical again → PASS
$ git diff --stat .claude/skills/speckit-merge/SKILL.md   # empty → restored byte-identical
```

## SC-001 — edit the CANONICAL slug without regenerating → FAIL; regenerate → PASS

```
$ printf '\n<!-- canonical edit, not yet regenerated -->\n' >> .agents/skills/speckit-merge/SKILL.md
$ ./fake.sh build -t SkillSyncCheck    # exit 1
SkillSyncCheck: ... stale derived skills (bytes differ from canonical): speckit-merge.
Regenerate via ./fake.sh build -t RefreshSurfaceBaselines.

$ ./fake.sh build -t RefreshSurfaceBaselines   # exit 0
$ grep -c "canonical edit, not yet regenerated" .claude/skills/speckit-merge/SKILL.md   # → 1
$ ./fake.sh build -t SkillSyncCheck    # exit 0 → PASS (derived now byte-identical to edited canonical)
```

The regeneration propagated the canonical edit into the derived tree byte-for-byte, and
the gate passed. The canonical edit and both trees were then reverted/regenerated to the
committed state (`git diff --stat` over both files is empty).

**Verdict: PASS** — edit-without-regenerate fails with an actionable diagnostic on a
previously-unguarded slug; regenerate makes it byte-identical and the gate passes; a direct
derived edit is reported as drift (SC-008).
