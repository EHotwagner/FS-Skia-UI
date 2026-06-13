# Focused Gates Evidence

## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T10:06:34.2759115+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## SymbolCrossCheck

- command: `./fake.sh build -t SymbolCrossCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T10:08:19.9505320+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/symbol-cross-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/symbol-cross-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SymbolCrossCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T10:12:56.6312317+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## SkillSyncCheck

- command: `./fake.sh build -t SkillSyncCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T10:52:56.2382041+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/skill-sync-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/skill-sync-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SkillSyncCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PerPackageSurfaceDiff

- command: `./fake.sh build -t PerPackageSurfaceDiff`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:03:39.4440661+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/PerPackageSurfaceDiff.txt`
- readiness-path: `(none)`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `PerPackageSurfaceDiff`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsDocCoverageCheck

- command: `./fake.sh build -t ControlsDocCoverageCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:03:41.9394796+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/ControlsDocCoverageCheck.txt`
- readiness-path: `(none)`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `ControlsDocCoverageCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: Build
- timestamp-utc: `2026-06-13T11:08:19.5461470+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: Build
- timestamp-utc: `2026-06-13T11:10:16.2198284+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogCheck

- command: `./fake.sh build -t ControlsCatalogCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:10:25.5822595+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/controls-catalog-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/control-catalog.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `ControlsCatalogCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsCatalogGenerationCheck

- command: `./fake.sh build -t ControlsCatalogGenerationCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:10:28.2542922+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/controls-catalog-generation-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/control-catalog-generation.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `ControlsCatalogGenerationCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## DesignTokenDrift

- command: `./fake.sh build -t DesignTokenDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:10:30.9786719+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/DesignTokenDrift.txt`
- readiness-path: `(none)`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `DesignTokenDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ContrastCheck

- command: `./fake.sh build -t ContrastCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:10:33.8877028+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/ContrastCheck.txt`
- readiness-path: `(none)`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `ContrastCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsInteractionCheck

- command: `./fake.sh build -t ControlsInteractionCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:10:42.9546902+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/controls-interaction-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/interaction-tests.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `ControlsInteractionCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## ControlsRenderingCheck

- command: `./fake.sh build -t ControlsRenderingCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:10:52.2657521+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/controls-rendering-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/layout-rendering.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj, requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `ControlsRenderingCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:10:56.8555836+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:10:59.3784097+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductStructure

- command: `./fake.sh build -t GeneratedProductStructure`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:12:31.0545152+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/GeneratedProductStructure.txt`
- readiness-path: `(none)`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `GeneratedProductStructure`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## SkillSyncCheck

- command: `./fake.sh build -t SkillSyncCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:13:17.1288912+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/skill-sync-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/skill-sync-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SkillSyncCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedConsumerValidation

- command: `./fake.sh build -t GeneratedConsumerValidation`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:13:36.5744556+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/GeneratedConsumerValidation.txt`
- readiness-path: `(none)`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `GeneratedConsumerValidation`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:16:42.6210333+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:16:50.5085891+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-13T11:17:24.2926137+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-06-13T11:17:24.4354639+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-06-13T11:34:43.9295072+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


