# Quickstart: validating the live-path skill refresh

This is a documentation-currency change (Tier 2, no `.fsi`/behavior delta). Validation proves
**currency**, **sync**, and **zero source delta** — not new runtime behavior.

## 1. Route the change (authoritative gate list)

```sh
./fake.sh build -t Route
```

Obey the printed tier + gate list. Expect the skill gates (`SkillQualityCheck`, `SkillSyncCheck`)
and — because US2 edits `src/Controls/skill/SKILL.md` under `src/Controls/**` — likely the
escalated controls-public-surface set + `EvidenceGraph` + `EvidenceAudit` (feature-102 precedent).
FAKE-backed targets run **sequentially**.

## 2. Regenerate the mirror + skillist after editing canonical files

Edit only canonical inputs (`.agents/skills/fs-skia-reconciliation`,
`.agents/skills/fs-skia-controls-host`, `src/Controls/skill/SKILL.md`), then:

```sh
./fake.sh build -t RefreshSurfaceBaselines
```

Regenerates `.claude/skills/**` (byte-identical) and `template/base/docs/skillist-reference.md`
(registers the new `fs-skia-controls-host` id). Never hand-edit `.claude/**`.

## 3. Run the gates (sequential, deterministic order)

```sh
./fake.sh build -t Dev
./fake.sh build -t SkillQualityCheck   # all in-scope skills PASS the 7-section rubric
./fake.sh build -t SkillSyncCheck      # no .agents <-> .claude drift
# then any escalated gates Route printed (GeneratedGuidanceCheck/TemplateCheck/
# GeneratedProductCheck/EvidenceGraph/EvidenceAudit as applicable)
```

## 4. Prove the pure-honesty invariant (FR-008 / SC-005)

```sh
git diff --stat | grep -E 'src/.*\.fsi' && echo 'VIOLATION: .fsi changed' || echo 'OK: zero .fsi delta'
git diff --name-only | grep -E '^src/.*\.fs$' && echo 'CHECK: a .fs file changed' || echo 'OK: no .fs change'
```

Expected: zero `.fsi` lines; the only `src/**` file touched is `src/Controls/skill/SKILL.md`
(Markdown). No product test file changes.

## 5. Spot-check the claims landed (maps to contracts/currency-claims.md)

```sh
# US1: reconciliation skill names the live-path additions and drops "future work" framing
grep -n 'RemeasuredNodeCount\|AnimationClock\|sampleOnPaint\|applyRuntimeVisualState' \
  .agents/skills/fs-skia-reconciliation/SKILL.md
grep -n 'builds atop the wired path' .agents/skills/fs-skia-reconciliation/SKILL.md \
  && echo 'STALE LINE STILL PRESENT' || echo 'OK: stale forward-looking line removed'

# US2: Controls skill E3/E4 current
grep -n 'deriveVisualState\|NavIntent\|navRange' src/Controls/skill/SKILL.md

# US3: new host skill exists, mirrored, and registered
test -f .agents/skills/fs-skia-controls-host/SKILL.md && echo 'canonical OK'
test -f .claude/skills/fs-skia-controls-host/SKILL.md && echo 'mirror OK'
grep -n 'fs-skia-controls-host' template/base/docs/skillist-reference.md
```

## Acceptance (ties to Success Criteria)

- **SC-001/SC-002**: grep checks in §5 pass; a cold read matches `RetainedRender.fsi`/`Focus.fsi`/
  `ControlRuntime.fsi`.
- **SC-003**: one dedicated host skill, rubric-green, in `skillist-reference.md`.
- **SC-004**: `SkillQualityCheck` + `SkillSyncCheck` PASS; mirror byte-identical.
- **SC-005**: §4 shows zero `.fsi` delta and no test-outcome change.
