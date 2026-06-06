# Focused Gates Evidence

## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-06T19:37:43.5063673+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-06T19:37:51.6899577+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-06T19:41:00.0631798+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/logs/symbol-cross-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/symbol-cross-check.md`
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
- timestamp-utc: `2026-06-06T19:49:56.8951002+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-06T19:49:59.2225005+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/logs/symbol-cross-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/symbol-cross-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SymbolCrossCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: Build
- timestamp-utc: `2026-06-06T20:21:21.6019739+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## PerPackageSurfaceDiff

- command: `./fake.sh build -t PerPackageSurfaceDiff`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-06T20:22:40.8840554+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/logs/PerPackageSurfaceDiff.txt`
- readiness-path: `(none)`
- verdict-category: `degraded`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `PerPackageSurfaceDiff`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## FsiTranscripts

- command: `./fake.sh build -t FsiTranscripts`
- direct-prerequisites: Build
- timestamp-utc: `2026-06-06T20:24:08.6855143+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/fsi/prelude.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/fsi`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `FsiTranscripts`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## SkillSyncCheck

- command: `./fake.sh build -t SkillSyncCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-06T20:29:58.6176685+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/logs/skill-sync-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/skill-sync-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SkillSyncCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-06T20:32:00.9478363+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateDrift

- command: `./fake.sh build -t TemplateDrift`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-06T20:32:05.3310858+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/logs/template-drift.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/template-drift.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `TemplateDrift`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-06T20:32:29.7046036+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-06T20:32:32.1489634+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-06T20:32:32.2517287+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/073-add-animations/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


