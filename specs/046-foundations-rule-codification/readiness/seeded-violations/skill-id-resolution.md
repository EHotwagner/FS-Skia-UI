# Seeded-violation proof — skill-id resolution / no-dangling-id (Stage 6.1, shipped)

**Rule**: every advertised skill id resolves to a declared skill `name:`, and the
`.agents`/`.claude` speckit-tasks advertised id sets stay in sync
(`build/Governance/Guidance.fs` `validateSkillIdResolution`; the same no-dangling-id
guarantee enforced through `Evidence/Engine.fs` skill-registry resolution).
**Gate**: `GeneratedGuidanceCheck` (`runGeneratedGuidanceScan`).
**Authoritative command**: `./fake.sh build -t GeneratedGuidanceCheck`

Real seeded failure (a dangling advertised id is genuinely appended, then restored):

## FAIL — dangling advertised id appended to `.agents/skills/speckit-tasks/SKILL.md`

```
$ printf 'example mapping -> speckit-doesnotexist\n' >> .agents/skills/speckit-tasks/SKILL.md
$ ./fake.sh build -t GeneratedGuidanceCheck
.agents/skills/speckit-tasks/SKILL.md:192: advertised skill id `speckit-doesnotexist` does not resolve to any declared skill `name:` [skill-id-resolution]
.agents/.claude speckit-tasks advertised id sets drift: speckit-doesnotexist [skill-id-resolution]
Status:                  Failure
(exit 134)
```

## PASS — file restored

`cp` the backup back; `git status --porcelain .agents/skills/speckit-tasks/SKILL.md`
→ empty (restored byte-for-byte). This Stage-6.1 gate is **still blocking**; its prose
may be trimmed under FR-008.
