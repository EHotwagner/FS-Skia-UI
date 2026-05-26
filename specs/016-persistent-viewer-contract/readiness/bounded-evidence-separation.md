# Bounded Evidence Separation Readiness

## T009 Real Rejection Packages

Replaced the original fixture-only path with command-derived audit rejection packages for:

- bounded-only substitution
- unsupported-host-only launch diagnostics
- missing persistent launch fields

Evidence packages:

- `readiness/audit-rejections/bounded-only`
- `readiness/audit-rejections/unsupported-host-only`
- `readiness/audit-rejections/missing-persistent-fields`

Verification:

- Each package was audited with `.specify/extensions/evidence/scripts/bash/run-audit.sh`.
- Each package reports `readiness-contract: 0 blocking`.
- The bounded-only package reports `bounded-only substitution`.
- The unsupported-host-only package reports `unsupported-host-only persistent launch evidence`.
- The missing-fields package reports `missing persistent launch fields`.
- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "persistent viewer evidence contracts"` passed.

## T031 Documentation Separation

Updated:

- `docs/evidence.md`
- `docs/generated-apps.md`
- `template/fragments/skiaviewer/README.md`

The docs now label bounded smoke, first-frame, frame-count, scene metadata, and
unsupported-host diagnostics as CI/reviewer diagnostic helpers rather than
interactive readiness substitutes.

Verification:

- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "bounded viewer docs"` passed.

## T032 Rejection Evidence Summary

Status: real command-derived rejection evidence.

Evidence names:

- `specs/016-persistent-viewer-contract/readiness/audit-rejections/bounded-only/audit.log`
- `specs/016-persistent-viewer-contract/readiness/audit-rejections/unsupported-host-only/audit.log`
- `specs/016-persistent-viewer-contract/readiness/audit-rejections/missing-persistent-fields/audit.log`

Rejected categories:

- bounded smoke, first-frame, frame-count, and scene metadata without a supported-host persistent launch artifact
- unsupported-host diagnostics without a supported-host persistent launch artifact
- ambiguous persistent launch artifacts missing required fields

Risk level: broad Tier 1, because the feature changes package API, generated
template behavior, generated product validation, evidence audit behavior, and
public guidance.

Focused validation commands:

- `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/016-persistent-viewer-contract/readiness/audit-rejections/bounded-only` returned exit code `2` with expected persistent-launch blockers.
- `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/016-persistent-viewer-contract/readiness/audit-rejections/unsupported-host-only` returned exit code `2` with expected persistent-launch blockers.
- `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/016-persistent-viewer-contract/readiness/audit-rejections/missing-persistent-fields` returned exit code `2` with expected persistent-launch blockers.
- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "persistent viewer evidence contracts"` passed.

Audit command snapshot:

- `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/016-persistent-viewer-contract` reports `persistent-launch: 0 blocking` for the supported-host artifact.

Non-authoritative aggregate-result notes:

- The broad `Verify` aggregate previously hung in the smoke-test stage; focused reruns are recorded in `readiness/aggregate-hang-diagnostics.md`.
- Final merge readiness is now determined by `EvidenceAudit`, focused tests, and the supported-host persistent launch artifact.
