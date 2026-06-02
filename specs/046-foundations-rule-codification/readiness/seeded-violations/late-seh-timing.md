# Seeded-violation proof — late-`[SEH]` design-phase-timing (Stage 6.1, shipped)

**Rule**: a `[SEH]` task whose `design-source` / `acceptance-status` carries a
"late"/"after audit failure"/"implementation-time" classification is rejected — `[SEH]`
must be a design-phase decision, never an implementation-time relabel
(`build/Governance/Evidence/Audit.fs:399-411`, `LateSehTasks`).
**Gate**: `EvidenceAudit` (`runAudit`).

Live-seeding this gate on *this* feature would require introducing a real `[SEH]` task
(this feature ships **zero** `[SEH]` tasks — see the Synthetic-Evidence Inventory). The
rule's blocking behaviour is instead proven by the deterministic in-process audit test
that drives the real `Engine.runAudit` over a seeded late-`[SEH]` fixture:

`tests/Governance.Tests/SyntheticErrorEvidenceTests.fs`
→ test **"EvidenceAudit Synthetic rejects late or non-eligible SEH classification"**:

```
Seed task:   - [S] T001 [US1] [SEH] synthetic-error-handling-approved ... placeholder output shortcut
Seed row:    design-source = "implementation readiness cleanup after audit failure" (late)
Assertions:  res.Verdict = Fail
             res.SehSummary.LateSehTasks length = 1
             SehAuditSummary contains "non-eligible synthetic evidence class"
```

Result: **Passed** (run `dotnet test tests/Governance.Tests --filter SyntheticError`). The
real audit engine genuinely rejects the late `[SEH]` classification. This Stage-6.1 gate
is **still blocking**; its prose may be trimmed under FR-008.
