# Spec Kit Governance

Generated specs and implementation plans must carry the same governance
questions that this repository expects from contributors.

## Specification Prompts

Spec templates ask for package impact, public contract impact, state workflow
impact, layout/rendering impact, evidence obligations, unsupported scope, and
build-target impact before implementation planning begins.

## Planning Prompts

Plan templates require decisions for template ownership, dependency impact,
command-surface impact, generated project impact, evidence paths, `.fsi` or
contract impact, MVU/effect boundary applicability, synthetic evidence, test
evidence, observability, and deferred scope.

## Task Skill Governance

Task generation is not complete until every task has been evaluated against
available capability skills and the result is recorded in both task artifacts:

- `tasks.deps.yml` stores structured object metadata for every `Tnnn` task with
  `deps` and `skillist` fields.
- `tasks.md` mirrors the same ordered skill list on each task line using
  `[skillist: ...]`, or `[skillist: []]` when no capability skill applies.

Capability skills include repository-local `.agents/skills/*/SKILL.md` files,
package-owned `src/*/skill/SKILL.md` files, template capability skills, and
generated-product skill destinations where applicable. The task generator must
choose the minimal ordered skill set that materially helps the work; capability
skills take precedence over generic guidance when both match.

Implementation must read each task's structured `skillist`, resolve every skill
identifier to exactly one readable `SKILL.md`, load those skills in declared
order before code changes for that task, and record the loaded paths in
readiness evidence or the task-specific verification log. Missing, unreadable,
ambiguous, mismatched, non-list, or obviously omitted skill metadata blocks
readiness and implementation until the task list is migrated or regenerated.

## Preset Inheritance

The active `.specify/templates/` files and the
`.specify/presets/fsharp-opinionated/templates/` overrides must stay aligned so
new generated products inherit the same governance prompts without manual
copying from historical feature directories.

The same inheritance rule applies to task metadata templates, task-generation
skills, implementation skills, constitution templates, and generated command
guidance. `GeneratedGuidanceCheck` validates that those governed sources keep
the mandatory `skillist` gate and implementation-time skill-loading rule.

## Readiness Validation

`EvidenceGraph` validates task topology and task skill metadata before
implementation proceeds. It rejects missing task ids, dangling dependencies,
cycles, legacy bare-list `tasks.deps.yml` entries, missing or malformed
`skillist` fields, missing `tasks.md` mirrors, mirror mismatches, unresolved
declared skills, obvious capability omissions, non-minimal invalid skill sets,
and invalid multi-skill ordering. `EvidenceAudit` consumes the refreshed graph
for synthetic-evidence propagation and diff-scan readiness.

Approved synthetic error-handling is visible in the same path. A task tagged
`[SEH]` with `synthetic-error-handling-approved` is accepted only when its
Synthetic-Evidence Inventory row records the design source, synthetic input
class, expected error behavior, rationale, and `accepted-seh` status before
implementation starts. The audit summary separates `accepted-seh-tasks` from
`unaccepted-synthetic-tasks`, `auto-synthetic-tasks`, and `late-seh-tasks`, so
reviewers can find all accepted synthetic malformed-input/error-path evidence
from `tasks.md`, `task-graph.md`, or `evidence-audit.md` within the ordinary
review window. Implementation-time relabeling remains a readiness failure.

## Deferred Roadmap

Generated guidance validation is section-aware: prompts must appear in the
expected governance section, cannot be satisfied solely by deferred roadmap
text, and active templates must remain aligned with the F# preset templates.

Generated artifacts distinguish current V2 obligations from deferred visual
evidence, release validation, an external repository split, and distribution
automation.

Deferred visual evidence remains outside V2 pass/fail validation.
Deferred distribution automation remains outside V2 pass/fail validation.

## Tiered process and the `Route` entry point (feature 042)

Spec Kit work no longer applies the full serialized governance order to every
change. Run **`./fake.sh build -t Route`** first: it selects the authoritative
**tier** for the change and prints the **minimal gate list** to run. Routine
framework work routes to the light `inner-loop` tier (`Dev` only); changes to the
consumer contract (`template/**`, `.specify/**`, public `src/**/*.fsi`, the
build-target paths) **escalate** to `focused-authority` / `agent-ready` /
`maintainer-verify`.

- **Framework-author vs consumer-agent.** The developer-class axis defaults to
  `framework-author`. `./fake.sh build -t Route consumer-agent` raises the floor
  to `focused-authority`; consumer-contract paths escalate regardless of class.
- **`--enforce`.** `./fake.sh build -t Route --enforce` blocks an escalated change
  that is missing its tier's required evidence artifacts.
- **Dogfood features** (e.g. `042`) are forced to the full serialized pipeline so
  the consumer-grade harness stays exercised and cannot rot.

The selector is compiled F# in `FS.Skia.UI.Build.Routing` (a mistyped gate is a
compile error); `validation.contract.yml` is generated from it. FAKE-backed
commands share `.fake` state and are not safe to run concurrently — run escalated
gates sequentially in the deterministic order.
