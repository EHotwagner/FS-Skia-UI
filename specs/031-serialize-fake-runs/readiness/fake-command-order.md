# FAKE Command Order

Status: focused guidance implementation complete; FAKE command execution pending.

Focused FAKE-backed validation commands must be recorded here in serialized
order. Aggregate `Verify` evidence is broad and does not replace ordered
focused logs.

Planned focused order:

1. `dotnet tool restore` (non-FAKE setup)
2. `./fake.sh build -t Dev`
3. `./fake.sh build -t GeneratedGuidanceCheck`
4. `./fake.sh build -t TemplateCheck`
5. `./fake.sh build -t GeneratedProductCheck`
6. `./fake.sh build -t EvidenceGraph`
7. `./fake.sh build -t EvidenceAudit`

Expected log paths:

| Order | Command | Expected log path | Authority |
|-------|---------|-------------------|-----------|
| 1 | `dotnet tool restore` | `readiness/logs/tool-restore.txt` | non-FAKE setup complete |
| 2 | `./fake.sh build -t Dev` | `readiness/logs/dev.txt` | focused complete |
| 3 | `./fake.sh build -t GeneratedGuidanceCheck` | `readiness/logs/generated-guidance-check.txt` and `readiness/generated-guidance.md` | focused complete |
| 4 | `./fake.sh build -t TemplateCheck` | `readiness/logs/template-check.txt` and `readiness/template/verdict.md` | focused generated-template complete |
| 5 | `./fake.sh build -t GeneratedProductCheck` | `readiness/logs/generated-product-check-final.txt` and `readiness/generated-product-validation.md` | focused generated-product complete after recorded failed attempts |
| 6 | `./fake.sh build -t EvidenceGraph` | `readiness/logs/evidence-graph.txt`, `readiness/task-graph.md`, and `.json` | focused graph complete |
| 7 | `./fake.sh build -t EvidenceAudit` | `readiness/logs/evidence-audit.txt`, `readiness/logs/evidence-audit-target-final.txt`, and `readiness/evidence-audit.md` | focused audit complete after readiness contract repairs |

Final notes:

- Generated validation authority: `GeneratedGuidanceCheck`,
  `TemplateCheck`, and `GeneratedProductCheck` all passed as public FAKE
  targets after repairs.
- Skill-loading workflow notes are recorded in
  `readiness/skill-loading-evidence.md`.
- Non-authoritative aggregate result: `Verify` was not run; focused command
  logs plus `EvidenceAudit` are the authoritative evidence for this feature.

Aggregate `Verify` remains broad validation and is recorded separately if run.
