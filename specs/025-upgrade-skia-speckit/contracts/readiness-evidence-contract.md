# Contract: Readiness Evidence

## Purpose

Define the required final evidence set for the upgrade plan.

## Required Artifacts

| Artifact | Required Facts |
|----------|----------------|
| `readiness/version-selection.md` | Final checked versions, sources, affected files, and risk notes. |
| `readiness/dependency-report.md` | Before/after dependency graph, cycle status, and dependency spread review. |
| `readiness/template-version-alignment.md` | Generated profile pins, Spec Kit assets, selected skills, and validation commands. |
| `readiness/compatibility-consumer-inventory.md` | Complete repository consumer inventory for `FS.Skia.UI`. |
| `readiness/compatibility-public-surface-map.md` | Public area classification and replacement coverage. |
| `readiness/compatibility-sample-migration.md` | Representative sample migration or keep-unchanged decisions. |
| `readiness/compatibility-release-policy.md` | User-facing compatibility posture and deferred decisions. |
| `readiness/package-surface-baseline.md` | Surface baseline status and intentional difference summary. |
| `readiness/evidence-audit.md` | Final evidence audit result. |

## Acceptance Rules

- Each artifact must be produced from real repository state, package metadata,
  generated output, or command output.
- Unsupported host facts must include command, platform, reason, and whether
  the result blocks acceptance.
- Evidence audit must finish with no unresolved synthetic or diff-scan blockers
  before merge readiness.
