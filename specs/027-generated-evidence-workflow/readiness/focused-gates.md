# Focused Gates Evidence

## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T13:33:26.8075772+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T13:36:05.8480434+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T13:36:51.1747927+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:17:09.9086285+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-28T14:17:44.5649711+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## T036 Focused Validation

- captured_at: `2026-05-28T16:17:00+02:00`
- governance_tests:
  - command: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj -m:1`
  - log_path: `specs/027-generated-evidence-workflow/readiness/logs/t036-governance-tests.txt`
  - result: `Passed! - Failed: 0, Passed: 183`
- generated_guidance_check:
  - command: `./fake.sh build -t GeneratedGuidanceCheck`
  - log_path: `specs/027-generated-evidence-workflow/readiness/logs/t036-generated-guidance-check.txt`
  - result: exit `0`
- template_check:
  - command: `./fake.sh build -t TemplateCheck`
  - log_path: `specs/027-generated-evidence-workflow/readiness/logs/t036-template-check.txt`
  - result: exit `0`
- non_authoritative_aggregate_failures: none
## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:18:11.7536509+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:18:11.7922989+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-28T14:18:12.4936016+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:21:55.3974408+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:22:05.4723469+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-28T14:23:18.8456003+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-28T14:26:56.8019052+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:28:25.9852175+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:28:36.5808900+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-28T14:29:38.6209196+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-28T14:31:38.4620051+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:31:40.1335678+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:31:41.7435378+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:31:43.3716580+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:31:45.5084658+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:31:45.5307874+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:31:47.7740453+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T14:31:47.8694592+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-28T14:31:48.5677880+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/027-generated-evidence-workflow/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


