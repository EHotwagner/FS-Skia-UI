# Governance Risk Levels — Feature 088

- **Small** (Tier 1/3 byte-identical): `Dev` + `TargetMetadataDrift` + the failing-first
  Feature 088 governance tests. No broad rerun.
- **Medium** (Tier 2 routing/target split): adds the `Route` before/after captures
  (`route-before.txt`, `route-after-*.txt`), the regenerated `validation.contract.yml` diff,
  and the `GeneratedProductCheck` umbrella/sub-target composition.
- **Broad** (could alter effective coverage): the escalated six-target order, recorded
  **non-authoritatively** in `logs/` with per-target verdicts. `GeneratedProductCheck` may
  fail locally for environment reasons (see `runtime-limitations.md`).

Authoritative gate for this change: `Dev` (build + full unit/governance suites) and
`TargetMetadataDrift` (contract currency). Both PASS.

## Required evidence per risk level

- **Small** — **required evidence**: `Dev` + `TargetMetadataDrift` + the Feature 088 tests.
- **Medium** — **required evidence**: the above plus `Route` before/after captures and the
  regenerated `validation.contract.yml` diff.
- **Broad validation** — **required evidence**: the escalated six-target order run
  sequentially, recorded non-authoritatively in `logs/`. **Broad validation** is required only
  when a Tier 2 change could alter effective gate coverage; this feature's effective coverage is
  preserved (the doc-only relaxation is additive; mixed/source routing is unchanged), so the
  broad run is captured for completeness rather than as a coverage gate.
