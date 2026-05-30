# Contract: Readiness Scaffold Coverage

## Scope

Generated visual demo tasks make audit-required readiness obligations discoverable before implementation begins.

## Required Behavior

- Generated tasks or guidance enumerate every readiness file expected for a generated visual demo when the matching obligation applies.
- Scaffold guidance includes required terms, required `key=value` fields, authoritative command names, artifact paths, unsupported/failure classifications, and next-action fields.
- Scaffold guidance covers visual evidence, real-image evidence, interactive window visibility, close-reason separation, window-state diagnostics, window options, governance risk levels, aggregate hang diagnostics, runtime limitations, and generated validation.

## Acceptance Cues

- An implementer can identify all required readiness files without reading `.specify/extensions/evidence/scripts/bash/run-audit.sh`.
- `generated-validation.md` guidance includes fields such as `exact-package-match`, `generated-tests-ran`, `authoritative`, and `failure-class`.
- Window and visual evidence files distinguish environment readiness, runtime framework failure, unsupported capture, and consumer-authored workaround.
