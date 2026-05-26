# Focused Gates Evidence

## PackageSurfaceCheck

- command: `./fake.sh build -t PackageSurfaceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-26T20:03:53.4995728+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/package-surface-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/package-surfaces/index.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:tests/Package.Tests/Package.Tests.fsproj, requires-built-project:tests/Package.Tests/Package.Tests.fsproj
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `PackageSurfaceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## DependencyReport

- command: `./fake.sh build -t DependencyReport`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-26T20:05:39.6801918+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/dependency-report.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/dependency-report.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `DependencyReport`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-26T20:06:43.0246930+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-26T20:06:43.0319467+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-26T20:07:30.7364578+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-26T21:18:27.3951203+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-26T21:23:36.9120423+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-26T21:28:06.1230183+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-26T21:41:04.4708942+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-26T21:44:06.8897532+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-26T21:46:43.3596886+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-26T21:46:44.1846524+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-26T21:48:24.7042076+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-26T21:48:25.4364100+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-26T21:51:48.5973698+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-26T21:51:49.3038996+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-26T21:53:13.0765327+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-26T21:53:13.7976129+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-26T21:55:04.7902779+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-26T21:56:40.6914245+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/task-graph.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceGraph`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

## EvidenceAudit

- command: `./fake.sh build -t EvidenceAudit`
- direct-prerequisites: EvidenceGraph
- timestamp-utc: `2026-05-26T21:56:41.4228696+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/018-persistent-gui-runtime/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale

