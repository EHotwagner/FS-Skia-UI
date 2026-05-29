# Sequential FAKE Validation

Status: focused repository validation started.

This artifact records FAKE-backed commands one at a time because repository
FAKE targets share `.fake` state and are not safe to run concurrently.

| Order | Command | Working directory | Purpose | Start | End | Exit code | Log path |
|-------|---------|-------------------|---------|-------|-----|-----------|----------|
| 0 | `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --no-restore --logger "console;verbosity=minimal"` | `/home/developer/projects/FS-Skia-UI` | Non-FAKE focused governance test compile and semantic scanner evidence | before FAKE validation | before FAKE validation | 0 | terminal output |
| 1 | `dotnet tool restore` | `/home/developer/projects/FS-Skia-UI` | Non-FAKE setup before FAKE-backed validation | 2026-05-29T14:11:04Z | 2026-05-29T14:11:05Z | 0 | `specs/031-serialize-fake-runs/readiness/logs/tool-restore.txt` |
| 2 | `./fake.sh build -t Dev` | `/home/developer/projects/FS-Skia-UI` | First focused FAKE-backed repository validation command | 2026-05-29T14:11:24Z | 2026-05-29T14:14:26Z | 0 | `specs/031-serialize-fake-runs/readiness/logs/dev.txt` |
| 3 | `./fake.sh build -t GeneratedGuidanceCheck` | `/home/developer/projects/FS-Skia-UI` | Generated guidance scanner validation | 2026-05-29T14:14:48Z | 2026-05-29T14:14:50Z | 0 | `specs/031-serialize-fake-runs/readiness/logs/generated-guidance-check.txt`; report `readiness/generated-guidance.md` |
| 4 | `./fake.sh build -t TemplateCheck` | `/home/developer/projects/FS-Skia-UI` | Template source/package guidance validation | 2026-05-29T14:16:10Z | 2026-05-29T14:18:34Z | 0 | `specs/031-serialize-fake-runs/readiness/logs/template-check.txt` |
| 5 | `./fake.sh build -t GeneratedProductCheck` | `/home/developer/projects/FS-Skia-UI` | Generated product validation, first attempt | 2026-05-29T14:19:09Z | 2026-05-29T14:21:37Z | 1 | `specs/031-serialize-fake-runs/readiness/logs/generated-product-check.txt`; failed on generated app audit identifiers |
| 6 | `./fake.sh build -t GeneratedGuidanceCheck` | `/home/developer/projects/FS-Skia-UI` | Scanner rerun after identifier repair | 2026-05-29T14:23:02Z | 2026-05-29T14:23:04Z | 0 | `specs/031-serialize-fake-runs/readiness/logs/generated-guidance-check-rerun.txt` |
| 7 | `./fake.sh build -t GeneratedProductCheck` | `/home/developer/projects/FS-Skia-UI` | Generated product validation, environment-aborted attempt | 2026-05-29T14:23:10Z | 2026-05-29T14:24:53Z | 1 | `specs/031-serialize-fake-runs/readiness/logs/generated-product-check-rerun.txt`; `SkiaViewer.Tests` host aborted on `libdecor-gtk.so` |
| 8 | `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --no-build --no-restore --logger "console;verbosity=minimal" -- --sequenced` | `/home/developer/projects/FS-Skia-UI` | Non-FAKE direct check for environment-aborted prerequisite | after row 7 | before row 9 | 0 | terminal output |
| 9 | `./fake.sh build -t GeneratedProductCheck` | `/home/developer/projects/FS-Skia-UI` | Generated product validation, final successful attempt | 2026-05-29T14:25:23Z | 2026-05-29T14:29:16Z | 0 | `specs/031-serialize-fake-runs/readiness/logs/generated-product-check-final.txt` |
| 10 | `./fake.sh build -t EvidenceGraph` | `/home/developer/projects/FS-Skia-UI` | Task graph and synthetic propagation refresh | 2026-05-29T14:30:06Z | 2026-05-29T14:30:08Z | 0 | `specs/031-serialize-fake-runs/readiness/logs/evidence-graph.txt`; report `readiness/evidence-graph.md` |
| 11 | `./fake.sh build -t EvidenceAudit` | `/home/developer/projects/FS-Skia-UI` | Evidence audit first attempt | 2026-05-29T14:30:43Z | 2026-05-29T14:30:46Z | 1 | `specs/031-serialize-fake-runs/readiness/logs/evidence-audit-target.txt`; readiness contract fields missing |
| 12 | `./fake.sh build -t EvidenceAudit` | `/home/developer/projects/FS-Skia-UI` | Evidence audit second attempt | 2026-05-29T14:31:04Z | 2026-05-29T14:31:07Z | 1 | `specs/031-serialize-fake-runs/readiness/logs/evidence-audit-target-rerun.txt`; runtime limitation exact term missing |
| 13 | `./fake.sh build -t EvidenceAudit` | `/home/developer/projects/FS-Skia-UI` | Evidence audit final successful attempt | 2026-05-29T14:31:22Z | 2026-05-29T14:31:25Z | 0 | `specs/031-serialize-fake-runs/readiness/logs/evidence-audit-target-final.txt`; report `readiness/evidence-audit.md` |
| 14 | `./fake.sh build -t EvidenceAudit` | `/home/developer/projects/FS-Skia-UI` | Post-status final audit after T033/T034 updates | 2026-05-29T14:32:25Z | 2026-05-29T14:32:28Z | 0 | `specs/031-serialize-fake-runs/readiness/logs/evidence-audit-target-post-status.txt`; report `readiness/evidence-audit.md` |

Failure triage:

| Failed command | Concurrent FAKE context | `.fake` race status | Required rerun order | Follow-up classification |
|----------------|-------------------------|---------------------|----------------------|--------------------------|
| `./fake.sh build -t GeneratedProductCheck` row 5 | none known | not-suspected | `GeneratedGuidanceCheck`, then `GeneratedProductCheck` | Product governance identifier repair completed; final rerun passed |
| `./fake.sh build -t GeneratedProductCheck` row 7 | none known | not-suspected | direct `SkiaViewer.Tests`, then `GeneratedProductCheck` | Environment/native host transient; direct test and final rerun passed |
| `./fake.sh build -t EvidenceAudit` rows 11-12 | none known | not-suspected | update readiness fields, then `EvidenceAudit` | Readiness contract repairs completed; final audit passed |
