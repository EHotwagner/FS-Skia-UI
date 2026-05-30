# Title Trigger Validation

Command:

`dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "Task validator feedback follow-ups|V3 local skill validation|Synthetic error evidence governance|Generated project validation contract" -m:1 --disable-build-servers`

Result:

- PASS: 50 tests passed, 0 failed.
- The synthetic filename-only fixture accepts `Complete readiness notes for readiness/skill-loading-evidence-workflow.md placeholder` with `skillist: []`.
- The synthetic whole-word fixture rejects `Run EvidenceGraph task graph validation` with `trigger_group=graph validation` and `matched_trigger=task graph`.

Direct graph-only command:

`.specify/extensions/evidence/scripts/bash/run-audit.sh specs/033-fix-task-validator-feedback --graph-only`

Result:

- PASS: graph validation completed and wrote `readiness/task-graph.json` plus `readiness/task-graph.md`.
