# Graph-Only Output Label

Commands:

- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "Task validator feedback follow-ups|V3 local skill validation|Synthetic error evidence governance|Generated project validation contract" -m:1 --disable-build-servers`
- `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/033-fix-task-validator-feedback --graph-only`

Result:

- PASS: 50 focused tests passed, 0 failed.
- PASS: graph-only command output starts with `speckit.evidence.graph (graph validation only)`.
- PASS: graph-only command output says `Run EvidenceAudit for full merge-gate validation`.
- PASS: generated evidence reports label `EvidenceGraph` as `graph-validation-only` and direct reviewers to `EvidenceAudit`.

Current feature graph-only output:

```text
=== speckit.evidence.graph (graph validation only) ===
[graph validation] Computing task graph only; merge-gate diff scan remains in EvidenceAudit.
graph validation PASS; graph-only mode skipped diff scan and synthetic merge-gate checks.
Run EvidenceAudit for full merge-gate validation.
```
