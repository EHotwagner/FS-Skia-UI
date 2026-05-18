# Contract: Task Skilllist Governance

## Structured Metadata

`tasks.deps.yml` remains the structured sibling to `tasks.md`. Each task key MUST include both dependencies and `skillist`.

```yaml
T001:
  deps: []
  skillist: ["speckit-tasks"]
T002:
  deps: ["T001"]
  skillist: []
```

Compatibility note: existing bare-list dependency entries are invalid for new task lists after this feature. Existing task lists must be migrated or regenerated before implementation.

## Human Mirror

Each task line in `tasks.md` MUST mirror the structured value.

```markdown
- [ ] T001 [skillist: speckit-tasks] Update task template guidance
- [ ] T002 [skillist: []] Record readiness evidence
```

Rules:

- Empty skill lists MUST be written as `[skillist: []]`.
- Non-empty lists MUST preserve the structured order.
- The mirror MUST use canonical skill identifiers.

## Readiness Validation

Task readiness validation MUST fail when:

- A task is missing structured `skillist`.
- `skillist` is not a list.
- `tasks.md` omits the mirror.
- The mirror and structured field differ.
- A declared skill is missing, unreadable, or ambiguous.
- A task description obviously matches a known capability skill but `skillist` omits it.

Diagnostics MUST include the task id and the failing field, for example:

```text
T014: missing structured skillist in tasks.deps.yml
T021: tasks.md mirror [fs-skia-layout] does not match tasks.deps.yml [fs-skia-layout, fs-skia-testing]
T033: declared skill fs-skia-charts is not readable or not registered
```

## Implementation Loading

Before implementing a task, `/speckit.implement` MUST:

1. Read the task's structured `skillist`.
2. Resolve every skill id to exactly one readable `SKILL.md`.
3. Load those skills in declared order.
4. Record that loading happened before implementation work.
5. Block the task with an actionable diagnostic on missing, unreadable, or ambiguous skills.

## Capability Inventory

The canonical local capability inventory is:

- `.agents/skills/*/SKILL.md`
- `src/*/skill/SKILL.md`
- template-owned skill paths declared by `template/capabilities.yml`
- active generated product skill destinations when implementing inside a generated product

Capability skills take precedence over generic guidance when both match.
