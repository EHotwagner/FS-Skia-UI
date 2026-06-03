# Prose-Size Accounting

Honest guidance-prose accounting against the corrected baseline (FR-007,
FR-008, SC-005). The discredited original over-estimate / "low hundreds"
figure is no longer the live target; tracking is against the baseline below.

- Corrected baseline (feature 046): 6882 lines
- `.agents/skills/**/*.md`: 4072 lines
- `.specify/**/*.md`: 2817 lines
- Current measured guidance-prose count: 6889 lines
- Delta vs baseline: +7 lines
- Restated target: no fixed line count is mandated by this feature; the freeze is
  lifted and tracking is against the corrected ≈6,882 baseline, with the actual
  large-scale prose reduction recorded as a bounded follow-up.

## Reproduction

```bash
find .agents/skills -name '*.md' | xargs wc -l | tail -1
find .specify       -name '*.md' | xargs wc -l | tail -1
```

## Notes

The +7 delta reflects the corrected ≈6,882 baseline established by feature 046,
not the discredited original figure. This feature lifts the prose freeze (the
literal-substring table no longer pins author prose) so the corpus *can* now
shrink; it deliberately does not mandate a final line count (see spec
Assumptions). The byte-deterministic render is produced by
`Guidance.renderProseSizeAccounting` and unit-tested in
`tests/Governance.Tests/ProseSizeAccountingTests.fs`.
