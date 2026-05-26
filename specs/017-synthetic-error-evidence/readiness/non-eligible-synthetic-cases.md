# Non-Eligible Synthetic Cases

Command:

```bash
.specify/extensions/evidence/scripts/bash/run-audit.sh specs/017-synthetic-error-evidence/readiness/audit-fixtures/non-eligible --base HEAD
```

Result: exit code `2`.

Key output:

```text
verdict=FAIL
accepted-seh-tasks=0
unaccepted-synthetic-tasks=1
late-seh-tasks=0
```

The fixture uses a convenience mock and remains ordinary synthetic evidence.
The existing `--accept-synthetic` override remains available, but the audit
verdict is not weakened by `[SEH]`.
