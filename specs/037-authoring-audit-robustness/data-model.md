# Phase 1 Data Model: Fail-Loud Authoring & Audit Robustness

These are the conceptual entities the design manipulates. There is no runtime
domain model (no Elmish state) — these describe governance-tooling and
generation inputs/outputs.

## Active-Feature Resolution (US1)

The decision of which feature directory the graph/audit operates on.

| Field | Meaning | Validation |
|---|---|---|
| `source` | Where the id came from (`feature.json` \| `env-override` \| `branch-prefix`) | `feature.json` is authoritative when present |
| `featureId` | Resolved feature directory name (e.g. `037-authoring-audit-robustness`) | Non-empty; must correspond to an existing `specs/<id>` dir |
| `featureDir` | Absolute path passed to the audit | Must be an existing directory |
| `taskCount` | Real task count parsed from the feature's `tasks.md` | Echoed for mismatch detection (FR-003) |
| `resolutionStatus` | `resolved` \| `unresolved` | `unresolved` ⇒ hard-fail, never a stub pass (FR-002) |
| `mismatch` | Recorded feature id ≠ scanned dir | Surfaced in output (US1 scenario 3) |

**State transitions**: `unresolved` is terminal-fail. There is **no**
`fallback-to-placeholder` transition (the removed `"007-v2-template-packaging"`
branch).

## Audit Status Region (US2)

The single authoritative region from which machine-readable status is read.

| Field | Meaning | Validation |
|---|---|---|
| `fenceInfoString` | The code-fence info string marking the region (`audit-status`) | Only this label is authoritative |
| `ordinal` | Which region in the file (top-to-bottom) | First declared region wins (FR-005 rule 1) |
| `body` | Lines inside the fence | Only these are fed to the key/value parser |

Prose, markdown bullets, and other/unlabeled fenced blocks are **not** regions
and are never read.

## Status Key/Value (US2)

A parsed `key=value` from the authoritative region.

| Field | Meaning | Validation |
|---|---|---|
| `key` | Status key (e.g. `exact-package-match`, `taskbar-only`) | Duplicate within region ⇒ parse error (FR-005 rule 2) |
| `value` | Declared value | Malformed ⇒ surfaced parse error, never silently pass/fail (Edge Case) |
| `authoritative` | Always true (only region values are constructed) | Prose occurrences never become Status Key/Values |

## Blocker Condition (US2)

A condition that hard-blocks the audit, now driven by structured fields.

| Field | Meaning | Validation |
|---|---|---|
| `name` | e.g. `process/taskbar-only`, `unresolved package mismatch` | Stable reason string |
| `trigger` | Structured predicate (e.g. `taskbar-only=true`, `exact-package-match ∉ {true,yes}`) | Evaluated over Status Key/Values only — no raw substring scan |
| `severity` | `block` | Genuine violations still block (FR-006) |

**Removed**: substring triggers (`"taskbar-only" in text`, `"mismatch" in text`,
`"nu1603" in text`) that matched prose/negation.

## Generated FSI Load Script (US4)

| Field | Meaning | Validation |
|---|---|---|
| `path` | Location in generated output (root of the generated product) | Present in generated file list |
| `appAssemblyRef` | `#r` to the generated `Product` output assembly | Matches generated `Product.fsproj` output |
| `transitiveRefs` | `#r` set for `FS.Skia.UI.*` transitive deps | Matches pinned `Directory.Packages.props` / resolved `project.assets.json` |
| `inSync` | Derived from the pinned manifest, not hand-maintained | Regenerated, never edited by author (FR-009) |
| `benignWarningPolicy` | Preserves spec 021 host-warning classification | Real failures stay fatal; only known benign env warnings tolerated |

## Surface-Baseline Delta (US3)

| Field | Meaning | Validation |
|---|---|---|
| `type` | `FS.Skia.UI.Controls.ControlEventOrigin` | Gains qualified-access marker in baseline |
| `nestedTags` | `ControlEventOrigin+Tags` | Remains present |
| `baselineFiles` | `FS.Skia.UI.Controls.txt`, `FS.Skia.UI.txt` | Refreshed via `refresh-surface-baselines.fsx`; validated by `PackageSurfaceCheck` |
| `reversalRecord` | spec 035 decision note | Reversal + rationale recorded (FR-010) |
