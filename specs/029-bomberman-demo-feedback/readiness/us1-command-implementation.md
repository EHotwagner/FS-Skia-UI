# US1 Command Implementation Evidence

Tasks: T018, T019, T020, T021
Captured: 2026-05-29T11:58:00+02:00

## Graph and Audit Wrappers

Implementation file: `template/base/build.fsx`

Observed production code:

- `authoritativeEvidenceScriptContract = ".specify/extensions/evidence/scripts/bash/run-audit.sh"`
- `runAuthoritativeEvidence target featureDir graphOnly`
- `ProcessStartInfo("bash", arguments)`
- graph mode appends `--graph-only`
- audit mode runs graph first, then full audit through the same bash runner
- missing script returns exit code `4` with diagnostics

This satisfies the wrapper requirement without requiring executable mode or chmod repair.

## Verification Log Capture

Implementation file: `template/base/build.fsx`

Observed production code:

- `runProcess` redirects stdout/stderr as text streams.
- `let output = stdout + stderr`
- `File.WriteAllText(logPath, output)`
- `printf "%s" output`

No binary log writer path is present in the generated build file for Verify output capture.

## Verification

Executable repository governance scan passed:

```text
dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "generated evidence" --logger "console;verbosity=minimal"
```

Result: 6 passed, 0 failed.

Guidance update verification:

```text
dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "generated evidence|reliable evidence" --logger "console;verbosity=minimal"
```

Result: 7 passed, 0 failed.

Updated guidance files:

- `template/base/README.md`
- `template/base/docs/product.md`
- `template/fragments/full-governance/README.md`

## Failure Diagnostics

Implementation file: `template/base/build.fsx`

Added or verified diagnostics for:

- missing authoritative evidence script
- failed command launch
- nonzero exit code with report path
- unreadable readiness log writes
- failed generated `Verify` process with log path

Focused governance verification after diagnostics changes:

```text
dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "generated evidence|reliable evidence" --logger "console;verbosity=minimal"
```

Result: 7 passed, 0 failed.
