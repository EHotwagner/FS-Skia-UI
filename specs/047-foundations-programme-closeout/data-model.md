# Data Model — Stage 7 Closeout

This feature produces **documents**, not F# types. The "entities" below are the *shapes* of the
committed artifacts (the fields a reviewer can check), so the tasks have a precise target.

## ScaffoldingProofEntry (US1 — `readiness/scaffolding-proof.md`)

One row per FR-001 pattern.

| Field | Meaning |
|---|---|
| `pattern` | The interim-scaffolding pattern (e.g. root `build.fsx`, `--legacy-evidence`) |
| `proof_kind` | `file-existence` (`git ls-files`) or `scoped-grep` |
| `command` | The exact reproducible command |
| `raw_result` | The unscoped command output (for scoped-grep, the full token matches) |
| `scoped_result` | Zero, after the allowlist exclusion (scoped-grep only) |
| `allowlist_note` | Why each retained match is non-scaffolding (history / enforcement-string / absence-comment / live-FAKE) |
| `verdict` | `clean` (zero / fully-allowlisted) or `residual-removed` (with the corrective edit) |

**Validation:** every entry's `verdict` is `clean` or `residual-removed`; no entry asserts a
non-zero result without an allowlist note (SC-001).

## DefinitionOfDoneRow (US2 — `docs/reports/_baselines/2026-06-02-foundations-after.md`)

Exactly **11 rows** — the canonical "Whole-programme definition of done" set.

| Field | Meaning |
|---|---|
| `dimension` | The plan's dimension name |
| `baseline_value` | The Stage-0 "before" value (from `2026-05-31-foundations.md`) |
| `after_value` | The current value |
| `command` | Reproduction command (omitted only for an explicit estimate) |
| `sha` | The pinned feature SHA the after-value was measured at |
| `status` | `met-target` **or** a `rationale` string (never both empty) |

**Validation:** 11 rows present (SC-002); each has a non-empty `status`; each non-estimate row has a
`command` that reproduces `after_value` at `sha` (SC-003); any unreached target carries a `rationale`
(FR-005) — notably the governance-Markdown corrected-baseline row and the framework-author-process
estimate row.

## EstimateMetric (US2 — supplementary section of the after-baseline)

Exactly the **three** softer 7.2 metrics, clearly labelled, **not** counted in the 100% total.

| Field | Meaning |
|---|---|
| `metric` | `per-feature ceremony time` \| `agent context bytes` \| `warm-build time` |
| `baseline_value` / `after_value` | before/after |
| `basis` | `estimate` (and why it is not command-reproducible — e.g. no timing harness) |

## ClosingAdr (US3 — `docs/adr/0006-foundations-programme-closeout.md`)

Follows the 0001–0005 format: `Status`, `Date`, `Decision source`, `## Context`, `## Decision`
(programme outcome + steady-state model), realized **D1–D6**, `## Alternatives considered`,
`## Consequences / rationale`.
Cross-linked from the after-baseline (SC-005); links to the Stage-0 baseline and the impl plan.

## RetrospectiveEntry (US4 — `readiness/retrospective.md`)

| Field | Meaning |
|---|---|
| `dogfood_feature` | `042` and `043` |
| `pipeline_evidence` | Confirmation each ran the full serialized pipeline green (with pointer to its readiness) |
| `harness_kept_honest` | The retrospective conclusion |

## RecurringRunMechanism (US4 — tracked schedule file + retrospective)

| Field | Meaning |
|---|---|
| `schedule_file_path` | The discoverable tracked path (fixed in `contracts/recurring-run.md`) |
| `schedule_spec` | Names the dogfood set + the full six-target pipeline + cadence |
| `manual_fallback` | The documented serialized-six-target command sequence |
| `no_live_ci` | Asserts no dependency on a live external CI service |

**Validation:** the schedule file is tracked + discoverable, the manual fallback is documented and
runnable, and neither requires live CI (FR-009, SC-005).
