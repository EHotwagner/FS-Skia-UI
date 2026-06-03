# Contract — After-Baseline Report (FR-003 / FR-004 / FR-005, SC-002 / SC-003)

Defines the schema of `docs/reports/_baselines/2026-06-02-foundations-after.md`, paired
side-by-side with the Stage-0 `docs/reports/_baselines/2026-05-31-foundations.md`.

## Header

- Pinned context block (mirrors Stage 0): `git_commit` (full + short), `branch`, `captured_at`,
  toolchain — the **feature SHA** all after-values are measured at.
- A one-line pointer to the Stage-0 baseline (the comparison oracle) and to the closing ADR 0006.

## Section A — Definition-of-done table (the canonical 100% set)

Exactly **11 rows**, one per "Whole-programme definition of done" dimension. Columns:

| Dimension | Baseline (2026-05-31) | After (this SHA) | Reproduction command | Met-target / rationale |

Rules:

- **SC-002:** all 11 dimensions present; each row's final column is non-empty (a met-target marker
  **or** a written rationale).
- **FR-004 / SC-003:** every non-estimate row's `command`, re-run at the pinned SHA, yields the
  reported `After` value. The reproducibility re-run is captured in
  `readiness/after-baseline-repro.md`.
- **FR-005:** a dimension whose after-value does not reach the plan's literal target carries a
  **written rationale**, never a padded or omitted number. Known rationale rows:
  - **Governance Markdown (rules)** — the plan's original over-estimate (~23,000-line / 21:1) was
    just that; feature 046 established the corrected rule/guidance baseline at **≈6,882 lines**. The
    row states the correction explicitly and measures the after-delta against the corrected figure.
    **Feature 055 restatement (FR-008):** the literal "low hundreds" target is **retired as the live
    target** — it was anchored to the discredited over-estimate. Feature 055 decoupled author-guidance
    prose from the generation-currency anchors (the literal-substring table no longer freezes prose),
    so the corpus *can* now shrink; tracking is against the corrected **≈6,882** baseline and the
    actual large-scale prose reduction is a **bounded follow-up**, not a number chased against a figure
    everyone agrees was wrong.
  - **Framework-author process** — the ~12–14 h/feature figure is an author **estimate** (no timing
    harness; same Stage-0 exemption). The target-met judgement rests on the *mechanism* (the
    `inner-loop` light tier is now the `Route` default), with the hour delta cross-referenced to
    Section B.

## Section B — Supplementary estimates (NOT counted toward the 100% total)

Clearly labelled. Exactly the **three** softer 7.2 metrics absent from the definition-of-done table:

| Metric | Baseline | After | Basis |
|---|---|---|---|
| Per-feature ceremony time | ~12–14 h | (inner-loop estimate) | estimate — no timing harness |
| Agent context bytes | (baseline) | (after) | estimate / measured-where-possible |
| Warm-build time | (baseline) | (after) | estimate / measured-where-possible |

This section is explicitly excluded from SC-002's 100% coverage count (spec Clarification).

## Cross-links (SC-005)

The report links the closing ADR `docs/adr/0006-foundations-programme-closeout.md` and the dogfood
retrospective, so the closeout artifacts form a connected record.
