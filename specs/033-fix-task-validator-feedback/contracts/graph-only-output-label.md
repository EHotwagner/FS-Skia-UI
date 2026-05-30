# Contract: Graph-Only Output Label

## Scope

This contract covers user-visible output and generated report text for graph-only validation.

## Required Behavior

- `EvidenceGraph` and direct `compute-task-graph.py` output must identify the run as graph validation.
- Output must not imply that the full evidence audit ran.
- Generated command reports should direct users to `EvidenceAudit` for merge-gate audit checks.
- The label must be visible in one log scan in both success and failure paths.

## Readiness Evidence

Record output proof in `specs/033-fix-task-validator-feedback/readiness/graph-only-output-label.md`.
