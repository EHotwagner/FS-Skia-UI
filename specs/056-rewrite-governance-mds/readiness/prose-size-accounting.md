# Prose-Size Accounting

Honest guidance-prose accounting against the corrected baseline (FR-007,
FR-008, SC-005). The discredited original over-estimate / "low hundreds"
figure is no longer the live target; tracking is against the baseline below.

- Corrected baseline (feature 046): 6882 lines
- `.agents/skills/**/*.md`: 3972 lines
- `.specify/**/*.md`: 2800 lines
- Current measured guidance-prose count: 6772 lines
- Delta vs baseline: -110 lines
- Restated target: lose no meaning, drop every word that earns nothing; no fixed
  line count, no discredited ~23,000 figure.

## Reproduction

```bash
find .agents/skills -name '*.md' | xargs wc -l | tail -1
find .specify       -name '*.md' | xargs wc -l | tail -1
```

## Notes

Reconciled against the T004 before-state (`readiness/logs/baseline-snapshot.md`):
pre-feature `.agents/skills`=4072, `.specify`=2817, summed `Current`=6889. The
rewrite tightened the corpus purely by removing redundancy, restatement, and
ceremony — no obligation, contract token, or rule was dropped — landing
`Current`=6772, a **117-line reduction** from the 6889 pre-feature count and
**110 below** the corrected ≈6,882 baseline (the prose freeze previously held it
at +7 above baseline; this feature swings it to −110).

The achieved reduction is modest because the corpus is unusually rule-dense:
the bulk of each file is load-bearing directives, verbatim contract tokens,
fenced F# code (untouchable), parity-critical grammar specs, and concrete file
paths. The tightening removed all genuine restatement between intros and recap
sections without dropping a single rule; reaching a larger reduction would have
required deleting rules or examples, which the contract (C5) forbids. SC-001 is
satisfied: `Current` is materially below 6889, achieved purely by tightening.

The byte-deterministic core of this report is produced by
`Guidance.renderProseSizeAccounting` (unit-tested in
`tests/Governance.Tests/ProseSizeAccountingTests.fs`); this file is its sole
writer for feature 056.
