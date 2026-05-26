# Quickstart: Synthetic Error Evidence

## Validate Planning Outputs

Review the design artifacts:

```bash
sed -n '1,220p' specs/017-synthetic-error-evidence/plan.md
sed -n '1,220p' specs/017-synthetic-error-evidence/data-model.md
sed -n '1,220p' specs/017-synthetic-error-evidence/contracts/synthetic-error-evidence-contract.md
sed -n '1,220p' specs/017-synthetic-error-evidence/contracts/evidence-audit-contract.md
```

## Implement Guidance Changes First

Update governance text before audit implementation:

```bash
rg -n "Synthetic evidence|\\[S\\]|EvidenceAudit|accept-synthetic" .specify docs tests
```

Expected changes include:

- `.specify/memory/constitution.md`
- `.specify/templates/tasks-template.md`
- `.specify/presets/fsharp-opinionated/templates/tasks-template.md`
- `.specify/presets/fsharp-opinionated/commands/speckit.tasks.md`
- `.specify/presets/fsharp-opinionated/commands/speckit.implement.md`
- `docs/evidence.md`

## Add Failing Fixtures

Create or update governance fixtures that cover:

- valid design-approved `[SEH]` task graph returns PASS
- ordinary `[S]` task remains FAIL
- late `[SEH]` classification remains FAIL
- non-eligible synthetic fixture remains FAIL
- missing rationale, label, or design source remains FAIL

## Run Focused Governance Checks

```bash
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Capture readiness artifacts:

- `specs/017-synthetic-error-evidence/readiness/seh-classification-rules.md`
- `specs/017-synthetic-error-evidence/readiness/task-generation-seh.md`
- `specs/017-synthetic-error-evidence/readiness/audit-accepted-seh.md`
- `specs/017-synthetic-error-evidence/readiness/audit-rejects-late-seh.md`
- `specs/017-synthetic-error-evidence/readiness/non-eligible-synthetic-cases.md`
- `specs/017-synthetic-error-evidence/readiness/generated-guidance-check.md`
- `specs/017-synthetic-error-evidence/readiness/evidence-graph.md`
- `specs/017-synthetic-error-evidence/readiness/evidence-audit.md`

## Final Verification

```bash
./fake.sh build -t Dev
./fake.sh build -t Verify
```

If the accepted `[SEH]` fixture is the only synthetic evidence in its graph, `EvidenceAudit` must report PASS and show accepted synthetic counts. Any ordinary `[S]`, `[S*]`, late `[SEH]`, or non-eligible synthetic case must still report FAIL.
