# Quickstart: Generated Evidence Workflow Authority

## Preconditions

- Current branch is `027-generated-evidence-workflow`.
- Feature directory is `specs/027-generated-evidence-workflow`.
- Generated product template assets are available under `template/base`.

## Failing-First Checks

1. Add governance tests that demonstrate generated `EvidenceGraph` and `EvidenceAudit` fail on malformed generated evidence packages instead of writing completion-only logs.
2. Add tests or fixtures proving collapsed skill-loading rows are rejected and one row per `(task id, skill id)` is required.
3. Add audit diagnostic tests where readiness files exist but omit required terms, and assert missing terms appear in output.
4. Add generated guidance tests for message qualification, vector-to-point conversion, semantic scene evidence, and screenshot/fallback vocabulary.

## Implementation Path

1. Update generated project evidence targets in `template/base/build.fsx` to use authoritative graph/audit validation or clearly delegated authoritative validation.
2. Update root command contracts in `build.fsx` only where target dependencies, readiness paths, or aggregation need to reflect the stronger generated behavior.
3. Extend `.specify/extensions/evidence/scripts/python/compute-task-graph.py` or adjacent helpers to generate/validate skill-loading rows from `tasks.deps.yml`.
4. Extend `.specify/extensions/evidence/scripts/bash/run-audit.sh` diagnostics so missing readiness terms are printed and persisted.
5. Update generated docs and fragments with the required FS.Skia.UI guidance.
6. Update governance and generated product tests until the failing-first checks pass.
7. Produce readiness evidence under `specs/027-generated-evidence-workflow/readiness/`.

## Verification Commands

```bash
dotnet test tests/Governance.Tests/Governance.Tests.fsproj
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
./fake.sh build -t Verify
```

## Required Readiness Outputs

- `readiness/generated-validation-authority.md`: commands proving generated graph/audit authority and rejection behavior.
- `readiness/skill-loading-evidence-workflow.md`: generated row coverage and malformed-row rejection evidence.
- `readiness/audit-diagnostics.md`: missing file/term diagnostic evidence.
- `readiness/readiness-contract-discovery.md`: task or placeholder discovery evidence.
- `readiness/framework-guidance.md`: generated guidance coverage evidence.
- `readiness/evidence-vocabulary.md`: screenshot/fallback vocabulary validation.
- `readiness/evidence-graph.md`: graph validation output.
- `readiness/evidence-audit.md`: final audit output.

## Completion Criteria

- Generated evidence commands cannot pass without authoritative validation.
- Skill-loading evidence coverage is generated or validated per row.
- Audit readiness diagnostics name missing files and missing terms.
- Generated guidance includes all required FS.Skia.UI evidence-safe patterns.
- Normal generated interactive launch remains separate from evidence commands.
