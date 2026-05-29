# Evidence Graph Invocation

Status: complete.

Task: T022
Captured: 2026-05-29T12:08:00+02:00

## Generated Checkout

Path: `artifacts/template-check/029-bomberman-demo-feedback/source-app`

Fresh generation command:

```text
./fake.sh build -t TemplateCheck
```

Result: pass. Log: `readiness/templatecheck-us1-rerun.log`

## EvidenceGraph Command

Command run from generated checkout:

```text
./fake.sh build -t EvidenceGraph
```

Result:

- Exit code: 0
- Log: `readiness/generated-evidencegraph-run.log`
- Generated report: `artifacts/template-check/029-bomberman-demo-feedback/source-app/readiness/evidence-graph.md`
- Report status: `status=ok`
- Authority: `authority=delegated-authoritative`
- Validation area: `validation-area=task-graph`
- Script invocation: generated `build.fsx` uses `ProcessStartInfo("bash", arguments)` and passes `.specify/extensions/evidence/scripts/bash/run-audit.sh ... --graph-only` as arguments.

No executable-mode repair or `chmod` step was used.
