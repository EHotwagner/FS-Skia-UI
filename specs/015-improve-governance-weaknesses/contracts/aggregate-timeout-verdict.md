# Contract: Aggregate Timeout Verdict

## Record Shape

```yaml
validation_verdict:
  target: Dev
  verdict: timeout
  stage: Smoke.Tests
  elapsed_duration: "00:10:00"
  last_observed_command: "dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj -m:1"
  focused_rerun:
    command: "dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj -m:1 --filter Smoke"
    result: pass
    evidence_path: specs/015-improve-governance-weaknesses/readiness/aggregate-hang-diagnostics.md
  diagnostic: "Aggregate validation timed out in Smoke.Tests; focused smoke rerun passed, so classify as orchestration concern unless another product check failed."
  evidence_path: specs/015-improve-governance-weaknesses/readiness/aggregate-hang-diagnostics.md
```

## Required Behavior

- Bounded smoke-level aggregate stages must have a timeout policy.
- Timeout evidence must include stage, elapsed duration, last observed command, recommended focused rerun, and final verdict category.
- A focused pass after aggregate timeout separates product check evidence from unresolved aggregate orchestration evidence.
- The verdict must not claim product failure without a product check failure.
