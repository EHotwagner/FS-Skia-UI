# Generated Command Workflow

Task: T009
Captured: 2026-05-29T11:48:32+02:00

## Evidence Graph and Audit

Generated commands must invoke Spec Kit scripts through `bash`:

```text
bash .specify/extensions/evidence/scripts/bash/run-audit.sh <feature-dir> --graph-only
bash .specify/extensions/evidence/scripts/bash/run-audit.sh <feature-dir>
```

The command must preserve exit code, command path, output path, and diagnostics. It must not depend on executable file mode.

## Verify Logs

Redirected `Verify` output must be written with text APIs. Passing and failing runs must preserve readable stdout/stderr diagnostics and must not include embedded NUL bytes.
