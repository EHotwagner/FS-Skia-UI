# Evidence Audit

`./fake.sh build -t EvidenceAudit` runs the task graph computation and diff scan.

Final result:

- command: `./fake.sh build -t EvidenceAudit`
- verdict: PASS
- declared synthetic tasks: 0
- propagated synthetic tasks: 0
- blocking diff-scan hits: 0
- advisory diff-scan hits: 0
- T108 explicitly skipped for external human-evaluator evidence

The detailed command transcript is written to
`readiness/logs/evidence-audit.txt`, and diff-scan details are written to
`readiness/diff-scan-hits.json`.
