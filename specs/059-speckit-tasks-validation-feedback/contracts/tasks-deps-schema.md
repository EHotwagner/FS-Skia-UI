# Contract: `tasks.deps.yml` schema (versioned)

**Contract version**: 2.0 (adds per-task `owns:`; `1.x` files without `owns:`
remain valid — `owns` is optional).
**Consumed by**: `build/Governance/Evidence/DepsParser.fs` at the `build.fsx`
interpreter edge, via `./fake.sh build -t EvidenceGraph` / `EvidenceAudit`.
**Shipped to consumers via**:
`.specify/presets/fsharp-opinionated/templates/tasks-deps-template.yml` and the
`.agents/skills/speckit-tasks` guidance.

## Required document shape

```yaml
schema_version: "1.0"        # REQUIRED top-level scalar
tasks:                       # REQUIRED top-level mapping (the wrapper)
  T001:
    deps: []                 # ordered list of non-phase dependency ids
    skillist: []             # ordered list of capability skill ids
    owns: []                 # OPTIONAL gated-evidence ownership (default none)
  T032:
    deps: [T031]
    skillist: ["speckit-evidence-graph"]
    owns: ["graph-validation"]
  T033:
    deps: [T032]
    skillist: ["speckit-evidence-audit"]
    owns: ["evidence-audit"]
```

The single fact that gates validation is the top-level `tasks:` wrapper. Bare
top-level `Tnnn:` keys (no wrapper) are rejected with the directive error below.

## `owns:` vocabulary (closed set)

| Value                     | Task owns…                              | Required skill in `skillist` |
|---------------------------|-----------------------------------------|------------------------------|
| `graph-validation`        | task-graph / readiness validation       | `speckit-evidence-graph`     |
| `evidence-audit`          | synthetic-propagation / diff-scan audit | `speckit-evidence-audit`     |
| `task-generation`         | `/speckit.tasks` task-generation        | `speckit-tasks`              |
| `implementation-loading`  | `/speckit.implement` skill-loading      | `speckit-implement`          |
| `constitution`            | constitution authoring                  | `speckit-constitution`       |

- `owns:` is optional; omit it (or use `[]`) for tasks that own no gated
  evidence. Most tasks own nothing.
- Task **titles are free-form**: ownership is determined solely by `owns:`. The
  former title-trigger matcher is removed (no title is scanned for capability
  phrases).

## Error contract (directive)

| Condition | Message |
|-----------|---------|
| bare top-level task keys, no `tasks:` | `tasks.deps.yml: missing or malformed 'tasks' mapping (found bare task keys; nest them under a top-level 'tasks:' mapping with schema_version)` |
| unknown `owns:` value | `<Tid>: unknown owns value '<v>'; allowed: graph-validation, evidence-audit, task-generation, implementation-loading, constitution` |
| `owns:` value missing its implied skill | `<Tid>: owns <v> requires skill <skill> in skillist; declared_skillist=[…]` |
| id in `tasks.md` not in deps | `tasks.md declares <Tid> but tasks.deps.yml has no key for it` |
| id in deps not in `tasks.md` | `tasks.deps.yml declares <Tid> but tasks.md has no task line` |

The wrapper error MUST be emitted standalone (not buried under per-id "no key"
errors) when bare keys are detected (FR-007).

## Migration (FR-010)

Existing task files that relied on title-trigger matching: re-express ownership
by adding `owns: [<value>]` to the owning task and removing any awkward
title rewording that existed only to satisfy/avoid the matcher. No `tasks.md`
title change is required for correctness anymore.
