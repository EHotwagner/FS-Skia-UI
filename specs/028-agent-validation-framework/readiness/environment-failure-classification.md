# Environment Failure Classification Evidence

Status: classified from real `AgentReady` command evidence.

Real command:

```bash
./fake.sh build -t AgentReady
```

Evidence:

- command log: `specs/028-agent-validation-framework/readiness/logs/t031-agent-ready.txt`
- fallback assertion log: `specs/028-agent-validation-framework/readiness/logs/t032-degraded-fallback.txt`
- machine verdict: `specs/028-agent-validation-framework/readiness/agent-verdict.json`
- reviewer verdict: `specs/028-agent-validation-framework/readiness/agent-ready-verdict.md`

Observed classification:

- status: `degraded`
- failure owner: `governance`
- failure class: `missing-evidence`
- missing gate: `EvidenceAudit`
- next command: `./fake.sh build -t Verify`

This run did not observe a real environment or unsupported-host failure. The
environment, unsupported-host, stale-prerequisite, and missing-evidence mapping
logic is covered by the approved `[SEH]` forced-outcome tests for T028 and the
real serializer/aggregation evidence for T029-T030. Those forced outcomes are
not claimed as real host failures here; they only prove governed diagnostic
classification when such outcomes are received.
