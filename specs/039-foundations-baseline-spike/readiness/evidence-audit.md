# Evidence Audit Readiness — 039

| Field | Value |
|---|---|
| Authoritative command | `./fake.sh build -t EvidenceAudit` (reads `.specify/feature.json` → 039) |
| Artifact path | `specs/039-foundations-baseline-spike/readiness/logs/evidence-audit.txt` + `seh-audit-summary.json` |
| Failure class | synthetic-propagation / diff-scan merge-gate |
| Next action on failure | Resolve the named synthetic/diff-scan hit; never weaken an assertion or `--accept-synthetic` without recorded justification |

## Notes

This feature anticipates **no synthetic evidence**: baseline counts are real
`wc`/`git` measurements, golden fixtures are real outputs of the existing
evidence engine, and the spike runs a real compiled target. The expected audit
verdict is **PASS** with:

- `accepted-seh-tasks=0`
- `unaccepted-synthetic-tasks=0`
- `auto-synthetic-tasks=0`
- `late-seh-tasks=0`

Any synthetic propagation or blocking diff-scan hit must be resolved, not
suppressed.
