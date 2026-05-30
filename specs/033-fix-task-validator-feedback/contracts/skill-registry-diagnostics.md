# Contract: Skill Registry Diagnostics

## Scope

This contract covers skill id discovery and diagnostics for `tasks.deps.yml` `skillist` entries.

## Required Behavior

- The authoritative skill registry is built from readable `SKILL.md` files under:
  - `.agents/skills/*/SKILL.md`
  - `src/*/skill/SKILL.md`
  - `template/fragments/*/skill/SKILL.md`
- The accepted skill id is the declared `name:` field in `SKILL.md` when present.
- The directory name is a fallback only when `name:` is absent.
- If an author declares a directory-like id for a readable skill whose declared `name:` differs, diagnostics should identify the accepted declared skill id and the path that exposed it.
- Existing unreadable, missing, duplicate, and ambiguous skill failures remain blocking.

## Readiness Evidence

Record diagnostic proof in `specs/033-fix-task-validator-feedback/readiness/skill-registry-diagnostics.md`.
