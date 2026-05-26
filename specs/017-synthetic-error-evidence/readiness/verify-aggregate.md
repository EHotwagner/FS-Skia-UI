# Verify Aggregate

Command:

```bash
./fake.sh build -t Verify
```

Initial result: failed/incomplete. The run reached `Starting target 'Test'` and
then entered `dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj -m:1`. No
additional output was emitted for more than seven minutes, so the process was
interrupted to avoid leaving a hung validation process running.

Observed process state before interruption:

```text
bash ./fake.sh build -t Verify
dotnet fake build -t Verify
dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj -m:1
vstest.console.dll ... Smoke.Tests.dll
testhost.dll ... Smoke.Tests.runtimeconfig.json
```

Investigation:

- `dotnet run --project samples/KeyboardInputGallery/KeyboardInputGallery.fsproj -- --contract-smoke` exited `0` and printed `status=ok`.
- `dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj -m:1 --logger "console;verbosity=detailed" --diag specs/017-synthetic-error-evidence/readiness/logs/smoke-tests-diag.log` exited `0`; all 3 smoke tests passed in about 5 seconds.
- A subsequent `Verify` attempt progressed past `Smoke.Tests` and failed only on missing readiness files:
  `public-surface.md`, `package-boundary.md`, `generated-product-usage.md`, and `compatibility-impact.md`.
- Those readiness files were added for this governance-only feature.

Final result:

```text
./fake.sh build -t Verify
Finished target 'Verify'
exit:0
Script running: 5 minutes, 22 seconds
Runtime: 5 minutes, 23 seconds
```

Focused authoritative evidence for this governance change:

- `dotnet run --project tests/Governance.Tests/Governance.Tests.fsproj -- --filter "Synthetic error evidence governance"` passed 5 tests.
- `./fake.sh build -t GeneratedGuidanceCheck` passed.
- `./fake.sh build -t EvidenceGraph` passed.
- `./fake.sh build -t EvidenceAudit` passed.
- `./fake.sh build -t Verify` passed after the missing readiness artifacts were added.

The earlier hang appears transient/stale process state rather than a
deterministic smoke failure. The concrete T039 blocker was the missing V1/V2
readiness artifact set.
