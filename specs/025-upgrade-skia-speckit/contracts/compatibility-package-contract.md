# Contract: Compatibility Package Evidence

## Purpose

Define the evidence required before accepting any compatibility-package posture
or package-surface conclusion for `FS.Skia.UI`.

## Consumer Inventory Contract

| Field | Required | Meaning |
|-------|----------|---------|
| `path` | yes | Repository path containing the usage. |
| `consumer-kind` | yes | Sample, test, source project, template, documentation, generated output, or package metadata. |
| `usage-kind` | yes | Project reference, package reference, namespace open, guidance mention, or generated pin. |
| `package-mode` | yes | Local project-reference, packaged mode, generated mode, docs-only, or unknown. |
| `focused-replacement` | yes | Focused package replacement when known, or explicit gap. |
| `migration-status` | yes | Keep unchanged, migrated, deferred, compatibility-only, or not applicable. |

## Public Surface Classification Contract

| Field | Required | Meaning |
|-------|----------|---------|
| `symbol-or-area` | yes | Public member, module, or capability area. |
| `classification` | yes | Primary-only, duplicate, facade candidate, deprecated candidate, or permanent compatibility surface. |
| `focused-equivalent` | yes | Replacement package/member or explicit gap. |
| `surface-baseline-status` | yes | Unchanged, changed intentionally, or blocked. |
| `migration-guidance` | yes | User-facing guidance or deferral reason. |

## Acceptance Rules

- Existing repository consumers must either keep working unchanged or have an
  explicit migration entry.
- No public API may be removed only because a focused package exists.
- Focused packages must not gain a dependency on the broad compatibility
  package.
- Any package-surface difference must be intentional, documented, covered by
  surface baseline evidence, and reflected in release guidance.
