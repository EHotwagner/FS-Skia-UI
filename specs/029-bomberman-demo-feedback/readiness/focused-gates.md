# Focused Gates Evidence

## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-29T10:00:27.9308100+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-29T10:02:45.5692932+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-29T10:10:47.6211204+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-29T10:19:05.3143627+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:19:07.2050730+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-29T10:19:37.5581395+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:20:54.9760769+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:20:58.5936964+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:23:19.0818138+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:23:19.2254292+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:23:21.3627890+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:23:21.3853999+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:23:23.5280146+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:23:25.1458813+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:23:26.8269257+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:23:28.4231854+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:23:53.8836777+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:23:55.3778614+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:25:35.4196318+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:25:35.5511650+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:25:37.6404909+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:25:37.6644946+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:25:39.7075305+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:25:41.3791470+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:25:42.9938195+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:25:44.6576298+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:26:10.9475654+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:26:12.3583027+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:26:56.6821610+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:26:56.8097338+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:26:58.9194396+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:26:58.9425529+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:27:01.1403690+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:27:02.7472139+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:27:04.2482455+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:27:05.7675978+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:27:29.9864020+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:27:31.4930210+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:27:48.9672418+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T10:27:49.6475020+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:29:34.0281443+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:29:34.1604364+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:29:36.2398875+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:29:36.2639259+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:29:38.3218088+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:29:39.9087113+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:29:41.4385842+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:29:42.9712495+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:30:06.6222260+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:30:08.0478720+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:30:25.8238990+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T10:30:26.5212815+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:32:25.6306057+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:32:25.7579404+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:32:27.7386959+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:32:27.7610953+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:32:29.7862461+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:32:31.3574239+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:32:32.9424932+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:32:34.5884363+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:32:59.5098632+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:33:00.9490855+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:33:18.5559966+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T10:33:19.3654855+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:35:28.9665212+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:35:29.0807053+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:35:31.0689435+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:35:31.0888877+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:35:33.2000307+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:35:34.8286840+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:35:36.3683312+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:35:37.9514499+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:36:01.9894139+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:36:03.4087230+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:36:20.3465307+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T10:36:20.9994594+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-29T10:38:53.6210439+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:40:19.5787044+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:40:19.6929423+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:40:22.0426710+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:40:22.0622652+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:40:24.3255007+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:40:26.1799152+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:40:28.1252646+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:40:30.1769704+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:40:57.4583218+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:40:58.8817069+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:41:16.1214979+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T10:41:16.7753481+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-29T10:43:47.9316240+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:46:54.7500968+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:46:54.8650717+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:46:56.9379016+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:46:56.9580408+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:46:59.0624195+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:47:00.5968849+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:47:02.1031104+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:47:03.6753178+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:47:41.8728914+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:47:43.3229008+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:48:00.9102865+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T10:48:01.5872515+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:50:22.0171542+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:50:22.1603054+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:50:24.4183507+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:50:24.4441717+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:50:26.6692456+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:50:28.2762627+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:50:29.8378607+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:50:31.4002034+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:51:08.1921075+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:51:09.5755706+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:51:26.7890519+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T10:51:27.4677950+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:53:24.3643541+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:53:24.4808103+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:53:26.5831200+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:53:26.6058986+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:53:28.6205698+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:53:30.1631790+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:53:31.6697810+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:53:33.1881284+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:54:09.2672243+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:54:10.6562012+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:54:27.9701085+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T10:54:28.6888171+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:56:23.7813541+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:56:23.9122707+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:56:25.8803093+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:56:25.9029729+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:56:27.9482283+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:56:29.5139173+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:56:31.0251472+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:56:32.5476372+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:57:08.4187515+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:57:09.8180416+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:57:26.8129541+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T10:57:27.4834110+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:58:54.5528299+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:58:54.6931311+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:58:56.6973918+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:58:56.7210119+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:58:58.7281778+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:59:00.2728417+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:59:01.7835277+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:59:03.2750296+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:59:39.6951437+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:59:41.1258035+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T10:59:58.1939566+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T10:59:58.8652440+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:01:16.5417934+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:01:16.6525253+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:01:18.6244670+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:01:18.6444263+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:01:20.6844207+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:01:22.2206386+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:01:23.7145125+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:01:25.2470900+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:02:13.8459562+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:02:15.2444981+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:02:32.5449354+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T11:02:33.2147296+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:05:34.4774887+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:05:34.5879778+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:05:36.6549886+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:05:36.6751410+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:05:38.6608029+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:05:40.1869560+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:05:41.7013906+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:05:43.2033227+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:06:32.6020940+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:06:33.9998026+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:06:51.0812284+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T11:06:51.7204751+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:08:38.2106012+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:08:38.3322552+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:08:40.3453635+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:08:40.3645674+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:08:42.4087532+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:08:43.9473497+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:08:45.4582983+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:08:46.9435546+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:10:34.2277643+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:10:34.3505993+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:10:36.3464628+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:10:36.3694826+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:10:38.3943084+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:10:39.9417138+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:10:41.4285827+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:10:42.9526223+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:15:35.1076071+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:15:36.7731779+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:15:40.4077510+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:15:41.9939935+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:15:45.5492416+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:15:48.6234125+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:15:51.7373186+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:15:54.8470292+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:17:07.2048818+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T11:17:31.1073135+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T11:17:33.6839771+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-29T11:20:17.3454096+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:02:29.4848922+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:02:34.9820361+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:02:35.1581019+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:02:37.2929528+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:02:37.3191042+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:02:39.4553669+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:02:41.3782938+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:02:43.0346811+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:02:44.7612915+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:03:41.0611402+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:03:41.2112928+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:03:43.8072515+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:03:43.8324151+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:03:45.8416632+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:03:47.4982478+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:03:49.0571203+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:03:50.6418117+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: Build
- timestamp-utc: `2026-05-29T12:11:04.5129439+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:11:09.4990836+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:11:09.6203708+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:11:11.5458205+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:11:11.5688050+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:11:13.4170367+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:11:14.9996587+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:11:16.5155804+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:11:18.0157070+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadata

- command: `./fake.sh build -t TargetMetadata`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:13:07.6166857+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadata.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadata`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:13:07.7342687+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:13:09.7493495+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:13:09.7718012+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:13:11.6944775+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:13:13.2567099+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:13:14.8253570+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:13:16.3902697+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TargetMetadataDrift

- command: `./fake.sh build -t TargetMetadataDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:14:08.6685419+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/TargetMetadataDrift.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TargetMetadataDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T12:14:09.6809213+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: Build
- timestamp-utc: `2026-05-29T12:15:12.7910054+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: Build
- timestamp-utc: `2026-05-29T12:15:15.7952927+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-29T12:15:56.2115233+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-29T12:17:36.5589767+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:18:03.7735129+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T12:18:13.9033299+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-29T12:18:14.9988377+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/029-bomberman-demo-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


