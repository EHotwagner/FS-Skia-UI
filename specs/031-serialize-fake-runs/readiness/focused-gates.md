# Focused Gates Evidence

## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T14:14:50.6127241+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-29T14:18:34.4949313+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-29T14:21:22.0377966+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T14:23:04.0882808+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/generated-guidance.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `GeneratedGuidanceCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-05-29T14:27:36.8979872+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedProductCheck

- command: `./fake.sh build -t GeneratedProductCheck`
- direct-prerequisites: CapabilityCheck, SkillCheck
- timestamp-utc: `2026-05-29T14:29:16.1957595+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T14:30:08.3497800+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/task-graph.md`
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
- timestamp-utc: `2026-05-29T14:30:45.7184473+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/task-graph.md`
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
- timestamp-utc: `2026-05-29T14:31:06.3356040+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/task-graph.md`
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
- timestamp-utc: `2026-05-29T14:31:24.6305733+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/task-graph.md`
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
- timestamp-utc: `2026-05-29T14:31:25.3584727+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T14:32:27.3041802+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/task-graph.md`
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
- timestamp-utc: `2026-05-29T14:32:27.9589300+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/031-serialize-fake-runs/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


