# Contract: Generated Evidence Policy Separation

## Normal Launch

Generated product normal launch must remain persistent and interactive. It must not:

- run evidence graph or audit checks
- close the window for evidence collection
- write readiness evidence artifacts
- claim screenshot, visual, package, or final-readiness proof

## Evidence Commands

Evidence behavior must be invoked through explicit evidence commands or build targets. Those commands may:

- collect product-owned facts
- run governed validation gates
- write readiness reports
- emit agent verdicts
- classify unsupported hosts, stale prerequisites, and missing artifacts

## Ownership Boundary

Product-owned facts include product model/view/update behavior, layout facts, key mappings, generated host identity, and product-specific evidence adapters.

Policy-owned behavior includes command orchestration, audit execution, report wording, authority classification, failure ownership, stale-prerequisite remediation, and final readiness claims.

## Report Wording

Evidence reports must:

- state the authority level of completed validation
- identify skipped or unsupported gates
- avoid success-only completion claims
- preserve environment and prerequisite failures as separate outcomes
- name the next command when the run is incomplete or degraded

## Validation

Generated product inspection and tests must prove that normal launch remains evidence-free and that explicit evidence commands produce governed reports without changing everyday product execution.
