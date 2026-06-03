# Validation Contract Currency — Feature 055

Docs-only / single-source-generation currency note (FR-010). The `Route`
docs-only and evidence-governance rules require this artifact.

## Single-source-generation invariants held green

- **`validation.contract.yml` ← `Routing.fs`** — `Routing.fs` is **not edited**
  by this feature, so `validation.contract.yml` does not regenerate and
  `TargetMetadataDrift` stays current.
- **`.claude/skills/**` ← `.agents/skills/**`** — no canonical `.agents` skill
  prose was tightened (the prose edit landed only in `.specify/templates/**`),
  so the `.claude` tree remains a byte-identical reproduction and `SkillSyncCheck`
  stays green. `RefreshSurfaceBaselines` produces no delta.

## What changed

- `build/Governance/Guidance.fs[i]` — the three guidance validators
  (`validateTaskSkillistGuidance`, `validateControlsBoundaryGuidance`,
  `validateSerializedRunnerGuidance`) refactored onto the pure
  `evaluateGuidanceCheck` over `ContractToken` + `GuidanceObligation`; the gate
  entry point `runGeneratedGuidanceScan` is unchanged.
- `tests/Governance.Tests/**` — added US1/US2/SC-004/FR-006 pure-core tests and
  the prose-size-accounting render test.
- `.specify/templates/tasks-template.md` (+ preset twin) — prose tightened.
- `docs/reports/_baselines/2026-06-02-foundations-after.md` and
  `specs/047-foundations-programme-closeout/contracts/after-baseline.md` — the
  size goal restated against the corrected ≈6,882 baseline (FR-008).

No product `.fsi` surface, package identity, or runtime behavior changed.
