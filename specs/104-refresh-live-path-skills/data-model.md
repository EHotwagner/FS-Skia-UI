# Data Model: Refresh live-path skill currency

This feature has no runtime data model (no `.fs`/`.fsi` change). The "entities" are the skill
artifacts and their governed structure — the things the implementation creates/edits and the
gates validate.

## Entity: Skill artifact

A Markdown file with YAML frontmatter, governed by `SkillQualityCheck` / `SkillSyncCheck`.

| Field | Meaning | Source of truth |
|---|---|---|
| `name` | the skill id (kebab) | frontmatter `name:` |
| `description` | one-line recall hook | frontmatter `description:` |
| body sections | the 7 required rubric sections | `contracts/rubric.md` |
| canonical path | hand-authored input | `.agents/skills/<id>/SKILL.md` (or `src/Controls/skill/SKILL.md` for the package skill) |
| mirror path | generated, byte-identical | `.claude/skills/<id>/SKILL.md` (`.agents` skills only) |
| registry row | id listed for discovery | `template/base/docs/skillist-reference.md` (`.agents` skills) |

### Instances in scope

| Instance | Action | Canonical path | Mirrored? | Registry? |
|---|---|---|---|---|
| `fs-skia-reconciliation` | **edit** (US1) | `.agents/skills/fs-skia-reconciliation/SKILL.md` | yes (regenerate) | already listed |
| `fs-skia-ui-widgets` (Controls) | **edit** (US2) | `src/Controls/skill/SKILL.md` | no (package skill) | n/a (package) |
| `fs-skia-controls-host` | **create** (US3) | `.agents/skills/fs-skia-controls-host/SKILL.md` | yes (regenerate) | **add row** |

## Entity: Currency claim

A single assertion a skill must make, bound to a verified `.fsi` anchor. The full set is
`contracts/currency-claims.md` (C1/C2/C3). Validation rule: every "Must state" row appears in the
edited skill; every "Must NOT state" row is absent.

State transition (per skill, conceptually):

```
stale (asserts pre-roadmap facts / forward-looking-as-future)
  → edited (all C-rows present, stale rows removed)
  → regenerated (.claude mirror + skillist refreshed)
  → green (SkillQualityCheck PASS, SkillSyncCheck no-drift)
```

## Invariants (cross-cutting, from FR-008)

- **Zero `.fsi` delta**: `git diff` touches no `src/**/*.fsi` line. (SC-005)
- **Zero behavior/test-outcome change**: no `src/**/*.fs` edit; product test suites unchanged.
- **Mirror byte-identity**: every edited/added `.agents` skill equals its `.claude` copy.
- **Anchor liveness**: every cited anchor exists on `main` (re-checked at authoring time).
