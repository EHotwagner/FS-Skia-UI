# Detached Launch Guidance

## T026/T027 Red Governance Tests

Recorded at: 2026-05-28T08:34:39+02:00

Added generated guidance coverage in
`tests/Governance.Tests/GeneratedGuidanceTests.fs` for Linux detached viewer
launch instructions.

Required accepted pattern facts:

- `setsid`
- `> readiness/logs/`
- `2>&1`
- `< /dev/null`
- trailing `&`

Rejected stale guidance patterns:

- `nohup dotnet run`
- `dotnet run &`
- `simple backgrounding`

Red evidence:

| Command | Expected result | Evidence |
|---------|-----------------|----------|
| `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --logger "console;verbosity=minimal"` | FAIL, generated docs do not yet contain `setsid` detached launch guidance | `readiness/logs/t007-governance-tests-red.txt` |
| `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --logger "console;verbosity=minimal"` | FAIL remains after unrelated US2 work; detached launch guidance still missing | `readiness/logs/t019-governance-tests.txt` |

## T028/T029 Detached Launch Guidance Implementation

Recorded at: 2026-05-28T08:34:39+02:00

Reviewed and updated files:

- `docs/generated-apps.md`
- `template/base/docs/product.md`
- `template/fragments/skiaviewer/README.md`

Accepted command patterns:

- `setsid dotnet run --project src/Product/Product.fsproj > readiness/logs/generated-viewer.log 2>&1 < /dev/null &`
- `setsid dotnet run --project src/Product/Product.fsproj > readiness/logs/product-viewer.log 2>&1 < /dev/null &`
- `setsid dotnet run --project src/Product/Product.fsproj > readiness/logs/viewer-launch.log 2>&1 < /dev/null &`

Validation facts:

- detached session uses `setsid`
- stdout is captured under `readiness/logs/`
- stderr is redirected with `2>&1`
- stdin is detached with `< /dev/null`
- launch continues in the background with trailing `&`
- stale preferred guidance patterns remain absent: `nohup dotnet run`,
  `dotnet run &`, and `simple backgrounding`

Verification:

| Command | Result | Evidence |
|---------|--------|----------|
| `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --logger "console;verbosity=minimal"` | PASS, 167 tests | `readiness/logs/t029-governance-tests.txt` |
| `./fake.sh build -t GeneratedGuidanceCheck` | PASS | `readiness/logs/t029-generated-guidance-check.txt` |
