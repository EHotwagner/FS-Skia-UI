# Aggregate Hang Diagnostics

validation_verdict:
  target: Dev
  verdict: non-authoritative aggregate pass
  stage: Test aggregate
  elapsed_duration: bounded by per-process 30 minute timeout policy
  last_observed_command: dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj -m:1
  timeout_policy: aggregate process effects are bounded; smoke-level isolation uses a focused rerun
  recommended_focused_rerun: dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj -m:1 --filter Smoke
  focused_rerun:
    command: dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj -m:1 --filter Smoke
    result: pass when the focused smoke check succeeds
    evidence_path: specs/015-improve-governance-weaknesses/readiness/aggregate-hang-diagnostics.md
  final_classification: orchestration concern when aggregate Dev times out and the focused smoke rerun passes
  diagnostic: Aggregate validation timeout evidence must name stage, elapsed duration, last observed command, focused rerun result, and verdict category. A focused pass after aggregate timeout is not a product failure; it is a non-authoritative aggregate result until orchestration is isolated.

## Fixture Coverage

- Timeout verdicts include target, stage, elapsed duration, last observed
  command, timeout policy, and recommended focused rerun.
- Focused smoke rerun separation keeps a passing direct smoke result distinct
  from unresolved aggregate orchestration behavior.
- Product failure is reported only when a product check fails.
- Environment failure is reserved for runner, SDK, process-health, or bootstrap
  problems.
