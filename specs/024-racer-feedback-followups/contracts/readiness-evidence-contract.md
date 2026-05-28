# Readiness Evidence Contract

## Scope

Defines the feature-level readiness files required before acceptance.

## Required Artifacts

- `specs/024-racer-feedback-followups/readiness/baseline-status.md`
- `specs/024-racer-feedback-followups/readiness/generated-guidance-validation.md`
- `specs/024-racer-feedback-followups/readiness/screenshot-capability-detail.md`
- `specs/024-racer-feedback-followups/readiness/screenshot-success-artifact.md`
- `specs/024-racer-feedback-followups/readiness/host-warning-classification.md`
- `specs/024-racer-feedback-followups/readiness/detached-launch-guidance.md`

## Acceptance Rules

- Each artifact MUST name the command or manual validation step that produced
  it, the host scope, and the facts reviewed.
- Screenshot acceptance MUST include real screenshot success on at least one
  supported Windows or Linux desktop host.
- If the other supported OS is unavailable, the readiness artifact MUST record
  explicit capability evidence or deferral status for that OS.
- Evidence audit and graph validation MUST remain discoverable for the feature.
- Synthetic screenshot success, hidden warning suppression, and deterministic
  render output relabeled as screenshot proof are invalid.
