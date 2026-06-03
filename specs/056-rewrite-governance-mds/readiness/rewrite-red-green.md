# Rewrite Red→Green — Drift Detection Survived at 055 Strength

The negative proof for SC-003/SC-005 (FR-002/FR-004): after the rewrite, the
currency gate still **fails** on a real source-of-truth mutation, a contract-token
removal, and a reintroduced forbidden term — each naming the file and the unmet
rule — then returns green on revert. Authoritative command for every run:
`./fake.sh build -t GeneratedGuidanceCheck`.

## 1. Source-of-truth obligation mutation (AllOf concept deleted)

- **Mutation:** deleted the AllOf concept phrase `mandatory pre-task skill loading gate`
  from `.specify/memory/constitution.md` (the `constitution-skill-gates` obligation).
- **Result: FAIL** —
  `.specify/memory/constitution.md: obligation 'constitution-skill-gates' (constitution:Local Agent Skills) not reflected [task-skillist-guidance]`
- **Revert** (`git checkout`) → gate green again.

## 2. Contract-token removal

- **Mutation:** removed the `loaded_at` contract token from
  `.agents/skills/speckit-implement/SKILL.md`.
- **Result: FAIL** —
  ``.agents/skills/speckit-implement/SKILL.md: missing `loaded_at` [task-skillist-guidance]``
- **Revert** → gate green again.

## 3. Reintroduced C3 forbidden term

- **Mutation:** added `renderer neutral` to `template/fragments/controls/README.md`.
- **Result: FAIL** —
  ``generated controls guidance contains stale term `renderer neutral` [controls-boundary-guidance]``
- **Revert** → gate green again.

## Control

After reverting all three mutations, `./fake.sh build -t GeneratedGuidanceCheck`
exits **Status: Ok** (green). Drift detection retains full 055 strength over the
rewritten corpus: a deleted obligation, a removed token, and a reintroduced
forbidden term each still hard-fail the gate with a file+rule diagnostic.

> Note: the three reverts used `git checkout`, which restores the committed HEAD;
> the two files that also carried this feature's uncommitted tightening
> (`constitution.md` Principle III, `speckit-implement/SKILL.md`) were
> re-applied/re-synced afterward and re-verified green (`GeneratedGuidanceCheck`,
> `SkillSyncCheck`, `TargetMetadataDrift` all PASS).
