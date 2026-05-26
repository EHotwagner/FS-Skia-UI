# Task Workflow Guidance Evidence

Task: T015

## Elmish/MVU Evidence Obligations

Stateful launch, desktop diagnostics, generated-host lifecycle, keyboard input,
tick progression, evidence capture, and package/test workflow checks must use an
Elmish/MVU-style boundary when implementation owns workflow state or I/O:

- `Model` records owned state such as not-started, starting,
  interactive-running, evidence-running, first-frame-presented,
  input-observed, user-close-observed, self-closed-for-evidence, failed, and
  unsupported.
- `Msg` records user actions, host/runtime callbacks, external responses, and
  internal transitions.
- `Effect` or `Cmd<Msg>` records requested I/O such as opening a window,
  rendering, dispatching input, capturing screenshots, reading pixels, closing
  for evidence, writing readiness files, or emitting diagnostics.
- `init` returns the initial `Model` plus startup effects.
- `update` is pure and must not touch filesystem, network, display session,
  process state, wall clock, random source, or mutable global state.
- Interpreters at the edge execute effects and map results back to `Msg`.

Before a user-story task is marked `[X]`, evidence must include public
`init`/`update` transition assertions, emitted-effect assertions, and a
user-reachable interpreter path where safe. Unit tests against internal helpers
alone are not enough for `[US*]` completion.

## Synthetic Fake Window-Loop Limits

Fake or test window loops are allowed only for the narrow keep-open regression
when a native desktop is unavailable in CI. Such tests are synthetic-only and
must include:

- a code/test disclosure containing `SYNTHETIC`;
- a Synthetic-Evidence Inventory row in `tasks.md`;
- a real-evidence path to supported-host screenshot, pixel-readback, or
  unsupported-host diagnostics;
- no claim that bounded, first-frame, scene metadata, or self-closing evidence
  completes interactive graphical readiness.

Real interpreter evidence remains required for package resolution, generated
test execution, desktop-session diagnostics, visual proof, and default
interactive launch behavior.

## Risk Levels

- `small`: isolated documentation or readiness wording. Focused validation:
  file scan or relevant unit/governance test.
- `medium`: one package, template fragment, or focused governance workflow.
  Focused validation: affected project tests plus the owning FAKE target.
- `broad`: public API, generated product defaults, package resolution,
  evidence/audit rules, or cross-template workflow changes. Broad validation is
  required before final completion through `./fake.sh build -t Verify`, plus
  `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`.

This feature is broad Tier 1. Focused reruns are still required after each
change, but final readiness must not rely on a narrow check alone.

## Implementation Batch Records

Each implementation batch should record:

- task ids included in the batch;
- loaded skill paths from `readiness/skill-loading-evidence.md`;
- shared evidence paths;
- graph before/after paths or timestamps;
- focused commands run and result;
- whether any aggregate command result is non-authoritative.

## Red-Green Evidence Log Format

Use this shape for readiness log entries:

```text
task_ids:
red_command:
red_result:
red_log:
green_command:
green_result:
green_log:
graph_before:
graph_after:
focused_rerun:
aggregate_result:
authoritative_product_evidence:
non_authoritative_aggregate_reason:
```

A timed-out or interrupted aggregate target is non-authoritative for product
behavior unless the affected focused rerun also fails. Record the stage, elapsed
duration, last observed command, recommended focused rerun, focused rerun
result, and final verdict category instead of treating the aggregate result as a
product defect.

## Recorded Implementation Batches

batch=us1 task-ids=T016,T017,T018,T019,T020,T021,T022,T023 graph-before=readiness/logs/t016-evidence-graph.txt graph-after=readiness/logs/t023-evidence-graph.txt skill-loading=readiness/skill-loading-evidence.md red-green-log=readiness/interactive-lifecycle.md aggregate-result=focused authoritative_product_evidence=true non_authoritative_aggregate_reason=none

batch=us2 task-ids=T024,T025,T026,T027,T028 graph-before=readiness/logs/t024-evidence-graph.txt graph-after=readiness/logs/t028-evidence-graph.txt skill-loading=readiness/skill-loading-evidence.md red-green-log=readiness/evidence-launch-mode.md aggregate-result=focused authoritative_product_evidence=true non_authoritative_aggregate_reason=none

batch=us3 task-ids=T029,T030,T031,T032,T033 graph-before=readiness/logs/t029-evidence-graph.txt graph-after=readiness/logs/t033-evidence-graph.txt skill-loading=readiness/skill-loading-evidence.md red-green-log=readiness/container-session-diagnostics.md aggregate-result=focused authoritative_product_evidence=true non_authoritative_aggregate_reason=none

batch=us4 task-ids=T034,T035,T036,T037,T038,T039,T040,T041 graph-before=readiness/logs/t034-evidence-graph.txt graph-after=readiness/logs/t041-evidence-graph.txt skill-loading=readiness/skill-loading-evidence.md red-green-log=readiness/package-resolution.md;readiness/generated-verify.md;readiness/game-visual-evidence.md aggregate-result=focused authoritative_product_evidence=false non_authoritative_aggregate_reason=package-resolution is blocking until generated packages resolve exactly without NU1603

## Graph Before/After Records

- T034 after status change: `specs/018-persistent-gui-runtime/readiness/logs/t034-evidence-graph.txt`
- T035 after status change: `specs/018-persistent-gui-runtime/readiness/logs/t035-evidence-graph.txt`
- T036 after status change: `specs/018-persistent-gui-runtime/readiness/logs/t036-evidence-graph.txt`
- T037 after status change: `specs/018-persistent-gui-runtime/readiness/logs/t037-evidence-graph.txt`
- T038 after status change: `specs/018-persistent-gui-runtime/readiness/logs/t038-evidence-graph.txt`
- T039 after status change: `specs/018-persistent-gui-runtime/readiness/logs/t039-evidence-graph.txt`
- T040 after status change: `specs/018-persistent-gui-runtime/readiness/logs/t040-evidence-graph.txt`
- T041 after status change: `specs/018-persistent-gui-runtime/readiness/logs/t041-evidence-graph.txt`

## Skill Loading Notes

Skill loading evidence is centralized in
`specs/018-persistent-gui-runtime/readiness/skill-loading-evidence.md`.
Every non-empty `skillist` task from T034 through T040 has a row with loaded
path, load result, work-start timestamp, evidence path, and reviewer exception.
