# Audit Accepted SEH

Command:

```bash
.specify/extensions/evidence/scripts/bash/run-audit.sh specs/017-synthetic-error-evidence/readiness/audit-fixtures/accepted --base HEAD
```

Result: exit code `0`.

Key output:

```text
verdict=PASS
real-tasks=1
accepted-seh-tasks=1
unaccepted-synthetic-tasks=0
auto-synthetic-tasks=0
late-seh-tasks=0
diff-scan-hits=0
```

The fixture keeps T001 as `[S] [SEH] synthetic-error-handling-approved` and the
audit still reports accepted synthetic counts separately from real tasks.
