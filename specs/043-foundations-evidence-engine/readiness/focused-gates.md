# Focused Gates Evidence

## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-01T09:15:20.7633479+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-01T09:16:44.7399901+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/skill-sync-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/skill-sync-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SkillSyncCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## SkillExamplesCheck

- command: `./fake.sh build -t SkillExamplesCheck`
- direct-prerequisites: SkillSyncCheck
- timestamp-utc: `2026-06-01T09:17:26.7706645+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/skill-examples-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/skill-examples-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:build/SkillExamples/SkillExamples.fsproj
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SkillExamplesCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## EvidenceGraph

- command: `./fake.sh build -t EvidenceGraph`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-01T09:20:25.4431574+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-01T09:22:38.7993709+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-01T09:23:19.7383477+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-01T09:23:19.8872916+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/evidence-audit.md`
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
- timestamp-utc: `2026-06-01T09:27:02.2860246+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-01T09:27:02.4508531+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## SkillSyncCheck

- command: `./fake.sh build -t SkillSyncCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-01T10:03:34.5491416+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/skill-sync-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/skill-sync-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SkillSyncCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## SkillExamplesCheck

- command: `./fake.sh build -t SkillExamplesCheck`
- direct-prerequisites: SkillSyncCheck
- timestamp-utc: `2026-06-01T10:05:17.5847859+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/skill-examples-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/skill-examples-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:build/SkillExamples/SkillExamples.fsproj
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SkillExamplesCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## SkillSyncCheck

- command: `./fake.sh build -t SkillSyncCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-01T10:08:14.7971969+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/skill-sync-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/skill-sync-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SkillSyncCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## SkillExamplesCheck

- command: `./fake.sh build -t SkillExamplesCheck`
- direct-prerequisites: SkillSyncCheck
- timestamp-utc: `2026-06-01T10:09:01.6944535+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/skill-examples-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/skill-examples-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:build/SkillExamples/SkillExamples.fsproj
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SkillExamplesCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-06-01T10:10:16.2993693+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/template/verdict.md`
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
- timestamp-utc: `2026-06-01T10:11:51.5554245+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/generated-file-lists/summary.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `GeneratedProductCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-06-01T10:15:48.3584750+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## SkillSyncCheck

- command: `./fake.sh build -t SkillSyncCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-01T10:24:42.2036476+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/skill-sync-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/skill-sync-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SkillSyncCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## SkillExamplesCheck

- command: `./fake.sh build -t SkillExamplesCheck`
- direct-prerequisites: SkillSyncCheck
- timestamp-utc: `2026-06-01T10:25:18.0559060+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/skill-examples-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/skill-examples-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:build/SkillExamples/SkillExamples.fsproj
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SkillExamplesCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## GeneratedGuidanceCheck

- command: `./fake.sh build -t GeneratedGuidanceCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-01T10:26:42.8662116+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/generated-guidance.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/generated-guidance.md`
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
- timestamp-utc: `2026-06-01T10:29:12.5889857+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/template/verdict.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `TemplateCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## SkillSyncCheck

- command: `./fake.sh build -t SkillSyncCheck`
- direct-prerequisites: (none)
- timestamp-utc: `2026-06-01T10:30:01.6217671+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/skill-sync-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/skill-sync-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SkillSyncCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## SkillExamplesCheck

- command: `./fake.sh build -t SkillExamplesCheck`
- direct-prerequisites: SkillSyncCheck
- timestamp-utc: `2026-06-01T10:30:42.8652365+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/skill-examples-check.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/skill-examples-check.md`
- verdict-category: `success`
- stale-build-restore-assumptions: requires-restored-project:build/SkillExamples/SkillExamples.fsproj
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `SkillExamplesCheck`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


## TemplateCheck

- command: `./fake.sh build -t TemplateCheck`
- direct-prerequisites: TemplatePack, TemplateInstallSource, TemplateInstallPackage, TemplateInstantiate, TemplateSmoke
- timestamp-utc: `2026-06-01T10:31:43.6726708+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/template/verdict.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/template/verdict.md`
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
- timestamp-utc: `2026-06-01T10:33:27.8587970+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/generated-file-lists/summary.md`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/generated-file-lists/summary.md`
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
- timestamp-utc: `2026-06-01T10:33:44.4974902+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-01T10:33:58.3686240+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-01T10:33:58.7107492+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/evidence-audit.md`
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
- timestamp-utc: `2026-06-01T10:34:45.5485004+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-01T10:34:56.9254341+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/evidence-graph.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/task-graph.md`
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
- timestamp-utc: `2026-06-01T10:34:57.1858129+00:00`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/logs/evidence-audit.txt`
- readiness-path: `/home/developer/projects/FS-Skia-UI/specs/043-foundations-evidence-engine/readiness/evidence-audit.md`
- verdict-category: `success`
- stale-build-restore-assumptions: (none)
- failure-rule: `stale-build-restore-assumption`
- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active
- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists
- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared
- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure
- affected-gate: `EvidenceAudit`
- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale


