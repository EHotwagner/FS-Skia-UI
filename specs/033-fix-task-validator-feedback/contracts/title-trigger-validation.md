# Contract: Title Trigger Validation

## Scope

This contract covers high-confidence required-skill matching in `.specify/extensions/evidence/scripts/python/compute-task-graph.py`.

## Required Behavior

- The validator must preserve blocking checks for omitted Spec Kit skills when a task title clearly requests graph validation, evidence audit, task generation, implementation loading, or constitution work.
- Trigger tokens found only inside longer filenames or longer words must not be treated as high-confidence matches.
- Setup/readiness tasks that cite mandated readiness filenames must validate with `skillist: []` when the title does not request the corresponding workflow.
- Titles beginning with `Complete readiness notes` continue to suppress capability expectation checks for readiness aggregation tasks.

## Diagnostics

When a title creates a blocking skill expectation, the diagnostic must include:

- task id
- candidate skill id
- matched trigger group or term
- declared skill list
- next action

## Readiness Evidence

Record validation in `specs/033-fix-task-validator-feedback/readiness/title-trigger-validation.md`.
