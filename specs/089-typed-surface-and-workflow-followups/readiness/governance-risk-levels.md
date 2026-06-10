# Governance Risk Levels — Feature 089

Governance risk level for this change is **broad** (consumer-contract surface +
governance code + Spec-Kit skill tree), so the focused validation is the
serialized six-target order.

- **Small** (framework-internal byte-identical): `Dev` + the failing-first
  Feature 089 governance tests. No broad rerun.
- **Medium** (single governance seam): adds `TargetMetadataDrift` /
  `GeneratedGuidanceCheck` currency over the regenerated artifacts.
- **Broad** (consumer-contract + skill-tree): the escalated six-target order,
  recorded **non-authoritatively** in `logs/` with per-target verdicts.
  `GeneratedProductCheck` may fail locally for environment reasons (see
  `runtime-limitations.md`).

Authoritative gates for this change: `Dev` (build + full unit/governance suites),
`TargetMetadataDrift` / currency (api-surface + catalog + skill-sync), and the
Feature 089 governance tests — all PASS.

## Required evidence per risk level

- **Small** — **required evidence**: `Dev` + the Feature 089 tests.
- **Medium** — **required evidence**: the above plus the currency gates
  (`GeneratedGuidanceCheck`, `TemplateCheck`) over the regenerated api-surface,
  `catalog.yml`, and the `.claude` skill mirror.
- **Broad validation** — **required evidence**: the escalated six-target order run
  sequentially, recorded non-authoritatively in `logs/`. **Broad validation** is
  required here because the change touches `template/**`, the emitted
  `docs/api-surface` tree, and the `.agents`/`.claude` skill tree; the additions
  are additive (the legacy builder surface stays published), so effective gate
  coverage is preserved.
