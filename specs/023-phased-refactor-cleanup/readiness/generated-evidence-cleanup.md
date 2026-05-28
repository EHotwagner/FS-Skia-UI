# Generated Evidence Cleanup

Status: pending phase evidence.

## Batch Evidence Log

| Task | Command | Exit code | Risk | Changed ownership area | Pre-existing failure attribution | Verdict |
|------|---------|-----------|------|------------------------|----------------------------------|---------|
| T010 | `git status --short` | 0 | medium | generated evidence tests | none | PASS |
| T010 | `dotnet test tests/Testing.Tests/Testing.Tests.fsproj` | 0 | medium | generated evidence tests | none | PASS, 28 passed before generated report consolidation. |
| T010 | `./fake.sh build -t TemplateCheck` | 0 | medium | generated evidence tests | none | PASS before generated report consolidation. |
| T010-T011 | `./fake.sh build -t TemplateCheck` | 0 | medium | generated product evidence/layout tests | none | PASS after tightening generated report and readable layout expectations. |
| T010-T011 | `dotnet test tests/Testing.Tests/Testing.Tests.fsproj` | 0 | medium | generated product evidence/layout tests | none | PASS after tightening generated evidence expectations. |
| T012-T014 | `./fake.sh build -t TemplateCheck` | 0 | medium | generated product evidence implementation | none | PASS after consolidating generated evidence line writing. |
| T012-T014 | `dotnet test tests/Testing.Tests/Testing.Tests.fsproj` | 0 | medium | generated product evidence implementation | none | PASS after consolidating generated evidence line writing. |
| T015 | `dotnet test tests/Testing.Tests/Testing.Tests.fsproj` | 0 | medium | generated product evidence final verification | none | PASS, 28 passed. |
| T015 | `./fake.sh build -t TemplateCheck` | 0 | medium | generated product evidence final verification | none | PASS. |
| T015 | `./fake.sh build -t GeneratedGuidanceCheck` | 0 | medium | generated product evidence final verification | none | PASS. |

## Test Tightening

- T010 tightened `template/base/tests/Product.Tests/Tests.fs` so generated
  evidence commands explicitly preserve normalized status vocabulary,
  unsupported/failure exit-code meanings, report parent-directory writing,
  stdout echo behavior, command fields, output fields, and command names.
- T011 tightened generated game layout evidence checks for measurement mode and
  absence of unsupported-host classifications when readable layout proof is
  present.
- T012 introduced `writeGeneratedEvidenceLines` as the single local
  parent-directory/write/stdout/exit-code path.
- T013 kept layout/image/screenshot/visual evidence on `writeEvidenceReport`
  while preserving layout proof fields and unsupported classifications.
- T014 removed specialized launch evidence report functions and routed launch,
  bounded smoke, and window option file writes through the shared generated
  line writer without adding fields to those schemas.

## US1 Verdict

PASS. Generated evidence command names, report fields, unsupported
classifications, layout readability fields, stdout echo behavior, parent
directory creation, and exit-code meanings are preserved by focused tests and
phase FAKE checks.
