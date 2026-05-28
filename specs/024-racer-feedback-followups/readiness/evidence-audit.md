# Evidence Audit Evidence

PASS: `EvidenceAudit` completed with synthetic propagation and diff-scan outputs present.

Final audit command after downstream skip status updates:

```bash
./fake.sh build -t EvidenceAudit
```

Evidence:

- `readiness/logs/evidence-audit.txt`
- `readiness/logs/t035-evidence-audit-final.txt`

Final audit facts:

- task status: 31 `[X]`, 1 `[S]`, 1 `[F]`, 3 `[-]`
- real tasks: 31
- accepted SEH tasks: 1
- unaccepted synthetic tasks: 0
- auto-synthetic tasks: 0
- late SEH tasks: 0
- readiness contract hits: 0
- blocking diff-scan hits: 0
- advisory diff-scan hits: 6 synthetic-banner disclosures in `tests/Testing.Tests/Tests.fs`

Accepted `[SEH]` evidence remains synthetic and is reported separately from
real task evidence. T020 remains failed because live screenshot proof could not
be collected; T032, T033, and T036 are skipped downstream of that failed
dependency.
