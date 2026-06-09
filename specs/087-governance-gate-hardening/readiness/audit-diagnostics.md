# Audit diagnostics — feature 087 (placeholder)

- **Authoritative command**: `./fake.sh build -t EvidenceAudit`.
- **Artifact path**: `readiness/audit-three-verdicts.txt`,
  `readiness/seh-audit-summary.json`, `readiness/synthetic-evidence.json`,
  `readiness/evidence-audit.md`.
- **Failure class**: `Fail` on any unaccepted synthetic or any blocking hit
  (diff-scan, readiness-contract, persistent-launch, persistent-gui-runtime,
  window-visibility, audit-status, invalid-seh). `PassWithAcceptedDeferrals`
  (FR-007) is reachable **only** with zero unaccepted synthetic and zero blocking
  hits; accepted deferrals are recorded as durable structured data (FR-008).
- **Next action**: three seeded inputs produce the three distinct verdicts
  (T022); each `--accept-synthetic` override is recorded with justification.
