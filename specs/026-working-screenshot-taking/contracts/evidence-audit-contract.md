# Evidence Audit Contract

## Scope

Applies to feature readiness packages that declare screenshot-required visual
evidence.

## Audit Inputs

The audit consumes:

- screenshot evidence record
- screenshot artifact
- generated guidance report
- package surface baseline report
- evidence graph
- task synthetic-evidence classifications

## Rejection Conditions

The audit MUST reject screenshot-required packages when:

- the screenshot record is missing
- the PNG artifact is missing, unreadable, zero-dimension, blank, or outside the
  readiness package
- the record is metadata-only or deterministic-scene-only
- `capture-source` is not accepted screenshot capture
- `proves-screenshot` is false or absent
- required traceability fields are absent
- synthetic screenshot success is used
- existing layout/launch/scene evidence is cited as substitute screenshot proof

## Accepted Diagnostics

Unsupported-host and failure diagnostics are accepted as diagnostic evidence but
do not satisfy screenshot proof. Final readiness for this feature still requires
at least one supported-host successful screenshot artifact.
