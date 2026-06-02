# Contract — Recurring-Run Mechanism (FR-009, SC-005)

Defines the discoverable, runnable mechanism that keeps the consumer-governance pipeline from
rotting, **without** depending on a live external CI service (spec Clarifications / Unsupported
scope).

## Required parts

1. **A tracked schedule-definition file** committed under a discoverable repo path.
   - It is a *committed* file (not the transient runtime `.claude/scheduled_tasks.lock`).
   - It names the **dogfood set** (`042`, `043`) and the **full serialized six-target pipeline** as
     the body to re-run, plus a cadence.
   - It is a documented routine/cron spec committed at
     **`.specify/schedules/foundations-dogfood-pipeline.yml`** (under the existing `.specify/`
     governance surface, beside `extensions.yml`), mirroring the Claude Code `schedule` routine
     shape. The contract fixes the path and the required fields (tracked, discoverable, names the
     dogfood set + the six-target pipeline + a cadence); the exact YAML key spelling is finalized in
     implementation against that surface.
2. **A documented manual full-pipeline fallback** — the serialized six-target command sequence, so
   the pipeline is runnable by hand:
   ```
   ./fake.sh build -t Dev
   ./fake.sh build -t GeneratedGuidanceCheck
   ./fake.sh build -t TemplateCheck
   ./fake.sh build -t GeneratedProductCheck
   ./fake.sh build -t EvidenceGraph
   ./fake.sh build -t EvidenceAudit
   ```
   (Run **sequentially** — FAKE shares `.fake` state and is not concurrency-safe.)

## Constraints

- **No live CI dependency.** The feature is complete when the mechanism is *defined, discoverable,
  and runnable*; no external CI service need exist (spec Assumptions / Unsupported scope).
- **Discoverable.** A maintainer inspecting the tree finds the schedule file and the fallback
  command from the retrospective without prior knowledge.

## Acceptance (SC-005)

The schedule-definition file is committed and discoverable; the manual fallback is documented and
runnable; the dogfooding retrospective (`readiness/retrospective.md`) identifies this mechanism and
is cross-linked from the after-baseline.
