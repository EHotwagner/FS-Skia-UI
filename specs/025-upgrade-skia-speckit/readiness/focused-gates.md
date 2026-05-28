# Focused Gates Evidence

## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T09:54:14.2302841+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T09:54:22.5801378+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-28T09:54:56.6057423+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T09:55:04.1153652+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:06:38.2783227+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:06:48.4315813+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-28T10:08:19.2933331+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:09:13.8991086+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:09:15.1794491+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-28T10:09:43.6248501+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:10:29.6848912+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:14:33.2537189+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:18:55.7357327+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:19:05.8711563+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-28T10:20:04.6878613+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-28T10:21:52.6165387+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:21:54.1198761+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:21:55.7156446+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:21:57.2517738+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:21:59.3333956+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:21:59.3526359+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:22:01.5180353+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:22:01.5989937+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:26:44.8542895+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:30:03.3924965+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:30:13.0751791+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-28T10:31:11.5015826+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-28T10:32:57.9539582+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:32:59.4552812+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:33:01.0460451+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:33:02.5989361+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:33:04.5909404+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:33:04.6106748+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:33:06.6735269+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:33:06.7563275+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:35:24.0823013+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:35:33.7962366+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-28T10:36:35.0428680+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-28T10:38:22.1099569+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:38:23.6340276+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:38:25.1698976+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:38:26.7843803+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:38:28.8180224+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:38:28.8376947+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:38:30.9849154+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:38:31.0706892+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-28T10:38:31.7449848+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:39:16.5534643+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-28T10:39:17.2025331+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T10:40:26.6412490+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-28T10:40:27.2946583+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/025-upgrade-skia-speckit/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


