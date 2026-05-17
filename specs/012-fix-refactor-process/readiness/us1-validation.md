# US1 Independent Validation

## Command Path

US1 is reachable through maintainer-facing FAKE targets:

- `./fake.sh build -t VerifyPreflight`
- `./fake.sh build -t CiPreflight`
- `./fake.sh build -t Verify`
- `./fake.sh build -t Ci`

## Evidence

| Requirement | Evidence |
|-------------|----------|
| FR-001, SC-001 | `process-health.md` records timestamp, target, platform, memory, process count, zombie count, thread/file descriptor headroom, dotnet startup, FAKE bootstrap, unsupported signals, threshold decisions, and `preflight-elapsed-ms`. |
| FR-001a, SC-001a | `logs/t020-verify-fail-fast.txt` shows `Verify` stops in `VerifyPreflight` before high-pressure target dependencies start. |
| FR-001b, SC-001b | `logs/t017-threshold-override.txt` and `process-health.md` record rule id, default threshold, override value, source, and reason. `logs/t016-malformed-threshold-override.txt` records malformed override failure. |
| FR-002, FR-003, FR-014, SC-002 | `verification-verdicts.md` records `environment-failure`, product checks run as `(none)`, `authoritative-product-evidence: False`, diagnostics, and rerun guidance. |
| FR-004, FR-005 | `bootstrap-runner.md` records wrapper/tool/package status and warning classification for repeated FAKE `netstandard` startup noise. |
| FR-017, FR-018, SC-009 | `verification-verdicts.md`, `quickstart.md`, `docs/build.md`, and `docs/evidence.md` state that final readiness waits for a later healthy broad aggregate pass after environment failure. |

## Current Runner Status

The local runner produced real healthy preflight evidence and real forced
fail-fast evidence. Full healthy broad `Verify`/`Ci` product proof is not
claimed because the runner has high zombie-process pressure and previously
showed CoreCLR startup failures during governance suite execution. Final
readiness remains waiting for a fresh shell, fresh container, or CI runner
broad aggregate pass.

