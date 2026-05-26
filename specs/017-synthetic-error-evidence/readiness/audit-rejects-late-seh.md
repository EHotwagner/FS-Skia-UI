# Audit Rejects Late SEH

Command:

```bash
.specify/extensions/evidence/scripts/bash/run-audit.sh specs/017-synthetic-error-evidence/readiness/audit-fixtures/late --base HEAD
```

Result: exit code `2`.

Key output:

```text
verdict=FAIL
accepted-seh-tasks=1
late-seh-tasks=1
diagnostic=T001 failed-rule=late [SEH] classification; non-eligible synthetic evidence class source=implementation readiness cleanup after audit failure required-action=Return to design/task generation and record valid [SEH] classification before implementation.
```

The diagnostic names the task, failed rule, observed late source, and required
planning action.
