# Governance Risk Levels

- recorded_at: `2026-05-28T15:47:57+02:00`
- selected_level: `medium`
- rationale: This feature changes generated command behavior, evidence scripts, template output, generated guidance, and root target aggregation, but does not add a new public `.fsi` library API by default.
- small: documentation-only or local readiness-only edits may use `./fake.sh build -t EvidenceGraph` plus the directly affected text check.
- medium: after each story, run `dotnet test tests/Governance.Tests/Governance.Tests.fsproj`, `./fake.sh build -t GeneratedGuidanceCheck`, and `./fake.sh build -t TemplateCheck`; record command output paths and failures separately from authoritative graph/audit verdicts.
- broad: run `./fake.sh build -t Verify` when graph/audit commands, template output, evidence scripts, or root target aggregation changes.
- authoritative_gates: `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`.
- non_authoritative_aggregate: If an aggregate command hangs or fails outside the authoritative target under review, capture the log, classify it as non-authoritative aggregate evidence, and run the focused target named by the failure.
- required evidence: story-specific governance/template checks plus the readiness logs named by each completed task.
- broad validation: required when generated graph/audit commands, template output, evidence scripts, or root target aggregation changes.
