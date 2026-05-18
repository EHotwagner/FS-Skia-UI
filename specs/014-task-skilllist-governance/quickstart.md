# Quickstart: Task Skilllist Governance

## Generate Tasks

1. Run `/speckit.tasks` for a feature.
2. Confirm it emits both `tasks.md` and `tasks.deps.yml`.
3. Confirm every task has structured metadata:

```yaml
T001:
  deps: []
  skillist: []
```

4. Confirm every task line mirrors the same value:

```markdown
- [ ] T001 [skillist: []] Record feature evidence obligations
```

## Validate Readiness

Run the graph validation for the feature:

```bash
.specify/extensions/evidence/scripts/bash/run-audit.sh specs/014-task-skilllist-governance --graph-only
```

Expected result:

- Missing `skillist` fields fail.
- Mirror mismatches fail.
- Missing, unreadable, or ambiguous skills fail.
- Valid empty lists pass.

## Implement Tasks

Before starting each task, `/speckit.implement` reads that task's `skillist` and loads the listed `SKILL.md` files in order.

Expected result:

- Non-empty `skillist` records load evidence before implementation.
- Missing or unreadable declared skills block the task.
- Empty `skillist` is valid and requires no skill load.

## Final Evidence

Store focused evidence under:

- `specs/014-task-skilllist-governance/readiness/logs/skillist-validation.txt`
- `specs/014-task-skilllist-governance/readiness/logs/evidence-graph.txt`
- `specs/014-task-skilllist-governance/readiness/logs/evidence-audit.txt`
- `specs/014-task-skilllist-governance/readiness/logs/generated-guidance-check.txt`
