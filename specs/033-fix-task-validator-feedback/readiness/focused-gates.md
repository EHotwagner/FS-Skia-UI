# Focused Gates Evidence

## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-05-29T20:53:53.1884583+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/generated-guidance.md`
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
- timestamp-utc: `2026-05-29T20:56:18.3601067+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/template/verdict.md`
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
- timestamp-utc: `2026-05-29T21:00:44.2793506+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/template/verdict.md`
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
- timestamp-utc: `2026-05-29T21:02:25.3555507+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/generated-file-lists/summary.md`
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
- timestamp-utc: `2026-05-29T21:02:36.2152815+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/task-graph.md`
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
- timestamp-utc: `2026-05-29T21:02:49.4953046+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/task-graph.md`
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
- timestamp-utc: `2026-05-29T21:03:16.8072058+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/task-graph.md`
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
- timestamp-utc: `2026-05-29T21:03:17.5047906+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/evidence-audit.md`
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
- timestamp-utc: `2026-05-29T21:04:02.5901826+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/task-graph.md`
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
- timestamp-utc: `2026-05-29T21:04:03.2963741+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/033-fix-task-validator-feedback/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


