# Phase 1 Data Model

This feature is governance-tooling + documentation; the "entities" are the
parsed/validated records in the compiled engine and the consumer-facing skill
registry, not application runtime state.

## Entity: `DepsEntry` (build/Governance/Evidence/DepsParser.fs)

The parsed per-task value in `tasks.deps.yml`.

| Field           | Type                  | Change         | Notes |
|-----------------|-----------------------|----------------|-------|
| `Deps`          | `string list option`  | unchanged      | explicit non-phase dependency task ids |
| `Skillist`      | `string list option`  | unchanged      | ordered capability skill ids |
| `Owns`          | `string list option`  | **NEW**        | gated-evidence ownership (see vocabulary) |
| `LegacyBareList`| `bool`                | unchanged      | legacy bare-list metadata flag |

**Validation rules**:

- `Owns` values MUST be drawn from the closed vocabulary in
  `contracts/tasks-deps-schema.md`. An unknown value is a directive error:
  `"<Tid>: unknown owns value '<v>'; allowed: graph-validation, evidence-audit,
  task-generation, implementation-loading, constitution"`.
- When `Owns` contains a value with an implied skill, that task's `Skillist`
  MUST include the implied skill, else:
  `"<Tid>: owns <v> requires skill <skill> in skillist; declared_skillist=[…]"`.
- The top-level document MUST have `schema_version` and a `tasks:` mapping. Bare
  top-level `Tnnn:` keys (no `tasks:` wrapper) short-circuit to:
  `"tasks.deps.yml: missing or malformed 'tasks' mapping (found bare task keys;
  nest them under a top-level 'tasks:' mapping with schema_version)"` (FR-007).

## Entity: `DepsModel` (unchanged shape)

`{ Order: string list; Map: Map<string, DepsEntry>; Errors: string list }` —
`Map` now carries `Owns` per entry.

## Removed: title-trigger capability matcher (Audit.fs)

Deleted entities/functions:

- `capabilityTriggerGroups : (string * string * string list) list`
- `triggerMatchesTitle`
- `expectedCapabilityMatches` (also removed from `Audit.fsi`)

Replaced by `owns`-driven assessment. The `SkillAssessment` record is retained
but its `CandidateSkillId` / `MatchedSignals` / `Confidence` now derive from
`Owns` (or report trusted-as-declared), never from title text. Title is no
longer read for capability inference (FR-010, SC-006).

## Entity: Skill registry (build/Governance/Evidence/SkillRegistry.fs)

Enumerates `.agents/skills/*`, `src/*/skill`, `template/fragments/*/skill`,
keyed by each `SKILL.md`'s `name:`.

**Changes (FR-012)**:

- Remove registered id `fs-skia-layout-evidence`.
- Add `fs-skia-evidence-mode` (deterministic-evidence-mode + host-warning
  classification).
- Add `fs-skia-layout-readability` (HUD/gameplay-region/public-scene naming
  guidance).

Both new skills are canonical under `.agents/skills/<id>/SKILL.md` and ship to
consumers via new `.template.config/template.json` `sources` entries (to both
`.agents/skills/` and `.claude/skills/`). `SkillSyncCheck` enforces the
`.claude` regeneration; `SkillQualityCheck` enforces per-skill quality.

## Entity: Hint table (consumer-facing, in skill prose + deps template)

A mapping `concern → skill-id` used advisorily by authors. Each `skill-id` MUST
resolve to exactly one consumer-registerable skill (canonical-in-all-profiles or
product-conditional, annotated by profile). Enforced by the hint-resolution
contract (`contracts/skill-hint-resolution.md`).

State/derivation summary:

```
.specify/feature.json (feature_directory)
        │  read at build.fsx interpreter edge (template + framework, identical pattern)
        ▼
resolved feature dir ──► tasks.md + tasks.deps.yml (now with owns:) + readiness/
        │  DepsParser → DepsModel(+Owns)   Audit.validateAndMerge (no title scan)
        ▼
task-graph.json / task-graph.md  +  errors (directive)
```
