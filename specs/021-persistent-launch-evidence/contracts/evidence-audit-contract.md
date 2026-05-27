# Contract: Evidence Audit

## Purpose

Make persistent-launch readiness obligations discoverable before the final
audit and enforce the machine-readable artifact contract at the final gate.

## Required Readiness Files

Task generation and implementation must produce:

- `specs/021-persistent-launch-evidence/readiness/persistent-launch-evidence.md`
- `specs/021-persistent-launch-evidence/readiness/window-observation-diagnostics.md`
- `specs/021-persistent-launch-evidence/readiness/host-warning-classification.md`
- `specs/021-persistent-launch-evidence/readiness/generated-guidance.md`
- `specs/021-persistent-launch-evidence/readiness/evidence-audit.md`

## Blocking Conditions

EvidenceAudit must block when:

- Any required readiness file is missing.
- A supported-host pass artifact is missing required fields.
- `status=ok` is claimed without real window-opened, first-frame, and exit-path
  facts.
- Observation/capture failure is classified as headless-only despite desktop
  prerequisites and a live process.
- Synthetic evidence is used to satisfy supported-host persistent launch.
- Benign host warnings hide concrete launch, render, layout, package, or
  artifact failure facts.

## Non-Blocking Conditions

EvidenceAudit must not block solely because:

- Known benign desktop warning messages are present and all required launch
  facts pass.
- Input dispatch is explicitly recorded as not verified where the contract
  permits that limitation.
- External title/window search fails while viewer-owned facts identify the
  blocked stage as observation or capture.

## Reporting

The audit summary must report:

- Artifact path.
- Accepted or rejected status.
- Missing fields.
- Blocked stage.
- Classification.
- Warning classification summary.
- Required readiness file coverage.
