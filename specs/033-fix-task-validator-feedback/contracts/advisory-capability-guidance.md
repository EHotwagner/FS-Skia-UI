# Contract: Advisory Capability Guidance

## Scope

This contract covers non-blocking FS.Skia.UI capability guidance for task authors and reviewers.

## Required Behavior

- Guidance or diagnostics should help authors choose capability skills for at least five common categories from:
  - scene primitives
  - viewer host behavior
  - Elmish wiring
  - keyboard input
  - Yoga-backed layout
  - Controls and widgets
  - generated product testing
  - sample-pack work
  - layout evidence and host-warning classification
- These hints must remain advisory unless a later specification expands validator enforcement.
- Otherwise valid task metadata must not fail solely because an advisory FS.Skia.UI hint was omitted.

## Readiness Evidence

Record coverage and non-blocking proof in `specs/033-fix-task-validator-feedback/readiness/advisory-capability-guidance.md`.
