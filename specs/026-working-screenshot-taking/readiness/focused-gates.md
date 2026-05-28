# Focused Gates Evidence

## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:07:37.4859827+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:07:38.9543936+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-28T12:10:02.5631994+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-28T12:13:36.0886664+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:16:13.3805009+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:16:23.0994124+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-28T12:17:33.6820987+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-28T12:19:18.9188210+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:19:20.4520759+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:19:22.0333730+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:19:23.6354794+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:19:25.6429265+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:19:25.6616150+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:20:18.2479937+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:21:42.4296920+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:21:51.9011678+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-28T12:22:55.7903664+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-28T12:24:48.3152170+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:24:49.9228590+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:24:51.4689324+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:24:53.0248672+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:24:55.0861506+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:24:55.1063525+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:24:57.3730206+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-28T12:24:57.4831059+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-28T12:24:58.1982110+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/026-working-screenshot-taking/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


