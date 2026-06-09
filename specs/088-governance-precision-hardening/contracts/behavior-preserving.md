# Contract: behavior-preserving acceptance (Tier 1 & Tier 3 — FR-013, SC-005, SC-006)

## What MUST stay byte-identical

- **Tier 1**: `target-metadata.json` for non-routable/internal targets; `AgentValidation.knownGates`
  (set + rendered order) equals prior literal; `Verify`'s `ProductChecksRun` equals prior literal in
  order; `validation.contract.yml` byte-identical (Tier 1 introduces no new target/rule).
- **Tier 3**: the five generated-product file-list reports
  (`app-source.txt`, `app-package.txt`, `headless-scene-source.txt`, `governed-source.txt`,
  `sample-pack-source.txt`) + `GeneratedProductValidationPath`; all governance test goldens; **no**
  `.fsi` / `validation.contract.yml` change.

## Baseline procedure

1. Before refactor, run the escalated six-target order and copy the listed artifacts to a baseline dir
   (e.g. `readiness/behavior-preserving-baseline/`).
2. After refactor, re-run and `diff` against the baseline — zero differences for the artifacts above.
3. The full governance Expecto suite passes with no expected-output (golden) changes.

## What MAY change

- **Tier 2 only**: `validation.contract.yml` / `target-metadata.json` (new sub-targets + routing rules) —
  intentional, with diff rationale; `Route` output for doc-only diffs (the point of the feature).

## Independent shippability (SC-007)

Each tier passes its routed gates without the other tiers present:
- Tier 1 alone → byte-identical contract, six-target order green.
- Tier 2 alone → intentional contract diff, `Route` doc-only relaxation demonstrated, six-target green.
- Tier 3 alone → byte-identical artifacts/goldens, six-target green.

### Verification method (not an isolated triple gate run)

SC-007 is verified as a **design + ordering property**, *not* by checking out and
gate-running each tier in isolation. The tiers ship on one branch in priority
order (US1 → US2 → US3), so each tier's own checkpoint task establishes its
slice's contract posture at the moment it lands:

- **US1** — T014 proves `target-metadata.json` / `validation.contract.yml` are
  byte-identical to baseline (US2's contract change has not yet run).
- **US2** — T020/T021 produce the single intentional `validation.contract.yml`
  diff and the byte-identical umbrella evidence.
- **US3** — T025 proves byte-identical scan findings/goldens with no `.fsi` /
  contract change.

Because Tier 1 and Tier 3 touch **disjoint** files from Tier 2's contract
regeneration (typed keying + derived lists + scan-helper extraction vs. new
`Targets` cases + routing rules), reordering or dropping any single tier leaves
the other two byte-identical. T030 **records this argument** (the disjoint-file
rationale + the per-tier checkpoint evidence) in
`readiness/agent-ready-verdict.md`; it does not re-run an isolated single-tier
six-target pass.
