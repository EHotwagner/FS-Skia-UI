# Contract: Skill-Id Resolution Guard (US1)

**Satisfies:** FR-001, FR-002, FR-003 · SC-007

## Inputs

- Advertised-id set: `... -> <id>` mappings in
  `.agents/skills/speckit-tasks/SKILL.md` (and `.claude/` mirror) plus the
  harness "available skills" surface.
- Declared `name:` set: every `SKILL.md` under `src/*/skill/`,
  `.agents/skills/*/`, `.claude/skills/*/`, `template/fragments/*/skill/`.

## Rules

1. **Resolution (FR-001).** Every advertised id MUST equal some declared
   `name:`. An unresolved id is a failure naming the id and its advertising
   file:line.
2. **Triple agreement (FR-002).** For each skill, `directory` name, declared
   `name:`, and every advertised id MUST be mutually consistent — for repo skills
   and for skills generated into a consumer project.
3. **Peer sync (FR-003).** The `.agents/skills/<x>` and `.claude/skills/<x>`
   copies MUST declare the same `name:` and advertise the same id.

## Remediation in this feature

- Remove `speckit-debug-loop` from `speckit-tasks/SKILL.md:145,149` (both copies).
  No debug-loop skill exists to repoint to.

## Failing-first fixture

`readiness/skill-resolution-fixtures/` — a deliberately dangling id and a
directory/`name:` mismatch; the guard FAILS on them and PASSES on the corrected
repository.
