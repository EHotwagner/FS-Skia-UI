# Research: Task Skilllist Governance

## Decision: Store `skillist` in `tasks.deps.yml`

**Rationale**: The existing evidence graph treats `tasks.deps.yml` as the structured sibling to `tasks.md`. Adding `skillist` beside each task's dependency list gives validators a stable machine-readable source without inventing another metadata file.

**Alternatives considered**:

- Only annotate `tasks.md`: rejected because FR-003 requires structured metadata and validators would need fragile Markdown parsing.
- Add a separate `tasks.meta.yml`: rejected because it creates a third lockstep artifact and increases drift risk.

## Decision: Mirror `skillist` on each `tasks.md` line

**Rationale**: Reviewers need to see applicable skills in the human checklist without opening YAML. The mirror also gives validation a direct consistency check between human and structured views.

**Alternatives considered**:

- Add one table at the end of `tasks.md`: rejected because task lines remain incomplete during review.
- Show only non-empty skills: rejected because an explicit empty list is required when no skill applies.

## Decision: Extend evidence graph/readiness validation

**Rationale**: `speckit.evidence.graph` already blocks implementation on broken task topology and writes readiness reports. Extending the same validation surface keeps task readiness in one place and aligns with the existing `before_implement` evidence hook.

**Alternatives considered**:

- Validate only inside `/speckit.tasks`: rejected because manually edited task lists could bypass the rule.
- Validate only inside `/speckit.implement`: rejected because readiness should fail before implementation begins.

## Decision: Implementation must load skills before each task

**Rationale**: A task-level `skillist` is only useful if the implementer reads the declared skill guidance before touching the task. The implementation workflow should block on missing, unreadable, or ambiguous skills so invalid metadata cannot silently degrade to generic guidance.

**Alternatives considered**:

- Load all skills once at feature start: rejected because it over-loads context and weakens the per-task contract.
- Treat missing skills as warnings: rejected because FR-006 requires a block.

## Decision: Capability matching uses explicit skill descriptions and known aliases

**Rationale**: The constitution already defines capability skills through package-owned skill files and `template/capabilities.yml`. Matching should use skill identifiers, descriptions, and declared paths, with diagnostics for ambiguous identifiers.

**Alternatives considered**:

- Infer from filesystem paths only: rejected because task descriptions often name capabilities in prose.
- Use a remote registry: rejected because this repository already owns the authoritative local skill inventory.

## Decision: No runtime API or package dependency changes

**Rationale**: The requested behavior changes Spec Kit governance and implementation workflow, not FS.Skia.UI runtime behavior.

**Alternatives considered**:

- Add an F# library for task metadata validation: deferred unless existing scripts prove insufficient during implementation.
