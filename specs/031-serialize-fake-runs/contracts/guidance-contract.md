# Contract: Sequential FAKE Guidance

Any repository, agent-facing, generated-template, or readiness guidance that mentions FAKE-backed tests or FAKE targets must satisfy this contract.

## Required Text Semantics

- Names the command class as FAKE-backed tests, FAKE targets, or commands that invoke `fake.sh`, `fake.cmd`, or `dotnet fake`.
- States that FAKE-backed commands are not safe to run concurrently in this repository.
- Names the shared `.fake` state race risk.
- Requires sequential execution when more than one FAKE-backed command is needed, even if the commands look independent.
- Preserves the distinction that unrelated non-FAKE checks may still run in parallel when otherwise safe.

## Required Ordering

When multiple FAKE-backed commands are listed, they must appear as a deterministic sequence. Valid forms include:

```text
1. ./fake.sh build -t Dev
2. ./fake.sh build -t GeneratedGuidanceCheck
3. ./fake.sh build -t EvidenceGraph
4. ./fake.sh build -t EvidenceAudit
```

Invalid forms include prose or scripts that instruct agents to launch more than one FAKE-backed command concurrently, background a FAKE command, or split independent FAKE targets across parallel tool calls.

## Validation Expectations

- Focused tests or scanners should fail if an updated agent-facing validation instruction mentions FAKE-backed commands without sequential guidance.
- Generated guidance validation should prove template source and package outputs carry the same rule.
- A failure message should name the path, missing required concept, and the command snippet or heading that needs repair.
