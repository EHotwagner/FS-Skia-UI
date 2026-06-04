# Contract: SkillContractPathCheck (FR-004)

## Purpose
Fail the build when a capability/product skill names a `docs/api-surface/...fsi`
contract source that a generated project does not emit, or claims "no DLL reflection
needed" against an absent path — so skill claims and generated output cannot drift.

## Inputs
- `SkillContractClaim[]`: parsed from `template/product-skills/*/SKILL.md` and
  `src/*/skill/SKILL.md` (every `docs/api-surface/<Pkg>/<file>.fsi` reference and any
  "no DLL reflection needed" assertion).
- `ApiSurfaceEntry[]`: the emitted `template/base/docs/api-surface/` tree.

## Rules (each violation = build failure with a named diagnostic)
1. Every `claimedPath` MUST equal some emitted `ApiSurfaceEntry.emittedPath`.
   - Failure: `MISS api-surface: <claimedPath> claimed by <skillPath> is not emitted`.
2. A skill asserting `claimsNoReflection` MUST have an existing `claimedPath`.
3. (Orphan, advisory→error) An emitted api-surface file no skill claims is reported;
   promote to error if the spec's "named contract path renamed/moved" edge applies.

## Routing
Routed by the `template/product-skills/**`, `src/**/skill/SKILL.md`, and
`template/base/docs/api-surface/**` globs in `Routing.fs`; appears in
`validation.contract.yml` after regeneration.

## Test evidence (failing-first)
- Negative: a fixture skill claiming a nonexistent api-surface path makes the check
  fail before the emitter exists; passes after emission.
- Positive: all five product skills resolve to emitted files.

## Acceptance
SC-002.
