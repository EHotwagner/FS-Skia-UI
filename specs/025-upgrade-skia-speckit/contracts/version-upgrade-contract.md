# Contract: Version Upgrade Evidence

## Purpose

Define the required review facts for SkiaSharp and Spec Kit version movement.

## Required Fields

| Field | Required | Meaning |
|-------|----------|---------|
| `dependency-or-asset` | yes | `SkiaSharp package family` or `Spec Kit asset set`. |
| `current-version` | yes | Version currently declared in repository-owned files before implementation. |
| `target-version` | yes | Version selected immediately before implementation. |
| `source-of-truth` | yes | Official package/release source or local governed source file used for selection. |
| `checked-at` | yes | Timestamp/date of the final version check. |
| `affected-files` | yes | Repository-owned files that must change or were intentionally left unchanged. |
| `alignment-rule` | yes | Rule used to keep variants or generated copies consistent. |
| `risk-notes` | yes | Preview, native asset, template, or compatibility risks reviewed. |
| `validation-status` | yes | Pass/fail/unsupported result with command or evidence path. |

## Acceptance Rules

- SkiaSharp managed and native asset packages must share one approved version
  family unless a documented official exception is recorded.
- Spec Kit root assets and generated template copies must either share the
  approved version/range or document a compatibility reason for divergence.
- Version evidence must be real package/release metadata or repository file
  state, not synthetic examples.
- Every changed version must appear in dependency or template alignment
  evidence.
