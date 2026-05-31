# Skill-Id Resolution Evidence (US1, FR-001/002/003, SC-007)

Authoritative guard: `validateSkillIdResolution` in `build.fsx`, surfaced through
`./fake.sh build -t GeneratedGuidanceCheck`. It reads only repository files (a
FAKE target cannot enumerate the runtime "available skills" harness surface).

## Inputs

- **Advertised ids** — single-line `-> <id>` mappings in
  `.agents/skills/speckit-tasks/SKILL.md` and `.claude/skills/speckit-tasks/SKILL.md`.
- **Declared `name:`** — every `SKILL.md` under `.agents/skills/*`,
  `.claude/skills/*`, `template/base/.agents/skills/*`,
  `template/base/.claude/skills/*`, `src/*/skill`, `template/fragments/*/skill`.

## Rules enforced

1. Resolution (FR-001): every advertised id resolves to a declared `name:`.
2. Triple agreement (FR-002): for the `.agents`/`.claude` registries (repo and
   `template/base`), directory == declared `name:`.
3. Peer sync (FR-003): `.agents/skills/<x>` and `.claude/skills/<x>` declare the
   same `name:` (repo and `template/base` — the generated-project skill set).

## PASS on the corrected repository (SC-007)

```
$ ./fake.sh build -t GeneratedGuidanceCheck
GeneratedGuidanceCheck   00:00:00.06   Status: Ok   (GGC_EXIT=0)
```

Report line: `PASS: every advertised skill id in the speckit-tasks hints
resolves to a declared skill name:; skill directory/name agree and
.agents/.claude peers are synchronized.`

The six advertised ids — `fs-skia-scene`, `fs-skia-skiaviewer`,
`fs-skia-layout-evidence`, `fs-skia-template-update`, `speckit-evidence-graph`,
`speckit-evidence-audit` — all resolve.

## FAIL on a dangling id (failing-first, authoritative guard)

Temporarily reintroducing `... -> speckit-debug-loop` on line 149 of both
speckit-tasks copies and re-running the real guard:

```
GGC_FAIL_EXIT=1
Generated guidance check failed:
.agents/skills/speckit-tasks/SKILL.md:149: advertised skill id `speckit-debug-loop` does not resolve to any declared skill `name:` [skill-id-resolution]
.claude/skills/speckit-tasks/SKILL.md:149: advertised skill id `speckit-debug-loop` does not resolve to any declared skill `name:` [skill-id-resolution]
```

The guard names the offending id **and** the advertising `file:line`. The
reintroduction was reverted; the corrected repository passes (above).

## FAIL on all three drift classes (fixtures)

`skill-resolution-fixtures/run-check.fsx` (mirror of the guard rules) over three
controlled fixture trees — see `skill-resolution-fixtures/run-check-output.txt`:

```
## fixture-dangling — dangling advertised id
   FAIL: .agents/skills/speckit-tasks/SKILL.md:2: advertised id `speckit-debug-loop` does not resolve
## fixture-dirname-mismatch — directory / declared-name disagreement
   FAIL: directory `fs-skia-scene` disagrees with declared name `fs-skia-renamed-scene`
## fixture-peer-drift — .agents/.claude peer drift
   FAIL: peer skill `fs-skia-scene` declares different name (Some "fs-skia-scene" vs Some "fs-skia-scene-drifted")
RESULT: all three fixtures FAILED as required (failing-first satisfied).
```

## `.agents` ↔ `.claude` peer comparison (FR-003)

`sha1sum` of the two speckit-tasks copies after the debug-loop removal:

```
agents=7da0788bd706b992ea3f6baf04a6d0816307ff03
claude=7da0788bd706b992ea3f6baf04a6d0816307ff03
IDENTICAL PEERS
```

Every `.agents/skills/<x>` has a matching `.claude/skills/<x>` declaring the same
`name:` (and likewise under `template/base`), so the peer-sync rule passes.

## Generated-project skill set (FR-002 edge case, T010)

The guard also validates `template/base/.agents/skills/*` and
`template/base/.claude/skills/*` (the `fs-skia-project` skill a generated project
receives) and the `template/fragments/*/skill` set the same way, covering the
case where an id resolves in this repo but not in the generated project's skills.
